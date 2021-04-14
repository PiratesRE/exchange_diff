using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class AddressTemplate : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return AddressTemplate.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AddressTemplate.mostDerivedClass;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[AddressTemplateSchema.DisplayName];
			}
		}

		public byte[] AddressSyntax
		{
			get
			{
				return (byte[])this[AddressTemplateSchema.AddressSyntax];
			}
		}

		public string AddressType
		{
			get
			{
				return (string)this[AddressTemplateSchema.AddressType];
			}
		}

		public byte[] PerMsgDialogDisplayTable
		{
			get
			{
				return (byte[])this[AddressTemplateSchema.PerMsgDialogDisplayTable];
			}
		}

		public byte[] PerRecipDialogDisplayTable
		{
			get
			{
				return (byte[])this[AddressTemplateSchema.PerRecipDialogDisplayTable];
			}
		}

		public bool ProxyGenerationEnabled
		{
			get
			{
				return (bool)this[AddressTemplateSchema.ProxyGenerationEnabled];
			}
		}

		public byte[] TemplateBlob
		{
			get
			{
				return (byte[])this[AddressTemplateSchema.TemplateBlob];
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this[AddressTemplateSchema.ExchangeLegacyDN];
			}
		}

		internal static readonly ADObjectId ContainerId = new ADObjectId("CN=Address-Templates,CN=Addressing");

		private static AddressTemplateSchema schema = ObjectSchema.GetInstance<AddressTemplateSchema>();

		private static string mostDerivedClass = "addressTemplate";
	}
}
