using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class TransportProcessingQuotaConfig : ConfigurableObject
	{
		public TransportProcessingQuotaConfig() : base(new SimpleProviderPropertyBag())
		{
			this[TransportProcessingQuotaConfigSchema.Id] = this.identity;
			this[TransportProcessingQuotaConfigSchema.SettingName] = base.GetType().Name;
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.identity.ToString());
			}
		}

		public bool ThrottlingEnabled
		{
			get
			{
				return (bool)this[TransportProcessingQuotaConfigSchema.ThrottlingEnabled];
			}
			set
			{
				this[TransportProcessingQuotaConfigSchema.ThrottlingEnabled] = value;
			}
		}

		public bool CalculationEnabled
		{
			get
			{
				return (bool)this[TransportProcessingQuotaConfigSchema.CalculationEnabled];
			}
			set
			{
				this[TransportProcessingQuotaConfigSchema.CalculationEnabled] = value;
			}
		}

		public double AmWeight
		{
			get
			{
				return (double)this[TransportProcessingQuotaConfigSchema.AmWeight];
			}
			set
			{
				this[TransportProcessingQuotaConfigSchema.AmWeight] = value;
			}
		}

		public double AsWeight
		{
			get
			{
				return (double)this[TransportProcessingQuotaConfigSchema.AsWeight];
			}
			set
			{
				this[TransportProcessingQuotaConfigSchema.AsWeight] = value;
			}
		}

		public int CalculationFrequency
		{
			get
			{
				return (int)this[TransportProcessingQuotaConfigSchema.CalculationFrequency];
			}
			set
			{
				this[TransportProcessingQuotaConfigSchema.CalculationFrequency] = value;
			}
		}

		public int CostThreshold
		{
			get
			{
				return (int)this[TransportProcessingQuotaConfigSchema.CostThreshold];
			}
			set
			{
				this[TransportProcessingQuotaConfigSchema.CostThreshold] = value;
			}
		}

		public double EtrWeight
		{
			get
			{
				return (double)this[TransportProcessingQuotaConfigSchema.EtrWeight];
			}
			set
			{
				this[TransportProcessingQuotaConfigSchema.EtrWeight] = value;
			}
		}

		public int TimeWindow
		{
			get
			{
				return (int)this[TransportProcessingQuotaConfigSchema.TimeWindow];
			}
			set
			{
				this[TransportProcessingQuotaConfigSchema.TimeWindow] = value;
			}
		}

		public double ThrottleFactor
		{
			get
			{
				return (double)this[TransportProcessingQuotaConfigSchema.ThrottleFactor];
			}
			set
			{
				this[TransportProcessingQuotaConfigSchema.ThrottleFactor] = value;
			}
		}

		public double RelativeCostThreshold
		{
			get
			{
				return (double)this[TransportProcessingQuotaConfigSchema.RelativeCostThreshold];
			}
			set
			{
				this[TransportProcessingQuotaConfigSchema.RelativeCostThreshold] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return TransportProcessingQuotaConfig.SchemaObject;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly TransportProcessingQuotaConfigSchema SchemaObject = ObjectSchema.GetInstance<TransportProcessingQuotaConfigSchema>();

		private readonly Guid identity = Guid.Parse("A6E1583F-3A26-DC67-A881-229DB7431D92");
	}
}
