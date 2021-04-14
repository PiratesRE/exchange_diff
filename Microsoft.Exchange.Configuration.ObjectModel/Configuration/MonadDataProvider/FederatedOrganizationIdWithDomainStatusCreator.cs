using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class FederatedOrganizationIdWithDomainStatusCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"Name",
				"AccountNamespace",
				"Domains",
				"OrganizationContact",
				"Enabled",
				"DelegationTrustLink"
			};
		}
	}
}
