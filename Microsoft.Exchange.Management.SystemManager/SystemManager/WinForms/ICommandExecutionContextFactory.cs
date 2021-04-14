using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal interface ICommandExecutionContextFactory
	{
		CommandExecutionContext CreateExecutionContext();
	}
}
