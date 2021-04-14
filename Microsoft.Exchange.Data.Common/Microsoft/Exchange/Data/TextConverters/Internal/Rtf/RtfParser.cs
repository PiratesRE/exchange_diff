using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal class RtfParser : RtfParserBase
	{
		public RtfParser(Stream input, bool push, int inputBufferSize, bool testBoundaryConditions, IProgressMonitor progressMonitor, IReportBytes reportBytes) : base(inputBufferSize, testBoundaryConditions, reportBytes)
		{
			this.progressMonitor = progressMonitor;
			if (push)
			{
				this.pushSource = (ConverterStream)input;
			}
			else
			{
				this.pullSource = input;
			}
			this.runQueue = new RtfRunEntry[testBoundaryConditions ? 15 : 256];
			this.token = new RtfToken(base.ParseBuffer, this.runQueue);
			this.fonts = new RtfParser.RtfParserFont[testBoundaryConditions ? 5 : 16];
			this.fontDirectory = new short[testBoundaryConditions ? 9 : 65];
			this.Initialize();
		}

		internal RtfParser(Stream input, int inputBufferSize, bool testBoundaryConditions, IProgressMonitor progressMonitor, RtfParserBase previewParser, IReportBytes reportBytes) : base(inputBufferSize, testBoundaryConditions, previewParser, reportBytes)
		{
			this.progressMonitor = progressMonitor;
			this.pullSource = input;
			this.runQueue = new RtfRunEntry[testBoundaryConditions ? 15 : 256];
			this.token = new RtfToken(base.ParseBuffer, this.runQueue);
			this.fonts = new RtfParser.RtfParserFont[testBoundaryConditions ? 5 : 16];
			this.fontDirectory = new short[testBoundaryConditions ? 9 : 65];
			this.Initialize();
		}

		public RtfToken Token
		{
			get
			{
				return this.token;
			}
		}

		public short CurrentFontIndex
		{
			get
			{
				return this.state.FontIndex;
			}
		}

		public short CurrentFontSize
		{
			get
			{
				return this.state.FontSize;
			}
		}

		public bool CurrentFontBold
		{
			get
			{
				return this.state.Bold;
			}
		}

		public bool CurrentFontItalic
		{
			get
			{
				return this.state.Italic;
			}
		}

		public bool CurrentRunBiDi
		{
			get
			{
				return this.state.BiDi;
			}
		}

		public bool CurrentRunComplexScript
		{
			get
			{
				return this.state.ComplexScript;
			}
		}

		public ushort CurrentCodePage
		{
			get
			{
				return this.currentCodePage;
			}
		}

		public TextMapping CurrentTextMapping
		{
			get
			{
				return this.currentTextMapping;
			}
		}

		public RtfSupport.CharRep CurrentCharRep
		{
			get
			{
				return this.currentCharRep;
			}
		}

		public short CurrentLanguage
		{
			get
			{
				return this.state.Language;
			}
		}

		public short DefaultLanguage
		{
			get
			{
				return this.state.DefaultLanguage;
			}
		}

		public short DefaultLanguageFE
		{
			get
			{
				return this.state.DefaultLanguageFE;
			}
		}

		public void Restart()
		{
			base.InitializeBase();
			this.Initialize();
		}

		public short FontIndex(short fontHandle)
		{
			if (fontHandle < 0)
			{
				return -1;
			}
			if ((int)fontHandle < this.fontDirectory.Length)
			{
				return this.fontDirectory[(int)fontHandle];
			}
			for (int i = (int)(this.fontsCount - 1); i >= 0; i--)
			{
				if (this.fonts[i].Handle == fontHandle)
				{
					return (short)i;
				}
			}
			return -1;
		}

		public int FontCodePage(short fontIndex)
		{
			return (int)this.fonts[(int)fontIndex].CodePage;
		}

		public RtfSupport.CharRep FontCharRep(short fontIndex)
		{
			return this.fonts[(int)fontIndex].CharRep;
		}

		public short FontHandle(short fontIndex)
		{
			return this.fonts[(int)fontIndex].Handle;
		}

		public RtfFontFamily FontFamily(short fontIndex)
		{
			return this.fonts[(int)fontIndex].Family;
		}

		public RtfFontPitch FontPitch(short fontIndex)
		{
			return this.fonts[(int)fontIndex].Pitch;
		}

		public RtfTokenId Parse()
		{
			RtfTokenId rtfTokenId = RtfTokenId.None;
			this.runQueueTail = 0;
			int length;
			if (this.overflowRun)
			{
				rtfTokenId = RtfToken.TokenIdFromRunKind(this.run.Kind);
				this.runQueue[this.runQueueTail++] = this.run;
				this.overflowRun = false;
				if (this.run.Kind == RtfRunKind.Begin)
				{
					this.state.Push();
				}
				else if (this.run.Kind == RtfRunKind.End)
				{
					this.PopState();
				}
				else if (this.run.Kind == RtfRunKind.Keyword && RTFData.keywords[(int)this.run.KeywordId].affectsParsing)
				{
					this.ProcessInterestingKeyword();
				}
				if (rtfTokenId <= RtfTokenId.Binary)
				{
					length = base.ParseOffset - base.ParseStart;
					this.token.Initialize(rtfTokenId, this.runQueueTail, base.ParseStart, base.ParseOffset - base.ParseStart);
					base.ReportConsumed(length);
					return rtfTokenId;
				}
			}
			if (!base.EndOfFileVisible && base.ParseBufferNeedsRefill && !this.ReadMoreData(true))
			{
				if (this.runQueueTail != 0)
				{
					this.overflowRun = true;
				}
				this.token.Reset();
				return RtfTokenId.None;
			}
			ushort num = 0;
			while (this.CanQueueRun() && base.ParseRun())
			{
				RtfTokenId rtfTokenId2 = RtfToken.TokenIdFromRunKind(this.run.Kind);
				if (rtfTokenId == RtfTokenId.None)
				{
					rtfTokenId = rtfTokenId2;
				}
				else if (rtfTokenId2 != rtfTokenId && rtfTokenId2 != RtfTokenId.None)
				{
					this.overflowRun = true;
					num = this.run.Length;
					break;
				}
				this.runQueue[this.runQueueTail++] = this.run;
				if (this.run.Kind == RtfRunKind.Begin)
				{
					this.state.Push();
				}
				else if (this.run.Kind == RtfRunKind.End)
				{
					this.PopState();
				}
				else if (this.run.Kind == RtfRunKind.Keyword && RTFData.keywords[(int)this.run.KeywordId].affectsParsing)
				{
					this.ProcessInterestingKeyword();
				}
				if (rtfTokenId <= RtfTokenId.Binary)
				{
					break;
				}
			}
			if (rtfTokenId == RtfTokenId.None)
			{
				rtfTokenId = RtfTokenId.Keywords;
			}
			else if (rtfTokenId == RtfTokenId.Text)
			{
				this.token.SetCodePage((int)this.currentCodePage, this.currentTextMapping);
				if (this.state.Destination == RtfParser.RtfParserDestination.FontTable && !this.fontNameRecognizer.IsRejected)
				{
					int num2 = base.ParseStart;
					for (int i = 0; i < this.runQueueTail; i++)
					{
						if (this.runQueue[i].Kind == RtfRunKind.Text)
						{
							for (int j = 0; j < (int)this.runQueue[i].Length; j++)
							{
								this.fontNameRecognizer.AddCharacter(base.ParseBuffer[num2 + j]);
							}
						}
						else if (this.run.Kind != RtfRunKind.Ignore && this.run.Kind != RtfRunKind.Zero)
						{
							this.fontNameRecognizer.AddCharacter(byte.MaxValue);
						}
						num2 += (int)this.runQueue[i].Length;
					}
				}
				else if (!this.overflowRun && this.lastLeadByte)
				{
					int num3 = this.runQueueTail - 1;
					if (this.runQueue[num3].Kind != RtfRunKind.Ignore)
					{
						this.overflowRun = true;
						num = this.run.Length;
						this.runQueueTail--;
					}
				}
			}
			length = base.ParseOffset - (int)num - base.ParseStart;
			this.token.Initialize(rtfTokenId, this.runQueueTail, base.ParseStart, length);
			base.ReportConsumed(length);
			return rtfTokenId;
		}

		internal bool ReadMoreData(bool compact)
		{
			int num;
			int bufferSpace = base.GetBufferSpace(compact, out num);
			if (bufferSpace == 0)
			{
				return true;
			}
			bool endOfFileVisible;
			int num2;
			if (this.pushSource != null)
			{
				byte[] src;
				int srcOffset;
				int val;
				if (!this.pushSource.GetInputChunk(out src, out srcOffset, out val, out endOfFileVisible))
				{
					return false;
				}
				num2 = Math.Min(val, bufferSpace);
				if (num2 != 0)
				{
					Buffer.BlockCopy(src, srcOffset, base.ParseBuffer, num, num2);
					this.pushSource.ReportRead(num2);
					this.bytesReadTotal += num2;
				}
			}
			else
			{
				num2 = this.pullSource.Read(base.ParseBuffer, num, bufferSpace);
				if (num2 == 0)
				{
					endOfFileVisible = true;
				}
				else
				{
					endOfFileVisible = false;
					this.bytesReadTotal += num2;
					this.progressMonitor.ReportProgress();
				}
			}
			base.ReportMoreDataAvailable(num2, endOfFileVisible);
			return true;
		}

		private void Initialize()
		{
			this.runQueueTail = 0;
			this.overflowRun = false;
			this.token.Reset();
			this.fonts[0].Initialize(-1, 1252);
			this.fonts[0].Family = RtfFontFamily.Swiss;
			this.fontsCount = 1;
			for (int i = 0; i < this.fontDirectory.Length; i++)
			{
				this.fontDirectory[i] = -1;
			}
			this.state.Initialize();
		}

		private bool CanQueueRun()
		{
			return this.runQueueTail < this.runQueue.Length - 1;
		}

		private void PopState()
		{
			if (this.state.Level != 0)
			{
				short fontIndex = this.state.FontIndex;
				RtfParser.RtfParserDestination destination = this.state.Destination;
				this.state.Pop();
				if (destination == RtfParser.RtfParserDestination.FontTable)
				{
					if (this.state.Destination != RtfParser.RtfParserDestination.FontTable)
					{
						if (fontIndex >= 0)
						{
							this.fonts[(int)fontIndex].TextMapping = this.fontNameRecognizer.TextMapping;
							if (this.fonts[(int)fontIndex].TextMapping == TextMapping.Unicode && this.fonts[(int)fontIndex].CharRep == RtfSupport.CharRep.SYMBOL_INDEX)
							{
								this.fonts[(int)fontIndex].TextMapping = TextMapping.OtherSymbol;
							}
						}
						this.fontNameRecognizer = default(RecognizeInterestingFontName);
						this.state.FontIndex = 0;
						this.state.FontIndexAscii = 0;
						if (this.defaultCodePage == 0)
						{
							if (this.state.DefaultLanguageFE != -1 && this.state.DefaultLanguageFE != 1033)
							{
								this.defaultCodePage = RtfSupport.CodePageFromCharRep(RtfSupport.CharRepFromLanguage((int)this.state.DefaultLanguageFE));
							}
							else if (this.state.DefaultLanguage != -1 && this.state.DefaultLanguage != 1033)
							{
								this.defaultCodePage = RtfSupport.CodePageFromCharRep(RtfSupport.CharRepFromLanguage((int)this.state.DefaultLanguage));
							}
						}
						this.SelectCurrentFont();
					}
					else
					{
						this.state.FontIndex = fontIndex;
					}
				}
				base.SetCodePage(this.state.CodePage, this.state.TextMapping);
				this.currentCharRep = this.state.CharRep;
				this.bytesSkipForUnicodeEscape = this.state.BytesSkipForUnicodeEscape;
			}
		}

		private void ProcessInterestingKeyword()
		{
			int num = this.run.Value;
			short keywordId = this.run.KeywordId;
			short num2;
			Culture culture;
			if (keywordId <= 137)
			{
				if (keywordId <= 49)
				{
					if (keywordId <= 30)
					{
						if (keywordId != 1)
						{
							if (keywordId != 18 && keywordId != 30)
							{
								return;
							}
							goto IL_6B1;
						}
						else
						{
							if (this.run.Lead && this.state.Destination == RtfParser.RtfParserDestination.FontTable)
							{
								this.state.Destination = RtfParser.RtfParserDestination.IgnorableDestinationInFontTable;
								return;
							}
							return;
						}
					}
					else if (keywordId <= 37)
					{
						if (keywordId != 34)
						{
							if (keywordId != 37)
							{
								return;
							}
						}
						else
						{
							if (this.state.Destination != RtfParser.RtfParserDestination.FontTable || this.state.FontIndex < 0)
							{
								return;
							}
							this.fonts[(int)this.state.FontIndex].CharRep = RtfSupport.CharRepFromCharSet(num);
							this.fonts[(int)this.state.FontIndex].CodePage = RtfSupport.CodePageFromCharRep(this.fonts[(int)this.state.FontIndex].CharRep);
							if (!this.state.CodePageFixed && this.fonts[(int)this.state.FontIndex].CodePage != 42)
							{
								this.state.CodePage = this.fonts[(int)this.state.FontIndex].CodePage;
								this.state.TextMapping = TextMapping.Unicode;
								if (this.defaultCodePage == 0)
								{
									this.defaultCodePage = this.state.CodePage;
								}
								base.SetCodePage(this.state.CodePage, this.state.TextMapping);
							}
							if (RtfSupport.IsRtlCharSet(num) && !RtfSupport.IsBiDiCharRep(this.state.CharRepBiDi))
							{
								this.state.CharRepBiDi = this.fonts[(int)this.state.FontIndex].CharRep;
								return;
							}
							return;
						}
					}
					else if (keywordId != 42)
					{
						if (keywordId != 49)
						{
							return;
						}
						goto IL_6B1;
					}
					else
					{
						if (this.state.Destination == RtfParser.RtfParserDestination.FontTable && this.state.FontIndex >= 0 && num >= 0 && num <= 2)
						{
							this.fonts[(int)this.state.FontIndex].Pitch = (RtfFontPitch)num;
							return;
						}
						return;
					}
				}
				else if (keywordId <= 81)
				{
					if (keywordId == 60)
					{
						goto IL_357;
					}
					switch (keywordId)
					{
					case 71:
						goto IL_FAD;
					case 72:
						goto IL_6B1;
					default:
						if (keywordId != 81)
						{
							return;
						}
						if (0 <= num && num <= 2 && (int)this.state.BytesSkipForUnicodeEscape != num)
						{
							this.state.BytesSkipForUnicodeEscape = (this.bytesSkipForUnicodeEscape = (byte)num);
							return;
						}
						return;
					}
				}
				else if (keywordId <= 109)
				{
					switch (keywordId)
					{
					case 93:
					case 94:
						goto IL_6B1;
					case 95:
						goto IL_C50;
					default:
						if (keywordId != 109)
						{
							return;
						}
						goto IL_CEA;
					}
				}
				else
				{
					switch (keywordId)
					{
					case 113:
						if (this.state.Destination != RtfParser.RtfParserDestination.FontTable)
						{
							num2 = this.FontIndex((short)num);
							if (num2 == -1)
							{
								num2 = this.state.DefaultFontIndex;
								goto IL_E77;
							}
							goto IL_E77;
						}
						else
						{
							if (this.state.FontIndex >= 0)
							{
								this.fonts[(int)this.state.FontIndex].TextMapping = this.fontNameRecognizer.TextMapping;
								if (this.fonts[(int)this.state.FontIndex].TextMapping == TextMapping.Unicode && this.fonts[(int)this.state.FontIndex].CharRep == RtfSupport.CharRep.SYMBOL_INDEX)
								{
									this.fonts[(int)this.state.FontIndex].TextMapping = TextMapping.OtherSymbol;
								}
							}
							this.fontNameRecognizer = default(RecognizeInterestingFontName);
							if (num < 0)
							{
								return;
							}
							if (num > 32767)
							{
								return;
							}
							this.state.FontIndex = this.FontIndex((short)num);
							if (this.state.FontIndex >= 0)
							{
								return;
							}
							if ((int)this.fontsCount == this.fonts.Length)
							{
								if (this.fontsCount >= 2048)
								{
									throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
								}
								RtfParser.RtfParserFont[] destinationArray = new RtfParser.RtfParserFont[this.fonts.Length * 2];
								Array.Copy(this.fonts, destinationArray, (int)this.fontsCount);
								this.fonts = destinationArray;
							}
							if (this.defaultFontHandle == (short)num)
							{
								this.state.DefaultFontIndex = this.fontsCount;
							}
							short fontIndex;
							this.fontsCount = (fontIndex = this.fontsCount) + 1;
							this.state.FontIndex = fontIndex;
							this.fonts[(int)this.state.FontIndex].Initialize((short)num, this.defaultCodePage);
							this.fonts[(int)this.state.FontIndex].Handle = (short)num;
							if ((int)this.fonts[(int)this.state.FontIndex].Handle < this.fontDirectory.Length)
							{
								this.fontDirectory[(int)this.fonts[(int)this.state.FontIndex].Handle] = this.state.FontIndex;
								return;
							}
							return;
						}
						break;
					case 114:
						return;
					case 115:
						goto IL_D63;
					default:
						switch (keywordId)
						{
						case 133:
						case 135:
						case 136:
							return;
						case 134:
							goto IL_B11;
						case 137:
							goto IL_C50;
						default:
							return;
						}
						break;
					}
				}
			}
			else
			{
				if (keywordId <= 223)
				{
					if (keywordId <= 175)
					{
						if (keywordId != 149)
						{
							switch (keywordId)
							{
							case 164:
								goto IL_B11;
							case 165:
								if (num > 0 && num < 65535 && !this.state.CodePageFixed)
								{
									this.defaultCodePage = (ushort)num;
									this.fonts[0].CodePage = (ushort)num;
									this.state.CodePage = this.defaultCodePage;
									base.SetCodePage(this.defaultCodePage, TextMapping.Unicode);
									return;
								}
								return;
							case 166:
							case 167:
							case 168:
								return;
							case 169:
								break;
							case 170:
								this.state.SetPlain();
								this.SelectCurrentFont();
								return;
							default:
								if (keywordId != 175)
								{
									return;
								}
								if (!this.run.Lead || this.state.Destination != RtfParser.RtfParserDestination.Default)
								{
									return;
								}
								this.state.Destination = RtfParser.RtfParserDestination.FontTable;
								this.state.FontIndex = -1;
								if (this.defaultCodePage != 0 && !this.state.CodePageFixed)
								{
									base.SetCodePage(this.defaultCodePage, TextMapping.Unicode);
									return;
								}
								return;
							}
						}
						else
						{
							if (Culture.TryGetCulture((int)((short)num), out culture) && RtfSupport.IsFECharRep(RtfSupport.CharRepFromLanguage((int)((short)num))))
							{
								this.state.DefaultLanguageFE = (short)num;
								return;
							}
							return;
						}
					}
					else if (keywordId <= 210)
					{
						if (keywordId != 185)
						{
							if (keywordId != 210)
							{
								return;
							}
							if (this.run.Lead && this.state.Destination == RtfParser.RtfParserDestination.FontTable && this.state.FontIndex >= 0)
							{
								this.state.Destination = RtfParser.RtfParserDestination.RealFontName;
								return;
							}
							return;
						}
					}
					else
					{
						if (keywordId == 214)
						{
							goto IL_357;
						}
						switch (keywordId)
						{
						case 222:
							goto IL_C50;
						case 223:
							num2 = this.FontIndex((short)num);
							if (num2 == -1)
							{
								num2 = this.state.DefaultFontIndex;
								goto IL_E77;
							}
							goto IL_E77;
						default:
							return;
						}
					}
				}
				else if (keywordId <= 287)
				{
					if (keywordId <= 271)
					{
						switch (keywordId)
						{
						case 238:
							goto IL_FAD;
						case 239:
							return;
						case 240:
						{
							if (num <= 0 || num >= 65535)
							{
								return;
							}
							Encoding encoding;
							if (num != 42 && !Charset.TryGetEncoding(num, out encoding))
							{
								num = (int)this.defaultCodePage;
							}
							if (!this.state.CodePageFixed)
							{
								this.state.CodePage = (ushort)num;
								this.state.TextMapping = TextMapping.Unicode;
								if (num != 42 || this.state.Destination != RtfParser.RtfParserDestination.FontTable)
								{
									base.SetCodePage((ushort)num, TextMapping.Unicode);
								}
							}
							if (this.state.Destination != RtfParser.RtfParserDestination.FontTable || this.state.FontIndex < 0)
							{
								return;
							}
							this.fonts[(int)this.state.FontIndex].CodePage = (ushort)num;
							this.fonts[(int)this.state.FontIndex].CharRep = RtfSupport.CharRepFromCodePage((ushort)num);
							if (this.defaultCodePage == 0 && num != 42)
							{
								this.defaultCodePage = (ushort)num;
							}
							if (RtfSupport.IsBiDiCharRep(this.fonts[(int)this.state.FontIndex].CharRep) && !RtfSupport.IsBiDiCharRep(this.state.CharRepBiDi))
							{
								this.state.CharRepBiDi = this.fonts[(int)this.state.FontIndex].CharRep;
								return;
							}
							return;
						}
						default:
							switch (keywordId)
							{
							case 265:
								break;
							case 266:
								goto IL_6B1;
							case 267:
							case 269:
							case 270:
								return;
							case 268:
								if (this.run.Lead && this.state.Destination == RtfParser.RtfParserDestination.FontTable && this.state.FontIndex >= 0)
								{
									this.state.Destination = RtfParser.RtfParserDestination.AltFontName;
									return;
								}
								return;
							case 271:
								if (this.state.Level == 1 && this.run.Lead)
								{
									this.state.CodePage = 65001;
									this.state.TextMapping = TextMapping.Unicode;
									this.state.CodePageFixed = true;
									this.defaultCodePage = 65001;
									base.SetCodePage(65001, TextMapping.Unicode);
									return;
								}
								return;
							default:
								return;
							}
							break;
						}
					}
					else
					{
						switch (keywordId)
						{
						case 280:
							if (num < 0 || num > 32767)
							{
								return;
							}
							this.defaultFontHandle = (short)num;
							num2 = this.FontIndex((short)num);
							if (num2 != -1)
							{
								this.state.DefaultFontIndex = num2;
								return;
							}
							if (this.fonts[0].Handle != -1)
							{
								return;
							}
							this.fonts[0].Handle = (short)num;
							if ((int)this.fonts[0].Handle < this.fontDirectory.Length)
							{
								this.fontDirectory[(int)this.fonts[0].Handle] = 0;
								return;
							}
							return;
						case 281:
							goto IL_FAD;
						default:
							if (keywordId != 287)
							{
								return;
							}
							goto IL_B11;
						}
					}
				}
				else if (keywordId <= 308)
				{
					switch (keywordId)
					{
					case 291:
						goto IL_D63;
					case 292:
						goto IL_34B;
					default:
						switch (keywordId)
						{
						case 307:
							goto IL_CEA;
						case 308:
							goto IL_6B1;
						default:
							return;
						}
						break;
					}
				}
				else if (keywordId != 312)
				{
					if (keywordId != 329)
					{
						return;
					}
					goto IL_34B;
				}
				else
				{
					num2 = this.FontIndex((short)num);
					if (num2 == -1)
					{
						num2 = this.state.DefaultFontIndex;
					}
					if (!this.state.InAssocFont && this.state.AssociateRunKind != RtfTextRunKind.None)
					{
						if (this.state.AssociateRunKind == RtfTextRunKind.Dbch && !RtfSupport.IsFECharRep(this.FontCharRep(num2)))
						{
							num2 = this.state.DefaultFontIndex;
						}
						this.state.AssociateFontIndex = num2;
						this.state.AssociateRunKind = RtfTextRunKind.None;
						return;
					}
					goto IL_E77;
				}
				if (num >= 0 && num <= 3276)
				{
					this.state.FontSizeOther = (short)(num * 10);
					this.state.FontSize = this.state.FontSizeOther;
					return;
				}
				return;
			}
			IL_34B:
			this.state.SetDetectSingleChpRtl();
			return;
			IL_357:
			if (Culture.TryGetCulture((int)((short)num), out culture))
			{
				this.state.DefaultLanguage = (short)num;
				return;
			}
			return;
			IL_6B1:
			if (this.state.Destination != RtfParser.RtfParserDestination.FontTable || this.state.FontIndex < 0)
			{
				return;
			}
			this.fonts[(int)this.state.FontIndex].Family = (RtfFontFamily)RTFData.keywords[(int)this.run.KeywordId].idx;
			if (this.run.KeywordId == 266 && this.fonts[(int)this.state.FontIndex].CharRep == RtfSupport.CharRep.DEFAULT_INDEX)
			{
				this.fonts[(int)this.state.FontIndex].CharRep = RtfSupport.CharRep.SYMBOL_INDEX;
				return;
			}
			return;
			IL_B11:
			if (this.run.KeywordId == 164)
			{
				this.state.SetFcs(num != 0);
			}
			else
			{
				this.state.SetRtlch(this.run.KeywordId == 287);
			}
			if (this.state.FillBiProps)
			{
				if (this.state.FontIndexCS != -1)
				{
					this.state.FontIndex = this.state.FontIndexCS;
				}
				if (this.state.FontSizeCS != 0)
				{
					this.state.FontSize = this.state.FontSizeCS;
				}
				this.state.Bold = this.state.BoldCS;
				this.state.Italic = this.state.ItalicCS;
			}
			else
			{
				if (this.state.FontIndexOther != -1)
				{
					this.state.FontIndex = this.state.FontIndexOther;
				}
				if (this.state.FontSizeOther != 0)
				{
					this.state.FontSize = this.state.FontSizeOther;
				}
				this.state.Bold = this.state.BoldOther;
				this.state.Italic = this.state.ItalicOther;
			}
			this.SelectCurrentFont();
			return;
			IL_C50:
			this.state.AssociateRunKind = (RtfTextRunKind)RTFData.keywords[(int)this.run.KeywordId].idx;
			if (this.state.AssociateFontIndex != -1)
			{
				this.state.FontIndex = this.state.AssociateFontIndex;
			}
			if (this.state.FontSizeOther != 0)
			{
				this.state.FontSize = this.state.FontSizeOther;
			}
			this.state.Bold = this.state.BoldOther;
			this.state.Italic = this.state.ItalicOther;
			return;
			IL_CEA:
			if (!this.state.DualChpRtfCS)
			{
				this.state.BoldOther = (0 != num);
				this.state.BoldCS = (0 != num);
			}
			else if (!this.state.FillBiProps)
			{
				this.state.BoldOther = (0 != num);
			}
			else
			{
				this.state.BoldCS = (0 != num);
			}
			this.state.Bold = (0 != num);
			return;
			IL_D63:
			if (!this.state.DualChpRtfCS)
			{
				this.state.ItalicOther = (0 != num);
				this.state.ItalicCS = (0 != num);
			}
			else if (!this.state.FillBiProps)
			{
				this.state.ItalicOther = (0 != num);
			}
			else
			{
				this.state.ItalicCS = (0 != num);
			}
			this.state.Italic = (0 != num);
			return;
			IL_E77:
			if (this.state.AssociateRunKind != RtfTextRunKind.None)
			{
				if (this.state.AssociateRunKind == RtfTextRunKind.Dbch && !RtfSupport.IsFECharRep(this.FontCharRep(num2)))
				{
					num2 = this.state.DefaultFontIndex;
				}
				this.state.AssociateFontIndex = num2;
				if (!this.state.DualChpRtfCS)
				{
					this.state.FontIndexCS = num2;
				}
				this.state.AssociateRunKind = RtfTextRunKind.None;
			}
			else if (this.state.DualChpRtfCS)
			{
				if (this.state.FillBiProps)
				{
					this.state.FontIndexCS = num2;
				}
				else
				{
					this.state.FontIndexAscii = num2;
					if (!RtfSupport.IsFECharRep(this.FontCharRep(num2)))
					{
						this.state.FontIndexOther = num2;
					}
					else
					{
						this.state.FontIndexDbCh = num2;
					}
				}
			}
			else
			{
				if (RtfSupport.IsBiDiCharRep(this.FontCharRep(num2)))
				{
					this.state.FontIndexCS = num2;
				}
				this.state.FontIndexAscii = num2;
				if (!RtfSupport.IsFECharRep(this.FontCharRep(num2)))
				{
					this.state.FontIndexOther = num2;
				}
				else
				{
					this.state.FontIndexDbCh = num2;
				}
			}
			this.state.FontIndex = num2;
			this.SelectCurrentFont();
			return;
			IL_FAD:
			if (num > 0 && num <= 32767 && Culture.TryGetCulture(num, out culture))
			{
				this.state.Language = (short)num;
				if (RtfSupport.IsBiDiLanguage(num))
				{
					this.state.CharRepBiDi = RtfSupport.CharRepFromLanguage(num);
					if (this.state.AssociateRunKind == RtfTextRunKind.Ltrch)
					{
						this.state.AssociateRunKind = RtfTextRunKind.Rtlch;
					}
					if (this.state.CharRepBiDi > RtfSupport.CharRep.NCHARSETS && this.state.FontIndex >= 0)
					{
						this.fonts[(int)this.state.FontIndex].CharRep = this.state.CharRepBiDi;
						return;
					}
				}
			}
		}

		private void SelectCurrentFont()
		{
			if (this.state.FontIndex == -1)
			{
				this.state.FontIndex = this.state.DefaultFontIndex;
			}
			if (this.state.Destination != RtfParser.RtfParserDestination.FontTable)
			{
				this.state.CharRep = (this.currentCharRep = this.fonts[(int)this.state.FontIndex].CharRep);
				if (this.state.CharRep == RtfSupport.CharRep.DEFAULT_INDEX && this.state.Language != -1)
				{
					RtfSupport.CharRep charRep = RtfSupport.CharRepFromLanguage((int)this.state.Language);
					if (charRep > RtfSupport.CharRep.NCHARSETS)
					{
						this.state.CharRep = (this.currentCharRep = charRep);
						if (charRep == this.state.CharRepBiDi)
						{
							this.state.FontIndexCS = this.state.FontIndex;
						}
					}
				}
				if (RtfSupport.IsBiDiCharRep(this.state.CharRep) && this.fonts[(int)this.state.FontIndex].CodePage == 1252)
				{
					this.fonts[(int)this.state.FontIndex].CodePage = RtfSupport.CodePageFromCharRep(this.state.CharRep);
				}
			}
			if (!this.state.CodePageFixed)
			{
				if (this.fonts[(int)this.state.FontIndex].CodePage == 0)
				{
					this.state.CodePage = this.defaultCodePage;
					this.state.TextMapping = TextMapping.Unicode;
				}
				else
				{
					this.state.CodePage = this.fonts[(int)this.state.FontIndex].CodePage;
					this.state.TextMapping = this.fonts[(int)this.state.FontIndex].TextMapping;
				}
				if (this.state.CodePage <= 0)
				{
					this.state.CodePage = 1252;
					this.state.TextMapping = TextMapping.Unicode;
				}
				else if (this.state.CodePage == 1255)
				{
					if (!RtfSupport.IsHebrewLanguage((int)this.state.Language))
					{
						this.state.Language = 13;
					}
				}
				else if (this.state.CodePage == 1256)
				{
					if (!RtfSupport.IsArabicLanguage((int)this.state.Language))
					{
						this.state.Language = 1;
					}
				}
				else if (this.state.CodePage == 874 && !RtfSupport.IsThaiLanguage((int)this.state.Language))
				{
					this.state.Language = 30;
				}
				if (!this.state.DualChpRtfCS || !this.state.InAssocFont)
				{
					base.SetCodePage(this.state.CodePage, this.state.TextMapping);
				}
			}
		}

		private const int UndefinedDefaultFontIndex = 0;

		private Stream pullSource;

		private ConverterStream pushSource;

		private IProgressMonitor progressMonitor;

		private int bytesReadTotal;

		private RtfRunEntry[] runQueue;

		private int runQueueTail;

		private bool overflowRun;

		private RtfToken token;

		private short fontsCount;

		private RtfParser.RtfParserFont[] fonts;

		private short[] fontDirectory;

		private RtfParser.RtfParserState state;

		private short defaultFontHandle;

		private RecognizeInterestingFontName fontNameRecognizer;

		internal enum RtfParserDestination : byte
		{
			Default,
			FontTable,
			RealFontName,
			AltFontName,
			IgnorableDestinationInFontTable
		}

		internal struct RtfParserState
		{
			public bool DualChpRtfCS
			{
				get
				{
					return this.detectAssocSA || (this.detectAssocRtl && !this.detectSingleChpRtl);
				}
			}

			public bool FillBiProps
			{
				get
				{
					return (this.BiDi && this.ComplexScript) || (this.BiDi && !this.inAssocSA) || (this.ComplexScript && !this.inAssocRtl);
				}
			}

			public bool InAssocFont
			{
				get
				{
					return this.inAssocRtl || this.inAssocSA;
				}
			}

			public bool CodePageFixed
			{
				get
				{
					return this.codePageFixed;
				}
				set
				{
					this.codePageFixed = value;
				}
			}

			public short DefaultFontIndex
			{
				get
				{
					return this.defaultFontIndex;
				}
				set
				{
					this.defaultFontIndex = value;
				}
			}

			public short DefaultLanguage
			{
				get
				{
					return this.defaultLanguage;
				}
				set
				{
					this.defaultLanguage = value;
				}
			}

			public short DefaultLanguageFE
			{
				get
				{
					return this.defaultLanguageFE;
				}
				set
				{
					this.defaultLanguageFE = value;
				}
			}

			public RtfSupport.CharRep CharRepBiDi
			{
				get
				{
					return this.charRepBiDi;
				}
				set
				{
					this.charRepBiDi = value;
				}
			}

			public RtfParser.RtfParserDestination Destination
			{
				get
				{
					return this.Current.Dest;
				}
				set
				{
					if (this.Current.Dest != value)
					{
						this.SetDirty();
						this.Current.Dest = value;
					}
				}
			}

			public bool BiDi
			{
				get
				{
					return (byte)(this.Current.Flags & RtfParser.RtfParserState.StateFlags.BiDi) != 0;
				}
				set
				{
					if (this.BiDi != value)
					{
						this.SetDirty();
						this.Current.Flags = (this.Current.Flags ^ RtfParser.RtfParserState.StateFlags.BiDi);
					}
				}
			}

			public bool ComplexScript
			{
				get
				{
					return (byte)(this.Current.Flags & RtfParser.RtfParserState.StateFlags.ComplexScript) != 0;
				}
				set
				{
					if (this.ComplexScript != value)
					{
						this.SetDirty();
						this.Current.Flags = (this.Current.Flags ^ RtfParser.RtfParserState.StateFlags.ComplexScript);
					}
				}
			}

			public bool Bold
			{
				get
				{
					return (byte)(this.Current.Flags & RtfParser.RtfParserState.StateFlags.Bold) != 0;
				}
				set
				{
					if (this.Bold != value)
					{
						this.SetDirty();
						this.Current.Flags = (this.Current.Flags ^ RtfParser.RtfParserState.StateFlags.Bold);
					}
				}
			}

			public bool BoldCS
			{
				get
				{
					return (byte)(this.Current.Flags & RtfParser.RtfParserState.StateFlags.BoldCS) != 0;
				}
				set
				{
					if (this.BoldCS != value)
					{
						this.SetDirty();
						this.Current.Flags = (this.Current.Flags ^ RtfParser.RtfParserState.StateFlags.BoldCS);
					}
				}
			}

			public bool BoldOther
			{
				get
				{
					return (byte)(this.Current.Flags & RtfParser.RtfParserState.StateFlags.BoldOther) != 0;
				}
				set
				{
					if (this.BoldOther != value)
					{
						this.SetDirty();
						this.Current.Flags = (this.Current.Flags ^ RtfParser.RtfParserState.StateFlags.BoldOther);
					}
				}
			}

			public bool Italic
			{
				get
				{
					return (byte)(this.Current.Flags & RtfParser.RtfParserState.StateFlags.Italic) != 0;
				}
				set
				{
					if (this.Italic != value)
					{
						this.SetDirty();
						this.Current.Flags = (this.Current.Flags ^ RtfParser.RtfParserState.StateFlags.Italic);
					}
				}
			}

			public bool ItalicCS
			{
				get
				{
					return (byte)(this.Current.Flags & RtfParser.RtfParserState.StateFlags.ItalicCS) != 0;
				}
				set
				{
					if (this.ItalicCS != value)
					{
						this.SetDirty();
						this.Current.Flags = (this.Current.Flags ^ RtfParser.RtfParserState.StateFlags.ItalicCS);
					}
				}
			}

			public bool ItalicOther
			{
				get
				{
					return (byte)(this.Current.Flags & RtfParser.RtfParserState.StateFlags.ItalicOther) != 0;
				}
				set
				{
					if (this.ItalicOther != value)
					{
						this.SetDirty();
						this.Current.Flags = (this.Current.Flags ^ RtfParser.RtfParserState.StateFlags.ItalicOther);
					}
				}
			}

			public byte BytesSkipForUnicodeEscape
			{
				get
				{
					return this.Current.BytesSkipForUnicodeEscape;
				}
				set
				{
					if (this.Current.BytesSkipForUnicodeEscape != value)
					{
						this.SetDirty();
						this.Current.BytesSkipForUnicodeEscape = value;
					}
				}
			}

			public ushort CodePage
			{
				get
				{
					return this.Current.CodePage;
				}
				set
				{
					if (this.Current.CodePage != value)
					{
						this.SetDirty();
						this.Current.CodePage = value;
					}
				}
			}

			public TextMapping TextMapping
			{
				get
				{
					return this.Current.TextMapping;
				}
				set
				{
					if (this.Current.TextMapping != value)
					{
						this.SetDirty();
						this.Current.TextMapping = value;
					}
				}
			}

			public RtfSupport.CharRep CharRep
			{
				get
				{
					return this.Current.CharRep;
				}
				set
				{
					if (this.Current.CharRep != value)
					{
						this.SetDirty();
						this.Current.CharRep = value;
					}
				}
			}

			public short Language
			{
				get
				{
					return this.Current.Language;
				}
				set
				{
					if (this.Current.Language != value)
					{
						this.SetDirty();
						this.Current.Language = value;
					}
				}
			}

			public short FontIndex
			{
				get
				{
					return this.Current.FontIndex;
				}
				set
				{
					if (this.Current.FontIndex != value)
					{
						this.SetDirty();
						this.Current.FontIndex = value;
					}
				}
			}

			public short FontIndexAscii
			{
				get
				{
					return this.Current.FontIndexAscii;
				}
				set
				{
					if (this.Current.FontIndexAscii != value)
					{
						this.SetDirty();
						this.Current.FontIndexAscii = value;
					}
				}
			}

			public short FontIndexDbCh
			{
				get
				{
					return this.Current.FontIndexDbCh;
				}
				set
				{
					if (this.Current.FontIndexDbCh != value)
					{
						this.SetDirty();
						this.Current.FontIndexDbCh = value;
					}
				}
			}

			public short FontIndexCS
			{
				get
				{
					return this.Current.FontIndexCS;
				}
				set
				{
					if (this.Current.FontIndexCS != value)
					{
						this.SetDirty();
						this.Current.FontIndexCS = value;
					}
				}
			}

			public short FontIndexOther
			{
				get
				{
					return this.Current.FontIndexOther;
				}
				set
				{
					if (this.Current.FontIndexOther != value)
					{
						this.SetDirty();
						this.Current.FontIndexOther = value;
					}
				}
			}

			public short FontSize
			{
				get
				{
					return this.Current.FontSize;
				}
				set
				{
					if (this.Current.FontSize != value)
					{
						this.SetDirty();
						this.Current.FontSize = value;
					}
				}
			}

			public short FontSizeCS
			{
				get
				{
					return this.Current.FontSizeCS;
				}
				set
				{
					if (this.Current.FontSizeCS != value)
					{
						this.SetDirty();
						this.Current.FontSizeCS = value;
					}
				}
			}

			public short FontSizeOther
			{
				get
				{
					return this.Current.FontSizeOther;
				}
				set
				{
					if (this.Current.FontSizeOther != value)
					{
						this.SetDirty();
						this.Current.FontSizeOther = value;
					}
				}
			}

			public RtfTextRunKind AssociateRunKind
			{
				get
				{
					return this.Current.RunKind;
				}
				set
				{
					if (this.Current.RunKind != value)
					{
						this.SetDirty();
						this.Current.RunKind = value;
					}
				}
			}

			public short AssociateFontIndex
			{
				get
				{
					if (this.Current.RunKind == RtfTextRunKind.None)
					{
						return this.Current.FontIndex;
					}
					if (this.Current.RunKind == RtfTextRunKind.Dbch)
					{
						return this.Current.FontIndexDbCh;
					}
					if (this.Current.RunKind == RtfTextRunKind.Loch)
					{
						return this.Current.FontIndexAscii;
					}
					return this.Current.FontIndexOther;
				}
				set
				{
					if (this.Current.RunKind == RtfTextRunKind.None)
					{
						if (this.Current.FontIndex != value)
						{
							this.SetDirty();
							this.Current.FontIndex = value;
							return;
						}
					}
					else if (this.Current.RunKind == RtfTextRunKind.Dbch)
					{
						if (this.Current.FontIndexDbCh != value)
						{
							this.SetDirty();
							this.Current.FontIndexDbCh = value;
							return;
						}
					}
					else if (this.Current.RunKind == RtfTextRunKind.Loch)
					{
						if (this.Current.FontIndexAscii != value)
						{
							this.SetDirty();
							this.Current.FontIndexAscii = value;
							return;
						}
					}
					else if (this.Current.FontIndexOther != value)
					{
						this.SetDirty();
						this.Current.FontIndexOther = value;
					}
				}
			}

			public void Initialize()
			{
				this.Level = 0;
				this.stackTop = 0;
				this.defaultFontIndex = 0;
				this.defaultLanguage = -1;
				this.defaultLanguageFE = -1;
				this.charRepBiDi = RtfSupport.CharRep.ANSI_INDEX;
				this.codePageFixed = false;
				this.Current.Depth = 1;
				this.Current.Dest = RtfParser.RtfParserDestination.Default;
				this.Current.BytesSkipForUnicodeEscape = 1;
				this.SetPlain();
			}

			public void SetDetectSingleChpRtl()
			{
				this.detectSingleChpRtl = true;
			}

			public void SetRtlch(bool value)
			{
				this.detectAssocRtl = true;
				this.inAssocRtl = !this.inAssocRtl;
				this.BiDi = value;
			}

			public void SetFcs(bool value)
			{
				this.detectAssocSA = true;
				this.inAssocSA = !this.inAssocSA;
				this.ComplexScript = value;
			}

			public void SetPlain()
			{
				this.SetDirty();
				this.Current.Flags = RtfParser.RtfParserState.StateFlags.None;
				this.Current.RunKind = RtfTextRunKind.None;
				this.Current.FontIndex = this.defaultFontIndex;
				this.Current.FontIndexCS = -1;
				this.Current.FontIndexAscii = -1;
				this.Current.FontIndexDbCh = -1;
				this.Current.FontIndexOther = this.defaultFontIndex;
				this.Current.FontSize = RtfParserBase.TwelvePointsInTwips;
				this.Current.FontSizeCS = 0;
				this.Current.FontSizeOther = 0;
				this.Current.CodePage = 0;
				this.Current.TextMapping = TextMapping.Unicode;
				this.Current.Language = ((this.defaultLanguageFE != -1) ? this.defaultLanguageFE : this.defaultLanguage);
				this.Current.CharRep = RtfSupport.CharRep.DEFAULT_INDEX;
			}

			public void Push()
			{
				this.Level++;
				this.Current.Depth = this.Current.Depth + 1;
				if (this.Current.Depth == 32767)
				{
					this.PushReally();
				}
			}

			public void SetDirty()
			{
				if (this.Current.Depth > 1)
				{
					this.PushReally();
				}
			}

			public void Pop()
			{
				if (this.Level > 1)
				{
					this.Current.Depth = this.Current.Depth - 1;
					if (this.Current.Depth == 0 && this.stackTop != 0)
					{
						this.Current = this.stack[--this.stackTop];
					}
					this.Level--;
				}
			}

			private void PushReally()
			{
				if (this.Level >= 4096)
				{
					throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
				}
				if (this.stack == null || this.stackTop == this.stack.Length)
				{
					RtfParser.RtfParserState.State[] destinationArray;
					if (this.stack != null)
					{
						destinationArray = new RtfParser.RtfParserState.State[this.stackTop * 2];
						Array.Copy(this.stack, 0, destinationArray, 0, this.stackTop);
					}
					else
					{
						destinationArray = new RtfParser.RtfParserState.State[8];
					}
					this.stack = destinationArray;
				}
				this.stack[this.stackTop] = this.Current;
				RtfParser.RtfParserState.State[] array = this.stack;
				int num = this.stackTop;
				array[num].Depth = array[num].Depth - 1;
				this.stackTop++;
				this.Current.Depth = 1;
			}

			public int Level;

			public RtfParser.RtfParserState.State Current;

			private RtfParser.RtfParserState.State[] stack;

			private int stackTop;

			private bool codePageFixed;

			private short defaultFontIndex;

			private short defaultLanguage;

			private short defaultLanguageFE;

			private RtfSupport.CharRep charRepBiDi;

			private bool detectSingleChpRtl;

			private bool detectAssocRtl;

			private bool detectAssocSA;

			private bool inAssocRtl;

			private bool inAssocSA;

			internal enum StateFlagsIndex : byte
			{
				BiDi,
				ComplexScript,
				Bold,
				BoldCS,
				BoldOther,
				Italic,
				ItalicCS,
				ItalicOther
			}

			[Flags]
			internal enum StateFlags : byte
			{
				None = 0,
				BiDi = 1,
				ComplexScript = 2,
				Bold = 4,
				BoldCS = 8,
				BoldOther = 16,
				Italic = 32,
				ItalicCS = 64,
				ItalicOther = 128
			}

			public struct State
			{
				public short Depth;

				public RtfParser.RtfParserState.StateFlags Flags;

				public RtfParser.RtfParserDestination Dest;

				public byte BytesSkipForUnicodeEscape;

				public RtfSupport.CharRep CharRep;

				public ushort CodePage;

				public TextMapping TextMapping;

				public short Language;

				public short FontIndex;

				public short FontIndexAscii;

				public short FontIndexDbCh;

				public short FontIndexCS;

				public short FontIndexOther;

				public short FontSize;

				public short FontSizeCS;

				public short FontSizeOther;

				public RtfTextRunKind RunKind;
			}
		}

		internal struct RtfParserFont
		{
			public void Initialize(short handle, ushort cpid)
			{
				this.Handle = handle;
				this.CodePage = cpid;
				this.CharRep = RtfSupport.CharRep.DEFAULT_INDEX;
				this.Family = RtfFontFamily.Default;
				this.Pitch = RtfFontPitch.Default;
				this.TextMapping = TextMapping.Unicode;
			}

			public short Handle;

			public ushort CodePage;

			public RtfSupport.CharRep CharRep;

			public RtfFontFamily Family;

			public RtfFontPitch Pitch;

			public TextMapping TextMapping;
		}
	}
}
