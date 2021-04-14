using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Set", "TextMessagingAccount", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetTextMessagingAccount : SetXsoObjectWithIdentityTaskBase<TextMessagingAccount>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetTextMessagingAccount(this.Identity.ToString());
			}
		}

		internal override IConfigDataProvider CreateXsoMailboxDataProvider(ExchangePrincipal principal, ISecurityAccessToken userToken)
		{
			return new VersionedXmlDataProvider(principal, userToken, "Set-TextMessagingAccount");
		}

		protected override void StampChangesOnXsoObject(IConfigurable dataObject)
		{
			TextMessagingAccount textMessagingAccount = (TextMessagingAccount)dataObject;
			TextMessagingAccount textMessagingAccount2 = new TextMessagingAccount();
			textMessagingAccount2.TextMessagingSettings = textMessagingAccount.TextMessagingSettings;
			textMessagingAccount2.ResetChangeTracking();
			base.StampChangesOnXsoObject(textMessagingAccount2);
			List<ProviderPropertyDefinition> list = new List<ProviderPropertyDefinition>();
			bool flag = false;
			int num = 0;
			while (SetTextMessagingAccount.propertyDependencies.Length - 1 > num)
			{
				ProviderPropertyDefinition providerPropertyDefinition = SetTextMessagingAccount.propertyDependencies[num];
				ProviderPropertyDefinition providerPropertyDefinition2 = SetTextMessagingAccount.propertyDependencies[1 + num];
				bool flag2 = !object.Equals(textMessagingAccount2[providerPropertyDefinition], textMessagingAccount[providerPropertyDefinition]);
				bool flag3 = ((TextMessagingAccount)this.GetDynamicParameters()).IsModified(providerPropertyDefinition2);
				if (flag2 && typeof(RegionInfo) == providerPropertyDefinition.Type && textMessagingAccount[providerPropertyDefinition] != null && textMessagingAccount2[providerPropertyDefinition] != null)
				{
					flag2 = (((RegionInfo)textMessagingAccount[providerPropertyDefinition]).GeoId != ((RegionInfo)textMessagingAccount2[providerPropertyDefinition]).GeoId);
				}
				if (!flag)
				{
					flag = (flag2 && !flag3);
				}
				if (flag && !object.Equals(providerPropertyDefinition2.DefaultValue, textMessagingAccount[providerPropertyDefinition2]) && !list.Contains(providerPropertyDefinition2))
				{
					list.Add(providerPropertyDefinition2);
				}
				num++;
			}
			if (0 < list.Count)
			{
				foreach (ProviderPropertyDefinition providerPropertyDefinition3 in list)
				{
					textMessagingAccount2[providerPropertyDefinition3] = providerPropertyDefinition3.DefaultValue;
				}
			}
			if (textMessagingAccount2.TextMessagingSettings.MachineToPersonMessagingPolicies.EffectivePossibleRecipients.Count == 0)
			{
				base.WriteError(new IncompleteSettingsException(), ErrorCategory.InvalidOperation, this.Identity);
			}
			if (!object.Equals(textMessagingAccount.CountryRegionId, textMessagingAccount2.CountryRegionId) || !object.Equals(textMessagingAccount.MobileOperatorId, textMessagingAccount2.MobileOperatorId) || !object.Equals(textMessagingAccount.NotificationPhoneNumber, textMessagingAccount2.NotificationPhoneNumber))
			{
				foreach (PossibleRecipient possibleRecipient in textMessagingAccount2.TextMessagingSettings.MachineToPersonMessagingPolicies.PossibleRecipients)
				{
					if (possibleRecipient.Acknowledged)
					{
						possibleRecipient.SetAcknowledged(false);
					}
				}
			}
			textMessagingAccount.TextMessagingSettings = textMessagingAccount2.TextMessagingSettings;
		}

		protected override void SaveXsoObject(IConfigDataProvider provider, IConfigurable dataObject)
		{
			Exception ex = null;
			try
			{
				TextMessagingHelper.SaveTextMessagingAccount((TextMessagingAccount)dataObject, (VersionedXmlDataProvider)provider, this.DataObject, (IRecipientSession)base.DataSession);
				CalendarNotification calendarNotification = (CalendarNotification)provider.Read<CalendarNotification>(this.DataObject.Identity);
				if (calendarNotification.TextMessagingPhoneNumber != ((TextMessagingAccount)dataObject).NotificationPhoneNumber)
				{
					calendarNotification.TextMessagingPhoneNumber = ((TextMessagingAccount)dataObject).NotificationPhoneNumber;
					provider.Save(calendarNotification);
				}
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

		private static ProviderPropertyDefinition[] propertyDependencies = new ProviderPropertyDefinition[]
		{
			TextMessagingAccountSchema.CountryRegionId,
			TextMessagingAccountSchema.MobileOperatorId,
			TextMessagingAccountSchema.NotificationPhoneNumber
		};
	}
}
