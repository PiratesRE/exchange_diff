using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RecipientTaskHelper
	{
		public static ActiveManager GetActiveManagerInstance()
		{
			return ActiveManager.GetNoncachingActiveManagerInstance();
		}

		public static bool IsExchange2007OrLater(int versionNumber)
		{
			return Server.E2007MinVersion <= versionNumber;
		}

		public static bool IsE14OrLater(int versionNumber)
		{
			return Server.E14MinVersion <= versionNumber;
		}

		public static bool IsE15OrLater(int versionNumber)
		{
			return Server.E15MinVersion <= versionNumber;
		}

		internal static IConfigurable ResolveDataObject<TDataObject>(IConfigDataProvider dataSession, IConfigDataProvider globalCatalogSession, ADServerSettings serverSettings, IIdentityParameter identity, ObjectId rootId, OptionalIdentityData optionalData, string domainController, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObjectHandler, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate errorHandler) where TDataObject : IConfigurable, new()
		{
			if (serverSettings == null)
			{
				throw new ArgumentNullException("serverSettings");
			}
			IConfigurable configurable = null;
			IDirectorySession directorySession = dataSession as IDirectorySession;
			bool flag = directorySession != null && (directorySession.SessionSettings.IncludeSoftDeletedObjects || directorySession.SessionSettings.IncludeInactiveMailbox);
			ADObjectId adobjectId;
			if (RecipientTaskHelper.IsValidDistinguishedName(identity, out adobjectId))
			{
				OptionalIdentityData optionalIdentityData = optionalData.Clone();
				optionalIdentityData.ConfigurationContainerRdn = null;
				optionalIdentityData.RootOrgDomainContainerId = null;
				configurable = getDataObjectHandler(identity, dataSession, rootId ?? adobjectId.Parent, optionalIdentityData, null, new LocalizedString?(Strings.ErrorManagementObjectAmbiguous(identity.ToString())), ExchangeErrorCategory.Client);
			}
			else if (domainController != null || !serverSettings.ViewEntireForest || flag)
			{
				configurable = getDataObjectHandler(identity, dataSession, rootId, optionalData, null, new LocalizedString?(Strings.ErrorManagementObjectAmbiguous(identity.ToString())), ExchangeErrorCategory.Client);
			}
			else
			{
				IConfigurable configurable2 = getDataObjectHandler(identity, globalCatalogSession, rootId, optionalData, null, new LocalizedString?(Strings.ErrorManagementObjectAmbiguous(identity.ToString())), ExchangeErrorCategory.Client);
				logHandler(Strings.VerboseRereadADObject(configurable2.Identity.ToString(), typeof(TDataObject).Name, ((ADObjectId)configurable2.Identity).ToDNString()));
				IDirectorySession directorySession2 = (IDirectorySession)dataSession;
				IConfigDataProvider configDataProvider = null;
				ADScopeException ex = null;
				if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(directorySession2, (ADObject)configurable2))
				{
					try
					{
						configDataProvider = (IConfigDataProvider)TaskHelper.UnderscopeSessionToOrganization(directorySession2, ((ADObject)configurable2).OrganizationId, true);
					}
					catch (ADScopeException ex2)
					{
						ex = ex2;
						errorHandler(new ManagementObjectNotFoundException(new LocalizedString(ex.Message)), ExchangeErrorCategory.Client, configurable2.Identity);
					}
					if (ex == null)
					{
						configurable = configDataProvider.Read<TDataObject>(configurable2.Identity);
					}
				}
				else
				{
					configurable = dataSession.Read<TDataObject>(configurable2.Identity);
				}
				logHandler(TaskVerboseStringHelper.GetSourceVerboseString(configDataProvider ?? dataSession));
				if (configurable == null)
				{
					errorHandler(new ManagementObjectNotFoundException(Strings.ErrorFailedToReadFromDC(configurable2.Identity.ToString(), (configDataProvider == null) ? dataSession.Source : configDataProvider.Source)), ExchangeErrorCategory.Client, configurable2.Identity);
				}
			}
			return configurable;
		}

		internal static bool IsValidDistinguishedName(IIdentityParameter identity, out ADObjectId id)
		{
			bool flag = false;
			string distinguishedName = null;
			id = null;
			if (!string.IsNullOrEmpty(identity.RawIdentity))
			{
				flag = ADObjectId.IsValidDistinguishedName(identity.RawIdentity);
				if (flag)
				{
					distinguishedName = identity.RawIdentity;
				}
			}
			if (!flag && identity is ADIdParameter)
			{
				ADIdParameter adidParameter = (ADIdParameter)identity;
				if (adidParameter.InternalADObjectId != null && !string.IsNullOrEmpty(adidParameter.InternalADObjectId.DistinguishedName))
				{
					flag = ADObjectId.IsValidDistinguishedName(adidParameter.InternalADObjectId.DistinguishedName);
					if (flag)
					{
						distinguishedName = adidParameter.InternalADObjectId.DistinguishedName;
					}
				}
			}
			if (flag)
			{
				id = new ADObjectId(distinguishedName);
				if (id.IsRelativeDn)
				{
					flag = false;
					id = null;
				}
			}
			return flag;
		}

		internal static bool IsMailEnabledRecipientType(RecipientType recipientType)
		{
			return RecipientType.UserMailbox == recipientType || RecipientType.MailContact == recipientType || RecipientType.MailUser == recipientType || RecipientType.MailNonUniversalGroup == recipientType || RecipientType.MailUniversalDistributionGroup == recipientType || RecipientType.MailUniversalSecurityGroup == recipientType || RecipientType.DynamicDistributionGroup == recipientType || RecipientType.SystemAttendantMailbox == recipientType || RecipientType.SystemMailbox == recipientType || RecipientType.PublicFolder == recipientType || RecipientType.PublicDatabase == recipientType;
		}

		internal static bool IsPropertyValueUnique(IRecipientSession recipientSession, ADScope scope, ADObjectId selfId, ADPropertyDefinition[] propertyDefinitionsToSearch, ADPropertyDefinition propertyDefinitionToReportError, object value, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate errorHandler, ExchangeErrorCategory errorLoggerCategory)
		{
			return RecipientTaskHelper.IsPropertyValueUnique(recipientSession, scope, selfId, propertyDefinitionsToSearch, propertyDefinitionToReportError, value, false, logHandler, errorHandler, errorLoggerCategory, false);
		}

		internal static bool IsPropertyValueUnique(IRecipientSession recipientSession, ADScope scope, ADObjectId selfId, ADPropertyDefinition[] propertyDefinitionsToSearch, ADPropertyDefinition propertyDefinitionToReportError, object value, bool showDuplicatedObjectInError, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate errorHandler, ExchangeErrorCategory errorLoggerCategory, bool includeSoftDeletedObjects)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (propertyDefinitionsToSearch == null || propertyDefinitionsToSearch.Length == 0)
			{
				throw new ArgumentNullException("propertyDefinitionToSearch");
			}
			if (value == null || string.IsNullOrEmpty(value.ToString()))
			{
				throw new ArgumentNullException("value");
			}
			if (errorHandler != null)
			{
				if (logHandler == null)
				{
					throw new ArgumentNullException("logHandler");
				}
				if (propertyDefinitionToReportError == null)
				{
					throw new ArgumentNullException("propertyDefinitionToReportError");
				}
			}
			bool enforceDefaultScope = recipientSession.EnforceDefaultScope;
			bool useGlobalCatalog = recipientSession.UseGlobalCatalog;
			bool useConfigNC = recipientSession.UseConfigNC;
			bool includeSoftDeletedObjects2 = recipientSession.SessionSettings.IncludeSoftDeletedObjects;
			bool result = true;
			try
			{
				recipientSession.EnforceDefaultScope = false;
				recipientSession.UseGlobalCatalog = (scope == null || scope.Root == null);
				recipientSession.UseConfigNC = false;
				if (includeSoftDeletedObjects)
				{
					recipientSession.SessionSettings.IncludeSoftDeletedObjects = true;
				}
				List<QueryFilter> list = new List<QueryFilter>(propertyDefinitionsToSearch.Length);
				foreach (ADPropertyDefinition adpropertyDefinition in propertyDefinitionsToSearch)
				{
					ComparisonFilter comparisonFilter = new ComparisonFilter(ComparisonOperator.Equal, adpropertyDefinition, value);
					if (adpropertyDefinition == propertyDefinitionToReportError && selfId != null)
					{
						ComparisonFilter comparisonFilter2 = new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, selfId);
						list.Add(new AndFilter(new QueryFilter[]
						{
							comparisonFilter2,
							comparisonFilter
						}));
					}
					else
					{
						list.Add(comparisonFilter);
					}
				}
				QueryFilter queryFilter = new OrFilter(list.ToArray());
				List<QueryFilter> list2 = new List<QueryFilter>();
				list2.Add(queryFilter);
				if (scope != null && scope.Filter != null)
				{
					list2.Add(new AndFilter(new QueryFilter[]
					{
						queryFilter,
						scope.Filter
					}));
				}
				if (list2.Count == 1)
				{
					queryFilter = list2[0];
				}
				else
				{
					queryFilter = new AndFilter(list2.ToArray());
				}
				ADObjectId rootId = (scope != null) ? scope.Root : null;
				if (logHandler != null)
				{
					logHandler(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(recipientSession, typeof(ADRecipient), queryFilter, rootId, true));
				}
				ADRecipient[] array = null;
				try
				{
					array = recipientSession.Find(rootId, QueryScope.SubTree, queryFilter, null, 1);
				}
				finally
				{
					if (logHandler != null)
					{
						logHandler(TaskVerboseStringHelper.GetSourceVerboseString(recipientSession));
					}
				}
				if (0 < array.Length)
				{
					result = false;
					if (errorHandler != null)
					{
						PropertyValueExistsException exception;
						if (showDuplicatedObjectInError)
						{
							exception = new PropertyValueExistsException(Strings.ErrorRecipientPropertyValueAlreadyExists(propertyDefinitionToReportError.Name, value.ToString(), array[0].Id.ToString()));
						}
						else
						{
							exception = new PropertyValueExistsException(Strings.ErrorRecipientPropertyValueAlreadybeUsed(propertyDefinitionToReportError.Name, value.ToString()));
						}
						StringBuilder stringBuilder = new StringBuilder();
						foreach (ADRecipient adrecipient in array)
						{
							stringBuilder.Append(adrecipient.Identity.ToString());
							stringBuilder.Append(',');
						}
						CmdletLogger.SafeAppendGenericInfo("PropertyValueExists", value.ToString());
						CmdletLogger.SafeAppendGenericInfo("DuplicatedRecipients", stringBuilder.ToString().TrimEnd(new char[]
						{
							','
						}));
						errorHandler(exception, errorLoggerCategory, selfId);
					}
				}
			}
			finally
			{
				recipientSession.EnforceDefaultScope = enforceDefaultScope;
				recipientSession.UseGlobalCatalog = useGlobalCatalog;
				recipientSession.UseConfigNC = useConfigNC;
				recipientSession.SessionSettings.IncludeSoftDeletedObjects = includeSoftDeletedObjects2;
			}
			return result;
		}

		public static bool IsUserPrincipalNameUnique(IRecipientSession recipientSession, ADRecipient recipient, string userPrincipalName, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate writeError, ExchangeErrorCategory errorLoggerCategory)
		{
			return RecipientTaskHelper.IsUserPrincipalNameUnique(recipientSession, recipient, userPrincipalName, logHandler, writeError, errorLoggerCategory, true);
		}

		public static bool IsUserPrincipalNameUnique(IRecipientSession recipientSession, ADRecipient recipient, string userPrincipalName, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate writeError, ExchangeErrorCategory errorLoggerCategory, bool excludeSelfCheck)
		{
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			return RecipientTaskHelper.IsPropertyValueUnique(recipientSession, null, excludeSelfCheck ? recipient.Id : null, new ADPropertyDefinition[]
			{
				ADUserSchema.UserPrincipalName
			}, ADUserSchema.UserPrincipalName, userPrincipalName, logHandler, writeError, errorLoggerCategory);
		}

		public static bool IsSamAccountNameUnique(ADRecipient recipient, string samAccountName, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate writeError, ExchangeErrorCategory errorLoggerCategory)
		{
			IRecipientSession recipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 611, "IsSamAccountNameUnique", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RecipientTaskHelper.cs");
			recipientSession.UseGlobalCatalog = true;
			bool flag = RecipientTaskHelper.IsSamAccountNameUnique(recipientSession, recipient, samAccountName, logHandler, writeError, errorLoggerCategory);
			if (VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ServiceAccountForest.Enabled)
			{
				recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(recipient.OrganizationId), 628, "IsSamAccountNameUnique", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RecipientTaskHelper.cs");
				recipientSession.UseGlobalCatalog = true;
				flag = (flag && RecipientTaskHelper.IsSamAccountNameUnique(recipientSession, recipient, samAccountName, logHandler, writeError, errorLoggerCategory));
			}
			return flag;
		}

		public static bool IsSamAccountNameUnique(IRecipientSession recipientSession, ADRecipient recipient, string samAccountName, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate writeError, ExchangeErrorCategory errorLoggerCategory)
		{
			return RecipientTaskHelper.IsSamAccountNameUnique(recipientSession, recipient, samAccountName, logHandler, writeError, errorLoggerCategory, true);
		}

		public static bool IsSamAccountNameUnique(IRecipientSession recipientSession, ADRecipient recipient, string samAccountName, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate writeError, ExchangeErrorCategory errorLoggerCategory, bool excludeSelfCheck)
		{
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			ADScope scope = null;
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ServiceAccountForest.Enabled)
			{
				scope = new ADScope(recipient.Id.DomainId, null);
			}
			if (null != recipient.Id.GetPartitionId())
			{
				recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAccountPartitionWideScopeSet(recipient.Id.GetPartitionId()), 709, "IsSamAccountNameUnique", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RecipientTaskHelper.cs");
				recipientSession.UseGlobalCatalog = true;
			}
			return RecipientTaskHelper.IsPropertyValueUnique(recipientSession, scope, excludeSelfCheck ? recipient.Id : null, new ADPropertyDefinition[]
			{
				ADMailboxRecipientSchema.SamAccountName
			}, ADMailboxRecipientSchema.SamAccountName, samAccountName, logHandler, writeError, errorLoggerCategory);
		}

		private static bool IsPropertyValueUniqueWithArgValidation(IRecipientSession recipientSession, OrganizationId organizationId, ADRecipient recipient, ADPropertyDefinition schemaPropertyAttribute, object value, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate writeError, ExchangeErrorCategory errorLoggerCategory)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			ADObjectId selfId = (recipient == null) ? null : recipient.Id;
			ADScope scope = null;
			if (organizationId.OrganizationalUnit != null)
			{
				scope = new ADScope(organizationId.OrganizationalUnit, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, organizationId.OrganizationalUnit));
			}
			return RecipientTaskHelper.IsPropertyValueUnique(recipientSession, scope, selfId, new ADPropertyDefinition[]
			{
				schemaPropertyAttribute
			}, schemaPropertyAttribute, value, logHandler, writeError, errorLoggerCategory);
		}

		public static bool IsJournalArchiveAddressUnique(IRecipientSession recipientSession, OrganizationId organizationId, ADRecipient recipient, SmtpAddress journalArchiveAddress, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate writeError, ExchangeErrorCategory errorLoggerCategory)
		{
			return RecipientTaskHelper.IsPropertyValueUniqueWithArgValidation(recipientSession, organizationId, recipient, ADRecipientSchema.JournalArchiveAddress, journalArchiveAddress, logHandler, writeError, errorLoggerCategory);
		}

		public static bool IsAliasUnique(IRecipientSession recipientSession, OrganizationId organizationId, ADRecipient recipient, string alias, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate writeError, ExchangeErrorCategory errorLoggerCategory)
		{
			return RecipientTaskHelper.IsPropertyValueUniqueWithArgValidation(recipientSession, organizationId, recipient, ADRecipientSchema.Alias, alias, logHandler, writeError, errorLoggerCategory);
		}

		public static bool IsExchangeGuidOrArchiveGuidUnique(ADRecipient recipient, ADPropertyDefinition propertyDefinitionToReportError, Guid guid, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate writeError, ExchangeErrorCategory errorLoggerCategory)
		{
			return RecipientTaskHelper.IsExchangeGuidOrArchiveGuidUnique(RecipientTaskHelper.CreatePartitionOrRootOrgScopedGcSession(null, recipient.Id), recipient, propertyDefinitionToReportError, guid, logHandler, writeError, errorLoggerCategory);
		}

		public static bool IsExchangeGuidOrArchiveGuidUnique(IRecipientSession recipientSession, ADRecipient recipient, ADPropertyDefinition propertyDefinitionToReportError, Guid guid, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate writeError, ExchangeErrorCategory errorLoggerCategory)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (logHandler == null)
			{
				throw new ArgumentNullException("logHandler");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			if (propertyDefinitionToReportError != ADMailboxRecipientSchema.ExchangeGuid && propertyDefinitionToReportError != ADUserSchema.ArchiveGuid)
			{
				throw new ArgumentException("propertyDefinitionToReportError must be ExchangeGuid or ArchiveGuid", "propertyDefinitionToReportError");
			}
			return RecipientTaskHelper.IsPropertyValueUnique(recipientSession, null, recipient.Id, new ADPropertyDefinition[]
			{
				ADMailboxRecipientSchema.ExchangeGuid,
				ADUserSchema.ArchiveGuid
			}, propertyDefinitionToReportError, guid, false, logHandler, writeError, errorLoggerCategory, true);
		}

		public static bool SMTPAddressCheckWithAcceptedDomain(IConfigurationSession cfgSession, OrganizationId organizationId, Task.ErrorLoggerDelegate errorLogger, ProvisioningCache provisioningCache)
		{
			if (errorLogger == null)
			{
				throw new ArgumentNullException("errorLogger");
			}
			if (organizationId.ConfigurationUnit == null)
			{
				return true;
			}
			ExchangeConfigurationUnit exchangeConfigurationUnit = provisioningCache.TryAddAndGetOrganizationData<ExchangeConfigurationUnit>(CannedProvisioningCacheKeys.OrganizationCUContainer, organizationId, delegate()
			{
				ADObjectId configurationUnit = organizationId.ConfigurationUnit;
				IConfigurationSession configurationSession = (IConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(cfgSession, organizationId, true);
				return configurationSession.Read<ExchangeConfigurationUnit>(configurationUnit);
			});
			if (exchangeConfigurationUnit == null)
			{
				errorLogger(new TaskInvalidOperationException(Strings.ErrorConfigurationUnitNotFound(organizationId.ConfigurationUnit.ToString())), ExchangeErrorCategory.ServerOperation, null);
			}
			return exchangeConfigurationUnit.SMTPAddressCheckWithAcceptedDomain;
		}

		public static void ValidateInAcceptedDomain(IConfigurationSession cfgSession, OrganizationId organizationId, string domainName, Task.ErrorLoggerDelegate errorLogger, ProvisioningCache provisioningCache)
		{
			if (errorLogger == null)
			{
				throw new ArgumentNullException("errorLogger");
			}
			if (!RecipientTaskHelper.IsAcceptedDomain(cfgSession, organizationId, domainName, provisioningCache))
			{
				RecipientTaskHelper.ReportNotAcceptedDomainError(errorLogger, domainName);
			}
		}

		public static bool IsAcceptedDomain(IConfigurationSession cfgSession, OrganizationId organizationId, string domainName, ProvisioningCache provisioningCache)
		{
			if (cfgSession == null)
			{
				throw new ArgumentNullException("cfgSession");
			}
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			if (string.IsNullOrEmpty(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			IEnumerable<SmtpDomainWithSubdomains> acceptedDomains = RecipientTaskHelper.GetAcceptedDomains(cfgSession, provisioningCache, organizationId);
			return RecipientTaskHelper.IsAcceptedDomain(acceptedDomains, domainName);
		}

		private static bool IsAcceptedDomain(IEnumerable<SmtpDomainWithSubdomains> domains, string domainName)
		{
			bool result = false;
			if (domains != null)
			{
				foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in domains)
				{
					if (smtpDomainWithSubdomains.Match(domainName) != -1)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private static IEnumerable<SmtpDomainWithSubdomains> GetAcceptedDomains(IConfigurationSession cfgSession, ProvisioningCache provisioningCache, OrganizationId orgId)
		{
			return provisioningCache.TryAddAndGetOrganizationData<IEnumerable<SmtpDomainWithSubdomains>>(CannedProvisioningCacheKeys.OrganizationAcceptedDomains, orgId, delegate()
			{
				ADObjectId rootId = orgId.ConfigurationUnit ?? provisioningCache.TryAddAndGetGlobalData<ADObjectId>(CannedProvisioningCacheKeys.FirstOrgContainerId, () => cfgSession.GetOrgContainerId());
				QueryFilter filter = new NotFilter(new BitMaskAndFilter(AcceptedDomainSchema.AcceptedDomainFlags, 1UL));
				ADPagedReader<AcceptedDomain> adpagedReader = cfgSession.FindPaged<AcceptedDomain>(rootId, QueryScope.SubTree, filter, null, 0);
				AcceptedDomain[] array = adpagedReader.ReadAllPages();
				SmtpDomainWithSubdomains[] array2 = new SmtpDomainWithSubdomains[array.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = array[i].DomainName;
				}
				return array2;
			});
		}

		public static Dictionary<string, ADObjectId> ValidateEmailAddress(IRecipientSession tenantCatalogSession, ProxyAddressCollection emailAddresses, ADRecipient self, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			if (tenantCatalogSession == null)
			{
				throw new ArgumentNullException("tenantCatalogSession");
			}
			if (emailAddresses == null)
			{
				throw new ArgumentNullException("emailAddresses");
			}
			if (self == null)
			{
				throw new ArgumentNullException("self");
			}
			if (writeVerbose == null)
			{
				throw new ArgumentNullException("writeVerbose");
			}
			Dictionary<string, ADObjectId> dictionary = new Dictionary<string, ADObjectId>();
			List<QueryFilter> list = new List<QueryFilter>(emailAddresses.Count);
			foreach (ProxyAddress proxyAddress in emailAddresses)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, proxyAddress.ToString()));
				if (proxyAddress.Prefix.Equals(ProxyAddressPrefix.X500))
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.LegacyExchangeDN, proxyAddress.ValueString));
				}
			}
			int num = LdapFilterBuilder.MaxCustomFilterTreeSize - 5;
			while (0 < list.Count)
			{
				QueryFilter[] array;
				if (num >= list.Count)
				{
					array = list.ToArray();
					list.Clear();
				}
				else
				{
					array = new QueryFilter[num];
					list.CopyTo(0, array, 0, num);
					list.RemoveRange(0, num);
				}
				QueryFilter queryFilter = new OrFilter(array);
				QueryFilter queryFilter2 = new NotFilter(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, self.Identity));
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					queryFilter2,
					queryFilter
				});
				writeVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(tenantCatalogSession, typeof(ADRecipient), filter, null, true));
				ADPagedReader<ADRecipient> adpagedReader = tenantCatalogSession.FindPaged(null, QueryScope.SubTree, filter, null, 0);
				foreach (ADRecipient adrecipient in adpagedReader)
				{
					ADObjectId value = (ADObjectId)adrecipient.Identity;
					foreach (ProxyAddress proxyAddress2 in emailAddresses)
					{
						if (adrecipient.EmailAddresses.Contains(proxyAddress2) || (proxyAddress2.Prefix.Equals(ProxyAddressPrefix.X500) && proxyAddress2.ValueString.Equals(adrecipient.LegacyExchangeDN, StringComparison.OrdinalIgnoreCase)))
						{
							dictionary[proxyAddress2.ToString()] = value;
						}
					}
				}
			}
			return dictionary;
		}

		public static string GetLocalPartOfUserPrincalName(string userPrincipalName)
		{
			if (string.IsNullOrEmpty(userPrincipalName))
			{
				return string.Empty;
			}
			int num = userPrincipalName.LastIndexOf('@');
			return userPrincipalName.Substring(0, (num > 0) ? num : 0);
		}

		public static string GetDomainPartOfUserPrincalName(string userPrincipalName)
		{
			if (string.IsNullOrEmpty(userPrincipalName))
			{
				return string.Empty;
			}
			int num = userPrincipalName.LastIndexOf('@');
			return userPrincipalName.Substring(num + 1);
		}

		public static string GenerateAlias(string preferredAlias)
		{
			if (string.IsNullOrEmpty(preferredAlias))
			{
				return null;
			}
			return AliasHelper.GenerateAlias(preferredAlias, false);
		}

		public static string GenerateSamAccountName(string preferredName, bool isGroup)
		{
			return RecipientTaskHelper.GenerateSamAccountName(preferredName, isGroup ? 256 : 20, isGroup ? "\"\\/[]:|<>+=;?,*\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f" : ADUser.SamAccountNameInvalidCharacters);
		}

		private static string GenerateSamAccountName(string preferredName, int maxLength, string invalidcharacters)
		{
			if (string.IsNullOrEmpty(preferredName))
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(preferredName);
			for (int i = 0; i < stringBuilder.Length; i++)
			{
				if (invalidcharacters.IndexOf(stringBuilder[i]) != -1)
				{
					stringBuilder[i] = '_';
				}
			}
			if (stringBuilder.Length > maxLength)
			{
				stringBuilder.Remove(maxLength, stringBuilder.Length - maxLength);
			}
			if (stringBuilder[stringBuilder.Length - 1] == '.')
			{
				stringBuilder[stringBuilder.Length - 1] = '_';
			}
			else
			{
				bool flag = true;
				for (int j = 0; j < stringBuilder.Length; j++)
				{
					if (!char.IsWhiteSpace(stringBuilder[j]))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					stringBuilder[stringBuilder.Length - 1] = '_';
				}
			}
			return stringBuilder.ToString();
		}

		public static string GenerateUniqueAlias(IRecipientSession recipientSession, OrganizationId organizationId, string preferredAlias, Task.TaskVerboseLoggingDelegate logHandler)
		{
			return RecipientTaskHelper.GenerateUniqueAlias(recipientSession, organizationId, preferredAlias, logHandler, AliasHelper.MaximalAliasLength);
		}

		public static string GenerateUniqueAlias(IRecipientSession recipientSession, OrganizationId organizationId, string preferredAlias, Task.TaskVerboseLoggingDelegate logHandler, int maximalAliasLength)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			string text = string.IsNullOrEmpty(preferredAlias) ? "User" : RecipientTaskHelper.GenerateAlias(preferredAlias);
			if (string.IsNullOrEmpty(text))
			{
				text = "User";
			}
			if (organizationId.OrganizationalUnit != null)
			{
				return RecipientTaskHelper.GenerateUniqueString(recipientSession, new ADScope(organizationId.OrganizationalUnit, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, organizationId.OrganizationalUnit)), ADRecipientSchema.Alias, text, null, maximalAliasLength, logHandler);
			}
			return RecipientTaskHelper.GenerateUniqueString(recipientSession, null, ADRecipientSchema.Alias, text, null, maximalAliasLength, logHandler);
		}

		public static string GenerateUniqueSamAccountName(IRecipientSession recipientSession, ADObjectId rootId, string preferredName, bool isGroup)
		{
			return RecipientTaskHelper.GenerateUniqueSamAccountName(recipientSession, rootId, preferredName, isGroup, null);
		}

		public static string GenerateUniqueSamAccountName(IRecipientSession recipientSession, ADObjectId rootId, string preferredName, bool isGroup, Task.TaskVerboseLoggingDelegate logHandler)
		{
			return RecipientTaskHelper.GenerateUniqueSamAccountName(recipientSession, rootId, preferredName, isGroup, logHandler, false);
		}

		public static string GenerateUniqueSamAccountName(IRecipientSession recipientSession, ADObjectId rootId, string preferredName, bool isGroup, Task.TaskVerboseLoggingDelegate logHandler, bool useRandomSuffix)
		{
			return RecipientTaskHelper.GenerateUniqueSamAccountName(new IRecipientSession[]
			{
				recipientSession
			}, rootId, preferredName, isGroup, logHandler, useRandomSuffix);
		}

		public static string GenerateUniqueSamAccountName(IRecipientSession[] recipientSessions, ADObjectId rootId, string preferredName, bool isGroup, Task.TaskVerboseLoggingDelegate logHandler, bool useRandomSuffix)
		{
			if (recipientSessions == null && recipientSessions.Length < 1)
			{
				throw new ArgumentNullException("recipientSessions");
			}
			if (rootId == null)
			{
				throw new ArgumentNullException("rootId");
			}
			string preferredString = string.IsNullOrEmpty(preferredName) ? "User" : RecipientTaskHelper.GenerateSamAccountName(preferredName, isGroup);
			ADScope scope = null;
			using (new CmdletMonitoredScope(Guid.Empty, "BizLogic", "VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ServiceAccountForest", LoggerHelper.CmdletPerfMonitors))
			{
				if (!VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ServiceAccountForest.Enabled)
				{
					scope = new ADScope(rootId, null);
				}
			}
			if (null != rootId.GetPartitionId())
			{
				using (new CmdletMonitoredScope(Guid.Empty, "BizLogic", "GenerateUniqueSamAccountName/GetTenantOrRootOrgRecipientSession", LoggerHelper.CmdletPerfMonitors))
				{
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAccountPartitionWideScopeSet(rootId.GetPartitionId()), 1495, "GenerateUniqueSamAccountName", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RecipientTaskHelper.cs");
					tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
					recipientSessions = new IRecipientSession[]
					{
						tenantOrRootOrgRecipientSession
					};
				}
			}
			string result;
			using (new CmdletMonitoredScope(Guid.Empty, "BizLogic", "GenerateUniqueSamAccountName/GenerateUniqueString", LoggerHelper.CmdletPerfMonitors))
			{
				result = RecipientTaskHelper.GenerateUniqueString(recipientSessions, scope, IADSecurityPrincipalSchema.SamAccountName, preferredString, null, isGroup ? 256 : 20, logHandler, useRandomSuffix);
			}
			return result;
		}

		public static string GenerateUniqueUserPrincipalName(IRecipientSession recipientSession, string preferredLocalPart, string domainName, Task.TaskVerboseLoggingDelegate logHandler)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (string.IsNullOrEmpty(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			if (string.IsNullOrEmpty(preferredLocalPart))
			{
				throw new ArgumentNullException("preferredLocalPart");
			}
			return RecipientTaskHelper.GenerateUniqueString(recipientSession, null, ADUserSchema.UserPrincipalName, preferredLocalPart, "@" + domainName, 1024, logHandler);
		}

		private static string GenerateUniqueString(IRecipientSession recipientSession, ADScope scope, ADPropertyDefinition propertyToCompare, string preferredString, string lastPart, int maxLength, Task.TaskVerboseLoggingDelegate logHandler)
		{
			return RecipientTaskHelper.GenerateUniqueString(recipientSession, scope, propertyToCompare, preferredString, lastPart, maxLength, logHandler, false);
		}

		private static string GenerateUniqueString(IRecipientSession recipientSession, ADScope scope, ADPropertyDefinition propertyToCompare, string preferredString, string lastPart, int maxLength, Task.TaskVerboseLoggingDelegate logHandler, bool useRandomSuffix)
		{
			return RecipientTaskHelper.GenerateUniqueString(new IRecipientSession[]
			{
				recipientSession
			}, scope, propertyToCompare, preferredString, lastPart, maxLength, logHandler, useRandomSuffix);
		}

		private static string GenerateUniqueString(IRecipientSession[] recipientSessions, ADScope scope, ADPropertyDefinition propertyToCompare, string preferredString, string lastPart, int maxLength, Task.TaskVerboseLoggingDelegate logHandler, bool useRandomSuffix)
		{
			int num = 0;
			int num2 = RecipientTaskHelper.GenerateSuffix(num, useRandomSuffix);
			if (string.IsNullOrEmpty(lastPart))
			{
				lastPart = string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num3 = -1;
			if (useRandomSuffix)
			{
				using (new CmdletMonitoredScope(Guid.Empty, "BizLogic", "RecipientTaskHelper.GenerateUniqueString/LocalSiteCache/LocalSite", LoggerHelper.CmdletPerfMonitors))
				{
					num3 = LocalSiteCache.LocalSite.MinorPartnerId;
				}
			}
			for (;;)
			{
				stringBuilder.Length = 0;
				stringBuilder.Append(preferredString);
				stringBuilder.Append(lastPart);
				if (num2 != 0)
				{
					string text = string.Empty;
					if (useRandomSuffix)
					{
						text = string.Format("{0}{1}", num3, num2);
						if (text.Length > maxLength)
						{
							text = text.Remove(maxLength);
						}
					}
					else
					{
						text = num2.ToString();
					}
					stringBuilder.Insert(preferredString.Length, text);
					if (stringBuilder.Length > maxLength)
					{
						stringBuilder.Remove(maxLength - text.ToString().Length - lastPart.Length, stringBuilder.Length - maxLength);
					}
				}
				bool flag = true;
				foreach (IRecipientSession recipientSession in recipientSessions)
				{
					using (new CmdletMonitoredScope(Guid.Empty, "BizLogic", "RecipientTaskHelper.GenerateUniqueString/IsPropertyValueUnique", LoggerHelper.CmdletPerfMonitors))
					{
						flag = RecipientTaskHelper.IsPropertyValueUnique(recipientSession, scope, null, new ADPropertyDefinition[]
						{
							propertyToCompare
						}, null, stringBuilder.ToString(), logHandler, null, (ExchangeErrorCategory)0);
					}
					if (!flag)
					{
						break;
					}
				}
				if (flag)
				{
					break;
				}
				num++;
				num2 = RecipientTaskHelper.GenerateSuffix(num, useRandomSuffix);
			}
			return stringBuilder.ToString();
		}

		private static int GenerateSuffix(int count, bool useRandomSuffix)
		{
			int result = 0;
			if (useRandomSuffix)
			{
				result = Guid.NewGuid().ToString("N").GetHashCode();
			}
			else if (count != 0)
			{
				if (count < 10)
				{
					result = count;
				}
				else
				{
					ExDateTime now = ExDateTime.Now;
					result = (now.Minute * 60 + now.Second) * 1000 + now.Millisecond;
				}
			}
			return result;
		}

		public static ADObjectId GetDefaultOUIdForRecipient(ADObjectId defaultScope)
		{
			string canonicalName;
			if (defaultScope != null && defaultScope.DomainId != null)
			{
				if (!defaultScope.DomainId.Equals(defaultScope))
				{
					return defaultScope;
				}
				canonicalName = defaultScope.DomainId.ToCanonicalName();
			}
			else
			{
				canonicalName = NativeHelpers.GetDomainName();
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 1784, "GetDefaultOUIdForRecipient", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RecipientTaskHelper.cs");
			topologyConfigurationSession.UseConfigNC = false;
			ADObject adobject = topologyConfigurationSession.ResolveWellKnownGuid<ExchangeOrganizationalUnit>(WellKnownGuid.UsersWkGuid, NativeHelpers.DistinguishedNameFromCanonicalName(canonicalName));
			if (adobject == null)
			{
				ExchangeOrganizationalUnit exchangeOrganizationalUnit = topologyConfigurationSession.ResolveWellKnownGuid<ExchangeOrganizationalUnit>(WellKnownGuid.DomainControllersWkGuid, NativeHelpers.DistinguishedNameFromCanonicalName(canonicalName));
				QueryFilter filter = (exchangeOrganizationalUnit == null) ? null : new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, exchangeOrganizationalUnit.Id);
				ADOrganizationalUnit[] array = topologyConfigurationSession.Find<ADOrganizationalUnit>(null, QueryScope.OneLevel, filter, null, 1);
				if (array.Length <= 0)
				{
					return null;
				}
				adobject = array[0];
			}
			return adobject.Id;
		}

		public static string GetDefaultOUForRecipient(ADObjectId defaultScope)
		{
			ADObjectId defaultOUIdForRecipient = RecipientTaskHelper.GetDefaultOUIdForRecipient(defaultScope);
			if (defaultOUIdForRecipient != null)
			{
				return defaultOUIdForRecipient.ToCanonicalName();
			}
			return null;
		}

		internal static void ValidateUserIsGroupManager(ADObjectId userId, ADGroup group, Task.ErrorLoggerDelegate errorLogger, bool deepSearch = false, IRecipientSession session = null)
		{
			if (userId == null)
			{
				errorLogger(new OperationRequiresGroupManagerException(), ExchangeErrorCategory.Client, null);
			}
			RecipientTaskHelper.ValidateUserIsGroupManager(new ADObjectId[]
			{
				userId
			}, group, errorLogger, deepSearch, session);
		}

		internal static void ValidateUserIsGroupManager(ADObjectId[] userOrGroupIds, ADGroup group, Task.ErrorLoggerDelegate errorLogger, bool deepSearch = false, IRecipientSession session = null)
		{
			if (userOrGroupIds == null)
			{
				throw new ArgumentNullException("userOrGroupIds");
			}
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (errorLogger == null)
			{
				throw new ArgumentNullException("errorLogger");
			}
			if (group.ManagedBy == null)
			{
				errorLogger(new OperationRequiresGroupManagerException(), ExchangeErrorCategory.Client, null);
			}
			if (session == null && deepSearch)
			{
				throw new ArgumentNullException("session", "Parameter session should not be null when deepSearch is true.");
			}
			int i = 0;
			while (i < userOrGroupIds.Length)
			{
				ADObjectId adobjectId = userOrGroupIds[i];
				if (!group.ManagedBy.Contains(adobjectId))
				{
					if (deepSearch)
					{
						foreach (ADObjectId propertyValue in group.ManagedBy)
						{
							if (session.Find(null, QueryScope.SubTree, new AndFilter(new QueryFilter[]
							{
								new InChainFilter(ADGroupSchema.Members, adobjectId),
								new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, propertyValue)
							}), null, 1, Array<PropertyDefinition>.Empty).Length > 0)
							{
								return;
							}
						}
					}
					i++;
					continue;
				}
				return;
			}
			SharedConfiguration sharedConfiguration = SharedConfiguration.GetSharedConfiguration(group.OrganizationId);
			if (sharedConfiguration != null)
			{
				List<ADObjectId> list = new List<ADObjectId>(group.ManagedBy.Count);
				list.AddRange(sharedConfiguration.GetSharedRoleGroupIds(group.ManagedBy.ToArray()));
				foreach (ADObjectId item in userOrGroupIds)
				{
					if (list.Contains(item))
					{
						return;
					}
				}
			}
			errorLogger(new OperationRequiresGroupManagerException(), ExchangeErrorCategory.Client, null);
		}

		public static void ValidateEmailAddressErrorOut(IRecipientSession tenantCatalogSession, ProxyAddressCollection emailAddresses, ADRecipient self, Task.TaskVerboseLoggingDelegate writeVerbose, Task.ErrorLoggerReThrowDelegate writeError)
		{
			Dictionary<string, ADObjectId> dictionary = RecipientTaskHelper.ValidateEmailAddress(tenantCatalogSession, emailAddresses, self, writeVerbose);
			if (dictionary.Count > 0)
			{
				int num = 0;
				foreach (KeyValuePair<string, ADObjectId> keyValuePair in dictionary)
				{
					num++;
					writeError(new ProxyAddressExistsException(Strings.ErrorProxyAddressAlreadyExists(keyValuePair.Key, keyValuePair.Value.ToString())), ExchangeErrorCategory.Client, self.Identity, dictionary.Count == num);
				}
			}
		}

		public static void StripConflictEmailAddress(IRecipientSession tenantCatalogSession, ADRecipient self, Task.TaskVerboseLoggingDelegate writeVerbose, Task.ErrorLoggerReThrowDelegate writeError)
		{
			Dictionary<string, ADObjectId> dictionary = RecipientTaskHelper.ValidateEmailAddress(tenantCatalogSession, self.EmailAddresses, self, writeVerbose);
			if (dictionary.Count > 0)
			{
				ProxyAddressCollection proxyAddressCollection = new ProxyAddressCollection();
				foreach (ProxyAddress proxyAddress in self.EmailAddresses)
				{
					if (!dictionary.ContainsKey(proxyAddress.ToString()))
					{
						proxyAddressCollection.Add(proxyAddress);
					}
				}
				self.EmailAddresses = proxyAddressCollection;
			}
		}

		public static void StripInvalidSMTPAddress(IConfigurationSession cfgSession, ADRecipient self, ProvisioningCache provisioningCache, Task.TaskVerboseLoggingDelegate writeVerbose, Task.ErrorLoggerReThrowDelegate writeError)
		{
			IEnumerable<SmtpDomainWithSubdomains> acceptedDomains = RecipientTaskHelper.GetAcceptedDomains(cfgSession, provisioningCache, self.OrganizationId);
			ProxyAddressCollection proxyAddressCollection = new ProxyAddressCollection();
			foreach (ProxyAddress proxyAddress in self.EmailAddresses)
			{
				string text = null;
				SmtpProxyAddress smtpProxyAddress = proxyAddress as SmtpProxyAddress;
				if (smtpProxyAddress != null)
				{
					text = smtpProxyAddress.SmtpAddress;
				}
				else if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
				{
					if (proxyAddress.Prefix == ProxyAddressPrefix.UM)
					{
						EumProxyAddress value = (EumProxyAddress)proxyAddress;
						EumAddress eumAddress = (EumAddress)value;
						if (eumAddress.IsSipExtension)
						{
							text = eumAddress.Extension;
						}
					}
					else if (proxyAddress.Prefix == ProxyAddressPrefix.SIP)
					{
						text = proxyAddress.ValueString;
					}
				}
				if (text != null)
				{
					string domain = new SmtpAddress(text).Domain;
					if (domain != null && !RecipientTaskHelper.IsAcceptedDomain(acceptedDomains, domain))
					{
						continue;
					}
				}
				proxyAddressCollection.Add(proxyAddress);
			}
			self.EmailAddresses = proxyAddressCollection;
		}

		public static void ValidateSmtpAddress(IConfigurationSession cfgSession, ProxyAddressCollection emailAddresses, ADRecipient recipient, Task.ErrorLoggerDelegate writeError, ProvisioningCache provisioningCache)
		{
			RecipientTaskHelper.ValidateSmtpAddress(cfgSession, emailAddresses, recipient, writeError, provisioningCache, false);
		}

		public static void ValidateSmtpAddress(IConfigurationSession cfgSession, ProxyAddressCollection emailAddresses, ADRecipient recipient, Task.ErrorLoggerDelegate writeError, ProvisioningCache provisioningCache, bool ignoreValidationSkip)
		{
			if (cfgSession == null)
			{
				throw new ArgumentNullException("cfgSession");
			}
			if (emailAddresses == null)
			{
				throw new ArgumentNullException("emailAddresses");
			}
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (writeError == null)
			{
				throw new ArgumentException("writeError");
			}
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && !ignoreValidationSkip && !RecipientTaskHelper.SMTPAddressCheckWithAcceptedDomain(cfgSession, recipient.OrganizationId, writeError, provisioningCache) && (recipient.RecipientType == RecipientType.MailContact || recipient.RecipientType == RecipientType.MailUser))
			{
				return;
			}
			IEnumerable<SmtpDomainWithSubdomains> acceptedDomains = RecipientTaskHelper.GetAcceptedDomains(cfgSession, provisioningCache, recipient.OrganizationId);
			if (recipient is ADContact)
			{
				using (MultiValuedProperty<ProxyAddress>.Enumerator enumerator = emailAddresses.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ProxyAddress proxyAddress = enumerator.Current;
						SmtpProxyAddress smtpProxyAddress = proxyAddress as SmtpProxyAddress;
						if (smtpProxyAddress != null)
						{
							string domain = new SmtpAddress(smtpProxyAddress.SmtpAddress).Domain;
							if (RecipientTaskHelper.IsAcceptedDomain(acceptedDomains, domain))
							{
								writeError(new NotAcceptedDomainException(Strings.ErrorIsAcceptedDomain(domain)), ExchangeErrorCategory.Client, null);
							}
						}
					}
					return;
				}
			}
			foreach (ProxyAddress proxyAddress2 in emailAddresses)
			{
				string text = null;
				SmtpProxyAddress smtpProxyAddress2 = proxyAddress2 as SmtpProxyAddress;
				if (smtpProxyAddress2 != null)
				{
					text = smtpProxyAddress2.SmtpAddress;
				}
				else if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
				{
					if (proxyAddress2.Prefix == ProxyAddressPrefix.UM)
					{
						EumProxyAddress value = (EumProxyAddress)proxyAddress2;
						EumAddress eumAddress = (EumAddress)value;
						if (eumAddress.IsSipExtension)
						{
							text = eumAddress.Extension;
						}
					}
					else if (proxyAddress2.Prefix == ProxyAddressPrefix.SIP)
					{
						text = proxyAddress2.ValueString;
					}
				}
				if (text != null)
				{
					string domain2 = new SmtpAddress(text).Domain;
					if (domain2 != null && !RecipientTaskHelper.IsAcceptedDomain(acceptedDomains, domain2))
					{
						RecipientTaskHelper.ReportNotAcceptedDomainError(writeError, domain2);
					}
				}
			}
		}

		internal static bool IsOrgnizationalUnitInOrganization(IConfigurationSession cfgSession, OrganizationId organizationId, ExchangeOrganizationalUnit ou, Task.TaskVerboseLoggingDelegate writeVerbose, Task.ErrorLoggerDelegate writeError)
		{
			if (cfgSession == null)
			{
				throw new ArgumentNullException("cfgSession");
			}
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			if (ou == null)
			{
				throw new ArgumentNullException("ou");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			OrganizationId organization = RecipientTaskHelper.GetOrganization(cfgSession, ou, writeVerbose);
			if (!organizationId.Equals(organization))
			{
				writeError(new TaskInvalidOperationException(Strings.ErrorOuOutOfOrganization(ou.Identity.ToString())), ExchangeErrorCategory.Client, null);
			}
			return true;
		}

		private static OrganizationId GetOrganization(IConfigurationSession cfgSession, ExchangeOrganizationalUnit ou, Task.TaskVerboseLoggingDelegate verboseLogger)
		{
			OrganizationId result = OrganizationId.ForestWideOrgId;
			ADObjectId adobjectId = ou.Id;
			bool useConfigNC = cfgSession.UseConfigNC;
			cfgSession.UseConfigNC = false;
			verboseLogger(Strings.VerboseRecipientTaskHelperGetOrgnization(ou.Identity.ToString()));
			if (!adobjectId.IsDescendantOf(ADSession.GetHostedOrganizationsRoot(cfgSession.SessionSettings.GetAccountOrResourceForestFqdn())))
			{
				return result;
			}
			while (!adobjectId.DistinguishedName.StartsWith("DC="))
			{
				IEnumerable<ADOrganizationalUnit> objects = new OrganizationIdParameter(adobjectId.DistinguishedName).GetObjects<ADOrganizationalUnit>(null, cfgSession);
				using (IEnumerator<ADOrganizationalUnit> enumerator = objects.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						result = enumerator.Current.OrganizationId;
						break;
					}
					adobjectId = adobjectId.Parent;
				}
			}
			cfgSession.UseConfigNC = useConfigNC;
			return result;
		}

		internal static ExchangeOrganizationalUnit ResolveOrganizationalUnitInOrganization(OrganizationalUnitIdParameter ouIdParameter, IConfigurationSession cfgSession, OrganizationId organizationId, DataAccessHelper.CategorizedGetDataObjectDelegate getUniqueObject, ExchangeErrorCategory errorCategory, Task.TaskVerboseLoggingDelegate writeVerbose, Task.ErrorLoggerDelegate writeError)
		{
			if (ouIdParameter == null)
			{
				throw new ArgumentNullException("ouIdParameter");
			}
			if (cfgSession == null)
			{
				throw new ArgumentNullException("cfgSession");
			}
			if (getUniqueObject == null)
			{
				throw new ArgumentNullException("getUniqueObject");
			}
			if (writeVerbose == null)
			{
				throw new ArgumentNullException("writeVerbose");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			bool useGlobalCatalog = cfgSession.UseGlobalCatalog;
			bool useConfigNC = cfgSession.UseConfigNC;
			ExchangeOrganizationalUnit result;
			try
			{
				cfgSession.UseGlobalCatalog = true;
				cfgSession.UseConfigNC = false;
				ExchangeOrganizationalUnit exchangeOrganizationalUnit = (ExchangeOrganizationalUnit)getUniqueObject(ouIdParameter, cfgSession, (null == organizationId) ? null : organizationId.OrganizationalUnit, null, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(ouIdParameter.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(ouIdParameter.ToString())), errorCategory);
				if (null != organizationId)
				{
					RecipientTaskHelper.IsOrgnizationalUnitInOrganization(cfgSession, organizationId, exchangeOrganizationalUnit, writeVerbose, writeError);
				}
				result = exchangeOrganizationalUnit;
			}
			finally
			{
				cfgSession.UseGlobalCatalog = useGlobalCatalog;
				cfgSession.UseConfigNC = useConfigNC;
			}
			return result;
		}

		internal static ADObject ConvertRecipientToPresentationObject(ADRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			ADObject result;
			switch (recipient.RecipientType)
			{
			case RecipientType.Invalid:
				result = recipient;
				break;
			case RecipientType.User:
				result = new User((ADUser)recipient);
				break;
			case RecipientType.UserMailbox:
				result = new Mailbox((ADUser)recipient);
				break;
			case RecipientType.MailUser:
				result = new MailUser((ADUser)recipient);
				break;
			case RecipientType.Contact:
				result = new Contact((ADContact)recipient);
				break;
			case RecipientType.MailContact:
				result = new MailContact((ADContact)recipient);
				break;
			case RecipientType.Group:
				result = new WindowsGroup((ADGroup)recipient);
				break;
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
				result = new DistributionGroup((ADGroup)recipient);
				break;
			case RecipientType.DynamicDistributionGroup:
				result = new DynamicDistributionGroup((ADDynamicGroup)recipient);
				break;
			case RecipientType.PublicFolder:
				result = new MailPublicFolder((ADPublicFolder)recipient);
				break;
			case RecipientType.PublicDatabase:
				result = new PublicDatabasePresentationObject((ADPublicDatabase)recipient);
				break;
			case RecipientType.SystemAttendantMailbox:
				result = new SystemAttendantMailboxPresentationObject((ADSystemAttendantMailbox)recipient);
				break;
			case RecipientType.SystemMailbox:
				if (recipient is ADSystemMailbox)
				{
					result = new SystemMailboxPresentationObject((ADSystemMailbox)recipient);
				}
				else
				{
					result = new Mailbox((ADUser)recipient);
				}
				break;
			case RecipientType.MicrosoftExchange:
				result = new MicrosoftExchangeRecipientPresentationObject((ADMicrosoftExchangeRecipient)recipient);
				break;
			default:
				throw new NotSupportedException(Strings.ErrorCannotFormatRecipient((int)recipient.RecipientType));
			}
			return result;
		}

		public static IRecipientSession CreatePartitionOrRootOrgScopedGcSession(string domainController, ADObjectId referenceADObjectId)
		{
			ADSessionSettings sessionSettings = VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ServiceAccountForest.Enabled ? ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(referenceADObjectId) : ADSessionSettings.FromRootOrgScopeSet();
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 2470, "CreatePartitionOrRootOrgScopedGcSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RecipientTaskHelper.cs");
			if (!tenantOrRootOrgRecipientSession.IsReadConnectionAvailable())
			{
				tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 2478, "CreatePartitionOrRootOrgScopedGcSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RecipientTaskHelper.cs");
			}
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
			return tenantOrRootOrgRecipientSession;
		}

		internal static PartitionId ResolvePartitionId(AccountPartitionIdParameter accountPartition, Task.TaskErrorLoggingDelegate errorHandler)
		{
			if (accountPartition == null)
			{
				throw new ArgumentNullException("accountPartition");
			}
			PartitionId result = null;
			LocalizedString? localizedString;
			IEnumerable<AccountPartition> objects = accountPartition.GetObjects<AccountPartition>(null, DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.SessionSettingsFactory.Default.FromRootOrgScopeSet(), 2508, "ResolvePartitionId", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RecipientTaskHelper.cs"), null, out localizedString);
			Exception ex = null;
			using (IEnumerator<AccountPartition> enumerator = objects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					AccountPartition accountPartition2 = enumerator.Current;
					if (!accountPartition2.TryGetPartitionId(out result))
					{
						ex = new NotSupportedException(Strings.ErrorCorruptedPartition(accountPartition.ToString()));
					}
					else if (enumerator.MoveNext())
					{
						ex = new ManagementObjectAmbiguousException(Strings.ErrorManagementObjectAmbiguous(accountPartition.ToString()));
					}
				}
				else
				{
					ex = new ManagementObjectNotFoundException(localizedString ?? Strings.ErrorManagementObjectNotFound(accountPartition.ToString()));
				}
			}
			if (ex != null)
			{
				errorHandler(ex, (ErrorCategory)1000, null);
			}
			return result;
		}

		internal static ADUser ResetOldDefaultPlan(IRecipientSession session, ADObjectId mailboxPlan, ADObjectId organizationalRoot, Task.ErrorLoggerDelegate errorHandler)
		{
			ADUser aduser = null;
			QueryFilter queryFilter;
			if (organizationalRoot == null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.MailboxPlan),
					new ComparisonFilter(ComparisonOperator.Equal, MailboxPlanSchema.IsDefault, true),
					new NotFilter(new ExistsFilter(ADObjectSchema.OrganizationalUnitRoot))
				});
			}
			else
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.MailboxPlan),
					new ComparisonFilter(ComparisonOperator.Equal, MailboxPlanSchema.IsDefault, true),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, organizationalRoot)
				});
			}
			if (mailboxPlan != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, mailboxPlan)
				});
			}
			ADRecipient[] array = session.Find(null, QueryScope.SubTree, queryFilter, null, 3);
			if (array.Length > 1)
			{
				errorHandler(new ManagementObjectAmbiguousException(Strings.MultipleDefaultMailboxPlansFound(array[0].Identity.ToString(), session.Source)), ExchangeErrorCategory.Client, array[0].Identity);
			}
			else if (array.Length > 0)
			{
				aduser = (ADUser)array[0];
				aduser.IsDefault = false;
				session.Save(aduser);
			}
			return aduser;
		}

		public static MultiValuedProperty<ADObjectId> GetModeratedByAdObjectIdFromParameterID(IRecipientSession recipientSession, MultiValuedProperty<ModeratorIDParameter> recipientIdParameters, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject, ADRecipient moderatedObject, Task.ErrorLoggerDelegate writeError)
		{
			if (MultiValuedPropertyBase.IsNullOrEmpty(recipientIdParameters))
			{
				return null;
			}
			MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>();
			foreach (ModeratorIDParameter parameter in recipientIdParameters)
			{
				ADRecipient moderatedBy = RecipientTaskHelper.GetModeratedBy(parameter, recipientSession, getDataObject, writeError);
				RecipientTaskHelper.CheckRecipientInSameOrganizationWithDataObject(moderatedObject, moderatedBy, writeError);
				if (!multiValuedProperty.Contains(moderatedBy.Id))
				{
					multiValuedProperty.Add(moderatedBy.Id);
				}
			}
			return multiValuedProperty;
		}

		internal static ADRecipient GetModeratedBy(ModeratorIDParameter parameter, IRecipientSession recipientSession, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject, Task.ErrorLoggerDelegate writeError)
		{
			ADRecipient adrecipient = (ADRecipient)getDataObject(parameter, recipientSession, null, null, new LocalizedString?(Strings.ErrorRecipientNotFound(parameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(parameter.ToString())), ExchangeErrorCategory.Client);
			RecipientTaskHelper.ValidateModeratedBy(adrecipient, adrecipient.Identity.ToString(), writeError);
			return adrecipient;
		}

		public static void ValidateModeratedBy(ADRecipient recipient, string recipientId, Task.ErrorLoggerDelegate writeError)
		{
			if (!RecipientTaskHelper.IsAllowedModerator(recipient.RecipientTypeDetails))
			{
				writeError(new TaskInvalidOperationException(Strings.ErrorInvalidModerator(recipient.Identity.ToString())), ExchangeErrorCategory.Client, recipient.Identity);
			}
		}

		public static bool IsAllowedModerator(RecipientTypeDetails type)
		{
			return type == RecipientTypeDetails.UserMailbox || type == RecipientTypeDetails.LinkedMailbox || type == RecipientTypeDetails.SharedMailbox || type == RecipientTypeDetails.TeamMailbox || type == RecipientTypeDetails.GroupMailbox || type == RecipientTypeDetails.LegacyMailbox || type == RecipientTypeDetails.MailContact || type == RecipientTypeDetails.MailUser || type == (RecipientTypeDetails)((ulong)int.MinValue) || type == RecipientTypeDetails.RemoteTeamMailbox || type == RecipientTypeDetails.RemoteSharedMailbox;
		}

		public static void CheckRecipientInSameOrganizationWithDataObject(ADObject dataObject, ADObject recipient, Task.ErrorLoggerDelegate writeError)
		{
			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			if (!recipient.OrganizationId.Equals(dataObject.OrganizationId))
			{
				writeError(new TaskInvalidOperationException(Strings.ErrorRecipientNotInSameOrgWithDataObject(dataObject.Id.ToString(), dataObject.OrganizationId.ToString(), recipient.Id.ToString(), recipient.OrganizationId.ToString())), ExchangeErrorCategory.Client, dataObject.Identity);
			}
		}

		public static void CheckRecipientsInSameOrganizationWithDataObject(ADObject dataObject, IEnumerable<ADRecipient> recipients, Task.ErrorLoggerDelegate writeError)
		{
			if (recipients == null)
			{
				return;
			}
			foreach (ADRecipient recipient in recipients)
			{
				RecipientTaskHelper.CheckRecipientInSameOrganizationWithDataObject(dataObject, recipient, writeError);
			}
		}

		internal static RecipientType RecipientTypeDetailsToRecipientType(RecipientTypeDetails recipientTypeDetails)
		{
			RecipientType result = RecipientType.Invalid;
			if (recipientTypeDetails <= RecipientTypeDetails.NonUniversalGroup)
			{
				if (recipientTypeDetails <= RecipientTypeDetails.MailUniversalSecurityGroup)
				{
					if (recipientTypeDetails <= RecipientTypeDetails.EquipmentMailbox)
					{
						if (recipientTypeDetails <= RecipientTypeDetails.LegacyMailbox)
						{
							if (recipientTypeDetails <= RecipientTypeDetails.SharedMailbox)
							{
								if (recipientTypeDetails < RecipientTypeDetails.UserMailbox)
								{
									return result;
								}
								switch ((int)(recipientTypeDetails - RecipientTypeDetails.UserMailbox))
								{
								case 0:
								case 1:
								case 3:
									goto IL_2CE;
								case 2:
									return result;
								}
							}
							if (recipientTypeDetails != RecipientTypeDetails.LegacyMailbox)
							{
								return result;
							}
							goto IL_2CE;
						}
						else
						{
							if (recipientTypeDetails != RecipientTypeDetails.RoomMailbox && recipientTypeDetails != RecipientTypeDetails.EquipmentMailbox)
							{
								return result;
							}
							goto IL_2CE;
						}
					}
					else if (recipientTypeDetails <= RecipientTypeDetails.MailUser)
					{
						if (recipientTypeDetails != RecipientTypeDetails.MailContact)
						{
							if (recipientTypeDetails != RecipientTypeDetails.MailUser)
							{
								return result;
							}
							goto IL_2D6;
						}
					}
					else
					{
						if (recipientTypeDetails == RecipientTypeDetails.MailUniversalDistributionGroup)
						{
							goto IL_2DA;
						}
						if (recipientTypeDetails == RecipientTypeDetails.MailNonUniversalGroup)
						{
							return RecipientType.MailNonUniversalGroup;
						}
						if (recipientTypeDetails != RecipientTypeDetails.MailUniversalSecurityGroup)
						{
							return result;
						}
						return RecipientType.MailUniversalSecurityGroup;
					}
				}
				else if (recipientTypeDetails <= RecipientTypeDetails.MailForestContact)
				{
					if (recipientTypeDetails <= RecipientTypeDetails.PublicFolder)
					{
						if (recipientTypeDetails == RecipientTypeDetails.DynamicDistributionGroup)
						{
							return RecipientType.DynamicDistributionGroup;
						}
						if (recipientTypeDetails != RecipientTypeDetails.PublicFolder)
						{
							return result;
						}
						return RecipientType.PublicFolder;
					}
					else
					{
						if (recipientTypeDetails == RecipientTypeDetails.SystemAttendantMailbox)
						{
							return RecipientType.SystemAttendantMailbox;
						}
						if (recipientTypeDetails == RecipientTypeDetails.SystemMailbox)
						{
							return RecipientType.SystemMailbox;
						}
						if (recipientTypeDetails != RecipientTypeDetails.MailForestContact)
						{
							return result;
						}
					}
				}
				else if (recipientTypeDetails <= RecipientTypeDetails.Contact)
				{
					if (recipientTypeDetails == RecipientTypeDetails.User)
					{
						goto IL_2FB;
					}
					if (recipientTypeDetails != RecipientTypeDetails.Contact)
					{
						return result;
					}
					return RecipientType.Contact;
				}
				else
				{
					if (recipientTypeDetails != RecipientTypeDetails.UniversalDistributionGroup && recipientTypeDetails != RecipientTypeDetails.UniversalSecurityGroup && recipientTypeDetails != RecipientTypeDetails.NonUniversalGroup)
					{
						return result;
					}
					goto IL_303;
				}
				return RecipientType.MailContact;
			}
			if (recipientTypeDetails <= RecipientTypeDetails.RemoteRoomMailbox)
			{
				if (recipientTypeDetails <= RecipientTypeDetails.LinkedUser)
				{
					if (recipientTypeDetails <= RecipientTypeDetails.MicrosoftExchange)
					{
						if (recipientTypeDetails == RecipientTypeDetails.DisabledUser)
						{
							goto IL_2FB;
						}
						if (recipientTypeDetails != RecipientTypeDetails.MicrosoftExchange)
						{
							return result;
						}
						return RecipientType.MicrosoftExchange;
					}
					else if (recipientTypeDetails != RecipientTypeDetails.ArbitrationMailbox && recipientTypeDetails != RecipientTypeDetails.MailboxPlan)
					{
						if (recipientTypeDetails != RecipientTypeDetails.LinkedUser)
						{
							return result;
						}
						goto IL_2FB;
					}
				}
				else if (recipientTypeDetails <= RecipientTypeDetails.DiscoveryMailbox)
				{
					if (recipientTypeDetails == RecipientTypeDetails.RoomList)
					{
						goto IL_2DA;
					}
					if (recipientTypeDetails != RecipientTypeDetails.DiscoveryMailbox)
					{
						return result;
					}
				}
				else
				{
					if (recipientTypeDetails == RecipientTypeDetails.RoleGroup)
					{
						goto IL_303;
					}
					if (recipientTypeDetails != (RecipientTypeDetails)((ulong)-2147483648) && recipientTypeDetails != RecipientTypeDetails.RemoteRoomMailbox)
					{
						return result;
					}
					goto IL_2D6;
				}
			}
			else if (recipientTypeDetails <= RecipientTypeDetails.RemoteTeamMailbox)
			{
				if (recipientTypeDetails <= RecipientTypeDetails.RemoteSharedMailbox)
				{
					if (recipientTypeDetails != RecipientTypeDetails.RemoteEquipmentMailbox && recipientTypeDetails != RecipientTypeDetails.RemoteSharedMailbox)
					{
						return result;
					}
					goto IL_2D6;
				}
				else if (recipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox && recipientTypeDetails != RecipientTypeDetails.TeamMailbox)
				{
					if (recipientTypeDetails != RecipientTypeDetails.RemoteTeamMailbox)
					{
						return result;
					}
					goto IL_2D6;
				}
			}
			else if (recipientTypeDetails <= RecipientTypeDetails.GroupMailbox)
			{
				if (recipientTypeDetails != RecipientTypeDetails.MonitoringMailbox && recipientTypeDetails != RecipientTypeDetails.GroupMailbox)
				{
					return result;
				}
			}
			else if (recipientTypeDetails != RecipientTypeDetails.LinkedRoomMailbox && recipientTypeDetails != RecipientTypeDetails.AuditLogMailbox)
			{
				if (recipientTypeDetails != RecipientTypeDetails.AllUniqueRecipientTypes)
				{
					return result;
				}
				return RecipientType.Invalid;
			}
			IL_2CE:
			return RecipientType.UserMailbox;
			IL_2D6:
			return RecipientType.MailUser;
			IL_2DA:
			return RecipientType.MailUniversalDistributionGroup;
			IL_2FB:
			return RecipientType.User;
			IL_303:
			result = RecipientType.Group;
			return result;
		}

		internal static void ReportNotAcceptedDomainError(WriteErrorDelegate errorDelegate, string domainName)
		{
			if (errorDelegate == null)
			{
				throw new ArgumentNullException("errorDelegate");
			}
			if (string.IsNullOrEmpty(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			domainName = string.Empty;
			errorDelegate(new NotAcceptedDomainException(Strings.ErrorNotAcceptedDomain(domainName)), ExchangeErrorCategory.Client);
		}

		internal static void ReportNotAcceptedDomainError(Task.ErrorLoggerDelegate errorDelegate, string domainName)
		{
			if (errorDelegate == null)
			{
				throw new ArgumentNullException("errorDelegate");
			}
			if (string.IsNullOrEmpty(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			domainName = string.Empty;
			errorDelegate(new NotAcceptedDomainException(Strings.ErrorNotAcceptedDomain(domainName)), ExchangeErrorCategory.Client, null);
		}

		public static IConfigurationSession GetTenantLocalConfigSession(OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId, ADObjectId rootOrgContainerId)
		{
			return RecipientTaskHelper.GetTenantLocalConfigSession(currentOrganizationId, executingUserOrganizationId, rootOrgContainerId, true, null, null);
		}

		public static IConfigurationSession GetTenantLocalConfigSession(OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId, ADObjectId rootOrgContainerId, bool readOnly, string domainControllerName, NetworkCredential cred)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerId, currentOrganizationId, executingUserOrganizationId, false);
			adsessionSettings.IsSharedConfigChecked = true;
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(domainControllerName, readOnly, ConsistencyMode.IgnoreInvalid, cred, adsessionSettings, 2961, "GetTenantLocalConfigSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RecipientTaskHelper.cs");
		}

		public static IRecipientSession GetTenantLocalRecipientSession(OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId, ADObjectId rootOrgContainerId)
		{
			return RecipientTaskHelper.GetTenantLocalRecipientSession(currentOrganizationId, executingUserOrganizationId, rootOrgContainerId, null, null);
		}

		public static IRecipientSession GetTenantLocalRecipientSession(OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId, ADObjectId rootOrgContainerId, string domainControllerName, NetworkCredential cred)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerId, currentOrganizationId, executingUserOrganizationId, false);
			adsessionSettings.IsSharedConfigChecked = true;
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainControllerName, true, ConsistencyMode.IgnoreInvalid, cred, adsessionSettings, 2992, "GetTenantLocalRecipientSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RecipientTaskHelper.cs");
		}

		public static RoleAssignmentPolicy FindDefaultRoleAssignmentPolicy(IConfigurationSession session, Task.ErrorLoggerDelegate errorLogger, LocalizedString errorDefaultRoleAssignmentPolicyNotUnique, LocalizedString errorDefaultRoleAssignmentPolicyNotFound)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, RoleAssignmentPolicySchema.IsDefault, true);
			RoleAssignmentPolicy[] array = session.Find<RoleAssignmentPolicy>(null, QueryScope.SubTree, filter, null, 2);
			if (array.Length == 1)
			{
				return array[0];
			}
			if (array.Length > 1)
			{
				errorLogger(new TaskInvalidOperationException(errorDefaultRoleAssignmentPolicyNotUnique), ExchangeErrorCategory.Client, null);
			}
			array = session.Find<RoleAssignmentPolicy>(null, QueryScope.SubTree, null, null, 1);
			if (array.Length == 0)
			{
				return null;
			}
			errorLogger(new TaskInvalidOperationException(errorDefaultRoleAssignmentPolicyNotFound), ExchangeErrorCategory.Client, null);
			return null;
		}

		public static void SetExceptionErrorCategory(Exception ex, ExchangeErrorCategory category)
		{
			ex.Data["ExchangeErrorCategory"] = category;
		}

		public static ExchangeErrorCategory GetExceptionErrorCategory(Exception ex)
		{
			if (ex.Data != null && ex.Data.Contains("ExchangeErrorCategory"))
			{
				return (ExchangeErrorCategory)ex.Data["ExchangeErrorCategory"];
			}
			return (ExchangeErrorCategory)0;
		}

		public static RecipientTypeDetails GetAcceptedRecipientTypes()
		{
			return RecipientTypeDetails.UserMailbox | RecipientTypeDetails.LinkedMailbox | RecipientTypeDetails.SharedMailbox | RecipientTypeDetails.LegacyMailbox | RecipientTypeDetails.RoomMailbox | RecipientTypeDetails.EquipmentMailbox | RecipientTypeDetails.MailContact | RecipientTypeDetails.MailUser | RecipientTypeDetails.MailUniversalDistributionGroup | RecipientTypeDetails.MailNonUniversalGroup | RecipientTypeDetails.MailUniversalSecurityGroup | RecipientTypeDetails.TeamMailbox | RecipientTypeDetails.LinkedRoomMailbox;
		}

		internal static void CreateSoftDeletedObjectsContainerIfNecessary(ADObjectId containerId, string domainController)
		{
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(domainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsObjectId(containerId), 3099, "CreateSoftDeletedObjectsContainerIfNecessary", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RecipientTaskHelper.cs");
			tenantConfigurationSession.UseConfigNC = false;
			if (tenantConfigurationSession.Read<ADOrganizationalUnit>(containerId) == null)
			{
				ADOrganizationalUnit adorganizationalUnit = new ADOrganizationalUnit();
				adorganizationalUnit.SetId(containerId);
				try
				{
					tenantConfigurationSession.Save(adorganizationalUnit);
				}
				catch (ADObjectAlreadyExistsException)
				{
				}
			}
		}

		internal static void RemoveEmptyValueFromEmailAddresses(ADRecipient dataObject)
		{
			if (dataObject.IsChanged(ADRecipientSchema.EmailAddresses))
			{
				ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)dataObject[ADRecipientSchema.EmailAddresses];
				if (proxyAddressCollection != null && proxyAddressCollection.Count > 0)
				{
					dataObject[ADRecipientSchema.EmailAddresses] = (from x in proxyAddressCollection
					where !string.IsNullOrWhiteSpace(x.AddressString)
					select x).ToArray<ProxyAddress>();
				}
			}
		}

		public static bool IsOrganizationInPilot(IConfigurationSession session, OrganizationId organizationId)
		{
			ExchangeConfigurationUnit exchangeConfigUnit = RecipientTaskHelper.GetExchangeConfigUnit(session, organizationId);
			return exchangeConfigUnit != null && exchangeConfigUnit.IsPilotingOrganization;
		}

		public static ExchangeConfigurationUnit GetExchangeConfigUnit(IConfigurationSession session, OrganizationId organizationId)
		{
			return ProvisioningCache.Instance.TryAddAndGetOrganizationData<ExchangeConfigurationUnit>(CannedProvisioningCacheKeys.OrganizationCUContainer, organizationId, () => session.Read<ExchangeConfigurationUnit>(organizationId.ConfigurationUnit));
		}

		internal static void UpgradeArchiveQuotaOnArchiveAddOnSKU(ADUser user, MultiValuedProperty<Capability> capabilities)
		{
			if (user != null && capabilities != null && capabilities.Contains(Capability.BPOS_S_ArchiveAddOn))
			{
				if (user.ArchiveQuota < RecipientTaskHelper.ArchiveAddOnQuota)
				{
					user.ArchiveQuota = RecipientTaskHelper.ArchiveAddOnQuota;
				}
				if (user.ArchiveWarningQuota < RecipientTaskHelper.ArchiveAddOnWarningQuota)
				{
					user.ArchiveWarningQuota = RecipientTaskHelper.ArchiveAddOnWarningQuota;
				}
			}
		}

		private const int MaxSamAccountNameLengthForUser = 20;

		private const int MaxSamAccountNameLengthForGroup = 256;

		private const int MaxUserPrinciplNameLength = 1024;

		private const int EmptySuffix = 0;

		private const string ExchangeErrorCategoryId = "ExchangeErrorCategory";

		internal static readonly Unlimited<ByteQuantifiedSize> ArchiveAddOnQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(100UL));

		internal static readonly Unlimited<ByteQuantifiedSize> ArchiveAddOnWarningQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(90UL));
	}
}
