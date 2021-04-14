using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal class ConfigObjectComparer : IComparer
	{
		public ConfigObjectComparer(string property, ListSortDirection direction)
		{
			if (string.IsNullOrEmpty(property))
			{
				throw new ArgumentException("Argument 'property' was null or emtpy.");
			}
			this.propertyToSort = property;
			this.sortDirection = direction;
			this.sorts = null;
		}

		public ConfigObjectComparer(ListSortDescriptionCollection sorts)
		{
			if (sorts == null)
			{
				throw new ArgumentNullException("sorts");
			}
			this.sorts = sorts;
		}

		public ConfigObjectComparer(ListSortDescription sort)
		{
			if (sort == null)
			{
				throw new ArgumentNullException("sort");
			}
			this.sorts = new ListSortDescriptionCollection(new ListSortDescription[]
			{
				sort
			});
		}

		int IComparer.Compare(object x, object y)
		{
			int num = 0;
			if (!typeof(ConfigObject).IsAssignableFrom(x.GetType()) || !typeof(ConfigObject).IsAssignableFrom(x.GetType()))
			{
				throw new ArgumentException();
			}
			if (x != null && y != null)
			{
				if (this.sorts != null)
				{
					using (IEnumerator enumerator = ((IEnumerable)this.sorts).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							ListSortDescription listSortDescription = (ListSortDescription)obj;
							string name = listSortDescription.PropertyDescriptor.Name;
							num = this.CompareObjectProperties(x, y, listSortDescription.PropertyDescriptor.Name, listSortDescription.SortDirection == ListSortDirection.Descending);
							if (num != 0)
							{
								break;
							}
						}
						return num;
					}
				}
				num = this.CompareObjectProperties(x, y, this.propertyToSort, this.sortDirection == ListSortDirection.Descending);
			}
			else if (x == null && y != null)
			{
				num = -1;
			}
			else if (y == null && x != null)
			{
				num = 1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		private int CompareObjectProperties(object x, object y, string property, bool reverseResult)
		{
			object obj = (x as ConfigObject).Fields[property];
			object obj2 = (y as ConfigObject).Fields[property];
			if (!(obj is IComparable) || !(obj2 is IComparable))
			{
				throw new ArgumentException();
			}
			int num = (obj as IComparable).CompareTo(obj2);
			if (reverseResult)
			{
				num = -1 * num;
			}
			return num;
		}

		private ListSortDescriptionCollection sorts;

		private string propertyToSort;

		private ListSortDirection sortDirection;
	}
}
