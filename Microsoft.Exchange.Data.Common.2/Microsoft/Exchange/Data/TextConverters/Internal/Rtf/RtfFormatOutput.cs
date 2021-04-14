using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal class RtfFormatOutput : FormatOutput, IRestartable
	{
		public RtfFormatOutput(Stream destination, bool push, bool restartable, bool testBoundaryConditions, IResultsFeedback resultFeedback, ImageRenderingCallbackInternal imageRenderingCallback, Stream formatTraceStream, Stream formatOutputTraceStream, Encoding preferredEncoding) : base(formatOutputTraceStream)
		{
			this.output = new RtfOutput(destination, push, restartable);
			this.resultFeedback = resultFeedback;
			this.restartable = restartable;
			this.imageRenderingCallback = imageRenderingCallback;
			this.preferredEncoding = preferredEncoding;
		}

		public RtfFormatOutput(RtfOutput output, Stream formatTraceStream, Stream formatOutputTraceStream) : base(formatOutputTraceStream)
		{
			this.output = output;
		}

		public override bool CanAcceptMoreOutput
		{
			get
			{
				return this.output.CanAcceptMoreOutput;
			}
		}

		bool IRestartable.CanRestart()
		{
			return this.restartable && ((IRestartable)this.output).CanRestart();
		}

		void IRestartable.Restart()
		{
			((IRestartable)this.output).Restart();
			base.Restart();
			this.tableLevel = 0;
			this.listLevel = 0;
			this.startedBlock = false;
			this.fontNameDictionary.Clear();
			this.colorDictionary.Clear();
			this.fontsTop = 0;
			this.delayedBottomMargin = 0;
			this.restartable = false;
		}

		void IRestartable.DisableRestart()
		{
			((IRestartable)this.output).DisableRestart();
			this.restartable = false;
		}

		public override void Initialize(FormatStore store, SourceFormat sourceFormat, string comment)
		{
			base.Initialize(store, sourceFormat, comment);
			store.InitializeCodepageDetector();
		}

		public void OutputColors(int nextColorIndex)
		{
			this.firstColorHandle = nextColorIndex;
			this.AddColor(new PropertyValue(new RGBT(192U, 192U, 192U)));
			this.BuildColorsTable(base.FormatStore.RootNode);
			this.OutputColorsTableEntries();
		}

		public void OutputFonts(int firstAvailableFontHandle)
		{
			int bestWindowsCodePage = base.FormatStore.GetBestWindowsCodePage();
			Encoding utf;
			if (!Charset.TryGetEncoding(bestWindowsCodePage, out utf))
			{
				utf = Encoding.UTF8;
			}
			this.output.SetEncoding(utf);
			int charset = RtfSupport.CharSetFromCodePage((ushort)bestWindowsCodePage);
			this.firstFontHandle = firstAvailableFontHandle;
			int num;
			if (!this.fontNameDictionary.TryGetValue("Symbol", out num))
			{
				this.fonts[this.fontsTop].Name = "Symbol";
				this.fonts[this.fontsTop].SymbolFont = true;
				this.fontNameDictionary.Add(this.fonts[this.fontsTop].Name, this.fontsTop);
				this.symbolFont = this.fontsTop;
				this.fontsTop++;
			}
			else
			{
				this.symbolFont = num;
			}
			this.BuildFontsTable(base.FormatStore.RootNode);
			this.OutputFontsTableEntries(charset);
		}

		public void WriteText(string buffer)
		{
			this.WriteText(buffer, 0, buffer.Length);
		}

		public void WriteText(string buffer, int offset, int count)
		{
			this.HtmlRtfOffReally();
			this.output.WriteText(buffer, offset, count);
		}

		public void WriteText(char[] buffer, int offset, int count)
		{
			this.HtmlRtfOffReally();
			this.output.WriteText(buffer, offset, count);
		}

		public void WriteEncapsulatedMarkupText(char[] buffer, int offset, int count)
		{
			this.HtmlRtfOffReally();
			this.output.WriteEncapsulatedMarkupText(buffer, offset, count);
		}

		public void WriteDoubleEscapedText(string buffer)
		{
			this.WriteDoubleEscapedText(buffer, 0, buffer.Length);
		}

		public void WriteDoubleEscapedText(string buffer, int offset, int count)
		{
			this.HtmlRtfOffReally();
			this.output.WriteDoubleEscapedText(buffer, offset, count);
		}

		protected override bool StartRoot()
		{
			return true;
		}

		protected override void EndRoot()
		{
		}

		protected override bool StartDocument()
		{
			int num;
			if (this.preferredEncoding != null)
			{
				num = CodePageMap.GetCodePage(this.preferredEncoding);
				num = base.FormatStore.GetBestWindowsCodePage(num);
			}
			else
			{
				num = base.FormatStore.GetBestWindowsCodePage();
			}
			Encoding utf;
			if (!Charset.TryGetEncoding(num, out utf))
			{
				utf = Encoding.UTF8;
			}
			this.output.SetEncoding(utf);
			if (this.resultFeedback != null)
			{
				this.resultFeedback.Set(ConfigParameter.OutputEncoding, utf);
			}
			int charset = RtfSupport.CharSetFromCodePage((ushort)num);
			this.fonts[this.fontsTop].Name = "Times New Roman";
			this.fontNameDictionary.Add(this.fonts[this.fontsTop].Name, this.fontsTop);
			int value = this.fontsTop;
			this.fontsTop++;
			this.fonts[this.fontsTop].Name = "Symbol";
			this.fonts[this.fontsTop].SymbolFont = true;
			this.fontNameDictionary.Add(this.fonts[this.fontsTop].Name, this.fontsTop);
			this.symbolFont = this.fontsTop;
			this.fontsTop++;
			this.firstColorHandle = 1;
			this.AddColor(new PropertyValue(new RGBT(192U, 192U, 192U)));
			this.BuildTables(base.CurrentNode);
			this.WriteControlText("{\\rtf1\\ansi", true);
			this.WriteControlText("\\fbidis", true);
			this.WriteKeyword("\\ansicpg", num);
			this.WriteKeyword("\\deff", value);
			if (base.SourceFormat == SourceFormat.Text)
			{
				this.WriteControlText("\\deftab720\\fromtext", true);
			}
			else if (base.SourceFormat == SourceFormat.HtmlEncapsulateMarkup)
			{
				this.WriteControlText("\\fromhtml1", true);
			}
			this.WriteControlText("{\\fonttbl", true);
			this.OutputFontsTableEntries(charset);
			this.WriteControlText("}\r\n", false);
			this.output.RtfLineLength = 0;
			this.WriteControlText("{\\colortbl;", false);
			this.OutputColorsTableEntries();
			this.WriteControlText("}\r\n", false);
			this.output.RtfLineLength = 0;
			this.WriteControlText("{\\*\\generator Microsoft Exchange Server;}\r\n", false);
			this.output.RtfLineLength = 0;
			if (base.Comment != null)
			{
				this.WriteControlText("{\\*\\formatConverter ", false);
				this.WriteControlText(base.Comment, false);
				this.WriteControlText(";}\r\n", false);
				this.output.RtfLineLength = 0;
			}
			if (!base.CurrentNode.FirstChild.IsNull && base.CurrentNode.FirstChild == base.CurrentNode.LastChild && (byte)(base.CurrentNode.LastChild.NodeType & FormatContainerType.BlockFlag) != 0)
			{
				PropertyValue property = base.CurrentNode.FirstChild.GetProperty(PropertyId.BackColor);
				if (property.IsColor)
				{
					this.WriteControlText("{\\*\\background {\\shp{\\*\\shpinst{\\sp{\\sn fillColor}{\\sv ", false);
					this.WriteControlText(RtfSupport.RGB((int)property.Color.Red, (int)property.Color.Green, (int)property.Color.Blue).ToString(), false);
					this.WriteControlText("}}}}}", false);
				}
			}
			this.WriteControlText("\\viewkind5\\viewscale100\r\n", false);
			this.output.RtfLineLength = 0;
			this.HtmlRtfOn();
			this.WriteControlText("{\\*\\bkmkstart BM_BEGIN}", false);
			this.HtmlRtfOff();
			if (base.SourceFormat == SourceFormat.HtmlEncapsulateMarkup)
			{
				this.WriteControlText("{\\*\\htmltag64}", false);
				this.OutputNodeStartEncapsulatedMarkup();
			}
			return true;
		}

		protected override void EndDocument()
		{
			this.OutputNodeEndEncapsulatedMarkup();
			this.WriteControlText("}\r\n", false);
			this.output.RtfLineLength = 0;
			this.output.Flush();
		}

		protected override bool StartTable()
		{
			this.OutputNodeStartEncapsulatedMarkup();
			if (this.startedBlock)
			{
				this.ReallyEndBlock();
			}
			this.tableLevel++;
			return true;
		}

		protected override void EndTable()
		{
			this.tableLevel--;
			this.OutputNodeEndEncapsulatedMarkup();
		}

		protected override bool StartTableRow()
		{
			this.OutputNodeStartEncapsulatedMarkup();
			if (this.tableLevel == 1)
			{
				this.HtmlRtfOn();
				this.OutputRowProps();
				this.HtmlRtfOff();
			}
			return true;
		}

		protected override void EndTableRow()
		{
			this.HtmlRtfOn();
			this.OutputTableLevel();
			if (this.tableLevel > 1)
			{
				this.WriteControlText("{\\*\\nesttableprops", true);
				this.OutputRowProps();
				this.WriteControlText("\\nestrow}{\\nonesttables\\par}\r\n", false);
				this.textPosition += 2;
				this.output.RtfLineLength = 0;
			}
			else
			{
				this.WriteControlText("\\row\r\n", false);
				this.textPosition += 2;
				this.output.RtfLineLength = 0;
			}
			this.HtmlRtfOff();
			this.OutputNodeEndEncapsulatedMarkup();
		}

		protected override bool StartTableCell()
		{
			this.OutputNodeStartEncapsulatedMarkup();
			this.textPosition += 5;
			return true;
		}

		protected override void EndTableCell()
		{
			this.HtmlRtfOn();
			if (!this.startedBlock)
			{
				this.OutputTableLevel();
				this.OutputBlockProps();
			}
			if (this.tableLevel > 1)
			{
				this.WriteControlText("\\nestcell{\\nonesttables\\tab}", false);
			}
			else
			{
				this.WriteControlText("\\cell", true);
			}
			this.textPosition++;
			this.startedBlock = false;
			this.HtmlRtfOff();
			this.OutputNodeEndEncapsulatedMarkup();
		}

		protected override bool StartTableCaption()
		{
			if (base.CurrentNode.Parent.NodeType == FormatContainerType.Table)
			{
				this.tableLevel--;
			}
			return this.StartBlockContainer();
		}

		protected override void EndTableCaption()
		{
			this.EndBlockContainer();
			if (this.startedBlock)
			{
				this.ReallyEndBlock();
			}
			if (base.CurrentNode.Parent.NodeType == FormatContainerType.Table)
			{
				this.tableLevel++;
			}
		}

		protected override bool StartList()
		{
			this.OutputNodeStartEncapsulatedMarkup();
			if (this.startedBlock)
			{
				this.ReallyEndBlock();
			}
			if (this.listStack == null)
			{
				this.listStack = new RtfFormatOutput.ListLevel[8];
			}
			else if (this.listStack.Length == this.listLevel)
			{
				RtfFormatOutput.ListLevel[] destinationArray = new RtfFormatOutput.ListLevel[this.listStack.Length * 2];
				Array.Copy(this.listStack, 0, destinationArray, 0, this.listLevel);
				this.listStack = destinationArray;
			}
			if (this.listLevel == -1)
			{
				this.listLevel = 0;
			}
			this.listStack[this.listLevel].Reset();
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.ListStyle);
			if (!effectiveProperty.IsNull)
			{
				this.listStack[this.listLevel].ListType = (RtfNumbering)effectiveProperty.Enum;
			}
			effectiveProperty = base.GetEffectiveProperty(PropertyId.ListStart);
			if (!effectiveProperty.IsNull)
			{
				this.listStack[this.listLevel].NextIndex = (short)effectiveProperty.Integer;
			}
			this.listLevel++;
			return true;
		}

		protected override void EndList()
		{
			this.listLevel--;
			this.OutputNodeEndEncapsulatedMarkup();
		}

		protected override bool StartListItem()
		{
			this.OutputNodeStartEncapsulatedMarkup();
			if (this.startedBlock)
			{
				this.ReallyEndBlock();
			}
			if (this.listLevel == 0)
			{
				this.listLevel = -1;
			}
			return true;
		}

		protected override void EndListItem()
		{
			if (base.CurrentNode.FirstChild.IsNull)
			{
				this.ReallyStartAppropriateBlock();
			}
			if (this.startedBlock)
			{
				this.ReallyEndBlock();
			}
			if (this.listLevel > 0)
			{
				RtfFormatOutput.ListLevel[] array = this.listStack;
				int num = this.listLevel - 1;
				array[num].NextIndex = array[num].NextIndex + 1;
			}
			else
			{
				this.listLevel = 0;
			}
			this.OutputNodeEndEncapsulatedMarkup();
		}

		protected override bool StartHyperLink()
		{
			this.OutputNodeStartEncapsulatedMarkup();
			this.HtmlRtfOn();
			if (!this.startedBlock)
			{
				this.ReallyStartAppropriateBlock();
			}
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.HyperlinkUrl);
			if (!effectiveProperty.IsNull)
			{
				this.WriteControlText("{\\field{\\*\\fldinst HYPERLINK ", false);
				string text = base.FormatStore.GetStringValue(effectiveProperty).GetString();
				bool flag = false;
				if (text[0] == '#')
				{
					this.WriteControlText("\\\\l ", false);
					if (text.Length > 1)
					{
						if (!char.IsLetter(text[1]))
						{
							text = "BM_" + text.Substring(1);
						}
						else
						{
							text = text.Substring(1);
						}
					}
					else
					{
						text = string.Empty;
					}
					flag = true;
				}
				this.WriteControlText("\"", false);
				if (text.Length != 0)
				{
					this.WriteDoubleEscapedText(text);
				}
				else if (flag)
				{
					this.WriteControlText("BM_BEGIN", false);
				}
				else
				{
					this.WriteControlText("http://", false);
				}
				this.WriteControlText("\" }{\\fldrslt", true);
			}
			this.HtmlRtfOff();
			return true;
		}

		protected override void EndHyperLink()
		{
			this.HtmlRtfOn();
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.HyperlinkUrl);
			if (!effectiveProperty.IsNull)
			{
				this.WriteControlText("}}", false);
				string @string = base.FormatStore.GetStringValue(effectiveProperty).GetString();
				this.textPosition += @string.Length + 2;
			}
			FormatNode nextSibling = base.CurrentNode.NextSibling;
			if (!nextSibling.IsNull && (nextSibling.NodeType == FormatContainerType.Block || nextSibling.NodeType == FormatContainerType.List || nextSibling.NodeType == FormatContainerType.Table || nextSibling.NodeType == FormatContainerType.HorizontalLine))
			{
				this.EndBlockContainer();
			}
			this.HtmlRtfOff();
			this.OutputNodeEndEncapsulatedMarkup();
		}

		protected override bool StartBookmark()
		{
			this.OutputNodeStartEncapsulatedMarkup();
			this.HtmlRtfOn();
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.BookmarkName);
			if (!effectiveProperty.IsNull)
			{
				string @string = base.FormatStore.GetStringValue(effectiveProperty).GetString();
				if (@string != "BM_BEGIN")
				{
					this.WriteControlText("{\\*\\bkmkstart", true);
					if (!char.IsLetter(@string[0]))
					{
						this.WriteText("BM_");
					}
					this.WriteText(@string);
					this.WriteControlText("}", false);
				}
			}
			this.HtmlRtfOff();
			return true;
		}

		protected override void EndBookmark()
		{
			this.HtmlRtfOn();
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.BookmarkName);
			if (!effectiveProperty.IsNull)
			{
				string @string = base.FormatStore.GetStringValue(effectiveProperty).GetString();
				if (@string != "BM_BEGIN")
				{
					this.WriteControlText("{\\*\\bkmkend", true);
					this.WriteText(@string);
					this.WriteControlText("}", false);
				}
			}
			this.HtmlRtfOff();
			this.OutputNodeEndEncapsulatedMarkup();
		}

		protected override void StartEndImage()
		{
			this.OutputNodeStartEncapsulatedMarkup();
			this.HtmlRtfOn();
			if (!this.startedBlock)
			{
				this.ReallyStartAppropriateBlock();
			}
			PropertyValue propertyValue = base.GetEffectiveProperty(PropertyId.Width);
			PropertyValue propertyValue2 = base.GetEffectiveProperty(PropertyId.Height);
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.ImageUrl);
			PropertyValue effectiveProperty2 = base.GetEffectiveProperty(PropertyId.ImageAltText);
			string text = null;
			if (!effectiveProperty.IsNull)
			{
				text = base.FormatStore.GetStringValue(effectiveProperty).GetString();
			}
			string text2 = null;
			if (!effectiveProperty2.IsNull)
			{
				text2 = base.FormatStore.GetStringValue(effectiveProperty2).GetString();
			}
			bool flag = false;
			if (this.imageRenderingCallback != null && text != null)
			{
				flag = this.imageRenderingCallback(text, this.textPosition);
			}
			if (flag)
			{
				this.WriteControlText("\\objattph  ", false);
				this.textPosition++;
			}
			else
			{
				if (propertyValue.IsNull || (!propertyValue.IsAbsLength && !propertyValue.IsPixels))
				{
					propertyValue = PropertyValue.Null;
				}
				else if (propertyValue.TwipsInteger == 0)
				{
					propertyValue.Set(LengthUnits.Pixels, 1f);
				}
				if (propertyValue2.IsNull || (!propertyValue2.IsAbsLength && !propertyValue2.IsPixels))
				{
					propertyValue2 = PropertyValue.Null;
				}
				else if (propertyValue2.TwipsInteger == 0)
				{
					propertyValue2.Set(LengthUnits.Pixels, 1f);
				}
				if (text != null)
				{
					this.WriteControlText("{\\field{\\*\\fldinst INCLUDEPICTURE \"", false);
					this.WriteDoubleEscapedText(text);
					this.WriteControlText("\" \\\\d \\\\* MERGEFORMAT}{\\fldrslt", true);
				}
				this.WriteControlText("{\\pict{\\*\\picprop{\\sp{\\sn fillColor}{\\sv 14286846}}{\\sp{\\sn fillOpacity}{\\sv 16384}}{\\sp{\\sn fFilled}{\\sv 1}}", false);
				if (text2 != null)
				{
					this.WriteControlText("{\\sp{\\sn wzDescription}{\\sv ", false);
					this.WriteText(text2);
					this.WriteControlText("}}", false);
				}
				this.WriteControlText("}", false);
				this.WriteControlText("\\brdrt\\brdrs\\brdrw10", true);
				this.WriteKeyword("\\brdrcf", this.firstColorHandle);
				this.WriteControlText("\\brdrl\\brdrs\\brdrw10", true);
				this.WriteKeyword("\\brdrcf", this.firstColorHandle);
				this.WriteControlText("\\brdrb\\brdrs\\brdrw10", true);
				this.WriteKeyword("\\brdrcf", this.firstColorHandle);
				this.WriteControlText("\\brdrr\\brdrs\\brdrw10", true);
				this.WriteKeyword("\\brdrcf", this.firstColorHandle);
				if (!propertyValue.IsNull)
				{
					this.WriteKeyword("\\picwgoal", propertyValue.TwipsInteger);
				}
				if (!propertyValue2.IsNull)
				{
					this.WriteKeyword("\\pichgoal", propertyValue2.TwipsInteger);
				}
				this.WriteControlText("\\wmetafile8 0100090000032100000000000500000000000400000003010800050000000b0200000000050000000c0202000200030000001e00040000002701ffff030000000000}", false);
				if (text != null)
				{
					this.WriteControlText("}}", false);
				}
				if ((propertyValue2.IsNull || propertyValue2.PixelsInteger >= 8) && (propertyValue.IsNull || propertyValue.PixelsInteger >= 8) && text != null)
				{
					this.textPosition += text.Length + 2;
				}
			}
			FormatNode nextSibling = base.CurrentNode.NextSibling;
			if (!nextSibling.IsNull && (nextSibling.NodeType == FormatContainerType.Block || nextSibling.NodeType == FormatContainerType.List || nextSibling.NodeType == FormatContainerType.Table || nextSibling.NodeType == FormatContainerType.HorizontalLine))
			{
				this.EndBlockContainer();
			}
			this.HtmlRtfOff();
			this.OutputNodeEndEncapsulatedMarkup();
		}

		protected override void StartEndHorizontalLine()
		{
			this.OutputNodeStartEncapsulatedMarkup();
			this.HtmlRtfOn();
			if (this.startedBlock)
			{
				this.WriteControlText("\\par\r\n", false);
				this.textPosition += 2;
				this.output.RtfLineLength = 0;
				this.startedBlock = false;
			}
			this.OutputTableLevel();
			this.WriteControlText("\\plain", true);
			this.WriteControlText("{\\f0\\qc\\qd\\cf1\\ulth\\~ ________________________________ \\~\\par}\r\n", false);
			this.textPosition += 36;
			this.output.RtfLineLength = 0;
			this.HtmlRtfOff();
			this.OutputNodeEndEncapsulatedMarkup();
		}

		protected override void StartEndArea()
		{
		}

		protected override bool StartOption()
		{
			return true;
		}

		protected override bool StartText()
		{
			this.OutputNodeStartEncapsulatedMarkup();
			this.HtmlRtfOn();
			if (!this.startedBlock)
			{
				this.ReallyStartAppropriateBlock();
			}
			this.WriteControlText("{", false);
			this.OutputTextProps();
			this.HtmlRtfOff();
			return true;
		}

		protected override bool ContinueText(uint beginTextPosition, uint endTextPosition)
		{
			this.OutputTextRuns(beginTextPosition, endTextPosition, false);
			return true;
		}

		protected override void EndText()
		{
			this.HtmlRtfOn();
			this.WriteControlText("}", false);
			this.HtmlRtfOff();
			this.OutputNodeEndEncapsulatedMarkup();
		}

		protected override bool StartBlockContainer()
		{
			this.OutputNodeStartEncapsulatedMarkup();
			if (this.startedBlock)
			{
				this.ReallyEndBlock();
			}
			if (base.CurrentNode.FirstChild.IsNull || (byte)(base.CurrentNode.FirstChild.NodeType & FormatContainerType.BlockFlag) == 0)
			{
				this.ReallyStartAppropriateBlock();
			}
			return true;
		}

		protected override void EndBlockContainer()
		{
			if (this.startedBlock && base.CurrentNode != base.CurrentNode.Parent.LastChild)
			{
				this.ReallyEndBlock();
			}
			this.OutputNodeEndEncapsulatedMarkup();
		}

		protected override void Dispose(bool disposing)
		{
			if (this.output != null && this.output != null)
			{
				((IDisposable)this.output).Dispose();
			}
			this.fonts = null;
			this.fontNameDictionary = null;
			this.colors = null;
			this.colorDictionary = null;
			this.output = null;
			base.Dispose(disposing);
		}

		private static bool IsTaggedFontName(string name, out int lengthWithoutTag)
		{
			lengthWithoutTag = name.Length;
			for (int i = 0; i < RtfFormatOutput.FontSuffixes.Length; i++)
			{
				if (RtfFormatOutput.FontSuffixes[i].Length < name.Length && name.EndsWith(RtfFormatOutput.FontSuffixes[i], StringComparison.OrdinalIgnoreCase))
				{
					lengthWithoutTag -= RtfFormatOutput.FontSuffixes[i].Length;
					return true;
				}
			}
			return false;
		}

		private static int ConvertLengthToHalfPoints(PropertyValue pv)
		{
			switch (pv.Type)
			{
			case PropertyType.Percentage:
				return 0;
			case PropertyType.AbsLength:
				return pv.BaseUnits / 8 / 10;
			case PropertyType.RelLength:
				return 0;
			case PropertyType.Pixels:
				return pv.Value / 8 / 10;
			case PropertyType.Ems:
				return pv.Value / 8 / 10;
			case PropertyType.Exs:
				return pv.Value / 8 / 10;
			case PropertyType.HtmlFontUnits:
				return PropertyValue.ConvertHtmlFontUnitsToTwips(pv.HtmlFontUnits) / 10;
			case PropertyType.RelHtmlFontUnits:
				return 0;
			default:
				return 0;
			}
		}

		private void HtmlRtfOn()
		{
			if (base.SourceFormat == SourceFormat.HtmlEncapsulateMarkup)
			{
				if (!this.htmlRtfOn)
				{
					this.htmlRtfOn = true;
					this.WriteControlText("\\htmlrtf", true);
					return;
				}
				if (this.htmlRtfOnOff)
				{
					this.htmlRtfOnOff = false;
				}
			}
		}

		private void HtmlRtfOff()
		{
			if (this.htmlRtfOn)
			{
				this.htmlRtfOnOff = true;
			}
		}

		private void HtmlRtfOffReally()
		{
			if (this.htmlRtfOnOff && this.outOfOrderNesting == 0)
			{
				this.htmlRtfOnOff = false;
				this.htmlRtfOn = false;
				this.WriteControlText("\\htmlrtf0", true);
			}
		}

		private void OutputTextRuns(uint start, uint end, bool encapsulatedMarkup)
		{
			if (start != end)
			{
				TextRun textRun = base.FormatStore.GetTextRun(start);
				do
				{
					int num = textRun.EffectiveLength;
					TextRunType type = textRun.Type;
					if (type <= TextRunType.NbSp)
					{
						if (type != TextRunType.Markup)
						{
							if (type != TextRunType.NonSpace)
							{
								if (type == TextRunType.NbSp)
								{
									this.textPosition += num;
									int num2 = 0;
									do
									{
										this.WriteText(" ");
									}
									while (++num2 < num);
								}
							}
							else
							{
								this.textPosition += num;
								int num2 = 0;
								do
								{
									char[] buffer;
									int offset;
									int num3;
									textRun.GetChunk(num2, out buffer, out offset, out num3);
									this.WriteText(buffer, offset, num3);
									num2 += num3;
								}
								while (num2 < num);
							}
						}
						else if (this.outOfOrderNesting == 0)
						{
							if (!encapsulatedMarkup)
							{
								this.WriteControlText("{\\*\\htmltag0 ", false);
							}
							int num2 = 0;
							do
							{
								char[] buffer;
								int offset;
								int num3;
								textRun.GetChunk(num2, out buffer, out offset, out num3);
								this.WriteEncapsulatedMarkupText(buffer, offset, num3);
								num2 += num3;
							}
							while (num2 < num);
							if (!encapsulatedMarkup)
							{
								this.WriteControlText("}", false);
							}
						}
					}
					else if (type != TextRunType.Space)
					{
						if (type != TextRunType.Tabulation)
						{
							if (type == TextRunType.NewLine)
							{
								this.textPosition += num;
								if (!encapsulatedMarkup)
								{
									this.HtmlRtfOn();
									int num2 = 0;
									do
									{
										this.WriteControlText("\\line\r\n", false);
									}
									while (++num2 < num);
									this.output.RtfLineLength = 0;
									this.HtmlRtfOff();
								}
								if (base.SourceFormat == SourceFormat.HtmlEncapsulateMarkup && this.outOfOrderNesting == 0)
								{
									if (!encapsulatedMarkup)
									{
										this.WriteControlText("{\\*\\htmltag0", true);
									}
									do
									{
										this.WriteControlText("\\par", true);
									}
									while (0 < --num);
									if (!encapsulatedMarkup)
									{
										this.WriteControlText("}", false);
									}
								}
							}
						}
						else
						{
							this.textPosition += num;
							if (encapsulatedMarkup)
							{
								this.HtmlRtfOn();
								int num2 = 0;
								do
								{
									this.WriteControlText("\\tab", true);
								}
								while (++num2 < num);
								this.HtmlRtfOff();
							}
							if (base.SourceFormat == SourceFormat.HtmlEncapsulateMarkup && this.outOfOrderNesting == 0)
							{
								if (!encapsulatedMarkup)
								{
									this.WriteControlText("{\\*\\htmltag0 ", false);
								}
								do
								{
									this.WriteText("\t");
								}
								while (0 < --num);
								if (!encapsulatedMarkup)
								{
									this.WriteControlText("}", false);
								}
							}
						}
					}
					else
					{
						this.textPosition += num;
						do
						{
							this.WriteText(" ");
						}
						while (0 < --num);
					}
					textRun.MoveNext();
				}
				while (textRun.Position < end);
			}
		}

		private void OutputNodeStartEncapsulatedMarkup()
		{
			if (base.SourceFormat == SourceFormat.HtmlEncapsulateMarkup)
			{
				if (!base.CurrentNode.IsInOrder)
				{
					this.outOfOrderNesting++;
					return;
				}
				if (base.CurrentNode.IsText || this.outOfOrderNesting != 0)
				{
					return;
				}
				uint beginTextPosition = base.CurrentNode.BeginTextPosition;
				FormatNode formatNode = base.CurrentNode.FirstChild;
				while (!formatNode.IsNull && !formatNode.IsInOrder)
				{
					formatNode = formatNode.NextSibling;
				}
				uint num;
				if (!formatNode.IsNull)
				{
					num = formatNode.BeginTextPosition;
				}
				else
				{
					num = base.CurrentNode.EndTextPosition;
				}
				if (beginTextPosition != num)
				{
					this.WriteControlText("{\\*\\htmltag0 ", false);
					this.OutputTextRuns(beginTextPosition, num, true);
					this.WriteControlText("}", false);
					this.runningPosition = num;
				}
			}
		}

		private void OutputNodeEndEncapsulatedMarkup()
		{
			if (base.SourceFormat == SourceFormat.HtmlEncapsulateMarkup)
			{
				if (this.outOfOrderNesting > 0)
				{
					if (!base.CurrentNode.IsInOrder)
					{
						this.outOfOrderNesting--;
					}
					return;
				}
				uint endTextPosition = base.CurrentNode.EndTextPosition;
				FormatNode nextSibling = base.CurrentNode.NextSibling;
				while (!nextSibling.IsNull && !nextSibling.IsInOrder)
				{
					nextSibling = nextSibling.NextSibling;
				}
				uint num;
				if (!nextSibling.IsNull)
				{
					num = nextSibling.BeginTextPosition;
				}
				else
				{
					num = base.CurrentNode.Parent.EndTextPosition;
				}
				if (endTextPosition < num && num >= this.runningPosition)
				{
					if (endTextPosition < this.runningPosition)
					{
						endTextPosition = this.runningPosition;
					}
					if (endTextPosition != num)
					{
						this.WriteControlText("{\\*\\htmltag0 ", false);
						this.OutputTextRuns(endTextPosition, num, true);
						this.WriteControlText("}", false);
						this.runningPosition = num;
					}
				}
			}
		}

		private void ReallyStartAppropriateBlock()
		{
			this.OutputTableLevel();
			FormatNode currentNode = base.CurrentNode;
			if (currentNode.NodeType != FormatContainerType.Document)
			{
				this.OutputBlockProps();
			}
			if (this.listLevel != 0)
			{
				this.OutputListProperties();
			}
			this.WriteControlText("\\plain", true);
			if (currentNode.GetProperty(PropertyId.FontFace).IsNull)
			{
				this.WriteControlText("\\f0", true);
			}
			this.startedBlock = true;
		}

		private void ReallyEndBlock()
		{
			this.HtmlRtfOn();
			this.WriteControlText("\\par\r\n", false);
			this.textPosition += 2;
			this.output.RtfLineLength = 0;
			this.startedBlock = false;
			this.HtmlRtfOff();
		}

		private void BuildTables(FormatNode node)
		{
			foreach (FormatNode formatNode in node.Subtree)
			{
				this.AddFont(formatNode.GetProperty(PropertyId.FontFace));
				this.AddColor(formatNode.GetProperty(PropertyId.FontColor));
				this.AddColor(formatNode.GetProperty(PropertyId.BackColor));
				this.AddColor(formatNode.GetProperty(PropertyId.BorderColors));
				this.AddColor(formatNode.GetProperty(PropertyId.RightBorderColor));
				this.AddColor(formatNode.GetProperty(PropertyId.BottomBorderColor));
				this.AddColor(formatNode.GetProperty(PropertyId.LeftBorderColor));
			}
		}

		private void BuildFontsTable(FormatNode node)
		{
			foreach (FormatNode formatNode in node.Subtree)
			{
				this.AddFont(formatNode.GetProperty(PropertyId.FontFace));
			}
		}

		private void BuildColorsTable(FormatNode node)
		{
			foreach (FormatNode formatNode in node.Subtree)
			{
				this.AddFont(formatNode.GetProperty(PropertyId.FontFace));
				this.AddColor(formatNode.GetProperty(PropertyId.FontColor));
				this.AddColor(formatNode.GetProperty(PropertyId.BackColor));
				this.AddColor(formatNode.GetProperty(PropertyId.BorderColors));
				this.AddColor(formatNode.GetProperty(PropertyId.RightBorderColor));
				this.AddColor(formatNode.GetProperty(PropertyId.BottomBorderColor));
				this.AddColor(formatNode.GetProperty(PropertyId.LeftBorderColor));
			}
		}

		private void OutputFontsTableEntries(int charset)
		{
			for (int i = 0; i < this.fontsTop; i++)
			{
				this.WriteKeyword("{\\f", i + this.firstFontHandle);
				this.WriteControlText("\\fswiss", true);
				if (!this.fonts[i].SymbolFont)
				{
					this.WriteKeyword("\\fcharset", charset);
				}
				else
				{
					this.WriteKeyword("\\fcharset", 2);
				}
				int count;
				if (RtfFormatOutput.IsTaggedFontName(this.fonts[i].Name, out count))
				{
					this.WriteControlText("{\\fname", true);
					this.WriteText(this.fonts[i].Name);
					this.WriteControlText(";}", false);
					this.WriteText(this.fonts[i].Name, 0, count);
					this.WriteControlText(";}", false);
				}
				else
				{
					this.WriteText(this.fonts[i].Name);
					this.WriteControlText(";}", false);
				}
			}
		}

		private void OutputColorsTableEntries()
		{
			for (int i = 0; i < this.colorsTop; i++)
			{
				this.WriteKeyword("\\red", (int)this.colors[i].Red);
				this.WriteKeyword("\\green", (int)this.colors[i].Green);
				this.WriteKeyword("\\blue", (int)this.colors[i].Blue);
				this.WriteControlText(";", false);
			}
		}

		private void AddColor(PropertyValue pv)
		{
			if (pv.IsEnum)
			{
				pv = HtmlSupport.TranslateSystemColor(pv);
			}
			if (pv.IsColor && !this.colorDictionary.ContainsKey(pv.Color.RawValue) && this.colorsTop < this.colors.Length)
			{
				this.colors[this.colorsTop] = pv.Color;
				this.colorDictionary.Add(pv.Color.RawValue, this.colorsTop);
				this.colorsTop++;
			}
		}

		private int FindColorHandle(PropertyValue pv)
		{
			if (pv.IsEnum)
			{
				pv = HtmlSupport.TranslateSystemColor(pv);
			}
			int num = 0;
			if (pv.IsColor && !pv.Color.IsTransparent && this.colorDictionary.TryGetValue(pv.Color.RawValue, out num))
			{
				return num + this.firstColorHandle;
			}
			return 0;
		}

		private void AddFont(PropertyValue pv)
		{
			if (!pv.IsNull && this.fontsTop < this.fonts.Length)
			{
				string text = null;
				if (pv.IsString)
				{
					text = base.FormatStore.GetStringValue(pv).GetString();
				}
				else if (pv.IsMultiValue)
				{
					MultiValue multiValue = base.FormatStore.GetMultiValue(pv);
					if (multiValue.Length > 0)
					{
						text = multiValue.GetStringValue(0).GetString();
					}
				}
				if (text != null)
				{
					int num;
					if (!this.fontNameDictionary.TryGetValue(text, out num))
					{
						num = this.fontsTop;
						this.fonts[num].Name = text;
						this.fontNameDictionary.Add(text, num);
						foreach (string value in RtfFormatOutput.symbolFonts)
						{
							if (text.Equals(value, StringComparison.OrdinalIgnoreCase))
							{
								this.fonts[num].SymbolFont = true;
								break;
							}
						}
						this.fontsTop++;
					}
					RtfFormatOutput.OutputFont[] array2 = this.fonts;
					int num2 = num;
					array2[num2].Count = array2[num2].Count + 1;
				}
			}
		}

		private int FindFontHandle(PropertyValue pv, out bool symbolFont)
		{
			int num = 0;
			symbolFont = false;
			if (!pv.IsNull)
			{
				string text = null;
				if (pv.IsString)
				{
					text = base.FormatStore.GetStringValue(pv).GetString();
				}
				else if (pv.IsMultiValue)
				{
					MultiValue multiValue = base.FormatStore.GetMultiValue(pv);
					if (multiValue.Length > 0)
					{
						text = multiValue.GetStringValue(0).GetString();
					}
				}
				if (text != null && this.fontNameDictionary.TryGetValue(text, out num))
				{
					symbolFont = this.fonts[num].SymbolFont;
					return num + this.firstFontHandle;
				}
			}
			return 0;
		}

		private void OutputTableLevel()
		{
			this.WriteControlText("\\pard", true);
			if (this.tableLevel > 0)
			{
				this.WriteControlText("\\intbl", true);
				if (this.tableLevel > 1)
				{
					this.WriteKeyword("\\itap", this.tableLevel);
				}
			}
		}

		private void OutputListProperties()
		{
			bool flag = false;
			FormatNode x = base.CurrentNode;
			while (x.NodeType != FormatContainerType.Root && x.NodeType != FormatContainerType.ListItem)
			{
				if ((byte)(x.NodeType & FormatContainerType.BlockFlag) != 0 && !x.PreviousSibling.IsNull)
				{
					flag = true;
				}
				x = x.Parent;
			}
			if (x.NodeType != FormatContainerType.Root)
			{
				bool flag2 = x == x.Parent.FirstChild;
				bool flag3 = x == x.Parent.LastChild;
				x = x.Parent;
				if (!x.IsNull)
				{
					if (flag2)
					{
						PropertyValue property = x.GetProperty(PropertyId.Margins);
						if (property.IsAbsRelLength)
						{
							this.WriteKeyword("\\sb", property.TwipsInteger);
						}
					}
					if (flag3)
					{
						PropertyValue property2 = x.GetProperty(PropertyId.BottomMargin);
						if (property2.IsAbsRelLength)
						{
							this.WriteKeyword("\\sa", property2.TwipsInteger);
						}
					}
				}
			}
			if (this.listLevel == 1 && this.listStack[this.listLevel - 1].ListType == RtfNumbering.Arabic)
			{
				if (flag)
				{
					this.WriteControlText("\\pnlvlcont", true);
					this.WriteControlText("\\pnindent360", true);
					return;
				}
				this.WriteControlText("{\\pntext", true);
				string text = this.listStack[this.listLevel - 1].NextIndex.ToString();
				this.WriteText(text);
				this.WriteControlText(". ", false);
				if (text.Length == 1)
				{
					this.WriteControlText(" ", false);
				}
				this.WriteControlText("}", false);
				this.WriteControlText("{\\*\\pn", true);
				this.WriteControlText("\\pnlvlbody", true);
				this.WriteControlText("\\pndec", true);
				this.WriteKeyword("\\pnstart", (int)this.listStack[this.listLevel - 1].NextIndex);
				this.WriteControlText("\\pnindent360", true);
				this.WriteControlText("\\pnql", true);
				this.WriteControlText("{\\pntxta.}}", false);
				return;
			}
			else
			{
				if (flag)
				{
					this.WriteControlText("\\pnlvlcont", true);
					this.WriteControlText("\\pnindent240", true);
					return;
				}
				this.WriteControlText("{\\pntext", true);
				this.WriteControlText("*   }", false);
				this.WriteControlText("{\\*\\pn", true);
				this.WriteControlText("\\pnlvlblt", true);
				this.WriteKeyword("\\pnf", this.firstFontHandle + this.symbolFont);
				this.WriteControlText("\\pnindent240", true);
				this.WriteControlText("\\pnql", true);
				this.WriteControlText("{\\pntxtb\\'B7}}", false);
				return;
			}
		}

		private void OutputTextProps()
		{
			PropertyValue pv = base.GetEffectiveProperty(PropertyId.FontFace);
			if (!pv.IsNull)
			{
				bool flag;
				int value = this.FindFontHandle(pv, out flag);
				if (flag)
				{
					this.WriteKeyword("\\loch\\af", value);
					this.WriteControlText("\\dbch\\af0\\hich", true);
				}
				this.WriteKeyword("\\f", value);
			}
			pv = base.GetEffectiveProperty(PropertyId.RightToLeft);
			if (pv.IsBool)
			{
				this.WriteControlText(pv.Bool ? "\\rtlch" : "\\ltrch", true);
			}
			pv = base.GetEffectiveProperty(PropertyId.Language);
			if (pv.IsInteger)
			{
				this.WriteKeyword("\\lang", pv.Integer);
			}
			pv = base.GetEffectiveProperty(PropertyId.FontColor);
			if (pv.IsColor)
			{
				this.WriteKeyword("\\cf", this.FindColorHandle(pv));
			}
			pv = base.GetDistinctProperty(PropertyId.BackColor);
			if (pv.IsColor)
			{
				this.WriteKeyword("\\highlight", this.FindColorHandle(pv));
			}
			pv = base.GetEffectiveProperty(PropertyId.FontSize);
			if (!pv.IsNull)
			{
				int num = RtfFormatOutput.ConvertLengthToHalfPoints(pv);
				if (num != 0)
				{
					this.WriteKeyword("\\fs", num);
				}
			}
			pv = base.GetEffectiveProperty(PropertyId.FirstFlag);
			if (pv.IsBool)
			{
				this.WriteControlText(pv.Bool ? "\\b" : "\\b0", true);
			}
			pv = base.GetEffectiveProperty(PropertyId.Italic);
			if (pv.IsBool)
			{
				this.WriteControlText(pv.Bool ? "\\i" : "\\i0", true);
			}
			pv = base.GetEffectiveProperty(PropertyId.Underline);
			if (pv.IsBool)
			{
				this.WriteControlText(pv.Bool ? "\\ul" : "\\ul0", true);
			}
			pv = base.GetEffectiveProperty(PropertyId.Subscript);
			if (pv.IsBool)
			{
				this.WriteControlText(pv.Bool ? "\\sub" : "\\sub0", true);
			}
			pv = base.GetEffectiveProperty(PropertyId.Superscript);
			if (pv.IsBool)
			{
				this.WriteControlText(pv.Bool ? "\\super" : "\\super0", true);
			}
			pv = base.GetEffectiveProperty(PropertyId.Strikethrough);
			if (pv.IsBool)
			{
				this.WriteControlText(pv.Bool ? "\\strike" : "\\strike0", true);
			}
		}

		private void OutputBlockProps()
		{
			int num = 0;
			int num2 = this.delayedBottomMargin;
			this.delayedBottomMargin = 0;
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.BackColor);
			if (effectiveProperty.IsColor)
			{
				this.WriteKeyword("\\cbpat", this.FindColorHandle(effectiveProperty));
			}
			effectiveProperty = base.GetEffectiveProperty(PropertyId.TextAlignment);
			if (effectiveProperty.IsEnum)
			{
				switch (effectiveProperty.Enum)
				{
				case 1:
					this.WriteControlText("\\qc", true);
					break;
				case 3:
					this.WriteControlText("\\ql", true);
					break;
				case 4:
					this.WriteControlText("\\qr", true);
					break;
				case 6:
					this.WriteControlText("\\qj", true);
					break;
				}
			}
			effectiveProperty = base.GetEffectiveProperty(PropertyId.Margins);
			if (effectiveProperty.IsAbsRelLength && (base.CurrentNode.Parent.IsNull || base.CurrentNode.Parent.FirstChild != base.CurrentNode))
			{
				num = effectiveProperty.TwipsInteger;
			}
			effectiveProperty = base.GetEffectiveProperty(PropertyId.BottomMargin);
			if (effectiveProperty.IsAbsRelLength)
			{
				this.delayedBottomMargin = effectiveProperty.TwipsInteger;
			}
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			if (this.listLevel != 0)
			{
				if (this.listLevel == 1 && this.listStack[this.listLevel - 1].ListType == RtfNumbering.Arabic)
				{
					num3 = this.listLevel * 600;
					num5 = -360;
				}
				else
				{
					num3 = this.listLevel * 600;
					num5 = -240;
				}
			}
			effectiveProperty = base.GetEffectiveProperty(PropertyId.LeftMargin);
			if (effectiveProperty.IsAbsRelLength)
			{
				num3 += effectiveProperty.TwipsInteger;
			}
			effectiveProperty = base.GetEffectiveProperty(PropertyId.RightMargin);
			if (effectiveProperty.IsAbsRelLength)
			{
				num4 += effectiveProperty.TwipsInteger;
			}
			effectiveProperty = base.GetEffectiveProperty(PropertyId.FirstLineIndent);
			if (effectiveProperty.IsAbsRelLength)
			{
				num5 = effectiveProperty.TwipsInteger;
			}
			if (num3 != 0)
			{
				this.WriteKeyword("\\li", num3);
			}
			if (num4 != 0)
			{
				this.WriteKeyword("\\ri", num4);
			}
			if (num5 != 0)
			{
				this.WriteKeyword("\\fi", num5);
			}
			if (num2 != 0 || num != 0)
			{
				int num6;
				if (num < 0 != num2 < 0)
				{
					num6 = num + num2;
				}
				else
				{
					num6 = ((num >= 0) ? Math.Max(num, num2) : Math.Min(num, num2));
				}
				if (num6 != 0)
				{
					this.WriteKeyword("\\sb", num6);
				}
			}
		}

		private void OutputRowProps()
		{
			this.WriteControlText("\\trowd", true);
			this.WriteKeyword("\\irow", base.CurrentNodeIndex);
			this.WriteKeyword("\\irowband", base.CurrentNodeIndex);
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.RightToLeft);
			if (effectiveProperty.IsBool && effectiveProperty.Bool)
			{
				this.WriteControlText("\\rtlrow", true);
			}
			effectiveProperty = base.GetEffectiveProperty(PropertyId.BackColor);
			if (effectiveProperty.IsColor)
			{
				this.WriteKeyword("\\trcbpat", this.FindColorHandle(effectiveProperty));
			}
			effectiveProperty = base.GetEffectiveProperty(PropertyId.Width);
			if (effectiveProperty.IsPercentage)
			{
				this.WriteControlText("\\trftsWidth2", true);
				this.WriteKeyword("\\trwWidth", effectiveProperty.Percentage10K / 200);
			}
			effectiveProperty = base.GetEffectiveProperty(PropertyId.Height);
			if (effectiveProperty.IsAbsLength || effectiveProperty.IsPixels)
			{
				this.WriteKeyword("\\trrh", effectiveProperty.TwipsInteger);
			}
			int num = 8856;
			FormatNode node = base.CurrentNode.FirstChild;
			if (!node.IsNull)
			{
				int num2 = 0;
				while (!node.IsNull)
				{
					num2++;
					node = node.NextSibling;
				}
				int num3 = num / num2;
				int num4 = 0;
				int num5 = 0;
				int num6 = num2;
				node = base.CurrentNode.FirstChild;
				while (!node.IsNull)
				{
					PropertyValue propertyValue;
					PropertyValue propertyValue2;
					this.OutputCellProps(node, out propertyValue, out propertyValue2);
					if (num2 == 1 && !propertyValue2.IsNull && node.FirstChild.IsNull)
					{
						this.WriteKeyword("\\fs", 2);
						if (propertyValue2.IsAbsLength || propertyValue2.IsPixels)
						{
							this.WriteKeyword("\\trrh", propertyValue2.TwipsInteger);
						}
					}
					int num7;
					if (num6 == 1)
					{
						num7 = ((num - num4 > 360) ? (num - num4) : num3);
					}
					else
					{
						num7 = num3;
						if (!propertyValue.IsNull)
						{
							if (propertyValue.IsPercentage)
							{
								num7 = num * (propertyValue.Percentage10K / 100) / 100 / 100;
							}
							else if (propertyValue.IsAbsLength || propertyValue.IsPixels)
							{
								num7 = propertyValue.TwipsInteger;
							}
						}
					}
					num4 += num7;
					this.WriteKeyword("\\cellx", num4);
					node = node.NextSibling;
					num5++;
					num6--;
				}
				return;
			}
			this.WriteKeyword("\\cellx", num);
			this.WriteControlText("\\cell", true);
		}

		private void OutputCellProps(FormatNode node, out PropertyValue cellWidth, out PropertyValue cellHeight)
		{
			bool flag = false;
			cellWidth = PropertyValue.Null;
			cellHeight = PropertyValue.Null;
			using (NodePropertiesEnumerator propertiesEnumerator = node.PropertiesEnumerator)
			{
				foreach (Property property in propertiesEnumerator)
				{
					switch (property.Id)
					{
					case PropertyId.BlockAlignment:
						switch (property.Value.Enum)
						{
						case 1:
							this.WriteControlText("\\clvertalc", true);
							break;
						case 2:
							this.WriteControlText("\\clvertalb", true);
							break;
						}
						break;
					case PropertyId.BackColor:
						this.WriteKeyword("\\clcbpat", this.FindColorHandle(property.Value));
						flag = true;
						break;
					case PropertyId.Width:
						cellWidth = property.Value;
						break;
					case PropertyId.Height:
						cellHeight = property.Value;
						break;
					case PropertyId.Paddings:
						if (property.Value.IsAbsRelLength)
						{
							this.WriteKeyword("\\clpadl", property.Value.TwipsInteger);
							this.WriteControlText("\\clpadfl3", true);
						}
						break;
					case PropertyId.RightPadding:
						if (property.Value.IsAbsRelLength)
						{
							this.WriteKeyword("\\clpadr", property.Value.TwipsInteger);
							this.WriteControlText("\\clpadfr3", true);
						}
						break;
					case PropertyId.BottomPadding:
						if (property.Value.IsAbsRelLength)
						{
							this.WriteKeyword("\\clpadb", property.Value.TwipsInteger);
							this.WriteControlText("\\clpadfb3", true);
						}
						break;
					case PropertyId.LeftPadding:
						if (property.Value.IsAbsRelLength)
						{
							this.WriteKeyword("\\clpadt", property.Value.TwipsInteger);
							this.WriteControlText("\\clpadft3", true);
						}
						break;
					}
				}
			}
			if (!flag)
			{
				PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.BackColor);
				if (!effectiveProperty.IsNull)
				{
					this.WriteKeyword("\\clcbpat", this.FindColorHandle(effectiveProperty));
				}
			}
		}

		private void WriteKeyword(string keyword, int value)
		{
			this.HtmlRtfOffReally();
			this.output.WriteKeyword(keyword, value);
		}

		private void WriteControlText(string controlText, bool lastKeyword)
		{
			this.HtmlRtfOffReally();
			this.output.WriteControlText(controlText, lastKeyword);
		}

		private static readonly string[] FontSuffixes = new string[]
		{
			" CE",
			" Cyr",
			" Greek",
			" Tur",
			" Baltic",
			" UPC"
		};

		private static string[] symbolFonts = new string[]
		{
			"Symbol",
			"Wingdings",
			"Wingdings 2",
			"Wingdings 3",
			"Webdings",
			"Marlett",
			"Map Symbols",
			"ZapfDingbats",
			"Monotype Sorts",
			"MT Extra",
			"Bookshelf Symbol 1",
			"Bookshelf Symbol 2",
			"Bookshelf Symbol 3",
			"Sign Language",
			"Shapes1",
			"Shapes2",
			"Bullets1",
			"Bullets2",
			"Bullets3",
			"Common Bullets",
			"Geographic Symbols",
			"Carta",
			"MICR",
			"Musical Symbols",
			"Sonata",
			"Almanac MT",
			"Bon Apetit MT",
			"Directions MT",
			"Holidays MT",
			"Keystrokes MT",
			"MS Outlook",
			"Parties MT",
			"Signs MT",
			"Sports Three MT",
			"Sports Two MT",
			"Transport MT",
			"Vacation MT"
		};

		private bool startedBlock;

		private IResultsFeedback resultFeedback;

		private bool restartable;

		private RtfOutput output;

		private int tableLevel;

		private RtfFormatOutput.ListLevel[] listStack;

		private int listLevel;

		private int textPosition;

		private Dictionary<string, int> fontNameDictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

		private Dictionary<uint, int> colorDictionary = new Dictionary<uint, int>();

		private RtfFormatOutput.OutputFont[] fonts = new RtfFormatOutput.OutputFont[100];

		private int fontsTop;

		private int firstFontHandle;

		private int symbolFont;

		private RGBT[] colors = new RGBT[100];

		private int colorsTop;

		private int firstColorHandle;

		private Encoding preferredEncoding;

		private int delayedBottomMargin;

		private ImageRenderingCallbackInternal imageRenderingCallback;

		private bool htmlRtfOn;

		private bool htmlRtfOnOff;

		private int outOfOrderNesting;

		private uint runningPosition;

		private struct ListLevel
		{
			public void Reset()
			{
				this.ListType = RtfNumbering.Bullet;
				this.Restart = false;
				this.NextIndex = 1;
			}

			public RtfNumbering ListType;

			public bool Restart;

			public short NextIndex;
		}

		private struct OutputFont
		{
			public string Name;

			public int Count;

			public bool SymbolFont;
		}
	}
}
