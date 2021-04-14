using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RemoveProvisionedServerDataHandler : ConfigurationDataHandler
	{
		public RemoveProvisionedServerDataHandler(ISetupContext context, MonadConnection connection) : base(context, "", "Remove-ProvisionedServer", connection)
		{
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			string removeProvisionedServerName = base.SetupContext.RemoveProvisionedServerName;
			if (!string.IsNullOrEmpty(removeProvisionedServerName))
			{
				base.Parameters.AddWithValue("ServerName", removeProvisionedServerName);
			}
			SetupLogger.TraceExit();
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
			SetupLogger.TraceEnter(new object[0]);
			PrerequisiteAnalysisTaskDataHandler instance = PrerequisiteAnalysisTaskDataHandler.GetInstance(base.SetupContext, base.Connection);
			instance.AddRole("Global");
			instance.RemoveProvisionedServerName = base.SetupContext.RemoveProvisionedServerName;
			SetupLogger.TraceExit();
		}
	}
}
