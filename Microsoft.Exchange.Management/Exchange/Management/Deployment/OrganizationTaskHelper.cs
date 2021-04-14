using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	internal static class OrganizationTaskHelper
	{
		internal static void SetOrganizationStatusTimeout(int seconds)
		{
			OrganizationTaskHelper.organizationStatusTimeout = new TimeSpan(0, 0, seconds);
		}

		internal static ADOrganizationalUnit GetOUFromOrganizationId(OrganizationIdParameter organization, IConfigurationSession session, Task.TaskErrorLoggingDelegate errorLogger, bool reportError)
		{
			bool useConfigNC = session.UseConfigNC;
			ADOrganizationalUnit result = null;
			try
			{
				session.UseConfigNC = false;
				IEnumerable<ADOrganizationalUnit> objects = organization.GetObjects<ADOrganizationalUnit>(null, session);
				using (IEnumerator<ADOrganizationalUnit> enumerator = objects.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						result = enumerator.Current;
						if (reportError && enumerator.MoveNext())
						{
							errorLogger(new ManagementObjectAmbiguousException(Strings.ErrorOrganizationNotUnique(organization.ToString())), ErrorCategory.InvalidArgument, null);
						}
					}
					else if (reportError)
					{
						errorLogger(new ManagementObjectNotFoundException(Strings.ErrorOrganizationNotFound(organization.ToString())), ErrorCategory.InvalidArgument, null);
					}
				}
			}
			finally
			{
				session.UseConfigNC = useConfigNC;
			}
			return result;
		}

		internal static ExchangeConfigurationUnit GetExchangeConfigUnitFromOrganizationId(OrganizationIdParameter organization, IConfigurationSession session, Task.TaskErrorLoggingDelegate errorLogger, bool reportError)
		{
			ExchangeConfigurationUnit result = null;
			IEnumerable<ExchangeConfigurationUnit> objects = organization.GetObjects<ExchangeConfigurationUnit>(null, session);
			using (IEnumerator<ExchangeConfigurationUnit> enumerator = objects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (reportError && enumerator.MoveNext())
					{
						errorLogger(new ManagementObjectAmbiguousException(Strings.ErrorOrganizationNotUnique(organization.ToString())), ErrorCategory.InvalidArgument, null);
					}
				}
				else if (reportError)
				{
					errorLogger(new ManagementObjectNotFoundException(Strings.ErrorOrganizationNotFound(organization.ToString())), ErrorCategory.InvalidArgument, null);
				}
			}
			return result;
		}

		internal static void SetOrganizationStatus(IConfigurationSession session, ExchangeConfigurationUnit tenantCU, OrganizationStatus newStatus)
		{
			tenantCU.OrganizationStatus = newStatus;
			if (!tenantCU.IsTenantAccessBlocked && ExchangeConfigurationUnit.IsBeingDeleted(newStatus))
			{
				tenantCU.IsTenantAccessBlocked = true;
			}
			session.Save(tenantCU);
		}

		internal static OrganizationStatus SetOrganizationStatus(OrganizationIdParameter orgIdParam, IConfigurationSession session, OrganizationStatus statusToSet, Task.TaskErrorLoggingDelegate errorLogger)
		{
			ExchangeConfigurationUnit exchangeConfigUnitFromOrganizationId = OrganizationTaskHelper.GetExchangeConfigUnitFromOrganizationId(orgIdParam, session, errorLogger, true);
			OrganizationStatus organizationStatus = exchangeConfigUnitFromOrganizationId.OrganizationStatus;
			if (statusToSet != organizationStatus)
			{
				bool useConfigNC = session.UseConfigNC;
				try
				{
					session.UseConfigNC = true;
					exchangeConfigUnitFromOrganizationId.OrganizationStatus = statusToSet;
					session.Save(exchangeConfigUnitFromOrganizationId);
				}
				finally
				{
					session.UseConfigNC = useConfigNC;
				}
			}
			return organizationStatus;
		}

		internal static OrganizationStatus GetOrganizationStatus(OrganizationIdParameter orgIdParam, IConfigurationSession session, Task.TaskErrorLoggingDelegate errorLogger)
		{
			ExchangeConfigurationUnit exchangeConfigUnitFromOrganizationId = OrganizationTaskHelper.GetExchangeConfigUnitFromOrganizationId(orgIdParam, session, errorLogger, true);
			return exchangeConfigUnitFromOrganizationId.OrganizationStatus;
		}

		internal static bool CanProceedWithOrganizationTask(OrganizationIdParameter orgIdParam, IConfigurationSession session, OrganizationStatus[] ignorableFlagsOnStatusTimeout, Task.TaskErrorLoggingDelegate errorLogger)
		{
			bool result = false;
			ExchangeConfigurationUnit exchangeConfigUnitFromOrganizationId = OrganizationTaskHelper.GetExchangeConfigUnitFromOrganizationId(orgIdParam, session, errorLogger, true);
			if (ExchangeConfigurationUnit.IsOrganizationActive(exchangeConfigUnitFromOrganizationId.OrganizationStatus))
			{
				result = true;
			}
			else
			{
				DateTime? whenOrganizationStatusSet = exchangeConfigUnitFromOrganizationId.WhenOrganizationStatusSet;
				if (whenOrganizationStatusSet != null)
				{
					DateTime value = whenOrganizationStatusSet.Value.ToUniversalTime();
					if (DateTime.UtcNow.Subtract(value) > OrganizationTaskHelper.organizationStatusTimeout && ignorableFlagsOnStatusTimeout != null)
					{
						foreach (OrganizationStatus organizationStatus in ignorableFlagsOnStatusTimeout)
						{
							if (organizationStatus == exchangeConfigUnitFromOrganizationId.OrganizationStatus)
							{
								result = true;
								break;
							}
						}
					}
				}
			}
			return result;
		}

		internal static AcceptedDomain GetAcceptedDomain(AcceptedDomainIdParameter acceptedDomainId, IConfigurationSession adSession, Task.TaskErrorLoggingDelegate errorLogger, bool reportError)
		{
			AcceptedDomain result = null;
			IEnumerable<AcceptedDomain> objects = acceptedDomainId.GetObjects<AcceptedDomain>(null, adSession);
			using (IEnumerator<AcceptedDomain> enumerator = objects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (reportError && enumerator.MoveNext())
					{
						errorLogger(new ManagementObjectAmbiguousException(Strings.ErrorSecondaryDomainNotUnique(acceptedDomainId.ToString())), ErrorCategory.InvalidArgument, null);
					}
				}
				else if (reportError)
				{
					errorLogger(new ManagementObjectNotFoundException(Strings.ErrorSecondaryDomainNotFound(acceptedDomainId.ToString())), ErrorCategory.InvalidArgument, null);
				}
			}
			return result;
		}

		internal static void ValidateParamString(string paramName, string value, Task.TaskErrorLoggingDelegate errorLogger)
		{
			OrganizationTaskHelper.ValidateParamString(paramName, value, errorLogger, false);
		}

		internal static void ValidateParamString(string paramName, string value, Task.TaskErrorLoggingDelegate errorLogger, bool blockWildcards)
		{
			if (string.IsNullOrEmpty(paramName))
			{
				throw new ArgumentNullException("paramName");
			}
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}
			if (value.Contains("\"") || value.Contains("$") || (blockWildcards && value.Contains("*")))
			{
				errorLogger(new ArgumentException(Strings.ErrorInvalidCharactersInParameterValue(paramName, value, blockWildcards ? "{'\"', '$', '*'}" : "{'\"', '$'}")), ErrorCategory.InvalidArgument, null);
			}
		}

		internal static OrganizationId ResolveOrganization(Task task, OrganizationIdParameter organization, ADObjectId rootOrgContainerId, LocalizedString cannotResolveOrganizationMessage)
		{
			if (organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerId, task.CurrentOrganizationId, task.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.RescopeToSubtree(sessionSettings), 371, "ResolveOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\OrganizationTaskHelper.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit oufromOrganizationId = OrganizationTaskHelper.GetOUFromOrganizationId(organization, tenantOrTopologyConfigurationSession, new Task.TaskErrorLoggingDelegate(task.WriteError), true);
				if (oufromOrganizationId == null)
				{
					task.WriteError(new ArgumentException(cannotResolveOrganizationMessage), ErrorCategory.InvalidOperation, null);
					return null;
				}
				return oufromOrganizationId.OrganizationId;
			}
			else
			{
				if (task.CurrentOrganizationId == OrganizationId.ForestWideOrgId)
				{
					task.WriteError(new ArgumentException(cannotResolveOrganizationMessage), ErrorCategory.InvalidOperation, null);
					return null;
				}
				return task.CurrentOrganizationId;
			}
		}

		internal static ExchangeConfigurationUnit[] FindSharedConfigurations(SharedConfigurationInfo sci, PartitionId partitionId)
		{
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 406, "FindSharedConfigurations", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\OrganizationTaskHelper.cs");
			return tenantConfigurationSession.FindSharedConfiguration(sci, true);
		}

		internal static bool IsSharedConfigLinkedToOtherTenants(OrganizationId organizationId, IConfigurationSession session)
		{
			ExchangeConfigurationUnit[] array = session.Find<ExchangeConfigurationUnit>(organizationId.ConfigurationUnit, QueryScope.Base, new ExistsFilter(OrganizationSchema.SupportedSharedConfigurationsBL), null, 1);
			return array != null && array.Length > 0;
		}

		internal static TimeSpan organizationStatusTimeout = new TimeSpan(0, 10, 0);

		internal static FileVersionInfo ManagementDllVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

		internal static ServerVersion CurrentBuildVersion = new ServerVersion(OrganizationTaskHelper.ManagementDllVersion.FileMajorPart, OrganizationTaskHelper.ManagementDllVersion.FileMinorPart, OrganizationTaskHelper.ManagementDllVersion.FileBuildPart, OrganizationTaskHelper.ManagementDllVersion.FilePrivatePart);
	}
}
