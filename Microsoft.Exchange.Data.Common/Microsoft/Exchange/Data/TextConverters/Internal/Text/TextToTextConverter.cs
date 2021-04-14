using System;
using System.IO;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
	internal class TextToTextConverter : IProducerConsumer, IDisposable
	{
		public TextToTextConverter(TextParser parser, TextOutput output, Injection injection, bool convertFragment, bool treatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
		{
			this.treatNbspAsBreakable = treatNbspAsBreakable;
			this.convertFragment = convertFragment;
			this.output = output;
			this.parser = parser;
			this.injection = injection;
		}

		public void Initialize(string fragment)
		{
			this.parser.Initialize(fragment);
			this.endOfFile = false;
			this.lineLength = 0;
			this.newLines = 0;
			this.spaces = 0;
			this.nbsps = 0;
			this.paragraphStarted = false;
			this.started = false;
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
			if (!this.started)
			{
				if (!this.convertFragment)
				{
					this.output.OpenDocument();
					if (this.injection != null && this.injection.HaveHead)
					{
						this.injection.Inject(true, this.output);
					}
				}
				this.output.SetQuotingLevel(0);
				this.started = true;
			}
			switch (tokenId)
			{
			case TextTokenId.EndOfFile:
				if (!this.convertFragment)
				{
					if (this.injection != null && this.injection.HaveTail)
					{
						if (!this.output.LineEmpty)
						{
							this.output.OutputNewLine();
						}
						this.injection.Inject(false, this.output);
					}
					this.output.Flush();
				}
				else
				{
					this.output.CloseParagraph();
				}
				this.endOfFile = true;
				break;
			case TextTokenId.Text:
				this.OutputFragmentSimple(this.parser.Token);
				return;
			case TextTokenId.EncodingChange:
				if (!this.convertFragment && this.output.OutputCodePageSameAsInput)
				{
					this.output.OutputEncoding = this.parser.Token.TokenEncoding;
					return;
				}
				break;
			default:
				return;
			}
		}

		private void OutputFragmentSimple(TextToken token)
		{
			foreach (TokenRun tokenRun in token.Runs)
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
								this.output.OutputNewLine();
								continue;
							}
							if (textType != RunTextType.Tabulation)
							{
								continue;
							}
							this.output.OutputTabulation(tokenRun.Length);
							continue;
						}
					}
					else if (textType != RunTextType.UnusualWhitespace)
					{
						if (textType != RunTextType.Nbsp)
						{
							if (textType != RunTextType.NonSpace)
							{
								continue;
							}
							this.output.OutputNonspace(tokenRun.RawBuffer, tokenRun.RawOffset, tokenRun.RawLength, TextMapping.Unicode);
							continue;
						}
						else
						{
							if (this.treatNbspAsBreakable)
							{
								this.output.OutputSpace(tokenRun.Length);
								continue;
							}
							this.output.OutputNbsp(tokenRun.Length);
							continue;
						}
					}
					this.output.OutputSpace(tokenRun.Length);
				}
				else if (tokenRun.IsSpecial && tokenRun.Kind == 167772160U)
				{
					this.output.SetQuotingLevel((int)((ushort)tokenRun.Value));
				}
			}
		}

		void IDisposable.Dispose()
		{
			if (this.parser != null)
			{
				((IDisposable)this.parser).Dispose();
			}
			if (!this.convertFragment && this.output != null && this.output != null)
			{
				((IDisposable)this.output).Dispose();
			}
			if (this.injection != null)
			{
				((IDisposable)this.injection).Dispose();
			}
			this.parser = null;
			this.output = null;
			this.injection = null;
			GC.SuppressFinalize(this);
		}

		protected TextParser parser;

		protected bool endOfFile;

		protected TextOutput output;

		protected bool convertFragment;

		protected int lineLength;

		protected int newLines;

		protected int spaces;

		protected int nbsps;

		protected bool paragraphStarted;

		protected bool treatNbspAsBreakable;

		private bool started;

		protected Injection injection;
	}
}
