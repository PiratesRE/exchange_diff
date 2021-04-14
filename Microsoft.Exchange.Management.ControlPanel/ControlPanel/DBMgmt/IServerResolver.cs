using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel.DBMgmt
{
	public interface IServerResolver
	{
		IEnumerable<ServerResolverRow> ResolveObjects(IEnumerable<ADObjectId> identities);
	}
}
