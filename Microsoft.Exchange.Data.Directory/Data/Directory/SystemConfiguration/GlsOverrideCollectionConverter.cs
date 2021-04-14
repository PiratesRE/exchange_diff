using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public class GlsOverrideCollectionConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null || value is string)
			{
				return new GlsOverrideCollection((string)value);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(GlsOverrideCollection))
			{
				return this.ConvertFrom(context, culture, value);
			}
			GlsOverrideCollection glsOverrideCollection = value as GlsOverrideCollection;
			if (glsOverrideCollection == null)
			{
				throw new ArgumentException("value");
			}
			if (destinationType == typeof(string))
			{
				return glsOverrideCollection.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
