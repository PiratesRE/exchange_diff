using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Hygiene.Data.Domain;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal class DnsLocalConfiguration : DnsConfiguration
	{
		public DnsLocalConfiguration(XmlElement configNode, TracingContext traceContext) : base(configNode, traceContext, true)
		{
		}

		protected override void InitDnsServerIps(bool useSingleDnsServer)
		{
			base.DnsServerIps = this.GetLocalIpAddress();
		}

		protected override void InitSupportedZones()
		{
			DomainSession domainSession = new DomainSession();
			string internalZoneName = this.GetInternalZone();
			if (DnsConfiguration.Zones == null)
			{
				lock (this.lockObject)
				{
					if (DnsConfiguration.Zones == null)
					{
						DnsConfiguration.Zones = (from zone in domainSession.FindZoneAll()
						select zone.DomainName).ToList<string>();
					}
				}
			}
			base.AllZones = (from name in DnsConfiguration.Zones
			where !string.Equals(name, internalZoneName, StringComparison.OrdinalIgnoreCase)
			select name).ToList<string>();
			base.ZonesWithFallback = this.GetZonesWithFallback();
			base.ZonesWithoutFallback = base.AllZones.Except(base.ZonesWithFallback).ToList<string>();
		}

		protected override void InitSupportedTargetServices()
		{
			this.InitDnsSupportedTargetServicesFromRegistry();
		}

		protected override void InitMonitorDomain()
		{
			base.MonitorDomain = this.GetDnsMonitorDomain();
		}

		protected override void InitIpV6Prefix()
		{
			base.IpV6Prefix = this.GetIpV6Prefix();
		}

		private static Tuple<string, string, bool> GetTargetServiceTuple(string targetService)
		{
			string[] array = targetService.Split(new char[]
			{
				':'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 3 && (array[2] == "1" || array[2] == "0"))
			{
				bool item = array[2] == "1";
				return new Tuple<string, string, bool>(array[0], array[1], item);
			}
			throw new FormatException(string.Format("Invalid format for target service, expected='region:version:bool_SupportIpV6', actual='{0}'", targetService));
		}

		private List<string> GetZonesWithFallback()
		{
			List<string> list = new List<string>();
			XmlElement elementFromDnsConfig = this.GetElementFromDnsConfig("/configuration/customSettings/plugins/add[@name='Database Plugin']/fallbackTargetServices");
			if (elementFromDnsConfig != null)
			{
				foreach (object obj in elementFromDnsConfig.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlElement xmlElement = xmlNode as XmlElement;
					if (xmlElement != null)
					{
						list.Add(Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("zone"), "fallbackTargetServices.Zone"));
					}
				}
			}
			return list;
		}

		private string GetDnsMonitorDomain()
		{
			XmlElement elementFromDnsConfig = this.GetElementFromDnsConfig("/configuration/customSettings/plugins/add[@name='Database Plugin']/MonitorDomain");
			if (elementFromDnsConfig == null)
			{
				throw new FormatException("Monitor Domain element missing");
			}
			return Utils.CheckNullOrWhiteSpace(elementFromDnsConfig.GetAttribute("Domain"), "MonitorDomain.Domain");
		}

		private string GetIpV6Prefix()
		{
			XmlElement elementFromDnsConfig = this.GetElementFromDnsConfig("/configuration/customSettings/plugins/add[@name='Database Plugin']/settings/add[@name='IpV6Prefix']");
			if (elementFromDnsConfig == null)
			{
				throw new FormatException("IpV6Prefix element missing");
			}
			return Utils.CheckNullOrWhiteSpace(elementFromDnsConfig.GetAttribute("value"), "IpV6Prefix.value");
		}

		private XmlElement GetElementFromDnsConfig(string xPath)
		{
			string text = Path.Combine(this.GetDnsInstallPath(), "Bin\\Microsoft.Exchange.Hygiene.ServiceLocator.FfoDnsServer.exe.config");
			if (!File.Exists(text))
			{
				throw new DnsMonitorException(string.Format("DNS configuration file missing, path={0}", text), null);
			}
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.DNSTracer, base.TraceContext, "Trying to read file, file={0}, xpath={1}", text, xPath, null, "GetElementFromDnsConfig", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsLocalConfiguration.cs", 223);
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.Load(text);
			return safeXmlDocument.DocumentElement.SelectSingleNode(xPath) as XmlElement;
		}

		private string GetDnsInstallPath()
		{
			string stringValueFromRegistry = Utils.GetStringValueFromRegistry("SOFTWARE\\Microsoft\\FfoDomainNameServer\\Setup", "MsiInstallPath");
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DNSTracer, base.TraceContext, "DnsLocalConfiguration: Found install path={0}", stringValueFromRegistry, null, "GetDnsInstallPath", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsLocalConfiguration.cs", 238);
			return stringValueFromRegistry;
		}

		private void InitDnsSupportedTargetServicesFromRegistry()
		{
			string stringValueFromRegistry = Utils.GetStringValueFromRegistry("SOFTWARE\\Microsoft\\FfoDomainNameServer\\Config", "SupportedTargetServices");
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DNSTracer, base.TraceContext, "DnsLocalConfiguration: Found TargetServices={0}", stringValueFromRegistry, null, "InitDnsSupportedTargetServicesFromRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsLocalConfiguration.cs", 250);
			base.IpV4TargetServices = new List<Tuple<string, string>>();
			base.IpV6TargetServices = new List<Tuple<string, string>>();
			foreach (string targetService in stringValueFromRegistry.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries))
			{
				Tuple<string, string, bool> targetServiceTuple = DnsLocalConfiguration.GetTargetServiceTuple(targetService);
				Tuple<string, string> item = new Tuple<string, string>(targetServiceTuple.Item1, targetServiceTuple.Item2);
				base.IpV4TargetServices.Add(item);
				if (targetServiceTuple.Item3)
				{
					base.IpV6TargetServices.Add(item);
				}
			}
		}

		private string GetInternalZone()
		{
			string stringValueFromRegistry = Utils.GetStringValueFromRegistry("SOFTWARE\\Microsoft\\FfoDomainNameServer\\Config", "InternalZoneName");
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DNSTracer, base.TraceContext, "DnsLocalConfiguration: Found internal zone={0}", stringValueFromRegistry, null, "GetInternalZone", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsLocalConfiguration.cs", 277);
			return stringValueFromRegistry;
		}

		private List<IPAddress> GetLocalIpAddress()
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, base.TraceContext, "DnsLocalConfiguration: Trying to get local Ip Address", null, "GetLocalIpAddress", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsLocalConfiguration.cs", 288);
			List<IPAddress> list = new List<IPAddress>();
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				if (networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
				{
					IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
					foreach (UnicastIPAddressInformation unicastIPAddressInformation in ipproperties.UnicastAddresses)
					{
						if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							list.Add(unicastIPAddressInformation.Address);
							break;
						}
					}
				}
			}
			if (list.Count == 0)
			{
				throw new DnsMonitorException("DnsLocalConfiguration: Could not retrive the local IpAddress for DNS", null);
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DNSTracer, base.TraceContext, "DnsLocalConfiguration: LocalIpAddress={0}", string.Join<IPAddress>(",", list), null, "GetLocalIpAddress", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsLocalConfiguration.cs", 313);
			return list;
		}

		private const string DnsConfigRegistryKeyPath = "SOFTWARE\\Microsoft\\FfoDomainNameServer\\Config";

		private const string DnsSetupRegistryKeyPath = "SOFTWARE\\Microsoft\\FfoDomainNameServer\\Setup";

		private const string DnsConfigRelativePath = "Bin\\Microsoft.Exchange.Hygiene.ServiceLocator.FfoDnsServer.exe.config";

		private const string DnsInstallPathNameKey = "MsiInstallPath";

		private const string DnsInternalZoneNameKey = "InternalZoneName";

		private const string DnsSupportedTargetServices = "SupportedTargetServices";

		private object lockObject = new object();
	}
}
