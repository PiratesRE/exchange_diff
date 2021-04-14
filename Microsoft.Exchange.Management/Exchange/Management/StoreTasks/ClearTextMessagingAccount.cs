using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Clear", "TextMessagingAccount", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class ClearTextMessagingAccount : SetXsoObjectWithIdentityTaskBase<TextMessagingAccount>
	{
		public override object GetDynamicParameters()
		{
			return null;
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageClearTextMessagingAccount(this.Identity.ToString());
			}
		}

		internal override IConfigDataProvider CreateXsoMailboxDataProvider(ExchangePrincipal principal, ISecurityAccessToken userToken)
		{
			return new VersionedXmlDataProvider(principal, userToken, "Clear-TextMessagingAccount");
		}

		protected override void SaveXsoObject(IConfigDataProvider provider, IConfigurable dataObject)
		{
			Exception ex = null;
			try
			{
				TextMessagingHelper.SaveTextMessagingAccount((TextMessagingAccount)dataObject, (VersionedXmlDataProvider)provider, this.DataObject, (IRecipientSession)base.DataSession);
				MailboxTaskHelper.ProcessRecord(delegate
				{
					Rules inboxRules = ((VersionedXmlDataProvider)provider).MailboxSession.InboxRules;
					bool flag = false;
					foreach (Rule rule in inboxRules)
					{
						if (rule.IsEnabled)
						{
							bool flag2 = true;
							foreach (ActionBase actionBase in rule.Actions)
							{
								if (!(actionBase is SendSmsAlertToRecipientsAction) && !(actionBase is StopProcessingAction))
								{
									flag2 = false;
									break;
								}
							}
							if (flag2)
							{
								rule.IsEnabled = false;
								flag = true;
							}
						}
					}
					if (flag)
					{
						inboxRules.Save();
					}
				}, new MailboxTaskHelper.ThrowTerminatingErrorDelegate(base.WriteError), this.Identity);
			}
			catch (ObjectExistedException ex2)
			{
				ex = ex2;
			}
			catch (SaveConflictException ex3)
			{
				ex = ex3;
			}
			catch (ADObjectEntryAlreadyExistsException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				base.WriteError(new ConflictSavingException(this.DataObject.Identity.ToString(), ex), ErrorCategory.InvalidOperation, this.Identity);
			}
		}

		protected override void StampChangesOnXsoObject(IConfigurable dataObject)
		{
			TextMessagingAccount textMessagingAccount = (TextMessagingAccount)dataObject;
			textMessagingAccount.NotificationPhoneNumber = (E164Number)TextMessagingAccountSchema.NotificationPhoneNumber.DefaultValue;
			textMessagingAccount.CountryRegionId = (RegionInfo)TextMessagingAccountSchema.CountryRegionId.DefaultValue;
			textMessagingAccount.MobileOperatorId = (int)TextMessagingAccountSchema.MobileOperatorId.DefaultValue;
			textMessagingAccount.TextMessagingSettings.MachineToPersonMessagingPolicies.PossibleRecipients.Clear();
		}
	}
}
