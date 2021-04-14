using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	internal class QueueViewerInputObject : ConfigurableObject
	{
		public QueueViewerInputObject(int bookmarkIndex, IConfigurable bookmarkObject, bool includeBookmark, bool includeComponentLatencyInfo, bool includeDetails, int pageSize, QueryFilter queryFilter, bool searchForward, QueueViewerSortOrderEntry[] sortOrder) : base(new QueueViewerInputPropertyBag())
		{
			this.BookmarkIndex = bookmarkIndex;
			this.BookmarkObject = bookmarkObject;
			this.IncludeBookmark = includeBookmark;
			this.IncludeComponentLatencyInfo = includeComponentLatencyInfo;
			this.IncludeDetails = includeDetails;
			this.PageSize = pageSize;
			this.QueryFilter = queryFilter;
			this.SearchForward = searchForward;
			this.SortOrder = sortOrder;
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return QueueViewerInputObject.schema;
			}
		}

		public QueryFilter QueryFilter
		{
			get
			{
				return (QueryFilter)this.propertyBag[QueueViewerInputObjectSchema.QueryFilter];
			}
			internal set
			{
				this.propertyBag[QueueViewerInputObjectSchema.QueryFilter] = value;
			}
		}

		public QueueViewerSortOrderEntry[] SortOrder
		{
			get
			{
				MultiValuedProperty<QueueViewerSortOrderEntry> multiValuedProperty = (MultiValuedProperty<QueueViewerSortOrderEntry>)this.propertyBag[QueueViewerInputObjectSchema.SortOrder];
				if (multiValuedProperty == null || multiValuedProperty.Count <= 0)
				{
					return null;
				}
				return multiValuedProperty.ToArray();
			}
			internal set
			{
				this.propertyBag[QueueViewerInputObjectSchema.SortOrder] = ((value != null) ? new MultiValuedProperty<QueueViewerSortOrderEntry>(value) : null);
			}
		}

		public bool SearchForward
		{
			get
			{
				return (bool)this.propertyBag[QueueViewerInputObjectSchema.SearchForward];
			}
			internal set
			{
				this.propertyBag[QueueViewerInputObjectSchema.SearchForward] = value;
			}
		}

		public int PageSize
		{
			get
			{
				return (int)this.propertyBag[QueueViewerInputObjectSchema.PageSize];
			}
			internal set
			{
				this.propertyBag[QueueViewerInputObjectSchema.PageSize] = value;
			}
		}

		public IConfigurable BookmarkObject
		{
			get
			{
				return (IConfigurable)this.propertyBag[QueueViewerInputObjectSchema.BookmarkObject];
			}
			internal set
			{
				this.propertyBag[QueueViewerInputObjectSchema.BookmarkObject] = value;
			}
		}

		public int BookmarkIndex
		{
			get
			{
				return (int)this.propertyBag[QueueViewerInputObjectSchema.BookmarkIndex];
			}
			internal set
			{
				this.propertyBag[QueueViewerInputObjectSchema.BookmarkIndex] = value;
			}
		}

		public bool IncludeBookmark
		{
			get
			{
				return (bool)this.propertyBag[QueueViewerInputObjectSchema.IncludeBookmark];
			}
			internal set
			{
				this.propertyBag[QueueViewerInputObjectSchema.IncludeBookmark] = value;
			}
		}

		public bool IncludeDetails
		{
			get
			{
				return (bool)this.propertyBag[QueueViewerInputObjectSchema.IncludeDetails];
			}
			internal set
			{
				this.propertyBag[QueueViewerInputObjectSchema.IncludeDetails] = value;
			}
		}

		public bool IncludeComponentLatencyInfo
		{
			get
			{
				return (bool)this.propertyBag[QueueViewerInputObjectSchema.IncludeComponentLatencyInfo];
			}
			internal set
			{
				this.propertyBag[QueueViewerInputObjectSchema.IncludeComponentLatencyInfo] = value;
			}
		}

		private static QueueViewerInputObjectSchema schema = ObjectSchema.GetInstance<QueueViewerInputObjectSchema>();
	}
}
