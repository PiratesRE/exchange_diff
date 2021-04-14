using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class ExchangeVirtualDirectory : ADVirtualDirectory
	{
		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ExchangeVirtualDirectory.schema;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal bool ADPropertiesOnly { get; set; }

		public string MetabasePath
		{
			get
			{
				return (string)this[ExchangeVirtualDirectorySchema.MetabasePath];
			}
			internal set
			{
				this[ExchangeVirtualDirectorySchema.MetabasePath] = value;
			}
		}

		public string Path
		{
			get
			{
				return this.path;
			}
			internal set
			{
				this.path = (value ?? string.Empty);
			}
		}

		public ExtendedProtectionTokenCheckingMode ExtendedProtectionTokenChecking
		{
			get
			{
				return (ExtendedProtectionTokenCheckingMode)this[ExchangeVirtualDirectorySchema.ExtendedProtectionTokenChecking];
			}
			internal set
			{
				this[ExchangeVirtualDirectorySchema.ExtendedProtectionTokenChecking] = value;
			}
		}

		public MultiValuedProperty<ExtendedProtectionFlag> ExtendedProtectionFlags
		{
			get
			{
				return ExchangeVirtualDirectory.ExtendedProtectionFlagsToMultiValuedProperty((ExtendedProtectionFlag)this[ExchangeVirtualDirectorySchema.ExtendedProtectionFlags]);
			}
			internal set
			{
				this[ExchangeVirtualDirectorySchema.ExtendedProtectionFlags] = ExchangeVirtualDirectory.ExtendedProtectionMultiValuedPropertyToFlags(value);
			}
		}

		public MultiValuedProperty<string> ExtendedProtectionSPNList
		{
			get
			{
				return (MultiValuedProperty<string>)this[ExchangeVirtualDirectorySchema.ExtendedProtectionSPNList];
			}
			internal set
			{
				this[ExchangeVirtualDirectorySchema.ExtendedProtectionSPNList] = value;
			}
		}

		internal static MultiValuedProperty<string> RemoveDNStringSyntax(MultiValuedProperty<string> objectDNString, ProviderPropertyDefinition propertyDefinition)
		{
			if (objectDNString == null)
			{
				return objectDNString;
			}
			string[] array = objectDNString.ToArray();
			char[] separator = new char[]
			{
				':'
			};
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				string[] array2 = text.Split(separator);
				int length = int.Parse(array2[1]);
				int startIndex = text.IndexOf(':', 2) + 1;
				array[i] = text.Substring(startIndex, length);
			}
			return new MultiValuedProperty<string>(false, propertyDefinition, array);
		}

		internal static MultiValuedProperty<string> AddDNStringSyntax(MultiValuedProperty<string> objectDNString, ProviderPropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (objectDNString == null)
			{
				return objectDNString;
			}
			string[] array = objectDNString.ToArray();
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			string distinguishedName = adobjectId.DistinguishedName;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = string.Concat(new string[]
				{
					"S:",
					array[i].Length.ToString(),
					":",
					array[i],
					":",
					distinguishedName
				});
			}
			return new MultiValuedProperty<string>(false, propertyDefinition, array);
		}

		internal static MultiValuedProperty<ExtendedProtectionFlag> ExtendedProtectionFlagsToMultiValuedProperty(ExtendedProtectionFlag flags)
		{
			if (flags == ExtendedProtectionFlag.None)
			{
				return ExchangeVirtualDirectory.EmptyExtendedProtectionFlagsPropertyValue;
			}
			MultiValuedProperty<ExtendedProtectionFlag> multiValuedProperty = new MultiValuedProperty<ExtendedProtectionFlag>();
			foreach (ExtendedProtectionFlag extendedProtectionFlag in ExchangeVirtualDirectory.extendedProtectionFlagMasks)
			{
				if ((flags & extendedProtectionFlag) == extendedProtectionFlag)
				{
					multiValuedProperty.Add(extendedProtectionFlag);
				}
			}
			return multiValuedProperty;
		}

		internal static ExtendedProtectionFlag ExtendedProtectionMultiValuedPropertyToFlags(MultiValuedProperty<ExtendedProtectionFlag> flagsCollection)
		{
			ExtendedProtectionFlag extendedProtectionFlag = ExtendedProtectionFlag.None;
			if (flagsCollection != null)
			{
				foreach (ExtendedProtectionFlag extendedProtectionFlag2 in flagsCollection)
				{
					extendedProtectionFlag |= extendedProtectionFlag2;
				}
			}
			return extendedProtectionFlag;
		}

		private static readonly ExchangeVirtualDirectorySchema schema = ObjectSchema.GetInstance<ExchangeVirtualDirectorySchema>();

		private static readonly MultiValuedProperty<ExtendedProtectionFlag> EmptyExtendedProtectionFlagsPropertyValue = new MultiValuedProperty<ExtendedProtectionFlag>();

		private static ExtendedProtectionFlag[] extendedProtectionFlagMasks = new ExtendedProtectionFlag[]
		{
			ExtendedProtectionFlag.Proxy,
			ExtendedProtectionFlag.NoServiceNameCheck,
			ExtendedProtectionFlag.ProxyCohosting,
			ExtendedProtectionFlag.AllowDotlessSpn
		};

		private string path = string.Empty;
	}
}
