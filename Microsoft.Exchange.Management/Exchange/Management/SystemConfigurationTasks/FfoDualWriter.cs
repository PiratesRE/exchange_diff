using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class FfoDualWriter
	{
		private static IConfigDataProvider FfoDataProvider
		{
			get
			{
				if (FfoDualWriter.configDataProvider == null)
				{
					Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.WebserviceDataProvider");
					Type type = assembly.GetType("Microsoft.Exchange.Hygiene.WebserviceDataProvider.WebserviceDataProviderFactory");
					MethodInfo method = type.GetMethod("CreateDataProvider", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					FfoDualWriter.configDataProvider = (IConfigDataProvider)method.Invoke(null, new object[]
					{
						"Directory"
					});
				}
				return FfoDualWriter.configDataProvider;
			}
		}

		public FfoDualWriter(string oldName = null)
		{
			this.oldName = oldName;
		}

		public void Save<T>(Task task, T adObject) where T : ADObject, new()
		{
			FfoDualWriter.SaveToFfo<T>(task, adObject, this.oldName);
		}

		public static void SaveToFfo<T>(Task task, T adObject, string oldName = null) where T : ADObject, new()
		{
			FfoDualWriter.SaveToFfo<T>(task, adObject, TenantSettingSyncLogGenerator.Instance.GetLogType(adObject), oldName);
		}

		public static void SaveToFfo<T>(Task task, T adObject, TenantSettingSyncLogType logType, string oldName = null) where T : ADObject, new()
		{
			bool flag = false;
			try
			{
				if (!DatacenterRegistry.IsForefrontForOffice() && !task.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId) && DatacenterRegistry.IsDualWriteAllowed() && adObject != null)
				{
					flag = true;
					if (adObject.m_Session != null)
					{
						ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, adObject.Name);
						T t = adObject.m_Session.Find<T>(null, QueryScope.SubTree, filter, null, 0).FirstOrDefault<T>();
						if (t != null)
						{
							adObject = t;
						}
					}
					FfoDualWriter.FixTenantId(adObject);
					FfoDualWriter.HandleRenaming(adObject, oldName);
					FfoDualWriter.FfoDataProvider.Save(adObject);
				}
			}
			catch (Exception ex)
			{
				if (flag)
				{
					FfoDualWriter.LogToFile<T>(adObject, logType, oldName);
				}
				FfoDualWriter.LogException(ex);
			}
		}

		public static void DeleteFromFfo<T>(Task task, T adObject) where T : ADObject, new()
		{
			FfoDualWriter.DeleteFromFfo<T>(task, adObject, TenantSettingSyncLogGenerator.Instance.GetLogType(adObject));
		}

		public static void DeleteFromFfo<T>(Task task, T adObject, TenantSettingSyncLogType logType) where T : ADObject, new()
		{
			bool flag = false;
			try
			{
				if (!DatacenterRegistry.IsForefrontForOffice() && !task.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId) && DatacenterRegistry.IsDualWriteAllowed())
				{
					flag = true;
					FfoDualWriter.FixTenantId(adObject);
					TenantSettingSyncLogGenerator.Instance.LogChangesForDelete(adObject, logType, new Guid?(adObject.OrganizationalUnitRoot.ObjectGuid));
					FfoDualWriter.FfoDataProvider.Delete(adObject);
				}
			}
			catch (Exception ex)
			{
				if (flag)
				{
					FfoDualWriter.LogToFile<T>(adObject, logType, null);
				}
				FfoDualWriter.LogException(ex);
			}
		}

		private static void FixTenantId(IConfigurable instance)
		{
			IPropertyBag propertyBag = instance as IPropertyBag;
			OrganizationId organizationId = (OrganizationId)propertyBag[ADObjectSchema.OrganizationId];
			if (organizationId == null)
			{
				return;
			}
			IConfigDataProvider tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(organizationId), 192, "FixTenantId", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageHygiene\\HygieneConfiguration\\FfoDualWriter.cs");
			Guid objectGuid;
			ADOperationResult externalDirectoryOrganizationId = SystemConfigurationTasksHelper.GetExternalDirectoryOrganizationId(tenantOrTopologyConfigurationSession, organizationId, out objectGuid);
			if (!externalDirectoryOrganizationId.Succeeded)
			{
				throw new InvalidOperationException("Error resolving orgId to external org id", externalDirectoryOrganizationId.Exception);
			}
			string distinguishedName = FfoDualWriter.FfoRootDN.GetChildId(organizationId.OrganizationalUnit.Name).GetChildId(objectGuid.ToString()).DistinguishedName;
			propertyBag[ADObjectSchema.OrganizationalUnitRoot] = new ADObjectId(distinguishedName, objectGuid);
		}

		private static bool HandleRenaming(ADObject adObject, string oldName)
		{
			if (!string.IsNullOrWhiteSpace(oldName) && oldName != adObject.Name)
			{
				adObject[FfoDualWriter.oldNameProp] = oldName;
				return true;
			}
			return false;
		}

		private static void LogException(Exception ex)
		{
			TaskLogger.LogError(ex);
		}

		private static void LogToFile<T>(T adObject, TenantSettingSyncLogType logType, string oldName) where T : ADObject, new()
		{
			try
			{
				if (FfoDualWriter.HandleRenaming(adObject, oldName))
				{
					TenantSettingSyncLogGenerator.Instance.LogChangesForSave(adObject, logType, new Guid?(adObject.OrganizationalUnitRoot.ObjectGuid), null, new List<KeyValuePair<string, object>>
					{
						new KeyValuePair<string, object>(FfoDualWriter.oldNameProp.Name, adObject[FfoDualWriter.oldNameProp])
					});
				}
				else
				{
					TenantSettingSyncLogGenerator.Instance.LogChangesForSave(adObject, logType, new Guid?(adObject.OrganizationalUnitRoot.ObjectGuid), null, null);
				}
			}
			catch (Exception ex)
			{
				FfoDualWriter.LogException(ex);
			}
		}

		private static ADObjectId FfoRootDN = new ADObjectId("OU=Microsoft Exchange Hosted Organizations,DC=FFO,DC=extest,DC=microsoft,DC=com", Guid.Empty);

		private static IConfigDataProvider configDataProvider;

		private static readonly ADPropertyDefinition oldNameProp = new ADPropertyDefinition("OldName", ExchangeObjectVersion.Exchange2003, typeof(string), "oldName", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		private readonly string oldName;
	}
}
