using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class TargetFolderMruConfiguration
	{
		[DataMember]
		public TargetFolderMRUEntry[] FolderMruEntries
		{
			get
			{
				return this.folderMruEntries;
			}
			set
			{
				this.folderMruEntries = value;
			}
		}

		internal void LoadAll(CallContext callContext)
		{
			SimpleConfiguration<TargetFolderMRUEntry> simpleConfiguration = new SimpleConfiguration<TargetFolderMRUEntry>();
			simpleConfiguration.Load(callContext);
			bool flag = this.ConvertLegacyItemIdFormatIfNecessary(simpleConfiguration, callContext);
			this.PopulateConfigEntries(simpleConfiguration.Entries);
			if (flag)
			{
				this.Save(callContext);
			}
		}

		internal void Save(CallContext callContext)
		{
			new SimpleConfiguration<TargetFolderMRUEntry>
			{
				Entries = new List<TargetFolderMRUEntry>(this.folderMruEntries)
			}.Save(callContext);
		}

		private bool ConvertLegacyItemIdFormatIfNecessary(SimpleConfiguration<TargetFolderMRUEntry> folderMruConfig, CallContext callContext)
		{
			bool result = false;
			foreach (TargetFolderMRUEntry targetFolderMRUEntry in folderMruConfig.Entries)
			{
				if (targetFolderMRUEntry.EwsFolderIdEntry == null)
				{
					try
					{
						OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromString(targetFolderMRUEntry.FolderId);
						ExchangePrincipal exchangePrincipal = (owaStoreObjectId.MailboxOwnerLegacyDN != null) ? ExchangePrincipal.FromLegacyDN(callContext.SessionCache.GetMailboxIdentityMailboxSession().GetADSessionSettings(), owaStoreObjectId.MailboxOwnerLegacyDN) : callContext.AccessingPrincipal;
						new IdConverter(callContext);
						FolderId folderIdFromStoreId = IdConverter.GetFolderIdFromStoreId(owaStoreObjectId.StoreId, new MailboxId(exchangePrincipal.MailboxInfo.MailboxGuid));
						targetFolderMRUEntry.EwsFolderIdEntry = folderIdFromStoreId.Id;
						result = true;
					}
					catch (OwaInvalidIdFormatException)
					{
					}
					catch (OwaInvalidRequestException)
					{
					}
					catch (ObjectNotFoundException)
					{
					}
				}
			}
			return result;
		}

		private void PopulateConfigEntries(IList<TargetFolderMRUEntry> entries)
		{
			this.folderMruEntries = new TargetFolderMRUEntry[entries.Count];
			for (int i = 0; i < entries.Count; i++)
			{
				this.folderMruEntries[i] = entries[i];
			}
		}

		private TargetFolderMRUEntry[] folderMruEntries;
	}
}
