using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RestrictionConverter : IDataConverter<Restriction, RestrictionData>
	{
		Restriction IDataConverter<Restriction, RestrictionData>.GetNativeRepresentation(RestrictionData rd)
		{
			return rd.GetRestriction();
		}

		RestrictionData IDataConverter<Restriction, RestrictionData>.GetDataRepresentation(Restriction r)
		{
			return RestrictionData.GetRestrictionData(r);
		}
	}
}
