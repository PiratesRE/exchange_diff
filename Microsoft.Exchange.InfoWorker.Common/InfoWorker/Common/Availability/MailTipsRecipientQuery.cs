using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class MailTipsRecipientQuery : RecipientQuery
	{
		internal MailTipsRecipientQuery(ClientContext clientContext, ADObjectId queryBaseDn, OrganizationId organizationId, DateTime queryPrepareDeadline) : base(clientContext, queryBaseDn, organizationId, queryPrepareDeadline, MailTipsRecipientQuery.RecipientProperties)
		{
		}

		internal RecipientQueryResults Query(ProxyAddress[] recipientAddressArray, ProxyAddress sendingAsAddress)
		{
			return base.Query(MailTipsRecipientQuery.GetEmailAddressArray(recipientAddressArray, sendingAsAddress));
		}

		internal RestrictionCheckResult CheckDeliveryRestriction(RecipientData senderData, RecipientData recipientData)
		{
			RestrictionCheckResult checkResult = RestrictionCheckResult.AcceptedNoPermissionList;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				checkResult = ADRecipientRestriction.CheckDeliveryRestriction(senderData.IsEmpty ? null : senderData.Id, true, recipientData.RejectMessagesFrom, recipientData.RejectMessagesFromDLMembers, recipientData.AcceptMessagesOnlyFrom, recipientData.AcceptMessagesOnlyFromDLMembers, recipientData.BypassModerationFrom, recipientData.BypassModerationFromDLMembers, recipientData.ModeratedBy, recipientData.ManagedBy, true, recipientData.ModerationEnabled, recipientData.RecipientType, this.ADRecipientSession, null, 150);
			});
			if (!adoperationResult.Succeeded)
			{
				recipientData.Exception = adoperationResult.Exception;
			}
			return checkResult;
		}

		private static EmailAddress[] GetEmailAddressArray(ProxyAddress[] recipientAddressArray, ProxyAddress sendingAsAddress)
		{
			EmailAddress[] array = new EmailAddress[recipientAddressArray.Length + 1];
			array[0] = ((null == sendingAsAddress) ? null : new EmailAddress(string.Empty, sendingAsAddress.AddressString, sendingAsAddress.PrefixString));
			for (int i = 0; i < recipientAddressArray.Length; i++)
			{
				array[i + 1] = new EmailAddress(string.Empty, recipientAddressArray[i].AddressString, recipientAddressArray[i].PrefixString);
			}
			return array;
		}

		private const int AdQueryLimit = 150;

		internal static readonly PropertyDefinition[] RecipientProperties = new PropertyDefinition[]
		{
			ADObjectSchema.OrganizationId,
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.RecipientDisplayType,
			ADRecipientSchema.RejectMessagesFrom,
			ADRecipientSchema.RejectMessagesFromDLMembers,
			ADRecipientSchema.AcceptMessagesOnlyFrom,
			ADRecipientSchema.AcceptMessagesOnlyFromDLMembers,
			ADRecipientSchema.BypassModerationFrom,
			ADRecipientSchema.BypassModerationFromDLMembers,
			ADRecipientSchema.ModeratedBy,
			ADRecipientSchema.ModerationEnabled,
			ADGroupSchema.ManagedBy,
			ADRecipientSchema.ExternalEmailAddress,
			ADRecipientSchema.MaxReceiveSize,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.LegacyExchangeDN,
			ADMailboxRecipientSchema.Database,
			ADRecipientSchema.MasterAccountSid,
			ADMailboxRecipientSchema.ServerLegacyDN,
			ADMailboxRecipientSchema.ExchangeGuid,
			ADMailboxRecipientSchema.Database,
			ADObjectSchema.ExchangeVersion,
			ADRecipientSchema.MailTipTranslations,
			ADObjectSchema.Id,
			ADGroupSchema.GroupMemberCount,
			ADGroupSchema.GroupExternalMemberCount
		};
	}
}
