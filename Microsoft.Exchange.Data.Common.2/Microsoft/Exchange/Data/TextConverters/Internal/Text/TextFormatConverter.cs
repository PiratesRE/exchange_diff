using System;
using System.IO;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
	internal class TextFormatConverter : FormatConverter, IProducerConsumer, IDisposable
	{
		public TextFormatConverter(TextParser parser, FormatOutput output, Injection injection, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream) : base(formatConverterTraceStream)
		{
			this.parser = parser;
			this.output = output;
			if (this.output != null)
			{
				this.output.Initialize(this.Store, SourceFormat.Text, "converted from text");
			}
			this.injection = injection;
			base.InitializeDocument();
			if (this.injection != null)
			{
				bool haveHead = this.injection.HaveHead;
			}
			base.OpenContainer(FormatContainerType.Block, false);
			base.Last.SetProperty(PropertyPrecedence.NonStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Points, 10));
		}

		public TextFormatConverter(TextParser parser, FormatStore store, Injection injection, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream) : base(store, formatConverterTraceStream)
		{
			this.parser = parser;
			this.injection = injection;
			base.InitializeDocument();
			base.OpenContainer(FormatContainerType.Block, false);
			base.Last.SetProperty(PropertyPrecedence.NonStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Points, 10));
		}

		public void Initialize(string fragment)
		{
			this.parser.Initialize(fragment);
			this.lineLength = 0;
			this.newLines = 0;
			this.spaces = 0;
			this.nbsps = 0;
			this.paragraphStarted = false;
		}

		public override void Run()
		{
			if (this.output != null && base.MustFlush)
			{
				if (this.CanFlush)
				{
					this.FlushOutput();
					return;
				}
			}
			else if (!base.EndOfFile)
			{
				TextTokenId textTokenId = this.parser.Parse();
				if (textTokenId != TextTokenId.None)
				{
					this.Process(textTokenId);
				}
			}
		}

		public bool Flush()
		{
			this.Run();
			return base.EndOfFile && !base.MustFlush;
		}

		void IDisposable.Dispose()
		{
			if (this.parser != null)
			{
				((IDisposable)this.parser).Dispose();
			}
			this.parser = null;
			GC.SuppressFinalize(this);
		}

		private bool CanFlush
		{
			get
			{
				return this.output.CanAcceptMoreOutput;
			}
		}

		private bool FlushOutput()
		{
			if (this.output.Flush())
			{
				base.MustFlush = false;
				return true;
			}
			return false;
		}

		protected void Process(TextTokenId tokenId)
		{
			switch (tokenId)
			{
			case TextTokenId.EndOfFile:
				if (this.injection != null && this.injection.HaveTail)
				{
					base.AddLineBreak(1);
				}
				base.CloseContainer();
				base.CloseAllContainersAndSetEOF();
				break;
			case TextTokenId.Text:
				this.OutputFragmentSimple(this.parser.Token);
				return;
			case TextTokenId.EncodingChange:
				if (this.output != null && this.output.OutputCodePageSameAsInput)
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
					if (textType <= RunTextType.NewLine)
					{
						if (textType == RunTextType.Unknown)
						{
							goto IL_B3;
						}
						if (textType != RunTextType.Space)
						{
							if (textType != RunTextType.NewLine)
							{
								continue;
							}
							base.AddLineBreak(1);
							continue;
						}
					}
					else if (textType <= RunTextType.UnusualWhitespace)
					{
						if (textType == RunTextType.Tabulation)
						{
							base.AddTabulation(tokenRun.Length);
							continue;
						}
						if (textType != RunTextType.UnusualWhitespace)
						{
							continue;
						}
					}
					else
					{
						if (textType == RunTextType.Nbsp)
						{
							base.AddNbsp(tokenRun.Length);
							continue;
						}
						if (textType != RunTextType.NonSpace)
						{
							continue;
						}
						goto IL_B3;
					}
					base.AddSpace(tokenRun.Length);
					continue;
					IL_B3:
					base.AddNonSpaceText(tokenRun.RawBuffer, tokenRun.RawOffset, tokenRun.RawLength);
				}
				else if (tokenRun.IsSpecial)
				{
					uint kind = tokenRun.Kind;
				}
			}
		}

		protected TextParser parser;

		private FormatOutput output;

		protected int lineLength;

		protected int newLines;

		protected int spaces;

		protected int nbsps;

		protected bool paragraphStarted;

		protected Injection injection;
	}
}
