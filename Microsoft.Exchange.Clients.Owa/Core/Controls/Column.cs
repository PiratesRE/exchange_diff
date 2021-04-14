using System;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public sealed class Column
	{
		public Column(ColumnId id, ColumnBehavior behavior, bool groupable, ColumnHeader header, SortBoundaries sortBoundaries, params PropertyDefinition[] properties)
		{
			if (behavior == null)
			{
				behavior = Column.defaultBehavior;
			}
			if (groupable)
			{
				this.groupType = behavior.GroupType;
			}
			else
			{
				this.groupType = GroupType.None;
			}
			if (this.GroupType != GroupType.None && sortBoundaries == null)
			{
				throw new ArgumentException("sortBoundaries may not be null if groupType does not equal GroupType.None");
			}
			if (properties == null || properties.Length <= 0)
			{
				throw new ArgumentException("properties may not be null or an empty array");
			}
			this.id = id;
			this.behavior = behavior;
			this.properties = properties;
			this.header = header;
			this.sortBoundaries = sortBoundaries;
			if (1 <= properties.Length)
			{
				this.isTypeDownCapable = (properties[0].Type.Equals(typeof(string)) || properties[0].Type.Equals(typeof(string[])));
			}
		}

		public Column(ColumnId id, ColumnBehavior behavior, bool groupable, ColumnHeader header, params PropertyDefinition[] properties) : this(id, behavior, groupable, header, null, properties)
		{
		}

		public Column(ColumnId id, params PropertyDefinition[] properties) : this(id, null, false, null, null, properties)
		{
		}

		public Column(ColumnId id, ColumnBehavior behavior, bool groupable, ColumnHeader header, SortBoundaries sortBoundaries, bool isTypeDownCapable, params PropertyDefinition[] properties) : this(id, behavior, groupable, header, sortBoundaries, properties)
		{
			this.isTypeDownCapable = isTypeDownCapable;
		}

		public PropertyDefinition this[int index]
		{
			get
			{
				return this.properties[index];
			}
		}

		public ColumnId Id
		{
			get
			{
				return this.id;
			}
		}

		public int PropertyCount
		{
			get
			{
				return this.properties.Length;
			}
		}

		public ColumnHeader Header
		{
			get
			{
				if (this.header == null)
				{
					throw new OwaInvalidOperationException("Header not available for this column.");
				}
				return this.header;
			}
		}

		public SortBoundaries SortBoundaries
		{
			get
			{
				if (this.sortBoundaries == null)
				{
					throw new OwaInvalidOperationException("Sort boundaries not available for this column.");
				}
				return this.sortBoundaries;
			}
		}

		public SortOrder DefaultSortOrder
		{
			get
			{
				return this.behavior.DefaultSortOrder;
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
				return this.behavior.HorizontalAlign;
			}
		}

		public int Width
		{
			get
			{
				return this.behavior.Width;
			}
		}

		public bool IsFixedWidth
		{
			get
			{
				return this.behavior.IsFixedWidth;
			}
		}

		public bool IsTypeDownCapable
		{
			get
			{
				return this.isTypeDownCapable;
			}
		}

		public bool IsSortable
		{
			get
			{
				return this.behavior.IsSortable;
			}
		}

		private static readonly ColumnBehavior defaultBehavior = new ColumnBehavior(10, true, SortOrder.Ascending, GroupType.None);

		private ColumnId id;

		private ColumnBehavior behavior;

		private ColumnHeader header;

		private SortBoundaries sortBoundaries;

		private PropertyDefinition[] properties;

		private GroupType groupType;

		private bool isTypeDownCapable;
	}
}
