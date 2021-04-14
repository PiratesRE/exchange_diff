using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class RemoteOnPremiseMonadCommandExecutionContextFactory : ICommandExecutionContextFactory
	{
		public RemoteOnPremiseMonadCommandExecutionContextFactory(string serverName)
		{
			this.serverName = serverName;
		}

		public CommandExecutionContext CreateExecutionContext()
		{
			return new RemoteOnPremiseMonadCommandExecutionContext
			{
				ServerName = this.serverName
			};
		}

		private string serverName;
	}
}
