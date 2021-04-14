using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class RecipientMessageTrackingReportId
	{
		public RecipientMessageTrackingReportId(string messageTractingReportId, string recipient)
		{
			this.MessageTrackingReportId = messageTractingReportId;
			this.Recipient = recipient;
			this.RawIdentity = messageTractingReportId + ((recipient != null) ? (",Recip=" + recipient) : string.Empty);
		}

		private RecipientMessageTrackingReportId(string rawIdentity)
		{
			this.RawIdentity = rawIdentity;
		}

		public string Recipient { get; private set; }

		public string MessageTrackingReportId { get; private set; }

		public string RawIdentity { get; private set; }

		public static RecipientMessageTrackingReportId Parse(Identity identity)
		{
			return RecipientMessageTrackingReportId.Parse(identity.RawIdentity);
		}

		public static RecipientMessageTrackingReportId Parse(string rawIdentity)
		{
			RecipientMessageTrackingReportId recipientMessageTrackingReportId = new RecipientMessageTrackingReportId(rawIdentity);
			recipientMessageTrackingReportId.ParseRawIdentity();
			return recipientMessageTrackingReportId;
		}

		private void ParseRawIdentity()
		{
			int num = this.RawIdentity.LastIndexOf(",Recip=");
			if (num > 0)
			{
				this.MessageTrackingReportId = this.RawIdentity.Substring(0, num);
				this.Recipient = this.RawIdentity.Substring(num + ",Recip=".Length);
				return;
			}
			this.MessageTrackingReportId = this.RawIdentity;
		}

		internal const string RecipientParameterPrefix = ",Recip=";
	}
}
