using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Management.TransportLogSearchTasks
{
	[Serializable]
	public sealed class MessageTrackingLatency
	{
		private MessageTrackingLatency(MessageTrackingEvent mte, LatencyComponent component)
		{
			this.mte = mte;
			this.component = component;
		}

		public DateTime Timestamp
		{
			get
			{
				return this.mte.Timestamp;
			}
		}

		public string ClientIp
		{
			get
			{
				return this.mte.ClientIp;
			}
		}

		public string ClientHostname
		{
			get
			{
				return this.mte.ClientHostname;
			}
		}

		public string ServerIp
		{
			get
			{
				return this.mte.ServerIp;
			}
		}

		public string ServerHostname
		{
			get
			{
				return this.mte.ServerHostname;
			}
		}

		public string SourceContext
		{
			get
			{
				return this.mte.SourceContext;
			}
		}

		public string ConnectorId
		{
			get
			{
				return this.mte.ConnectorId;
			}
		}

		public string Source
		{
			get
			{
				return this.mte.Source;
			}
		}

		public string EventId
		{
			get
			{
				return this.mte.EventId;
			}
		}

		public string InternalMessageId
		{
			get
			{
				return this.mte.InternalMessageId;
			}
		}

		public string MessageId
		{
			get
			{
				return this.mte.MessageId;
			}
		}

		public string[] Recipients
		{
			get
			{
				return this.mte.Recipients;
			}
		}

		public string[] RecipientStatus
		{
			get
			{
				return this.mte.RecipientStatus;
			}
		}

		public int? TotalBytes
		{
			get
			{
				return this.mte.TotalBytes;
			}
		}

		public int? RecipientCount
		{
			get
			{
				return this.mte.RecipientCount;
			}
		}

		public string RelatedRecipientAddress
		{
			get
			{
				return this.mte.RelatedRecipientAddress;
			}
		}

		public string[] Reference
		{
			get
			{
				return this.mte.Reference;
			}
		}

		public string MessageSubject
		{
			get
			{
				return this.mte.MessageSubject;
			}
		}

		public string Sender
		{
			get
			{
				return this.mte.Sender;
			}
		}

		public string ReturnPath
		{
			get
			{
				return this.mte.ReturnPath;
			}
		}

		public EnhancedTimeSpan? MessageLatency
		{
			get
			{
				return this.mte.MessageLatency;
			}
		}

		public MessageLatencyType MessageLatencyType
		{
			get
			{
				return this.mte.MessageLatencyType;
			}
		}

		public string ComponentServerFqdn
		{
			get
			{
				return this.component.ServerFqdn;
			}
		}

		public string ComponentCode
		{
			get
			{
				return this.component.Code;
			}
		}

		public LocalizedString ComponentName
		{
			get
			{
				return this.component.Name;
			}
		}

		public Unlimited<EnhancedTimeSpan> ComponentLatency
		{
			get
			{
				if ((double)this.component.Latency >= TransportAppConfig.LatencyTrackerConfig.MaxLatency.TotalSeconds)
				{
					return Unlimited<EnhancedTimeSpan>.UnlimitedValue;
				}
				return EnhancedTimeSpan.FromSeconds((double)this.component.Latency);
			}
		}

		public int ComponentSequenceNumber
		{
			get
			{
				return this.component.SequenceNumber;
			}
		}

		public static IEnumerable<MessageTrackingLatency> GetLatencies(MessageTrackingEvent mte)
		{
			if (mte != null)
			{
				ComponentLatencyParser parser = new ComponentLatencyParser();
				if (parser.TryParse(mte.MessageInfo))
				{
					foreach (LatencyComponent component in parser.Components)
					{
						yield return new MessageTrackingLatency(mte, component);
					}
				}
			}
			yield break;
		}

		private MessageTrackingEvent mte;

		private LatencyComponent component;
	}
}
