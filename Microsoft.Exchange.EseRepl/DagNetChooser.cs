using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Cluster.Common;

namespace Microsoft.Exchange.EseRepl
{
	public class DagNetChooser
	{
		private static ITracer Tracer
		{
			get
			{
				return Dependencies.DagNetChooserTracer;
			}
		}

		public static DagNetRoute[] ProposeRoutes(string targetServer, out DagNetConfig dagNetConfig)
		{
			DagNetChooser netChooser = DagNetEnvironment.NetChooser;
			return netChooser.BuildRoutes(targetServer, out dagNetConfig);
		}

		public DagNetRoute[] BuildRoutes(string targetServer, out DagNetConfig dagNetConfig)
		{
			return this.BuildRoutes(targetServer, false, null, out dagNetConfig);
		}

		public DagNetRoute[] BuildRoutes(string targetServer, bool dnsOnly, string chooseNetworkByName, out DagNetConfig dagNetConfig)
		{
			lock (this)
			{
				this.LoadMapIfNeeded();
				DagNetChooser.OutboundNetInfo[] array = this.FetchNetworkOrdering();
				if (array.Length > 0)
				{
					List<DagNetRoute> list = new List<DagNetRoute>(array.Length);
					foreach (DagNetChooser.OutboundNetInfo outboundNetInfo in array)
					{
						DagNetChooser.OutboundNetInfo.TargetNicInfo targetNicInfo;
						if (outboundNetInfo.Targets.TryGetValue(targetServer, out targetNicInfo) && (!dnsOnly || outboundNetInfo.Network.IsDnsMapped) && (chooseNetworkByName == null || StringUtil.IsEqualIgnoreCase(chooseNetworkByName, outboundNetInfo.Network.Name)))
						{
							list.Add(new DagNetRoute
							{
								NetworkName = outboundNetInfo.Network.Name,
								SourceIPAddr = outboundNetInfo.SourceIPAddr,
								TargetPort = targetNicInfo.TargetPort,
								TargetIPAddr = targetNicInfo.IPAddr,
								IsCrossSubnet = targetNicInfo.IsCrossSubnet
							});
							if (dnsOnly || chooseNetworkByName != null)
							{
								break;
							}
						}
					}
					dagNetConfig = this.netConfig;
					return list.ToArray();
				}
			}
			dagNetConfig = this.netConfig;
			return null;
		}

		private void LoadMapIfNeeded()
		{
			DagNetConfig dagNetConfig = DagNetEnvironment.FetchNetConfig();
			if (!object.ReferenceEquals(dagNetConfig, this.netConfig))
			{
				this.LoadMap(dagNetConfig);
			}
		}

		private void LoadMap(DagNetConfig cfg)
		{
			this.netConfig = cfg;
			Dictionary<string, DagNetChooser.OutboundNetInfo> dictionary = new Dictionary<string, DagNetChooser.OutboundNetInfo>(cfg.Networks.Count);
			foreach (DagNetwork dagNetwork in cfg.Networks)
			{
				if (dagNetwork.ReplicationEnabled)
				{
					DagNetChooser.OutboundNetInfo outboundNetInfo = new DagNetChooser.OutboundNetInfo();
					outboundNetInfo.Network = dagNetwork;
					dictionary.Add(dagNetwork.Name, outboundNetInfo);
				}
			}
			foreach (DagNode dagNode in cfg.Nodes)
			{
				foreach (DagNode.Nic nic in dagNode.Nics)
				{
					IPAddress ipaddress = NetworkUtil.ConvertStringToIpAddress(nic.IpAddress);
					DagNetChooser.OutboundNetInfo outboundNetInfo2;
					if (ipaddress != null && dictionary.TryGetValue(nic.NetworkName, out outboundNetInfo2))
					{
						if (MachineName.IsEffectivelyLocalComputerName(dagNode.Name))
						{
							outboundNetInfo2.SourceIPAddr = ipaddress;
						}
						else
						{
							if (outboundNetInfo2.Targets == null)
							{
								outboundNetInfo2.Targets = new Dictionary<string, DagNetChooser.OutboundNetInfo.TargetNicInfo>(cfg.Nodes.Count, MachineName.Comparer);
							}
							DagNetChooser.OutboundNetInfo.TargetNicInfo targetNicInfo;
							if (outboundNetInfo2.Targets.TryGetValue(dagNode.Name, out targetNicInfo))
							{
								DagNetChooser.Tracer.TraceError((long)this.GetHashCode(), "LoadMap found dup ipAddr for node {0} on net {1} ip1 {2} ip2 {3}", new object[]
								{
									dagNode.Name,
									nic.NetworkName,
									targetNicInfo.IPAddr,
									nic.IpAddress
								});
							}
							else
							{
								targetNicInfo = new DagNetChooser.OutboundNetInfo.TargetNicInfo();
								targetNicInfo.IPAddr = ipaddress;
								targetNicInfo.IsCrossSubnet = true;
								targetNicInfo.TargetPort = this.netConfig.ReplicationPort;
								if (dagNode.ReplicationPort != 0)
								{
									targetNicInfo.TargetPort = dagNode.ReplicationPort;
								}
								outboundNetInfo2.Targets.Add(dagNode.Name, targetNicInfo);
							}
						}
					}
				}
			}
			List<DagNetChooser.OutboundNetInfo> list = new List<DagNetChooser.OutboundNetInfo>(cfg.Networks.Count);
			int num = 0;
			foreach (DagNetChooser.OutboundNetInfo outboundNetInfo3 in dictionary.Values)
			{
				if (outboundNetInfo3.SourceIPAddr != null && outboundNetInfo3.Targets != null)
				{
					if (outboundNetInfo3.Network.IsDnsMapped)
					{
						list.Add(outboundNetInfo3);
					}
					else
					{
						list.Insert(0, outboundNetInfo3);
						num++;
					}
				}
			}
			this.roundRobinIndex = 0;
			this.outboundNets = list.ToArray();
			if (num > 0)
			{
				this.roundRobinLimit = num;
				return;
			}
			this.roundRobinLimit = this.outboundNets.Length;
		}

		private DagNetChooser.OutboundNetInfo[] FetchNetworkOrdering()
		{
			DagNetChooser.OutboundNetInfo[] array = new DagNetChooser.OutboundNetInfo[this.outboundNets.Length];
			int num = this.roundRobinIndex;
			this.roundRobinIndex = DagNetEnvironment.CircularIncrement(this.roundRobinIndex, this.roundRobinLimit);
			int num2 = 0;
			for (int i = num; i < this.roundRobinLimit; i++)
			{
				array[num2++] = this.outboundNets[i];
			}
			for (int i = 0; i < num; i++)
			{
				array[num2++] = this.outboundNets[i];
			}
			for (int i = this.roundRobinLimit; i < this.outboundNets.Length; i++)
			{
				array[num2++] = this.outboundNets[i];
			}
			return array;
		}

		private DagNetConfig netConfig;

		private DagNetChooser.OutboundNetInfo[] outboundNets;

		private int roundRobinLimit;

		private int roundRobinIndex;

		private class OutboundNetInfo
		{
			public DagNetwork Network { get; set; }

			public IPAddress SourceIPAddr { get; set; }

			public Dictionary<string, DagNetChooser.OutboundNetInfo.TargetNicInfo> Targets { get; set; }

			public class TargetNicInfo
			{
				public TargetNicInfo()
				{
					this.IsCrossSubnet = true;
				}

				public IPAddress IPAddr { get; set; }

				public bool IsCrossSubnet { get; set; }

				public int TargetPort { get; set; }
			}
		}
	}
}
