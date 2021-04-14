using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class IcalSquigglieFixHtmlReader : TextReader
	{
		internal IcalSquigglieFixHtmlReader(Stream bodyStream, Charset charset, bool trustMetaTag)
		{
			IcalSquigglieFixHtmlReader <>4__this = this;
			ConvertUtils.CallCts(ExTraceGlobals.CcBodyTracer, "IcalSquigglieFixHtmlReader::Constructor", ServerStrings.ConversionBodyConversionFailed, delegate
			{
				Encoding unicodeEncoding = ConvertUtils.UnicodeEncoding;
				HtmlToHtml htmlToHtml = new HtmlToHtml();
				htmlToHtml.InputEncoding = charset.GetEncoding();
				htmlToHtml.OutputEncoding = unicodeEncoding;
				htmlToHtml.DetectEncodingFromMetaTag = trustMetaTag;
				<>4__this.htmlReader = new ConverterReader(bodyStream, htmlToHtml);
			});
		}

		public override int Peek()
		{
			if (this.IsSquigglieFound() && !this.IsSquigglieInsertWrittenOut())
			{
				return (int)"<BR>"[this.squigglieInsertPosition];
			}
			return this.htmlReader.Peek();
		}

		public override int Read()
		{
			if (this.IsSquigglieFound() && !this.IsSquigglieInsertWrittenOut())
			{
				return (int)"<BR>"[this.squigglieInsertPosition++];
			}
			int num = this.htmlReader.Read();
			if (num != -1 && !this.IsSquigglieFound())
			{
				int num2 = this.squigglieTextPosition;
				while (num2 != -1 && num != (int)"*~*~*~*~*~*~*~*~*~*"[num2])
				{
					num2 = IcalSquigglieFixHtmlReader.squigglieRollback[num2];
				}
				this.squigglieTextPosition = num2 + 1;
			}
			if (num != 126 && num != 42)
			{
				this.lastCharacterRead = num;
			}
			return num;
		}

		public override int ReadBlock(char[] buffer, int index, int count)
		{
			int num;
			for (num = 0; num != count; num++)
			{
				if (this.IsSquigglieFound() && this.IsSquigglieInsertWrittenOut())
				{
					return num + this.htmlReader.ReadBlock(buffer, index + num, count - num);
				}
				int num2 = this.Read();
				if (num2 == -1)
				{
					break;
				}
				buffer[index + num] = (char)num2;
			}
			return num;
		}

		private bool IsSquigglieFound()
		{
			if (this.squigglieTextPosition == "*~*~*~*~*~*~*~*~*~*".Length)
			{
				if (this.lastCharacterRead != 32)
				{
					return true;
				}
				this.squigglieTextPosition = 0;
			}
			return false;
		}

		private bool IsSquigglieInsertWrittenOut()
		{
			return this.squigglieInsertPosition == "<BR>".Length;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.htmlReader != null)
			{
				this.htmlReader.Dispose();
			}
			base.Dispose(disposing);
		}

		private const string SquigglieInsert = "<BR>";

		private const string SquigglieText = "*~*~*~*~*~*~*~*~*~*";

		private static int[] squigglieRollback = new int[]
		{
			-1,
			0,
			-1,
			0,
			-1,
			0,
			-1,
			0,
			-1,
			0,
			-1,
			0,
			-1,
			0,
			-1,
			0,
			-1,
			0,
			-1
		};

		private int squigglieTextPosition;

		private int squigglieInsertPosition;

		private int lastCharacterRead;

		private ConverterReader htmlReader;
	}
}
