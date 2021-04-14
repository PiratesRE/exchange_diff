using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class RemoteMonadCommandExecutionContextFactory : ICommandExecutionContextFactory
	{
		public CommandExecutionContext CreateExecutionContext()
		{
			return new RemoteMonadCommandExecutionContext();
		}
	}
}
