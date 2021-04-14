using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal abstract class ContentSynchronizationScopeBase : DisposableBase, IContentSynchronizationScope, IDisposable
	{
		protected ContentSynchronizationScopeBase(MapiFolder folder, FastTransferDownloadContext context)
		{
			this.folder = folder;
			this.context = context;
		}

		public MapiLogon Logon
		{
			get
			{
				return this.folder.Logon;
			}
		}

		public MapiFolder Folder
		{
			get
			{
				return this.folder;
			}
		}

		public FastTransferDownloadContext DownloadContext
		{
			get
			{
				return this.context;
			}
		}

		public MapiContext CurrentOperationContext
		{
			get
			{
				return (MapiContext)this.Logon.StoreMailbox.CurrentOperationContext;
			}
		}

		public ExchangeId GetExchangeId(long shortTermId)
		{
			return ExchangeId.CreateFromInt64(this.CurrentOperationContext, this.Logon.StoreMailbox.ReplidGuidMap, shortTermId);
		}

		public ReplId GuidToReplid(Guid guid)
		{
			return new ReplId(this.Logon.StoreMailbox.ReplidGuidMap.GetReplidFromGuid(this.CurrentOperationContext, guid));
		}

		public IdSet GetServerCnsetSeen(MapiContext operationContext, bool conversations)
		{
			Folder folder;
			if (conversations)
			{
				folder = Microsoft.Exchange.Server.Storage.LogicalDataModel.Folder.OpenFolder(operationContext, this.Logon.StoreMailbox, ConversationItem.GetConversationFolderId(operationContext, this.Logon.StoreMailbox));
			}
			else
			{
				folder = this.Folder.StoreFolder;
			}
			return folder.GetCnsetSeen(operationContext);
		}

		public abstract IEnumerable<Properties> GetChangedMessages(MapiContext operationContext, IcsState icsState);

		public abstract IdSet GetDeletes(MapiContext operationContext, IcsState icsState);

		public abstract IdSet GetSoftDeletes(MapiContext operationContext, IcsState icsState);

		public abstract void GetNewReadsUnreads(MapiContext operationContext, IcsState icsState, out IdSet midsetNewReads, out IdSet midsetNewUnreads, out IdSet finalCnsetRead);

		public abstract FastTransferMessage OpenMessage(ExchangeId mid);

		public abstract PropertyGroupMapping GetPropertyGroupMapping();

		public virtual IChunked PrepareIndexes(MapiContext operationContext, IcsState icsState)
		{
			return null;
		}

		internal static Restriction CreateCnsetSeenRestriction(MapiContext operationContext, ReplidGuidMap replidGuidMap, StorePropTag propTag, IdSet cnset, bool inclusive, out bool residualFilteringRequired)
		{
			residualFilteringRequired = false;
			if (cnset == null || cnset.IsEmpty)
			{
				return null;
			}
			bool flag = propTag == PropTag.Folder.ChangeNumberBin;
			int countRanges = cnset.CountRanges;
			List<ContentSynchronizationScopeBase.IdRange> list = new List<ContentSynchronizationScopeBase.IdRange>((countRanges > 20) ? 20 : countRanges);
			ulong value = ConfigurationSchema.MinRangeSizeForCnRestriction.Value;
			foreach (object obj in ((IEnumerable)cnset))
			{
				GuidGlobCountSet guidGlobCountSet = (GuidGlobCountSet)obj;
				ushort replidFromGuid = replidGuidMap.GetReplidFromGuid(operationContext, guidGlobCountSet.Guid);
				foreach (GlobCountRange globCountRange in guidGlobCountSet.GlobCountSet)
				{
					if (inclusive || globCountRange.HighBound - globCountRange.LowBound > value || countRanges < 20)
					{
						byte[] lowBound;
						byte[] highBound;
						if (flag)
						{
							lowBound = ExchangeIdHelpers.To26ByteArray(replidFromGuid, guidGlobCountSet.Guid, globCountRange.LowBound);
							highBound = ExchangeIdHelpers.To26ByteArray(replidFromGuid, guidGlobCountSet.Guid, globCountRange.HighBound);
						}
						else
						{
							lowBound = ExchangeIdHelpers.To9ByteArray(replidFromGuid, globCountRange.LowBound);
							highBound = ExchangeIdHelpers.To9ByteArray(replidFromGuid, globCountRange.HighBound);
						}
						list.Add(new ContentSynchronizationScopeBase.IdRange(lowBound, highBound));
					}
					else
					{
						residualFilteringRequired = true;
					}
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			list.Sort();
			int num = 0;
			Restriction[] array;
			if (inclusive)
			{
				array = new Restriction[list.Count];
			}
			else
			{
				array = new Restriction[list.Count + 1];
			}
			ContentSynchronizationScopeBase.IdRange idRange = default(ContentSynchronizationScopeBase.IdRange);
			foreach (ContentSynchronizationScopeBase.IdRange idRange2 in list)
			{
				if (inclusive)
				{
					array[num++] = new RestrictionAND(new Restriction[]
					{
						new RestrictionProperty(propTag, RelationOperator.GreaterThanEqual, idRange2.LowBound),
						new RestrictionProperty(propTag, RelationOperator.LessThanEqual, idRange2.HighBound)
					});
				}
				else if (idRange.HighBound == null)
				{
					array[num++] = new RestrictionProperty(propTag, RelationOperator.LessThan, idRange2.LowBound);
				}
				else
				{
					array[num++] = new RestrictionAND(new Restriction[]
					{
						new RestrictionProperty(propTag, RelationOperator.GreaterThan, idRange.HighBound),
						new RestrictionProperty(propTag, RelationOperator.LessThan, idRange2.LowBound)
					});
				}
				idRange = idRange2;
			}
			if (!inclusive)
			{
				array[num++] = new RestrictionProperty(propTag, RelationOperator.GreaterThan, idRange.HighBound);
			}
			return new RestrictionOR(array);
		}

		private MapiFolder folder;

		private FastTransferDownloadContext context;

		private struct IdRange : IComparable<ContentSynchronizationScopeBase.IdRange>
		{
			public IdRange(byte[] lowBound, byte[] highBound)
			{
				this.lowBound = lowBound;
				this.highBound = highBound;
			}

			public byte[] LowBound
			{
				get
				{
					return this.lowBound;
				}
			}

			public byte[] HighBound
			{
				get
				{
					return this.highBound;
				}
			}

			public int CompareTo(ContentSynchronizationScopeBase.IdRange other)
			{
				return ValueHelper.ArraysCompare<byte>(this.lowBound, other.lowBound);
			}

			private byte[] lowBound;

			private byte[] highBound;
		}
	}
}
