using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class AudioQualityType
	{
		public float NMOS;

		public float NMOSDegradation;

		public float NMOSDegradationPacketLoss;

		public float NMOSDegradationJitter;

		public float Jitter;

		public float PacketLoss;

		public float RoundTrip;

		public float BurstDensity;

		public float BurstDuration;

		public string AudioCodec;
	}
}
