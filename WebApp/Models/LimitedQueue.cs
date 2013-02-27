using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
	public class LimitedQueue<T> : List<T>
	{
		private int limit = 20;

		public int Limit
		{
			get { return limit; }
			set { limit = value; }
		}

		public LimitedQueue()
			: base()
		{
			this.Limit = limit;
		}

		public void Enqueue(T item)
		{
			if(this.Count >= this.Limit)
			{
				this.RemoveAt(0);
			}
			base.Add(item);
		}
	}
}