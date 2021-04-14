using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class AudioQualityType
	{
		public float NMOS
		{
			get
			{
				return this.nMOSField;
			}
			set
			{
				this.nMOSField = value;
			}
		}

		public float NMOSDegradation
		{
			get
			{
				return this.nMOSDegradationField;
			}
			set
			{
				this.nMOSDegradationField = value;
			}
		}

		public float NMOSDegradationPacketLoss
		{
			get
			{
				return this.nMOSDegradationPacketLossField;
			}
			set
			{
				this.nMOSDegradationPacketLossField = value;
			}
		}

		public float NMOSDegradationJitter
		{
			get
			{
				return this.nMOSDegradationJitterField;
			}
			set
			{
				this.nMOSDegradationJitterField = value;
			}
		}

		public float Jitter
		{
			get
			{
				return this.jitterField;
			}
			set
			{
				this.jitterField = value;
			}
		}

		public float PacketLoss
		{
			get
			{
				return this.packetLossField;
			}
			set
			{
				this.packetLossField = value;
			}
		}

		public float RoundTrip
		{
			get
			{
				return this.roundTripField;
			}
			set
			{
				this.roundTripField = value;
			}
		}

		public float BurstDensity
		{
			get
			{
				return this.burstDensityField;
			}
			set
			{
				this.burstDensityField = value;
			}
		}

		public float BurstDuration
		{
			get
			{
				return this.burstDurationField;
			}
			set
			{
				this.burstDurationField = value;
			}
		}

		public string AudioCodec
		{
			get
			{
				return this.audioCodecField;
			}
			set
			{
				this.audioCodecField = value;
			}
		}

		private float nMOSField;

		private float nMOSDegradationField;

		private float nMOSDegradationPacketLossField;

		private float nMOSDegradationJitterField;

		private float jitterField;

		private float packetLossField;

		private float roundTripField;

		private float burstDensityField;

		private float burstDurationField;

		private string audioCodecField;
	}
}
