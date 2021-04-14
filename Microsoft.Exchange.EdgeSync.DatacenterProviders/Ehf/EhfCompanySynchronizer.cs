using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Datacenter;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfCompanySynchronizer : EhfSynchronizer
	{
		public EhfCompanySynchronizer(EhfTargetConnection ehfConnection) : base(ehfConnection)
		{
		}

		private EhfConfigTargetConnection EhfConfigConnection
		{
			get
			{
				EhfConfigTargetConnection ehfConfigTargetConnection = base.EhfConnection as EhfConfigTargetConnection;
				if (ehfConfigTargetConnection == null)
				{
					throw new InvalidOperationException("EHfCompanySynchronizer should be using EhfConfigTargetConnection. But it is using connection of type " + base.EhfConnection.GetType());
				}
				return ehfConfigTargetConnection;
			}
		}

		public static TypeSynchronizer CreateTypeSynchronizer()
		{
			return new TypeSynchronizer(null, new PreDecorate(EhfCompanySynchronizer.LoadPerimeterSettings), null, null, null, null, "Perimeter Settings", Schema.Query.QueryPerimeterSettings, null, SearchScope.Subtree, EhfCompanySynchronizer.PerimeterSettingsSyncAttributes, null, false);
		}

		public static bool TryGetEhfAdminSyncState(string configUnitDN, EhfADAdapter adAdapter, EhfTargetConnection targetConnection, string missingIdAction, out EhfAdminSyncState ehfAdminSyncState)
		{
			ehfAdminSyncState = null;
			string[] adminSyncPerimeterSettingsAttributes = EhfCompanySynchronizer.AdminSyncPerimeterSettingsAttributes;
			ExSearchResultEntry exSearchResultEntry;
			if (!EhfCompanySynchronizer.TryGetPerimeterConfigEntry(configUnitDN, adAdapter, targetConnection.DiagSession, missingIdAction, adminSyncPerimeterSettingsAttributes, out exSearchResultEntry))
			{
				return false;
			}
			EhfCompanyIdentity ehfCompanyIdentity = EhfCompanySynchronizer.GetEhfCompanyIdentity(configUnitDN, targetConnection.DiagSession, missingIdAction, exSearchResultEntry);
			ehfAdminSyncState = EhfAdminSyncState.Create(ehfCompanyIdentity, exSearchResultEntry, targetConnection);
			return true;
		}

		public void CreateOrModifyEhfCompany(ExSearchResultEntry entry)
		{
			EhfCompanyItem ehfCompanyItem = EhfCompanyItem.CreateForActive(entry, base.DiagSession, base.Config.ResellerId);
			if (ehfCompanyItem.PreviouslySynced)
			{
				if (!ehfCompanyItem.EhfConfigSyncEnabled)
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Sync to EHF is disabled for <{0}>. Trying to disable and then delete the company. CompanyId:<{1}>; CompanyGuid:<{2}>;", new object[]
					{
						ehfCompanyItem.DistinguishedName,
						ehfCompanyItem.CompanyId,
						ehfCompanyItem.GetCompanyGuid()
					});
					this.AddCompanyToDisableBatch(ehfCompanyItem);
					return;
				}
				this.AddCompanyToUpdateBatch(ehfCompanyItem);
				this.CacheEhfCompanyId(ehfCompanyItem);
				return;
			}
			else
			{
				if (!ehfCompanyItem.EhfConfigSyncEnabled)
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Sync to EHF is disabled for <{0}>. Trying to delete the company if it is already present in EHF. CompanyGuid:<{1}>;", new object[]
					{
						ehfCompanyItem.DistinguishedName,
						ehfCompanyItem.GetCompanyGuid()
					});
					this.DeleteEhfCompanyWithoutId(ehfCompanyItem);
					return;
				}
				this.AddCompanyToCreateBatch(ehfCompanyItem);
				return;
			}
		}

		public void DeleteEhfCompany(ExSearchResultEntry entry)
		{
			EhfCompanyItem ehfCompanyItem = EhfCompanyItem.CreateForTombstone(entry, base.DiagSession);
			if (ehfCompanyItem.PreviouslySynced)
			{
				this.AddCompanyToDisableBatch(ehfCompanyItem);
				return;
			}
			this.DeleteEhfCompanyWithoutId(ehfCompanyItem);
		}

		public bool TryGetEhfCompanyIdentity(string configUnitDN, string missingIdAction, out EhfCompanyIdentity companyIdentity)
		{
			companyIdentity = null;
			if (this.configUnitsToCompanyIdentities != null && this.configUnitsToCompanyIdentities.TryGetValue(configUnitDN, out companyIdentity))
			{
				base.DiagSession.Tracer.TraceDebug<int, Guid, string>((long)base.DiagSession.GetHashCode(), "Successully retrieved cached EHF company ID {0} and company Guid {1} for ConfigUnit root DN <{2}>", companyIdentity.EhfCompanyId, companyIdentity.EhfCompanyGuid, configUnitDN);
				return true;
			}
			if (EhfCompanySynchronizer.TryGetEhfCompanyIdentity(configUnitDN, base.ADAdapter, base.EhfConnection, missingIdAction, out companyIdentity) && companyIdentity.EhfCompanyId != 0)
			{
				this.CacheEhfCompanyIdentity(configUnitDN, companyIdentity);
				base.DiagSession.Tracer.TraceDebug<int, Guid, string>((long)base.DiagSession.GetHashCode(), "Successully retrieved EHF company ID {0} and companyGuid {1} for ConfigUnit root DN <{2}>", companyIdentity.EhfCompanyId, companyIdentity.EhfCompanyGuid, configUnitDN);
				return true;
			}
			return false;
		}

		public override void ClearBatches()
		{
			this.companiesToCreate = null;
			this.companiesToUpdate = null;
			this.companiesToDisable = null;
			this.companiesToDelete = null;
			this.companiesToCreateLast = null;
			this.configUnitsToCompanyIdentities = null;
			base.ClearBatches();
		}

		public override bool FlushBatches()
		{
			this.InvokeEhfDisableCompanies();
			this.InvokeEhfDeleteCompanies();
			this.handledAllDeletedCompanies = true;
			if (this.companiesToCreateLast != null)
			{
				foreach (EhfCompanyItem company in this.companiesToCreateLast)
				{
					this.AddCompanyToCreateBatch(company);
				}
				this.companiesToCreateLast = null;
			}
			for (int i = 0; i < 2; i++)
			{
				this.InvokeEhfCreateCompanies();
				this.InvokeEhfUpdateCompanies();
			}
			return base.FlushBatches();
		}

		private static bool LoadPerimeterSettings(ExSearchResultEntry entry, Connection sourceConnection, TargetConnection targetConnection, object state)
		{
			return EhfSynchronizer.LoadFullEntry(entry, EhfCompanySynchronizer.PerimeterSettingsAllAttributes, (EhfTargetConnection)targetConnection);
		}

		private static bool TryGetPerimeterConfigEntry(string configUnitDN, EhfADAdapter adAdapter, EdgeSyncDiag diagSession, string missingIdAction, string[] attributes, out ExSearchResultEntry perimeterSettingsEntry)
		{
			ADObjectId perimeterConfigObjectIdFromConfigUnitId = EhfTargetConnection.GetPerimeterConfigObjectIdFromConfigUnitId(new ADObjectId(configUnitDN));
			perimeterSettingsEntry = adAdapter.ReadObjectEntry(perimeterConfigObjectIdFromConfigUnitId.DistinguishedName, false, attributes);
			if (perimeterSettingsEntry == null)
			{
				diagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Unable to read Perimeter Settings object for ConfigUnit root DN <{0}>; {1}", new object[]
				{
					configUnitDN,
					missingIdAction
				});
				return false;
			}
			return true;
		}

		private static EhfCompanyIdentity GetEhfCompanyIdentity(string configUnitDN, EdgeSyncDiag diagSession, string missingIdAction, ExSearchResultEntry perimeterSettingsEntry)
		{
			int companyId;
			string text;
			if (!EhfCompanyItem.TryGetEhfCompanyId(perimeterSettingsEntry, diagSession, out companyId, out text))
			{
				if (string.IsNullOrEmpty(text))
				{
					diagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "EHF company ID is not set for tenant organization with ConfigUnit root DN <{0}>; {1}", new object[]
					{
						configUnitDN,
						missingIdAction
					});
				}
				else
				{
					diagSession.LogAndTraceError("Failure occurred while retrieving EHF company ID for tenant organization with ConfigUnit root DN <{0}>; {1}; failure details: {2}", new object[]
					{
						configUnitDN,
						missingIdAction,
						text
					});
				}
			}
			Guid objectGuid = perimeterSettingsEntry.GetObjectGuid();
			return new EhfCompanyIdentity(companyId, objectGuid);
		}

		private static bool TryGetEhfCompanyIdentity(string configUnitDN, EhfADAdapter adAdapter, EhfTargetConnection targetConnection, string missingIdAction, out EhfCompanyIdentity ehfCompanyIdentity)
		{
			ehfCompanyIdentity = null;
			string[] perimeterSettingsCompanyIdentityAttributes = EhfCompanySynchronizer.PerimeterSettingsCompanyIdentityAttributes;
			ExSearchResultEntry perimeterSettingsEntry;
			if (!EhfCompanySynchronizer.TryGetPerimeterConfigEntry(configUnitDN, adAdapter, targetConnection.DiagSession, missingIdAction, perimeterSettingsCompanyIdentityAttributes, out perimeterSettingsEntry))
			{
				return false;
			}
			ehfCompanyIdentity = EhfCompanySynchronizer.GetEhfCompanyIdentity(configUnitDN, targetConnection.DiagSession, missingIdAction, perimeterSettingsEntry);
			return true;
		}

		private void AddCompanyToCreateBatch(EhfCompanyItem company)
		{
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Adding company <{0}> to Create batch", new object[]
			{
				company.DistinguishedName
			});
			if (base.AddItemToBatch<EhfCompanyItem>(company, ref this.companiesToCreate))
			{
				this.InvokeEhfCreateCompanies();
			}
		}

		private void AddCompanyToUpdateBatch(EhfCompanyItem company)
		{
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Adding company <{0}> to Update batch", new object[]
			{
				company.DistinguishedName
			});
			if (base.AddItemToBatch<EhfCompanyItem>(company, ref this.companiesToUpdate))
			{
				this.InvokeEhfUpdateCompanies();
			}
		}

		private void AddCompanyToDisableBatch(EhfCompanyItem company)
		{
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Adding company <{0}> to Disable batch", new object[]
			{
				company.DistinguishedName
			});
			if (base.AddItemToBatch<EhfCompanyItem>(company, ref this.companiesToDisable))
			{
				this.InvokeEhfDisableCompanies();
			}
		}

		private void AddCompanyToDeleteBatch(EhfCompanyItem company)
		{
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Adding company <{0}> to Delete batch", new object[]
			{
				company.DistinguishedName
			});
			if (base.AddItemToBatch<EhfCompanyItem>(company, ref this.companiesToDelete))
			{
				this.InvokeEhfDeleteCompanies();
			}
		}

		private void InvokeEhfCreateCompanies()
		{
			if (this.companiesToCreate == null || this.companiesToCreate.Count == 0)
			{
				return;
			}
			CompanyResponseInfoSet responseSet = null;
			base.InvokeProvisioningService("Create Company", delegate
			{
				responseSet = this.ProvisioningService.CreateCompanies(this.companiesToCreate);
			}, this.companiesToCreate.Count);
			List<EhfCompanyItem> list = new List<EhfCompanyItem>(base.Config.EhfSyncAppConfig.BatchSize);
			int num = 0;
			int permanentFailureCount = 0;
			for (int i = 0; i < responseSet.ResponseInfo.Length; i++)
			{
				CompanyResponseInfo companyResponseInfo = responseSet.ResponseInfo[i];
				EhfCompanyItem ehfCompanyItem = this.companiesToCreate[i];
				bool flag = false;
				if (companyResponseInfo.Status == ResponseStatus.Success)
				{
					flag = true;
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Successfully created EHF company: DN=<{0}>; Name=<{1}>; GUID=<{2}>; EHF-ID=<{3}>", new object[]
					{
						ehfCompanyItem.DistinguishedName,
						ehfCompanyItem.Company.Name,
						companyResponseInfo.CompanyGuid.Value,
						companyResponseInfo.CompanyId
					});
				}
				else if (companyResponseInfo.Fault.Id == FaultId.CompanyExistsUnderThisReseller)
				{
					Guid companyGuid = ehfCompanyItem.GetCompanyGuid();
					if (companyResponseInfo.CompanyGuid.Value.Equals(companyGuid))
					{
						flag = true;
						base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Attempted to create a company that already exists: DN=<{0}>; Name=<{1}>; GUID=<{2}>; EHF-ID=<{3}>", new object[]
						{
							ehfCompanyItem.DistinguishedName,
							ehfCompanyItem.Company.Name,
							companyGuid,
							companyResponseInfo.CompanyId
						});
					}
					else if (!this.handledAllDeletedCompanies)
					{
						base.AddItemToLazyList<EhfCompanyItem>(ehfCompanyItem, ref this.companiesToCreateLast);
						base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Attempted to create a company with the name that already exists but a different GUID: DN=<{0}>; Name=<{1}>; AD-GUID=<{2}>; EHF-GUID=<{3}>; EHF-ID=<{4}>", new object[]
						{
							ehfCompanyItem.DistinguishedName,
							ehfCompanyItem.Company.Name,
							companyGuid,
							companyResponseInfo.CompanyGuid.Value,
							companyResponseInfo.CompanyId
						});
					}
					else
					{
						this.HandleFailedCompany("Create Company", ehfCompanyItem, companyResponseInfo, ref num, ref permanentFailureCount);
					}
				}
				else
				{
					this.HandleFailedCompany("Create Company", ehfCompanyItem, companyResponseInfo, ref num, ref permanentFailureCount);
				}
				if (flag)
				{
					EhfADResultCode ehfADResultCode = ehfCompanyItem.TryStoreEhfCompanyId(companyResponseInfo.CompanyId, base.ADAdapter);
					switch (ehfADResultCode)
					{
					case EhfADResultCode.Failure:
						num++;
						break;
					case EhfADResultCode.NoSuchObject:
						base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Unable to store EHF company ID {0} in AD for Perimeter Settings object <{1}> because the AD object has been deleted; ignoring the object", new object[]
						{
							companyResponseInfo.CompanyId,
							ehfCompanyItem.DistinguishedName
						});
						break;
					case EhfADResultCode.Success:
						list.Add(ehfCompanyItem);
						this.CacheEhfCompanyId(ehfCompanyItem);
						break;
					default:
						throw new InvalidOperationException("Unexpected EhfADResultCode " + ehfADResultCode);
					}
				}
				if (!ehfCompanyItem.EventLogAndTryStoreSyncErrors(base.ADAdapter))
				{
					num++;
				}
			}
			base.HandlePerEntryFailureCounts("Create Company", this.companiesToCreate.Count, num, permanentFailureCount, true);
			this.companiesToCreate.Clear();
			foreach (EhfCompanyItem ehfCompanyItem2 in list)
			{
				base.DiagSession.Tracer.TraceDebug<int, string>((long)base.DiagSession.GetHashCode(), "Adding newly-created company ID=<{0}> DN=<{1}> to the update batch", ehfCompanyItem2.CompanyId, ehfCompanyItem2.DistinguishedName);
				this.AddCompanyToUpdateBatch(ehfCompanyItem2);
			}
		}

		private void InvokeEhfUpdateCompanies()
		{
			if (this.companiesToUpdate == null || this.companiesToUpdate.Count == 0)
			{
				return;
			}
			CompanyResponseInfoSet responseSet = null;
			base.InvokeProvisioningService("Update Company", delegate
			{
				responseSet = this.ProvisioningService.UpdateCompanies(this.companiesToUpdate);
			}, this.companiesToUpdate.Count);
			int num = 0;
			int permanentFailureCount = 0;
			List<EhfCompanyItem> list = new List<EhfCompanyItem>();
			for (int i = 0; i < responseSet.ResponseInfo.Length; i++)
			{
				CompanyResponseInfo companyResponseInfo = responseSet.ResponseInfo[i];
				EhfCompanyItem ehfCompanyItem = this.companiesToUpdate[i];
				if (companyResponseInfo.Status == ResponseStatus.Success)
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Successfully updated EHF company: DN=<{0}>; GUID=<{1}>; EHF-ID=<{2}>", new object[]
					{
						ehfCompanyItem.DistinguishedName,
						companyResponseInfo.CompanyGuid.Value,
						companyResponseInfo.CompanyId
					});
					if (ehfCompanyItem.TryUpdateAdminSyncEnabledFlag(base.ADAdapter, false) != EhfADResultCode.Success)
					{
						num++;
					}
				}
				else if (companyResponseInfo.Fault.Id == FaultId.CompanyDoesNotExist)
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Attempted to update a company that does not exists in EHF: DN=<{0}>; GUID=<{1}>; EHF-ID=<{2}>", new object[]
					{
						ehfCompanyItem.DistinguishedName,
						ehfCompanyItem.GetCompanyGuid(),
						companyResponseInfo.CompanyId
					});
					list.Add(ehfCompanyItem);
				}
				else
				{
					this.HandleFailedCompany("Update Company", ehfCompanyItem, companyResponseInfo, ref num, ref permanentFailureCount);
				}
				if (!ehfCompanyItem.EventLogAndTryStoreSyncErrors(base.ADAdapter))
				{
					num++;
				}
			}
			base.HandlePerEntryFailureCounts("Create Company", this.companiesToUpdate.Count, num, permanentFailureCount, false);
			foreach (EhfCompanyItem ehfCompanyItem2 in this.companiesToUpdate)
			{
				if (ehfCompanyItem2.IsForcedDomainSyncRequired())
				{
					this.EhfConfigConnection.AddDomainsForNewCompany(ehfCompanyItem2);
				}
			}
			this.companiesToUpdate.Clear();
			foreach (EhfCompanyItem ehfCompanyItem3 in list)
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Trying to re-create the company <{0}>. CompanyId:<{1}>", new object[]
				{
					ehfCompanyItem3.DistinguishedName,
					ehfCompanyItem3.CompanyId
				});
				ehfCompanyItem3.ADEntry.Attributes.Remove("msExchTenantPerimeterSettingsOrgID");
				EhfCompanyItem company = EhfCompanyItem.CreateForActive(ehfCompanyItem3.ADEntry, base.DiagSession, base.Config.ResellerId);
				this.AddCompanyToCreateBatch(company);
			}
		}

		private void InvokeEhfDisableCompanies()
		{
			if (this.companiesToDisable == null || this.companiesToDisable.Count == 0)
			{
				return;
			}
			CompanyResponseInfoSet responseSet = null;
			base.InvokeProvisioningService("Disable Company", delegate
			{
				responseSet = this.ProvisioningService.DisableCompanies(this.companiesToDisable);
			}, this.companiesToDisable.Count);
			List<EhfCompanyItem> list = new List<EhfCompanyItem>(base.Config.EhfSyncAppConfig.BatchSize);
			int num = 0;
			int permanentFailureCount = 0;
			for (int i = 0; i < responseSet.ResponseInfo.Length; i++)
			{
				CompanyResponseInfo companyResponseInfo = responseSet.ResponseInfo[i];
				EhfCompanyItem ehfCompanyItem = this.companiesToDisable[i];
				if (companyResponseInfo.Status == ResponseStatus.Success)
				{
					list.Add(ehfCompanyItem);
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Successfully disabled EHF company: DN=<{0}>; GUID=<{1}>; EHF-ID=<{2}>", new object[]
					{
						ehfCompanyItem.DistinguishedName,
						companyResponseInfo.CompanyGuid.Value,
						companyResponseInfo.CompanyId
					});
				}
				else if (companyResponseInfo.Fault.Id == FaultId.CompanyDoesNotExist)
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Attempted to disable a company that does not exist; ignoring the company: DN=<{0}>; GUID=<{1}>; EHF-ID=<{2}>", new object[]
					{
						ehfCompanyItem.DistinguishedName,
						ehfCompanyItem.GetCompanyGuid(),
						ehfCompanyItem.CompanyId
					});
					if (!ehfCompanyItem.ADEntry.IsDeleted)
					{
						if (ehfCompanyItem.TryClearEhfCompanyIdAndDisableAdminSync(base.ADAdapter) == EhfADResultCode.Failure)
						{
							num++;
						}
						else
						{
							this.RemoveCompanyItemFromCache(ehfCompanyItem);
						}
					}
				}
				else
				{
					this.HandleFailedCompany("Disable Company", ehfCompanyItem, companyResponseInfo, ref num, ref permanentFailureCount);
				}
				if (!ehfCompanyItem.EventLogAndTryStoreSyncErrors(base.ADAdapter))
				{
					throw new InvalidOperationException("EventLogAndTryStoreSyncErrors() returned false for a deleted object");
				}
			}
			base.HandlePerEntryFailureCounts("Disable Company", this.companiesToDisable.Count, num, permanentFailureCount, false);
			this.companiesToDisable.Clear();
			foreach (EhfCompanyItem ehfCompanyItem2 in list)
			{
				base.DiagSession.Tracer.TraceDebug<int, string>((long)base.DiagSession.GetHashCode(), "Adding disabled company ID=<{0}> DN=<{1}> to the delete batch", ehfCompanyItem2.CompanyId, ehfCompanyItem2.DistinguishedName);
				this.AddCompanyToDeleteBatch(ehfCompanyItem2);
			}
		}

		private void RemoveCompanyItemFromCache(EhfCompanyItem company)
		{
			if (this.configUnitsToCompanyIdentities != null)
			{
				this.configUnitsToCompanyIdentities.Remove(company.GetConfigUnitDN());
			}
		}

		private void InvokeEhfDeleteCompanies()
		{
			if (this.companiesToDelete == null || this.companiesToDelete.Count == 0)
			{
				return;
			}
			CompanyResponseInfoSet responseSet = null;
			base.InvokeProvisioningService("Delete Company", delegate
			{
				responseSet = this.ProvisioningService.DeleteCompanies(this.companiesToDelete);
			}, this.companiesToDelete.Count);
			int num = 0;
			int permanentFailureCount = 0;
			for (int i = 0; i < responseSet.ResponseInfo.Length; i++)
			{
				CompanyResponseInfo companyResponseInfo = responseSet.ResponseInfo[i];
				EhfCompanyItem ehfCompanyItem = this.companiesToDelete[i];
				bool flag = false;
				if (companyResponseInfo.Status == ResponseStatus.Success)
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Successfully deleted EHF company: DN=<{0}>; GUID=<{1}>; EHF-ID=<{2}>", new object[]
					{
						ehfCompanyItem.DistinguishedName,
						companyResponseInfo.CompanyGuid.Value,
						companyResponseInfo.CompanyId
					});
					flag = true;
				}
				else if (companyResponseInfo.Fault.Id == FaultId.CompanyDoesNotExist)
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Attempted to delete a company that does not exist; ignoring the company: DN=<{0}>; GUID=<{1}>; EHF-ID=<{2}>", new object[]
					{
						ehfCompanyItem.DistinguishedName,
						ehfCompanyItem.GetCompanyGuid(),
						ehfCompanyItem.CompanyId
					});
					flag = true;
				}
				else
				{
					this.HandleFailedCompany("Delete Company", ehfCompanyItem, companyResponseInfo, ref num, ref permanentFailureCount);
					if (!ehfCompanyItem.ADEntry.IsDeleted && ehfCompanyItem.TryUpdateAdminSyncEnabledFlag(base.ADAdapter, true) == EhfADResultCode.Failure)
					{
						num++;
					}
				}
				if (!ehfCompanyItem.ADEntry.IsDeleted && flag)
				{
					if (ehfCompanyItem.TryClearEhfCompanyIdAndDisableAdminSync(base.ADAdapter) == EhfADResultCode.Failure)
					{
						num++;
					}
					else
					{
						this.RemoveCompanyItemFromCache(ehfCompanyItem);
					}
				}
				if (!ehfCompanyItem.EventLogAndTryStoreSyncErrors(base.ADAdapter))
				{
					throw new InvalidOperationException("EventLogAndTryStoreSyncErrors() returned false for a deleted object");
				}
			}
			base.HandlePerEntryFailureCounts("Delete Company", this.companiesToDelete.Count, num, permanentFailureCount, false);
			this.companiesToDelete.Clear();
		}

		private void DeleteEhfCompanyWithoutId(EhfCompanyItem companyItem)
		{
			Company company = null;
			bool companyExists = false;
			Guid companyGuid = companyItem.GetCompanyGuid();
			base.InvokeProvisioningService("Get Company By Guid", delegate
			{
				companyExists = this.ProvisioningService.TryGetCompanyByGuid(companyGuid, out company);
			}, 1);
			if (!companyExists)
			{
				if (!companyItem.EventLogAndTryStoreSyncErrors(base.ADAdapter))
				{
					throw new InvalidOperationException("EventLogAndTryStoreSyncErrors() returned false for a deleted object");
				}
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "EHF company for AD object <{0}> (GUID=<{1}>) has never been created; ignoring the deleted entry", new object[]
				{
					companyItem.DistinguishedName,
					companyGuid
				});
				return;
			}
			else
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "EHF company for AD object <{0}> (GUID=<{1}>) has been retrieved from EHF by GUID", new object[]
				{
					companyItem.DistinguishedName,
					companyGuid
				});
				companyItem.RecoverLostCompanyId(company);
				if (company.IsEnabled)
				{
					this.AddCompanyToDisableBatch(companyItem);
					return;
				}
				this.AddCompanyToDeleteBatch(companyItem);
				return;
			}
		}

		private void HandleFailedCompany(string operationName, EhfCompanyItem company, CompanyResponseInfo response, ref int transientFailureCount, ref int permanentFailureCount)
		{
			if (response.Status == ResponseStatus.TransientFailure)
			{
				transientFailureCount++;
			}
			else
			{
				permanentFailureCount++;
			}
			string text = (company.Company != null) ? company.Company.Name : "not available";
			string responseTargetValuesString = EhfProvisioningService.GetResponseTargetValuesString(response);
			EdgeSyncDiag diagSession = base.DiagSession;
			string messageFormat = "Failed to {0}: FaultId={1}; FaultType={2}; FaultDetail={3}; Target={4}; TargetValues=({5}); DN=({6}); AD-Name={7}; EHF-Name={8}; AD-GUID=({9}); EHF-GUID=({10}); EHF-ID={11}";
			object[] array = new object[12];
			array[0] = operationName;
			array[1] = response.Fault.Id;
			array[2] = response.Status;
			array[3] = (response.Fault.Detail ?? "null");
			array[4] = response.Target;
			array[5] = responseTargetValuesString;
			array[6] = company.DistinguishedName;
			array[7] = text;
			array[8] = (response.CompanyName ?? "null");
			array[9] = company.GetCompanyGuid();
			object[] array2 = array;
			int num = 10;
			Guid? companyGuid = response.CompanyGuid;
			array2[num] = ((companyGuid != null) ? companyGuid.GetValueOrDefault() : "null");
			array[11] = response.CompanyId;
			company.AddSyncError(diagSession.LogAndTraceError(messageFormat, array));
		}

		private void CacheEhfCompanyIdentity(string configUnitDN, int ehfCompanyId, Guid ehfCompanyGuid)
		{
			this.CacheEhfCompanyIdentity(configUnitDN, new EhfCompanyIdentity(ehfCompanyId, ehfCompanyGuid));
		}

		private void CacheEhfCompanyIdentity(string configUnitDN, EhfCompanyIdentity identity)
		{
			if (identity.EhfCompanyId == 0)
			{
				throw new ArgumentOutOfRangeException("ehfCompanyId", identity.EhfCompanyId, "Company ID must be valid");
			}
			if (identity.EhfCompanyGuid == Guid.Empty)
			{
				throw new ArgumentException("ehfCompanyGuid cannot be empty", "ehfCompanyGuid");
			}
			if (this.configUnitsToCompanyIdentities == null)
			{
				this.configUnitsToCompanyIdentities = new Dictionary<string, EhfCompanyIdentity>();
			}
			this.configUnitsToCompanyIdentities[configUnitDN] = identity;
		}

		private void CacheEhfCompanyId(EhfCompanyItem companyItem)
		{
			this.CacheEhfCompanyIdentity(companyItem.GetConfigUnitDN(), companyItem.CompanyId, companyItem.GetCompanyGuid());
		}

		internal const string CreateCompanyOperation = "Create Company";

		internal const string UpdateCompanyOperation = "Update Company";

		internal const string DisableCompanyOperation = "Disable Company";

		internal const string DeleteCompanyOperation = "Delete Company";

		internal const string GetCompanyByGuidOperation = "Get Company By Guid";

		private static readonly string[] PerimeterSettingsSyncAttributes = new string[]
		{
			"msExchTenantPerimeterSettingsFlags",
			"msExchTenantPerimeterSettingsGatewayIPAddresses",
			"msExchTenantPerimeterSettingsInternalServerIPAddresses"
		};

		private static readonly string[] PerimeterSettingsExtraAttributes = new string[]
		{
			"msExchTenantPerimeterSettingsOrgID",
			"msExchTransportInboundSettings",
			"objectGUID",
			"msExchEdgeSyncCookies",
			"msExchOURoot"
		};

		private static readonly string[] PerimeterSettingsCompanyIdentityAttributes = new string[]
		{
			"objectGUID",
			"msExchTenantPerimeterSettingsOrgID"
		};

		private static readonly string[] AdminSyncPerimeterSettingsAttributes = new string[]
		{
			"objectGUID",
			"msExchTenantPerimeterSettingsOrgID",
			"msExchTargetServerAdmins",
			"msExchTargetServerViewOnlyAdmins",
			"msExchTargetServerPartnerAdmins",
			"msExchTargetServerPartnerViewOnlyAdmins"
		};

		private static readonly string[] PerimeterSettingsAllAttributes = EhfSynchronizer.CombineArrays<string>(EhfCompanySynchronizer.PerimeterSettingsSyncAttributes, EhfCompanySynchronizer.PerimeterSettingsExtraAttributes);

		private List<EhfCompanyItem> companiesToCreate;

		private List<EhfCompanyItem> companiesToUpdate;

		private List<EhfCompanyItem> companiesToDisable;

		private List<EhfCompanyItem> companiesToDelete;

		private List<EhfCompanyItem> companiesToCreateLast;

		private bool handledAllDeletedCompanies;

		private Dictionary<string, EhfCompanyIdentity> configUnitsToCompanyIdentities;
	}
}
