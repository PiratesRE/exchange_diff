using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage.Approval;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class AutoGroupApprovalRequestWriter : ApprovalRequestWriter
	{
		private AutoGroupApprovalRequestWriter(InitiationMessage initiationMessage)
		{
			this.initiationMessage = initiationMessage;
		}

		public override bool WriteSubjectAndBody(MessageItemApprovalRequest approvalRequest, CultureInfo cultureInfo, out CultureInfo cultureInfoWritten)
		{
			approvalRequest.MessageItem.Subject = this.initiationMessage.Subject;
			cultureInfoWritten = null;
			int? messageItemLocale = this.initiationMessage.MessageItemLocale;
			string address = (string)this.initiationMessage.Requestor;
			string approvalData = this.initiationMessage.ApprovalData;
			if (string.IsNullOrEmpty(approvalData))
			{
				return false;
			}
			Culture culture = null;
			if (messageItemLocale != null && Culture.TryGetCulture(messageItemLocale.Value, out culture))
			{
				cultureInfoWritten = culture.GetCultureInfo();
			}
			else
			{
				cultureInfoWritten = cultureInfo;
			}
			string displayNameFromSmtpAddress = ApprovalProcessor.GetDisplayNameFromSmtpAddress(address);
			string group = ApprovalProcessor.ResolveDisplayNameForDistributionGroupFromApprovalData(approvalData, ApprovalProcessor.CreateRecipientSessionFromSmtpAddress(address));
			string body = ApprovalProcessor.GenerateMessageBodyForRequestMessage(Strings.AutoGroupRequestHeader(displayNameFromSmtpAddress, group), Strings.AutoGroupRequestBody, LocalizedString.Empty, cultureInfoWritten);
			approvalRequest.SetBody(body);
			return true;
		}

		internal static AutoGroupApprovalRequestWriter GetInstance(InitiationMessage initiationMessage)
		{
			return new AutoGroupApprovalRequestWriter(initiationMessage);
		}

		private InitiationMessage initiationMessage;
	}
}
