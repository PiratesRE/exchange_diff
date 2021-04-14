using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal class HtmlInRtfExtractingInput : ConverterInput
	{
		public HtmlInRtfExtractingInput(RtfParser parser, int maxParseToken, bool testBoundaryConditions, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum) : base(null)
		{
			this.parser = parser;
			this.state = new HtmlInRtfExtractingInput.RtfState(16);
			this.maxTokenSize = (testBoundaryConditions ? 123 : maxParseToken);
			this.parseBuffer = new char[Math.Min((long)maxParseToken + 1L, 4096L)];
		}

		public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
		{
			if (this.parseBuffer.Length - this.parseEnd < 6 && !this.EnsureFreeSpace())
			{
				return true;
			}
			int num = this.parseEnd;
			if (this.incompleteToken != null)
			{
				if (this.incompleteToken.Id == RtfTokenId.Keywords)
				{
					this.ProcessKeywords(this.incompleteToken);
				}
				else
				{
					this.ProcessText(this.incompleteToken);
				}
			}
			while (!this.endOfFile && this.parseBuffer.Length - this.parseEnd >= 6)
			{
				RtfTokenId rtfTokenId = this.parser.Parse();
				if (rtfTokenId == RtfTokenId.None)
				{
					break;
				}
				this.Process(rtfTokenId);
			}
			buffer = this.parseBuffer;
			if (start != this.parseStart)
			{
				current = this.parseStart + (current - start);
				start = this.parseStart;
			}
			end = this.parseEnd;
			return end != num || this.endOfFile;
		}

		public override void ReportProcessed(int processedSize)
		{
			this.parseStart += processedSize;
		}

		public override int RemoveGap(int gapBegin, int gapEnd)
		{
			if (gapEnd == this.parseEnd)
			{
				this.parseEnd = gapBegin;
				this.parseBuffer[gapBegin] = '\0';
				return gapBegin;
			}
			Buffer.BlockCopy(this.parseBuffer, gapEnd, this.parseBuffer, gapBegin, this.parseEnd - gapEnd);
			this.parseEnd = gapBegin + (this.parseEnd - gapEnd);
			this.parseBuffer[this.parseEnd] = '\0';
			return this.parseEnd;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.parser != null && this.parser is IDisposable)
			{
				((IDisposable)this.parser).Dispose();
			}
			this.parser = null;
			base.Dispose(disposing);
		}

		private void Process(RtfTokenId tokenId)
		{
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
			this.state.Push();
			if (this.state.Skip)
			{
				return;
			}
			this.firstKeyword = true;
			this.ignorableDestination = false;
		}

		private void ProcessEndGroup()
		{
			if (this.state.Skip)
			{
				this.state.Pop();
				if (this.state.Skip)
				{
					return;
				}
			}
			this.firstKeyword = false;
			if (this.state.CanPop)
			{
				this.state.Pop();
			}
		}

		private void ProcessKeywords(RtfToken token)
		{
			if (this.state.Skip)
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
					if (id <= 210)
					{
						if (id <= 24)
						{
							if (id != 15 && id != 24)
							{
								goto IL_19D;
							}
						}
						else if (id != 50)
						{
							switch (id)
							{
							case 175:
								if (this.state.Destination == RtfDestination.RTF)
								{
									this.state.Destination = RtfDestination.FontTable;
									continue;
								}
								continue;
							case 176:
								goto IL_19D;
							case 177:
								this.state.Destination = RtfDestination.HtmlTagIndex;
								continue;
							default:
								if (id != 210)
								{
									goto IL_19D;
								}
								if (this.state.Destination == RtfDestination.FontTable)
								{
									this.state.Destination = RtfDestination.RealFontName;
									continue;
								}
								continue;
							}
						}
					}
					else if (id <= 252)
					{
						if (id != 246)
						{
							if (id != 252)
							{
								goto IL_19D;
							}
							if (this.state.Destination == RtfDestination.RTF)
							{
								this.state.Destination = RtfDestination.ColorTable;
								continue;
							}
							continue;
						}
					}
					else if (id != 268)
					{
						if (id != 315)
						{
							if (id != 319)
							{
								goto IL_19D;
							}
							this.state.Destination = RtfDestination.ListText;
							continue;
						}
					}
					else
					{
						if (this.state.Destination == RtfDestination.FontTable)
						{
							this.state.Destination = RtfDestination.AltFontName;
							continue;
						}
						continue;
					}
					this.state.PushSkipGroup();
					return;
					IL_19D:
					if (this.ignorableDestination)
					{
						this.state.PushSkipGroup();
						return;
					}
				}
				short id2 = rtfKeyword.Id;
				if (id2 <= 68)
				{
					if (id2 == 40 || id2 == 48 || id2 == 68)
					{
						if (!this.ignoreRtf && (this.state.Destination == RtfDestination.RTF || this.state.Destination == RtfDestination.HtmlTagIndex))
						{
							this.Output("\r\n");
						}
					}
				}
				else
				{
					if (id2 <= 119)
					{
						if (id2 == 83)
						{
							goto IL_27B;
						}
						if (id2 != 119)
						{
							goto IL_27B;
						}
					}
					else if (id2 != 126)
					{
						if (id2 != 153)
						{
							goto IL_27B;
						}
						this.ignoreRtf = (rtfKeyword.Value != 0);
						goto IL_27B;
					}
					if (!this.ignoreRtf && (this.state.Destination == RtfDestination.RTF || this.state.Destination == RtfDestination.HtmlTagIndex))
					{
						this.Output("\t");
					}
				}
				IL_27B:
				if (this.parseBuffer.Length - this.parseEnd >= 6)
				{
					continue;
				}
				this.incompleteToken = token;
				return;
			}
			this.incompleteToken = null;
		}

		private void ProcessText(RtfToken token)
		{
			if (this.state.Skip)
			{
				return;
			}
			RtfDestination destination = this.state.Destination;
			if (destination != RtfDestination.RTF)
			{
				if (destination != RtfDestination.HtmlTagIndex)
				{
					this.firstKeyword = false;
					return;
				}
			}
			else if (this.ignoreRtf)
			{
				return;
			}
			token.StripZeroBytes = true;
			int num = token.Text.Read(this.parseBuffer, this.parseEnd, this.parseBuffer.Length - this.parseEnd - 1);
			this.parseEnd += num;
			this.parseBuffer[this.parseEnd] = '\0';
			if (this.parseEnd == this.parseBuffer.Length - 1)
			{
				this.incompleteToken = token;
				return;
			}
			this.incompleteToken = null;
		}

		private void ProcessBinary(RtfToken token)
		{
		}

		private void ProcessEOF()
		{
			while (this.state.CanPop)
			{
				this.ProcessEndGroup();
			}
			this.endOfFile = true;
		}

		private void Output(string str)
		{
			str.CopyTo(0, this.parseBuffer, this.parseEnd, str.Length);
			this.parseEnd += str.Length;
			this.parseBuffer[this.parseEnd] = '\0';
		}

		private bool EnsureFreeSpace()
		{
			if (this.parseBuffer.Length - (this.parseEnd - this.parseStart) < 6 || (this.parseStart < 1 && (long)this.parseBuffer.Length < (long)this.maxTokenSize + 1L))
			{
				if ((long)this.parseBuffer.Length >= (long)this.maxTokenSize + 5L + 1L)
				{
					return false;
				}
				long num = (long)(this.parseBuffer.Length * 2);
				if (num > (long)this.maxTokenSize + 5L + 1L)
				{
					num = (long)this.maxTokenSize + 5L + 1L;
				}
				if (num > 2147483647L)
				{
					num = 2147483647L;
				}
				char[] dst = new char[(int)num];
				Buffer.BlockCopy(this.parseBuffer, this.parseStart * 2, dst, 0, (this.parseEnd - this.parseStart + 1) * 2);
				this.parseBuffer = dst;
				this.parseEnd -= this.parseStart;
				this.parseStart = 0;
			}
			else
			{
				Buffer.BlockCopy(this.parseBuffer, this.parseStart * 2, this.parseBuffer, 0, (this.parseEnd - this.parseStart + 1) * 2);
				this.parseEnd -= this.parseStart;
				this.parseStart = 0;
			}
			return true;
		}

		private RtfParser parser;

		private bool firstKeyword;

		private bool ignorableDestination;

		private HtmlInRtfExtractingInput.RtfState state;

		private bool ignoreRtf;

		private RtfToken incompleteToken;

		private char[] parseBuffer;

		private int parseStart;

		private int parseEnd;

		private struct RtfState
		{
			public RtfState(int initialStackSize)
			{
				this.level = 0;
				this.skipLevel = 0;
				this.current = default(HtmlInRtfExtractingInput.RtfState.State);
				this.stack = new HtmlInRtfExtractingInput.RtfState.State[initialStackSize];
				this.stackTop = 0;
				this.current.LevelsDeep = 1;
				this.current.Dest = RtfDestination.RTF;
			}

			public RtfDestination Destination
			{
				get
				{
					return this.current.Dest;
				}
				set
				{
					this.SetDirty();
					this.current.Dest = value;
				}
			}

			public bool Skip
			{
				get
				{
					return this.skipLevel != 0;
				}
			}

			public bool CanPop
			{
				get
				{
					return this.level > 1;
				}
			}

			public void Push()
			{
				this.level++;
				this.current.LevelsDeep = this.current.LevelsDeep + 1;
				if (this.current.LevelsDeep == 32767)
				{
					this.PushReally();
				}
			}

			public void PushSkipGroup()
			{
				this.skipLevel = this.level;
				this.Push();
			}

			public void Pop()
			{
				if (this.level > 1)
				{
					if ((this.current.LevelsDeep = this.current.LevelsDeep - 1) == 0 && this.stackTop != 0)
					{
						this.current = this.stack[--this.stackTop];
					}
					if (--this.level == this.skipLevel)
					{
						this.skipLevel = 0;
					}
				}
			}

			public void SetDirty()
			{
				if (this.current.LevelsDeep > 1)
				{
					this.PushReally();
				}
			}

			private void PushReally()
			{
				if (this.stackTop >= 4096)
				{
					throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
				}
				if (this.stackTop == this.stack.Length)
				{
					HtmlInRtfExtractingInput.RtfState.State[] destinationArray = new HtmlInRtfExtractingInput.RtfState.State[this.stackTop * 2];
					Array.Copy(this.stack, 0, destinationArray, 0, this.stackTop);
					this.stack = destinationArray;
				}
				this.stack[this.stackTop] = this.current;
				HtmlInRtfExtractingInput.RtfState.State[] array = this.stack;
				int num = this.stackTop;
				array[num].LevelsDeep = array[num].LevelsDeep - 1;
				this.stackTop++;
				this.current.LevelsDeep = 1;
			}

			private int level;

			private int skipLevel;

			private HtmlInRtfExtractingInput.RtfState.State current;

			private HtmlInRtfExtractingInput.RtfState.State[] stack;

			private int stackTop;

			public struct State
			{
				public RtfDestination Dest;

				public short LevelsDeep;
			}
		}
	}
}
