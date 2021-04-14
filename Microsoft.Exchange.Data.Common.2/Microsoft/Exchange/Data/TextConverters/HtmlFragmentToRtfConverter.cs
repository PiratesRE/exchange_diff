using System;
using System.IO;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;
using Microsoft.Exchange.Data.TextConverters.Internal.Rtf;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class HtmlFragmentToRtfConverter
	{
		public HtmlFragmentToRtfConverter(RtfOutput output, Stream traceStream, IProgressMonitor progressMonitor)
		{
			HtmlParser parser = new HtmlParser(new ConverterBufferInput(string.Empty, progressMonitor), false, false, 64, 8, false);
			HtmlNormalizingParser parser2 = new HtmlNormalizingParser(parser, null, false, 4096, false, null, true, 0);
			this.formatStore = new FormatStore();
			this.formatStore.InitializeCodepageDetector();
			this.converter = new HtmlFormatConverter(parser2, this.formatStore, true, false, traceStream, true, 0, null, progressMonitor);
			this.output = output;
		}

		public void PrepareHead(string headHtml)
		{
			this.headFragmentNode = this.converter.Initialize(headHtml);
			this.converter.ConvertToStore();
		}

		public void PrepareTail(string tailHtml)
		{
			this.tailFragmentNode = this.converter.Initialize(tailHtml);
			this.converter.ConvertToStore();
		}

		public void EndPrepare()
		{
			this.converter = null;
			this.formatOutput = new RtfFormatOutput(this.output, null, null);
			this.formatOutput.Initialize(this.formatStore, SourceFormat.Html, null);
		}

		public void InjectFonts(int firstAvailableFontHandle)
		{
			this.formatOutput.OutputFonts(firstAvailableFontHandle);
			this.fontsWritten = true;
		}

		public void InjectColors(int nextColorIndex)
		{
			this.formatOutput.OutputColors(nextColorIndex);
			this.colorsWritten = true;
		}

		public void InjectHead()
		{
			if (!this.fontsWritten)
			{
				this.output.WriteControlText("{\\fonttbl", false);
				this.InjectFonts(1);
				this.output.WriteControlText("}", false);
			}
			if (!this.colorsWritten)
			{
				this.output.WriteControlText("{\\colortbl;", false);
				this.InjectColors(1);
				this.output.WriteControlText("}", false);
			}
			this.InjectFragment(this.headFragmentNode);
			this.output.WriteControlText("\\pard\\par\r\n", false);
			this.output.RtfLineLength = 0;
		}

		public void InjectTail(bool immediatelyAfterText)
		{
			if (!this.fontsWritten)
			{
				this.output.WriteControlText("{\\fonttbl", false);
				this.InjectFonts(1);
				this.output.WriteControlText("}", false);
			}
			if (!this.colorsWritten)
			{
				this.output.WriteControlText("{\\colortbl;", false);
				this.InjectColors(1);
				this.output.WriteControlText("}", false);
			}
			if (immediatelyAfterText)
			{
				this.output.WriteControlText("\\pard\\par\r\n", true);
			}
			this.output.WriteControlText("\\pard\\plain", true);
			this.InjectFragment(this.tailFragmentNode);
		}

		private void InjectFragment(FormatNode fragmentNode)
		{
			this.formatOutput.OutputFragment(fragmentNode);
		}

		private FormatStore formatStore;

		private FormatNode headFragmentNode;

		private FormatNode tailFragmentNode;

		private HtmlFormatConverter converter;

		private RtfFormatOutput formatOutput;

		private RtfOutput output;

		private bool colorsWritten;

		private bool fontsWritten;
	}
}
