using System;
using System.Globalization;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class DefaultApprovalRequestWriter : ApprovalRequestWriter
	{
		private DefaultApprovalRequestWriter(InitiationMessage initiationMessage)
		{
			this.initiationMessage = initiationMessage;
		}

		public override bool WriteSubjectAndBody(MessageItemApprovalRequest approvalRequest, CultureInfo cultureInfo, out CultureInfo cultureInfoWritten)
		{
			cultureInfoWritten = null;
			approvalRequest.MessageItem.Subject = this.initiationMessage.Subject;
			approvalRequest.SetBody(this.initiationMessage.EmailMessage.Body);
			return true;
		}

		internal static DefaultApprovalRequestWriter GetInstance(InitiationMessage initiationMessage)
		{
			return new DefaultApprovalRequestWriter(initiationMessage);
		}

		private InitiationMessage initiationMessage;
	}
}
