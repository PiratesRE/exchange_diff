using System;
using System.Linq;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class AuthConfigSchema : ADConfigurationObjectSchema
	{
		internal static object MultiValuedStringKeyGetter(IPropertyBag propertyBag, ADPropertyDefinition rawProperty, int offset)
		{
			if (offset < 0 || offset > 2)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			MultiValuedProperty<string> multiValuedProperty = propertyBag[rawProperty] as MultiValuedProperty<string>;
			if (multiValuedProperty == null || multiValuedProperty.Count == 0)
			{
				return string.Empty;
			}
			string tag = offset.ToString("d1") + AuthConfigSchema.NameValueSeparator;
			string text = string.Empty;
			string text2 = multiValuedProperty.SingleOrDefault((string s) => s.StartsWith(tag));
			if (text2 != null && text2.Length > tag.Length)
			{
				text = text2.Substring(tag.Length);
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			if (multiValuedProperty.Count == 1 && offset == 0)
			{
				text = multiValuedProperty[0];
			}
			return text;
		}

		internal static void MultiValuedStringKeySetter(object value, IPropertyBag propertyBag, ADPropertyDefinition rawProperty, int offset)
		{
			if (offset < 0 || offset > 2)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			MultiValuedProperty<string> multiValuedProperty = propertyBag[rawProperty] as MultiValuedProperty<string>;
			string text = value as string;
			if (multiValuedProperty.Count == 1 && !multiValuedProperty[0].StartsWith("0" + AuthConfigSchema.NameValueSeparator, StringComparison.OrdinalIgnoreCase))
			{
				multiValuedProperty[0] = "0" + AuthConfigSchema.NameValueSeparator + multiValuedProperty[0];
			}
			string text2 = offset.ToString("d1") + AuthConfigSchema.NameValueSeparator;
			int num = 0;
			while (num < multiValuedProperty.Count && (string.IsNullOrEmpty(multiValuedProperty[num]) || !multiValuedProperty[num].StartsWith(text2)))
			{
				num++;
			}
			if (num >= multiValuedProperty.Count)
			{
				if (!string.IsNullOrEmpty(text))
				{
					multiValuedProperty.Add(text2 + text);
					return;
				}
			}
			else
			{
				int i = num + 1;
				while (i < multiValuedProperty.Count)
				{
					if (string.IsNullOrEmpty(multiValuedProperty[i]) || multiValuedProperty[i].StartsWith(text2))
					{
						multiValuedProperty.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					multiValuedProperty.RemoveAt(num);
					return;
				}
				multiValuedProperty[num] = text2 + text;
			}
		}

		internal static object MultiValuedBytesKeyGetter(IPropertyBag propertyBag, ADPropertyDefinition rawProperty, int offset)
		{
			MultiValuedProperty<byte[]> multiValuedProperty = propertyBag[rawProperty] as MultiValuedProperty<byte[]>;
			if (multiValuedProperty.Count < offset + 1)
			{
				return string.Empty;
			}
			byte[] array = multiValuedProperty[offset];
			if (array.Length == 1 && array[0] == 0)
			{
				return string.Empty;
			}
			return Convert.ToBase64String(array);
		}

		internal static void MultiValuedBytesKeySetter(object value, IPropertyBag propertyBag, ADPropertyDefinition rawProperty, int offset)
		{
			MultiValuedProperty<byte[]> multiValuedProperty = propertyBag[rawProperty] as MultiValuedProperty<byte[]>;
			string text = value as string;
			if (string.IsNullOrEmpty(text))
			{
				if (offset > multiValuedProperty.Count - 1)
				{
					return;
				}
				if (offset == multiValuedProperty.Count - 1)
				{
					multiValuedProperty.RemoveAt(offset);
					return;
				}
			}
			while (multiValuedProperty.Count < offset + 1)
			{
				MultiValuedProperty<byte[]> multiValuedProperty2 = multiValuedProperty;
				byte[] item = new byte[1];
				multiValuedProperty2.Add(item);
			}
			if (!string.IsNullOrEmpty(text))
			{
				multiValuedProperty[offset] = Convert.FromBase64String(text);
				return;
			}
			MultiValuedProperty<byte[]> multiValuedProperty3 = multiValuedProperty;
			byte[] value2 = new byte[1];
			multiValuedProperty3[offset] = value2;
		}

		private static readonly string NameValueSeparator = ":";

		public static readonly ADPropertyDefinition ServiceName = new ADPropertyDefinition("ServiceName", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAuthApplicationIdentifier", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new ContainingNonWhitespaceConstraint(),
			new MandatoryStringLengthConstraint(1, 64)
		}, null, null);

		public static readonly ADPropertyDefinition Realm = new ADPropertyDefinition("Realm", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAuthRealm", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new ContainingNonWhitespaceConstraint(),
			new StringLengthConstraint(0, 512)
		}, null, null);

		public static readonly ADPropertyDefinition NextCertificateEffectiveDate = new ADPropertyDefinition("NextCertificateEffectiveDate", ExchangeObjectVersion.Exchange2012, typeof(DateTime?), "msExchAuthNextEffectiveDate", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CertificateThumbprintRaw = new ADPropertyDefinition("CertificateThumbprintRaw", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAuthCertificateThumbprint", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CurrentCertificateThumbprint = new ADPropertyDefinition("CurrentCertificateThumbprint", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AuthConfigSchema.CertificateThumbprintRaw
		}, null, (IPropertyBag b) => AuthConfigSchema.MultiValuedStringKeyGetter(b, AuthConfigSchema.CertificateThumbprintRaw, 0), delegate(object v, IPropertyBag b)
		{
			AuthConfigSchema.MultiValuedStringKeySetter(v, b, AuthConfigSchema.CertificateThumbprintRaw, 0);
		}, null, null);

		public static readonly ADPropertyDefinition PreviousCertificateThumbprint = new ADPropertyDefinition("PreviousCertificateThumbprint", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AuthConfigSchema.CertificateThumbprintRaw
		}, null, (IPropertyBag b) => AuthConfigSchema.MultiValuedStringKeyGetter(b, AuthConfigSchema.CertificateThumbprintRaw, 1), delegate(object v, IPropertyBag b)
		{
			AuthConfigSchema.MultiValuedStringKeySetter(v, b, AuthConfigSchema.CertificateThumbprintRaw, 1);
		}, null, null);

		public static readonly ADPropertyDefinition NextCertificateThumbprint = new ADPropertyDefinition("NextCertificateThumbprint", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AuthConfigSchema.CertificateThumbprintRaw
		}, null, (IPropertyBag b) => AuthConfigSchema.MultiValuedStringKeyGetter(b, AuthConfigSchema.CertificateThumbprintRaw, 2), delegate(object v, IPropertyBag b)
		{
			AuthConfigSchema.MultiValuedStringKeySetter(v, b, AuthConfigSchema.CertificateThumbprintRaw, 2);
		}, null, null);
	}
}
