using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class MonadCommandExecutionContextForPropertyPageFactory : ICommandExecutionContextFactory
	{
		public CommandExecutionContext CreateExecutionContext()
		{
			return new MonadCommandExecutionContext
			{
				IsPropertyPage = true
			};
		}
	}
}
