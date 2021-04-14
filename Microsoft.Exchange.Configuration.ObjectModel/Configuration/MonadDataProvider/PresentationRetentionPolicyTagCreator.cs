using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class PresentationRetentionPolicyTagCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"Name",
				"WhenChanged",
				"Type",
				"Comment",
				"AgeLimitForRetention",
				"RetentionEnabled",
				"RetentionAction"
			};
		}

		protected override void FillProperty(Type type, PSObject psObject, ConfigurableObject configObject, string propertyName)
		{
			if (propertyName == "RetentionEnabled" || propertyName == "AgeLimitForRetention" || propertyName == "RetentionAction")
			{
				PropertyInfo property = configObject.GetType().GetProperty(propertyName);
				property.SetValue(configObject, MockObjectCreator.GetSingleProperty(psObject.Members[propertyName].Value, property.PropertyType), null);
				return;
			}
			base.FillProperty(type, psObject, configObject, propertyName);
		}
	}
}
