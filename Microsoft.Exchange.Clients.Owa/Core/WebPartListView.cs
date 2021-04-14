using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class WebPartListView
	{
		public WebPartListView()
		{
		}

		public WebPartListView(int? columnId, int? sortOrder, bool? isMultiline, bool isPremiumOnly)
		{
			this.columnId = columnId;
			this.sortOrder = sortOrder;
			this.isMultiLine = isMultiline;
			this.isPremiumOnly = isPremiumOnly;
		}

		public int? ColumnId
		{
			get
			{
				return this.columnId;
			}
			set
			{
				this.columnId = value;
			}
		}

		public int? SortOrder
		{
			get
			{
				return this.sortOrder;
			}
			set
			{
				this.sortOrder = value;
			}
		}

		public bool? IsMultiLine
		{
			get
			{
				return this.isMultiLine;
			}
			set
			{
				this.isMultiLine = value;
			}
		}

		public bool IsPremiumOnly
		{
			get
			{
				return this.isPremiumOnly;
			}
			set
			{
				this.isPremiumOnly = value;
			}
		}

		private int? columnId;

		private int? sortOrder;

		private bool? isMultiLine;

		private bool isPremiumOnly;
	}
}
