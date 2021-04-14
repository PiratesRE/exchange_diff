using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network.Probes
{
	internal class NetworkAdapterProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			NetworkAdapterProbe.GetNetworkInterfaceSetting(base.TraceContext, base.Result, cancellationToken);
		}

		internal static bool GetNetworkInterfaceSetting(TracingContext traceContext, ProbeResult result, CancellationToken localCancellationToken)
		{
			StringBuilder stringBuilder = new StringBuilder();
			NetworkAdapterProbe.MissingEntriesInNetworkAdapter missingEntriesInNetworkAdapter = (NetworkAdapterProbe.MissingEntriesInNetworkAdapter)0;
			IPGlobalProperties ipglobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			NetworkUtils.LogWorkItemMessage(traceContext, result, string.Format("Interface information for {0}.{1}:\n", ipglobalProperties.HostName, ipglobalProperties.DomainName), new object[0]);
			if (allNetworkInterfaces == null || allNetworkInterfaces.Length < 1)
			{
				NetworkUtils.LogWorkItemMessage(traceContext, result, "No network interfaces found. Returning...", new object[0]);
				return false;
			}
			bool flag = Environment.MachineName.ToUpperInvariant().Contains("NT");
			NetworkInterface[] array = (from nic in allNetworkInterfaces
			where nic.Name.ToUpperInvariant().Equals("MAPI")
			select nic).ToArray<NetworkInterface>();
			if (array.Length == 0)
			{
				NetworkUtils.LogWorkItemMessage(traceContext, result, "No MAPI network interface is found. Returning...", new object[0]);
				return false;
			}
			if (array.Length > 1)
			{
				NetworkUtils.LogWorkItemMessage(traceContext, result, "More than one MAPI network interfaces are found. Returning...", new object[0]);
				return false;
			}
			if (localCancellationToken.IsCancellationRequested)
			{
				NetworkUtils.LogWorkItemMessage(traceContext, result, "GetNetworkInterfaceSetting: Operation is cancelled", new object[0]);
				throw new OperationCanceledException(localCancellationToken);
			}
			NetworkInterface networkInterface = array[0];
			IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
			UnicastIPAddressInformationCollection unicastAddresses = ipproperties.UnicastAddresses;
			string text = null;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			if (unicastAddresses != null)
			{
				text = string.Join<IPAddress>(" ", from information in unicastAddresses
				select information.Address into address
				where address.AddressFamily == AddressFamily.InterNetwork
				select address);
				text2 = string.Join<IPAddress>(" ", from information in unicastAddresses
				select information.IPv4Mask into mask
				where mask.AddressFamily == AddressFamily.InterNetwork && !mask.Equals(IPAddress.Any)
				select mask);
				stringBuilder.AppendLine(string.Format("IpAddress:{0}", text));
				stringBuilder.AppendLine(string.Format("SubnetMask:{0}", text2));
				NetworkUtils.LogWorkItemMessage(traceContext, result, string.Format("Ip Address ............................ : {0}\n", text), new object[0]);
				NetworkUtils.LogWorkItemMessage(traceContext, result, string.Format("Subnet Mask ............................ : {0}\n", text2), new object[0]);
				text3 = string.Join(" ", from ip in ipproperties.GatewayAddresses
				select ip.Address.ToString());
				text4 = string.Join<IPAddress>(" ", ipproperties.DnsAddresses);
				stringBuilder.AppendLine(string.Format("DefaultGateway:{0}", text3));
				stringBuilder.AppendLine(string.Format("DnsAddresses:{0}", text4));
				NetworkUtils.LogWorkItemMessage(traceContext, result, string.Format("Gateway ............................ : {0}\n", text3), new object[0]);
				NetworkUtils.LogWorkItemMessage(traceContext, result, string.Format("DNSAddresses ............................... : {0}\n", text4), new object[0]);
			}
			bool flag2 = false;
			if (string.IsNullOrEmpty(text))
			{
				missingEntriesInNetworkAdapter = NetworkAdapterProbe.MissingEntriesInNetworkAdapter.IpAddress;
				flag2 = true;
			}
			if (string.IsNullOrEmpty(text2))
			{
				missingEntriesInNetworkAdapter |= NetworkAdapterProbe.MissingEntriesInNetworkAdapter.SubnetMask;
				flag2 = true;
			}
			if (!flag && string.IsNullOrEmpty(text3))
			{
				missingEntriesInNetworkAdapter |= NetworkAdapterProbe.MissingEntriesInNetworkAdapter.DefaultGateway;
				flag2 = true;
			}
			if (string.IsNullOrEmpty(text4))
			{
				missingEntriesInNetworkAdapter |= NetworkAdapterProbe.MissingEntriesInNetworkAdapter.DnsAddresses;
				flag2 = true;
			}
			if (flag2)
			{
				if (result != null)
				{
					result.StateAttribute6 = (double)missingEntriesInNetworkAdapter;
				}
				NetworkUtils.LogWorkItemMessage(traceContext, result, "Missing configuration found, will throw.", new object[0]);
				throw new Exception(string.Format("Network Adapter setting found to be incomplete in: {0}. Missing entries are: {1}.", ipglobalProperties.HostName, missingEntriesInNetworkAdapter.ToString()));
			}
			if (localCancellationToken.IsCancellationRequested)
			{
				NetworkUtils.LogWorkItemMessage(traceContext, result, "GetNetworkInterfaceSetting: Operation is cancelled", new object[0]);
				throw new OperationCanceledException(localCancellationToken);
			}
			FileInfo fileInfo = new FileInfo("D:\\NetworkMonitoring\\ServerNetworkAdapterConfiguration.txt");
			bool result2;
			try
			{
				if (!fileInfo.Directory.Exists)
				{
					fileInfo.Directory.Create();
				}
				NetworkUtils.LogWorkItemMessage(traceContext, result, "Overwriting the file with Network Adapter Entries...", new object[0]);
				using (StreamWriter streamWriter = new StreamWriter("D:\\NetworkMonitoring\\ServerNetworkAdapterConfiguration.txt", false))
				{
					streamWriter.Write(stringBuilder.ToString());
					streamWriter.Close();
				}
				NetworkUtils.LogWorkItemMessage(traceContext, result, "Network Adapter Entries have been written to the file successfully", new object[0]);
				result2 = true;
			}
			catch (SystemException ex)
			{
				NetworkUtils.LogWorkItemMessage(traceContext, result, "Error while writing to the file. Exception: {0}", new object[]
				{
					ex.Message
				});
				result2 = false;
			}
			return result2;
		}

		internal const string NetworkAdapterConfigurationFileName = "D:\\NetworkMonitoring\\ServerNetworkAdapterConfiguration.txt";

		[Flags]
		internal enum MissingEntriesInNetworkAdapter
		{
			IpAddress = 1,
			SubnetMask = 2,
			DefaultGateway = 4,
			DnsAddresses = 8
		}
	}
}
