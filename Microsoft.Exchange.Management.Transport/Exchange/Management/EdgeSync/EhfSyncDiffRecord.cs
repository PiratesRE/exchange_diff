using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.EdgeSync
{
	[Serializable]
	public class EhfSyncDiffRecord<T>
	{
		public EhfSyncDiffRecord(string organization, ObjectId identity, EhfSyncObjectType ehfSyncObjectType, string name, T expectedObject, T actualObject)
		{
			this.organization = organization;
			this.identity = identity;
			this.syncObject = ehfSyncObjectType;
			this.name = name;
			this.onlyInEhf = new MultiValuedProperty<T>(actualObject);
			this.onlyInExchange = new MultiValuedProperty<T>(expectedObject);
			this.common = MultiValuedProperty<T>.Empty;
			this.syncErrors = MultiValuedProperty<string>.Empty;
		}

		public EhfSyncDiffRecord(string organization, ObjectId identity, EhfSyncObjectType ehfSyncObjectType, string name, T expectedObject, T actualObject, T common, MultiValuedProperty<string> syncErrors) : this(organization, identity, ehfSyncObjectType, name, expectedObject, actualObject)
		{
			this.common = new MultiValuedProperty<T>(common);
			this.syncErrors = syncErrors;
		}

		public string Organization
		{
			get
			{
				return this.organization;
			}
		}

		public ObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		public EhfSyncObjectType SyncObject
		{
			get
			{
				return this.syncObject;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public MultiValuedProperty<T> OnlyInExchange
		{
			get
			{
				return this.onlyInExchange;
			}
		}

		public MultiValuedProperty<T> OnlyInEhf
		{
			get
			{
				return this.onlyInEhf;
			}
		}

		public MultiValuedProperty<T> Common
		{
			get
			{
				return this.common;
			}
		}

		public MultiValuedProperty<string> SyncErrors
		{
			get
			{
				return this.syncErrors;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.Organization != null)
			{
				stringBuilder.AppendFormat("Organization = {0}", this.Organization);
				stringBuilder.AppendLine();
			}
			if (this.Identity != null)
			{
				stringBuilder.AppendFormat("Identity = {0}", this.Identity);
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendFormat("SyncObject = {0}", this.SyncObject);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("Name = {0}", this.Name);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("OnlyInExchange = ");
			foreach (T t in this.OnlyInExchange)
			{
				stringBuilder.AppendLine(t.ToString());
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("OnlyInEhf = ");
			foreach (T t2 in this.OnlyInEhf)
			{
				stringBuilder.AppendLine(t2.ToString());
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Common = ");
			foreach (T t3 in this.Common)
			{
				stringBuilder.AppendLine(t3.ToString());
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("SyncError = ");
			foreach (object obj in this.SyncErrors)
			{
				stringBuilder.AppendLine(obj.ToString());
			}
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		internal static IList<object> Compare(ExchangeTenantRecord exchangeTenantRecord, EhfCompanyRecord ehfCompanyRecord)
		{
			List<object> list = new List<object>();
			if (!exchangeTenantRecord.PerimeterConfig.EhfConfigSyncEnabled && ehfCompanyRecord == null)
			{
				return list;
			}
			if (exchangeTenantRecord.PerimeterConfig.EhfConfigSyncEnabled && ehfCompanyRecord == null)
			{
				list.Add(new EhfSyncDiffRecord<SyncedPerimeterConfig>(exchangeTenantRecord.Organization.RawIdentity, exchangeTenantRecord.PerimeterConfig.Identity, EhfSyncObjectType.PerimeterConfig, "PerimeterConfig", exchangeTenantRecord.PerimeterConfig, null, null, exchangeTenantRecord.PerimeterConfig.SyncErrors));
				list.AddRange(EhfSyncDiffRecord<object>.CompareDomain(exchangeTenantRecord, ehfCompanyRecord));
			}
			if (!exchangeTenantRecord.PerimeterConfig.EhfConfigSyncEnabled && ehfCompanyRecord != null)
			{
				list.Add(new EhfSyncDiffRecord<string>(exchangeTenantRecord.Organization.RawIdentity, exchangeTenantRecord.PerimeterConfig.Identity, EhfSyncObjectType.PerimeterConfig, "Company ID", exchangeTenantRecord.CompanyId, ehfCompanyRecord.CompanyId));
				foreach (DomainSyncRecord actualObject in ehfCompanyRecord.Domains)
				{
					list.Add(new EhfSyncDiffRecord<DomainSyncRecord>(exchangeTenantRecord.Organization.RawIdentity, null, EhfSyncObjectType.AcceptedDomain, string.Empty, null, actualObject));
				}
			}
			if (ehfCompanyRecord != null && exchangeTenantRecord.PerimeterConfig.EhfConfigSyncEnabled)
			{
				EhfSyncDiffRecord<IList<string>> item;
				if (EhfSyncDiffRecord<IList<string>>.TryGetEdgeSyncDiffRecord(exchangeTenantRecord.Organization, exchangeTenantRecord.PerimeterConfig, exchangeTenantRecord.GatewayIPAddresses, ehfCompanyRecord.GatewayIPAddresses, "GatewayIPAddresses", out item))
				{
					list.Add(item);
				}
				EhfSyncDiffRecord<IList<string>> item2;
				if (EhfSyncDiffRecord<IList<string>>.TryGetEdgeSyncDiffRecord(exchangeTenantRecord.Organization, exchangeTenantRecord.PerimeterConfig, exchangeTenantRecord.InternalIPAddresses, ehfCompanyRecord.InternalIPAddresses, "InternalServerIPAddresses", out item2))
				{
					list.Add(item2);
				}
				if (!exchangeTenantRecord.CompanyId.Equals(ehfCompanyRecord.CompanyId))
				{
					list.Add(new EhfSyncDiffRecord<string>(exchangeTenantRecord.Organization.RawIdentity, exchangeTenantRecord.PerimeterConfig.Identity, EhfSyncObjectType.PerimeterConfig, "Company ID", exchangeTenantRecord.CompanyId, ehfCompanyRecord.CompanyId));
				}
				if (!exchangeTenantRecord.Guid.Equals(ehfCompanyRecord.Guid))
				{
					list.Add(new EhfSyncDiffRecord<Guid>(exchangeTenantRecord.Organization.RawIdentity, exchangeTenantRecord.PerimeterConfig.Identity, EhfSyncObjectType.PerimeterConfig, "GUID", exchangeTenantRecord.Guid, ehfCompanyRecord.Guid));
				}
				if (!ehfCompanyRecord.CompanyName.EndsWith(exchangeTenantRecord.CompanyName))
				{
					list.Add(new EhfSyncDiffRecord<string>(exchangeTenantRecord.Organization.RawIdentity, exchangeTenantRecord.PerimeterConfig.Identity, EhfSyncObjectType.PerimeterConfig, "Name", exchangeTenantRecord.CompanyName, ehfCompanyRecord.CompanyName));
				}
				if (exchangeTenantRecord.IPSkiplistingEnabled != ehfCompanyRecord.IPSkiplistingEnabled)
				{
					list.Add(new EhfSyncDiffRecord<bool>(exchangeTenantRecord.Organization.RawIdentity, exchangeTenantRecord.PerimeterConfig.Identity, EhfSyncObjectType.PerimeterConfig, "IPSkiplistingEnabled", exchangeTenantRecord.IPSkiplistingEnabled, ehfCompanyRecord.IPSkiplistingEnabled));
				}
				list.AddRange(EhfSyncDiffRecord<object>.CompareDomain(exchangeTenantRecord, ehfCompanyRecord));
			}
			return list;
		}

		private static IList<object> CompareDomain(ExchangeTenantRecord exchangeTenantRecord, EhfCompanyRecord ehfCompanyRecord)
		{
			List<object> list = new List<object>();
			if (ehfCompanyRecord != null)
			{
				IList<DomainSyncRecord> list2 = ehfCompanyRecord.Domains.Intersect(exchangeTenantRecord.Domains).ToList<DomainSyncRecord>();
				if (list2.Count != ehfCompanyRecord.Domains.Count || ehfCompanyRecord.Domains.Count != exchangeTenantRecord.Domains.Count)
				{
					IList<DomainSyncRecord> list3 = exchangeTenantRecord.Domains.Except(ehfCompanyRecord.Domains).ToList<DomainSyncRecord>();
					IList<DomainSyncRecord> list4 = ehfCompanyRecord.Domains.Except(exchangeTenantRecord.Domains).ToList<DomainSyncRecord>();
					foreach (DomainSyncRecord domainSyncRecord in list3)
					{
						list.Add(new EhfSyncDiffRecord<DomainSyncRecord>(exchangeTenantRecord.Organization.RawIdentity, exchangeTenantRecord.AcceptedDomains[domainSyncRecord.Name].Identity, EhfSyncObjectType.AcceptedDomain, string.Empty, domainSyncRecord, null, null, exchangeTenantRecord.AcceptedDomains[domainSyncRecord.Name].SyncErrors));
					}
					if (list4.Count > 0)
					{
						foreach (DomainSyncRecord actualObject in list4)
						{
							list.Add(new EhfSyncDiffRecord<DomainSyncRecord>(exchangeTenantRecord.Organization.RawIdentity, null, EhfSyncObjectType.AcceptedDomain, string.Empty, null, actualObject));
						}
					}
				}
				using (IEnumerator<DomainSyncRecord> enumerator3 = list2.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						DomainSyncRecord domainSyncRecord2 = enumerator3.Current;
						SyncedAcceptedDomain syncedAcceptedDomain = exchangeTenantRecord.AcceptedDomains[domainSyncRecord2.Name];
						if (!domainSyncRecord2.Enabled)
						{
							list.Add(new EhfSyncDiffRecord<string>(exchangeTenantRecord.Organization.RawIdentity, syncedAcceptedDomain.Identity, EhfSyncObjectType.AcceptedDomain, "ENABLED", bool.TrueString, domainSyncRecord2.Enabled.ToString(), string.Empty, syncedAcceptedDomain.SyncErrors));
						}
						if (!domainSyncRecord2.Guid.Equals(syncedAcceptedDomain.Guid))
						{
							list.Add(new EhfSyncDiffRecord<string>(exchangeTenantRecord.Organization.RawIdentity, syncedAcceptedDomain.Identity, EhfSyncObjectType.AcceptedDomain, "GUID", syncedAcceptedDomain.Guid.ToString(), domainSyncRecord2.Guid.ToString(), string.Empty, syncedAcceptedDomain.SyncErrors));
						}
					}
					return list;
				}
			}
			foreach (string text in exchangeTenantRecord.AcceptedDomains.Keys)
			{
				SyncedAcceptedDomain syncedAcceptedDomain2 = exchangeTenantRecord.AcceptedDomains[text];
				list.Add(new EhfSyncDiffRecord<string>(exchangeTenantRecord.Organization.RawIdentity, syncedAcceptedDomain2.Identity, EhfSyncObjectType.AcceptedDomain, string.Empty, text, string.Empty, string.Empty, syncedAcceptedDomain2.SyncErrors));
			}
			return list;
		}

		private static bool TryGetEdgeSyncDiffRecord(OrganizationIdParameter organization, SyncedPerimeterConfig perimeterConfig, IList<string> exchangeIpAddresses, IList<string> ehfIpAddresses, string label, out EhfSyncDiffRecord<IList<string>> ehfSyncDiffRecord)
		{
			ehfSyncDiffRecord = null;
			IList<string> list = exchangeIpAddresses.Intersect(ehfIpAddresses).ToList<string>();
			if (list.Count != exchangeIpAddresses.Count || exchangeIpAddresses.Count != ehfIpAddresses.Count)
			{
				IList<string> expectedObject = exchangeIpAddresses.Except(ehfIpAddresses).ToList<string>();
				IList<string> actualObject = ehfIpAddresses.Except(exchangeIpAddresses).ToList<string>();
				ehfSyncDiffRecord = new EhfSyncDiffRecord<IList<string>>(organization.RawIdentity, perimeterConfig.Identity, EhfSyncObjectType.PerimeterConfig, label, expectedObject, actualObject, list, perimeterConfig.SyncErrors);
				return true;
			}
			return false;
		}

		private readonly string organization;

		private ObjectId identity;

		private EhfSyncObjectType syncObject;

		private readonly string name;

		private MultiValuedProperty<T> onlyInExchange;

		private MultiValuedProperty<T> onlyInEhf;

		private MultiValuedProperty<T> common;

		private MultiValuedProperty<string> syncErrors;
	}
}
