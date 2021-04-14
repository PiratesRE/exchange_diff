using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class AdFolderReader : AdReader
	{
		internal static List<ELCFolder> GetAllFolders(bool orgFoldersOnly)
		{
			return AdFolderReader.GetAllFolders(orgFoldersOnly, null);
		}

		internal static List<ELCFolder> GetAllFolders(bool orgFoldersOnly, IBudget budget)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 123, "GetAllFolders", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\ELC\\AdFolderReader.cs");
			if (budget != null)
			{
				tenantOrTopologyConfigurationSession.SessionSettings.AccountingObject = budget;
			}
			QueryFilter filter = null;
			if (orgFoldersOnly)
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, ELCFolderSchema.FolderType, ElcFolderType.ManagedCustomFolder);
			}
			ADPagedReader<ELCFolder> elcFolders = tenantOrTopologyConfigurationSession.FindPaged<ELCFolder>(tenantOrTopologyConfigurationSession.GetOrgContainerId(), QueryScope.SubTree, filter, null, 0);
			AdReader.Tracer.TraceDebug(0L, "Found ELCFolders in the AD.");
			List<ELCFolder> list = new List<ELCFolder>();
			AdFolderReader.ExtractTheGoodFolders(elcFolders, list);
			return list;
		}

		internal static void LoadFoldersInOrg(OrganizationId orgId, List<ELCFolder> allFolders)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, sessionSettings, 175, "LoadFoldersInOrg", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\ELC\\AdFolderReader.cs");
			ADPagedReader<ELCFolder> elcFolders = tenantOrTopologyConfigurationSession.FindPaged<ELCFolder>(null, QueryScope.SubTree, null, null, 0);
			AdReader.Tracer.TraceDebug(0L, "Found ELCFolders in the AD.");
			AdFolderReader.ExtractTheGoodFolders(elcFolders, allFolders);
		}

		internal static bool ElcMailboxPoliciesExist(OrganizationId orgId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, sessionSettings, 202, "ElcMailboxPoliciesExist", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\ELC\\AdFolderReader.cs");
			bool result;
			try
			{
				ManagedFolderMailboxPolicy[] array = tenantOrTopologyConfigurationSession.Find<ManagedFolderMailboxPolicy>(orgId.ConfigurationUnit, QueryScope.SubTree, null, null, 0);
				AdReader.Tracer.TraceDebug<int>(0L, "Found '{0}' ManagedFolderMailboxPolicy objects in the AD.", array.Length);
				result = (array.Length > 0);
			}
			catch (DataValidationException)
			{
				AdReader.Tracer.TraceError(0L, "Retrieval of ManagedFolderMailboxPolicies hit data validation issue.");
				throw;
			}
			return result;
		}

		internal static List<AdFolderData> GetUserElcFolders(MailboxSession session, ADUser aduser, List<ELCFolder> allAdFolders, bool getFoldersOnly, bool getOrgFoldersOnly)
		{
			if (allAdFolders == null || allAdFolders.Count == 0 || aduser == null)
			{
				return null;
			}
			string arg = session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(session.MailboxOwner.MailboxInfo.OrganizationId);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, sessionSettings, 258, "GetUserElcFolders", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\ELC\\AdFolderReader.cs");
			ADObjectId managedFolderMailboxPolicy = aduser.ManagedFolderMailboxPolicy;
			if (managedFolderMailboxPolicy == null)
			{
				AdReader.Tracer.TraceDebug<string>(0L, "Mailbox '{0}' does not have an ELC Mailbox policy.", arg);
				return null;
			}
			ManagedFolderMailboxPolicy managedFolderMailboxPolicy2 = tenantOrTopologyConfigurationSession.Read<ManagedFolderMailboxPolicy>(managedFolderMailboxPolicy);
			if (managedFolderMailboxPolicy2 == null)
			{
				AdReader.Tracer.TraceDebug<string, ADObjectId>(0L, "Mailbox '{0}' no matching ELC Mailbox policy for Template '{1}'.", arg, managedFolderMailboxPolicy);
				return null;
			}
			MultiValuedProperty<ADObjectId> managedFolderLinks = managedFolderMailboxPolicy2.ManagedFolderLinks;
			List<AdFolderData> list = new List<AdFolderData>();
			using (MultiValuedProperty<ADObjectId>.Enumerator enumerator = managedFolderLinks.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ADObjectId elcFolderId = enumerator.Current;
					ELCFolder elcfolder = allAdFolders.Find((ELCFolder adFolder) => elcFolderId.ObjectGuid == adFolder.Id.ObjectGuid);
					if (elcfolder == null)
					{
						throw new ELCNoMatchingOrgFoldersException(elcFolderId.DistinguishedName);
					}
					if (!getOrgFoldersOnly || elcfolder.FolderType == ElcFolderType.ManagedCustomFolder)
					{
						AdFolderData adFolderData = new AdFolderData();
						adFolderData.LinkedToTemplate = true;
						adFolderData.Synced = false;
						adFolderData.Folder = elcfolder;
						if (!getFoldersOnly)
						{
							adFolderData.FolderSettings = AdFolderReader.FetchFolderContentSettings(elcfolder);
						}
						list.Add(adFolderData);
					}
				}
			}
			return list;
		}

		internal static bool HasOrganizationalFolders(List<AdFolderData> adFolderList)
		{
			int num = adFolderList.FindIndex((AdFolderData adFolder) => ElcFolderType.ManagedCustomFolder == adFolder.Folder.FolderType);
			return -1 != num;
		}

		internal static string GetElcRootUrl()
		{
			return AdFolderReader.GetElcRootUrl(null);
		}

		internal static string GetElcRootUrl(IBudget budget)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 362, "GetElcRootUrl", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\ELC\\AdFolderReader.cs");
			if (budget != null)
			{
				tenantOrTopologyConfigurationSession.SessionSettings.AccountingObject = budget;
			}
			Organization organization = tenantOrTopologyConfigurationSession.FindSingletonConfigurationObject<Organization>();
			if (!string.IsNullOrEmpty(organization.ManagedFolderHomepage))
			{
				return organization.ManagedFolderHomepage;
			}
			return null;
		}

		internal static ContentSetting[] FetchFolderContentSettings(ELCFolder elcFolder)
		{
			ElcContentSettings[] array = elcFolder.GetELCContentSettings().ReadAllPages();
			List<ContentSetting> list = new List<ContentSetting>();
			foreach (ElcContentSettings elcContentSettings in array)
			{
				ValidationError[] array3 = elcContentSettings.ValidateRead();
				if (array3 != null && array3.Length > 0)
				{
					ValidationError[] array4 = array3;
					int num = 0;
					if (num < array4.Length)
					{
						ValidationError validationError = array4[num];
						AdReader.Tracer.TraceError<Guid, LocalizedString>(0L, "The ElcContentSettings '{0}' has a validation error: {1}", elcContentSettings.Guid, validationError.Description);
						throw new DataValidationException(validationError);
					}
				}
				list.Add(new ContentSetting(elcContentSettings));
			}
			return list.ToArray();
		}

		private static void ExtractTheGoodFolders(ADPagedReader<ELCFolder> elcFolders, List<ELCFolder> elcFoldersToReturn)
		{
			foreach (ELCFolder elcfolder in elcFolders)
			{
				ValidationError[] array = elcfolder.ValidateRead();
				if (array != null && array.Length > 0)
				{
					foreach (ValidationError validationError in array)
					{
						AdReader.Tracer.TraceError<string>(0L, "The elcFolder '{0}' has a validation error.", elcfolder.Name);
						if (!(validationError is PropertyValidationError))
						{
							AdReader.Tracer.TraceError<Type, LocalizedString>(0L, "The error type is '{0}'.  Validation error: {1}", validationError.GetType(), validationError.Description);
							throw new DataValidationException(validationError);
						}
						PropertyDefinition propertyDefinition = ((PropertyValidationError)validationError).PropertyDefinition;
						if (propertyDefinition != ELCFolderSchema.FolderType)
						{
							AdReader.Tracer.TraceError<Type, string, LocalizedString>(0L, "The error type is '{0}'.  Property '{1}' has a property validation error: {2}", validationError.GetType(), propertyDefinition.Name, validationError.Description);
							throw new DataValidationException(validationError);
						}
						AdReader.Tracer.TraceError<string>(0L, "New default folder types were added.  Skipping folder '{0}' because this version of Exchange can't handle it.", elcfolder.Name);
					}
				}
				else
				{
					elcFoldersToReturn.Add(elcfolder);
				}
			}
		}
	}
}
