using System;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public sealed class ColumnBehavior
	{
		public ColumnBehavior(HorizontalAlign horizontalAlign, int width, bool isFixedWidth, SortOrder defaultSortOrder, GroupType groupType)
		{
			this.horizontalAlign = horizontalAlign;
			this.width = width;
			this.isFixedWidth = isFixedWidth;
			this.defaultSortOrder = defaultSortOrder;
			this.groupType = groupType;
		}

		public ColumnBehavior(HorizontalAlign horizontalAlign, int width, bool isFixedWidth, SortOrder defaultSortOrder) : this(horizontalAlign, width, isFixedWidth, defaultSortOrder, GroupType.None)
		{
		}

		public ColumnBehavior(int width, bool isFixedWidth, SortOrder defaultSortOrder, GroupType groupType) : this(HorizontalAlign.NotSet, width, isFixedWidth, defaultSortOrder, groupType)
		{
		}

		public ColumnBehavior(int width, bool isFixedWidth, SortOrder defaultSortOrder) : this(HorizontalAlign.NotSet, width, isFixedWidth, defaultSortOrder, GroupType.None)
		{
		}

		public ColumnBehavior(HorizontalAlign horizontalAlign, int width, bool isFixedWidth)
		{
			this.horizontalAlign = horizontalAlign;
			this.width = width;
			this.isFixedWidth = isFixedWidth;
			this.isSortable = false;
		}

		public SortOrder DefaultSortOrder
		{
			get
			{
				return this.defaultSortOrder;
			}
		}

		public GroupType GroupType
		{
			get
			{
				return this.groupType;
			}
		}

		public HorizontalAlign HorizontalAlign
		{
			get
			{
				return this.horizontalAlign;
			}
		}

		public int Width
		{
			get
			{
				return this.width;
			}
		}

		public bool IsFixedWidth
		{
			get
			{
				return this.isFixedWidth;
			}
		}

		public bool IsSortable
		{
			get
			{
				return this.isSortable;
			}
		}

		private HorizontalAlign horizontalAlign;

		private int width;

		private bool isFixedWidth;

		private GroupType groupType;

		private SortOrder defaultSortOrder;

		private bool isSortable = true;
	}
}
