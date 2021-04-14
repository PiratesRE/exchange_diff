using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Approval;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal abstract class ApprovalRequestWriter : IDisposable
	{
		public virtual bool SupportMultipleRequestsForDifferentCultures
		{
			get
			{
				return false;
			}
		}

		public static ApprovalRequestWriter GetInstance(ApprovalApplicationId? applicationId, OrganizationId organizationId, InitiationMessage initiationMessage)
		{
			if (applicationId != null)
			{
				switch (applicationId.Value)
				{
				case ApprovalApplicationId.AutoGroup:
					return AutoGroupApprovalRequestWriter.GetInstance(initiationMessage);
				case ApprovalApplicationId.ModeratedRecipient:
					return ModerationApprovalRequestWriter.GetInstance(organizationId, initiationMessage);
				}
			}
			return DefaultApprovalRequestWriter.GetInstance(initiationMessage);
		}

		public static string FormatApprovalRequestMessageId(string local, int identifier, string domain, bool addAngleBrackets)
		{
			if (addAngleBrackets)
			{
				return string.Concat(new object[]
				{
					'<',
					local,
					'-',
					identifier.ToString(NumberFormatInfo.InvariantInfo),
					'@',
					domain,
					'>'
				});
			}
			return string.Concat(new object[]
			{
				local,
				'-',
				identifier.ToString(NumberFormatInfo.InvariantInfo),
				'@',
				domain
			});
		}

		public static string FormatStoredApprovalRequestMessageId(string local, string domain)
		{
			return string.Concat(new object[]
			{
				'<',
				local,
				'@',
				domain,
				'>'
			});
		}

		public abstract bool WriteSubjectAndBody(MessageItemApprovalRequest approvalRequest, CultureInfo cultureInfo, out CultureInfo cultureInfoWritten);

		public virtual void Dispose()
		{
		}
	}
}
