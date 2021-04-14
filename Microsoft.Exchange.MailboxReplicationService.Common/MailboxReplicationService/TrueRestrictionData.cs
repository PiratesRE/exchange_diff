using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class TrueRestrictionData : RestrictionData
	{
		internal override Restriction GetRestriction()
		{
			return Restriction.True();
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new TrueFilter();
		}

		internal override string ToStringInternal()
		{
			return "TRUE";
		}
	}
}
