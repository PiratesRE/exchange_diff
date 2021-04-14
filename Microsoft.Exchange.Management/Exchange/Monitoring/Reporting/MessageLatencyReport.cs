using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Monitoring.Reporting
{
	[Serializable]
	public sealed class MessageLatencyReport : ConfigurableObject
	{
		public MessageLatencyReport() : base(new MessageLatencyReportPropertyBag())
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

		public short SlaTargetInSeconds
		{
			get
			{
				return (short)this.propertyBag[MessageLatencyReportSchema.SlaTargetInSeconds];
			}
			internal set
			{
				this.propertyBag[MessageLatencyReportSchema.SlaTargetInSeconds] = value;
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

		public decimal PercentOfMessageInGivenSla
		{
			get
			{
				return (decimal)this.propertyBag[MessageLatencyReportSchema.PercentOfMessageInGivenSla];
			}
			internal set
			{
				this.propertyBag[MessageLatencyReportSchema.PercentOfMessageInGivenSla] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MessageLatencyReport.Schema;
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

		private static MessageLatencyReportSchema Schema = ObjectSchema.GetInstance<MessageLatencyReportSchema>();
	}
}
