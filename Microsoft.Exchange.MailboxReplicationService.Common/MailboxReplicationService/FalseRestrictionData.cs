using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class FalseRestrictionData : RestrictionData
	{
		internal override Restriction GetRestriction()
		{
			return Restriction.False();
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new FalseFilter();
		}

		internal override string ToStringInternal()
		{
			return "FALSE";
		}
	}
}
