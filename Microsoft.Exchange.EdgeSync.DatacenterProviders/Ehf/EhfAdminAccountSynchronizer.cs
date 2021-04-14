using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Datacenter;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfAdminAccountSynchronizer : EhfSynchronizer
	{
		public EhfAdminAccountSynchronizer(EhfTargetConnection ehfConnection, EhfSyncErrorTracker errorTracker) : base(ehfConnection)
		{
			if (errorTracker.CriticalTransientFailureCount != 0 || errorTracker.PermanentFailureCount != 0 || errorTracker.AllTransientFailuresCount != 0)
			{
				throw new ArgumentException(string.Format("There should not be any failures before the sync cycle starts. Critical {0}; Permanent {1} AllTransientFailures {2}", errorTracker.CriticalTransientFailureCount, errorTracker.PermanentFailureCount, errorTracker.AllTransientFailuresCount));
			}
			EhfAdminAccountSynchronizer.cycleCount++;
			base.DiagSession.Tracer.TraceDebug<int>((long)base.DiagSession.GetHashCode(), "Creating EhfAdminAccountSynchronizer ({0})", EhfAdminAccountSynchronizer.cycleCount);
			this.errorTracker = errorTracker;
		}

		public static TypeSynchronizer CreateTypeSynchronizer()
		{
			return new TypeSynchronizer(null, new PreDecorate(EhfAdminAccountSynchronizer.LoadAdminGroups), null, null, null, null, "Tenant Admins", Schema.Query.QueryUsgMailboxAndOrganization, null, SearchScope.Subtree, EhfAdminAccountSynchronizer.TenantAdminsRoleGroupSyncAttributes, null, false);
		}

		public override bool FlushBatches()
		{
			if (this.adminAccountChange.Count == 0 && this.groupsToRemove.Count == 0)
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "{0}: No admin changes to Sync to FOSE", new object[]
				{
					EhfAdminAccountSynchronizer.cycleCount
				});
				return true;
			}
			this.InvokeRemoveGroups();
			if (this.adminAccountChange.Count != 0)
			{
				Exception ex;
				EhfADAdapter configADAdapter = base.ADAdapter.GetConfigADAdapter(base.DiagSession, out ex);
				if (configADAdapter == null)
				{
					base.DiagSession.LogAndTraceError("Could not create a LDAP connection to the Configuration naming context. Details {0}", new object[]
					{
						ex
					});
					base.DiagSession.EventLog.LogEvent(EdgeSyncEventLogConstants.Tuple_EhfAdminSyncFailedToConnectToConfigNamingContext, null, new object[]
					{
						ex.Message
					});
					base.EhfConnection.AbortSyncCycle(ex);
					return false;
				}
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "{0} : Changes to <{1}> tenant(s) detected. Checking if sync is required.", new object[]
				{
					EhfAdminAccountSynchronizer.cycleCount.ToString(),
					this.adminAccountChange.Count.ToString()
				});
				foreach (KeyValuePair<string, EhfAdminSyncChangeBuilder> keyValuePair in this.adminAccountChange)
				{
					this.AbortSyncCycleIfTooManyFailures();
					EhfAdminSyncChangeBuilder value = keyValuePair.Value;
					if (value.ChangeExists)
					{
						EhfCompanyAdmins ehfCompanyAdmins = value.Flush(configADAdapter);
						if (ehfCompanyAdmins == null)
						{
							this.errorTracker.AddCriticalFailure();
						}
						else if (ehfCompanyAdmins.IsSyncRequired)
						{
							base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "AdminSync: {0}", new object[]
							{
								ehfCompanyAdmins
							});
							if (ehfCompanyAdmins.CompanyId != 0)
							{
								this.InvokeSyncAdminAccountsAndSyncGroupUsers(ehfCompanyAdmins, configADAdapter);
							}
							else
							{
								base.DiagSession.LogAndTraceError("Not syncing {0} since companyId is not set", new object[]
								{
									ehfCompanyAdmins.TenantOU
								});
								if (!ehfCompanyAdmins.PerimeterConfigNotReplicatedOrIsDeleted)
								{
									this.errorTracker.AddTransientFailure(ehfCompanyAdmins.EhfCompanyIdentity, new EhfAdminAccountSynchronizer.EhfAdminSyncTransientException("PerimeterConfig object does not have Ehf CompanyId set."), string.Empty);
								}
								else
								{
									this.errorTracker.AddCriticalFailure();
								}
							}
						}
						else
						{
							base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "No adminsync is required for: {0}", new object[]
							{
								keyValuePair.Key
							});
						}
						value.ClearCachedChanges();
					}
				}
				this.adminAccountChange.Clear();
			}
			return true;
		}

		public void HandleGroupChangedEvent(ExSearchResultEntry entry)
		{
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "GroupChanged: {0}", new object[]
			{
				entry.DistinguishedName
			});
			EhfAdminSyncChangeBuilder adminBuilderForChange = this.GetAdminBuilderForChange(entry);
			if (adminBuilderForChange != null)
			{
				this.ThrowIfAdminSyncNotEnabled(adminBuilderForChange.TenantOU, entry);
				adminBuilderForChange.AddGroupMembershipChange(entry);
			}
		}

		public void HandleGroupAddedEvent(ExSearchResultEntry entry)
		{
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "GroupAdded: {0}", new object[]
			{
				entry.DistinguishedName
			});
			EhfAdminSyncChangeBuilder adminBuilderForChange = this.GetAdminBuilderForChange(entry);
			if (adminBuilderForChange != null)
			{
				this.ThrowIfAdminSyncNotEnabled(adminBuilderForChange.TenantOU, entry);
				adminBuilderForChange.AddGroupAdditionChange(entry);
			}
		}

		public void HandleGroupDeletedEvent(ExSearchResultEntry entry)
		{
			if (EhfAdminAccountSynchronizer.IsEventForDeletedOrganization(entry, base.DiagSession))
			{
				throw new InvalidOperationException("Change entry " + entry.DistinguishedName + " is for a deleted organization. The entry should have been ignored from PreDecorate.");
			}
			EhfAdminSyncChangeBuilder adminBuilderForChange = this.GetAdminBuilderForChange(entry);
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Encountered a DELETE rolegroup event. ObjectGuid: <{0}>; Company: <{1}>", new object[]
			{
				entry.GetObjectGuid(),
				adminBuilderForChange.TenantOU
			});
			if (adminBuilderForChange != null)
			{
				adminBuilderForChange.HandleGroupDeletedEvent(entry);
			}
			if (!EhfWellKnownGroup.IsWellKnownPartnerGroupDN(entry.DistinguishedName))
			{
				return;
			}
			Guid externalDirectoryObjectId;
			if (EhfCompanyAdmins.TryGetExternalDirectoryObjectId(entry, base.DiagSession, out externalDirectoryObjectId))
			{
				this.AddGroupToDeleteGroupsBatch(externalDirectoryObjectId);
				return;
			}
			base.DiagSession.LogAndTraceError("Could not find the ExternalDirectoryObjectId for well known partner group {0}", new object[]
			{
				entry.DistinguishedName
			});
		}

		public void HandleWlidChangedEvent(ExSearchResultEntry entry)
		{
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "WLIDChanged: {0}", new object[]
			{
				entry.DistinguishedName
			});
			EhfAdminSyncChangeBuilder adminBuilderForChange = this.GetAdminBuilderForChange(entry);
			if (adminBuilderForChange != null)
			{
				this.ThrowIfAdminSyncNotEnabled(adminBuilderForChange.TenantOU, entry);
				adminBuilderForChange.AddWlidChanges(entry);
			}
		}

		public void HandleWlidAddedEvent(ExSearchResultEntry entry)
		{
		}

		public void HandleWlidDeletedEvent(ExSearchResultEntry entry)
		{
			if (!EhfAdminAccountSynchronizer.IsEventForDeletedOrganization(entry, base.DiagSession))
			{
				EhfAdminSyncChangeBuilder adminBuilderForChange = this.GetAdminBuilderForChange(entry);
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Encountered a DELETE mailbox event. ObjectGuid: <{0}>; Company: <{1}>", new object[]
				{
					entry.GetObjectGuid(),
					adminBuilderForChange.TenantOU
				});
				if (adminBuilderForChange != null)
				{
					adminBuilderForChange.HandleWlidDeletedEvent(entry);
					return;
				}
			}
			else
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Ignoring the WLID delete event '{0}' for a deleted organization", new object[]
				{
					entry.DistinguishedName
				});
			}
		}

		public void HandleOrganizationDeletedEvent(ExSearchResultEntry entry)
		{
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Ignoring the Organization Delete event for org {0}", new object[]
			{
				entry.DistinguishedName
			});
		}

		public void HandleOrganizationChangedEvent(ExSearchResultEntry entry)
		{
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Organization changed: <{0}>", new object[]
			{
				entry.DistinguishedName
			});
			EhfAdminSyncChangeBuilder adminBuilderForChange = this.GetAdminBuilderForChange(entry);
			if (adminBuilderForChange != null)
			{
				this.ThrowIfAdminSyncNotEnabled(adminBuilderForChange.TenantOU, entry);
				adminBuilderForChange.HandleOrganizationChangedEvent(entry);
			}
		}

		public void HandleOrganizationAddedEvent(ExSearchResultEntry entry)
		{
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Organization created: <{0}>", new object[]
			{
				entry.DistinguishedName
			});
			EhfAdminSyncChangeBuilder adminBuilderForChange = this.GetAdminBuilderForChange(entry);
			if (adminBuilderForChange != null)
			{
				this.ThrowIfAdminSyncNotEnabled(adminBuilderForChange.TenantOU, entry);
				adminBuilderForChange.HandleOrganizationAddedEvent(entry);
			}
		}

		public override void ClearBatches()
		{
			this.adminAccountChange.Clear();
		}

		public bool FilterOrLoadFullEntry(ExSearchResultEntry entry)
		{
			bool flag = false;
			string tenantOU;
			bool isOrgChange;
			return this.TryGetTenantOrgUnitDNForChange(entry, out tenantOU, out isOrgChange, ref flag) && !this.IgnoreChange(entry, tenantOU, isOrgChange, ref flag) && (flag || EhfSynchronizer.LoadFullEntry(entry, EhfAdminAccountSynchronizer.AdminSyncAllAttributes, base.EhfConnection));
		}

		public void LogAndAbortSyncCycle()
		{
			string message = string.Format(CultureInfo.InvariantCulture, "The EHFAdminsync encountered <{0}> transient failures and <{1}> permanent failures. Aborting the sync cycle", new object[]
			{
				this.errorTracker.AllTransientFailuresCount,
				this.errorTracker.PermanentFailureCount
			});
			base.EhfConnection.AbortSyncCycle(new EdgeSyncCycleFailedException(message));
		}

		private static bool LoadAdminGroups(ExSearchResultEntry entry, Connection sourceConnection, TargetConnection targetConnection, object state)
		{
			EhfRecipientTargetConnection ehfRecipientTargetConnection = (EhfRecipientTargetConnection)targetConnection;
			return ehfRecipientTargetConnection.AdminAccountSynchronizer.FilterOrLoadFullEntry(entry);
		}

		private static bool IsEventForDeletedOrganization(ExSearchResultEntry entry, EdgeSyncDiag diagSession)
		{
			string distinguishedName;
			return EhfAdminAccountSynchronizer.TryGetOrganizationUnit(entry, diagSession, out distinguishedName) && ExSearchResultEntry.IsDeletedDN(distinguishedName);
		}

		private static bool TryGetOrganizationUnit(ExSearchResultEntry entry, EdgeSyncDiag diagSession, out string exchOURoot)
		{
			exchOURoot = string.Empty;
			DirectoryAttribute directoryAttribute;
			if (entry.Attributes.TryGetValue("msExchOURoot", out directoryAttribute) && directoryAttribute != null)
			{
				exchOURoot = (string)directoryAttribute[0];
				return true;
			}
			diagSession.LogAndTraceError("Could not find ExchOURoot for {0}. Every object is expected to contain this attribute.", new object[]
			{
				entry.DistinguishedName
			});
			return false;
		}

		private static void ClassifyFailedResponse<T>(Dictionary<T, ErrorInfo> response, ref int transientFailureCount, ref int permanentFailureCount, ref bool hasCriticalError, StringBuilder logBuilder)
		{
			logBuilder.Append("[");
			foreach (KeyValuePair<T, ErrorInfo> keyValuePair in response)
			{
				logBuilder.AppendFormat(CultureInfo.InstalledUICulture, "<{0}: {1} {2} {3}>", new object[]
				{
					keyValuePair.Key,
					keyValuePair.Value.Code,
					keyValuePair.Value.ErrorType,
					EhfAdminAccountSynchronizer.FormatAdditionalData(keyValuePair.Value.Data)
				});
				if (keyValuePair.Value.ErrorType == ErrorType.Transient)
				{
					transientFailureCount++;
					if (keyValuePair.Value.Code != ErrorCode.InvalidFormat)
					{
						hasCriticalError = true;
					}
				}
				else
				{
					permanentFailureCount++;
				}
			}
			logBuilder.Append("]");
		}

		private static string FormatAdditionalData(Dictionary<string, string> additionalData)
		{
			if (additionalData == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AdditionalData: ");
			foreach (KeyValuePair<string, string> keyValuePair in additionalData)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "({0}),({1}) ", new object[]
				{
					keyValuePair.Key,
					keyValuePair.Value
				});
			}
			return stringBuilder.ToString();
		}

		private static bool TryGetOURootFromDN(string dn, out string tenantOU, out bool isOrgChange)
		{
			tenantOU = string.Empty;
			isOrgChange = false;
			ADObjectId adobjectId = new ADObjectId(dn);
			int num = adobjectId.Depth - adobjectId.DomainId.Depth;
			if (num < EhfAdminAccountSynchronizer.OrganizationUnitDepth)
			{
				return false;
			}
			ADObjectId adobjectId2;
			if (num == EhfAdminAccountSynchronizer.OrganizationUnitDepth)
			{
				adobjectId2 = adobjectId;
				isOrgChange = true;
			}
			else
			{
				adobjectId2 = adobjectId.AncestorDN(num - EhfAdminAccountSynchronizer.OrganizationUnitDepth);
			}
			if (!EhfAdminAccountSynchronizer.IsOrganizationUnitDN(adobjectId2))
			{
				return false;
			}
			tenantOU = adobjectId2.DistinguishedName;
			return true;
		}

		private static bool IsOrganizationUnitDN(ADObjectId tenantObjectId)
		{
			if (!tenantObjectId.Rdn.Prefix.Equals("OU", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			AdName rdn = tenantObjectId.Parent.Rdn;
			return rdn.Prefix.Equals("OU", StringComparison.OrdinalIgnoreCase) && rdn.UnescapedName.Equals("Microsoft Exchange Hosted Organizations");
		}

		private static bool TryGetFlagsValue(string flagsAttrName, ExSearchResultEntry entry, out int flagValue)
		{
			flagValue = 0;
			DirectoryAttribute attribute = entry.GetAttribute(flagsAttrName);
			if (attribute == null)
			{
				return false;
			}
			string text = (string)attribute[0];
			return !string.IsNullOrEmpty(text) && int.TryParse(text, out flagValue);
		}

		private bool IgnoreChange(ExSearchResultEntry entry, string tenantOU, bool isOrgChange, ref bool fullLoadComplete)
		{
			EhfAdminSyncChangeBuilder ehfAdminSyncChangeBuilder;
			if (this.adminAccountChange.TryGetValue(tenantOU, out ehfAdminSyncChangeBuilder) && ehfAdminSyncChangeBuilder.IsFullTenantAdminSyncRequired())
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Complete adminsync is required for the org <{0}>. Ignoring the change <{1}>", new object[]
				{
					tenantOU,
					entry.DistinguishedName
				});
				return true;
			}
			bool flag;
			if (!this.syncEnabledConfigCache.TryGetValue(tenantOU, out flag))
			{
				ExSearchResultEntry exSearchResultEntry;
				if (isOrgChange && entry.Attributes.ContainsKey("msExchProvisioningFlags"))
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.High, "AdminSyncEnabled value for <{0}> is present in the organization change entry ", new object[]
					{
						tenantOU
					});
					exSearchResultEntry = entry;
				}
				else
				{
					exSearchResultEntry = base.ADAdapter.ReadObjectEntry(tenantOU, false, EhfAdminAccountSynchronizer.AdminSyncAllAttributes);
					if (exSearchResultEntry == null)
					{
						base.DiagSession.LogAndTraceError("Failed to read the organization unit entry for <{0}>. Ignoring the entry <{1}>", new object[]
						{
							tenantOU,
							entry.DistinguishedName
						});
						return true;
					}
					if (isOrgChange)
					{
						fullLoadComplete = true;
					}
				}
				OrganizationProvisioningFlags provisioningFlags = this.GetProvisioningFlags(exSearchResultEntry);
				flag = ((provisioningFlags & OrganizationProvisioningFlags.EhfAdminAccountSyncEnabled) == OrganizationProvisioningFlags.EhfAdminAccountSyncEnabled);
				this.AddToSyncEnabledCache(tenantOU, flag);
				base.DiagSession.Tracer.TraceDebug<bool, string>((long)this.GetHashCode(), "AdminSyncEnabled is set to '{0}' for tenant <{1}>", flag, tenantOU);
			}
			else
			{
				base.DiagSession.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "AdminSyncEnabled value for <{0}> is present in cache. Change entry <{1}>", tenantOU, entry.DistinguishedName);
			}
			if (!flag)
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "AdminSync is disabled for tenant <{0}>. Ignoring the change <{1}>", new object[]
				{
					tenantOU,
					entry.DistinguishedName
				});
				return true;
			}
			return false;
		}

		private void AddToSyncEnabledCache(string tenantOU, bool syncEnabled)
		{
			if (!this.IsSyncEnabledCacheFull())
			{
				this.syncEnabledConfigCache.Add(tenantOU, syncEnabled);
				return;
			}
			base.DiagSession.Tracer.TraceDebug<string, bool>((long)this.GetHashCode(), "SyncEnabled cache is full. Not adding item {0}:{1} to cache", tenantOU, syncEnabled);
		}

		private bool IsSyncEnabledCacheFull()
		{
			return this.syncEnabledConfigCache.Count >= 20000;
		}

		private bool TryGetTenantOrgUnitDNForChange(ExSearchResultEntry entry, out string tenantOU, out bool isOrgChange, ref bool fullLoadComplete)
		{
			tenantOU = null;
			isOrgChange = false;
			if (entry.IsDeleted)
			{
				if (!entry.Attributes.ContainsKey("msExchOURoot"))
				{
					if (!EhfSynchronizer.LoadFullEntry(entry, EhfAdminAccountSynchronizer.AdminSyncAllAttributes, base.EhfConnection))
					{
						return false;
					}
					fullLoadComplete = true;
				}
				else
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Delete change <{0}> already contains OURoot. Using that.", new object[]
					{
						entry.DistinguishedName
					});
				}
				if (!EhfAdminAccountSynchronizer.TryGetOrganizationUnit(entry, base.DiagSession, out tenantOU))
				{
					base.DiagSession.LogAndTraceError("Could not extract OURoot attribute from deleted change entry <{0}>. Ignoring the change.", new object[]
					{
						entry.DistinguishedName
					});
					return false;
				}
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Extracted the OURoot <{0}> for the deleted change from the change entry <{1}>", new object[]
				{
					tenantOU,
					entry.DistinguishedName
				});
			}
			else if (!EhfAdminAccountSynchronizer.TryGetOURootFromDN(entry.DistinguishedName, out tenantOU, out isOrgChange))
			{
				base.DiagSession.LogAndTraceError("Failed to get the OrganizationUnit Root from the DN of <{0}>. Ignoring the item.", new object[]
				{
					entry.DistinguishedName
				});
				return false;
			}
			if (string.IsNullOrEmpty(tenantOU))
			{
				base.DiagSession.LogAndTraceError("OrganizationUnit DN for <{0}> is null or emtpy. Ignoring the item.", new object[]
				{
					entry.DistinguishedName
				});
				return false;
			}
			if (ExSearchResultEntry.IsDeletedDN(tenantOU))
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Change <{0}> is for a deleted organization <{1}>. Ignoring the item", new object[]
				{
					entry.DistinguishedName,
					tenantOU
				});
				return false;
			}
			return true;
		}

		private OrganizationProvisioningFlags GetProvisioningFlags(ExSearchResultEntry entry)
		{
			int result;
			if (EhfAdminAccountSynchronizer.TryGetFlagsValue("msExchProvisioningFlags", entry, out result))
			{
				return (OrganizationProvisioningFlags)result;
			}
			return OrganizationProvisioningFlags.None;
		}

		private EhfAdminSyncChangeBuilder GetAdminBuilderForChange(ExSearchResultEntry entry)
		{
			string text;
			if (!EhfAdminAccountSynchronizer.TryGetOrganizationUnit(entry, base.DiagSession, out text))
			{
				return null;
			}
			EhfAdminSyncChangeBuilder ehfAdminSyncChangeBuilder;
			if (!this.adminAccountChange.TryGetValue(text, out ehfAdminSyncChangeBuilder))
			{
				DirectoryAttribute attribute = entry.GetAttribute("msExchCU");
				if (attribute == null)
				{
					base.DiagSession.LogAndTraceError("Could not find ConfigUnitDN for {0}. Every object is expected to contain this attribute.", new object[]
					{
						entry.DistinguishedName
					});
					return null;
				}
				string tenantConfigUnitDN = (string)attribute[0];
				ehfAdminSyncChangeBuilder = new EhfAdminSyncChangeBuilder(text, tenantConfigUnitDN, base.EhfConnection);
				this.adminAccountChange.Add(text, ehfAdminSyncChangeBuilder);
			}
			return ehfAdminSyncChangeBuilder;
		}

		private void InvokeSyncAdminAccountsAndSyncGroupUsers(EhfCompanyAdmins admin, EhfADAdapter configADAdapter)
		{
			bool syncAdminAccountsCompleted = true;
			bool syncAdminAgentCompleted = true;
			bool syncHelpdeskAgentCompleted = true;
			if (admin.HasLocalAdminChanges)
			{
				FailedAdminAccounts syncAdminAccountsResponse = null;
				FaultException syncAdminException = null;
				string syncAdminsOperation = EhfAdminAccountSynchronizer.SyncAdminsOperation;
				base.InvokeProvisioningService(syncAdminsOperation, delegate
				{
					syncAdminAccountsResponse = this.ProvisioningService.SyncAdminAccounts(admin.GetLocalAdminsToSync(this.DiagSession), out syncAdminException);
				}, 1);
				if (syncAdminException != null)
				{
					this.HandleOperationLevelException(syncAdminException, syncAdminsOperation, admin.EhfCompanyIdentity);
				}
				else
				{
					this.ProcessSyncAdminAccountsResponse(syncAdminAccountsResponse, admin, syncAdminsOperation);
				}
				syncAdminAccountsCompleted = (syncAdminException == null);
			}
			if (admin.HasPartnerAdminGroupChanges)
			{
				syncAdminAgentCompleted = this.InvokeSyncGroupUsers(admin.EhfCompanyIdentity, admin.AdminAgent, admin.TenantOU);
				syncHelpdeskAgentCompleted = this.InvokeSyncGroupUsers(admin.EhfCompanyIdentity, admin.HelpdeskAgent, admin.TenantOU);
			}
			this.UpdateSyncStateInAD(admin, configADAdapter, syncAdminAccountsCompleted, syncAdminAgentCompleted, syncHelpdeskAgentCompleted);
		}

		private void HandleOperationLevelException(FaultException adminSyncException, string operationName, EhfCompanyIdentity companyIdentity)
		{
			FaultException<InvalidCompanyFault> faultException = adminSyncException as FaultException<InvalidCompanyFault>;
			FaultException<InternalFault> faultException2 = adminSyncException as FaultException<InternalFault>;
			if (faultException != null)
			{
				if (faultException.Detail.Code == InvalidCompanyCode.CompanyDoesNotExist || faultException.Detail.ErrorType == ErrorType.Transient)
				{
					this.HandleFaultAsTransientFailure<InvalidCompanyFault>(companyIdentity, operationName, faultException.Detail.Code.ToString(), faultException, false);
					return;
				}
				this.HandleFaultAsPermanentFailure<InvalidCompanyFault>(operationName, faultException.Detail.Code.ToString(), faultException);
				return;
			}
			else
			{
				if (faultException2 != null)
				{
					this.HandleFaultAsTransientFailure<InternalFault>(companyIdentity, operationName, faultException2.Detail.Code.ToString(), faultException2, true);
					return;
				}
				throw new InvalidOperationException("Encountered unexpected fault exception.", adminSyncException);
			}
		}

		private void ProcessSyncAdminAccountsResponse(FailedAdminAccounts syncAdminAccountsResponse, EhfCompanyAdmins requestAdmins, string operationName)
		{
			if (syncAdminAccountsResponse == null)
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Successfully completed SyncAdminAccounts operation. Sync details: <{0}>", new object[]
				{
					requestAdmins
				});
				return;
			}
			int num = 0;
			int num2 = 0;
			bool hasCriticalError = false;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Tenant: <{0}> ", new object[]
			{
				requestAdmins.TenantOU
			});
			stringBuilder.Append("SyncAdminAccountUserErrors: ");
			if (syncAdminAccountsResponse.FailedUsers != null)
			{
				EhfAdminAccountSynchronizer.ClassifyFailedResponse<string>(syncAdminAccountsResponse.FailedUsers, ref num, ref num2, ref hasCriticalError, stringBuilder);
			}
			stringBuilder.Append(" SyncAdminAccountGroupErrors: ");
			if (syncAdminAccountsResponse.FailedGroups != null)
			{
				EhfAdminAccountSynchronizer.ClassifyFailedResponse<Guid>(syncAdminAccountsResponse.FailedGroups, ref num, ref num2, ref hasCriticalError, stringBuilder);
			}
			string text = stringBuilder.ToString();
			this.HandleOperationFailureCounts(requestAdmins.EhfCompanyIdentity, operationName, (num > 0) ? 1 : 0, (num2 > 0) ? 1 : 0, text, hasCriticalError);
			this.LogAdminSyncOperationFailure(operationName, num, num2, text);
		}

		private bool InvokeSyncGroupUsers(EhfCompanyIdentity companyIdentity, EhfWellKnownGroup partnerGroup, string tenantOU)
		{
			string syncPartnerAdminGroupOperation = EhfAdminAccountSynchronizer.SyncPartnerAdminGroupOperation;
			if (partnerGroup == null)
			{
				return true;
			}
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Syncing partner admin group {0} for tenant {1}", new object[]
			{
				partnerGroup.ExternalDirectoryObjectId,
				tenantOU
			});
			string[] members = partnerGroup.GetWlidsOfGroupMembers(2000, base.DiagSession);
			Dictionary<string, ErrorInfo> syncGroupUsersResponse = null;
			FaultException syncGroupsException = null;
			base.InvokeProvisioningService(syncPartnerAdminGroupOperation, delegate
			{
				syncGroupUsersResponse = this.ProvisioningService.SyncGroupUsers(companyIdentity.EhfCompanyId, partnerGroup.ExternalDirectoryObjectId, members, out syncGroupsException);
			}, 1);
			if (syncGroupsException != null)
			{
				FaultException<InvalidGroupFault> faultException = syncGroupsException as FaultException<InvalidGroupFault>;
				if (faultException != null)
				{
					if (faultException.Detail.Code == InvalidGroupCode.GroupDoesNotExist)
					{
						base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "SyncGroupUsers returned GroupDoesNotExist fault while trying to sync an empty group <{0}> in company <{1}>.", new object[]
						{
							partnerGroup.WellKnownGroupName,
							tenantOU
						});
					}
					else if (faultException.Detail.Code == InvalidGroupCode.GroupBelongsToDifferentCompany || faultException.Detail.ErrorType == ErrorType.Transient)
					{
						this.HandleFaultAsTransientFailure<InvalidGroupFault>(companyIdentity, syncPartnerAdminGroupOperation, faultException.Detail.Code.ToString(), faultException, faultException.Detail.Code != InvalidGroupCode.GroupBelongsToDifferentCompany);
					}
					else
					{
						this.HandleFaultAsPermanentFailure<InvalidGroupFault>(syncPartnerAdminGroupOperation, faultException.Detail.Code.ToString(), faultException);
					}
				}
				else
				{
					this.HandleOperationLevelException(syncGroupsException, syncPartnerAdminGroupOperation, companyIdentity);
				}
				return false;
			}
			if (syncGroupUsersResponse == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string value in members)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(value);
				}
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Successfully completed SyncGroupUsers operation for partnerGroup <{0} : {1}>. Details: <{2}>; Members={3}", new object[]
				{
					partnerGroup.WellKnownGroupName,
					tenantOU,
					partnerGroup,
					stringBuilder.ToString()
				});
			}
			else
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append("SyncGroupUsers: ");
				int num = 0;
				int num2 = 0;
				bool hasCriticalError = false;
				EhfAdminAccountSynchronizer.ClassifyFailedResponse<string>(syncGroupUsersResponse, ref num, ref num2, ref hasCriticalError, stringBuilder2);
				string text = stringBuilder2.ToString();
				this.HandleOperationFailureCounts(companyIdentity, syncPartnerAdminGroupOperation, (num > 0) ? 1 : 0, (num2 > 0) ? 1 : 0, text, hasCriticalError);
				this.LogAdminSyncOperationFailure(syncPartnerAdminGroupOperation, num, num2, text);
			}
			return true;
		}

		private void HandleFaultAsPermanentFailure<TFault>(string operationName, string faultCode, FaultException<TFault> exception) where TFault : AdminServiceFault
		{
			this.errorTracker.AddPermanentFailure();
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			string format = "{0}({1})";
			object[] array = new object[2];
			array[0] = faultCode;
			object[] array2 = array;
			int num = 1;
			TFault detail = exception.Detail;
			array2[num] = detail.ErrorType;
			string exceptionReason = string.Format(invariantCulture, format, array);
			ExEventLog.EventTuple tuple_EhfAdminSyncPermanentFailure = EdgeSyncEventLogConstants.Tuple_EhfAdminSyncPermanentFailure;
			base.EventLogAndTraceException(operationName, tuple_EhfAdminSyncPermanentFailure, exception, exceptionReason);
			base.EhfConnection.PerfCounterHandler.OnOperationCommunicationFailure(operationName);
		}

		private void HandleFaultAsTransientFailure<TFault>(EhfCompanyIdentity companyIdentity, string operationName, string faultCode, FaultException<TFault> exception, bool isCriticalError) where TFault : AdminServiceFault
		{
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			string format = "{0}({1})";
			object[] array = new object[2];
			array[0] = faultCode;
			object[] array2 = array;
			int num = 1;
			TFault detail = exception.Detail;
			array2[num] = detail.ErrorType;
			string exceptionReason = string.Format(invariantCulture, format, array);
			this.HandleFaultAsTransientFailure(companyIdentity, operationName, exception, isCriticalError, exceptionReason);
		}

		private void HandleFaultAsTransientFailure(EhfCompanyIdentity companyIdentity, string operationName, Exception exception, bool isCriticalError, string exceptionReason)
		{
			ExEventLog.EventTuple tuple_EhfAdminSyncTransientFailure = EdgeSyncEventLogConstants.Tuple_EhfAdminSyncTransientFailure;
			if (!this.errorTracker.HasTransientFailure)
			{
				base.EventLogAndTraceException(operationName, tuple_EhfAdminSyncTransientFailure, exception, exceptionReason);
			}
			else
			{
				base.LogAndTraceException(operationName, exception, exceptionReason);
			}
			if (isCriticalError || companyIdentity == null)
			{
				this.errorTracker.AddCriticalFailure();
			}
			else
			{
				this.errorTracker.AddTransientFailure(companyIdentity, exception, operationName);
			}
			base.EhfConnection.PerfCounterHandler.OnOperationTransientFailure(operationName);
		}

		private void AddGroupToDeleteGroupsBatch(Guid externalDirectoryObjectId)
		{
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Adding Group <{0}> to Delete batch", new object[]
			{
				externalDirectoryObjectId
			});
			this.groupsToRemove.Add(externalDirectoryObjectId);
			if (this.groupsToRemove.Count == this.GetDeleteGroupsEffectiveBatchSize())
			{
				this.InvokeRemoveGroups();
				this.groupsToRemove.Clear();
			}
		}

		private int GetDeleteGroupsEffectiveBatchSize()
		{
			int num = Math.Min(base.Config.EhfSyncAppConfig.BatchSize, 1000);
			if (num <= 0)
			{
				num = 1;
			}
			return num;
		}

		private void InvokeRemoveGroups()
		{
			if (this.groupsToRemove.Count != 0)
			{
				if (this.groupsToRemove.Count > this.GetDeleteGroupsEffectiveBatchSize())
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "RemoveGroups batch size should not be greater than '{0}', but the batch size is '{1}'", new object[]
					{
						this.GetDeleteGroupsEffectiveBatchSize(),
						this.groupsToRemove.Count
					}));
				}
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Removing groups {0}", new object[]
				{
					this.groupsToRemove.Count
				});
				Dictionary<Guid, RemoveGroupErrorInfo> removeGroupsResponse = null;
				string removeGroupsOperation = EhfAdminAccountSynchronizer.RemoveGroupsOperation;
				FaultException removeGroupsException = null;
				base.InvokeProvisioningService(removeGroupsOperation, delegate
				{
					removeGroupsResponse = this.ProvisioningService.RemoveGroups(this.groupsToRemove.ToArray<Guid>(), out removeGroupsException);
				}, this.groupsToRemove.Count);
				if (removeGroupsException != null)
				{
					this.HandleOperationLevelException(removeGroupsException, removeGroupsOperation, null);
					return;
				}
				this.ProcessRemoveGroupsResponse(removeGroupsResponse, removeGroupsOperation);
			}
		}

		private void ProcessRemoveGroupsResponse(Dictionary<Guid, RemoveGroupErrorInfo> removeGroupsResponse, string operationName)
		{
			foreach (Guid guid in this.groupsToRemove)
			{
				if (removeGroupsResponse == null || !removeGroupsResponse.ContainsKey(guid))
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Successfully removed partner group <{0}>.", new object[]
					{
						guid
					});
				}
			}
			if (removeGroupsResponse != null)
			{
				int num = 0;
				int num2 = 0;
				foreach (KeyValuePair<Guid, RemoveGroupErrorInfo> keyValuePair in removeGroupsResponse)
				{
					RemoveGroupErrorInfo value = keyValuePair.Value;
					if (value.Code == RemoveGroupErrorCode.GroupDoesNotExist)
					{
						base.DiagSession.LogAndTraceError("Attempted to remove a partner group <{0}> which does not exists. Ignoring the item.", new object[]
						{
							keyValuePair.Key
						});
					}
					else
					{
						base.DiagSession.LogAndTraceError("Encountered <{0}>:<{1}> InternalServerError while trying to remove a partner group <{2}>.", new object[]
						{
							keyValuePair.Value.Code,
							keyValuePair.Value.ErrorType,
							keyValuePair.Key
						});
						if (value.ErrorType == ErrorType.Transient)
						{
							num++;
						}
						else
						{
							num2++;
						}
					}
				}
				string empty = string.Empty;
				this.HandleOperationFailureCounts(null, operationName, num, num2, empty, true);
				this.LogAdminSyncOperationFailure(operationName, num, num2, empty);
			}
		}

		private void HandleOperationFailureCounts(EhfCompanyIdentity companyIdentity, string operationName, int operationTransientFailuresCount, int operationPermanentFailuresCount, string failureDetails, bool hasCriticalError)
		{
			if (operationTransientFailuresCount != 0 || operationPermanentFailuresCount != 0)
			{
				base.EhfConnection.PerfCounterHandler.OnPerEntryFailures(operationName, operationTransientFailuresCount, operationPermanentFailuresCount);
			}
			if (operationTransientFailuresCount > 0)
			{
				if (hasCriticalError || companyIdentity == null)
				{
					this.errorTracker.AddCriticalFailure();
				}
				else
				{
					this.errorTracker.AddTransientFailure(companyIdentity, new EhfAdminAccountSynchronizer.EhfAdminSyncTransientException(failureDetails), operationName);
				}
			}
			if (operationPermanentFailuresCount > 0)
			{
				this.errorTracker.AddPermanentFailure();
			}
		}

		private void LogAdminSyncOperationFailure(string operationName, int transientFailure, int permanentFailure, string details)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "AdminSync operation <{0}> failed with <{1}> transient failure(s) and <{2}> permanent failure(s). Details: {3}", new object[]
			{
				operationName,
				transientFailure,
				permanentFailure,
				details
			});
			Exception exception = new EhfAdminAccountSynchronizer.EhfAdminSyncTransientResponseException(text);
			if (transientFailure > 0 && this.errorTracker.AllTransientFailuresCount == 1)
			{
				ExEventLog.EventTuple tuple_EhfAdminSyncTransientFailure = EdgeSyncEventLogConstants.Tuple_EhfAdminSyncTransientFailure;
				base.EventLogAndTraceException(operationName, tuple_EhfAdminSyncTransientFailure, exception, details);
				return;
			}
			if (permanentFailure > 0)
			{
				ExEventLog.EventTuple tuple_EhfAdminSyncPermanentFailure = EdgeSyncEventLogConstants.Tuple_EhfAdminSyncPermanentFailure;
				base.EventLogAndTraceException(operationName, tuple_EhfAdminSyncPermanentFailure, exception, details);
				return;
			}
			base.DiagSession.LogAndTraceError(text, new object[0]);
		}

		private void AbortSyncCycleIfTooManyFailures()
		{
			if (this.errorTracker.HasTooManyFailures)
			{
				this.LogAndAbortSyncCycle();
			}
		}

		private void ThrowIfAdminSyncNotEnabled(string dn, ExSearchResultEntry entry)
		{
			if (this.IsSyncEnabledCacheFull())
			{
				return;
			}
			if (!this.syncEnabledConfigCache[dn])
			{
				throw new InvalidOperationException(string.Format("Adminsync is not enabled for tenant <{0}>, but still got a change entry <{1}>", dn, entry.DistinguishedName));
			}
		}

		private void UpdateSyncStateInAD(EhfCompanyAdmins admins, EhfADAdapter configADAdapter, bool syncAdminAccountsCompleted, bool syncAdminAgentCompleted, bool syncHelpdeskAgentCompleted)
		{
			EhfAdminSyncState ehfAdminSyncState = EhfAdminSyncState.Create(admins, syncAdminAccountsCompleted, syncAdminAccountsCompleted, syncAdminAgentCompleted, syncHelpdeskAgentCompleted, base.EhfConnection);
			if (ehfAdminSyncState.IsEmpty)
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Admin state is up to date for <{0}>. No need to update the state.", new object[]
				{
					admins.TenantOU
				});
				return;
			}
			try
			{
				configADAdapter.SetAttributeValues(admins.EhfCompanyIdentity.EhfCompanyGuid, ehfAdminSyncState.GetStatesToUpdate());
			}
			catch (ExDirectoryException exception)
			{
				this.HandleFaultAsTransientFailure(admins.EhfCompanyIdentity, "Update Sync State", exception, true, string.Empty);
			}
		}

		private const int MaxCacheSize = 20000;

		private static readonly int OrganizationUnitDepth = new ADObjectId("OU=foo.com,OU=Microsoft Exchange Hosted Organizations").Depth + 1;

		private static readonly string[] TenantAdminsRoleGroupSyncAttributes = new string[]
		{
			"member",
			"msExchWindowsLiveID",
			"msExchProvisioningFlags"
		};

		private static readonly string[] TenantAdminsRoleGroupExtraAttributes = new string[]
		{
			"msExchOURoot",
			"msExchCU",
			"msExchExternalDirectoryObjectId"
		};

		private static readonly string[] AdminSyncAllAttributes = EhfSynchronizer.CombineArrays<string>(EhfAdminAccountSynchronizer.TenantAdminsRoleGroupSyncAttributes, EhfAdminAccountSynchronizer.TenantAdminsRoleGroupExtraAttributes);

		private static readonly string SyncAdminsOperation = "Sync Local Tenant Admins";

		private static readonly string SyncPartnerAdminGroupOperation = "Sync Partner Admin Groups";

		private static readonly string RemoveGroupsOperation = "Remove Admin Groups";

		private static int cycleCount;

		private Dictionary<string, EhfAdminSyncChangeBuilder> adminAccountChange = new Dictionary<string, EhfAdminSyncChangeBuilder>();

		private Dictionary<string, bool> syncEnabledConfigCache = new Dictionary<string, bool>();

		private HashSet<Guid> groupsToRemove = new HashSet<Guid>();

		private EhfSyncErrorTracker errorTracker;

		private class EhfAdminSyncTransientResponseException : Exception
		{
			public EhfAdminSyncTransientResponseException(string message) : base(message)
			{
			}
		}

		private class EhfAdminSyncTransientException : Exception
		{
			public EhfAdminSyncTransientException(string message) : base(message)
			{
			}
		}
	}
}
