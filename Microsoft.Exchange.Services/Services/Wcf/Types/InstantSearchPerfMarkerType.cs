using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "InstantSearchPerfMarkerType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class InstantSearchPerfMarkerType
	{
		public InstantSearchPerfMarkerType()
		{
		}

		public InstantSearchPerfMarkerType(InstantSearchPerfKey perfKey, double value)
		{
			this.PerfKey = perfKey;
			this.PerfValueMS = Math.Round(value, 3);
		}

		[DataMember]
		public InstantSearchPerfKey PerfKey { get; set; }

		[DataMember]
		public double PerfValueMS { get; set; }
	}
}
