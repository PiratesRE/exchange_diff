using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class QueueViewerSortOrderEntry
	{
		public QueueViewerSortOrderEntry(string property, ListSortDirection sortDirection)
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

		public static QueueViewerSortOrderEntry Parse(string s)
		{
			if (s == null || s.Trim().Length < 2)
			{
				throw new FormatException(string.Format("The sort order format {0} is invalid. The format is: [+/-]PropertyName, where '+' indicates ascending sort; '-' indicates descending sort, and PropertyName is the name of the property to sort by.", s));
			}
			ListSortDirection listSortDirection;
			switch (s[0])
			{
			case '+':
				listSortDirection = ListSortDirection.Ascending;
				goto IL_5A;
			case '-':
				listSortDirection = ListSortDirection.Descending;
				goto IL_5A;
			}
			throw new FormatException(string.Format("The sort order format {0} is invalid. The format is: [+/-]PropertyName, where '+' indicates ascending sort; '-' indicates descending sort, and PropertyName is the name of the property to sort by.", s));
			IL_5A:
			string property = s.Substring(1);
			return new QueueViewerSortOrderEntry(property, listSortDirection);
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
			QueueViewerSortOrderEntry queueViewerSortOrderEntry = obj as QueueViewerSortOrderEntry;
			return queueViewerSortOrderEntry != null && StringComparer.Ordinal.Compare(this.Property, queueViewerSortOrderEntry.Property) == 0 && this.SortDirection == queueViewerSortOrderEntry.SortDirection;
		}

		public override int GetHashCode()
		{
			return this.Property.GetHashCode() ^ this.SortDirection.GetHashCode();
		}

		private string propertyName;

		private ListSortDirection sortDirection;
	}
}
