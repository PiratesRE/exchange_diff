using System;
using System.Net;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class MailSubmissionResult
	{
		public MailSubmissionResult()
		{
			this.RemoteHostName = string.Empty;
		}

		public MailSubmissionResult(uint ec)
		{
			this.RemoteHostName = string.Empty;
		}

		public string DiagnosticInfo { get; set; }

		public uint ErrorCode { get; set; }

		public string From { get; set; }

		public string MessageId { get; set; }

		public string Sender { get; set; }

		public string Subject { get; set; }

		public OrganizationId OrganizationId { get; set; }

		public Guid ExternalOrganizationId { get; set; }

		public string[] RecipientAddresses { get; set; }

		public Guid NetworkMessageId { get; set; }

		public string RemoteHostName { get; set; }

		public IPAddress OriginalClientIPAddress { get; set; }

		public TimeSpan QuarantineTimeSpan { get; set; }
	}
}
