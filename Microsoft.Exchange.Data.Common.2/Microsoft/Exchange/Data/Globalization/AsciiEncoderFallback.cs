using System;
using System.Text;

namespace Microsoft.Exchange.Data.Globalization
{
	internal class AsciiEncoderFallback : EncoderFallback
	{
		public override int MaxCharCount
		{
			get
			{
				return AsciiEncoderFallback.AsciiFallbackBuffer.MaxCharCount;
			}
		}

		public static string GetCharacterFallback(char charUnknown)
		{
			if (charUnknown <= 'œ' && charUnknown >= '\u0082')
			{
				if (charUnknown <= 'æ')
				{
					switch (charUnknown)
					{
					case '\u0082':
					case '\u0091':
					case '\u0092':
						return "'";
					case '\u0083':
						return "f";
					case '\u0084':
					case '\u0093':
					case '\u0094':
						return "\"";
					case '\u0085':
						return "...";
					case '\u0086':
					case '\u0087':
					case '\u0088':
					case '\u0089':
					case '\u008a':
					case '\u008d':
					case '\u008e':
					case '\u008f':
					case '\u0090':
					case '\u009a':
					case '\u009d':
					case '\u009e':
					case '\u009f':
					case '¡':
					case '£':
					case '§':
					case '¨':
					case 'ª':
					case '¬':
					case '¯':
					case '°':
					case '±':
					case '´':
					case 'µ':
					case '¶':
					case 'º':
					case '¿':
					case 'À':
					case 'Á':
					case 'Â':
					case 'Ã':
					case 'Ä':
					case 'Å':
						break;
					case '\u008b':
						return "<";
					case '\u008c':
						return "OE";
					case '\u0095':
						return "*";
					case '\u0096':
						return "-";
					case '\u0097':
						return "-";
					case '\u0098':
						return "~";
					case '\u0099':
						return "(tm)";
					case '\u009b':
						return ">";
					case '\u009c':
						return "oe";
					case '\u00a0':
						return " ";
					case '¢':
						return "c";
					case '¤':
						return "$";
					case '¥':
						return "Y";
					case '¦':
						return "|";
					case '©':
						return "(c)";
					case '«':
						return "<";
					case '­':
						return string.Empty;
					case '®':
						return "(r)";
					case '²':
						return "^2";
					case '³':
						return "^3";
					case '·':
						return "*";
					case '¸':
						return ",";
					case '¹':
						return "^1";
					case '»':
						return ">";
					case '¼':
						return "(1/4)";
					case '½':
						return "(1/2)";
					case '¾':
						return "(3/4)";
					case 'Æ':
						return "AE";
					default:
						if (charUnknown == 'æ')
						{
							return "ae";
						}
						break;
					}
				}
				else
				{
					switch (charUnknown)
					{
					case 'Ĳ':
						return "IJ";
					case 'ĳ':
						return "ij";
					default:
						switch (charUnknown)
						{
						case 'Œ':
							return "OE";
						case 'œ':
							return "oe";
						}
						break;
					}
				}
			}
			else if (charUnknown >= '\u2002' && charUnknown <= '™')
			{
				if (charUnknown <= '…')
				{
					switch (charUnknown)
					{
					case '\u2002':
					case '\u2003':
						return " ";
					default:
						switch (charUnknown)
						{
						case '‑':
							return "-";
						case '‒':
						case '―':
						case '‖':
						case '‗':
						case '‛':
						case '‟':
						case '†':
						case '‡':
							break;
						case '–':
						case '—':
							return "-";
						case '‘':
						case '’':
						case '‚':
							return "'";
						case '“':
						case '”':
						case '„':
							return "\"";
						case '•':
							return "*";
						default:
							if (charUnknown == '…')
							{
								return "...";
							}
							break;
						}
						break;
					}
				}
				else
				{
					switch (charUnknown)
					{
					case '‹':
						return "<";
					case '›':
						return ">";
					default:
						if (charUnknown == '€')
						{
							return "EUR";
						}
						if (charUnknown == '™')
						{
							return "(tm)";
						}
						break;
					}
				}
			}
			else if (charUnknown >= '☹' && charUnknown <= '☺')
			{
				switch (charUnknown)
				{
				case '☹':
					return ":(";
				case '☺':
					return ":)";
				}
			}
			return null;
		}

		public override EncoderFallbackBuffer CreateFallbackBuffer()
		{
			return new AsciiEncoderFallback.AsciiFallbackBuffer();
		}

		private class AsciiFallbackBuffer : EncoderFallbackBuffer
		{
			public static int MaxCharCount
			{
				get
				{
					return 5;
				}
			}

			public override int Remaining
			{
				get
				{
					if (this.fallbackString != null)
					{
						return this.fallbackString.Length - this.fallbackIndex;
					}
					return 0;
				}
			}

			public override bool Fallback(char charUnknown, int index)
			{
				this.fallbackIndex = 0;
				this.fallbackString = AsciiEncoderFallback.GetCharacterFallback(charUnknown);
				if (this.fallbackString == null)
				{
					this.fallbackString = "?";
				}
				return true;
			}

			public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
			{
				this.fallbackIndex = 0;
				this.fallbackString = "?";
				return true;
			}

			public override char GetNextChar()
			{
				if (this.fallbackString == null || this.fallbackIndex == this.fallbackString.Length)
				{
					return '\0';
				}
				return this.fallbackString[this.fallbackIndex++];
			}

			public override bool MovePrevious()
			{
				if (this.fallbackIndex > 0)
				{
					this.fallbackIndex--;
					return true;
				}
				return false;
			}

			public override void Reset()
			{
				this.fallbackString = "?";
				this.fallbackIndex = 0;
			}

			private int fallbackIndex;

			private string fallbackString;
		}
	}
}
