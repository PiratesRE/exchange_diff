using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class TenantRelocationRequestSchema : ADLegacyVersionableObjectSchema
	{
		internal static IList<ADPropertyDefinition> RelocationSpecificProperties
		{
			get
			{
				if (TenantRelocationRequestSchema.relocationSpecificProperties == null)
				{
					TenantRelocationRequestSchema.relocationSpecificProperties = new List<ADPropertyDefinition>
					{
						TenantRelocationRequestSchema.RelocationSyncStartTime,
						TenantRelocationRequestSchema.LockdownStartTime,
						TenantRelocationRequestSchema.RetiredStartTime,
						TenantRelocationRequestSchema.TransitionCounterRaw,
						TenantRelocationRequestSchema.SafeLockdownSchedule,
						TenantRelocationRequestSchema.RelocationSourceForestRaw,
						TenantRelocationRequestSchema.TargetForest,
						TenantRelocationRequestSchema.TenantRelocationFlags,
						TenantRelocationRequestSchema.RelocationStateRequested,
						TenantRelocationRequestSchema.RelocationStatusDetailsRaw,
						TenantRelocationRequestSchema.TenantSyncCookie,
						TenantRelocationRequestSchema.TenantRelocationCompletionTargetVector
					};
				}
				return TenantRelocationRequestSchema.relocationSpecificProperties;
			}
		}

		private static IList<ADPropertyDefinition> relocationSpecificProperties;

		public static readonly ADPropertyDefinition RelocationSyncStartTime = new ADPropertyDefinition("RelocationSyncStartTime", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), "msExchRelocateTenantStartSync", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LockdownStartTime = new ADPropertyDefinition("LockdownStartTime", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), "msExchRelocateTenantStartLockdown", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RetiredStartTime = new ADPropertyDefinition("RetiredStartTime", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), "msExchRelocateTenantStartRetired", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GLSResolvedForest = new ADPropertyDefinition("GLSResolvedForest", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SourceForestRIDMaster = new ADPropertyDefinition("SourceForestRIDMaster", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TargetForestRIDMaster = new ADPropertyDefinition("TargetForestRIDMaster", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransitionCounterRaw = new ADPropertyDefinition("TransitionCounterRaw", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchRelocateTenantTransitionCounter", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransitionCounter = new ADPropertyDefinition("TransitionCounter", ExchangeObjectVersion.Exchange2003, typeof(TransitionCount), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantRelocationRequestSchema.TransitionCounterRaw
		}, null, new GetterDelegate(TenantRelocationRequest.GetTransitionCounter), new SetterDelegate(TenantRelocationRequest.SetTransitionCounter), null, null);

		public static readonly ADPropertyDefinition TargetOrganizationId = new ADPropertyDefinition("TargetOrganizationId", ExchangeObjectVersion.Exchange2003, typeof(OrganizationId), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RelocationInProgress = new ADPropertyDefinition("RelocationInProgress", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TenantSyncCookie = new ADPropertyDefinition("TenantSyncCookie", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "msExchSyncCookie", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TargetOriginatingServer = new ADPropertyDefinition("TargetOriginatingServer", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition SafeLockdownSchedule = ExchangeConfigurationUnitSchema.SafeLockdownSchedule;

		public static readonly ADPropertyDefinition RelocationSourceForestRaw = ExchangeConfigurationUnitSchema.RelocationSourceForestRaw;

		public static readonly ADPropertyDefinition TargetForest = ExchangeConfigurationUnitSchema.TargetForest;

		public static readonly ADPropertyDefinition SourceForest = ExchangeConfigurationUnitSchema.SourceForest;

		public static readonly ADPropertyDefinition TenantRelocationFlags = ExchangeConfigurationUnitSchema.TenantRelocationFlags;

		public static readonly ADPropertyDefinition RelocationStateRequested = ExchangeConfigurationUnitSchema.RelocationStateRequested;

		public static readonly ADPropertyDefinition Suspended = ExchangeConfigurationUnitSchema.Suspended;

		public static readonly ADPropertyDefinition RelocationLastError = ExchangeConfigurationUnitSchema.RelocationLastError;

		public static readonly ADPropertyDefinition AutoCompletionEnabled = ExchangeConfigurationUnitSchema.AutoCompletionEnabled;

		public static readonly ADPropertyDefinition LargeTenantModeEnabled = ExchangeConfigurationUnitSchema.LargeTenantModeEnabled;

		public static readonly ADPropertyDefinition RelocationStatusDetailsRaw = ExchangeConfigurationUnitSchema.RelocationStatusDetailsRaw;

		public static readonly ADPropertyDefinition RelocationStatus = ExchangeConfigurationUnitSchema.RelocationStatus;

		public static readonly ADPropertyDefinition RelocationStatusDetailsSource = ExchangeConfigurationUnitSchema.RelocationStatusDetailsSource;

		public static readonly ADPropertyDefinition RelocationStatusDetailsDestination = ExchangeConfigurationUnitSchema.RelocationStatusDetailsDestination;

		public static readonly ADPropertyDefinition LastSuccessfulRelocationSyncStart = ExchangeConfigurationUnitSchema.LastSuccessfulRelocationSyncStart;

		public static readonly ADPropertyDefinition TenantRelocationCompletionTargetVector = ExchangeConfigurationUnitSchema.TenantRelocationCompletionTargetVector;

		public static readonly ADPropertyDefinition ExternalDirectoryOrganizationId = ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId;

		public static readonly ADPropertyDefinition ServicePlan = ExchangeConfigurationUnitSchema.ServicePlan;

		public static readonly ADPropertyDefinition OrganizationStatus = ExchangeConfigurationUnitSchema.OrganizationStatus;

		public static readonly ADPropertyDefinition TargetOrganizationStatus = ExchangeConfigurationUnitSchema.TargetOrganizationStatus;

		public static readonly ADPropertyDefinition AdminDisplayVersion = OrganizationSchema.AdminDisplayVersion;

		public static readonly ADPropertyDefinition ExchangeUpgradeBucket = ExchangeConfigurationUnitSchema.ExchangeUpgradeBucket;

		public static readonly ADPropertyDefinition OrganizationFlags = OrganizationSchema.OrganizationFlags;

		public static readonly ADPropertyDefinition ObjectVersion = OrganizationSchema.ObjectVersion;
	}
}
