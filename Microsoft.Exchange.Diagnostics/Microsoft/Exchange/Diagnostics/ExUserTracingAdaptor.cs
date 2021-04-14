using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class ExUserTracingAdaptor : ExCustomTracingAdaptor<string>
	{
		private ExUserTracingAdaptor()
		{
		}

		public static ExUserTracingAdaptor Instance
		{
			get
			{
				if (ExUserTracingAdaptor.instance == null)
				{
					lock (ExUserTracingAdaptor.instanceLock)
					{
						if (ExUserTracingAdaptor.instance == null)
						{
							ExUserTracingAdaptor.instance = new ExUserTracingAdaptor();
						}
					}
				}
				return ExUserTracingAdaptor.instance;
			}
		}

		public bool IsTracingEnabledUser(string userName)
		{
			return base.IsTracingEnabled(userName);
		}

		protected override HashSet<string> LoadEnabledIdentities(ExTraceConfiguration currentInstance)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			List<string> list;
			if (currentInstance.CustomParameters.TryGetValue("UserDN", out list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					hashSet.Add(list[i]);
				}
			}
			if (currentInstance.CustomParameters.TryGetValue("WindowsIdentity", out list))
			{
				for (int j = 0; j < list.Count; j++)
				{
					hashSet.Add(list[j]);
				}
			}
			return hashSet;
		}

		private static ExUserTracingAdaptor instance;

		private static object instanceLock = new object();
	}
}
