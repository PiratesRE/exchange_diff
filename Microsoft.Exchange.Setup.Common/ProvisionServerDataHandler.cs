using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ProvisionServerDataHandler : ConfigurationDataHandler
	{
		public ProvisionServerDataHandler(ISetupContext context, MonadConnection connection) : base(context, "", "New-ProvisionedServer", connection)
		{
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			string newProvisionedServerName = base.SetupContext.NewProvisionedServerName;
			if (!string.IsNullOrEmpty(newProvisionedServerName))
			{
				base.Parameters.AddWithValue("ServerName", newProvisionedServerName);
			}
			SetupLogger.TraceExit();
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
			SetupLogger.TraceEnter(new object[0]);
			PrerequisiteAnalysisTaskDataHandler instance = PrerequisiteAnalysisTaskDataHandler.GetInstance(base.SetupContext, base.Connection);
			instance.AddRole("Global");
			instance.NewProvisionedServerName = base.SetupContext.NewProvisionedServerName;
			SetupLogger.TraceExit();
		}
	}
}
