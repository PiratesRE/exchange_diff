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
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class IcsHierarchyDownloadContext : IcsDownloadContext
	{
		public ErrorCode Configure(MapiLogon logon, IHierarchySynchronizationScope scope, FastTransferSendOption sendOptions, SyncFlag syncFlags, SyncExtraFlag extraFlags, StorePropTag[] propertyTags)
		{
			ErrorCode errorCode = base.Configure(logon, sendOptions);
			if (errorCode == ErrorCode.NoError)
			{
				this.scope = scope;
				this.syncFlags = syncFlags;
				this.extraFlags = extraFlags;
				if ((ushort)(syncFlags & SyncFlag.OnlySpecifiedProps) != 0)
				{
					if (propertyTags == null || propertyTags.Length == 0)
					{
						propertyTags = IcsHierarchyDownloadContext.defaultFolderProperties;
					}
					else
					{
						HashSet<StorePropTag> hashSet = new HashSet<StorePropTag>(propertyTags);
						hashSet.UnionWith(IcsHierarchyDownloadContext.defaultFolderProperties);
						propertyTags = new StorePropTag[hashSet.Count];
						hashSet.CopyTo(propertyTags);
					}
				}
				this.propertyTags = propertyTags;
				if (this.propertyTags != null)
				{
					int num = Array.FindIndex<StorePropTag>(this.propertyTags, (StorePropTag ptag) => ptag == PropTag.Folder.AclTableAndSecurityDescriptor);
					if (num != -1)
					{
						this.propertyTags[num] = PropTag.Folder.NTSecurityDescriptor;
					}
				}
			}
			return errorCode;
		}

		protected override IFastTransferProcessor<FastTransferDownloadContext> GetFastTransferProcessor(MapiContext operationContext)
		{
			IcsHierarchyDownloadContext.HierarchySynchronizer hierarchySynchronizer = new IcsHierarchyDownloadContext.HierarchySynchronizer(operationContext, this, this.scope, base.IcsState, this.syncFlags, this.extraFlags, this.propertyTags);
			IcsHierarchySynchronizer.Options options = IcsHierarchySynchronizer.Options.None;
			if ((this.extraFlags & SyncExtraFlag.Eid) != SyncExtraFlag.None)
			{
				options |= IcsHierarchySynchronizer.Options.IncludeFid;
			}
			if ((this.extraFlags & SyncExtraFlag.Cn) != SyncExtraFlag.None)
			{
				options |= IcsHierarchySynchronizer.Options.IncludeChangeNumber;
			}
			return new IcsHierarchySynchronizer(hierarchySynchronizer, options);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<IcsHierarchyDownloadContext>(this);
		}

		private static StorePropTag[] defaultFolderProperties = new StorePropTag[]
		{
			PropTag.Folder.ParentSourceKey,
			PropTag.Folder.SourceKey,
			PropTag.Folder.LastModificationTime,
			PropTag.Folder.ChangeKey,
			PropTag.Folder.PredecessorChangeList,
			PropTag.Folder.DisplayName,
			PropTag.Folder.Fid,
			PropTag.Folder.ChangeNumber
		};

		private IHierarchySynchronizationScope scope;

		private SyncFlag syncFlags;

		private SyncExtraFlag extraFlags;

		private StorePropTag[] propertyTags;

		internal class HierarchySynchronizer : DisposableBase, IHierarchySynchronizer, IDisposable
		{
			public HierarchySynchronizer(MapiContext operationContext, IcsHierarchyDownloadContext context, IHierarchySynchronizationScope scope, IcsState state, SyncFlag syncFlags, SyncExtraFlag extraFlags, StorePropTag[] propertyTags)
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
				this.propertyTags = new HashSet<StorePropTag>(propertyTags);
				this.cnsetSeenServer = scope.GetServerCnsetSeen(operationContext);
				scope.GetChangedAndDeletedFolders(operationContext, this.syncFlags, state.CnsetSeen, state.IdsetGiven, out this.changedFolders, out this.idsetDeletes);
			}

			public IEnumerator<IFolderChange> GetChanges()
			{
				if (this.changedFolders != null)
				{
					ushort lastCnReplid = 0;
					Guid lastCnGuid = default(Guid);
					IReplidGuidMap replidGuidMap = this.context.Logon.StoreMailbox.ReplidGuidMap;
					foreach (FolderChangeEntry folderHeader in this.changedFolders)
					{
						ExchangeId fid = ExchangeId.CreateFromInternalShortId(this.context.Logon.StoreMailbox.CurrentOperationContext, replidGuidMap, folderHeader.FolderId);
						bool skipFolder = false;
						if ((ushort)(this.syncFlags & SyncFlag.CatchUp) == 0)
						{
							using (MapiFolder folder = this.scope.OpenFolder(fid))
							{
								if (folder != null)
								{
									yield return new IcsHierarchyDownloadContext.FolderChange(this.context, this.scope, this.syncFlags, this.extraFlags, this.propertyTags, folder);
								}
								else
								{
									skipFolder = true;
								}
							}
						}
						if (!skipFolder)
						{
							ushort num;
							ulong counter;
							ExchangeIdHelpers.FromLong(folderHeader.Cn, out num, out counter);
							if (num != lastCnReplid)
							{
								lastCnGuid = this.scope.ReplidToGuid(new ReplId(num));
								lastCnReplid = num;
							}
							this.state.CnsetSeen.Insert(lastCnGuid, counter);
							this.state.IdsetGiven.Insert(fid);
						}
					}
				}
				this.state.CnsetSeen.Insert(this.cnsetSeenServer);
				yield break;
			}

			public IPropertyBag GetDeletions()
			{
				MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag(this.context);
				if ((ushort)(this.syncFlags & SyncFlag.CatchUp) == 0 && (ushort)(this.syncFlags & SyncFlag.NoDeletions) == 0 && this.idsetDeletes != null && !this.idsetDeletes.IsEmpty)
				{
					byte[] value = this.idsetDeletes.Serialize(new Func<Guid, ReplId>(this.scope.GuidToReplid));
					memoryPropertyBag.SetProperty(new PropertyValue(PropertyTag.IdsetDeleted, value));
				}
				return memoryPropertyBag;
			}

			public IIcsState GetFinalState()
			{
				if (this.idsetDeletes != null)
				{
					this.state.IdsetGiven.Remove(this.idsetDeletes);
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
				return DisposeTracker.Get<IcsHierarchyDownloadContext.HierarchySynchronizer>(this);
			}

			protected override void InternalDispose(bool isCalledFromDispose)
			{
			}

			private IcsHierarchyDownloadContext context;

			private IHierarchySynchronizationScope scope;

			private IcsState state;

			private SyncFlag syncFlags;

			private SyncExtraFlag extraFlags;

			private HashSet<StorePropTag> propertyTags;

			private IdSet cnsetSeenServer;

			private IdSet idsetDeletes;

			private IList<FolderChangeEntry> changedFolders;
		}

		internal class FolderChange : FastTransferPropertyBag, IFolderChange, IDisposable
		{
			public FolderChange(IcsDownloadContext context, IHierarchySynchronizationScope scope, SyncFlag syncFlags, SyncExtraFlag extraFlags, HashSet<StorePropTag> propList, MapiFolder folder) : base(context, folder, (ushort)(syncFlags & SyncFlag.OnlySpecifiedProps) == 0, propList)
			{
				this.scope = scope;
				this.syncFlags = syncFlags;
				this.extraFlags = extraFlags;
			}

			private MapiFolder MapiFolder
			{
				get
				{
					return (MapiFolder)base.MapiPropBag;
				}
				set
				{
					base.MapiPropBag = value;
				}
			}

			protected override List<Property> LoadAllPropertiesImp()
			{
				List<Property> list = base.LoadAllPropertiesImp();
				if (base.ForMoveUser)
				{
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.CnExport);
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.PclExport);
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.CnMvExport);
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.LastModificationTime);
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.MidsetDeletedExport);
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.ChangeKey);
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.PredecessorChangeList);
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.SourceKey);
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.LastConflict);
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.ArticleNumNext);
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.FolderAdminFlags);
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.ELCPolicyComment);
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.ELCPolicyId);
					FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.ELCFolderQuota);
				}
				FastTransferPropertyBag.ResetPropertyIfPresent(list, PropTag.Folder.SourceKey);
				FastTransferPropertyBag.ResetPropertyIfPresent(list, PropTag.Folder.ParentSourceKey);
				ValueHelper.SortAndRemoveDuplicates<Property>(list, PropertyComparerByTag.Comparer);
				return list;
			}

			protected override Property GetPropertyImp(StorePropTag propTag)
			{
				if (propTag == PropTag.Folder.SourceKey)
				{
					if ((ushort)(this.syncFlags & SyncFlag.NoForeignKeys) != 0)
					{
						object value = this.MapiFolder.Fid.To22ByteArray();
						return new Property(PropTag.Folder.SourceKey, value);
					}
				}
				else if (propTag == PropTag.Folder.ParentSourceKey)
				{
					if (this.MapiFolder.GetParentFid(base.Context.CurrentOperationContext) == this.scope.GetRootFid())
					{
						object empty = Array<byte>.Empty;
						return new Property(PropTag.Folder.ParentSourceKey, empty);
					}
					if ((ushort)(this.syncFlags & SyncFlag.NoForeignKeys) != 0)
					{
						object value2 = this.MapiFolder.GetParentFid(base.Context.CurrentOperationContext).To22ByteArray();
						return new Property(PropTag.Folder.ParentSourceKey, value2);
					}
				}
				return base.GetPropertyImp(propTag);
			}

			protected override void SetPropertyImp(Property property)
			{
				throw new NotSupportedException();
			}

			protected override void DeleteImp(StorePropTag propTag)
			{
				throw new NotSupportedException();
			}

			public override Stream GetPropertyStreamImp(StorePropTag propTag)
			{
				return base.GetPropertyStreamImp(propTag);
			}

			public override Stream SetPropertyStreamImp(StorePropTag propTag, long dataSize)
			{
				throw new NotSupportedException();
			}

			protected override bool IncludeTag(StorePropTag propTag)
			{
				if (base.ForMoveUser && propTag.IsCategory(4))
				{
					ushort propId = propTag.PropId;
					if (propId != 26514)
					{
						switch (propId)
						{
						case 26532:
						case 26533:
						case 26534:
							break;
						default:
							return true;
						}
					}
					return false;
				}
				return IcsHierarchyDownloadContext.FolderChange.specialFolderProps.Contains(propTag) || (propTag == PropTag.Folder.Fid && (this.extraFlags & SyncExtraFlag.Eid) != SyncExtraFlag.None) || (propTag == PropTag.Folder.ChangeNumber && (this.extraFlags & SyncExtraFlag.Cn) != SyncExtraFlag.None) || (propTag == PropTag.Folder.ParentFid && ((this.extraFlags & SyncExtraFlag.ManifestMode) != SyncExtraFlag.None || (ushort)(this.syncFlags & SyncFlag.NoForeignKeys) != 0 || (this.extraFlags & SyncExtraFlag.Eid) != SyncExtraFlag.None)) || ((26112 > propTag.PropId || propTag.PropId > 26623) && !propTag.IsNamedProperty && (!(propTag == PropTag.Folder.FreeBusyNTSD) || !base.ExcludeProps) && base.IncludeTag(propTag));
			}

			public IPropertyBag FolderPropertyBag
			{
				get
				{
					return this;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<IcsHierarchyDownloadContext.FolderChange>(this);
			}

			protected override void InternalDispose(bool isCalledFromDispose)
			{
				if (isCalledFromDispose && this.MapiFolder != null)
				{
					this.MapiFolder.Dispose();
					this.MapiFolder = null;
				}
				base.InternalDispose(isCalledFromDispose);
			}

			private static HashSet<StorePropTag> specialFolderProps = new HashSet<StorePropTag>
			{
				PropTag.Folder.ParentSourceKey,
				PropTag.Folder.SourceKey,
				PropTag.Folder.LastModificationTime,
				PropTag.Folder.ChangeKey,
				PropTag.Folder.PredecessorChangeList,
				PropTag.Folder.DisplayName,
				PropTag.Folder.ELCFolderQuota,
				PropTag.Folder.ELCFolderSize,
				PropTag.Folder.FolderAdminFlags,
				PropTag.Folder.ELCPolicyComment,
				PropTag.Folder.ELCPolicyId
			};

			private IHierarchySynchronizationScope scope;

			private SyncFlag syncFlags;

			private SyncExtraFlag extraFlags;
		}
	}
}
