using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class MRSHealthCheckOutcome : ConfigurableObject
	{
		public MRSHealthCheckOutcome() : base(new SimpleProviderPropertyBag())
		{
		}

		internal MRSHealthCheckOutcome(string server, MRSHealthCheckId checkId, bool passed, LocalizedString msg) : this()
		{
			this[SimpleProviderObjectSchema.Identity] = new MRSHealthCheckOutcomeObjectId(server);
			this.Check = checkId;
			this.Passed = passed;
			this.Message = msg;
		}

		public MRSHealthCheckId Check
		{
			get
			{
				return (MRSHealthCheckId)this[MRSHealthCheckOutcomeSchema.Check];
			}
			private set
			{
				this[MRSHealthCheckOutcomeSchema.Check] = value;
			}
		}

		public bool Passed
		{
			get
			{
				return (bool)(this[MRSHealthCheckOutcomeSchema.Passed] ?? false);
			}
			private set
			{
				this[MRSHealthCheckOutcomeSchema.Passed] = value;
			}
		}

		public LocalizedString Message
		{
			get
			{
				return (LocalizedString)this[MRSHealthCheckOutcomeSchema.Message];
			}
			private set
			{
				this[MRSHealthCheckOutcomeSchema.Message] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MRSHealthCheckOutcome.schema;
			}
		}

		private static MRSHealthCheckOutcomeSchema schema = ObjectSchema.GetInstance<MRSHealthCheckOutcomeSchema>();
	}
}
