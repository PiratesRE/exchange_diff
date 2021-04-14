using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal interface ITransportSettingsFacade
	{
		bool ClearCategories { get; }

		bool Rfc2231EncodingEnabled { get; }

		bool OpenDomainRoutingEnabled { get; }

		bool AddressBookPolicyRoutingEnabled { get; }

		IList<string> SupervisionTags { get; }

		string OrganizationFederatedMailbox { get; }
	}
}
