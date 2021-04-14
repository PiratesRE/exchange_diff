using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Search.Core.Diagnostics
{
	internal class TransportFlowOperatorTimings
	{
		public TransportFlowOperatorTimings(string timingString)
		{
			this.ProcessOperatorTimings(timingString);
		}

		public long TimeInQueueInMsec { get; set; }

		public long TimeInTransportRetrieverInMsec { get; set; }

		public long TimeInDocParserInMsec { get; set; }

		public long TimeInWordbreakerInMsec { get; set; }

		public long TimeInNLGSubflowInMsec { get; set; }

		public long TimeSpentWaitingForConnectInMsec { get; set; }

		public long TimeSpentMessageSendInMsec { get; set; }

		public string OperatorTimingString { get; private set; }

		private void ProcessOperatorTimings(string timingString)
		{
			this.OperatorTimingString = timingString;
			if (string.IsNullOrWhiteSpace(this.OperatorTimingString))
			{
				return;
			}
			foreach (OperatorTimingEntry operatorTimingEntry in OperatorTimingEntry.DeserializeList(timingString))
			{
				string name;
				if ((name = operatorTimingEntry.Name) != null)
				{
					if (!(name == "RetrieverOperator"))
					{
						if (!(name == "PostDocParserOperator"))
						{
							if (!(name == "PostWordBreakerDiagnosticOperator"))
							{
								if (name == "TransportWriterProducer")
								{
									if (operatorTimingEntry.Location == OperatorLocation.BeginWrite)
									{
										this.TimeInNLGSubflowInMsec = operatorTimingEntry.Elapsed;
									}
								}
							}
							else if (operatorTimingEntry.Location == OperatorLocation.BeginProcessRecord)
							{
								this.TimeInWordbreakerInMsec = operatorTimingEntry.Elapsed;
							}
						}
						else if (operatorTimingEntry.Location == OperatorLocation.BeginProcessRecord)
						{
							this.TimeInDocParserInMsec = operatorTimingEntry.Elapsed;
						}
					}
					else if (operatorTimingEntry.Location == OperatorLocation.BeginProcessRecord)
					{
						this.TimeInQueueInMsec = operatorTimingEntry.Elapsed;
					}
					else if (operatorTimingEntry.Location == OperatorLocation.EndProcessRecord)
					{
						this.TimeInTransportRetrieverInMsec = operatorTimingEntry.Elapsed;
					}
				}
			}
		}

		private const string TransportRetrieverOperatorName = "RetrieverOperator";

		private const string PostDocParserProducerName = "PostDocParserOperator";

		private const string TransportWriterProducerName = "TransportWriterProducer";

		private const string PostWordBreakerDiagnosticOperatorName = "PostWordBreakerDiagnosticOperator";

		internal static readonly List<string> TimingEntryNames = new List<string>
		{
			"RetrieverOperator",
			"PostDocParserOperator",
			"PostWordBreakerDiagnosticOperator",
			"TransportWriterProducer"
		};
	}
}
