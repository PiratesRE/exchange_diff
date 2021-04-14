using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal abstract class ConversationBodyScanner : IProgressMonitor
	{
		public int InputStreamBufferSize
		{
			get
			{
				return this.InputBufferSize;
			}
			set
			{
				this.AssertNotLocked();
				if (value < 1024 || value > 81920)
				{
					throw new ArgumentOutOfRangeException("value", TextConvertersStrings.BufferSizeValueRange);
				}
				this.InputBufferSize = value;
			}
		}

		public bool FilterHtml
		{
			get
			{
				return this.filterHtml;
			}
			set
			{
				this.AssertNotLocked();
				this.filterHtml = value;
			}
		}

		public HtmlTagCallback HtmlTagCallback
		{
			get
			{
				return this.HtmlCallback;
			}
			set
			{
				this.AssertNotLocked();
				this.HtmlCallback = value;
			}
		}

		public IList<TextRun> Words
		{
			get
			{
				return this.scanner.Words;
			}
		}

		public IList<ConversationBodyScanner.Scanner.FragmentInfo> Fragments
		{
			get
			{
				return this.scanner.Fragments;
			}
		}

		public IList<ConversationBodyScanner.Scanner.LineInfo> Lines
		{
			get
			{
				return this.scanner.Lines;
			}
		}

		public FormatStore FormatStore
		{
			get
			{
				return this.scanner.Store;
			}
		}

		internal bool TestBoundaryConditions
		{
			get
			{
				return this.testBoundaryConditions;
			}
			set
			{
				this.AssertNotLocked();
				this.testBoundaryConditions = value;
			}
		}

		internal int CountQuotingLevels
		{
			get
			{
				return this.scanner.CountQuotingLevels;
			}
		}

		public void Load(Stream sourceStream)
		{
			if (sourceStream == null)
			{
				throw new ArgumentNullException("sourceStream");
			}
			if (!sourceStream.CanRead)
			{
				throw new ArgumentException(TextConvertersStrings.CannotReadFromSource, "sourceStream");
			}
			FormatConverter converter = this.CreatePullChain(sourceStream, this);
			this.Load(converter, 100000 + this.InputStreamBufferSize);
		}

		public void Load(TextReader sourceReader)
		{
			if (sourceReader == null)
			{
				throw new ArgumentNullException("sourceReader");
			}
			FormatConverter converter = this.CreatePullChain(sourceReader, this);
			this.Load(converter, 100000 + this.InputStreamBufferSize);
		}

		public void WriteBody(HtmlWriter writer)
		{
			this.scanner.WriteBody(writer, 0);
		}

		public void WriteAll(HtmlWriter writer)
		{
			this.scanner.WriteAll(writer, 0);
		}

		public void WriteLines(HtmlWriter writer, int fromLineIndex, int tillLineIndex)
		{
			this.scanner.WriteLines(writer, fromLineIndex, tillLineIndex);
		}

		internal bool IsHeading(TextRun run, string[] headingWords)
		{
			return this.scanner.IsHeading(run, headingWords);
		}

		void IProgressMonitor.ReportProgress()
		{
			this.madeProgress = true;
		}

		internal void WriteBody(int level, HtmlWriter writer)
		{
			this.scanner.WriteBody(writer, level);
		}

		internal abstract FormatConverter CreatePullChain(Stream sourceStream, IProgressMonitor progressMonitor);

		internal abstract FormatConverter CreatePullChain(TextReader sourceReader, IProgressMonitor progressMonitor);

		internal void AssertNotLocked()
		{
			if (this.Locked)
			{
				throw new InvalidOperationException(TextConvertersStrings.ParametersCannotBeChangedAfterConverterObjectIsUsed);
			}
		}

		internal int CalculateLatestMessagePartWordCount()
		{
			int num = int.MaxValue;
			for (int i = 0; i < this.Fragments.Count; i++)
			{
				if (this.Fragments[i].Category == ConversationBodyScanner.Scanner.FragmentCategory.MsHeader || this.Fragments[i].Category == ConversationBodyScanner.Scanner.FragmentCategory.NonMsHeader)
				{
					num = this.Fragments[i].FirstWord;
					break;
				}
			}
			if (num == 2147483647)
			{
				num = this.Words.Count;
			}
			return num;
		}

		private void Load(FormatConverter converter, int maxLoopsWithoutProgress)
		{
			long num = 0L;
			while (!converter.EndOfFile)
			{
				converter.Run();
				if (this.madeProgress)
				{
					this.madeProgress = false;
					num = 0L;
				}
				else
				{
					long num2 = (long)maxLoopsWithoutProgress;
					long num3 = num;
					num = num3 + 1L;
					if (num2 == num3)
					{
						throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToProduceOutput);
					}
				}
			}
			this.scanner = new ConversationBodyScanner.Scanner(converter.Store, this.filterHtml, this.HtmlCallback, this.RecognizeHyperlinks, this.TestFormatTraceStream, this.TestFormatOutputTraceStream, this.testBoundaryConditions);
			this.scanner.Scan();
		}

		internal Stream TestFormatTraceStream;

		internal Stream TestFormatOutputTraceStream;

		internal int InputBufferSize = 4096;

		internal HtmlTagCallback HtmlCallback;

		internal bool RecognizeHyperlinks;

		internal bool Locked;

		private bool testBoundaryConditions;

		private bool filterHtml;

		private ConversationBodyScanner.Scanner scanner;

		private bool madeProgress;

		internal class Scanner
		{
			public Scanner(FormatStore store, bool filterHtml, HtmlTagCallback htmlCallback, bool recognizeHyperlinks, Stream testFormatTraceStream, Stream testFormatOutputTraceStream, bool testBoundaryConditions)
			{
				this.store = store;
				this.output = new HtmlFormatOutput(null, null, true, null, testFormatOutputTraceStream, filterHtml, htmlCallback, recognizeHyperlinks);
				this.output.Initialize(this.store, SourceFormat.Html, null);
			}

			public static ConversationBodyScanner.Scanner.MsHeading[] MsHeadings
			{
				get
				{
					if (ConversationBodyScanner.Scanner.msHeadings == null)
					{
						ConversationBodyScanner.Scanner.InitializeHeadings();
					}
					return ConversationBodyScanner.Scanner.msHeadings;
				}
			}

			public static ConversationBodyScanner.Scanner.NonMsHeading[] NonMsHeadings
			{
				get
				{
					if (ConversationBodyScanner.Scanner.nonMsHeadings == null)
					{
						ConversationBodyScanner.Scanner.InitializeHeadings();
					}
					return ConversationBodyScanner.Scanner.nonMsHeadings;
				}
			}

			public int CountQuotingLevels
			{
				get
				{
					int num = 0;
					for (int i = 0; i < this.fragments.Count; i++)
					{
						if ((int)(this.fragments[i].QuotingLevel + 1) > num)
						{
							num = (int)(this.fragments[i].QuotingLevel + 1);
						}
					}
					return num;
				}
			}

			public FormatStore Store
			{
				get
				{
					return this.store;
				}
			}

			public IList<TextRun> Words
			{
				get
				{
					if (this.readonlyWords == null)
					{
						this.readonlyWords = new ReadOnlyCollection<TextRun>(this.words);
					}
					return this.readonlyWords;
				}
			}

			public IList<ConversationBodyScanner.Scanner.FragmentInfo> Fragments
			{
				get
				{
					if (this.readonlyFragments == null)
					{
						this.readonlyFragments = new ReadOnlyCollection<ConversationBodyScanner.Scanner.FragmentInfo>(this.fragments);
					}
					return this.readonlyFragments;
				}
			}

			public IList<ConversationBodyScanner.Scanner.LineInfo> Lines
			{
				get
				{
					if (this.readonlyLines == null)
					{
						this.readonlyLines = new ReadOnlyCollection<ConversationBodyScanner.Scanner.LineInfo>(this.lines);
					}
					return this.readonlyLines;
				}
			}

			private short LineCount
			{
				get
				{
					return (short)this.lines.Count;
				}
			}

			public void Scan()
			{
				this.store.RootNode.FirstChild.ChangeNodeType(FormatContainerType.Fragment);
				this.ScanLines();
				this.ScanWords();
				this.BuildFragmentList();
			}

			public void WriteBody(HtmlWriter writer, int level)
			{
				this.output.SetWriter(writer);
				bool flag = false;
				short num = 0;
				for (int i = 0; i < this.fragments.Count; i++)
				{
					if ((int)this.fragments[i].QuotingLevel >= level && this.fragments[i].Category == ConversationBodyScanner.Scanner.FragmentCategory.Normal)
					{
						if ((int)this.fragments[i].QuotingLevel == level)
						{
							if (flag)
							{
								writer.WriteStartTag(HtmlNameIndex.Div);
								writer.WriteAttribute(HtmlNameIndex.Style, "margin-left:0.2in;padding-left:6pt;border-left:black 1pt solid;margin-top:6pt;margin-bottom:6pt;background:#E8E8E8;font-size:10pt;");
								writer.WriteStartTag(HtmlNameIndex.A);
								writer.WriteAttribute(HtmlNameIndex.Name, "quoteN");
								writer.WriteAttribute(HtmlNameIndex.Href, "#quoteN");
								writer.WriteAttribute(HtmlNameIndex.Style, "text-decoration:none");
								int num2 = 256;
								if (this.lines[(int)this.fragments[i].FirstLine].TextPosition - this.lines[(int)(num + 1)].TextPosition < 512U)
								{
									num2 = 512;
								}
								while ((ulong)this.lines[(int)(num + 1)].TextPosition + (ulong)((long)num2) < (ulong)this.lines[(int)this.fragments[i].FirstLine].TextPosition)
								{
									num += 1;
								}
								this.WriteQuotedText(this.lines[(int)num].TextPosition, this.lines[(int)this.fragments[i].FirstLine].TextPosition, num2, writer);
								writer.WriteEndTag(HtmlNameIndex.A);
								writer.WriteEndTag(HtmlNameIndex.Div);
								flag = false;
							}
							this.output.OutputFragment(this.lines[(int)this.fragments[i].FirstLine].Node, this.lines[(int)this.fragments[i].FirstLine].TextPosition, this.fragments[i].EndNode, this.fragments[i].EndTextPosition);
						}
						else if (!flag)
						{
							flag = true;
							num = this.fragments[i].FirstLine;
						}
					}
				}
			}

			public void WriteLines(HtmlWriter writer, int fromLineIndex, int toLineIndex)
			{
				if (toLineIndex >= this.Lines.Count || toLineIndex < 0)
				{
					throw new ArgumentException("tillFragmentIndex");
				}
				if (fromLineIndex > toLineIndex || fromLineIndex < 0)
				{
					throw new ArgumentException("fromFragmentIndex");
				}
				this.output.Writer = writer;
				if (toLineIndex < this.Lines.Count - 1 && this.lines.Count < 8191)
				{
					FormatNode endNode;
					uint endTextPosition;
					this.GetEndNode(toLineIndex, out endNode, out endTextPosition);
					this.output.OutputFragment(this.Lines[fromLineIndex].Node, this.Lines[fromLineIndex].TextPosition, endNode, endTextPosition);
					return;
				}
				this.output.OutputFragment(this.Lines[fromLineIndex].Node, this.Lines[fromLineIndex].TextPosition, FormatNode.Null, uint.MaxValue);
			}

			public void WriteAll(HtmlWriter writer, int level)
			{
				this.output.SetWriter(writer);
				this.output.OutputFragment(this.store.RootNode.FirstChild);
			}

			public bool IsHeading(TextRun run, string[] headingWords)
			{
				return this.IsHeading(run, true, headingWords);
			}

			private static string[] ReadWords(XmlReader xmlReader)
			{
				List<string> list = new List<string>();
				xmlReader.Read();
				if (!xmlReader.EOF && xmlReader.NodeType == XmlNodeType.Text)
				{
					list.Add(xmlReader.ReadContentAsString());
					xmlReader.Read();
					return list.ToArray();
				}
				while (xmlReader.NodeType != XmlNodeType.EndElement && !xmlReader.EOF)
				{
					if (xmlReader.NodeType == XmlNodeType.Element)
					{
						string name;
						if ((name = xmlReader.Name) == null || !(name == "Word"))
						{
							throw new ArgumentException(string.Format("InvalidTag:{0}", xmlReader.Name));
						}
						list.Add(xmlReader.ReadElementContentAsString());
					}
					else
					{
						xmlReader.Read();
					}
				}
				return list.ToArray();
			}

			private static void InitializeHeadings()
			{
				Assembly assembly = typeof(ConversationBodyScanner).GetTypeInfo().Assembly;
				using (Stream manifestResourceStream = assembly.GetManifestResourceStream("MessageHeaders.xml"))
				{
					List<ConversationBodyScanner.Scanner.MsHeading> list = null;
					List<ConversationBodyScanner.Scanner.NonMsHeading> list2 = null;
					using (XmlReader xmlReader = XmlReader.Create(manifestResourceStream))
					{
						while (xmlReader.NodeType != XmlNodeType.EndElement && !xmlReader.EOF)
						{
							if (xmlReader.NodeType == XmlNodeType.Element)
							{
								string name;
								if ((name = xmlReader.Name) != null)
								{
									if (name == "NonMsHeader")
									{
										list2.Add(ConversationBodyScanner.Scanner.NonMsHeading.LoadFromXml(xmlReader));
										xmlReader.Read();
										continue;
									}
									if (name == "MsHeader")
									{
										list.Add(ConversationBodyScanner.Scanner.MsHeading.LoadFromXml(xmlReader));
										xmlReader.Read();
										continue;
									}
									if (name == "MessageHeaders")
									{
										list = new List<ConversationBodyScanner.Scanner.MsHeading>();
										list2 = new List<ConversationBodyScanner.Scanner.NonMsHeading>();
										xmlReader.Read();
										continue;
									}
								}
								throw new ArgumentException(string.Format("InvalidTag:{0}", xmlReader.Name));
							}
							xmlReader.Read();
						}
					}
					ConversationBodyScanner.Scanner.msHeadings = list.ToArray();
					ConversationBodyScanner.Scanner.nonMsHeadings = list2.ToArray();
				}
			}

			private void ScanWords()
			{
				if (this.lines.Count == 0)
				{
					return;
				}
				TextRun textRun = this.store.GetTextRun(this.store.RootNode.BeginTextPosition);
				if (textRun.Equals(TextRun.Invalid))
				{
					return;
				}
				bool flag = false;
				int num = 0;
				while (!textRun.IsEnd())
				{
					if (textRun.Type == TextRunType.NonSpace)
					{
						bool flag2 = true;
						if (!flag)
						{
							while (num < this.lines.Count && this.lines[num].TextPosition <= textRun.Position)
							{
								if (textRun.Position < this.lines[num].TextPositionAfterTextQuoting)
								{
									flag2 = false;
									break;
								}
								ConversationBodyScanner.Scanner.LineInfo lineInfo = this.lines[num];
								lineInfo.FirstWordIndex = (uint)this.words.Count;
								this.lines[num] = lineInfo;
								num++;
							}
							if (flag2)
							{
								this.words.Add(textRun);
								this.words[this.words.Count - 1].MakeImmutable();
							}
						}
						flag = true;
					}
					else
					{
						flag = false;
					}
					textRun.MoveNext();
				}
				while (num < this.lines.Count && this.lines[num].TextPosition <= textRun.Position)
				{
					ConversationBodyScanner.Scanner.LineInfo lineInfo2 = this.lines[num];
					lineInfo2.FirstWordIndex = (uint)this.words.Count;
					this.lines[num] = lineInfo2;
					num++;
				}
			}

			private void ScanLines()
			{
				bool flag = true;
				short num = 0;
				int leftIndentPoints = 0;
				using (FormatNode.SubtreeEnumerator enumerator = this.store.RootNode.Subtree.GetEnumerator(true))
				{
					while (enumerator.MoveNext())
					{
						FormatNode node = enumerator.Current;
						if (node.NodeType == FormatContainerType.TableContainer)
						{
							node = node.LastChild;
						}
						if (node.NodeType == FormatContainerType.Text)
						{
							TextRun textRun = this.store.GetTextRun(node.BeginTextPosition);
							do
							{
								if (flag)
								{
									this.InspectAddLine(node, ref textRun, (int)num, leftIndentPoints);
									flag = false;
								}
								if (textRun.Type == TextRunType.NewLine)
								{
									flag = true;
								}
								textRun.MoveNext();
							}
							while (textRun.Position < node.EndTextPosition);
						}
						else if (node.NodeType == FormatContainerType.Image || node.NodeType == FormatContainerType.Map)
						{
							if (flag)
							{
								flag = false;
								if (this.CanAddLines(2))
								{
									this.AddLine(ConversationBodyScanner.Scanner.LineCategory.Normal, node, node.BeginTextPosition, leftIndentPoints);
								}
								else if (this.CanAddLines(1))
								{
									this.AddLine(ConversationBodyScanner.Scanner.LineCategory.Skipped, node, node.BeginTextPosition, leftIndentPoints);
								}
							}
						}
						else if ((byte)(node.NodeType & FormatContainerType.BlockFlag) != 0 && (enumerator.FirstVisit || enumerator.LastVisit))
						{
							flag = true;
							if (node.NodeType == FormatContainerType.HorizontalLine)
							{
								this.PopQuotingLevel((int)num, 0);
								if (this.CanAddLines(2))
								{
									this.AddLine(ConversationBodyScanner.Scanner.LineCategory.HorizontalLineDelimiter, node, node.BeginTextPosition, leftIndentPoints);
								}
								else if (this.CanAddLines(1))
								{
									this.AddLine(ConversationBodyScanner.Scanner.LineCategory.Skipped, node, node.BeginTextPosition, leftIndentPoints);
								}
							}
							else if (node.NodeType == FormatContainerType.BlockQuote)
							{
								if (enumerator.FirstVisit)
								{
									this.PopQuotingLevel((int)num, 0);
									num += 1;
									this.PushQuotingLevel((int)num, 0, node, node.BeginTextPosition, leftIndentPoints);
								}
								if (enumerator.LastVisit)
								{
									num -= 1;
									this.PopQuotingLevel((int)num, 0);
								}
							}
							else if (node.NodeType == FormatContainerType.Table || node.NodeType == FormatContainerType.List)
							{
								enumerator.SkipChildren();
								this.PopQuotingLevel((int)num, 0);
								if (this.CanAddLines(1))
								{
									this.AddLine(ConversationBodyScanner.Scanner.LineCategory.Normal, node, node.BeginTextPosition, leftIndentPoints);
								}
							}
						}
					}
				}
			}

			private void GetEndNode(int lastLineIndex, out FormatNode endFormatNode, out uint endTextPosition)
			{
				if (this.Lines[lastLineIndex + 1].Node.NodeType == FormatContainerType.Text && this.Lines[lastLineIndex + 1].TextPosition > this.lines[lastLineIndex + 1].Node.BeginTextPosition)
				{
					endFormatNode = this.Lines[lastLineIndex + 1].Node;
					endTextPosition = this.Lines[lastLineIndex + 1].TextPosition;
					return;
				}
				endFormatNode = this.lines[lastLineIndex + 1].Node;
				while (endFormatNode == endFormatNode.Parent.FirstChild && endFormatNode != this.lines[lastLineIndex].Node)
				{
					endFormatNode = endFormatNode.Parent;
				}
				endTextPosition = endFormatNode.BeginTextPosition;
			}

			private void BuildFragmentList()
			{
				if (this.lines.Count != 0)
				{
					this.RecursiveBuildFragmentList(0, 0, this.LineCount, 0, 0, 0);
					for (int i = 0; i < this.fragments.Count; i++)
					{
						if (i < this.fragments.Count - 1)
						{
							FormatNode endNode;
							uint endTextPosition;
							this.GetEndNode((int)(this.Fragments[i + 1].FirstLine - 1), out endNode, out endTextPosition);
							this.fragments[i] = new ConversationBodyScanner.Scanner.FragmentInfo(this.fragments[i].Category, this.fragments[i].QuotingLevel, this.fragments[i].TextQuotingLevel, this.fragments[i].FirstLine, this.fragments[i].FirstWord, endNode, endTextPosition);
						}
					}
				}
			}

			private void RecursiveBuildFragmentList(short firstLine, short lineIndex, short endLine, short quotingLevel, short effectiveQuotingLevel, byte textQuotingLevel)
			{
				while (lineIndex != -1 && lineIndex < endLine)
				{
					if (lineIndex >= firstLine)
					{
						if (this.lines[(int)lineIndex].Category == ConversationBodyScanner.Scanner.LineCategory.PotentialMsHeader)
						{
							short num = 0;
							short num2 = 0;
							if (this.IsMsReplyForwardHeader(firstLine, lineIndex, ref num, ref num2))
							{
								short num3 = num;
								this.SkipBlankLinesBackward(firstLine, num, ref num3);
								if (num3 != firstLine)
								{
									this.AddFragment(ConversationBodyScanner.Scanner.FragmentCategory.Normal, effectiveQuotingLevel, firstLine, textQuotingLevel);
								}
								if (num3 != num)
								{
									this.AddFragment(ConversationBodyScanner.Scanner.FragmentCategory.Blank, effectiveQuotingLevel, num3, textQuotingLevel);
								}
								effectiveQuotingLevel += 1;
								this.AddFragment(ConversationBodyScanner.Scanner.FragmentCategory.MsHeader, effectiveQuotingLevel, num, textQuotingLevel);
								short num4 = num2;
								this.SkipBlankLinesForward(num2, endLine, ref num4, false);
								if (num4 != num2)
								{
									this.AddFragment(ConversationBodyScanner.Scanner.FragmentCategory.Blank, effectiveQuotingLevel, num2 + 1, textQuotingLevel);
								}
								lineIndex = num4;
								firstLine = lineIndex + 1;
							}
						}
						else if (this.lines[(int)lineIndex].Category == ConversationBodyScanner.Scanner.LineCategory.PotentialNonMsHeader)
						{
							short num5 = 0;
							if (this.IsNonMsReplyForwardHeader(firstLine, lineIndex, ref num5))
							{
								short num6 = num5;
								this.SkipBlankLinesBackward(firstLine, num5, ref num6);
								if (num6 != firstLine)
								{
									this.AddFragment(ConversationBodyScanner.Scanner.FragmentCategory.Normal, effectiveQuotingLevel, firstLine, textQuotingLevel);
								}
								if (num6 != num5)
								{
									this.AddFragment(ConversationBodyScanner.Scanner.FragmentCategory.Blank, effectiveQuotingLevel, num6, textQuotingLevel);
								}
								this.AddFragment(ConversationBodyScanner.Scanner.FragmentCategory.NonMsHeader, effectiveQuotingLevel + 1, num5, textQuotingLevel);
								short num7 = lineIndex;
								this.SkipBlankLinesForward(lineIndex, endLine, ref num7, false);
								if (num7 != lineIndex)
								{
									this.AddFragment(ConversationBodyScanner.Scanner.FragmentCategory.Blank, effectiveQuotingLevel, lineIndex + 1, textQuotingLevel);
								}
								lineIndex = num7;
								firstLine = lineIndex + 1;
							}
						}
					}
					lineIndex += 1;
				}
				if (firstLine != endLine)
				{
					this.AddFragment(ConversationBodyScanner.Scanner.FragmentCategory.Normal, effectiveQuotingLevel, firstLine, textQuotingLevel);
				}
			}

			private bool IsMsReplyForwardHeader(short firstLine, short lineIndex, ref short headerFirstLine, ref short headerLastLine)
			{
				TextRun textRun = this.store.GetTextRun(this.lines[(int)lineIndex].TextPositionAfterTextQuoting);
				this.SkipLeadingSpace(ref textRun);
				char c = textRun[0];
				for (int i = 0; i < ConversationBodyScanner.Scanner.MsHeadings.Length; i++)
				{
					if (c == ConversationBodyScanner.Scanner.MsHeadings[i].FromFields[0][0] && this.IsHeading(textRun, true, ConversationBodyScanner.Scanner.MsHeadings[i].FromFields))
					{
						bool flag = false;
						short num = (short)(this.lines[(int)lineIndex].BlockQuotingLevel + this.lines[(int)lineIndex].TextQuotingLevel);
						short num2 = lineIndex + 1;
						while ((int)num2 < Math.Min((int)(lineIndex + 10), (int)this.LineCount) && (short)(this.lines[(int)num2].BlockQuotingLevel + this.lines[(int)num2].TextQuotingLevel) == num)
						{
							if (this.lines[(int)num2].Category == ConversationBodyScanner.Scanner.LineCategory.PotentialMsHeader || this.lines[(int)num2].Category == ConversationBodyScanner.Scanner.LineCategory.PotentialNonMsHeader)
							{
								if (!flag)
								{
									TextRun textRun2 = this.store.GetTextRun(this.lines[(int)num2].TextPositionAfterTextQuoting);
									this.SkipLeadingSpace(ref textRun2);
									for (int j = 0; j < ConversationBodyScanner.Scanner.MsHeadings[i].AdditionalHeadings.Length; j++)
									{
										if (this.IsHeading(textRun2, true, ConversationBodyScanner.Scanner.MsHeadings[i].AdditionalHeadings[j].SentFields))
										{
											flag = true;
											break;
										}
									}
								}
							}
							else
							{
								if (!flag)
								{
									break;
								}
								bool flag2 = false;
								int num3 = (int)(num2 - 1);
								if (num3 >= Math.Min((int)(lineIndex + 10), (int)this.LineCount) || this.lines[num3].Category != ConversationBodyScanner.Scanner.LineCategory.PotentialMsHeader || this.lines[(int)num2].Category != ConversationBodyScanner.Scanner.LineCategory.Normal)
								{
									break;
								}
								TextRun textRun3 = this.store.GetTextRun(this.lines[num3].TextPositionAfterTextQuoting);
								this.SkipLeadingSpace(ref textRun3);
								for (int k = 0; k < ConversationBodyScanner.Scanner.MsHeadings[i].AdditionalHeadings.Length; k++)
								{
									if (ConversationBodyScanner.Scanner.MsHeadings[i].AdditionalHeadings[k].ToFields != null && ConversationBodyScanner.Scanner.MsHeadings[i].AdditionalHeadings[k].ToFields.Length > 0 && this.IsHeading(textRun3, true, ConversationBodyScanner.Scanner.MsHeadings[i].AdditionalHeadings[k].ToFields))
									{
										flag2 = true;
										break;
									}
								}
								if (!flag2)
								{
									break;
								}
							}
							num2 += 1;
						}
						if (flag)
						{
							headerLastLine = num2 - 1;
							headerFirstLine = lineIndex - 1;
							while (headerFirstLine >= firstLine && (this.lines[(int)headerFirstLine].Category == ConversationBodyScanner.Scanner.LineCategory.PotentialDelimiterLine || this.lines[(int)headerFirstLine].Category == ConversationBodyScanner.Scanner.LineCategory.HorizontalLineDelimiter || this.lines[(int)headerFirstLine].Category == ConversationBodyScanner.Scanner.LineCategory.Invalid))
							{
								headerFirstLine -= 1;
							}
							headerFirstLine += 1;
							return true;
						}
					}
				}
				return false;
			}

			private bool IsNonMsReplyForwardHeader(short firstLine, short lineIndex, ref short headerFirstLine)
			{
				short num = (short)(this.lines[(int)lineIndex].BlockQuotingLevel + this.lines[(int)lineIndex].TextQuotingLevel);
				short num2 = lineIndex + 1;
				while (num2 < this.LineCount && num2 <= lineIndex + 3 && (short)(this.lines[(int)num2].BlockQuotingLevel + this.lines[(int)num2].TextQuotingLevel) == num && (this.lines[(int)num2].Category == ConversationBodyScanner.Scanner.LineCategory.Blank || this.lines[(int)num2].Category == ConversationBodyScanner.Scanner.LineCategory.Invalid || this.lines[(int)num2].Category == ConversationBodyScanner.Scanner.LineCategory.Quoting))
				{
					num2 += 1;
				}
				if (num2 < this.LineCount && num2 <= lineIndex + 3 && (short)(this.lines[(int)num2].BlockQuotingLevel + this.lines[(int)num2].TextQuotingLevel) >= num)
				{
					TextRun textRun = this.store.GetTextRun(this.lines[(int)lineIndex].TextPositionAfterTextQuoting);
					this.SkipLeadingSpace(ref textRun);
					int num3 = 0;
					if (this.LineContainsInterestingWordForNonMsHeader(textRun, ref num3))
					{
						headerFirstLine = lineIndex;
						if (lineIndex > firstLine && this.lines[(int)(lineIndex - 1)].Category == ConversationBodyScanner.Scanner.LineCategory.Normal && (short)(this.lines[(int)(lineIndex - 1)].BlockQuotingLevel + this.lines[(int)(lineIndex - 1)].TextQuotingLevel) == num)
						{
							textRun = this.store.GetTextRun(this.lines[(int)(lineIndex - 1)].TextPositionAfterTextQuoting);
							this.SkipLeadingSpace(ref textRun);
							foreach (string word in ConversationBodyScanner.Scanner.NonMsHeadings[num3].StartsWith)
							{
								if (this.LineStartsWith(textRun, word))
								{
									headerFirstLine -= 1;
									break;
								}
							}
						}
						return true;
					}
				}
				return false;
			}

			private void SkipBlankLinesBackward(short firstLine, short lineIndex, ref short firstBlankLineBeforeHeader)
			{
				short num = (short)(this.lines[(int)lineIndex].BlockQuotingLevel + this.lines[(int)lineIndex].TextQuotingLevel);
				firstBlankLineBeforeHeader = lineIndex - 1;
				while (firstBlankLineBeforeHeader >= firstLine && num == (short)(this.lines[(int)firstBlankLineBeforeHeader].BlockQuotingLevel + this.lines[(int)firstBlankLineBeforeHeader].TextQuotingLevel) && (this.lines[(int)firstBlankLineBeforeHeader].Category == ConversationBodyScanner.Scanner.LineCategory.Blank || this.lines[(int)firstBlankLineBeforeHeader].Category == ConversationBodyScanner.Scanner.LineCategory.Invalid))
				{
					firstBlankLineBeforeHeader -= 1;
				}
				firstBlankLineBeforeHeader += 1;
			}

			private void SkipBlankLinesForward(short lineIndex, short endLine, ref short lastBlankLine, bool anyLevel)
			{
				short num = (short)(this.lines[(int)lineIndex].BlockQuotingLevel + this.lines[(int)lineIndex].TextQuotingLevel);
				lastBlankLine = lineIndex + 1;
				while (lastBlankLine < endLine && (anyLevel || num == (short)(this.lines[(int)lastBlankLine].BlockQuotingLevel + this.lines[(int)lastBlankLine].TextQuotingLevel)) && (this.lines[(int)lastBlankLine].Category == ConversationBodyScanner.Scanner.LineCategory.Blank || this.lines[(int)lastBlankLine].Category == ConversationBodyScanner.Scanner.LineCategory.Invalid || this.lines[(int)lastBlankLine].Category == ConversationBodyScanner.Scanner.LineCategory.Quoting))
				{
					lastBlankLine += 1;
				}
				lastBlankLine -= 1;
			}

			private bool FollowingLinesAreBlankOrNotInteresting(short nextSibling)
			{
				return true;
			}

			private void AddFragment(ConversationBodyScanner.Scanner.FragmentCategory category, short quotingLevel, short firstLineIndex, byte textQuotingLevel)
			{
				this.fragments.Add(new ConversationBodyScanner.Scanner.FragmentInfo(category, quotingLevel, textQuotingLevel, firstLineIndex, (int)this.lines[(int)firstLineIndex].FirstWordIndex, FormatNode.Null, uint.MaxValue));
			}

			private bool CanAddLines(int num)
			{
				return (int)(this.LineCount + (short)this.currentBlockQuotingLevel + (short)this.currentTextQuotingLevel) + num < 8191;
			}

			private void AddLine(ConversationBodyScanner.Scanner.LineCategory category, FormatNode node, uint beginTextPosition, int leftIndentPoints160)
			{
				ConversationBodyScanner.Scanner.LineInfo item = new ConversationBodyScanner.Scanner.LineInfo(node, beginTextPosition, 0U, beginTextPosition, category, this.currentBlockQuotingLevel, this.currentTextQuotingLevel, 1, this.parent, -1);
				if (this.lastSibling != -1)
				{
					ConversationBodyScanner.Scanner.LineInfo lineInfo = this.lines[(int)this.lastSibling];
					lineInfo.NextSibling = this.LineCount;
				}
				this.lastSibling = this.LineCount;
				this.lines.Add(item);
			}

			private void InspectAddLine(FormatNode node, ref TextRun lineStartRun, int blockQuotingLevel, int leftIndentPoints160)
			{
				TextRun textRun = lineStartRun;
				TextRun invalid = TextRun.Invalid;
				int num = 0;
				int count = this.words.Count;
				textRun.SkipInvalid();
				while (textRun.Type == TextRunType.NonSpace && textRun[0] == '>')
				{
					num += textRun.EffectiveLength;
					textRun.MoveNext();
					textRun.SkipInvalid();
					if ((textRun.Type == TextRunType.NbSp || textRun.Type == TextRunType.Space) && textRun.EffectiveLength <= 3)
					{
						TextRun textRun2 = textRun;
						int num2 = 0;
						do
						{
							num2 += textRun2.EffectiveLength;
							textRun2.MoveNext();
							textRun2.SkipInvalid();
						}
						while ((textRun2.Type == TextRunType.NbSp || textRun2.Type == TextRunType.Space) && num2 + textRun2.EffectiveLength <= 3);
						if (textRun2.Type == TextRunType.NonSpace && textRun2[0] == '>')
						{
							textRun = textRun2;
						}
					}
				}
				this.PopQuotingLevel(blockQuotingLevel, num);
				this.PushQuotingLevel(blockQuotingLevel, num, node, lineStartRun.Position, leftIndentPoints160);
				if (this.CanAddLines(2))
				{
					uint position = textRun.Position;
					int num3 = 0;
					while (textRun.Type == TextRunType.NbSp || textRun.Type == TextRunType.Space || textRun.Type == TextRunType.Tabulation)
					{
						num3 += textRun.EffectiveLength * ((textRun.Type == TextRunType.Tabulation) ? 8 : 1);
						textRun.MoveNext();
						textRun.SkipInvalid();
					}
					int num4 = 1;
					ConversationBodyScanner.Scanner.LineCategory category;
					if (textRun.Type == TextRunType.NewLine || textRun.Type == TextRunType.BlockBoundary)
					{
						category = ConversationBodyScanner.Scanner.LineCategory.Blank;
						if (textRun.Type == TextRunType.NewLine)
						{
							num4 = textRun.EffectiveLength;
						}
					}
					else if (num3 <= 3 && textRun.Type == TextRunType.NonSpace)
					{
						if (this.IsPotentialMsHeading(textRun))
						{
							category = ConversationBodyScanner.Scanner.LineCategory.PotentialMsHeader;
						}
						else if (this.IsPotentialNonMsHeading(textRun))
						{
							category = ConversationBodyScanner.Scanner.LineCategory.PotentialNonMsHeader;
						}
						else if (this.IsPotentialDelimiterLine(textRun))
						{
							category = ConversationBodyScanner.Scanner.LineCategory.PotentialDelimiterLine;
						}
						else
						{
							category = ConversationBodyScanner.Scanner.LineCategory.Normal;
						}
					}
					else
					{
						category = ConversationBodyScanner.Scanner.LineCategory.Normal;
					}
					if (num > 254)
					{
						num = 254;
					}
					ConversationBodyScanner.Scanner.LineInfo item = new ConversationBodyScanner.Scanner.LineInfo(node, lineStartRun.Position, 0U, position, category, (byte)blockQuotingLevel, (byte)num, (short)num4, this.parent, -1);
					if (this.lastSibling != -1)
					{
						ConversationBodyScanner.Scanner.LineInfo lineInfo = this.lines[(int)this.lastSibling];
						lineInfo.NextSibling = this.LineCount;
					}
					this.lastSibling = this.LineCount;
					this.lines.Add(item);
					return;
				}
				if (this.CanAddLines(1))
				{
					this.AddLine(ConversationBodyScanner.Scanner.LineCategory.Skipped, node, lineStartRun.Position, leftIndentPoints160);
				}
			}

			private void PopQuotingLevel(int blockQuotingLevel, int textQuotingLevel)
			{
				if (blockQuotingLevel >= 254)
				{
					blockQuotingLevel = 254;
					textQuotingLevel = 0;
				}
				if (textQuotingLevel > 254)
				{
					textQuotingLevel = 254;
				}
				if (blockQuotingLevel != (int)this.currentBlockQuotingLevel)
				{
					while (this.currentTextQuotingLevel != 0)
					{
						this.lastSibling = this.parent;
						this.parent = this.lines[(int)this.lastSibling].Parent;
						this.currentTextQuotingLevel -= 1;
					}
					if (blockQuotingLevel < (int)this.currentBlockQuotingLevel)
					{
						this.lastSibling = this.parent;
						this.parent = this.lines[(int)this.lastSibling].Parent;
						this.currentBlockQuotingLevel -= 1;
						return;
					}
				}
				else if (textQuotingLevel < (int)this.currentTextQuotingLevel)
				{
					do
					{
						this.lastSibling = this.parent;
						this.parent = this.lines[(int)this.lastSibling].Parent;
						this.currentTextQuotingLevel -= 1;
					}
					while ((int)this.currentTextQuotingLevel != textQuotingLevel);
				}
			}

			private void PushQuotingLevel(int blockQuotingLevel, int textQuotingLevel, FormatNode node, uint textPosition, int leftIndentPoints160)
			{
				if (blockQuotingLevel >= 254)
				{
					blockQuotingLevel = 254;
					textQuotingLevel = 0;
				}
				if (textQuotingLevel > 254)
				{
					textQuotingLevel = 254;
				}
				if (blockQuotingLevel > (int)this.currentBlockQuotingLevel)
				{
					if (this.CanAddLines(3))
					{
						this.AddLine(ConversationBodyScanner.Scanner.LineCategory.Quoting, node, textPosition, leftIndentPoints160);
						this.currentBlockQuotingLevel += 1;
						this.lastSibling = -1;
						this.parent = (short)(this.lines.Count - 1);
						return;
					}
					if (this.CanAddLines(1))
					{
						this.AddLine(ConversationBodyScanner.Scanner.LineCategory.Skipped, node, textPosition, leftIndentPoints160);
						return;
					}
				}
				else if (textQuotingLevel > (int)this.currentTextQuotingLevel)
				{
					if (this.lines.Count == 0)
					{
						this.AddLine(ConversationBodyScanner.Scanner.LineCategory.Quoting, node, textPosition, leftIndentPoints160);
						this.lastSibling = -1;
						this.parent = (short)(this.lines.Count - 1);
						this.currentTextQuotingLevel += 1;
					}
					else if (this.lastSibling == this.LineCount - 1 && this.CanAddLines(2))
					{
						this.lastSibling = -1;
						this.parent = (short)(this.lines.Count - 1);
						this.currentTextQuotingLevel += 1;
					}
					while (textQuotingLevel > (int)this.currentTextQuotingLevel && this.CanAddLines(3))
					{
						this.AddLine(ConversationBodyScanner.Scanner.LineCategory.Quoting, node, textPosition, leftIndentPoints160);
						this.lastSibling = -1;
						this.parent = (short)(this.lines.Count - 1);
						this.currentTextQuotingLevel += 1;
					}
				}
			}

			private bool LineStartsWithOneOrTwoWordsAndColon(ref TextRun run)
			{
				int num = run.EffectiveLength;
				if (num < 20)
				{
					if (run[run.EffectiveLength - 1] == ':')
					{
						return num > 1;
					}
					run.MoveNext();
					run.SkipInvalid();
					if (!run.IsEnd())
					{
						if (run.Type == TextRunType.NonSpace && run.EffectiveLength == 1 && run[0] == ':')
						{
							return true;
						}
						if ((run.Type == TextRunType.NbSp || run.Type == TextRunType.Space) && run.EffectiveLength == 1)
						{
							run.MoveNext();
							run.SkipInvalid();
							if (!run.IsEnd() && run.Type == TextRunType.NonSpace)
							{
								num += run.EffectiveLength;
								if (num < 20)
								{
									if (run[run.EffectiveLength - 1] == ':')
									{
										return true;
									}
									run.MoveNext();
									run.SkipInvalid();
									if (!run.IsEnd() && run.Type == TextRunType.NonSpace && run.EffectiveLength == 1 && run[0] == ':')
									{
										return true;
									}
									if (!run.IsEnd() && run.Type == TextRunType.Space && run.EffectiveLength == 1)
									{
										run.MoveNext();
										run.SkipInvalid();
										if (!run.IsEnd() && run.Type == TextRunType.NonSpace && run.EffectiveLength == 1 && run[0] == ':')
										{
											return true;
										}
									}
								}
							}
						}
					}
				}
				return false;
			}

			private bool IsPotentialMsHeading(TextRun run)
			{
				if (this.LineStartsWithOneOrTwoWordsAndColon(ref run))
				{
					run.MoveNext();
					run.SkipInvalid();
					if (!run.IsEnd() && (run.Type == TextRunType.NbSp || run.Type == TextRunType.Space || run.Type == TextRunType.Tabulation))
					{
						run.MoveNext();
						run.SkipInvalid();
						if (!run.IsEnd() && (run.Type == TextRunType.NbSp || run.Type == TextRunType.Space || run.Type == TextRunType.Tabulation))
						{
							run.MoveNext();
							run.SkipInvalid();
							if (!run.IsEnd() && (run.Type == TextRunType.NbSp || run.Type == TextRunType.Space || run.Type == TextRunType.Tabulation))
							{
								run.MoveNext();
								run.SkipInvalid();
							}
						}
						if (!run.IsEnd() && run.Type == TextRunType.NonSpace)
						{
							return true;
						}
					}
				}
				return false;
			}

			private bool IsPotentialNonMsHeading(TextRun run)
			{
				bool result = false;
				int num = 0;
				for (;;)
				{
					if (run.Type != TextRunType.Invalid)
					{
						num += run.EffectiveLength;
						if (num > 120)
						{
							break;
						}
						if (run.Type == TextRunType.NonSpace)
						{
							result = (run[run.EffectiveLength - 1] == ':');
						}
					}
					run.MoveNext();
					if (run.IsEnd() || run.Type == TextRunType.NewLine || run.Type == TextRunType.BlockBoundary)
					{
						return result;
					}
				}
				return false;
			}

			private bool IsPotentialDelimiterLine(TextRun run)
			{
				char c = run[0];
				if (c == '-' || c == '_')
				{
					char[] buffer;
					int offset;
					int count;
					run.GetChunk(0, out buffer, out offset, out count);
					BufferString bufferString = new BufferString(buffer, offset, count);
					if (bufferString.StartsWithString("----") || bufferString.StartsWithString("____"))
					{
						bool result = false;
						int num = 0;
						for (;;)
						{
							if (run.Type != TextRunType.Invalid)
							{
								num += run.EffectiveLength;
								if (num > 60)
								{
									break;
								}
								if (run.Type == TextRunType.NonSpace)
								{
									if (run[run.EffectiveLength - 1] == c)
									{
										run.GetChunk(0, out buffer, out offset, out count);
										bufferString = new BufferString(buffer, offset, count);
										result = (bufferString.EndsWithString("----") || bufferString.EndsWithString("____"));
									}
									else
									{
										result = false;
									}
								}
							}
							run.MoveNext();
							if (run.IsEnd() || run.Type == TextRunType.NewLine || run.Type == TextRunType.BlockBoundary)
							{
								return result;
							}
						}
						return false;
					}
				}
				return false;
			}

			private BufferString[] GetLastNWords(TextRun run, int n)
			{
				Queue<BufferString> queue = new Queue<BufferString>(n);
				for (;;)
				{
					if (run.Type == TextRunType.NonSpace)
					{
						char[] buffer;
						int offset;
						int count;
						run.GetChunk(0, out buffer, out offset, out count);
						BufferString item = new BufferString(buffer, offset, count);
						queue.Enqueue(item);
						run.MoveNext();
						run.SkipInvalid();
						if (run.Type == TextRunType.Space || run.Type == TextRunType.NbSp)
						{
							run.MoveNext();
							run.SkipInvalid();
						}
						else if (run.Type != TextRunType.NewLine && run.Type != TextRunType.BlockBoundary)
						{
							queue.Clear();
						}
					}
					else
					{
						queue.Clear();
						run.MoveNext();
						run.SkipInvalid();
					}
					if (run.IsEnd() || run.Type == TextRunType.NewLine || run.Type == TextRunType.BlockBoundary)
					{
						break;
					}
					if (queue.Count == n)
					{
						queue.Dequeue();
					}
				}
				return queue.ToArray();
			}

			private bool LineContainsInterestingWordForNonMsHeader(TextRun run, ref int nonMsHeadingIndex)
			{
				BufferString[] lastNWords = this.GetLastNWords(run, ConversationBodyScanner.Scanner.NonMsHeading.MaxWordsEndsWith + 1);
				BufferString bufferString = BufferString.Null;
				if (lastNWords == null || lastNWords.Length < 1)
				{
					return false;
				}
				bufferString = lastNWords[lastNWords.Length - 1];
				if (bufferString[bufferString.Length - 1] != ':')
				{
					return false;
				}
				bool flag = false;
				if (bufferString.Length > 1)
				{
					lastNWords[lastNWords.Length - 1] = bufferString.SubString(0, bufferString.Length - 1);
				}
				else
				{
					flag = true;
				}
				for (int i = 0; i < ConversationBodyScanner.Scanner.NonMsHeadings.Length; i++)
				{
					string[] endsWith = ConversationBodyScanner.Scanner.NonMsHeadings[i].EndsWith;
					if (lastNWords.Length >= endsWith.Length)
					{
						int num = lastNWords.Length - (flag ? 2 : 1);
						int num2 = endsWith.Length - 1;
						while (num2 >= 0 && num >= 0 && lastNWords[num][0] == endsWith[num2][0] && lastNWords[num].Length == endsWith[num2].Length && lastNWords[num].StartsWithString(endsWith[num2]))
						{
							num--;
							num2--;
						}
						if (num2 < 0)
						{
							nonMsHeadingIndex = i;
							return true;
						}
					}
				}
				return false;
			}

			private bool LineStartsWith(TextRun run, string word)
			{
				if (run.Type == TextRunType.NonSpace && run.EffectiveLength == word.Length && run[0] == word[0])
				{
					char[] buffer;
					int offset;
					int count;
					run.GetChunk(0, out buffer, out offset, out count);
					BufferString bufferString = new BufferString(buffer, offset, count);
					return bufferString.StartsWithString(word);
				}
				return false;
			}

			private bool IsHeading(TextRun run, bool expectColon, string[] headingWords)
			{
				char[] buffer;
				int offset;
				int num;
				run.GetChunk(0, out buffer, out offset, out num);
				BufferString bufferString = new BufferString(buffer, offset, num);
				if ((num == headingWords[0].Length || num == headingWords[0].Length + 1) && bufferString.StartsWithString(headingWords[0]))
				{
					if (headingWords.Length == 1 && num == headingWords[0].Length + 1 && (bufferString[headingWords[0].Length] == ':' || !expectColon))
					{
						return true;
					}
					if (num == headingWords[0].Length)
					{
						run.MoveNext();
						run.SkipInvalid();
						if (headingWords.Length == 1)
						{
							if (run.Type == TextRunType.NonSpace && run.EffectiveLength == 1 && run[0] == ':')
							{
								return true;
							}
							if (run.Type == TextRunType.Space && run.EffectiveLength == 1)
							{
								run.MoveNext();
								run.SkipInvalid();
								if (!run.IsEnd() && run.Type == TextRunType.NonSpace && run.EffectiveLength == 1 && run[0] == ':')
								{
									return true;
								}
							}
						}
						else if (run.Type != TextRunType.NonSpace)
						{
							run.MoveNext();
							run.SkipInvalid();
							if (run.Type == TextRunType.NonSpace)
							{
								run.GetChunk(0, out buffer, out offset, out num);
								bufferString = new BufferString(buffer, offset, num);
								if ((num == headingWords[1].Length || num == headingWords[1].Length + 1) && bufferString.StartsWithString(headingWords[1]))
								{
									if (num == headingWords[1].Length + 1 && bufferString[headingWords[1].Length] == ':')
									{
										return true;
									}
									if (num == headingWords[1].Length)
									{
										run.MoveNext();
										run.SkipInvalid();
										if (run.Type == TextRunType.NonSpace && run.EffectiveLength == 1 && (run[0] == ':' || !expectColon))
										{
											return true;
										}
										if (run.Type == TextRunType.Space && run.EffectiveLength == 1)
										{
											run.MoveNext();
											run.SkipInvalid();
											if (!run.IsEnd() && run.Type == TextRunType.NonSpace && run.EffectiveLength == 1 && run[0] == ':')
											{
												return true;
											}
										}
									}
								}
							}
						}
					}
				}
				return false;
			}

			private void SkipLeadingSpace(ref TextRun run)
			{
				while (run.Type == TextRunType.Space || run.Type == TextRunType.NbSp || run.Type == TextRunType.Tabulation || run.Type == TextRunType.Invalid)
				{
					run.MoveNext();
				}
			}

			private void WriteQuotedText(uint start, uint end, int maxLength, HtmlWriter writer)
			{
				if (start < end)
				{
					TextRun textRun = this.store.GetTextRun(start);
					while ((ulong)(end - textRun.Position) > (ulong)((long)maxLength))
					{
						textRun.MoveNext();
					}
					writer.WriteText("... ");
					int num = 0;
					int num2 = 0;
					while (textRun.Position < end)
					{
						TextRunType type = textRun.Type;
						if (type <= TextRunType.NbSp)
						{
							if (type != TextRunType.NonSpace && type != TextRunType.FirstShort)
							{
								if (type == TextRunType.NbSp)
								{
									goto IL_B1;
								}
							}
							else
							{
								if (num != 0)
								{
									writer.WriteEmptyElementTag(HtmlTagId.BR);
									if (num >= 2)
									{
										writer.WriteEmptyElementTag(HtmlTagId.BR);
									}
									num = 0;
								}
								if (num2 != 0)
								{
									writer.WriteText(" ");
									if (num2 > 4)
									{
										num2 = 4;
									}
									while (--num2 != 0)
									{
										writer.WriteText("\u00a0");
									}
								}
								if (textRun.Type == TextRunType.NonSpace)
								{
									char[] buffer;
									int index;
									int count;
									textRun.GetChunk(0, out buffer, out index, out count);
									writer.WriteText(buffer, index, count);
								}
								else
								{
									writer.WriteText("<@>");
								}
							}
						}
						else if (type <= TextRunType.Tabulation)
						{
							if (type == TextRunType.Space || type == TextRunType.Tabulation)
							{
								goto IL_B1;
							}
						}
						else if (type == TextRunType.NewLine || type == TextRunType.BlockBoundary)
						{
							num += textRun.EffectiveLength;
							num2 = 0;
						}
						IL_170:
						textRun.MoveNext();
						continue;
						IL_B1:
						num2 += ((textRun.Type == TextRunType.Tabulation) ? (textRun.EffectiveLength * 8) : textRun.EffectiveLength);
						goto IL_170;
					}
				}
			}

			private const int MaxNumberOfLines = 8191;

			private const int MaxPhysicalQuotingNesting = 50;

			private static ConversationBodyScanner.Scanner.MsHeading[] msHeadings;

			private static ConversationBodyScanner.Scanner.NonMsHeading[] nonMsHeadings;

			private FormatStore store;

			private HtmlFormatOutput output;

			private List<TextRun> words = new List<TextRun>(128);

			private List<ConversationBodyScanner.Scanner.LineInfo> lines = new List<ConversationBodyScanner.Scanner.LineInfo>(32);

			private List<ConversationBodyScanner.Scanner.FragmentInfo> fragments = new List<ConversationBodyScanner.Scanner.FragmentInfo>(8);

			private ReadOnlyCollection<ConversationBodyScanner.Scanner.FragmentInfo> readonlyFragments;

			private ReadOnlyCollection<ConversationBodyScanner.Scanner.LineInfo> readonlyLines;

			private ReadOnlyCollection<TextRun> readonlyWords;

			private bool failedToAddLines;

			private byte currentBlockQuotingLevel;

			private byte currentTextQuotingLevel;

			private short lastSibling = -1;

			private short parent = -1;

			internal enum LineCategory
			{
				Invalid,
				Quoting,
				Normal,
				Skipped,
				Blank,
				PotentialMsHeader,
				PotentialNonMsHeader,
				PotentialDelimiterLine,
				HorizontalLineDelimiter,
				PotentialSignatureSeparator
			}

			internal enum FragmentCategory
			{
				Normal,
				Blank,
				MsHeader,
				NonMsHeader
			}

			internal struct FragmentInfo
			{
				internal FragmentInfo(ConversationBodyScanner.Scanner.FragmentCategory category, short quotingLevel, byte textQuotingLevel, short firstLine, int firstWord, FormatNode endNode, uint endTextPosition)
				{
					this.Category = category;
					this.QuotingLevel = quotingLevel;
					this.TextQuotingLevel = textQuotingLevel;
					this.FirstLine = firstLine;
					this.FirstWord = firstWord;
					this.EndNode = endNode;
					this.EndTextPosition = endTextPosition;
				}

				public readonly ConversationBodyScanner.Scanner.FragmentCategory Category;

				public readonly short QuotingLevel;

				public readonly byte TextQuotingLevel;

				public readonly short FirstLine;

				public readonly int FirstWord;

				public readonly FormatNode EndNode;

				public readonly uint EndTextPosition;
			}

			internal struct MsAdditionalHeadings
			{
				private MsAdditionalHeadings(string[] sent, string[] subject, string[] to, string[] cc, string[][] others)
				{
					this.SentFields = sent;
					this.SubjectFields = subject;
					this.ToFields = to;
					this.CcFields = cc;
					this.OtherFields = others;
				}

				public static ConversationBodyScanner.Scanner.MsAdditionalHeadings LoadFromXml(XmlReader xmlReader)
				{
					string[] sent = null;
					string[] subject = null;
					string[] to = null;
					string[] cc = null;
					List<string[]> list = new List<string[]>();
					while (xmlReader.NodeType != XmlNodeType.EndElement && !xmlReader.EOF)
					{
						if (xmlReader.NodeType == XmlNodeType.Element)
						{
							string name;
							switch (name = xmlReader.Name)
							{
							case "MsAdditionalHeadings":
								xmlReader.Read();
								continue;
							case "Sent":
								sent = ConversationBodyScanner.Scanner.ReadWords(xmlReader);
								xmlReader.Read();
								continue;
							case "Subject":
								subject = ConversationBodyScanner.Scanner.ReadWords(xmlReader);
								xmlReader.Read();
								continue;
							case "To":
								to = ConversationBodyScanner.Scanner.ReadWords(xmlReader);
								xmlReader.Read();
								continue;
							case "Cc":
								cc = ConversationBodyScanner.Scanner.ReadWords(xmlReader);
								xmlReader.Read();
								continue;
							case "Others":
								list.Add(ConversationBodyScanner.Scanner.ReadWords(xmlReader));
								xmlReader.Read();
								continue;
							}
							int num2 = (xmlReader is IXmlLineInfo) ? ((IXmlLineInfo)xmlReader).LineNumber : -1;
							throw new ArgumentException(string.Format("InvalidTag:{0} at line#{1}", xmlReader.Name, num2));
						}
						xmlReader.Read();
					}
					return new ConversationBodyScanner.Scanner.MsAdditionalHeadings(sent, subject, to, cc, list.ToArray());
				}

				public const string NodeNameStr = "MsAdditionalHeadings";

				private const string SentStr = "Sent";

				private const string SubjectStr = "Subject";

				private const string ToStr = "To";

				private const string CcStr = "Cc";

				private const string OthersStr = "Others";

				public readonly string[] SentFields;

				public readonly string[] SubjectFields;

				public readonly string[] ToFields;

				public readonly string[] CcFields;

				public readonly string[][] OtherFields;
			}

			internal struct MsHeading
			{
				private MsHeading(string[] from, ConversationBodyScanner.Scanner.MsAdditionalHeadings[] additionalHeadings)
				{
					this.FromFields = from;
					this.AdditionalHeadings = additionalHeadings;
				}

				public static ConversationBodyScanner.Scanner.MsHeading LoadFromXml(XmlReader xmlReader)
				{
					string[] from = null;
					List<ConversationBodyScanner.Scanner.MsAdditionalHeadings> list = new List<ConversationBodyScanner.Scanner.MsAdditionalHeadings>();
					while (xmlReader.NodeType != XmlNodeType.EndElement && !xmlReader.EOF)
					{
						if (xmlReader.NodeType == XmlNodeType.Element)
						{
							string name;
							if ((name = xmlReader.Name) != null)
							{
								if (name == "MsHeader")
								{
									xmlReader.Read();
									continue;
								}
								if (name == "MsAdditionalHeadings")
								{
									list.Add(ConversationBodyScanner.Scanner.MsAdditionalHeadings.LoadFromXml(xmlReader));
									xmlReader.Read();
									continue;
								}
								if (name == "From")
								{
									from = ConversationBodyScanner.Scanner.ReadWords(xmlReader);
									xmlReader.Read();
									continue;
								}
							}
							throw new ArgumentException(string.Format("InvalidTag:{0}", xmlReader.Name));
						}
						xmlReader.Read();
					}
					return new ConversationBodyScanner.Scanner.MsHeading(from, list.ToArray());
				}

				public const string NodeNameStr = "MsHeader";

				private const string FromStr = "From";

				public string[] FromFields;

				public ConversationBodyScanner.Scanner.MsAdditionalHeadings[] AdditionalHeadings;
			}

			internal struct NonMsHeading
			{
				private NonMsHeading(string[] endsWith, string[] contains)
				{
					this.EndsWith = endsWith;
					this.StartsWith = contains;
				}

				public static int MaxWordsEndsWith
				{
					get
					{
						if (ConversationBodyScanner.Scanner.NonMsHeading.maxWordsEndsWith == 0)
						{
							ConversationBodyScanner.Scanner.InitializeHeadings();
						}
						return ConversationBodyScanner.Scanner.NonMsHeading.maxWordsEndsWith;
					}
				}

				public static ConversationBodyScanner.Scanner.NonMsHeading LoadFromXml(XmlReader xmlReader)
				{
					string[] array = null;
					List<string> list = new List<string>();
					while (xmlReader.NodeType != XmlNodeType.EndElement && !xmlReader.EOF)
					{
						if (xmlReader.NodeType == XmlNodeType.Element)
						{
							string name;
							if ((name = xmlReader.Name) != null)
							{
								if (name == "NonMsHeader")
								{
									xmlReader.Read();
									continue;
								}
								if (!(name == "EndsWith"))
								{
									if (name == "StartsWith")
									{
										list.Add(xmlReader.ReadElementContentAsString());
										continue;
									}
								}
								else
								{
									array = ConversationBodyScanner.Scanner.ReadWords(xmlReader);
									if (array != null && array.Length > ConversationBodyScanner.Scanner.NonMsHeading.maxWordsEndsWith)
									{
										ConversationBodyScanner.Scanner.NonMsHeading.maxWordsEndsWith = array.Length;
										continue;
									}
									continue;
								}
							}
							throw new ArgumentException(string.Format("InvalidTag:{0}", xmlReader.Name));
						}
						xmlReader.Read();
					}
					return new ConversationBodyScanner.Scanner.NonMsHeading(array, list.ToArray());
				}

				public const string NodeNameStr = "NonMsHeader";

				private const string StartsWithStr = "StartsWith";

				private const string EndsWithStr = "EndsWith";

				public readonly string[] EndsWith;

				public readonly string[] StartsWith;

				private static int maxWordsEndsWith;
			}

			internal class LineInfo
			{
				public LineInfo(FormatNode node, uint textPosition, uint firstWordIndex, uint textPositionAfterTextQuoting, ConversationBodyScanner.Scanner.LineCategory category, byte blockQuotingLevel, byte textQuotingLevel, short count, short parent, short nextSibling)
				{
					this.node = node;
					this.textPosition = textPosition;
					this.firstWordIndex = firstWordIndex;
					this.textPositionAfterTextQuoting = textPositionAfterTextQuoting;
					this.category = category;
					this.blockQuotingLevel = blockQuotingLevel;
					this.textQuotingLevel = textQuotingLevel;
					this.count = count;
					this.parent = parent;
					this.nextSibling = nextSibling;
				}

				public FormatNode Node
				{
					get
					{
						return this.node;
					}
				}

				public uint TextPosition
				{
					get
					{
						return this.textPosition;
					}
				}

				public uint FirstWordIndex
				{
					get
					{
						return this.firstWordIndex;
					}
					set
					{
						this.firstWordIndex = value;
					}
				}

				public uint TextPositionAfterTextQuoting
				{
					get
					{
						return this.textPositionAfterTextQuoting;
					}
				}

				public ConversationBodyScanner.Scanner.LineCategory Category
				{
					get
					{
						return this.category;
					}
				}

				public byte BlockQuotingLevel
				{
					get
					{
						return this.blockQuotingLevel;
					}
				}

				public byte TextQuotingLevel
				{
					get
					{
						return this.textQuotingLevel;
					}
				}

				public short Count
				{
					get
					{
						return this.count;
					}
				}

				public short Parent
				{
					get
					{
						return this.parent;
					}
				}

				public short NextSibling
				{
					get
					{
						return this.nextSibling;
					}
					set
					{
						this.nextSibling = value;
					}
				}

				private readonly FormatNode node;

				private readonly uint textPosition;

				private readonly uint textPositionAfterTextQuoting;

				private readonly ConversationBodyScanner.Scanner.LineCategory category;

				private readonly byte blockQuotingLevel;

				private readonly byte textQuotingLevel;

				private readonly short count;

				private readonly short parent;

				private uint firstWordIndex;

				private short nextSibling;
			}
		}
	}
}
