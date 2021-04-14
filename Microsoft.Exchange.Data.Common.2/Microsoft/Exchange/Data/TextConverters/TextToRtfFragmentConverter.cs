using System;
using System.IO;
using Microsoft.Exchange.Data.TextConverters.Internal.Rtf;
using Microsoft.Exchange.Data.TextConverters.Internal.Text;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class TextToRtfFragmentConverter : IProducerConsumer, IDisposable
	{
		public TextToRtfFragmentConverter(TextParser parser, RtfOutput output, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
		{
			this.output = output;
			this.parser = parser;
		}

		public void Initialize(string fragment)
		{
			this.parser.Initialize(fragment);
			this.endOfFile = false;
		}

		public void Run()
		{
			if (this.endOfFile)
			{
				return;
			}
			TextTokenId textTokenId = this.parser.Parse();
			if (textTokenId == TextTokenId.None)
			{
				return;
			}
			this.Process(textTokenId);
		}

		public bool Flush()
		{
			if (!this.endOfFile)
			{
				this.Run();
			}
			return this.endOfFile;
		}

		protected void Process(TextTokenId tokenId)
		{
			switch (tokenId)
			{
			case TextTokenId.EndOfFile:
				this.endOfFile = true;
				break;
			case TextTokenId.Text:
				foreach (TokenRun tokenRun in this.parser.Token.Runs)
				{
					if (tokenRun.IsTextRun)
					{
						RunTextType textType = tokenRun.TextType;
						if (textType <= RunTextType.Tabulation)
						{
							if (textType != RunTextType.Space)
							{
								if (textType == RunTextType.NewLine)
								{
									this.output.WriteControlText("\\line\r\n", false);
									continue;
								}
								if (textType != RunTextType.Tabulation)
								{
									continue;
								}
								for (int i = 0; i < tokenRun.Length; i++)
								{
									this.output.WriteControlText("\\tab", true);
								}
								continue;
							}
						}
						else if (textType != RunTextType.UnusualWhitespace)
						{
							if (textType == RunTextType.Nbsp)
							{
								for (int j = 0; j < tokenRun.Length; j++)
								{
									this.output.WriteText("\u00a0");
								}
								continue;
							}
							if (textType != RunTextType.NonSpace)
							{
								continue;
							}
							this.output.WriteText(tokenRun.RawBuffer, tokenRun.RawOffset, tokenRun.RawLength);
							continue;
						}
						for (int k = 0; k < tokenRun.Length; k++)
						{
							this.output.WriteText(" ");
						}
					}
					else if (tokenRun.IsSpecial)
					{
						uint kind = tokenRun.Kind;
					}
				}
				return;
			case TextTokenId.EncodingChange:
				break;
			default:
				return;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.parser != null)
			{
				((IDisposable)this.parser).Dispose();
			}
			this.parser = null;
			this.output = null;
		}

		protected TextParser parser;

		protected bool endOfFile;

		protected RtfOutput output;
	}
}
