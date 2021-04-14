using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class RetentionPolicyCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"Name",
				"WhenChanged",
				"RetentionPolicyTagLinks",
				"ObjectId"
			};
		}

		protected override void FillProperty(Type type, PSObject psObject, ConfigurableObject configObject, string propertyName)
		{
			if (propertyName == "RetentionPolicyTagLinks")
			{
				configObject.propertyBag[RetentionPolicySchema.RetentionPolicyTagLinks] = MockObjectCreator.GetPropertyBasedOnDefinition(RetentionPolicySchema.RetentionPolicyTagLinks, psObject.Members[propertyName].Value);
				return;
			}
			base.FillProperty(type, psObject, configObject, propertyName);
		}
	}
}
