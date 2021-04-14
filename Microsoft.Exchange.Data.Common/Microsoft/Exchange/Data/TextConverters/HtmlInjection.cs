using System;
using System.IO;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;
using Microsoft.Exchange.Data.TextConverters.Internal.Rtf;
using Microsoft.Exchange.Data.TextConverters.Internal.Text;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class HtmlInjection : Injection
	{
		public HtmlInjection(string injectHead, string injectTail, HeaderFooterFormat injectionFormat, bool filterHtml, HtmlTagCallback callback, bool testBoundaryConditions, Stream traceStream, IProgressMonitor progressMonitor)
		{
			this.injectHead = injectHead;
			this.injectTail = injectTail;
			this.injectionFormat = injectionFormat;
			this.filterHtml = filterHtml;
			this.callback = callback;
			this.testBoundaryConditions = testBoundaryConditions;
			this.progressMonitor = progressMonitor;
		}

		public bool Active
		{
			get
			{
				return this.documentParser != null;
			}
		}

		public bool InjectingHead
		{
			get
			{
				return this.injectingHead;
			}
		}

		public IHtmlParser Push(bool head, IHtmlParser documentParser)
		{
			if (head)
			{
				if (this.injectHead != null && !this.headInjected)
				{
					this.documentParser = documentParser;
					if (this.fragmentParser == null)
					{
						this.fragmentParser = new HtmlParser(new ConverterBufferInput(this.injectHead, this.progressMonitor), false, this.injectionFormat == HeaderFooterFormat.Text, 64, 8, this.testBoundaryConditions);
					}
					else
					{
						this.fragmentParser.Initialize(this.injectHead, this.injectionFormat == HeaderFooterFormat.Text);
					}
					this.injectingHead = true;
					return this.fragmentParser;
				}
			}
			else
			{
				if (this.injectHead != null && !this.headInjected)
				{
					this.headInjected = true;
				}
				if (this.injectTail != null && !this.tailInjected)
				{
					this.documentParser = documentParser;
					if (this.fragmentParser == null)
					{
						this.fragmentParser = new HtmlParser(new ConverterBufferInput(this.injectTail, this.progressMonitor), false, this.injectionFormat == HeaderFooterFormat.Text, 64, 8, this.testBoundaryConditions);
					}
					else
					{
						this.fragmentParser.Initialize(this.injectTail, this.injectionFormat == HeaderFooterFormat.Text);
					}
					this.injectingHead = false;
					return this.fragmentParser;
				}
			}
			return documentParser;
		}

		public IHtmlParser Pop()
		{
			if (this.injectingHead)
			{
				this.headInjected = true;
				if (this.injectTail == null)
				{
					((IDisposable)this.fragmentParser).Dispose();
					this.fragmentParser = null;
				}
			}
			else
			{
				this.tailInjected = true;
				((IDisposable)this.fragmentParser).Dispose();
				this.fragmentParser = null;
			}
			IHtmlParser result = this.documentParser;
			this.documentParser = null;
			return result;
		}

		public void Inject(bool head, HtmlWriter writer)
		{
			if (head)
			{
				if (this.injectHead != null && !this.headInjected)
				{
					if (this.injectionFormat == HeaderFooterFormat.Text)
					{
						writer.WriteStartTag(HtmlNameIndex.TT);
						writer.WriteStartTag(HtmlNameIndex.Pre);
						writer.WriteNewLine();
					}
					this.CreateHtmlToHtmlConverter(this.injectHead, writer);
					while (!this.fragmentToHtmlConverter.Flush())
					{
					}
					this.headInjected = true;
					if (this.injectTail == null)
					{
						((IDisposable)this.fragmentToHtmlConverter).Dispose();
						this.fragmentToHtmlConverter = null;
					}
					if (this.injectionFormat == HeaderFooterFormat.Text)
					{
						writer.WriteEndTag(HtmlNameIndex.Pre);
						writer.WriteEndTag(HtmlNameIndex.TT);
						return;
					}
				}
			}
			else
			{
				if (this.injectHead != null && !this.headInjected)
				{
					this.headInjected = true;
				}
				if (this.injectTail != null && !this.tailInjected)
				{
					if (this.injectionFormat == HeaderFooterFormat.Text)
					{
						writer.WriteStartTag(HtmlNameIndex.TT);
						writer.WriteStartTag(HtmlNameIndex.Pre);
						writer.WriteNewLine();
					}
					if (this.fragmentToHtmlConverter == null)
					{
						this.CreateHtmlToHtmlConverter(this.injectTail, writer);
					}
					else
					{
						this.fragmentToHtmlConverter.Initialize(this.injectTail, this.injectionFormat == HeaderFooterFormat.Text);
					}
					while (!this.fragmentToHtmlConverter.Flush())
					{
					}
					((IDisposable)this.fragmentToHtmlConverter).Dispose();
					this.fragmentToHtmlConverter = null;
					this.tailInjected = true;
					if (this.injectionFormat == HeaderFooterFormat.Text)
					{
						writer.WriteEndTag(HtmlNameIndex.Pre);
						writer.WriteEndTag(HtmlNameIndex.TT);
					}
				}
			}
		}

		private void CreateHtmlToHtmlConverter(string fragment, HtmlWriter writer)
		{
			HtmlParser htmlParser = new HtmlParser(new ConverterBufferInput(fragment, this.progressMonitor), false, this.injectionFormat == HeaderFooterFormat.Text, 64, 8, this.testBoundaryConditions);
			IHtmlParser parser = htmlParser;
			if (this.injectionFormat == HeaderFooterFormat.Html)
			{
				parser = new HtmlNormalizingParser(htmlParser, null, false, 4096, this.testBoundaryConditions, null, true, 0);
			}
			this.fragmentToHtmlConverter = new HtmlToHtmlConverter(parser, writer, true, this.injectionFormat == HeaderFooterFormat.Html, this.filterHtml, this.callback, true, false, this.traceStream, true, 0, -1, false, this.progressMonitor);
		}

		public override void Inject(bool head, TextOutput output)
		{
			if (head)
			{
				if (this.injectHead != null && !this.headInjected)
				{
					HtmlParser parser = new HtmlParser(new ConverterBufferInput(this.injectHead, this.progressMonitor), false, this.injectionFormat == HeaderFooterFormat.Text, 64, 8, this.testBoundaryConditions);
					this.fragmentToTextConverter = new HtmlToTextConverter(parser, output, null, true, this.injectionFormat == HeaderFooterFormat.Text, false, this.traceStream, true, 0, false, true, true);
					while (!this.fragmentToTextConverter.Flush())
					{
					}
					this.headInjected = true;
					if (this.injectTail == null)
					{
						((IDisposable)this.fragmentToTextConverter).Dispose();
						this.fragmentToTextConverter = null;
						return;
					}
				}
			}
			else
			{
				if (this.injectHead != null && !this.headInjected)
				{
					this.headInjected = true;
				}
				if (this.injectTail != null && !this.tailInjected)
				{
					if (this.fragmentToTextConverter == null)
					{
						HtmlParser parser = new HtmlParser(new ConverterBufferInput(this.injectTail, this.progressMonitor), false, this.injectionFormat == HeaderFooterFormat.Text, 64, 8, this.testBoundaryConditions);
						this.fragmentToTextConverter = new HtmlToTextConverter(parser, output, null, true, this.injectionFormat == HeaderFooterFormat.Text, false, this.traceStream, true, 0, false, true, true);
					}
					else
					{
						this.fragmentToTextConverter.Initialize(this.injectTail, this.injectionFormat == HeaderFooterFormat.Text);
					}
					while (!this.fragmentToTextConverter.Flush())
					{
					}
					((IDisposable)this.fragmentToTextConverter).Dispose();
					this.fragmentToTextConverter = null;
					this.tailInjected = true;
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (this.fragmentToHtmlConverter != null)
			{
				((IDisposable)this.fragmentToHtmlConverter).Dispose();
				this.fragmentToHtmlConverter = null;
			}
			if (this.fragmentToTextConverter != null)
			{
				((IDisposable)this.fragmentToTextConverter).Dispose();
				this.fragmentToTextConverter = null;
			}
			if (this.fragmentParser != null)
			{
				((IDisposable)this.fragmentParser).Dispose();
				this.fragmentParser = null;
			}
			this.Reset();
			base.Dispose(disposing);
		}

		public override void CompileForRtf(RtfOutput output)
		{
			if (this.fragmentToRtfConverter == null)
			{
				if (this.injectHead != null)
				{
					this.fragmentToRtfConverter = new HtmlFragmentToRtfConverter(output, this.traceStream, this.progressMonitor);
					this.fragmentToRtfConverter.PrepareHead(this.injectHead);
				}
				if (this.injectTail != null)
				{
					if (this.fragmentToRtfConverter == null)
					{
						this.fragmentToRtfConverter = new HtmlFragmentToRtfConverter(output, this.traceStream, this.progressMonitor);
					}
					this.fragmentToRtfConverter.PrepareTail(this.injectTail);
				}
			}
			this.fragmentToRtfConverter.EndPrepare();
		}

		public override void InjectRtfFonts(int firstAvailableFontHandle)
		{
			if (this.fragmentToRtfConverter != null)
			{
				this.fragmentToRtfConverter.InjectFonts(firstAvailableFontHandle);
			}
		}

		public override void InjectRtfColors(int nextColorIndex)
		{
			if (this.fragmentToRtfConverter != null)
			{
				this.fragmentToRtfConverter.InjectColors(nextColorIndex);
			}
		}

		public override void InjectRtf(bool head, bool immediatelyAfterText)
		{
			if (head)
			{
				if (this.injectHead != null && !this.headInjected)
				{
					this.fragmentToRtfConverter.InjectHead();
					this.headInjected = true;
					return;
				}
			}
			else
			{
				if (this.injectHead != null && !this.headInjected)
				{
					this.fragmentToRtfConverter.InjectHead();
					this.headInjected = true;
				}
				if (this.injectTail != null && !this.tailInjected)
				{
					this.fragmentToRtfConverter.InjectTail(immediatelyAfterText);
					this.tailInjected = true;
				}
			}
		}

		protected bool filterHtml;

		protected HtmlTagCallback callback;

		protected bool injectingHead;

		protected IProgressMonitor progressMonitor;

		protected IHtmlParser documentParser;

		protected HtmlParser fragmentParser;

		protected HtmlToHtmlConverter fragmentToHtmlConverter;

		protected HtmlToTextConverter fragmentToTextConverter;

		private HtmlFragmentToRtfConverter fragmentToRtfConverter;
	}
}
