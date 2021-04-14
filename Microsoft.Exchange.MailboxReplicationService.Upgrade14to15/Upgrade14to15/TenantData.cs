using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	public class TenantData
	{
		public TenantData(string tenantName) : this(Guid.Empty, tenantName, null, null, false, false, null)
		{
		}

		public TenantData(Guid tenantId, string tenantName, string servicePlan, ExchangeObjectVersion version, bool isUpgradingOrganization, bool isPilotingOrganization, string[] constraints)
		{
			this.TenantId = tenantId;
			this.TenantName = tenantName;
			this.ServicePlan = servicePlan;
			this.Constraints = constraints;
			this.Version = version;
			this.IsUpgradingOrganization = isUpgradingOrganization;
			this.IsPilotingOrganization = isPilotingOrganization;
		}

		public Guid TenantId { get; private set; }

		public string TenantName { get; private set; }

		public string ServicePlan { get; private set; }

		public string[] Constraints { get; private set; }

		public VersionData E14MbxData
		{
			get
			{
				return this.e14MbxData;
			}
		}

		public VersionData E15MbxData
		{
			get
			{
				return this.e15MbxData;
			}
		}

		public int TenantSize { get; set; }

		public int TotalPrimaryMbxCount
		{
			get
			{
				return this.E14MbxData.PrimaryData.Count + this.E15MbxData.PrimaryData.Count;
			}
		}

		public double TotalPrimaryMbxSize
		{
			get
			{
				return this.E14MbxData.PrimaryData.Size + this.E15MbxData.PrimaryData.Size;
			}
		}

		public int TotalArchiveMbxCount
		{
			get
			{
				return this.E14MbxData.ArchiveData.Count + this.E15MbxData.ArchiveData.Count;
			}
		}

		public double TotalArchiveMbxSize
		{
			get
			{
				return this.E14MbxData.ArchiveData.Size + this.E15MbxData.ArchiveData.Size;
			}
		}

		public string ProgramId { get; private set; }

		public string OfferId { get; private set; }

		public bool ShouldIgnore
		{
			get
			{
				return !this.ShouldUpload();
			}
		}

		public ExchangeObjectVersion Version { get; private set; }

		public bool IsUpgradingOrganization { get; private set; }

		public bool IsPilotingOrganization { get; private set; }

		public bool? UpgradeConstraintsDisabled { get; private set; }

		public int? UpgradeUnitsOverride { get; private set; }

		public void AddValues(int primaryCount, double primarySize, int archiveCount, double archiveSize, bool isE14Data)
		{
			if (isE14Data)
			{
				this.e14MbxData.Add(primaryCount, primarySize, archiveCount, archiveSize);
				return;
			}
			this.e15MbxData.Add(primaryCount, primarySize, archiveCount, archiveSize);
		}

		public void UpdateFromTenant(TenantOrganizationPresentationObjectWrapper tenant)
		{
			List<string> list = new List<string>();
			if (tenant.UpgradeConstraints != null && tenant.UpgradeConstraints.UpgradeConstraints != null && tenant.UpgradeConstraints.UpgradeConstraints.Length > 0)
			{
				foreach (UpgradeConstraint upgradeConstraint in tenant.UpgradeConstraints.UpgradeConstraints)
				{
					if (!string.IsNullOrWhiteSpace(upgradeConstraint.Name) && (upgradeConstraint.ExpirationDate > DateTime.UtcNow || upgradeConstraint.ExpirationDate == DateTime.MinValue))
					{
						list.Add(upgradeConstraint.Name);
					}
				}
			}
			this.Constraints = list.ToArray();
			this.Version = tenant.AdminDisplayVersion;
			this.IsPilotingOrganization = tenant.IsPilotingOrganization;
			this.IsUpgradingOrganization = tenant.IsUpgradingOrganization;
			this.ServicePlan = tenant.ServicePlan;
			this.ProgramId = tenant.ProgramId;
			this.OfferId = tenant.OfferId;
			this.UpgradeConstraintsDisabled = tenant.UpgradeConstraintsDisabled;
			this.UpgradeUnitsOverride = tenant.UpgradeUnitsOverride;
			Guid tenantId;
			Guid.TryParse(tenant.ExternalDirectoryOrganizationId, out tenantId);
			this.TenantId = tenantId;
		}

		public bool IsEdu()
		{
			return this.ProgramId.Equals("EDU", StringComparison.OrdinalIgnoreCase) || (this.ProgramId.Equals("MSOnlineMigration", StringComparison.OrdinalIgnoreCase) && this.OfferId.Equals("EDU", StringComparison.OrdinalIgnoreCase));
		}

		public bool IsFfdf()
		{
			return this.ProgramId.Equals("FFDF", StringComparison.OrdinalIgnoreCase) || (this.ProgramId.Equals("MSOnlineMigration", StringComparison.OrdinalIgnoreCase) && this.OfferId.Equals("FFDF", StringComparison.OrdinalIgnoreCase));
		}

		internal void Validate()
		{
			if (this.Version.ExchangeBuild.Major == ExchangeObjectVersion.Exchange2010.ExchangeBuild.Major)
			{
				if (this.IsPilotingOrganization)
				{
					if (this.E15MbxData.PrimaryData.Count > 101 || this.E15MbxData.ArchiveData.Count > 101)
					{
						throw new TooManyPilotMailboxesException();
					}
				}
				else if (this.E15MbxData.PrimaryData.Count > 0 || this.E15MbxData.ArchiveData.Count > 0)
				{
					throw new InvalidE15MailboxesException();
				}
			}
			else if (this.Version.ExchangeBuild.Major == ExchangeObjectVersion.Exchange2012.ExchangeBuild.Major && !this.IsPilotingOrganization && !this.IsUpgradingOrganization && (this.E14MbxData.PrimaryData.Count > 0 || this.E14MbxData.PrimaryData.Count > 0))
			{
				throw new InvalidE14MailboxesException();
			}
		}

		private bool ShouldUpload()
		{
			return !this.TenantId.Equals(Guid.Empty) && !this.IsEdu() && !this.IsFfdf() && this.TenantSize >= 0;
		}

		private const string Edu = "EDU";

		private const string Ffdf = "FFDF";

		private const string MsOnlineMigration = "MSOnlineMigration";

		private VersionData e14MbxData;

		private VersionData e15MbxData;
	}
}
