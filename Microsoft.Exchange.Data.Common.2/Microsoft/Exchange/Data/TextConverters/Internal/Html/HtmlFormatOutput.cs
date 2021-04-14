using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal class HtmlFormatOutput : FormatOutput, IRestartable
	{
		public HtmlFormatOutput(HtmlWriter writer, HtmlInjection injection, bool outputFragment, Stream formatTraceStream, Stream formatOutputTraceStream, bool filterHtml, HtmlTagCallback callback, bool recognizeHyperlinks) : base(formatOutputTraceStream)
		{
			this.writer = writer;
			this.injection = injection;
			this.outputFragment = outputFragment;
			this.filterHtml = filterHtml;
			this.callback = callback;
			this.recognizeHyperlinks = recognizeHyperlinks;
		}

		private static bool IsHyperLinkStartDelimiter(char c)
		{
			return c == '<' || c == '"' || c == '\'' || c == '(' || c == '[';
		}

		private static bool IsHyperLinkEndDelimiter(char c)
		{
			return c == '>' || c == '"' || c == '\'' || c == ')' || c == ']';
		}

		private void WriteIdAttribute(bool saveToCallbackContext)
		{
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.Id);
			if (!distinctProperty.IsNull)
			{
				string @string = base.FormatStore.GetStringValue(distinctProperty).GetString();
				if (saveToCallbackContext)
				{
					this.callbackContext.AddAttribute(HtmlNameIndex.Id, @string);
					return;
				}
				this.writer.WriteAttributeName(HtmlNameIndex.Id);
				this.writer.WriteAttributeValue(@string);
			}
		}

		internal HtmlWriter Writer
		{
			get
			{
				return this.writer;
			}
			set
			{
				this.writer = value;
			}
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
				return this.writer.CanAcceptMore;
			}
		}

		bool IRestartable.CanRestart()
		{
			return this.writer != null && ((IRestartable)this.writer).CanRestart();
		}

		void IRestartable.Restart()
		{
			((IRestartable)this.writer).Restart();
			base.Restart();
			if (this.injection != null)
			{
				this.injection.Reset();
			}
			this.hyperlinkLevel = 0;
		}

		void IRestartable.DisableRestart()
		{
			if (this.writer != null)
			{
				((IRestartable)this.writer).DisableRestart();
			}
		}

		public override bool Flush()
		{
			if (!base.Flush())
			{
				return false;
			}
			this.writer.Flush();
			return true;
		}

		internal void SetWriter(HtmlWriter writer)
		{
			this.writer = writer;
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
			if (!this.outputFragment)
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				this.writer.WriteStartTag(HtmlNameIndex.Html);
				if (this.callback != null)
				{
					if (this.callbackContext == null)
					{
						this.callbackContext = new HtmlFormatOutputCallbackContext(this);
					}
					this.callbackContext.InitializeTag(false, HtmlNameIndex.Head, false);
				}
				else
				{
					this.writer.WriteStartTag(HtmlNameIndex.Head);
				}
				if (this.callback != null)
				{
					this.callbackContext.InitializeFragment(false);
					this.callback(this.callbackContext, this.writer);
					this.callbackContext.UninitializeFragment();
					if (this.callbackContext.IsInvokeCallbackForEndTag)
					{
						flag = true;
					}
					if (this.callbackContext.IsDeleteInnerContent)
					{
						flag2 = true;
					}
					if (this.callbackContext.IsDeleteEndTag)
					{
						flag3 = true;
					}
				}
				if (!flag2)
				{
					if (this.writer.HasEncoding)
					{
						this.writer.WriteStartTag(HtmlNameIndex.Meta);
						this.writer.WriteAttribute(HtmlNameIndex.HttpEquiv, "Content-Type");
						this.writer.WriteAttributeName(HtmlNameIndex.Content);
						this.writer.WriteAttributeValueInternal("text/html; charset=");
						this.writer.WriteAttributeValue(Charset.GetCharset(this.writer.Encoding).Name);
						this.writer.WriteNewLine(true);
					}
					this.writer.WriteStartTag(HtmlNameIndex.Meta);
					this.writer.WriteAttribute(HtmlNameIndex.Name, "Generator");
					this.writer.WriteAttribute(HtmlNameIndex.Content, "Microsoft Exchange Server");
					this.writer.WriteNewLine(true);
					if (base.Comment != null)
					{
						this.writer.WriteMarkupText("<!-- " + base.Comment + " -->");
						this.writer.WriteNewLine(true);
					}
					this.writer.WriteStartTag(HtmlNameIndex.Style);
					this.writer.WriteMarkupText("<!-- .EmailQuote { margin-left: 1pt; padding-left: 4pt; border-left: #800000 2px solid; } -->");
					this.writer.WriteEndTag(HtmlNameIndex.Style);
				}
				if (flag)
				{
					this.callbackContext.InitializeTag(true, HtmlNameIndex.Head, flag3);
					this.callbackContext.InitializeFragment(false);
					this.callback(this.callbackContext, this.writer);
					this.callbackContext.UninitializeFragment();
				}
				else if (!flag3)
				{
					this.writer.WriteEndTag(HtmlNameIndex.Head);
					this.writer.WriteNewLine(true);
				}
				this.writer.WriteStartTag(HtmlNameIndex.Body);
				this.writer.WriteNewLine(true);
			}
			else
			{
				this.writer.WriteStartTag(HtmlNameIndex.Div);
				this.writer.WriteAttribute(HtmlNameIndex.Class, "BodyFragment");
				this.writer.WriteNewLine(true);
			}
			if (this.injection != null && this.injection.HaveHead)
			{
				this.injection.Inject(true, this.writer);
			}
			this.ApplyCharFormat();
			return true;
		}

		protected override void EndDocument()
		{
			this.RevertCharFormat();
			if (this.injection != null && this.injection.HaveTail)
			{
				this.injection.Inject(false, this.writer);
			}
			if (!this.outputFragment)
			{
				this.writer.WriteNewLine(true);
				this.writer.WriteEndTag(HtmlNameIndex.Body);
				this.writer.WriteNewLine(true);
				this.writer.WriteEndTag(HtmlNameIndex.Html);
			}
			else
			{
				this.writer.WriteNewLine(true);
				this.writer.WriteEndTag(HtmlNameIndex.Div);
			}
			this.writer.WriteNewLine(true);
		}

		protected override void StartEndBaseFont()
		{
		}

		protected override bool StartTable()
		{
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.FontFace);
			if (!distinctProperty.IsNull)
			{
				this.writer.WriteStartTag(HtmlNameIndex.Font);
				this.writer.WriteAttributeName(HtmlNameIndex.Face);
				if (distinctProperty.IsMultiValue)
				{
					MultiValue multiValue = base.FormatStore.GetMultiValue(distinctProperty);
					for (int i = 0; i < multiValue.Length; i++)
					{
						string @string = multiValue.GetStringValue(i).GetString();
						if (i != 0)
						{
							this.writer.WriteAttributeValue(",");
						}
						this.writer.WriteAttributeValue(@string);
					}
				}
				else
				{
					string @string = base.FormatStore.GetStringValue(distinctProperty).GetString();
					this.writer.WriteAttributeValue(@string);
				}
			}
			this.writer.WriteNewLine(true);
			this.writer.WriteStartTag(HtmlNameIndex.Table);
			this.OutputTableTagAttributes();
			bool flag = false;
			this.OutputTableCssProperties(ref flag);
			this.OutputBlockCssProperties(ref flag);
			this.writer.WriteNewLine(true);
			return true;
		}

		protected override void EndTable()
		{
			this.writer.WriteNewLine(true);
			this.writer.WriteEndTag(HtmlNameIndex.Table);
			this.writer.WriteNewLine(true);
			if (!base.GetDistinctProperty(PropertyId.FontFace).IsNull)
			{
				this.writer.WriteEndTag(HtmlNameIndex.Font);
			}
		}

		protected override bool StartTableColumnGroup()
		{
			this.writer.WriteNewLine(true);
			this.writer.WriteStartTag(HtmlNameIndex.ColGroup);
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.Width);
			if (!distinctProperty.IsNull && distinctProperty.IsAbsRelLength)
			{
				this.writer.WriteAttribute(HtmlNameIndex.Width, distinctProperty.PixelsInteger.ToString());
			}
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.NumColumns);
			if (!distinctProperty2.IsNull && distinctProperty2.IsAbsRelLength)
			{
				this.writer.WriteAttribute(HtmlNameIndex.Span, distinctProperty2.Integer.ToString());
			}
			bool flag = false;
			this.OutputTableColumnCssProperties(ref flag);
			return true;
		}

		protected override void EndTableColumnGroup()
		{
			this.writer.WriteEndTag(HtmlNameIndex.ColGroup);
			this.writer.WriteNewLine(true);
		}

		protected override void StartEndTableColumn()
		{
			this.writer.WriteStartTag(HtmlNameIndex.Col);
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.Width);
			if (!distinctProperty.IsNull && distinctProperty.IsAbsRelLength)
			{
				this.writer.WriteAttribute(HtmlNameIndex.Width, distinctProperty.PixelsInteger.ToString());
			}
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.NumColumns);
			if (!distinctProperty2.IsNull && distinctProperty2.IsAbsRelLength)
			{
				this.writer.WriteAttribute(HtmlNameIndex.Span, distinctProperty2.Integer.ToString());
			}
			bool flag = false;
			this.OutputTableColumnCssProperties(ref flag);
			this.writer.WriteNewLine(true);
		}

		protected override bool StartTableCaption()
		{
			this.writer.WriteNewLine(true);
			if (!base.CurrentNode.Parent.IsNull && base.CurrentNode.Parent.NodeType == FormatContainerType.Table)
			{
				this.writer.WriteStartTag(HtmlNameIndex.Caption);
				FormatStyle style = base.FormatStore.GetStyle(13);
				base.SubtractDefaultContainerPropertiesFromDistinct(style.FlagProperties, style.PropertyList);
				PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.BlockAlignment);
				if (!distinctProperty.IsNull)
				{
					string blockAlignmentString = HtmlSupport.GetBlockAlignmentString(distinctProperty);
					if (blockAlignmentString != null)
					{
						this.writer.WriteAttribute(HtmlNameIndex.Align, blockAlignmentString);
					}
				}
				this.writer.WriteNewLine(true);
			}
			this.ApplyCharFormat();
			return true;
		}

		protected override void EndTableCaption()
		{
			this.RevertCharFormat();
			if (!base.CurrentNode.Parent.IsNull && base.CurrentNode.Parent.NodeType == FormatContainerType.Table)
			{
				this.writer.WriteNewLine(true);
				this.writer.WriteEndTag(HtmlNameIndex.Caption);
			}
			this.writer.WriteNewLine(true);
		}

		protected override bool StartTableExtraContent()
		{
			return this.StartBlockContainer();
		}

		protected override void EndTableExtraContent()
		{
			this.EndBlockContainer();
		}

		protected override bool StartTableRow()
		{
			this.writer.WriteNewLine(true);
			this.writer.WriteStartTag(HtmlNameIndex.TR);
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.Height);
			if (!distinctProperty.IsNull && distinctProperty.IsAbsRelLength)
			{
				this.writer.WriteAttribute(HtmlNameIndex.Height, distinctProperty.PixelsInteger.ToString());
			}
			bool flag = false;
			this.OutputBlockCssProperties(ref flag);
			this.writer.WriteNewLine(true);
			return true;
		}

		protected override void EndTableRow()
		{
			this.writer.WriteNewLine(true);
			this.writer.WriteEndTag(HtmlNameIndex.TR);
			this.writer.WriteNewLine(true);
		}

		protected override bool StartTableCell()
		{
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.MergedCell);
			if (distinctProperty.IsNull || !distinctProperty.Bool)
			{
				this.writer.WriteNewLine(true);
				this.writer.WriteStartTag(HtmlNameIndex.TD);
				this.OutputTableCellTagAttributes();
				bool flag = false;
				this.OutputBlockCssProperties(ref flag);
				this.ApplyCharFormat();
			}
			return true;
		}

		protected override void EndTableCell()
		{
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.MergedCell);
			if (distinctProperty.IsNull || !distinctProperty.Bool)
			{
				this.RevertCharFormat();
				this.writer.WriteEndTag(HtmlNameIndex.TD);
				this.writer.WriteNewLine(true);
			}
		}

		protected override bool StartList()
		{
			this.writer.WriteNewLine(true);
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.ListStyle);
			bool flag = true;
			if (effectiveProperty.IsNull || effectiveProperty.Enum == 1)
			{
				this.writer.WriteStartTag(HtmlNameIndex.UL);
			}
			else
			{
				this.writer.WriteStartTag(HtmlNameIndex.OL);
				flag = false;
			}
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.RightToLeft);
			if (!distinctProperty.IsNull)
			{
				this.writer.WriteAttribute(HtmlNameIndex.Dir, distinctProperty.Bool ? "rtl" : "ltr");
			}
			if (!flag && effectiveProperty.Enum != 2)
			{
				this.writer.WriteAttribute(HtmlNameIndex.Type, HtmlFormatOutput.listType[effectiveProperty.Enum]);
			}
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.ListStart);
			if (!flag && distinctProperty2.IsInteger && distinctProperty2.Integer != 1)
			{
				this.writer.WriteAttribute(HtmlNameIndex.Start, distinctProperty2.Integer.ToString());
			}
			bool flag2 = false;
			this.OutputBlockCssProperties(ref flag2);
			this.writer.WriteNewLine(true);
			this.ApplyCharFormat();
			return true;
		}

		protected override void EndList()
		{
			this.RevertCharFormat();
			PropertyValue effectiveProperty = base.GetEffectiveProperty(PropertyId.ListStyle);
			this.writer.WriteNewLine(true);
			if (effectiveProperty.IsNull || effectiveProperty.Enum == 1)
			{
				this.writer.WriteEndTag(HtmlNameIndex.UL);
			}
			else
			{
				this.writer.WriteEndTag(HtmlNameIndex.OL);
			}
			this.writer.WriteNewLine(true);
		}

		protected override bool StartListItem()
		{
			this.writer.WriteNewLine(true);
			this.writer.WriteStartTag(HtmlNameIndex.LI);
			bool flag = false;
			this.OutputBlockCssProperties(ref flag);
			this.ApplyCharFormat();
			return true;
		}

		protected override void EndListItem()
		{
			this.RevertCharFormat();
			this.writer.WriteEndTag(HtmlNameIndex.LI);
			this.writer.WriteNewLine(true);
		}

		protected override bool StartHyperLink()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			FlagProperties flags = default(FlagProperties);
			flags.Set(PropertyId.Underline, true);
			base.SubtractDefaultContainerPropertiesFromDistinct(flags, HtmlFormatOutput.defaultHyperlinkProperties);
			if (this.callback != null)
			{
				if (this.callbackContext == null)
				{
					this.callbackContext = new HtmlFormatOutputCallbackContext(this);
				}
				this.callbackContext.InitializeTag(false, HtmlNameIndex.A, false);
			}
			else
			{
				this.writer.WriteStartTag(HtmlNameIndex.A);
			}
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.HyperlinkUrl);
			if (!distinctProperty.IsNull)
			{
				string text = base.FormatStore.GetStringValue(distinctProperty).GetString();
				if (this.filterHtml && !HtmlToHtmlConverter.IsUrlSafe(text, this.callback != null))
				{
					text = string.Empty;
				}
				if (this.callback != null)
				{
					this.callbackContext.AddAttribute(HtmlNameIndex.Href, text);
				}
				else
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Href);
					this.writer.WriteAttributeValue(text);
				}
				distinctProperty = base.GetDistinctProperty(PropertyId.HyperlinkTarget);
				if (!distinctProperty.IsNull)
				{
					string targetString = HtmlSupport.GetTargetString(distinctProperty);
					if (this.callback != null)
					{
						this.callbackContext.AddAttribute(HtmlNameIndex.Target, targetString);
					}
					else
					{
						this.writer.WriteAttributeName(HtmlNameIndex.Target);
						this.writer.WriteAttributeValue(targetString);
					}
				}
				this.WriteIdAttribute(this.callback != null);
			}
			if (this.callback != null)
			{
				this.callbackContext.InitializeFragment(false);
				this.callback(this.callbackContext, this.writer);
				this.callbackContext.UninitializeFragment();
				if (this.callbackContext.IsInvokeCallbackForEndTag)
				{
					flag3 = true;
				}
				if (this.callbackContext.IsDeleteInnerContent)
				{
					flag2 = true;
				}
				if (this.callbackContext.IsDeleteEndTag)
				{
					flag = true;
				}
				if (flag || flag3)
				{
					if (this.endTagActionStack == null)
					{
						this.endTagActionStack = new HtmlFormatOutput.EndTagActionEntry[4];
					}
					else if (this.endTagActionStack.Length == this.endTagActionStackTop)
					{
						HtmlFormatOutput.EndTagActionEntry[] destinationArray = new HtmlFormatOutput.EndTagActionEntry[this.endTagActionStack.Length * 2];
						Array.Copy(this.endTagActionStack, 0, destinationArray, 0, this.endTagActionStackTop);
						this.endTagActionStack = destinationArray;
					}
					this.endTagActionStack[this.endTagActionStackTop].TagLevel = this.hyperlinkLevel;
					this.endTagActionStack[this.endTagActionStackTop].Drop = flag;
					this.endTagActionStack[this.endTagActionStackTop].Callback = flag3;
					this.endTagActionStackTop++;
				}
			}
			this.hyperlinkLevel++;
			if (!flag2)
			{
				this.ApplyCharFormat();
			}
			else
			{
				this.CloseHyperLink();
			}
			if (this.writer.IsTagOpen)
			{
				this.writer.WriteTagEnd();
			}
			return !flag2;
		}

		protected override void EndHyperLink()
		{
			this.hyperlinkLevel--;
			this.RevertCharFormat();
			this.CloseHyperLink();
			if (this.writer.IsTagOpen)
			{
				this.writer.WriteTagEnd();
			}
		}

		protected override bool StartBookmark()
		{
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.BookmarkName);
			if (!distinctProperty.IsNull)
			{
				this.writer.WriteStartTag(HtmlNameIndex.A);
				string @string = base.FormatStore.GetStringValue(distinctProperty).GetString();
				this.writer.WriteAttributeName(HtmlNameIndex.Name);
				this.writer.WriteAttributeValue(@string);
			}
			this.ApplyCharFormat();
			if (this.writer.IsTagOpen)
			{
				this.writer.WriteTagEnd();
			}
			return true;
		}

		protected override void EndBookmark()
		{
			this.RevertCharFormat();
			if (!base.GetDistinctProperty(PropertyId.BookmarkName).IsNull)
			{
				this.writer.WriteEndTag(HtmlNameIndex.A);
			}
			if (this.writer.IsTagOpen)
			{
				this.writer.WriteTagEnd();
			}
		}

		protected override void StartEndImage()
		{
			if (this.callback != null)
			{
				if (this.callbackContext == null)
				{
					this.callbackContext = new HtmlFormatOutputCallbackContext(this);
				}
				this.callbackContext.InitializeTag(false, HtmlNameIndex.Img, false);
			}
			else
			{
				this.writer.WriteStartTag(HtmlNameIndex.Img);
			}
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.Width);
			if (!distinctProperty.IsNull)
			{
				BufferString value = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty);
				if (value.Length != 0)
				{
					if (this.callback != null)
					{
						this.callbackContext.AddAttribute(HtmlNameIndex.Width, value.ToString());
					}
					else
					{
						this.writer.WriteAttribute(HtmlNameIndex.Width, value);
					}
				}
			}
			distinctProperty = base.GetDistinctProperty(PropertyId.Height);
			if (!distinctProperty.IsNull)
			{
				BufferString value2 = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty);
				if (value2.Length != 0)
				{
					if (this.callback != null)
					{
						this.callbackContext.AddAttribute(HtmlNameIndex.Height, value2.ToString());
					}
					else
					{
						this.writer.WriteAttribute(HtmlNameIndex.Height, value2);
					}
				}
			}
			distinctProperty = base.GetDistinctProperty(PropertyId.BlockAlignment);
			if (!distinctProperty.IsNull)
			{
				string blockAlignmentString = HtmlSupport.GetBlockAlignmentString(distinctProperty);
				if (blockAlignmentString != null)
				{
					if (this.callback != null)
					{
						this.callbackContext.AddAttribute(HtmlNameIndex.Align, blockAlignmentString);
					}
					else
					{
						this.writer.WriteAttribute(HtmlNameIndex.Align, blockAlignmentString);
					}
				}
			}
			distinctProperty = base.GetDistinctProperty(PropertyId.TableBorder);
			if (!distinctProperty.IsNull)
			{
				BufferString value3 = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty);
				if (value3.Length != 0)
				{
					if (this.callback != null)
					{
						this.callbackContext.AddAttribute(HtmlNameIndex.Border, value3.ToString());
					}
					else
					{
						this.writer.WriteAttribute(HtmlNameIndex.Border, value3);
					}
				}
			}
			distinctProperty = base.GetDistinctProperty(PropertyId.ImageUrl);
			if (!distinctProperty.IsNull)
			{
				string text = base.FormatStore.GetStringValue(distinctProperty).GetString();
				if (this.filterHtml && !HtmlToHtmlConverter.IsUrlSafe(text, this.callback != null))
				{
					text = string.Empty;
				}
				if (this.callback != null)
				{
					this.callbackContext.AddAttribute(HtmlNameIndex.Src, text);
				}
				else
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Src);
					this.writer.WriteAttributeValue(text);
				}
			}
			distinctProperty = base.GetDistinctProperty(PropertyId.BookmarkName);
			if (!distinctProperty.IsNull)
			{
				string text2 = base.FormatStore.GetStringValue(distinctProperty).GetString();
				if (this.filterHtml && !HtmlToHtmlConverter.IsUrlSafe(text2, this.callback != null))
				{
					text2 = string.Empty;
				}
				if (this.callback != null)
				{
					this.callbackContext.AddAttribute(HtmlNameIndex.UseMap, text2);
				}
				else
				{
					this.writer.WriteAttributeName(HtmlNameIndex.UseMap);
					this.writer.WriteAttributeValue(text2);
				}
			}
			distinctProperty = base.GetDistinctProperty(PropertyId.RightToLeft);
			if (distinctProperty.IsBool)
			{
				if (this.callback != null)
				{
					this.callbackContext.AddAttribute(HtmlNameIndex.Dir, distinctProperty.Bool ? "rtl" : "ltr");
				}
				else
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Dir);
					this.writer.WriteAttributeValue(distinctProperty.Bool ? "rtl" : "ltr");
				}
			}
			distinctProperty = base.GetDistinctProperty(PropertyId.Language);
			Culture culture;
			if (distinctProperty.IsInteger && (Culture.TryGetCulture(distinctProperty.Integer, out culture) || string.IsNullOrEmpty(culture.Name)))
			{
				if (this.callback != null)
				{
					this.callbackContext.AddAttribute(HtmlNameIndex.Lang, culture.Name);
				}
				else
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Lang);
					this.writer.WriteAttributeValue(culture.Name);
				}
			}
			distinctProperty = base.GetDistinctProperty(PropertyId.ImageAltText);
			if (!distinctProperty.IsNull)
			{
				string @string = base.FormatStore.GetStringValue(distinctProperty).GetString();
				if (this.callback != null)
				{
					this.callbackContext.AddAttribute(HtmlNameIndex.Alt, @string);
				}
				else
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Alt);
					this.writer.WriteAttributeValue(@string);
				}
			}
			if (this.callback != null)
			{
				this.callbackContext.InitializeFragment(true);
				this.callback(this.callbackContext, this.writer);
				this.callbackContext.UninitializeFragment();
			}
			if (this.writer.IsTagOpen)
			{
				this.writer.WriteTagEnd();
			}
		}

		protected override void StartEndHorizontalLine()
		{
			this.writer.WriteNewLine(true);
			this.writer.WriteStartTag(HtmlNameIndex.HR);
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.Width);
			if (!distinctProperty.IsNull)
			{
				BufferString value = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty);
				if (value.Length != 0)
				{
					this.writer.WriteAttribute(HtmlNameIndex.Width, value);
				}
			}
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.Height);
			if (!distinctProperty2.IsNull && distinctProperty2.IsAbsRelLength)
			{
				this.writer.WriteAttribute(HtmlNameIndex.Size, distinctProperty2.PixelsInteger.ToString());
			}
			PropertyValue distinctProperty3 = base.GetDistinctProperty(PropertyId.BlockAlignment);
			if (!distinctProperty3.IsNull)
			{
				string horizontalAlignmentString = HtmlSupport.GetHorizontalAlignmentString(distinctProperty3);
				if (horizontalAlignmentString != null)
				{
					this.writer.WriteAttribute(HtmlNameIndex.Align, horizontalAlignmentString);
				}
			}
			PropertyValue distinctProperty4 = base.GetDistinctProperty(PropertyId.FontColor);
			if (!distinctProperty4.IsNull)
			{
				BufferString value2 = HtmlSupport.FormatColor(ref this.scratchBuffer, distinctProperty4);
				if (value2.Length != 0)
				{
					this.writer.WriteAttribute(HtmlNameIndex.Color, value2);
				}
			}
			if (!distinctProperty.IsNull)
			{
				this.writer.WriteAttributeName(HtmlNameIndex.Style);
				if (!distinctProperty.IsNull)
				{
					BufferString value3 = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty);
					if (value3.Length != 0)
					{
						this.writer.WriteAttributeValue("width:");
						this.writer.WriteAttributeValue(value3);
						this.writer.WriteAttributeValue(";");
					}
				}
			}
			if (this.writer.LiteralWhitespaceNesting == 0)
			{
				this.writer.WriteNewLine(true);
			}
		}

		protected override bool StartInline()
		{
			this.ApplyCharFormat();
			return true;
		}

		protected override void EndInline()
		{
			this.RevertCharFormat();
		}

		protected override bool StartMap()
		{
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.BookmarkName);
			if (!distinctProperty.IsNull)
			{
				this.writer.WriteStartTag(HtmlNameIndex.Map);
				string text = base.FormatStore.GetStringValue(distinctProperty).GetString();
				if (this.filterHtml && !HtmlToHtmlConverter.IsUrlSafe(text, this.callback != null))
				{
					text = string.Empty;
				}
				this.writer.WriteAttributeName(HtmlNameIndex.Name);
				this.writer.WriteAttributeValue(text);
				this.writer.WriteNewLine(true);
			}
			return true;
		}

		protected override void EndMap()
		{
			if (!base.GetDistinctProperty(PropertyId.BookmarkName).IsNull)
			{
				this.writer.WriteEndTag(HtmlNameIndex.Map);
				this.writer.WriteNewLine(true);
			}
		}

		protected override void StartEndArea()
		{
			if (this.callback != null)
			{
				if (this.callbackContext == null)
				{
					this.callbackContext = new HtmlFormatOutputCallbackContext(this);
				}
				this.callbackContext.InitializeTag(false, HtmlNameIndex.Area, false);
			}
			else
			{
				this.writer.WriteStartTag(HtmlNameIndex.Area);
			}
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.HyperlinkUrl);
			if (!distinctProperty.IsNull)
			{
				string text = base.FormatStore.GetStringValue(distinctProperty).GetString();
				if (this.filterHtml && !HtmlToHtmlConverter.IsUrlSafe(text, this.callback != null))
				{
					text = string.Empty;
				}
				if (this.callback != null)
				{
					this.callbackContext.AddAttribute(HtmlNameIndex.Href, text);
				}
				else
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Href);
					this.writer.WriteAttributeValue(text);
				}
			}
			distinctProperty = base.GetDistinctProperty(PropertyId.HyperlinkTarget);
			if (!distinctProperty.IsNull)
			{
				string targetString = HtmlSupport.GetTargetString(distinctProperty);
				if (this.callback != null)
				{
					this.callbackContext.AddAttribute(HtmlNameIndex.Target, targetString);
				}
				else
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Target);
					this.writer.WriteAttributeValue(targetString);
				}
			}
			distinctProperty = base.GetDistinctProperty(PropertyId.Shape);
			if (!distinctProperty.IsNull)
			{
				string areaShapeString = HtmlSupport.GetAreaShapeString(distinctProperty);
				if (this.callback != null)
				{
					this.callbackContext.AddAttribute(HtmlNameIndex.Shape, areaShapeString);
				}
				else
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Shape);
					this.writer.WriteAttributeValue(areaShapeString);
				}
			}
			distinctProperty = base.GetDistinctProperty(PropertyId.Coords);
			if (!distinctProperty.IsNull)
			{
				string @string = base.FormatStore.GetStringValue(distinctProperty).GetString();
				if (this.callback != null)
				{
					this.callbackContext.AddAttribute(HtmlNameIndex.Coords, @string);
				}
				else
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Coords);
					this.writer.WriteAttributeValue(@string);
				}
			}
			distinctProperty = base.GetDistinctProperty(PropertyId.ImageAltText);
			if (!distinctProperty.IsNull)
			{
				string string2 = base.FormatStore.GetStringValue(distinctProperty).GetString();
				if (this.callback != null)
				{
					this.callbackContext.AddAttribute(HtmlNameIndex.Alt, string2);
				}
				else
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Alt);
					this.writer.WriteAttributeValue(string2);
				}
			}
			if (this.callback != null)
			{
				this.callbackContext.InitializeFragment(true);
				this.callback(this.callbackContext, this.writer);
				this.callbackContext.UninitializeFragment();
			}
			if (this.writer.IsTagOpen)
			{
				this.writer.WriteTagEnd();
				this.writer.WriteNewLine(true);
			}
		}

		protected override bool StartForm()
		{
			return this.StartInlineContainer();
		}

		protected override void EndForm()
		{
			this.EndInlineContainer();
		}

		protected override bool StartFieldSet()
		{
			return this.StartBlockContainer();
		}

		protected override void EndFieldSet()
		{
			this.EndBlockContainer();
		}

		protected override bool StartSelect()
		{
			return true;
		}

		protected override void EndSelect()
		{
		}

		protected override bool StartOptionGroup()
		{
			return true;
		}

		protected override void EndOptionGroup()
		{
		}

		protected override bool StartOption()
		{
			return true;
		}

		protected override void EndOption()
		{
		}

		protected override bool StartText()
		{
			this.ApplyCharFormat();
			this.writer.StartTextChunk();
			return true;
		}

		protected override bool ContinueText(uint beginTextPosition, uint endTextPosition)
		{
			if (beginTextPosition != endTextPosition)
			{
				TextRun textRun = base.FormatStore.GetTextRun(beginTextPosition);
				for (;;)
				{
					int effectiveLength = textRun.EffectiveLength;
					TextRunType type = textRun.Type;
					if (type <= TextRunType.NbSp)
					{
						if (type != TextRunType.NonSpace)
						{
							if (type != TextRunType.NbSp)
							{
								goto IL_377;
							}
							this.writer.WriteNbsp(effectiveLength);
							goto IL_377;
						}
						else
						{
							int num = 0;
							if (this.recognizeHyperlinks && this.hyperlinkLevel == 0 && effectiveLength > 10 && effectiveLength < 4096)
							{
								int num2;
								int num3;
								bool flag2;
								bool flag3;
								bool flag = this.RecognizeHyperLink(textRun, out num2, out num3, out flag2, out flag3);
								if (flag)
								{
									if (num2 != 0)
									{
										this.writer.WriteTextInternal(this.scratchBuffer.Buffer, 0, num2);
									}
									if (this.callback != null)
									{
										if (this.callbackContext == null)
										{
											this.callbackContext = new HtmlFormatOutputCallbackContext(this);
										}
										this.callbackContext.InitializeTag(false, HtmlNameIndex.A, false);
										string text = new string(this.scratchBuffer.Buffer, num2, num3);
										if (flag3)
										{
											text = "http://" + text;
										}
										else if (flag2)
										{
											text = "file://" + text;
										}
										this.callbackContext.AddAttribute(HtmlNameIndex.Href, text);
										this.callbackContext.InitializeFragment(false);
										this.callback(this.callbackContext, this.writer);
										this.callbackContext.UninitializeFragment();
										if (this.writer.IsTagOpen)
										{
											this.writer.WriteTagEnd();
										}
										if (!this.callbackContext.IsDeleteInnerContent)
										{
											this.writer.WriteTextInternal(this.scratchBuffer.Buffer, num2, num3);
										}
										if (this.callbackContext.IsInvokeCallbackForEndTag)
										{
											this.callbackContext.InitializeTag(true, HtmlNameIndex.A, this.callbackContext.IsDeleteEndTag);
											this.callbackContext.InitializeFragment(false);
											this.callback(this.callbackContext, this.writer);
											this.callbackContext.UninitializeFragment();
										}
										else if (!this.callbackContext.IsDeleteEndTag)
										{
											this.writer.WriteEndTag(HtmlNameIndex.A);
										}
										if (this.writer.IsTagOpen)
										{
											this.writer.WriteTagEnd();
										}
									}
									else
									{
										this.writer.WriteStartTag(HtmlNameIndex.A);
										this.writer.WriteAttributeName(HtmlNameIndex.Href);
										if (flag3)
										{
											this.writer.WriteAttributeValue("http://");
										}
										else if (flag2)
										{
											this.writer.WriteAttributeValue("file://");
										}
										this.writer.WriteAttributeValue(this.scratchBuffer.Buffer, num2, num3);
										this.writer.WriteTagEnd();
										this.writer.WriteTextInternal(this.scratchBuffer.Buffer, num2, num3);
										this.writer.WriteEndTag(HtmlNameIndex.A);
									}
									num += num2 + num3;
									if (num == effectiveLength)
									{
										textRun.MoveNext();
										goto IL_37E;
									}
								}
							}
							for (;;)
							{
								char[] buffer;
								int index;
								int num4;
								textRun.GetChunk(num, out buffer, out index, out num4);
								this.writer.WriteTextInternal(buffer, index, num4);
								num += num4;
								if (num == effectiveLength)
								{
									goto IL_377;
								}
							}
						}
					}
					else
					{
						if (type == TextRunType.Space)
						{
							this.writer.WriteSpace(effectiveLength);
							goto IL_377;
						}
						if (type == TextRunType.Tabulation)
						{
							this.writer.WriteTabulation(effectiveLength);
							goto IL_377;
						}
						if (type != TextRunType.NewLine)
						{
							goto IL_377;
						}
						while (effectiveLength-- != 0)
						{
							if (this.writer.LiteralWhitespaceNesting == 0)
							{
								this.writer.WriteStartTag(HtmlNameIndex.BR);
							}
							this.writer.WriteNewLine(false);
						}
						goto IL_377;
					}
					IL_37E:
					if (textRun.Position >= endTextPosition)
					{
						break;
					}
					continue;
					IL_377:
					textRun.MoveNext();
					goto IL_37E;
				}
			}
			return true;
		}

		protected override void EndText()
		{
			this.writer.EndTextChunk();
			this.RevertCharFormat();
		}

		protected override bool StartBlockContainer()
		{
			this.writer.WriteNewLine(true);
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.Preformatted);
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.QuotingLevelDelta);
			if (!distinctProperty.IsNull && distinctProperty.Bool)
			{
				FormatStyle style = base.FormatStore.GetStyle(14);
				base.SubtractDefaultContainerPropertiesFromDistinct(FlagProperties.AllOff, style.PropertyList);
				this.writer.WriteStartTag(HtmlNameIndex.Pre);
			}
			else if (!distinctProperty2.IsNull && distinctProperty2.Integer != 0)
			{
				for (int i = 0; i < distinctProperty2.Integer; i++)
				{
					this.writer.WriteStartTag(HtmlNameIndex.Div);
					this.writer.WriteAttribute(HtmlNameIndex.Class, "EmailQuote");
				}
			}
			else
			{
				if (base.SourceFormat == SourceFormat.Text)
				{
					this.ApplyCharFormat();
				}
				this.writer.WriteStartTag(HtmlNameIndex.Div);
				if (base.SourceFormat == SourceFormat.Text)
				{
					this.writer.WriteAttribute(HtmlNameIndex.Class, "PlainText");
				}
			}
			this.OutputBlockTagAttributes();
			bool flag = false;
			this.OutputBlockCssProperties(ref flag);
			if (base.SourceFormat != SourceFormat.Text)
			{
				this.ApplyCharFormat();
			}
			if (base.CurrentNode.FirstChild.IsNull)
			{
				this.writer.WriteText('\u00a0');
			}
			else if (base.CurrentNode.FirstChild == base.CurrentNode.LastChild && base.CurrentNode.FirstChild.NodeType == FormatContainerType.Text)
			{
				FormatNode firstChild = base.CurrentNode.FirstChild;
				if (firstChild.BeginTextPosition + 1U == firstChild.EndTextPosition && base.FormatStore.GetTextRun(firstChild.BeginTextPosition).Type == TextRunType.Space)
				{
					this.writer.WriteText('\u00a0');
					this.EndBlockContainer();
					return false;
				}
			}
			return true;
		}

		protected override void EndBlockContainer()
		{
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.Preformatted);
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.QuotingLevelDelta);
			if (base.SourceFormat != SourceFormat.Text)
			{
				this.RevertCharFormat();
			}
			if (!distinctProperty.IsNull && distinctProperty.Bool)
			{
				this.writer.WriteEndTag(HtmlNameIndex.Pre);
			}
			else if (!distinctProperty2.IsNull && distinctProperty2.Integer != 0)
			{
				for (int i = 0; i < distinctProperty2.Integer; i++)
				{
					this.writer.WriteEndTag(HtmlNameIndex.Div);
				}
			}
			else
			{
				this.writer.WriteEndTag(HtmlNameIndex.Div);
				if (base.SourceFormat == SourceFormat.Text)
				{
					this.RevertCharFormat();
				}
			}
			this.writer.WriteNewLine(true);
		}

		protected override bool StartInlineContainer()
		{
			return true;
		}

		protected override void EndInlineContainer()
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (this.writer != null && this.writer != null)
			{
				((IDisposable)this.writer).Dispose();
			}
			this.writer = null;
			base.Dispose(disposing);
		}

		private void CloseHyperLink()
		{
			bool flag = false;
			bool flag2 = false;
			if (this.endTagActionStackTop != 0 && this.endTagActionStack[this.endTagActionStackTop - 1].TagLevel == this.hyperlinkLevel)
			{
				this.endTagActionStackTop--;
				flag = this.endTagActionStack[this.endTagActionStackTop].Drop;
				flag2 = this.endTagActionStack[this.endTagActionStackTop].Callback;
			}
			if (flag2)
			{
				this.callbackContext.InitializeTag(true, HtmlNameIndex.A, flag);
				this.callbackContext.InitializeFragment(false);
				this.callback(this.callbackContext, this.writer);
				this.callbackContext.UninitializeFragment();
				return;
			}
			if (!flag)
			{
				this.writer.WriteEndTag(HtmlNameIndex.A);
			}
		}

		private void ApplyCharFormat()
		{
			this.scratchBuffer.Reset();
			FlagProperties distinctFlags = base.GetDistinctFlags();
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.FontSize);
			if (!distinctProperty.IsNull && !distinctProperty.IsHtmlFontUnits && !distinctProperty.IsRelativeHtmlFontUnits)
			{
				this.scratchBuffer.Append("font-size:");
				HtmlSupport.AppendCssFontSize(ref this.scratchBuffer, distinctProperty);
				this.scratchBuffer.Append(';');
			}
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.BackColor);
			if (distinctProperty2.IsColor)
			{
				this.scratchBuffer.Append("background-color:");
				HtmlSupport.AppendColor(ref this.scratchBuffer, distinctProperty2);
				this.scratchBuffer.Append(';');
			}
			Culture culture = null;
			PropertyValue distinctProperty3 = base.GetDistinctProperty(PropertyId.Language);
			if (distinctProperty3.IsInteger && (!Culture.TryGetCulture(distinctProperty3.Integer, out culture) || string.IsNullOrEmpty(culture.Name)))
			{
				culture = null;
			}
			if ((byte)(base.CurrentNode.NodeType & FormatContainerType.BlockFlag) == 0)
			{
				PropertyValue distinctProperty4 = base.GetDistinctProperty(PropertyId.Display);
				PropertyValue distinctProperty5 = base.GetDistinctProperty(PropertyId.UnicodeBiDi);
				if (!distinctProperty4.IsNull)
				{
					string displayString = HtmlSupport.GetDisplayString(distinctProperty4);
					if (displayString != null)
					{
						this.scratchBuffer.Append("display:");
						this.scratchBuffer.Append(displayString);
						this.scratchBuffer.Append(";");
					}
				}
				if (distinctFlags.IsDefined(PropertyId.Visible))
				{
					this.scratchBuffer.Append(distinctFlags.IsOn(PropertyId.Visible) ? "visibility:visible;" : "visibility:hidden;");
				}
				if (!distinctProperty5.IsNull)
				{
					string unicodeBiDiString = HtmlSupport.GetUnicodeBiDiString(distinctProperty5);
					if (unicodeBiDiString != null)
					{
						this.scratchBuffer.Append("unicode-bidi:");
						this.scratchBuffer.Append(unicodeBiDiString);
						this.scratchBuffer.Append(";");
					}
				}
			}
			if (distinctFlags.IsDefinedAndOff(PropertyId.FirstFlag))
			{
				this.scratchBuffer.Append("font-weight:normal;");
			}
			if (distinctFlags.IsDefined(PropertyId.SmallCaps))
			{
				this.scratchBuffer.Append(distinctFlags.IsOn(PropertyId.SmallCaps) ? "font-variant:small-caps;" : "font-variant:normal;");
			}
			if (distinctFlags.IsDefined(PropertyId.Capitalize))
			{
				this.scratchBuffer.Append(distinctFlags.IsOn(PropertyId.Capitalize) ? "text-transform:uppercase;" : "text-transform:none;");
			}
			PropertyValue distinctProperty6 = base.GetDistinctProperty(PropertyId.FontFace);
			PropertyValue distinctProperty7 = base.GetDistinctProperty(PropertyId.FontColor);
			if (!distinctProperty6.IsNull || !distinctProperty.IsNull || !distinctProperty7.IsNull)
			{
				this.writer.WriteStartTag(HtmlNameIndex.Font);
				if (!distinctProperty6.IsNull)
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Face);
					if (distinctProperty6.IsMultiValue)
					{
						MultiValue multiValue = base.FormatStore.GetMultiValue(distinctProperty6);
						for (int i = 0; i < multiValue.Length; i++)
						{
							string @string = multiValue.GetStringValue(i).GetString();
							if (i != 0)
							{
								this.writer.WriteAttributeValue(",");
							}
							this.writer.WriteAttributeValue(@string);
						}
					}
					else
					{
						string @string = base.FormatStore.GetStringValue(distinctProperty6).GetString();
						this.writer.WriteAttributeValue(@string);
					}
				}
				if (!distinctProperty.IsNull)
				{
					BufferString value = HtmlSupport.FormatFontSize(ref this.scratchValueBuffer, distinctProperty);
					if (value.Length != 0)
					{
						this.writer.WriteAttribute(HtmlNameIndex.Size, value);
					}
				}
				if (!distinctProperty7.IsNull)
				{
					BufferString value = HtmlSupport.FormatColor(ref this.scratchValueBuffer, distinctProperty7);
					if (value.Length != 0)
					{
						this.writer.WriteAttribute(HtmlNameIndex.Color, value);
					}
				}
			}
			if (this.scratchBuffer.Length != 0 || distinctFlags.IsDefined(PropertyId.RightToLeft) || culture != null)
			{
				this.writer.WriteStartTag(HtmlNameIndex.Span);
				if (this.scratchBuffer.Length != 0)
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Style);
					this.writer.WriteAttributeValue(this.scratchBuffer.BufferString);
				}
				if (distinctFlags.IsDefined(PropertyId.RightToLeft))
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Dir);
					this.writer.WriteAttributeValue(distinctFlags.IsOn(PropertyId.RightToLeft) ? "rtl" : "ltr");
				}
				if (culture != null)
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Lang);
					this.writer.WriteAttributeValue(culture.Name);
				}
			}
			if (distinctFlags.IsDefinedAndOn(PropertyId.FirstFlag))
			{
				this.writer.WriteStartTag(HtmlNameIndex.B);
			}
			if (distinctFlags.IsDefinedAndOn(PropertyId.Italic))
			{
				this.writer.WriteStartTag(HtmlNameIndex.I);
			}
			if (distinctFlags.IsDefinedAndOn(PropertyId.Underline))
			{
				this.writer.WriteStartTag(HtmlNameIndex.U);
			}
			if (distinctFlags.IsDefinedAndOn(PropertyId.Subscript))
			{
				this.writer.WriteStartTag(HtmlNameIndex.Sub);
			}
			if (distinctFlags.IsDefinedAndOn(PropertyId.Superscript))
			{
				this.writer.WriteStartTag(HtmlNameIndex.Sup);
			}
			if (distinctFlags.IsDefinedAndOn(PropertyId.Strikethrough))
			{
				this.writer.WriteStartTag(HtmlNameIndex.Strike);
			}
		}

		private void RevertCharFormat()
		{
			FlagProperties distinctFlags = base.GetDistinctFlags();
			bool flag = false;
			bool flag2 = false;
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.FontSize);
			if (!distinctProperty.IsNull && !distinctProperty.IsHtmlFontUnits && !distinctProperty.IsRelativeHtmlFontUnits)
			{
				flag2 = true;
			}
			if (base.GetDistinctProperty(PropertyId.BackColor).IsColor)
			{
				flag2 = true;
			}
			Culture culture = null;
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.Language);
			if (distinctProperty2.IsInteger && Culture.TryGetCulture(distinctProperty2.Integer, out culture) && !string.IsNullOrEmpty(culture.Name))
			{
				flag2 = true;
			}
			if ((byte)(base.CurrentNode.NodeType & FormatContainerType.BlockFlag) == 0)
			{
				PropertyValue distinctProperty3 = base.GetDistinctProperty(PropertyId.Display);
				PropertyValue distinctProperty4 = base.GetDistinctProperty(PropertyId.UnicodeBiDi);
				if (!distinctProperty3.IsNull)
				{
					string displayString = HtmlSupport.GetDisplayString(distinctProperty3);
					if (displayString != null)
					{
						flag2 = true;
					}
				}
				if (distinctFlags.IsDefined(PropertyId.Visible))
				{
					flag2 = true;
				}
				if (!distinctProperty4.IsNull)
				{
					string unicodeBiDiString = HtmlSupport.GetUnicodeBiDiString(distinctProperty4);
					if (unicodeBiDiString != null)
					{
						flag2 = true;
					}
				}
			}
			if (distinctFlags.IsDefinedAndOff(PropertyId.FirstFlag))
			{
				flag2 = true;
			}
			if (distinctFlags.IsDefined(PropertyId.SmallCaps))
			{
				flag2 = true;
			}
			if (distinctFlags.IsDefined(PropertyId.Capitalize))
			{
				flag2 = true;
			}
			if (distinctFlags.IsDefined(PropertyId.RightToLeft))
			{
				flag2 = true;
			}
			PropertyValue distinctProperty5 = base.GetDistinctProperty(PropertyId.FontFace);
			PropertyValue distinctProperty6 = base.GetDistinctProperty(PropertyId.FontColor);
			if (!distinctProperty5.IsNull || !distinctProperty.IsNull || !distinctProperty6.IsNull)
			{
				flag = true;
			}
			if (distinctFlags.IsDefinedAndOn(PropertyId.Strikethrough))
			{
				this.writer.WriteEndTag(HtmlNameIndex.Strike);
			}
			if (distinctFlags.IsDefinedAndOn(PropertyId.Superscript))
			{
				this.writer.WriteEndTag(HtmlNameIndex.Sup);
			}
			if (distinctFlags.IsDefinedAndOn(PropertyId.Subscript))
			{
				this.writer.WriteEndTag(HtmlNameIndex.Sub);
			}
			if (distinctFlags.IsDefinedAndOn(PropertyId.Underline))
			{
				this.writer.WriteEndTag(HtmlNameIndex.U);
			}
			if (distinctFlags.IsDefinedAndOn(PropertyId.Italic))
			{
				this.writer.WriteEndTag(HtmlNameIndex.I);
			}
			if (distinctFlags.IsDefinedAndOn(PropertyId.FirstFlag))
			{
				this.writer.WriteEndTag(HtmlNameIndex.B);
			}
			if (flag2)
			{
				this.writer.WriteEndTag(HtmlNameIndex.Span);
			}
			if (flag)
			{
				this.writer.WriteEndTag(HtmlNameIndex.Font);
			}
		}

		private void OutputBlockCssProperties(ref bool styleAttributeOpen)
		{
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.Display);
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.Visible);
			PropertyValue distinctProperty3 = base.GetDistinctProperty(PropertyId.Height);
			PropertyValue distinctProperty4 = base.GetDistinctProperty(PropertyId.Width);
			PropertyValue distinctProperty5 = base.GetDistinctProperty(PropertyId.UnicodeBiDi);
			PropertyValue distinctProperty6 = base.GetDistinctProperty(PropertyId.FirstLineIndent);
			PropertyValue distinctProperty7 = base.GetDistinctProperty(PropertyId.TextAlignment);
			PropertyValue distinctProperty8 = base.GetDistinctProperty(PropertyId.BackColor);
			PropertyValue distinctProperty9 = base.GetDistinctProperty(PropertyId.Margins);
			PropertyValue distinctProperty10 = base.GetDistinctProperty(PropertyId.RightMargin);
			PropertyValue distinctProperty11 = base.GetDistinctProperty(PropertyId.BottomMargin);
			PropertyValue distinctProperty12 = base.GetDistinctProperty(PropertyId.LeftMargin);
			PropertyValue distinctProperty13 = base.GetDistinctProperty(PropertyId.Paddings);
			PropertyValue distinctProperty14 = base.GetDistinctProperty(PropertyId.RightPadding);
			PropertyValue distinctProperty15 = base.GetDistinctProperty(PropertyId.BottomPadding);
			PropertyValue distinctProperty16 = base.GetDistinctProperty(PropertyId.LeftPadding);
			PropertyValue distinctProperty17 = base.GetDistinctProperty(PropertyId.BorderWidths);
			PropertyValue distinctProperty18 = base.GetDistinctProperty(PropertyId.RightBorderWidth);
			PropertyValue distinctProperty19 = base.GetDistinctProperty(PropertyId.BottomBorderWidth);
			PropertyValue distinctProperty20 = base.GetDistinctProperty(PropertyId.LeftBorderWidth);
			PropertyValue distinctProperty21 = base.GetDistinctProperty(PropertyId.BorderStyles);
			PropertyValue distinctProperty22 = base.GetDistinctProperty(PropertyId.RightBorderStyle);
			PropertyValue distinctProperty23 = base.GetDistinctProperty(PropertyId.BottomBorderStyle);
			PropertyValue distinctProperty24 = base.GetDistinctProperty(PropertyId.LeftBorderStyle);
			PropertyValue distinctProperty25 = base.GetDistinctProperty(PropertyId.BorderColors);
			PropertyValue distinctProperty26 = base.GetDistinctProperty(PropertyId.RightBorderColor);
			PropertyValue distinctProperty27 = base.GetDistinctProperty(PropertyId.BottomBorderColor);
			PropertyValue distinctProperty28 = base.GetDistinctProperty(PropertyId.LeftBorderColor);
			if (!distinctProperty2.IsNull || !distinctProperty.IsNull || !distinctProperty5.IsNull || !distinctProperty4.IsNull || !distinctProperty3.IsNull)
			{
				if (!styleAttributeOpen)
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Style);
					styleAttributeOpen = true;
				}
				if (!distinctProperty.IsNull)
				{
					string displayString = HtmlSupport.GetDisplayString(distinctProperty);
					if (displayString != null)
					{
						this.scratchBuffer.Append("display:");
						this.scratchBuffer.Append(displayString);
						this.scratchBuffer.Append(";");
					}
				}
				if (!distinctProperty2.IsNull)
				{
					this.scratchBuffer.Append(distinctProperty2.Bool ? "visibility:visible;" : "visibility:hidden;");
				}
				if (!distinctProperty4.IsNull)
				{
					BufferString value = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty4);
					if (value.Length != 0)
					{
						this.writer.WriteAttributeValue("width:");
						this.writer.WriteAttributeValue(value);
						this.writer.WriteAttributeValue(";");
					}
				}
				if (!distinctProperty3.IsNull)
				{
					BufferString value2 = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty3);
					if (value2.Length != 0)
					{
						this.writer.WriteAttributeValue("height:");
						this.writer.WriteAttributeValue(value2);
						this.writer.WriteAttributeValue(";");
					}
				}
				if (!distinctProperty5.IsNull)
				{
					string unicodeBiDiString = HtmlSupport.GetUnicodeBiDiString(distinctProperty5);
					if (unicodeBiDiString != null)
					{
						this.writer.WriteAttributeValue("unicode-bidi:");
						this.writer.WriteAttributeValue(unicodeBiDiString);
						this.writer.WriteAttributeValue(";");
					}
				}
			}
			if (!distinctProperty6.IsNull || !distinctProperty7.IsNull || !distinctProperty8.IsNull)
			{
				if (!styleAttributeOpen)
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Style);
					styleAttributeOpen = true;
				}
				if (!distinctProperty6.IsNull)
				{
					BufferString value3 = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty6);
					if (value3.Length != 0)
					{
						this.writer.WriteAttributeValue("text-indent:");
						this.writer.WriteAttributeValue(value3);
						this.writer.WriteAttributeValue(";");
					}
				}
				if (!distinctProperty7.IsNull && distinctProperty7.IsEnum && distinctProperty7.Enum < HtmlSupport.TextAlignmentEnumeration.Length)
				{
					this.writer.WriteAttributeValue("text-align:");
					this.writer.WriteAttributeValue(HtmlSupport.TextAlignmentEnumeration[distinctProperty7.Enum].Name);
					this.writer.WriteAttributeValue(";");
				}
				if (!distinctProperty8.IsNull)
				{
					BufferString value4 = HtmlSupport.FormatColor(ref this.scratchBuffer, distinctProperty8);
					if (value4.Length != 0)
					{
						this.writer.WriteAttributeValue("background-color:");
						this.writer.WriteAttributeValue(value4);
						this.writer.WriteAttributeValue(";");
					}
				}
			}
			if (!distinctProperty9.IsNull || !distinctProperty10.IsNull || !distinctProperty11.IsNull || !distinctProperty12.IsNull)
			{
				if (!styleAttributeOpen)
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Style);
					styleAttributeOpen = true;
				}
				this.OutputMarginAndPaddingProperties("margin", distinctProperty9, distinctProperty10, distinctProperty11, distinctProperty12);
			}
			if (!distinctProperty13.IsNull || !distinctProperty14.IsNull || !distinctProperty15.IsNull || !distinctProperty16.IsNull)
			{
				if (!styleAttributeOpen)
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Style);
					styleAttributeOpen = true;
				}
				this.OutputMarginAndPaddingProperties("padding", distinctProperty13, distinctProperty14, distinctProperty15, distinctProperty16);
			}
			if (!distinctProperty17.IsNull || !distinctProperty18.IsNull || !distinctProperty19.IsNull || !distinctProperty20.IsNull || !distinctProperty21.IsNull || !distinctProperty22.IsNull || !distinctProperty23.IsNull || !distinctProperty24.IsNull || !distinctProperty25.IsNull || !distinctProperty26.IsNull || !distinctProperty27.IsNull || !distinctProperty28.IsNull)
			{
				if (!styleAttributeOpen)
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Style);
					styleAttributeOpen = true;
				}
				this.OutputBorderProperties(distinctProperty17, distinctProperty18, distinctProperty19, distinctProperty20, distinctProperty21, distinctProperty22, distinctProperty23, distinctProperty24, distinctProperty25, distinctProperty26, distinctProperty27, distinctProperty28);
			}
		}

		private void OutputMarginAndPaddingProperties(string name, PropertyValue topValue, PropertyValue rightValue, PropertyValue bottomValue, PropertyValue leftValue)
		{
			int num = 0;
			if (!topValue.IsNull)
			{
				num++;
			}
			if (!rightValue.IsNull)
			{
				num++;
			}
			if (!bottomValue.IsNull)
			{
				num++;
			}
			if (!leftValue.IsNull)
			{
				num++;
			}
			if (num == 4)
			{
				this.writer.WriteAttributeValue(name);
				this.writer.WriteAttributeValue(":");
				if (topValue == rightValue && topValue == bottomValue && topValue == leftValue)
				{
					this.OutputLengthPropertyValue(topValue);
				}
				else if (topValue == bottomValue && rightValue == leftValue)
				{
					this.OutputCompositeLengthPropertyValue(topValue, rightValue);
				}
				else
				{
					this.OutputCompositeLengthPropertyValue(topValue, rightValue, bottomValue, leftValue);
				}
				this.writer.WriteAttributeValue(";");
				return;
			}
			if (!topValue.IsNull)
			{
				this.writer.WriteAttributeValue(name);
				this.writer.WriteAttributeValue("-top:");
				this.OutputLengthPropertyValue(topValue);
				this.writer.WriteAttributeValue(";");
			}
			if (!rightValue.IsNull)
			{
				this.writer.WriteAttributeValue(name);
				this.writer.WriteAttributeValue("-right:");
				this.OutputLengthPropertyValue(rightValue);
				this.writer.WriteAttributeValue(";");
			}
			if (!bottomValue.IsNull)
			{
				this.writer.WriteAttributeValue(name);
				this.writer.WriteAttributeValue("-bottom:");
				this.OutputLengthPropertyValue(bottomValue);
				this.writer.WriteAttributeValue(";");
			}
			if (!leftValue.IsNull)
			{
				this.writer.WriteAttributeValue(name);
				this.writer.WriteAttributeValue("-left:");
				this.OutputLengthPropertyValue(leftValue);
				this.writer.WriteAttributeValue(";");
			}
		}

		private void OutputBorderProperties(PropertyValue topBorderWidth, PropertyValue rightBorderWidth, PropertyValue bottomBorderWidth, PropertyValue leftBorderWidth, PropertyValue topBorderStyle, PropertyValue rightBorderStyle, PropertyValue bottomBorderStyle, PropertyValue leftBorderStyle, PropertyValue topBorderColor, PropertyValue rightBorderColor, PropertyValue bottomBorderColor, PropertyValue leftBorderColor)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			if (!topBorderWidth.IsNull)
			{
				num++;
				num4++;
			}
			if (!rightBorderWidth.IsNull)
			{
				num++;
				num5++;
			}
			if (!bottomBorderWidth.IsNull)
			{
				num++;
				num6++;
			}
			if (!leftBorderWidth.IsNull)
			{
				num++;
				num7++;
			}
			if (!topBorderStyle.IsNull)
			{
				num2++;
				num4++;
			}
			if (!rightBorderStyle.IsNull)
			{
				num2++;
				num5++;
			}
			if (!bottomBorderStyle.IsNull)
			{
				num2++;
				num6++;
			}
			if (!leftBorderStyle.IsNull)
			{
				num2++;
				num7++;
			}
			if (!topBorderColor.IsNull)
			{
				num3++;
				num4++;
			}
			if (!rightBorderColor.IsNull)
			{
				num3++;
				num5++;
			}
			if (!bottomBorderColor.IsNull)
			{
				num3++;
				num6++;
			}
			if (!leftBorderColor.IsNull)
			{
				num3++;
				num7++;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			if (num == 4 && topBorderWidth == bottomBorderWidth && rightBorderWidth == leftBorderWidth)
			{
				flag2 = true;
				flag = (topBorderWidth == rightBorderWidth);
			}
			if (num2 == 4 && topBorderStyle == bottomBorderStyle && rightBorderStyle == leftBorderStyle)
			{
				flag4 = true;
				flag3 = (topBorderStyle == rightBorderStyle);
			}
			if (num3 == 4 && topBorderColor == bottomBorderColor && rightBorderColor == leftBorderColor)
			{
				flag6 = true;
				flag5 = (topBorderColor == rightBorderColor);
			}
			if (num != 4 || num2 != 4 || num3 != 4)
			{
				bool flag7 = false;
				bool flag8 = false;
				bool flag9 = false;
				bool flag10 = false;
				bool flag11 = false;
				bool flag12 = false;
				bool flag13 = false;
				bool flag14 = false;
				bool flag15 = false;
				bool flag16 = false;
				bool flag17 = false;
				bool flag18 = false;
				if (num == 4 || num2 == 4 || num3 == 4)
				{
					if (num == 4)
					{
						this.writer.WriteAttributeValue("border-width:");
						if (flag)
						{
							this.OutputBorderWidthPropertyValue(topBorderWidth);
						}
						else if (flag2)
						{
							this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth);
						}
						else
						{
							this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth, bottomBorderWidth, leftBorderWidth);
						}
						this.writer.WriteAttributeValue(";");
						flag7 = true;
						flag8 = true;
						flag9 = true;
						flag10 = true;
					}
					if (num2 == 4)
					{
						this.writer.WriteAttributeValue("border-style:");
						if (flag3)
						{
							this.OutputBorderStylePropertyValue(topBorderStyle);
						}
						else if (flag4)
						{
							this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle);
						}
						else
						{
							this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle, bottomBorderStyle, leftBorderStyle);
						}
						this.writer.WriteAttributeValue(";");
						flag11 = true;
						flag12 = true;
						flag13 = true;
						flag14 = true;
					}
					if (num3 == 4)
					{
						this.writer.WriteAttributeValue("border-color:");
						if (flag5)
						{
							this.OutputBorderColorPropertyValue(topBorderColor);
						}
						else if (flag6)
						{
							this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor);
						}
						else
						{
							this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor, bottomBorderColor, leftBorderColor);
						}
						this.writer.WriteAttributeValue(";");
						flag15 = true;
						flag16 = true;
						flag17 = true;
						flag18 = true;
					}
				}
				else if (num4 == 3 || num5 == 3 || num6 == 3 || num7 == 3)
				{
					if (num4 == 3)
					{
						this.writer.WriteAttributeValue("border-top:");
						this.OutputCompositeBorderSidePropertyValue(topBorderWidth, topBorderStyle, topBorderColor);
						this.writer.WriteAttributeValue(";");
						flag7 = true;
						flag11 = true;
						flag15 = true;
					}
					if (num5 == 3)
					{
						this.writer.WriteAttributeValue("border-right:");
						this.OutputCompositeBorderSidePropertyValue(rightBorderWidth, rightBorderStyle, rightBorderColor);
						this.writer.WriteAttributeValue(";");
						flag8 = true;
						flag12 = true;
						flag16 = true;
					}
					if (num6 == 3)
					{
						this.writer.WriteAttributeValue("border-bottom:");
						this.OutputCompositeBorderSidePropertyValue(bottomBorderWidth, bottomBorderStyle, bottomBorderColor);
						this.writer.WriteAttributeValue(";");
						flag9 = true;
						flag13 = true;
						flag17 = true;
					}
					if (num7 == 3)
					{
						this.writer.WriteAttributeValue("border-left:");
						this.OutputCompositeBorderSidePropertyValue(leftBorderWidth, leftBorderStyle, leftBorderColor);
						this.writer.WriteAttributeValue(";");
						flag10 = true;
						flag14 = true;
						flag18 = true;
					}
				}
				if (!flag7 && !topBorderWidth.IsNull)
				{
					this.writer.WriteAttributeValue("border-top-width:");
					this.OutputBorderWidthPropertyValue(topBorderWidth);
					this.writer.WriteAttributeValue(";");
				}
				if (!flag8 && !rightBorderWidth.IsNull)
				{
					this.writer.WriteAttributeValue("border-right-width:");
					this.OutputBorderWidthPropertyValue(rightBorderWidth);
					this.writer.WriteAttributeValue(";");
				}
				if (!flag9 && !bottomBorderWidth.IsNull)
				{
					this.writer.WriteAttributeValue("border-bottom-width:");
					this.OutputBorderWidthPropertyValue(bottomBorderWidth);
					this.writer.WriteAttributeValue(";");
				}
				if (!flag10 && !leftBorderWidth.IsNull)
				{
					this.writer.WriteAttributeValue("border-left-width:");
					this.OutputBorderWidthPropertyValue(leftBorderWidth);
					this.writer.WriteAttributeValue(";");
				}
				if (!flag11 && !topBorderStyle.IsNull)
				{
					this.writer.WriteAttributeValue("border-top-style:");
					this.OutputBorderStylePropertyValue(topBorderStyle);
					this.writer.WriteAttributeValue(";");
				}
				if (!flag12 && !rightBorderStyle.IsNull)
				{
					this.writer.WriteAttributeValue("border-right-style:");
					this.OutputBorderStylePropertyValue(rightBorderStyle);
					this.writer.WriteAttributeValue(";");
				}
				if (!flag13 && !bottomBorderStyle.IsNull)
				{
					this.writer.WriteAttributeValue("border-bottom-style:");
					this.OutputBorderStylePropertyValue(bottomBorderStyle);
					this.writer.WriteAttributeValue(";");
				}
				if (!flag14 && !leftBorderStyle.IsNull)
				{
					this.writer.WriteAttributeValue("border-left-style:");
					this.OutputBorderStylePropertyValue(leftBorderStyle);
					this.writer.WriteAttributeValue(";");
				}
				if (!flag15 && !topBorderColor.IsNull)
				{
					this.writer.WriteAttributeValue("border-top-color:");
					this.OutputBorderColorPropertyValue(topBorderColor);
					this.writer.WriteAttributeValue(";");
				}
				if (!flag16 && !rightBorderColor.IsNull)
				{
					this.writer.WriteAttributeValue("border-right-color:");
					this.OutputBorderColorPropertyValue(rightBorderColor);
					this.writer.WriteAttributeValue(";");
				}
				if (!flag17 && !bottomBorderColor.IsNull)
				{
					this.writer.WriteAttributeValue("border-bottom-color:");
					this.OutputBorderColorPropertyValue(bottomBorderColor);
					this.writer.WriteAttributeValue(";");
				}
				if (!flag18 && !leftBorderColor.IsNull)
				{
					this.writer.WriteAttributeValue("border-left-color:");
					this.OutputBorderColorPropertyValue(leftBorderColor);
					this.writer.WriteAttributeValue(";");
				}
				return;
			}
			if (flag && flag3 && flag5)
			{
				this.writer.WriteAttributeValue("border:");
				this.OutputCompositeBorderSidePropertyValue(topBorderWidth, topBorderStyle, topBorderColor);
				this.writer.WriteAttributeValue(";");
				return;
			}
			this.writer.WriteAttributeValue("border-width:");
			if (flag)
			{
				this.OutputBorderWidthPropertyValue(topBorderWidth);
			}
			else if (flag2)
			{
				this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth);
			}
			else
			{
				this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth, bottomBorderWidth, leftBorderWidth);
			}
			this.writer.WriteAttributeValue(";");
			this.writer.WriteAttributeValue("border-style:");
			if (flag3)
			{
				this.OutputBorderStylePropertyValue(topBorderStyle);
			}
			else if (flag4)
			{
				this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle);
			}
			else
			{
				this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle, bottomBorderStyle, leftBorderStyle);
			}
			this.writer.WriteAttributeValue(";");
			this.writer.WriteAttributeValue("border-color:");
			if (flag5)
			{
				this.OutputBorderColorPropertyValue(topBorderColor);
			}
			else if (flag6)
			{
				this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor);
			}
			else
			{
				this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor, bottomBorderColor, leftBorderColor);
			}
			this.writer.WriteAttributeValue(";");
		}

		private void OutputCompositeBorderSidePropertyValue(PropertyValue width, PropertyValue style, PropertyValue color)
		{
			this.OutputBorderWidthPropertyValue(width);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderStylePropertyValue(style);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderColorPropertyValue(color);
		}

		private void OutputCompositeLengthPropertyValue(PropertyValue topBottom, PropertyValue rightLeft)
		{
			this.OutputLengthPropertyValue(topBottom);
			this.writer.WriteAttributeValue(" ");
			this.OutputLengthPropertyValue(rightLeft);
		}

		private void OutputCompositeLengthPropertyValue(PropertyValue top, PropertyValue right, PropertyValue bottom, PropertyValue left)
		{
			this.OutputLengthPropertyValue(top);
			this.writer.WriteAttributeValue(" ");
			this.OutputLengthPropertyValue(right);
			this.writer.WriteAttributeValue(" ");
			this.OutputLengthPropertyValue(bottom);
			this.writer.WriteAttributeValue(" ");
			this.OutputLengthPropertyValue(left);
		}

		private void OutputCompositeBorderWidthPropertyValue(PropertyValue topBottom, PropertyValue rightLeft)
		{
			this.OutputBorderWidthPropertyValue(topBottom);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderWidthPropertyValue(rightLeft);
		}

		private void OutputCompositeBorderWidthPropertyValue(PropertyValue top, PropertyValue right, PropertyValue bottom, PropertyValue left)
		{
			this.OutputBorderWidthPropertyValue(top);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderWidthPropertyValue(right);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderWidthPropertyValue(bottom);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderWidthPropertyValue(left);
		}

		private void OutputCompositeBorderStylePropertyValue(PropertyValue topBottom, PropertyValue rightLeft)
		{
			this.OutputBorderStylePropertyValue(topBottom);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderStylePropertyValue(rightLeft);
		}

		private void OutputCompositeBorderStylePropertyValue(PropertyValue top, PropertyValue right, PropertyValue bottom, PropertyValue left)
		{
			this.OutputBorderStylePropertyValue(top);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderStylePropertyValue(right);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderStylePropertyValue(bottom);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderStylePropertyValue(left);
		}

		private void OutputCompositeBorderColorPropertyValue(PropertyValue topBottom, PropertyValue rightLeft)
		{
			this.OutputBorderColorPropertyValue(topBottom);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderColorPropertyValue(rightLeft);
		}

		private void OutputCompositeBorderColorPropertyValue(PropertyValue top, PropertyValue right, PropertyValue bottom, PropertyValue left)
		{
			this.OutputBorderColorPropertyValue(top);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderColorPropertyValue(right);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderColorPropertyValue(bottom);
			this.writer.WriteAttributeValue(" ");
			this.OutputBorderColorPropertyValue(left);
		}

		private void OutputLengthPropertyValue(PropertyValue width)
		{
			BufferString value = HtmlSupport.FormatLength(ref this.scratchBuffer, width);
			if (value.Length != 0)
			{
				this.writer.WriteAttributeValue(value);
				return;
			}
			this.writer.WriteAttributeValue("0");
		}

		private void OutputBorderWidthPropertyValue(PropertyValue width)
		{
			BufferString value = HtmlSupport.FormatLength(ref this.scratchBuffer, width);
			if (value.Length != 0)
			{
				this.writer.WriteAttributeValue(value);
				return;
			}
			this.writer.WriteAttributeValue("medium");
		}

		private void OutputBorderStylePropertyValue(PropertyValue style)
		{
			string borderStyleString = HtmlSupport.GetBorderStyleString(style);
			if (borderStyleString != null)
			{
				this.writer.WriteAttributeValue(borderStyleString);
				return;
			}
			this.writer.WriteAttributeValue("solid");
		}

		private void OutputBorderColorPropertyValue(PropertyValue color)
		{
			BufferString value = HtmlSupport.FormatColor(ref this.scratchBuffer, color);
			if (value.Length != 0)
			{
				this.writer.WriteAttributeValue(value);
				return;
			}
			this.writer.WriteAttributeValue("black");
		}

		private void OutputTableCssProperties(ref bool styleAttributeOpen)
		{
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.Overloaded1);
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.Overloaded2);
			PropertyValue distinctProperty3 = base.GetDistinctProperty(PropertyId.TableShowEmptyCells);
			PropertyValue distinctProperty4 = base.GetDistinctProperty(PropertyId.TableCaptionSideTop);
			PropertyValue distinctProperty5 = base.GetDistinctProperty(PropertyId.TableBorderSpacingVertical);
			PropertyValue distinctProperty6 = base.GetDistinctProperty(PropertyId.TableBorderSpacingHorizontal);
			if (!distinctProperty.IsNull || !distinctProperty2.IsNull || !distinctProperty3.IsNull || !distinctProperty4.IsNull || !distinctProperty5.IsNull || !distinctProperty6.IsNull)
			{
				if (!styleAttributeOpen)
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Style);
					styleAttributeOpen = true;
				}
				if (!distinctProperty.IsNull)
				{
					this.writer.WriteAttributeValue("table-layout:");
					this.writer.WriteAttributeValue(distinctProperty.Bool ? "fixed" : "auto");
					this.writer.WriteAttributeValue(";");
				}
				if (!distinctProperty2.IsNull)
				{
					this.writer.WriteAttributeValue("border-collapse:");
					this.writer.WriteAttributeValue(distinctProperty2.Bool ? "collapse" : "separate");
					this.writer.WriteAttributeValue(";");
				}
				if (!distinctProperty3.IsNull)
				{
					this.writer.WriteAttributeValue("empty-cells:");
					this.writer.WriteAttributeValue(distinctProperty3.Bool ? "show" : "hide");
					this.writer.WriteAttributeValue(";");
				}
				if (!distinctProperty4.IsNull)
				{
					this.writer.WriteAttributeValue("caption-side:");
					this.writer.WriteAttributeValue(distinctProperty4.Bool ? "top" : "bottom");
					this.writer.WriteAttributeValue(";");
				}
				if (!distinctProperty5.IsNull && !distinctProperty5.IsNull)
				{
					BufferString value = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty5);
					if (value.Length != 0)
					{
						this.writer.WriteAttributeValue("border-spacing:");
						this.writer.WriteAttributeValue(value);
						if (distinctProperty5 != distinctProperty6)
						{
							value = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty6);
							if (value.Length != 0)
							{
								this.writer.WriteAttributeValue(" ");
								this.writer.WriteAttributeValue(value);
							}
						}
						this.writer.WriteAttributeValue(";");
					}
				}
			}
		}

		private void OutputTableColumnCssProperties(ref bool styleAttributeOpen)
		{
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.Width);
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.BackColor);
			if (!distinctProperty2.IsNull || !distinctProperty.IsNull)
			{
				if (!styleAttributeOpen)
				{
					this.writer.WriteAttributeName(HtmlNameIndex.Style);
					styleAttributeOpen = true;
				}
				if (!distinctProperty.IsNull)
				{
					BufferString value = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty);
					if (value.Length != 0)
					{
						this.writer.WriteAttributeValue("width:");
						this.writer.WriteAttributeValue(value);
						this.writer.WriteAttributeValue(";");
					}
				}
				if (!distinctProperty2.IsNull)
				{
					BufferString value2 = HtmlSupport.FormatColor(ref this.scratchBuffer, distinctProperty2);
					if (value2.Length != 0)
					{
						this.writer.WriteAttributeValue("background-color:");
						this.writer.WriteAttributeValue(value2);
					}
				}
			}
		}

		private void OutputBlockTagAttributes()
		{
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.RightToLeft);
			if (!distinctProperty.IsNull)
			{
				this.writer.WriteAttribute(HtmlNameIndex.Dir, distinctProperty.Bool ? "rtl" : "ltr");
			}
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.TextAlignment);
			if (!distinctProperty2.IsNull)
			{
				string textAlignmentString = HtmlSupport.GetTextAlignmentString(distinctProperty2);
				if (textAlignmentString != null)
				{
					this.writer.WriteAttribute(HtmlNameIndex.Align, textAlignmentString);
				}
			}
			this.WriteIdAttribute(false);
		}

		private void OutputTableTagAttributes()
		{
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.Width);
			if (!distinctProperty.IsNull)
			{
				BufferString value = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty);
				if (value.Length != 0)
				{
					this.writer.WriteAttribute(HtmlNameIndex.Width, value);
				}
			}
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.BlockAlignment);
			if (!distinctProperty2.IsNull)
			{
				string horizontalAlignmentString = HtmlSupport.GetHorizontalAlignmentString(distinctProperty2);
				if (horizontalAlignmentString != null)
				{
					this.writer.WriteAttribute(HtmlNameIndex.Align, horizontalAlignmentString);
				}
			}
			PropertyValue distinctProperty3 = base.GetDistinctProperty(PropertyId.RightToLeft);
			if (!distinctProperty3.IsNull)
			{
				this.writer.WriteAttribute(HtmlNameIndex.Dir, distinctProperty3.Bool ? "rtl" : "ltr");
			}
			PropertyValue distinctProperty4 = base.GetDistinctProperty(PropertyId.TableBorder);
			if (!distinctProperty4.IsNull)
			{
				BufferString value2 = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty4);
				if (value2.Length != 0)
				{
					this.writer.WriteAttribute(HtmlNameIndex.Border, value2);
				}
			}
			PropertyValue distinctProperty5 = base.GetDistinctProperty(PropertyId.TableFrame);
			if (!distinctProperty5.IsNull)
			{
				string tableFrameString = HtmlSupport.GetTableFrameString(distinctProperty5);
				if (tableFrameString != null)
				{
					this.writer.WriteAttribute(HtmlNameIndex.Frame, tableFrameString);
				}
			}
			PropertyValue distinctProperty6 = base.GetDistinctProperty(PropertyId.TableRules);
			if (!distinctProperty6.IsNull)
			{
				string tableRulesString = HtmlSupport.GetTableRulesString(distinctProperty6);
				if (tableRulesString != null)
				{
					this.writer.WriteAttribute(HtmlNameIndex.Rules, tableRulesString);
				}
			}
			PropertyValue distinctProperty7 = base.GetDistinctProperty(PropertyId.TableCellSpacing);
			if (!distinctProperty7.IsNull)
			{
				BufferString value3 = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty7);
				if (value3.Length != 0)
				{
					this.writer.WriteAttribute(HtmlNameIndex.CellSpacing, value3);
				}
			}
			PropertyValue distinctProperty8 = base.GetDistinctProperty(PropertyId.TableCellPadding);
			if (!distinctProperty8.IsNull)
			{
				BufferString value4 = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty8);
				if (value4.Length != 0)
				{
					this.writer.WriteAttribute(HtmlNameIndex.CellPadding, value4);
				}
			}
		}

		private void OutputTableCellTagAttributes()
		{
			PropertyValue distinctProperty = base.GetDistinctProperty(PropertyId.NumColumns);
			if (distinctProperty.IsInteger && distinctProperty.Integer != 1)
			{
				this.writer.WriteAttribute(HtmlNameIndex.ColSpan, distinctProperty.Integer.ToString());
			}
			PropertyValue distinctProperty2 = base.GetDistinctProperty(PropertyId.NumRows);
			if (distinctProperty2.IsInteger && distinctProperty2.Integer != 1)
			{
				this.writer.WriteAttribute(HtmlNameIndex.RowSpan, distinctProperty2.Integer.ToString());
			}
			PropertyValue distinctProperty3 = base.GetDistinctProperty(PropertyId.Width);
			if (!distinctProperty3.IsNull && distinctProperty3.IsAbsRelLength)
			{
				this.writer.WriteAttribute(HtmlNameIndex.Width, distinctProperty3.PixelsInteger.ToString());
			}
			PropertyValue distinctProperty4 = base.GetDistinctProperty(PropertyId.TextAlignment);
			if (!distinctProperty4.IsNull)
			{
				string textAlignmentString = HtmlSupport.GetTextAlignmentString(distinctProperty4);
				if (textAlignmentString != null)
				{
					this.writer.WriteAttribute(HtmlNameIndex.Align, textAlignmentString);
				}
			}
			PropertyValue distinctProperty5 = base.GetDistinctProperty(PropertyId.BlockAlignment);
			if (!distinctProperty5.IsNull)
			{
				string verticalAlignmentString = HtmlSupport.GetVerticalAlignmentString(distinctProperty5);
				if (verticalAlignmentString != null)
				{
					this.writer.WriteAttribute(HtmlNameIndex.Valign, verticalAlignmentString);
				}
			}
			PropertyValue distinctProperty6 = base.GetDistinctProperty(PropertyId.TableCellNoWrap);
			if (!distinctProperty6.IsNull && distinctProperty6.Bool)
			{
				this.writer.WriteAttribute(HtmlNameIndex.NoWrap, string.Empty);
			}
		}

		private bool RecognizeHyperLink(TextRun run, out int offset, out int length, out bool addFilePrefix, out bool addHttpPrefix)
		{
			this.scratchBuffer.Reset();
			int i = run.AppendFragment(0, ref this.scratchBuffer, 30);
			offset = 0;
			length = 0;
			bool flag = false;
			while (offset < Math.Min(i - 10, 20))
			{
				if (HtmlFormatOutput.IsHyperLinkStartDelimiter(this.scratchBuffer[offset]))
				{
					flag = true;
					break;
				}
				offset++;
			}
			if (!flag)
			{
				offset = 0;
			}
			bool flag2 = false;
			while (offset < i - 4 && HtmlFormatOutput.IsHyperLinkStartDelimiter(this.scratchBuffer[offset]))
			{
				flag2 = true;
				offset++;
			}
			bool flag3 = false;
			addHttpPrefix = false;
			addFilePrefix = false;
			if (this.scratchBuffer[offset] == '\\')
			{
				if (this.scratchBuffer[offset + 1] == '\\' && char.IsLetterOrDigit(this.scratchBuffer[offset + 2]))
				{
					flag3 = true;
					addFilePrefix = true;
				}
			}
			else if (i - offset > 4 && this.scratchBuffer[offset] == 'h')
			{
				if (this.scratchBuffer[offset + 1] == 't' && this.scratchBuffer[offset + 2] == 't' && this.scratchBuffer[offset + 3] == 'p' && (this.scratchBuffer[offset + 4] == ':' || (i - offset > 5 && this.scratchBuffer[offset + 4] == 's' && this.scratchBuffer[offset + 5] == ':')))
				{
					flag3 = true;
				}
			}
			else if (i - offset > 3 && this.scratchBuffer[offset] == 'f')
			{
				if (this.scratchBuffer[offset + 1] == 't' && this.scratchBuffer[offset + 2] == 'p' && this.scratchBuffer[offset + 3] == ':')
				{
					flag3 = true;
				}
				else if (i - offset > 6 && this.scratchBuffer[offset + 1] == 'i' && this.scratchBuffer[offset + 2] == 'l' && this.scratchBuffer[offset + 3] == 'e' && this.scratchBuffer[offset + 4] == ':' && this.scratchBuffer[offset + 5] == '/' && this.scratchBuffer[offset + 6] == '/')
				{
					flag3 = true;
				}
			}
			else if (i - offset > 6 && this.scratchBuffer[offset] == 'm')
			{
				if (this.scratchBuffer[offset + 1] == 'a' && this.scratchBuffer[offset + 2] == 'i' && this.scratchBuffer[offset + 3] == 'l' && this.scratchBuffer[offset + 4] == 't' && this.scratchBuffer[offset + 5] == 'o' && this.scratchBuffer[offset + 6] == ':')
				{
					flag3 = true;
				}
			}
			else if (i - offset > 3 && this.scratchBuffer[offset] == 'w')
			{
				if (this.scratchBuffer[offset + 1] == 'w' && this.scratchBuffer[offset + 2] == 'w' && this.scratchBuffer[offset + 3] == '.')
				{
					flag3 = true;
					addHttpPrefix = true;
				}
			}
			else if (i - offset > 7 && this.scratchBuffer[offset] == 'n' && this.scratchBuffer[offset + 1] == 'o' && this.scratchBuffer[offset + 2] == 't' && this.scratchBuffer[offset + 3] == 'e' && this.scratchBuffer[offset + 4] == 's' && this.scratchBuffer[offset + 5] == ':' && this.scratchBuffer[offset + 6] == '/' && this.scratchBuffer[offset + 7] == '/')
			{
				flag3 = true;
			}
			if (flag3)
			{
				i += run.AppendFragment(i, ref this.scratchBuffer, 4096 - i);
				if (flag2)
				{
					while (i > offset)
					{
						if (HtmlFormatOutput.IsHyperLinkEndDelimiter(this.scratchBuffer[i - 1]))
						{
							break;
						}
						i--;
					}
					while (i > offset)
					{
						if (!HtmlFormatOutput.IsHyperLinkEndDelimiter(this.scratchBuffer[i - 1]))
						{
							break;
						}
						i--;
					}
				}
				else
				{
					while (HtmlFormatOutput.IsHyperLinkEndDelimiter(this.scratchBuffer[i - 1]) || this.scratchBuffer[i - 1] == '.' || this.scratchBuffer[i - 1] == ',' || this.scratchBuffer[i - 1] == ';')
					{
						i--;
					}
				}
				length = i - offset;
			}
			return flag3;
		}

		private const int MaxRecognizedHyperlinkLength = 4096;

		private static string[] listType = new string[]
		{
			null,
			null,
			"1",
			"a",
			"A",
			"i",
			"I"
		};

		private static Property[] defaultHyperlinkProperties = new Property[]
		{
			new Property(PropertyId.FontColor, new PropertyValue(new RGBT(0U, 0U, 255U)))
		};

		private HtmlWriter writer;

		private HtmlInjection injection;

		private bool filterHtml;

		private HtmlTagCallback callback;

		private HtmlFormatOutputCallbackContext callbackContext;

		private bool outputFragment;

		private bool recognizeHyperlinks;

		private int hyperlinkLevel;

		private HtmlFormatOutput.EndTagActionEntry[] endTagActionStack;

		private int endTagActionStackTop;

		private struct EndTagActionEntry
		{
			public int TagLevel;

			public bool Drop;

			public bool Callback;
		}
	}
}
