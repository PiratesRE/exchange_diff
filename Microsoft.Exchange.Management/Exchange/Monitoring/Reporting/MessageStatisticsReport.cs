using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Monitoring.Reporting
{
	[Serializable]
	public sealed class MessageStatisticsReport : ConfigurableObject
	{
		public MessageStatisticsReport() : base(new MessageStatisticsReportPropertyBag())
		{
		}

		public new ObjectId Identity
		{
			get
			{
				return (ObjectId)this.propertyBag[TransportReportSchema.Identity];
			}
			internal set
			{
				this.propertyBag[TransportReportSchema.Identity] = value;
			}
		}

		public ExDateTime StartDate
		{
			get
			{
				return (ExDateTime)this.propertyBag[TransportReportSchema.StartDate];
			}
			internal set
			{
				this.propertyBag[TransportReportSchema.StartDate] = value;
			}
		}

		public ExDateTime EndDate
		{
			get
			{
				return (ExDateTime)this.propertyBag[TransportReportSchema.EndDate];
			}
			internal set
			{
				this.propertyBag[TransportReportSchema.EndDate] = value;
			}
		}

		public int TotalMessagesSent
		{
			get
			{
				return (int)this.propertyBag[MessageStatisticsReportSchema.TotalMessagesSent];
			}
			internal set
			{
				this.propertyBag[MessageStatisticsReportSchema.TotalMessagesSent] = value;
			}
		}

		public int TotalMessagesReceived
		{
			get
			{
				return (int)this.propertyBag[MessageStatisticsReportSchema.TotalMessagesReceived];
			}
			internal set
			{
				this.propertyBag[MessageStatisticsReportSchema.TotalMessagesReceived] = value;
			}
		}

		public int TotalMessagesSentToForeign
		{
			get
			{
				return (int)this.propertyBag[MessageStatisticsReportSchema.TotalMessagesSentToForeign];
			}
			internal set
			{
				this.propertyBag[MessageStatisticsReportSchema.TotalMessagesSentToForeign] = value;
			}
		}

		public int TotalMessagesReceivedFromForeign
		{
			get
			{
				return (int)this.propertyBag[MessageStatisticsReportSchema.TotalMessagesReceivedFromForeign];
			}
			internal set
			{
				this.propertyBag[MessageStatisticsReportSchema.TotalMessagesReceivedFromForeign] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MessageStatisticsReport.Schema;
			}
		}

		private new bool IsValid
		{
			get
			{
				return true;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static MessageStatisticsReportSchema Schema = ObjectSchema.GetInstance<MessageStatisticsReportSchema>();
	}
}
