using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal interface IDataAdapterExecutionContextFactory
	{
		DataAdapterExecutionContext CreateExecutionContext();
	}
}
