using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class MonadCommandExecutionContextFactory : ICommandExecutionContextFactory
	{
		public CommandExecutionContext CreateExecutionContext()
		{
			return new MonadCommandExecutionContext();
		}
	}
}
