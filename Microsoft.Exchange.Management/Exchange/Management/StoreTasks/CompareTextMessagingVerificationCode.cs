using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.SQM;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Compare", "TextMessagingVerificationCode", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class CompareTextMessagingVerificationCode : RecipientObjectActionTask<MailboxIdParameter, ADUser>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageCompareTextMessagingVerificationCode(this.Identity.ToString());
			}
		}

		[Parameter(ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0, ParameterSetName = "Identity")]
		public override MailboxIdParameter Identity
		{
			get
			{
				if (base.Identity != null)
				{
					return base.Identity;
				}
				ADObjectId adObjectId;
				if (!base.TryGetExecutingUserId(out adObjectId))
				{
					throw new ExecutingUserPropertyNotFoundException("executingUserid");
				}
				return base.Identity = new MailboxIdParameter(adObjectId);
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string VerificationCode
		{
			get
			{
				return (string)base.Fields["VerificationCode"];
			}
			set
			{
				base.Fields["VerificationCode"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			using (VersionedXmlDataProvider versionedXmlDataProvider = new VersionedXmlDataProvider(XsoStoreDataProviderBase.GetExchangePrincipalWithAdSessionSettingsForOrg(base.SessionSettings.CurrentOrganizationId, this.DataObject), (base.ExchangeRunspaceConfig == null) ? null : base.ExchangeRunspaceConfig.SecurityAccessToken, "Compare-TextMessagingVerificationCode"))
			{
				TextMessagingAccount textMessagingAccount = (TextMessagingAccount)versionedXmlDataProvider.Read<TextMessagingAccount>(this.DataObject.Identity);
				IList<PossibleRecipient> effectivePossibleRecipients = textMessagingAccount.TextMessagingSettings.MachineToPersonMessagingPolicies.EffectivePossibleRecipients;
				if (effectivePossibleRecipients.Count == 0)
				{
					base.WriteError(new NotificationPhoneNumberAbsentException(this.Identity.ToString()), ErrorCategory.InvalidData, this.Identity);
				}
				PossibleRecipient possibleRecipient = effectivePossibleRecipients[0];
				bool flag = false;
				if (string.IsNullOrEmpty(possibleRecipient.Passcode))
				{
					base.WriteError(new VerificationCodeNeverSentException(this.Identity.ToString()), ErrorCategory.InvalidData, this.Identity);
				}
				DateTime utcNow = DateTime.UtcNow;
				if (6 <= PossibleRecipient.CountTimesSince(possibleRecipient.PasscodeVerificationFailedTimeHistory, utcNow - TimeSpan.FromDays(1.0), true))
				{
					base.WriteError(new VerificationCodeTooManyFailedException(), ErrorCategory.InvalidData, this.Identity);
				}
				if (string.Equals(possibleRecipient.Passcode, this.VerificationCode, StringComparison.InvariantCultureIgnoreCase))
				{
					possibleRecipient.SetAcknowledged(true);
					ADUser dataObject = this.DataObject;
					SmsSqmDataPointHelper.AddNotificationTurningOnDataPoint(SmsSqmSession.Instance, dataObject.Id, dataObject.LegacyExchangeDN, textMessagingAccount);
				}
				else
				{
					possibleRecipient.PasscodeVerificationFailedTimeHistory.Add(utcNow);
					flag = true;
				}
				versionedXmlDataProvider.Save(textMessagingAccount);
				if (flag)
				{
					base.WriteError(new VerificationCodeUnmatchException(this.VerificationCode), ErrorCategory.InvalidData, this.Identity);
				}
				else if (!textMessagingAccount.EasEnabled)
				{
					TextMessagingHelper.SendSystemTextMessage(versionedXmlDataProvider.MailboxSession, textMessagingAccount.NotificationPhoneNumber, Strings.CalendarNotificationConfirmation.ToString(textMessagingAccount.NotificationPreferredCulture ?? TextMessagingHelper.GetSupportedUserCulture(this.DataObject)), true);
				}
			}
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}

		private const string ParamVerificationCode = "VerificationCode";
	}
}
