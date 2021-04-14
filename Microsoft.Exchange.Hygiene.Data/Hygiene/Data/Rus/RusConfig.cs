using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.Rus
{
	internal class RusConfig : ADObject
	{
		public RusConfig()
		{
			base.SetId(new ADObjectId(DalHelper.GetTenantDistinguishedName("RUS_Default"), CombGuidGenerator.NewGuid()));
			base.Name = "RUS_Default";
		}

		public string UniversalManifestVersion
		{
			get
			{
				return (string)this[RusConfigSchema.UniversalManifestVersion];
			}
			set
			{
				this[RusConfigSchema.UniversalManifestVersion] = value;
			}
		}

		public string UniversalManifestVersionV2
		{
			get
			{
				return (string)this[RusConfigSchema.UniversalManifestVersionV2];
			}
			set
			{
				this[RusConfigSchema.UniversalManifestVersionV2] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return RusConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RusConfig.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public const string RusDistinguishedName = "RUS_Default";

		private static readonly string mostDerivedClass = "RusConfig";

		private static readonly RusConfigSchema schema = ObjectSchema.GetInstance<RusConfigSchema>();
	}
}
