using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Enriched
{
	internal class EnrichedFormatOutput : FormatOutput, IRestartable, IFallback
	{
		public EnrichedFormatOutput(ConverterOutput output, Injection injection, bool fallbacks, Stream formatTraceStream, Stream formatOutputTraceStream) : base(formatOutputTraceStream)
		{
			this.output = output;
			this.injection = injection;
			this.fallbacks = fallbacks;
		}

		public override bool OutputCodePageSameAsInput
		{
			get
			{
				return false;
			}
		}

		public override Encoding OutputEncoding
		{
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override bool CanAcceptMoreOutput
		{
			get
			{
				return this.output.CanAcceptMore;
			}
		}

		bool IRestartable.CanRestart()
		{
			return this.output is IRestartable && ((IRestartable)this.output).CanRestart();
		}

		void IRestartable.Restart()
		{
			((IRestartable)this.output).Restart();
			base.Restart();
			this.blockEmpty = true;
			this.blockEnd = false;
			this.lineLength = 0;
			this.insideNofill = 0;
			this.listLevel = 0;
			this.listIndex = 0;
			this.spaceBefore = 0;
			if (this.injection != null)
			{
				this.injection.Reset();
			}
		}

		void IRestartable.DisableRestart()
		{
			if (this.output is IRestartable)
			{
				((IRestartable)this.output).DisableRestart();
			}
		}

		public override bool Flush()
		{
			if (!base.Flush())
			{
				return false;
			}
			this.output.Flush();
			return true;
		}

		byte[] IFallback.GetUnsafeAsciiMap(out byte unsafeAsciiMask)
		{
			unsafeAsciiMask = 1;
			return HtmlSupport.UnsafeAsciiMap;
		}

		bool IFallback.HasUnsafeUnicode()
		{
			return false;
		}

		bool IFallback.TreatNonAsciiAsUnsafe(string charset)
		{
			return false;
		}

		bool IFallback.IsUnsafeUnicode(char ch, bool isFirstChar)
		{
			return false;
		}

		bool IFallback.FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int outputEnd)
		{
			string text = null;
			if (ch == '<')
			{
				text = "<<";
			}
			else if (this.fallbacks)
			{
				text = this.GetSubstitute(ch);
			}
			if (text != null)
			{
				if (outputEnd - outputBufferCount < text.Length)
				{
					return false;
				}
				text.CopyTo(0, outputBuffer, outputBufferCount, text.Length);
				outputBufferCount += text.Length;
			}
			else
			{
				outputBuffer[outputBufferCount++] = ch;
			}
			return true;
		}

		protected override bool StartDocument()
		{
			if (this.injection != null)
			{
				bool haveHead = this.injection.HaveHead;
			}
			this.ApplyCharFormat();
			return true;
		}

		protected override void EndDocument()
		{
			this.RevertCharFormat();
			if (this.lineLength != 0)
			{
				this.output.Write("\r\n");
				if (!this.blockEnd)
				{
					this.output.Write("\r\n");
				}
			}
			if (this.injection != null)
			{
				bool haveTail = this.injection.HaveTail;
			}
		}

		protected override bool StartBlockContainer()
		{
			if (!this.blockEmpty)
			{
				if (this.lineLength != 0 && this.insideNofill == 0)
				{
					this.output.Write("\r\n");
				}
				this.output.Write("\r\n");
				this.lineLength = 0;
				this.blockEmpty = true;
			}
			this.blockEnd = false;
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.Margins);
			if (!effectiveProperty.IsNull && effectiveProperty.IsAbsLength)
			{
				this.spaceBefore = Math.Max(this.spaceBefore, effectiveProperty.PointsInteger);
			}
			PropertyValue effectiveProperty2 = base.GetEffectiveProperty(PropertyId.Preformatted);
			if (!effectiveProperty2.IsNull && effectiveProperty2.Bool)
			{
				this.output.Write("<Nofill>");
				this.lineLength = "<Nofill>".Length;
				this.insideNofill++;
			}
			else
			{
				StringBuilder stringBuilder = null;
				PropertyValue effectiveProperty3 = base.GetEffectiveProperty(PropertyId.QuotingLevelDelta);
				if (!effectiveProperty3.IsNull && effectiveProperty3.IsInteger && effectiveProperty3.Integer > 0)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					for (int i = 0; i < effectiveProperty3.Integer; i++)
					{
						stringBuilder.Append("<Excerpt>");
					}
				}
				PropertyValue effectiveProperty4 = base.GetEffectiveProperty(PropertyId.RightToLeft);
				bool flag = effectiveProperty4.IsNull || !effectiveProperty4.Bool;
				PropertyValue propertyValue = flag ? base.GetEffectiveProperty(PropertyId.LeftPadding) : base.GetEffectiveProperty(PropertyId.RightPadding);
				PropertyValue propertyValue2 = flag ? base.GetEffectiveProperty(PropertyId.RightPadding) : base.GetEffectiveProperty(PropertyId.LeftPadding);
				PropertyValue effectiveProperty5 = base.GetEffectiveProperty(PropertyId.FirstLineIndent);
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				if (!propertyValue.IsNull && propertyValue.IsAbsLength)
				{
					num = (propertyValue.PointsInteger + 12) / 30;
					num = EnrichedFormatOutput.CheckRange(0, num, 50);
				}
				if (!effectiveProperty5.IsNull && effectiveProperty5.IsAbsLength)
				{
					num3 = (effectiveProperty5.PointsInteger + ((effectiveProperty5.PointsInteger > 0) ? 12 : -12)) / 30;
					num3 = EnrichedFormatOutput.CheckRange(-50, num3, 50);
				}
				if (!propertyValue2.IsNull && propertyValue2.IsAbsLength)
				{
					num2 = propertyValue2.PointsInteger / 30;
					num2 = EnrichedFormatOutput.CheckRange(0, num2, 50);
				}
				if (num != 0 || num2 != 0 || num3 != 0)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					int num4 = 0;
					if (num3 < 0)
					{
						num4 = -num3;
						num3 = 0;
						num -= num4;
						if (num < 0)
						{
							num = 0;
						}
					}
					stringBuilder.Append("<ParaIndent><Param>");
					bool flag2 = false;
					while (num-- != 0)
					{
						if (flag2)
						{
							stringBuilder.Append(',');
						}
						stringBuilder.Append("Left");
						flag2 = true;
					}
					while (num2-- != 0)
					{
						if (flag2)
						{
							stringBuilder.Append(',');
						}
						stringBuilder.Append("Right");
						flag2 = true;
					}
					while (num3-- != 0)
					{
						if (flag2)
						{
							stringBuilder.Append(',');
						}
						stringBuilder.Append("In");
						flag2 = true;
					}
					while (num4-- != 0)
					{
						if (flag2)
						{
							stringBuilder.Append(',');
						}
						stringBuilder.Append("Out");
						flag2 = true;
					}
					stringBuilder.Append("</Param>");
				}
				PropertyValue effectiveProperty6 = base.GetEffectiveProperty(PropertyId.TextAlignment);
				if (!effectiveProperty6.IsNull)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					switch (effectiveProperty6.Enum)
					{
					case 1:
						stringBuilder.Append("<Center>");
						break;
					case 3:
						stringBuilder.Append("<FlushLeft>");
						break;
					case 4:
						stringBuilder.Append("<FlushRight>");
						break;
					case 6:
						stringBuilder.Append("<FlushBoth>");
						break;
					}
				}
				if (stringBuilder != null && stringBuilder.Length != 0)
				{
					this.lineLength += stringBuilder.Length;
					this.output.Write(stringBuilder.ToString());
				}
			}
			return true;
		}

		protected override void EndBlockContainer()
		{
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.Preformatted);
			if (!effectiveProperty.IsNull && effectiveProperty.Bool)
			{
				this.insideNofill--;
				this.output.Write("</Nofill>");
				this.lineLength += "</Nofill>".Length;
			}
			else
			{
				StringBuilder stringBuilder = null;
				PropertyValue effectiveProperty2 = base.GetEffectiveProperty(PropertyId.TextAlignment);
				if (!effectiveProperty2.IsNull)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					switch (effectiveProperty2.Enum)
					{
					case 1:
						stringBuilder.Append("</Center>");
						break;
					case 3:
						stringBuilder.Append("</FlushLeft>");
						break;
					case 4:
						stringBuilder.Append("</FlushRight>");
						break;
					case 6:
						stringBuilder.Append("</FlushBoth>");
						break;
					}
				}
				PropertyValue effectiveProperty3 = base.GetEffectiveProperty(PropertyId.LeftPadding);
				PropertyValue effectiveProperty4 = base.GetEffectiveProperty(PropertyId.RightPadding);
				PropertyValue effectiveProperty5 = base.GetEffectiveProperty(PropertyId.FirstLineIndent);
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				if (!effectiveProperty3.IsNull && effectiveProperty3.IsAbsLength)
				{
					num = (effectiveProperty3.PointsInteger + 12) / 30;
					num = EnrichedFormatOutput.CheckRange(0, num, 50);
				}
				if (!effectiveProperty5.IsNull && effectiveProperty5.IsAbsLength)
				{
					num3 = (effectiveProperty5.PointsInteger + ((effectiveProperty5.PointsInteger > 0) ? 12 : -12)) / 30;
					num3 = EnrichedFormatOutput.CheckRange(-50, num3, 50);
				}
				if (!effectiveProperty4.IsNull && effectiveProperty4.IsAbsLength)
				{
					num2 = effectiveProperty4.PointsInteger / 30;
					num2 = EnrichedFormatOutput.CheckRange(0, num2, 50);
				}
				if (num != 0 || num2 != 0 || num3 != 0)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					stringBuilder.Append("</ParaIndent>");
				}
				PropertyValue effectiveProperty6 = base.GetEffectiveProperty(PropertyId.QuotingLevelDelta);
				if (!effectiveProperty6.IsNull && effectiveProperty6.IsInteger && effectiveProperty6.Integer > 0)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					for (int i = 0; i < effectiveProperty6.Integer; i++)
					{
						stringBuilder.Append("</Excerpt>");
					}
				}
				if (stringBuilder != null && stringBuilder.Length != 0)
				{
					this.lineLength += stringBuilder.Length;
					this.output.Write(stringBuilder.ToString());
				}
			}
			this.blockEnd = true;
			PropertyValue effectiveProperty7 = base.GetEffectiveProperty(PropertyId.BottomMargin);
			if (!effectiveProperty7.IsNull && effectiveProperty7.IsAbsLength)
			{
				this.spaceBefore = Math.Max(this.spaceBefore, effectiveProperty7.PointsInteger);
			}
		}

		protected override bool StartTable()
		{
			if (!this.blockEmpty)
			{
				if (this.lineLength != 0 && this.insideNofill == 0)
				{
					this.output.Write("\r\n");
				}
				this.output.Write("\r\n");
				this.lineLength = 0;
				this.blockEmpty = true;
			}
			this.blockEnd = false;
			return true;
		}

		protected override void EndTable()
		{
			this.blockEnd = true;
		}

		protected override bool StartTableCaption()
		{
			if (!this.blockEmpty)
			{
				if (this.lineLength != 0 && this.insideNofill == 0)
				{
					this.output.Write("\r\n");
				}
				this.output.Write("\r\n");
				this.lineLength = 0;
				this.blockEmpty = true;
			}
			this.blockEnd = false;
			this.ApplyCharFormat();
			return true;
		}

		protected override void EndTableCaption()
		{
			this.RevertCharFormat();
			this.blockEnd = true;
		}

		protected override bool StartTableExtraContent()
		{
			if (!this.blockEmpty)
			{
				if (this.lineLength != 0 && this.insideNofill == 0)
				{
					this.output.Write("\r\n");
				}
				this.output.Write("\r\n");
				this.lineLength = 0;
				this.blockEmpty = true;
			}
			this.blockEnd = false;
			this.ApplyCharFormat();
			return true;
		}

		protected override void EndTableExtraContent()
		{
			this.RevertCharFormat();
			this.blockEnd = true;
		}

		protected override bool StartTableRow()
		{
			if (!this.blockEmpty)
			{
				if (this.lineLength != 0 && this.insideNofill == 0)
				{
					this.output.Write("\r\n");
				}
				this.output.Write("\r\n");
				this.lineLength = 0;
				this.blockEmpty = true;
			}
			this.blockEnd = false;
			return true;
		}

		protected override void EndTableRow()
		{
			this.blockEnd = true;
		}

		protected override bool StartTableCell()
		{
			this.ApplyCharFormat();
			return true;
		}

		protected override void EndTableCell()
		{
			this.RevertCharFormat();
			if (!base.CurrentNode.NextSibling.IsNull)
			{
				this.output.Write("\t");
			}
		}

		protected override bool StartList()
		{
			this.listLevel++;
			this.StartBlockContainer();
			if (this.listLevel == 1)
			{
				PropertyValue property = base.CurrentNode.Parent.GetProperty(PropertyId.ListStart);
				if (!property.IsNull)
				{
					this.listIndex = property.Integer;
				}
				else
				{
					this.listIndex = 1;
				}
			}
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.RightToLeft);
			bool flag = effectiveProperty.IsNull || !effectiveProperty.Bool;
			int num = 0;
			PropertyValue propertyValue = flag ? base.CurrentNode.Parent.GetProperty(PropertyId.LeftMargin) : base.CurrentNode.Parent.GetProperty(PropertyId.RightMargin);
			if (!propertyValue.IsNull && propertyValue.IsAbsLength)
			{
				num = propertyValue.PointsInteger / 30;
				num = EnrichedFormatOutput.CheckRange(0, num, 50);
			}
			num++;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<ParaIndent><Param>");
			bool flag2 = false;
			while (num-- != 0)
			{
				if (flag2)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append("Left");
				flag2 = true;
			}
			stringBuilder.Append("</Param>");
			this.lineLength += stringBuilder.Length;
			this.output.Write(stringBuilder.ToString());
			this.ApplyCharFormat();
			return true;
		}

		protected override void EndList()
		{
			this.RevertCharFormat();
			this.output.Write("</ParaIndent>");
			this.lineLength += "</Paraindent>".Length;
			this.EndBlockContainer();
			this.listLevel--;
		}

		protected override bool StartListItem()
		{
			this.StartBlockContainer();
			this.ApplyCharFormat();
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.ListStyle);
			if (effectiveProperty.IsNull || effectiveProperty.Enum == 1 || this.listLevel > 1)
			{
				this.output.Write("*   ");
				this.lineLength += 2;
			}
			else if (effectiveProperty.Enum != 2)
			{
				this.output.Write("*   ");
				this.lineLength += 2;
			}
			else
			{
				string text = this.listIndex.ToString();
				this.output.Write(text);
				this.output.Write(". ");
				if (text.Length == 1)
				{
					this.output.Write(' ');
				}
				this.listIndex++;
				this.lineLength += text.Length + ((text.Length == 1) ? 3 : 2);
			}
			this.blockEmpty = false;
			return true;
		}

		protected override void EndListItem()
		{
			this.RevertCharFormat();
			this.EndBlockContainer();
		}

		protected override bool StartHyperLink()
		{
			this.ApplyCharFormat();
			return true;
		}

		protected override void EndHyperLink()
		{
			this.RevertCharFormat();
		}

		protected override void StartEndImage()
		{
		}

		protected override void StartEndHorizontalLine()
		{
			if (!this.blockEmpty)
			{
				if (this.lineLength != 0 && this.insideNofill == 0)
				{
					this.output.Write("\r\n");
				}
				this.output.Write("\r\n");
			}
			this.output.Write("________________________________\r\n\r\n");
			this.lineLength = 0;
			this.blockEmpty = true;
		}

		protected override bool StartInlineContainer()
		{
			this.ApplyCharFormat();
			return true;
		}

		protected override void EndInlineContainer()
		{
			this.RevertCharFormat();
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
			if (this.blockEnd)
			{
				if (this.lineLength != 0 && this.insideNofill == 0)
				{
					this.output.Write("\r\n");
					this.lineLength = 0;
				}
				this.output.Write("\r\n");
				this.blockEnd = false;
			}
			this.blockEmpty = false;
			this.ApplyCharFormat();
			return true;
		}

		protected override bool ContinueText(uint beginTextPosition, uint endTextPosition)
		{
			if (beginTextPosition != endTextPosition)
			{
				TextRun textRun = base.FormatStore.GetTextRun(beginTextPosition);
				do
				{
					int num = textRun.EffectiveLength;
					TextRunType type = textRun.Type;
					if (type <= TextRunType.NbSp)
					{
						if (type != TextRunType.NonSpace)
						{
							if (type == TextRunType.NbSp)
							{
								this.lineLength += num;
								while (num-- != 0)
								{
									this.output.Write(' ');
								}
							}
						}
						else
						{
							this.lineLength += num;
							int num2 = 0;
							do
							{
								char[] buffer;
								int offset;
								int num3;
								textRun.GetChunk(num2, out buffer, out offset, out num3);
								this.output.Write(buffer, offset, num3, this);
								num2 += num3;
							}
							while (num2 != num);
						}
					}
					else if (type != TextRunType.Space)
					{
						if (type != TextRunType.Tabulation)
						{
							if (type == TextRunType.NewLine)
							{
								if (this.insideNofill != 0)
								{
									while (num-- != 0)
									{
										this.output.Write("\r\n");
									}
								}
								else
								{
									if (this.lineLength != 0)
									{
										this.output.Write("\r\n");
									}
									while (num-- != 0)
									{
										this.output.Write("\r\n");
									}
								}
								this.lineLength = 0;
								this.blockEmpty = true;
							}
						}
						else
						{
							while (num-- != 0)
							{
								this.output.Write('\t');
								this.lineLength = (this.lineLength + 8) / 8 * 8;
							}
						}
					}
					else
					{
						if (this.lineLength + num > 80 && this.lineLength != 0 && this.insideNofill == 0)
						{
							this.output.Write("\r\n");
							num--;
							this.lineLength = 0;
						}
						if (num != 0)
						{
							this.lineLength += num;
							while (num-- != 0)
							{
								this.output.Write(' ');
							}
						}
					}
					textRun.MoveNext();
				}
				while (textRun.Position < endTextPosition);
			}
			return true;
		}

		protected override void EndText()
		{
			this.RevertCharFormat();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.output != null && this.output != null)
			{
				((IDisposable)this.output).Dispose();
			}
			this.output = null;
			base.Dispose(disposing);
		}

		private static int CheckRange(int min, int value, int max)
		{
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		private void ApplyCharFormat()
		{
			StringBuilder stringBuilder = null;
			FlagProperties effectiveFlags = base.GetEffectiveFlags();
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.FontFace);
			PropertyValue effectiveProperty2 = base.GetEffectiveProperty(PropertyId.FontSize);
			if (!effectiveProperty.IsNull && effectiveProperty.IsString && base.FormatStore.GetStringValue(effectiveProperty).GetString().Equals("Courier New") && !effectiveProperty2.IsNull && effectiveProperty2.IsRelativeHtmlFontUnits && effectiveProperty2.RelativeHtmlFontUnits == -1)
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append("<Fixed>");
			}
			else
			{
				if (!effectiveProperty.IsNull)
				{
					string text = null;
					if (effectiveProperty.IsMultiValue)
					{
						MultiValue multiValue = base.FormatStore.GetMultiValue(effectiveProperty);
						if (multiValue.Length != 0)
						{
							text = multiValue.GetStringValue(0).GetString();
						}
					}
					else
					{
						text = base.FormatStore.GetStringValue(effectiveProperty).GetString();
					}
					if (text != null)
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder();
						}
						stringBuilder.Append("<FontFamily><Param>");
						stringBuilder.Append(text);
						stringBuilder.Append("</Param>");
					}
				}
				if (!effectiveProperty2.IsNull && !effectiveProperty2.IsAbsLength && !effectiveProperty2.IsHtmlFontUnits && effectiveProperty2.IsRelativeHtmlFontUnits && effectiveProperty2.RelativeHtmlFontUnits != 0)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					if (effectiveProperty2.RelativeHtmlFontUnits > 0)
					{
						stringBuilder.Append("<Bigger>");
						for (int i = 1; i < effectiveProperty2.RelativeHtmlFontUnits; i++)
						{
							stringBuilder.Append("<Bigger>");
						}
					}
					else
					{
						stringBuilder.Append("<Smaller>");
						for (int j = -1; j > effectiveProperty2.RelativeHtmlFontUnits; j--)
						{
							stringBuilder.Append("<Smaller>");
						}
					}
				}
			}
			PropertyValue value = base.GetEffectiveProperty(PropertyId.FontColor);
			if (value.IsEnum)
			{
				value = HtmlSupport.TranslateSystemColor(value);
			}
			if (value.IsColor && value.Color.RGB != 0U)
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				uint num = value.Color.Red << 8;
				uint num2 = value.Color.Green << 8;
				uint num3 = value.Color.Blue << 8;
				if ((num & 256U) != 0U)
				{
					num += 255U;
				}
				if ((num2 & 256U) != 0U)
				{
					num2 += 255U;
				}
				if ((num3 & 256U) != 0U)
				{
					num3 += 255U;
				}
				stringBuilder.Append("<Color><Param>");
				stringBuilder.Append(string.Format("{0:X4},{1:X4},{2:X4}", num, num2, num3));
				stringBuilder.Append("</Param>");
			}
			if (effectiveFlags.IsDefinedAndOn(PropertyId.FirstFlag))
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append("<Bold>");
			}
			if (effectiveFlags.IsDefinedAndOn(PropertyId.Italic))
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append("<Italic>");
			}
			if (effectiveFlags.IsDefinedAndOn(PropertyId.Underline))
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append("<Underline>");
			}
			if (stringBuilder != null && stringBuilder.Length != 0)
			{
				this.lineLength += stringBuilder.Length;
				this.output.Write(stringBuilder.ToString());
			}
		}

		private void RevertCharFormat()
		{
			StringBuilder stringBuilder = null;
			FlagProperties effectiveFlags = base.GetEffectiveFlags();
			if (effectiveFlags.IsDefinedAndOn(PropertyId.Underline))
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append("</Underline>");
			}
			if (effectiveFlags.IsDefinedAndOn(PropertyId.Italic))
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append("</Italic>");
			}
			if (effectiveFlags.IsDefinedAndOn(PropertyId.FirstFlag))
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append("</Bold>");
			}
			PropertyValue value = base.GetEffectiveProperty(PropertyId.FontColor);
			if (value.IsEnum)
			{
				value = HtmlSupport.TranslateSystemColor(value);
			}
			if (value.IsColor && value.Color.RGB != 0U)
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append("</Color>");
			}
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.FontFace);
			PropertyValue effectiveProperty2 = base.GetEffectiveProperty(PropertyId.FontSize);
			if (!effectiveProperty.IsNull && effectiveProperty.IsString && base.FormatStore.GetStringValue(effectiveProperty).GetString().Equals("Courier New") && !effectiveProperty2.IsNull && effectiveProperty2.IsRelativeHtmlFontUnits && effectiveProperty2.RelativeHtmlFontUnits == -1)
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append("</Fixed>");
			}
			else
			{
				if (!effectiveProperty2.IsNull && !effectiveProperty2.IsAbsLength && !effectiveProperty2.IsHtmlFontUnits && effectiveProperty2.IsRelativeHtmlFontUnits && effectiveProperty2.RelativeHtmlFontUnits != 0)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					if (effectiveProperty2.RelativeHtmlFontUnits > 0)
					{
						stringBuilder.Append("</Bigger>");
						for (int i = 1; i < effectiveProperty2.RelativeHtmlFontUnits; i++)
						{
							stringBuilder.Append("</Bigger>");
						}
					}
					else
					{
						stringBuilder.Append("</Smaller>");
						for (int j = -1; j > effectiveProperty2.RelativeHtmlFontUnits; j--)
						{
							stringBuilder.Append("</Smaller>");
						}
					}
				}
				if (!effectiveProperty.IsNull)
				{
					string text = null;
					if (effectiveProperty.IsMultiValue)
					{
						MultiValue multiValue = base.FormatStore.GetMultiValue(effectiveProperty);
						if (multiValue.Length != 0)
						{
							text = multiValue.GetStringValue(0).GetString();
						}
					}
					else
					{
						text = base.FormatStore.GetStringValue(effectiveProperty).GetString();
					}
					if (text != null)
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder();
						}
						stringBuilder.Append("</FontFamily>");
					}
				}
			}
			if (stringBuilder != null && stringBuilder.Length != 0)
			{
				this.lineLength += stringBuilder.Length;
				this.output.Write(stringBuilder.ToString());
			}
		}

		private string GetSubstitute(char ch)
		{
			return AsciiEncoderFallback.GetCharacterFallback(ch);
		}

		private ConverterOutput output;

		private Injection injection;

		private bool fallbacks;

		private bool blockEmpty = true;

		private bool blockEnd;

		private int lineLength;

		private int insideNofill;

		private int listLevel;

		private int listIndex;

		private int spaceBefore;
	}
}
