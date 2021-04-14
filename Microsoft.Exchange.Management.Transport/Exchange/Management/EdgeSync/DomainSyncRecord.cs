using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;

namespace Microsoft.Exchange.Management.EdgeSync
{
	internal class DomainSyncRecord
	{
		public DomainSyncRecord(AcceptedDomain acceptedDomain)
		{
			if (acceptedDomain.OutboundOnly || acceptedDomain.PerimeterDuplicateDetected)
			{
				this.name = AcceptedDomain.FormatEhfOutboundOnlyDomainName(acceptedDomain.DomainName.Domain.Trim(), acceptedDomain.Guid).ToLower().Trim();
			}
			else
			{
				this.name = acceptedDomain.DomainName.Domain.ToLower().Trim();
			}
			this.guid = acceptedDomain.Guid;
			this.enabled = true;
		}

		public DomainSyncRecord(Domain domain)
		{
			if (domain.DomainGuid != null)
			{
				this.guid = domain.DomainGuid.Value;
			}
			else
			{
				this.guid = Guid.Empty;
			}
			this.enabled = domain.IsEnabled;
			this.name = domain.Name.ToLower().Trim();
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override bool Equals(object obj)
		{
			return obj != null && (object.ReferenceEquals(this, obj) || this.Equals(obj as DomainSyncRecord));
		}

		public bool Equals(DomainSyncRecord record)
		{
			return record != null && (object.ReferenceEquals(this, record) || this.name.Equals(record.Name, StringComparison.OrdinalIgnoreCase));
		}

		public override int GetHashCode()
		{
			return this.name.GetHashCode();
		}

		public static IList<DomainSyncRecord> CreateDomainSyncRecordList(IEnumerable<SyncedAcceptedDomain> acceptedDomains)
		{
			List<DomainSyncRecord> list = new List<DomainSyncRecord>();
			foreach (AcceptedDomain acceptedDomain in acceptedDomains)
			{
				list.Add(new DomainSyncRecord(acceptedDomain));
			}
			return list;
		}

		public static IList<DomainSyncRecord> CreateDomainSyncRecordList(IEnumerable<Domain> ehfDomains)
		{
			List<DomainSyncRecord> list = new List<DomainSyncRecord>();
			foreach (Domain domain in ehfDomains)
			{
				list.Add(new DomainSyncRecord(domain));
			}
			return list;
		}

		private readonly string name;

		private readonly bool enabled;

		private readonly Guid guid;
	}
}
