using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mapi;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MailboxStatisticsCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"LastLoggedOnUserAccount",
				"ItemCount",
				"TotalItemSize"
			};
		}

		protected override void FillProperty(Type type, PSObject psObject, ConfigurableObject configObject, string propertyName)
		{
			if (propertyName == "ItemCount")
			{
				configObject.propertyBag.DangerousSetValue(MapiPropertyDefinitions.ItemCount, psObject.Members[propertyName].Value);
				return;
			}
			if (propertyName == "TotalItemSize")
			{
				configObject.propertyBag.DangerousSetValue(MapiPropertyDefinitions.TotalItemSize, MockObjectCreator.GetSingleProperty(psObject.Members[propertyName].Value, MapiPropertyDefinitions.TotalItemSize.Type));
				return;
			}
			if (propertyName == "LastLoggedOnUserAccount")
			{
				configObject.propertyBag.DangerousSetValue(MapiPropertyDefinitions.LastLoggedOnUserAccount, psObject.Members[propertyName].Value);
				return;
			}
			base.FillProperty(type, psObject, configObject, propertyName);
		}
	}
}
