using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueStatusReport : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.RequestId.ToString());
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[AsyncQueueStatusReportSchema.OrganizationalUnitRootProperty];
			}
		}

		public Guid RequestId
		{
			get
			{
				return (Guid)this[AsyncQueueStatusReportSchema.RequestIdProperty];
			}
		}

		public string FriendlyName
		{
			get
			{
				return (string)this[AsyncQueueStatusReportSchema.FriendlyNameProperty];
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return (DateTime)this[AsyncQueueStatusReportSchema.CreatedDatetimeProperty];
			}
		}

		public string StepName
		{
			get
			{
				return (string)this[AsyncQueueStatusReportSchema.StepNameProperty];
			}
		}

		public Guid RequestStepId
		{
			get
			{
				return (Guid)this[AsyncQueueStatusReportSchema.RequestStepIdProperty];
			}
		}

		public AsyncQueueStatus StepStatus
		{
			get
			{
				return (AsyncQueueStatus)this[AsyncQueueStatusReportSchema.StepStatusProperty];
			}
		}

		public int FetchCount
		{
			get
			{
				return (int)this[AsyncQueueStatusReportSchema.FetchCountProperty];
			}
		}

		public int ErrorCount
		{
			get
			{
				return (int)this[AsyncQueueStatusReportSchema.ErrorCountProperty];
			}
		}

		public DateTime? NextFetchDatetime
		{
			get
			{
				return (DateTime?)this[AsyncQueueStatusReportSchema.NextFetchDatetimeProperty];
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(AsyncQueueStatusReportSchema);
		}
	}
}
