using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DummyRpcTask : DummyTask
	{
		public DummyRpcTask(IContext context) : base(context, RpcHelper.DependenciesOfBuildCompleteBindingInfo)
		{
		}

		protected override IEmsmdbClient CreateEmsmdbClient()
		{
			return base.Environment.CreateEmsmdbClient(RpcHelper.BuildCompleteBindingInfo(base.Properties, 6001));
		}
	}
}
