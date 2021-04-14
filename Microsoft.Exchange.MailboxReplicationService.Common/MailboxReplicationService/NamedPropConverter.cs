using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class NamedPropConverter : IDataConverter<NamedProp, NamedPropData>
	{
		NamedProp IDataConverter<NamedProp, NamedPropData>.GetNativeRepresentation(NamedPropData npd)
		{
			if (npd == null)
			{
				return null;
			}
			if (npd.Kind == 0)
			{
				return new NamedProp(npd.Guid, npd.Id);
			}
			return new NamedProp(npd.Guid, npd.Name);
		}

		NamedPropData IDataConverter<NamedProp, NamedPropData>.GetDataRepresentation(NamedProp np)
		{
			if (np == null)
			{
				return null;
			}
			NamedPropData namedPropData = new NamedPropData();
			namedPropData.Kind = (int)np.Kind;
			namedPropData.Guid = np.Guid;
			if (np.Kind == NamedPropKind.String)
			{
				namedPropData.Name = np.Name;
				namedPropData.Id = 0;
			}
			else
			{
				namedPropData.Name = null;
				namedPropData.Id = np.Id;
			}
			return namedPropData;
		}
	}
}
