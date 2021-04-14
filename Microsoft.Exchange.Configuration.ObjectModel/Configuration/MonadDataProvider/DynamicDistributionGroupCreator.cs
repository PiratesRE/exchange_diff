using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class DynamicDistributionGroupCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"DisplayName",
				"Alias",
				"WhenChanged",
				"CustomAttribute1",
				"CustomAttribute2",
				"CustomAttribute3",
				"CustomAttribute4",
				"CustomAttribute5",
				"CustomAttribute6",
				"CustomAttribute7",
				"CustomAttribute8",
				"CustomAttribute9",
				"CustomAttribute10",
				"CustomAttribute11",
				"CustomAttribute12",
				"CustomAttribute13",
				"CustomAttribute14",
				"CustomAttribute15",
				"Name",
				"ManagedBy",
				"Notes",
				"RecipientContainer",
				"IncludedRecipients",
				"RecipientFilterMetadata",
				"RecipientFilter",
				"LdapRecipientFilter",
				"ConditionalCompany",
				"ConditionalDepartment",
				"ConditionalStateOrProvince",
				"ConditionalCustomAttribute1",
				"ConditionalCustomAttribute2",
				"ConditionalCustomAttribute3",
				"ConditionalCustomAttribute4",
				"ConditionalCustomAttribute5",
				"ConditionalCustomAttribute6",
				"ConditionalCustomAttribute7",
				"ConditionalCustomAttribute8",
				"ConditionalCustomAttribute9",
				"ConditionalCustomAttribute10",
				"ConditionalCustomAttribute11",
				"ConditionalCustomAttribute12",
				"ConditionalCustomAttribute13",
				"ConditionalCustomAttribute14",
				"ConditionalCustomAttribute15",
				"EmailAddressPolicyEnabled",
				"EmailAddresses",
				"SimpleDisplayName",
				"ExpansionServer",
				"HiddenFromAddressListsEnabled",
				"SendOofMessageToOriginatorEnabled",
				"ReportToManagerEnabled",
				"ReportToOriginatorEnabled",
				"MaxSendSize",
				"MaxReceiveSize",
				"AcceptMessagesOnlyFromSendersOrMembers",
				"RejectMessagesFromSendersOrMembers",
				"RequireSenderAuthenticationEnabled"
			};
		}

		protected override void FillProperty(Type type, PSObject psObject, ConfigurableObject configObject, string propertyName)
		{
			if (propertyName == "RequireSenderAuthenticationEnabled")
			{
				configObject.propertyBag[MailEnabledRecipientSchema.RequireSenderAuthenticationEnabled] = MockObjectCreator.GetSingleProperty(psObject.Members[propertyName].Value, MailEnabledRecipientSchema.RequireSenderAuthenticationEnabled.Type);
				return;
			}
			if (propertyName == "ManagedBy")
			{
				configObject.propertyBag[DynamicDistributionGroupSchema.ManagedBy] = MockObjectCreator.GetSingleProperty(psObject.Members[propertyName].Value, DynamicDistributionGroupSchema.ManagedBy.Type);
				return;
			}
			base.FillProperty(type, psObject, configObject, propertyName);
		}
	}
}
