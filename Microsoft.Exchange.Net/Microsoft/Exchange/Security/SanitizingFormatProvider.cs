using System;

namespace Microsoft.Exchange.Security
{
	internal class SanitizingFormatProvider<SanitizingPolicy> : IFormatProvider, ICustomFormatter where SanitizingPolicy : ISanitizingPolicy, new()
	{
		public SanitizingFormatProvider(IFormatProvider innerFormatProvider)
		{
			this.innerFormatProvider = innerFormatProvider;
		}

		public IFormatProvider InnerFormatProvider
		{
			get
			{
				return this.innerFormatProvider;
			}
		}

		public object GetFormat(Type formatType)
		{
			if (formatType == typeof(ICustomFormatter))
			{
				return this;
			}
			return null;
		}

		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			string text = arg as string;
			if (arg == null)
			{
				return string.Empty;
			}
			if (text != null && StringSanitizer<SanitizingPolicy>.IsTrustedString(text))
			{
				return text;
			}
			ISanitizedString<SanitizingPolicy> sanitizedString = arg as ISanitizedString<SanitizingPolicy>;
			if (sanitizedString != null)
			{
				return sanitizedString.ToString();
			}
			if (!(arg is string))
			{
				IFormattable formattable = arg as IFormattable;
				if (formattable == null)
				{
					text = arg.ToString();
				}
				else
				{
					text = formattable.ToString(format, this.InnerFormatProvider);
				}
			}
			return StringSanitizer<SanitizingPolicy>.Sanitize(text);
		}

		private readonly IFormatProvider innerFormatProvider;
	}
}
