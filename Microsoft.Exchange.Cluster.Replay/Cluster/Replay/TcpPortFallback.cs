using System;
using System.IO;
using System.Security;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class TcpPortFallback
	{
		internal static bool LoadPortNumber(out ushort portNumber)
		{
			portNumber = 64327;
			Exception ex = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters"))
				{
					RegistryValueKind valueKind = registryKey.GetValueKind("ReplicationPort");
					if (valueKind == RegistryValueKind.DWord)
					{
						int num = (int)registryKey.GetValue("ReplicationPort");
						if (num > 0 && num < 65536)
						{
							portNumber = (ushort)num;
							return true;
						}
						NetworkManager.TraceError("{0}\\{1} = {2} which is an invalid port number", new object[]
						{
							"SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters",
							"ReplicationPort",
							num
						});
					}
				}
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (SecurityException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				NetworkManager.TraceError("TcpPortFallback.LoadPortNumber fails: {0}", new object[]
				{
					ex
				});
			}
			return false;
		}

		internal static bool StorePortNumber(ushort portNumber)
		{
			Exception ex = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters"))
				{
					registryKey.SetValue("ReplicationPort", portNumber, RegistryValueKind.DWord);
					return true;
				}
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (SecurityException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				NetworkManager.TraceError("TcpPortFallback.StorePortNumber fails: {0}", new object[]
				{
					ex
				});
			}
			return false;
		}

		private const string RegistryKeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters";

		private const string TcpPortValueName = "ReplicationPort";
	}
}
