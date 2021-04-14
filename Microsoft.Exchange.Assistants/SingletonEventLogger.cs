using System;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class SingletonEventLogger
	{
		public static EventLogger Logger
		{
			get
			{
				return SingletonEventLogger.logger;
			}
		}

		public static EventLogger GetSingleton(string serviceName)
		{
			if (SingletonEventLogger.logger == null)
			{
				SingletonEventLogger.logger = new EventLogger(serviceName);
			}
			return SingletonEventLogger.logger;
		}

		private static EventLogger logger;
	}
}
