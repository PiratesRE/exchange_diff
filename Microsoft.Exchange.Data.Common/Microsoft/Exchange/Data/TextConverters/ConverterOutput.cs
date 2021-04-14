using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal abstract class ConverterOutput : ITextSink, IDisposable
	{
		public ConverterOutput()
		{
			this.stringBuffer = new char[128];
		}

		public abstract bool CanAcceptMore { get; }

		bool ITextSink.IsEnough
		{
			get
			{
				return false;
			}
		}

		internal IReportBytes ReportBytes
		{
			get
			{
				return this.reportBytes;
			}
			set
			{
				this.reportBytes = value;
			}
		}

		public abstract void Write(char[] buffer, int offset, int count, IFallback fallback);

		public abstract void Flush();

		public void Write(char[] buffer, int offset, int count)
		{
			this.Write(buffer, offset, count, null);
		}

		public virtual void Write(string text)
		{
			this.Write(text, 0, text.Length, null);
		}

		public void Write(string text, IFallback fallback)
		{
			this.Write(text, 0, text.Length, fallback);
		}

		public void Write(string text, int offset, int count)
		{
			this.Write(text, offset, count, null);
		}

		public void Write(string text, int offset, int count, IFallback fallback)
		{
			if (this.stringBuffer.Length < count)
			{
				this.stringBuffer = new char[count * 2];
			}
			text.CopyTo(offset, this.stringBuffer, 0, count);
			this.Write(this.stringBuffer, 0, count, fallback);
		}

		public void Write(char ch)
		{
			this.Write(ch, null);
		}

		public void Write(char ch, IFallback fallback)
		{
			this.stringBuffer[0] = ch;
			this.Write(this.stringBuffer, 0, 1, fallback);
		}

		public void Write(int ucs32Literal)
		{
			this.Write(ucs32Literal, null);
		}

		public void Write(int ucs32Literal, IFallback fallback)
		{
			int count = 1;
			if (ucs32Literal > 65535)
			{
				if (fallback != null && fallback is HtmlWriter)
				{
					uint num = (uint)ucs32Literal;
					int num2 = (num < 10U) ? 1 : ((num < 100U) ? 2 : ((num < 1000U) ? 3 : ((num < 10000U) ? 4 : ((num < 100000U) ? 5 : ((num < 1000000U) ? 6 : 7)))));
					int num3 = 2 + num2;
					this.stringBuffer[0] = '&';
					this.stringBuffer[1] = '#';
					this.stringBuffer[num3] = ';';
					while (num != 0U)
					{
						uint num4 = num % 10U;
						this.stringBuffer[--num3] = (char)(num4 + 48U);
						num /= 10U;
					}
					count = 3 + num2;
					this.Write(this.stringBuffer, 0, count, null);
					return;
				}
				this.stringBuffer[0] = ParseSupport.HighSurrogateCharFromUcs4(ucs32Literal);
				this.stringBuffer[1] = ParseSupport.LowSurrogateCharFromUcs4(ucs32Literal);
				count = 2;
			}
			else
			{
				this.stringBuffer[0] = (char)ucs32Literal;
			}
			this.Write(this.stringBuffer, 0, count, fallback);
		}

		public ITextSink PrepareSink(IFallback fallback)
		{
			this.fallback = fallback;
			return this;
		}

		void ITextSink.Write(char[] buffer, int offset, int count)
		{
			this.Write(buffer, offset, count, this.fallback);
		}

		void ITextSink.Write(int ucs32Literal)
		{
			this.Write(ucs32Literal, this.fallback);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		private const int StringBufferMax = 128;

		protected char[] stringBuffer;

		protected IReportBytes reportBytes;

		private IFallback fallback;
	}
}
