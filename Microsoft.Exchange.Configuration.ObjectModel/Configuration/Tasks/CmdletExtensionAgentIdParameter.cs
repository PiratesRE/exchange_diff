using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class CmdletExtensionAgentIdParameter : ADIdParameter
	{
		public CmdletExtensionAgentIdParameter()
		{
		}

		public CmdletExtensionAgentIdParameter(string identity) : base(identity)
		{
		}

		public CmdletExtensionAgentIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public CmdletExtensionAgentIdParameter(CmdletExtensionAgent agent) : base(agent.Id)
		{
		}

		public CmdletExtensionAgentIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static CmdletExtensionAgentIdParameter Parse(string identity)
		{
			return new CmdletExtensionAgentIdParameter(identity);
		}
	}
}
