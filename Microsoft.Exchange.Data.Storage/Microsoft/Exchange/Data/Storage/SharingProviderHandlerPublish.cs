using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SharingProviderHandlerPublish : SharingProviderHandler
	{
		internal SharingProviderHandlerPublish()
		{
		}

		internal override void FillSharingMessageProvider(SharingContext context, SharingMessageProvider sharingMessageProvider)
		{
			sharingMessageProvider.BrowseUrl = context.BrowseUrl.ToString();
			if (context.ICalUrl != null)
			{
				sharingMessageProvider.ICalUrl = context.ICalUrl.ToString();
			}
		}

		internal override void ParseSharingMessageProvider(SharingContext context, SharingMessageProvider sharingMessageProvider)
		{
			Uri uri = null;
			if (!PublishingUrl.IsAbsoluteUriString(sharingMessageProvider.BrowseUrl, out uri))
			{
				ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: BrowseUrl is invalid: {1}", context.UserLegacyDN, sharingMessageProvider.BrowseUrl);
				throw new InvalidSharingDataException("BrowseUrl", sharingMessageProvider.BrowseUrl);
			}
			context.BrowseUrl = sharingMessageProvider.BrowseUrl;
			if (context.DataType == SharingDataType.Calendar)
			{
				if (string.IsNullOrEmpty(sharingMessageProvider.ICalUrl))
				{
					ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: ICalUrl is missing: {1}", context.UserLegacyDN, sharingMessageProvider.ICalUrl);
					throw new InvalidSharingDataException("ICalUrl", string.Empty);
				}
				if (!PublishingUrl.IsAbsoluteUriString(sharingMessageProvider.ICalUrl, out uri))
				{
					ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: ICalUrl is invalid: {1}", context.UserLegacyDN, sharingMessageProvider.ICalUrl);
					throw new InvalidSharingDataException("ICalUrl", sharingMessageProvider.ICalUrl);
				}
				context.ICalUrl = sharingMessageProvider.ICalUrl;
			}
		}

		protected override bool InternalValidateCompatibility(Folder folderToShare)
		{
			CalendarFolder calendarFolder = folderToShare as CalendarFolder;
			return calendarFolder != null && calendarFolder.IsExchangePublishedCalendar;
		}

		protected override ValidRecipient InternalCheckOneRecipient(ADRecipient mailboxOwner, string recipient, IRecipientSession recipientSession)
		{
			ADRecipient adrecipient = recipientSession.FindByProxyAddress(new SmtpProxyAddress(recipient, false));
			if (adrecipient != null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient, string>((long)this.GetHashCode(), "{0}: {1} is found from AD in {0}'s organization.", mailboxOwner, recipient);
				return null;
			}
			return new ValidRecipient(recipient, adrecipient);
		}

		protected override PerformInvitationResults InternalPerformInvitation(MailboxSession mailboxSession, SharingContext context, ValidRecipient[] recipients, IFrontEndLocator frontEndLocator)
		{
			using (Folder folder = Folder.Bind(mailboxSession, context.FolderId))
			{
				context.PopulateUrls(folder);
			}
			return new PerformInvitationResults(recipients);
		}

		protected override void InternalPerformRevocation(MailboxSession mailboxSession, SharingContext context)
		{
			throw new NotSupportedException("cannot revoke published folder");
		}

		protected override SubscribeResults InternalPerformSubscribe(MailboxSession mailboxSession, SharingContext context)
		{
			if (context.ICalUrl == null)
			{
				throw new NotSupportedException("cannot subscribe to non ical url");
			}
			PublishingSubscriptionData newSubscription = this.CreateSubscriptionData(context);
			return WebCalendar.InternalSubscribe(mailboxSession, newSubscription, context.InitiatorSmtpAddress, context.InitiatorName);
		}

		private PublishingSubscriptionData CreateSubscriptionData(SharingContext context)
		{
			if (context.ICalUrl == null)
			{
				throw new InvalidSharingDataException("ICalUrl", string.Empty);
			}
			return new PublishingSubscriptionData(context.DataType.PublishName, new Uri(context.ICalUrl, UriKind.Absolute), context.FolderName, null);
		}
	}
}
