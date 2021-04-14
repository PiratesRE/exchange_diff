using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Ehf;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfProvisioningService : IDisposeTrackable, IDisposable
	{
		public EhfProvisioningService(EhfTargetServerConfig config)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.provisioningService = new EhfProvisioningService.ProvisioningServiceWrapper(config);
			this.managementService = new EhfProvisioningService.ManagementServiceWrapper(config);
			this.adminSyncService = new EhfProvisioningService.AdminSyncServiceWrapper(config);
		}

		public EhfProvisioningService(IProvisioningService provisioningService, IManagementService managementService, IAdminSyncService adminSyncService)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.provisioningService = new EhfProvisioningService.ProvisioningServiceWrapper(provisioningService);
			this.managementService = new EhfProvisioningService.ManagementServiceWrapper(managementService);
			this.adminSyncService = new EhfProvisioningService.AdminSyncServiceWrapper(adminSyncService);
		}

		public static string GetResponseTargetValuesString(ResponseInfo response)
		{
			string result = "none";
			if (response.TargetValue != null && response.TargetValue.Length > 0)
			{
				result = string.Join(", ", response.TargetValue);
			}
			return result;
		}

		public static EhfProvisioningService.MessageSecurityExceptionReason DecodeMessageSecurityException(MessageSecurityException exception)
		{
			FaultException ex = exception.InnerException as FaultException;
			if (ex != null && ex.Code != null && ex.Code.SubCode != null && string.Equals(ex.Code.Name, "Sender", StringComparison.OrdinalIgnoreCase))
			{
				if (string.Equals(ex.Code.SubCode.Name, "InvalidSecurity", StringComparison.OrdinalIgnoreCase))
				{
					return EhfProvisioningService.MessageSecurityExceptionReason.DatabaseFailure;
				}
				if (string.Equals(ex.Code.SubCode.Name, "InvalidSecurityToken", StringComparison.OrdinalIgnoreCase))
				{
					return EhfProvisioningService.MessageSecurityExceptionReason.InvalidCredentials;
				}
			}
			return EhfProvisioningService.MessageSecurityExceptionReason.Other;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<EhfProvisioningService>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public CompanyResponseInfoSet CreateCompanies(IList<EhfCompanyItem> companies)
		{
			if (companies == null || companies.Count == 0)
			{
				throw new ArgumentNullException("companies");
			}
			Company[] array = new Company[companies.Count];
			for (int i = 0; i < companies.Count; i++)
			{
				array[i] = companies[i].Company;
				array[i].Settings = companies[i].SettingsForCreate;
				if (array[i].CompanyGuid == null || array[i].CompanyGuid.Value == Guid.Empty)
				{
					throw new ArgumentException("Attempted to create a company without setting its GUID", "companies");
				}
			}
			CompanyResponseInfoSet companyResponseInfoSet = this.provisioningService.CreateCompanies(array);
			EhfProvisioningService.ValidateResponseSetForCompanies(companyResponseInfoSet, "CreateCompanies", companies);
			return companyResponseInfoSet;
		}

		public CompanyResponseInfoSet UpdateCompanies(IList<EhfCompanyItem> companies)
		{
			if (companies == null || companies.Count == 0)
			{
				throw new ArgumentNullException("companies");
			}
			CompanyConfigurationSettings[] array = new CompanyConfigurationSettings[companies.Count];
			for (int i = 0; i < companies.Count; i++)
			{
				array[i] = companies[i].Settings;
				if (array[i].CompanyId == 0)
				{
					throw new ArgumentException("Attempted to update a company without setting its CompanyId", "companies");
				}
			}
			CompanyResponseInfoSet companyResponseInfoSet = this.managementService.UpdateCompanySettings(array);
			EhfProvisioningService.ValidateResponseSetForCompanies(companyResponseInfoSet, "UpdateCompanies", companies);
			return companyResponseInfoSet;
		}

		public CompanyResponseInfo UpdateReseller(int resellerId, IPAddress[] inboundIPs, IPAddress[] outboundIPs)
		{
			if (inboundIPs == null && outboundIPs == null)
			{
				throw new ArgumentException("inboundIPs and outboundIPs can't both be null");
			}
			CompanyConfigurationSettings companyConfigurationSettings = new CompanyConfigurationSettings();
			companyConfigurationSettings.CompanyId = resellerId;
			if (inboundIPs != null)
			{
				SmtpProfile smtpProfile = new SmtpProfile();
				smtpProfile.ProfileName = "InboundSmtpProfile";
				smtpProfile.IPList = new SmtpProfileEntry[inboundIPs.Length];
				for (int i = 0; i < inboundIPs.Length; i++)
				{
					smtpProfile.IPList[i] = new SmtpProfileEntry();
					smtpProfile.IPList[i].IP = inboundIPs[i].ToString();
					smtpProfile.IPList[i].Priority = 1;
				}
				companyConfigurationSettings.InboundIPList = new InboundIPListConfig();
				companyConfigurationSettings.InboundIPList.IPList = new SmtpProfile[]
				{
					smtpProfile
				};
				companyConfigurationSettings.InboundIPList.TargetAction = TargetAction.Set;
			}
			if (outboundIPs != null)
			{
				IPListConfig iplistConfig = new IPListConfig();
				iplistConfig.IPList = new string[outboundIPs.Length];
				for (int j = 0; j < outboundIPs.Length; j++)
				{
					iplistConfig.IPList[j] = outboundIPs[j].ToString();
				}
				companyConfigurationSettings.OutboundIPList = iplistConfig;
				companyConfigurationSettings.OutboundIPList.TargetAction = TargetAction.Set;
			}
			CompanyResponseInfoSet companyResponseInfoSet = this.managementService.UpdateCompanySettings(new CompanyConfigurationSettings[]
			{
				companyConfigurationSettings
			});
			EhfProvisioningService.ValidateResponseSetForReseller(companyResponseInfoSet, "UpdateReseller", resellerId);
			return companyResponseInfoSet.ResponseInfo[0];
		}

		public CompanyResponseInfoSet DisableCompanies(IList<EhfCompanyItem> companies)
		{
			int[] companyIds = EhfProvisioningService.GetCompanyIds(companies);
			CompanyResponseInfoSet companyResponseInfoSet = this.managementService.DisableCompanies(companyIds);
			EhfProvisioningService.ValidateResponseSetForCompanies(companyResponseInfoSet, "DisableCompanies", companies);
			return companyResponseInfoSet;
		}

		public CompanyResponseInfoSet DeleteCompanies(IList<EhfCompanyItem> companies)
		{
			int[] companyIds = EhfProvisioningService.GetCompanyIds(companies);
			CompanyResponseInfoSet companyResponseInfoSet = this.provisioningService.DeleteCompanies(companyIds);
			EhfProvisioningService.ValidateResponseSetForCompanies(companyResponseInfoSet, "DeleteCompanies", companies);
			return companyResponseInfoSet;
		}

		public FailedAdminAccounts SyncAdminAccounts(CompanyAdministrators companyAdministrators, out FaultException syncAdminAccountsHandledException)
		{
			EhfProvisioningService.ValidateBeforeSync(companyAdministrators);
			syncAdminAccountsHandledException = null;
			FailedAdminAccounts result;
			try
			{
				FailedAdminAccounts failedAdminAccounts = this.adminSyncService.SyncAdminAccounts(companyAdministrators);
				EhfProvisioningService.ValidateResponseForSyncAdminAccounts(failedAdminAccounts, companyAdministrators, "SyncAdminAccounts");
				result = failedAdminAccounts;
			}
			catch (FaultException<InvalidCompanyFault> faultException)
			{
				syncAdminAccountsHandledException = faultException;
				result = null;
			}
			catch (FaultException<InternalFault> faultException2)
			{
				syncAdminAccountsHandledException = faultException2;
				result = null;
			}
			return result;
		}

		public Dictionary<string, ErrorInfo> SyncGroupUsers(int companyId, Guid groupGuid, string[] users, out FaultException syncGroupsHandledException)
		{
			if (companyId == 0)
			{
				throw new ArgumentException("Attempted to sync group without the companyId", "companyId");
			}
			if (groupGuid == Guid.Empty)
			{
				throw new ArgumentException("groupGuid");
			}
			syncGroupsHandledException = null;
			Dictionary<string, ErrorInfo> result;
			try
			{
				Dictionary<string, ErrorInfo> dictionary = this.adminSyncService.SyncGroupUsers(companyId, groupGuid, users);
				if (dictionary != null)
				{
					if (dictionary.Count == 0)
					{
						EhfProvisioningService.HandleContractViolation("SyncGroupUsers", "A failed response from {0} should contain atleast one failed entry", new object[]
						{
							"SyncGroupUsers"
						});
					}
					string responseDetails = string.Format(CultureInfo.InvariantCulture, "OperationName : {0} CompanyId : {1} GroupGuid : {2}", new object[]
					{
						"SyncGroupUsers",
						companyId,
						groupGuid
					});
					EhfProvisioningService.ValidateAdminSyncResponseSet<string, ErrorInfo>(dictionary, "SyncGroupUsers", responseDetails, new HashSet<string>(users));
				}
				result = dictionary;
			}
			catch (FaultException<InvalidCompanyFault> faultException)
			{
				syncGroupsHandledException = faultException;
				result = null;
			}
			catch (FaultException<InternalFault> faultException2)
			{
				syncGroupsHandledException = faultException2;
				result = null;
			}
			catch (FaultException<InvalidGroupFault> faultException3)
			{
				if (faultException3.Detail.Code == InvalidGroupCode.GroupDoesNotExist && users.Length != 0)
				{
					string errorMessage = string.Format(CultureInfo.InvariantCulture, "SyncGroupUsers() operation returned GroupDoesNotExist fault while syncing partner group <{0}>:<{1}>. EHF is expected to create the group if it does not exist and if the group is non-empty.", new object[]
					{
						groupGuid,
						companyId
					});
					throw new EhfProvisioningService.ContractViolationException("SyncGroupUsers", errorMessage, faultException3);
				}
				syncGroupsHandledException = faultException3;
				result = null;
			}
			return result;
		}

		public Dictionary<Guid, RemoveGroupErrorInfo> RemoveGroups(Guid[] groups, out FaultException removeGroupsException)
		{
			if (groups == null)
			{
				throw new ArgumentNullException("groups");
			}
			if (groups.Length == 0)
			{
				throw new ArgumentException("groups");
			}
			removeGroupsException = null;
			Dictionary<Guid, RemoveGroupErrorInfo> result;
			try
			{
				Dictionary<Guid, RemoveGroupErrorInfo> dictionary = this.adminSyncService.RemoveGroups(groups);
				if (dictionary != null)
				{
					if (dictionary.Count == 0)
					{
						EhfProvisioningService.HandleContractViolation("RemoveGroups", "A failed response from {0} should contain atleast one failed entry", new object[]
						{
							"RemoveGroups"
						});
					}
					EhfProvisioningService.ValidateAdminSyncResponseSet<Guid, RemoveGroupErrorInfo>(dictionary, "RemoveGroups", string.Empty, new HashSet<Guid>(groups));
				}
				result = dictionary;
			}
			catch (FaultException<InternalFault> faultException)
			{
				removeGroupsException = faultException;
				result = null;
			}
			return result;
		}

		public bool TryGetCompanyByGuid(Guid companyGuid, out Company company)
		{
			company = null;
			try
			{
				company = this.managementService.GetCompanyByGuid(companyGuid);
			}
			catch (FaultException<ServiceFault> faultException)
			{
				ServiceFault detail = faultException.Detail;
				if (detail.Id == FaultId.CompanyDoesNotExist)
				{
					return false;
				}
				throw;
			}
			if (company == null)
			{
				EhfProvisioningService.HandleContractViolation("GetCompanyByGuid", "GetCompanyByGuid({0}) returned null", new object[]
				{
					companyGuid
				});
			}
			if (company.CompanyGuid == null || !company.CompanyGuid.Value.Equals(companyGuid))
			{
				string operationName = "GetCompanyByGuid";
				string violationFormat = "GetCompanyByGuid({0}) returned a company with GUID <{1}>";
				object[] array = new object[2];
				array[0] = companyGuid;
				object[] array2 = array;
				int num = 1;
				Guid? companyGuid2 = company.CompanyGuid;
				array2[num] = ((companyGuid2 != null) ? companyGuid2.GetValueOrDefault() : "null");
				EhfProvisioningService.HandleContractViolation(operationName, violationFormat, array);
			}
			if (company.CompanyId == 0)
			{
				EhfProvisioningService.HandleContractViolation("GetCompanyByGuid", "GetCompanyByGuid({0}) returned a company with invalid CompanyId <{1}>", new object[]
				{
					companyGuid,
					company.CompanyId
				});
			}
			return true;
		}

		public Company GetReseller(int resellerId)
		{
			Company company = this.GetCompany(resellerId, "GetReseller");
			if (company == null)
			{
				EhfProvisioningService.HandleContractViolation("GetReseller", "GetCompanyById({0}) returned null", new object[]
				{
					resellerId
				});
			}
			return company;
		}

		public Company GetCompany(int companyId)
		{
			return this.GetCompany(companyId, "GetCompany");
		}

		public DomainResponseInfoSet CreateDomains(IList<EhfDomainItem> domains)
		{
			Domain[] ehfDomains = EhfProvisioningService.GetEhfDomains(domains);
			DomainResponseInfoSet domainResponseInfoSet = this.provisioningService.AddDomains(ehfDomains);
			EhfProvisioningService.ValidateResponseSetForDomains<EhfDomainItem>(domainResponseInfoSet, "CreateDomains", domains);
			return domainResponseInfoSet;
		}

		public DomainResponseInfoSet DisableDomains(IList<EhfDomainItem> domains)
		{
			Domain[] ehfDomains = EhfProvisioningService.GetEhfDomains(domains);
			DomainResponseInfoSet domainResponseInfoSet = this.managementService.DisableDomains(ehfDomains);
			EhfProvisioningService.ValidateResponseSetForDomains<EhfDomainItem>(domainResponseInfoSet, "DisableDomains", domains);
			return domainResponseInfoSet;
		}

		public DomainResponseInfoSet DeleteDomains(IList<EhfDomainItem> domains)
		{
			Domain[] ehfDomains = EhfProvisioningService.GetEhfDomains(domains);
			DomainResponseInfoSet domainResponseInfoSet = this.provisioningService.DeleteDomains(ehfDomains);
			EhfProvisioningService.ValidateResponseSetForDomains<EhfDomainItem>(domainResponseInfoSet, "DeleteDomains", domains);
			return domainResponseInfoSet;
		}

		public DomainResponseInfoSet UpdateDomainSettingsByGuids(IList<EhfDomainItemVersion2> domains)
		{
			if (domains == null || domains.Count == 0)
			{
				throw new ArgumentNullException("domains");
			}
			DomainConfigurationSettings[] array = new DomainConfigurationSettings[domains.Count];
			for (int i = 0; i < domains.Count; i++)
			{
				array[i] = domains[i].Domain.Settings;
				if (array[i].DomainGuid == null || array[i].DomainGuid.Value.Equals(Guid.Empty))
				{
					throw new ArgumentException("Attempted to update a domain without setting its DomainGuid", "domains");
				}
			}
			DomainResponseInfoSet domainResponseInfoSet = this.managementService.UpdateDomainSettingsByGuids(array);
			EhfProvisioningService.ValidateResponseSetForDomains<EhfDomainItemVersion2>(domainResponseInfoSet, "UpdateDomainSettingsByGuids", domains);
			return domainResponseInfoSet;
		}

		public Domain[] GetAllDomains(int companyId)
		{
			Domain[] allDomains = this.managementService.GetAllDomains(companyId);
			if (allDomains == null)
			{
				EhfProvisioningService.HandleContractViolation("GetAllDomains", "GetAllDomains({0}) returned null", new object[]
				{
					companyId
				});
			}
			foreach (Domain domain in allDomains)
			{
				if (domain.DomainGuid == null)
				{
					EhfProvisioningService.HandleContractViolation("GetAllDomains", "Domain without DomainGuid was returned for domain {0} for companyID {1}", new object[]
					{
						domain.Name,
						companyId
					});
				}
				if (string.IsNullOrEmpty(domain.Name))
				{
					EhfProvisioningService.HandleContractViolation("GetAllDomains", "Domain with domain name set to null or empty was returned for domain with GUID {0} for companyID {1}", new object[]
					{
						domain.DomainGuid,
						companyId
					});
				}
				if (domain.CompanyId != companyId)
				{
					EhfProvisioningService.HandleContractViolation("GetAllDomains", "Domain {0} contains CompanyId {1} that does not match expected CompanyId {2}", new object[]
					{
						domain.Name,
						domain.CompanyId,
						companyId
					});
				}
			}
			return allDomains;
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			if (this.provisioningService != null)
			{
				this.provisioningService.Dispose();
				this.provisioningService = null;
			}
			if (this.managementService != null)
			{
				this.managementService.Dispose();
				this.managementService = null;
			}
			if (this.adminSyncService != null)
			{
				this.adminSyncService.Dispose();
				this.adminSyncService = null;
			}
		}

		private static void HandleContractViolation(string operationName, string violationFormat, params object[] args)
		{
			string errorMessage = string.Format(CultureInfo.InvariantCulture, violationFormat, args);
			throw new EhfProvisioningService.ContractViolationException(operationName, errorMessage);
		}

		private static void ValidateCompanyResponseSetSize(CompanyResponseInfoSet responseSet, string operationName, int numExpectedResponses)
		{
			if (responseSet == null)
			{
				EhfProvisioningService.HandleContractViolation(operationName, "Returned CompanyResponseInfoSet value is null", new object[0]);
			}
			if (responseSet.ResponseInfo == null)
			{
				EhfProvisioningService.HandleContractViolation(operationName, "Returned CompanyResponseInfoSet.ResponseInfo value is null", new object[0]);
			}
			if (responseSet.ResponseInfo.Length != numExpectedResponses)
			{
				EhfProvisioningService.HandleContractViolation(operationName, "CompanyResponseInfoSet.ResponseInfo length {0} does not match expected value {1}", new object[]
				{
					responseSet.ResponseInfo.Length,
					numExpectedResponses
				});
			}
		}

		private static void ValidateResponseSetForCompanies(CompanyResponseInfoSet responseSet, string operationName, IList<EhfCompanyItem> companies)
		{
			EhfProvisioningService.ValidateCompanyResponseSetSize(responseSet, operationName, companies.Count);
			for (int i = 0; i < responseSet.ResponseInfo.Length; i++)
			{
				CompanyResponseInfo companyResponseInfo = responseSet.ResponseInfo[i];
				if (companyResponseInfo.Status == ResponseStatus.Success)
				{
					if (companyResponseInfo.CompanyId == 0)
					{
						EhfProvisioningService.HandleContractViolation(operationName, "CompanyResponseInfo at offset {0} contains invalid CompanyId value {1}; Successful company response must be accompanied by valid CompanyId", new object[]
						{
							i,
							companyResponseInfo.CompanyId
						});
					}
					Guid companyGuid = companies[i].GetCompanyGuid();
					if (companyResponseInfo.CompanyGuid == null || !companyResponseInfo.CompanyGuid.Value.Equals(companyGuid))
					{
						string violationFormat = "CompanyResponseInfo at offset {0} contains GUID <{1}> that does not match expected GUID <{2}>";
						object[] array = new object[3];
						array[0] = i;
						object[] array2 = array;
						int num = 1;
						Guid? companyGuid2 = companyResponseInfo.CompanyGuid;
						array2[num] = ((companyGuid2 != null) ? companyGuid2.GetValueOrDefault() : "null");
						array[2] = companyGuid;
						EhfProvisioningService.HandleContractViolation(operationName, violationFormat, array);
					}
				}
				else if (companyResponseInfo.Fault == null)
				{
					EhfProvisioningService.HandleContractViolation(operationName, "Unsuccessful CompanyResponseInfo at offset {0} does not contain Fault", new object[]
					{
						i
					});
				}
				else if (companyResponseInfo.Fault.Id == FaultId.CompanyExistsUnderThisReseller)
				{
					if (companyResponseInfo.CompanyId == 0)
					{
						EhfProvisioningService.HandleContractViolation(operationName, "CompanyResponseInfo at offset {0} contains invalid CompanyId value {1}; FaultId <{2}> must be accompanied by valid CompanyId", new object[]
						{
							i,
							companyResponseInfo.CompanyId,
							companyResponseInfo.Fault.Id
						});
					}
					if (companyResponseInfo.CompanyGuid == null || companyResponseInfo.CompanyGuid.Value == Guid.Empty)
					{
						EhfProvisioningService.HandleContractViolation(operationName, "CompanyResponseInfo at offset {0} does not contain CompanyGuid value; FaultId <{1}> must be accompanied by non-null CompanyGuid", new object[]
						{
							i,
							companyResponseInfo.Fault.Id
						});
					}
				}
			}
		}

		private static void ValidateResponseSetForReseller(CompanyResponseInfoSet responseSet, string operationName, int resellerId)
		{
			EhfProvisioningService.ValidateCompanyResponseSetSize(responseSet, operationName, 1);
			CompanyResponseInfo companyResponseInfo = responseSet.ResponseInfo[0];
			if (companyResponseInfo.Status == ResponseStatus.Success && companyResponseInfo.CompanyId != resellerId)
			{
				EhfProvisioningService.HandleContractViolation(operationName, "CompanyResponseInfo contains CompanyId <{0}>, expected CompanyId <{1}>", new object[]
				{
					companyResponseInfo.CompanyId,
					resellerId
				});
			}
		}

		private static void ValidateDomainResponseSetSize(DomainResponseInfoSet responseSet, string operationName, int numExpectedResponses)
		{
			if (responseSet == null)
			{
				EhfProvisioningService.HandleContractViolation(operationName, "Returned DomainResponseInfoSet value is null", new object[0]);
			}
			if (responseSet.ResponseInfo == null)
			{
				EhfProvisioningService.HandleContractViolation(operationName, "Returned DomainResponseInfoSet.ResponseInfo value is null", new object[0]);
			}
			if (responseSet.ResponseInfo.Length != numExpectedResponses)
			{
				EhfProvisioningService.HandleContractViolation(operationName, "DomainResponseInfoSet.ResponseInfo length {0} does not match expected value {1}", new object[]
				{
					responseSet.ResponseInfo.Length,
					numExpectedResponses
				});
			}
		}

		private static void ValidateResponseSetForDomains<EhfDomainT>(DomainResponseInfoSet responseSet, string operationName, IList<EhfDomainT> domains) where EhfDomainT : EhfDomainItem
		{
			EhfProvisioningService.ValidateDomainResponseSetSize(responseSet, operationName, domains.Count);
			for (int i = 0; i < responseSet.ResponseInfo.Length; i++)
			{
				DomainResponseInfo domainResponseInfo = responseSet.ResponseInfo[i];
				if (domainResponseInfo.Status == ResponseStatus.Success)
				{
					EhfDomainT ehfDomainT = domains[i];
					Guid domainGuid = ehfDomainT.GetDomainGuid();
					if (domainResponseInfo.DomainGuid == null || !domainResponseInfo.DomainGuid.Value.Equals(domainGuid))
					{
						string violationFormat = "DomainResponseInfo at offset {0} contains GUID <{1}> that does not match expected GUID <{2}>";
						object[] array = new object[3];
						array[0] = i;
						object[] array2 = array;
						int num = 1;
						Guid? domainGuid2 = domainResponseInfo.DomainGuid;
						array2[num] = ((domainGuid2 != null) ? domainGuid2.GetValueOrDefault() : "null");
						array[2] = domainGuid;
						EhfProvisioningService.HandleContractViolation(operationName, violationFormat, array);
					}
				}
				else if (domainResponseInfo.Fault == null)
				{
					EhfProvisioningService.HandleContractViolation(operationName, "Unsuccessful DomainResponseInfo at offset {0} does not contain Fault", new object[]
					{
						i
					});
				}
				else if (domainResponseInfo.Fault.Id == FaultId.DomainExistUnderThisCompany && (domainResponseInfo.DomainGuid == null || domainResponseInfo.DomainGuid.Value == Guid.Empty))
				{
					EhfProvisioningService.HandleContractViolation(operationName, "DomainResponseInfo at offset {0} does not contain DomainGuid value; FaultId <{1}> must be accompanied by non-null DomainGuid", new object[]
					{
						i,
						domainResponseInfo.Fault.Id
					});
				}
			}
		}

		private static void ValidateResponseForSyncAdminAccounts(FailedAdminAccounts response, CompanyAdministrators companyAdministrators, string operationName)
		{
			if (response == null)
			{
				return;
			}
			EhfProvisioningService.ValidateAdminSyncResponse<string>(companyAdministrators.AdminUsers, response.FailedUsers, companyAdministrators.CompanyId, operationName, "FailedUsers");
			EhfProvisioningService.ValidateAdminSyncResponse<Guid>(companyAdministrators.AdminGroups, response.FailedGroups, companyAdministrators.CompanyId, operationName, "FailedGroups");
			if ((response.FailedUsers == null || response.FailedUsers.Count == 0) && (response.FailedGroups == null || response.FailedGroups.Count == 0))
			{
				EhfProvisioningService.HandleContractViolation(operationName, "A failed response from {0} operation should contain atleast one failed entry", new object[]
				{
					operationName
				});
			}
		}

		private static void ValidateBeforeSync(CompanyAdministrators companyAdministrators)
		{
			if (companyAdministrators == null)
			{
				throw new ArgumentNullException("companyAdministrators");
			}
			if (companyAdministrators.CompanyId == 0)
			{
				throw new ArgumentException("Attempted to sync admins without the companyId", "companyAdministrators");
			}
			if (companyAdministrators.AdminUsers != null && companyAdministrators.AdminUsers.Count > 0)
			{
				foreach (KeyValuePair<Role, string[]> keyValuePair in companyAdministrators.AdminUsers)
				{
					if (keyValuePair.Key != Role.Administrator && keyValuePair.Key != Role.ReadOnlyAdministrator)
					{
						throw new ArgumentException(string.Format("Attempted to sync admins with unexpected role '{0}'. Only '{1}' and '{2}' is expected", keyValuePair.Key, Role.Administrator, Role.ReadOnlyAdministrator), "companyAdministrators");
					}
				}
			}
			if (companyAdministrators.AdminGroups != null && companyAdministrators.AdminGroups.Count > 0)
			{
				foreach (KeyValuePair<Role, Guid[]> keyValuePair2 in companyAdministrators.AdminGroups)
				{
					if (keyValuePair2.Key != Role.Administrator && keyValuePair2.Key != Role.ReadOnlyAdministrator)
					{
						throw new ArgumentException(string.Format("Attempted to sync partneradmins with unexpected role {0}.  Only '{1}' and '{2}' is expected", keyValuePair2.Key, Role.Administrator, Role.ReadOnlyAdministrator), "companyAdministrators");
					}
				}
			}
			if (EhfProvisioningService.IsEmtpy<Guid>(companyAdministrators.AdminGroups) && EhfProvisioningService.IsEmtpy<string>(companyAdministrators.AdminUsers))
			{
				throw new ArgumentException("There are no changes to sync. Every Sync operation should have atleast one change to sync.", "companyAdministrators");
			}
		}

		private static bool IsEmtpy<T>(Dictionary<Role, T[]> admins)
		{
			if (admins == null)
			{
				return true;
			}
			if (admins.Count == 0)
			{
				return true;
			}
			foreach (KeyValuePair<Role, T[]> keyValuePair in admins)
			{
				if (keyValuePair.Value != null)
				{
					return false;
				}
			}
			return true;
		}

		private static void ValidateAdminSyncResponse<T>(Dictionary<Role, T[]> request, Dictionary<T, ErrorInfo> response, int companyId, string operationName, string fieldName)
		{
			if (response != null && response.Count != 0)
			{
				if (request == null || request.Count == 0)
				{
					EhfProvisioningService.HandleContractViolation(operationName, "'{0}' field in FailedAdminAccounts response for company <{1}> contains more entries than that was present in the request; RequestAdminCount: <{2}>; FailedAdminResponseCount: <{3}>", new object[]
					{
						fieldName,
						companyId,
						(request == null) ? "<null>" : request.Count.ToString(),
						response.Count
					});
				}
				HashSet<T> hashSet = new HashSet<T>();
				foreach (KeyValuePair<Role, T[]> keyValuePair in request)
				{
					if (keyValuePair.Value != null)
					{
						hashSet.AddRange(keyValuePair.Value);
					}
				}
				EhfProvisioningService.ValidateAdminSyncResponseSet<T, ErrorInfo>(response, operationName, fieldName, hashSet);
			}
		}

		private static void ValidateAdminSyncResponseSet<TUser, TErrorInfo>(Dictionary<TUser, TErrorInfo> response, string operationName, string responseDetails, HashSet<TUser> adminsInRequest) where TErrorInfo : AdminServiceFault
		{
			foreach (KeyValuePair<TUser, TErrorInfo> keyValuePair in response)
			{
				if (keyValuePair.Value == null)
				{
					EhfProvisioningService.HandleContractViolation(operationName, "Failed response set for operation <{0}> null error value for admin item <{1}>. ResponseDetails: <{2}>", new object[]
					{
						operationName,
						keyValuePair.Key,
						responseDetails
					});
				}
				if (!adminsInRequest.Contains(keyValuePair.Key))
				{
					EhfProvisioningService.HandleContractViolation(operationName, "Failed response set for operation <{0}> has as entry <{1}> which is not present in the request. RequestDetails: <{2}>", new object[]
					{
						operationName,
						keyValuePair.Key,
						responseDetails
					});
				}
			}
		}

		private static int[] GetCompanyIds(IList<EhfCompanyItem> companies)
		{
			if (companies == null || companies.Count == 0)
			{
				throw new ArgumentNullException("companies");
			}
			int[] array = new int[companies.Count];
			for (int i = 0; i < companies.Count; i++)
			{
				array[i] = companies[i].CompanyId;
				if (array[i] == 0)
				{
					throw new ArgumentException("Attempted to delete or disable a company without setting its CompanyId", "companies");
				}
			}
			return array;
		}

		private static Domain[] GetEhfDomains(IList<EhfDomainItem> domains)
		{
			if (domains == null || domains.Count == 0)
			{
				throw new ArgumentNullException("domains");
			}
			Domain[] array = new Domain[domains.Count];
			for (int i = 0; i < domains.Count; i++)
			{
				array[i] = domains[i].Domain;
				if (array[i].CompanyId == 0)
				{
					throw new ArgumentException("Attempted to create, delete or disable a domain without setting its CompanyId", "domains");
				}
				if (array[i].DomainGuid == null || array[i].DomainGuid.Value == Guid.Empty)
				{
					throw new ArgumentException("Attempted to create, delete or disable a domain without setting its GUID", "domains");
				}
			}
			return array;
		}

		private Company GetCompany(int companyId, string operationName)
		{
			Company companyById = this.managementService.GetCompanyById(companyId);
			if (companyById != null && companyById.CompanyId != companyId)
			{
				EhfProvisioningService.HandleContractViolation(operationName, "GetCompanyById({0}) returned a company with id <{1}>", new object[]
				{
					companyId,
					companyById.CompanyId
				});
			}
			return companyById;
		}

		public const string DefaultSmtpProfileName = "InboundSmtpProfile";

		private const int MaxBufferPoolSize = 524288;

		private const int MaxArrayLength = 163840;

		private DisposeTracker disposeTracker;

		private EhfProvisioningService.ProvisioningServiceWrapper provisioningService;

		private EhfProvisioningService.ManagementServiceWrapper managementService;

		private EhfProvisioningService.AdminSyncServiceWrapper adminSyncService;

		public enum MessageSecurityExceptionReason
		{
			DatabaseFailure,
			InvalidCredentials,
			Other
		}

		public class ContractViolationException : Exception
		{
			public ContractViolationException(string operationName, string errorMessage) : base(errorMessage)
			{
				this.operationName = operationName;
			}

			public ContractViolationException(string operationName, string errorMessage, Exception innerException) : base(errorMessage, innerException)
			{
				this.operationName = operationName;
			}

			public string OperationName
			{
				get
				{
					return this.operationName;
				}
			}

			private readonly string operationName;
		}

		private abstract class ServiceWrapper<TService, TClient> : IDisposeTrackable, IDisposable where TService : class where TClient : ClientBase<TService>, TService
		{
			public ServiceWrapper(EhfTargetServerConfig config)
			{
				if (config == null)
				{
					throw new ArgumentNullException("config");
				}
				this.disposeTracker = this.GetDisposeTracker();
				this.config = config;
				this.InitializeClient();
			}

			public ServiceWrapper(TService service)
			{
				this.service = service;
				this.disposeTracker = this.GetDisposeTracker();
			}

			protected TService Service
			{
				get
				{
					return this.service;
				}
			}

			public void Dispose()
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				if (this.client != null)
				{
					if (this.client.State == CommunicationState.Faulted)
					{
						this.client.Abort();
					}
					else
					{
						try
						{
							this.client.Close();
						}
						catch (TimeoutException arg)
						{
							EhfProvisioningService.ServiceWrapper<TService, TClient>.tracer.TraceError<TimeoutException>((long)this.GetHashCode(), "TimeoutException occurred while closing EHF service client; exception details: {0}", arg);
							this.client.Abort();
						}
						catch (CommunicationException arg2)
						{
							EhfProvisioningService.ServiceWrapper<TService, TClient>.tracer.TraceError<CommunicationException>((long)this.GetHashCode(), "CommunicationException occurred while closing EHF service client; exception details: {0}", arg2);
							this.client.Abort();
						}
					}
					this.client = default(TClient);
				}
				this.service = default(TService);
			}

			public DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<EhfProvisioningService.ServiceWrapper<TService, TClient>>(this);
			}

			public void SuppressDisposeTracker()
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Suppress();
				}
			}

			protected abstract TClient CreateClientInstance(Binding binding, EndpointAddress remoteAddress);

			protected void RecoverFromFailure()
			{
				if (this.client != null && this.client.State == CommunicationState.Faulted)
				{
					this.client.Abort();
					TClient tclient = default(TClient);
					this.client = tclient;
					this.service = (TService)((object)tclient);
					this.InitializeClient();
				}
			}

			private void InitializeClient()
			{
				bool flag = !string.IsNullOrEmpty(this.config.UserName);
				WSHttpBinding wshttpBinding;
				if (flag)
				{
					wshttpBinding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
					wshttpBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
					wshttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
					wshttpBinding.Security.Message.EstablishSecurityContext = true;
				}
				else
				{
					wshttpBinding = new WSHttpBinding(SecurityMode.None);
				}
				wshttpBinding.MessageEncoding = WSMessageEncoding.Text;
				wshttpBinding.OpenTimeout = this.config.EhfSyncAppConfig.RequestTimeout;
				wshttpBinding.CloseTimeout = this.config.EhfSyncAppConfig.RequestTimeout;
				wshttpBinding.ReceiveTimeout = this.config.EhfSyncAppConfig.RequestTimeout;
				wshttpBinding.SendTimeout = this.config.EhfSyncAppConfig.RequestTimeout;
				wshttpBinding.MaxReceivedMessageSize = (long)this.config.EhfSyncAppConfig.MaxMessageSize;
				wshttpBinding.MaxBufferPoolSize = 524288L;
				wshttpBinding.ReaderQuotas.MaxArrayLength = 163840;
				wshttpBinding.ReliableSession.Enabled = false;
				wshttpBinding.ReliableSession.InactivityTimeout = this.config.EhfSyncAppConfig.RequestTimeout;
				if (this.config.InternetWebProxy != null)
				{
					wshttpBinding.UseDefaultWebProxy = false;
					wshttpBinding.BypassProxyOnLocal = true;
					wshttpBinding.ProxyAddress = this.config.InternetWebProxy;
				}
				string uri = this.config.ProvisioningUrl.ToString();
				EndpointAddress remoteAddress = new EndpointAddress(uri);
				this.client = this.CreateClientInstance(wshttpBinding, remoteAddress);
				if (flag)
				{
					this.client.ClientCredentials.UserName.UserName = this.config.UserName;
					this.client.ClientCredentials.UserName.Password = (this.config.Password.AsUnsecureString() ?? string.Empty);
				}
				this.service = (TService)((object)this.client);
			}

			private static Trace tracer = ExTraceGlobals.TargetConnectionTracer;

			private TService service;

			private TClient client;

			private EhfTargetServerConfig config;

			private DisposeTracker disposeTracker;
		}

		private class ProvisioningServiceWrapper : EhfProvisioningService.ServiceWrapper<IProvisioningService, ProvisioningServiceClient>
		{
			public ProvisioningServiceWrapper(EhfTargetServerConfig config) : base(config)
			{
			}

			public ProvisioningServiceWrapper(IProvisioningService service) : base(service)
			{
			}

			public CompanyResponseInfoSet CreateCompanies(Company[] companies)
			{
				CompanyResponseInfoSet result;
				try
				{
					result = base.Service.CreateCompanies(companies);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return result;
			}

			public DomainResponseInfoSet AddDomains(Domain[] domains)
			{
				DomainResponseInfoSet result;
				try
				{
					result = base.Service.AddDomains(domains);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return result;
			}

			public CompanyResponseInfoSet DeleteCompanies(int[] companyIds)
			{
				CompanyResponseInfoSet result;
				try
				{
					result = base.Service.DeleteCompanies(companyIds);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return result;
			}

			public DomainResponseInfoSet DeleteDomains(Domain[] domains)
			{
				DomainResponseInfoSet result;
				try
				{
					result = base.Service.DeleteDomains(domains);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return result;
			}

			protected override ProvisioningServiceClient CreateClientInstance(Binding binding, EndpointAddress remoteAddress)
			{
				return new ProvisioningServiceClient(binding, remoteAddress);
			}
		}

		private class ManagementServiceWrapper : EhfProvisioningService.ServiceWrapper<IManagementService, ManagementServiceClient>
		{
			public ManagementServiceWrapper(EhfTargetServerConfig config) : base(config)
			{
			}

			public ManagementServiceWrapper(IManagementService service) : base(service)
			{
			}

			public CompanyResponseInfoSet UpdateCompanySettings(CompanyConfigurationSettings[] settings)
			{
				CompanyResponseInfoSet result;
				try
				{
					result = base.Service.UpdateCompanySettings(settings);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return result;
			}

			public DomainResponseInfoSet UpdateDomainSettingsByGuids(DomainConfigurationSettings[] settings)
			{
				DomainResponseInfoSet result;
				try
				{
					result = base.Service.UpdateDomainSettingsByGuids(settings);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return result;
			}

			public CompanyResponseInfoSet DisableCompanies(int[] companyIds)
			{
				CompanyResponseInfoSet result;
				try
				{
					result = base.Service.DisableCompanies(companyIds);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return result;
			}

			public DomainResponseInfoSet DisableDomains(Domain[] domains)
			{
				DomainResponseInfoSet result;
				try
				{
					result = base.Service.DisableDomains(domains);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return result;
			}

			public Company GetCompanyByGuid(Guid companyGuid)
			{
				Company companyByGuid;
				try
				{
					companyByGuid = base.Service.GetCompanyByGuid(companyGuid);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return companyByGuid;
			}

			public Company GetCompanyById(int companyId)
			{
				Company companyById;
				try
				{
					companyById = base.Service.GetCompanyById(companyId);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return companyById;
			}

			public Domain[] GetAllDomains(int companyId)
			{
				Domain[] allDomains;
				try
				{
					allDomains = base.Service.GetAllDomains(companyId);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return allDomains;
			}

			protected override ManagementServiceClient CreateClientInstance(Binding binding, EndpointAddress remoteAddress)
			{
				return new ManagementServiceClient(binding, remoteAddress);
			}
		}

		private class AdminSyncServiceWrapper : EhfProvisioningService.ServiceWrapper<IAdminSyncService, AdminSyncServiceClient>
		{
			public AdminSyncServiceWrapper(EhfTargetServerConfig config) : base(config)
			{
			}

			public AdminSyncServiceWrapper(IAdminSyncService service) : base(service)
			{
			}

			public FailedAdminAccounts SyncAdminAccounts(CompanyAdministrators companyAdministrators)
			{
				FailedAdminAccounts result;
				try
				{
					result = base.Service.SyncAdminAccounts(companyAdministrators);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return result;
			}

			public Dictionary<string, ErrorInfo> SyncGroupUsers(int companyId, Guid groupGuid, string[] users)
			{
				Dictionary<string, ErrorInfo> result;
				try
				{
					result = base.Service.SyncGroupUsers(companyId, groupGuid, users);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return result;
			}

			public Dictionary<Guid, RemoveGroupErrorInfo> RemoveGroups(Guid[] groupGuids)
			{
				Dictionary<Guid, RemoveGroupErrorInfo> result;
				try
				{
					result = base.Service.RemoveGroups(groupGuids);
				}
				finally
				{
					base.RecoverFromFailure();
				}
				return result;
			}

			protected override AdminSyncServiceClient CreateClientInstance(Binding binding, EndpointAddress remoteAddress)
			{
				return new AdminSyncServiceClient(binding, remoteAddress);
			}
		}
	}
}
