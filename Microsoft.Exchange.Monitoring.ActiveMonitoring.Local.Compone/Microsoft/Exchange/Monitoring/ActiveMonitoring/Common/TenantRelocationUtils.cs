using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class TenantRelocationUtils
	{
		public static IList<UserExperienceVerificationRequest> GetRelocatingTenantsFromEndpoint()
		{
			IList<UserExperienceVerificationRequest> list = new List<UserExperienceVerificationRequest>();
			DirectoryInfo directoryInfo = new DirectoryInfo(TenantRelocationUtils.MonitoringRequestPath);
			if (directoryInfo.Exists)
			{
				foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.request"))
				{
					string name = fileInfo.Name;
					string text = name.Substring(0, name.Length - ".request".Length);
					string[] array = text.Split(new char[]
					{
						'_'
					});
					if (array.Length >= 3)
					{
						string tenantName = array[0];
						string stage = array[1];
						List<string> list2 = new List<string>();
						for (int j = 2; j < array.Length; j++)
						{
							list2.Add(array[j]);
						}
						list.Add(new UserExperienceVerificationRequest(tenantName, stage, list2));
					}
				}
			}
			return list;
		}

		public static void GetUserExperienceMonitoringAccount(string tenantName, out ADUser monitoringAccount, out string password)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromTenantAcceptedDomain(tenantName);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, false, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 87, "GetUserExperienceMonitoringAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\TenantRelocationUtils.cs");
			IConfigurationSession session = DirectorySessionFactory.Default.CreateTenantConfigurationSession(null, false, ConsistencyMode.FullyConsistent, sessionSettings, 95, "GetUserExperienceMonitoringAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\TenantRelocationUtils.cs");
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "TenantRelocationUserExperienceMonitoringUser");
			ADUser[] array = tenantOrRootOrgRecipientSession.Find<ADUser>(null, QueryScope.SubTree, filter, null, 1);
			if (array != null && array.Length > 0)
			{
				monitoringAccount = array[0];
				password = DirectoryAccessor.Instance.GeneratePasswordForMailbox(monitoringAccount);
				ILiveIdHelper liveIdHelper = TenantRelocationUtils.GetLiveIdHelper();
				liveIdHelper.ResetPassword(monitoringAccount.WindowsLiveID, monitoringAccount.NetID, session, password);
				return;
			}
			monitoringAccount = null;
			password = null;
		}

		private static ILiveIdHelper GetLiveIdHelper()
		{
			string text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			text = Path.Combine(text, "Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Datacenter.Components.dll");
			Assembly assembly = Assembly.LoadFile(text);
			Type type = assembly.GetType("Microsoft.Exchange.Monitoring.ActiveMonitoring.Common.Datacenter.LiveIdHelper");
			return (ILiveIdHelper)Activator.CreateInstance(type, new object[0]);
		}

		private const string DatacenterComponentAssemblyName = "Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Datacenter.Components.dll";

		private const string LiveIdHelperTypeName = "Microsoft.Exchange.Monitoring.ActiveMonitoring.Common.Datacenter.LiveIdHelper";

		public static readonly string MonitoringRequestPath = Path.Combine(ConfigurationContext.Setup.InstallPath, UserExperienceMonitoringContants.MonitoringRequestPath);

		public static readonly string MonitoringResponsePath = Path.Combine(ConfigurationContext.Setup.InstallPath, UserExperienceMonitoringContants.MonitoringResponsePath);
	}
}
