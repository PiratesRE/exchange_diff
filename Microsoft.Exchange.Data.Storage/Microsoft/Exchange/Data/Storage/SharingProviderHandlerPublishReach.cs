using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SharingProviderHandlerPublishReach : SharingProviderHandlerPublish
	{
		protected override PerformInvitationResults InternalPerformInvitation(MailboxSession mailboxSession, SharingContext context, ValidRecipient[] recipients, IFrontEndLocator frontEndLocator)
		{
			if (recipients.Length != 1)
			{
				throw new InvalidOperationException("Only single recipient is allowed.");
			}
			ValidRecipient validRecipient = recipients[0];
			string folderId = context.FolderId.ToBase64String();
			ExternalUser externalUser = this.ApplyPermission(mailboxSession, new SmtpAddress(validRecipient.SmtpAddress), context);
			IRecipientSession adrecipientSession = mailboxSession.GetADRecipientSession(false, ConsistencyMode.FullyConsistent);
			ADUser aduser = (ADUser)adrecipientSession.Read(mailboxSession.MailboxOwner.ObjectId);
			string text = aduser.SharingAnonymousIdentities.FindExistingUrlId(SharingDataType.ReachCalendar.PublishResourceName, folderId);
			Uri owaVdirUrl = OwaAnonymousVdirLocater.Instance.GetOwaVdirUrl(mailboxSession.MailboxOwner, frontEndLocator);
			SmtpAddress primarySmtpAddress = mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress;
			ObscureUrl obscureUrl = ObscureUrl.CreatePublishReachCalendarUrl(owaVdirUrl.ToString(), mailboxSession.MailboxGuid, primarySmtpAddress.Domain, text, externalUser.Sid);
			if (string.IsNullOrEmpty(text))
			{
				aduser.SharingAnonymousIdentities.AddOrUpdate(SharingDataType.ReachCalendar.PublishResourceName, obscureUrl.Identity, folderId);
				adrecipientSession.Save(aduser);
			}
			context.BrowseUrl = obscureUrl.ToString() + ".html";
			context.ICalUrl = obscureUrl.ToString() + ".ics";
			return new PerformInvitationResults(recipients);
		}

		protected override bool InternalValidateCompatibility(Folder folderToShare)
		{
			return folderToShare is CalendarFolder;
		}

		private ExternalUser ApplyPermission(MailboxSession mailboxSession, SmtpAddress smtpAddress, SharingContext context)
		{
			ExternalUser externalUser = null;
			ExternalUser externalUser2;
			using (ExternalUserCollection externalUsers = mailboxSession.GetExternalUsers())
			{
				externalUser2 = externalUsers.FindReachUserWithOriginalSmtpAddress(smtpAddress);
				externalUser = externalUsers.FindFederatedUserWithOriginalSmtpAddress(smtpAddress);
				if (externalUser2 == null)
				{
					externalUser2 = externalUsers.AddReachUser(smtpAddress);
					externalUsers.Save();
				}
			}
			using (FolderPermissionContext current = FolderPermissionContext.GetCurrent(mailboxSession, context))
			{
				FreeBusyAccess freeBusy = this.GetFreeBusy(context);
				PermissionSecurityPrincipal principal = new PermissionSecurityPrincipal(externalUser2);
				PermissionLevel permissionLevel = (context.SharingDetail == SharingContextDetailLevel.FullDetails) ? PermissionLevel.Reviewer : PermissionLevel.None;
				current.AddOrChangePermission(principal, permissionLevel, new FreeBusyAccess?(freeBusy));
				if (externalUser != null)
				{
					current.RemovePermission(new PermissionSecurityPrincipal(externalUser));
				}
			}
			return externalUser2;
		}

		private FreeBusyAccess GetFreeBusy(SharingContext context)
		{
			switch (context.SharingDetail)
			{
			case SharingContextDetailLevel.AvailabilityOnly:
				return FreeBusyAccess.Basic;
			case SharingContextDetailLevel.Limited:
				return FreeBusyAccess.Details;
			case SharingContextDetailLevel.FullDetails:
				return FreeBusyAccess.Details;
			}
			throw new ArgumentOutOfRangeException("context");
		}
	}
}
