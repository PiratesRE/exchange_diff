using System;
using System.Text;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class SmtpProxyAddress : ProxyAddress
	{
		public SmtpProxyAddress(string address, bool isPrimaryAddress) : base(ProxyAddressPrefix.Smtp, address, isPrimaryAddress)
		{
			if (SmtpProxyAddress.IsValidProxyAddress(address))
			{
				this.smtpAddress = address;
				return;
			}
			throw new ArgumentOutOfRangeException(DataStrings.ExceptionInvalidSmtpAddress(address));
		}

		public string SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
		}

		public static bool IsValidProxyAddress(string address)
		{
			return !string.IsNullOrEmpty(address) && Microsoft.Exchange.Data.SmtpAddress.IsValidSmtpAddress(address);
		}

		public static explicit operator SmtpAddress(SmtpProxyAddress value)
		{
			return new SmtpAddress(value.AddressString);
		}

		public static bool TryEncapsulate(ProxyAddress address, string domain, out SmtpProxyAddress smtpProxyAddress)
		{
			return SmtpProxyAddress.TryEncapsulate(address.PrefixString, address.AddressString, domain, out smtpProxyAddress);
		}

		public static bool TryEncapsulate(string addressType, string address, string domain, out SmtpProxyAddress smtpProxyAddress)
		{
			smtpProxyAddress = null;
			string address2 = SmtpProxyAddress.EncapsulateAddress(addressType, address, domain);
			if (Microsoft.Exchange.Data.SmtpAddress.IsValidSmtpAddress(address2))
			{
				smtpProxyAddress = new SmtpProxyAddress(address2, true);
				return true;
			}
			return false;
		}

		public static string EncapsulateAddress(string addressType, string address, string domain)
		{
			int num = (domain != null) ? domain.Length : 0;
			StringBuilder stringBuilder = new StringBuilder("IMCEA", "IMCEA".Length + addressType.Length + address.Length + num + 2);
			stringBuilder.Append(addressType);
			stringBuilder.Append('-');
			if (SmtpProxyAddress.HasTwoByteCharValue(address))
			{
				stringBuilder.Append("UTF8");
				stringBuilder.Append('-');
				SmtpProxyAddress.EncapsulateStringWithUTF8(stringBuilder, address);
			}
			else
			{
				SmtpProxyAddress.EncapsulateString(stringBuilder, address);
			}
			if (num != 0)
			{
				stringBuilder.Append('@');
				stringBuilder.Append(domain);
			}
			return stringBuilder.ToString();
		}

		private static void EncapsulateStringWithUTF8(StringBuilder encapsulated, string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			foreach (byte b in bytes)
			{
				char c = (char)b;
				if (SmtpProxyAddress.Is1252LetterOrDigit(c))
				{
					encapsulated.Append(c);
				}
				else
				{
					char c2 = c;
					switch (c2)
					{
					case '-':
						break;
					case '.':
						goto IL_65;
					case '/':
						encapsulated.Append('_');
						goto IL_98;
					default:
						if (c2 != '=')
						{
							goto IL_65;
						}
						break;
					}
					encapsulated.Append(c);
					goto IL_98;
					IL_65:
					encapsulated.Append('+');
					encapsulated.Append("0123456789ABCDEF"[(int)(b / 16)]);
					encapsulated.Append("0123456789ABCDEF"[(int)(b % 16)]);
				}
				IL_98:;
			}
		}

		private static void EncapsulateString(StringBuilder encapsulated, string str)
		{
			foreach (char c in str)
			{
				if (SmtpProxyAddress.Is1252LetterOrDigit(c))
				{
					encapsulated.Append(c);
				}
				else
				{
					char c2 = c;
					switch (c2)
					{
					case '-':
						break;
					case '.':
						goto IL_59;
					case '/':
						encapsulated.Append('_');
						goto IL_93;
					default:
						if (c2 != '=')
						{
							goto IL_59;
						}
						break;
					}
					encapsulated.Append(c);
					goto IL_93;
					IL_59:
					int num = Convert.ToInt32(c);
					encapsulated.Append('+');
					encapsulated.Append("0123456789ABCDEF"[num / 16]);
					encapsulated.Append("0123456789ABCDEF"[num % 16]);
				}
				IL_93:;
			}
		}

		public bool TryDeencapsulate(out ProxyAddress proxyAddress)
		{
			return SmtpProxyAddress.TryDeencapsulate(base.AddressString, out proxyAddress);
		}

		public static bool TryDeencapsulate(string address, out ProxyAddress proxyAddress)
		{
			proxyAddress = SmtpProxyAddress.Deencapsulate(address);
			return proxyAddress != null && !(proxyAddress is InvalidProxyAddress);
		}

		public static bool IsEncapsulatedAddress(string smtpAddress)
		{
			return SmtpProxyAddress.HasEncapsulationPrefix(smtpAddress) && smtpAddress.IndexOf('-') != -1;
		}

		public static bool TryDeencapsulateExchangeGuid(string address, out Guid exchangeGuid)
		{
			exchangeGuid = Guid.Empty;
			if (string.Compare("ExchangeGuid+", 0, address, 0, "ExchangeGuid+".Length, StringComparison.OrdinalIgnoreCase) == 0)
			{
				try
				{
					exchangeGuid = new Guid(address.Substring("ExchangeGuid+".Length, 36));
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
			}
			return !Guid.Empty.Equals(exchangeGuid);
		}

		public static string EncapsulateExchangeGuid(string domain, Guid exchangeGuid)
		{
			return string.Format("ExchangeGuid+{0}@{1}", exchangeGuid.ToString("D"), domain);
		}

		private static ProxyAddress Deencapsulate(string smtpAddress)
		{
			if (!SmtpProxyAddress.HasEncapsulationPrefix(smtpAddress))
			{
				return null;
			}
			int num = smtpAddress.IndexOf('-');
			if (-1 == num)
			{
				return null;
			}
			string text = SmtpProxyAddress.DeencapsulateString(smtpAddress, "IMCEA".Length, num - "IMCEA".Length);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			num++;
			int num2 = smtpAddress.LastIndexOf('@');
			if (num2 == -1 || num2 == smtpAddress.Length - 1)
			{
				return null;
			}
			string text2 = SmtpProxyAddress.DeencapsulateString(smtpAddress, num, num2 - num);
			if (string.IsNullOrEmpty(text2))
			{
				return null;
			}
			ProxyAddress result;
			try
			{
				result = ProxyAddress.Parse(text, text2);
			}
			catch (ArgumentOutOfRangeException)
			{
				result = null;
			}
			return result;
		}

		private static string DeencapsulateString(string encapsulated, int startIndex, int length)
		{
			string text = encapsulated.Substring(startIndex, length);
			if (text.StartsWith("UTF8" + '-'))
			{
				return SmtpProxyAddress.DeencapsulateUTF8String(encapsulated.Substring(startIndex + "UTF8".Length + 1, length - "UTF8".Length - 1));
			}
			return SmtpProxyAddress.DeencapsulateString(text);
		}

		private static string DeencapsulateString(string encapsulated)
		{
			StringBuilder stringBuilder = new StringBuilder(encapsulated.Length);
			int i = 0;
			while (i < encapsulated.Length)
			{
				char c = encapsulated[i];
				if (c != '+')
				{
					if (c != '_')
					{
						goto IL_48;
					}
					stringBuilder.Append('/');
				}
				else
				{
					char value;
					if (!SmtpProxyAddress.ConvertTwoHexCharToChar(encapsulated, i + 1, out value))
					{
						goto IL_48;
					}
					stringBuilder.Append(value);
					i += 2;
				}
				IL_56:
				i++;
				continue;
				IL_48:
				stringBuilder.Append(encapsulated[i]);
				goto IL_56;
			}
			return stringBuilder.ToString();
		}

		private static string DeencapsulateUTF8String(string encapsulated)
		{
			byte[] array = new byte[encapsulated.Length];
			int num = 0;
			int i = 0;
			while (i < encapsulated.Length)
			{
				char c = encapsulated[i];
				if (c != '+')
				{
					if (c != '_')
					{
						goto IL_4E;
					}
					array[num] = 47;
					num++;
				}
				else
				{
					char c2;
					if (!SmtpProxyAddress.ConvertTwoHexCharToChar(encapsulated, i + 1, out c2))
					{
						goto IL_4E;
					}
					array[num] = (byte)c2;
					i += 2;
					num++;
				}
				IL_6D:
				i++;
				continue;
				IL_4E:
				if (encapsulated[i] > 'ÿ')
				{
					return null;
				}
				array[num] = (byte)encapsulated[i];
				num++;
				goto IL_6D;
			}
			string result;
			try
			{
				Encoding encoding = new UTF8Encoding(false, true);
				result = encoding.GetString(array, 0, num);
			}
			catch (DecoderFallbackException)
			{
				result = null;
			}
			return result;
		}

		private static bool Is1252LetterOrDigit(char c)
		{
			return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9');
		}

		private static bool HasTwoByteCharValue(string str)
		{
			foreach (char value in str)
			{
				int num = Convert.ToInt32(value);
				if (num >= 256 || num < 0)
				{
					return true;
				}
			}
			return false;
		}

		private static bool ConvertTwoHexCharToChar(string str, int offset, out char ch)
		{
			ch = '\0';
			if (offset > str.Length - 2)
			{
				return false;
			}
			if (SmtpProxyAddress.Is1252LetterOrDigit(str[offset]) && SmtpProxyAddress.Is1252LetterOrDigit(str[offset + 1]))
			{
				try
				{
					ch = (char)(HexConverter.NumFromHex(str[offset]) * 16 + HexConverter.NumFromHex(str[offset + 1]));
					return true;
				}
				catch (FormatException)
				{
					return false;
				}
				return false;
			}
			return false;
		}

		private static bool HasEncapsulationPrefix(string smtpAddress)
		{
			return smtpAddress.Length > "IMCEA".Length && smtpAddress.StartsWith("IMCEA", StringComparison.Ordinal);
		}

		private const string IMCEA = "IMCEA";

		private const string UTF8 = "UTF8";

		private const string HexDigits = "0123456789ABCDEF";

		private const string ExchangeGuid = "ExchangeGuid+";

		private const string ExchangeGuidFormatString = "ExchangeGuid+{0}@{1}";

		private readonly string smtpAddress;
	}
}
