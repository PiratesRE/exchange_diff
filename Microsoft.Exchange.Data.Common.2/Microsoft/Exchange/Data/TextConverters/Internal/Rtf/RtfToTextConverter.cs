using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;
using Microsoft.Exchange.Data.TextConverters.Internal.Text;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal class RtfToTextConverter : IProducerConsumer, IDisposable
	{
		public RtfToTextConverter(RtfParser parser, TextOutput output, Injection injection, bool treatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
		{
			this.treatNbspAsBreakable = treatNbspAsBreakable;
			this.output = output;
			this.parser = parser;
			this.injection = injection;
			this.state = new RtfToTextConverter.RtfState(128);
		}

		public void Run()
		{
			if (this.endOfFile)
			{
				return;
			}
			RtfTokenId tokenId = this.parser.Parse();
			this.Process(tokenId);
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
			if (this.parser != null && this.parser is IDisposable)
			{
				((IDisposable)this.parser).Dispose();
			}
			if (this.output != null && this.output != null)
			{
				((IDisposable)this.output).Dispose();
			}
			this.parser = null;
			this.output = null;
			GC.SuppressFinalize(this);
		}

		private void Process(RtfTokenId tokenId)
		{
			if (!this.started)
			{
				this.output.OpenDocument();
				if (this.injection != null && this.injection.HaveHead)
				{
					this.injection.Inject(true, this.output);
				}
				this.started = true;
			}
			switch (tokenId)
			{
			case RtfTokenId.None:
			case (RtfTokenId)6:
				break;
			case RtfTokenId.EndOfFile:
				this.ProcessEOF();
				break;
			case RtfTokenId.Begin:
				this.ProcessBeginGroup();
				return;
			case RtfTokenId.End:
				this.ProcessEndGroup();
				return;
			case RtfTokenId.Binary:
				this.ProcessBinary(this.parser.Token);
				return;
			case RtfTokenId.Keywords:
				this.ProcessKeywords(this.parser.Token);
				return;
			case RtfTokenId.Text:
				this.ProcessText(this.parser.Token);
				return;
			default:
				return;
			}
		}

		private void ProcessBeginGroup()
		{
			if (this.state.SkipLevel != 0)
			{
				this.state.Level = this.state.Level + 1;
				return;
			}
			this.state.Push();
			this.firstKeyword = true;
			this.ignorableDestination = false;
		}

		private void ProcessEndGroup()
		{
			if (this.state.SkipLevel != 0)
			{
				if (this.state.Level != this.state.SkipLevel)
				{
					this.state.Level = this.state.Level - 1;
					return;
				}
				this.state.SkipLevel = 0;
			}
			this.firstKeyword = false;
			if (this.state.Level > 0)
			{
				this.EndGroup();
				this.state.Pop();
			}
		}

		private void ProcessKeywords(RtfToken token)
		{
			if (this.state.SkipLevel != 0 && this.state.Level >= this.state.SkipLevel)
			{
				return;
			}
			foreach (RtfKeyword rtfKeyword in token.Keywords)
			{
				if (this.firstKeyword)
				{
					if (rtfKeyword.Id == 1)
					{
						this.ignorableDestination = true;
						continue;
					}
					this.firstKeyword = false;
					short id = rtfKeyword.Id;
					if (id <= 135)
					{
						if (id <= 29)
						{
							if (id != 15 && id != 24)
							{
								if (id != 29)
								{
									goto IL_2EB;
								}
								if (this.state.Destination == RtfDestination.RTF || this.state.Destination == RtfDestination.FieldResult)
								{
									this.state.Destination = RtfDestination.Field;
								}
								break;
							}
						}
						else if (id <= 50)
						{
							if (id != 43)
							{
								if (id != 50)
								{
									goto IL_2EB;
								}
								if (this.state.Destination == RtfDestination.RTF || this.state.Destination == RtfDestination.FieldResult)
								{
									this.state.Destination = RtfDestination.Picture;
									this.pictureWidth = 0;
									this.pictureHeight = 0;
									goto IL_303;
								}
								this.state.SkipGroup();
								break;
							}
							else
							{
								if (this.state.Destination == RtfDestination.Picture && this.includePictureField)
								{
									this.state.Destination = RtfDestination.PictureProperties;
									continue;
								}
								this.state.SkipGroup();
								break;
							}
						}
						else if (id != 124)
						{
							if (id != 135)
							{
								goto IL_2EB;
							}
							if (this.state.Destination == RtfDestination.PictureProperties)
							{
								this.state.Destination = RtfDestination.ShapeName;
								continue;
							}
							continue;
						}
						else
						{
							if (this.state.Destination == RtfDestination.PictureProperties)
							{
								this.state.Destination = RtfDestination.ShapeValue;
								continue;
							}
							continue;
						}
					}
					else if (id <= 246)
					{
						if (id != 175)
						{
							if (id != 201 && id != 246)
							{
								goto IL_2EB;
							}
						}
						else
						{
							if (this.state.Destination == RtfDestination.RTF)
							{
								this.state.SkipGroup();
								break;
							}
							continue;
						}
					}
					else if (id <= 269)
					{
						if (id != 252)
						{
							if (id != 269)
							{
								goto IL_2EB;
							}
							if (this.state.Destination == RtfDestination.Field && !this.skipFieldResult)
							{
								this.state.Destination = RtfDestination.FieldResult;
								continue;
							}
							this.skipFieldResult = false;
							this.state.SkipGroup();
							break;
						}
						else
						{
							if (this.state.Destination == RtfDestination.RTF)
							{
								this.state.SkipGroup();
								break;
							}
							continue;
						}
					}
					else if (id != 306)
					{
						if (id != 315)
						{
							goto IL_2EB;
						}
					}
					else
					{
						if (this.state.Destination == RtfDestination.Field)
						{
							this.state.Destination = RtfDestination.FieldInstruction;
							continue;
						}
						this.state.SkipGroup();
						break;
					}
					this.state.SkipGroup();
					break;
					IL_2EB:
					if (this.ignorableDestination)
					{
						this.state.SkipGroup();
						break;
					}
				}
				IL_303:
				short id2 = rtfKeyword.Id;
				if (id2 <= 126)
				{
					if (id2 <= 68)
					{
						if (id2 <= 40)
						{
							if (id2 != 6)
							{
								if (id2 != 40)
								{
									continue;
								}
								goto IL_466;
							}
							else
							{
								if (this.state.Destination == RtfDestination.Picture)
								{
									this.pictureHeight = rtfKeyword.Value;
									continue;
								}
								continue;
							}
						}
						else if (id2 != 48)
						{
							if (id2 != 68)
							{
								continue;
							}
							goto IL_466;
						}
						IL_4A5:
						this.output.OutputNewLine();
						continue;
						IL_466:
						if (this.hyperLinkActive)
						{
							if (!this.urlCompareSink.IsMatch)
							{
								this.output.CloseAnchor();
							}
							else
							{
								this.output.CancelAnchor();
							}
							this.hyperLinkActive = false;
							this.urlCompareSink.Reset();
							goto IL_4A5;
						}
						goto IL_4A5;
					}
					else if (id2 <= 95)
					{
						if (id2 != 71)
						{
							if (id2 != 95)
							{
							}
						}
						else if (rtfKeyword.Value > 0 && rtfKeyword.Value <= 32767)
						{
						}
					}
					else if (id2 == 119 || id2 == 126)
					{
						this.output.OutputTabulation(1);
					}
				}
				else if (id2 <= 170)
				{
					if (id2 <= 142)
					{
						switch (id2)
						{
						case 134:
						case 135:
						case 137:
							break;
						case 136:
							this.state.IsInvisible = (rtfKeyword.Value != 0);
							break;
						default:
							if (id2 == 142)
							{
								if (rtfKeyword.Value >= 75 && this.output.LineEmpty && this.output.RenderingPosition() != 0)
								{
									this.output.CloseParagraph();
								}
							}
							break;
						}
					}
					else if (id2 != 154)
					{
						if (id2 != 170)
						{
						}
					}
					else if (this.state.Destination == RtfDestination.Picture)
					{
						this.pictureWidth = rtfKeyword.Value;
					}
				}
				else if (id2 <= 206)
				{
					if (id2 != 184)
					{
						if (id2 == 206)
						{
							this.lineIndent = rtfKeyword.Value;
						}
					}
					else
					{
						this.lineIndent = 0;
					}
				}
				else if (id2 != 222)
				{
					if (id2 != 284)
					{
						if (id2 != 287)
						{
						}
					}
					else
					{
						this.lineIndent += rtfKeyword.Value;
					}
				}
			}
		}

		private void ProcessText(RtfToken token)
		{
			if (this.state.SkipLevel != 0 && this.state.Level >= this.state.SkipLevel)
			{
				return;
			}
			RtfDestination destination = this.state.Destination;
			if (destination != RtfDestination.RTF)
			{
				switch (destination)
				{
				case RtfDestination.Field:
					break;
				case RtfDestination.FieldResult:
					if (this.hyperLinkActive && this.urlCompareSink.IsActive)
					{
						token.Text.WriteTo(this.urlCompareSink);
						token.Text.Rewind();
						goto IL_107;
					}
					goto IL_107;
				case RtfDestination.FieldInstruction:
					this.firstKeyword = false;
					this.scratch.AppendRtfTokenText(token, 4096);
					return;
				default:
					switch (destination)
					{
					case RtfDestination.Picture:
					case RtfDestination.PictureProperties:
						break;
					case RtfDestination.ShapeName:
						this.firstKeyword = false;
						this.scratch.AppendRtfTokenText(token, 128);
						return;
					case RtfDestination.ShapeValue:
						this.firstKeyword = false;
						if (this.descriptionProperty)
						{
							this.scratch.AppendRtfTokenText(token, 4096);
							return;
						}
						return;
					default:
						this.firstKeyword = false;
						return;
					}
					break;
				}
				this.firstKeyword = false;
				return;
			}
			IL_107:
			if (this.state.IsInvisible)
			{
				return;
			}
			if (this.output.LineEmpty && this.lineIndent >= 120 && this.lineIndent < 7200)
			{
				this.output.OutputSpace(this.lineIndent / 120);
			}
			token.TextElements.OutputTextElements(this.output, this.treatNbspAsBreakable);
		}

		private void ProcessBinary(RtfToken token)
		{
		}

		private void ProcessEOF()
		{
			this.output.CloseParagraph();
			if (this.injection != null && this.injection.HaveTail)
			{
				this.injection.Inject(false, this.output);
			}
			this.output.Flush();
			this.endOfFile = true;
		}

		private void EndGroup()
		{
			RtfDestination destination = this.state.Destination;
			switch (destination)
			{
			case RtfDestination.Field:
				if (this.state.ParentDestination != RtfDestination.Field)
				{
					this.skipFieldResult = false;
					if (this.includePictureField)
					{
						PropertyValue propertyValue = new PropertyValue(LengthUnits.Twips, this.pictureHeight);
						PropertyValue propertyValue2 = new PropertyValue(LengthUnits.Twips, this.pictureWidth);
						this.output.OutputImage(this.imageUrl, this.imageAltText, propertyValue2.PixelsInteger, propertyValue.PixelsInteger);
						this.includePictureField = false;
						this.imageUrl = null;
						this.imageAltText = null;
						return;
					}
					if (this.hyperLinkActive)
					{
						if (!this.urlCompareSink.IsMatch)
						{
							this.output.CloseAnchor();
						}
						else
						{
							this.output.CancelAnchor();
						}
						this.hyperLinkActive = false;
					}
				}
				break;
			case RtfDestination.FieldResult:
				break;
			case RtfDestination.FieldInstruction:
				if (this.state.ParentDestination == RtfDestination.Field)
				{
					bool flag;
					BufferString bufferString;
					TextMapping textMapping;
					char ucs32Literal;
					short num;
					if (RtfSupport.IsHyperlinkField(ref this.scratch, out flag, out bufferString))
					{
						if (!flag)
						{
							if (this.hyperLinkActive)
							{
								if (!this.urlCompareSink.IsMatch)
								{
									this.output.CloseAnchor();
								}
								else
								{
									this.output.CancelAnchor();
								}
								this.hyperLinkActive = false;
								this.urlCompareSink.Reset();
							}
							bufferString.TrimWhitespace();
							if (bufferString.Length != 0)
							{
								string text = bufferString.ToString();
								this.output.OpenAnchor(text);
								this.hyperLinkActive = true;
								if (this.urlCompareSink == null)
								{
									this.urlCompareSink = new UrlCompareSink();
								}
								this.urlCompareSink.Initialize(text);
							}
						}
					}
					else if (RtfSupport.IsIncludePictureField(ref this.scratch, out bufferString))
					{
						bufferString.TrimWhitespace();
						if (bufferString.Length != 0)
						{
							this.includePictureField = true;
							bufferString.TrimWhitespace();
							if (bufferString.Length != 0)
							{
								this.imageUrl = bufferString.ToString();
							}
							this.pictureWidth = 0;
							this.pictureHeight = 0;
						}
					}
					else if (RtfSupport.IsSymbolField(ref this.scratch, out textMapping, out ucs32Literal, out num))
					{
						if (this.output.LineEmpty && this.lineIndent >= 120 && this.lineIndent < 7200)
						{
							this.output.OutputSpace(this.lineIndent / 120);
						}
						this.output.OutputNonspace((int)ucs32Literal, textMapping);
						this.skipFieldResult = true;
					}
					this.scratch.Reset();
					return;
				}
				break;
			default:
				switch (destination)
				{
				case RtfDestination.Picture:
					if (this.state.ParentDestination != RtfDestination.Picture && !this.includePictureField)
					{
						PropertyValue propertyValue3 = new PropertyValue(LengthUnits.Twips, this.pictureHeight);
						PropertyValue propertyValue4 = new PropertyValue(LengthUnits.Twips, this.pictureWidth);
						this.output.OutputImage(null, null, propertyValue4.PixelsInteger, propertyValue3.PixelsInteger);
						return;
					}
					break;
				case RtfDestination.PictureProperties:
					break;
				case RtfDestination.ShapeName:
					if (this.state.ParentDestination != RtfDestination.ShapeName)
					{
						BufferString bufferString2 = this.scratch.BufferString;
						bufferString2.TrimWhitespace();
						this.descriptionProperty = bufferString2.EqualsToLowerCaseStringIgnoreCase("wzdescription");
						this.scratch.Reset();
						return;
					}
					break;
				case RtfDestination.ShapeValue:
					if (this.state.ParentDestination != RtfDestination.ShapeValue && this.descriptionProperty)
					{
						BufferString bufferString3 = this.scratch.BufferString;
						bufferString3.TrimWhitespace();
						if (bufferString3.Length != 0)
						{
							this.imageAltText = bufferString3.ToString();
						}
						this.scratch.Reset();
						return;
					}
					break;
				default:
					return;
				}
				break;
			}
		}

		private bool treatNbspAsBreakable;

		private RtfParser parser;

		private bool endOfFile;

		private TextOutput output;

		private bool firstKeyword;

		private bool ignorableDestination;

		private RtfToTextConverter.RtfState state;

		private bool hyperLinkActive;

		private bool skipFieldResult;

		private bool includePictureField;

		private bool descriptionProperty;

		private string imageUrl;

		private string imageAltText;

		private int pictureWidth;

		private int pictureHeight;

		private int lineIndent;

		private Injection injection;

		private ScratchBuffer scratch;

		private UrlCompareSink urlCompareSink;

		private bool started;

		internal struct RtfState
		{
			public RtfState(int initialStackSize)
			{
				this.Level = 0;
				this.SkipLevel = 0;
				this.Current = default(RtfToTextConverter.RtfState.State);
				this.stack = new RtfToTextConverter.RtfState.State[initialStackSize];
				this.stackTop = 0;
				this.Current.LevelsDeep = 1;
				this.Current.Dest = RtfDestination.RTF;
			}

			public RtfDestination Destination
			{
				get
				{
					return this.Current.Dest;
				}
				set
				{
					if (value != this.Current.Dest)
					{
						this.SetDirty();
						this.Current.Dest = value;
					}
				}
			}

			public RtfDestination ParentDestination
			{
				get
				{
					if (this.Current.LevelsDeep <= 1 && this.Level != 0)
					{
						return this.stack[this.stackTop - 1].Dest;
					}
					return this.Current.Dest;
				}
			}

			public bool IsInvisible
			{
				get
				{
					return this.Current.Invisible;
				}
				set
				{
					if (value != this.Current.Invisible)
					{
						this.SetDirty();
						this.Current.Invisible = value;
					}
				}
			}

			public void Push()
			{
				this.Level++;
				this.Current.LevelsDeep = this.Current.LevelsDeep + 1;
				if (this.Current.LevelsDeep == 32767)
				{
					this.PushReally();
				}
			}

			public void SetDirty()
			{
				if (this.Current.LevelsDeep > 1)
				{
					this.PushReally();
				}
			}

			public void Pop()
			{
				if (this.Level > 1)
				{
					this.Current.LevelsDeep = this.Current.LevelsDeep - 1;
					if (this.Current.LevelsDeep == 0)
					{
						this.Current = this.stack[--this.stackTop];
					}
					this.Level--;
				}
			}

			public void SkipGroup()
			{
				this.SkipLevel = this.Level;
			}

			private void PushReally()
			{
				if (this.Level >= 4096)
				{
					throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
				}
				if (this.stackTop == this.stack.Length)
				{
					RtfToTextConverter.RtfState.State[] destinationArray = new RtfToTextConverter.RtfState.State[this.stackTop * 2];
					Array.Copy(this.stack, 0, destinationArray, 0, this.stackTop);
					this.stack = destinationArray;
				}
				this.stack[this.stackTop] = this.Current;
				RtfToTextConverter.RtfState.State[] array = this.stack;
				int num = this.stackTop;
				array[num].LevelsDeep = array[num].LevelsDeep - 1;
				this.stackTop++;
				this.Current.LevelsDeep = 1;
			}

			public int Level;

			public int SkipLevel;

			public RtfToTextConverter.RtfState.State Current;

			private RtfToTextConverter.RtfState.State[] stack;

			private int stackTop;

			public struct State
			{
				public short LevelsDeep;

				public RtfDestination Dest;

				public bool Invisible;
			}
		}
	}
}
