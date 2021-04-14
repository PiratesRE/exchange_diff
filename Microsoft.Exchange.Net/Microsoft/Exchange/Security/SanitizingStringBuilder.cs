using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Security
{
	public sealed class SanitizingStringBuilder<SanitizingPolicy> where SanitizingPolicy : ISanitizingPolicy, new()
	{
		public SanitizingStringBuilder()
		{
			this.builder = new StringBuilder();
		}

		public SanitizingStringBuilder(int capacity)
		{
			this.builder = new StringBuilder(capacity);
		}

		public SanitizingStringBuilder(string content) : this((content == null) ? 0 : content.Length)
		{
			if (content == null)
			{
				content = string.Empty;
			}
			this.Append(content);
		}

		public int Capacity
		{
			get
			{
				return this.builder.Capacity;
			}
			set
			{
				this.builder.Capacity = value;
			}
		}

		public int MaxCapacity
		{
			get
			{
				return this.builder.MaxCapacity;
			}
		}

		public int Length
		{
			get
			{
				return this.builder.Length;
			}
		}

		public StringBuilder UnsafeInnerStringBuilder
		{
			get
			{
				return this.builder;
			}
		}

		public char this[int index]
		{
			get
			{
				return this.builder[index];
			}
		}

		public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
		{
			this.builder.CopyTo(sourceIndex, destination, destinationIndex, count);
		}

		public int EnsureCapacity(int capacity)
		{
			return this.builder.EnsureCapacity(capacity);
		}

		public void Clear()
		{
			this.builder.Length = 0;
		}

		public void Append(string str)
		{
			if (str == null)
			{
				str = string.Empty;
			}
			if (StringSanitizer<SanitizingPolicy>.IsTrustedString(str))
			{
				this.builder.Append(str);
				return;
			}
			this.builder.Append(StringSanitizer<SanitizingPolicy>.Sanitize(str));
		}

		public void Append<T>(T obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (obj is ISanitizedString<SanitizingPolicy>)
			{
				this.builder.Append(obj.ToString());
				return;
			}
			this.Append(obj.ToString());
		}

		public void AppendLine(string str)
		{
			if (str == null)
			{
				str = string.Empty;
			}
			this.Append(str);
			this.builder.AppendLine();
		}

		public void AppendLine<T>(T obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (obj is ISanitizedString<SanitizingPolicy>)
			{
				this.builder.AppendLine(obj.ToString());
				return;
			}
			this.AppendLine(obj.ToString());
		}

		public void AppendLine()
		{
			this.builder.AppendLine();
		}

		public void AppendFormat(string format, params object[] args)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			this.builder.Append(StringSanitizer<SanitizingPolicy>.SanitizeFormat(CultureInfo.InvariantCulture, format, args));
		}

		public void AppendFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			this.builder.Append(StringSanitizer<SanitizingPolicy>.SanitizeFormat(formatProvider, format, args));
		}

		public override string ToString()
		{
			return this.builder.ToString();
		}

		public T ToSanitizedString<T>() where T : ISanitizedString<SanitizingPolicy>, new()
		{
			T result = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			result.UntrustedValue = this.builder.ToString();
			result.DecreeToBeTrusted();
			return result;
		}

		private StringBuilder builder;
	}
}
