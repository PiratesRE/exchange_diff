using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class Encryption : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return Encryption.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return Encryption.mostDerivedClass;
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(EncryptionSchema.DefaultMessageFormat))
			{
				this[EncryptionSchema.DefaultMessageFormat] = true;
			}
			if (!base.IsModified(EncryptionSchema.EncryptAlgListNA))
			{
				this[EncryptionSchema.EncryptAlgListNA] = new MultiValuedProperty<string>(new string[]
				{
					Encryption.KeyManagementServiceCast64,
					Encryption.KeyManagementServiceDes
				});
			}
			if (!base.IsModified(EncryptionSchema.EncryptAlgListOther))
			{
				this[EncryptionSchema.EncryptAlgListOther] = new MultiValuedProperty<string>(new string[]
				{
					Encryption.KeyManagementServiceCast40
				});
			}
			if (!base.IsModified(EncryptionSchema.EncryptAlgSelectedNA))
			{
				this[EncryptionSchema.EncryptAlgSelectedNA] = Encryption.KeyManagementServiceDes;
			}
			if (!base.IsModified(EncryptionSchema.EncryptAlgSelectedOther))
			{
				this[EncryptionSchema.EncryptAlgSelectedOther] = Encryption.KeyManagementServiceCast40;
			}
			if (!base.IsModified(EncryptionSchema.SMimeAlgListNA))
			{
				this[EncryptionSchema.SMimeAlgListNA] = new MultiValuedProperty<string>(new string[]
				{
					Encryption.KeyManagementService3Des,
					Encryption.KeyManagementServiceDes,
					Encryption.KeyManagementService128BitsRC2,
					Encryption.KeyManagementService40BitsRC2,
					Encryption.KeyManagementService64BitsRC2
				});
			}
			if (!base.IsModified(EncryptionSchema.SMimeAlgListOther))
			{
				this[EncryptionSchema.SMimeAlgListOther] = new MultiValuedProperty<string>(new string[]
				{
					Encryption.KeyManagementService40BitsRC2
				});
			}
			if (!base.IsModified(EncryptionSchema.SMimeAlgSelectedNA))
			{
				this[EncryptionSchema.SMimeAlgSelectedNA] = Encryption.KeyManagementService3Des;
			}
			if (!base.IsModified(EncryptionSchema.SMimeAlgSelectedOther))
			{
				this[EncryptionSchema.SMimeAlgSelectedOther] = Encryption.KeyManagementService40BitsRC2;
			}
			if (!base.IsModified(ADConfigurationObjectSchema.SystemFlags))
			{
				this[ADConfigurationObjectSchema.SystemFlags] = SystemFlagsEnum.None;
			}
			base.StampPersistableDefaultValues();
		}

		public static readonly string DefaultName = "Encryption";

		public static readonly string KeyManagementServiceCast64 = "CAST-64";

		public static readonly string KeyManagementServiceCast40 = "CAST-40";

		public static readonly string KeyManagementServiceDes = "DES";

		public static readonly string KeyManagementService3Des = "3DES";

		public static readonly string KeyManagementService40BitsRC2 = "RC2-40";

		public static readonly string KeyManagementService64BitsRC2 = "RC2-64";

		public static readonly string KeyManagementService128BitsRC2 = "RC2-128";

		private static EncryptionSchema schema = ObjectSchema.GetInstance<EncryptionSchema>();

		private static string mostDerivedClass = "encryptionCfg";
	}
}
