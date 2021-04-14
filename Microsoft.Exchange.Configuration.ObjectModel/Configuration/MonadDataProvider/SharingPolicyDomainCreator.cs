using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class SharingPolicyDomainCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"Name",
				"WhenChanged",
				"Enabled",
				"Domains",
				"ObjectId"
			};
		}
	}
}
