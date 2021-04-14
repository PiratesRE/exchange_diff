using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueReport : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		public string Report
		{
			get
			{
				return (string)this[AsyncQueueReportSchema.ReportProperty];
			}
			set
			{
				this[AsyncQueueReportSchema.ReportProperty] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(AsyncQueueReportSchema);
		}

		private ConfigObjectId identity = new ConfigObjectId(Guid.NewGuid().ToString());
	}
}
