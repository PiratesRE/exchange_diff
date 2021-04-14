using System;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.PowerShell.RbacHostingTools.Asp.Net
{
	public class RbacRunspaceFactory : RunspaceFactory
	{
		public RbacRunspaceFactory(InitialSessionStateSectionFactory issFactory) : base(issFactory, null, true)
		{
		}

		public RbacRunspaceFactory(InitialSessionStateSectionFactory issFactory, PSHostFactory hostFactory) : base(issFactory, hostFactory, true)
		{
		}

		protected override void InitializeRunspace(Runspace runspace)
		{
			base.InitializeRunspace(runspace);
			try
			{
				runspace.SessionStateProxy.SetVariable(ExchangePropertyContainer.ADServerSettingsVarName, this.CreateRunspaceServerSettings());
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

		internal virtual RunspaceServerSettings CreateRunspaceServerSettings()
		{
			string runspaceServerSettingsToken = this.GetRunspaceServerSettingsToken();
			if (runspaceServerSettingsToken == null)
			{
				return RunspaceServerSettings.CreateRunspaceServerSettings(false);
			}
			return RunspaceServerSettings.CreateGcOnlyRunspaceServerSettings(runspaceServerSettingsToken.ToLowerInvariant(), false);
		}

		protected virtual string GetRunspaceServerSettingsToken()
		{
			if (RbacPrincipal.Current.ExecutingUserId == null)
			{
				return RbacPrincipal.Current.CacheKeys[0];
			}
			return RbacPrincipal.Current.ExecutingUserId.ToString();
		}
	}
}
