using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class FederationTrustCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"Name",
				"OrgCertificate",
				"OrgNextCertificate",
				"OrgPrevCertificate",
				"NamespaceProvisioner",
				"ApplicationIdentifier"
			};
		}
	}
}
