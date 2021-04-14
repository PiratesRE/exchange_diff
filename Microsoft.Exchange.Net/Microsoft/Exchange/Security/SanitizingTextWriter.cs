using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Security
{
	internal sealed class SanitizingTextWriter<SanitizingPolicy> : TextWriter where SanitizingPolicy : ISanitizingPolicy, new()
	{
		public SanitizingTextWriter(TextWriter writer, bool takeOwnership)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
			this.ownWriter = takeOwnership;
		}

		public SanitizingTextWriter(TextWriter writer) : this(writer, false)
		{
		}

		public override IFormatProvider FormatProvider
		{
			get
			{
				return this.writer.FormatProvider;
			}
		}

		public override Encoding Encoding
		{
			get
			{
				return this.writer.Encoding;
			}
		}

		public override void Flush()
		{
			this.writer.Flush();
		}

		public override void Write(char value)
		{
			this.writer.Write(value);
		}

		public override void Write(string value)
		{
			if (value == null)
			{
				value = string.Empty;
			}
			if (StringSanitizer<SanitizingPolicy>.IsTrustedString(value))
			{
				this.writer.Write(value);
				return;
			}
			StringSanitizer<SanitizingPolicy>.Sanitize(this.writer, value);
		}

		public override void Write(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value is SanitizingStringBuilder<SanitizingPolicy>)
			{
				this.writer.Write(value.ToString());
				return;
			}
			ISanitizedString<SanitizingPolicy> sanitizedString = value as ISanitizedString<SanitizingPolicy>;
			if (sanitizedString != null)
			{
				this.writer.Write(sanitizedString.ToString());
				return;
			}
			this.Write(value.ToString());
		}

		public override void WriteLine(string value)
		{
			this.Write(value);
			this.writer.WriteLine();
		}

		public override void WriteLine(object value)
		{
			this.Write(value);
			this.writer.WriteLine();
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

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && this.ownWriter)
			{
				this.writer.Dispose();
			}
		}

		private void WriteCommon(string format, params object[] args)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			this.writer.Write(StringSanitizer<SanitizingPolicy>.SanitizeFormat(this.writer.FormatProvider, format, args));
		}

		private void WriteLineCommon(string format, params object[] args)
		{
			this.WriteCommon(format, args);
			this.writer.WriteLine();
		}

		private TextWriter writer;

		private bool ownWriter;
	}
}
