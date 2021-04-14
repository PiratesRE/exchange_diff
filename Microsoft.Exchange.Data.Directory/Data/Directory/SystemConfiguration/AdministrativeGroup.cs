using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class AdministrativeGroup : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return AdministrativeGroup.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AdministrativeGroup.mostDerivedClass;
			}
		}

		public ADObjectId PublicFolderDatabase
		{
			get
			{
				return (ADObjectId)this[AdministrativeGroupSchema.PublicFolderDatabase];
			}
			internal set
			{
				this[AdministrativeGroupSchema.PublicFolderDatabase] = value;
			}
		}

		public string LegacyExchangeDN
		{
			get
			{
				return (string)this[AdministrativeGroupSchema.LegacyExchangeDN];
			}
		}

		public bool DefaultAdminGroup
		{
			get
			{
				return (bool)this[AdministrativeGroupSchema.DefaultAdminGroup];
			}
			internal set
			{
				this[AdministrativeGroupSchema.DefaultAdminGroup] = value;
			}
		}

		internal void StampLegacyExchangeDN(string orgLegDN, string agName)
		{
			this[AdministrativeGroupSchema.LegacyExchangeDN] = string.Format("{0}/ou={1}", orgLegDN, LegacyDN.LegitimizeCN(agName));
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(AdministrativeGroupSchema.DisplayName))
			{
				this[AdministrativeGroupSchema.DisplayName] = AdministrativeGroup.DefaultName;
			}
			if (!base.IsModified(ADConfigurationObjectSchema.SystemFlags))
			{
				this[ADConfigurationObjectSchema.SystemFlags] = SystemFlagsEnum.None;
			}
			base.StampPersistableDefaultValues();
		}

		private static AdministrativeGroupSchema schema = ObjectSchema.GetInstance<AdministrativeGroupSchema>();

		private static string mostDerivedClass = "msExchAdminGroup";

		public static readonly string DefaultName = "Exchange Administrative Group (FYDIBOHF23SPDLT)";
	}
}
