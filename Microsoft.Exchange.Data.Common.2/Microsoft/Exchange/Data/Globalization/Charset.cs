using System;
using System.Text;

namespace Microsoft.Exchange.Data.Globalization
{
	[Serializable]
	public class Charset
	{
		internal Charset(int codePage, string name)
		{
			this.codePage = codePage;
			this.name = name;
			this.culture = null;
			this.available = true;
			this.mapIndex = -1;
		}

		public static Charset DefaultMimeCharset
		{
			get
			{
				return Culture.Default.MimeCharset;
			}
		}

		public static bool FallbackToDefaultCharset
		{
			get
			{
				return Culture.FallbackToDefaultCharset;
			}
		}

		public static Charset DefaultWebCharset
		{
			get
			{
				return Culture.Default.WebCharset;
			}
		}

		public static Charset DefaultWindowsCharset
		{
			get
			{
				return Culture.Default.WindowsCharset;
			}
		}

		public static Charset ASCII
		{
			get
			{
				return CultureCharsetDatabase.Data.AsciiCharset;
			}
		}

		public static Charset UTF8
		{
			get
			{
				return CultureCharsetDatabase.Data.Utf8Charset;
			}
		}

		public static Charset Unicode
		{
			get
			{
				return CultureCharsetDatabase.Data.UnicodeCharset;
			}
		}

