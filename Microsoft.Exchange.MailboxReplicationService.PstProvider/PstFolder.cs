using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PST;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class PstFolder : DisposeTrackableBase, IFolder, IDisposable
	{
		public PstFolder()
		{
		}

		public PstFxFolder Folder
		{
			get
			{
				return this.folder;
			}
		}

		public byte[] FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		FolderRec IFolder.GetFolderRec(PropTag[] additionalPtagsToLoad, GetFolderRecFlags flags)
		{
			if (this.folderRec != null)
			{
				return this.folderRec;
			}
			PropTag[] pta;
			if (additionalPtagsToLoad != null)
			{
				List<PropTag> list = new List<PropTag>();
				list.AddRange(FolderRec.PtagsToLoad);
				list.AddRange(additionalPtagsToLoad);
				pta = list.ToArray();
			}
			else
			{
				pta = FolderRec.PtagsToLoad;
			}
			PropertyValue[] momtPva = null;
			try
			{
				momtPva = this.folder.GetProps(PstMailbox.MoMTPtaFromPta(pta));
			}
			catch (PSTIOException innerException)
			{
				throw new UnableToGetPSTFolderPropsTransientException(BitConverter.ToUInt32(this.folderId, 0), innerException);
			}
			catch (PSTExceptionBase innerException2)
			{
				uint nodeIdFromEntryId = PstMailbox.GetNodeIdFromEntryId(this.folder.PstMailbox.IPst.MessageStore.Guid, this.folderId);
				throw new UnableToGetPSTFolderPropsPermanentException(nodeIdFromEntryId, innerException2);
			}
			this.folderRec = FolderRec.Create(PstMailbox.PvaFromMoMTPva(momtPva));
			return this.folderRec;
		}

		List<MessageRec> IFolder.EnumerateMessages(EnumerateMessagesFlags emFlags, PropTag[] additionalPtagsToLoad)
		{
			List<MessageRec> list = new List<MessageRec>();
			this.EnumerateMessagesHelper(this.folder.IPstFolder.AssociatedMessageIds, additionalPtagsToLoad, list);
			this.EnumerateMessagesHelper(this.folder.IPstFolder.MessageIds, additionalPtagsToLoad, list);
			MrsTracer.Provider.Debug("PstFolder.EnumerateMessages returns {0} items.", new object[]
			{
				list.Count
			});
			return list;
		}

		List<MessageRec> IFolder.LookupMessages(PropTag ptagToLookup, List<byte[]> keysToLookup, PropTag[] additionalPtagsToLoad)
		{
			if (ptagToLookup != PropTag.EntryId)
			{
				throw new NotImplementedException();
			}
			List<uint> list = new List<uint>(keysToLookup.Count);
			foreach (byte[] entryId in keysToLookup)
			{
				list.Add(PstMailbox.GetNodeIdFromEntryId(this.folder.PstMailbox.IPst.MessageStore.Guid, entryId));
			}
			List<MessageRec> list2 = new List<MessageRec>();
			this.EnumerateMessagesHelper(list, additionalPtagsToLoad, list2);
			return list2;
		}

		RawSecurityDescriptor IFolder.GetSecurityDescriptor(SecurityProp secProp)
		{
			throw new NotImplementedException();
		}

		void IFolder.DeleteMessages(byte[][] entryIds)
		{
			throw new NotImplementedException();
		}

		byte[] IFolder.GetFolderId()
		{
			return this.FolderId;
		}

		void IFolder.SetContentsRestriction(RestrictionData restriction)
		{
		}

		PropValueData[] IFolder.GetProps(PropTag[] pta)
		{
			MrsTracer.Provider.Function("PstFolder.IFolder.GetProps", new object[0]);
			PropValue[] a = PstMailbox.PvaFromMoMTPva(this.folder.GetProps(PstMailbox.MoMTPtaFromPta(pta)));
			return DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(a);
		}

		void IFolder.GetSearchCriteria(out RestrictionData restriction, out byte[][] entryIds, out SearchState state)
		{
			MrsTracer.Provider.Function("PstFolder.GetSearchCriteria", new object[0]);
			restriction = null;
			entryIds = null;
			state = SearchState.None;
		}

		RuleData[] IFolder.GetRules(PropTag[] extraProps)
		{
			throw new NotImplementedException();
		}

		PropValueData[][] IFolder.GetACL(SecurityProp secProp)
		{
			throw new NotImplementedException();
		}

		PropValueData[][] IFolder.GetExtendedAcl(AclFlags aclFlags)
		{
			throw new NotImplementedException();
		}

		PropProblemData[] IFolder.SetProps(PropValueData[] pvda)
		{
			MrsTracer.Provider.Function("PstFolder.SetProps(num of props={0})", new object[]
			{
				pvda.Length
			});
			foreach (PropValueData data in pvda)
			{
				this.Folder.PropertyBag.SetProperty(PstMailbox.MoMTPvFromPv(DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(data)));
			}
			try
			{
				this.Folder.IPstFolder.Save();
			}
			catch (PSTExceptionBase innerException)
			{
				throw new MailboxReplicationPermanentException(new LocalizedString(this.Folder.PstMailbox.IPst.FileName), innerException);
			}
			return null;
		}

		internal virtual void Config(byte[] folderId, PstFxFolder folder)
		{
			this.folderId = folderId;
			this.folder = folder;
			this.folderRec = null;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.folder != null)
			{
				this.folder.Dispose();
				this.folder = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PstFolder>(this);
		}

		private void EnumerateMessagesHelper(List<uint> messageIds, PropTag[] additionalPtagsToLoad, List<MessageRec> messageList)
		{
			foreach (uint num in messageIds)
			{
				IMessage message = null;
				PropValueData[] additionalProps = null;
				PropertyValue property = PstFolder.errorPropertyValue;
				PropertyValue property2 = PstFolder.errorPropertyValue;
				try
				{
					message = this.folder.PstMailbox.IPst.ReadMessage(num);
				}
				catch (PSTIOException innerException)
				{
					throw new UnableToGetPSTFolderPropsTransientException(BitConverter.ToUInt32(this.folderId, 0), innerException);
				}
				catch (PSTExceptionBase pstexceptionBase)
				{
					MrsTracer.Provider.Error("PstFolder.EnumerateMessages failed while reading message: {0}", new object[]
					{
						pstexceptionBase
					});
				}
				if (message != null)
				{
					using (PSTMessage pstmessage = new PSTMessage(this.folder.PstMailbox, message))
					{
						if (additionalPtagsToLoad != null)
						{
							additionalProps = new PropValueData[additionalPtagsToLoad.Length];
							PropertyValue[] array = new PropertyValue[additionalPtagsToLoad.Length];
							for (int i = 0; i < additionalPtagsToLoad.Length; i++)
							{
								array[i] = pstmessage.RawPropertyBag.GetProperty(PstMailbox.MoMTPtagFromPtag(additionalPtagsToLoad[i]));
							}
							additionalProps = DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(PstMailbox.PvaFromMoMTPva(array));
						}
						property = pstmessage.RawPropertyBag.GetProperty(PropertyTag.CreationTime);
						property2 = pstmessage.RawPropertyBag.GetProperty(PropertyTag.MessageSize);
					}
				}
				byte[] entryId = PstMailbox.CreateEntryIdFromNodeId(this.folder.PstMailbox.IPst.MessageStore.Guid, num);
				MessageRec item = new MessageRec(entryId, this.FolderId, property.IsError ? DateTime.MinValue : ((DateTime)((ExDateTime)property.Value)), property2.IsError ? 1000 : ((int)property2.Value), this.folder.PstMailbox.IPst.CheckIfAssociatedMessageId(num) ? MsgRecFlags.Associated : MsgRecFlags.None, additionalProps);
				messageList.Add(item);
			}
		}

		private static PropertyValue errorPropertyValue = PropertyValue.Error(PropertyId.Invalid, (ErrorCode)2147746063U);

		private byte[] folderId;

		private PstFxFolder folder;

		private FolderRec folderRec;
	}
}
