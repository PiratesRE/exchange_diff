using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class DomainContentConfigCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"Name",
				"WhenChanged",
				"AllowedOOFType",
				"DomainName",
				"AutoReplyEnabled",
				"AutoForwardEnabled",
				"DeliveryReportEnabled",
				"NDREnabled",
				"DisplaySenderName",
				"TNEFEnabled",
				"LineWrapSize",
				"CharacterSet",
				"NonMimeCharacterSet",
				"TargetDeliveryDomain"
			};
		}

		protected override void FillProperty(Type type, PSObject psObject, ConfigurableObject configObject, string propertyName)
		{
			if (propertyName == "DisplaySenderName")
			{
				configObject.propertyBag[DomainContentConfigSchema.DisplaySenderName] = MockObjectCreator.GetPropertyBasedOnDefinition(DomainContentConfigSchema.DisplaySenderName, psObject.Members[propertyName].Value);
			}
			base.FillProperty(type, psObject, configObject, propertyName);
		}
	}
}
