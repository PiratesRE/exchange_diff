using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum ExternalAudience
	{
		None,
		Known,
		All
	}
}
