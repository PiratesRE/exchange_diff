using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal static class AmClusterResourceHelper
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ClusterTracer;
			}
		}

		public static string ConstructIPDependencyExpression(ILogTraceHelper logger, IEnumerable<AmClusterResource> ipResList, bool enforceIPv4AndIPv6)
		{
			int capacity = 80;
			StringBuilder stringBuilder = new StringBuilder(capacity);
			StringBuilder stringBuilder2 = new StringBuilder(capacity);
			StringBuilder stringBuilder3 = new StringBuilder(capacity);
			logger = (logger ?? NullLogTraceHelper.GetNullLogger());
			foreach (AmClusterResource amClusterResource in ipResList)
			{
				if (amClusterResource.IsIpv4())
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" OR ");
					}
					stringBuilder.Append("[" + amClusterResource.Name + "]");
				}
				else
				{
					if (stringBuilder2.Length > 0)
					{
						stringBuilder2.Append(" OR ");
					}
					stringBuilder2.Append("[" + amClusterResource.Name + "]");
				}
			}
			if (enforceIPv4AndIPv6)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder3.Append("(" + stringBuilder + ")");
				}
				if (stringBuilder2.Length > 0 && stringBuilder.Length > 0)
				{
					stringBuilder3.Append(" AND ");
				}
				if (stringBuilder2.Length > 0)
				{
					stringBuilder3.Append("(" + stringBuilder2 + ")");
				}
			}
			else
			{
				stringBuilder3.Append(stringBuilder);
				if (stringBuilder2.Length > 0 && stringBuilder.Length > 0)
				{
					stringBuilder3.Append(" OR ");
				}
				stringBuilder3.Append(stringBuilder2);
			}
			logger.AppendLogMessage("Constructed dependency expression: {0} (enforceIPv4AndIPv6={1})", new object[]
			{
				stringBuilder3,
				enforceIPv4AndIPv6
			});
			return stringBuilder3.ToString();
		}

		public static void ValidateIpv4Addresses(IAmCluster cluster, IPAddress[] ipv4Addresses)
		{
			for (int i = 0; i < ipv4Addresses.Length - 1; i++)
			{
				for (int j = i + 1; j < ipv4Addresses.Length; j++)
				{
					if (object.Equals(ipv4Addresses[i], ipv4Addresses[j]))
					{
						throw new IPv4AddressesHasDuplicateEntries(ipv4Addresses[i].ToString());
					}
				}
			}
			if (cluster != null)
			{
				IDictionary<IPAddress, AmClusterNetwork> dictionary = null;
				try
				{
					dictionary = AmClusterResourceHelper.GetIpv4StaticNetworkMap(cluster, ipv4Addresses);
				}
				finally
				{
					if (dictionary != null)
					{
						SharedHelper.DisposeObjectList<AmClusterNetwork>(dictionary.Values);
					}
				}
			}
		}

		public static void ValidateIpv4Networks(IAmCluster cluster, string[] ipv4Networks)
		{
			for (int i = 0; i < ipv4Networks.Length - 1; i++)
			{
				for (int j = i + 1; j < ipv4Networks.Length; j++)
				{
					if (SharedHelper.StringIEquals(ipv4Networks[i], ipv4Networks[j]))
					{
						throw new IPv4NetworksHasDuplicateEntries(ipv4Networks[i]);
					}
				}
			}
			Dictionary<string, AmClusterNetwork> ipv4DhcpNetworkMap = AmClusterResourceHelper.GetIpv4DhcpNetworkMap(cluster, ipv4Networks);
			if (ipv4DhcpNetworkMap != null)
			{
				SharedHelper.DisposeObjectList<AmClusterNetwork>(ipv4DhcpNetworkMap.Values);
			}
		}

		public static void ValidateIpv6Networks(IAmCluster cluster, string[] ipv6Networks)
		{
			for (int i = 0; i < ipv6Networks.Length - 1; i++)
			{
				for (int j = i + 1; j < ipv6Networks.Length; j++)
				{
					if (SharedHelper.StringIEquals(ipv6Networks[i], ipv6Networks[j]))
					{
						throw new IPv6NetworksHasDuplicateEntries(ipv6Networks[i]);
					}
				}
			}
			Dictionary<string, AmClusterNetwork> ipv6AutoCfgNetworkMap = AmClusterResourceHelper.GetIpv6AutoCfgNetworkMap(cluster, ipv6Networks);
			if (ipv6AutoCfgNetworkMap != null)
			{
				SharedHelper.DisposeObjectList<AmClusterNetwork>(ipv6AutoCfgNetworkMap.Values);
			}
		}

		public static Dictionary<IPAddress, AmClusterNetwork> GetIpv4StaticNetworkMap(IAmCluster cluster, IPAddress[] ipv4Addresses)
		{
			Dictionary<IPAddress, AmClusterNetwork> dictionary = new Dictionary<IPAddress, AmClusterNetwork>(8);
			foreach (IPAddress ipaddress in ipv4Addresses)
			{
				AmClusterNetwork amClusterNetwork = cluster.FindNetworkByIPv4Address(ipaddress);
				if (amClusterNetwork == null)
				{
					throw new FailedToFindNetwork(ipaddress.ToString());
				}
				dictionary[ipaddress] = amClusterNetwork;
			}
			return dictionary;
		}

		public static Dictionary<string, AmClusterNetwork> GetIpv4DhcpNetworkMap(IAmCluster cluster, string[] ipv4Networks)
		{
			Dictionary<string, AmClusterNetwork> dictionary = new Dictionary<string, AmClusterNetwork>(8);
			foreach (string text in ipv4Networks)
			{
				AmClusterNetwork amClusterNetwork = cluster.FindNetworkByName(text, IPVersion.IPv4);
				if (amClusterNetwork == null)
				{
					throw new FailedToFindNetwork(text);
				}
				if (!amClusterNetwork.SupportsIPv4Dhcp())
				{
					throw new RequestedNetworkIsNotDhcpEnabled(text);
				}
				dictionary[text] = amClusterNetwork;
			}
			return dictionary;
		}

		public static Dictionary<string, AmClusterNetwork> GetIpv6AutoCfgNetworkMap(IAmCluster cluster, string[] ipv6Networks)
		{
			Dictionary<string, AmClusterNetwork> dictionary = new Dictionary<string, AmClusterNetwork>(8);
			foreach (string text in ipv6Networks)
			{
				AmClusterNetwork amClusterNetwork = cluster.FindNetworkByName(text, IPVersion.IPv6);
				if (amClusterNetwork == null)
				{
					throw new FailedToFindNetwork(text);
				}
				if (!amClusterNetwork.SupportsIPv6AutoConfiguration())
				{
					throw new RequestedNetworkIsNotIPv6Enabled(text);
				}
				dictionary[text] = amClusterNetwork;
			}
			return dictionary;
		}

		public static List<AmClusterResource> ConfigureIPv4DhcpResources(ILogTraceHelper logger, AmClusterGroup group, string[] ipv4Networks)
		{
			bool flag = false;
			Dictionary<string, AmClusterResource> dictionary = new Dictionary<string, AmClusterResource>(8);
			Dictionary<string, AmClusterNetwork> dictionary2 = null;
			AmClusterResource[] array = null;
			List<AmClusterResource> list = new List<AmClusterResource>(10);
			List<AmClusterResource> list2 = new List<AmClusterResource>(10);
			List<AmClusterResource> list3 = null;
			try
			{
				logger = (logger ?? NullLogTraceHelper.GetNullLogger());
				dictionary2 = AmClusterResourceHelper.GetIpv4DhcpNetworkMap(group.OwningCluster, ipv4Networks);
				IEnumerable<AmClusterResource> enumerable = group.EnumerateResourcesOfType("IP Address");
				if (enumerable != null)
				{
					array = enumerable.ToArray<AmClusterResource>();
					for (int i = 0; i < array.Length; i++)
					{
						AmClusterResource amClusterResource = array[i];
						if (amClusterResource.GetPrivateProperty<int>("EnableDhcp") != 0)
						{
							list.Add(amClusterResource);
							array[i] = null;
						}
					}
				}
				foreach (string key in ipv4Networks)
				{
					for (int k = 0; k < list.Count; k++)
					{
						AmClusterResource amClusterResource2 = list[k];
						string privateProperty = amClusterResource2.GetPrivateProperty<string>("Network");
						AmClusterNetwork amClusterNetwork = dictionary2[key];
						if (SharedHelper.StringIEquals(amClusterNetwork.Name, privateProperty))
						{
							logger.AppendLogMessage("Reusing ipv4 dhcp resource because its network is matching. (resource:{0}, network:{1}/{2}", new object[]
							{
								amClusterResource2.Name,
								privateProperty,
								amClusterNetwork.Name
							});
							dictionary[key] = amClusterResource2;
							list[k] = null;
							break;
						}
					}
				}
				if (dictionary.Count < ipv4Networks.Length)
				{
					int num = 1;
					foreach (string text in ipv4Networks)
					{
						if (!dictionary.ContainsKey(text))
						{
							AmClusterResource amClusterResource2 = (AmClusterResource)group.CreateUniqueResource(logger, "IPv4 DHCP Address", "IP Address", ref num);
							list2.Add(amClusterResource2);
							logger.AppendLogMessage("Created new ipv4 dhcp resource. (resource:{0}, network:{1})", new object[]
							{
								amClusterResource2.Name,
								text
							});
							dictionary[text] = amClusterResource2;
						}
					}
				}
				list3 = new List<AmClusterResource>(ipv4Networks.Length);
				foreach (string key2 in ipv4Networks)
				{
					AmClusterResource amClusterResource2 = dictionary[key2];
					AmClusterNetwork amClusterNetwork = dictionary2[key2];
					int num2;
					using (AmClusterPropListDisposable amClusterPropListDisposable = AmClusPropListMaker.CreatePropListInt("EnableDhcp", 1, out num2))
					{
						using (AmClusterPropListDisposable amClusterPropListDisposable2 = AmClusPropListMaker.DupeAndAppendPropListString(amClusterPropListDisposable.RawBuffer, (int)amClusterPropListDisposable.BufferSize, "Network", amClusterNetwork.Name, out num2))
						{
							logger.AppendLogMessage("ConfigureIPv4DhcpResources: Setting resource '{0}' to be DHCP-enabled on network '{1}' (role={2}).", new object[]
							{
								amClusterResource2.Name,
								amClusterNetwork.Name,
								amClusterNetwork.GetNativeRole()
							});
							amClusterResource2.SetPrivatePropertyList(amClusterPropListDisposable2);
						}
					}
					AmClusterResourceHelper.SetPossibleOwnersForIpResource(logger, amClusterResource2);
					list3.Add(amClusterResource2);
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					SharedHelper.DisposeObjectList<AmClusterResource>(list2);
				}
				SharedHelper.DisposeObjectList<AmClusterResource>(array);
				SharedHelper.DisposeObjectList<AmClusterResource>(list);
				if (dictionary2 != null)
				{
					SharedHelper.DisposeObjectList<AmClusterNetwork>(dictionary2.Values);
				}
			}
			return list3;
		}

		public static List<AmClusterResource> ConfigureIPv6AutoCfgResources(ILogTraceHelper logger, AmClusterGroup group, string[] ipv6Networks)
		{
			bool flag = false;
			Dictionary<string, AmClusterResource> dictionary = new Dictionary<string, AmClusterResource>(8);
			Dictionary<string, AmClusterNetwork> dictionary2 = null;
			AmClusterResource[] array = null;
			List<AmClusterResource> list = new List<AmClusterResource>(10);
			List<AmClusterResource> list2 = null;
			try
			{
				logger = (logger ?? NullLogTraceHelper.GetNullLogger());
				dictionary2 = AmClusterResourceHelper.GetIpv6AutoCfgNetworkMap(group.OwningCluster, ipv6Networks);
				IEnumerable<AmClusterResource> source = group.EnumerateResourcesOfType("IPv6 Address");
				if (source.Count<AmClusterResource>() > 0)
				{
					array = source.ToArray<AmClusterResource>();
				}
				if (array != null)
				{
					foreach (string key in ipv6Networks)
					{
						for (int j = 0; j < array.Length; j++)
						{
							AmClusterResource amClusterResource = array[j];
							string privateProperty = amClusterResource.GetPrivateProperty<string>("Network");
							AmClusterNetwork amClusterNetwork = dictionary2[key];
							if (SharedHelper.StringIEquals(amClusterNetwork.Name, privateProperty))
							{
								logger.AppendLogMessage("Reusing ipv6 resource because its network is matching. (resource:{0}, network:{1}/{2}", new object[]
								{
									amClusterResource.Name,
									privateProperty,
									amClusterNetwork.Name
								});
								dictionary[key] = amClusterResource;
								array[j] = null;
								break;
							}
						}
					}
				}
				if (dictionary.Count < ipv6Networks.Length)
				{
					int num = 1;
					foreach (string text in ipv6Networks)
					{
						if (!dictionary.ContainsKey(text))
						{
							AmClusterResource amClusterResource = (AmClusterResource)group.CreateUniqueResource(logger, "IPv6 Auto Config Address", "IPv6 Address", ref num);
							list.Add(amClusterResource);
							logger.AppendLogMessage("Created new ipv6 resource. (resource:{0}, network:{1})", new object[]
							{
								amClusterResource.Name,
								text
							});
							dictionary[text] = amClusterResource;
						}
					}
				}
				list2 = new List<AmClusterResource>(ipv6Networks.Length);
				foreach (string key2 in ipv6Networks)
				{
					AmClusterResource amClusterResource = dictionary[key2];
					AmClusterNetwork amClusterNetwork = dictionary2[key2];
					logger.AppendLogMessage("ConfigureIPv6AutoCfgResources: Setting resource '{0}' to be on network '{1}' (role={2}).", new object[]
					{
						amClusterResource.Name,
						amClusterNetwork.Name,
						amClusterNetwork.GetNativeRole()
					});
					amClusterResource.SetPrivateProperty<string>("Network", amClusterNetwork.Name);
					AmClusterResourceHelper.SetPossibleOwnersForIpResource(logger, amClusterResource);
					list2.Add(amClusterResource);
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					SharedHelper.DisposeObjectList<AmClusterResource>(list);
				}
				SharedHelper.DisposeObjectList<AmClusterResource>(array);
				if (dictionary2 != null)
				{
					SharedHelper.DisposeObjectList<AmClusterNetwork>(dictionary2.Values);
				}
			}
			return list2;
		}

		internal static AmClusterNetwork GetNetworkFromIpResource(ILogTraceHelper logger, AmClusterResource ipRes)
		{
			AmClusterNetwork result = null;
			string networkNameFromIpResource = AmClusterResourceHelper.GetNetworkNameFromIpResource(logger, ipRes);
			if (!string.IsNullOrEmpty(networkNameFromIpResource))
			{
				result = ipRes.OwningCluster.OpenNetwork(networkNameFromIpResource);
			}
			return result;
		}

		internal static string GetNetworkNameFromIpResource(ILogTraceHelper logger, AmClusterResource ipRes)
		{
			logger = (logger ?? NullLogTraceHelper.GetNullLogger());
			if (!ipRes.IsIpv4() && !ipRes.IsIpv6())
			{
				throw new ArgumentException("GetNetworkFromIpResource() needs an IP resource.");
			}
			return ipRes.GetPrivateProperty<string>("Network");
		}

		internal static void SetPossibleOwnersForIpResource(ILogTraceHelper output, AmClusterResource ipRes)
		{
			ipRes.SetAllPossibleOwnerNodes();
		}

		internal static IEnumerable<AmClusterResource> GetIpResourcesFromNetName(AmClusterGroup resGroup, AmClusterResource netName)
		{
			if (!SharedHelper.StringIEquals(netName.GetTypeName(), "Network Name"))
			{
				throw new ArgumentException("GetIpResourceFromNetName() needs a NetName resource.");
			}
			List<AmClusterResource> list = new List<AmClusterResource>(8);
			list.AddRange(resGroup.EnumerateResourcesOfType("IP Address"));
			list.AddRange(resGroup.EnumerateResourcesOfType("IPv6 Address"));
			return list;
		}

		internal static LocalizedString FixUpIpAddressesForNetName(ITaskOutputHelper logger, IAmCluster cluster, AmClusterGroup group, AmClusterResource netname, IEnumerable<IPAddress> staticIpAddresses)
		{
			logger = (logger ?? NullTaskOutputHelper.GetNullLogger());
			LocalizedString localizedString = Strings.FixUpIpAddressStatusUnchanged;
			if (staticIpAddresses != null && staticIpAddresses.Count<IPAddress>() == 1 && IPAddress.Any.Equals(staticIpAddresses.ElementAt(0)))
			{
				logger.AppendLogMessage("DAG IP Address was specified in AD, but it's the sentinel value for forcing DHCP ({0}).", new object[]
				{
					IPAddress.Any
				});
				staticIpAddresses = new IPAddress[0];
			}
			bool flag = staticIpAddresses != null && staticIpAddresses.Count<IPAddress>() > 0;
			bool flag2 = staticIpAddresses != null && staticIpAddresses.Count<IPAddress>() == 0;
			logger.AppendLogMessage("FixUpIpAddressesForNetName: goingToStaticIps: {0}, goingToDynamicIps = {1}.", new object[]
			{
				flag,
				flag2
			});
			if (flag)
			{
				logger.AppendLogMessage("FixUpIpAddressesForNetName: The static IPs specified are:", new object[0]);
				foreach (IPAddress ipaddress in staticIpAddresses)
				{
					logger.AppendLogMessage("  -> {0}", new object[]
					{
						ipaddress
					});
				}
			}
			IEnumerable<AmClusterNetwork> enumerable = null;
			IEnumerable<AmClusterResource> enumerable2 = null;
			IEnumerable<AmClusterResource> enumerable3 = null;
			IEnumerable<AmClusterResource> enumerable4 = null;
			try
			{
				enumerable = cluster.EnumerateNetworks();
				IEnumerable<AmClusterNetwork> enumerable5 = AmClusterResourceHelper.FilterPublicNetworksFromAllNetworks(enumerable);
				enumerable2 = group.EnumerateResourcesOfType("IP Address");
				enumerable3 = group.EnumerateResourcesOfType("IPv6 Address");
				IEnumerable<AmClusterResource> enumerable6 = enumerable2.Concat(enumerable3);
				IList<string> networksWithResources;
				int num = AmClusterResourceHelper.DeleteOrphanedIpAddresses(logger, group, netname, enumerable6, staticIpAddresses, enumerable5, out networksWithResources);
				if (num > 0)
				{
					SharedHelper.DisposeObjectList<AmClusterResource>(enumerable6);
					enumerable2 = group.EnumerateResourcesOfType("IP Address");
					enumerable3 = group.EnumerateResourcesOfType("IPv6 Address");
				}
				if (flag)
				{
					enumerable4 = AmClusterResourceHelper.AddStaticIpAddressesToStrandedNetworks(logger, group, netname, enumerable5, networksWithResources, staticIpAddresses);
				}
				else
				{
					enumerable4 = AmClusterResourceHelper.AddIpAddressesToStrandedNetworks(logger, group, netname, enumerable5, networksWithResources);
				}
				int num2 = enumerable4.Count<AmClusterResource>();
				if (num2 > 0)
				{
					HashSet<string> hashSet = new HashSet<string>();
					foreach (AmClusterResource amClusterResource in enumerable2)
					{
						hashSet.Add(amClusterResource.Name);
					}
					foreach (AmClusterResource amClusterResource2 in enumerable3)
					{
						hashSet.Add(amClusterResource2.Name);
					}
					foreach (AmClusterResource amClusterResource3 in enumerable4)
					{
						hashSet.Add(amClusterResource3.Name);
					}
					string text = string.Format("[{0}]", string.Join("] OR [", hashSet.ToArray<string>()));
					logger.AppendLogMessage("Setting the dependency on netname '{0}' to '{1}'", new object[]
					{
						netname.Name,
						text
					});
					netname.OfflineResource();
					uint num3 = netname.SetDependencyExpression(text);
					logger.AppendLogMessage("SetDependencyExpression returned {0}.", new object[]
					{
						num3
					});
				}
				localizedString = Strings.FixUpIpAddressStatusUpdated(num, num2);
			}
			finally
			{
				SharedHelper.DisposeObjectList<AmClusterNetwork>(enumerable);
				SharedHelper.DisposeObjectList<AmClusterResource>(enumerable2);
				SharedHelper.DisposeObjectList<AmClusterResource>(enumerable3);
				SharedHelper.DisposeObjectList<AmClusterResource>(enumerable4);
				netname.OnlineResource();
			}
			logger.AppendLogMessage("Successfully completed fixing up the IP addresses for netname '{0}'. Changes made: {1}", new object[]
			{
				netname.Name,
				localizedString
			});
			return localizedString;
		}

		internal static IEnumerable<AmClusterNetwork> FilterPublicNetworksFromAllNetworks(IEnumerable<AmClusterNetwork> allNetworks)
		{
			return from network in allNetworks
			where AmClusterResourceHelper.IsMapiNetwork(network.GetNativeRole())
			select network;
		}

		internal static int DeleteOrphanedIpAddresses(ILogTraceHelper logger, AmClusterGroup owningGroup, AmClusterResource netname, IEnumerable<AmClusterResource> currentIpResources, IEnumerable<IPAddress> staticIpAddresses, IEnumerable<AmClusterNetwork> allNetworks, out IList<string> networksWithResources)
		{
			logger = (logger ?? NullLogTraceHelper.GetNullLogger());
			int num = 0;
			networksWithResources = new List<string>(allNetworks.Count<AmClusterNetwork>());
			bool flag = staticIpAddresses != null && staticIpAddresses.Count<IPAddress>() > 0;
			bool flag2 = staticIpAddresses != null && staticIpAddresses.Count<IPAddress>() == 0;
			if (currentIpResources.Count<AmClusterResource>() < 1)
			{
				logger.AppendLogMessage("DeleteOrphanedIpAddresses() found no IP address resources in the cluster at all!", new object[0]);
			}
			else
			{
				IAmCluster owningCluster = owningGroup.OwningCluster;
				Dictionary<string, int> dictionary = new Dictionary<string, int>(networksWithResources.Count<string>());
				IEnumerable<string> enumerable = netname.EnumerateDependentNames();
				logger.AppendLogMessage("Netname resource '{0}' depends on the following resources:", new object[]
				{
					netname.Name
				});
				foreach (string text in enumerable)
				{
					logger.AppendLogMessage("  -> {0}", new object[]
					{
						text
					});
				}
				foreach (AmClusterResource amClusterResource in currentIpResources)
				{
					bool flag3 = false;
					string privateProperty = amClusterResource.GetPrivateProperty<string>("Network");
					int privateProperty2 = amClusterResource.GetPrivateProperty<int>("EnableDhcp");
					logger.AppendLogMessage("Resource '{0}' is on network '{1}' (enableDhcp={2}).", new object[]
					{
						amClusterResource.Name,
						privateProperty,
						privateProperty2
					});
					if (!enumerable.Contains(amClusterResource.Name))
					{
						logger.AppendLogMessage("Resource '{0}' should be deleted because the cluster netname does not depend on it.", new object[]
						{
							amClusterResource.Name
						});
						flag3 = true;
					}
					else if (string.IsNullOrEmpty(privateProperty))
					{
						logger.AppendLogMessage("Resource '{0}' should be deleted because it does not have a valid network.", new object[]
						{
							amClusterResource.Name
						});
						flag3 = true;
					}
					else
					{
						if (flag)
						{
							if (privateProperty2 != 0)
							{
								logger.AppendLogMessage("Resource '{0}' should be deleted because it's a DHCP resource and we're converting to static addresses.", new object[]
								{
									amClusterResource.Name
								});
								flag3 = true;
							}
							else if (amClusterResource.IsIpv6())
							{
								logger.AppendLogMessage("Resource '{0}' should be deleted because it's an IPv6 resource and we're converting to static addresses.", new object[]
								{
									amClusterResource.Name
								});
								flag3 = true;
							}
							else
							{
								string privateProperty3 = amClusterResource.GetPrivateProperty<string>("Address");
								IPAddress ipaddress = IPAddress.Parse(privateProperty3);
								if (!staticIpAddresses.Contains(ipaddress))
								{
									logger.AppendLogMessage("Resource '{0}' should be deleted because it's a static address ({1}) that is not in the list of updated addresses.", new object[]
									{
										amClusterResource.Name,
										ipaddress
									});
									flag3 = true;
								}
							}
						}
						else if (flag2 && privateProperty2 == 0 && amClusterResource.IsIpv4())
						{
							logger.AppendLogMessage("Resource '{0}' should be deleted because it's a static IPv4 resource and we're converting to dynamic addresses.", new object[]
							{
								amClusterResource.Name
							});
							flag3 = true;
						}
						if (!flag3)
						{
							using (AmClusterNetwork networkFromIpResource = AmClusterResourceHelper.GetNetworkFromIpResource(logger, amClusterResource))
							{
								if (networkFromIpResource == null)
								{
									logger.AppendLogMessage("Resource '{0}' should be deleted because it does not have a valid network.", new object[]
									{
										amClusterResource.Name
									});
									flag3 = true;
								}
								else
								{
									dictionary[networkFromIpResource.Name] = 1;
								}
							}
						}
					}
					if (flag3)
					{
						logger.AppendLogMessage("Deleting resource named '{0}' because its network ({1}) does not appear to be valid (enableDhcp={2}).", new object[]
						{
							amClusterResource.Name,
							privateProperty,
							privateProperty2
						});
						amClusterResource.OfflineResource();
						netname.RemoveDependency(amClusterResource);
						amClusterResource.DeleteResource();
						amClusterResource.Dispose();
						num++;
					}
				}
				foreach (string item in dictionary.Keys)
				{
					networksWithResources.Add(item);
				}
			}
			return num;
		}

		private static bool IsMapiNetwork(AmNetworkRole role)
		{
			return role == AmNetworkRole.ClusterNetworkRoleClientAccess || role == AmNetworkRole.ClusterNetworkRoleInternalAndClient;
		}

		private static List<AmClusterResource> AddIpAddressesToStrandedNetworks(ILogTraceHelper logger, AmClusterGroup group, AmClusterResource netname, IEnumerable<AmClusterNetwork> publicNetworks, IEnumerable<string> networksWithResources)
		{
			logger = (logger ?? NullLogTraceHelper.GetNullLogger());
			if (publicNetworks.Count<AmClusterNetwork>() <= networksWithResources.Count<string>())
			{
				logger.AppendLogMessage("AddIpAddressesToStrandedNetworks: publicNetworks.Count({0}) <= networksWithResources.Count({1}). So we're doing nothing.", new object[]
				{
					publicNetworks.Count<AmClusterNetwork>(),
					networksWithResources.Count<string>()
				});
				return new List<AmClusterResource>(0);
			}
			List<AmClusterResource> list = new List<AmClusterResource>(publicNetworks.Count<AmClusterNetwork>() - networksWithResources.Count<string>());
			IEnumerable<AmClusterNetwork> enumerable = from publicNet in publicNetworks
			where !networksWithResources.Contains(publicNet.Name)
			select publicNet;
			foreach (AmClusterNetwork amClusterNetwork in enumerable)
			{
				logger.AppendLogMessage("AddIpAddressesToStrandedNetworks() There doesn't appear to be an IP resource on network '{0}'.", new object[]
				{
					amClusterNetwork.Name
				});
				IEnumerable<string> source = amClusterNetwork.EnumerateAlternateIPv4Names();
				IEnumerable<string> source2 = amClusterNetwork.EnumeratePureAlternateIPv6Names();
				List<AmClusterResource> collection = AmClusterResourceHelper.CreateIpAddressResources(logger, group, netname, null, source.ToArray<string>(), source2.ToArray<string>());
				list.AddRange(collection);
			}
			return list;
		}

		private static IEnumerable<AmClusterResource> AddStaticIpAddressesToStrandedNetworks(ITaskOutputHelper logger, AmClusterGroup group, AmClusterResource netname, IEnumerable<AmClusterNetwork> publicNetworks, IEnumerable<string> networksWithResources, IEnumerable<IPAddress> staticIpAddresses)
		{
			logger = (logger ?? NullTaskOutputHelper.GetNullLogger());
			if (publicNetworks.Count<AmClusterNetwork>() <= networksWithResources.Count<string>())
			{
				logger.AppendLogMessage("AddStaticIpAddressesToStrandedNetworks: publicNetworks.Count({0}) <= networksWithResources.Count({1}). So we're doing nothing.", new object[]
				{
					publicNetworks.Count<AmClusterNetwork>(),
					networksWithResources.Count<string>()
				});
				return new List<AmClusterResource>(0);
			}
			List<AmClusterResource> list = new List<AmClusterResource>(publicNetworks.Count<AmClusterNetwork>() - networksWithResources.Count<string>());
			IEnumerable<AmClusterNetwork> enumerable = from publicNet in publicNetworks
			where !networksWithResources.Contains(publicNet.Name)
			select publicNet;
			bool flag = false;
			using (IEnumerator<AmClusterNetwork> enumerator = enumerable.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AmClusterNetwork publicNetwork = enumerator.Current;
					logger.AppendLogMessage("AddStaticIpAddressesToStrandedNetworks() There doesn't appear to be an IP resource on network '{0}'.", new object[]
					{
						publicNetwork.Name
					});
					IEnumerable<IPAddress> enumerable2 = from staticIp in staticIpAddresses
					where publicNetwork.IsIPInNetwork(staticIp)
					select staticIp;
					if (enumerable2.Count<IPAddress>() > 0)
					{
						flag = true;
					}
					foreach (IPAddress ipaddress in enumerable2)
					{
						logger.AppendLogMessage("AddStaticIpAddressesToStrandedNetworks() Adding IP {0} to network '{1}'.", new object[]
						{
							ipaddress,
							publicNetwork.Name
						});
						int num = 1;
						bool flag2 = false;
						AmClusterResource amClusterResource = null;
						try
						{
							amClusterResource = (AmClusterResource)group.CreateUniqueResource(logger, "IPv4 Static Address", "IP Address", ref num);
							logger.AppendLogMessage("Created new ipv4 static resource. (resource:{0}, address:{1})", new object[]
							{
								amClusterResource.Name,
								ipaddress.ToString()
							});
							int num2;
							using (AmClusterPropListDisposable amClusterPropListDisposable = AmClusPropListMaker.CreatePropListString("Address", ipaddress.ToString(), out num2))
							{
								using (AmClusterPropListDisposable amClusterPropListDisposable2 = AmClusPropListMaker.DupeAndAppendPropListString(amClusterPropListDisposable.RawBuffer, (int)amClusterPropListDisposable.BufferSize, "Network", publicNetwork.Name, out num2))
								{
									using (AmClusterPropListDisposable amClusterPropListDisposable3 = AmClusPropListMaker.DupeAndAppendPropListString(amClusterPropListDisposable2.RawBuffer, (int)amClusterPropListDisposable2.BufferSize, "SubnetMask", publicNetwork.GetAddressMask(), out num2))
									{
										logger.AppendLogMessage("Created new ipv4 resource: name '{0}' to be {1} on network named '{2}' with mask={3}.", new object[]
										{
											amClusterResource.Name,
											ipaddress.ToString(),
											publicNetwork.Name,
											publicNetwork.GetAddressMask()
										});
										amClusterResource.SetPrivatePropertyList(amClusterPropListDisposable3);
									}
								}
							}
							list.Add(amClusterResource);
							flag2 = true;
						}
						finally
						{
							if (!flag2 && amClusterResource != null)
							{
								logger.AppendLogMessage("There was some error creating and configuring the IP address resource {0}. Making a best-effort attempt to delete it.", new object[]
								{
									ipaddress
								});
								try
								{
									amClusterResource.OfflineResource();
									amClusterResource.DeleteResource();
								}
								catch (ClusterApiException ex)
								{
									logger.AppendLogMessage("There was an error deleting the incomplete IP address. Ignoring the error and continuing. The error was {0}", new object[]
									{
										ex
									});
								}
								if (amClusterResource != null)
								{
									amClusterResource.Dispose();
									amClusterResource = null;
								}
							}
						}
					}
				}
			}
			if (!flag)
			{
				string[] value = (from network in enumerable
				select network.Name).ToArray<string>();
				string[] value2 = (from staticIp in staticIpAddresses
				select staticIp.ToString()).ToArray<string>();
				logger.WriteWarning(Strings.DagTaskNotEnoughStaticIPAddresses(string.Join(",", value), string.Join(",", value2)));
			}
			return list;
		}

		private static List<AmClusterResource> CreateIpAddressResources(ILogTraceHelper logger, AmClusterGroup group, AmClusterResource netname, IPAddress ipv4StaticAddress, string[] ipv4Networks, string[] ipv6Networks)
		{
			logger = (logger ?? NullLogTraceHelper.GetNullLogger());
			logger.AppendLogMessage("CreateIpAddressResources(). IPv4Networks=[{0}]. IPv6Networks=[{1}].", new object[]
			{
				(ipv4Networks == null) ? string.Empty : string.Join(",", ipv4Networks),
				(ipv6Networks == null) ? string.Empty : string.Join(",", ipv6Networks)
			});
			List<AmClusterResource> list = new List<AmClusterResource>(2);
			if (ipv4Networks != null)
			{
				list.AddRange(AmClusterResourceHelper.ConfigureIPv4DhcpResources(logger, group, ipv4Networks));
			}
			if (ipv6Networks != null)
			{
				list.AddRange(AmClusterResourceHelper.ConfigureIPv6AutoCfgResources(logger, group, ipv6Networks));
			}
			return list;
		}

		private const string IPv4StaticResourcePrefix = "IPv4 Static Address";

		private const string IPv4DhcpResourcePrefix = "IPv4 DHCP Address";

		private const string IPv6AutoCfgResourcePrefix = "IPv6 Auto Config Address";
	}
}
