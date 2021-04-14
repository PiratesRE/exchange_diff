using System;
using System.Text;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
	internal class TextCodePageConverter : IProducerConsumer, IDisposable
	{
		public TextCodePageConverter(ConverterInput input, ConverterOutput output)
		{
			this.input = input;
			this.output = output;
		}

		public void Run()
		{
			if (this.endOfFile)
			{
				return;
			}
			char[] buffer = null;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (!this.input.ReadMore(ref buffer, ref num, ref num2, ref num3))
			{
				return;
			}
			if (this.input.EndOfFile)
			{
				this.endOfFile = true;
			}
			if (num3 - num != 0)
			{
				if (!this.gotAnyText)
				{
					if (this.output is ConverterEncodingOutput)
					{
						ConverterEncodingOutput converterEncodingOutput = this.output as ConverterEncodingOutput;
						if (converterEncodingOutput.CodePageSameAsInput)
						{
							if (this.input is ConverterDecodingInput)
							{
								converterEncodingOutput.Encoding = (this.input as ConverterDecodingInput).Encoding;
							}
							else
							{
								converterEncodingOutput.Encoding = Encoding.UTF8;
							}
						}
					}
					this.gotAnyText = true;
				}
				this.output.Write(buffer, num, num3 - num);
				this.input.ReportProcessed(num3 - num);
			}
			if (this.endOfFile)
			{
				this.output.Flush();
			}
		}

		public bool Flush()
		{
			if (!this.endOfFile)
			{
				this.Run();
			}
			return this.endOfFile;
		}

		void IDisposable.Dispose()
		{
			if (this.input != null)
			{
				((IDisposable)this.input).Dispose();
			}
			if (this.output != null)
			{
				((IDisposable)this.output).Dispose();
			}
			this.input = null;
			this.output = null;
			GC.SuppressFinalize(this);
		}

		protected ConverterInput input;

		protected bool endOfFile;

		protected bool gotAnyText;

		protected ConverterOutput output;
	}
}
