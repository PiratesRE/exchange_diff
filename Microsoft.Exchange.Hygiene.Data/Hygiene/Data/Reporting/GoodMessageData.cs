using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class GoodMessageData : ConfigurablePropertyBag
	{
		public bool GoodMessageExists
		{
			get
			{
				return (bool)this[GoodMessageSchema.GoodMessageExistsProperty];
			}
			set
			{
				this[GoodMessageSchema.GoodMessageExistsProperty] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.identity.ToString());
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(GoodMessageSchema);
		}

		private readonly Guid identity = ReportingSession.GenerateNewId();
	}
}
