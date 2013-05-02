using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Web;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Client.Embedded;
using Raven.Database.Server;
using Raven.Bundles.CascadeDelete;
using Raven.Abstractions.Indexing;
using WebApp.Models.CourseListing;

namespace WebApp.Helpers
{
	public class DocumentStoreHolder
	{
		private static IDocumentStore _store;
		public static IDocumentStore Store
		{
			get
			{
				if (_store == null)
					Initialize();
				return _store;
			}
			set { _store = value; }
		}

		public static void Initialize()
		{
			try
			{				
				Store = new EmbeddableDocumentStore
				{
					DataDirectory = "Data"
					//,UseEmbeddedHttpServer = true
					,Configuration = { Catalog = { Catalogs = { new AssemblyCatalog(typeof(CascadeDeleteTrigger).Assembly) }}}
				};

				//NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);
				
				SetupConventions(Store.Conventions);

				Store.Initialize();

				var types = Assembly.GetCallingAssembly().GetTypes().Where(t => t.GetCustomAttributes(typeof(ExplicitIndexAttribute), true).Length == 0);
				IndexCreation.CreateIndexes(new CompositionContainer(new TypeCatalog(types)), Store);

				ConfigureVersioning();
			}
			catch (Exception e)
			{
				if (RedirectToErrorPageIfRavenDbIsDown(e) == false)
					throw;
			}
		}

		private static void ConfigureVersioning()
		{
			using (var s = _store.OpenSession())
			{
				var item = s.Load<dynamic>("Raven/Versioning/DefaultConfiguration");
				if (item != null) return;

				s.Store(new
				{
					Exclude = true,
					Id = "Raven/Versioning/DefaultConfiguration",
				});
				s.SaveChanges();
			}
		}

		private static void SetupConventions(DocumentConvention conventions)
		{
			//var generator = new MultiTypeHiLoKeyGenerator(Store, 5);
			//Store.Conventions.DocumentKeyGenerator = entity => generator.GenerateDocumentKey(Store.Conventions, entity);
		}

		public static void Shutdown()
		{
			if (_store != null && !_store.WasDisposed)
				_store.Dispose();
		}

		private static bool RedirectToErrorPageIfRavenDbIsDown(Exception e)
		{
			var socketException = e.InnerException as SocketException;
			if (socketException == null)
				return false;

			switch (socketException.SocketErrorCode)
			{
				case SocketError.AddressNotAvailable:
				case SocketError.NetworkDown:
				case SocketError.NetworkUnreachable:
				case SocketError.ConnectionAborted:
				case SocketError.ConnectionReset:
				case SocketError.TimedOut:
				case SocketError.ConnectionRefused:
				case SocketError.HostDown:
				case SocketError.HostUnreachable:
				case SocketError.HostNotFound:
					return true;
				default:
					return false;
			}
		}
	}

	public class AllDocumentsById : AbstractIndexCreationTask
	{
		public override IndexDefinition CreateIndexDefinition()
		{
			return new IndexDefinition
			{
				Name = "AllDocumentsById",
				Map = "from doc in docs let DocId = doc[\"@metadata\"][\"@id\"] select new {DocId};"
			};
		}
	}

	public class DepartmentIndex : AbstractIndexCreationTask<Department>
	{
		public DepartmentIndex()
		{
			Map = departments => from department in departments select new { department.SemesterID, department.DepartmentID };
		}
	}

	public class CourseIndex : AbstractIndexCreationTask<Course>
	{
		public CourseIndex()
		{
			Map = courses => from course in courses select new { course.SemesterID, course.DepartmentID, course.CourseNumber };
		}
	}
}