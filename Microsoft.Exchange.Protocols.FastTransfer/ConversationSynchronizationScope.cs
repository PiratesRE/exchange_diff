using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LazyIndexing;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class ConversationSynchronizationScope : ContentSynchronizationScopeBase
	{
		public ConversationSynchronizationScope(MapiFolder folder, SyncFlag syncFlags, SyncExtraFlag extraFlags, StorePropTag[] propertyTags, FastTransferDownloadContext context) : base(folder, context)
		{
			this.syncFlags = syncFlags;
			this.conversationHeaderColumns = IcsContentDownloadContext.MessageChange.StandardConversationHeaderColumns;
			if ((ushort)(this.syncFlags & SyncFlag.CatchUp) == 0 && (extraFlags & SyncExtraFlag.CatchUpFull) == SyncExtraFlag.None && (extraFlags & SyncExtraFlag.NoChanges) == SyncExtraFlag.None && propertyTags != null)
			{
				HashSet<StorePropTag> hashSet = new HashSet<StorePropTag>(propertyTags);
				hashSet.ExceptWith(this.conversationHeaderColumns);
				hashSet.Remove(PropTag.Message.SourceKey);
				hashSet.Remove(PropTag.Message.ChangeKey);
				hashSet.Remove(PropTag.Message.ChangeNumber);
				StorePropTag[] array = new StorePropTag[this.conversationHeaderColumns.Length + hashSet.Count];
				Array.Copy(this.conversationHeaderColumns, 0, array, 0, this.conversationHeaderColumns.Length);
				hashSet.CopyTo(array, this.conversationHeaderColumns.Length);
				this.conversationHeaderColumns = array;
			}
		}

		public override IEnumerable<Properties> GetChangedMessages(MapiContext operationContext, IcsState icsState)
		{
			this.PrepareViews(operationContext, icsState);
			List<Properties> list = new List<Properties>(100);
			using (MapiViewTableBase.RowReader rowReader = this.conversationChangesView.QueryRows(operationContext, QueryRowsFlags.None))
			{
				Properties item;
				while (rowReader.ReadNext(operationContext, out item))
				{
					if (this.residualFilteringRequired)
					{
						ExchangeId id = ExchangeId.CreateFrom9ByteArray(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, (byte[])item[1].Value);
						if (icsState.CnsetSeen.Contains(id))
						{
							continue;
						}
					}
					item[3] = new Property(PropTag.Message.ChangeType, 2);
					list.Add(item);
				}
			}
			LogicalIndex logicalIndex = LogicalIndexCache.FindIndex(operationContext, base.Logon.StoreMailbox, base.Folder.Fid, LogicalIndexType.ConversationDeleteHistory, 1);
			if (logicalIndex != null)
			{
				logicalIndex.UpdateIndex(operationContext, LogicalIndex.CannotRepopulate);
				Column column = PropertySchema.MapToColumn(operationContext.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message, PropTag.Message.Internal9ByteChangeNumber);
				Column column2 = PropertySchema.MapToColumn(operationContext.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message, PropTag.Message.Mid);
				Column column3 = PropertySchema.MapToColumn(operationContext.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message, PropTag.Message.LastModificationTime);
				Column column4 = PropertySchema.MapToColumn(operationContext.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message, PropTag.Message.ConversationId);
				IList<KeyRange> keyRanges;
				if (this.cnRestriction != null)
				{
					keyRanges = QueryPlanner.BuildKeyRangesFromRegularOrCriteria(logicalIndex.IndexKeyPrefix, logicalIndex, this.cnRestriction.ToSearchCriteria(operationContext.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message), false, operationContext.Culture);
				}
				else
				{
					keyRanges = new KeyRange[]
					{
						new KeyRange(new StartStopKey(true, logicalIndex.IndexKeyPrefix), new StartStopKey(true, logicalIndex.IndexKeyPrefix))
					};
				}
				using (TableOperator tableOperator = Factory.CreateTableOperator(operationContext.Culture, operationContext, logicalIndex.IndexTable, logicalIndex.IndexTable.PrimaryKeyIndex, new Column[]
				{
					column,
					column2,
					column3,
					column4
				}, null, logicalIndex.RenameDictionary, 0, 0, keyRanges, false, true))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						HashSet<long> hashSet = null;
						while (reader.Read())
						{
							byte[] binary = reader.GetBinary(column);
							if (this.residualFilteringRequired)
							{
								ExchangeId id2 = ExchangeId.CreateFrom9ByteArray(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, binary);
								if (icsState.CnsetSeen.Contains(id2))
								{
									continue;
								}
							}
							if (hashSet == null)
							{
								hashSet = new HashSet<long>();
								foreach (Properties properties in list)
								{
									hashSet.Add((long)properties[0].Value);
								}
							}
							long @int = reader.GetInt64(column2);
							if (hashSet.Contains(@int))
							{
								list.Add(new Properties(IcsContentDownloadContext.MessageChange.StandardConversationHeaderColumns.Length)
								{
									new Property(PropTag.Message.Mid, 0L),
									new Property(PropTag.Message.Internal9ByteChangeNumber, binary),
									new Property(PropTag.Message.LastModificationTime, DateTime.MinValue),
									new Property(PropTag.Message.ChangeType, 0),
									new Property(PropTag.Message.ConversationId.ConvertToError(), ErrorCodeValue.NotFound)
								});
							}
							else
							{
								hashSet.Add(@int);
								DateTime dateTime = reader.GetDateTime(column3);
								byte[] binary2 = reader.GetBinary(column4);
								list.Add(new Properties(IcsContentDownloadContext.MessageChange.StandardConversationHeaderColumns.Length)
								{
									new Property(PropTag.Message.Mid, @int),
									new Property(PropTag.Message.Internal9ByteChangeNumber, binary),
									new Property(PropTag.Message.LastModificationTime, dateTime),
									new Property(PropTag.Message.ChangeType, 3),
									new Property(PropTag.Message.ConversationId, binary2)
								});
							}
						}
					}
				}
			}
			list.Sort(delegate(Properties x, Properties y)
			{
				short num = (short)x[3].Value;
				short value = (short)y[3].Value;
				int num2 = num.CompareTo(value);
				if (num2 != 0)
				{
					return num2;
				}
				DateTime t = (DateTime)x[2].Value;
				DateTime t2 = (DateTime)y[2].Value;
				return DateTime.Compare(t2, t);
			});
			return list;
		}

		public override IdSet GetDeletes(MapiContext operationContext, IcsState icsState)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "this method should not be called for conversation sync");
			return null;
		}

		public override IdSet GetSoftDeletes(MapiContext operationContext, IcsState icsState)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "this method should not be called for conversation sync");
			return null;
		}

		public override void GetNewReadsUnreads(MapiContext operationContext, IcsState icsState, out IdSet midsetNewReads, out IdSet midsetNewUnreads, out IdSet finalCnsetRead)
		{
			midsetNewReads = null;
			midsetNewUnreads = null;
			finalCnsetRead = null;
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "this method should not be called for conversation sync");
		}

		public override FastTransferMessage OpenMessage(ExchangeId mid)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "conversation ICS should be manifest-only");
			return null;
		}

		public override PropertyGroupMapping GetPropertyGroupMapping()
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "conversation ICS should be manifest-only");
			return null;
		}

		public override IChunked PrepareIndexes(MapiContext operationContext, IcsState icsState)
		{
			this.PrepareViews(operationContext, icsState);
			if (this.conversationChangesView != null)
			{
				return this.conversationChangesView.PrepareIndexes(operationContext, null);
			}
			return null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ConversationSynchronizationScope>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.conversationChangesView != null)
			{
				this.conversationChangesView.Dispose();
				this.conversationChangesView = null;
			}
		}

		private void PrepareViews(MapiContext operationContext, IcsState icsState)
		{
			if (!this.viewsPrepared)
			{
				if ((ushort)(this.syncFlags & SyncFlag.CatchUp) == 0)
				{
					SortOrder[] legacySortOrder = new SortOrder[]
					{
						new SortOrder(new PropertyTag(PropTag.Message.Internal9ByteChangeNumber.PropTag), SortOrderFlags.Ascending)
					};
					this.cnRestriction = ContentSynchronizationScopeBase.CreateCnsetSeenRestriction(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, PropTag.Message.Internal9ByteChangeNumber, icsState.CnsetSeen, false, out this.residualFilteringRequired);
					this.conversationChangesView = new MapiViewMessage();
					ViewMessageConfigureFlags flags = ViewMessageConfigureFlags.NoNotifications | ViewMessageConfigureFlags.Conversation | ViewMessageConfigureFlags.UseCoveringIndex;
					this.conversationChangesView.Configure(operationContext, base.Logon, base.Folder, flags);
					this.conversationChangesView.SetColumns(operationContext, this.conversationHeaderColumns, MapiViewSetColumnsFlag.NoColumnValidation);
					this.conversationChangesView.Sort(operationContext, legacySortOrder, SortTableFlags.None);
					this.conversationChangesView.Restrict(operationContext, 0, this.cnRestriction);
				}
				this.viewsPrepared = true;
			}
		}

		private SyncFlag syncFlags;

		private StorePropTag[] conversationHeaderColumns;

		private bool viewsPrepared;

		private MapiViewMessage conversationChangesView;

		private Restriction cnRestriction;

		private bool residualFilteringRequired;
	}
}
