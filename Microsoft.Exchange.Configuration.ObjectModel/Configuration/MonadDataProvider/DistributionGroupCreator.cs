﻿using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class DistributionGroupCreator : ConfigurableObjectCreator
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
				"SamAccountName",
				"ManagedBy",
				"MemberJoinRestriction",
				"MemberDepartRestriction",
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
				"RequireSenderAuthenticationEnabled",
				"ModerationEnabled",
				"ModeratedBy",
				"BypassModerationFromSendersOrMembers",
				"SendModerationNotifications"
			};
		}

		protected override void FillProperty(Type type, PSObject psObject, ConfigurableObject configObject, string propertyName)
		{
			if (propertyName == "RequireSenderAuthenticationEnabled")
			{
				configObject.propertyBag[MailEnabledRecipientSchema.RequireSenderAuthenticationEnabled] = MockObjectCreator.GetSingleProperty(psObject.Members[propertyName].Value, MailEnabledRecipientSchema.RequireSenderAuthenticationEnabled.Type);
				return;
			}
			base.FillProperty(type, psObject, configObject, propertyName);
		}
	}
}
