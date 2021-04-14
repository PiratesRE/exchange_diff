using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters.Internal.Rtf;
using Microsoft.Exchange.Data.TextConverters.Internal.Text;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class TextInjection : Injection
	{
		public TextInjection(string injectHead, string injectTail, bool testBoundaryConditions, Stream traceStream, IProgressMonitor progressMonitor)
		{
			this.injectHead = injectHead;
			this.injectTail = injectTail;
			this.injectionFormat = HeaderFooterFormat.Text;
			this.testBoundaryConditions = testBoundaryConditions;
			this.traceStream = traceStream;
			this.progressMonitor = progressMonitor;
		}

		public override void Inject(bool head, TextOutput output)
		{
			if (head)
			{
				if (this.injectHead != null && !this.headInjected)
				{
					if (this.fragmentToTextConverter == null)
					{
						TextParser parser = new TextParser(new ConverterBufferInput(this.injectHead, this.progressMonitor), false, false, 64, this.testBoundaryConditions);
						this.fragmentToTextConverter = new TextToTextConverter(parser, output, null, true, false, this.traceStream, true, 0);
					}
					else
					{
						this.fragmentToTextConverter.Initialize(this.injectHead);
					}
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
						TextParser parser = new TextParser(new ConverterBufferInput(this.injectTail, this.progressMonitor), false, false, 64, this.testBoundaryConditions);
						this.fragmentToTextConverter = new TextToTextConverter(parser, output, null, true, false, this.traceStream, true, 0);
					}
					else
					{
						this.fragmentToTextConverter.Initialize(this.injectTail);
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
			if (disposing)
			{
				if (this.fragmentToTextConverter != null)
				{
					((IDisposable)this.fragmentToTextConverter).Dispose();
					this.fragmentToTextConverter = null;
				}
				if (this.fragmentToRtfConverter != null)
				{
					((IDisposable)this.fragmentToRtfConverter).Dispose();
					this.fragmentToRtfConverter = null;
				}
			}
			this.Reset();
			base.Dispose(disposing);
		}

		public override void CompileForRtf(RtfOutput output)
		{
			this.rtfOutput = output;
		}

		public override void InjectRtf(bool head, bool immediatelyAfterText)
		{
			if (!this.fontsWritten)
			{
				this.rtfOutput.WriteControlText("{\\fonttbl", false);
				this.InjectRtfFonts(1);
				this.rtfOutput.WriteControlText("}", false);
			}
			if (!this.colorsWritten)
			{
				this.rtfOutput.WriteControlText("{\\colortbl;", false);
				this.InjectRtfColors(1);
				this.rtfOutput.WriteControlText("}", false);
			}
			if (head)
			{
				if (this.injectHead != null && !this.headInjected)
				{
					this.rtfOutput.WriteControlText("\\pard\\plain", true);
					this.rtfOutput.WriteKeyword("\\cf", 0);
					this.rtfOutput.WriteKeyword("\\f", this.firstAvailableFontHandle);
					this.rtfOutput.WriteControlText("\\fs24", true);
					if (this.fragmentToRtfConverter == null)
					{
						TextParser parser = new TextParser(new ConverterBufferInput(this.injectHead, this.progressMonitor), false, false, 64, this.testBoundaryConditions);
						this.fragmentToRtfConverter = new TextToRtfFragmentConverter(parser, this.rtfOutput, this.traceStream, false, 0);
					}
					else
					{
						this.fragmentToRtfConverter.Initialize(this.injectHead);
					}
					while (!this.fragmentToRtfConverter.Flush())
					{
					}
					this.headInjected = true;
					this.rtfOutput.WriteControlText("\\par\r\n", false);
					this.rtfOutput.WriteControlText("\\plain\r\n", false);
					this.rtfOutput.RtfLineLength = 0;
					if (this.injectTail == null)
					{
						((IDisposable)this.fragmentToRtfConverter).Dispose();
						this.fragmentToRtfConverter = null;
						return;
					}
				}
			}
			else
			{
				if (this.injectHead != null && !this.headInjected)
				{
					this.InjectRtf(true, false);
					this.headInjected = true;
				}
				if (this.injectTail != null && !this.tailInjected)
				{
					if (immediatelyAfterText)
					{
						this.rtfOutput.WriteControlText("\\par\r\n", true);
					}
					this.rtfOutput.WriteControlText("\\pard\\plain", true);
					this.rtfOutput.WriteKeyword("\\cf", 0);
					this.rtfOutput.WriteKeyword("\\f", this.firstAvailableFontHandle);
					this.rtfOutput.WriteControlText("\\fs24", true);
					if (this.fragmentToRtfConverter == null)
					{
						TextParser parser = new TextParser(new ConverterBufferInput(this.injectTail, this.progressMonitor), false, false, 64, this.testBoundaryConditions);
						this.fragmentToRtfConverter = new TextToRtfFragmentConverter(parser, this.rtfOutput, this.traceStream, false, 0);
					}
					else
					{
						this.fragmentToRtfConverter.Initialize(this.injectTail);
					}
					while (!this.fragmentToRtfConverter.Flush())
					{
					}
					this.tailInjected = true;
					((IDisposable)this.fragmentToRtfConverter).Dispose();
					this.fragmentToRtfConverter = null;
				}
			}
		}

		public override void InjectRtfFonts(int firstAvailableFontHandle)
		{
			SimpleCodepageDetector simpleCodepageDetector = default(SimpleCodepageDetector);
			if (this.injectHead != null)
			{
				simpleCodepageDetector.AddData(this.injectHead, 0, this.injectHead.Length);
			}
			if (this.injectTail != null)
			{
				simpleCodepageDetector.AddData(this.injectTail, 0, this.injectTail.Length);
			}
			int windowsCodePage = simpleCodepageDetector.GetWindowsCodePage(null, true);
			Encoding encoding;
			if (!Charset.TryGetEncoding(windowsCodePage, out encoding))
			{
				encoding = Charset.DefaultWindowsCharset.GetEncoding();
			}
			this.rtfOutput.SetEncoding(encoding);
			int value = RtfSupport.CharSetFromCodePage((ushort)windowsCodePage);
			this.firstAvailableFontHandle = firstAvailableFontHandle;
			this.rtfOutput.WriteControlText("{", false);
			this.rtfOutput.WriteKeyword("\\f", firstAvailableFontHandle);
			this.rtfOutput.WriteControlText("\\fmodern", true);
			this.rtfOutput.WriteKeyword("\\fcharset", value);
			this.rtfOutput.WriteControlText(" Courier New;}", false);
			this.fontsWritten = true;
		}

		public override void InjectRtfColors(int nextColorIndex)
		{
			this.rtfOutput.WriteControlText("\\red0\\green0\\blue0;", false);
			this.colorsWritten = true;
		}

		private const int NextColorIndex = 0;

		protected TextToTextConverter fragmentToTextConverter;

		protected IProgressMonitor progressMonitor;

		private int firstAvailableFontHandle;

		private RtfOutput rtfOutput;

		private TextToRtfFragmentConverter fragmentToRtfConverter;

		private bool fontsWritten;

		private bool colorsWritten;
	}
}
