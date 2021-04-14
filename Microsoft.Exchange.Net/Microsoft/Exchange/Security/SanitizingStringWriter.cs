using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Security
{
	internal class SanitizingStringWriter<SanitizingPolicy> : StringWriter where SanitizingPolicy : ISanitizingPolicy, new()
	{
		public SanitizingStringWriter()
		{
		}

		public SanitizingStringWriter(IFormatProvider formatProvider) : base(formatProvider)
		{
		}

		public SanitizingStringWriter(StringBuilder builder) : base(builder)
		{
		}

		public SanitizingStringWriter(StringBuilder builder, IFormatProvider formatProvider) : base(builder, formatProvider)
		{
		}

		public T ToSanitizedString<T>() where T : ISanitizedString<SanitizingPolicy>, new()
		{
			T result = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			result.UntrustedValue = base.ToString();
			result.DecreeToBeTrusted();
			return result;
		}

		public override void Write(string value)
		{
			if (value == null)
			{
				value = string.Empty;
			}
			if (StringSanitizer<SanitizingPolicy>.IsTrustedString(value))
			{
				base.Write(value);
				return;
			}
			base.Write(StringSanitizer<SanitizingPolicy>.Sanitize(value));
		}

		public override void Write(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			ISanitizedString<SanitizingPolicy> sanitizedString = value as ISanitizedString<SanitizingPolicy>;
			if (sanitizedString != null)
			{
				base.Write(sanitizedString.ToString());
				return;
			}
			this.Write(value.ToString());
		}

		public override void Write(string format, params object[] args)
		{
			this.WriteCommon(format, args);
		}

		public override void Write(string format, object arg0)
		{
			this.WriteCommon(format, new object[]
			{
				arg0
			});
		}

		public override void Write(string format, object arg0, object arg1)
		{
			this.WriteCommon(format, new object[]
			{
				arg0,
				arg1
			});
		}

		public override void Write(string format, object arg0, object arg1, object arg2)
		{
			this.WriteCommon(format, new object[]
			{
				arg0,
				arg1,
				arg2
			});
		}

		public override void WriteLine(string value)
		{
			this.Write(value);
			base.WriteLine();
		}

		public override void WriteLine(object value)
		{
			this.Write(value);
			base.WriteLine();
		}

		public override void WriteLine(string format, params object[] args)
		{
			this.WriteLineCommon(format, args);
		}

		public override void WriteLine(string format, object arg0)
		{
			this.WriteLineCommon(format, new object[]
			{
				arg0
			});
		}

		public override void WriteLine(string format, object arg0, object arg1)
		{
			this.WriteLineCommon(format, new object[]
			{
				arg0,
				arg1
			});
		}

		public override void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			this.WriteLineCommon(format, new object[]
			{
				arg0,
				arg1,
				arg2
			});
		}

		private void WriteCommon(string format, params object[] args)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			base.Write(StringSanitizer<SanitizingPolicy>.SanitizeFormat(base.FormatProvider, format, args));
		}

		private void WriteLineCommon(string format, params object[] args)
		{
			this.WriteCommon(format, args);
			base.WriteLine();
		}
	}
}
