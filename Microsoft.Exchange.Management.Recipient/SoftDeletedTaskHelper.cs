using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal static class SoftDeletedTaskHelper
	{
		internal static IRecipientSession GetSessionForSoftDeletedObjects(IRecipientSession dataProvider, ADObjectId rootId)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(dataProvider.DomainController, rootId ?? dataProvider.SearchRoot, dataProvider.Lcid, dataProvider.ReadOnly, dataProvider.ConsistencyMode, dataProvider.NetworkCredential, dataProvider.SessionSettings, 47, "GetSessionForSoftDeletedObjects", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\common\\SoftDeletedTaskHelper.cs");
			tenantOrRootOrgRecipientSession.SessionSettings.IncludeSoftDeletedObjects = true;
			tenantOrRootOrgRecipientSession.EnforceDefaultScope = dataProvider.EnforceDefaultScope;
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = dataProvider.UseGlobalCatalog;
			tenantOrRootOrgRecipientSession.LinkResolutionServer = dataProvider.LinkResolutionServer;
			return tenantOrRootOrgRecipientSession;
		}

		internal static IRecipientSession GetSessionForSoftDeletedObjects(OrganizationId orgId, Fqdn domainController)
		{
			ADObjectId searchRoot = null;
			if (orgId != null && orgId.OrganizationalUnit != null)
			{
				searchRoot = new ADObjectId("OU=Soft Deleted Objects," + orgId.OrganizationalUnit.DistinguishedName);
			}
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), orgId ?? OrganizationId.ForestWideOrgId, null, false);
			adsessionSettings.IncludeSoftDeletedObjects = true;
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, searchRoot, CultureInfo.CurrentCulture.LCID, false, ConsistencyMode.IgnoreInvalid, null, adsessionSettings, 83, "GetSessionForSoftDeletedObjects", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\common\\SoftDeletedTaskHelper.cs");
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
			return tenantOrRootOrgRecipientSession;
		}

		public static IRecipientSession CreateTenantOrRootOrgRecipientSessionIncludeInactiveMailbox(IRecipientSession dataProvider, OrganizationId orgId)
		{
			return SoftDeletedTaskHelper.CreateIncludeInactiveMailboxTenantOrRootOrgRecipientSession(dataProvider, null);
		}

		public static IRecipientSession CreateTenantOrRootOrgRecipientSessionInactiveMailboxOnly(IRecipientSession dataProvider, OrganizationId orgId)
		{
			ADObjectId rootId = dataProvider.SearchRoot;
			if (orgId != null && orgId.OrganizationalUnit != null)
			{
				rootId = new ADObjectId("OU=Soft Deleted Objects," + orgId.OrganizationalUnit.DistinguishedName);
			}
			return SoftDeletedTaskHelper.CreateIncludeInactiveMailboxTenantOrRootOrgRecipientSession(dataProvider, rootId);
		}

		internal static ADUser GetSoftDeletedADUser(OrganizationId organizationId, RecipientIdParameter identity, Task.ErrorLoggerDelegate errorLogger)
		{
			IRecipientSession sessionForSoftDeletedObjects = SoftDeletedTaskHelper.GetSessionForSoftDeletedObjects(organizationId, null);
			IEnumerable<ADUser> enumerable = identity.GetObjects<ADUser>(organizationId.OrganizationalUnit, sessionForSoftDeletedObjects) ?? new List<ADUser>();
			ADUser result;
			using (IEnumerator<ADUser> enumerator = enumerable.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					errorLogger(new RecipientTaskException(Strings.ErrorMailboxNotFound(identity.ToString())), ExchangeErrorCategory.Client, null);
				}
				result = enumerator.Current;
				if (enumerator.MoveNext())
				{
					errorLogger(new RecipientTaskException(Strings.ErrorMailboxNotUnique(identity.ToString())), ExchangeErrorCategory.Client, null);
				}
			}
			return result;
		}

		internal static void UpdateRecipientForSoftDelete(IRecipientSession session, ADUser recipient, bool includeInGarbageCollection)
		{
			SoftDeletedTaskHelper.UpdateRecipientForSoftDelete(session, recipient, includeInGarbageCollection, false);
		}

		internal static void UpdateRecipientForSoftDelete(IRecipientSession session, ADUser recipient, bool includeInGarbageCollection, bool isInactive)
		{
			int num = 1;
			if (includeInGarbageCollection)
			{
				num |= 4;
			}
			if (isInactive)
			{
				num |= 8;
			}
			recipient.propertyBag.SetField(ADRecipientSchema.RecipientSoftDeletedStatus, num);
			recipient.propertyBag.SetField(ADRecipientSchema.WhenSoftDeleted, new DateTime?(DateTime.UtcNow));
			int num2 = (int)recipient.propertyBag[ADRecipientSchema.TransportSettingFlags];
			num2 |= 8;
			recipient.propertyBag.SetField(ADRecipientSchema.TransportSettingFlags, num2);
			if (!"Soft Deleted Objects".Equals(recipient.Id.Parent.Name))
			{
				ADObjectId childId = recipient.Id.Parent.GetChildId("OU", "Soft Deleted Objects");
				childId = childId.GetChildId(SoftDeletedTaskHelper.ReservedADNameStringRegex.Replace(recipient.Id.Name, string.Empty));
				string userPrincipalName = recipient.UserPrincipalName;
				session.SessionSettings.IncludeSoftDeletedObjects = true;
				if (session.Read(childId) != null)
				{
					childId = childId.Parent.GetChildId(MailboxTaskHelper.AppendRandomNameSuffix(childId.Name));
				}
				session.SessionSettings.IncludeSoftDeletedObjects = false;
				recipient.SetId(childId);
				recipient.UserPrincipalName = userPrincipalName;
			}
		}

		internal static void UpdateMailboxForDisconnectInactiveMailbox(ADUser mailbox)
		{
			string text = mailbox[ADUserSchema.AdminDisplayName] as string;
			if (text == null || !text.Contains("MSOID:"))
			{
				mailbox[ADUserSchema.AdminDisplayName] = text + "MSOID:" + mailbox.ExternalDirectoryObjectId;
			}
			mailbox.ExternalDirectoryObjectId = string.Empty;
		}

		internal static string GetUniqueNameForRecovery(IRecipientSession session, string name, ADObjectId id)
		{
			string result = name;
			ADRecipient adrecipient = session.Read(id.Parent.Parent.GetChildId(name));
			if (adrecipient != null)
			{
				result = MailboxTaskHelper.AppendRandomNameSuffix(name);
			}
			return result;
		}

		internal static void UpdateExchangeGuidForMailEnabledUser(ADUser user)
		{
			if (user.ExchangeGuid != Guid.Empty && user.ExchangeGuid != SoftDeletedTaskHelper.PredefinedExchangeGuid)
			{
				user.PreviousExchangeGuid = user.ExchangeGuid;
				user.ExchangeGuid = SoftDeletedTaskHelper.PredefinedExchangeGuid;
			}
		}

		public static bool MSOSyncEnabled(IConfigurationSession session, OrganizationId organizationId)
		{
			ExchangeConfigurationUnit exchangeConfigUnit = RecipientTaskHelper.GetExchangeConfigUnit(session, organizationId);
			return exchangeConfigUnit != null && exchangeConfigUnit.MSOSyncEnabled;
		}

		public static bool MSODirSyncEnabled(IConfigurationSession session, OrganizationId organizationId)
		{
			ExchangeConfigurationUnit exchangeConfigUnit = RecipientTaskHelper.GetExchangeConfigUnit(session, organizationId);
			return exchangeConfigUnit != null && exchangeConfigUnit.MSOSyncEnabled && exchangeConfigUnit.IsDirSyncRunning;
		}

		public static bool IsSoftDeleteSupportedRecipientTypeDetail(RecipientTypeDetails typeDetail)
		{
			if (typeDetail <= RecipientTypeDetails.RoomMailbox)
			{
				if (typeDetail != RecipientTypeDetails.UserMailbox && typeDetail != RecipientTypeDetails.SharedMailbox && typeDetail != RecipientTypeDetails.RoomMailbox)
				{
					return false;
				}
			}
			else if (typeDetail != RecipientTypeDetails.EquipmentMailbox && typeDetail != RecipientTypeDetails.MailUser && typeDetail != RecipientTypeDetails.User)
			{
				return false;
			}
			return true;
		}

		public static void UpdateShadowWhenSoftDeletedProperty(IRecipientSession session, IConfigurationSession configSession, OrganizationId organizationId, ADUser user)
		{
			if (!session.ServerSettings.WriteShadowProperties || user.propertyBag[ADRecipientSchema.WhenSoftDeleted.ShadowProperty] != null)
			{
				return;
			}
			ExchangeConfigurationUnit exchangeConfigUnit = RecipientTaskHelper.GetExchangeConfigUnit(configSession, organizationId);
			if (exchangeConfigUnit != null && exchangeConfigUnit.MSOSyncEnabled)
			{
				user.propertyBag.SetField(ADRecipientSchema.WhenSoftDeleted.ShadowProperty, user.WhenSoftDeleted);
				session.Save(user);
			}
		}

		private static IRecipientSession CreateIncludeInactiveMailboxTenantOrRootOrgRecipientSession(IRecipientSession dataProvider, ADObjectId rootId)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(dataProvider.DomainController, (rootId == null) ? dataProvider.SearchRoot : rootId, dataProvider.Lcid, dataProvider.ReadOnly, dataProvider.ConsistencyMode, dataProvider.NetworkCredential, dataProvider.SessionSettings, ConfigScopes.TenantSubTree, 342, "CreateIncludeInactiveMailboxTenantOrRootOrgRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\common\\SoftDeletedTaskHelper.cs");
			tenantOrRootOrgRecipientSession.SessionSettings.IncludeInactiveMailbox = true;
			tenantOrRootOrgRecipientSession.EnforceDefaultScope = dataProvider.EnforceDefaultScope;
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = dataProvider.UseGlobalCatalog;
			tenantOrRootOrgRecipientSession.LinkResolutionServer = dataProvider.LinkResolutionServer;
			return tenantOrRootOrgRecipientSession;
		}

		private const string ReservedStringPattern = "\\x0a(CNF|DEL):([0-9a-f]){8}-(([0-9a-f]){4}-){3}([0-9a-f]){12}$";

		private static readonly Regex ReservedADNameStringRegex = new Regex("\\x0a(CNF|DEL):([0-9a-f]){8}-(([0-9a-f]){4}-){3}([0-9a-f]){12}$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		internal static readonly Guid PredefinedExchangeGuid = new Guid("{1B2EAA95-0D64-4469-9FB2-D8F9BE3E28CE}");
	}
}
