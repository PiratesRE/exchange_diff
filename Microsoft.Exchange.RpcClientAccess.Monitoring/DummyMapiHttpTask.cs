using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DummyMapiHttpTask : DummyTask
	{
		public DummyMapiHttpTask(IContext context) : base(context, RpcHelper.DependenciesOfBuildMapiHttpBindingInfo)
		{
		}

		protected override IEmsmdbClient CreateEmsmdbClient()
		{
			return base.Environment.CreateEmsmdbClient(RpcHelper.BuildCompleteMapiHttpBindingInfo(base.Properties));
		}
	}
}
