using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UpgradePrecheckDataHandler : PreCheckDataHandler
	{
		public UpgradePrecheckDataHandler(ISetupContext setupContext, DataHandler topLevelHandler, MonadConnection connection) : base(setupContext, topLevelHandler, connection)
		{
		}

		public override string ShortDescription
		{
			get
			{
				return Strings.UpgradePreCheckText;
			}
		}

		public override string Title
		{
			get
			{
				return Strings.UpgradePrereq;
			}
		}
	}
}
