using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Management.Hybrid;

namespace Microsoft.Exchange.Setup.Common
{
	public class BridgeLogger : ILogger, IDisposable
	{
		public BridgeLogger(ISetupLogger setupLogger)
		{
			if (setupLogger == null)
			{
				throw new ArgumentNullException();
			}
			this.logger = setupLogger;
		}

		public void Log(LocalizedString text)
		{
			this.logger.Log(text);
		}

		public void Log(Exception e)
		{
			this.logger.LogError(e);
		}

		public void LogError(string text)
		{
			this.logger.Log(new LocalizedString(text));
		}

		public void LogWarning(string text)
		{
			this.logger.Log(new LocalizedString(text));
		}

		public void LogInformation(string text)
		{
			this.logger.Log(new LocalizedString(text));
		}

		public void Dispose()
		{
		}

		private ISetupLogger logger;
	}
}
