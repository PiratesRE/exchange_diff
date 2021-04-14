using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class UMMailboxPinCreator : MockObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"LockedOut",
				"PinExpired"
			};
		}

		protected override void FillProperties(Type type, PSObject psObject, object dummyObject, IList<string> properties)
		{
			foreach (PSMemberInfo psmemberInfo in psObject.Members)
			{
				if (properties.Contains(psmemberInfo.Name))
				{
					PropertyInfo property = dummyObject.GetType().GetProperty(psmemberInfo.Name);
					property.SetValue(dummyObject, MockObjectCreator.GetSingleProperty(psObject.Members[psmemberInfo.Name].Value, property.PropertyType), null);
				}
			}
		}
	}
}
