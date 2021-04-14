using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SharingFolderManager : SharingFolderManagerBase<SharingSubscriptionData>
	{
		public SharingFolderManager(MailboxSession mailboxSession) : base(mailboxSession)
		{
		}

		protected override ExtendedFolderFlags SharingFolderFlags
		{
			get
			{
				return ExtendedFolderFlags.ReadOnly | ExtendedFolderFlags.SharedIn | ExtendedFolderFlags.PersonalShare | ExtendedFolderFlags.ExclusivelyBound | ExtendedFolderFlags.ExchangeCrossOrgShareFolder;
			}
		}

		protected override void CreateOrUpdateSharingBinding(SharingSubscriptionData sharingSubscriptionData, string localFolderName, StoreObjectId folderId)
		{
			SharingBindingData bindingData = SharingBindingData.CreateSharingBindingData(sharingSubscriptionData.DataType, sharingSubscriptionData.SharerName, sharingSubscriptionData.SharerIdentity, sharingSubscriptionData.RemoteFolderName, sharingSubscriptionData.RemoteFolderId, localFolderName, folderId, sharingSubscriptionData.IsPrimary);
			new SharingBindingManager(base.MailboxSession).CreateOrUpdateSharingBinding(bindingData);
		}

		private string ResolveInContacts(string smtpAddress)
		{
			using (ContactsFolder contactsFolder = ContactsFolder.Bind(base.MailboxSession, DefaultFolderType.Contacts))
			{
				using (FindInfo<Contact> findInfo = contactsFolder.FindByEmailAddress(smtpAddress, new PropertyDefinition[]
				{
					StoreObjectSchema.DisplayName
				}))
				{
					if (findInfo.FindStatus != FindStatus.NotFound && !string.IsNullOrEmpty(findInfo.Result.DisplayName))
					{
						ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, string, string>((long)this.GetHashCode(), "{0}: {1} is resolved as {2}", base.MailboxSession.MailboxOwner, smtpAddress, findInfo.Result.DisplayName);
						return findInfo.Result.DisplayName;
					}
				}
			}
			return smtpAddress;
		}

		protected override LocalizedString CreateLocalFolderName(SharingSubscriptionData sharingSubscriptionData)
		{
			string text = this.ResolveInContacts(sharingSubscriptionData.SharerIdentity).ToString(base.MailboxSession.InternalCulture);
			return sharingSubscriptionData.IsPrimary ? new LocalizedString(text) : ServerStrings.SharingFolderName(sharingSubscriptionData.RemoteFolderName.ToString(base.MailboxSession.InternalCulture), text);
		}

		private const ExtendedFolderFlags ExternalSharingFolderFlags = ExtendedFolderFlags.ReadOnly | ExtendedFolderFlags.SharedIn | ExtendedFolderFlags.PersonalShare | ExtendedFolderFlags.ExclusivelyBound | ExtendedFolderFlags.ExchangeCrossOrgShareFolder;
	}
}
