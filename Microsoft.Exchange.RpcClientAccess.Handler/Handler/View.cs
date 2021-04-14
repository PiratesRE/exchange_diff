using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class View : ServerObject
	{
		protected View(Logon logon, TableFlags tableFlags, View.Capabilities capabilities, ViewType viewType, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandle, QueryFilter defaultQueryFilter = null) : base(logon)
		{
			this.tableFlags = tableFlags;
			this.capabilities = capabilities;
			this.viewType = viewType;
			this.notificationHandler = notificationHandler;
			this.returnNotificationHandle = returnNotificationHandle;
			this.defaultQueryFilter = defaultQueryFilter;
			this.filter = defaultQueryFilter;
		}

		internal virtual byte[] CreateBookmark()
		{
			throw new RopExecutionException("View not supported", (ErrorCode)2147746050U);
		}

		internal virtual void FreeBookmark(byte[] bookmark)
		{
			throw new RopExecutionException("View not supported", (ErrorCode)2147746050U);
		}

		internal virtual int SeekRowBookmark(byte[] bookmark, int rowCount, bool wantMoveCount, out bool soughtLess, out bool positionChanged)
		{
			throw new RopExecutionException("View not supported", (ErrorCode)2147746050U);
		}

		internal PropertyTag[] Columns
		{
			get
			{
				base.CheckDisposed();
				return this.originalColumns;
			}
			private set
			{
				base.CheckDisposed();
				this.originalColumns = value;
				this.serverColumns = this.PropertyConverter.ConvertPropertyTagsFromClient(this.originalColumns);
				if (value == null)
				{
					this.instanceKeyColumnIndex = -1;
					Util.DisposeIfPresent(this.viewNotificationSink);
					this.viewNotificationSink = null;
					return;
				}
				this.instanceKeyColumnIndex = Array.IndexOf<PropertyTag>(this.serverColumns, PropertyTag.InstanceKey);
				if (this.instanceKeyColumnIndex < 0)
				{
					this.serverColumns = View.AddColumn(PropertyTag.InstanceKey, this.serverColumns);
					this.instanceKeyColumnIndex = 0;
				}
				if (this.viewCache != null)
				{
					this.viewCache.SetColumns(this.ColumnPropertyDefinitions, this.ServerColumns);
				}
				this.RegisterNotification();
			}
		}

		internal PropertyTag[] ServerColumns
		{
			get
			{
				base.CheckDisposed();
				return this.serverColumns;
			}
		}

		internal IViewDataSource DataSource
		{
			get
			{
				if (this.viewCache != null)
				{
					return this.viewCache.DataSource;
				}
				return null;
			}
		}

		internal QueryFilter Filter
		{
			get
			{
				base.CheckDisposed();
				return this.filter;
			}
		}

		protected abstract PropertyConverter PropertyConverter { get; }

		internal RopResult ProtectAgainstNoColumnsSet(Func<View, RopResult> protectedMethod)
		{
			bool flag = this.Columns == null;
			RopResult result;
			try
			{
				if (flag)
				{
					this.Columns = Array<PropertyTag>.Empty;
					this.Validate();
				}
				result = protectedMethod(this);
			}
			finally
			{
				if (flag)
				{
					this.Columns = null;
				}
			}
			return result;
		}

		internal bool TryConvertOriginalRowFromServerRow(PropertyValue[] serverRow, out PropertyValue[] originalRow)
		{
			if (serverRow == null || this.serverColumns == null || serverRow.Length != this.serverColumns.Length)
			{
				originalRow = null;
				return false;
			}
			if (this.originalColumns.Length != this.serverColumns.Length)
			{
				if (this.serverColumns[0] != PropertyTag.InstanceKey)
				{
					throw new ArgumentException(string.Format("We should have added instance key at index = 0. Columns[0] = {0},", this.serverColumns[0]));
				}
				originalRow = new PropertyValue[serverRow.Length - 1];
				Array.Copy(serverRow, 1, originalRow, 0, originalRow.Length);
			}
			else
			{
				originalRow = (PropertyValue[])serverRow.Clone();
			}
			int num = 0;
			this.PropertyConverter.ConvertPropertyValuesToClientAndSuppressClientSide(base.LogonObject.Session, this.StorageObjectProperties, originalRow, this.originalColumns, this.ClientSideProperties);
			for (int i = 0; i < originalRow.Length; i++)
			{
				PropertyValue propertyValue = originalRow[i];
				if (propertyValue.PropertyTag.PropertyId != this.originalColumns[num].PropertyId)
				{
					return false;
				}
				num++;
			}
			return true;
		}

		internal bool IsAvailable
		{
			get
			{
				return !base.IsDisposed;
			}
		}

		internal bool IsRowWithinUnreadCache(byte[] instanceKey, View.RowLookupPosition rowLookupPosition)
		{
			return !base.IsDisposed && this.viewCache != null && this.viewCache.IsRowWithinUnreadCache(this.instanceKeyColumnIndex, instanceKey, rowLookupPosition);
		}

		internal SortBy[] SortBys
		{
			get
			{
				base.CheckDisposed();
				return this.sortBys;
			}
		}

		internal GroupByAndOrder[] GroupBys
		{
			get
			{
				base.CheckDisposed();
				return this.groupBys;
			}
		}

		internal int ExpandedCount
		{
			get
			{
				base.CheckDisposed();
				return this.expandedCount;
			}
		}

		internal void SetColumns(SetColumnsFlags flags, PropertyTag[] propertyTags)
		{
			base.CheckDisposed();
			this.Columns = null;
			RopHandler.CheckEnum<SetColumnsFlags>(flags);
			Util.ThrowOnNullArgument(propertyTags, "propertyTags");
			if (propertyTags.Length == 0)
			{
				throw new RopExecutionException("Empty PropertyTag[] is invalid.", (ErrorCode)2147942487U);
			}
			foreach (PropertyTag propertyTag in propertyTags)
			{
				if (!PropertyValue.IsSupportedPropertyType(propertyTag) || propertyTag.PropertyType == PropertyType.Unspecified)
				{
					throw new RopExecutionException(string.Format("Invalid PropertyTag used in columns: {0}", propertyTag), (ErrorCode)2147942487U);
				}
			}
			this.CheckPropertiesAllowed(propertyTags);
			this.Columns = propertyTags;
		}

		protected abstract void CheckPropertiesAllowed(PropertyTag[] propertyTags);

		protected NativeStorePropertyDefinition[] GetColumnPropertyDefinitions(StoreSession session, ICorePropertyBag propertyBag)
		{
			NativeStorePropertyDefinition[] array;
			if (!MEDSPropertyTranslator.TryGetPropertyDefinitionsFromPropertyTags(session, propertyBag, this.ServerColumns, out array) && !View.TryFixUnresolvedPropertyDefinitions(array, this.serverColumns))
			{
				Util.DisposeIfPresent(this.viewCache);
				this.viewCache = null;
				throw new RopExecutionException("Properties for some columns could not be resolved", (ErrorCode)2147746050U);
			}
			return array;
		}

		protected abstract NativeStorePropertyDefinition[] ColumnPropertyDefinitions { get; }

		internal int GetPosition()
		{
			base.CheckDisposed();
			this.ValidateCursorPosition();
			this.Validate();
			return this.GetViewCache().GetPosition();
		}

		internal int GetRowCount()
		{
			base.CheckDisposed();
			return this.GetViewCache().GetRowCount();
		}

		internal void Reset()
		{
			base.CheckDisposed();
			if ((this.capabilities & View.Capabilities.CanReset) != View.Capabilities.CanReset)
			{
				throw new RopExecutionException(string.Format("Reset not supported for view of type {0}.", this.ViewType), (ErrorCode)2147746050U);
			}
			this.originalColumns = null;
			this.serverColumns = null;
			this.sortBys = null;
			this.groupBys = null;
			this.expandedCount = 0;
			this.filter = this.defaultQueryFilter;
			this.hasRestrictFailed = false;
			this.hasSortTableFailed = false;
			this.isCursorPositionUndefined = false;
			this.ClearViewCache();
		}

		internal bool CollectRows(RowCollector rowCollector, QueryRowsFlags flags, bool useForwardDirection, ushort rowCount)
		{
			base.CheckDisposed();
			if (!useForwardDirection && (byte)(flags & QueryRowsFlags.DoNotAdvance) != 0)
			{
				throw Feature.NotImplemented(220256, "DoNotAdvance is not supported on query backward.");
			}
			if (rowCount <= 0)
			{
				throw Feature.NotImplemented(25408, "Consider supporting QueryRows(rowCount = 0)");
			}
			this.ValidateCursorPosition();
			this.Validate();
			ViewCache cache = this.GetViewCache();
			this.UpdateRowCollectorColumns(rowCollector);
			this.UseForwardDirection = useForwardDirection;
			if ((byte)(flags & QueryRowsFlags.DoNotAdvance) == 1)
			{
				return this.CollectRowsNoAdvance(cache, rowCollector, (uint)rowCount);
			}
			return this.CollectRowsAdvance(cache, rowCollector, rowCount);
		}

		internal void Sort(SortTableFlags flags, ushort categoryCount, ushort expandedCount, SortOrder[] sortOrders)
		{
			base.CheckDisposed();
			this.ClearViewCache();
			this.expandedCount = 0;
			this.hasSortTableFailed = true;
			if ((this.capabilities & View.Capabilities.CanSort) != View.Capabilities.CanSort)
			{
				throw new RopExecutionException(string.Format("Sort not supported for view of type {0}.", this.ViewType), (ErrorCode)2147746050U);
			}
			RopHandler.CheckEnum<SortTableFlags>(flags);
			if (sortOrders.Length > 6)
			{
				throw new RopExecutionException(string.Format("View does not support more than {0} sort orders.", 6), (ErrorCode)2147746071U);
			}
			if (categoryCount != 0 && categoryCount < expandedCount)
			{
				throw new RopExecutionException("Number of categories cannot be less than expandedCount.", (ErrorCode)2147942487U);
			}
			foreach (SortOrder sortOrder in sortOrders)
			{
				if (!PropertyValue.IsSupportedPropertyType(sortOrder.Tag) || sortOrder.Tag.PropertyType == PropertyType.Unspecified || sortOrder.Tag.PropertyType == PropertyType.Null || sortOrder.Tag.PropertyType == PropertyType.Error)
				{
					throw new RopExecutionException(string.Format("Invalid PropertyTag used in sort order: {0}", sortOrder.Tag), (ErrorCode)2147942487U);
				}
			}
			this.UpdateSortParameters(sortOrders, (int)categoryCount);
			this.expandedCount = (int)expandedCount;
			this.hasSortTableFailed = false;
			this.isCursorPositionUndefined = false;
		}

		internal void Restrict(RestrictFlags flags, Restriction restriction)
		{
			base.CheckDisposed();
			this.ClearViewCache();
			this.filter = this.defaultQueryFilter;
			this.hasRestrictFailed = true;
			if ((this.capabilities & View.Capabilities.CanRestrict) != View.Capabilities.CanRestrict)
			{
				throw new RopExecutionException(string.Format("Restrict not supported for view of type {0}.", this.ViewType), (ErrorCode)2147746050U);
			}
			RopHandler.CheckEnum<RestrictFlags>(flags);
			RestrictionHelper.ConvertRestrictionFromClient(this.Session, ref restriction, this.ViewType);
			if (restriction != null)
			{
				FilterRestrictionTranslator filterRestrictionTranslator = new FilterRestrictionTranslator(this.Session);
				QueryFilter queryFilter = filterRestrictionTranslator.Translate(restriction);
				if (this.defaultQueryFilter != null)
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						this.defaultQueryFilter,
						queryFilter
					});
				}
				this.filter = queryFilter;
			}
			this.hasRestrictFailed = false;
			this.isCursorPositionUndefined = false;
		}

		internal int SeekRow(BookmarkOrigin bookmarkOrigin, int rowCount)
		{
			base.CheckDisposed();
			if (bookmarkOrigin == BookmarkOrigin.Current)
			{
				this.ValidateCursorPosition();
			}
			this.isCursorPositionUndefined = true;
			if (bookmarkOrigin > BookmarkOrigin.End)
			{
				throw new RopExecutionException("User-defined bookmarks are not supported by SeekRow", (ErrorCode)2147942487U);
			}
			this.Validate();
			int result = this.GetViewCache().SeekRow(bookmarkOrigin, rowCount);
			this.isCursorPositionUndefined = false;
			return result;
		}

		internal void SeekRowApproximate(uint numerator, uint denominator)
		{
			base.CheckDisposed();
			if (denominator == 0U)
			{
				throw new RopExecutionException("SeekRowApproximate does not accept denominator = 0", (ErrorCode)2147942487U);
			}
			if (numerator > denominator)
			{
				numerator = denominator;
			}
			int rowCount = this.GetRowCount();
			ulong num = (ulong)((long)rowCount * (long)((ulong)numerator) / (long)((ulong)denominator));
			this.SeekRow(BookmarkOrigin.Beginning, (int)num);
		}

		internal virtual bool TryFindRow(FindRowFlags flags, Restriction restriction, BookmarkOrigin bookmarkOrigin, byte[] bookmark, RowCollector rowCollector)
		{
			base.CheckDisposed();
			if (bookmarkOrigin == BookmarkOrigin.Current)
			{
				this.ValidateCursorPosition();
			}
			this.isCursorPositionUndefined = true;
			if ((this.capabilities & View.Capabilities.CanRestrict) != View.Capabilities.CanRestrict)
			{
				throw new RopExecutionException(string.Format("Restrict not supported for views of type {0}.", this.ViewType), (ErrorCode)2147746050U);
			}
			if (restriction == null)
			{
				throw new RopExecutionException("Invalid Restriction.", (ErrorCode)2147942487U);
			}
			if (bookmarkOrigin > BookmarkOrigin.Custom)
			{
				throw new RopExecutionException("Invalid BookmarkOrigin.", (ErrorCode)2147942487U);
			}
			if (bookmarkOrigin != BookmarkOrigin.Custom != (bookmark == null || bookmark.Length == 0))
			{
				string message = string.Format("BookmarkOrigin indicated that a custom bookmark is {0}expected, but then a custom bookmark was {1}provided", (bookmarkOrigin != BookmarkOrigin.Custom) ? "not " : " ", (bookmark == null || bookmark.Length == 0) ? "not " : " ");
				throw new RopExecutionException(message, (ErrorCode)2147942487U);
			}
			RestrictionHelper.ConvertRestrictionFromClient(this.Session, ref restriction, this.ViewType);
			this.Validate();
			ViewCache viewCache = this.GetViewCache();
			this.UpdateRowCollectorColumns(rowCollector);
			PropertyValue[] serverRow;
			bool flag;
			if (bookmark != null)
			{
				flag = viewCache.FindRow(restriction, bookmark, (byte)(flags & FindRowFlags.Backward) == 0, out serverRow);
			}
			else
			{
				flag = viewCache.FindRow(restriction, (uint)bookmarkOrigin, (byte)(flags & FindRowFlags.Backward) == 0, out serverRow);
			}
			PropertyValue[] rowValues;
			if (flag && this.TryConvertOriginalRowFromServerRow(serverRow, out rowValues))
			{
				rowCollector.TryAddRow(rowValues);
			}
			this.isCursorPositionUndefined = false;
			return flag;
		}

		internal int ExpandRow(short maxRows, StoreId categoryId, RowCollector rowCollector)
		{
			base.CheckDisposed();
			if ((this.capabilities & View.Capabilities.CanGroup) != View.Capabilities.CanGroup)
			{
				throw new RopExecutionException(string.Format("ExpandRow not supported for view of type {0}.", this.ViewType), (ErrorCode)2147746050U);
			}
			ViewCache viewCache = this.GetViewCache();
			this.UpdateRowCollectorColumns(rowCollector);
			int result = 0;
			PropertyValue[][] array = viewCache.ExpandRow((int)maxRows, categoryId, out result);
			ushort num = 0;
			PropertyValue[] rowValues;
			while ((int)num < array.Length && this.TryConvertOriginalRowFromServerRow(array[(int)num], out rowValues) && rowCollector.TryAddRow(rowValues))
			{
				num += 1;
			}
			return result;
		}

		internal int CollapseRow(StoreId categoryId)
		{
			base.CheckDisposed();
			if ((this.capabilities & View.Capabilities.CanGroup) != View.Capabilities.CanGroup)
			{
				throw new RopExecutionException(string.Format("CollapseRow not supported for view of type {0}.", this.ViewType), (ErrorCode)2147746050U);
			}
			ViewCache viewCache = this.GetViewCache();
			return viewCache.CollapseRow(categoryId);
		}

		internal PropertyTag[] QueryColumnsAll()
		{
			base.CheckDisposed();
			ViewCache viewCache = this.GetViewCache();
			PropertyTag[] source = viewCache.QueryColumnsAll();
			return (from propertyTag in source
			where this.ClientSideProperties.ShouldBeReturnedIfRequested(propertyTag.PropertyId)
			select this.PropertyConverter.ConvertPropertyTagToClient(propertyTag)).ToArray<PropertyTag>();
		}

		public StoreSession Session
		{
			get
			{
				base.CheckDisposed();
				return this.CoreObject.Session;
			}
		}

		protected abstract StoreId? ContainerFolderId { get; }

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<View>(this);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.viewNotificationSink);
			this.viewNotificationSink = null;
			Util.DisposeIfPresent(this.viewCache);
			base.InternalDispose();
		}

		protected abstract ICoreObject CoreObject { get; }

		protected abstract IViewDataSource CreateDataSource();

		protected TableFlags TableFlags
		{
			get
			{
				base.CheckDisposed();
				return this.tableFlags;
			}
		}

		protected ViewCache GetViewCache()
		{
			base.CheckDisposed();
			if (this.viewCache == null)
			{
				this.Validate();
				this.viewCache = new ViewCache(this.CreateDataSource());
				this.RegisterNotification();
			}
			return this.viewCache;
		}

		protected int InternalSeekRowBookmark(byte[] bookmark, int rowCount, bool wantMoveCount, out bool soughtLess, out bool positionChanged)
		{
			base.CheckDisposed();
			this.isCursorPositionUndefined = true;
			this.Validate();
			int result = this.GetViewCache().SeekRowBookmark(bookmark, rowCount, wantMoveCount, out soughtLess, out positionChanged);
			this.isCursorPositionUndefined = false;
			return result;
		}

		protected virtual ClientSideProperties ClientSideProperties
		{
			get
			{
				return ClientSideProperties.EmptyInstance;
			}
		}

		private IStorageObjectProperties StorageObjectProperties
		{
			get
			{
				return new CoreObjectProperties(this.CoreObject.PropertyBag);
			}
		}

		private static bool ContainsPropertyId(PropertyTag[] tags, PropertyId propertyId)
		{
			foreach (PropertyTag propertyTag in tags)
			{
				if (propertyTag.PropertyId == propertyId)
				{
					return true;
				}
			}
			return false;
		}

		private static PropertyTag[] AddColumn(PropertyTag newColumn, PropertyTag[] columns)
		{
			PropertyTag[] array = new PropertyTag[columns.Length + 1];
			array[0] = newColumn;
			Array.Copy(columns, 0, array, 1, columns.Length);
			return array;
		}

		private static bool TryFixUnresolvedPropertyDefinitions(NativeStorePropertyDefinition[] propertyDefinitions, PropertyTag[] columns)
		{
			if (columns == null || propertyDefinitions == null || propertyDefinitions.Length != columns.Length)
			{
				return false;
			}
			for (int i = 0; i < propertyDefinitions.Length; i++)
			{
				if (propertyDefinitions[i] == null)
				{
					if (!columns[i].IsNamedProperty)
					{
						return false;
					}
					propertyDefinitions[i] = GuidIdPropertyDefinition.CreateCustom(columns[i].PropertyId.ToString(), (ushort)columns[i].PropertyType, View.mapiNamespace, (int)columns[i].PropertyId, PropertyFlags.None);
				}
			}
			return true;
		}

		private PropertyValue[] ConvertOriginalRowFromServerRow(PropertyValue[] serverRow)
		{
			PropertyValue[] result;
			if (this.TryConvertOriginalRowFromServerRow(serverRow, out result))
			{
				return result;
			}
			throw new InvalidOperationException("We failed to convert the rows, because the columns are not properly set.");
		}

		private bool CollectRowsAdvance(ViewCache cache, RowCollector rowCollector, ushort rowCount)
		{
			int num = (int)(this.UseForwardDirection ? rowCount : (-(int)rowCount));
			for (ushort num2 = 0; num2 < rowCount; num2 += 1)
			{
				PropertyValue[] rowValues;
				if (!this.TryGetCurrentRow(cache, num, out rowValues))
				{
					return false;
				}
				if (!rowCollector.TryAddRow(rowValues))
				{
					break;
				}
				num -= cache.MoveNext(this.UseForwardDirection ? 1 : -1);
			}
			return true;
		}

		private bool CollectRowsNoAdvance(ViewCache cache, RowCollector rowCollector, uint rowCount)
		{
			PropertyValue[][] array = cache.FetchNoAdvance(rowCount);
			foreach (PropertyValue[] serverRow in array)
			{
				PropertyValue[] rowValues = this.ConvertOriginalRowFromServerRow(serverRow);
				if (!rowCollector.TryAddRow(rowValues))
				{
					break;
				}
			}
			return array.Length > 0;
		}

		private void RegisterNotification()
		{
			if (this.viewCache != null && this.viewNotificationSink == null && this.originalColumns != null && this.originalColumns.Length > 0)
			{
				this.viewNotificationSink = base.LogonObject.NotificationQueue.Register(this.notificationHandler, this, this.tableFlags & ~NotificationSink.NonNotificationTableFlags, this.ContainerFolderId, this.returnNotificationHandle, this.String8Encoding);
			}
		}

		private void UpdateRowCollectorColumns(RowCollector rowCollector)
		{
			foreach (PropertyTag propertyTag in this.originalColumns)
			{
				if (propertyTag.IsMultiValueInstanceProperty && (this.sortPropertyTags == null || !View.ContainsPropertyId(this.sortPropertyTags, propertyTag.PropertyId)))
				{
					throw new RopExecutionException("MVI flag on the property that is not part of the sort order is not supported.", (ErrorCode)2147746050U);
				}
			}
			if (this.sortPropertyTags != null)
			{
				foreach (PropertyTag propertyTag2 in this.sortPropertyTags)
				{
					if (propertyTag2.IsMultiValuedProperty)
					{
						foreach (PropertyTag propertyTag3 in this.originalColumns)
						{
							if (propertyTag2.PropertyId == propertyTag3.PropertyId && !propertyTag3.IsMultiValueInstanceProperty)
							{
								throw new RopExecutionException("Sorting by MV values is not supported if the corresponding SetColumns flag is not MVI.", (ErrorCode)2147746050U);
							}
						}
					}
				}
			}
			rowCollector.SetColumns(this.originalColumns);
		}

		private void Validate()
		{
			if (this.originalColumns == null || this.hasSortTableFailed || this.hasRestrictFailed)
			{
				throw new RopExecutionException(string.Format("The current view is not valid. Columns not defined = {0}. Failed sort = {1}. Failed restrict = {2}.", this.originalColumns == null, this.hasSortTableFailed, this.hasRestrictFailed), ErrorCode.NullObject);
			}
		}

		private void ValidateCursorPosition()
		{
			if (this.isCursorPositionUndefined)
			{
				throw new RopExecutionException("Current cursor position is unknown.", ErrorCode.NullObject);
			}
		}

		private void ClearViewCache()
		{
			base.CheckDisposed();
			if (this.viewNotificationSink != null)
			{
				this.viewNotificationSink.Dispose();
				this.viewNotificationSink = null;
			}
			if (this.viewCache != null)
			{
				this.viewCache.Dispose();
				this.viewCache = null;
			}
		}

		private void UpdateSortParameters(SortOrder[] sortOrders, int categoryCount)
		{
			this.sortBys = null;
			this.groupBys = null;
			this.sortPropertyTags = null;
			if (sortOrders.Length == 0)
			{
				return;
			}
			PropertyTag[] array = new PropertyTag[sortOrders.Length];
			for (int i = 0; i < sortOrders.Length; i++)
			{
				if (sortOrders[i].Tag.IsMultiValuedProperty && !sortOrders[i].Tag.IsMultiValueInstanceProperty)
				{
					throw new RopExecutionException("Multi value columns sort is not support without MVI flag set.", (ErrorCode)2147746071U);
				}
				array[i] = sortOrders[i].Tag;
			}
			NativeStorePropertyDefinition[] array2;
			if (!MEDSPropertyTranslator.TryGetPropertyDefinitionsFromPropertyTags(this.Session, this.CoreObject.PropertyBag, array, out array2))
			{
				throw new RopExecutionException("Properties in some SortOrders could not be resolved", (ErrorCode)2147746050U);
			}
			int num = 0;
			int num2 = 0;
			List<SortBy> list = new List<SortBy>();
			List<GroupByAndOrder> list2 = new List<GroupByAndOrder>();
			for (int j = 0; j < sortOrders.Length; j++)
			{
				SortOrder sortOrder;
				switch (sortOrders[j].Flags)
				{
				case SortOrderFlags.Ascending:
					sortOrder = SortOrder.Ascending;
					break;
				case SortOrderFlags.Descending:
					sortOrder = SortOrder.Descending;
					break;
				default:
					throw new RopExecutionException("Out of order aggregation sort entry.", (ErrorCode)2147746071U);
				}
				if (num < categoryCount)
				{
					NativeStorePropertyDefinition nativeStorePropertyDefinition = array2[j];
					NativeStorePropertyDefinition columnDefinition = nativeStorePropertyDefinition;
					Aggregate aggregate = Aggregate.Min;
					if (j + 1 < sortOrders.Length && (sortOrders[j + 1].Flags == SortOrderFlags.CategoryMaximum || sortOrders[j + 1].Flags == SortOrderFlags.CategoryMinimum))
					{
						columnDefinition = array2[j + 1];
						aggregate = ((sortOrders[j + 1].Flags == SortOrderFlags.CategoryMaximum) ? Aggregate.Max : Aggregate.Min);
						j++;
					}
					list2.Add(new GroupByAndOrder(nativeStorePropertyDefinition, new GroupSort(columnDefinition, sortOrder, aggregate)));
					num++;
					num2 = j;
				}
				else
				{
					list.Add(new SortBy(array2[j], sortOrder));
				}
			}
			if (num != categoryCount)
			{
				throw new RopExecutionException("Number of categories doesn't match the sortOrders array length.", (ErrorCode)2147746071U);
			}
			if (num2 >= 4)
			{
				throw new RopExecutionException(string.Format("View doesn't support more than {0} categories and aggregations.", 4), (ErrorCode)2147746071U);
			}
			if (list.Count != 0)
			{
				this.sortBys = list.ToArray();
			}
			if (list2.Count != 0)
			{
				this.groupBys = list2.ToArray();
			}
			this.sortPropertyTags = array;
		}

		private bool TryGetCurrentRow(ViewCache cache, int fetchRowCount, out PropertyValue[] row)
		{
			PropertyValue[] serverRow;
			if (cache != null && cache.TryGetCurrentRow(this.UseForwardDirection ? 0 : -1, fetchRowCount, out serverRow))
			{
				row = this.ConvertOriginalRowFromServerRow(serverRow);
				return true;
			}
			row = null;
			return false;
		}

		private ViewType ViewType
		{
			get
			{
				base.CheckDisposed();
				return this.viewType;
			}
		}

		private const int MaximumOfCategoriesAndAggregations = 4;

		private const int MaximumOfSortEntries = 6;

		private static readonly Guid mapiNamespace = new Guid("{00020328-0000-0000-C000-000000000046}");

		private readonly TableFlags tableFlags;

		private readonly View.Capabilities capabilities;

		private readonly ServerObjectHandle returnNotificationHandle;

		private readonly NotificationHandler notificationHandler;

		private PropertyTag[] originalColumns;

		private PropertyTag[] serverColumns;

		private int instanceKeyColumnIndex = -1;

		private ViewCache viewCache;

		private int expandedCount;

		private bool hasSortTableFailed;

		private bool hasRestrictFailed;

		private bool isCursorPositionUndefined;

		private QueryFilter filter;

		private QueryFilter defaultQueryFilter;

		private ViewType viewType;

		private NotificationSink viewNotificationSink;

		private GroupByAndOrder[] groupBys;

		private SortBy[] sortBys;

		private PropertyTag[] sortPropertyTags;

		internal bool UseForwardDirection = true;

		internal enum RowLookupPosition : byte
		{
			Previous,
			Current
		}

		[Flags]
		internal enum Capabilities
		{
			Basic = 0,
			CanRestrict = 1,
			CanSort = 2,
			CanReset = 4,
			CanAbort = 8,
			CanUseBookmarks = 16,
			CanFreeBookmarks = 32,
			CanGroup = 64,
			All = 127
		}
	}
}
