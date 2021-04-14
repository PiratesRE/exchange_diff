using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class NullRestrictionData : RestrictionData
	{
		internal override Restriction GetRestriction()
		{
			return null;
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new NullFilter();
		}

		internal override string ToStringInternal()
		{
			return "NULL";
		}
	}
}
