using System;
using System.Security;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ProbeOrganizationInfo : ADObject
	{
		public ADObjectId ProbeOrganizationId
		{
			get
			{
				return this[ADObjectSchema.OrganizationalUnitRoot] as ADObjectId;
			}
			set
			{
				this[ADObjectSchema.OrganizationalUnitRoot] = value;
			}
		}

		public string FeatureTag
		{
			get
			{
				return this[ProbeOrganizationInfo.ProbeOrganizationInfoSchema.FeatureTagProperty] as string;
			}
			set
			{
				this[ProbeOrganizationInfo.ProbeOrganizationInfoSchema.FeatureTagProperty] = value;
			}
		}

		public string Region
		{
			get
			{
				return this[ProbeOrganizationInfo.ProbeOrganizationInfoSchema.RegionProperty] as string;
			}
			set
			{
				this[ProbeOrganizationInfo.ProbeOrganizationInfoSchema.RegionProperty] = value;
			}
		}

		public SecureString LoginPassword
		{
			get
			{
				return this[ProbeOrganizationInfo.ProbeOrganizationInfoSchema.LoginPasswordProperty] as SecureString;
			}
			set
			{
				this[ProbeOrganizationInfo.ProbeOrganizationInfoSchema.LoginPasswordProperty] = value;
			}
		}

		public CustomerType CustomerType
		{
			get
			{
				return (CustomerType)this[ProbeOrganizationInfo.ProbeOrganizationInfoSchema.CustomerTypeProperty];
			}
			set
			{
				this[ProbeOrganizationInfo.ProbeOrganizationInfoSchema.CustomerTypeProperty] = value;
			}
		}

		public string OrganizationName
		{
			get
			{
				return this[ProbeOrganizationInfo.ProbeOrganizationInfoSchema.OrganizationNameProperty] as string;
			}
			set
			{
				this[ProbeOrganizationInfo.ProbeOrganizationInfoSchema.OrganizationNameProperty] = value;
			}
		}

		public string InitializationScript
		{
			get
			{
				return this[ProbeOrganizationInfo.ProbeOrganizationInfoSchema.InitializationScriptProperty] as string;
			}
			set
			{
				this[ProbeOrganizationInfo.ProbeOrganizationInfoSchema.InitializationScriptProperty] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ProbeOrganizationInfo.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ProbeOrganizationInfo.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public static ProbeEnvironment FindEnvironment(string tenantName)
		{
			if (Regex.IsMatch(tenantName, "@.*msol-test.com", RegexOptions.IgnoreCase))
			{
				return ProbeEnvironment.Test;
			}
			if (Regex.IsMatch(tenantName, "@.*ccsctp.net", RegexOptions.IgnoreCase))
			{
				return ProbeEnvironment.Dogfood;
			}
			if (Regex.IsMatch(tenantName, "@.*onmicrosoft.com", RegexOptions.IgnoreCase))
			{
				return ProbeEnvironment.Production;
			}
			throw new ArgumentException(string.Format("Tenant name {0} has an invalid suffix.", tenantName));
		}

		private const string TestEnvironmentSuffix = "@.*msol-test.com";

		private const string DogfoodEnvironmentSuffix = "@.*ccsctp.net";

		private const string ProductionEnvironmentSuffix = "@.*onmicrosoft.com";

		private static readonly string mostDerivedClass = "ProbeOrganizationInfo";

		private static readonly ProbeOrganizationInfo.ProbeOrganizationInfoSchema schema = ObjectSchema.GetInstance<ProbeOrganizationInfo.ProbeOrganizationInfoSchema>();

		internal class ProbeOrganizationInfoSchema : ADObjectSchema
		{
			internal static readonly HygienePropertyDefinition FeatureTagProperty = new HygienePropertyDefinition("FeatureTag", typeof(string));

			internal static readonly HygienePropertyDefinition RegionProperty = new HygienePropertyDefinition("Region", typeof(string));

			internal static readonly HygienePropertyDefinition LoginPasswordProperty = new HygienePropertyDefinition("LoginPassword", typeof(SecureString));

			internal static readonly HygienePropertyDefinition CustomerTypeProperty = new HygienePropertyDefinition("CustomerType", typeof(CustomerType));

			internal static readonly HygienePropertyDefinition OrganizationNameProperty = new HygienePropertyDefinition("OrganizationName", typeof(string));

			internal static readonly HygienePropertyDefinition InitializationScriptProperty = new HygienePropertyDefinition("InitializationScript", typeof(string));
		}
	}
}
