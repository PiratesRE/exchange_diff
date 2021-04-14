using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Rpc.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	internal class UMPromptPreviewRpcTarget : UMVersionedRpcTargetBase
	{
		internal UMPromptPreviewRpcTarget(Server server) : base(server)
		{
		}

		protected override UMVersionedRpcClientBase CreateRpcClient()
		{
			return new UMPromptPreviewRpcClient(base.Server.Fqdn);
		}
	}
}
