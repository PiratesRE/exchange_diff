using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class InterceptorRule : ADConfigurationObject
	{
		public InterceptorRule()
		{
		}

		public InterceptorRule(string ruleName)
		{
			base.SetId(InterceptorRule.InterceptorRulesContainer.GetChildId(ruleName));
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return InterceptorRule.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return InterceptorRule.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return InterceptorRule.InterceptorRulesContainer;
			}
		}

		public string Xml
		{
			get
			{
				return (string)this[InterceptorRuleSchema.Xml];
			}
			internal set
			{
				this[InterceptorRuleSchema.Xml] = value;
			}
		}

		public string Version
		{
			get
			{
				return (string)this[InterceptorRuleSchema.Version];
			}
			internal set
			{
				this[InterceptorRuleSchema.Version] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Target
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[InterceptorRuleSchema.Target];
			}
			internal set
			{
				this[InterceptorRuleSchema.Target] = value;
			}
		}

		public DateTime ExpireTime
		{
			get
			{
				return this.ExpireTimeUtc.ToLocalTime();
			}
		}

		public DateTime ExpireTimeUtc
		{
			get
			{
				return (DateTime)this[InterceptorRuleSchema.ExpireTimeUtc];
			}
			internal set
			{
				this[InterceptorRuleSchema.ExpireTimeUtc] = value;
			}
		}

		internal static ADObjectId InterceptorRulesContainer = new ADObjectId("CN=Interceptor Rules,CN=Transport Settings");

		private static InterceptorRuleSchema schema = ObjectSchema.GetInstance<InterceptorRuleSchema>();

		private static string mostDerivedClass = "msExchTransportRule";
	}
}
