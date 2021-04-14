using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class MailFlowPartnerIdParameter : ADIdParameter
	{
		public MailFlowPartnerIdParameter()
		{
		}

		public MailFlowPartnerIdParameter(string identity) : base(identity)
		{
		}

		public MailFlowPartnerIdParameter(ADObjectId objectId) : base(objectId)
		{
		}

		public MailFlowPartnerIdParameter(MailFlowPartner connector) : base(connector.Id)
		{
		}

		public MailFlowPartnerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static MailFlowPartnerIdParameter Parse(string identity)
		{
			return new MailFlowPartnerIdParameter(identity);
		}
	}
}
