using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.FastTransfer;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class IcsContentDownloadContext : IcsDownloadContext
	{
		public ErrorCode Configure(MapiLogon logon, IContentSynchronizationScope scope, FastTransferSendOption sendOptions, SyncFlag syncFlags, SyncExtraFlag extraFlags, StorePropTag[] propertyTags, ExchangeId[] messageIds)
		{
			this.scope = scope;
			this.syncFlags = syncFlags;
			this.extraFlags = extraFlags;
			this.propertyTags = propertyTags;
			this.messageIds = messageIds;
			return base.Configure(logon, sendOptions);
		}

		public SyncFlag SyncFlags
		{
			get
			{
				return this.syncFlags;
			}
		}

		public SyncExtraFlag ExtraFlags
		{
			get
			{
				return this.extraFlags;
			}
		}

		public StorePropTag[] PropertyTags
		{
			get
			{
				return this.propertyTags;
			}
		}

		public IContentSynchronizationScope Scope
		{
			get
			{
				return this.scope;
			}
		}

		public ExchangeId[] MessageIds
		{
			get
			{
				return this.messageIds;
			}
		}

		public override IChunked PrepareIndexes(MapiContext context)
		{
			if (!ConfigurationSchema.ChunkedIndexPopulationEnabled.Value)
			{
				return null;
			}
			if (!this.indexesPrepared)
			{
				this.indexesPrepared = true;
				ContentSynchronizationScopeBase contentSynchronizationScopeBase = this.scope as ContentSynchronizationScopeBase;
				if (contentSynchronizationScopeBase != null)
				{
					return contentSynchronizationScopeBase.PrepareIndexes(context, base.IcsState);
				}
			}
			return null;
		}

		protected override IFastTransferProcessor<FastTransferDownloadContext> GetFastTransferProcessor(MapiContext operationContext)
		{
			IcsContentDownloadContext.ContentsSynchronizer contentsSynchronizer = new IcsContentDownloadContext.ContentsSynchronizer(operationContext, this, this.scope, base.IcsState, this.SyncFlags, this.ExtraFlags);
			IcsContentsSynchronizer.Options options = IcsContentsSynchronizer.Options.None;
			if ((this.ExtraFlags & SyncExtraFlag.Eid) != SyncExtraFlag.None)
			{
				options |= IcsContentsSynchronizer.Options.IncludeMid;
			}
			if ((this.ExtraFlags & SyncExtraFlag.MessageSize) != SyncExtraFlag.None)
			{
				options |= IcsContentsSynchronizer.Options.IncludeMessageSize;
			}
			if ((this.ExtraFlags & SyncExtraFlag.Cn) != SyncExtraFlag.None)
			{
				options |= IcsContentsSynchronizer.Options.IncludeChangeNumber;
			}
			if ((this.ExtraFlags & SyncExtraFlag.ReadCn) != SyncExtraFlag.None)
			{
				options |= IcsContentsSynchronizer.Options.IncludeReadChangeNumber;
			}
			if ((ushort)(this.SyncFlags & SyncFlag.ProgressMode) != 0)
			{
				options |= IcsContentsSynchronizer.Options.ProgressMode;
			}
			if ((byte)(base.SendOptions & FastTransferSendOption.PartialItem) != 0)
			{
				options |= IcsContentsSynchronizer.Options.PartialItem;
			}
			if ((ushort)(this.SyncFlags & SyncFlag.Conversations) != 0)
			{
				options |= IcsContentsSynchronizer.Options.Conversations;
			}
			List<PropertyTag> list = null;
			if ((this.ExtraFlags & SyncExtraFlag.ManifestMode) != SyncExtraFlag.None)
			{
				options |= IcsContentsSynchronizer.Options.ManifestMode;
				if (this.PropertyTags != null && this.PropertyTags.Length != 0)
				{
					list = new List<PropertyTag>(this.PropertyTags.Length);
					foreach (StorePropTag storePropTag in this.PropertyTags)
					{
						list.Add(new PropertyTag(storePropTag.PropTag));
					}
				}
			}
			return new IcsContentsSynchronizer(contentsSynchronizer, options, list);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<IcsContentDownloadContext>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.scope != null)
			{
				this.scope.Dispose();
				this.scope = null;
			}
			base.InternalDispose(calledFromDispose);
		}

		private IContentSynchronizationScope scope;

		private SyncFlag syncFlags;

		private SyncExtraFlag extraFlags;

		private StorePropTag[] propertyTags;

		private ExchangeId[] messageIds;

		private bool indexesPrepared;

		internal class ContentsSynchronizer : DisposableBase, IContentsSynchronizer, IDisposable
		{
			public ContentsSynchronizer(MapiContext operationContext, IcsContentDownloadContext context, IContentSynchronizationScope scope, IcsState state, SyncFlag syncFlags, SyncExtraFlag extraFlags)
			{
				state.ReloadIfNecessary();
				if (ExTraceGlobals.IcsDownloadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("InitialState=[");
					stringBuilder.Append(state.ToString());
					stringBuilder.Append("]");
					ExTraceGlobals.IcsDownloadStateTracer.TraceDebug(0L, stringBuilder.ToString());
				}
				this.context = context;
				this.scope = scope;
				this.state = state;
				this.syncFlags = syncFlags;
				this.extraFlags = extraFlags;
				this.cnsetSeenServer = scope.GetServerCnsetSeen(operationContext, (ushort)(this.syncFlags & SyncFlag.Conversations) != 0);
				if (ExTraceGlobals.IcsDownloadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder2 = new StringBuilder(100);
					stringBuilder2.Append("Server CnsetSeen=[");
					stringBuilder2.Append(this.cnsetSeenServer.ToString());
					stringBuilder2.Append("]");
					ExTraceGlobals.IcsDownloadStateTracer.TraceDebug(0L, stringBuilder2.ToString());
				}
				if ((ushort)(this.syncFlags & SyncFlag.CatchUp) == 0 || ((this.extraFlags & SyncExtraFlag.CatchUpFull) != SyncExtraFlag.None && (ushort)(this.syncFlags & SyncFlag.Conversations) == 0))
				{
					this.changedMessages = scope.GetChangedMessages(operationContext, state);
					if ((ushort)(this.syncFlags & SyncFlag.Conversations) == 0)
					{
						if ((ushort)(this.syncFlags & SyncFlag.NoDeletions) == 0)
						{
							this.idsetDeletes = scope.GetDeletes(operationContext, state);
							if ((ushort)(this.syncFlags & SyncFlag.NoSoftDeletions) == 0)
							{
								this.idsetSoftDeletes = scope.GetSoftDeletes(operationContext, state);
							}
						}
						if ((ushort)(this.syncFlags & SyncFlag.ReadState) != 0)
						{
							scope.GetNewReadsUnreads(operationContext, state, out this.idsetNewReads, out this.idsetNewUnreads, out this.finalCnsetRead);
						}
					}
				}
				if ((ushort)(this.syncFlags & SyncFlag.ProgressMode) != 0)
				{
					int num = 0;
					long num2 = 0L;
					int num3 = 0;
					long num4 = 0L;
					if (this.changedMessages != null)
					{
						foreach (Properties properties in this.changedMessages)
						{
							bool flag = (bool)properties[4].Value;
							uint num5 = (uint)((int)properties[5].Value);
							if (flag)
							{
								num++;
								num2 += (long)((ulong)num5);
							}
							else
							{
								num3++;
								num4 += (long)((ulong)num5);
							}
						}
					}
					this.progressInfo = new ProgressInformation(0, num3, num, (ulong)num4, (ulong)num2);
				}
			}

			public ProgressInformation ProgressInformation
			{
				get
				{
					return this.progressInfo;
				}
			}

			public IEnumerator<IMessageChange> GetChanges()
			{
				if (this.changedMessages != null)
				{
					if (((this.extraFlags & SyncExtraFlag.NoChanges) != SyncExtraFlag.None || (this.extraFlags & SyncExtraFlag.CatchUpFull) != SyncExtraFlag.None) && (ushort)(this.syncFlags & SyncFlag.MessageSelective) == 0)
					{
						if ((ushort)(this.syncFlags & SyncFlag.Normal) != 0 || (ushort)(this.syncFlags & SyncFlag.Conversations) != 0)
						{
							this.state.CnsetSeen.Insert(this.cnsetSeenServer);
						}
						if ((ushort)(this.syncFlags & SyncFlag.Associated) != 0)
						{
							this.state.CnsetSeenAssociated.Insert(this.cnsetSeenServer);
						}
					}
					foreach (Properties messageHeader in this.changedMessages)
					{
						bool skipItem = false;
						if ((this.extraFlags & SyncExtraFlag.NoChanges) == SyncExtraFlag.None && (this.extraFlags & SyncExtraFlag.CatchUpFull) == SyncExtraFlag.None)
						{
							IcsContentDownloadContext.MessageChange messageChange = null;
							bool success = false;
							try
							{
								messageChange = new IcsContentDownloadContext.MessageChange(this.context, this.scope, this.syncFlags, messageHeader);
								if ((ushort)(this.syncFlags & SyncFlag.Conversations) != 0 || (this.extraFlags & SyncExtraFlag.ManifestMode) != SyncExtraFlag.None || messageChange.FastTransferMessage != null)
								{
									success = true;
									yield return messageChange;
								}
								else
								{
									DiagnosticContext.TraceLocation((LID)35936U);
									skipItem = true;
								}
							}
							finally
							{
								if (!success && messageChange != null)
								{
									messageChange.Dispose();
								}
							}
						}
						if (!skipItem)
						{
							Context currentOperationContext = this.context.CurrentOperationContext;
							IReplidGuidMap replidGuidMap = this.context.Logon.StoreMailbox.ReplidGuidMap;
							Properties properties = messageHeader;
							ExchangeId id = ExchangeId.CreateFrom9ByteArray(currentOperationContext, replidGuidMap, (byte[])properties[1].Value);
							if (this.cnsetSeenServer.Contains(id))
							{
								if ((ushort)(this.syncFlags & SyncFlag.Conversations) == 0)
								{
									Properties properties2 = messageHeader;
									if ((bool)properties2[4].Value)
									{
										this.state.CnsetSeenAssociated.Insert(id);
										goto IL_2CB;
									}
								}
								this.state.CnsetSeen.Insert(id);
							}
							IL_2CB:
							if ((ushort)(this.syncFlags & SyncFlag.Conversations) == 0)
							{
								Context currentOperationContext2 = this.context.CurrentOperationContext;
								IReplidGuidMap replidGuidMap2 = this.context.Logon.StoreMailbox.ReplidGuidMap;
								Properties properties3 = messageHeader;
								ExchangeId id2 = ExchangeId.CreateFromInt64(currentOperationContext2, replidGuidMap2, (long)properties3[0].Value);
								this.state.IdsetGiven.Insert(id2);
							}
						}
					}
				}
				if ((ushort)(this.syncFlags & SyncFlag.MessageSelective) == 0)
				{
					if ((ushort)(this.syncFlags & SyncFlag.Normal) != 0 || (ushort)(this.syncFlags & SyncFlag.Conversations) != 0)
					{
						this.state.CnsetSeen.Insert(this.cnsetSeenServer);
						this.state.CnsetSeen.IdealPack();
					}
					if ((ushort)(this.syncFlags & SyncFlag.Associated) != 0)
					{
						this.state.CnsetSeenAssociated.Insert(this.cnsetSeenServer);
						this.state.CnsetSeenAssociated.IdealPack();
					}
				}
				yield break;
			}

			public IPropertyBag GetDeletions()
			{
				MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag(this.context);
				if ((this.extraFlags & SyncExtraFlag.CatchUpFull) == SyncExtraFlag.None)
				{
					if (this.idsetDeletes != null && !this.idsetDeletes.IsEmpty)
					{
						byte[] value = this.idsetDeletes.Serialize(new Func<Guid, ReplId>(this.scope.GuidToReplid));
						memoryPropertyBag.SetProperty(new PropertyValue(PropertyTag.IdsetDeleted, value));
					}
					if (this.idsetSoftDeletes != null && !this.idsetSoftDeletes.IsEmpty)
					{
						byte[] value2 = this.idsetSoftDeletes.Serialize(new Func<Guid, ReplId>(this.scope.GuidToReplid));
						memoryPropertyBag.SetProperty(new PropertyValue(PropertyTag.IdsetSoftDeleted, value2));
					}
				}
				return memoryPropertyBag;
			}

			public IPropertyBag GetReadUnreadStateChanges()
			{
				if (this.idsetDeletes != null)
				{
					this.state.IdsetGiven.Remove(this.idsetDeletes);
				}
				if (this.idsetSoftDeletes != null)
				{
					this.state.IdsetGiven.Remove(this.idsetSoftDeletes);
				}
				MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag(this.context);
				if ((this.extraFlags & SyncExtraFlag.CatchUpFull) == SyncExtraFlag.None)
				{
					if (this.idsetNewReads != null)
					{
						byte[] value = this.idsetNewReads.Serialize(new Func<Guid, ReplId>(this.scope.GuidToReplid));
						memoryPropertyBag.SetProperty(new PropertyValue(PropertyTag.IdsetRead, value));
					}
					if (this.idsetNewUnreads != null)
					{
						byte[] value2 = this.idsetNewUnreads.Serialize(new Func<Guid, ReplId>(this.scope.GuidToReplid));
						memoryPropertyBag.SetProperty(new PropertyValue(PropertyTag.IdsetUnread, value2));
					}
				}
				return memoryPropertyBag;
			}

			public IIcsState GetFinalState()
			{
				if ((ushort)(this.syncFlags & SyncFlag.ReadState) != 0 && (ushort)(this.syncFlags & SyncFlag.Conversations) == 0)
				{
					if (this.finalCnsetRead != null)
					{
						this.state.CnsetRead.Insert(this.finalCnsetRead);
					}
					if (this.state.CnsetRead.Contains(IcsState.NonPerUserIdsetIndicator))
					{
						this.state.CnsetRead.IdealPack();
					}
				}
				if (ExTraceGlobals.IcsDownloadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("FinalState=[");
					stringBuilder.Append(this.state.ToString());
					stringBuilder.Append("]");
					ExTraceGlobals.IcsDownloadStateTracer.TraceDebug(0L, stringBuilder.ToString());
				}
				return this.state;
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<IcsContentDownloadContext.ContentsSynchronizer>(this);
			}

			protected override void InternalDispose(bool isCalledFromDispose)
			{
			}

			private const int MaxNumberOfChangesToKeepInMemory = 100;

			private IcsContentDownloadContext context;

			private IContentSynchronizationScope scope;

			private IcsState state;

			private SyncFlag syncFlags;

			private SyncExtraFlag extraFlags;

			private IdSet cnsetSeenServer;

			private IdSet idsetDeletes;

			private IdSet idsetSoftDeletes;

			private IdSet idsetNewReads;

			private IdSet idsetNewUnreads;

			private IdSet finalCnsetRead;

			private IEnumerable<Properties> changedMessages;

			private ProgressInformation progressInfo;
		}

		internal class MessageChange : DisposableBase, IMessageChange, IMessageChangePartial, IDisposable, IPropertyBag
		{
			public MessageChange(IcsDownloadContext context, IContentSynchronizationScope scope, SyncFlag syncFlags, Properties headerValues)
			{
				this.context = context;
				this.scope = scope;
				this.syncFlags = syncFlags;
				this.headerValues = headerValues;
			}

			public IMessage Message
			{
				get
				{
					return this.FastTransferMessage;
				}
			}

			public int MessageSize
			{
				get
				{
					if ((ushort)(this.syncFlags & SyncFlag.Conversations) == 0)
					{
						return (int)this.headerValues[5].Value;
					}
					return 0;
				}
			}

			public bool IsAssociatedMessage
			{
				get
				{
					return (ushort)(this.syncFlags & SyncFlag.Conversations) == 0 && (bool)this.headerValues[4].Value;
				}
			}

			public IPropertyBag MessageHeaderPropertyBag
			{
				get
				{
					return this;
				}
			}

			public IMessageChangePartial PartialChange
			{
				get
				{
					bool flag = this.CanDoPartialDownload();
					if (ExTraceGlobals.IcsDownloadTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExchangeId mid = this.FastTransferMessage.MapiMessage.Mid;
						ExchangeId fid = this.FastTransferMessage.MapiMessage.GetFid();
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append(flag ? "ICS partial download" : "ICS full download");
						stringBuilder.Append(" - Fid:[");
						stringBuilder.Append(fid.ToString());
						stringBuilder.Append("] Mid=[");
						stringBuilder.Append(mid.ToString());
						if (flag)
						{
							stringBuilder.Append("] ChangedGroups:[");
							stringBuilder.AppendAsString(this.changedGroups);
						}
						stringBuilder.Append("]");
						ExTraceGlobals.IcsDownloadTracer.TraceDebug(0L, stringBuilder.ToString());
					}
					if (!flag)
					{
						return null;
					}
					return this;
				}
			}

			public PropertyGroupMapping PropertyGroupMapping
			{
				get
				{
					return this.scope.GetPropertyGroupMapping();
				}
			}

			public IEnumerable<int> ChangedPropGroups
			{
				get
				{
					return this.changedGroups;
				}
			}

			public IEnumerable<PropertyTag> OtherGroupPropTags
			{
				get
				{
					PropertyGroupMapping propertyGroupMapping = this.scope.GetPropertyGroupMapping();
					return from propertyTag in this.FastTransferMessage.GetPropertyList()
					where !propertyGroupMapping.IsPropertyInAnyGroup(propertyTag) && ContentSynchronizationScope.ValidClientSideGroupProperty((ushort)propertyTag.PropertyId)
					select propertyTag;
				}
			}

			public FastTransferMessage FastTransferMessage
			{
				get
				{
					if (this.message == null)
					{
						ExchangeId exchangeId = this.scope.GetExchangeId((long)this.headerValues[0].Value);
						this.message = this.scope.OpenMessage(exchangeId);
					}
					return this.message;
				}
			}

			private PropertyValue GetProperty(PropertyTag property)
			{
				if (property == PropertyTag.SourceKey && (ushort)(this.syncFlags & SyncFlag.Conversations) == 0)
				{
					object value;
					if (this.headerValues[6].IsError || this.headerValues[6].Value == null || (ushort)(this.syncFlags & SyncFlag.NoForeignKeys) != 0)
					{
						value = ExchangeId.CreateFromInt64(this.context.CurrentOperationContext, this.context.Logon.StoreMailbox.ReplidGuidMap, (long)this.headerValues[0].Value).To22ByteArray();
						return RcaTypeHelpers.MassageOutgoingProperty(new Property(PropTag.Message.SourceKey, value), false);
					}
					value = this.headerValues[6].Value;
					return RcaTypeHelpers.MassageOutgoingProperty(new Property(PropTag.Message.SourceKey, value), false);
				}
				else if (property == PropertyTag.ChangeKey && (ushort)(this.syncFlags & SyncFlag.Conversations) == 0)
				{
					object value;
					if (this.headerValues[7].IsError || this.headerValues[7].Value == null)
					{
						value = ExchangeId.CreateFrom9ByteArray(this.context.CurrentOperationContext, this.context.Logon.StoreMailbox.ReplidGuidMap, (byte[])this.headerValues[1].Value).To22ByteArray();
						return RcaTypeHelpers.MassageOutgoingProperty(new Property(PropTag.Message.ChangeKey, value), false);
					}
					value = this.headerValues[7].Value;
					return RcaTypeHelpers.MassageOutgoingProperty(new Property(PropTag.Message.ChangeKey, value), false);
				}
				else
				{
					if (property == PropertyTag.ChangeNumber)
					{
						object value = ExchangeIdHelpers.Convert9ByteToLong((byte[])this.headerValues[1].Value);
						return RcaTypeHelpers.MassageOutgoingProperty(new Property(PropTag.Message.ChangeNumber, value), false);
					}
					int num = this.FindProperty(property);
					if (num < 0)
					{
						return PropertyValue.Error(property.PropertyId, (ErrorCode)2147746063U);
					}
					return RcaTypeHelpers.MassageOutgoingProperty(this.headerValues[num], true);
				}
			}

			void IPropertyBag.SetProperty(PropertyValue propertyValue)
			{
				throw new System.NotSupportedException();
			}

			private IEnumerable<PropertyTag> GetPropertyList()
			{
				throw new System.NotSupportedException();
			}

			AnnotatedPropertyValue IPropertyBag.GetAnnotatedProperty(PropertyTag propertyTag)
			{
				PropertyValue property = this.GetProperty(propertyTag);
				NamedProperty namedProperty = null;
				if (propertyTag.IsNamedProperty)
				{
					((IPropertyBag)this).Session.TryResolveToNamedProperty(propertyTag, out namedProperty);
				}
				return new AnnotatedPropertyValue(propertyTag, property, namedProperty);
			}

			IEnumerable<AnnotatedPropertyValue> IPropertyBag.GetAnnotatedProperties()
			{
				foreach (PropertyTag propertyTag in this.GetPropertyList())
				{
					yield return ((IPropertyBag)this).GetAnnotatedProperty(propertyTag);
				}
				yield break;
			}

			void IPropertyBag.Delete(PropertyTag property)
			{
				throw new System.NotSupportedException();
			}

			Stream IPropertyBag.GetPropertyStream(PropertyTag property)
			{
				throw new System.NotSupportedException();
			}

			Stream IPropertyBag.SetPropertyStream(PropertyTag property, long dataSizeEstimate)
			{
				throw new System.NotSupportedException();
			}

			ISession IPropertyBag.Session
			{
				get
				{
					return this.context;
				}
			}

			private int FindProperty(PropertyTag property)
			{
				for (int i = 0; i < this.headerValues.Count; i++)
				{
					if (property.PropertyId == (PropertyId)this.headerValues[i].Tag.PropId)
					{
						return i;
					}
				}
				return -1;
			}

			private bool CanDoPartialDownload()
			{
				ExchangeId mid = this.FastTransferMessage.MapiMessage.Mid;
				if (this.IsAssociatedMessage || !this.context.IcsState.IdsetGiven.Contains(mid))
				{
					return false;
				}
				PropGroupChangeInfo propGroupChangeInfo = this.FastTransferMessage.GetPropGroupChangeInfo();
				if (!propGroupChangeInfo.IsValid)
				{
					return false;
				}
				this.changedGroups = new List<int>(4);
				IdSet idSet = this.IsAssociatedMessage ? this.context.IcsState.CnsetSeenAssociated : this.context.IcsState.CnsetSeen;
				for (int i = 0; i < propGroupChangeInfo.Count; i++)
				{
					ExchangeId id = propGroupChangeInfo[i];
					if (!idSet.Contains(id))
					{
						this.changedGroups.Add(i);
					}
				}
				ExchangeId other = propGroupChangeInfo.Other;
				if (!idSet.Contains(other))
				{
					this.changedGroups.Add(-1);
				}
				return true;
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<IcsContentDownloadContext.MessageChange>(this);
			}

			protected override void InternalDispose(bool isCalledFromDispose)
			{
				if (isCalledFromDispose && this.message != null)
				{
					this.message.Dispose();
					this.message = null;
				}
			}

			public const int IndexOfMidColumn = 0;

			public const int IndexOfInternalChangeNumberColumn = 1;

			public const int IndexOfLastModificationTimeColumn = 2;

			public const int IndexOfMessageDeliveryTimeColumn = 3;

			public const int IndexOfAssociatedColumn = 4;

			public const int IndexOfMessageSizeColumn = 5;

			public const int IndexOfInternalSourceKeyColumn = 6;

			public const int IndexOfInternalChangeKeyColumn = 7;

			public const int IndexOfPredecessorChangeListColumn = 8;

			public const int IndexOfChangeTypeColumn = 3;

			public const int IndexOfConversationIdColumn = 4;

			public static StorePropTag[] StandardHeaderColumns = new StorePropTag[]
			{
				PropTag.Message.Mid,
				PropTag.Message.Internal9ByteChangeNumber,
				PropTag.Message.LastModificationTime,
				PropTag.Message.MessageDeliveryTime,
				PropTag.Message.Associated,
				PropTag.Message.MessageSize32,
				PropTag.Message.InternalSourceKey,
				PropTag.Message.InternalChangeKey,
				PropTag.Message.PredecessorChangeList
			};

			public static StorePropTag[] StandardConversationHeaderColumns = new StorePropTag[]
			{
				PropTag.Message.Mid,
				PropTag.Message.Internal9ByteChangeNumber,
				PropTag.Message.LastModificationTime,
				PropTag.Message.ChangeType,
				PropTag.Message.ConversationId
			};

			private IcsDownloadContext context;

			private IContentSynchronizationScope scope;

			private SyncFlag syncFlags;

			private Properties headerValues;

			private FastTransferMessage message;

			private List<int> changedGroups;
		}
	}
}
