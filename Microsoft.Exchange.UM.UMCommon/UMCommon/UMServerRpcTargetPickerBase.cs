using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class UMServerRpcTargetPickerBase<RpcTargetType> : ServerPickerBase<RpcTargetType, Guid> where RpcTargetType : class, IRpcTarget
	{
		protected abstract RpcTargetType CreateTarget(Server server);

		protected override List<RpcTargetType> LoadConfiguration()
		{
			string name = base.GetType().Name;
			CallIdTracer.TraceDebug(base.Tracer, this.GetHashCode(), "{0}.LoadConfiguration()", new object[]
			{
				name
			});
			List<RpcTargetType> list = new List<RpcTargetType>();
			ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
			Server localServer = adtopologyLookup.GetLocalServer();
			if (localServer != null && localServer.IsUnifiedMessagingServer)
			{
				RpcTargetType rpcTargetType = this.CreateTarget(localServer);
				CallIdTracer.TraceDebug(base.Tracer, this.GetHashCode(), "{0}.LoadConfiguration() - Adding {1} to the target list", new object[]
				{
					name,
					rpcTargetType
				});
				list.Add(rpcTargetType);
			}
			return list;
		}
	}
}
