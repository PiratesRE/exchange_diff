using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public sealed class MicrosoftMTAConfiguration : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return MicrosoftMTAConfiguration.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MicrosoftMTAConfiguration.mostDerivedClass;
			}
		}

		[Parameter(Mandatory = true)]
		public string LocalDesig
		{
			get
			{
				return (string)this[MicrosoftMTAConfigurationSchema.LocalDesig];
			}
			set
			{
				this[MicrosoftMTAConfigurationSchema.LocalDesig] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public int TransRetryMins
		{
			get
			{
				return (int)this[MicrosoftMTAConfigurationSchema.TransRetryMins];
			}
			set
			{
				this[MicrosoftMTAConfigurationSchema.TransRetryMins] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public int TransTimeoutMins
		{
			get
			{
				return (int)this[MicrosoftMTAConfigurationSchema.TransTimeoutMins];
			}
			set
			{
				this[MicrosoftMTAConfigurationSchema.TransTimeoutMins] = value;
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this[MicrosoftMTAConfigurationSchema.ExchangeLegacyDN];
			}
			internal set
			{
				this[MicrosoftMTAConfigurationSchema.ExchangeLegacyDN] = value;
			}
		}

		public const string MTAObjectRdn = "Microsoft MTA";

		private static MicrosoftMTAConfigurationSchema schema = ObjectSchema.GetInstance<MicrosoftMTAConfigurationSchema>();

		private static string mostDerivedClass = "mTA";
	}
}
