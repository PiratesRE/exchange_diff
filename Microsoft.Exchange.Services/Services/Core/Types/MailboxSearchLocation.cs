using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Flags]
	[XmlType(TypeName = "MailboxSearchLocationType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum MailboxSearchLocation
	{
		PrimaryOnly = 1,
		ArchiveOnly = 2,
		All = 3
	}
}
