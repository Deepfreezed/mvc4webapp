using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Helpers
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, string selectedValue)
		{
			return items.ToSelectListItems(name, value, selectedValue, false);
		}

		public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, string selectedValue, bool includeNotApplicable, string notApplicableValue = "", string notApplicableText = "(Not Applicable)")
		{
			return items.ToSelectListItems(name, value, x => value(x) == selectedValue, includeNotApplicable, notApplicableValue, notApplicableText);
		}

		public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, Func<T, bool> isSelected)
		{
			return items.ToSelectListItems(name, value, isSelected, false);
		}

		public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, Func<T, bool> isSelected, bool includeNotApplicable, string notApplicableValue = "", string notApplicableText = "(Not Applicable)")
		{
			if(includeNotApplicable)
			{
				var returnItems = new List<SelectListItem>
                          {
                            new SelectListItem
                              {
                                Text = notApplicableText,
                                Value = notApplicableValue,
                                Selected = false
                              }
                          };

				if(items != null)
				{
					returnItems.AddRange(items.Select(item => new SelectListItem
					{
						Text = name(item),
						Value = value(item),
						Selected = isSelected(item)
					}));
				}
				return returnItems;
			}

			if(items == null)
				return new List<SelectListItem>();

			return items.Select(item => new SelectListItem
			{
				Text = name(item),
				Value = value(item),
				Selected = isSelected(item)
			});
		}

		public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, IEnumerable<int> selectedValues)
		{
			if(selectedValues == null)
				selectedValues = new List<int>();
			return items.ToMultiSelectListItems(name, value, selectedValues.Select(x => x.ToString()));
		}

		public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, IEnumerable<long> selectedValues)
		{
			if(selectedValues == null)
				selectedValues = new List<long>();
			return items.ToMultiSelectListItems(name, value, selectedValues.Select(x => x.ToString()));
		}

		public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, IEnumerable<double> selectedValues)
		{
			if(selectedValues == null)
				selectedValues = new List<double>();
			return items.ToMultiSelectListItems(name, value, selectedValues.Select(x => x.ToString()));
		}

		public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, IEnumerable<decimal> selectedValues)
		{
			if(selectedValues == null)
				selectedValues = new List<decimal>();
			return items.ToMultiSelectListItems(name, value, selectedValues.Select(x => x.ToString()));
		}

		public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, IEnumerable<string> selectedValues)
		{
			if(items == null)
				return new List<SelectListItem>();

			if(selectedValues == null)
				selectedValues = new List<string>();

			return items.ToMultiSelectListItems(name, value, x => selectedValues.Contains(value(x)));
		}

		public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, Func<T, bool> isSelected)
		{
			if(items == null)
				return new List<SelectListItem>();

			return items.Select(item => new SelectListItem
			{
				Text = name(item),
				Value = value(item),
				Selected = isSelected(item)
			});
		}
	}
}