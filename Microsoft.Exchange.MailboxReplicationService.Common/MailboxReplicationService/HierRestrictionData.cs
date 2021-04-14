using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[KnownType(typeof(SizeRestrictionData))]
	[KnownType(typeof(AttachmentRestrictionData))]
	[KnownType(typeof(CommentRestrictionData))]
	[KnownType(typeof(TrueRestrictionData))]
	[KnownType(typeof(FalseRestrictionData))]
	[KnownType(typeof(AndRestrictionData))]
	[DataContract]
	[KnownType(typeof(NotRestrictionData))]
	[KnownType(typeof(RecipientRestrictionData))]
	[KnownType(typeof(CountRestrictionData))]
	[KnownType(typeof(PropertyRestrictionData))]
	[KnownType(typeof(OrRestrictionData))]
	[KnownType(typeof(ContentRestrictionData))]
	[KnownType(typeof(BitMaskRestrictionData))]
	[KnownType(typeof(ComparePropertyRestrictionData))]
	[KnownType(typeof(ExistRestrictionData))]
	internal abstract class HierRestrictionData : RestrictionData
	{
		public HierRestrictionData()
		{
		}

		[DataMember(Name = "restrictions")]
		public RestrictionData[] Restrictions { get; set; }

		internal void ParseRestrictions(params Restriction[] rest)
		{
			this.Restrictions = new RestrictionData[rest.Length];
			for (int i = 0; i < rest.Length; i++)
			{
				this.Restrictions[i] = RestrictionData.GetRestrictionData(rest[i]);
			}
		}

		internal void ParseQueryFilters(StoreSession storeSession, ReadOnlyCollection<QueryFilter> queryFilters)
		{
			this.Restrictions = new RestrictionData[queryFilters.Count];
			for (int i = 0; i < queryFilters.Count; i++)
			{
				this.Restrictions[i] = RestrictionData.GetRestrictionData(storeSession, queryFilters[i]);
			}
		}

		internal void ParseQueryFilter(StoreSession storeSession, QueryFilter queryFilter)
		{
			this.Restrictions = new RestrictionData[]
			{
				RestrictionData.GetRestrictionData(storeSession, queryFilter)
			};
		}

		internal Restriction[] GetRestrictions()
		{
			Restriction[] array = new Restriction[this.Restrictions.Length];
			for (int i = 0; i < this.Restrictions.Length; i++)
			{
				array[i] = this.Restrictions[i].GetRestriction();
			}
			return array;
		}

		internal QueryFilter[] GetQueryFilters(StoreSession storeSession)
		{
			QueryFilter[] array = new QueryFilter[this.Restrictions.Length];
			for (int i = 0; i < this.Restrictions.Length; i++)
			{
				array[i] = this.Restrictions[i].GetQueryFilter(storeSession);
			}
			return array;
		}

		internal string ConcatSubRestrictions(string prefix)
		{
			return string.Format("{0}{1}", prefix, CommonUtils.ConcatEntries<RestrictionData>(this.Restrictions, delegate(RestrictionData rd)
			{
				if (rd != null)
				{
					return rd.ToStringInternal();
				}
				return "(null)";
			}));
		}

		internal override int GetApproximateSize()
		{
			int num = base.GetApproximateSize();
			foreach (RestrictionData restrictionData in this.Restrictions)
			{
				num += restrictionData.GetApproximateSize();
			}
			return num;
		}
	}
}
