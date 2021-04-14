using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	internal class PSRemoteServerSchema : ObjectSchema
	{
		public static readonly AdminPropertyDefinition RemotePSServer = new AdminPropertyDefinition("RemotePSServer", ExchangeObjectVersion.Exchange2003, typeof(Fqdn), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
