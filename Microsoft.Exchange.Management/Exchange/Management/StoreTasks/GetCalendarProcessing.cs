using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Get", "CalendarProcessing")]
	public sealed class GetCalendarProcessing : GetXsoObjectWithIdentityTaskBase<CalendarConfiguration, ADUser>
	{
		public new PSCredential Credential
		{
			get
			{
				return base.Credential;
			}
			set
			{
				base.Credential = value;
			}
		}

		internal override IConfigDataProvider CreateXsoMailboxDataProvider(ExchangePrincipal principal, ISecurityAccessToken userToken)
		{
			return new CalendarConfigurationDataProvider(principal, "Get-CalendarProcessing");
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			IConfigurable configurable = base.ConvertDataObjectToPresentationObject(dataObject);
			CalendarConfiguration calendarConfiguration = configurable as CalendarConfiguration;
			if (calendarConfiguration != null)
			{
				calendarConfiguration.RequestOutOfPolicy = this.ResolveAndMerge(calendarConfiguration.RequestOutOfPolicyLegacy, calendarConfiguration.RequestOutOfPolicy);
				calendarConfiguration.BookInPolicy = this.ResolveAndMerge(calendarConfiguration.BookInPolicyLegacy, calendarConfiguration.BookInPolicy);
				calendarConfiguration.RequestInPolicy = this.ResolveAndMerge(calendarConfiguration.RequestInPolicyLegacy, calendarConfiguration.RequestInPolicy);
				if (dataObject is ADUser)
				{
					calendarConfiguration.ResourceDelegates = ((ADUser)dataObject).GrantSendOnBehalfTo;
				}
			}
			else
			{
				calendarConfiguration = new CalendarConfiguration();
				if (dataObject is ADUser)
				{
					calendarConfiguration.MailboxOwnerId = ((ADUser)dataObject).Id;
				}
			}
			return calendarConfiguration;
		}

		private MultiValuedProperty<string> ResolveAndMerge(MultiValuedProperty<ADObjectId> recipientList, MultiValuedProperty<string> existingAddress)
		{
			if (recipientList == null || recipientList.Count == 0)
			{
				return existingAddress;
			}
			MultiValuedProperty<string> multiValuedProperty = null;
			IRecipientSession tenantGlobalCatalogSession = base.TenantGlobalCatalogSession;
			Result<ADRecipient>[] array = tenantGlobalCatalogSession.ReadMultiple(recipientList.ToArray());
			multiValuedProperty = new MultiValuedProperty<string>();
			foreach (Result<ADRecipient> result in array)
			{
				if (result.Error == null && result.Data != null && !this.DoesExist(result.Data, existingAddress))
				{
					multiValuedProperty.Add(result.Data.LegacyExchangeDN);
				}
			}
			foreach (string item in existingAddress)
			{
				multiValuedProperty.Add(item);
			}
			return multiValuedProperty;
		}

		private bool DoesExist(ADRecipient recipient, MultiValuedProperty<string> existingAddress)
		{
			return existingAddress != null && existingAddress.Count != 0 && existingAddress.Contains(recipient.LegacyExchangeDN);
		}
	}
}
