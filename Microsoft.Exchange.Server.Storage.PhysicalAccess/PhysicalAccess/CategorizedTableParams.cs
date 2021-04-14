using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class CategorizedTableParams
	{
		public CategorizedTableParams(Table headerTable, Table leafTable, IReadOnlyDictionary<Column, Column> headerRenameDictionary, IReadOnlyDictionary<Column, Column> leafRenameDictionary, IList<object> headerKeyPrefix, IList<object> leafKeyPrefix, SortOrder headerLogicalSortOrder, SortOrder leafLogicalSortOrder, int categoryCount, bool baseMessageViewInReverseOrder, IList<StorePropTag> headerOnlyPropTags, Column depthColumn, Column categIdColumn, Column rowTypeColumn)
		{
			this.headerTable = headerTable;
			this.leafTable = leafTable;
			this.headerRenameDictionary = headerRenameDictionary;
			this.leafRenameDictionary = leafRenameDictionary;
			this.headerKeyPrefix = headerKeyPrefix;
			this.leafKeyPrefix = leafKeyPrefix;
			this.headerLogicalSortOrder = headerLogicalSortOrder;
			this.leafLogicalSortOrder = leafLogicalSortOrder;
			this.categoryCount = categoryCount;
			this.baseMessageViewInReverseOrder = baseMessageViewInReverseOrder;
			this.headerOnlyPropTags = headerOnlyPropTags;
			this.depthColumn = depthColumn;
			this.categIdColumn = categIdColumn;
			this.rowTypeColumn = rowTypeColumn;
		}

		public Table HeaderTable
		{
			get
			{
				return this.headerTable;
			}
		}

		public Table LeafTable
		{
			get
			{
				return this.leafTable;
			}
		}

		public IReadOnlyDictionary<Column, Column> HeaderRenameDictionary
		{
			get
			{
				return this.headerRenameDictionary;
			}
		}

		public IReadOnlyDictionary<Column, Column> LeafRenameDictionary
		{
			get
			{
				return this.leafRenameDictionary;
			}
		}

		public IList<object> HeaderKeyPrefix
		{
			get
			{
				return this.headerKeyPrefix;
			}
		}

		public IList<object> LeafKeyPrefix
		{
			get
			{
				return this.leafKeyPrefix;
			}
		}

		public SortOrder HeaderLogicalSortOrder
		{
			get
			{
				return this.headerLogicalSortOrder;
			}
		}

		public SortOrder LeafLogicalSortOrder
		{
			get
			{
				return this.leafLogicalSortOrder;
			}
		}

		public int CategoryCount
		{
			get
			{
				return this.categoryCount;
			}
		}

		public bool BaseMessageViewInReverseOrder
		{
			get
			{
				return this.baseMessageViewInReverseOrder;
			}
		}

		public IList<StorePropTag> HeaderOnlyPropTags
		{
			get
			{
				return this.headerOnlyPropTags;
			}
		}

		public Column DepthColumn
		{
			get
			{
				return this.depthColumn;
			}
		}

		public Column CategIdColumn
		{
			get
			{
				return this.categIdColumn;
			}
		}

		public Column RowTypeColumn
		{
			get
			{
				return this.rowTypeColumn;
			}
		}

		private readonly Table headerTable;

		private readonly Table leafTable;

		private readonly IReadOnlyDictionary<Column, Column> headerRenameDictionary;

		private readonly IReadOnlyDictionary<Column, Column> leafRenameDictionary;

		private readonly IList<object> headerKeyPrefix;

		private readonly IList<object> leafKeyPrefix;

		private readonly SortOrder headerLogicalSortOrder;

		private readonly SortOrder leafLogicalSortOrder;

		private readonly int categoryCount;

		private readonly bool baseMessageViewInReverseOrder;

		private readonly IList<StorePropTag> headerOnlyPropTags;

		private readonly Column depthColumn;

		private readonly Column categIdColumn;

		private readonly Column rowTypeColumn;
	}
}
