using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Common.Sniff
{
	public sealed class DataSniff
	{
		public DataSniff(int sampleSize)
		{
			this.sampleSize = ((sampleSize <= 256) ? sampleSize : 256);
			this.guessedMimeType = "application/octet-stream";
		}

		public string FindMimeFromData(Stream file)
		{
			if (file == null || this.sampleSize <= 0)
			{
				return this.guessedMimeType;
			}
			this.ReadBuffer(file);
			this.SampleData();
			if (this.sampleSize <= 0)
			{
				return this.guessedMimeType;
			}
			if (!this.FoundMimeType())
			{
				if (this.countCtrl > 0 || this.countText + this.countFF >= 16 * (this.countCtrl + this.countHigh))
				{
					if (!this.CheckTextHeaders() && !this.CheckBinaryHeaders())
					{
						this.guessedMimeType = "text/plain";
					}
				}
				else if (!this.CheckBinaryHeaders() && !this.CheckTextHeaders())
				{
					this.guessedMimeType = "application/octet-stream";
				}
			}
			return this.guessedMimeType;
		}

		public DataSniff.TypeOfStream TypeOfSniffedStream
		{
			get
			{
				return this.typeOfStream;
			}
		}

		private bool FoundMimeType()
		{
			bool result = false;
			if (this.foundCDF)
			{
				this.guessedMimeType = "application/x-cdf";
				result = true;
			}
			else if (this.foundXML)
			{
				this.guessedMimeType = "text/xml";
				result = true;
			}
			else if (this.foundHTML)
			{
				this.guessedMimeType = "text/html";
				result = true;
			}
			else if (this.foundXBitMap)
			{
				this.guessedMimeType = "image/x-xbitmap";
				result = true;
			}
			else if (this.foundMacBinhex)
			{
				this.guessedMimeType = "application/macbinhex40";
				result = true;
			}
			else if (this.foundTextScriptlet)
			{
				this.guessedMimeType = "text/scriptlet";
				result = true;
			}
			return result;
		}

		private void ReadBuffer(Stream stream)
		{
			bool flag = false;
			this.buffer = new byte[this.sampleSize];
			int num = stream.Read(this.buffer, 0, this.sampleSize);
			if (num >= 2)
			{
				if (this.buffer[0] == 255 && this.buffer[1] == 254)
				{
					this.typeOfStream = DataSniff.TypeOfStream.UCS16LittleEndian;
				}
				else if (this.buffer[0] == 254 && this.buffer[1] == 255)
				{
					this.typeOfStream = DataSniff.TypeOfStream.UCS16BigEndian;
				}
			}
			if (num == this.sampleSize && this.typeOfStream != DataSniff.TypeOfStream.Default)
			{
				byte[] array = new byte[this.sampleSize];
				int num2 = stream.Read(array, 0, this.sampleSize);
				if (num2 > 0 && num2 % 2 == 0)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.sampleSize = num;
				return;
			}
			Encoding srcEncoding = Encoding.Unicode;
			if (this.typeOfStream == DataSniff.TypeOfStream.UCS16BigEndian)
			{
				srcEncoding = Encoding.BigEndianUnicode;
			}
			Encoding ascii = Encoding.ASCII;
			byte[] array2 = Encoding.Convert(srcEncoding, ascii, this.buffer, 0, num);
			this.sampleSize = ((array2.Length > this.sampleSize) ? this.sampleSize : array2.Length);
			this.buffer = new byte[this.sampleSize];
			Array.Copy(array2, this.buffer, this.sampleSize);
		}

		private void SampleData()
		{
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			this.countNL = 0;
			this.countCR = 0;
			this.countFF = 0;
			this.countText = 0;
			this.countCtrl = 0;
			this.countHigh = 0;
			for (int i = 0; i < this.sampleSize - 1; i++)
			{
				byte b = this.buffer[i];
				bool flag3 = false;
				if (b == DataSniff.NLChar.Value)
				{
					this.countNL++;
				}
				else if (b == DataSniff.CRChar.Value)
				{
					this.countCR++;
				}
				else if (b == DataSniff.FFChar.Value)
				{
					this.countFF++;
				}
				else if (b == DataSniff.TabChar.Value)
				{
					this.countText++;
				}
				else if (b < 32)
				{
					this.countCtrl++;
				}
				else if (b >= 32 && b < 128)
				{
					this.countText++;
					flag3 = true;
				}
				else
				{
					this.countHigh++;
				}
				if (flag3)
				{
					if (b == DataSniff.AnchorChar.Value)
					{
						if (i + 5 < this.buffer.Length && DataSniff.AsciiString.EqualsNCI(this.buffer, i + 1, DataSniff.XMLStr) && (this.buffer[i + 5] == DataSniff.ColonChar.Value || this.buffer[i + 5] == DataSniff.SpaceChar.Value || this.buffer[i + 5] == DataSniff.TabChar.Value))
						{
							this.foundXML = true;
						}
						if (DataSniff.AsciiString.EqualsNCI(this.buffer, i + 1, DataSniff.ScripletStr))
						{
							this.foundTextScriptlet = true;
							return;
						}
						if (this.IsHtml(i, ref num))
						{
							return;
						}
						if (DataSniff.AsciiString.EqualsNCI(this.buffer, i + 1, DataSniff.ChannelStr))
						{
							this.foundCDF = true;
							return;
						}
					}
					else if (DataSniff.AsciiString.EqualsNCI(this.buffer, i, DataSniff.ChannelStr))
					{
						num += 50;
						if (num >= 100 && i == this.sampleSize - 1 && this.countText > this.sampleSize * 2 / 3)
						{
							this.foundHTML = true;
							return;
						}
					}
					else if (b == DataSniff.SharpChar.Value)
					{
						if (DataSniff.AsciiString.EqualsNC(this.buffer, i + 1, DataSniff.FileMagic.XbmMagic1))
						{
							flag = true;
						}
					}
					else if (b == DataSniff.UnderScoreChar.Value && flag2)
					{
						if (DataSniff.AsciiString.EqualsNC(this.buffer, i + 1, DataSniff.FileMagic.XbmMagic3))
						{
							this.foundXBitMap = true;
							return;
						}
					}
					else if (b == DataSniff.UnderScoreChar.Value && flag)
					{
						if (DataSniff.AsciiString.EqualsNC(this.buffer, i + 1, DataSniff.FileMagic.XbmMagic2))
						{
							flag2 = true;
						}
					}
					else if (b == DataSniff.CChar.Value && DataSniff.AsciiString.EqualsNC(this.buffer, i + 1, DataSniff.FileMagic.BinHexMagic))
					{
						this.foundMacBinhex = true;
						return;
					}
				}
			}
		}

		private bool IsHtml(int index, ref int htmlConfidence)
		{
			if (DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.HTMLStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.HeadStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.TitleStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.BodyStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.ScriptStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.AHRefStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.PreStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.ImgStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.AHRefStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.PlainTextStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.TableStr))
			{
				this.foundHTML = true;
			}
			else if (DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.HrStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.AStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.SlashAStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.BStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.SlashBStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.PStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.SlashPStr) || DataSniff.AsciiString.EqualsNCI(this.buffer, index + 1, DataSniff.CommentStr))
			{
				htmlConfidence += 50;
				if (htmlConfidence >= 100 && index == this.sampleSize - 1 && this.countText >= this.sampleSize * 2 / 3)
				{
					this.foundHTML = true;
				}
			}
			return this.foundHTML;
		}

		private bool IsBMP()
		{
			bool result = true;
			if (this.sampleSize < 2)
			{
				result = false;
			}
			else if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.BmpMagic))
			{
				result = false;
			}
			else if (this.sampleSize < 14)
			{
				result = false;
			}
			else if (this.buffer[6] != 0 || this.buffer[7] != 0 || this.buffer[8] != 0 || this.buffer[9] != 0)
			{
				result = false;
			}
			return result;
		}

		private bool MatchDWordAtOffset(uint magic, int offset)
		{
			bool result = true;
			if (this.sampleSize < offset + 4)
			{
				return false;
			}
			int num = (int)this.buffer[offset] << 24 | (int)this.buffer[offset + 1] << 16 | (int)this.buffer[offset + 2] << 8 | (int)this.buffer[offset + 3];
			if ((ulong)magic != (ulong)((long)num))
			{
				result = false;
			}
			return result;
		}

		private bool CheckTextHeaders()
		{
			bool result = true;
			if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.PdfMagic))
			{
				this.guessedMimeType = "application/pdf";
			}
			else if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.PostscriptMagic))
			{
				this.guessedMimeType = "application/postscript";
			}
			else if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.RichTextMagic))
			{
				this.guessedMimeType = "text/richtext";
			}
			else if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.Base64Magic))
			{
				this.guessedMimeType = "application/base64";
			}
			else
			{
				result = false;
			}
			return result;
		}

		private bool CheckBinaryHeaders()
		{
			bool result = true;
			if (DataSniff.AsciiString.EqualsNCI(this.buffer, 0, DataSniff.FileMagic.Gif87Magic) || DataSniff.AsciiString.EqualsNCI(this.buffer, 0, DataSniff.FileMagic.Gif89Magic))
			{
				this.guessedMimeType = "image/gif";
			}
			else if (this.buffer[0] == 255 && this.buffer[1] == 216)
			{
				this.guessedMimeType = "image/pjpeg";
			}
			else if (this.IsBMP())
			{
				this.guessedMimeType = "image/bmp";
			}
			else if (this.MatchDWordAtOffset(1380533830U, 0) && this.MatchDWordAtOffset(1463899717U, 8))
			{
				this.guessedMimeType = "audio/wav";
			}
			else if (this.MatchDWordAtOffset(779314176U, 0) || this.MatchDWordAtOffset(779316836U, 0) || this.MatchDWordAtOffset(6583086U, 0) || this.MatchDWordAtOffset(1684960046U, 0))
			{
				this.guessedMimeType = "audio/basic";
			}
			else if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.TiffMagic) && this.sampleSize >= 3 && this.buffer[2] == 0)
			{
				this.guessedMimeType = "image/tiff";
			}
			else if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.ExeMagic))
			{
				this.guessedMimeType = "application/x-msdownload";
			}
			else if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.PngMagic))
			{
				this.guessedMimeType = "image/x-png";
			}
			else if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.JGMagic) && this.sampleSize >= 3 && this.buffer[2] >= 3 && this.buffer[2] <= 31 && this.sampleSize >= 5 && this.buffer[4] == 0)
			{
				this.guessedMimeType = "image/x-jg";
			}
			else if (this.MatchDWordAtOffset(1297239878U, 0))
			{
				this.guessedMimeType = "audio/x-aiff";
			}
			else if (this.MatchDWordAtOffset(1179603533U, 0) && (this.MatchDWordAtOffset(1095321158U, 8) || this.MatchDWordAtOffset(1095321155U, 8)))
			{
				this.guessedMimeType = "audio/x-aiff";
			}
			else if (this.MatchDWordAtOffset(1380533830U, 0) && this.MatchDWordAtOffset(1096173856U, 8))
			{
				this.guessedMimeType = "video/avi";
			}
			else if (this.MatchDWordAtOffset(435U, 0) || this.MatchDWordAtOffset(442U, 0))
			{
				this.guessedMimeType = "video/mpeg";
			}
			else if (this.MatchDWordAtOffset(16777216U, 0) && this.MatchDWordAtOffset(541412678U, 40))
			{
				this.guessedMimeType = "image/x-emf";
			}
			else if (this.MatchDWordAtOffset(3620587162U, 0))
			{
				this.guessedMimeType = "image/x-wmf";
			}
			else if (this.MatchDWordAtOffset(3405691582U, 0))
			{
				this.guessedMimeType = "application/java";
			}
			else if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.ZipMagic))
			{
				this.guessedMimeType = "application/x-zip-compressed";
			}
			else if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.CompressMagic))
			{
				this.guessedMimeType = "application/x-compressed";
			}
			else if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.GzipMagic))
			{
				this.guessedMimeType = "application/x-compressed";
			}
			else if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.MIDMagic) && this.buffer[4] == 0)
			{
				this.guessedMimeType = "audio/mid";
			}
			else if (DataSniff.AsciiString.EqualsNC(this.buffer, 0, DataSniff.FileMagic.PdfMagic))
			{
				this.guessedMimeType = "application/pdf";
			}
			else
			{
				result = false;
			}
			return result;
		}

		private const int MaxSampleSize = 256;

		private static readonly DataSniff.AsciiChar NLChar = new DataSniff.AsciiChar('\n');

		private static readonly DataSniff.AsciiChar CRChar = new DataSniff.AsciiChar('\r');

		private static readonly DataSniff.AsciiChar FFChar = new DataSniff.AsciiChar('\f');

		private static readonly DataSniff.AsciiChar TabChar = new DataSniff.AsciiChar('\t');

		private static readonly DataSniff.AsciiChar AnchorChar = new DataSniff.AsciiChar('<');

		private static readonly DataSniff.AsciiChar ColonChar = new DataSniff.AsciiChar(':');

		private static readonly DataSniff.AsciiChar SpaceChar = new DataSniff.AsciiChar(' ');

		private static readonly DataSniff.AsciiChar SharpChar = new DataSniff.AsciiChar('#');

		private static readonly DataSniff.AsciiChar UnderScoreChar = new DataSniff.AsciiChar('_');

		private static readonly DataSniff.AsciiChar CChar = new DataSniff.AsciiChar('c');

		private static readonly DataSniff.AsciiString XMLStr = new DataSniff.AsciiString("?XML");

		private static readonly DataSniff.AsciiString ScripletStr = new DataSniff.AsciiString("SCRIPTLET");

		private static readonly DataSniff.AsciiString HTMLStr = new DataSniff.AsciiString("HTML");

		private static readonly DataSniff.AsciiString HeadStr = new DataSniff.AsciiString("HEAD");

		private static readonly DataSniff.AsciiString TitleStr = new DataSniff.AsciiString("TITLE");

		private static readonly DataSniff.AsciiString BodyStr = new DataSniff.AsciiString("BODY");

		private static readonly DataSniff.AsciiString ScriptStr = new DataSniff.AsciiString("SCRIPT");

		private static readonly DataSniff.AsciiString AHRefStr = new DataSniff.AsciiString("A HREF");

		private static readonly DataSniff.AsciiString PreStr = new DataSniff.AsciiString("PRE");

		private static readonly DataSniff.AsciiString ImgStr = new DataSniff.AsciiString("IMG");

		private static readonly DataSniff.AsciiString PlainTextStr = new DataSniff.AsciiString("PLAINTEXT");

		private static readonly DataSniff.AsciiString TableStr = new DataSniff.AsciiString("TABLE");

		private static readonly DataSniff.AsciiString HrStr = new DataSniff.AsciiString("HR");

		private static readonly DataSniff.AsciiString AStr = new DataSniff.AsciiString("A");

		private static readonly DataSniff.AsciiString SlashAStr = new DataSniff.AsciiString("/A");

		private static readonly DataSniff.AsciiString BStr = new DataSniff.AsciiString("B");

		private static readonly DataSniff.AsciiString SlashBStr = new DataSniff.AsciiString("/B");

		private static readonly DataSniff.AsciiString PStr = new DataSniff.AsciiString("P");

		private static readonly DataSniff.AsciiString SlashPStr = new DataSniff.AsciiString("/P");

		private static readonly DataSniff.AsciiString CommentStr = new DataSniff.AsciiString("!--");

		private static readonly DataSniff.AsciiString ChannelStr = new DataSniff.AsciiString("CHANNEL");

		private DataSniff.TypeOfStream typeOfStream;

		private bool foundHTML;

		private bool foundXBitMap;

		private bool foundMacBinhex;

		private bool foundCDF;

		private bool foundTextScriptlet;

		private bool foundXML;

		private int countNL;

		private int countCR;

		private int countFF;

		private int countText;

		private int countCtrl;

		private int countHigh;

		private byte[] buffer;

		private int sampleSize;

		private string guessedMimeType;

		public enum TypeOfStream
		{
			Default,
			UCS16LittleEndian,
			UCS16BigEndian
		}

		private sealed class FileMagic
		{
			private FileMagic()
			{
			}

			public const uint AU_SUN_MAGIC = 779316836U;

			public const uint AU_SUN_INV_MAGIC = 1684960046U;

			public const uint AU_DEC_MAGIC = 779314176U;

			public const uint AU_DEC_INV_MAGIC = 6583086U;

			public const uint AIFF_MAGIC = 1179603533U;

			public const uint AIFF_INV_MAGIC = 1297239878U;

			public const uint AIFF_MAGIC_MORE_1 = 1095321158U;

			public const uint AIFF_MAGIC_MORE_2 = 1095321155U;

			public const uint RIFF_MAGIC = 1380533830U;

			public const uint AVI_MAGIC = 1096173856U;

			public const uint WAV_MAGIC = 1463899717U;

			public const uint JAVA_MAGIC = 3405691582U;

			public const uint MPEG_MAGIC = 435U;

			public const uint MPEG_MAGIC_2 = 442U;

			public const uint EMF_MAGIC_1 = 16777216U;

			public const uint EMF_MAGIC_2 = 541412678U;

			public const uint WMF_MAGIC = 3620587162U;

			public const uint JPEG_MAGIC_1 = 255U;

			public const uint JPEG_MAGIC_2 = 216U;

			public static readonly DataSniff.AsciiString RichTextMagic = new DataSniff.AsciiString("{\\rtf");

			public static readonly DataSniff.AsciiString PostscriptMagic = new DataSniff.AsciiString("%!");

			public static readonly DataSniff.AsciiString BinHexMagic = new DataSniff.AsciiString("onverted with BinHex");

			public static readonly DataSniff.AsciiString Base64Magic = new DataSniff.AsciiString("begin");

			public static readonly DataSniff.AsciiString Gif87Magic = new DataSniff.AsciiString("GIF87");

			public static readonly DataSniff.AsciiString Gif89Magic = new DataSniff.AsciiString("GIF89");

			public static readonly DataSniff.AsciiString TiffMagic = new DataSniff.AsciiString("MM");

			public static readonly DataSniff.AsciiString BmpMagic = new DataSniff.AsciiString("BM");

			public static readonly DataSniff.AsciiString ZipMagic = new DataSniff.AsciiString("PK");

			public static readonly DataSniff.AsciiString ExeMagic = new DataSniff.AsciiString("MZ");

			public static readonly DataSniff.AsciiString PngMagic = new DataSniff.AsciiString("\\211PNG\\r\\n\\032\\n");

			public static readonly DataSniff.AsciiString CompressMagic = new DataSniff.AsciiString("\\037\\235");

			public static readonly DataSniff.AsciiString GzipMagic = new DataSniff.AsciiString("\\037\\213");

			public static readonly DataSniff.AsciiString XbmMagic1 = new DataSniff.AsciiString("define");

			public static readonly DataSniff.AsciiString XbmMagic2 = new DataSniff.AsciiString("width");

			public static readonly DataSniff.AsciiString XbmMagic3 = new DataSniff.AsciiString("bits");

			public static readonly DataSniff.AsciiString PdfMagic = new DataSniff.AsciiString("%PDF");

			public static readonly DataSniff.AsciiString JGMagic = new DataSniff.AsciiString("JG");

			public static readonly DataSniff.AsciiString MIDMagic = new DataSniff.AsciiString("MThd");
		}

		public sealed class MimeType
		{
			private MimeType()
			{
			}

			public const string NULL = "(null)";

			public const string TextPlain = "text/plain";

			public const string TextRichText = "text/richtext";

			public const string ImageXBitmap = "image/x-xbitmap";

			public const string ApplicationPostscript = "application/postscript";

			public const string ApplicationBase64 = "application/base64";

			public const string ApplicationMacBinhex = "application/macbinhex40";

			public const string ApplicationPdf = "application/pdf";

			public const string ApplicationCDF = "application/x-cdf";

			public const string ApplicationNETCDF = "application/x-netcdf";

			public const string Multipartmixedreplace = "multipart/x-mixed-replace";

			public const string Multipartmixed = "multipart/mixed";

			public const string TextScriptlet = "text/scriptlet";

			public const string TextComponent = "text/x-component";

			public const string TextXML = "text/xml";

			public const string ApplicationHTA = "application/hta";

			public const string AudioAiff = "audio/x-aiff";

			public const string AudioBasic = "audio/basic";

			public const string AudioWav = "audio/wav";

			public const string AudioMID = "audio/mid";

			public const string ImageGif = "image/gif";

			public const string ImagePJpeg = "image/pjpeg";

			public const string ImageJpeg = "image/jpeg";

			public const string ImageTiff = "image/tiff";

			public const string ImagePng = "image/x-png";

			public const string ImageBmp = "image/bmp";

			public const string ImageJG = "image/x-jg";

			public const string ImageEmf = "image/x-emf";

			public const string ImageWmf = "image/x-wmf";

			public const string VideoAvi = "video/avi";

			public const string VideoMpeg = "video/mpeg";

			public const string ApplicationCompressed = "application/x-compressed";

			public const string ApplicationJava = "application/java";

			public const string ApplicationMSDownload = "application/x-msdownload";

			public const string ApplicationCommonDataFormat = "application/x-netcdf";

			public const string ApplicationZipCompressed = "application/x-zip-compressed";

			public const string ApplicationGzipCompressed = "application/x-gzip-compressed";

			public const string TextHTML = "text/html";

			public const string ApplicationOctetStream = "application/octet-stream";
		}

		private class AsciiString
		{
			public AsciiString(string str)
			{
				char[] array = str.ToCharArray();
				this.asciiValue = new byte[array.Length];
				DataSniff.AsciiString.encoder.GetBytes(array, 0, array.Length, this.asciiValue, 0);
			}

			public byte[] Value
			{
				get
				{
					return this.asciiValue;
				}
			}

			public int Length
			{
				get
				{
					return this.asciiValue.Length;
				}
			}

			public static bool EqualsNCI(byte[] str1, int start, DataSniff.AsciiString str2)
			{
				return DataSniff.AsciiString.EqualsNCI(str1, start, str2.Value, str2.Length);
			}

			private static bool EqualsNCI(byte[] str1, int start, byte[] str2, int count)
			{
				if (start < 0)
				{
					return false;
				}
				for (int num = 0; num != count; num++)
				{
					if (start + num >= str1.Length)
					{
						return num >= str2.Length;
					}
					if (num == str2.Length)
					{
						return false;
					}
					if (DataSniff.AsciiChar.ToLower(str1[start + num]) != DataSniff.AsciiChar.ToLower(str2[num]))
					{
						return false;
					}
				}
				return true;
			}

			public static bool EqualsNC(byte[] str1, int start, DataSniff.AsciiString str2)
			{
				return DataSniff.AsciiString.EqualsNC(str1, start, str2.Value, str2.Length);
			}

			private static bool EqualsNC(byte[] str1, int start, byte[] str2, int count)
			{
				if (start < 0)
				{
					return false;
				}
				for (int num = 0; num != count; num++)
				{
					if (start + num >= str1.Length)
					{
						return num >= str2.Length;
					}
					if (num == str2.Length)
					{
						return false;
					}
					if (str1[start + num] != str2[num])
					{
						return false;
					}
				}
				return true;
			}

			private static ASCIIEncoding encoder = new ASCIIEncoding();

			private byte[] asciiValue;
		}

		private class AsciiChar
		{
			public AsciiChar(char ch)
			{
				char[] array = new char[1];
				byte[] array2 = new byte[2];
				array[0] = ch;
				DataSniff.AsciiChar.encoder.GetBytes(array, 0, 1, array2, 0);
				this.asciiValue = array2[0];
			}

			public byte Value
			{
				get
				{
					return this.asciiValue;
				}
			}

			public static byte ToLower(byte ch)
			{
				return DataSniff.AsciiChar.LowerC[(int)ch];
			}

			internal static readonly byte[] LowerC = new byte[]
			{
				0,
				1,
				2,
				3,
				4,
				5,
				6,
				7,
				8,
				9,
				10,
				11,
				12,
				13,
				14,
				15,
				16,
				17,
				18,
				19,
				20,
				21,
				22,
				23,
				24,
				25,
				26,
				27,
				28,
				29,
				30,
				31,
				32,
				33,
				34,
				35,
				36,
				37,
				38,
				39,
				40,
				41,
				42,
				43,
				44,
				45,
				46,
				47,
				48,
				49,
				50,
				51,
				52,
				53,
				54,
				55,
				56,
				57,
				58,
				59,
				60,
				61,
				62,
				63,
				64,
				97,
				98,
				99,
				100,
				101,
				102,
				103,
				104,
				105,
				106,
				107,
				108,
				109,
				110,
				111,
				112,
				113,
				114,
				115,
				116,
				117,
				118,
				119,
				120,
				121,
				122,
				91,
				92,
				93,
				94,
				95,
				96,
				97,
				98,
				99,
				100,
				101,
				102,
				103,
				104,
				105,
				106,
				107,
				108,
				109,
				110,
				111,
				112,
				113,
				114,
				115,
				116,
				117,
				118,
				119,
				120,
				121,
				122,
				123,
				124,
				125,
				126,
				127,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};

			private static ASCIIEncoding encoder = new ASCIIEncoding();

			private byte asciiValue;
		}
	}
}
