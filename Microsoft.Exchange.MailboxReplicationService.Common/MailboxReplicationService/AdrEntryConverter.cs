using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class AdrEntryConverter : IDataConverter<AdrEntry, AdrEntryData>
	{
		AdrEntry IDataConverter<AdrEntry, AdrEntryData>.GetNativeRepresentation(AdrEntryData data)
		{
			return new AdrEntry(DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(data.Values));
		}

		AdrEntryData IDataConverter<AdrEntry, AdrEntryData>.GetDataRepresentation(AdrEntry ae)
		{
			return new AdrEntryData
			{
				Values = DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(ae.Values)
			};
		}
	}
}
