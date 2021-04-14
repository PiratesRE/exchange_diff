using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetFolderMruConfiguration : ServiceCommand<TargetFolderMruConfiguration>
	{
		public GetFolderMruConfiguration(CallContext callContext) : base(callContext)
		{
		}

		protected override TargetFolderMruConfiguration InternalExecute()
		{
			TargetFolderMruConfiguration targetFolderMruConfiguration = new TargetFolderMruConfiguration();
			targetFolderMruConfiguration.LoadAll(base.CallContext);
			return targetFolderMruConfiguration;
		}
	}
}
