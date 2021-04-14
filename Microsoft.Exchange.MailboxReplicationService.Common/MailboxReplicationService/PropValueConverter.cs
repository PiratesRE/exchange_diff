using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PropValueConverter : IDataConverter<PropValue, PropValueData>
	{
		PropValue IDataConverter<PropValue, PropValueData>.GetNativeRepresentation(PropValueData data)
		{
			return new PropValue((PropTag)data.PropTag, data.Value);
		}

		PropValueData IDataConverter<PropValue, PropValueData>.GetDataRepresentation(PropValue pv)
		{
			return new PropValueData(pv.PropTag, pv.RawValue);
		}
	}
}
