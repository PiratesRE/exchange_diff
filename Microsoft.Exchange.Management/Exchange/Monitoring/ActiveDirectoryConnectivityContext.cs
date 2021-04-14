using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	internal sealed class ActiveDirectoryConnectivityContext
	{
		internal static ActiveDirectoryConnectivityContext CreateForActiveDirectoryConnectivity(TestActiveDirectoryConnectivityTask instance, MonitoringData monitoringData, string domainController)
		{
			return new ActiveDirectoryConnectivityContext(instance, monitoringData, domainController);
		}

		private ActiveDirectoryConnectivityContext(TestActiveDirectoryConnectivityTask instance, MonitoringData monitoringData, string domainController)
		{
			this.instance = instance;
			this.monitoringData = monitoringData;
			this.domainController = domainController;
			this.logger = new TaskLoggerAdaptor(this.Instance);
			if (this.MonitoringData != null)
			{
				this.logger.Append(new StringLogger());
			}
		}

		internal MonitoringData MonitoringData
		{
			get
			{
				return this.monitoringData;
			}
		}

		internal string CurrentDomainController
		{
			get
			{
				if (string.IsNullOrEmpty(this.domainController))
				{
					return "Auto-Selected by ADDriver";
				}
				return this.domainController;
			}
			set
			{
				this.domainController = value;
			}
		}

		internal DirectoryEntry CurrentDCDirectoryEntry
		{
			get
			{
				if (this.currentDCDirectoryEntry == null)
				{
					this.currentDCDirectoryEntry = ActiveDirectoryConnectivityContext.GetDirectoryEntry(this.CurrentDomainController);
				}
				return this.currentDCDirectoryEntry;
			}
		}

		internal bool UseADDriver
		{
			get
			{
				return this.Instance.UseADDriver;
			}
		}

		internal TestActiveDirectoryConnectivityTask Instance
		{
			get
			{
				return this.instance;
			}
		}

		internal IRecipientSession RecipientSession
		{
			get
			{
				if (this.recipientSession == null)
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), OrganizationId.ForestWideOrgId, null, false);
					this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.domainController, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, ConfigScopes.TenantSubTree, 166, "RecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\ActiveDirectory\\ActiveDirectoryConnectivityContext.cs");
				}
				return this.recipientSession;
			}
		}

		internal IConfigurationSession SystemConfigurationSession
		{
			get
			{
				if (this.systemConfigurationSession == null)
				{
					this.systemConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.CurrentDomainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 188, "SystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\ActiveDirectory\\ActiveDirectoryConnectivityContext.cs");
				}
				return this.systemConfigurationSession;
			}
		}

		internal ITopologyConfigurationSession SafeSystemConfigurationSession
		{
			get
			{
				if (this.safeSystemConfigurationSession == null)
				{
					this.safeSystemConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 207, "SafeSystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\ActiveDirectory\\ActiveDirectoryConnectivityContext.cs");
				}
				return this.safeSystemConfigurationSession;
			}
		}

		internal void WriteVerbose(LocalizedString message)
		{
			this.logger.WriteVerbose(message);
		}

		internal void WriteWarning(LocalizedString message)
		{
			this.logger.WriteWarning(message);
		}

		internal void WriteDebug(LocalizedString message)
		{
			this.logger.WriteDebug(message);
		}

		internal string GetDiagnosticInfo(string prefix)
		{
			return this.logger.GetDiagnosticInfo(prefix);
		}

		internal static DirectoryEntry GetDirectoryEntry(string directoryServer)
		{
			string str = "LDAP://";
			str += ((!string.IsNullOrEmpty(directoryServer)) ? (directoryServer + "/") : string.Empty);
			string path = string.Empty;
			using (DirectoryEntry directoryEntry = new DirectoryEntry(str + "rootDSE"))
			{
				if (directoryEntry == null)
				{
					return null;
				}
				string str2 = directoryEntry.Properties["defaultNamingContext"].Value.ToString();
				path = str + str2;
			}
			return new DirectoryEntry(path);
		}

		internal static T EnsureSingleObject<T>(Func<IEnumerable<T>> getObjects) where T : class
		{
			T t = default(T);
			foreach (T t2 in getObjects())
			{
				if (t != null)
				{
					throw new DataValidationException(new ObjectValidationError(Strings.MoreThanOneObjects(typeof(T).ToString()), null, null));
				}
				t = t2;
			}
			return t;
		}

		private readonly TestActiveDirectoryConnectivityTask instance;

		private readonly MonitoringData monitoringData;

		private readonly ChainedLogger logger;

		private IRecipientSession recipientSession;

		private IConfigurationSession systemConfigurationSession;

		private ITopologyConfigurationSession safeSystemConfigurationSession;

		private string domainController;

		private DirectoryEntry currentDCDirectoryEntry;
	}
}
