using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class SharingProviderHandler
	{
		internal abstract void FillSharingMessageProvider(SharingContext context, SharingMessageProvider sharingMessageProvider);

		internal abstract void ParseSharingMessageProvider(SharingContext context, SharingMessageProvider sharingMessageProvider);

		protected abstract bool InternalValidateCompatibility(Folder folderToShare);

		internal bool ValidateCompatibility(Folder folderToShare)
		{
			Util.ThrowOnNullArgument(folderToShare, "folderToShare");
			if (folderToShare.Session is PublicFolderSession)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Cannot share public folder.", folderToShare.Session.UserLegacyDN);
				throw new CannotShareFolderException(ServerStrings.CannotSharePublicFolder);
			}
			if (folderToShare.Id.ObjectId.ObjectType == StoreObjectType.SearchFolder)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Cannot share search folder.", folderToShare.Session.UserLegacyDN);
				throw new CannotShareFolderException(ServerStrings.CannotShareSearchFolder);
			}
			object obj = folderToShare.TryGetProperty(FolderSchema.ExtendedFolderFlags);
			if (!(obj is PropertyError))
			{
				int num = (int)obj;
				if ((num & 8192) != 0)
				{
					ExTraceGlobals.SharingTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Cannot share other people's personal folder.", folderToShare.Session.UserLegacyDN);
					throw new CannotShareFolderException(ServerStrings.CannotShareOtherPersonalFolder);
				}
			}
			return this.InternalValidateCompatibility(folderToShare);
		}

		protected abstract ValidRecipient InternalCheckOneRecipient(ADRecipient mailboxOwner, string recipient, IRecipientSession recipientSession);

		internal CheckRecipientsResults CheckRecipients(ADRecipient mailboxOwner, string[] recipients)
		{
			Util.ThrowOnNullArgument(mailboxOwner, "mailboxOwner");
			Util.ThrowOnNullArgument(recipients, "recipients");
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.FullyConsistent, mailboxOwner.Session.NetworkCredential, mailboxOwner.OrganizationId.ToADSessionSettings(), 127, "CheckRecipients", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\SharingProviderHandler.cs");
			List<ValidRecipient> list = new List<ValidRecipient>(recipients.Length);
			List<string> list2 = new List<string>(recipients.Length);
			foreach (string text in recipients)
			{
				ValidRecipient validRecipient = this.InternalCheckOneRecipient(mailboxOwner, text, tenantOrRootOrgRecipientSession);
				if (validRecipient != null)
				{
					ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient, string, SharingProviderHandler>((long)this.GetHashCode(), "{0}: {1} is a valid recipient for sharing handler {2}.", mailboxOwner, text, this);
					list.Add(validRecipient);
				}
				else
				{
					ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient, string, SharingProviderHandler>((long)this.GetHashCode(), "{0}: {1} is an invalid recipient for sharing handler {2}.", mailboxOwner, text, this);
					list2.Add(text);
				}
			}
			return new CheckRecipientsResults(list.ToArray(), list2.ToArray());
		}

		protected abstract PerformInvitationResults InternalPerformInvitation(MailboxSession mailboxSession, SharingContext context, ValidRecipient[] recipients, IFrontEndLocator frontEndLocator);

		internal PerformInvitationResults PerformInvitation(MailboxSession mailboxSession, SharingContext context, ValidRecipient[] recipients, IFrontEndLocator frontEndLocator)
		{
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			Util.ThrowOnNullArgument(context, "context");
			Util.ThrowOnNullArgument(recipients, "recipients");
			Util.ThrowOnNullArgument(frontEndLocator, "frontEndLocator");
			if (recipients.Length == 0)
			{
				return PerformInvitationResults.Ignored;
			}
			if (!context.SharingMessageType.IsInvitationOrAcceptOfRequest)
			{
				return PerformInvitationResults.Ignored;
			}
			return this.InternalPerformInvitation(mailboxSession, context, recipients, frontEndLocator);
		}

		protected abstract void InternalPerformRevocation(MailboxSession mailboxSession, SharingContext context);

		internal void PerformRevocation(MailboxSession mailboxSession, SharingContext context)
		{
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			Util.ThrowOnNullArgument(context, "context");
			if (!context.SharingMessageType.IsRequest)
			{
				throw new InvalidOperationException("Only can revoke on request message.");
			}
			context.FolderId = mailboxSession.GetDefaultFolderId(context.DataType.DefaultFolderType);
			this.InternalPerformRevocation(mailboxSession, context);
		}

		protected abstract SubscribeResults InternalPerformSubscribe(MailboxSession mailboxSession, SharingContext context);

		internal SubscribeResults PerformSubscribe(MailboxSession mailboxSession, SharingContext context)
		{
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			Util.ThrowOnNullArgument(context, "context");
			if (!context.SharingMessageType.IsInvitationOrAcceptOfRequest)
			{
				throw new InvalidOperationException("Only can subscribe on invitation or acceptofrequest message.");
			}
			return this.InternalPerformSubscribe(mailboxSession, context);
		}

		private const int MaxRetryTime = 3;
	}
}
