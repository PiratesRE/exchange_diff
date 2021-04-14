using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PST;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PstFxFolder : IFolder, IDisposable
	{
		public PstFxFolder(PstMailbox pstMailbox, IFolder iPstFolder)
		{
			this.pstMailbox = pstMailbox;
			this.iPstFolder = iPstFolder;
			this.propertyBag = new PSTPropertyBag(pstMailbox, iPstFolder.PropertyBag);
		}

		public IPropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public bool IsContentAvailable
		{
			get
			{
				return this.iPstFolder.MessageIds.Count != 0 || this.iPstFolder.SubFolderIds.Count != 0;
			}
		}

		public PstMailbox PstMailbox
		{
			get
			{
				return this.pstMailbox;
			}
		}

		public IFolder IPstFolder
		{
			get
			{
				return this.iPstFolder;
			}
		}

		public void Dispose()
		{
		}

		public PropertyValue[] GetProps(PropertyTag[] pta)
		{
			PropertyValue[] array = new PropertyValue[pta.Length];
			for (int i = 0; i < pta.Length; i++)
			{
				if (pta[i] == PropertyTag.EntryId)
				{
					array[i] = new PropertyValue(PropertyTag.EntryId, PstMailbox.CreateEntryIdFromNodeId(this.pstMailbox.IPst.MessageStore.Guid, this.iPstFolder.Id));
				}
				else if (pta[i] == PropertyTag.ParentEntryId)
				{
					array[i] = new PropertyValue(PropertyTag.ParentEntryId, PstMailbox.CreateEntryIdFromNodeId(this.pstMailbox.IPst.MessageStore.Guid, this.iPstFolder.ParentId));
				}
				else if (pta[i] == PropertyTag.LastModificationTime)
				{
					array[i] = this.propertyBag.GetProperty(PropertyTag.LastModificationTime);
					if (array[i].IsError)
					{
						array[i] = new PropertyValue(PropertyTag.LastModificationTime, ExDateTime.MinValue);
					}
				}
				else if (pta[i] == PropertyTag.FolderType)
				{
					array[i] = this.propertyBag.GetProperty(PropertyTag.FolderType);
					if (array[i].IsError)
					{
						array[i] = new PropertyValue(PropertyTag.FolderType, (this.iPstFolder.Id == this.iPstFolder.ParentId) ? 0 : 1);
					}
				}
				else if (pta[i] == PropertyTag.DisplayName)
				{
					array[i] = this.propertyBag.GetProperty(PropertyTag.DisplayName);
					if (array[i].IsError)
					{
						array[i] = new PropertyValue(PropertyTag.DisplayName, (this.iPstFolder.Id == this.iPstFolder.ParentId) ? "Root of PST" : string.Format("[{0}]", this.iPstFolder.Id));
					}
				}
				else if (pta[i] == PstMailbox.MessageSizeExtended)
				{
					array[i] = new PropertyValue(PstMailbox.MessageSizeExtended, (long)this.iPstFolder.TotalComputedMessageSize);
				}
				else
				{
					array[i] = this.propertyBag.GetProperty(pta[i]);
				}
			}
			return array;
		}

		public PropertyValue GetProp(PropertyTag ptag)
		{
			PropertyValue[] props = this.GetProps(new PropertyTag[]
			{
				ptag
			});
			return props[0];
		}

		public IFolder CreateFolder()
		{
			throw new NotSupportedException();
		}

		public IMessage CreateMessage(bool isAssociatedMessage)
		{
			throw new NotSupportedException();
		}

		public IEnumerable<IFolder> GetFolders()
		{
			foreach (uint subFolderId in this.iPstFolder.SubFolderIds)
			{
				IFolder subFolder = this.pstMailbox.IPst.ReadFolder(subFolderId);
				yield return new PstFxFolder(this.pstMailbox, subFolder);
			}
			yield break;
		}

		public IEnumerable<IMessage> GetContents()
		{
			foreach (uint messageId in this.iPstFolder.MessageIds)
			{
				IMessage message = this.pstMailbox.IPst.ReadMessage(messageId);
				yield return new PSTMessage(this.pstMailbox, message);
			}
			yield break;
		}

		public IEnumerable<IMessage> GetAssociatedContents()
		{
			foreach (uint associatedMessageId in this.iPstFolder.AssociatedMessageIds)
			{
				IMessage message = this.pstMailbox.IPst.ReadMessage(associatedMessageId);
				yield return new PSTMessage(this.pstMailbox, message);
			}
			yield break;
		}

		public void Save()
		{
		}

		public string[] GetReplicaDatabases(out ushort localSiteDatabaseCount)
		{
			throw new NotSupportedException();
		}

		public StoreLongTermId GetLongTermId()
		{
			throw new NotSupportedException();
		}

		private PstMailbox pstMailbox;

		private IFolder iPstFolder;

		private PSTPropertyBag propertyBag;
	}
}
