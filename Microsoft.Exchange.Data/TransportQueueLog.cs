using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Security.Compliance;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class TransportQueueLog : ConfigurableObject
	{
		public TransportQueueLog() : base(new SimpleProviderPropertyBag())
		{
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.QueueId.ToString());
			}
		}

		[XmlAttribute]
		public Guid QueueId
		{
			get
			{
				return (Guid)this[TransportQueueLogSchema.QueueIdProperty];
			}
			set
			{
				this[TransportQueueLogSchema.QueueIdProperty] = value;
			}
		}

		[XmlAttribute]
		public string QueueName
		{
			get
			{
				return (string)this[TransportQueueLogSchema.QueueNameProperty];
			}
			set
			{
				this[TransportQueueLogSchema.QueueNameProperty] = value;
				this.QueueId = TransportQueueLog.GenerateGuidFromString(value);
			}
		}

		[XmlAttribute]
		public DateTime SnapshotDatetime
		{
			get
			{
				return (DateTime)this[TransportQueueLogSchema.SnapshotDatetimeProperty];
			}
			set
			{
				this[TransportQueueLogSchema.SnapshotDatetimeProperty] = value;
			}
		}

		[XmlAttribute]
		public string TlsDomain
		{
			get
			{
				return (string)this[TransportQueueLogSchema.TlsDomainProperty];
			}
			set
			{
				this[TransportQueueLogSchema.TlsDomainProperty] = value;
			}
		}

		[XmlAttribute]
		public string NextHopDomain
		{
			get
			{
				return (string)this[TransportQueueLogSchema.NextHopDomainProperty];
			}
			set
			{
				this[TransportQueueLogSchema.NextHopDomainProperty] = value;
			}
		}

		[XmlAttribute]
		public string NextHopKey
		{
			get
			{
				return (string)this[TransportQueueLogSchema.NextHopKeyProperty];
			}
			set
			{
				this[TransportQueueLogSchema.NextHopKeyProperty] = value;
			}
		}

		[XmlAttribute]
		public Guid NextHopConnector
		{
			get
			{
				return (Guid)this[TransportQueueLogSchema.NextHopConnectorProperty];
			}
			set
			{
				this[TransportQueueLogSchema.NextHopConnectorProperty] = value;
			}
		}

		[XmlAttribute]
		public string NextHopCategory
		{
			get
			{
				return (string)this[TransportQueueLogSchema.NextHopCategoryProperty];
			}
			set
			{
				this[TransportQueueLogSchema.NextHopCategoryProperty] = value;
			}
		}

		[XmlAttribute]
		public string DeliveryType
		{
			get
			{
				return (string)this[TransportQueueLogSchema.DeliveryTypeProperty];
			}
			set
			{
				this[TransportQueueLogSchema.DeliveryTypeProperty] = value;
			}
		}

		[XmlAttribute]
		public string RiskLevel
		{
			get
			{
				return (string)this[TransportQueueLogSchema.RiskLevelProperty];
			}
			set
			{
				this[TransportQueueLogSchema.RiskLevelProperty] = value;
			}
		}

		[XmlAttribute]
		public string OutboundIPPool
		{
			get
			{
				return (string)this[TransportQueueLogSchema.OutboundIPPoolProperty];
			}
			set
			{
				this[TransportQueueLogSchema.OutboundIPPoolProperty] = value;
			}
		}

		[XmlAttribute]
		public string Status
		{
			get
			{
				return (string)this[TransportQueueLogSchema.StatusProperty];
			}
			set
			{
				this[TransportQueueLogSchema.StatusProperty] = value;
			}
		}

		[XmlAttribute]
		public string LastError
		{
			get
			{
				return (string)this[TransportQueueLogSchema.LastErrorProperty];
			}
			set
			{
				this[TransportQueueLogSchema.LastErrorProperty] = value;
			}
		}

		[XmlAttribute]
		public int MessageCount
		{
			get
			{
				return (int)this[TransportQueueLogSchema.MessageCountProperty];
			}
			set
			{
				this[TransportQueueLogSchema.MessageCountProperty] = value;
			}
		}

		[XmlAttribute]
		public int DeferredMessageCount
		{
			get
			{
				return (int)this[TransportQueueLogSchema.DeferredMessageCountProperty];
			}
			set
			{
				this[TransportQueueLogSchema.DeferredMessageCountProperty] = value;
			}
		}

		[XmlAttribute]
		public int LockedMessageCount
		{
			get
			{
				return (int)this[TransportQueueLogSchema.LockedMessageCountProperty];
			}
			set
			{
				this[TransportQueueLogSchema.LockedMessageCountProperty] = value;
			}
		}

		[XmlAttribute]
		public double IncomingRate
		{
			get
			{
				return (double)this[TransportQueueLogSchema.IncomingRateProperty];
			}
			set
			{
				this[TransportQueueLogSchema.IncomingRateProperty] = value;
			}
		}

		[XmlAttribute]
		public double OutgoingRate
		{
			get
			{
				return (double)this[TransportQueueLogSchema.OutgoingRateProperty];
			}
			set
			{
				this[TransportQueueLogSchema.OutgoingRateProperty] = value;
			}
		}

		[XmlAttribute]
		public double Velocity
		{
			get
			{
				return (double)this[TransportQueueLogSchema.VelocityProperty];
			}
			set
			{
				this[TransportQueueLogSchema.VelocityProperty] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return TransportQueueLog.SchemaObject;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public static MultiValuedProperty<TransportQueueLog> Parse(string stringXml)
		{
			MultiValuedProperty<TransportQueueLog> queueLogs = new MultiValuedProperty<TransportQueueLog>();
			if (!string.IsNullOrWhiteSpace(stringXml))
			{
				using (XmlReader xmlReader = XmlReader.Create(new StringReader(stringXml), new XmlReaderSettings
				{
					ConformanceLevel = ConformanceLevel.Fragment
				}))
				{
					TransportQueueLogs transportQueueLogs = (TransportQueueLogs)TransportQueueLogs.Serializer.Deserialize(xmlReader);
					if (transportQueueLogs != null && transportQueueLogs.Count > 0)
					{
						transportQueueLogs.ForEach(delegate(TransportQueueLog i)
						{
							queueLogs.Add(i);
						});
					}
				}
			}
			return queueLogs;
		}

		private static Guid GenerateGuidFromString(string inputString)
		{
			Guid result;
			using (MessageDigestForNonCryptographicPurposes messageDigestForNonCryptographicPurposes = new MessageDigestForNonCryptographicPurposes())
			{
				result = new Guid(messageDigestForNonCryptographicPurposes.ComputeHash(Encoding.Default.GetBytes(inputString)));
			}
			return result;
		}

		private static readonly TransportQueueLogSchema SchemaObject = ObjectSchema.GetInstance<TransportQueueLogSchema>();
	}
}
