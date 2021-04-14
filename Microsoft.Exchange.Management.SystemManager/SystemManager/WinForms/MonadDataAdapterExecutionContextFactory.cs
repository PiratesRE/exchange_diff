using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class MonadDataAdapterExecutionContextFactory : IDataAdapterExecutionContextFactory
	{
		public DataAdapterExecutionContext CreateExecutionContext()
		{
			return new MonadDataAdapterExecutionContext();
		}
	}
}
