using System;
using System.Management.Automation.Runspaces;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MonadRunspaceConfigurationFactory : RunspaceConfigurationFactory
	{
		public static MonadRunspaceConfigurationFactory GetInstance()
		{
			if (MonadRunspaceConfigurationFactory.instance == null)
			{
				MonadRunspaceConfigurationFactory.instance = new MonadRunspaceConfigurationFactory();
			}
			return MonadRunspaceConfigurationFactory.instance;
		}

		public override RunspaceConfiguration GetRunspaceConfiguration()
		{
			if (this.runspaceConfiguration == null)
			{
				lock (this.syncInstance)
				{
					if (this.runspaceConfiguration == null)
					{
						this.runspaceConfiguration = this.CreateRunspaceConfiguration();
					}
				}
			}
			return this.runspaceConfiguration;
		}

		public override RunspaceConfiguration CreateRunspaceConfiguration()
		{
			return MonadRunspaceConfiguration.Create();
		}

		private static MonadRunspaceConfigurationFactory instance;

		private object syncInstance = new object();

		private RunspaceConfiguration runspaceConfiguration;
	}
}
