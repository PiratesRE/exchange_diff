using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "TransportSettingsConfigXml")]
	[Serializable]
	public sealed class TransportSettingsConfigXml : XMLSerializableBase
	{
		public TransportSettingsConfigXml()
		{
			this.QueueAggregationIntervalTicks = TransportSettingsConfigXml.DefaultQueueAggregationIntervalTicks;
			this.DiagnosticsAggregationServicePort = TransportSettingsConfigXml.DefaultDiagnosticsAggregationServicePort;
			this.AgentGeneratedMessageLoopDetectionInSubmissionEnabled = TransportSettingsConfigXml.DefaultAgentGeneratedMessageLoopDetectionInSubmissionEnabled;
			this.AgentGeneratedMessageLoopDetectionInSmtpEnabled = TransportSettingsConfigXml.DefaultAgentGeneratedMessageLoopDetectionInSmtpEnabled;
			this.MaxAllowedAgentGeneratedMessageDepth = TransportSettingsConfigXml.DefaultMaxAllowedAgentGeneratedMessageDepth;
			this.MaxAllowedAgentGeneratedMessageDepthPerAgent = TransportSettingsConfigXml.DefaultMaxAllowedAgentGeneratedMessageDepthPerAgent;
		}

		[XmlElement(ElementName = "QAIT")]
		public long QueueAggregationIntervalTicks { get; set; }

		[XmlElement(ElementName = "DASP")]
		public int DiagnosticsAggregationServicePort { get; set; }

		[XmlElement(ElementName = "AGMLDE")]
		public bool AgentGeneratedMessageLoopDetectionInSubmissionEnabled { get; set; }

		[XmlElement(ElementName = "AGMLDSMTPE")]
		public bool AgentGeneratedMessageLoopDetectionInSmtpEnabled { get; set; }

		[XmlElement(ElementName = "MAAGMD")]
		public uint MaxAllowedAgentGeneratedMessageDepth { get; set; }

		[XmlElement(ElementName = "MAAGMDPA")]
		public uint MaxAllowedAgentGeneratedMessageDepthPerAgent { get; set; }

		internal static readonly int DefaultDiagnosticsAggregationServicePort = 9710;

		internal static readonly long DefaultQueueAggregationIntervalTicks = EnhancedTimeSpan.FromMinutes(1.0).Ticks;

		internal static readonly bool DefaultAgentGeneratedMessageLoopDetectionInSubmissionEnabled = true;

		internal static readonly bool DefaultAgentGeneratedMessageLoopDetectionInSmtpEnabled = true;

		internal static readonly uint DefaultMaxAllowedAgentGeneratedMessageDepth = 3U;

		internal static readonly uint DefaultMaxAllowedAgentGeneratedMessageDepthPerAgent = 2U;
	}
}
