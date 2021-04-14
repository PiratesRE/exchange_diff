using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class ADObjectIdResolutionHelper
	{
		private static IDirectorySession GetSessionForPartition(string partitionFQDN)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromAccountPartitionRootOrgScopeSet(new PartitionId(partitionFQDN));
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 28, "GetSessionForPartition", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADObjectIdResolutionHelper.cs");
		}

		internal static ADObjectId ResolveDN(ADObjectId obj)
		{
			if (!ADGlobalConfigSettings.SoftLinkEnabled)
			{
				return obj;
			}
			if (obj == null || !string.IsNullOrEmpty(obj.DistinguishedName) || obj.ObjectGuid == Guid.Empty)
			{
				return obj;
			}
			return ADObjectIdResolutionHelper.ResolveADObject(obj);
		}

		internal static ADObjectId ResolveDNIfNecessary(ADObjectId obj)
		{
			if (ADSessionSettings.IsRunningOnCmdlet() || ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("IsSoftLinkResolutionEnabledForAllProcesses"))
			{
				return ADObjectIdResolutionHelper.ResolveDN(obj);
			}
			return obj;
		}

		internal static ADObjectId ResolveADObjectIdWithoutCache(ADObjectId obj)
		{
			IDirectorySession sessionForPartition = ADObjectIdResolutionHelper.GetSessionForPartition(obj.PartitionFQDN);
			ADRawEntry adrawEntry = sessionForPartition.ReadADRawEntry(obj, ADObjectIdResolutionHelper.propertiesToRetrieve);
			if (adrawEntry != null)
			{
				return adrawEntry.Id;
			}
			return obj;
		}

		internal static ADObjectId ResolveSoftLink(ADObjectId obj)
		{
			if (!ADGlobalConfigSettings.SoftLinkEnabled)
			{
				return obj;
			}
			if (obj == null || string.IsNullOrEmpty(obj.DistinguishedName) || (obj.PartitionGuid != Guid.Empty && obj.ObjectGuid != Guid.Empty))
			{
				return obj;
			}
			return ADObjectIdResolutionHelper.ResolveADObjectIdWithoutCache(obj);
		}

		private static ADObjectId ResolveADObject(ADObjectId obj)
		{
			bool config = ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("IsSoftLinkResolutionCacheEnabled");
			if (config)
			{
				return ADObjectIdResolutionCache.Default.GetEntry(obj);
			}
			return ADObjectIdResolutionHelper.ResolveADObjectIdWithoutCache(obj);
		}

		private static readonly ADPropertyDefinition[] propertiesToRetrieve = new ADPropertyDefinition[]
		{
			ADObjectSchema.Id
		};
	}
}
