using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal class RtfToRtfConverter : IProducerConsumer, IDisposable
	{
		public RtfToRtfConverter(RtfParser parser, Stream destination, bool push, Injection injection, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
		{
			this.output = new RtfOutput(destination, push, false);
			this.parser = parser;
			this.injection = injection;
			this.state = new RtfToRtfConverter.RtfState(128);
			if (this.injection != null)
			{
				this.injection.CompileForRtf(this.output);
			}
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
			this.scratch.DisposeBuffer();
			GC.SuppressFinalize(this);
		}

		public void Run()
		{
			if (!this.endOfFile)
			{
				RtfTokenId rtfTokenId = this.parser.Parse();
				if (rtfTokenId != RtfTokenId.None)
				{
					this.Process(rtfTokenId);
				}
			}
		}

		public bool Flush()
		{
			this.Run();
			return this.endOfFile;
		}

		private void Process(RtfTokenId tokenId)
		{
			switch (tokenId)
			{
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
			case (RtfTokenId)6:
				break;
			case RtfTokenId.Text:
				this.ProcessText(this.parser.Token);
				return;
			default:
				return;
			}
		}

		private void ProcessBeginGroup()
		{
			if (this.state.Level >= 0)
			{
				if (this.state.SkipLevel != 0 || this.state.Level < 0 || this.firstKeyword)
				{
					this.output.WriteControlText("{", false);
				}
				this.state.Push();
				this.firstKeyword = true;
			}
		}

		private void ProcessEndGroup()
		{
			if (this.state.Level > 0)
			{
				if (this.state.SkipLevel != 0 && this.state.Level == this.state.SkipLevel)
				{
					this.state.SkipLevel = 0;
				}
				this.firstKeyword = false;
				this.EndGroup();
				this.state.Pop();
				if (this.state.Level == 0)
				{
					this.state.Level = this.state.Level - 1;
					if (this.injection != null && this.injection.HaveTail && !this.injection.TailDone)
					{
						if (this.lineLength != 0)
						{
							this.output.WriteControlText("\\par\r\n", false);
							this.output.RtfLineLength = 0;
							this.lineBreaks = 1;
						}
						if (this.output.RtfLineLength != 0)
						{
							this.output.WriteControlText("\r\n", false);
							this.output.RtfLineLength = 0;
						}
						this.injection.InjectRtf(false, this.lineBreaks <= 1);
					}
				}
				this.output.WriteControlText("}", false);
			}
		}

		private void ProcessKeywords(RtfToken token)
		{
			if (this.state.Level >= 0)
			{
				foreach (RtfKeyword kw in token.Keywords)
				{
					if (this.state.SkipLevel != 0)
					{
						this.WriteKeyword(kw);
					}
					else
					{
						if (this.firstKeyword)
						{
							this.firstKeyword = false;
							if (kw.Id == 1)
							{
								this.state.SkipGroup();
							}
							else
							{
								short id = kw.Id;
								if (id <= 241)
								{
									if (id <= 175)
									{
										if (id != 15 && id != 24)
										{
											if (id != 175)
											{
												goto IL_25B;
											}
											if (this.state.Destination == RtfDestination.RTF)
											{
												this.state.FontIndex = -1;
												this.state.Destination = RtfDestination.FontTable;
												goto IL_2B4;
											}
											goto IL_2B4;
										}
									}
									else if (id <= 210)
									{
										if (id != 201)
										{
											if (id != 210)
											{
												goto IL_25B;
											}
											if (this.state.Destination == RtfDestination.FontTable && this.state.FontIndex >= 0)
											{
												this.state.Destination = RtfDestination.RealFontName;
												goto IL_2B4;
											}
											goto IL_2B4;
										}
									}
									else if (id != 230 && id != 241)
									{
										goto IL_25B;
									}
								}
								else if (id <= 258)
								{
									if (id != 246)
									{
										switch (id)
										{
										case 252:
											if (this.state.Destination == RtfDestination.RTF)
											{
												this.state.Destination = RtfDestination.ColorTable;
												goto IL_2B4;
											}
											goto IL_2B4;
										case 253:
											break;
										default:
											if (id != 258)
											{
												goto IL_25B;
											}
											break;
										}
									}
								}
								else if (id <= 273)
								{
									switch (id)
									{
									case 267:
										this.output.WriteControlText("{", false);
										this.WriteKeyword(kw);
										continue;
									case 268:
										if (this.state.Destination == RtfDestination.FontTable && this.state.FontIndex >= 0)
										{
											this.state.Destination = RtfDestination.AltFontName;
											goto IL_2B4;
										}
										goto IL_2B4;
									default:
										if (id != 273)
										{
											goto IL_25B;
										}
										break;
									}
								}
								else if (id != 277)
								{
									switch (id)
									{
									case 315:
									case 316:
										break;
									default:
										goto IL_25B;
									}
								}
								this.state.SkipGroup();
								goto IL_2B4;
								IL_25B:
								if (this.state.Destination == RtfDestination.RTF && this.injection != null && this.injection.HaveHead && !this.injection.HeadDone)
								{
									this.output.WriteControlText("\r\n", false);
									this.output.RtfLineLength = 0;
									this.injection.InjectRtf(true, false);
								}
							}
							IL_2B4:
							this.output.WriteControlText("{", false);
						}
						short id2 = kw.Id;
						if (id2 <= 148)
						{
							if (id2 <= 76)
							{
								if (id2 != 48 && id2 != 68)
								{
									if (id2 == 76)
									{
										goto IL_584;
									}
								}
								else
								{
									if (this.state.Destination == RtfDestination.RTF)
									{
										this.lineLength = 0;
										this.lineBreaks++;
										goto IL_51D;
									}
									goto IL_51D;
								}
							}
							else if (id2 <= 116)
							{
								if (id2 == 88)
								{
									goto IL_480;
								}
								switch (id2)
								{
								case 109:
								case 112:
								case 115:
								case 116:
									goto IL_584;
								case 113:
									if (this.state.Destination == RtfDestination.FontTable || this.state.Destination == RtfDestination.AltFontName || this.state.Destination == RtfDestination.RealFontName)
									{
										if (this.state.FontIndex >= 0)
										{
											this.state.FontIndex = -1;
										}
										short num = this.parser.FontIndex((short)kw.Value);
										if (num >= 0)
										{
											this.state.FontIndex = num;
											if (this.parser.FontHandle(num) > this.maxFontHandle)
											{
												this.maxFontHandle = this.parser.FontHandle(num);
											}
										}
									}
									break;
								}
							}
							else
							{
								if (id2 == 126)
								{
									goto IL_51D;
								}
								if (id2 == 148)
								{
									goto IL_480;
								}
							}
						}
						else if (id2 <= 206)
						{
							if (id2 <= 184)
							{
								if (id2 == 170)
								{
									goto IL_584;
								}
								if (id2 == 184)
								{
									goto IL_51D;
								}
							}
							else
							{
								if (id2 == 200)
								{
									goto IL_51D;
								}
								if (id2 == 206)
								{
									goto IL_584;
								}
							}
						}
						else if (id2 <= 284)
						{
							if (id2 == 265 || id2 == 284)
							{
								goto IL_584;
							}
						}
						else
						{
							if (id2 == 309)
							{
								goto IL_480;
							}
							if (id2 == 331)
							{
								goto IL_584;
							}
						}
						IL_5DD:
						this.WriteKeyword(kw);
						continue;
						IL_480:
						if (this.state.Destination == RtfDestination.ColorTable)
						{
							this.color &= ~(255 << (int)(RTFData.keywords[(int)kw.Id].idx * 8));
							this.color |= (kw.Value & 255) << (int)(RTFData.keywords[(int)kw.Id].idx * 8);
							goto IL_5DD;
						}
						goto IL_5DD;
						IL_51D:
						if (this.state.Destination == RtfDestination.RTF && this.injection != null && this.injection.HaveHead && !this.injection.HeadDone)
						{
							this.output.WriteControlText("\r\n", false);
							this.output.RtfLineLength = 0;
							this.injection.InjectRtf(true, false);
							goto IL_5DD;
						}
						goto IL_5DD;
						IL_584:
						if (this.state.Destination == RtfDestination.RTF && this.injection != null && this.injection.HaveHead && !this.injection.HeadDone)
						{
							this.output.WriteControlText("\r\n", false);
							this.output.RtfLineLength = 0;
							this.injection.InjectRtf(true, false);
							goto IL_5DD;
						}
						goto IL_5DD;
					}
				}
			}
		}

		private void ProcessText(RtfToken token)
		{
			if (this.state.Level >= 0)
			{
				if (this.state.Destination == RtfDestination.RTF && this.state.SkipLevel == 0)
				{
					bool flag = true;
					foreach (RtfRun rtfRun in token.Runs)
					{
						if (rtfRun.Kind != RtfRunKind.Ignore)
						{
							flag = false;
							break;
						}
					}
					token.Runs.Rewind();
					if (!flag)
					{
						if (this.injection != null && this.injection.HaveHead && !this.injection.HeadDone)
						{
							this.output.WriteControlText("\r\n", false);
							this.output.RtfLineLength = 0;
							this.injection.InjectRtf(true, false);
						}
						this.lineLength++;
						this.lineBreaks = 0;
					}
				}
				if (this.firstKeyword && this.state.SkipLevel == 0)
				{
					this.output.WriteControlText("{", false);
					this.firstKeyword = false;
				}
				this.WriteToken(token);
				token.Runs.Rewind();
				switch (this.state.Destination)
				{
				case RtfDestination.FontTable:
				case RtfDestination.RealFontName:
				case RtfDestination.AltFontName:
					break;
				case RtfDestination.ColorTable:
					this.scratch.Reset();
					while (this.scratch.AppendRtfTokenText(token, 256))
					{
						for (int i = 0; i < this.scratch.Length; i++)
						{
							if (this.scratch[i] == ';')
							{
								this.colorsCount++;
								this.color = 0;
							}
						}
						this.scratch.Reset();
					}
					break;
				default:
					return;
				}
			}
		}

		private void ProcessBinary(RtfToken token)
		{
			if (this.state.Level >= 0)
			{
				this.WriteToken(token);
			}
		}

		private void ProcessEOF()
		{
			while (this.state.Level > 0)
			{
				this.ProcessEndGroup();
			}
			this.output.Flush();
			this.endOfFile = true;
		}

		private void EndGroup()
		{
			RtfDestination destination = this.state.Destination;
			if (destination != RtfDestination.FontTable)
			{
				if (destination != RtfDestination.ColorTable)
				{
					return;
				}
				if (this.state.ParentDestination != RtfDestination.ColorTable && this.injection != null)
				{
					if (this.color != 0)
					{
						this.colorsCount++;
						this.output.WriteControlText(";", false);
					}
					this.output.WriteControlText("\r\n", false);
					this.output.RtfLineLength = 0;
					this.injection.InjectRtfColors(this.colorsCount);
				}
			}
			else if (this.state.ParentDestination != RtfDestination.FontTable)
			{
				if (this.injection != null)
				{
					this.output.WriteControlText("\r\n", false);
					this.output.RtfLineLength = 0;
					this.injection.InjectRtfFonts((int)(this.maxFontHandle + 1));
					return;
				}
			}
			else if (this.state.FontIndex >= 0)
			{
				this.state.ParentFontIndex = this.state.FontIndex;
				return;
			}
		}

		private void WriteToken(RtfToken token)
		{
			foreach (RtfRun rtfRun in token.Runs)
			{
				this.output.WriteBinary(rtfRun.Buffer, rtfRun.Offset, rtfRun.Length);
			}
		}

		private void WriteKeyword(RtfKeyword kw)
		{
			this.output.WriteBinary(kw.Buffer, kw.Offset, kw.Length);
		}

		private RtfOutput output;

		private bool endOfFile;

		private RtfParser parser;

		private bool firstKeyword;

		private RtfToRtfConverter.RtfState state;

		private short maxFontHandle;

		private int colorsCount;

		private int lineLength;

		private int lineBreaks;

		private Injection injection;

		private int color;

		private ScratchBuffer scratch;

		internal struct RtfState
		{
			public RtfState(int initialStackSize)
			{
				this.Level = 0;
				this.SkipLevel = 0;
				this.Current = default(RtfToRtfConverter.RtfState.State);
				this.stack = new RtfToRtfConverter.RtfState.State[initialStackSize];
				this.stackTop = 0;
				this.Current.Dest = RtfDestination.RTF;
				this.Current.FontIndex = 0;
			}

			public RtfDestination Destination
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

			public RtfDestination ParentDestination
			{
				get
				{
					if (this.Current.Depth <= 1 && this.stackTop != 0)
					{
						return this.stack[this.stackTop - 1].Dest;
					}
					return this.Current.Dest;
				}
			}

			public short ParentFontIndex
			{
				get
				{
					if (this.Current.Depth <= 1 && this.stackTop != 0)
					{
						return this.stack[this.stackTop - 1].FontIndex;
					}
					return this.Current.FontIndex;
				}
				set
				{
					this.Current.FontIndex = value;
					if (this.stackTop > 0)
					{
						this.stack[this.stackTop - 1].FontIndex = value;
					}
				}
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
				if (this.Level > 0)
				{
					this.Current.Depth = this.Current.Depth - 1;
					if (this.Current.Depth == 0 && this.stackTop != 0)
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
				if (this.stackTop >= 4096)
				{
					throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
				}
				if (this.stack == null || this.stackTop == this.stack.Length)
				{
					RtfToRtfConverter.RtfState.State[] destinationArray;
					if (this.stack != null)
					{
						destinationArray = new RtfToRtfConverter.RtfState.State[this.stack.Length * 2];
						Array.Copy(this.stack, 0, destinationArray, 0, this.stackTop);
					}
					else
					{
						destinationArray = new RtfToRtfConverter.RtfState.State[4];
					}
					this.stack = destinationArray;
				}
				this.stack[this.stackTop] = this.Current;
				RtfToRtfConverter.RtfState.State[] array = this.stack;
				int num = this.stackTop;
				array[num].Depth = array[num].Depth - 1;
				this.stackTop++;
				this.Current.Depth = 1;
			}

			public int Level;

			public int SkipLevel;

			public RtfToRtfConverter.RtfState.State Current;

			private RtfToRtfConverter.RtfState.State[] stack;

			private int stackTop;

			public struct State
			{
				public short Depth;

				public RtfDestination Dest;

				public short FontIndex;
			}
		}
	}
}
