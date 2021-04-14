using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.OrganizationConfiguration;
using Microsoft.Exchange.MessagingPolicies.Rules.Classification;
using Microsoft.Exchange.MessagingPolicies.Rules.OutlookProtection;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal class CachedOrganizationConfiguration
	{
		public OrganizationDomains Domains
		{
			get
			{
				return this.domains;
			}
		}

		public PerTenantSmimeSettings SmimeSettings
		{
			get
			{
				return this.smimeSettings;
			}
		}

		public PerTenantTransportSettings TransportSettings
		{
			get
			{
				return this.transportSettings;
			}
		}

		public PerTenantOrganizationConfiguration OrganizationConfiguration
		{
			get
			{
				return this.organizationConfiguration;
			}
		}

		public IEnumerable<ClassificationRulePackage> ClassificationDefinitions
		{
			get
			{
				if (this.classificationDefinitions == null)
				{
					return CachedOrganizationConfiguration.EmptyClassificationDefinitionsArray;
				}
				return this.classificationDefinitions.ClassificationDefinitions;
			}
		}

		public PerTenantPolicyNudgeRulesCollection.PolicyNudgeRules PolicyNudgeRules
		{
			get
			{
				if (this.policyNudgeRules == null)
				{
					return PerTenantPolicyNudgeRulesCollection.PolicyNudgeRules.Empty;
				}
				return this.policyNudgeRules.Rules;
			}
		}

		public IEnumerable<OutlookProtectionRule> ProtectionRules
		{
			get
			{
				if (this.protectionRules == null)
				{
					return CachedOrganizationConfiguration.EmptyOutlookProtectionRulesArray;
				}
				return this.protectionRules.ProtectionRules;
			}
		}

		public GroupsConfiguration GroupsConfiguration
		{
			get
			{
				return this.groupsConfiguration;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		public static CachedOrganizationConfiguration GetInstance(OrganizationId organizationId, CachedOrganizationConfiguration.ConfigurationTypes configurationTypes = CachedOrganizationConfiguration.ConfigurationTypes.All)
		{
			return CachedOrganizationConfiguration.GetInstance(organizationId, TimeSpan.Zero, configurationTypes);
		}

		public static CachedOrganizationConfiguration GetInstance(OrganizationId organizationId, TimeSpan timeoutInterval, CachedOrganizationConfiguration.ConfigurationTypes configurationTypes = CachedOrganizationConfiguration.ConfigurationTypes.All)
		{
			CachedOrganizationConfiguration cachedOrganizationConfiguration;
			if (organizationId == OrganizationId.ForestWideOrgId && !VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				if (CachedOrganizationConfiguration.corporateInstance == null)
				{
					lock (CachedOrganizationConfiguration.corporateInstanceLock)
					{
						if (CachedOrganizationConfiguration.corporateInstance == null)
						{
							cachedOrganizationConfiguration = new CachedOrganizationConfiguration(organizationId, timeoutInterval, CachedOrganizationConfiguration.ConfigurationTypes.All);
							cachedOrganizationConfiguration.Initialize();
							CachedOrganizationConfiguration.corporateInstance = cachedOrganizationConfiguration;
						}
					}
				}
				cachedOrganizationConfiguration = CachedOrganizationConfiguration.corporateInstance;
			}
			else
			{
				cachedOrganizationConfiguration = new CachedOrganizationConfiguration(organizationId, timeoutInterval, configurationTypes);
			}
			cachedOrganizationConfiguration.Initialize();
			return cachedOrganizationConfiguration;
		}

		public override string ToString()
		{
			return this.organizationId.ToString();
		}

		internal CachedOrganizationConfiguration(OrganizationId organizationId, TimeSpan timeoutInterval, CachedOrganizationConfiguration.ConfigurationTypes configurationTypes = CachedOrganizationConfiguration.ConfigurationTypes.All)
		{
			this.organizationId = organizationId;
			if ((configurationTypes & CachedOrganizationConfiguration.ConfigurationTypes.Domains) == CachedOrganizationConfiguration.ConfigurationTypes.Domains)
			{
				this.domains = new OrganizationDomains(organizationId);
			}
			if ((configurationTypes & CachedOrganizationConfiguration.ConfigurationTypes.SmimeSettings) == CachedOrganizationConfiguration.ConfigurationTypes.SmimeSettings)
			{
				this.smimeSettings = new PerTenantSmimeSettings(organizationId, timeoutInterval);
			}
			if ((configurationTypes & CachedOrganizationConfiguration.ConfigurationTypes.TransportSettings) == CachedOrganizationConfiguration.ConfigurationTypes.TransportSettings)
			{
				this.transportSettings = new PerTenantTransportSettings(organizationId, timeoutInterval);
			}
			if ((configurationTypes & CachedOrganizationConfiguration.ConfigurationTypes.OrganizationConfiguration) == CachedOrganizationConfiguration.ConfigurationTypes.OrganizationConfiguration)
			{
				this.organizationConfiguration = new PerTenantOrganizationConfiguration(organizationId);
			}
			if ((configurationTypes & CachedOrganizationConfiguration.ConfigurationTypes.ClassificationDefinitions) == CachedOrganizationConfiguration.ConfigurationTypes.ClassificationDefinitions)
			{
				this.classificationDefinitions = new PerTenantClassificationDefinitionCollection(organizationId);
			}
			if ((configurationTypes & CachedOrganizationConfiguration.ConfigurationTypes.ProtectionRules) == CachedOrganizationConfiguration.ConfigurationTypes.ProtectionRules)
			{
				this.protectionRules = new PerTenantProtectionRulesCollection(organizationId);
			}
			if ((configurationTypes & CachedOrganizationConfiguration.ConfigurationTypes.PolicyNudgeRules) == CachedOrganizationConfiguration.ConfigurationTypes.PolicyNudgeRules)
			{
				this.policyNudgeRules = new PerTenantPolicyNudgeRulesCollection(organizationId);
			}
			if ((configurationTypes & CachedOrganizationConfiguration.ConfigurationTypes.GroupsConfiguration) == CachedOrganizationConfiguration.ConfigurationTypes.GroupsConfiguration)
			{
				this.groupsConfiguration = new GroupsConfiguration(organizationId);
			}
		}

		protected void Initialize()
		{
			if (this.domains != null)
			{
				this.domains.Initialize();
			}
			if (this.smimeSettings != null)
			{
				this.smimeSettings.Initialize();
			}
			if (this.transportSettings != null)
			{
				this.transportSettings.Initialize();
			}
			if (this.organizationConfiguration != null)
			{
				this.organizationConfiguration.Initialize();
			}
			if (this.classificationDefinitions != null)
			{
				this.classificationDefinitions.Initialize();
			}
			if (this.protectionRules != null)
			{
				this.protectionRules.Initialize();
			}
			if (this.policyNudgeRules != null)
			{
				this.policyNudgeRules.Initialize();
			}
			if (this.groupsConfiguration != null)
			{
				this.groupsConfiguration.Initialize();
			}
		}

		private const string EventSource = "MSExchange OrganizationConfiguration";

		internal static readonly Trace Tracer = ExTraceGlobals.OrganizationConfigurationTracer;

		internal static readonly ExEventLog Logger = new ExEventLog(CachedOrganizationConfiguration.Tracer.Category, "MSExchange OrganizationConfiguration");

		private static readonly ClassificationRulePackage[] EmptyClassificationDefinitionsArray = new ClassificationRulePackage[0];

		private static readonly OutlookProtectionRule[] EmptyOutlookProtectionRulesArray = new OutlookProtectionRule[0];

		private static CachedOrganizationConfiguration corporateInstance;

		private static object corporateInstanceLock = new object();

		private OrganizationDomains domains;

		private PerTenantSmimeSettings smimeSettings;

		private PerTenantTransportSettings transportSettings;

		private PerTenantOrganizationConfiguration organizationConfiguration;

		private PerTenantClassificationDefinitionCollection classificationDefinitions;

		private PerTenantPolicyNudgeRulesCollection policyNudgeRules;

		private PerTenantProtectionRulesCollection protectionRules;

		private GroupsConfiguration groupsConfiguration;

		private OrganizationId organizationId;

		[Flags]
		internal enum ConfigurationTypes
		{
			Domains = 1,
			TransportSettings = 2,
			OrganizationConfiguration = 4,
			ClassificationDefinitions = 8,
			PolicyNudgeRules = 16,
			ProtectionRules = 32,
			GroupsConfiguration = 64,
			SmimeSettings = 128,
			All = 255
		}
	}
}
