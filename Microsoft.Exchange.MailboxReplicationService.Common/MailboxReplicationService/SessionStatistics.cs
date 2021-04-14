using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	[Serializable]
	public sealed class SessionStatistics : XMLSerializableBase, ICloneable
	{
		public SessionStatistics()
		{
			this.SourceProviderInfo = new ProviderInfo();
			this.DestinationProviderInfo = new ProviderInfo();
			this.SourceLatencyInfo = new LatencyInfo();
			this.DestinationLatencyInfo = new LatencyInfo();
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public TimeSpan TotalTimeProcessingMessages { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public TimeSpan TimeInGetConnection { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public TimeSpan TimeInPropertyBagLoad { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public TimeSpan TimeInMessageItemConversion { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public TimeSpan TimeDeterminingAgeOfItem { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public TimeSpan TimeInMimeConversion { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public TimeSpan TimeInShouldAnnotateMessage { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public TimeSpan TimeInQueue { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public TimeSpan TimeInTransportRetriever { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public TimeSpan TimeInDocParser { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public TimeSpan TimeInWordbreaker { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public TimeSpan TimeInNLGSubflow { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public TimeSpan TimeProcessingFailedMessages { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public TimeSpan PreFinalSyncDataProcessingDuration { get; set; }

		[DataMember(Name = "TotalTimeProcessingMessagesTicks", IsRequired = false)]
		[XmlElement(ElementName = "TotalTimeProcessingMessagesTicks")]
		public long TotalTimeProcessingMessagesTicks
		{
			get
			{
				return this.TotalTimeProcessingMessages.Ticks;
			}
			set
			{
				this.TotalTimeProcessingMessages = new TimeSpan(value);
			}
		}

		[DataMember(Name = "TimeInGetConnectionTicks", IsRequired = false)]
		[XmlElement(ElementName = "TimeInGetConnectionTicks")]
		public long TimeInGetConnectionTicks
		{
			get
			{
				return this.TimeInGetConnection.Ticks;
			}
			set
			{
				this.TimeInGetConnection = new TimeSpan(value);
			}
		}

		[XmlElement(ElementName = "TimeInPropertyBagLoadTicks")]
		[DataMember(Name = "TimeInPropertyBagLoadTicks", IsRequired = false)]
		public long TimeInPropertyBagLoadTicks
		{
			get
			{
				return this.TimeInPropertyBagLoad.Ticks;
			}
			set
			{
				this.TimeInPropertyBagLoad = new TimeSpan(value);
			}
		}

		[XmlElement(ElementName = "TimeInMessageItemConversionTicks")]
		[DataMember(Name = "TimeInMessageItemConversionTicks", IsRequired = false)]
		public long TimeInMessageItemConversionTicks
		{
			get
			{
				return this.TimeInMessageItemConversion.Ticks;
			}
			set
			{
				this.TimeInMessageItemConversion = new TimeSpan(value);
			}
		}

		[DataMember(Name = "TimeDeterminingAgeOfItemTicks", IsRequired = false)]
		[XmlElement(ElementName = "TimeDeterminingAgeOfItemTicks")]
		public long TimeDeterminingAgeOfItemTicks
		{
			get
			{
				return this.TimeDeterminingAgeOfItem.Ticks;
			}
			set
			{
				this.TimeDeterminingAgeOfItem = new TimeSpan(value);
			}
		}

		[XmlElement(ElementName = "TimeInMimeConversionTicks")]
		[DataMember(Name = "TimeInMimeConversionTicks", IsRequired = false)]
		public long TimeInMimeConversionTicks
		{
			get
			{
				return this.TimeInMimeConversion.Ticks;
			}
			set
			{
				this.TimeInMimeConversion = new TimeSpan(value);
			}
		}

		[DataMember(Name = "TimeInShouldAnnotateMessageTicks", IsRequired = false)]
		[XmlElement(ElementName = "TimeInShouldAnnotateMessageTicks")]
		public long TimeInShouldAnnotateMessageTicks
		{
			get
			{
				return this.TimeInShouldAnnotateMessage.Ticks;
			}
			set
			{
				this.TimeInShouldAnnotateMessage = new TimeSpan(value);
			}
		}

		[XmlElement(ElementName = "TimeInQueueTicks")]
		[DataMember(Name = "TimeInQueueTicks", IsRequired = false)]
		public long TimeInQueueTicks
		{
			get
			{
				return this.TimeInQueue.Ticks;
			}
			set
			{
				this.TimeInQueue = new TimeSpan(value);
			}
		}

		[DataMember(Name = "TimeInTransportRetrieverTicks", IsRequired = false)]
		[XmlElement(ElementName = "TimeInTransportRetrieverTicks")]
		public long TimeInTransportRetrieverTicks
		{
			get
			{
				return this.TimeInTransportRetriever.Ticks;
			}
			set
			{
				this.TimeInTransportRetriever = new TimeSpan(value);
			}
		}

		[XmlElement(ElementName = "TimeInDocParserTicks")]
		[DataMember(Name = "TimeInDocParserTicks", IsRequired = false)]
		public long TimeInDocParserTicks
		{
			get
			{
				return this.TimeInDocParser.Ticks;
			}
			set
			{
				this.TimeInDocParser = new TimeSpan(value);
			}
		}

		[XmlElement(ElementName = "TimeInWordbreakerTicks")]
		[DataMember(Name = "TimeInWordbreakerTicks", IsRequired = false)]
		public long TimeInWordbreakerTicks
		{
			get
			{
				return this.TimeInWordbreaker.Ticks;
			}
			set
			{
				this.TimeInWordbreaker = new TimeSpan(value);
			}
		}

		[XmlElement(ElementName = "TimeInNLGSubflowTicks")]
		[DataMember(Name = "TimeInNLGSubflowTicks", IsRequired = false)]
		public long TimeInNLGSubflowTicks
		{
			get
			{
				return this.TimeInNLGSubflow.Ticks;
			}
			set
			{
				this.TimeInNLGSubflow = new TimeSpan(value);
			}
		}

		[XmlElement(ElementName = "TimeProcessingFailedMessagesTicks")]
		[DataMember(Name = "TimeProcessingFailedMessagesTicks", IsRequired = false)]
		public long TimeProcessingFailedMessagesTicks
		{
			get
			{
				return this.TimeProcessingFailedMessages.Ticks;
			}
			set
			{
				this.TimeProcessingFailedMessages = new TimeSpan(value);
			}
		}

		[DataMember(Name = "PreFinalSyncDataProcessingDurationTicks", IsRequired = false)]
		[XmlElement(ElementName = "PreFinalSyncDataProcessingDurationTicks")]
		public long PreFinalSyncDataProcessingDurationTicks
		{
			get
			{
				return this.PreFinalSyncDataProcessingDuration.Ticks;
			}
			set
			{
				this.PreFinalSyncDataProcessingDuration = new TimeSpan(value);
			}
		}

		[DataMember(Name = "TotalMessagesProcessed", IsRequired = false)]
		[XmlElement(ElementName = "TotalMessagesProcessed")]
		public int TotalMessagesProcessed { get; set; }

		[DataMember(Name = "MessagesSuccessfullyAnnotated", IsRequired = false)]
		[XmlElement(ElementName = "MessagesSuccessfullyAnnotated")]
		public int MessagesSuccessfullyAnnotated { get; set; }

		[XmlElement(ElementName = "AnnotationSkipped")]
		[DataMember(Name = "AnnotationSkipped", IsRequired = false)]
		public int AnnotationSkipped { get; set; }

		[DataMember(Name = "ConnectionLevelFailures", IsRequired = false)]
		[XmlElement(ElementName = "ConnectionLevelFailures")]
		public int ConnectionLevelFailures { get; set; }

		[DataMember(Name = "MessageLevelFailures", IsRequired = false)]
		[XmlElement(ElementName = "MessageLevelFailures")]
		public int MessageLevelFailures { get; set; }

		[DataMember(Name = "MapiDiagnosticGetProp", IsRequired = false)]
		[XmlElement(ElementName = "MapiDiagnosticGetProp")]
		public string MapiDiagnosticGetProp { get; set; }

		[XmlElement(ElementName = "SessionId")]
		[DataMember(Name = "SessionId", IsRequired = false)]
		public string SessionId { get; set; }

		[DataMember(Name = "SourceLatencyInfo", IsRequired = false)]
		[XmlElement(ElementName = "SourceLatencyInfo")]
		public LatencyInfo SourceLatencyInfo { get; set; }

		[XmlElement(ElementName = "DestinationLatencyInfo")]
		[DataMember(Name = "DestinationLatencyInfo", IsRequired = false)]
		public LatencyInfo DestinationLatencyInfo { get; set; }

		[DataMember(Name = "SourceProviderInfo", IsRequired = false)]
		[XmlElement(ElementName = "SourceProviderInfo")]
		public ProviderInfo SourceProviderInfo { get; set; }

		[DataMember(Name = "DestinationProviderInfo", IsRequired = false)]
		[XmlElement(ElementName = "DestinationProviderInfo")]
		public ProviderInfo DestinationProviderInfo { get; set; }

		public static SessionStatistics operator +(SessionStatistics stats1, SessionStatistics stats2)
		{
			return new SessionStatistics
			{
				SessionId = stats2.SessionId,
				TotalMessagesProcessed = stats1.TotalMessagesProcessed + stats2.TotalMessagesProcessed,
				TotalTimeProcessingMessages = stats1.TotalTimeProcessingMessages + stats2.TotalTimeProcessingMessages,
				TimeInGetConnection = stats1.TimeInGetConnection + stats2.TimeInGetConnection,
				TimeInPropertyBagLoad = stats1.TimeInPropertyBagLoad + stats2.TimeInPropertyBagLoad,
				TimeInMessageItemConversion = stats1.TimeInMessageItemConversion + stats2.TimeInMessageItemConversion,
				TimeDeterminingAgeOfItem = stats1.TimeDeterminingAgeOfItem + stats2.TimeDeterminingAgeOfItem,
				TimeInMimeConversion = stats1.TimeInMimeConversion + stats2.TimeInMimeConversion,
				TimeInShouldAnnotateMessage = stats1.TimeInShouldAnnotateMessage + stats2.TimeInShouldAnnotateMessage,
				TimeInWordbreaker = stats1.TimeInWordbreaker + stats2.TimeInWordbreaker,
				TimeInQueue = stats1.TimeInQueue + stats2.TimeInQueue,
				TimeProcessingFailedMessages = stats1.TimeProcessingFailedMessages + stats2.TimeProcessingFailedMessages,
				TimeInTransportRetriever = stats1.TimeInTransportRetriever + stats2.TimeInTransportRetriever,
				TimeInDocParser = stats1.TimeInDocParser + stats2.TimeInDocParser,
				TimeInNLGSubflow = stats1.TimeInNLGSubflow + stats2.TimeInNLGSubflow,
				MessageLevelFailures = stats1.MessageLevelFailures + stats2.MessageLevelFailures,
				MessagesSuccessfullyAnnotated = stats1.MessagesSuccessfullyAnnotated + stats2.MessagesSuccessfullyAnnotated,
				AnnotationSkipped = stats1.AnnotationSkipped + stats2.AnnotationSkipped,
				ConnectionLevelFailures = stats1.ConnectionLevelFailures + stats2.ConnectionLevelFailures,
				PreFinalSyncDataProcessingDuration = stats2.PreFinalSyncDataProcessingDuration,
				SourceLatencyInfo = stats1.SourceLatencyInfo + stats2.SourceLatencyInfo,
				DestinationLatencyInfo = stats1.DestinationLatencyInfo + stats2.DestinationLatencyInfo,
				SourceProviderInfo = stats1.SourceProviderInfo + stats2.SourceProviderInfo,
				DestinationProviderInfo = stats1.DestinationProviderInfo + stats2.DestinationProviderInfo
			};
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("--------------------------");
			stringBuilder.AppendLine("Source Provider Durations: " + this.SourceProviderInfo.TotalDuration);
			stringBuilder.AppendLine("--------------------------");
			foreach (DurationInfo durationInfo in this.SourceProviderInfo.Durations)
			{
				stringBuilder.AppendLine(string.Format("{0}: {1}", durationInfo.Name, durationInfo.Duration));
			}
			stringBuilder.AppendLine("--------------------------");
			stringBuilder.AppendLine("Destination Provider Durations:" + this.DestinationProviderInfo.TotalDuration);
			stringBuilder.AppendLine("--------------------------");
			foreach (DurationInfo durationInfo2 in this.DestinationProviderInfo.Durations)
			{
				stringBuilder.AppendLine(string.Format("{0}: {1}", durationInfo2.Name, durationInfo2.Duration));
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.AppendLine("--------------------------");
			stringBuilder2.AppendLine("CI WordBreaking Stats:");
			stringBuilder2.AppendLine("--------------------------");
			stringBuilder2.AppendLine("TotalMessagesProcessed: " + this.TotalMessagesProcessed);
			stringBuilder2.Append("TotalTimeProcessingMessages: " + this.TotalTimeProcessingMessages);
			stringBuilder2.Append("TimeInGetConnection: " + this.TimeInGetConnection);
			stringBuilder2.Append("TimeInPropertyBagLoad: " + this.TimeInPropertyBagLoad);
			stringBuilder2.Append("TimeInMessageItemConversion: " + this.TimeInMessageItemConversion);
			stringBuilder2.Append("TimeDeterminingAgeOfItem: " + this.TimeDeterminingAgeOfItem);
			stringBuilder2.Append("TimeInMimeConversion: " + this.TimeInMimeConversion);
			stringBuilder2.Append("TimeInShouldAnnotateMessage: " + this.TimeInShouldAnnotateMessage);
			stringBuilder2.Append("TimeInQueue: " + this.TimeInQueue);
			stringBuilder2.Append("TimeInTransportRetriever: " + this.TimeInTransportRetriever);
			stringBuilder2.Append("TimeInDocParser: " + this.TimeInDocParser);
			stringBuilder2.Append("TimeInWordbreaker: " + this.TimeInWordbreaker);
			stringBuilder2.Append("TimeInNLGSubflow: " + this.TimeInNLGSubflow);
			stringBuilder2.Append("TimeProcessingFailedMessages: " + this.TimeProcessingFailedMessages);
			stringBuilder2.Append("AnnotationSkipped: " + this.AnnotationSkipped);
			stringBuilder2.Append("MessageLevelFailures: " + this.MessageLevelFailures);
			stringBuilder2.Append("ConnectionLevelFailures: " + this.ConnectionLevelFailures);
			return string.Concat(new object[]
			{
				"SessionId: ",
				this.SessionId,
				Environment.NewLine,
				stringBuilder2.ToString(),
				Environment.NewLine,
				"SourceLatencyInfo:",
				Environment.NewLine,
				"------------------",
				Environment.NewLine,
				this.SourceLatencyInfo,
				Environment.NewLine,
				"DestinationLatencyInfo:",
				Environment.NewLine,
				"------------------",
				Environment.NewLine,
				this.DestinationLatencyInfo,
				Environment.NewLine,
				stringBuilder.ToString(),
				Environment.NewLine,
				"PreFinalSyncDataProcessingDuration: ",
				this.PreFinalSyncDataProcessingDuration
			});
		}

		public object Clone()
		{
			object result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
				binaryFormatter.Serialize(memoryStream, this);
				memoryStream.Position = 0L;
				result = binaryFormatter.Deserialize(memoryStream);
			}
			return result;
		}
	}
}
