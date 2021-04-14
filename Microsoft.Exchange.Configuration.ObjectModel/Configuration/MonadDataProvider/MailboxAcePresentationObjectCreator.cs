using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MailboxAcePresentationObjectCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return MailboxAcePresentationObjectCreator.mbxAcePresentationObjProperties;
		}

		protected override void FillProperty(Type type, PSObject psObject, ConfigurableObject configObject, string propertyName)
		{
			if (propertyName == "Identity")
			{
				IEnumerable<PropertyDefinition> source = from c in configObject.ObjectSchema.AllProperties
				where c.Name == propertyName
				select c;
				if (source.Count<PropertyDefinition>() == 1)
				{
					PropertyDefinition propertyDefinition = source.First<PropertyDefinition>();
					object value = psObject.Members[propertyName].Value;
					configObject.propertyBag[propertyDefinition] = MockObjectCreator.GetPropertyBasedOnDefinition(propertyDefinition, value);
					return;
				}
			}
			else
			{
				base.FillProperty(type, psObject, configObject, propertyName);
			}
		}

		private static readonly string[] mbxAcePresentationObjProperties = new string[]
		{
			"AccessRights",
			"Deny",
			"Identity",
			"InheritanceType",
			"IsInherited",
			"RealAce",
			"User"
		};
	}
}
