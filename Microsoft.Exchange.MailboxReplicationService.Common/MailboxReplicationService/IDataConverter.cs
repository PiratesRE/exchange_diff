using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IDataConverter<TNative, TData>
	{
		TNative GetNativeRepresentation(TData data);

		TData GetDataRepresentation(TNative src);
	}
}
