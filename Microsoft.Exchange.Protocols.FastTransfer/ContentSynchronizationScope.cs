using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.FastTransfer;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LazyIndexing;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class ContentSynchronizationScope : ContentSynchronizationScopeBase
	{
		public ContentSynchronizationScope(MapiContext operationContext, MapiFolder folder, Restriction restriction, SyncFlag syncFlags, SyncExtraFlag extraFlags, StorePropTag[] propertyTags, FastTransferDownloadContext context) : base(folder, context)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (restriction != null)
				{
					if (restriction.HasClauseMeetingPredicate((Restriction clause) => clause.RefersToProperty(PropTag.Message.BodyUnicode) || clause.RefersToProperty(PropTag.Message.RtfCompressed) || clause.RefersToProperty(PropTag.Message.BodyHtml)))
					{
						throw new StoreException((LID)38112U, ErrorCodeValue.TooComplex);
					}
					restriction = restriction.InspectAndFix(delegate(Restriction res)
					{
						if (res is RestrictionAND)
						{
							return res;
						}
						if (res is RestrictionBitmask)
						{
							RestrictionBitmask restrictionBitmask = (RestrictionBitmask)res;
							if (restrictionBitmask.PropertyTag == PropTag.Message.PropertyGroupChangeMask && restrictionBitmask.Operation == BitmaskOperation.NotEqualToZero)
							{
								this.changedGroupMask = (uint)restrictionBitmask.Mask;
								return new RestrictionTrue();
							}
						}
						return null;
					});
				}
				folder.GhostedFolderCheck(operationContext, (LID)33017U);
				this.restriction = restriction;
				this.syncFlags = syncFlags;
				this.extraFlags = extraFlags;
				this.perUserReadUnreadTracking = (base.Folder.StoreFolder.IsPerUserReadUnreadTrackingEnabled && operationContext.UserIdentity != Guid.Empty);
				this.messageHeaderColumns = IcsContentDownloadContext.MessageChange.StandardHeaderColumns;
				if (this.changedGroupMask != 0U)
				{
					StorePropTag[] array = new StorePropTag[this.messageHeaderColumns.Length + 1];
					Array.Copy(this.messageHeaderColumns, 0, array, 0, this.messageHeaderColumns.Length);
					array[this.messageHeaderColumns.Length] = PropTag.Message.PropGroupInfo;
					this.messageHeaderColumns = array;
				}
				if ((ushort)(this.syncFlags & SyncFlag.CatchUp) == 0 && (this.extraFlags & SyncExtraFlag.CatchUpFull) == SyncExtraFlag.None && (this.extraFlags & SyncExtraFlag.NoChanges) == SyncExtraFlag.None)
				{
					if ((this.extraFlags & SyncExtraFlag.ReadCn) != SyncExtraFlag.None)
					{
						StorePropTag[] array2 = new StorePropTag[this.messageHeaderColumns.Length + 1];
						Array.Copy(this.messageHeaderColumns, 0, array2, 0, this.messageHeaderColumns.Length);
						array2[this.messageHeaderColumns.Length] = PropTag.Message.ReadCnNew;
						this.messageHeaderColumns = array2;
					}
					this.excludeSpecifiedProperties = true;
					if (propertyTags != null)
					{
						if ((this.extraFlags & SyncExtraFlag.ManifestMode) != SyncExtraFlag.None)
						{
							HashSet<StorePropTag> hashSet = new HashSet<StorePropTag>(propertyTags);
							hashSet.ExceptWith(this.messageHeaderColumns);
							hashSet.Remove(PropTag.Message.SourceKey);
							hashSet.Remove(PropTag.Message.ChangeKey);
							hashSet.Remove(PropTag.Message.ChangeNumber);
							StorePropTag[] array3 = new StorePropTag[this.messageHeaderColumns.Length + hashSet.Count];
							Array.Copy(this.messageHeaderColumns, 0, array3, 0, this.messageHeaderColumns.Length);
							hashSet.CopyTo(array3, this.messageHeaderColumns.Length);
							this.messageHeaderColumns = array3;
						}
						else
						{
							this.excludeSpecifiedProperties = (0 == (ushort)(syncFlags & SyncFlag.OnlySpecifiedProps));
							this.specifiedProperties = new HashSet<StorePropTag>(propertyTags);
							this.excludeAttachments = !this.excludeSpecifiedProperties;
							this.excludeRecipients = !this.excludeSpecifiedProperties;
							if (this.specifiedProperties.Contains(PropTag.Message.MessageAttachments))
							{
								this.excludeAttachments = this.excludeSpecifiedProperties;
								this.specifiedProperties.Remove(PropTag.Message.MessageAttachments);
							}
							if (this.specifiedProperties.Contains(PropTag.Message.MessageRecipients))
							{
								this.excludeRecipients = this.excludeSpecifiedProperties;
								this.specifiedProperties.Remove(PropTag.Message.MessageRecipients);
							}
							if (this.excludeSpecifiedProperties)
							{
								this.specifiedProperties.Remove(PropTag.Message.BodyUnicode);
							}
						}
					}
				}
				disposeGuard.Success();
			}
		}

		internal static bool ValidClientSideGroupProperty(ushort propId)
		{
			if (26112 <= propId && propId <= 26623)
			{
				return false;
			}
			if (propId <= 4290)
			{
				if (propId <= 4084)
				{
					switch (propId)
					{
					case 3586:
					case 3587:
					case 3588:
					case 3592:
					case 3593:
						break;
					case 3589:
					case 3590:
					case 3591:
						return true;
					default:
						if (propId != 3611 && propId != 4084)
						{
							return true;
						}
						break;
					}
				}
				else
				{
					switch (propId)
					{
					case 4089:
					case 4090:
					case 4091:
					case 4094:
					case 4095:
						break;
					case 4092:
					case 4093:
						return true;
					default:
						if (propId != 4150)
						{
							switch (propId)
							{
							case 4288:
							case 4289:
							case 4290:
								break;
							default:
								return true;
							}
						}
						break;
					}
				}
			}
			else if (propId <= 26377)
			{
				if (propId != 25840 && propId != 26255 && propId != 26377)
				{
					return true;
				}
			}
			else if (propId <= 26442)
			{
				if (propId != 26380)
				{
					switch (propId)
					{
					case 26438:
					case 26439:
					case 26442:
						break;
					case 26440:
					case 26441:
						return true;
					default:
						return true;
					}
				}
			}
			else
			{
				switch (propId)
				{
				case 26476:
				case 26477:
					break;
				default:
					if (propId != 26538)
					{
						return true;
					}
					break;
				}
			}
			return false;
		}

		public override IEnumerable<Properties> GetChangedMessages(MapiContext operationContext, IcsState icsState)
		{
			this.PrepareViews(operationContext, icsState);
			IEnumerable<Properties> enumerable = this.GetChangedMessagesImpl(icsState);
			bool flag = (this.extraFlags & SyncExtraFlag.CatchUpFull) == SyncExtraFlag.None && (this.extraFlags & SyncExtraFlag.OrderByDeliveryTime) != SyncExtraFlag.None;
			if (flag && enumerable != null)
			{
				List<Properties> list = enumerable as List<Properties>;
				if (list == null)
				{
					list = new List<Properties>(enumerable);
					enumerable = list;
				}
				list.Sort(delegate(Properties x, Properties y)
				{
					DateTime t = (!x[3].IsError) ? ((DateTime)x[3].Value) : ((DateTime)x[2].Value);
					DateTime t2 = (!y[3].IsError) ? ((DateTime)y[3].Value) : ((DateTime)y[2].Value);
					return DateTime.Compare(t2, t);
				});
			}
			return enumerable;
		}

		public override IdSet GetDeletes(MapiContext operationContext, IcsState icsState)
		{
			IdSet midsetDeleted = base.Folder.StoreFolder.GetMidsetDeleted(operationContext);
			if (ExTraceGlobals.IcsDownloadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("FolderMidsetDeleted=[");
				stringBuilder.Append(midsetDeleted.ToString());
				stringBuilder.Append("]");
				ExTraceGlobals.IcsDownloadStateTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			IdSet idSet = IdSet.Intersect(midsetDeleted, icsState.IdsetGiven);
			if (ExTraceGlobals.IcsDownloadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder2 = new StringBuilder(100);
				stringBuilder2.Append("NewDeletes=[");
				stringBuilder2.Append(idSet.ToString());
				stringBuilder2.Append("]");
				ExTraceGlobals.IcsDownloadStateTracer.TraceDebug(0L, stringBuilder2.ToString());
			}
			return idSet;
		}

		public override IdSet GetSoftDeletes(MapiContext operationContext, IcsState icsState)
		{
			this.PrepareViews(operationContext, icsState);
			IdSet idSet = null;
			this.GetSoftDeletesImpl(operationContext, icsState, ref idSet);
			if (idSet != null && ExTraceGlobals.IcsDownloadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("NewSoftDeletes=[");
				stringBuilder.Append(idSet.ToString());
				stringBuilder.Append("]");
				ExTraceGlobals.IcsDownloadStateTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			return idSet;
		}

		public override void GetNewReadsUnreads(MapiContext operationContext, IcsState icsState, out IdSet midsetNewReads, out IdSet midsetNewUnreads, out IdSet finalCnsetRead)
		{
			this.PrepareViews(operationContext, icsState);
			this.GetNewReadsUnreadsImpl(operationContext, icsState, out midsetNewReads, out midsetNewUnreads, out finalCnsetRead);
			if (ExTraceGlobals.IcsDownloadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (midsetNewReads != null)
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("NewReads=[");
					stringBuilder.Append(midsetNewReads.ToString());
					stringBuilder.Append("]");
					ExTraceGlobals.IcsDownloadStateTracer.TraceDebug(0L, stringBuilder.ToString());
				}
				if (midsetNewUnreads != null)
				{
					StringBuilder stringBuilder2 = new StringBuilder(100);
					stringBuilder2.Append("NewUnreads=[");
					stringBuilder2.Append(midsetNewUnreads.ToString());
					stringBuilder2.Append("]");
					ExTraceGlobals.IcsDownloadStateTracer.TraceDebug(0L, stringBuilder2.ToString());
				}
			}
		}

		public override FastTransferMessage OpenMessage(ExchangeId mid)
		{
			IcsContentDownloadContext icsContentDownloadContext = base.DownloadContext as IcsContentDownloadContext;
			MapiMessage mapiMessage = null;
			FastTransferMessage result;
			try
			{
				mapiMessage = new MapiMessage();
				ErrorCode errorCode = mapiMessage.ConfigureMessage(base.CurrentOperationContext, base.Logon, base.Folder.Fid, mid, MessageConfigureFlags.None, base.Logon.CodePage);
				if (errorCode == ErrorCode.NoError)
				{
					FastTransferCopyFlag fastTransferCopyFlag = FastTransferCopyFlag.None;
					if ((byte)(base.DownloadContext.SendOptions & (FastTransferSendOption.Unicode | FastTransferSendOption.UseCpId | FastTransferSendOption.ForceUnicode)) != 0)
					{
						fastTransferCopyFlag |= (FastTransferCopyFlag)2147483648U;
					}
					HashSet<StorePropTag> hashSet = this.specifiedProperties;
					bool excludeProps = this.excludeSpecifiedProperties;
					bool flag = this.excludeAttachments;
					bool flag2 = this.excludeRecipients;
					if (hashSet != null && (mapiMessage.IsEmbedded || (mapiMessage.GetAssociated(base.CurrentOperationContext) && (ushort)(icsContentDownloadContext.SyncFlags & SyncFlag.IgnoreSpecifiedOnAssociated) != 0)))
					{
						hashSet = null;
						excludeProps = true;
						flag = false;
						flag2 = false;
					}
					FastTransferMessage fastTransferMessage = new FastTransferMessage(base.DownloadContext, mapiMessage, excludeProps, hashSet, flag, flag2, false, fastTransferCopyFlag);
					mapiMessage = null;
					result = fastTransferMessage;
				}
				else
				{
					if (errorCode != ErrorCodeValue.NotFound)
					{
						throw new StoreException((LID)58424U, errorCode);
					}
					result = null;
				}
			}
			catch (ExExceptionAccessDenied exception)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
				DiagnosticContext.TraceLocation((LID)60512U);
				result = null;
			}
			finally
			{
				if (mapiMessage != null)
				{
					mapiMessage.Dispose();
				}
			}
			return result;
		}

		public override PropertyGroupMapping GetPropertyGroupMapping()
		{
			if (this.propertyGroupMapping == null)
			{
				PropGroupMapping propGroupMapping = new PropGroupMapping(base.CurrentOperationContext, base.Logon.StoreMailbox);
				List<IList<AnnotatedPropertyTag>> list = new List<IList<AnnotatedPropertyTag>>(propGroupMapping.Count);
				for (int i = 0; i < propGroupMapping.Count; i++)
				{
					List<AnnotatedPropertyTag> list2 = new List<AnnotatedPropertyTag>(propGroupMapping[i].Length);
					foreach (StorePropTag storePropTag in propGroupMapping[i])
					{
						if (ContentSynchronizationScope.ValidClientSideGroupProperty(storePropTag.PropId) && storePropTag.PropType != PropertyType.SvrEid)
						{
							NamedProperty namedProperty = null;
							if (storePropTag.IsNamedProperty)
							{
								StorePropName propName = storePropTag.PropName;
								namedProperty = ((propName.Name != null) ? new NamedProperty(propName.Guid, propName.Name) : new NamedProperty(propName.Guid, propName.DispId));
							}
							list2.Add(new AnnotatedPropertyTag(new PropertyTag(storePropTag.PropTag), namedProperty));
						}
					}
					if (list2.Count == 0)
					{
						list2.Add(new AnnotatedPropertyTag(new PropertyTag(1U), null));
					}
					list.Add(list2);
				}
				this.propertyGroupMapping = new PropertyGroupMapping(propGroupMapping.MappingId, list);
			}
			return this.propertyGroupMapping;
		}

		public override IChunked PrepareIndexes(MapiContext operationContext, IcsState icsState)
		{
			long num = base.Folder.StoreFolder.GetMessageCount(operationContext) + base.Folder.StoreFolder.GetHiddenItemCount(operationContext);
			if (LogicalIndex.IndexUseCallbackTestHook != null || num > (long)ConfigurationSchema.ChunkedIndexPopulationFolderSizeThreshold.Value)
			{
				this.PrepareViews(operationContext, icsState);
				List<Func<Context, IChunked>> list = new List<Func<Context, IChunked>>(3);
				if (this.normalMessageChangesView != null)
				{
					list.Add((Context oc) => this.normalMessageChangesView.PrepareIndexes((MapiContext)oc, null));
				}
				if (this.faiMessageChangesView != null)
				{
					list.Add((Context oc) => this.faiMessageChangesView.PrepareIndexes((MapiContext)oc, null));
				}
				if (this.readUnreadView != null)
				{
					list.Add((Context oc) => this.readUnreadView.PrepareIndexes((MapiContext)oc, null));
				}
				if (list.Count != 0)
				{
					if (list.Count == 1)
					{
						return list[0](operationContext);
					}
					return new CompositeChunked(list);
				}
			}
			return null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ContentSynchronizationScope>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.normalMessageChangesView != null)
				{
					this.normalMessageChangesView.Dispose();
					this.normalMessageChangesView = null;
				}
				if (this.faiMessageChangesView != null)
				{
					this.faiMessageChangesView.Dispose();
					this.faiMessageChangesView = null;
				}
				if (this.normalSoftDeletesView != null)
				{
					this.normalSoftDeletesView.Dispose();
					this.normalSoftDeletesView = null;
				}
				if (this.faiSoftDeletesView != null)
				{
					this.faiSoftDeletesView.Dispose();
					this.faiSoftDeletesView = null;
				}
				if (this.readUnreadView != null)
				{
					this.readUnreadView.Dispose();
					this.readUnreadView = null;
				}
			}
		}

		private IEnumerable<Properties> GetChangedMessagesImpl(IcsState icsState)
		{
			int? indexOfReadCnNewToZeroOut = null;
			if (this.perUserReadUnreadTracking || base.Folder.StoreFolder.Mailbox.IsPublicFolderMailbox)
			{
				for (int j = this.messageHeaderColumns.Length - 1; j >= 0; j--)
				{
					if (this.messageHeaderColumns[j] == PropTag.Message.ReadCnNew)
					{
						indexOfReadCnNewToZeroOut = new int?(j);
						break;
					}
				}
			}
			int indexOfPropGroupInfo = -1;
			if (this.changedGroupMask != 0U)
			{
				for (int k = this.messageHeaderColumns.Length - 1; k >= 0; k--)
				{
					if (this.messageHeaderColumns[k] == PropTag.Message.PropGroupInfo)
					{
						indexOfPropGroupInfo = k;
						break;
					}
				}
			}
			IList<Properties> rows = null;
			int batchSize = ConfigurationSchema.GetChangedMessagesBatchSize.Value;
			foreach (MapiViewMessage messageView in new MapiViewMessage[]
			{
				this.normalMessageChangesView,
				this.faiMessageChangesView
			})
			{
				if (messageView != null)
				{
					bool soughtLessThanRowCount;
					int rowCountActuallySought;
					bool bookmarkPositionChanged;
					messageView.SeekRow(base.CurrentOperationContext, ViewSeekOrigin.Beginning, null, 0, false, out soughtLessThanRowCount, out rowCountActuallySought, false, out bookmarkPositionChanged);
					do
					{
						rows = messageView.QueryRowsBatch(base.CurrentOperationContext, batchSize, QueryRowsFlags.None);
						if (rows != null)
						{
							int i = 0;
							while (i < rows.Count)
							{
								Properties row = rows[i];
								if (!this.residualFilteringForChangesAndSoftDeletesRequired)
								{
									goto IL_25B;
								}
								ExchangeId id = ExchangeId.CreateFrom9ByteArray(base.CurrentOperationContext, base.Logon.StoreMailbox.ReplidGuidMap, (byte[])row[1].Value);
								if (!this.cnsetSeenAll.Contains(id))
								{
									goto IL_25B;
								}
								IL_33D:
								i++;
								continue;
								IL_25B:
								if (this.changedGroupMask != 0U)
								{
									byte[] value = row[indexOfPropGroupInfo].IsError ? null : ((byte[])row[indexOfPropGroupInfo].Value);
									uint num = PropGroupChangeInfo.Deserialize(base.CurrentOperationContext, base.Logon.StoreMailbox.ReplidGuidMap, value).ComputeChangeMask(this.cnsetSeenAll);
									if ((num & this.changedGroupMask) == 0U)
									{
										goto IL_33D;
									}
								}
								if (indexOfReadCnNewToZeroOut != null)
								{
									row[indexOfReadCnNewToZeroOut.Value] = ContentSynchronizationScope.zeroReadCnNew;
								}
								yield return row;
								goto IL_33D;
							}
						}
					}
					while (rows != null);
				}
			}
			yield break;
		}

		private void GetSoftDeletesImpl(MapiContext operationContext, IcsState icsState, ref IdSet midsetSoftDeletes)
		{
			foreach (MapiViewMessage mapiViewMessage in new MapiViewMessage[]
			{
				this.normalSoftDeletesView,
				this.faiSoftDeletesView
			})
			{
				if (mapiViewMessage != null)
				{
					using (MapiViewTableBase.RowReader rowReader = mapiViewMessage.QueryRows(operationContext, QueryRowsFlags.None))
					{
						Properties properties;
						while (rowReader.ReadNext(operationContext, out properties))
						{
							if (this.residualFilteringForChangesAndSoftDeletesRequired)
							{
								ExchangeId id = ExchangeId.CreateFrom9ByteArray(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, (byte[])properties[1].Value);
								if (this.cnsetSeenAll.Contains(id))
								{
									continue;
								}
							}
							ExchangeId id2 = ExchangeId.CreateFromInt64(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, (long)properties[0].Value);
							if (icsState.IdsetGiven.Contains(id2))
							{
								if (midsetSoftDeletes == null)
								{
									midsetSoftDeletes = new IdSet();
								}
								midsetSoftDeletes.Insert(id2);
							}
						}
					}
				}
			}
		}

		private void GetNewReadsUnreadsImpl(MapiContext operationContext, IcsState icsState, out IdSet midsetNewReads, out IdSet midsetNewUnreads, out IdSet finalCnsetRead)
		{
			midsetNewReads = null;
			midsetNewUnreads = null;
			finalCnsetRead = null;
			if (this.readUnreadView != null)
			{
				if (!this.perUserReadUnreadTracking)
				{
					finalCnsetRead = icsState.CnsetRead.Clone();
				}
				using (MapiViewTableBase.RowReader rowReader = this.readUnreadView.QueryRows(operationContext, QueryRowsFlags.None))
				{
					Properties properties;
					while (rowReader.ReadNext(operationContext, out properties))
					{
						if (this.perUserReadUnreadTracking)
						{
							ExchangeId id = ExchangeId.CreateFrom9ByteArray(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, (byte[])properties[1].Value);
							ExchangeId id2 = ExchangeId.CreateFromInt64(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, (long)properties[0].Value);
							if (icsState.IdsetGiven.Contains(id2))
							{
								if (this.cnsetNewReads.Contains(id))
								{
									if (midsetNewReads == null)
									{
										midsetNewReads = new IdSet();
									}
									midsetNewReads.Insert(id2);
								}
								if (this.cnsetNewUnreads.Contains(id))
								{
									if (midsetNewUnreads == null)
									{
										midsetNewUnreads = new IdSet();
									}
									midsetNewUnreads.Insert(id2);
								}
							}
						}
						else
						{
							ExchangeId id3 = ExchangeId.CreateFrom9ByteArray(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, (byte[])properties[2].Value);
							if (!this.residualFilteringForReadUnreadRequired || !icsState.CnsetRead.Contains(id3))
							{
								ExchangeId id4 = ExchangeId.CreateFrom9ByteArray(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, (byte[])properties[1].Value);
								ExchangeId id5 = ExchangeId.CreateFromInt64(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, (long)properties[0].Value);
								bool flag = (bool)properties[3].Value;
								finalCnsetRead.Insert(id3);
								if (this.cnsetSeenAll.Contains(id4) && icsState.IdsetGiven.Contains(id5))
								{
									if (flag)
									{
										if (midsetNewReads == null)
										{
											midsetNewReads = new IdSet();
										}
										midsetNewReads.Insert(id5);
									}
									else
									{
										if (midsetNewUnreads == null)
										{
											midsetNewUnreads = new IdSet();
										}
										midsetNewUnreads.Insert(id5);
									}
								}
							}
						}
					}
				}
				if (this.perUserReadUnreadTracking)
				{
					finalCnsetRead = this.cnsetReadCurrent;
					finalCnsetRead.Insert(IcsState.PerUserIdsetIndicator);
					return;
				}
				finalCnsetRead.Insert(IcsState.NonPerUserIdsetIndicator);
			}
		}

		private void PrepareViews(MapiContext operationContext, IcsState icsState)
		{
			if (!this.viewsPrepared)
			{
				this.cnsetSeenAll = IdSet.Union(icsState.CnsetSeen, icsState.CnsetSeenAssociated);
				IdSet idSet = new IdSet();
				Guid localIdGuid = base.Logon.StoreMailbox.GetLocalIdGuid(operationContext);
				idSet.Insert(localIdGuid, 281474976710655UL);
				idSet.IdealPack();
				IdSet serverCnsetSeen = base.GetServerCnsetSeen(operationContext, false);
				IdSet idSet2 = IdSet.Subtract(IdSet.Intersect(this.cnsetSeenAll, idSet), serverCnsetSeen);
				if (!idSet2.IsEmpty)
				{
					if (ExTraceGlobals.IcsDownloadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("Fixing corrupted CnsetSeen from the client, removing out-of-scope CNs:[");
						stringBuilder.Append(idSet2.ToString());
						stringBuilder.Append("]");
						ExTraceGlobals.IcsDownloadStateTracer.TraceDebug(0L, stringBuilder.ToString());
					}
					this.cnsetSeenAll.Remove(idSet2);
					icsState.CnsetSeen.Remove(idSet2);
					icsState.CnsetSeenAssociated.Remove(idSet2);
				}
				SortOrder[] array = new SortOrder[]
				{
					new SortOrder(new PropertyTag(PropTag.Message.Internal9ByteChangeNumber.PropTag), SortOrderFlags.Ascending)
				};
				Restriction restriction = ContentSynchronizationScopeBase.CreateCnsetSeenRestriction(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, PropTag.Message.Internal9ByteChangeNumber, this.cnsetSeenAll, false, out this.residualFilteringForChangesAndSoftDeletesRequired);
				Restriction restriction2 = restriction;
				if (restriction2 != null && this.restriction != null)
				{
					restriction2 = new RestrictionAND(new Restriction[]
					{
						restriction2,
						this.restriction
					});
				}
				else if (this.restriction != null)
				{
					restriction2 = this.restriction;
				}
				if ((ushort)(this.syncFlags & SyncFlag.Normal) != 0)
				{
					this.normalMessageChangesView = new MapiViewMessage();
					ViewMessageConfigureFlags flags = ViewMessageConfigureFlags.NoNotifications | ViewMessageConfigureFlags.UseCoveringIndex;
					this.normalMessageChangesView.Configure(base.CurrentOperationContext, base.Logon, base.Folder, flags);
					this.normalMessageChangesView.SetColumns(base.CurrentOperationContext, this.messageHeaderColumns, MapiViewSetColumnsFlag.NoColumnValidation);
					this.normalMessageChangesView.Sort(base.CurrentOperationContext, array, SortTableFlags.None);
					this.normalMessageChangesView.Restrict(base.CurrentOperationContext, 0, restriction2);
				}
				if ((ushort)(this.syncFlags & SyncFlag.Associated) != 0)
				{
					this.faiMessageChangesView = new MapiViewMessage();
					ViewMessageConfigureFlags flags2 = ViewMessageConfigureFlags.ViewFAI | ViewMessageConfigureFlags.NoNotifications | ViewMessageConfigureFlags.UseCoveringIndex;
					this.faiMessageChangesView.Configure(base.CurrentOperationContext, base.Logon, base.Folder, flags2);
					this.faiMessageChangesView.SetColumns(base.CurrentOperationContext, this.messageHeaderColumns, MapiViewSetColumnsFlag.NoColumnValidation);
					this.faiMessageChangesView.Sort(base.CurrentOperationContext, array, SortTableFlags.None);
					this.faiMessageChangesView.Restrict(base.CurrentOperationContext, 0, restriction2);
				}
				if ((ushort)(this.syncFlags & SyncFlag.NoSoftDeletions) == 0 && this.restriction != null && icsState.IdsetGiven != null && !icsState.IdsetGiven.IsEmpty)
				{
					StorePropTag[] softDeletesViewColumns = ContentSynchronizationScope.SoftDeletesViewColumns;
					restriction2 = restriction;
					if (restriction2 != null)
					{
						restriction2 = new RestrictionAND(new Restriction[]
						{
							restriction2,
							new RestrictionNOT(this.restriction)
						});
					}
					else
					{
						restriction2 = new RestrictionNOT(this.restriction);
					}
					if ((ushort)(this.syncFlags & SyncFlag.Normal) != 0)
					{
						this.normalSoftDeletesView = new MapiViewMessage();
						ViewMessageConfigureFlags flags3 = ViewMessageConfigureFlags.NoNotifications | ViewMessageConfigureFlags.UseCoveringIndex;
						this.normalSoftDeletesView.Configure(operationContext, base.Logon, base.Folder, flags3);
						this.normalSoftDeletesView.SetColumns(operationContext, softDeletesViewColumns, MapiViewSetColumnsFlag.NoColumnValidation);
						this.normalSoftDeletesView.Sort(operationContext, array, SortTableFlags.None);
						this.normalSoftDeletesView.Restrict(operationContext, 0, restriction2);
					}
					if ((ushort)(this.syncFlags & SyncFlag.Associated) != 0)
					{
						this.faiSoftDeletesView = new MapiViewMessage();
						ViewMessageConfigureFlags flags4 = ViewMessageConfigureFlags.ViewFAI | ViewMessageConfigureFlags.NoNotifications | ViewMessageConfigureFlags.UseCoveringIndex;
						this.faiSoftDeletesView.Configure(operationContext, base.Logon, base.Folder, flags4);
						this.faiSoftDeletesView.SetColumns(operationContext, softDeletesViewColumns, MapiViewSetColumnsFlag.NoColumnValidation);
						this.faiSoftDeletesView.Sort(operationContext, array, SortTableFlags.None);
						this.faiSoftDeletesView.Restrict(operationContext, 0, restriction2);
					}
				}
				if ((ushort)(this.syncFlags & SyncFlag.ReadState) != 0)
				{
					StorePropTag[] columns;
					SortOrder[] legacySortOrder;
					if (this.perUserReadUnreadTracking)
					{
						columns = ContentSynchronizationScope.ReadsViewColumnsPerUser;
						legacySortOrder = array;
						PerUser perUser = PerUser.LoadResident(operationContext, base.Folder.Logon.StoreMailbox, operationContext.UserIdentity, base.Folder.Fid);
						if (perUser != null)
						{
							using (LockManager.Lock(perUser, LockManager.LockType.PerUserShared, operationContext.Diagnostics))
							{
								this.cnsetReadCurrent = perUser.CNSet.Clone();
								goto IL_44E;
							}
						}
						this.cnsetReadCurrent = new IdSet();
						IL_44E:
						this.cnsetReadPrevious = icsState.CnsetRead;
						if (!this.cnsetReadPrevious.Remove(IcsState.PerUserIdsetIndicator))
						{
							this.cnsetReadPrevious = IdSet.Subtract(this.cnsetSeenAll, this.cnsetReadCurrent);
						}
						this.cnsetNewReads = IdSet.Subtract(IdSet.Intersect(this.cnsetSeenAll, this.cnsetReadCurrent), this.cnsetReadPrevious);
						this.cnsetNewUnreads = IdSet.Subtract(IdSet.Intersect(this.cnsetSeenAll, this.cnsetReadPrevious), this.cnsetReadCurrent);
						bool flag;
						restriction2 = ContentSynchronizationScopeBase.CreateCnsetSeenRestriction(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, PropTag.Message.Internal9ByteChangeNumber, IdSet.Union(this.cnsetNewReads, this.cnsetNewUnreads), true, out flag);
						this.residualFilteringForReadUnreadRequired = false;
					}
					else
					{
						columns = ContentSynchronizationScope.ReadsViewColumns;
						legacySortOrder = new SortOrder[]
						{
							new SortOrder(new PropertyTag(PropTag.Message.Internal9ByteReadCnNew.PropTag), SortOrderFlags.Ascending)
						};
						this.cnsetReadPrevious = icsState.CnsetRead;
						if (!this.cnsetReadPrevious.Remove(IcsState.NonPerUserIdsetIndicator))
						{
							this.cnsetReadPrevious = new IdSet();
						}
						restriction2 = ContentSynchronizationScopeBase.CreateCnsetSeenRestriction(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, PropTag.Message.Internal9ByteReadCnNew, this.cnsetReadPrevious, false, out this.residualFilteringForReadUnreadRequired);
					}
					if (restriction2 != null && this.restriction != null)
					{
						restriction2 = new RestrictionAND(new Restriction[]
						{
							restriction2,
							this.restriction
						});
					}
					else if (this.restriction != null)
					{
						restriction2 = this.restriction;
					}
					this.readUnreadView = new MapiViewMessage();
					ViewMessageConfigureFlags flags5 = ViewMessageConfigureFlags.NoNotifications | ViewMessageConfigureFlags.UseCoveringIndex;
					this.readUnreadView.Configure(operationContext, base.Logon, base.Folder, flags5);
					this.readUnreadView.SetColumns(operationContext, columns, MapiViewSetColumnsFlag.NoColumnValidation);
					this.readUnreadView.Sort(operationContext, legacySortOrder, SortTableFlags.None);
					this.readUnreadView.Restrict(operationContext, 0, restriction2);
				}
				this.viewsPrepared = true;
			}
		}

		private const int ReadsViewIndexOfMidColumn = 0;

		private const int ReadsViewIndexOfInternalChangeNumberColumn = 1;

		private const int ReadsViewIndexOfInternalReadCnNewColumn = 2;

		private const int ReadsViewIndexOfReadColumn = 3;

		private const int SoftDeletesViewIndexOfMidColumn = 0;

		private const int SoftDeletesViewIndexOfInternalChangeNumberColumn = 1;

		private const int SoftDeletesViewIndexOfLastModificationTimeColumn = 2;

		public static StorePropTag[] ReadsViewColumns = new StorePropTag[]
		{
			PropTag.Message.Mid,
			PropTag.Message.Internal9ByteChangeNumber,
			PropTag.Message.Internal9ByteReadCnNew,
			PropTag.Message.Read
		};

		public static StorePropTag[] ReadsViewColumnsPerUser = new StorePropTag[]
		{
			PropTag.Message.Mid,
			PropTag.Message.Internal9ByteChangeNumber
		};

		public static StorePropTag[] SoftDeletesViewColumns = new StorePropTag[]
		{
			PropTag.Message.Mid,
			PropTag.Message.Internal9ByteChangeNumber,
			PropTag.Message.LastModificationTime
		};

		private static Property zeroReadCnNew = new Property(PropTag.Message.ReadCnNew, 0L);

		private readonly bool perUserReadUnreadTracking;

		private readonly bool excludeSpecifiedProperties;

		private readonly bool excludeAttachments;

		private readonly bool excludeRecipients;

		private uint changedGroupMask;

		private Restriction restriction;

		private PropertyGroupMapping propertyGroupMapping;

		private SyncFlag syncFlags;

		private SyncExtraFlag extraFlags;

		private StorePropTag[] messageHeaderColumns;

		private HashSet<StorePropTag> specifiedProperties;

		private bool viewsPrepared;

		private MapiViewMessage normalMessageChangesView;

		private MapiViewMessage faiMessageChangesView;

		private MapiViewMessage normalSoftDeletesView;

		private MapiViewMessage faiSoftDeletesView;

		private MapiViewMessage readUnreadView;

		private IdSet cnsetSeenAll;

		private bool residualFilteringForChangesAndSoftDeletesRequired;

		private bool residualFilteringForReadUnreadRequired;

		private IdSet cnsetReadPrevious;

		private IdSet cnsetReadCurrent;

		private IdSet cnsetNewReads;

		private IdSet cnsetNewUnreads;
	}
}
