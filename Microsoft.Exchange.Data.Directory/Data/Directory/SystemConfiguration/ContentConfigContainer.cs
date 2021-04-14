using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ContentConfigContainer : ADConfigurationObject
	{
		public MultiValuedProperty<string> MimeTypes
		{
			get
			{
				return (MultiValuedProperty<string>)base[ContentConfigContainerSchema.MimeTypes];
			}
			internal set
			{
				base[ContentConfigContainerSchema.MimeTypes] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ContentConfigContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchContentConfigContainer";
			}
		}

		internal override bool IsShareable
		{
			get
			{
				return true;
			}
		}

		public const string DefaultName = "Internet Message Formats";

		private const string MostDerivedClass = "msExchContentConfigContainer";

		private static readonly ADObjectSchema schema = ObjectSchema.GetInstance<ContentConfigContainerSchema>();
	}
}
