using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class ServerResolverExtensions
	{
		public static ServerResolverRow ResolveServer(ADObjectId server)
		{
			List<ADObjectId> list = new List<ADObjectId>();
			list.Add(server);
			return ServerResolver.Instance.ResolveObjects(list).FirstOrDefault<ServerResolverRow>();
		}
	}
}
