using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Win32;
using Microsoft.Win32;

namespace Microsoft.Exchange.Net
{
	internal static class ComputerInformation
	{
		public static string NetbiosName
		{
			get
			{
				return ComputerInformation.GetComputerName(NativeMethods.ComputerNameFormat.NetBios);
			}
		}

		public static string DnsDomainName
		{
			get
			{
				return ComputerInformation.GetComputerName(NativeMethods.ComputerNameFormat.DnsDomain);
			}
		}

		public static string DnsFullyQualifiedDomainName
		{
			get
			{
				string result;
				if (ComputerInformation.runningOnNonCluster)
				{
					result = ComputerInformation.GetComputerName(NativeMethods.ComputerNameFormat.DnsFullyQualified);
				}
				else
				{
					result = (ComputerInformation.GetClusterDnsFullyQualifiedDomainName() ?? ComputerInformation.GetComputerName(NativeMethods.ComputerNameFormat.DnsFullyQualified));
				}
				return result;
			}
		}

		public static string DnsHostName
		{
			get
			{
				return ComputerInformation.GetComputerName(NativeMethods.ComputerNameFormat.DnsHostname);
			}
		}

		public static string NetbiosPhysicalName
		{
			get
			{
				return ComputerInformation.GetComputerName(NativeMethods.ComputerNameFormat.PhysicalNetBios);
			}
		}

		public static string DnsPhysicalDomainName
		{
			get
			{
				return ComputerInformation.GetComputerName(NativeMethods.ComputerNameFormat.PhysicalDnsDomain);
			}
		}

		public static string DnsPhysicalFullyQualifiedDomainName
		{
			get
			{
				return ComputerInformation.GetComputerName(NativeMethods.ComputerNameFormat.PhysicalDnsFullyQualified);
			}
		}

		public static string DnsPhysicalHostName
		{
			get
			{
				return ComputerInformation.GetComputerName(NativeMethods.ComputerNameFormat.PhysicalDnsHostname);
			}
		}

		public static List<IPAddress> GetLocalIPAddresses()
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			List<IPAddress> list = new List<IPAddress>(allNetworkInterfaces.Length);
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
				UnicastIPAddressInformationCollection unicastAddresses = ipproperties.UnicastAddresses;
				foreach (IPAddressInformation ipaddressInformation in unicastAddresses)
				{
					if (!IPAddress.IsLoopback(ipaddressInformation.Address))
					{
						list.Add(ipaddressInformation.Address);
					}
				}
			}
			return list;
		}

		private static string GetComputerName(NativeMethods.ComputerNameFormat type)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			uint capacity = (uint)stringBuilder.Capacity;
			if (!NativeMethods.GetComputerNameEx(type, stringBuilder, ref capacity))
			{
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
			return stringBuilder.ToString().Trim();
		}

		private static string GetClusterDnsFullyQualifiedDomainName()
		{
			string result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\WLBS\\Enum"))
			{
				if (registryKey == null)
				{
					ComputerInformation.runningOnNonCluster = true;
					result = null;
				}
				else
				{
					object value = registryKey.GetValue("Count");
					if (value == null || (int)value != 1)
					{
						ComputerInformation.runningOnNonCluster = true;
						result = null;
					}
					else
					{
						using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\WLBS\\Parameters\\Interface"))
						{
							if (registryKey2 == null || registryKey2.SubKeyCount == 0)
							{
								ComputerInformation.runningOnNonCluster = true;
								result = null;
							}
							else
							{
								using (RegistryKey registryKey3 = registryKey2.OpenSubKey(registryKey2.GetSubKeyNames()[0]))
								{
									if (registryKey3 == null)
									{
										ComputerInformation.runningOnNonCluster = true;
										result = null;
									}
									else
									{
										string text = (string)registryKey3.GetValue("ClusterName");
										if (!string.IsNullOrEmpty(text))
										{
											text = text.Trim();
											if (!string.IsNullOrEmpty(text))
											{
												result = text;
											}
											else
											{
												result = null;
											}
										}
										else
										{
											result = null;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private const string DefaultNLBParentKeyName = "System\\CurrentControlSet\\Services\\WLBS\\Parameters\\Interface";

		private const string DefaultNLBAdaptorKeyName = "System\\CurrentControlSet\\Services\\WLBS\\Enum";

		private const string ClusterName = "ClusterName";

		private static bool runningOnNonCluster;
	}
}
