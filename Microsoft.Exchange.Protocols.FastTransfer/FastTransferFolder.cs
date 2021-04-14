using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.FastTransfer;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class FastTransferFolder : FastTransferPropertyBag, IFolder, IMessageIterator, IMessageIteratorClient, IDisposable
	{
		public FastTransferFolder(FastTransferDownloadContext downloadContext, MapiFolder mapiFolder, bool excludeProps, HashSet<StorePropTag> propList, bool topLevel, FastTransferCopyFlag flags) : this(downloadContext, mapiFolder, excludeProps, propList, topLevel, flags, null)
		{
		}

		public FastTransferFolder(FastTransferDownloadContext downloadContext, MapiFolder mapiFolder, FastTransferCopyFlag flags, IList<ExchangeId> midList) : this(downloadContext, mapiFolder, true, null, true, flags, midList)
		{
		}

		private FastTransferFolder(FastTransferDownloadContext downloadContext, MapiFolder mapiFolder, bool excludeProps, HashSet<StorePropTag> propList, bool topLevel, FastTransferCopyFlag flags, IList<ExchangeId> midList) : base(downloadContext, mapiFolder, excludeProps, propList)
		{
			this.topLevel = topLevel;
			this.flags = flags;
			this.midList = midList;
		}

		public FastTransferFolder(FastTransferUploadContext uploadContext, MapiFolder mapiFolder, bool topLevel, FastTransferCopyFlag flags) : base(uploadContext, mapiFolder)
		{
			this.topLevel = topLevel;
			this.flags = flags;
		}

		public FastTransferFolder(FastTransferUploadContext uploadContext, ExchangeId parentFid, FastTransferCopyFlag flags) : base(uploadContext, null)
		{
			this.topLevel = false;
			this.flags = flags;
			this.parentFid = parentFid;
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

		public IPropertyBag PropertyBag
		{
			get
			{
				return this;
			}
		}

		public IEnumerable<IMessage> GetContents()
		{
			using (IEnumerator<IMessage> messageIterator = this.GetAllMessages(false))
			{
				while (messageIterator.MoveNext())
				{
					if (messageIterator.Current != null)
					{
						yield return messageIterator.Current;
					}
				}
			}
			yield break;
		}

		public IEnumerable<IMessage> GetAssociatedContents()
		{
			using (IEnumerator<IMessage> messageIterator = this.GetAllMessages(true))
			{
				while (messageIterator.MoveNext())
				{
					if (messageIterator.Current != null)
					{
						yield return messageIterator.Current;
					}
				}
			}
			yield break;
		}

		public IEnumerable<IFolder> GetFolders()
		{
			using (MapiViewFolder folderView = new MapiViewFolder())
			{
				folderView.Configure(base.Context.CurrentOperationContext, this.MapiFolder.Logon, this.MapiFolder, FolderViewTable.ConfigureFlags.NoNotifications);
				folderView.SetColumns(base.Context.CurrentOperationContext, FastTransferFolder.columnsToFetchFolderFid);
				IList<Properties> viewResults = folderView.QueryRowsBatch(base.Context.CurrentOperationContext, 100, QueryRowsFlags.None);
				while (viewResults != null && viewResults.Count != 0)
				{
					for (int i = 0; i < viewResults.Count; i++)
					{
						ExchangeId childFid = ExchangeId.CreateFrom26ByteArray(base.Context.CurrentOperationContext, this.MapiFolder.Logon.StoreMailbox.ReplidGuidMap, (byte[])viewResults[i][0].Value);
						using (MapiFolder child = MapiFolder.OpenFolder(base.Context.CurrentOperationContext, this.MapiFolder.Logon, childFid))
						{
							if (child != null)
							{
								if (ExTraceGlobals.SourceSendTracer.IsTraceEnabled(TraceType.DebugTrace))
								{
									StringBuilder stringBuilder = new StringBuilder(100);
									stringBuilder.Append("Send Folder=[FolderId = [");
									stringBuilder.Append(childFid.ToString());
									stringBuilder.Append("]]");
									ExTraceGlobals.SourceSendTracer.TraceDebug(0L, stringBuilder.ToString());
								}
								yield return new FastTransferFolder(base.DownloadContext, child, this.flags | FastTransferCopyFlag.SendEntryId, null);
							}
						}
					}
					viewResults = folderView.QueryRowsBatch(base.Context.CurrentOperationContext, 100, QueryRowsFlags.None);
				}
			}
			yield break;
		}

		public IFolder CreateFolder()
		{
			return new FastTransferFolder(base.UploadContext, this.MapiFolder.Fid, this.flags);
		}

		public IMessage CreateMessage(bool isAssociatedMessage)
		{
			return this.UploadMessage(isAssociatedMessage);
		}

		public void Save()
		{
			this.DeferredCreateMapiFolder();
			if (this.MapiFolder != null)
			{
				if (this.MapiFolder.Logon.IsPublicFolderSystem || this.MapiFolder.Logon.IsMoveDestination)
				{
					List<Property> list = this.LoadAllPropertiesImp();
					for (int i = 0; i < list.Count; i++)
					{
						Property property = list[i];
						if (this.CouldPropertyBeDeletedAtSource(property.Tag))
						{
							this.DeleteImp(property.Tag);
						}
					}
				}
				this.uploadedPropertyTags = null;
				if (this.MapiFolder.Logon.IsMoveUser && !this.MapiFolder.Logon.IsForPublicFolderMove)
				{
					this.MapiFolder.StoreFolder.TrackFolderChange = false;
				}
				this.MapiFolder.StoreFolder.Save(base.Context.CurrentOperationContext);
				if (ExTraceGlobals.SourceSendTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("Receive Folder Save Changes=[");
					stringBuilder.Append("FolderId = [").Append(this.MapiFolder.Fid.ToString());
					stringBuilder.Append("]]");
					ExTraceGlobals.SourceSendTracer.TraceDebug(0L, stringBuilder.ToString());
				}
			}
		}

		internal bool CouldPropertyBeDeletedAtSource(StorePropTag propTag)
		{
			return !propTag.IsCategory(10) && (!propTag.IsCategory(3) || propTag.IsCategory(5)) && !propTag.IsCategory(17) && (this.uploadedPropertyTags == null || !this.uploadedPropertyTags.Contains(propTag));
		}

		public bool IsContentAvailable
		{
			get
			{
				return true;
			}
		}

		public string[] GetReplicaDatabases(out ushort localSiteDatabaseCount)
		{
			throw new StoreException((LID)64568U, ErrorCodeValue.NotSupported);
		}

		public StoreLongTermId GetLongTermId()
		{
			throw new StoreException((LID)39992U, ErrorCodeValue.NotSupported);
		}

		public IEnumerator<IMessage> GetMessages()
		{
			if (this.midList == null)
			{
				using (IEnumerator<IMessage> messageIterator = this.GetAllMessages(true))
				{
					while (messageIterator.MoveNext())
					{
						IMessage message = messageIterator.Current;
						yield return message;
					}
				}
				using (IEnumerator<IMessage> messageIterator2 = this.GetAllMessages(false))
				{
					while (messageIterator2.MoveNext())
					{
						IMessage message2 = messageIterator2.Current;
						yield return message2;
					}
				}
			}
			else
			{
				foreach (ExchangeId mid in this.midList)
				{
					yield return this.PrepareSingleReturnMessage(mid);
				}
			}
			yield break;
		}

		private IMessage PrepareSingleReturnMessage(ExchangeId mid)
		{
			IMessage result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MapiMessage mapiMessage = disposeGuard.Add<MapiMessage>(new MapiMessage());
				ErrorCode first = mapiMessage.ConfigureMessage(base.Context.CurrentOperationContext, this.MapiFolder.Logon, this.MapiFolder.Fid, mid, MessageConfigureFlags.None, this.MapiFolder.Logon.CodePage);
				if (first == ErrorCode.NoError)
				{
					if (ExTraceGlobals.SourceSendTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("Send Message=[");
						stringBuilder.Append("FolderId = [");
						stringBuilder.Append(mapiMessage.GetFid().ToString());
						stringBuilder.Append("MessageId = [");
						stringBuilder.Append(mid.ToString());
						stringBuilder.Append("]]");
						ExTraceGlobals.SourceSendTracer.TraceDebug(0L, stringBuilder.ToString());
					}
					disposeGuard.Success();
					result = new FastTransferMessage(base.DownloadContext, mapiMessage, true, null, false, false, false, this.flags);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private IEnumerator<IMessage> GetAllMessages(bool returnAssociated)
		{
			using (MapiViewMessage mapiViewMessage = new MapiViewMessage())
			{
				ViewMessageConfigureFlags viewFlags = ViewMessageConfigureFlags.NoNotifications | ViewMessageConfigureFlags.DoNotUseLazyIndex;
				if (returnAssociated)
				{
					viewFlags |= ViewMessageConfigureFlags.ViewFAI;
				}
				mapiViewMessage.Configure(base.Context.CurrentOperationContext, this.MapiFolder.Logon, this.MapiFolder, viewFlags);
				mapiViewMessage.SetColumns(base.Context.CurrentOperationContext, FastTransferFolder.columnsToFetchMessageMid, MapiViewSetColumnsFlag.NoColumnValidation);
				IList<Properties> midRows = mapiViewMessage.QueryRowsBatch(base.Context.CurrentOperationContext, 100, QueryRowsFlags.None);
				while (midRows != null && midRows.Count != 0)
				{
					foreach (Properties midRow in midRows)
					{
						Context currentOperationContext = base.Context.CurrentOperationContext;
						IReplidGuidMap replidGuidMap = this.MapiFolder.Logon.MapiMailbox.StoreMailbox.ReplidGuidMap;
						Properties properties = midRow;
						ExchangeId mid = ExchangeId.CreateFrom26ByteArray(currentOperationContext, replidGuidMap, (byte[])properties[0].Value);
						yield return this.PrepareSingleReturnMessage(mid);
					}
					midRows = mapiViewMessage.QueryRowsBatch(base.Context.CurrentOperationContext, 100, QueryRowsFlags.None);
				}
			}
			yield break;
		}

		public IMessage UploadMessage(bool isAssociatedMessage)
		{
			MessageConfigureFlags messageConfigureFlags = MessageConfigureFlags.CreateNewMessage | MessageConfigureFlags.SkipQuotaCheck;
			messageConfigureFlags |= (isAssociatedMessage ? MessageConfigureFlags.IsAssociated : MessageConfigureFlags.None);
			MapiMessage mapiMessage = new MapiMessage();
			ErrorCode errorCode = mapiMessage.ConfigureMessage(base.Context.CurrentOperationContext, this.MapiFolder.Logon, this.MapiFolder.Fid, ExchangeId.Zero, messageConfigureFlags, this.MapiFolder.Logon.CodePage);
			if (!(errorCode != ErrorCode.NoError))
			{
				return new FastTransferMessage(base.UploadContext, mapiMessage, false, this.flags);
			}
			mapiMessage.Dispose();
			if (errorCode == ErrorCodeValue.NotFound)
			{
				throw new ExExceptionNotFound((LID)56376U, "parent folder not found");
			}
			throw new StoreException((LID)44088U, errorCode, "unexpected error from ConfigureMessage");
		}

		protected override List<Property> LoadAllPropertiesImp()
		{
			bool flag = false;
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
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.ELCPolicyComment);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.ELCPolicyId);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Folder.ELCFolderQuota);
				if (base.Context is FastTransferDownloadContext && base.Context.OtherSideVersion < FastTransferContext.Dumpster2SupportMinVersion)
				{
					FolderAdminFlags folderAdminFlags = (FolderAdminFlags)0;
					object propertyValue = this.MapiFolder.StoreFolder.GetPropertyValue(base.Context.CurrentOperationContext, PropTag.Folder.FolderAdminFlags);
					if (propertyValue != null)
					{
						folderAdminFlags = (FolderAdminFlags)((int)propertyValue);
					}
					if ((folderAdminFlags & FolderAdminFlags.DumpsterFolder) != (FolderAdminFlags)0)
					{
						flag = true;
					}
				}
			}
			ValueHelper.SortAndRemoveDuplicates<Property>(list, PropertyComparerByTag.Comparer);
			int num = list.BinarySearch(new Property(PropTag.Folder.AclTableAndSecurityDescriptor, null), PropertyComparerByTag.Comparer);
			if (num >= 0)
			{
				list.RemoveAt(num);
			}
			if (flag)
			{
				num = list.BinarySearch(new Property(PropTag.Folder.FolderAdminFlags, null), PropertyComparerByTag.Comparer);
				if (num >= 0)
				{
					list.RemoveAt(num);
				}
			}
			return list;
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
			if ((base.ForMoveUser || this.MapiFolder.Logon.IsPublicFolderSystem) && this.MapiFolder.Logon.MapiMailbox.IsPublicFolderMailbox)
			{
				ushort propId2 = propTag.PropId;
				if (propId2 <= 26309)
				{
					switch (propId2)
					{
					case 26264:
					case 26265:
						break;
					default:
						switch (propId2)
						{
						case 26308:
						case 26309:
							break;
						default:
							goto IL_F9;
						}
						break;
					}
				}
				else
				{
					switch (propId2)
					{
					case 26397:
					case 26399:
					case 26401:
					case 26402:
						break;
					case 26398:
					case 26400:
						goto IL_F9;
					default:
						if (propId2 != 26413)
						{
							switch (propId2)
							{
							case 26489:
							case 26491:
								break;
							case 26490:
								goto IL_F9;
							default:
								goto IL_F9;
							}
						}
						break;
					}
				}
				return true;
			}
			IL_F9:
			if (base.ForMoveUser)
			{
				ushort propId3 = propTag.PropId;
				if (propId3 <= 26083)
				{
					if (propId3 <= 16329)
					{
						if (propId3 != 12296 && propId3 != 16329)
						{
							goto IL_1ED;
						}
					}
					else if (propId3 != 16355)
					{
						switch (propId3)
						{
						case 26080:
						case 26082:
						case 26083:
							break;
						case 26081:
							goto IL_1ED;
						default:
							goto IL_1ED;
						}
					}
					else
					{
						if (FastTransferContext.OofHistorySupportMinVersion > base.Context.OtherSideVersion)
						{
							DiagnosticContext.TraceLocation((LID)54159U);
							return false;
						}
						goto IL_1ED;
					}
				}
				else if (propId3 <= 26449)
				{
					switch (propId3)
					{
					case 26413:
						break;
					case 26414:
					case 26415:
					case 26416:
						goto IL_1ED;
					case 26417:
					case 26418:
					case 26419:
						return false;
					default:
						if (propId3 != 26449)
						{
							goto IL_1ED;
						}
						break;
					}
				}
				else
				{
					switch (propId3)
					{
					case 26457:
					case 26458:
					case 26459:
					case 26460:
						break;
					default:
						if (propId3 != 26574)
						{
							goto IL_1ED;
						}
						break;
					}
				}
				return true;
			}
			IL_1ED:
			if (26112 <= propTag.PropId && propTag.PropId <= 26623)
			{
				return propTag == PropTag.Folder.Fid && !this.topLevel;
			}
			if (propTag.IsNamedProperty)
			{
				return false;
			}
			ushort propId4 = propTag.PropId;
			if (propId4 <= 4095)
			{
				if (propId4 != 3593)
				{
					switch (propId4)
					{
					case 4089:
					case 4090:
					case 4091:
					case 4094:
					case 4095:
						break;
					case 4092:
					case 4093:
						goto IL_29D;
					default:
						goto IL_29D;
					}
				}
			}
			else
			{
				switch (propId4)
				{
				case 13825:
				case 13826:
				case 13827:
					break;
				default:
					if (propId4 != 16352 && propId4 != 16382)
					{
						goto IL_29D;
					}
					return false;
				}
			}
			return false;
			IL_29D:
			return base.IncludeTag(propTag);
		}

		protected override bool IncludeToPackedNamedProperties(StorePropTag propTag)
		{
			return true;
		}

		protected override Microsoft.Exchange.Server.Storage.PropTags.ObjectType GetObjectTypeImp()
		{
			if (this.MapiFolder == null)
			{
				return Helper.GetPropTagObjectType(MapiObjectType.Folder);
			}
			return base.GetObjectTypeImp();
		}

		protected override Property GetPropertyImp(StorePropTag propTag)
		{
			if (this.MapiFolder != null)
			{
				return base.GetPropertyImp(propTag);
			}
			if (this.uploadedProperties == null || !this.uploadedProperties.ContainsKey(propTag))
			{
				return Property.NotFoundError(propTag);
			}
			return new Property(propTag, this.uploadedProperties[propTag]);
		}

		protected override void SetPropertyImp(Property property)
		{
			if (property.Tag == PropTag.Folder.OofHistory && base.ForMoveUser && FastTransferContext.OofHistorySupportMinVersion > base.Context.OtherSideVersion)
			{
				DiagnosticContext.TraceLocation((LID)41871U);
				return;
			}
			if (property.Tag == PropTag.Folder.AclTableAndSecurityDescriptor)
			{
				DiagnosticContext.TraceLocation((LID)33351U);
				return;
			}
			if (this.uploadedPropertyTags == null)
			{
				this.uploadedPropertyTags = new HashSet<StorePropTag>();
			}
			this.uploadedPropertyTags.Add(property.Tag);
			if (this.MapiFolder != null)
			{
				base.SetPropertyImp(property);
				return;
			}
			if (this.uploadedProperties == null)
			{
				this.uploadedProperties = new Dictionary<StorePropTag, object>();
			}
			this.uploadedProperties[property.Tag] = property.Value;
		}

		public override Stream SetPropertyStreamImp(StorePropTag propTag, long dataSize)
		{
			if (this.uploadedPropertyTags == null)
			{
				this.uploadedPropertyTags = new HashSet<StorePropTag>();
			}
			this.uploadedPropertyTags.Add(propTag);
			if (this.MapiFolder == null)
			{
				if (this.uploadedProperties == null)
				{
					this.uploadedProperties = new Dictionary<StorePropTag, object>();
				}
				FastTransferFolder.StreamWrapper streamWrapper = null;
				try
				{
					FastTransferFolder.CachedByteStream cachedByteStream = new FastTransferFolder.CachedByteStream();
					streamWrapper = new FastTransferFolder.StreamWrapper(dataSize, new Action<byte[]>(cachedByteStream.OnFlush));
					this.uploadedProperties[propTag] = cachedByteStream;
					Stream result = streamWrapper;
					streamWrapper = null;
					return result;
				}
				finally
				{
					if (streamWrapper != null)
					{
						streamWrapper.Dispose();
					}
				}
			}
			return base.SetPropertyStreamImp(propTag, dataSize);
		}

		protected override void DeleteImp(StorePropTag propTag)
		{
			if (this.MapiFolder != null)
			{
				base.DeleteImp(propTag);
				return;
			}
			if (this.uploadedProperties != null)
			{
				this.uploadedProperties.Remove(propTag);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferFolder>(this);
		}

		protected override void InternalDispose(bool isCalledFromDispose)
		{
			if (isCalledFromDispose && this.MapiFolder != null && !this.topLevel)
			{
				this.MapiFolder.Dispose();
				this.MapiFolder = null;
			}
			base.InternalDispose(isCalledFromDispose);
		}

		private void DeferredCreateMapiFolder()
		{
			if (this.MapiFolder == null && this.parentFid.IsValid)
			{
				using (MapiFolder mapiFolder = MapiFolder.OpenFolder(base.Context.CurrentOperationContext, base.Context.Logon, this.parentFid))
				{
					if (mapiFolder == null)
					{
						throw new StoreException((LID)35896U, ErrorCodeValue.NotFound);
					}
					MapiFolder mapiFolder2 = null;
					try
					{
						ExchangeId zero = ExchangeId.Zero;
						ErrorCode errorCode = MapiFolder.CreateFolder(base.Context.CurrentOperationContext, base.Context.Logon, ref zero, false, FolderConfigureFlags.None, mapiFolder, MapiFolder.ManageAssociatedDumpsterFolder(base.Context.Logon, false), out mapiFolder2);
						if (errorCode != ErrorCode.NoError)
						{
							throw new StoreException((LID)52280U, errorCode);
						}
						if (ExTraceGlobals.SourceSendTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder = new StringBuilder(100);
							stringBuilder.Append("Receive Folder=[");
							stringBuilder.Append("FolderId = [");
							stringBuilder.Append(zero.ToString());
							stringBuilder.Append("]]");
							ExTraceGlobals.SourceSendTracer.TraceDebug(0L, stringBuilder.ToString());
						}
						this.MapiFolder = mapiFolder2;
						mapiFolder2 = null;
						this.parentFid = ExchangeId.Null;
					}
					finally
					{
						if (mapiFolder2 != null)
						{
							mapiFolder2.Dispose();
						}
					}
					if (this.uploadedProperties != null)
					{
						Dictionary<StorePropTag, object> dictionary = this.uploadedProperties;
						this.uploadedProperties = null;
						foreach (KeyValuePair<StorePropTag, object> keyValuePair in dictionary)
						{
							FastTransferFolder.CachedByteStream cachedByteStream = keyValuePair.Value as FastTransferFolder.CachedByteStream;
							if (cachedByteStream != null)
							{
								if (cachedByteStream.Buffer == null)
								{
									continue;
								}
								using (Stream stream = this.SetPropertyStreamImp(keyValuePair.Key, (long)cachedByteStream.Buffer.Length))
								{
									stream.Write(cachedByteStream.Buffer, 0, cachedByteStream.Buffer.Length);
									stream.Flush();
									continue;
								}
							}
							this.SetPropertyImp(new Property(keyValuePair.Key, keyValuePair.Value));
						}
					}
				}
			}
		}

		private static readonly StorePropTag[] columnsToFetchMessageMid = new StorePropTag[]
		{
			PropTag.Message.MidBin
		};

		private static readonly StorePropTag[] columnsToFetchFolderFid = new StorePropTag[]
		{
			PropTag.Folder.FidBin
		};

		private bool topLevel;

		private FastTransferCopyFlag flags;

		private IList<ExchangeId> midList;

		private Dictionary<StorePropTag, object> uploadedProperties;

		private HashSet<StorePropTag> uploadedPropertyTags;

		private ExchangeId parentFid;

		private class CachedByteStream
		{
			public byte[] Buffer
			{
				get
				{
					return this.buffer;
				}
			}

			public void OnFlush(byte[] buffer)
			{
				this.buffer = buffer;
			}

			private byte[] buffer;
		}

		private class StreamWrapper : MemoryStream
		{
			public StreamWrapper(long dataSize, Action<byte[]> flushCallback) : base((int)dataSize)
			{
				this.flushCallback = flushCallback;
			}

			public override void Flush()
			{
				if (this.flushCallback != null)
				{
					this.flushCallback(this.ToArray());
				}
				base.Flush();
			}

			private Action<byte[]> flushCallback;
		}
	}
}
