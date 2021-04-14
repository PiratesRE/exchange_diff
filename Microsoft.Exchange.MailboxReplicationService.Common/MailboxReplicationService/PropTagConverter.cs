using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PropTagConverter : IDataConverter<PropTag, int>
	{
		PropTag IDataConverter<PropTag, int>.GetNativeRepresentation(int ptag)
		{
			return (PropTag)ptag;
		}

		int IDataConverter<PropTag, int>.GetDataRepresentation(PropTag ptag)
		{
			return (int)ptag;
		}
	}
}
