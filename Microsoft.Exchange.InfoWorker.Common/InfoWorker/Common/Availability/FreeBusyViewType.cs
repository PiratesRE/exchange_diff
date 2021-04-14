using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Flags]
	public enum FreeBusyViewType
	{
		None = 0,
		MergedOnly = 1,
		FreeBusy = 32,
		FreeBusyMerged = 33,
		Detailed = 96,
		DetailedMerged = 97
	}
}
