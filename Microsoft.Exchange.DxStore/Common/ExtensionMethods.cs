using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;

namespace Microsoft.Exchange.DxStore.Common
{
	public static class ExtensionMethods
	{
		public static InstanceGroupMemberConfig GetMemberConfig(this InstanceGroupConfig cfg, string memberName)
		{
			InstanceGroupMemberConfig result = null;
			if (memberName == null)
			{
				memberName = cfg.Self;
			}
			if (cfg.Members != null)
			{
				result = cfg.Members.FirstOrDefault((InstanceGroupMemberConfig mc) => Utils.IsEqual(mc.Name, memberName, StringComparison.OrdinalIgnoreCase));
			}
			return result;
		}

		public static string GetMemberNetworkAddress(this InstanceGroupConfig cfg, string target)
		{
			InstanceGroupMemberConfig memberConfig = cfg.GetMemberConfig(target);
			return EndpointBuilder.GetNetworkAddress(cfg.Self, target, (memberConfig != null) ? memberConfig.NetworkAddress : null, cfg.NameResolver, false);
		}

		public static string GetGroupEndPointAddress(this InstanceGroupConfig cfg, string interfaceName, string target, int portNumber, string protocolName, bool isUseDefaultGroupIdentifier)
		{
			return EndpointBuilder.ConstructEndpointAddress(interfaceName, cfg.ComponentName, cfg.Self, target, cfg.GetMemberNetworkAddress(target), isUseDefaultGroupIdentifier ? "B1563499-EA40-4101-A9E6-59A8EB26FF1E" : cfg.Name, cfg.IsZeroboxMode, portNumber, protocolName);
		}

		public static string GetEndPointAddress(this InstanceManagerConfig cfg, string target)
		{
			string networkAddress = EndpointBuilder.GetNetworkAddress(cfg.Self, target, cfg.NetworkAddress, cfg.NameResolver, false);
			return EndpointBuilder.ConstructEndpointAddress("Manager", cfg.ComponentName, cfg.Self, target, networkAddress, null, cfg.IsZeroboxMode, cfg.EndpointPortNumber, cfg.EndpointProtocolName);
		}

		public static string GetStoreAccessEndPointAddress(this InstanceGroupConfig cfg, string target, bool isUseDefaultGroupIdentifier)
		{
			return cfg.GetGroupEndPointAddress("Access", target, cfg.Settings.AccessEndpointPortNumber, cfg.Settings.AccessEndpointProtocolName, isUseDefaultGroupIdentifier);
		}

		public static string GetStoreInstanceEndPointAddress(this InstanceGroupConfig cfg, string target, bool isUseDefaultGroupIdentifier)
		{
			return cfg.GetGroupEndPointAddress("Instance", target, cfg.Settings.InstanceEndpointPortNumber, cfg.Settings.InstanceEndpointProtocolName, isUseDefaultGroupIdentifier);
		}

		public static string ConstructUniqueAccessBindingName(this InstanceGroupConfig cfg, string target, bool isUseDefaultGroup)
		{
			target = ExtensionMethods.ResolveTarget(cfg.Self, target);
			return EndpointBuilder.ConstructUniqueBindingName(target, cfg.ComponentName, cfg.Settings.AccessEndpointProtocolName, "Access", isUseDefaultGroup ? "B1563499-EA40-4101-A9E6-59A8EB26FF1E" : cfg.Name, cfg.IsZeroboxMode);
		}

		public static string ConstructUniqueInstanceBindingName(this InstanceGroupConfig cfg, string target, bool isUseDefaultGroup)
		{
			target = ExtensionMethods.ResolveTarget(cfg.Self, target);
			return EndpointBuilder.ConstructUniqueBindingName(target, cfg.ComponentName, cfg.Settings.InstanceEndpointProtocolName, "Instance", isUseDefaultGroup ? "B1563499-EA40-4101-A9E6-59A8EB26FF1E" : cfg.Name, cfg.IsZeroboxMode);
		}

		public static string ConstructUniqueBindingName(this InstanceManagerConfig cfg, string target)
		{
			target = ExtensionMethods.ResolveTarget(cfg.Self, target);
			return EndpointBuilder.ConstructUniqueBindingName(target, cfg.ComponentName, cfg.EndpointProtocolName, "Manager", null, cfg.IsZeroboxMode);
		}

		public static bool IsMembersChanged(this InstanceGroupConfig groupCfg, InstanceGroupMemberConfig[] members)
		{
			int num = (members != null) ? members.Length : 0;
			int num2 = (groupCfg != null) ? groupCfg.Members.Length : 0;
			if (num != num2)
			{
				return true;
			}
			if (groupCfg == null || members == null)
			{
				return false;
			}
			int num3 = members.Count((InstanceGroupMemberConfig member) => groupCfg.Members.Any((InstanceGroupMemberConfig gm) => Utils.IsEqual(gm.Name, member.Name, StringComparison.OrdinalIgnoreCase)));
			return num3 != num;
		}

		public static void Log(this IDxStoreEventLogger logger, string periodicKey, TimeSpan? periodicDuration, DxEventSeverity severity, int id, string formatString, params object[] args)
		{
			if (periodicKey != null)
			{
				logger.LogPeriodic(periodicKey, periodicDuration.Value, severity, id, formatString, args);
				return;
			}
			logger.Log(severity, id, formatString, args);
		}

		public static Dictionary<string, ServiceEndpoint> GetAllMemberAccessClientEndPoints(this InstanceGroupConfig cfg, bool isUseDefaultGroup = false, WcfTimeout timeout = null)
		{
			Dictionary<string, ServiceEndpoint> dictionary = new Dictionary<string, ServiceEndpoint>();
			foreach (InstanceGroupMemberConfig instanceGroupMemberConfig in cfg.Members)
			{
				ServiceEndpoint storeAccessEndpoint = cfg.GetStoreAccessEndpoint(instanceGroupMemberConfig.Name, false, false, timeout ?? cfg.Settings.StoreAccessWcfTimeout);
				dictionary[instanceGroupMemberConfig.Name] = storeAccessEndpoint;
			}
			return dictionary;
		}

		public static string ResolveNameBestEffort(this IServerNameResolver resolver, string shortName)
		{
			string result = null;
			try
			{
				if (resolver != null)
				{
					result = resolver.ResolveName(shortName);
				}
			}
			catch
			{
			}
			return result;
		}

		public static void AppendSpaces(this StringBuilder sb, int count)
		{
			if (count > 0)
			{
				sb.Append(new string(' ', count));
			}
		}

		public static string JoinWithComma(this IEnumerable<string> strArray, string defaultValue = "<null>")
		{
			if (strArray != null)
			{
				return string.Join(",", strArray);
			}
			return defaultValue;
		}

		public static string ToShortString(this DateTimeOffset dt)
		{
			return dt.ToString("yy/MM/dd hh:mm:ss.fff");
		}

		private static string ResolveTarget(string self, string target)
		{
			if (string.IsNullOrEmpty(self))
			{
				self = Environment.MachineName;
			}
			if (!string.IsNullOrEmpty(target))
			{
				return target;
			}
			return self;
		}
	}
}
