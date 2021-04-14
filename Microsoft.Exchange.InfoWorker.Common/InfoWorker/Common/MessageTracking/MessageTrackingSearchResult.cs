using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	[Serializable]
	internal sealed class MessageTrackingSearchResult
	{
		internal static MessageTrackingSearchResult Create(FindMessageTrackingSearchResultType wsResult, string targetInfoForDisplay)
		{
			if (wsResult.Sender == null)
			{
				TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "WS-Response Validation Error: Sender is null in FindMessageTrackingReport response from {0}", new object[]
				{
					targetInfoForDisplay
				});
			}
			SmtpAddress smtpAddress = new SmtpAddress(wsResult.Sender.EmailAddress);
			if (!smtpAddress.IsValidAddress || string.IsNullOrEmpty(wsResult.Sender.Name))
			{
				TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "WS-Response Validation Error: Sender {0} is invalid in FindMessageTrackingReport response from {1}", new object[]
				{
					smtpAddress.ToString(),
					targetInfoForDisplay
				});
			}
			smtpAddress = SmtpAddress.Parse(wsResult.Sender.EmailAddress);
			string name = wsResult.Sender.Name;
			EmailAddressType[] recipients = wsResult.Recipients;
			SmtpAddress[] array = new SmtpAddress[recipients.Length];
			if (recipients == null || recipients.Length == 0)
			{
				TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "WS-Response Validation Error: No recipients in FindMessageTrackingReport response from {0}", new object[]
				{
					targetInfoForDisplay
				});
			}
			for (int i = 0; i < recipients.Length; i++)
			{
				if (recipients[i] == null)
				{
					TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "WS-Response Validation Error: Null recipient in FindMessageTrackingReport response from {0}", new object[]
					{
						targetInfoForDisplay
					});
				}
				array[i] = new SmtpAddress(recipients[i].EmailAddress);
				if (!array[i].IsValidAddress)
				{
					TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "WS-Response Validation Error: Invalid Recipient {0} in FindMessageTrackingReport response from {1}", new object[]
					{
						array[i].ToString(),
						targetInfoForDisplay
					});
				}
			}
			MessageTrackingReportId identity = null;
			if (!MessageTrackingReportId.TryParse(wsResult.MessageTrackingReportId, out identity))
			{
				TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "WS-Response Validation Error: Invalid report ID {0} in FindMessageTrackingReport response from {1}", new object[]
				{
					wsResult.MessageTrackingReportId,
					targetInfoForDisplay
				});
			}
			return new MessageTrackingSearchResult(identity, smtpAddress, name, array, wsResult.Subject, wsResult.SubmittedTime, wsResult.PreviousHopServer, wsResult.FirstHopServer);
		}

		internal static int CompareSearchResultsByTimeStampDescending(MessageTrackingSearchResult leftValue, MessageTrackingSearchResult rightValue)
		{
			if (leftValue == null)
			{
				throw new ArgumentNullException("leftValue");
			}
			if (rightValue == null)
			{
				throw new ArgumentNullException("rightValue");
			}
			return rightValue.submittedDateTime.CompareTo(leftValue.submittedDateTime);
		}

		public MessageTrackingSearchResult()
		{
		}

		public MessageTrackingSearchResult(MessageTrackingReportId identity, SmtpAddress fromAddress, string fromDisplayName, SmtpAddress[] recipientAddresses, string subject, DateTime submittedDateTime, string previousHopServer, string firstHopServer)
		{
			this.messageTrackingReportId = identity;
			this.fromAddress = fromAddress;
			this.fromDisplayName = fromDisplayName;
			this.recipientAddresses = recipientAddresses;
			this.submittedDateTime = submittedDateTime;
			this.subject = subject;
			this.previousHopServer = previousHopServer;
			this.firstHopServer = firstHopServer;
		}

		public MessageTrackingReportId MessageTrackingReportId
		{
			get
			{
				return this.messageTrackingReportId;
			}
		}

		public string PreviousHopServer
		{
			get
			{
				return this.previousHopServer;
			}
		}

		public DateTime SubmittedDateTime
		{
			get
			{
				return this.submittedDateTime;
			}
			internal set
			{
				this.submittedDateTime = value;
			}
		}

		public string Subject
		{
			get
			{
				return this.subject;
			}
			internal set
			{
				this.subject = value;
			}
		}

		public SmtpAddress FromAddress
		{
			get
			{
				return this.fromAddress;
			}
			set
			{
				this.fromAddress = value;
			}
		}

		public string FromDisplayName
		{
			get
			{
				return this.fromDisplayName;
			}
			set
			{
				this.fromDisplayName = value;
			}
		}

		public SmtpAddress[] RecipientAddresses
		{
			get
			{
				return this.recipientAddresses;
			}
			internal set
			{
				this.recipientAddresses = value;
			}
		}

		public string FirstHopServer
		{
			get
			{
				return this.firstHopServer;
			}
		}

		private MessageTrackingReportId messageTrackingReportId;

		private string previousHopServer;

		private string firstHopServer;

		private string subject;

		private DateTime submittedDateTime;

		private SmtpAddress fromAddress;

		private string fromDisplayName;

		private SmtpAddress[] recipientAddresses;
	}
}
