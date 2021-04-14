using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.PST;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PstDestinationMailbox : PstMailbox, IDestinationMailbox, IMailbox, IDisposable
	{
		bool IDestinationMailbox.MailboxExists()
		{
			MrsTracer.Provider.Function("PstDestinationMailbox.IDestinationMailbox.MailboxExists()", new object[0]);
			return ((IMailbox)this).IsConnected();
		}

		CreateMailboxResult IDestinationMailbox.CreateMailbox(byte[] mailboxData, MailboxSignatureFlags sourceSignatureFlags)
		{
			MrsTracer.Provider.Function("PstDestinationMailbox.CreateMailbox", new object[0]);
			return CreateMailboxResult.Success;
		}

		void IDestinationMailbox.ProcessMailboxSignature(byte[] mailboxData)
		{
			throw new NotImplementedException();
		}

		IDestinationFolder IDestinationMailbox.GetFolder(byte[] entryId)
		{
			MrsTracer.Provider.Function("PstDestinationMailbox.GetFolder({0})", new object[]
			{
				TraceUtils.DumpEntryId(entryId)
			});
			return base.GetFolder<PstDestinationFolder>(entryId);
		}

		IFxProxy IDestinationMailbox.GetFxProxy()
		{
			throw new NotImplementedException();
		}

		IFxProxyPool IDestinationMailbox.GetFxProxyPool(ICollection<byte[]> folderIds)
		{
			MrsTracer.Provider.Function("PstDestinationMailbox.IDestinationMailbox.GetFxProxyPool", new object[0]);
			PSTFxProxyPool proxy = new PSTFxProxyPool(this, folderIds);
			return new FxProxyPoolWrapper(proxy, null);
		}

		PropProblemData[] IDestinationMailbox.SetProps(PropValueData[] pvda)
		{
			throw new NotImplementedException();
		}

		void IDestinationMailbox.CreateFolder(FolderRec sourceFolder, CreateFolderFlags createFolderFlags, out byte[] newFolderId)
		{
			MrsTracer.Provider.Function("PstDestinationMailbox.CreateFolder(\"{0}\")", new object[]
			{
				sourceFolder.FolderName
			});
			newFolderId = null;
			uint nodeIdFromEntryId = PstMailbox.GetNodeIdFromEntryId(base.IPst.MessageStore.Guid, sourceFolder.ParentId);
			if (sourceFolder.EntryId != null)
			{
				uint nodeIdFromEntryId2 = PstMailbox.GetNodeIdFromEntryId(base.IPst.MessageStore.Guid, sourceFolder.EntryId);
				IFolder folder = base.IPst.ReadFolder(nodeIdFromEntryId2);
				if (folder != null)
				{
					if (createFolderFlags.HasFlag(CreateFolderFlags.FailIfExists))
					{
						throw new UnableToReadPSTFolderPermanentException(nodeIdFromEntryId2);
					}
					if (nodeIdFromEntryId == folder.ParentId)
					{
						return;
					}
				}
			}
			IFolder folder2 = base.IPst.ReadFolder(nodeIdFromEntryId);
			if (folder2 == null)
			{
				throw new UnableToReadPSTFolderPermanentException(nodeIdFromEntryId);
			}
			using (PstFxFolder pstFxFolder = new PstFxFolder(this, folder2.AddFolder()))
			{
				pstFxFolder.PropertyBag.SetProperty(new PropertyValue(PropertyTag.DisplayName, sourceFolder.FolderName));
				pstFxFolder.PropertyBag.SetProperty(new PropertyValue(PropertyTag.FolderType, (int)sourceFolder.FolderType));
				newFolderId = PstMailbox.CreateEntryIdFromNodeId(base.IPst.MessageStore.Guid, pstFxFolder.IPstFolder.Id);
			}
		}

		void IDestinationMailbox.MoveFolder(byte[] folderId, byte[] oldParentId, byte[] newParentId)
		{
			throw new NotImplementedException();
		}

		void IDestinationMailbox.DeleteFolder(FolderRec folderRec)
		{
			throw new NotImplementedException();
		}

		string IMailbox.LoadSyncState(byte[] key)
		{
			MrsTracer.Provider.Function("PstDestinationMailbox.IMailbox.LoadSyncState(key={0})", new object[]
			{
				TraceUtils.DumpBytes(key)
			});
			this.LoadSyncStateCache();
			string text = this.syncStateCache[key];
			MrsTracer.Provider.Debug("key {0}found.", new object[]
			{
				(text != null) ? string.Empty : "NOT "
			});
			return text;
		}

		MessageRec IMailbox.SaveSyncState(byte[] key, string syncStateStr)
		{
			MrsTracer.Provider.Function("PstDestinationMailbox.IMailbox.SaveSyncState(key={0})", new object[]
			{
				TraceUtils.DumpBytes(key)
			});
			if (base.IPst == null)
			{
				MrsTracer.Provider.Debug("Skipping sync state save on a previously failed pst file", new object[0]);
				return null;
			}
			try
			{
				this.LoadSyncStateCache();
				this.syncStateCache[key] = syncStateStr;
				base.MessageStorePropertyBag.SetProperty(new PropertyValue(new PropertyTag((PropertyId)this.syncStatePropId, PropertyType.Unicode), this.syncStateCache.Serialize(false)));
				base.IPst.MessageStore.Save();
			}
			catch (PSTExceptionBase innerException)
			{
				throw new UnableToSavePSTSyncStatePermanentException(base.IPst.FileName, innerException);
			}
			return null;
		}

		void IDestinationMailbox.SetMailboxSecurityDescriptor(RawSecurityDescriptor sd)
		{
			throw new NotImplementedException();
		}

		void IDestinationMailbox.SetUserSecurityDescriptor(RawSecurityDescriptor sd)
		{
			throw new NotImplementedException();
		}

		void IDestinationMailbox.PreFinalSyncDataProcessing(int? sourceMailboxVersion)
		{
			throw new NotImplementedException();
		}

		ConstraintCheckResultType IDestinationMailbox.CheckDataGuarantee(DateTime commitTimestamp, out LocalizedString failureReason)
		{
			throw new NotImplementedException();
		}

		void IDestinationMailbox.ForceLogRoll()
		{
			throw new NotImplementedException();
		}

		List<ReplayAction> IDestinationMailbox.GetActions(string replaySyncState, int maxNumberOfActions)
		{
			throw new NotImplementedException();
		}

		void IDestinationMailbox.SetMailboxSettings(ItemPropertiesBase item)
		{
			throw new NotImplementedException();
		}

		private void LoadSyncStateCache()
		{
			MrsTracer.Provider.Function("PstDestinationMailbox.LoadSyncStateData", new object[0]);
			if (this.syncStateCache != null)
			{
				return;
			}
			try
			{
				if (this.syncStatePropId == 0)
				{
					this.syncStatePropId = base.IPst.ReadIdFromNamedProp(PstDestinationMailbox.syncStateNamedProp.Name, 0U, PstDestinationMailbox.syncStateNamedProp.Guid, true);
				}
				this.syncStateCache = new PSTSyncStateDictionary();
				PropertyValue property = base.MessageStorePropertyBag.GetProperty(new PropertyTag((PropertyId)this.syncStatePropId, PropertyType.Unicode));
				if (!property.IsError)
				{
					this.syncStateCache = XMLSerializableBase.Deserialize<PSTSyncStateDictionary>((string)property.Value, true);
				}
			}
			catch (PSTExceptionBase innerException)
			{
				throw new UnableToLoadPSTSyncStatePermanentException(base.IPst.FileName, innerException);
			}
			catch (MoveJobDeserializationFailedPermanentException innerException2)
			{
				throw new UnableToLoadPSTSyncStatePermanentException(base.IPst.FileName, innerException2);
			}
		}

		private static readonly NamedProperty syncStateNamedProp = new NamedProperty(new Guid("{EF406DC2-053E-42ff-9547-C52CAC90D184}"), "MRSSyncState");

		private ushort syncStatePropId;

		private PSTSyncStateDictionary syncStateCache;
	}
}
