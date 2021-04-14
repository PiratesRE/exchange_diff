using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal class RtfFormatConverter : FormatConverter, IProducerConsumer, IDisposable
	{
		public RtfFormatConverter(RtfParser parser, FormatOutput output, Injection injection, bool treatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream) : base(formatConverterTraceStream)
		{
			this.treatNbspAsBreakable = treatNbspAsBreakable;
			this.parser = parser;
			this.injection = injection;
			this.output = output;
			if (this.output != null)
			{
				this.output.Initialize(this.Store, SourceFormat.Rtf, "converted from rtf");
			}
			this.Construct();
		}

		public RtfFormatConverter(RtfParser parser, FormatStore formatStore, bool treatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream) : base(formatStore, formatConverterTraceStream)
		{
			this.treatNbspAsBreakable = treatNbspAsBreakable;
			this.parser = parser;
			this.Construct();
		}

		private bool CanFlush
		{
			get
			{
				return this.output.CanAcceptMoreOutput;
			}
		}

		void IDisposable.Dispose()
		{
			this.scratch.DisposeBuffer();
			this.scratchAlt.DisposeBuffer();
			if (this.parser != null && this.parser is IDisposable)
			{
				((IDisposable)this.parser).Dispose();
			}
			this.parser = null;
			GC.SuppressFinalize(this);
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
			return base.EndOfFile && !base.MustFlush;
		}

		public void BreakLine()
		{
			if (this.beforeContent)
			{
				this.PrepareToAddContent();
			}
			base.AddLineBreak(1);
		}

		public void OutputTabulation(int count)
		{
			if (this.beforeContent)
			{
				this.PrepareToAddContent();
			}
			base.AddTabulation(count);
		}

		public void OutputSpace(int count)
		{
			base.AddSpace(count);
		}

		public void OutputNbsp(int count)
		{
			base.AddNbsp(count);
		}

		public void OutputNonspace(char[] buffer, int offset, int count, TextMapping textMapping)
		{
			base.AddNonSpaceText(buffer, offset, count);
		}

		private static RtfNumbering ListTypeToNumbering(int listType)
		{
			switch (listType)
			{
			case 0:
				return RtfNumbering.Arabic;
			case 1:
				return RtfNumbering.UcRoman;
			case 2:
				return RtfNumbering.LcRoman;
			case 3:
				return RtfNumbering.UcLetter;
			case 4:
				return RtfNumbering.LcLetter;
			default:
				if (listType != 23)
				{
					return RtfNumbering.Bullet;
				}
				return RtfNumbering.Bullet;
			}
		}

		private void Construct()
		{
			this.state = default(RtfFormatConverter.RtfState);
			this.state.Initialize();
			this.colors = new int[64];
			this.fonts = new RtfFont[32];
			this.lists = new RtfFormatConverter.List[128];
			this.listDirectory = new short[128];
			this.fonts[0] = new RtfFont("Times New Roman");
			this.fontsCount += 1;
			this.lists[0].Levels = new RtfFormatConverter.ListLevel[9];
			this.lists[0].Levels[0].Type = RtfNumbering.Arabic;
			this.lists[0].Levels[0].Start = 1;
			this.lists[0].Levels[1].Type = RtfNumbering.LcLetter;
			this.lists[0].Levels[1].Start = 1;
			this.lists[0].Levels[2].Type = RtfNumbering.LcRoman;
			this.lists[0].Levels[2].Start = 1;
			this.lists[0].Levels[3].Type = RtfNumbering.Arabic;
			this.lists[0].Levels[3].Start = 1;
			this.lists[0].Levels[4].Type = RtfNumbering.LcLetter;
			this.lists[0].Levels[4].Start = 1;
			this.lists[0].Levels[5].Type = RtfNumbering.LcRoman;
			this.lists[0].Levels[5].Start = 1;
			this.lists[0].Levels[6].Type = RtfNumbering.Arabic;
			this.lists[0].Levels[6].Start = 1;
			this.lists[0].Levels[7].Type = RtfNumbering.LcLetter;
			this.lists[0].Levels[7].Start = 1;
			this.lists[0].Levels[8].Type = RtfNumbering.LcRoman;
			this.lists[0].Levels[8].Start = 1;
			this.lists[0].LevelCount = 9;
			this.listsCount += 1;
			this.lists[1].Levels = new RtfFormatConverter.ListLevel[9];
			this.lists[1].Levels[0].Type = RtfNumbering.Bullet;
			this.lists[1].Levels[0].Start = 1;
			this.lists[1].Levels[1].Type = RtfNumbering.Bullet;
			this.lists[1].Levels[1].Start = 1;
			this.lists[1].Levels[2].Type = RtfNumbering.Bullet;
			this.lists[1].Levels[2].Start = 1;
			this.lists[1].Levels[3].Type = RtfNumbering.Bullet;
			this.lists[1].Levels[3].Start = 1;
			this.lists[1].Levels[4].Type = RtfNumbering.Bullet;
			this.lists[1].Levels[4].Start = 1;
			this.lists[1].Levels[5].Type = RtfNumbering.Bullet;
			this.lists[1].Levels[5].Start = 1;
			this.lists[1].Levels[6].Type = RtfNumbering.Bullet;
			this.lists[1].Levels[6].Start = 1;
			this.lists[1].Levels[7].Type = RtfNumbering.Bullet;
			this.lists[1].Levels[7].Start = 1;
			this.lists[1].Levels[8].Type = RtfNumbering.Bullet;
			this.lists[1].Levels[8].Start = 1;
			this.lists[1].LevelCount = 9;
			this.listsCount += 1;
			this.listDirectory[0] = 0;
			this.listDirectoryCount += 1;
			this.state.SetPlain();
			base.InitializeDocument();
			this.RegisterFontValue(0);
			base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontFace, this.fonts[(int)this.state.FontIndex].Value);
			base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Twips, (int)this.state.FontSize));
			this.currentListState.ListIndex = -1;
			this.documentContainerStillOpen = true;
			if (this.injection != null)
			{
				bool haveHead = this.injection.HaveHead;
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
					if (id <= 203)
					{
						if (id <= 50)
						{
							if (id <= 24)
							{
								switch (id)
								{
								case 8:
									this.state.SkipGroup();
									return;
								case 9:
									if (this.state.Destination != RtfDestination.ListTableEntry)
									{
										goto IL_6D7;
									}
									if (this.lists[(int)this.listIdx].LevelCount != 9)
									{
										this.state.Destination = RtfDestination.ListLevelEntry;
										RtfFormatConverter.List[] array = this.lists;
										short num = this.listIdx;
										byte levelCount;
										array[(int)num].LevelCount = (levelCount = array[(int)num].LevelCount) + 1;
										this.listLevel = (short)levelCount;
										this.lists[(int)this.listIdx].Levels[(int)this.listLevel].Start = 1;
										goto IL_6D7;
									}
									continue;
								default:
									if (id != 15 && id != 24)
									{
										goto IL_6BF;
									}
									goto IL_28D;
								}
							}
							else
							{
								if (id == 29)
								{
									if (this.state.Destination == RtfDestination.RTF || (this.state.Destination == RtfDestination.FieldResult && !this.ignoreFieldResult))
									{
										this.state.Destination = RtfDestination.Field;
									}
									return;
								}
								if (id != 39)
								{
									if (id != 50)
									{
										goto IL_6BF;
									}
									if (this.state.Destination == RtfDestination.RTF || this.state.Destination == RtfDestination.FieldResult)
									{
										this.state.Destination = RtfDestination.Picture;
										this.pictureWidth = 0;
										this.pictureHeight = 0;
										continue;
									}
									this.state.SkipGroup();
									return;
								}
							}
						}
						else if (id <= 92)
						{
							if (id != 54)
							{
								if (id == 65)
								{
									this.state.Destination = RtfDestination.NestTableProps;
									continue;
								}
								if (id != 92)
								{
									goto IL_6BF;
								}
							}
							else
							{
								if (this.state.Destination == RtfDestination.ListTable && (int)this.listsCount < this.lists.Length)
								{
									this.lists[(int)this.listsCount].Levels = new RtfFormatConverter.ListLevel[9];
									this.listIdx = this.listsCount;
									this.listsCount += 1;
									this.state.Destination = RtfDestination.ListTableEntry;
									continue;
								}
								continue;
							}
						}
						else if (id <= 175)
						{
							if (id != 123)
							{
								if (id != 175)
								{
									goto IL_6BF;
								}
								if (this.state.Destination == RtfDestination.RTF)
								{
									this.state.FontIndex = -1;
									this.state.Destination = RtfDestination.FontTable;
									continue;
								}
								continue;
							}
							else
							{
								if (this.state.Destination != RtfDestination.Picture)
								{
									this.state.ListIndex = 0;
									this.state.ListLevel = 0;
									this.state.Destination = RtfDestination.ParaNumbering;
									goto IL_6D7;
								}
								goto IL_6D7;
							}
						}
						else if (id != 193)
						{
							switch (id)
							{
							case 201:
								goto IL_28D;
							case 202:
								goto IL_6BF;
							case 203:
								if (this.state.Destination == RtfDestination.ListOverrideTable)
								{
									if ((int)this.listDirectoryCount < this.lists.Length)
									{
										this.listDirectory[(int)this.listDirectoryCount] = 0;
										this.listDirectoryCount += 1;
									}
									this.state.Destination = RtfDestination.ListOverrideTableEntry;
									continue;
								}
								continue;
							default:
								goto IL_6BF;
							}
						}
						else
						{
							if (this.state.Destination == RtfDestination.RTF || (this.state.Destination == RtfDestination.FieldResult && !this.ignoreFieldResult))
							{
								this.state.Destination = RtfDestination.BookmarkName;
								continue;
							}
							this.state.SkipGroup();
							return;
						}
					}
					else if (id <= 258)
					{
						if (id <= 230)
						{
							switch (id)
							{
							case 210:
								if (this.state.Destination == RtfDestination.FontTable && this.state.FontIndex >= 0)
								{
									this.state.Destination = RtfDestination.RealFontName;
									continue;
								}
								continue;
							case 211:
								goto IL_6BF;
							case 212:
								this.state.SkipGroup();
								return;
							default:
								if (id != 227 && id != 230)
								{
									goto IL_6BF;
								}
								goto IL_28D;
							}
						}
						else if (id <= 241)
						{
							if (id != 233 && id != 241)
							{
								goto IL_6BF;
							}
							goto IL_28D;
						}
						else if (id != 246)
						{
							switch (id)
							{
							case 252:
								if (this.state.Destination == RtfDestination.RTF)
								{
									this.state.Destination = RtfDestination.ColorTable;
									continue;
								}
								continue;
							case 253:
							case 258:
								goto IL_28D;
							case 254:
							case 255:
							case 256:
								goto IL_6BF;
							case 257:
								this.state.SkipGroup();
								return;
							default:
								goto IL_6BF;
							}
						}
						else
						{
							if (this.state.Destination == RtfDestination.RTF && this.listsCount == 2)
							{
								this.state.Destination = RtfDestination.ListTable;
								continue;
							}
							continue;
						}
					}
					else if (id <= 277)
					{
						switch (id)
						{
						case 268:
							if (this.state.Destination == RtfDestination.FontTable && this.state.FontIndex >= 0)
							{
								this.state.Destination = RtfDestination.AltFontName;
								continue;
							}
							continue;
						case 269:
							if (this.state.Destination == RtfDestination.Field)
							{
								this.state.Destination = RtfDestination.FieldResult;
								continue;
							}
							this.state.SkipGroup();
							return;
						default:
							if (id != 273 && id != 277)
							{
								goto IL_6BF;
							}
							goto IL_28D;
						}
					}
					else if (id <= 301)
					{
						if (id == 283)
						{
							this.state.SkipGroup();
							return;
						}
						if (id != 301)
						{
							goto IL_6BF;
						}
					}
					else if (id != 306)
					{
						switch (id)
						{
						case 315:
							goto IL_28D;
						case 316:
							if (this.state.Destination == RtfDestination.RTF && this.listDirectoryCount == 1)
							{
								this.state.Destination = RtfDestination.ListOverrideTable;
								continue;
							}
							continue;
						case 317:
						case 318:
							goto IL_6BF;
						case 319:
							this.state.SkipGroup();
							return;
						default:
							goto IL_6BF;
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
						return;
					}
					this.state.SkipGroup();
					return;
					IL_6BF:
					if (this.ignorableDestination)
					{
						this.state.SkipGroup();
						return;
					}
					goto IL_6D7;
					IL_28D:
					this.state.SkipGroup();
					return;
				}
				IL_6D7:
				short id2 = rtfKeyword.Id;
				if (id2 <= 285)
				{
					switch (id2)
					{
					case 4:
					case 5:
					case 19:
					case 61:
					case 63:
					case 76:
					case 150:
					case 155:
					case 159:
					case 178:
					case 190:
					case 225:
						goto IL_1E32;
					case 6:
						if (this.state.Destination == RtfDestination.Picture)
						{
							this.pictureHeight = rtfKeyword.Value;
							continue;
						}
						continue;
					case 7:
					case 10:
					case 12:
					case 16:
					case 22:
					case 25:
					case 181:
					case 186:
					case 188:
					case 194:
					case 195:
					case 196:
					case 215:
					case 218:
						this.borderId = (RtfBorderId)RTFData.keywords[(int)rtfKeyword.Id].idx;
						continue;
					case 8:
					case 9:
					case 15:
					case 18:
					case 20:
					case 24:
					case 26:
					case 27:
					case 28:
					case 29:
					case 30:
					case 31:
					case 32:
					case 33:
					case 34:
					case 35:
					case 37:
					case 38:
					case 39:
					case 42:
					case 43:
					case 45:
					case 47:
					case 49:
					case 50:
					case 53:
					case 54:
					case 56:
					case 57:
					case 59:
					case 60:
					case 62:
					case 65:
					case 67:
					case 69:
					case 70:
					case 71:
					case 72:
					case 74:
					case 75:
					case 79:
					case 81:
					case 83:
					case 84:
					case 85:
					case 87:
					case 92:
					case 93:
					case 94:
					case 95:
					case 96:
					case 104:
					case 106:
					case 107:
					case 108:
					case 109:
					case 110:
					case 114:
					case 115:
					case 117:
					case 123:
					case 124:
					case 125:
					case 127:
					case 128:
					case 129:
					case 130:
					case 133:
					case 134:
					case 135:
					case 137:
					case 145:
					case 146:
					case 147:
					case 149:
					case 152:
					case 153:
					case 158:
					case 162:
					case 164:
					case 165:
					case 166:
					case 168:
					case 169:
					case 175:
					case 177:
					case 182:
					case 183:
					case 185:
					case 187:
					case 189:
					case 192:
					case 193:
					case 197:
					case 201:
					case 202:
					case 203:
					case 204:
					case 207:
					case 208:
					case 210:
					case 212:
					case 214:
					case 217:
					case 222:
					case 223:
					case 224:
					case 226:
					case 227:
					case 230:
					case 231:
					case 232:
					case 233:
					case 234:
					case 235:
					case 236:
					case 238:
					case 240:
					case 241:
					case 242:
					case 244:
					case 245:
					case 246:
					case 249:
					case 252:
					case 253:
					case 255:
					case 257:
					case 258:
					case 260:
					case 263:
						continue;
					case 11:
					case 14:
					case 23:
					case 46:
					case 52:
					case 64:
					case 80:
					case 89:
					case 90:
					case 97:
					case 122:
					case 132:
					case 156:
					case 171:
					case 209:
					case 216:
					case 239:
					case 243:
					case 247:
					case 251:
					case 254:
					case 256:
					case 261:
						goto IL_1B15;
					case 13:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32767 && this.newRow != null)
						{
							continue;
						}
						continue;
					case 17:
						this.state.Capitalize = (0 != rtfKeyword.Value);
						continue;
					case 21:
					case 36:
					case 77:
					case 248:
						break;
					case 40:
						this.EventEndRow(false);
						continue;
					case 41:
					case 91:
						if (this.newRow != null)
						{
							this.newRow.RightToLeft = true;
							continue;
						}
						continue;
					case 44:
						if (this.state.Destination == RtfDestination.ListLevelEntry && 0 < rtfKeyword.Value)
						{
							this.lists[(int)this.listIdx].Levels[(int)this.listLevel].Start = (short)rtfKeyword.Value;
							continue;
						}
						continue;
					case 48:
						if (this.state.Destination == RtfDestination.RTF || (this.state.Destination == RtfDestination.FieldResult && !this.ignoreFieldResult))
						{
							this.BreakLine();
							continue;
						}
						continue;
					case 51:
						if (this.state.Destination == RtfDestination.ParaNumbering)
						{
							this.state.ListIndex = 1;
							this.state.ListLevel = 0;
							this.state.ListStyle = RtfNumbering.Bullet;
							continue;
						}
						continue;
					case 55:
						this.EventEndRow(true);
						continue;
					case 58:
						if (rtfKeyword.Value < 0 || rtfKeyword.Value >= this.colorsCount)
						{
							continue;
						}
						if (rtfKeyword.Value != 0)
						{
							this.state.ParagraphBackColor = this.colors[rtfKeyword.Value];
							continue;
						}
						this.state.ParagraphBackColor = 16777215;
						continue;
					case 66:
						if (rtfKeyword.Value >= -32768 && rtfKeyword.Value < 32767 && this.newRow != null && this.newRow.CellCount == 0)
						{
							this.newRow.Left = (this.newRow.Right = (short)(rtfKeyword.Value + 108));
							continue;
						}
						continue;
					case 68:
						if (this.state.Destination == RtfDestination.RTF || (this.state.Destination == RtfDestination.FieldResult && !this.ignoreFieldResult))
						{
							this.EventEndParagraph();
							continue;
						}
						continue;
					case 73:
						if (this.newRow != null)
						{
							this.newRow.LastRow = true;
							continue;
						}
						continue;
					case 78:
						if (this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
						{
							this.newRow.Cells[(int)this.newRow.CellCount].MergeUp = true;
							continue;
						}
						continue;
					case 82:
						goto IL_1DD1;
					case 86:
						if (this.state.Destination == RtfDestination.ParaNumbering && this.state.ListIndex != -1)
						{
							this.lists[(int)this.state.ListIndex].Levels[(int)this.state.ListLevel].Start = (short)rtfKeyword.Value;
							continue;
						}
						continue;
					case 88:
					case 148:
						goto IL_D53;
					case 98:
					case 100:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < this.colorsCount && this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
						{
							this.newRow.Cells[(int)this.newRow.CellCount].VerticalAlignment = (RtfVertAlignment)RTFData.keywords[(int)rtfKeyword.Id].idx;
							continue;
						}
						continue;
					case 99:
					case 198:
						this.state.Strikethrough = (0 != rtfKeyword.Value);
						continue;
					case 101:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 10)
						{
							this.requiredTableLevel = rtfKeyword.Value;
							continue;
						}
						continue;
					case 102:
						if (this.state.Destination == RtfDestination.ParaNumbering && 1 <= rtfKeyword.Value && rtfKeyword.Value <= 9 && this.state.ListIndex == -1)
						{
							this.state.ListIndex = 0;
							this.state.ListLevel = (byte)(rtfKeyword.Value - 1);
							continue;
						}
						continue;
					case 103:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value <= 3 && this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
						{
							this.newRow.Cells[(int)this.newRow.CellCount].WidthType = (byte)rtfKeyword.Value;
							continue;
						}
						continue;
					case 105:
						this.EventEndCell(true);
						continue;
					case 111:
					case 112:
					case 116:
					case 120:
					case 140:
					case 141:
						this.state.ParagraphAlignment = (RtfAlignment)RTFData.keywords[(int)rtfKeyword.Id].idx;
						continue;
					case 113:
					{
						if (this.state.Destination != RtfDestination.FontTable && this.state.Destination != RtfDestination.AltFontName && this.state.Destination != RtfDestination.RealFontName)
						{
							continue;
						}
						if (this.state.FontIndex >= 0 && this.state.FontIndex < this.fontsCount)
						{
							this.fonts[(int)this.state.FontIndex].Name = RtfSupport.StringFontNameFromScratch(this.scratch);
							this.scratch.Reset();
							this.state.FontIndex = -1;
						}
						short num2 = this.parser.FontIndex((short)rtfKeyword.Value);
						if (num2 < 0)
						{
							continue;
						}
						this.state.FontIndex = num2;
						if (num2 >= this.fontsCount)
						{
							if ((int)num2 >= this.fonts.Length)
							{
								RtfFont[] destinationArray = new RtfFont[Math.Max(this.fonts.Length * 2, (int)(num2 + 1))];
								Array.Copy(this.fonts, destinationArray, (int)this.fontsCount);
								this.fonts = destinationArray;
							}
							this.fontsCount = num2 + 1;
						}
						if (this.fonts[(int)num2].Value.IsRefCountedHandle)
						{
							base.ReleasePropertyValue(this.fonts[(int)num2].Value);
							this.fonts[(int)num2].Value = PropertyValue.Null;
							continue;
						}
						continue;
					}
					case 118:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32767 && this.newRow != null)
						{
							this.newRow.CellPadding.Right = (short)rtfKeyword.Value;
							continue;
						}
						continue;
					case 119:
						this.EventEndCell(false);
						continue;
					case 121:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32767 && this.newRow != null)
						{
							this.newRow.CellPadding.Top = (short)rtfKeyword.Value;
							continue;
						}
						continue;
					case 126:
						if (this.state.Destination == RtfDestination.RTF || (this.state.Destination == RtfDestination.FieldResult && !this.ignoreFieldResult))
						{
							this.OutputTabulation(1);
							continue;
						}
						continue;
					case 131:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32767 && this.newRow != null)
						{
							this.newRow.CellPadding.Left = (short)rtfKeyword.Value;
							continue;
						}
						continue;
					case 136:
						this.state.Invisible = (rtfKeyword.Value != 0);
						continue;
					case 138:
						this.state.RightIndent = (short)rtfKeyword.Value;
						continue;
					case 139:
						this.state.SpaceAfter = (short)rtfKeyword.Value;
						continue;
					case 142:
						this.state.SpaceBefore = (short)rtfKeyword.Value;
						continue;
					case 143:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < this.colorsCount && this.newRow != null)
						{
							this.newRow.BackColor = this.colors[rtfKeyword.Value];
							continue;
						}
						continue;
					case 144:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32767 && this.newRow != null)
						{
							this.newRow.CellPadding.Bottom = (short)rtfKeyword.Value;
							continue;
						}
						continue;
					case 151:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32767 && this.newRow != null)
						{
							this.newRow.Height = (short)rtfKeyword.Value;
							continue;
						}
						continue;
					case 154:
						if (this.state.Destination == RtfDestination.Picture)
						{
							this.pictureWidth = rtfKeyword.Value;
							continue;
						}
						continue;
					case 157:
						if (this.beforeContent)
						{
							this.PrepareToAddContent();
						}
						base.OpenContainer(FormatContainerType.Image, true);
						base.Last.SetStringProperty(PropertyPrecedence.InlineStyle, PropertyId.ImageUrl, "objattph://");
						this.documentContainerStillOpen = false;
						continue;
					case 160:
						if (this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
						{
							this.newRow.Cells[(int)this.newRow.CellCount].MergeLeft = true;
							continue;
						}
						continue;
					case 161:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32767 && this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
						{
							this.newRow.Cells[(int)this.newRow.CellCount].Padding.Right = (short)rtfKeyword.Value;
							continue;
						}
						continue;
					case 163:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32767 && this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
						{
							this.newRow.Cells[(int)this.newRow.CellCount].Padding.Top = (short)rtfKeyword.Value;
							continue;
						}
						continue;
					case 167:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < this.colorsCount)
						{
							this.SetBorderColor(this.colors[rtfKeyword.Value]);
							continue;
						}
						continue;
					case 170:
						this.state.SetPlain();
						continue;
					case 172:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32767 && this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
						{
							this.newRow.Cells[(int)this.newRow.CellCount].Padding.Left = (short)rtfKeyword.Value;
							continue;
						}
						continue;
					case 173:
						if (this.state.Destination == RtfDestination.ListTableEntry)
						{
							this.lists[(int)this.listIdx].Id = rtfKeyword.Value;
							continue;
						}
						if (this.state.Destination == RtfDestination.ListOverrideTableEntry && this.listsCount != 1)
						{
							for (short num3 = 1; num3 < this.listsCount; num3 += 1)
							{
								if (this.lists[(int)num3].Id == rtfKeyword.Value)
								{
									this.listDirectory[(int)(this.listDirectoryCount - 1)] = num3;
									break;
								}
							}
							continue;
						}
						continue;
					case 174:
						goto IL_1D6F;
					case 176:
						this.state.ParagraphRtl = true;
						continue;
					case 179:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32767 && this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
						{
							this.newRow.Cells[(int)this.newRow.CellCount].Padding.Bottom = (short)rtfKeyword.Value;
							continue;
						}
						continue;
					case 180:
						this.state.SmallCaps = (0 != rtfKeyword.Value);
						continue;
					case 184:
						this.state.SetParagraphDefault();
						this.requiredTableLevel = 0;
						continue;
					case 191:
						if (this.requiredTableLevel == 0)
						{
							this.requiredTableLevel = 1;
							continue;
						}
						continue;
					case 199:
						if (this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
						{
							continue;
						}
						continue;
					case 200:
						this.InitializeFreshRowProperties();
						continue;
					case 205:
						if (this.state.ListIndex >= 0 && 0 <= rtfKeyword.Value && rtfKeyword.Value < (int)this.lists[(int)this.state.ListIndex].LevelCount)
						{
							this.state.ListLevel = (byte)rtfKeyword.Value;
							this.state.ListStyle = this.lists[(int)this.state.ListIndex].Levels[(int)this.state.ListLevel].Type;
							continue;
						}
						continue;
					case 206:
						this.state.LeftIndent = (short)rtfKeyword.Value;
						continue;
					case 211:
						this.state.Underline = false;
						continue;
					case 213:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32767 && this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
						{
							this.newRow.Cells[(int)this.newRow.CellCount].Width = (short)rtfKeyword.Value;
							continue;
						}
						continue;
					case 219:
						if (this.state.Destination != RtfDestination.ListOverrideTableEntry && rtfKeyword.Value >= 1)
						{
							short num4 = (short)rtfKeyword.Value;
							if (num4 < 0 || num4 >= this.listDirectoryCount)
							{
								num4 = 0;
							}
							this.state.ListIndex = this.listDirectory[(int)num4];
							this.state.ListLevel = 0;
							this.state.ListStyle = this.lists[(int)this.state.ListIndex].Levels[(int)this.state.ListLevel].Type;
							continue;
						}
						continue;
					case 220:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value <= 75)
						{
							this.SetBorderWidth((byte)rtfKeyword.Value);
							continue;
						}
						continue;
					case 221:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32767 && this.newRow != null)
						{
							continue;
						}
						continue;
					case 228:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32659 && this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
						{
							if (rtfKeyword.Value >= (int)this.newRow.Right)
							{
								this.newRow.Right = (this.newRow.Cells[(int)this.newRow.CellCount].Cellx = (short)(rtfKeyword.Value + 108));
							}
							else
							{
								this.newRow.Cells[(int)this.newRow.CellCount].Cellx = this.newRow.Right;
							}
							RtfFormatConverter.RtfRow rtfRow = this.newRow;
							rtfRow.CellCount += 1;
							continue;
						}
						continue;
					case 229:
						if (this.state.Destination == RtfDestination.ParaNumbering)
						{
							this.state.ListStyle = RtfNumbering.None;
							this.state.ListIndex = -1;
							continue;
						}
						continue;
					case 237:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32767 && this.newRow != null)
						{
							this.newRow.Width = (short)rtfKeyword.Value;
							continue;
						}
						continue;
					case 250:
						if (this.state.Destination == RtfDestination.ListLevelEntry && 0 <= rtfKeyword.Value && rtfKeyword.Value <= 2)
						{
							continue;
						}
						continue;
					case 259:
						if (this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
						{
							continue;
						}
						continue;
					case 262:
						this.state.Subscript = (0 != rtfKeyword.Value);
						continue;
					case 264:
						this.state.Subscript = false;
						this.state.Superscript = false;
						continue;
					default:
						switch (id2)
						{
						case 274:
							break;
						case 275:
						case 277:
							continue;
						case 276:
							goto IL_1B15;
						case 278:
						case 279:
							goto IL_1E32;
						default:
							switch (id2)
							{
							case 284:
								this.state.FirstLineIndent = (short)rtfKeyword.Value;
								continue;
							case 285:
								goto IL_1DD1;
							default:
								continue;
							}
							break;
						}
						break;
					}
					if (this.state.Destination == RtfDestination.ParaNumbering)
					{
						this.state.ListStyle = (RtfNumbering)RTFData.keywords[(int)rtfKeyword.Id].idx;
						continue;
					}
					continue;
					IL_1DD1:
					if (rtfKeyword.Value < 0 || rtfKeyword.Value >= this.colorsCount)
					{
						continue;
					}
					if (rtfKeyword.Value != 0)
					{
						this.state.FontBackColor = this.colors[rtfKeyword.Value];
						continue;
					}
					this.state.FontBackColor = this.state.ParagraphBackColor;
					continue;
				}
				else if (id2 <= 299)
				{
					if (id2 == 289)
					{
						goto IL_1B15;
					}
					switch (id2)
					{
					case 293:
						if (this.newRow != null)
						{
							this.newRow.HeaderRow = true;
							continue;
						}
						continue;
					case 294:
					case 299:
						goto IL_1E32;
					case 295:
					case 297:
					case 298:
						continue;
					case 296:
						if (this.state.Destination == RtfDestination.ListLevelEntry && 0 <= rtfKeyword.Value && rtfKeyword.Value <= 255)
						{
							this.lists[(int)this.listIdx].Levels[(int)this.listLevel].Type = RtfFormatConverter.ListTypeToNumbering(rtfKeyword.Value);
							continue;
						}
						continue;
					default:
						continue;
					}
				}
				else
				{
					if (id2 == 302)
					{
						continue;
					}
					switch (id2)
					{
					case 309:
						break;
					case 310:
					case 313:
					case 326:
						goto IL_1E32;
					case 311:
					case 312:
					case 314:
					case 315:
					case 316:
					case 317:
					case 319:
					case 324:
					case 325:
					case 327:
					case 329:
						continue;
					case 318:
						if (rtfKeyword.Value < 0 || rtfKeyword.Value >= this.colorsCount || this.newRow == null || !this.newRow.EnsureEntryForCurrentCell())
						{
							continue;
						}
						if (rtfKeyword.Value != 0)
						{
							this.newRow.Cells[(int)this.newRow.CellCount].BackColor = this.colors[rtfKeyword.Value];
							continue;
						}
						this.newRow.Cells[(int)this.newRow.CellCount].BackColor = 16777215;
						continue;
					case 320:
						if (rtfKeyword.Value >= 0 && rtfKeyword.Value <= 3 && this.newRow != null)
						{
							this.newRow.WidthType = (byte)rtfKeyword.Value;
							continue;
						}
						continue;
					case 321:
					case 328:
					case 330:
						goto IL_1B15;
					case 322:
						if (this.state.Destination == RtfDestination.ParaNumbering && this.state.ListIndex == -1)
						{
							this.state.ListIndex = 0;
							this.state.ListLevel = 0;
							continue;
						}
						continue;
					case 323:
						this.state.Superscript = (0 != rtfKeyword.Value);
						continue;
					case 331:
						goto IL_1D6F;
					default:
						continue;
					}
				}
				IL_D53:
				if (this.state.Destination == RtfDestination.ColorTable)
				{
					this.color &= ~(255 << (int)(RTFData.keywords[(int)rtfKeyword.Id].idx * 8));
					this.color |= (rtfKeyword.Value & 255) << (int)(RTFData.keywords[(int)rtfKeyword.Id].idx * 8);
					continue;
				}
				continue;
				IL_1B15:
				this.SetBorderKind((RtfBorderKind)RTFData.keywords[(int)rtfKeyword.Id].idx);
				continue;
				IL_1D6F:
				if (rtfKeyword.Value < 0 || rtfKeyword.Value >= this.colorsCount)
				{
					continue;
				}
				this.state.FontColor = this.colors[rtfKeyword.Value];
				if (this.state.FontColor == 16777215)
				{
					this.state.FontColor = 13619151;
					continue;
				}
				continue;
				IL_1E32:
				this.state.Underline = (0 != rtfKeyword.Value);
			}
			if (this.parser.CurrentFontIndex < this.fontsCount)
			{
				this.state.FontIndex = this.parser.CurrentFontIndex;
			}
			this.state.FontSize = this.parser.CurrentFontSize;
			this.state.Language = this.parser.CurrentLanguage;
			this.state.Bold = this.parser.CurrentFontBold;
			this.state.Italic = this.parser.CurrentFontItalic;
			this.state.BiDi = this.parser.CurrentRunBiDi;
			this.state.ComplexScript = this.parser.CurrentRunComplexScript;
		}

		private void ProcessText(RtfToken token)
		{
			if (this.state.SkipLevel != 0 && this.state.Level >= this.state.SkipLevel)
			{
				return;
			}
			switch (this.state.Destination)
			{
			case RtfDestination.RTF:
				goto IL_1C7;
			case RtfDestination.FontTable:
				this.firstKeyword = false;
				this.scratch.AppendRtfTokenText(token, 256);
				return;
			case RtfDestination.RealFontName:
			case RtfDestination.AltFontName:
				this.firstKeyword = false;
				return;
			case RtfDestination.ColorTable:
				this.firstKeyword = false;
				this.scratch.Reset();
				while (this.scratch.AppendRtfTokenText(token, this.scratch.Capacity))
				{
					for (int i = 0; i < this.scratch.Length; i++)
					{
						if (this.scratch[i] == ';')
						{
							if (this.colorsCount == this.colors.Length)
							{
								return;
							}
							this.colors[this.colorsCount] = this.color;
							this.colorsCount++;
							this.color = 0;
						}
					}
					this.scratch.Reset();
				}
				return;
			case RtfDestination.FieldResult:
				if (this.ignoreFieldResult)
				{
					return;
				}
				goto IL_1C7;
			case RtfDestination.FieldInstruction:
				this.firstKeyword = false;
				this.scratch.AppendRtfTokenText(token, 4096);
				return;
			case RtfDestination.BookmarkName:
				this.firstKeyword = false;
				this.scratch.AppendRtfTokenText(token, 4096);
				return;
			}
			this.firstKeyword = false;
			return;
			IL_1C7:
			if (this.state.Invisible)
			{
				return;
			}
			RtfToken.TextEnumerator textElements = token.TextElements;
			if (textElements.MoveNext())
			{
				if (this.beforeContent)
				{
					this.PrepareToAddContent();
				}
				if (this.state.TextPropertiesChanged)
				{
					base.OpenTextContainer();
					if (this.state.FontSize != this.containerFontSize)
					{
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Twips, (int)this.state.FontSize));
					}
					if (this.state.Bold)
					{
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FirstFlag, new PropertyValue(this.state.Bold));
					}
					if (this.state.Italic)
					{
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.Italic, new PropertyValue(this.state.Italic));
					}
					if (this.state.Underline)
					{
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.Underline, new PropertyValue(this.state.Underline));
					}
					if (this.state.Subscript)
					{
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.Subscript, new PropertyValue(this.state.Subscript));
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Twips, (int)(this.state.FontSize * 2 / 3)));
					}
					if (this.state.Superscript)
					{
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.Superscript, new PropertyValue(this.state.Superscript));
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Twips, (int)(this.state.FontSize * 2 / 3)));
					}
					if (this.state.Strikethrough)
					{
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.Strikethrough, new PropertyValue(this.state.Strikethrough));
					}
					if (this.state.SmallCaps)
					{
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.SmallCaps, new PropertyValue(this.state.SmallCaps));
					}
					if (this.state.Capitalize)
					{
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.Capitalize, new PropertyValue(this.state.Capitalize));
					}
					if (this.state.FontIndex != this.containerFontIndex && this.state.FontIndex != -1)
					{
						if (this.fonts[(int)this.state.FontIndex].Value.IsNull)
						{
							this.RegisterFontValue(this.state.FontIndex);
						}
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontFace, this.fonts[(int)this.state.FontIndex].Value);
					}
					if (this.state.FontColor != this.containerFontColor)
					{
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontColor, new PropertyValue(new RGBT((uint)this.state.FontColor)));
					}
					if (this.state.FontBackColor != this.containerBackColor)
					{
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.BackColor, new PropertyValue(new RGBT((uint)this.state.FontBackColor)));
					}
					this.state.TextPropertiesChanged = false;
				}
				if (this.firstKeyword)
				{
					RtfTextElement rtfTextElement = textElements.Current;
					if (rtfTextElement.TextType != RunTextType.Space || rtfTextElement.Length != 1)
					{
						if (textElements.MoveNext())
						{
							this.firstKeyword = false;
						}
						textElements.Rewind();
						textElements.MoveNext();
					}
				}
				do
				{
					RtfTextElement rtfTextElement = textElements.Current;
					RunTextType textType = rtfTextElement.TextType;
					if (textType <= RunTextType.UnusualWhitespace)
					{
						if (textType == RunTextType.Space || textType == RunTextType.UnusualWhitespace)
						{
							this.OutputSpace(rtfTextElement.Length);
						}
					}
					else if (textType != RunTextType.Nbsp)
					{
						if (textType == RunTextType.NonSpace)
						{
							this.OutputNonspace(rtfTextElement.RawBuffer, rtfTextElement.RawOffset, rtfTextElement.RawLength, token.TextMapping);
						}
					}
					else if (this.treatNbspAsBreakable)
					{
						this.OutputSpace(rtfTextElement.Length);
					}
					else
					{
						this.OutputNbsp(rtfTextElement.Length);
					}
				}
				while (textElements.MoveNext());
			}
		}

		private void ProcessBinary(RtfToken token)
		{
		}

		private void RegisterFontValue(short fontIndex)
		{
			PropertyValue value;
			if (this.fonts[(int)fontIndex].Name != null)
			{
				value = base.RegisterFaceName(false, this.fonts[(int)fontIndex].Name);
			}
			else
			{
				string text = RtfFormatConverter.familyGenericName[(int)this.parser.FontFamily(fontIndex)];
				if (text != null)
				{
					value = base.RegisterFaceName(false, text);
				}
				else
				{
					value = base.RegisterFaceName(false, "serif");
				}
			}
			this.fonts[(int)fontIndex].Value = value;
		}

		private void ProcessEOF()
		{
			while (base.LastNonEmpty.Type != FormatContainerType.Document)
			{
				base.CloseContainer();
			}
			if (this.injection != null && this.injection.HaveTail)
			{
				while (base.LastNonEmpty.Type != FormatContainerType.Document)
				{
					base.CloseContainer();
				}
			}
			base.CloseAllContainersAndSetEOF();
		}

		private void EndGroup()
		{
			short listIndex = -1;
			byte b = 0;
			RtfNumbering listStyle = RtfNumbering.None;
			int fontColor = 0;
			short fontSize = 0;
			short fontIndex = 0;
			RtfDestination destination = this.state.Destination;
			if (destination <= RtfDestination.ParaNumText)
			{
				switch (destination)
				{
				case RtfDestination.FontTable:
					if (this.state.ParentDestination != RtfDestination.FontTable)
					{
						if (this.state.FontIndex >= 0)
						{
							this.fonts[(int)this.state.FontIndex].Name = RtfSupport.StringFontNameFromScratch(this.scratch);
							this.scratch.Reset();
						}
					}
					else if (this.state.FontIndex >= 0)
					{
						this.state.SetParentFontIndex(this.state.FontIndex);
					}
					break;
				case RtfDestination.RealFontName:
					if (this.state.ParentDestination != RtfDestination.RealFontName && this.state.ParentFontIndex >= 0)
					{
						this.scratchAlt.Reset();
					}
					break;
				case RtfDestination.AltFontName:
					if (this.state.ParentDestination != RtfDestination.AltFontName && this.state.ParentFontIndex >= 0)
					{
						this.scratchAlt.Reset();
					}
					break;
				default:
					switch (destination)
					{
					case RtfDestination.Field:
						if (this.state.ParentDestination != RtfDestination.Field)
						{
							this.ignoreFieldResult = false;
							if (base.LastNonEmpty.Type == FormatContainerType.HyperLink && this.state.ParentDestination != RtfDestination.FieldResult)
							{
								base.CloseContainer();
							}
						}
						break;
					case RtfDestination.FieldInstruction:
						if (this.state.ParentDestination == RtfDestination.Field)
						{
							bool flag;
							BufferString value;
							TextMapping textMapping;
							char c;
							short num;
							if (RtfSupport.IsHyperlinkField(ref this.scratch, out flag, out value))
							{
								if (this.beforeContent)
								{
									this.PrepareToAddContent();
								}
								else if (base.LastNonEmpty.Type == FormatContainerType.HyperLink)
								{
									base.CloseContainer();
								}
								value.TrimWhitespace();
								base.OpenContainer(FormatContainerType.HyperLink, false);
								base.Last.SetStringProperty(PropertyPrecedence.InlineStyle, PropertyId.HyperlinkUrl, value);
								this.documentContainerStillOpen = false;
							}
							else if (RtfSupport.IsIncludePictureField(ref this.scratch, out value))
							{
								if (this.beforeContent)
								{
									this.PrepareToAddContent();
								}
								value.TrimWhitespace();
								base.OpenContainer(FormatContainerType.Image, true);
								base.Last.SetStringProperty(PropertyPrecedence.InlineStyle, PropertyId.ImageUrl, value);
								this.documentContainerStillOpen = false;
								this.ignoreFieldResult = true;
							}
							else if (RtfSupport.IsSymbolField(ref this.scratch, out textMapping, out c, out num))
							{
								if (this.beforeContent)
								{
									this.PrepareToAddContent();
								}
								base.OpenTextContainer();
								PropertyValue value2 = default(PropertyValue);
								switch (textMapping)
								{
								case TextMapping.Symbol:
									value2 = base.RegisterFaceName(false, "Symbol");
									goto IL_397;
								}
								value2 = base.RegisterFaceName(false, "Wingdings");
								IL_397:
								if (!value2.IsNull)
								{
									base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontFace, value2);
								}
								if (num != 0)
								{
									base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Points, (int)num));
								}
								this.scratch.Buffer[0] = c;
								this.OutputNonspace(this.scratch.Buffer, 0, 1, textMapping);
								this.documentContainerStillOpen = false;
								this.ignoreFieldResult = true;
							}
							this.scratch.Reset();
						}
						break;
					case RtfDestination.ParaNumbering:
					case RtfDestination.ParaNumText:
						listIndex = this.state.ListIndex;
						b = this.state.ListLevel;
						listStyle = this.state.ListStyle;
						fontColor = this.state.FontColor;
						fontSize = this.state.FontSize;
						fontIndex = this.state.FontIndex;
						break;
					}
					break;
				}
			}
			else if (destination != RtfDestination.Picture)
			{
				if (destination == RtfDestination.BookmarkName)
				{
					if (this.state.ParentDestination != RtfDestination.BookmarkName)
					{
						BufferString bufferString = this.scratch.BufferString;
						bufferString.TrimWhitespace();
						if (bufferString.Length != 0)
						{
							bool flag2 = this.beforeContent;
							base.OpenContainer(FormatContainerType.Bookmark, false);
							base.Last.SetStringProperty(PropertyPrecedence.InlineStyle, PropertyId.BookmarkName, bufferString);
							base.CloseContainer();
							this.documentContainerStillOpen = false;
						}
						this.scratch.Reset();
					}
				}
			}
			else if (this.state.ParentDestination != RtfDestination.Picture)
			{
				if (!this.ignoreFieldResult)
				{
					if (this.beforeContent)
					{
						this.PrepareToAddContent();
					}
					base.OpenContainer(FormatContainerType.Image, true);
					base.Last.SetStringProperty(PropertyPrecedence.InlineStyle, PropertyId.ImageUrl, "rtfimage://");
					this.documentContainerStillOpen = false;
				}
				if (base.Last.Type == FormatContainerType.Image)
				{
					if (this.pictureWidth != 0)
					{
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.Width, new PropertyValue(LengthUnits.Twips, this.pictureWidth));
					}
					if (this.pictureHeight != 0)
					{
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.Height, new PropertyValue(LengthUnits.Twips, this.pictureHeight));
					}
				}
			}
			RtfDestination destination2 = this.state.Destination;
			this.state.Pop();
			if (destination2 == RtfDestination.ParaNumbering && this.state.Destination != RtfDestination.ParaNumbering)
			{
				this.state.ListIndex = listIndex;
				this.state.ListLevel = b;
				this.state.ListStyle = listStyle;
				this.state.FontColor = fontColor;
				this.state.FontSize = fontSize;
				this.state.FontIndex = fontIndex;
			}
		}

		private void EventEndParagraph()
		{
			if (this.beforeContent)
			{
				this.PrepareToAddContent();
				base.AddNbsp(1);
			}
			if (base.LastNonEmpty.Type == FormatContainerType.HyperLink)
			{
				base.CloseContainer();
			}
			if (base.LastNonEmpty.Type == FormatContainerType.Block)
			{
				base.CloseContainer();
			}
			if (base.LastNonEmpty.Type == FormatContainerType.ListItem)
			{
				base.CloseContainer();
			}
			this.beforeContent = true;
		}

		private void EventEndRow(bool nested)
		{
			if (this.beforeContent && (this.requiredTableLevel != this.currentTableLevel || base.LastNonEmpty.Type != FormatContainerType.TableRow || base.LastNonEmpty.Node.LastChild.IsNull))
			{
				this.PrepareToAddContent();
			}
			if (this.currentTableLevel > (nested ? 1 : 0))
			{
				if (!nested && 1 != this.currentTableLevel)
				{
					this.AdjustTableLevel(1);
				}
				else if (-1 != this.currentListState.ListIndex)
				{
					this.CloseList();
				}
				else
				{
					if (base.LastNonEmpty.Type == FormatContainerType.HyperLink)
					{
						base.CloseContainer();
					}
					if (base.LastNonEmpty.Type == FormatContainerType.Block)
					{
						base.CloseContainer();
					}
				}
				if (base.LastNonEmpty.Type == FormatContainerType.Table)
				{
					this.OpenRow();
					this.OpenCell();
				}
				if (base.LastNonEmpty.Type == FormatContainerType.TableCell)
				{
					this.CloseCell();
				}
				this.CloseRow(false);
				this.beforeContent = true;
				return;
			}
			this.EventEndParagraph();
		}

		private void EventEndCell(bool nested)
		{
			if (this.beforeContent)
			{
				this.PrepareToAddContent();
			}
			if (this.currentTableLevel > (nested ? 1 : 0))
			{
				if (!nested && 1 != this.currentTableLevel)
				{
					this.AdjustTableLevel(1);
				}
				else if (-1 != this.currentListState.ListIndex)
				{
					this.CloseList();
				}
				else
				{
					if (base.LastNonEmpty.Type == FormatContainerType.HyperLink)
					{
						base.CloseContainer();
					}
					if (base.LastNonEmpty.Type == FormatContainerType.Block)
					{
						base.CloseContainer();
					}
				}
				if (base.LastNonEmpty.Type == FormatContainerType.Table)
				{
					this.OpenRow();
				}
				if (base.LastNonEmpty.Type == FormatContainerType.TableRow)
				{
					this.OpenCell();
				}
				this.CloseCell();
				this.beforeContent = true;
				return;
			}
			if (!nested)
			{
				base.AddLineBreak(1);
				this.documentContainerStillOpen = false;
			}
		}

		private void AdjustContainer()
		{
			if (this.requiredTableLevel != this.currentTableLevel)
			{
				this.AdjustTableLevel(this.requiredTableLevel);
			}
			if (this.state.ListIndex != this.currentListState.ListIndex || this.state.ListLevel != this.currentListState.ListLevel || this.state.ListStyle != this.currentListState.ListStyle || this.state.LeftIndent != this.currentListState.LeftIndent || this.state.RightIndent != this.currentListState.RightIndent || this.state.FirstLineIndent != this.currentListState.FirstLineIndent || this.state.ParagraphRtl != this.currentListState.Rtl)
			{
				this.CloseList();
			}
			if (this.state.ListIndex != -1)
			{
				this.OpenList();
			}
		}

		private void PrepareToAddContent()
		{
			if (this.documentContainerStillOpen)
			{
				short num = this.state.FontIndex;
				if (this.state.FontIndex != -1)
				{
					if (this.fonts[(int)this.state.FontIndex].Name != null && !this.fonts[(int)this.state.FontIndex].Name.StartsWith("Wingdings", StringComparison.OrdinalIgnoreCase) && !this.fonts[(int)this.state.FontIndex].Name.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
					{
						if (this.fonts[(int)this.state.FontIndex].Value.IsNull)
						{
							this.RegisterFontValue(this.state.FontIndex);
						}
						base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontFace, this.fonts[(int)this.state.FontIndex].Value);
					}
					else
					{
						num = -1;
					}
				}
				if (this.state.FontSize != RtfFormatConverter.TwelvePointsInTwips)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Twips, (int)this.state.FontSize));
				}
				this.documentFontIndex = num;
				this.documentFontSize = this.state.FontSize;
				this.documentContainerStillOpen = false;
			}
			this.AdjustContainer();
			if (base.LastNonEmpty.Type == FormatContainerType.Document)
			{
				base.OpenContainer(FormatContainerType.Block, false);
			}
			else if (base.LastNonEmpty.Type == FormatContainerType.Table)
			{
				this.OpenRow();
				this.OpenCell();
			}
			else if (base.LastNonEmpty.Type == FormatContainerType.TableRow)
			{
				this.OpenCell();
			}
			else if (base.LastNonEmpty.Type == FormatContainerType.List)
			{
				base.OpenContainer(FormatContainerType.ListItem, false);
				RtfFormatConverter.ListLevel[] levels = this.lists[(int)this.currentListState.ListIndex].Levels;
				byte b = this.currentListState.ListLevel;
				levels[(int)b].Start = levels[(int)b].Start + 1;
			}
			if (base.LastNonEmpty.Type == FormatContainerType.TableCell && !base.LastNonEmpty.Node.FirstChild.IsNull)
			{
				base.OpenContainer(FormatContainerType.Block, false);
			}
			if (base.LastNonEmpty.Type != FormatContainerType.ListItem)
			{
				short fontIndex = this.state.FontIndex;
				if (this.state.FontIndex != -1 && this.state.FontIndex != this.documentFontIndex && this.fonts[(int)this.state.FontIndex].Name != null && !this.fonts[(int)this.state.FontIndex].Name.StartsWith("Wingdings", StringComparison.OrdinalIgnoreCase) && !this.fonts[(int)this.state.FontIndex].Name.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
				{
					if (this.fonts[(int)this.state.FontIndex].Value.IsNull)
					{
						this.RegisterFontValue(this.state.FontIndex);
					}
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontFace, this.fonts[(int)this.state.FontIndex].Value);
				}
				else
				{
					fontIndex = this.documentFontIndex;
				}
				this.containerFontIndex = fontIndex;
				if (this.state.FontSize != this.documentFontSize)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Twips, (int)this.state.FontSize));
				}
				this.containerFontSize = this.state.FontSize;
				if (this.state.FontColor != 0)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontColor, new PropertyValue(new RGBT((uint)this.state.FontColor)));
				}
				this.containerFontColor = this.state.FontColor;
				if (this.state.ParagraphBackColor != 16777215)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.BackColor, new PropertyValue(new RGBT((uint)this.state.ParagraphBackColor)));
				}
				this.containerBackColor = this.state.ParagraphBackColor;
			}
			if (this.state.SpaceBefore != 0)
			{
				base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.Margins, new PropertyValue(LengthUnits.Twips, (int)this.state.SpaceBefore));
			}
			if (this.state.SpaceAfter != 0)
			{
				base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.BottomMargin, new PropertyValue(LengthUnits.Twips, (int)this.state.SpaceAfter));
			}
			if (base.LastNonEmpty.Type != FormatContainerType.ListItem)
			{
				if (this.state.LeftIndent != 0)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.LeftPadding, new PropertyValue(LengthUnits.Twips, (int)this.state.LeftIndent));
				}
				if (this.state.RightIndent != 0)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.RightPadding, new PropertyValue(LengthUnits.Twips, (int)this.state.RightIndent));
				}
				if (this.state.FirstLineIndent != 0)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FirstLineIndent, new PropertyValue(LengthUnits.Twips, (int)this.state.FirstLineIndent));
				}
				if (this.state.ParagraphAlignment != RtfAlignment.Left && this.state.ParagraphAlignment < RtfAlignment.Distributed)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.TextAlignment, new PropertyValue((TextAlign)this.state.ParagraphAlignment));
				}
				if (this.state.ParagraphRtl)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.RightToLeft, new PropertyValue(this.state.ParagraphRtl));
				}
			}
			this.state.TextPropertiesChanged = true;
			this.beforeContent = false;
		}

		private void AdjustTableLevel(int requiredTableLevel)
		{
			if (-1 != this.currentListState.ListIndex)
			{
				this.CloseList();
			}
			if (base.LastNonEmpty.Type == FormatContainerType.HyperLink)
			{
				base.CloseContainer();
			}
			if (base.LastNonEmpty.Type == FormatContainerType.Block)
			{
				base.CloseContainer();
			}
			if (this.currentTableLevel < requiredTableLevel)
			{
				if (base.LastNonEmpty.Type == FormatContainerType.Table)
				{
					this.OpenRow();
				}
				if (base.LastNonEmpty.Type == FormatContainerType.TableRow)
				{
					this.OpenCell();
				}
				do
				{
					this.OpenTable();
					this.OpenRow();
					this.OpenCell();
				}
				while (this.currentTableLevel < requiredTableLevel);
				this.documentContainerStillOpen = false;
				return;
			}
			do
			{
				if (base.LastNonEmpty.Type == FormatContainerType.TableCell)
				{
					this.CloseCell();
				}
				if (base.LastNonEmpty.Type == FormatContainerType.TableRow)
				{
					this.CloseRow(true);
				}
				this.CloseTable();
			}
			while (this.currentTableLevel < requiredTableLevel);
		}

		private void CloseList()
		{
			if (base.LastNonEmpty.Type == FormatContainerType.HyperLink)
			{
				base.CloseContainer();
			}
			if (base.LastNonEmpty.Type == FormatContainerType.Block)
			{
				base.CloseContainer();
			}
			if (base.LastNonEmpty.Type == FormatContainerType.ListItem)
			{
				base.CloseContainer();
			}
			if (base.LastNonEmpty.Type == FormatContainerType.List)
			{
				base.CloseContainer();
			}
			this.currentListState.ListIndex = -1;
		}

		private void OpenList()
		{
			if (base.LastNonEmpty.Type == FormatContainerType.HyperLink)
			{
				base.CloseContainer();
			}
			if (base.LastNonEmpty.Type == FormatContainerType.Block)
			{
				base.CloseContainer();
			}
			if (base.LastNonEmpty.Type == FormatContainerType.ListItem)
			{
				base.CloseContainer();
			}
			if (base.LastNonEmpty.Type != FormatContainerType.List)
			{
				if (base.LastNonEmpty.Type == FormatContainerType.Table)
				{
					this.OpenRow();
				}
				if (base.LastNonEmpty.Type == FormatContainerType.TableRow)
				{
					this.OpenCell();
				}
				base.OpenContainer(FormatContainerType.List, false);
				this.currentListState.ListIndex = this.state.ListIndex;
				this.currentListState.ListLevel = this.state.ListLevel;
				this.currentListState.ListStyle = this.state.ListStyle;
				this.currentListState.LeftIndent = this.state.LeftIndent;
				this.currentListState.RightIndent = this.state.RightIndent;
				this.currentListState.FirstLineIndent = this.state.FirstLineIndent;
				this.currentListState.Rtl = this.state.ParagraphRtl;
				base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.Margins, new PropertyValue(LengthUnits.Twips, 0));
				base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.RightMargin, new PropertyValue(LengthUnits.Twips, 0));
				base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.BottomMargin, new PropertyValue(LengthUnits.Twips, 0));
				base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.LeftMargin, new PropertyValue(LengthUnits.Twips, 0));
				if (this.lists[(int)this.currentListState.ListIndex].Levels[(int)this.currentListState.ListLevel].Start != 1)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.ListStart, new PropertyValue(PropertyType.Integer, (int)this.lists[(int)this.currentListState.ListIndex].Levels[(int)this.currentListState.ListLevel].Start));
				}
				if (this.state.ParagraphBackColor != 16777215)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.BackColor, new PropertyValue(new RGBT((uint)this.state.ParagraphBackColor)));
				}
				this.containerBackColor = this.state.ParagraphBackColor;
				if (this.state.ListStyle != RtfNumbering.Bullet)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.ListStyle, new PropertyValue((ListStyle)this.state.ListStyle));
				}
				if (this.state.LeftIndent != 0)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.LeftPadding, new PropertyValue(LengthUnits.Twips, (int)this.state.LeftIndent));
				}
				if (this.state.RightIndent != 0)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.RightMargin, new PropertyValue(LengthUnits.Twips, (int)this.state.RightIndent));
				}
				if (this.state.ParagraphAlignment != RtfAlignment.Left && this.state.ParagraphAlignment < RtfAlignment.Distributed)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.TextAlignment, new PropertyValue((TextAlign)this.state.ParagraphAlignment));
				}
				if (this.state.ParagraphRtl)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.RightToLeft, new PropertyValue(this.state.ParagraphRtl));
				}
				short fontIndex = this.state.FontIndex;
				if (this.state.FontIndex != -1 && this.state.FontIndex != this.documentFontIndex && this.fonts[(int)this.state.FontIndex].Name != null && !this.fonts[(int)this.state.FontIndex].Name.StartsWith("Wingdings", StringComparison.OrdinalIgnoreCase) && !this.fonts[(int)this.state.FontIndex].Name.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
				{
					if (this.fonts[(int)this.state.FontIndex].Value.IsNull)
					{
						this.RegisterFontValue(this.state.FontIndex);
					}
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontFace, this.fonts[(int)this.state.FontIndex].Value);
				}
				else
				{
					fontIndex = this.documentFontIndex;
				}
				this.containerFontIndex = fontIndex;
				if (this.state.FontSize != this.documentFontSize)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Twips, (int)this.state.FontSize));
				}
				this.containerFontSize = this.state.FontSize;
				if (this.state.FontColor != 0)
				{
					base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontColor, new PropertyValue(new RGBT((uint)this.state.FontColor)));
				}
				this.containerFontColor = this.state.FontColor;
				this.documentContainerStillOpen = false;
			}
		}

		private void InitializeFreshRowProperties()
		{
			if (this.state.Destination == RtfDestination.NestTableProps)
			{
				if (this.newRowNested != null)
				{
					this.newRowNested.NextFree = this.firstFreeRow;
					this.firstFreeRow = this.newRowNested;
				}
			}
			else if (this.newRowTopLevel != null)
			{
				this.newRowTopLevel.NextFree = this.firstFreeRow;
				this.firstFreeRow = this.newRowTopLevel;
			}
			if (this.firstFreeRow != null)
			{
				this.newRow = this.firstFreeRow;
				this.firstFreeRow = this.newRow.NextFree;
				this.newRow.NextFree = null;
				this.newRow.Initialize();
			}
			else
			{
				this.newRow = new RtfFormatConverter.RtfRow();
			}
			if (this.state.Destination == RtfDestination.NestTableProps)
			{
				this.newRowNested = this.newRow;
				return;
			}
			this.newRowTopLevel = this.newRow;
		}

		private void OpenTable()
		{
			if (this.tableStack == null)
			{
				this.tableStack = new RtfFormatConverter.RtfTable[4];
			}
			else if (this.currentTableLevel == this.tableStack.Length)
			{
				RtfFormatConverter.RtfTable[] destinationArray = new RtfFormatConverter.RtfTable[this.currentTableLevel * 2];
				Array.Copy(this.tableStack, 0, destinationArray, 0, this.currentTableLevel);
				this.tableStack = destinationArray;
			}
			this.currentTableLevel++;
			this.tableStack[this.currentTableLevel - 1].Initialize();
			this.tableStack[this.currentTableLevel - 1].TableContainer = base.OpenContainer(FormatContainerType.Table, false);
		}

		private void CloseTable()
		{
			FormatNode node = this.tableStack[this.currentTableLevel - 1].TableContainer.Node;
			FormatNode newChildNode = base.CreateNode(FormatContainerType.TableDefinition);
			node.PrependChild(newChildNode);
			for (int i = 0; i < (int)this.tableStack[this.currentTableLevel - 1].ColumnCount; i++)
			{
				FormatNode newChildNode2 = base.CreateNode(FormatContainerType.TableColumn);
				newChildNode2.SetProperty(PropertyId.Width, new PropertyValue(LengthUnits.Twips, (int)this.tableStack[this.currentTableLevel - 1].Columns[i].Width));
				newChildNode.AppendChild(newChildNode2);
			}
			base.CloseContainer();
			this.tableStack[this.currentTableLevel - 1].TableContainer = FormatConverterContainer.Null;
			this.currentTableLevel--;
		}

		private void OpenRow()
		{
			if (this.currentTableLevel == 1 && !this.tableStack[this.currentTableLevel - 1].FirstRow && this.newRowTopLevel != null && this.newRowTopLevel.CellCount != 0 && (this.newRowTopLevel.CellCount != this.tableStack[this.currentTableLevel - 1].ColumnCount || this.newRowTopLevel.Left != this.tableStack[this.currentTableLevel - 1].Left || this.newRowTopLevel.Right - this.newRowTopLevel.Left != this.tableStack[this.currentTableLevel - 1].Width))
			{
				this.CloseTable();
				this.OpenTable();
			}
			this.tableStack[this.currentTableLevel - 1].RowContainer = base.OpenContainer(FormatContainerType.TableRow, false);
			this.tableStack[this.currentTableLevel - 1].CellIndex = -1;
		}

		private void CloseRow(bool closingTable)
		{
			RtfFormatConverter.RtfRow rtfRow;
			if (this.currentTableLevel == 1)
			{
				rtfRow = this.newRowTopLevel;
			}
			else
			{
				rtfRow = this.newRowNested;
				this.newRowNested = null;
			}
			int num = this.currentTableLevel - 1;
			bool flag = false;
			if (rtfRow != null)
			{
				flag = rtfRow.LastRow;
				FormatNode node = this.tableStack[num].TableContainer.Node;
				FormatNode node2 = this.tableStack[num].RowContainer.Node;
				if (rtfRow.Height != 0)
				{
					node2.SetProperty(PropertyId.Height, new PropertyValue(LengthUnits.Twips, (int)rtfRow.Height));
				}
				if (this.tableStack[num].FirstRow)
				{
					if (rtfRow.Left != 0)
					{
						node.SetProperty(PropertyId.LeftMargin, new PropertyValue(LengthUnits.Twips, (int)rtfRow.Left));
					}
					node.SetProperty(PropertyId.Width, new PropertyValue(LengthUnits.Twips, (int)(rtfRow.Right - rtfRow.Left)));
					short count = Math.Max(rtfRow.CellCount, this.tableStack[num].CellIndex + 1);
					this.tableStack[num].RightToLeft = rtfRow.RightToLeft;
					this.tableStack[num].Left = rtfRow.Left;
					this.tableStack[num].Width = rtfRow.Right - rtfRow.Left;
					if (rtfRow.WidthType == 2)
					{
						this.tableStack[num].WidthPercentage = rtfRow.Width;
					}
					this.tableStack[num].BackColor = rtfRow.BackColor;
					this.tableStack[num].BorderKind = rtfRow.BorderKind;
					this.tableStack[num].BorderWidth = rtfRow.BorderWidth;
					this.tableStack[num].BorderColor = rtfRow.BorderColor;
					this.tableStack[num].EnsureColumns((int)count);
					FormatNode verticalMergeCell = node2.FirstChild;
					int num2 = 0;
					int num3 = (int)this.tableStack[num].Left;
					int num4 = 0;
					while (num4 < (int)rtfRow.CellCount && !verticalMergeCell.IsNull)
					{
						FormatNode nextSibling = verticalMergeCell.NextSibling;
						this.tableStack[num].Columns[num2].Cellx = rtfRow.Cells[num4].Cellx;
						this.tableStack[num].Columns[num2].Width = (short)((int)rtfRow.Cells[num4].Cellx - num3);
						if (rtfRow.Cells[num4].WidthType == 2)
						{
							this.tableStack[num].Columns[num2].WidthPercentage = rtfRow.Cells[num4].Width;
						}
						if (num4 == 0 || !rtfRow.Cells[num4].MergeLeft)
						{
							this.tableStack[num].Columns[num2].BackColor = rtfRow.Cells[num4].BackColor;
							this.tableStack[num].Columns[num2].VerticalMergeCell = verticalMergeCell;
							if (rtfRow.Cells[num4].BackColor != 16777215)
							{
								verticalMergeCell.SetProperty(PropertyId.BackColor, new PropertyValue(new RGBT((uint)rtfRow.Cells[num4].BackColor)));
							}
						}
						else
						{
							this.tableStack[num].Columns[num2].BackColor = this.tableStack[num].Columns[num4 - 1].BackColor;
							FormatNode previousSibling = verticalMergeCell.PreviousSibling;
							while (!verticalMergeCell.FirstChild.IsNull)
							{
								FormatNode firstChild = verticalMergeCell.FirstChild;
								firstChild.RemoveFromParent();
								previousSibling.AppendChild(firstChild);
							}
							verticalMergeCell.RemoveFromParent();
							int num5 = 1;
							PropertyValue property = previousSibling.GetProperty(PropertyId.NumColumns);
							if (!property.IsNull)
							{
								num5 = property.Integer;
							}
							previousSibling.SetProperty(PropertyId.NumColumns, new PropertyValue(PropertyType.Integer, num5 + 1));
							this.tableStack[num].Columns[num2].VerticalMergeCell = FormatNode.Null;
						}
						num3 = (int)rtfRow.Cells[num4].Cellx;
						num2++;
						verticalMergeCell = nextSibling;
						num4++;
					}
					this.tableStack[num].ColumnCount = (short)num4;
				}
				else if (rtfRow.Left == this.tableStack[num].Left && rtfRow.Right - rtfRow.Left == this.tableStack[num].Width)
				{
					FormatNode verticalMergeCell2 = node2.FirstChild;
					int num6 = 0;
					int num7 = 0;
					while (num7 < (int)rtfRow.CellCount && !verticalMergeCell2.IsNull && num6 < (int)this.tableStack[num].ColumnCount)
					{
						int num8 = num6;
						while (num8 < (int)this.tableStack[num].ColumnCount && rtfRow.Cells[num7].Cellx > this.tableStack[num].Columns[num8].Cellx)
						{
							num8++;
							if (num8 < (int)this.tableStack[num].ColumnCount)
							{
								this.tableStack[num].Columns[num8].VerticalMergeCell = FormatNode.Null;
							}
						}
						if (num8 < (int)this.tableStack[num].ColumnCount && rtfRow.Cells[num7].Cellx < this.tableStack[num].Columns[num8].Cellx)
						{
							this.tableStack[num].InsertColumn(num8, rtfRow.Cells[num7].Cellx);
							FormatNode x = node.FirstChild;
							while (x != node.LastChild)
							{
								FormatNode formatNode = x.FirstChild;
								int num9 = 0;
								while (!formatNode.IsNull)
								{
									int num10 = 1;
									PropertyValue property2 = formatNode.GetProperty(PropertyId.NumColumns);
									if (!property2.IsNull)
									{
										num10 = property2.Integer;
									}
									if (num9 + num10 > num8)
									{
										formatNode.SetProperty(PropertyId.NumColumns, new PropertyValue(PropertyType.Integer, num10 + 1));
										break;
									}
									num9 += num10;
									formatNode = formatNode.NextSibling;
								}
								x = x.NextSibling;
							}
						}
						FormatNode nextSibling2 = verticalMergeCell2.NextSibling;
						if (num7 != 0 && rtfRow.Cells[num7].MergeLeft)
						{
							this.tableStack[num].Columns[num6].VerticalMergeCell = FormatNode.Null;
							FormatNode previousSibling2 = verticalMergeCell2.PreviousSibling;
							while (!verticalMergeCell2.FirstChild.IsNull)
							{
								FormatNode firstChild2 = verticalMergeCell2.FirstChild;
								firstChild2.RemoveFromParent();
								previousSibling2.AppendChild(firstChild2);
							}
							verticalMergeCell2.RemoveFromParent();
							int num11 = 1;
							PropertyValue property3 = previousSibling2.GetProperty(PropertyId.NumColumns);
							if (!property3.IsNull)
							{
								num11 = property3.Integer;
							}
							previousSibling2.SetProperty(PropertyId.NumColumns, new PropertyValue(PropertyType.Integer, num11 + (num8 - num6 + 1)));
						}
						else
						{
							if (num8 != num6)
							{
								verticalMergeCell2.SetProperty(PropertyId.NumColumns, new PropertyValue(PropertyType.Integer, num8 - num6 + 1));
							}
							if (rtfRow.Cells[num7].MergeUp)
							{
								FormatNode verticalMergeCell3 = this.tableStack[num].Columns[num6].VerticalMergeCell;
								if (!verticalMergeCell3.IsNull)
								{
									verticalMergeCell2.SetProperty(PropertyId.MergedCell, new PropertyValue(true));
									int num12 = 1;
									PropertyValue property4 = verticalMergeCell3.GetProperty(PropertyId.NumRows);
									if (!property4.IsNull)
									{
										num12 = property4.Integer;
									}
									verticalMergeCell3.SetProperty(PropertyId.NumRows, new PropertyValue(PropertyType.Integer, num12 + 1));
								}
							}
							else
							{
								this.tableStack[num].Columns[num6].VerticalMergeCell = verticalMergeCell2;
								if (rtfRow.Cells[num7].BackColor != 16777215)
								{
									verticalMergeCell2.SetProperty(PropertyId.BackColor, new PropertyValue(new RGBT((uint)rtfRow.Cells[num7].BackColor)));
								}
							}
						}
						num6 = num8 + 1;
						verticalMergeCell2 = nextSibling2;
						num7++;
					}
				}
			}
			base.CloseContainer();
			this.tableStack[num].RowContainer = FormatConverterContainer.Null;
			this.tableStack[num].FirstRow = false;
			if (!closingTable && flag)
			{
				this.CloseTable();
			}
		}

		private void OpenCell()
		{
			RtfFormatConverter.RtfTable[] array = this.tableStack;
			int num = this.currentTableLevel - 1;
			array[num].CellIndex = array[num].CellIndex + 1;
			this.tableStack[this.currentTableLevel - 1].CellContainer = base.OpenContainer(FormatContainerType.TableCell, false);
		}

		private void CloseCell()
		{
			base.CloseContainer();
			this.tableStack[this.currentTableLevel - 1].CellContainer = FormatConverterContainer.Null;
		}

		private void SetBorderKind(RtfBorderKind kind)
		{
			switch (this.borderId)
			{
			case RtfBorderId.Left:
			case RtfBorderId.Top:
			case RtfBorderId.Right:
			case RtfBorderId.Bottom:
				this.state.SetParagraphBorderKind(this.borderId, kind);
				return;
			case RtfBorderId.RowLeft:
			case RtfBorderId.RowTop:
			case RtfBorderId.RowRight:
			case RtfBorderId.RowBottom:
			case RtfBorderId.RowHorizontal:
			case RtfBorderId.RowVertical:
				if (this.newRow != null)
				{
					this.newRow.SetBorderKind(this.borderId, kind);
					return;
				}
				break;
			case RtfBorderId.CellLeft:
			case RtfBorderId.CellTop:
			case RtfBorderId.CellRight:
			case RtfBorderId.CellBottom:
				if (this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
				{
					this.newRow.Cells[(int)this.newRow.CellCount].SetBorderKind(this.borderId, kind);
				}
				break;
			default:
				return;
			}
		}

		private void SetBorderColor(int color)
		{
			switch (this.borderId)
			{
			case RtfBorderId.Left:
			case RtfBorderId.Top:
			case RtfBorderId.Right:
			case RtfBorderId.Bottom:
				this.state.SetParagraphBorderColor(this.borderId, color);
				return;
			case RtfBorderId.RowLeft:
			case RtfBorderId.RowTop:
			case RtfBorderId.RowRight:
			case RtfBorderId.RowBottom:
			case RtfBorderId.RowHorizontal:
			case RtfBorderId.RowVertical:
				if (this.newRow != null)
				{
					this.newRow.SetBorderColor(this.borderId, color);
					return;
				}
				break;
			case RtfBorderId.CellLeft:
			case RtfBorderId.CellTop:
			case RtfBorderId.CellRight:
			case RtfBorderId.CellBottom:
				if (this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
				{
					this.newRow.Cells[(int)this.newRow.CellCount].SetBorderColor(this.borderId, color);
				}
				break;
			default:
				return;
			}
		}

		private void SetBorderWidth(byte width)
		{
			switch (this.borderId)
			{
			case RtfBorderId.Left:
			case RtfBorderId.Top:
			case RtfBorderId.Right:
			case RtfBorderId.Bottom:
				this.state.SetParagraphBorderWidth(this.borderId, width);
				return;
			case RtfBorderId.RowLeft:
			case RtfBorderId.RowTop:
			case RtfBorderId.RowRight:
			case RtfBorderId.RowBottom:
			case RtfBorderId.RowHorizontal:
			case RtfBorderId.RowVertical:
				if (this.newRow != null)
				{
					this.newRow.SetBorderWidth(this.borderId, width);
					return;
				}
				break;
			case RtfBorderId.CellLeft:
			case RtfBorderId.CellTop:
			case RtfBorderId.CellRight:
			case RtfBorderId.CellBottom:
				if (this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
				{
					this.newRow.Cells[(int)this.newRow.CellCount].SetBorderWidth(this.borderId, width);
				}
				break;
			default:
				return;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static RtfFormatConverter()
		{
			string[] array = new string[8];
			array[1] = "serif";
			array[2] = "sans-serif";
			array[3] = "monospace";
			array[4] = "cursive";
			array[5] = "fantasy";
			RtfFormatConverter.familyGenericName = array;
		}

		private const short TableDefaultLeftIndent = 108;

		protected short listsCount;

		protected RtfFormatConverter.List[] lists;

		protected short[] listDirectory;

		protected short listDirectoryCount;

		protected short listIdx;

		protected short listLevel;

		private static readonly short TwelvePointsInTwips = 240;

		private static string[] familyGenericName;

		private RtfParser parser;

		private FormatOutput output;

		private bool firstKeyword;

		private bool ignorableDestination;

		private RtfFormatConverter.RtfState state;

		private short fontsCount;

		private RtfFont[] fonts;

		private int colorsCount;

		private int[] colors;

		private short containerFontIndex;

		private int containerFontColor;

		private int containerBackColor;

		private short containerFontSize;

		private bool documentContainerStillOpen;

		private short documentFontIndex;

		private short documentFontSize;

		private bool ignoreFieldResult;

		private bool treatNbspAsBreakable;

		private Injection injection;

		private int color;

		private ScratchBuffer scratch;

		private ScratchBuffer scratchAlt;

		private int pictureWidth;

		private int pictureHeight;

		private bool beforeContent = true;

		private RtfBorderId borderId;

		private RtfFormatConverter.RtfTable[] tableStack;

		private int currentTableLevel;

		private RtfFormatConverter.RtfRow newRow;

		private RtfFormatConverter.RtfRow newRowTopLevel;

		private RtfFormatConverter.RtfRow newRowNested;

		private int requiredTableLevel;

		private RtfFormatConverter.RtfRow firstFreeRow;

		private RtfFormatConverter.ListState currentListState;

		internal struct ListLevel
		{
			public RtfNumbering Type;

			public short Start;
		}

		internal struct List
		{
			public int Id;

			public byte LevelCount;

			public RtfFormatConverter.ListLevel[] Levels;
		}

		internal struct RtfFourSideValue<T>
		{
			public void Initialize(T value)
			{
				this.Bottom = value;
				this.Top = value;
				this.Right = value;
				this.Left = value;
			}

			public T Left;

			public T Top;

			public T Right;

			public T Bottom;
		}

		internal struct RtfSixSideValue<T>
		{
			public void Initialize(T value)
			{
				this.Vertical = value;
				this.Horizontal = value;
				this.Bottom = value;
				this.Top = value;
				this.Right = value;
				this.Left = value;
			}

			public T Left;

			public T Top;

			public T Right;

			public T Bottom;

			public T Horizontal;

			public T Vertical;
		}

		internal struct RtfRowCell
		{
			public void Initialize()
			{
				this.BackColor = 16777215;
				this.Width = 0;
				this.WidthType = 0;
				this.Cellx = 0;
				this.VerticalAlignment = RtfVertAlignment.Undefined;
				this.MergeLeft = false;
				this.MergeUp = false;
				this.Padding.Initialize(-1);
				this.BorderWidth.Initialize(byte.MaxValue);
				this.BorderColor.Initialize(-1);
				this.BorderKind.Initialize(RtfBorderKind.None);
			}

			public void SetBorderKind(RtfBorderId borderId, RtfBorderKind value)
			{
				switch (borderId)
				{
				case RtfBorderId.CellLeft:
					this.BorderKind.Left = value;
					return;
				case RtfBorderId.CellTop:
					this.BorderKind.Top = value;
					return;
				case RtfBorderId.CellRight:
					this.BorderKind.Right = value;
					return;
				case RtfBorderId.CellBottom:
					this.BorderKind.Bottom = value;
					return;
				default:
					return;
				}
			}

			public void SetBorderWidth(RtfBorderId borderId, byte value)
			{
				switch (borderId)
				{
				case RtfBorderId.CellLeft:
					this.BorderWidth.Left = value;
					return;
				case RtfBorderId.CellTop:
					this.BorderWidth.Top = value;
					return;
				case RtfBorderId.CellRight:
					this.BorderWidth.Right = value;
					return;
				case RtfBorderId.CellBottom:
					this.BorderWidth.Bottom = value;
					return;
				default:
					return;
				}
			}

			public void SetBorderColor(RtfBorderId borderId, int value)
			{
				switch (borderId)
				{
				case RtfBorderId.CellLeft:
					this.BorderColor.Left = value;
					return;
				case RtfBorderId.CellTop:
					this.BorderColor.Top = value;
					return;
				case RtfBorderId.CellRight:
					this.BorderColor.Right = value;
					return;
				case RtfBorderId.CellBottom:
					this.BorderColor.Bottom = value;
					return;
				default:
					return;
				}
			}

			public bool MergeLeft;

			public bool MergeUp;

			public short Cellx;

			public short Width;

			public byte WidthType;

			public int BackColor;

			public RtfFormatConverter.RtfFourSideValue<byte> BorderWidth;

			public RtfFormatConverter.RtfFourSideValue<int> BorderColor;

			public RtfFormatConverter.RtfFourSideValue<RtfBorderKind> BorderKind;

			public RtfFormatConverter.RtfFourSideValue<short> Padding;

			public RtfVertAlignment VerticalAlignment;
		}

		internal struct RtfTable
		{
			public void SetBorderKind(RtfBorderId borderId, RtfBorderKind value)
			{
				switch (borderId)
				{
				case RtfBorderId.RowLeft:
					this.BorderKind.Left = value;
					return;
				case RtfBorderId.RowTop:
					this.BorderKind.Top = value;
					return;
				case RtfBorderId.RowRight:
					this.BorderKind.Right = value;
					return;
				case RtfBorderId.RowBottom:
					this.BorderKind.Bottom = value;
					return;
				case RtfBorderId.RowHorizontal:
					this.BorderKind.Horizontal = value;
					return;
				case RtfBorderId.RowVertical:
					this.BorderKind.Vertical = value;
					return;
				default:
					return;
				}
			}

			public void SetBorderWidth(RtfBorderId borderId, byte value)
			{
				switch (borderId)
				{
				case RtfBorderId.RowLeft:
					this.BorderWidth.Left = value;
					return;
				case RtfBorderId.RowTop:
					this.BorderWidth.Top = value;
					return;
				case RtfBorderId.RowRight:
					this.BorderWidth.Right = value;
					return;
				case RtfBorderId.RowBottom:
					this.BorderWidth.Bottom = value;
					return;
				case RtfBorderId.RowHorizontal:
					this.BorderWidth.Horizontal = value;
					return;
				case RtfBorderId.RowVertical:
					this.BorderWidth.Vertical = value;
					return;
				default:
					return;
				}
			}

			public void SetBorderColor(RtfBorderId borderId, int value)
			{
				switch (borderId)
				{
				case RtfBorderId.RowLeft:
					this.BorderColor.Left = value;
					return;
				case RtfBorderId.RowTop:
					this.BorderColor.Top = value;
					return;
				case RtfBorderId.RowRight:
					this.BorderColor.Right = value;
					return;
				case RtfBorderId.RowBottom:
					this.BorderColor.Bottom = value;
					return;
				case RtfBorderId.RowHorizontal:
					this.BorderColor.Horizontal = value;
					return;
				case RtfBorderId.RowVertical:
					this.BorderColor.Vertical = value;
					return;
				default:
					return;
				}
			}

			public void Initialize()
			{
				this.FirstRow = true;
				this.RightToLeft = false;
				this.Left = 0;
				this.Width = 0;
				this.WidthPercentage = 0;
				this.BackColor = 16777215;
				this.BorderWidth.Initialize(0);
				this.BorderColor.Initialize(0);
				this.BorderKind.Initialize(RtfBorderKind.None);
				this.ColumnCount = 0;
				if (this.Columns != null)
				{
					this.InitializeColumns(0);
				}
			}

			public void EnsureColumns(int count)
			{
				if (this.Columns == null)
				{
					this.Columns = new RtfFormatConverter.RtfTableColumn[Math.Max(count, 4)];
					this.InitializeColumns(0);
					return;
				}
				if (this.Columns.Length < count)
				{
					RtfFormatConverter.RtfTableColumn[] array = new RtfFormatConverter.RtfTableColumn[Math.Max(count, this.Columns.Length * 2)];
					if (this.ColumnCount != 0)
					{
						Array.Copy(this.Columns, 0, array, 0, (int)this.ColumnCount);
					}
					this.Columns = array;
					this.InitializeColumns((int)this.ColumnCount);
				}
			}

			public void InsertColumn(int index, short cellx)
			{
				this.EnsureColumns((int)(this.ColumnCount + 1));
				Array.Copy(this.Columns, index, this.Columns, index + 1, (int)this.ColumnCount - index);
				this.ColumnCount += 1;
				this.Columns[index].Cellx = cellx;
				short width = this.Columns[index].Width;
				this.Columns[index].Width = this.Columns[index].Cellx - ((index == 0) ? this.Left : this.Columns[index - 1].Cellx);
				this.Columns[index + 1].Width = this.Columns[index + 1].Cellx - cellx;
				if (this.Columns[index].WidthPercentage != 0)
				{
					this.Columns[index].WidthPercentage = this.Columns[index].WidthPercentage * this.Columns[index].Width / width;
					this.Columns[index + 1].WidthPercentage = this.Columns[index + 1].WidthPercentage * this.Columns[index + 1].Width / width;
				}
			}

			private void InitializeColumns(int startIndex)
			{
				for (int i = startIndex; i < this.Columns.Length; i++)
				{
					this.Columns[i].Initialize();
				}
			}

			public bool FirstRow;

			public bool RightToLeft;

			public short Left;

			public short Width;

			public short WidthPercentage;

			public int BackColor;

			public RtfFormatConverter.RtfSixSideValue<byte> BorderWidth;

			public RtfFormatConverter.RtfSixSideValue<int> BorderColor;

			public RtfFormatConverter.RtfSixSideValue<RtfBorderKind> BorderKind;

			public short ColumnCount;

			public RtfFormatConverter.RtfTableColumn[] Columns;

			public short CellIndex;

			public FormatConverterContainer TableContainer;

			public FormatConverterContainer RowContainer;

			public FormatConverterContainer CellContainer;
		}

		internal struct RtfTableColumn
		{
			public void Initialize()
			{
				this.Cellx = 0;
				this.Width = 0;
				this.WidthPercentage = 0;
				this.BackColor = 16777215;
				this.VerticalMergeCell = FormatNode.Null;
			}

			public short Cellx;

			public short Width;

			public short WidthPercentage;

			public int BackColor;

			public FormatNode VerticalMergeCell;
		}

		internal struct RtfState
		{
			public bool TextPropertiesChanged
			{
				get
				{
					return this.textPropertiesChanged;
				}
				set
				{
					this.textPropertiesChanged = value;
				}
			}

			public RtfDestination Destination
			{
				get
				{
					return this.basic.Destination;
				}
				set
				{
					if (this.basic.Destination != value)
					{
						this.DirtyBasicState();
						this.basic.Destination = value;
					}
				}
			}

			public bool BiDi
			{
				get
				{
					return (ushort)(this.font.Flags & RtfFormatConverter.RtfState.FontFlags.BiDi) != 0;
				}
				set
				{
					if (this.BiDi != value)
					{
						this.DirtyFontProps();
						this.font.Flags = (this.font.Flags ^ RtfFormatConverter.RtfState.FontFlags.BiDi);
					}
				}
			}

			public bool ComplexScript
			{
				get
				{
					return (ushort)(this.font.Flags & RtfFormatConverter.RtfState.FontFlags.ComplexScript) != 0;
				}
				set
				{
					if (this.ComplexScript != value)
					{
						this.DirtyFontProps();
						this.font.Flags = (this.font.Flags ^ RtfFormatConverter.RtfState.FontFlags.ComplexScript);
					}
				}
			}

			public bool Bold
			{
				get
				{
					return (ushort)(this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Bold) != 0;
				}
				set
				{
					if (this.Bold != value)
					{
						this.DirtyFontProps();
						this.font.Flags = (this.font.Flags ^ RtfFormatConverter.RtfState.FontFlags.Bold);
					}
				}
			}

			public bool Italic
			{
				get
				{
					return (ushort)(this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Italic) != 0;
				}
				set
				{
					if (this.Italic != value)
					{
						this.DirtyFontProps();
						this.font.Flags = (this.font.Flags ^ RtfFormatConverter.RtfState.FontFlags.Italic);
					}
				}
			}

			public bool Underline
			{
				get
				{
					return (ushort)(this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Underline) != 0;
				}
				set
				{
					if (this.Underline != value)
					{
						this.DirtyFontProps();
						this.font.Flags = (this.font.Flags ^ RtfFormatConverter.RtfState.FontFlags.Underline);
					}
				}
			}

			public bool Subscript
			{
				get
				{
					return (ushort)(this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Subscript) != 0;
				}
				set
				{
					if (this.Subscript != value)
					{
						this.DirtyFontProps();
						this.font.Flags = (this.font.Flags ^ RtfFormatConverter.RtfState.FontFlags.Subscript);
						if (value)
						{
							this.Superscript = false;
						}
					}
				}
			}

			public bool Superscript
			{
				get
				{
					return (ushort)(this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Superscript) != 0;
				}
				set
				{
					if (this.Superscript != value)
					{
						this.DirtyFontProps();
						this.font.Flags = (this.font.Flags ^ RtfFormatConverter.RtfState.FontFlags.Superscript);
						if (value)
						{
							this.Subscript = false;
						}
					}
				}
			}

			public bool Strikethrough
			{
				get
				{
					return (ushort)(this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Strikethrough) != 0;
				}
				set
				{
					if (this.Strikethrough != value)
					{
						this.DirtyFontProps();
						this.font.Flags = (this.font.Flags ^ RtfFormatConverter.RtfState.FontFlags.Strikethrough);
					}
				}
			}

			public bool SmallCaps
			{
				get
				{
					return (ushort)(this.font.Flags & RtfFormatConverter.RtfState.FontFlags.SmallCaps) != 0;
				}
				set
				{
					if (this.SmallCaps != value)
					{
						this.DirtyFontProps();
						this.font.Flags = (this.font.Flags ^ RtfFormatConverter.RtfState.FontFlags.SmallCaps);
					}
				}
			}

			public bool Capitalize
			{
				get
				{
					return (ushort)(this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Capitalize) != 0;
				}
				set
				{
					if (this.Capitalize != value)
					{
						this.DirtyFontProps();
						this.font.Flags = (this.font.Flags ^ RtfFormatConverter.RtfState.FontFlags.Capitalize);
					}
				}
			}

			public bool Invisible
			{
				get
				{
					return (ushort)(this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Invisible) != 0;
				}
				set
				{
					if (this.Invisible != value)
					{
						this.DirtyFontProps();
						this.font.Flags = (this.font.Flags ^ RtfFormatConverter.RtfState.FontFlags.Invisible);
					}
				}
			}

			public short FontIndex
			{
				get
				{
					return this.font.FontIndex;
				}
				set
				{
					if (this.font.FontIndex != value)
					{
						this.DirtyFontProps();
						this.font.FontIndex = value;
					}
				}
			}

			public short FontSize
			{
				get
				{
					return this.font.FontSize;
				}
				set
				{
					if (this.font.FontSize != value)
					{
						this.DirtyFontProps();
						this.font.FontSize = value;
					}
				}
			}

			public int FontColor
			{
				get
				{
					return this.font.FontColor;
				}
				set
				{
					if (this.font.FontColor != value)
					{
						this.DirtyFontProps();
						this.font.FontColor = value;
					}
				}
			}

			public int FontBackColor
			{
				get
				{
					return this.font.FontBackColor;
				}
				set
				{
					if (this.font.FontBackColor != value)
					{
						this.DirtyFontProps();
						this.font.FontBackColor = value;
					}
				}
			}

			public short Language
			{
				get
				{
					return this.font.Language;
				}
				set
				{
					if (this.font.Language != value)
					{
						this.DirtyFontProps();
						this.font.Language = value;
					}
				}
			}

			public bool Preformatted
			{
				get
				{
					return (byte)(this.paragraph.Flags & RtfFormatConverter.RtfState.ParagraphFlags.Preformatted) != 0;
				}
				set
				{
					if (this.Preformatted != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.Flags = (this.paragraph.Flags ^ RtfFormatConverter.RtfState.ParagraphFlags.Preformatted);
					}
				}
			}

			public bool ParagraphRtl
			{
				get
				{
					return (byte)(this.paragraph.Flags & RtfFormatConverter.RtfState.ParagraphFlags.ParagraphRtl) != 0;
				}
				set
				{
					if (this.ParagraphRtl != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.Flags = (this.paragraph.Flags ^ RtfFormatConverter.RtfState.ParagraphFlags.ParagraphRtl);
					}
				}
			}

			public short FirstLineIndent
			{
				get
				{
					return this.paragraph.FirstLineIndent;
				}
				set
				{
					if (this.paragraph.FirstLineIndent != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.FirstLineIndent = value;
					}
				}
			}

			public short LeftIndent
			{
				get
				{
					return this.paragraph.LeftIndent;
				}
				set
				{
					if (this.paragraph.LeftIndent != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.LeftIndent = value;
					}
				}
			}

			public short RightIndent
			{
				get
				{
					return this.paragraph.RightIndent;
				}
				set
				{
					if (this.paragraph.RightIndent != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.RightIndent = value;
					}
				}
			}

			public short SpaceBefore
			{
				get
				{
					return this.paragraph.SpaceBefore;
				}
				set
				{
					if (this.paragraph.SpaceBefore != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.SpaceBefore = value;
					}
				}
			}

			public short SpaceAfter
			{
				get
				{
					return this.paragraph.SpaceAfter;
				}
				set
				{
					if (this.paragraph.SpaceAfter != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.SpaceAfter = value;
					}
				}
			}

			public RtfAlignment ParagraphAlignment
			{
				get
				{
					return this.paragraph.Alignment;
				}
				set
				{
					if (this.paragraph.Alignment != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.Alignment = value;
					}
				}
			}

			public short ListIndex
			{
				get
				{
					return this.paragraph.ListIndex;
				}
				set
				{
					if (this.paragraph.ListIndex != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.ListIndex = value;
					}
				}
			}

			public byte ListLevel
			{
				get
				{
					return this.paragraph.ListLevel;
				}
				set
				{
					if (this.paragraph.ListLevel != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.ListLevel = value;
					}
				}
			}

			public RtfNumbering ListStyle
			{
				get
				{
					return this.paragraph.ListStyle;
				}
				set
				{
					if (this.paragraph.ListStyle != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.ListStyle = value;
					}
				}
			}

			public int ParagraphBackColor
			{
				get
				{
					return this.paragraph.BackColor;
				}
				set
				{
					if (this.paragraph.BackColor != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.BackColor = value;
					}
				}
			}

			public byte BorderWidthLeft
			{
				get
				{
					return this.paragraph.BorderWidth.Left;
				}
				set
				{
					if (this.paragraph.BorderWidth.Left != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.BorderWidth.Left = value;
					}
				}
			}

			public byte BorderWidthRight
			{
				get
				{
					return this.paragraph.BorderWidth.Right;
				}
				set
				{
					if (this.paragraph.BorderWidth.Right != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.BorderWidth.Right = value;
					}
				}
			}

			public byte BorderWidthTop
			{
				get
				{
					return this.paragraph.BorderWidth.Top;
				}
				set
				{
					if (this.paragraph.BorderWidth.Top != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.BorderWidth.Top = value;
					}
				}
			}

			public byte BorderWidthBottom
			{
				get
				{
					return this.paragraph.BorderWidth.Bottom;
				}
				set
				{
					if (this.paragraph.BorderWidth.Bottom != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.BorderWidth.Bottom = value;
					}
				}
			}

			public int BorderColorLeft
			{
				get
				{
					return this.paragraph.BorderColor.Left;
				}
				set
				{
					if (this.paragraph.BorderColor.Left != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.BorderColor.Left = value;
					}
				}
			}

			public int BorderColorRight
			{
				get
				{
					return this.paragraph.BorderColor.Right;
				}
				set
				{
					if (this.paragraph.BorderColor.Right != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.BorderColor.Right = value;
					}
				}
			}

			public int BorderColorTop
			{
				get
				{
					return this.paragraph.BorderColor.Top;
				}
				set
				{
					if (this.paragraph.BorderColor.Top != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.BorderColor.Top = value;
					}
				}
			}

			public int BorderColorBottom
			{
				get
				{
					return this.paragraph.BorderColor.Bottom;
				}
				set
				{
					if (this.paragraph.BorderColor.Bottom != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.BorderColor.Bottom = value;
					}
				}
			}

			public RtfBorderKind BorderKindLeft
			{
				get
				{
					return this.paragraph.BorderKind.Left;
				}
				set
				{
					if (this.paragraph.BorderKind.Left != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.BorderKind.Left = value;
					}
				}
			}

			public RtfBorderKind BorderKindRight
			{
				get
				{
					return this.paragraph.BorderKind.Right;
				}
				set
				{
					if (this.paragraph.BorderKind.Right != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.BorderKind.Right = value;
					}
				}
			}

			public RtfBorderKind BorderKindTop
			{
				get
				{
					return this.paragraph.BorderKind.Top;
				}
				set
				{
					if (this.paragraph.BorderKind.Top != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.BorderKind.Top = value;
					}
				}
			}

			public RtfBorderKind BorderKindBottom
			{
				get
				{
					return this.paragraph.BorderKind.Bottom;
				}
				set
				{
					if (this.paragraph.BorderKind.Bottom != value)
					{
						this.DirtyParagraphProps();
						this.paragraph.BorderKind.Bottom = value;
					}
				}
			}

			public RtfDestination ParentDestination
			{
				get
				{
					if (this.basic.Depth <= 1 && this.basicStackTop != 0)
					{
						return this.basicStack[this.basicStackTop - 1].Destination;
					}
					return this.basic.Destination;
				}
			}

			public short ParentFontIndex
			{
				get
				{
					if (this.font.Depth <= 1 && this.fontStackTop != 0)
					{
						return this.fontStack[this.fontStackTop - 1].FontIndex;
					}
					return this.font.FontIndex;
				}
			}

			public void Initialize()
			{
				this.basic.Depth = 1;
				this.font.Depth = 1;
				this.paragraph.Depth = 1;
				this.SetParagraphDefault();
				this.SetPlain();
				this.Destination = RtfDestination.RTF;
			}

			public byte GetParagraphBorderWidth(RtfBorderId borderId)
			{
				switch (borderId)
				{
				default:
					return this.BorderWidthLeft;
				case RtfBorderId.Top:
					return this.BorderWidthTop;
				case RtfBorderId.Right:
					return this.BorderWidthRight;
				case RtfBorderId.Bottom:
					return this.BorderWidthBottom;
				}
			}

			public void SetParagraphBorderWidth(RtfBorderId borderId, byte value)
			{
				switch (borderId)
				{
				case RtfBorderId.Left:
					this.BorderWidthLeft = value;
					return;
				case RtfBorderId.Top:
					this.BorderWidthTop = value;
					return;
				case RtfBorderId.Right:
					this.BorderWidthRight = value;
					return;
				case RtfBorderId.Bottom:
					this.BorderWidthBottom = value;
					return;
				default:
					return;
				}
			}

			public int GetParagraphBorderColor(RtfBorderId borderId)
			{
				switch (borderId)
				{
				default:
					return this.BorderColorLeft;
				case RtfBorderId.Top:
					return this.BorderColorTop;
				case RtfBorderId.Right:
					return this.BorderColorRight;
				case RtfBorderId.Bottom:
					return this.BorderColorBottom;
				}
			}

			public void SetParagraphBorderColor(RtfBorderId borderId, int value)
			{
				switch (borderId)
				{
				case RtfBorderId.Left:
					this.BorderColorLeft = value;
					return;
				case RtfBorderId.Top:
					this.BorderColorTop = value;
					return;
				case RtfBorderId.Right:
					this.BorderColorRight = value;
					return;
				case RtfBorderId.Bottom:
					this.BorderColorBottom = value;
					return;
				default:
					return;
				}
			}

			public RtfBorderKind GetParagraphBorderKind(RtfBorderId borderId)
			{
				switch (borderId)
				{
				default:
					return this.BorderKindLeft;
				case RtfBorderId.Top:
					return this.BorderKindTop;
				case RtfBorderId.Right:
					return this.BorderKindRight;
				case RtfBorderId.Bottom:
					return this.BorderKindBottom;
				}
			}

			public void SetParagraphBorderKind(RtfBorderId borderId, RtfBorderKind value)
			{
				switch (borderId)
				{
				case RtfBorderId.Left:
					this.BorderKindLeft = value;
					return;
				case RtfBorderId.Top:
					this.BorderKindTop = value;
					return;
				case RtfBorderId.Right:
					this.BorderKindRight = value;
					return;
				case RtfBorderId.Bottom:
					this.BorderKindBottom = value;
					return;
				default:
					return;
				}
			}

			public void SetParentFontIndex(short index)
			{
				this.font.FontIndex = index;
				if (this.fontStackTop > 0)
				{
					this.fontStack[this.fontStackTop - 1].FontIndex = index;
				}
			}

			public void SetPlain()
			{
				this.DirtyFontProps();
				this.font.Flags = RtfFormatConverter.RtfState.FontFlags.None;
				this.font.FontIndex = 0;
				this.font.FontSize = RtfFormatConverter.TwelvePointsInTwips;
				this.font.Language = 0;
				this.font.FontColor = 0;
				this.font.FontBackColor = 16777215;
			}

			public void SetParagraphDefault()
			{
				this.DirtyParagraphProps();
				this.paragraph.Flags = RtfFormatConverter.RtfState.ParagraphFlags.None;
				this.paragraph.Alignment = RtfAlignment.Left;
				this.paragraph.SpaceBefore = 0;
				this.paragraph.SpaceAfter = 0;
				this.paragraph.LeftIndent = 0;
				this.paragraph.RightIndent = 0;
				this.paragraph.FirstLineIndent = 0;
				this.paragraph.ListIndex = -1;
				this.paragraph.ListLevel = 0;
				this.paragraph.ListStyle = RtfNumbering.None;
				this.paragraph.BackColor = 16777215;
				this.paragraph.BorderWidth.Initialize(0);
				this.paragraph.BorderColor.Initialize(-1);
				this.paragraph.BorderKind.Initialize(RtfBorderKind.None);
			}

			public void Push()
			{
				this.Level++;
				this.basic.Depth = this.basic.Depth + 1;
				this.font.Depth = this.font.Depth + 1;
				this.paragraph.Depth = this.paragraph.Depth + 1;
				if (this.basic.Depth == 32767)
				{
					this.PushBasicState();
				}
				if (this.font.Depth == 32767)
				{
					this.PushFontProps();
				}
				if (this.paragraph.Depth == 32767)
				{
					this.PushParagraphProps();
				}
			}

			public void Pop()
			{
				if (this.Level > 1)
				{
					if ((this.basic.Depth = this.basic.Depth - 1) == 0)
					{
						this.basic = this.basicStack[--this.basicStackTop];
					}
					if ((this.font.Depth = this.font.Depth - 1) == 0)
					{
						this.font = this.fontStack[--this.fontStackTop];
						this.textPropertiesChanged = true;
					}
					if ((this.paragraph.Depth = this.paragraph.Depth - 1) == 0)
					{
						this.paragraph = this.paragraphStack[--this.paragraphStackTop];
					}
					this.Level--;
				}
			}

			public void SkipGroup()
			{
				this.SkipLevel = this.Level;
			}

			private void DirtyFontProps()
			{
				this.textPropertiesChanged = true;
				if (this.font.Depth > 1)
				{
					this.PushFontProps();
				}
			}

			private void PushFontProps()
			{
				if (this.fontStackTop >= 4096)
				{
					throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
				}
				if (this.fontStack == null || this.fontStackTop == this.fontStack.Length)
				{
					RtfFormatConverter.RtfState.FontProperties[] destinationArray;
					if (this.fontStack != null)
					{
						destinationArray = new RtfFormatConverter.RtfState.FontProperties[this.fontStackTop * 2];
						Array.Copy(this.fontStack, 0, destinationArray, 0, this.fontStackTop);
					}
					else
					{
						destinationArray = new RtfFormatConverter.RtfState.FontProperties[8];
					}
					this.fontStack = destinationArray;
				}
				this.fontStack[this.fontStackTop] = this.font;
				RtfFormatConverter.RtfState.FontProperties[] array = this.fontStack;
				int num = this.fontStackTop;
				array[num].Depth = array[num].Depth - 1;
				this.fontStackTop++;
				this.font.Depth = 1;
			}

			private void DirtyParagraphProps()
			{
				if (this.paragraph.Depth > 1)
				{
					this.PushParagraphProps();
				}
			}

			private void PushParagraphProps()
			{
				if (this.paragraphStackTop >= 4096)
				{
					throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
				}
				if (this.paragraphStack == null || this.paragraphStackTop == this.paragraphStack.Length)
				{
					RtfFormatConverter.RtfState.ParagraphProperties[] destinationArray;
					if (this.paragraphStack != null)
					{
						destinationArray = new RtfFormatConverter.RtfState.ParagraphProperties[this.paragraphStackTop * 2];
						Array.Copy(this.paragraphStack, 0, destinationArray, 0, this.paragraphStackTop);
					}
					else
					{
						destinationArray = new RtfFormatConverter.RtfState.ParagraphProperties[4];
					}
					this.paragraphStack = destinationArray;
				}
				this.paragraphStack[this.paragraphStackTop] = this.paragraph;
				RtfFormatConverter.RtfState.ParagraphProperties[] array = this.paragraphStack;
				int num = this.paragraphStackTop;
				array[num].Depth = array[num].Depth - 1;
				this.paragraphStackTop++;
				this.paragraph.Depth = 1;
			}

			private void DirtyBasicState()
			{
				if (this.basic.Depth > 1)
				{
					this.PushBasicState();
				}
			}

			private void PushBasicState()
			{
				if (this.basicStackTop >= 4096)
				{
					throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
				}
				if (this.basicStack == null || this.basicStackTop == this.basicStack.Length)
				{
					RtfFormatConverter.RtfState.BasicState[] destinationArray;
					if (this.basicStack != null)
					{
						destinationArray = new RtfFormatConverter.RtfState.BasicState[this.basicStackTop * 2];
						Array.Copy(this.basicStack, 0, destinationArray, 0, this.basicStackTop);
					}
					else
					{
						destinationArray = new RtfFormatConverter.RtfState.BasicState[16];
					}
					this.basicStack = destinationArray;
				}
				this.basicStack[this.basicStackTop] = this.basic;
				RtfFormatConverter.RtfState.BasicState[] array = this.basicStack;
				int num = this.basicStackTop;
				array[num].Depth = array[num].Depth - 1;
				this.basicStackTop++;
				this.basic.Depth = 1;
			}

			public int Level;

			public int SkipLevel;

			private RtfFormatConverter.RtfState.BasicState basic;

			private RtfFormatConverter.RtfState.BasicState[] basicStack;

			private int basicStackTop;

			private RtfFormatConverter.RtfState.FontProperties font;

			private RtfFormatConverter.RtfState.FontProperties[] fontStack;

			private int fontStackTop;

			private RtfFormatConverter.RtfState.ParagraphProperties paragraph;

			private RtfFormatConverter.RtfState.ParagraphProperties[] paragraphStack;

			private int paragraphStackTop;

			private bool textPropertiesChanged;

			internal enum FontFlagsIndex : byte
			{
				BiDi,
				ComplexScript,
				Bold,
				Italic,
				Underline,
				Subscript,
				Superscript,
				Strikethrough,
				SmallCaps,
				Capitalize,
				Invisible
			}

			[Flags]
			internal enum FontFlags : ushort
			{
				None = 0,
				BiDi = 1,
				ComplexScript = 2,
				Bold = 4,
				Italic = 8,
				Underline = 16,
				Subscript = 32,
				Superscript = 64,
				Strikethrough = 128,
				SmallCaps = 256,
				Capitalize = 512,
				Invisible = 1024
			}

			internal enum ParagraphFlagsIndex : byte
			{
				ParagraphRtl,
				Preformatted
			}

			[Flags]
			internal enum ParagraphFlags : byte
			{
				None = 0,
				ParagraphRtl = 1,
				Preformatted = 2
			}

			internal struct BasicState
			{
				public short Depth;

				public RtfDestination Destination;
			}

			internal struct FontProperties
			{
				public short Depth;

				public RtfFormatConverter.RtfState.FontFlags Flags;

				public short FontIndex;

				public short FontSize;

				public short Language;

				public int FontColor;

				public int FontBackColor;
			}

			internal struct ParagraphProperties
			{
				public short Depth;

				public RtfFormatConverter.RtfState.ParagraphFlags Flags;

				public RtfAlignment Alignment;

				public short FirstLineIndent;

				public short LeftIndent;

				public short RightIndent;

				public short SpaceBefore;

				public short SpaceAfter;

				public RtfFormatConverter.RtfFourSideValue<byte> BorderWidth;

				public RtfFormatConverter.RtfFourSideValue<int> BorderColor;

				public RtfFormatConverter.RtfFourSideValue<RtfBorderKind> BorderKind;

				public int BackColor;

				public short ListIndex;

				public byte ListLevel;

				public RtfNumbering ListStyle;
			}
		}

		private struct ListState
		{
			public short ListIndex;

			public byte ListLevel;

			public RtfNumbering ListStyle;

			public bool Rtl;

			public short LeftIndent;

			public short RightIndent;

			public short FirstLineIndent;
		}

		internal class RtfRow
		{
			public RtfRow()
			{
				this.BackColor = 16777215;
			}

			public void Initialize()
			{
				this.CellCount = 0;
				if (this.Cells != null)
				{
					this.InitializeCells(0);
				}
				this.RightToLeft = false;
				this.HeaderRow = false;
				this.LastRow = false;
				this.Left = 0;
				this.Right = 0;
				this.Width = 0;
				this.WidthType = 0;
				this.Height = 0;
				this.HeightExact = false;
				this.BackColor = 16777215;
				this.CellPadding.Initialize(0);
				this.BorderWidth.Initialize(0);
				this.BorderColor.Initialize(0);
				this.BorderKind.Initialize(RtfBorderKind.None);
			}

			public bool EnsureEntryForCurrentCell()
			{
				if (this.Cells == null)
				{
					this.Cells = new RtfFormatConverter.RtfRowCell[4];
					this.InitializeCells(0);
				}
				else if ((int)this.CellCount == this.Cells.Length)
				{
					if (this.CellCount >= 64)
					{
						return false;
					}
					RtfFormatConverter.RtfRowCell[] array = new RtfFormatConverter.RtfRowCell[this.Cells.Length * 2];
					Array.Copy(this.Cells, 0, array, 0, (int)this.CellCount);
					this.Cells = array;
					this.InitializeCells((int)this.CellCount);
				}
				return true;
			}

			public void SetBorderKind(RtfBorderId borderId, RtfBorderKind value)
			{
				switch (borderId)
				{
				case RtfBorderId.RowLeft:
					this.BorderKind.Left = value;
					return;
				case RtfBorderId.RowTop:
					this.BorderKind.Top = value;
					return;
				case RtfBorderId.RowRight:
					this.BorderKind.Right = value;
					return;
				case RtfBorderId.RowBottom:
					this.BorderKind.Bottom = value;
					return;
				case RtfBorderId.RowHorizontal:
					this.BorderKind.Horizontal = value;
					return;
				case RtfBorderId.RowVertical:
					this.BorderKind.Vertical = value;
					return;
				default:
					return;
				}
			}

			public void SetBorderWidth(RtfBorderId borderId, byte value)
			{
				switch (borderId)
				{
				case RtfBorderId.RowLeft:
					this.BorderWidth.Left = value;
					return;
				case RtfBorderId.RowTop:
					this.BorderWidth.Top = value;
					return;
				case RtfBorderId.RowRight:
					this.BorderWidth.Right = value;
					return;
				case RtfBorderId.RowBottom:
					this.BorderWidth.Bottom = value;
					return;
				case RtfBorderId.RowHorizontal:
					this.BorderWidth.Horizontal = value;
					return;
				case RtfBorderId.RowVertical:
					this.BorderWidth.Vertical = value;
					return;
				default:
					return;
				}
			}

			public void SetBorderColor(RtfBorderId borderId, int value)
			{
				switch (borderId)
				{
				case RtfBorderId.RowLeft:
					this.BorderColor.Left = value;
					return;
				case RtfBorderId.RowTop:
					this.BorderColor.Top = value;
					return;
				case RtfBorderId.RowRight:
					this.BorderColor.Right = value;
					return;
				case RtfBorderId.RowBottom:
					this.BorderColor.Bottom = value;
					return;
				case RtfBorderId.RowHorizontal:
					this.BorderColor.Horizontal = value;
					return;
				case RtfBorderId.RowVertical:
					this.BorderColor.Vertical = value;
					return;
				default:
					return;
				}
			}

			private void InitializeCells(int startIndex)
			{
				for (int i = startIndex; i < this.Cells.Length; i++)
				{
					this.Cells[i].Initialize();
				}
			}

			public RtfFormatConverter.RtfRow NextFree;

			public short CellCount;

			public RtfFormatConverter.RtfRowCell[] Cells;

			public bool RightToLeft;

			public bool HeaderRow;

			public bool LastRow;

			public short Left;

			public short Right;

			public short Width;

			public byte WidthType;

			public short Height;

			public bool HeightExact;

			public int BackColor;

			public RtfFormatConverter.RtfFourSideValue<short> CellPadding;

			public RtfFormatConverter.RtfSixSideValue<byte> BorderWidth;

			public RtfFormatConverter.RtfSixSideValue<int> BorderColor;

			public RtfFormatConverter.RtfSixSideValue<RtfBorderKind> BorderKind;
		}
	}
}