		public int CodePage
		{
			get
			{
				return this.codePage;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public Culture Culture
		{
			get
			{
				return this.culture;
			}
		}

		public bool IsDetectable
		{
			get
			{
				return this.mapIndex >= 0 && 0 != (byte)(CodePageMapData.codePages[(int)this.mapIndex].flags & CodePageFlags.Detectable);
			}
		}

		public bool IsAvailable
		{
			get
			{
				return this.available && (this.encoding != null || this.CheckAvailable());
			}
		}

		public bool IsWindowsCharset
		{
			get
			{
				return this.windows;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		internal static int MaxCharsetNameLength
		{
			get
			{
				return CultureCharsetDatabase.Data.MaxCharsetNameLength;
			}
		}

		internal int MapIndex
		{
			get
			{
				return (int)this.mapIndex;
			}
		}

		internal CodePageKind Kind
		{
			get
			{
				if (this.mapIndex >= 0)
				{
					return CodePageMapData.codePages[(int)this.mapIndex].kind;
				}
				return CodePageKind.Unknown;
			}
		}

		internal CodePageAsciiSupport AsciiSupport
		{
			get
			{
				if (this.mapIndex >= 0)
				{
					return CodePageMapData.codePages[(int)this.mapIndex].asciiSupport;
				}
				return CodePageAsciiSupport.Unknown;
			}
		}

		internal CodePageUnicodeCoverage UnicodeCoverage
		{
			get
			{
				if (this.mapIndex >= 0)
				{
					return CodePageMapData.codePages[(int)this.mapIndex].unicodeCoverage;
				}
				return CodePageUnicodeCoverage.Unknown;
			}
		}

		internal bool IsSevenBit
		{
			get
			{
				return this.mapIndex >= 0 && 0 != (byte)(CodePageMapData.codePages[(int)this.mapIndex].flags & CodePageFlags.SevenBit);
			}
		}

		internal int DetectableCodePageWithEquivalentCoverage
		{
			get
			{
				if (this.mapIndex < 0)
				{
					return 0;
				}
				if ((byte)(CodePageMapData.codePages[(int)this.mapIndex].flags & CodePageFlags.Detectable) == 0)
				{
					return (int)CodePageMapData.codePages[(int)this.mapIndex].detectCpid;
				}
				return this.codePage;
			}
		}

		public static Charset GetCharset(string name)
		{
			Charset result;
			if (!Charset.TryGetCharset(name, out result))
			{
				throw new InvalidCharsetException(name);
			}
			return result;
		}

		public static bool TryGetCharset(string name, out Charset charset)
		{
			if (name == null)
			{
				charset = null;
				return false;
			}
			if (CultureCharsetDatabase.Data.NameToCharset.TryGetValue(name, out charset))
			{
				return true;
			}
			if (name.StartsWith("cp", StringComparison.OrdinalIgnoreCase) || name.StartsWith("ms", StringComparison.OrdinalIgnoreCase))
			{
				int num = 0;
				for (int i = 2; i < name.Length; i++)
				{
					if (name[i] < '0' || name[i] > '9')
					{
						num = 0;
						break;
					}
					num = num * 10 + (int)(name[i] - '0');
					if (num >= 65536)
					{
						num = 0;
						break;
					}
				}
				if (num != 0 && Charset.TryGetCharset(num, out charset))
				{
					return true;
				}
			}
			return ((Charset.FallbackToDefaultCharset && Charset.DefaultMimeCharset != null) || (!Charset.FallbackToDefaultCharset && Charset.DefaultMimeCharset != null && Charset.DefaultMimeCharset.Name.Equals("iso-2022-jp", StringComparison.OrdinalIgnoreCase))) && CultureCharsetDatabase.Data.NameToCharset.TryGetValue(Charset.DefaultMimeCharset.Name, out charset);
		}

		public static Charset GetCharset(int codePage)
		{
			Charset result;
			if (!Charset.TryGetCharset(codePage, out result))
			{
				throw new InvalidCharsetException(codePage);
			}
			return result;
		}

		public static Charset GetCharset(Encoding encoding)
		{
			int num = CodePageMap.GetCodePage(encoding);
			return Charset.GetCharset(num);
		}

		public static bool TryGetCharset(int codePage, out Charset charset)
		{
			return CultureCharsetDatabase.Data.CodePageToCharset.TryGetValue(codePage, out charset);
		}

		public static bool TryGetEncoding(int codePage, out Encoding encoding)
		{
			Charset charset;
			if (!Charset.TryGetCharset(codePage, out charset))
			{
				encoding = null;
				return false;
			}
			return charset.TryGetEncoding(out encoding);
		}

		public static bool TryGetEncoding(string name, out Encoding encoding)
		{
			Charset charset;
			if (!Charset.TryGetCharset(name, out charset))
			{
				encoding = null;
				return false;
			}
			return charset.TryGetEncoding(out encoding);
		}

		public static Encoding GetEncoding(int codePage)
		{
			Charset charset = Charset.GetCharset(codePage);
			return charset.GetEncoding();
		}

		public static Encoding GetEncoding(string name)
		{
			Charset charset = Charset.GetCharset(name);
			return charset.GetEncoding();
		}

		public Encoding GetEncoding()
		{
			Encoding result;
			if (!this.TryGetEncoding(out result))
			{
				throw new CharsetNotInstalledException(this.codePage, this.name);
			}
			return result;
		}

		internal void SetCulture(Culture culture)
		{
			this.culture = culture;
		}

		internal void SetDescription(string description)
		{
			this.description = description;
		}

		internal void SetDefaultName(string name)
		{
			this.name = name;
		}

		internal void SetWindows()
		{
			this.windows = true;
		}

		internal void SetMapIndex(int index)
		{
			this.mapIndex = (short)index;
		}

		internal bool CheckAvailable()
		{
			Encoding encoding;
			return this.TryGetEncoding(out encoding);
		}

		public static bool TryGetCharset(Encoding encoding, out Charset charset)
		{
			return Charset.TryGetCharset(encoding.CodePage, out charset);
		}

		public bool TryGetEncoding(out Encoding encoding)
		{
			if (this.encoding == null && this.available)
			{
				try
				{
					if (this.codePage == 20127)
					{
						this.encoding = Encoding.GetEncoding(this.codePage, new AsciiEncoderFallback(), DecoderFallback.ReplacementFallback);
					}
					else if (this.codePage == 28591 || this.codePage == 28599)
					{
						this.encoding = new RemapEncoding(this.codePage);
					}
					else if (this.codePage == 50220 || this.codePage == 50221 || this.codePage == 50222)
					{
						this.encoding = new Iso2022JpEncoding(this.codePage);
					}
					else
					{
						this.encoding = Encoding.GetEncoding(this.codePage);
					}
				}
				catch (ArgumentException)
				{
					this.encoding = null;
				}
				catch (NotSupportedException)
				{
					this.encoding = null;
				}
				if (this.encoding == null)
				{
					this.available = false;
				}
			}
			encoding = this.encoding;
			return encoding != null;
		}

		private int codePage;

		private string name;

		private Culture culture;

		private short mapIndex;

		private bool windows;

		private bool available;

		private Encoding encoding;

		private string description;
	}
}
