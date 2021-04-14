using System;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Approval.Applications
{
	internal class ForestScopeRunspaceFactory : RunspaceFactory
	{
		public ForestScopeRunspaceFactory(InitialSessionStateFactory issFactory, PSHostFactory hostFactory) : base(issFactory, hostFactory, true)
		{
		}

		protected override void InitializeRunspace(Runspace runspace)
		{
			base.InitializeRunspace(runspace);
			try
			{
				RunspaceServerSettings runspaceServerSettings = RunspaceServerSettings.CreateRunspaceServerSettings(false);
				runspaceServerSettings.ViewEntireForest = true;
				runspace.SessionStateProxy.SetVariable(ExchangePropertyContainer.ADServerSettingsVarName, runspaceServerSettings);
			}
			catch (ADTransientException)
			{
				throw;
			}
			catch (ADExternalException)
			{
				throw;
			}
		}
	}
}
