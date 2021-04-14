using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class FindParameters
	{
		internal string MessageId { get; private set; }

		internal RecipientTrackingEvent ReferralEvent { get; private set; }

		internal TrackingAuthority Authority { get; private set; }

		internal FindParameters(string messageId, RecipientTrackingEvent referralEvent, TrackingAuthority authority)
		{
			this.MessageId = messageId;
			this.ReferralEvent = referralEvent;
			this.Authority = authority;
		}

		public override string ToString()
		{
			SmtpAddress? smtpAddress = null;
			if (this.ReferralEvent.EventDescription == EventDescription.TransferredToPartnerOrg && this.ReferralEvent.EventData != null && this.ReferralEvent.EventData.Length > 0)
			{
				smtpAddress = new SmtpAddress?(new SmtpAddress(this.ReferralEvent.EventData[0]));
				if (!smtpAddress.Value.IsValidAddress)
				{
					TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Federated delivery email address invalid", new object[0]);
					TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "RecipientEvent Invalid: Federated Delivery Address incorrect {0}", new object[]
					{
						smtpAddress.ToString()
					});
				}
			}
			string text = null;
			string text2 = null;
			if (this.ReferralEvent.EventDescription == EventDescription.SmtpSendCrossSite)
			{
				string[] eventData = this.ReferralEvent.EventData;
				if (eventData == null || eventData.Length < 2)
				{
					TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "No server-data for XSite send", new object[0]);
					TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "RecipientEvent Invalid: No server-name for cross-site SMTP send event", new object[0]);
				}
				text = this.ReferralEvent.EventData[1];
			}
			if (this.ReferralEvent.EventDescription == EventDescription.PendingModeration)
			{
				text2 = this.ReferralEvent.ExtendedProperties.ArbitrationMailboxAddress;
			}
			return string.Format("Fed={0},Mid={1},Server={2},authority={3},ArbMbx={4}", new object[]
			{
				(smtpAddress == null) ? "<null>" : smtpAddress.Value.ToString(),
				this.MessageId,
				(text == null) ? "<null>" : text,
				this.Authority.ToString(),
				(text2 == null) ? "<null>" : text2
			});
		}
	}
}
