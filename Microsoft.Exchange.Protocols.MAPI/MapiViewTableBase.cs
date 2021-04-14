using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public abstract class MapiViewTableBase : MapiBase
	{
		protected MapiViewTableBase(MapiObjectType mapiObjectType) : base(mapiObjectType)
		{
			this.configurationError = new MapiViewTableBase.ConfigurationOperationError();
		}

		internal ViewTable StoreViewTable
		{
			get
			{
				return this.storeViewTable;
			}
		}

		public virtual StorePropTag[] ViewColumns
		{
			get
			{
				return this.viewColumns;
			}
		}

		public uint Hsot
		{
			get
			{
				return this.hsot;
			}
		}

		public virtual bool IsCategorizedViewSupported
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsMultiValueExplosionSupported
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportSortOperation
		{
			get
			{
				return true;
			}
		}

		public virtual bool SupportSeekOperation
		{
			get
			{
				return true;
			}
		}

		public bool IsExplodingMultiValue
		{
			get
			{
				return this.sortOrderMultiValueTag != null;
			}
		}

		public Guid? CorrelationId { get; protected set; }

		internal MapiViewTableBase.ConfigurationOperationError ConfigurationError
		{
			get
			{
				return this.configurationError;
			}
		}

		public bool IsCommonCategorization
		{
			get
			{
				return this.commonCategorization;
			}
		}

		public virtual bool IsOptimizedInstantSearch
		{
			get
			{
				return false;
			}
		}

		internal void Configure(MapiContext context, MapiLogon logon, StorePropTag[] notificationColumns, ViewTable storeViewTable, uint hsot, NotificationSubscription subscription)
		{
			base.Logon = logon;
			if (base.IsValid)
			{
				if (this.subscription != null)
				{
					this.subscription.Unregister();
					this.subscription = null;
				}
			}
			else
			{
				base.ParentObject = logon;
				this.hsot = hsot;
			}
			this.storeViewTable = storeViewTable;
			this.subscription = subscription;
			if (subscription != null)
			{
				subscription.Register(context);
				this.mappedNotificationColumns = new Column[notificationColumns.Length];
				for (int i = 0; i < notificationColumns.Length; i++)
				{
					this.mappedNotificationColumns[i] = this.MapColumn(notificationColumns[i], logon.StoreMailbox.Database, base.MapiObjectType);
				}
			}
			else
			{
				this.mappedNotificationColumns = new Column[0];
			}
			base.IsValid = true;
		}

		protected static TableModifiedNotificationEvent CreateTableModifiedEvent(StoreDatabase database, int mailboxNumber, ClientType clientType, EventFlags eventFlags, TableEventType tableEventType, ExchangeId fid, ExchangeId mid, int inst, ExchangeId previousFid, ExchangeId previousMid, int previousInst, Properties row)
		{
			return new TableModifiedNotificationEvent(database, mailboxNumber, null, clientType, eventFlags, tableEventType, fid, mid, inst, previousFid, previousMid, previousInst, row);
		}

		protected abstract bool CanSortOnProperty(StorePropTag propTag);

		protected virtual bool GetTableNotificationInfo(Context context, SearchCriteria findRowCriteria, out Properties rowProps, out object[] prevRowProps)
		{
			rowProps = Properties.Empty;
			prevRowProps = null;
			bool flag = false;
			if (!this.IsMultiValueSettingValid())
			{
				DiagnosticContext.TraceLocation((LID)58568U);
				return false;
			}
			this.storeViewTable.SaveLastBookmark();
			try
			{
				bool flag2;
				using (Reader reader = this.storeViewTable.FindRow(context, findRowCriteria, ViewSeekOrigin.Beginning, null, false, out flag2))
				{
					if (reader != null)
					{
						rowProps = this.FillRow((MapiContext)context, reader);
						MapiViewTableBase.TruncateLongValues(context, this, rowProps);
						flag = true;
					}
				}
				if (flag && this.mappedNotificationColumns.Length != 0)
				{
					using (Reader reader2 = this.storeViewTable.QueryRows(context, 1, true))
					{
						if (reader2 != null && reader2.Read())
						{
							prevRowProps = new object[this.mappedNotificationColumns.Length];
							for (int i = 0; i < this.mappedNotificationColumns.Length; i++)
							{
								prevRowProps[i] = reader2.GetValue(this.mappedNotificationColumns[i]);
							}
						}
					}
				}
			}
			finally
			{
				this.storeViewTable.RevertToLastBookmark();
			}
			return flag;
		}

		protected Column MapColumn(StorePropTag propTag, StoreDatabase database, MapiObjectType objectType)
		{
			return PropertySchema.MapToColumn(database, Helper.GetPropTagObjectType(objectType), propTag);
		}

		protected Properties FillRow(MapiContext context, Reader reader)
		{
			int num = (this.viewColumns == null) ? 0 : this.viewColumns.Length;
			Properties properties = new Properties(num);
			if (num > 0)
			{
				MapiViewTableBase.RowColumnReader rowColumnReader = new MapiViewTableBase.RowColumnReader(this, reader);
				Property prop;
				while (rowColumnReader.ReadNextColumn(context, out prop))
				{
					properties.Add(prop);
				}
				if (ExTraceGlobals.FillRowTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.FillRowTracer.TraceDebug<int, Properties>(0L, "Fetched {0} properties: {1}", num, properties);
				}
			}
			if (ExTraceGlobals.FillRowTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.FillRowTracer.TraceDebug<int, Properties>(0L, "Fetched {0} properties: {1}", num, properties);
			}
			return properties;
		}

		public override void FormatDiagnosticInformation(TraceContentBuilder cb, int indentLevel)
		{
			if (this.storeViewTable != null)
			{
				cb.Indent(indentLevel);
				cb.AppendLine("Type: " + this.storeViewTable.GetType().Name);
				if (this.storeViewTable.ImplicitCriteria != null)
				{
					cb.Indent(indentLevel);
					cb.AppendLine("Implicit Criteria: " + this.storeViewTable.ImplicitCriteria.ToString());
				}
				if (this.storeViewTable.RestrictCriteria != null)
				{
					cb.Indent(indentLevel);
					cb.AppendLine("Restrict Criteria: " + this.storeViewTable.RestrictCriteria.ToString());
				}
				if (this.findRowRestriction != null)
				{
					cb.Indent(indentLevel);
					cb.AppendLine("FindRow Criteria: " + this.findRowRestriction.ToString());
				}
				cb.Indent(indentLevel);
				cb.AppendLine("SortOrder: " + this.storeViewTable.SortOrder.ToString());
			}
		}

		public override void ClearDiagnosticInformation()
		{
			this.findRowRestriction = null;
		}

		public override void GetSummaryInformation(ref ExecutionDiagnostics.LongOperationSummary summary)
		{
			if (this.storeViewTable != null)
			{
				summary.OperationDetail = this.storeViewTable.GetType().Name;
				summary.SortOrderCount = this.storeViewTable.SortOrder.Count;
			}
		}

		public virtual void ResetTable()
		{
			if (this.storeViewTable != null)
			{
				this.storeViewTable.Reset();
			}
			this.indexesPrepared = false;
			this.viewColumns = null;
			this.mappedViewColumns = null;
			this.configurationError.Reset();
		}

		public void SetColumns(MapiContext context, StorePropTag[] columns)
		{
			this.SetColumns(context, columns, MapiViewSetColumnsFlag.None);
		}

		public virtual void SetColumns(MapiContext context, StorePropTag[] columns, MapiViewSetColumnsFlag flags)
		{
			base.ThrowIfNotValid(null);
			this.indexesPrepared = false;
			try
			{
				this.viewColumns = columns;
				this.mappedViewColumns = new Column[this.viewColumns.Length];
				for (int i = 0; i < this.viewColumns.Length; i++)
				{
					if (this.viewColumns[i].PropType == PropertyType.Unspecified)
					{
						throw new ExExceptionInvalidParameter((LID)52796U, "PT_UNSPECIFIED property is not supported as a view column");
					}
					this.mappedViewColumns[i] = this.MapColumn(this.viewColumns[i], base.Logon.StoreMailbox.Database, base.MapiObjectType);
				}
				List<Column> list = new List<Column>(this.viewColumns.Length + this.mappedNotificationColumns.Length);
				foreach (StorePropTag storePropTag in this.AdjustColumnsToQuery(this.viewColumns))
				{
					if (storePropTag.PropType == PropertyType.Unspecified)
					{
						throw new ExExceptionInvalidParameter((LID)48440U, "PT_UNSPECIFIED property is not supported as a view column");
					}
					Column column = null;
					for (int j = 0; j < this.viewColumns.Length; j++)
					{
						if (this.viewColumns[j] == storePropTag)
						{
							column = this.mappedViewColumns[j];
							break;
						}
					}
					if (column == null)
					{
						column = this.MapColumn(storePropTag, base.Logon.StoreMailbox.Database, base.MapiObjectType);
					}
					if (!list.Contains(column))
					{
						list.Add(column);
					}
				}
				for (int k = 0; k < this.mappedNotificationColumns.Length; k++)
				{
					if (!list.Contains(this.mappedNotificationColumns[k]))
					{
						list.Add(this.mappedNotificationColumns[k]);
					}
				}
				if (this.storeViewTable != null)
				{
					ViewSetColumnsFlag viewSetColumnsFlag = ViewSetColumnsFlag.None;
					if ((flags & MapiViewSetColumnsFlag.NoColumnValidation) != MapiViewSetColumnsFlag.None)
					{
						viewSetColumnsFlag |= ViewSetColumnsFlag.NoColumnValidation;
					}
					this.storeViewTable.SetColumns(context, list, viewSetColumnsFlag);
				}
			}
			finally
			{
				if (this.Hsot != 4294967295U)
				{
					base.Logon.FlushStaleTableModifiedNotifications(this.Hsot);
				}
			}
		}

		public IList<StorePropTag> QueryColumnsAll()
		{
			return this.ViewColumns;
		}

		protected virtual IList<StorePropTag> AdjustColumnsToQuery(IList<StorePropTag> columns)
		{
			return columns;
		}

		protected virtual object GetPropertyValue(MapiContext context, Reader reader, StorePropTag propertyTag, Column column)
		{
			if (column == null)
			{
				column = this.MapColumn(propertyTag, base.Logon.StoreMailbox.Database, base.MapiObjectType);
			}
			return reader.GetValue(column);
		}

		internal MapiViewTableBase.RowReader QueryRows(MapiContext context, QueryRowsFlags flags)
		{
			return this.QueryRows(context, -1, flags);
		}

		internal virtual MapiViewTableBase.RowReader QueryRows(MapiContext context, int numRows, QueryRowsFlags flags)
		{
			if (this.viewColumns == null)
			{
				throw new StoreException((LID)40248U, ErrorCodeValue.NullObject);
			}
			base.ThrowIfNotValid(null);
			this.ValidateMultiValueConstrains();
			if (this.storeViewTable != null && numRows != 0)
			{
				return new MapiViewTableBase.RowReader(context, this, flags, numRows);
			}
			return default(MapiViewTableBase.RowReader);
		}

		internal IList<Properties> QueryRowsBatch(MapiContext context, int numRows, QueryRowsFlags flags)
		{
			List<Properties> list = null;
			using (MapiViewTableBase.RowReader rowReader = this.QueryRows(context, numRows, flags))
			{
				Properties item;
				if (rowReader.ReadNext(context, out item))
				{
					list = new List<Properties>(Math.Min(numRows, 100));
					do
					{
						list.Add(item);
					}
					while (rowReader.ReadNext(context, out item));
				}
			}
			return list;
		}

		public byte[] CreateBookmark()
		{
			if (this.storeViewTable != null)
			{
				return this.storeViewTable.CreateExternalBookmark();
			}
			return null;
		}

		public void FreeBookmark(byte[] bookmark)
		{
			if (this.storeViewTable != null)
			{
				this.storeViewTable.FreeExternalBookmark(bookmark);
			}
		}

		public void SeekRow(MapiContext context, ViewSeekOrigin origin, byte[] bookmark, int rowCountToSeek, bool wantSoughtRowCount, out bool soughtLessThanRowCount, out int rowCountActuallySought, bool wantValidateBookmarkPosition, out bool bookmarkPositionChanged)
		{
			base.ThrowIfNotValid(null);
			if (!this.SupportSeekOperation)
			{
				throw new StoreException((LID)54992U, ErrorCodeValue.NotSupported);
			}
			this.ValidateMultiValueConstrains();
			soughtLessThanRowCount = false;
			rowCountActuallySought = rowCountToSeek;
			bookmarkPositionChanged = false;
			if (this.storeViewTable != null)
			{
				this.storeViewTable.SeekRow(context, origin, bookmark, rowCountToSeek, wantSoughtRowCount, out soughtLessThanRowCount, out rowCountActuallySought, wantValidateBookmarkPosition, out bookmarkPositionChanged);
			}
		}

		public void SeekRowApprox(MapiContext context, int numerator, int denominator)
		{
			base.ThrowIfNotValid(null);
			if (!this.SupportSeekOperation)
			{
				throw new StoreException((LID)42704U, ErrorCodeValue.NotSupported);
			}
			if (this.storeViewTable == null)
			{
				return;
			}
			if (numerator > denominator || denominator == 0)
			{
				throw new ExExceptionInvalidParameter((LID)44344U, "Invalid parameter to SeekRowApprox");
			}
			int numberOfRows = (numerator == 0) ? 0 : (this.GetRowCount(context) * numerator / denominator);
			bool flag;
			int num;
			bool flag2;
			this.storeViewTable.SeekRow(context, ViewSeekOrigin.Beginning, null, numberOfRows, false, out flag, out num, false, out flag2);
		}

		public bool MayNeedIndexForQueryPosition(MapiContext context)
		{
			return this.storeViewTable != null && this.storeViewTable.NeedIndexForPositionOrRowCount(context);
		}

		public void QueryPosition(MapiContext context, ref int numerator, ref int denominator)
		{
			numerator = 0;
			denominator = 0;
			base.ThrowIfNotValid(null);
			if (this.storeViewTable != null)
			{
				denominator = this.GetRowCount(context);
				numerator = this.storeViewTable.GetPosition(context);
				if (numerator < 0)
				{
					numerator = 0;
					return;
				}
				if (numerator > denominator)
				{
					numerator = denominator;
				}
			}
		}

		internal void Sort(MapiContext context, SortOrder[] legacySortOrder, SortTableFlags flags)
		{
			this.Sort(context, legacySortOrder, flags, 0U, 0U);
		}

		internal virtual void Sort(MapiContext context, SortOrder[] legacySortOrder, SortTableFlags flags, uint categoryCount, uint expandedCount)
		{
			base.ThrowIfNotValid(null);
			if (!this.SupportSortOperation)
			{
				throw new StoreException((LID)59088U, ErrorCodeValue.NotSupported);
			}
			bool flag = false;
			int num = 0;
			if (legacySortOrder.Length > 6)
			{
				throw new ExExceptionTooComplex((LID)60728U, "SortOrder cannot exceed " + 6);
			}
			if (!EnumValidator.IsValidValue<SortTableFlags>(flags))
			{
				throw new ExExceptionInvalidParameter((LID)36152U, "Invalid Sort Flags " + flags);
			}
			if (categoryCount != 0U || expandedCount != 0U)
			{
				if (!this.IsCategorizedViewSupported)
				{
					throw new ExExceptionTooComplex((LID)61112U, "This view type does not support categorized views.");
				}
				if ((ulong)categoryCount > (ulong)((long)legacySortOrder.Length) || expandedCount > categoryCount)
				{
					throw new ExExceptionInvalidParameter((LID)36536U, "Invalid category count and/or expanded count.");
				}
				flag = true;
			}
			this.indexesPrepared = false;
			this.commonCategorization = (flag && categoryCount == 1U);
			if (this.storeViewTable != null)
			{
				SortOrderBuilder sortOrderBuilder = new SortOrderBuilder();
				CategoryHeaderSortOverride[] array = new CategoryHeaderSortOverride[categoryCount];
				bool flag2 = false;
				bool ascending = false;
				this.sortOrderMultiValueTag = null;
				foreach (SortOrder sortOrder in legacySortOrder)
				{
					StorePropTag propTag = this.ValidateAndConvertLegacyPropTag(sortOrder.Tag);
					if (!this.CanSortOnProperty(propTag))
					{
						throw new ExExceptionTooComplex((LID)30524U, "Can not sort on that prop tag.");
					}
					Column column = this.MapColumn(propTag, base.Logon.StoreMailbox.Database, base.MapiObjectType);
					this.ValidateSortOrderFlags(sortOrder.Flags);
					bool flag3 = (byte)(sortOrder.Flags & SortOrderFlags.Descending) == 0;
					bool flag4 = sortOrderBuilder.Contains(column);
					if ((byte)(sortOrder.Flags & (SortOrderFlags.CategoryMaximum | SortOrderFlags.CategoryMinimum)) != 0)
					{
						if (flag4)
						{
							throw new ExExceptionTooComplex((LID)39608U, "A category header sort override was already specified as a category header.");
						}
						if (CategoryHeaderSortOverride.ContainsColumn(array, column))
						{
							throw new ExExceptionTooComplex((LID)55992U, "The same prop cannot appear multiple times as a category header sort override.");
						}
						if (propTag.IsMultiValueInstance)
						{
							throw new ExExceptionTooComplex((LID)59064U, "Category header sort overrides cannot be multi-valued (or multi-value instance).");
						}
						if (!flag2)
						{
							throw new ExExceptionTooComplex((LID)52920U, "A category header sort override was specified, but the previous prop in the sort key was not a category header.");
						}
						int num2 = num - 1;
						bool aggregateByMaxValue = (byte)(sortOrder.Flags & SortOrderFlags.CategoryMaximum) != 0;
						array[num2] = new CategoryHeaderSortOverride(column, ascending, aggregateByMaxValue);
						flag2 = false;
						if (0UL == (propTag.GroupMask & 36028797018963968UL))
						{
							this.commonCategorization = false;
						}
					}
					else
					{
						if (!flag4)
						{
							sortOrderBuilder.Add(column, flag3);
						}
						if (flag)
						{
							if ((long)num < (long)((ulong)categoryCount))
							{
								if (flag4)
								{
									throw new ExExceptionTooComplex((LID)38584U, "The same prop cannot be specified as a category header multiple times.");
								}
								if (CategoryHeaderSortOverride.ContainsColumn(array, column))
								{
									throw new ExExceptionTooComplex((LID)64184U, "A category header was already specified as a category header sort override.");
								}
								num++;
								flag2 = true;
								ascending = flag3;
								if (0UL == (propTag.GroupMask & 36028797018963968UL))
								{
									this.commonCategorization = false;
								}
							}
							else
							{
								flag2 = false;
							}
						}
					}
				}
				if (num + CategoryHeaderSortOverride.NumberOfOverrides(array) > 4)
				{
					throw new ExExceptionTooComplex((LID)54968U, "Too many category headers and/or category header sort overrides specified.");
				}
				if ((long)num < (long)((ulong)categoryCount))
				{
					throw new ExExceptionTooComplex((LID)42680U, "There weren't enough category headers in the sort key.");
				}
				if (sortOrderBuilder.Count > 0)
				{
					this.storeViewTable.SortTable(sortOrderBuilder.ToSortOrder());
					if (flag)
					{
						this.storeViewTable.Categorize(context, num, (int)expandedCount, array);
					}
				}
			}
		}

		private void ValidateSortOrderFlags(SortOrderFlags flags)
		{
			switch (flags)
			{
			case SortOrderFlags.Ascending:
			case SortOrderFlags.Descending:
			case SortOrderFlags.CategoryMaximum:
			case SortOrderFlags.Descending | SortOrderFlags.CategoryMaximum:
			case SortOrderFlags.CategoryMinimum:
			case SortOrderFlags.Descending | SortOrderFlags.CategoryMinimum:
				return;
			default:
				if (EnumValidator.IsValidValue<SortOrderFlags>(flags))
				{
					throw new ExExceptionNoSupport((LID)46776U, "Unsupported SortOrderFlags value was specified.");
				}
				throw new ExExceptionTooComplex((LID)63160U, "SortOrderFlags value was recognized by the enum, but the particular combination specified is not currently supported.");
			}
		}

		private StorePropTag ValidateAndConvertLegacyPropTag(PropertyTag propertyTag)
		{
			if (propertyTag.PropertyType == PropertyType.Unspecified)
			{
				throw new ExExceptionInvalidParameter((LID)52536U, "PT_UNSPECIFIED property is not supported in a view sort order");
			}
			if (propertyTag.PropertyType == PropertyType.Error)
			{
				throw new ExExceptionInvalidParameter((LID)46392U, "PropertyType.Error propertyType is not supported in a view sort order");
			}
			if (propertyTag.PropertyType == PropertyType.Null)
			{
				throw new ExExceptionInvalidParameter((LID)58680U, "PropertyType.Null propertyType is not supported in a view sort order");
			}
			if (propertyTag.IsMultiValuedProperty)
			{
				if (!propertyTag.IsMultiValueInstanceProperty)
				{
					throw new ExExceptionTooComplex((LID)59704U, "Cannot sort by MV property without MVI flag set");
				}
			}
			else if (propertyTag.IsMultiValueInstanceProperty)
			{
				throw new ExExceptionInvalidParameter((LID)43320U, "MVI without MV property is not supported in a view sort order");
			}
			Microsoft.Exchange.Server.Storage.PropTags.ObjectType propTagObjectType = Helper.GetPropTagObjectType(base.MapiObjectType);
			StorePropTag storePropTag = LegacyHelper.ConvertFromLegacyPropTag(propertyTag, propTagObjectType, base.Logon.MapiMailbox, false);
			if (storePropTag.IsMultiValueInstance)
			{
				if (!this.IsMultiValueExplosionSupported)
				{
					throw new ExExceptionTooComplex((LID)35128U, "This view type does not support sorting by MVI property");
				}
				if (this.sortOrderMultiValueTag != null)
				{
					throw new ExExceptionInvalidParameter((LID)50488U, "Only one MVI property is supported in a view sort order");
				}
				this.sortOrderMultiValueTag = new StorePropTag?(storePropTag);
			}
			return storePropTag;
		}

		public virtual int GetRowCount(MapiContext context)
		{
			int result = 0;
			if (this.storeViewTable != null)
			{
				this.ValidateMultiValueConstrains();
				result = this.storeViewTable.GetRowCount(context);
			}
			return result;
		}

		public virtual bool FindRow(MapiContext context, ViewSeekOrigin origin, byte[] bookmark, bool backwards, Restriction restriction, out bool bookmarkPositionChanged, out Properties row)
		{
			bookmarkPositionChanged = false;
			this.findRowRestriction = restriction;
			if (this.viewColumns == null)
			{
				throw new StoreException((LID)62776U, ErrorCodeValue.NullObject);
			}
			this.ValidateMultiValueConstrains();
			if (this.storeViewTable != null)
			{
				if (this.preparedFindRowCriteria == null)
				{
					this.preparedFindRowCriteria = restriction.ToSearchCriteria(base.Logon.StoreMailbox.Database, Helper.GetPropTagObjectType(base.MapiObjectType));
					this.preparedFindRowCriteria = this.preparedFindRowCriteria.InspectAndFix(null, (context.Culture == null) ? null : context.Culture.CompareInfo, true);
				}
				using (Reader reader = this.storeViewTable.FindRow(context, this.preparedFindRowCriteria, origin, bookmark, backwards, out bookmarkPositionChanged))
				{
					this.preparedFindRowCriteria = null;
					if (reader != null)
					{
						row = this.FillRow(context, reader);
						return true;
					}
				}
			}
			row = Properties.Empty;
			return false;
		}

		public virtual void Restrict(MapiContext context, int flags, Restriction restriction)
		{
			if (!EnumValidator.IsValidValue<RestrictFlags>((RestrictFlags)flags))
			{
				throw new ExExceptionInvalidParameter((LID)38200U, "Invalid Restrict Flags " + flags);
			}
			this.indexesPrepared = false;
			if (this.storeViewTable != null)
			{
				SearchCriteria searchCriteria = (restriction == null) ? null : restriction.ToSearchCriteria(base.Logon.StoreMailbox.Database, Helper.GetPropTagObjectType(base.MapiObjectType));
				if (searchCriteria != null)
				{
					searchCriteria = searchCriteria.InspectAndFix(null, (context.Culture == null) ? null : context.Culture.CompareInfo, true);
				}
				this.storeViewTable.Restrict(context, searchCriteria);
			}
		}

		public virtual int CollapseRow(MapiContext context, ExchangeId categoryId)
		{
			base.ThrowIfNotValid(null);
			if (this.IsCategorizedViewSupported)
			{
				return this.storeViewTable.CollapseRow(context, categoryId);
			}
			throw new ExExceptionNoSupport((LID)33567U, "Categorized views (and collapsing rows) are not supported on this type of view table.");
		}

		public virtual int ExpandRow(MapiContext context, ExchangeId categoryId)
		{
			base.ThrowIfNotValid(null);
			if (this.IsCategorizedViewSupported)
			{
				return this.storeViewTable.ExpandRow(context, categoryId);
			}
			throw new ExExceptionNoSupport((LID)49951U, "Categorized views (and expanding rows) are not supported on this type of view table.");
		}

		public virtual byte[] GetCollapseState(MapiContext context, ExchangeId rowId, int rowInstanceNumber)
		{
			base.ThrowIfNotValid(null);
			if (!this.IsCategorizedViewSupported)
			{
				throw new ExExceptionNoSupport((LID)55168U, "Categorized views (and collapse state) are not supported on this type of view table.");
			}
			return this.storeViewTable.GetCollapseState(context, rowId, rowInstanceNumber);
		}

		public virtual byte[] SetCollapseState(MapiContext context, byte[] collapseState)
		{
			base.ThrowIfNotValid(null);
			if (!this.IsCategorizedViewSupported)
			{
				throw new ExExceptionNoSupport((LID)42880U, "Categorized views (and collapse state) are not supported on this type of view table.");
			}
			return this.storeViewTable.SetCollapseState(context, collapseState);
		}

		public IChunked PrepareIndexes(MapiContext context, Restriction findRowRestriction)
		{
			if (!ConfigurationSchema.ChunkedIndexPopulationEnabled.Value)
			{
				return null;
			}
			if (this.storeViewTable != null && (!this.indexesPrepared || findRowRestriction != null))
			{
				this.findRowRestriction = findRowRestriction;
				this.preparedFindRowCriteria = null;
				if (findRowRestriction != null)
				{
					this.preparedFindRowCriteria = findRowRestriction.ToSearchCriteria(base.Logon.StoreMailbox.Database, Helper.GetPropTagObjectType(base.MapiObjectType));
					this.preparedFindRowCriteria = this.preparedFindRowCriteria.InspectAndFix(null, (context.Culture == null) ? null : context.Culture.CompareInfo, true);
				}
				else
				{
					this.indexesPrepared = true;
				}
				return this.storeViewTable.PrepareIndexes(context, this.preparedFindRowCriteria);
			}
			return null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiViewTableBase>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.subscription != null)
			{
				this.subscription.Unregister();
				this.subscription = null;
			}
			base.InternalDispose(calledFromDispose);
		}

		internal static bool TruncateLongValue(Context context, MapiViewTableBase view, int columnIndex, ref Property property)
		{
			StorePropTag tag = property.Tag;
			string text = property.Value as string;
			if (text != null)
			{
				if (text.Length > 255)
				{
					if (view.MapiObjectType == MapiObjectType.MessageView && (tag == PropTag.Message.RuleMsgProvider || tag == PropTag.Message.RuleMsgName))
					{
						property = new Property(tag.ConvertToError(), LegacyHelper.BoxedErrorCodeNotEnoughMemory);
					}
					else
					{
						property = new Property(tag, text.Substring(0, 255));
					}
					return true;
				}
			}
			else
			{
				byte[] array = property.Value as byte[];
				if (array != null)
				{
					if (view.MapiObjectType == MapiObjectType.MessageView && (tag == PropTag.Message.ExtendedRuleActions || tag == PropTag.Message.ExtendedRuleCondition || tag == PropTag.Message.RuleMsgProviderData))
					{
						if (array.Length > 25600)
						{
							property = new Property(tag.ConvertToError(), LegacyHelper.BoxedErrorCodeNotEnoughMemory);
							return true;
						}
					}
					else
					{
						if (view.MapiObjectType == MapiObjectType.FolderView && tag == PropTag.Folder.NTSecurityDescriptor)
						{
							return false;
						}
						int num = 510;
						if (view.MapiObjectType == MapiObjectType.MessageView && view.viewColumns[columnIndex].IsNamedProperty)
						{
							if (tag.PropName.Equals(NamedPropInfo.Appointment.ApptRecur.PropName))
							{
								num = 25600;
							}
							else
							{
								ClientType clientType = context.ClientType;
								if (clientType <= ClientType.AvailabilityService)
								{
									switch (clientType)
									{
									case ClientType.AirSync:
									case ClientType.OWA:
										break;
									case ClientType.EventBasedAssistants:
										goto IL_1D5;
									default:
										if (clientType != ClientType.AvailabilityService)
										{
											goto IL_1D5;
										}
										break;
									}
								}
								else if (clientType != ClientType.WebServices && clientType != ClientType.OutlookService)
								{
									goto IL_1D5;
								}
								if (tag.PropName.Equals(NamedPropInfo.Appointment.ApptTZDefRecur.PropName) || tag.PropName.Equals(NamedPropInfo.Appointment.ApptTZDefStartDisplay.PropName) || tag.PropName.Equals(NamedPropInfo.Appointment.ApptTZDefEndDisplay.PropName))
								{
									num = ConfigurationSchema.TimeZoneBlobTruncationLength.Value;
								}
							}
						}
						IL_1D5:
						if (array.Length > num)
						{
							byte[] array2 = new byte[num];
							Buffer.BlockCopy(array, 0, array2, 0, array2.Length);
							property = new Property(tag, array2);
							return true;
						}
					}
				}
			}
			return false;
		}

		internal static void TruncateLongValues(Context context, MapiViewTableBase view, Properties row)
		{
			for (int i = 0; i < row.Count; i++)
			{
				Property value = row[i];
				if (MapiViewTableBase.TruncateLongValue(context, view, i, ref value))
				{
					row[i] = value;
				}
			}
		}

		private void ValidateMultiValueConstrains()
		{
			if (!this.IsMultiValueSettingValid())
			{
				throw new ExExceptionNoSupport((LID)54584U, "MVI on SetColumn without MVI on sort is not supported");
			}
		}

		private bool IsMultiValueSettingValid()
		{
			if (this.viewColumns == null)
			{
				return true;
			}
			StorePropTag[] array = this.viewColumns;
			int i = 0;
			while (i < array.Length)
			{
				StorePropTag tag = array[i];
				if (tag.IsMultiValueInstance)
				{
					if (this.sortOrderMultiValueTag == null || tag != this.sortOrderMultiValueTag.Value)
					{
						return false;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			return true;
		}

		private const int MaximumNumberofSortOrders = 6;

		protected Column[] mappedNotificationColumns;

		private StorePropTag[] viewColumns;

		private Column[] mappedViewColumns;

		private StorePropTag? sortOrderMultiValueTag;

		private ViewTable storeViewTable;

		private NotificationSubscription subscription;

		private uint hsot = uint.MaxValue;

		private Restriction findRowRestriction;

		private bool commonCategorization;

		private bool indexesPrepared;

		private SearchCriteria preparedFindRowCriteria;

		private MapiViewTableBase.ConfigurationOperationError configurationError;

		internal class ConfigurationOperationError
		{
			public ErrorCode SetColumns
			{
				get
				{
					return this.setColumns;
				}
				set
				{
					this.setColumns = value;
					MapiViewTableBase.ConfigurationOperationError.TraceError("SetColumns", this.setColumns);
				}
			}

			public ErrorCode SortTable
			{
				get
				{
					return this.sortTable;
				}
				set
				{
					this.sortTable = value;
					MapiViewTableBase.ConfigurationOperationError.TraceError("Sort", this.sortTable);
				}
			}

			public ErrorCode Restrict
			{
				get
				{
					return this.restrict;
				}
				set
				{
					this.restrict = value;
					MapiViewTableBase.ConfigurationOperationError.TraceError("Restrict", this.restrict);
				}
			}

			public ErrorCode ErrorCode
			{
				get
				{
					if (this.setColumns != ErrorCode.None)
					{
						return this.setColumns;
					}
					if (this.sortTable != ErrorCode.None)
					{
						return this.sortTable;
					}
					if (this.restrict != ErrorCode.None)
					{
						return this.restrict;
					}
					return ErrorCode.None;
				}
			}

			public bool HasConfigurationError
			{
				get
				{
					return this.ErrorCode != ErrorCode.None;
				}
			}

			public void Reset()
			{
				this.setColumns = ErrorCode.None;
				this.sortTable = ErrorCode.None;
				this.restrict = ErrorCode.None;
			}

			private static void TraceError(string operation, ErrorCode error)
			{
				if (error != ErrorCode.None && ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.GeneralTracer.TraceError<string, ErrorCode>(0L, "MapiViewTableBase:Operation [{0}] Encountered Error=[{1}]", operation, error);
				}
			}

			private ErrorCode setColumns;

			private ErrorCode sortTable;

			private ErrorCode restrict;
		}

		public struct RowColumnReader
		{
			internal RowColumnReader(MapiViewTableBase view, Reader reader)
			{
				this.view = view;
				this.reader = reader;
				this.index = 0;
			}

			public int ColumnCount
			{
				get
				{
					if (this.view == null || this.view.viewColumns == null)
					{
						return 0;
					}
					return this.view.viewColumns.Length;
				}
			}

			public bool ReadNextColumn(MapiContext context, out Property property)
			{
				if (this.view == null || this.index >= this.view.viewColumns.Length)
				{
					property = default(Property);
					return false;
				}
				object propertyValue = this.view.GetPropertyValue(context, this.reader, this.view.viewColumns[this.index], this.view.mappedViewColumns[this.index]);
				if (propertyValue != null)
				{
					property = new Property(this.view.viewColumns[this.index], propertyValue);
				}
				else
				{
					property = new Property(this.view.viewColumns[this.index].ConvertToError(), LegacyHelper.BoxedErrorCodeNotFound);
				}
				this.index++;
				return true;
			}

			private MapiViewTableBase view;

			private Reader reader;

			private int index;
		}

		public struct RowReader : IDisposable
		{
			internal RowReader(MapiContext context, MapiViewTableBase view, QueryRowsFlags flags, int numRows)
			{
				this.view = view;
				this.flags = flags;
				this.reader = view.StoreViewTable.QueryRows(context, (numRows < 0) ? 0 : numRows, 0 != (byte)(flags & QueryRowsFlags.Backwards));
			}

			public bool ReadNextRow(MapiContext context, out MapiViewTableBase.RowColumnReader columnReader)
			{
				if (this.reader != null && this.reader.Read())
				{
					columnReader = new MapiViewTableBase.RowColumnReader(this.view, this.reader);
					if ((byte)(this.flags & QueryRowsFlags.DoNotAdvance) == 0)
					{
						this.view.StoreViewTable.QueryRowsViewTable.SaveLastBookmark();
						int? cachedPosition = this.view.StoreViewTable.QueryRowsViewTable.GetCachedPosition();
						if (cachedPosition != null)
						{
							cachedPosition = new int?(cachedPosition.Value + (((byte)(this.flags & QueryRowsFlags.Backwards) != 0) ? -1 : 1));
						}
						this.view.StoreViewTable.QueryRowsViewTable.BookmarkCurrentRow(this.reader, 0 != (byte)(this.flags & QueryRowsFlags.Backwards), cachedPosition);
					}
					return true;
				}
				columnReader = default(MapiViewTableBase.RowColumnReader);
				return false;
			}

			public bool ReadNext(MapiContext context, out Properties row)
			{
				if (this.reader != null && this.reader.Read())
				{
					row = this.view.FillRow(context, this.reader);
					if ((byte)(this.flags & QueryRowsFlags.DoNotAdvance) == 0)
					{
						this.view.StoreViewTable.QueryRowsViewTable.SaveLastBookmark();
						int? cachedPosition = this.view.StoreViewTable.QueryRowsViewTable.GetCachedPosition();
						if (cachedPosition != null)
						{
							cachedPosition = new int?(cachedPosition.Value + (((byte)(this.flags & QueryRowsFlags.Backwards) != 0) ? -1 : 1));
						}
						this.view.StoreViewTable.QueryRowsViewTable.BookmarkCurrentRow(this.reader, 0 != (byte)(this.flags & QueryRowsFlags.Backwards), cachedPosition);
					}
					return true;
				}
				row = Properties.Empty;
				return false;
			}

			public void UnreadLast()
			{
				if ((byte)(this.flags & QueryRowsFlags.DoNotAdvance) == 0)
				{
					this.view.StoreViewTable.QueryRowsViewTable.RevertToLastBookmark();
				}
			}

			public void Dispose()
			{
				if (this.reader != null)
				{
					this.reader.Dispose();
				}
			}

			private MapiViewTableBase view;

			private Reader reader;

			private QueryRowsFlags flags;
		}
	}
}
