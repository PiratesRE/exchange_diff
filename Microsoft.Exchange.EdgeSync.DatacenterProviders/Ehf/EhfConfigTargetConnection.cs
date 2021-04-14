using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.EdgeSync.Logging;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfConfigTargetConnection : EhfTargetConnection
	{
		public EhfConfigTargetConnection(int localServerVersion, EhfTargetServerConfig config, EnhancedTimeSpan syncInterval, EdgeSyncLogSession logSession) : base(localServerVersion, config, syncInterval, logSession)
		{
		}

		public EhfConfigTargetConnection(int localServerVersion, EhfTargetServerConfig config, EdgeSyncLogSession logSession, EhfPerfCounterHandler perfCounterHandler, IProvisioningService provisioningService, IManagementService managementService, EhfADAdapter adapter, EnhancedTimeSpan syncInterval) : base(localServerVersion, config, logSession, perfCounterHandler, provisioningService, managementService, null, adapter, syncInterval)
		{
		}

		public override void AbortSyncCycle(Exception cause)
		{
			if (this.companySync != null)
			{
				this.companySync.ClearBatches();
			}
			if (this.domainSync != null)
			{
				this.domainSync.ClearBatches();
			}
			base.AbortSyncCycle(cause);
		}

		public override bool OnSynchronizing()
		{
			bool result = base.OnSynchronizing();
			if (this.companySync != null)
			{
				throw new InvalidOperationException("Company sync already exists");
			}
			this.companySync = new EhfCompanySynchronizer(this);
			if (this.domainSync != null)
			{
				throw new InvalidOperationException("Domain sync already exists");
			}
			if (base.Config.EhfWebServiceVersion == EhfWebServiceVersion.Version1)
			{
				this.domainSync = new EhfDomainSynchronizer(this);
			}
			else
			{
				if (base.Config.EhfWebServiceVersion != EhfWebServiceVersion.Version2)
				{
					throw new InvalidOperationException("This version of EHF provider does not support EHF Webservice version " + base.Config.EhfWebServiceVersion);
				}
				this.domainSync = new EhfDomainSynchronizerVersion2(this);
			}
			return result;
		}

		public override void OnConnectedToSource(Connection sourceConnection)
		{
			base.ADAdapter.SetConnection(sourceConnection);
		}

		public override bool OnSynchronized()
		{
			return (this.companySync == null || this.companySync.FlushBatches()) && (this.domainSync == null || this.domainSync.FlushBatches());
		}

		public override SyncResult OnAddEntry(ExSearchResultEntry entry, SortedList<string, DirectoryAttribute> sourceAttributes)
		{
			SyncResult result = SyncResult.Added;
			EhfConfigTargetConnection.ConfigObjectType syncObjectType = this.GetSyncObjectType(entry, "Add");
			switch (syncObjectType)
			{
			case EhfConfigTargetConnection.ConfigObjectType.AcceptedDomain:
				this.domainSync.HandleAddedDomain(entry);
				break;
			case EhfConfigTargetConnection.ConfigObjectType.PerimeterSettings:
				this.companySync.CreateOrModifyEhfCompany(entry);
				break;
			default:
				throw new InvalidOperationException("EhfConfigTargetConnection.GetSyncObjectType() returned unexpected value " + syncObjectType);
			}
			return result;
		}

		public override SyncResult OnModifyEntry(ExSearchResultEntry entry, SortedList<string, DirectoryAttribute> sourceAttributes)
		{
			SyncResult result = SyncResult.Modified;
			EhfConfigTargetConnection.ConfigObjectType syncObjectType = this.GetSyncObjectType(entry, "Modify");
			switch (syncObjectType)
			{
			case EhfConfigTargetConnection.ConfigObjectType.AcceptedDomain:
				this.domainSync.HandleModifiedDomain(entry);
				break;
			case EhfConfigTargetConnection.ConfigObjectType.PerimeterSettings:
				this.companySync.CreateOrModifyEhfCompany(entry);
				break;
			default:
				throw new InvalidOperationException("EhfConfigTargetConnection.GetSyncObjectType() returned unexpected value " + syncObjectType);
			}
			return result;
		}

		public override SyncResult OnDeleteEntry(ExSearchResultEntry entry)
		{
			EhfConfigTargetConnection.ConfigObjectType syncObjectType = this.GetSyncObjectType(entry, "Delete");
			SyncResult result;
			switch (syncObjectType)
			{
			case EhfConfigTargetConnection.ConfigObjectType.AcceptedDomain:
				this.domainSync.HandleDeletedDomain(entry);
				result = SyncResult.Deleted;
				break;
			case EhfConfigTargetConnection.ConfigObjectType.PerimeterSettings:
				this.companySync.DeleteEhfCompany(entry);
				result = SyncResult.Deleted;
				break;
			default:
				throw new InvalidOperationException("EhfConfigTargetConnection.GetSyncObjectType() returned unexpected value " + syncObjectType);
			}
			return result;
		}

		public bool TryGetEhfCompanyIdentity(string configUnitDN, string missingIdAction, out EhfCompanyIdentity companyIdentity)
		{
			return this.companySync.TryGetEhfCompanyIdentity(configUnitDN, missingIdAction, out companyIdentity);
		}

		public void AddDomainsForNewCompany(EhfCompanyItem company)
		{
			if (company.CompanyId == 0)
			{
				throw new ArgumentException("Cannot push domains for a company without CompanyId");
			}
			if (company.IsDeleted)
			{
				throw new ArgumentException("Cannot push domains for a deleted company");
			}
			ADObjectId parent = new ADObjectId(company.DistinguishedName).Parent;
			this.domainSync.CreateEhfDomainsForNewCompany(parent.DistinguishedName, company.CompanyId, company.GetCompanyGuid());
		}

		private EhfConfigTargetConnection.ConfigObjectType GetSyncObjectType(ExSearchResultEntry entry, string operation)
		{
			string objectClass = entry.ObjectClass;
			if (string.IsNullOrEmpty(objectClass))
			{
				base.DiagSession.LogAndTraceError("Entry <{0}> contains no objectClass attribute in operation {1}; cannot proceed", new object[]
				{
					entry.DistinguishedName,
					operation
				});
				throw new ArgumentException("Entry does not contain objectClass attribute", "entry");
			}
			string a;
			if ((a = objectClass) != null)
			{
				if (a == "msExchAcceptedDomain")
				{
					return EhfConfigTargetConnection.ConfigObjectType.AcceptedDomain;
				}
				if (a == "msExchTenantPerimeterSettings")
				{
					return EhfConfigTargetConnection.ConfigObjectType.PerimeterSettings;
				}
			}
			base.DiagSession.LogAndTraceError("Entry <{0}> contains unexpected objectClass value <{1}> in operation {2}; cannot proceed", new object[]
			{
				entry.DistinguishedName,
				objectClass,
				operation
			});
			throw new ArgumentException("Entry's objectClass is invalid: " + objectClass, "entry");
		}

		private EhfCompanySynchronizer companySync;

		private EhfDomainSynchronizer domainSync;

		private enum ConfigObjectType
		{
			AcceptedDomain,
			PerimeterSettings,
			Unknown
		}
	}
}
