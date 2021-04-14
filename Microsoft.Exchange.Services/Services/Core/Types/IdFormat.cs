using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "IdFormatType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum IdFormat
	{
		EwsLegacyId,
		EwsId,
		EntryId,
		HexEntryId,
		StoreId,
		OwaId
	}
}
