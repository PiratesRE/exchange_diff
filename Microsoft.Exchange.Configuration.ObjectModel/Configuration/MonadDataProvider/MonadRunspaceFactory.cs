using System;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MonadRunspaceFactory : RunspaceFactory
	{
		public static MonadRunspaceFactory GetInstance()
		{
			if (MonadRunspaceFactory.instance == null)
			{
				MonadRunspaceFactory.instance = new MonadRunspaceFactory();
			}
			return MonadRunspaceFactory.instance;
		}

		internal MonadRunspaceFactory() : base(MonadRunspaceConfigurationFactory.GetInstance(), MonadHostFactory.GetInstance())
		{
		}

		private static MonadRunspaceFactory instance;
	}
}
