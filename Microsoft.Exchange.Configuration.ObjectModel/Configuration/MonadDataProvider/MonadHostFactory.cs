using System;
using System.Management.Automation.Host;
using System.Threading;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MonadHostFactory : PSHostFactory
	{
		public static MonadHostFactory GetInstance()
		{
			if (MonadHostFactory.instance == null)
			{
				MonadHostFactory.instance = new MonadHostFactory();
			}
			return MonadHostFactory.instance;
		}

		public override PSHost CreatePSHost()
		{
			return new MonadHost(Thread.CurrentThread.CurrentCulture, Thread.CurrentThread.CurrentUICulture);
		}

		private static MonadHostFactory instance;
	}
}
