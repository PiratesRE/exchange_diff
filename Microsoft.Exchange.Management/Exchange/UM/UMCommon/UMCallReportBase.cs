using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	public abstract class UMCallReportBase : ConfigurableObject
	{
		public UMCallReportBase(ObjectId identity) : base(new SimpleProviderPropertyBag())
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.propertyBag.SetField(SimpleProviderObjectSchema.Identity, identity);
		}

		public float? NMOS
		{
			get
			{
				return (float?)this[UMCallReportBaseSchema.NMOS];
			}
			internal set
			{
				this[UMCallReportBaseSchema.NMOS] = value;
			}
		}

		public float? NMOSDegradation
		{
			get
			{
				return (float?)this[UMCallReportBaseSchema.NMOSDegradation];
			}
			internal set
			{
				this[UMCallReportBaseSchema.NMOSDegradation] = value;
			}
		}

		public float? PercentPacketLoss
		{
			get
			{
				return (float?)this[UMCallReportBaseSchema.PercentPacketLoss];
			}
			internal set
			{
				this[UMCallReportBaseSchema.PercentPacketLoss] = value;
			}
		}

		public float? Jitter
		{
			get
			{
				return (float?)this[UMCallReportBaseSchema.Jitter];
			}
			internal set
			{
				this[UMCallReportBaseSchema.Jitter] = value;
			}
		}

		public float? RoundTripMilliseconds
		{
			get
			{
				return (float?)this[UMCallReportBaseSchema.RoundTripMilliseconds];
			}
			internal set
			{
				this[UMCallReportBaseSchema.RoundTripMilliseconds] = value;
			}
		}

		public float? BurstLossDurationMilliseconds
		{
			get
			{
				return (float?)this[UMCallReportBaseSchema.BurstLossDurationMilliseconds];
			}
			internal set
			{
				this[UMCallReportBaseSchema.BurstLossDurationMilliseconds] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}
	}
}
