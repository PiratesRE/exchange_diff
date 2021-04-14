using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class TraceConfigurationSingleton<T> where T : TraceConfigurationBase, new()
	{
		public static T Instance
		{
			get
			{
				if (TraceConfigurationSingleton<T>.NeedInstance(TraceConfigurationSingleton<T>.instance))
				{
					lock (TraceConfigurationSingleton<T>.staticSyncObject)
					{
						if (TraceConfigurationSingleton<T>.NeedInstance(TraceConfigurationSingleton<T>.instance))
						{
							T t = Activator.CreateInstance<T>();
							t.Load(ExTraceConfiguration.Instance);
							TraceConfigurationSingleton<T>.instance = t;
						}
					}
				}
				return TraceConfigurationSingleton<T>.instance;
			}
		}

		private static bool NeedInstance(T instance)
		{
			return instance == null || instance.IsUpdateNeeded;
		}

		private static object staticSyncObject = new object();

		private static T instance = default(T);
	}
}
