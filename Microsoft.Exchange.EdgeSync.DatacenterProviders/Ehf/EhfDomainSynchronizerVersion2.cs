using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Datacenter;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfDomainSynchronizerVersion2 : EhfDomainSynchronizer
	{
		public EhfDomainSynchronizerVersion2(EhfTargetConnection ehfConnection) : base(ehfConnection)
		{
		}

		public override void HandleAddedDomain(ExSearchResultEntry entry)
		{
			base.AddDomainToCreateBatch(this.CreateEhfDomainItem(entry));
		}

		public override void HandleModifiedDomain(ExSearchResultEntry entry)
		{
			this.AddDomainToUpdateBatch(this.CreateEhfDomainItem(entry));
		}

		public override void HandleDeletedDomain(ExSearchResultEntry entry)
		{
			base.HandleDeletedDomain(entry);
			EhfDomainItem domain = EhfDomainItemVersion2.CreateForOutboundOnlyTombstone(entry, base.DiagSession);
			base.AddDomainToDisableBatch(domain);
		}

		public override bool FlushBatches()
		{
			this.InvokeEhfUpdateDomains(false);
			bool flag = base.FlushBatches();
			if (flag)
			{
				base.InvokeEhfCreateDomains();
				this.InvokeEhfUpdateDomains(true);
			}
			return flag;
		}

		protected override EhfDomainItem CreateDomainItemForNewCompany(ExSearchResultEntry entry, int ehfCompanyId)
		{
			return EhfDomainItemVersion2.CreateEhfDomainItem(entry, ehfCompanyId, this.GetMailFlowPartnerConnectorIds(entry), base.DiagSession);
		}

		protected override void HandleCreateDomainResponse(DomainResponseInfoSet responseSet, List<EhfDomainItem> domainsTriedToCreate)
		{
			int num = 0;
			int permanentFailureCount = 0;
			List<EhfDomainItemVersion2> list = new List<EhfDomainItemVersion2>();
			List<EhfDomainItemVersion2> list2 = new List<EhfDomainItemVersion2>();
			for (int i = 0; i < responseSet.ResponseInfo.Length; i++)
			{
				DomainResponseInfo domainResponseInfo = responseSet.ResponseInfo[i];
				EhfDomainItemVersion2 ehfDomainItemVersion = domainsTriedToCreate[i] as EhfDomainItemVersion2;
				if (ehfDomainItemVersion == null)
				{
					throw new InvalidOperationException("Version 2 of EhfConnector should be using version 2 of EhfDomainItem");
				}
				if (domainResponseInfo.Status == ResponseStatus.Success)
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Successfully created EHF domain: DN=<{0}>; Name=<{1}>; GUID=<{2}>; EHF-Company-ID=<{3}>", new object[]
					{
						ehfDomainItemVersion.DistinguishedName,
						ehfDomainItemVersion.Domain.Name,
						domainResponseInfo.DomainGuid.Value,
						ehfDomainItemVersion.Domain.CompanyId
					});
					if (ehfDomainItemVersion.TryClearForceDomainSyncFlagFromPerimeterConfig(base.ADAdapter) == EhfADResultCode.Failure)
					{
						num++;
					}
					if (!this.UpdateDuplicateDetectedFlag(ehfDomainItemVersion))
					{
						num++;
					}
				}
				else if (domainResponseInfo.Fault.Id == FaultId.DomainExistUnderThisCompany || domainResponseInfo.Fault.Id == FaultId.DomainExistOutsideThisCompany)
				{
					Guid domainGuid = ehfDomainItemVersion.GetDomainGuid();
					if (domainResponseInfo.Fault.Id == FaultId.DomainExistUnderThisCompany && domainResponseInfo.DomainGuid.Value.Equals(domainGuid))
					{
						if (ehfDomainItemVersion.TriedToUpdateDomain && !ehfDomainItemVersion.DomainCreatedWithGuid)
						{
							base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Attempted to create a domain that already exists. Treating this as error since update is already called: DN=<{0}>; Name=<{1}>; GUID=<{2}>; EHF-Company-ID=<{3}>", new object[]
							{
								ehfDomainItemVersion.DistinguishedName,
								ehfDomainItemVersion.Domain.Name,
								domainGuid,
								ehfDomainItemVersion.Domain.CompanyId
							});
							base.HandleFailedDomain("Create Domain", ehfDomainItemVersion, domainResponseInfo, ref num, ref permanentFailureCount);
						}
						else
						{
							base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Attempted to create a domain that already exists. Trying to update the domain. DN=<{0}>; Name=<{1}>; GUID=<{2}>; EHF-Company-ID=<{3}>", new object[]
							{
								ehfDomainItemVersion.DistinguishedName,
								ehfDomainItemVersion.Domain.Name,
								domainGuid,
								ehfDomainItemVersion.Domain.CompanyId
							});
							list.Add(ehfDomainItemVersion);
						}
					}
					else if (domainResponseInfo.Fault.Id == FaultId.DomainExistUnderThisCompany && domainResponseInfo.SourceId == ProvisioningSource.Other && !ehfDomainItemVersion.DomainCreatedWithGuid)
					{
						base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "A domain collision is detected for domain: DN=<{0}>; Name=<{1}>; AD-GUID=<{2}>; EHF-Company-ID=<{3}>. Trying to create the domain as OutboundOnly domain.", new object[]
						{
							ehfDomainItemVersion.DistinguishedName,
							ehfDomainItemVersion.Name,
							domainGuid,
							ehfDomainItemVersion.Domain.CompanyId
						});
						list2.Add(ehfDomainItemVersion);
					}
					else if (!base.HandledAllDeletedDomains)
					{
						base.AddDomainToCreateLastList(ehfDomainItemVersion);
						EdgeSyncDiag diagSession = base.DiagSession;
						EdgeSyncLoggingLevel level = EdgeSyncLoggingLevel.Low;
						string messageFormat = "Attempted to create a domain with the name that already exists but a different GUID: DN=<{0}>; Name=<{1}>; AD-GUID=<{2}>; EHF-GUID=<{3}>; EHF-Company-ID=<{4}>";
						object[] array = new object[5];
						array[0] = ehfDomainItemVersion.DistinguishedName;
						array[1] = ehfDomainItemVersion.Name;
						array[2] = domainGuid;
						object[] array2 = array;
						int num2 = 3;
						Guid? domainGuid2 = domainResponseInfo.DomainGuid;
						array2[num2] = ((domainGuid2 != null) ? domainGuid2.GetValueOrDefault() : "null");
						array[4] = ehfDomainItemVersion.Domain.CompanyId;
						diagSession.LogAndTraceInfo(level, messageFormat, array);
					}
					else if (!ehfDomainItemVersion.DomainCreatedWithGuid && domainResponseInfo.Fault.Id == FaultId.DomainExistOutsideThisCompany)
					{
						base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "A domain collision is detected for domain: DN=<{0}>; Name=<{1}>; AD-GUID=<{2}>; EHF-Company-ID=<{3}>. Trying to create the domain as OutboundOnly domain.", new object[]
						{
							ehfDomainItemVersion.DistinguishedName,
							ehfDomainItemVersion.Name,
							domainGuid,
							ehfDomainItemVersion.Domain.CompanyId
						});
						list2.Add(ehfDomainItemVersion);
					}
					else
					{
						base.HandleFailedDomain("Create Domain", ehfDomainItemVersion, domainResponseInfo, ref num, ref permanentFailureCount);
					}
				}
				else if (domainResponseInfo.Fault.Id == FaultId.DomainGuidIsNotUnique && !ehfDomainItemVersion.TriedToUpdateDomain)
				{
					list.Add(ehfDomainItemVersion);
				}
				else
				{
					base.HandleFailedDomain("Create Domain", ehfDomainItemVersion, domainResponseInfo, ref num, ref permanentFailureCount);
				}
				if (!ehfDomainItemVersion.EventLogAndTryStoreSyncErrors(base.ADAdapter))
				{
					num++;
				}
			}
			base.HandlePerEntryFailureCounts("Create Domain", domainsTriedToCreate.Count, num, permanentFailureCount, false);
			foreach (EhfDomainItemVersion2 domain in list)
			{
				this.AddDomainToUpdateBatch(domain);
			}
			foreach (EhfDomainItemVersion2 domain2 in list2)
			{
				this.AddDomainToCreateWithGuidBatch(domain2);
			}
		}

		private static bool TryGetMailFlowConnectorId(ExSearchResultEntry entry, string attribute, EdgeSyncDiag diagSession, out int connectorId)
		{
			connectorId = EhfDomainSynchronizerVersion2.InvalidConnectorId;
			DirectoryAttribute attribute2 = entry.GetAttribute(attribute);
			if (attribute2 == null)
			{
				diagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.High, "ConnectorId is null or empty for <{0}>:<{1}>", new object[]
				{
					attribute,
					entry.DistinguishedName
				});
				return false;
			}
			string text = (string)attribute2[0];
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if (int.TryParse(text, out connectorId))
			{
				return true;
			}
			diagSession.LogAndTraceError("ConnectorId '{0}' is not in the expected format for <{1}>:<{2}>", new object[]
			{
				text,
				attribute,
				entry.DistinguishedName
			});
			return false;
		}

		private void InvokeEhfUpdateDomains(bool finalUpdate)
		{
			if (this.domainsToUpdate == null || this.domainsToUpdate.Count == 0)
			{
				return;
			}
			DomainResponseInfoSet responseSet = null;
			base.InvokeProvisioningService("Update Domain", delegate
			{
				responseSet = this.ProvisioningService.UpdateDomainSettingsByGuids(this.domainsToUpdate);
			}, this.domainsToUpdate.Count);
			int num = 0;
			int permanentFailureCount = 0;
			List<EhfDomainItemVersion2> list = new List<EhfDomainItemVersion2>();
			for (int i = 0; i < responseSet.ResponseInfo.Length; i++)
			{
				DomainResponseInfo domainResponseInfo = responseSet.ResponseInfo[i];
				EhfDomainItemVersion2 ehfDomainItemVersion = this.domainsToUpdate[i];
				ehfDomainItemVersion.TriedToUpdateDomain = true;
				if (domainResponseInfo.Status == ResponseStatus.Success)
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Successfully updated EHF Domain: DN=<{0}>; GUID=<{1}>; Name =<{2}>; OutboundOnly <{3}>", new object[]
					{
						ehfDomainItemVersion.DistinguishedName,
						domainResponseInfo.DomainGuid.Value,
						ehfDomainItemVersion.Name,
						ehfDomainItemVersion.OutboundOnly
					});
					if (ehfDomainItemVersion.TryClearForceDomainSyncFlagFromPerimeterConfig(base.ADAdapter) == EhfADResultCode.Failure)
					{
						num++;
					}
					if (!this.UpdateDuplicateDetectedFlag(ehfDomainItemVersion))
					{
						num++;
					}
				}
				else if ((domainResponseInfo.Fault.Id == FaultId.DomainDoesNotExist || domainResponseInfo.Fault.Id == FaultId.DomainExistOutsideThisCompany) && !finalUpdate)
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Update Domain operation failed with FaultId <{0}> for domain: DN=<{1}>; Name=<{2}>", new object[]
					{
						domainResponseInfo.Fault.Id,
						ehfDomainItemVersion.DistinguishedName,
						ehfDomainItemVersion.Name
					});
					list.Add(ehfDomainItemVersion);
				}
				else
				{
					base.HandleFailedDomain("Update Domain", ehfDomainItemVersion, domainResponseInfo, ref num, ref permanentFailureCount);
				}
				if (!ehfDomainItemVersion.EventLogAndTryStoreSyncErrors(base.ADAdapter))
				{
					num++;
				}
			}
			base.HandlePerEntryFailureCounts("Update Domain", this.domainsToUpdate.Count, num, permanentFailureCount, false);
			this.domainsToUpdate.Clear();
			foreach (EhfDomainItemVersion2 ehfDomainItemVersion2 in list)
			{
				if (ehfDomainItemVersion2.DomainCreatedWithGuid)
				{
					base.AddDomainToCreateBatch(ehfDomainItemVersion2);
				}
				else
				{
					base.AddDomainToCreateLastList(ehfDomainItemVersion2);
				}
			}
		}

		private bool TryGetEhfMailFlowPartner(ExSearchResultEntry entry, out EhfDomainSynchronizerVersion2.EhfMailFlowPartner ehfMailFlowPartner)
		{
			ehfMailFlowPartner = null;
			DirectoryAttribute attribute = entry.GetAttribute("msExchTransportResellerSettingsLink");
			if (attribute == null)
			{
				base.DiagSession.Tracer.TraceDebug(0L, "MailFlowPartner attribute not found");
				return false;
			}
			string text = (string)attribute[0];
			if (string.IsNullOrEmpty(text))
			{
				base.DiagSession.Tracer.TraceDebug(0L, "MailFlowPartner attribute is null or empty");
				return false;
			}
			if (ExSearchResultEntry.IsDeletedDN(text))
			{
				base.DiagSession.LogAndTraceError("Found a reference to deleted partner <{0}> from domain <{1}>. Ignoring the partner.", new object[]
				{
					text,
					entry.DistinguishedName
				});
				return false;
			}
			if (this.ehfMailFlowPartnersCache == null)
			{
				base.DiagSession.Tracer.TraceDebug(0L, "Creating MailFlowPartner cache.");
				this.ehfMailFlowPartnersCache = new Dictionary<string, EhfDomainSynchronizerVersion2.EhfMailFlowPartner>();
			}
			if (!this.ehfMailFlowPartnersCache.TryGetValue(text, out ehfMailFlowPartner))
			{
				base.DiagSession.Tracer.TraceDebug(0L, "MailFlowPartner entry not found in cache. Loading from AD");
				if (this.TryGetEhfMailFlowPartnerFromAD(text, out ehfMailFlowPartner))
				{
					this.ehfMailFlowPartnersCache.Add(text, ehfMailFlowPartner);
				}
				else
				{
					base.DiagSession.LogAndTraceError("Could not find mailflow partner <{0}>, referenced from domain <{1}>", new object[]
					{
						text,
						entry.DistinguishedName
					});
				}
			}
			else
			{
				base.DiagSession.Tracer.TraceDebug(0L, "MailFlowPartner entry found in cache");
			}
			return ehfMailFlowPartner != null;
		}

		private bool TryGetEhfMailFlowPartnerFromAD(string mailFlowPartnerDN, out EhfDomainSynchronizerVersion2.EhfMailFlowPartner ehfMailFlowPartner)
		{
			ehfMailFlowPartner = null;
			ExSearchResultEntry exSearchResultEntry = base.EhfConnection.ADAdapter.ReadObjectEntry(mailFlowPartnerDN, false, EhfDomainSynchronizerVersion2.MailFlowPartnerAttributes);
			if (exSearchResultEntry != null)
			{
				int invalidConnectorId = EhfDomainSynchronizerVersion2.InvalidConnectorId;
				int invalidConnectorId2 = EhfDomainSynchronizerVersion2.InvalidConnectorId;
				if (!EhfDomainSynchronizerVersion2.TryGetMailFlowConnectorId(exSearchResultEntry, "msExchTransportResellerSettingsInboundGatewayID", base.DiagSession, out invalidConnectorId))
				{
					base.DiagSession.LogAndTraceError("Failed to get the Inbound connectorId for partner <{0}>", new object[]
					{
						mailFlowPartnerDN
					});
				}
				if (!EhfDomainSynchronizerVersion2.TryGetMailFlowConnectorId(exSearchResultEntry, "msExchTransportResellerSettingsOutboundGatewayID", base.DiagSession, out invalidConnectorId2))
				{
					base.DiagSession.LogAndTraceError("Failed to get the Outbound connectorId for partner <{0}>", new object[]
					{
						mailFlowPartnerDN
					});
				}
				ehfMailFlowPartner = new EhfDomainSynchronizerVersion2.EhfMailFlowPartner(invalidConnectorId, invalidConnectorId2);
				return true;
			}
			return false;
		}

		private void AddDomainToUpdateBatch(EhfDomainItemVersion2 domain)
		{
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Adding domain <{0}> to Update batch", new object[]
			{
				domain.DistinguishedName
			});
			if (base.AddItemToBatch<EhfDomainItemVersion2>(domain, ref this.domainsToUpdate))
			{
				this.InvokeEhfUpdateDomains(false);
			}
		}

		private void AddDomainToCreateWithGuidBatch(EhfDomainItemVersion2 domain)
		{
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Adding domain <{0}> to CreateWithGuid batch. Name=<{1}>; CreatedWithGuid=<{2}>", new object[]
			{
				domain.DistinguishedName,
				domain.Name,
				domain.DomainCreatedWithGuid
			});
			domain.SetAsGuidDomain();
			base.AddDomainToCreateBatch(domain);
		}

		private EhfDomainItemVersion2 CreateEhfDomainItem(ExSearchResultEntry entry)
		{
			return EhfDomainItemVersion2.CreateEhfDomainItem(entry, this.GetMailFlowPartnerConnectorIds(entry), base.EhfConnection.DiagSession);
		}

		private int[] GetMailFlowPartnerConnectorIds(ExSearchResultEntry entry)
		{
			List<int> list = new List<int>(2);
			EhfDomainSynchronizerVersion2.EhfMailFlowPartner ehfMailFlowPartner;
			if (this.TryGetEhfMailFlowPartner(entry, out ehfMailFlowPartner) && ehfMailFlowPartner != null)
			{
				if (ehfMailFlowPartner.InboundConnectorId != EhfDomainSynchronizerVersion2.InvalidConnectorId)
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Adding InboundConnectorId <{0}> for domain <{1}>", new object[]
					{
						ehfMailFlowPartner.InboundConnectorId,
						entry.DistinguishedName
					});
					list.Add(ehfMailFlowPartner.InboundConnectorId);
				}
				if (ehfMailFlowPartner.OutboundConnectorId != EhfDomainSynchronizerVersion2.InvalidConnectorId)
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Adding OutboundConnectorId <{0}> for domain <{1}>", new object[]
					{
						ehfMailFlowPartner.OutboundConnectorId,
						entry.DistinguishedName
					});
					list.Add(ehfMailFlowPartner.OutboundConnectorId);
				}
				if (list.Count == 0)
				{
					base.DiagSession.LogAndTraceError("Mailflow partner is set for domain '{0}', but no connector is defined", new object[]
					{
						entry.DistinguishedName
					});
				}
			}
			else
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "No Mailflow partner is defined for '{0}'", new object[]
				{
					entry.DistinguishedName
				});
			}
			return list.ToArray();
		}

		private bool UpdateDuplicateDetectedFlag(EhfDomainItemVersion2 domain)
		{
			bool isDuplicate = domain.DomainCreatedWithGuid && !domain.OutboundOnly;
			return domain.TrySetDuplicateDomain(base.ADAdapter, isDuplicate) != EhfADResultCode.Failure;
		}

		internal const string UpdateDomainOperation = "Update Domain";

		private static readonly string[] MailFlowPartnerAttributes = new string[]
		{
			"msExchTransportResellerSettingsInboundGatewayID",
			"msExchTransportResellerSettingsOutboundGatewayID"
		};

		private static readonly int InvalidConnectorId = -1;

		private Dictionary<string, EhfDomainSynchronizerVersion2.EhfMailFlowPartner> ehfMailFlowPartnersCache;

		private List<EhfDomainItemVersion2> domainsToUpdate;

		private class EhfMailFlowPartner
		{
			public EhfMailFlowPartner(int inboundConnectorId, int outboundConnectorId)
			{
				this.inboundConnectorId = inboundConnectorId;
				this.outboundConnectorId = outboundConnectorId;
			}

			public int InboundConnectorId
			{
				get
				{
					return this.inboundConnectorId;
				}
			}

			public int OutboundConnectorId
			{
				get
				{
					return this.outboundConnectorId;
				}
			}

			private int inboundConnectorId;

			private int outboundConnectorId;
		}
	}
}
