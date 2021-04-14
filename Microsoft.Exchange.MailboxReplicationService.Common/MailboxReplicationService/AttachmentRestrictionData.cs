using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class AttachmentRestrictionData : HierRestrictionData
	{
		public AttachmentRestrictionData()
		{
		}

		internal AttachmentRestrictionData(Restriction.AttachmentRestriction r)
		{
			base.ParseRestrictions(new Restriction[]
			{
				r.Restriction
			});
		}

		internal AttachmentRestrictionData(StoreSession storeSession, SubFilter filter)
		{
			base.ParseQueryFilter(storeSession, filter.Filter);
		}

		internal override Restriction GetRestriction()
		{
			return Restriction.Sub(PropTag.MessageAttachments, base.GetRestrictions()[0]);
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new SubFilter(SubFilterProperties.Attachments, base.GetQueryFilters(storeSession)[0]);
		}

		internal override string ToStringInternal()
		{
			return string.Format("ATTACHMENT[{0}]", base.Restrictions[0].ToStringInternal());
		}
	}
}
