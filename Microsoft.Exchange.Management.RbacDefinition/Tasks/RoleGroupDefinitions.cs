using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class RoleGroupDefinitions
	{
		internal static readonly RoleGroupCollection RoleGroups = new RoleGroupCollection
		{
			new RoleGroupDefinition(RoleGroup.OrganizationManagement_InitInfo, CoreStrings.ExchangeOrgAdminDescription, new Guid[]
			{
				WellKnownGuid.EoaWkGuid_E12
			}),
			new RoleGroupDefinition(RoleGroup.RecipientManagement_InitInfo, CoreStrings.ExchangeRecipientAdminDescription, new Guid[]
			{
				WellKnownGuid.EmaWkGuid_E12
			}),
			new RoleGroupDefinition(RoleGroup.ViewOnlyOrganizationManagement_InitInfo, CoreStrings.ExchangeViewOnlyAdminDescription, new Guid[]
			{
				WellKnownGuid.EraWkGuid_E12
			}),
			new RoleGroupDefinition(RoleGroup.PublicFolderManagement_InitInfo, CoreStrings.ExchangePublicFolderAdminDescription, new Guid[]
			{
				WellKnownGuid.EpaWkGuid_E12
			}),
			new RoleGroupDefinition(RoleGroup.UMManagement_InitInfo, CoreStrings.ExchangeUMManagementDescription, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.HelpDesk_InitInfo, CoreStrings.ExchangeHelpDeskDescription, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.RecordsManagement_InitInfo, CoreStrings.ExchangeRecordsManagementDescription, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.DiscoveryManagement_InitInfo, CoreStrings.ExchangeDiscoveryManagementDescription, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ServerManagement_InitInfo, CoreStrings.ExchangeServerManagementDescription, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.DelegatedSetup_InitInfo, CoreStrings.ExchangeDelegatedSetupDescription, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.HygieneManagement_InitInfo, CoreStrings.ExchangeHygieneManagementDescription, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ManagementForestOperator_InitInfo, CoreStrings.ExchangeManagementForestOperatorDescription, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ManagementForestTier1Support_InitInfo, CoreStrings.ExchangeManagementForestTier1SupportDescription, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ViewOnlyManagementForestOperator_InitInfo, CoreStrings.ExchangeViewOnlyManagementForestOperatorDescription, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ManagementForestMonitoring_InitInfo, CoreStrings.ExchangeManagementForestMonitoringDescription, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.DataCenterManagement_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ViewOnlyLocalServerAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.DestructiveAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ElevatedPermissions_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ServiceAccounts_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.Operations_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ViewOnly_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ComplianceManagement_InitInfo, CoreStrings.ComplianceManagementGroupDescription, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ViewOnlyPII_InitInfo, CoreStrings.ViewOnlyPIIGroupDescription, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.CapacityDestructiveAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.CapacityServerAdmins_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.CapacityFrontendServerAdmins_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.CustomerChangeAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.CustomerDataAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.AccessToCustomerDataDCOnly_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.DatacenterOperationsDCOnly_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.CustomerDestructiveAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.CustomerPIIAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ManagementAdminAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ManagementCACoreAdmin_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ManagementChangeAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ManagementDestructiveAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ManagementServerAdmins_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.CapacityDCAdmins_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.NetworkingAdminAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.NetworkingChangeAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.CommunicationManagers_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.MailboxManagement_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.FfoAntiSpamAdmins_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.DedicatedSupportAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ECSAdminServerAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ECSPIIAccessServerAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ECSAdmin_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.ECSPIIAccess_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0]),
			new RoleGroupDefinition(RoleGroup.AppLockerExemption_InitInfo, null, new List<Datacenter.ExchangeSku>
			{
				Datacenter.ExchangeSku.ExchangeDatacenter,
				Datacenter.ExchangeSku.DatacenterDedicated
			}, new Guid[0])
		};
	}
}
