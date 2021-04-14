using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal sealed class OwaSegmentationSettings
	{
		public static OwaSegmentationSettings GetInstance(IConfigurationSession configSession, ADObjectId owaMailboxPolicyId, OrganizationId orgId)
		{
			return new OwaSegmentationSettings(configSession, owaMailboxPolicyId, orgId);
		}

		public bool this[ADPropertyDefinition owaFeature]
		{
			get
			{
				return (bool)this.owaSegmentationSettings[owaFeature];
			}
		}

		private ITopologyConfigurationSession TopologyConfigSession
		{
			get
			{
				return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 64, "TopologyConfigSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\OwaSegmentationSettings.cs");
			}
		}

		private OwaSegmentationSettings(IConfigurationSession configSession, ADObjectId owaMailboxPolicyId, OrganizationId orgId)
		{
			OwaMailboxPolicy owaMailboxPolicy;
			if ((owaMailboxPolicy = this.GetSettingsFromOwaMailboxPolicy(configSession, owaMailboxPolicyId)) == null && (owaMailboxPolicy = this.GetSettingsFromTenantConfig(orgId)) == null)
			{
				owaMailboxPolicy = (this.GetSettingsFromOwaVirtualDirectory(this.TopologyConfigSession) ?? this.GetDefaultSettings());
			}
			this.owaSegmentationSettings = owaMailboxPolicy;
			this.owaSegmentationSettings.ValidateRead();
		}

		private OwaMailboxPolicy GetSettingsFromOwaMailboxPolicy(IConfigurationSession configSession, ADObjectId owaMailboxPolicyId)
		{
			if (owaMailboxPolicyId == null)
			{
				ExTraceGlobals.OwaSegmentationTracer.TraceDebug((long)this.GetHashCode(), "No OwaMailboxPolicy applied for the user, doing fallback on to TenantConfig settings.");
				return null;
			}
			ExTraceGlobals.OwaSegmentationTracer.TraceDebug<string>((long)this.GetHashCode(), "Reading OwaMailboxPolicy: DN={0} from AD.", owaMailboxPolicyId.DistinguishedName);
			OwaMailboxPolicy owaMailboxPolicy = configSession.Read<OwaMailboxPolicy>(owaMailboxPolicyId);
			if (owaMailboxPolicy != null)
			{
				return owaMailboxPolicy;
			}
			ExTraceGlobals.OwaSegmentationTracer.TraceWarning<string>((long)this.GetHashCode(), "Error reading OwaMailboxPolicy: DN={0}; User settings may be incorrect.", owaMailboxPolicyId.DistinguishedName);
			return null;
		}

		private OwaMailboxPolicy GetSettingsFromTenantConfig(OrganizationId orgId)
		{
			OwaMailboxPolicy defaultOwaMailboxPolicy = OwaSegmentationSettings.GetDefaultOwaMailboxPolicy(orgId);
			if (defaultOwaMailboxPolicy == null)
			{
				ExTraceGlobals.OwaSegmentationTracer.TraceDebug((long)this.GetHashCode(), "No OwaMailboxPolicy found in tenant config, doing fallback on to OWA vDir.");
			}
			return defaultOwaMailboxPolicy;
		}

		public static OwaMailboxPolicy GetDefaultOwaMailboxPolicy(OrganizationId orgId)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 161, "GetDefaultOwaMailboxPolicy", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\OwaSegmentationSettings.cs");
			OwaMailboxPolicy[] array = tenantOrTopologyConfigurationSession.Find<OwaMailboxPolicy>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, OwaMailboxPolicySchema.IsDefault, true), new SortBy(ADObjectSchema.WhenChanged, SortOrder.Descending), 1);
			if (array != null && array.Length > 0)
			{
				return array[0];
			}
			return null;
		}

		public static bool UpdateOwaMailboxPolicy(OrganizationId orgId, ADObjectId owaPolicyId, out ADObjectId newOwaPolicy)
		{
			newOwaPolicy = null;
			if (owaPolicyId == null)
			{
				OwaMailboxPolicy defaultOwaMailboxPolicy = OwaSegmentationSettings.GetDefaultOwaMailboxPolicy(orgId);
				if (defaultOwaMailboxPolicy != null)
				{
					newOwaPolicy = defaultOwaMailboxPolicy.Id;
					return true;
				}
			}
			return false;
		}

		private ADOwaVirtualDirectory GetSettingsFromOwaVirtualDirectory(ITopologyConfigurationSession configSession)
		{
			ExTraceGlobals.OwaSegmentationTracer.TraceDebug((long)this.GetHashCode(), "Fetching OwaVirtualDirectory objects on local server from AD.");
			ADOwaVirtualDirectory[] array = configSession.FindOWAVirtualDirectoriesForLocalServer();
			if (array.Length > 0)
			{
				foreach (ADOwaVirtualDirectory adowaVirtualDirectory in array)
				{
					if (adowaVirtualDirectory.MetabasePath.EndsWith("/owa", StringComparison.OrdinalIgnoreCase))
					{
						ExTraceGlobals.OwaSegmentationTracer.TraceDebug<string>((long)this.GetHashCode(), "Using OWA segmentation settings from OwaVirtualDirectory: DN={0}.", adowaVirtualDirectory.DistinguishedName);
						return adowaVirtualDirectory;
					}
				}
			}
			ExTraceGlobals.OwaSegmentationTracer.TraceDebug((long)this.GetHashCode(), "No OwaVirtualDirectory found on local server.");
			return null;
		}

		private OwaMailboxPolicy GetDefaultSettings()
		{
			ExTraceGlobals.OwaSegmentationTracer.TraceError((long)this.GetHashCode(), "Unable to fetch OWA segmentation settings from OwaMailboxPolicy or OwaVirtualDirectory.Using default settings from a new OwaMailboxPolicy object");
			return new OwaMailboxPolicy();
		}

		[Conditional("DEBUG")]
		private void ThrowIfFeatureTypeNotBoolean(ADPropertyDefinition owaFeature)
		{
			if (owaFeature.Type != typeof(bool))
			{
				throw new ArgumentException("owaFeature: OWA Segmentation features must be of type System.Boolean. Please fix it.", "owaFeature");
			}
		}

		private readonly ADObject owaSegmentationSettings;
	}
}
