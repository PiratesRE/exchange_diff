using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal abstract class DnsConfiguration
	{
		protected DnsConfiguration(XmlElement configNode, TracingContext traceContext, bool isLocal)
		{
			this.IsLocal = isLocal;
			this.TraceContext = traceContext;
			this.LoadConfig(configNode);
			this.Validate();
		}

		public List<IPAddress> DnsServerIps { get; protected set; }

		public string IpV6Prefix { get; set; }

		protected static List<string> Zones { get; set; }

		protected TracingContext TraceContext { get; set; }

		protected bool IsLocal { get; set; }

		protected List<string> AllZones { get; set; }

		protected List<string> ZonesWithFallback { get; set; }

		protected List<string> ZonesWithoutFallback { get; set; }

		protected List<Tuple<string, string>> IpV6TargetServices { get; set; }

		protected List<Tuple<string, string>> IpV4TargetServices { get; set; }

		protected string MonitorDomain { get; set; }

		public static DnsConfiguration CreateInstance(XmlElement configNode, TracingContext traceContext, bool isLocal)
		{
			if (isLocal)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, traceContext, "DnsConfiguration: Found IsLocal element creating DnsLocalConfiguration", null, "CreateInstance", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsConfiguration.cs", 113);
				Type type = Assembly.GetExecutingAssembly().GetType("Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus.DnsLocalConfiguration");
				if (type == null)
				{
					throw new InvalidOperationException("Could not find Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus.DnsLocalConfiguration");
				}
				return Activator.CreateInstance(type, new object[]
				{
					configNode,
					traceContext
				}) as DnsConfiguration;
			}
			else
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, traceContext, "DnsConfiguration: Did not find IsLocal element creating DnsRemoteConfiguration", null, "CreateInstance", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsConfiguration.cs", 124);
				Type type2 = Assembly.GetExecutingAssembly().GetType("Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus.DnsRemoteConfiguration");
				if (type2 == null)
				{
					throw new InvalidOperationException("Could not find Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus.DnsRemoteConfiguration");
				}
				return Activator.CreateInstance(type2, new object[]
				{
					configNode,
					traceContext
				}) as DnsConfiguration;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("ZonesWithFallback:{0} ", string.Join(",", this.ZonesWithFallback));
			stringBuilder.AppendFormat("ZonesWithoutFallback:{0} ", string.Join(",", this.ZonesWithoutFallback));
			stringBuilder.AppendFormat("IpV4TargetServices:{0} ", string.Join(",", from t in this.IpV4TargetServices
			select t.Item1 + "-" + t.Item2));
			stringBuilder.AppendFormat("IpV6TargetServices:{0} ", string.Join(",", from t in this.IpV6TargetServices
			select t.Item1 + "-" + t.Item2));
			stringBuilder.AppendFormat("DnsServerIps:{0} ", string.Join<IPAddress>(",", this.DnsServerIps));
			stringBuilder.AppendFormat("MonitorDomain:{0}", this.MonitorDomain);
			return stringBuilder.ToString();
		}

		public bool IsSupportedZone(string zoneName)
		{
			return this.AllZones.Contains(zoneName, StringComparer.OrdinalIgnoreCase);
		}

		public List<string> GetDomainsToLookup(DomainSelection selectionFilter)
		{
			WTFDiagnostics.TraceInformation<DomainSelection>(ExTraceGlobals.DNSTracer, this.TraceContext, "DnsConfiguration: Trying to get Domains for filter={0}", selectionFilter, null, "GetDomainsToLookup", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsConfiguration.cs", 169);
			List<string> list = new List<string>();
			switch (selectionFilter)
			{
			case DomainSelection.AnyTargetServiceAnyZone:
			{
				Tuple<string, string> randomItem = this.GetRandomItem<Tuple<string, string>>(this.IpV4TargetServices);
				list.Add(string.Format("{0}--{1}--{2}.{3}", new object[]
				{
					this.MonitorDomain,
					randomItem.Item1,
					randomItem.Item2,
					this.GetRandomItem<string>(this.AllZones)
				}));
				goto IL_3C6;
			}
			case DomainSelection.AllTargetServicesAnyZone:
				using (List<Tuple<string, string>>.Enumerator enumerator = this.IpV4TargetServices.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Tuple<string, string> tuple = enumerator.Current;
						list.Add(string.Format("{0}--{1}--{2}.{3}", new object[]
						{
							this.MonitorDomain,
							tuple.Item1,
							tuple.Item2,
							this.GetRandomItem<string>(this.AllZones)
						}));
					}
					goto IL_3C6;
				}
				break;
			case DomainSelection.AnyTargetServiceAllZones:
				break;
			case DomainSelection.Ipv6AnyTargetServiceAnyZone:
				goto IL_1BB;
			case DomainSelection.Ipv6AllTargetServicesAnyZone:
				using (List<Tuple<string, string>>.Enumerator enumerator2 = this.IpV6TargetServices.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Tuple<string, string> tuple2 = enumerator2.Current;
						list.Add(string.Format("{0}--{1}--{2}.{3}", new object[]
						{
							this.IpV6Prefix + this.MonitorDomain,
							tuple2.Item1,
							tuple2.Item2,
							this.GetRandomItem<string>(this.AllZones)
						}));
					}
					goto IL_3C6;
				}
				goto IL_2AC;
			case DomainSelection.Ipv6AnyTargetServiceAllZones:
				goto IL_2AC;
			case DomainSelection.AnyZone:
				goto IL_339;
			case DomainSelection.AllZones:
				list.AddRange(this.AllZones);
				goto IL_3C6;
			case DomainSelection.InvalidDomainWithFallback:
				if (this.ZonesWithFallback.Count > 0)
				{
					list.Add(string.Format("InvalidDomainWithFallback.{0}", this.GetRandomItem<string>(this.ZonesWithFallback)));
					goto IL_3C6;
				}
				goto IL_3C6;
			case DomainSelection.InvalidDomainWithoutFallback:
				if (this.ZonesWithoutFallback.Count > 0)
				{
					list.Add(string.Format("InvalidDomainWithoutFallback.{0}", this.GetRandomItem<string>(this.ZonesWithoutFallback)));
					goto IL_3C6;
				}
				goto IL_3C6;
			case DomainSelection.InvalidZone:
				list.Add("InvalidZone.com");
				goto IL_3C6;
			default:
				throw new NotImplementedException();
			}
			Tuple<string, string> randomItem2 = this.GetRandomItem<Tuple<string, string>>(this.IpV4TargetServices);
			using (List<string>.Enumerator enumerator3 = this.AllZones.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					string text = enumerator3.Current;
					list.Add(string.Format("{0}--{1}--{2}.{3}", new object[]
					{
						this.MonitorDomain,
						randomItem2.Item1,
						randomItem2.Item2,
						text
					}));
				}
				goto IL_3C6;
			}
			IL_1BB:
			Tuple<string, string> randomItem3 = this.GetRandomItem<Tuple<string, string>>(this.IpV6TargetServices);
			list.Add(string.Format("{0}--{1}--{2}.{3}", new object[]
			{
				this.IpV6Prefix + this.MonitorDomain,
				randomItem3.Item1,
				randomItem3.Item2,
				this.GetRandomItem<string>(this.AllZones)
			}));
			goto IL_3C6;
			IL_2AC:
			Tuple<string, string> randomItem4 = this.GetRandomItem<Tuple<string, string>>(this.IpV6TargetServices);
			using (List<string>.Enumerator enumerator4 = this.AllZones.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					string text2 = enumerator4.Current;
					list.Add(string.Format("{0}--{1}--{2}.{3}", new object[]
					{
						this.IpV6Prefix + this.MonitorDomain,
						randomItem4.Item1,
						randomItem4.Item2,
						text2
					}));
				}
				goto IL_3C6;
			}
			IL_339:
			list.Add(this.GetRandomItem<string>(this.AllZones));
			IL_3C6:
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DNSTracer, this.TraceContext, "DnsConfiguration: Domains identified={0}", string.Join(",", list), null, "GetDomainsToLookup", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsConfiguration.cs", 278);
			return list;
		}

		protected abstract void InitDnsServerIps(bool useSingleDnsServer);

		protected abstract void InitSupportedZones();

		protected abstract void InitSupportedTargetServices();

		protected abstract void InitMonitorDomain();

		protected abstract void InitIpV6Prefix();

		private T GetRandomItem<T>(List<T> items)
		{
			int index = this.randomGenerator.Next(items.Count);
			return items[index];
		}

		private void LoadConfig(XmlElement configNode)
		{
			this.LoadSupportedZones(configNode);
			this.LoadSupportedTargetServices(configNode);
			this.LoadMonitorDomain(configNode);
			this.LoadIpV6Prefix(configNode);
			this.LoadDnsServerIps(configNode);
		}

		private void LoadDnsServerIps(XmlElement configNode)
		{
			bool flag = false;
			XmlElement xmlElement = null;
			if (configNode != null)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, this.TraceContext, "DnsConfiguration: Parsing /DnsServerIps", null, "LoadDnsServerIps", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsConfiguration.cs", 345);
				flag = Utils.GetBoolean(configNode.GetAttribute("UseSingleDnsServer"), "DnsConfiguration.UseSingleDnsServer");
				xmlElement = (configNode.SelectSingleNode("DnsServerIps") as XmlElement);
			}
			WTFDiagnostics.TraceInformation<bool>(ExTraceGlobals.DNSTracer, this.TraceContext, "DnsConfiguration: DnsConfiguration.UseSingleDnsServer={0}", flag, null, "LoadDnsServerIps", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsConfiguration.cs", 350);
			if (xmlElement == null || Utils.GetBoolean(xmlElement.GetAttribute("AutoDetect"), "DnsServerIps.AutoDetect"))
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, this.TraceContext, "DnsConfiguration: detecting DnsServerIps", null, "LoadDnsServerIps", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsConfiguration.cs", 356);
				this.InitDnsServerIps(flag);
				return;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, this.TraceContext, "DnsConfiguration: reading DnsServerIps", null, "LoadDnsServerIps", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsConfiguration.cs", 363);
			this.DnsServerIps = new List<IPAddress>();
			foreach (object obj in xmlElement.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlElement xmlElement2 = xmlNode as XmlElement;
				if (xmlElement2 != null)
				{
					string ipString = Utils.CheckNullOrWhiteSpace(xmlElement2.GetAttribute("Address"), "DnsServerIps/Ip.Address");
					this.DnsServerIps.Add(IPAddress.Parse(ipString));
				}
			}
		}

		private void LoadSupportedZones(XmlElement configNode)
		{
			XmlElement xmlElement = null;
			if (configNode != null)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, this.TraceContext, "DnsConfiguration: Parsing /SupportedZones", null, "LoadSupportedZones", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsConfiguration.cs", 391);
				xmlElement = (configNode.SelectSingleNode("SupportedZones") as XmlElement);
			}
			if (xmlElement == null || Utils.GetBoolean(xmlElement.GetAttribute("AutoDetect"), "SupportedZones.AutoDetect"))
			{
				this.InitSupportedZones();
				return;
			}
			this.ZonesWithFallback = new List<string>();
			this.ZonesWithoutFallback = new List<string>();
			this.AllZones = new List<string>();
			foreach (object obj in xmlElement.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlElement xmlElement2 = xmlNode as XmlElement;
				if (xmlElement2 != null)
				{
					string item = Utils.CheckNullOrWhiteSpace(xmlElement2.GetAttribute("Name"), "SupportedZones/Zone.Name");
					bool boolean = Utils.GetBoolean(xmlElement2.GetAttribute("HasFallback"), "SupportedZones/Zone.HasFallback");
					if (boolean)
					{
						this.ZonesWithFallback.Add(item);
					}
					else
					{
						this.ZonesWithoutFallback.Add(item);
					}
					this.AllZones.Add(item);
				}
			}
		}

		private void LoadSupportedTargetServices(XmlElement configNode)
		{
			XmlElement xmlElement = null;
			XmlElement xmlElement2 = null;
			if (configNode != null)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, this.TraceContext, "DnsConfiguration: Parsing /SupportedTargetServices", null, "LoadSupportedTargetServices", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsConfiguration.cs", 446);
				xmlElement = (configNode.SelectSingleNode("SupportedTargetServices") as XmlElement);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, this.TraceContext, "DnsConfiguration: Parsing /IpV4OnlyVersions", null, "LoadSupportedTargetServices", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsConfiguration.cs", 449);
				xmlElement2 = (configNode.SelectSingleNode("IpV4OnlyVersions") as XmlElement);
			}
			if (xmlElement == null || Utils.GetBoolean(xmlElement.GetAttribute("AutoDetect"), "SupportedTargetServices.AutoDetect"))
			{
				this.InitSupportedTargetServices();
				return;
			}
			HashSet<string> hashSet = new HashSet<string>();
			if (xmlElement2 != null)
			{
				foreach (object obj in xmlElement2.SelectNodes("Version"))
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlElement xmlElement3 = xmlNode as XmlElement;
					if (xmlElement3 != null)
					{
						hashSet.Add(Utils.CheckNullOrWhiteSpace(xmlElement3.GetAttribute("Value"), "IpV4OnlyVersions/Version.Value"));
					}
				}
			}
			this.IpV6TargetServices = new List<Tuple<string, string>>();
			this.IpV4TargetServices = new List<Tuple<string, string>>();
			foreach (object obj2 in xmlElement.SelectNodes("TargetService"))
			{
				XmlNode xmlNode2 = (XmlNode)obj2;
				XmlElement xmlElement4 = xmlNode2 as XmlElement;
				if (xmlElement4 != null)
				{
					Tuple<string, string> tuple = new Tuple<string, string>(Utils.CheckNullOrWhiteSpace(xmlElement4.GetAttribute("Region"), "SupportedTargetServices/TargetService.Region"), Utils.CheckNullOrWhiteSpace(xmlElement4.GetAttribute("Version"), "SupportedTargetServices/TargetService.Version"));
					this.IpV4TargetServices.Add(tuple);
					if (!hashSet.Contains(tuple.Item2))
					{
						this.IpV6TargetServices.Add(tuple);
					}
				}
			}
		}

		private void LoadMonitorDomain(XmlElement configNode)
		{
			XmlElement xmlElement = null;
			if (configNode != null)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, this.TraceContext, "DnsConfiguration: Parsing /MonitorDomain", null, "LoadMonitorDomain", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsConfiguration.cs", 517);
				xmlElement = (configNode.SelectSingleNode("MonitorDomain") as XmlElement);
			}
			if (xmlElement == null || Utils.GetBoolean(xmlElement.GetAttribute("AutoDetect"), "MonitorDomain.AutoDetect"))
			{
				this.InitMonitorDomain();
				return;
			}
			this.MonitorDomain = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Name"), "MonitorDomain.Name");
		}

		private void LoadIpV6Prefix(XmlElement configNode)
		{
			XmlElement xmlElement = null;
			if (configNode != null)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, this.TraceContext, "DnsConfiguration: Parsing /IpV6Prefix", null, "LoadIpV6Prefix", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsConfiguration.cs", 546);
				xmlElement = (configNode.SelectSingleNode("IpV6Prefix") as XmlElement);
			}
			if (xmlElement == null || Utils.GetBoolean(xmlElement.GetAttribute("AutoDetect"), "IpV6Prefix.AutoDetect"))
			{
				this.InitIpV6Prefix();
				return;
			}
			this.IpV6Prefix = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Value"), "IpV6Prefix.Value");
		}

		private void Validate()
		{
			if (string.IsNullOrWhiteSpace(this.MonitorDomain))
			{
				throw new DnsMonitorException("DnsConfiguration: Monitor domain was not found", null);
			}
			if (this.IpV4TargetServices.Count == 0)
			{
				throw new DnsMonitorException("DnsConfiguration: IPV4 Target services were not found", null);
			}
			if (this.IpV6TargetServices.Count == 0)
			{
				throw new DnsMonitorException("DnsConfiguration: IPV6 Target services were not found", null);
			}
			if (this.AllZones.Count == 0)
			{
				throw new DnsMonitorException("DnsConfiguration: Supported zones were not found", null);
			}
			if (this.DnsServerIps.Count == 0)
			{
				throw new DnsMonitorException("DnsConfiguration: DNS++ server IPs were not found", null);
			}
		}

		private const string DomainKeyFormat = "{0}--{1}--{2}.{3}";

		private Random randomGenerator = new Random();
	}
}
