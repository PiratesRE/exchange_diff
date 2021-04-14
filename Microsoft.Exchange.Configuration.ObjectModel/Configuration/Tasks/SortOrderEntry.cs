using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class SortOrderEntry
	{
		public SortOrderEntry(string property, ListSortDirection sortDirection)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			this.propertyName = property;
			this.sortDirection = sortDirection;
		}

		public string Property
		{
			get
			{
				return this.propertyName;
			}
		}

		public ListSortDirection SortDirection
		{
			get
			{
				return this.sortDirection;
			}
		}

		public static SortOrderEntry Parse(string s)
		{
			if (s == null || s.Trim().Length < 2)
			{
				throw new SortOrderFormatException(s);
			}
			ListSortDirection listSortDirection;
			switch (s[0])
			{
			case '+':
				listSortDirection = ListSortDirection.Ascending;
				goto IL_46;
			case '-':
				listSortDirection = ListSortDirection.Descending;
				goto IL_46;
			}
			throw new SortOrderFormatException(s);
			IL_46:
			string property = s.Substring(1);
			return new SortOrderEntry(property, listSortDirection);
		}

		public override string ToString()
		{
			string arg = string.Empty;
			switch (this.SortDirection)
			{
			case ListSortDirection.Ascending:
				arg = "+";
				break;
			case ListSortDirection.Descending:
				arg = "-";
				break;
			}
			return string.Format("{0}{1}", arg, this.Property);
		}

		public override bool Equals(object obj)
		{
			SortOrderEntry sortOrderEntry = obj as SortOrderEntry;
			return sortOrderEntry != null && string.Compare(this.Property, sortOrderEntry.Property, StringComparison.Ordinal) == 0 && this.SortDirection == sortOrderEntry.SortDirection;
		}

		public override int GetHashCode()
		{
			return this.Property.GetHashCode() ^ this.SortDirection.GetHashCode();
		}

		private string propertyName;

		private ListSortDirection sortDirection;
	}
}
