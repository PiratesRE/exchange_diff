using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class ServiceAvailabilityReport : ConfigurableObject
	{
		public ServiceAvailabilityReport() : base(new ServiceAvailabilityReportPropertyBag())
		{
			this.dailyStatistics = new MultiValuedProperty<DailyAvailability>();
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ServiceAvailabilityReport.schema;
			}
		}

		public new ObjectId Identity
		{
			get
			{
				return (ObjectId)this.propertyBag[ServiceAvailabilityReportSchema.Identity];
			}
			internal set
			{
				this.propertyBag[ServiceAvailabilityReportSchema.Identity] = value;
			}
		}

		public string IdentityFormatted
		{
			get
			{
				if (this.Identity != null)
				{
					ADObjectId adobjectId = (ADObjectId)this.Identity;
					return adobjectId.Name;
				}
				return null;
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

		public DateTime StartDate
		{
			get
			{
				return (DateTime)this.propertyBag[ServiceAvailabilityReportSchema.StartDate];
			}
			internal set
			{
				this.propertyBag[ServiceAvailabilityReportSchema.StartDate] = value;
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
				return (DateTime)this.propertyBag[ServiceAvailabilityReportSchema.EndDate];
			}
			internal set
			{
				this.propertyBag[ServiceAvailabilityReportSchema.EndDate] = value;
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
				return (double)this.propertyBag[ServiceAvailabilityReportSchema.AvailabilityPercentage];
			}
			internal set
			{
				this.propertyBag[ServiceAvailabilityReportSchema.AvailabilityPercentage] = value;
			}
		}

		public string AvailabilityPercentageFormatted
		{
			get
			{
				return this.AvailabilityPercentage.ToString("P2");
			}
		}

		private static ServiceAvailabilityReportSchema schema = ObjectSchema.GetInstance<ServiceAvailabilityReportSchema>();

		private MultiValuedProperty<DailyAvailability> dailyStatistics;
	}
}
