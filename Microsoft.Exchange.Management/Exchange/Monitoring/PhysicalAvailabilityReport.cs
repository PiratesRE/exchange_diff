using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class PhysicalAvailabilityReport : ConfigurableObject
	{
		public PhysicalAvailabilityReport() : base(new PhysicalAvailabilityReportPropertyBag())
		{
			this.dailyStatistics = new MultiValuedProperty<DailyAvailability>();
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return PhysicalAvailabilityReport.schema;
			}
		}

		private new bool IsValid
		{
			get
			{
				return true;
			}
		}

		public MultiValuedProperty<DailyAvailability> DailyStatistics
		{
			get
			{
				return this.dailyStatistics;
			}
			internal set
			{
				this.dailyStatistics = value;
			}
		}

		public string SiteName
		{
			get
			{
				return (string)this.propertyBag[PhysicalAvailabilityReportSchema.SiteName];
			}
			internal set
			{
				this.propertyBag[PhysicalAvailabilityReportSchema.SiteName] = value;
			}
		}

		public DateTime StartDate
		{
			get
			{
				return (DateTime)this.propertyBag[PhysicalAvailabilityReportSchema.StartDate];
			}
			internal set
			{
				this.propertyBag[PhysicalAvailabilityReportSchema.StartDate] = value;
			}
		}

		public string StartDateFormatted
		{
			get
			{
				return this.StartDate.ToShortDateString();
			}
		}

		public DateTime EndDate
		{
			get
			{
				return (DateTime)this.propertyBag[PhysicalAvailabilityReportSchema.EndDate];
			}
			internal set
			{
				this.propertyBag[PhysicalAvailabilityReportSchema.EndDate] = value;
			}
		}

		public string EndDateFormatted
		{
			get
			{
				return this.EndDate.ToShortDateString();
			}
		}

		public double AvailabilityPercentage
		{
			get
			{
				return (double)this.propertyBag[PhysicalAvailabilityReportSchema.AvailabilityPercentage];
			}
			internal set
			{
				this.propertyBag[PhysicalAvailabilityReportSchema.AvailabilityPercentage] = value;
			}
		}

		public string AvailabilityPercentageFormatted
		{
			get
			{
				return this.AvailabilityPercentage.ToString("P2");
			}
		}

		public double RawAvailabilityPercentage
		{
			get
			{
				return (double)this.propertyBag[PhysicalAvailabilityReportSchema.RawAvailabilityPercentage];
			}
			internal set
			{
				this.propertyBag[PhysicalAvailabilityReportSchema.RawAvailabilityPercentage] = value;
			}
		}

		public string RawAvailabilityPercentageFormatted
		{
			get
			{
				return this.RawAvailabilityPercentage.ToString("P2");
			}
		}

		public ADObjectId Database
		{
			get
			{
				return (ADObjectId)this.propertyBag[PhysicalAvailabilityReportSchema.Database];
			}
			internal set
			{
				this.propertyBag[PhysicalAvailabilityReportSchema.Database] = value;
			}
		}

		public ADObjectId ExchangeServer
		{
			get
			{
				return (ADObjectId)this.propertyBag[PhysicalAvailabilityReportSchema.ExchangeServer];
			}
			internal set
			{
				this.propertyBag[PhysicalAvailabilityReportSchema.ExchangeServer] = value;
			}
		}

		private static PhysicalAvailabilityReportSchema schema = ObjectSchema.GetInstance<PhysicalAvailabilityReportSchema>();

		private MultiValuedProperty<DailyAvailability> dailyStatistics;
	}
}
