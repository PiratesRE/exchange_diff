using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class RecipientStatisticsReport : ConfigurableObject
	{
		public RecipientStatisticsReport() : base(new RecipientStatisticsReportPropertyBag())
		{
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return RecipientStatisticsReport.schema;
			}
		}

		public new ADObjectId Identity
		{
			get
			{
				return (ADObjectId)this.propertyBag[RecipientStatisticsReportSchema.Identity];
			}
			internal set
			{
				this.propertyBag[RecipientStatisticsReportSchema.Identity] = value;
			}
		}

		private new bool IsValid
		{
			get
			{
				return true;
			}
		}

		public uint TotalNumberOfMailboxes
		{
			get
			{
				return (uint)this.propertyBag[RecipientStatisticsReportSchema.TotalNumberOfMailboxes];
			}
			internal set
			{
				this.propertyBag[RecipientStatisticsReportSchema.TotalNumberOfMailboxes] = value;
			}
		}

		public uint TotalNumberOfActiveMailboxes
		{
			get
			{
				return (uint)this.propertyBag[RecipientStatisticsReportSchema.TotalNumberOfActiveMailboxes];
			}
			internal set
			{
				this.propertyBag[RecipientStatisticsReportSchema.TotalNumberOfActiveMailboxes] = value;
			}
		}

		public uint NumberOfContacts
		{
			get
			{
				return (uint)this.propertyBag[RecipientStatisticsReportSchema.NumberOfContacts];
			}
			internal set
			{
				this.propertyBag[RecipientStatisticsReportSchema.NumberOfContacts] = value;
			}
		}

		public uint NumberOfDistributionLists
		{
			get
			{
				return (uint)this.propertyBag[RecipientStatisticsReportSchema.NumberOfDistributionLists];
			}
			internal set
			{
				this.propertyBag[RecipientStatisticsReportSchema.NumberOfDistributionLists] = value;
			}
		}

		public DateTime LastUpdated
		{
			get
			{
				return (DateTime)this.propertyBag[RecipientStatisticsReportSchema.LastUpdated];
			}
			internal set
			{
				this.propertyBag[RecipientStatisticsReportSchema.LastUpdated] = value;
			}
		}

		public string LastUpdatedFormatted
		{
			get
			{
				return this.LastUpdated.ToShortDateString();
			}
		}

		private static RecipientStatisticsReportSchema schema = ObjectSchema.GetInstance<RecipientStatisticsReportSchema>();
	}
}
