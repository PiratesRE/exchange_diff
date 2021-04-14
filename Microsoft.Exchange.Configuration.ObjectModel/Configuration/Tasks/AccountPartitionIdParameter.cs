using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AccountPartitionIdParameter : ADIdParameter
	{
		public AccountPartitionIdParameter()
		{
		}

		public AccountPartitionIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
			this.fqdn = new Fqdn(adobjectid.GetPartitionId().ForestFQDN);
		}

		public AccountPartitionIdParameter(Fqdn fqdn) : base((fqdn == null) ? null : fqdn.ToString())
		{
			if (fqdn == null)
			{
				throw new ArgumentNullException("fqdn");
			}
			this.fqdn = fqdn;
		}

		public AccountPartitionIdParameter(AccountPartition partition)
		{
			if (partition == null)
			{
				throw new ArgumentNullException("partition");
			}
			this.Initialize(partition.Id);
		}

		public AccountPartitionIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected AccountPartitionIdParameter(string identity) : base(identity)
		{
			Fqdn.TryParse(identity, out this.fqdn);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (subTreeSession == null)
			{
				throw new ArgumentNullException("subTreeSession");
			}
			EnumerableWrapper<T> enumerableWrapper = null;
			enumerableWrapper = base.GetEnumerableWrapper<T>(enumerableWrapper, base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason));
			if (enumerableWrapper.HasElements())
			{
				return enumerableWrapper;
			}
			if (!typeof(T).IsAssignableFrom(typeof(AccountPartition)))
			{
				return enumerableWrapper;
			}
			if (this.fqdn != null)
			{
				ADObjectId adobjectId = ADSession.GetDomainNamingContextForLocalForest();
				adobjectId = adobjectId.GetChildId("System").GetChildId(this.fqdn.ToString());
				ADPagedReader<T> collection = session.FindPaged<T>(rootId, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, AccountPartitionSchema.TrustedDomainLink, adobjectId.DistinguishedName), null, 0, null);
				enumerableWrapper = base.GetEnumerableWrapper<T>(enumerableWrapper, collection);
				if (enumerableWrapper.HasElements())
				{
					return enumerableWrapper;
				}
				Guid g;
				Guid.TryParse(this.fqdn, out g);
				if (TopologyProvider.LocalForestFqdn.Equals(this.fqdn.ToString(), StringComparison.OrdinalIgnoreCase) || ADObjectId.ResourcePartitionGuid.Equals(g))
				{
					collection = session.FindPaged<T>(rootId, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, AccountPartitionSchema.IsLocalForest, true), null, 0, null);
					enumerableWrapper = base.GetEnumerableWrapper<T>(enumerableWrapper, collection);
				}
				if (enumerableWrapper.HasElements())
				{
					return enumerableWrapper;
				}
				PartitionId partitionId;
				if (ADAccountPartitionLocator.IsSingleForestTopology(out partitionId) && this.fqdn.ToString().Equals(partitionId.ForestFQDN, StringComparison.OrdinalIgnoreCase) && partitionId.PartitionObjectId != null)
				{
					base.UpdateInternalADObjectId(new ADObjectId(partitionId.PartitionObjectId.Value));
					enumerableWrapper = base.GetEnumerableWrapper<T>(enumerableWrapper, base.GetExactMatchObjects<T>(rootId, session, optionalData));
				}
			}
			return enumerableWrapper;
		}

		public static AccountPartitionIdParameter Parse(string identity)
		{
			return new AccountPartitionIdParameter(identity);
		}

		private Fqdn fqdn;
	}
}
