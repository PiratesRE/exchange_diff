using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum BusyType
	{
		Free,
		Tentative,
		Busy,
		OOF,
		WorkingElsewhere,
		NoData
	}
}
