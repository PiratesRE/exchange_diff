using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ExchangeVersionHeader
	{
		public ExchangeVersionHeader(string headerValue)
		{
			this.rawValue = headerValue;
			this.parseError = null;
			this.Parse();
		}

		public bool IsMissing
		{
			get
			{
				return !this.numericVersionFound && !this.enumVersionFound && this.parseError == null;
			}
		}

		public ExchangeVersionType CheckAndGetRequestedVersion()
		{
			if (this.parseError != null)
			{
				throw new InvalidServerVersionException();
			}
			ExchangeVersionType result;
			try
			{
				if (this.enumVersionFound)
				{
					result = this.versionEnum;
				}
				else
				{
					string value;
					if (ExchangeVersionHeader.IsGreaterThanCurrent(this.major, this.minor))
					{
						value = string.Format("V{0}_{1}", ExchangeVersionHeader.MaxSupportedMajor.Member, ExchangeVersionHeader.MaxSupportedMinor.Member);
					}
					else
					{
						value = string.Format("V{0}_{1}", this.major, this.minor);
					}
					ExchangeVersionType exchangeVersionType = (ExchangeVersionType)Enum.Parse(typeof(ExchangeVersionType), value);
					this.CheckMinimumVersion();
					result = exchangeVersionType;
				}
			}
			catch (ArgumentException)
			{
				throw new InvalidServerVersionException();
			}
			return result;
		}

		private void CheckMinimumVersion()
		{
			if (!this.minimumVersionFound)
			{
				return;
			}
			if (!this.numericVersionFound)
			{
				this.RecordParseError("minimum greater than requested");
				throw new InvalidServerVersionException();
			}
			if (this.minMajor > this.major || (this.minMajor == this.major && this.minMinor > this.minor))
			{
				this.RecordParseError("minimum greater than requested");
				throw new InvalidServerVersionException();
			}
		}

		private void Parse()
		{
			if (string.IsNullOrWhiteSpace(this.rawValue))
			{
				return;
			}
			string[] array = this.rawValue.Trim().Split(new char[]
			{
				';'
			});
			switch (array.Length)
			{
			case 0:
				return;
			case 1:
				break;
			case 2:
			{
				string[] array2 = array[1].Trim().Split(new char[]
				{
					'='
				});
				if (array2 != null && array2.Length == 2 && array2[0].Equals("minimum", StringComparison.OrdinalIgnoreCase))
				{
					if (this.TryParseNumericVersion(array2[1].Trim(), out this.minMajor, out this.minMinor))
					{
						this.minimumVersionFound = true;
					}
					else
					{
						this.RecordParseError("second part didn't match x.y");
					}
				}
				else
				{
					this.RecordParseError("second part not recognized as minimum=x.y");
				}
				break;
			}
			default:
				this.RecordParseError("more than 2 semicolon-separated parts");
				return;
			}
			string text = array[0].Trim();
			if (this.TryParseNumericVersion(text, out this.major, out this.minor))
			{
				this.numericVersionFound = true;
				return;
			}
			if (this.TryParseEnumVersion(text, out this.versionEnum))
			{
				this.enumVersionFound = true;
				return;
			}
			this.RecordParseError("first part didn't match x.y or Exchange20xx");
		}

		private bool TryParseNumericVersion(string version, out int major, out int minor)
		{
			major = 0;
			minor = 0;
			string[] array = version.Split(new char[]
			{
				'.'
			});
			return array.Length == 2 && int.TryParse(array[0], out major) && int.TryParse(array[1], out minor);
		}

		private bool TryParseEnumVersion(string v, out ExchangeVersionType enumVersion)
		{
			enumVersion = ExchangeVersionType.Exchange2007;
			bool result;
			try
			{
				if (!v.StartsWith("Exchange", StringComparison.OrdinalIgnoreCase))
				{
					result = false;
				}
				else
				{
					enumVersion = EnumUtilities.Parse<ExchangeVersionType>(v);
					result = (enumVersion <= ExchangeVersionType.Exchange2013);
				}
			}
			catch (ArgumentException)
			{
				result = false;
			}
			return result;
		}

		private static bool IsGreaterThanCurrent(int major, int minor)
		{
			return major > ExchangeVersionHeader.MaxSupportedMajor.Member || (major == ExchangeVersionHeader.MaxSupportedMajor.Member && minor > ExchangeVersionHeader.MaxSupportedMinor.Member);
		}

		private void RecordParseError(string s)
		{
			this.parseError = s;
		}

		private const string MinimumParameterName = "minimum";

		private const string ExchangeVersionTypeFormat = "V{0}_{1}";

		private const string EnumPrefix = "Exchange";

		private const char ParameterSeparator = ';';

		private const char MajorMinorSeparator = '.';

		private const char ParameterValueSeparator = '=';

		internal static LazyMember<string> MaxSupportedVersionString = new LazyMember<string>(() => ExchangeVersionHeader.MaxSupportedMajor.Member + "." + ExchangeVersionHeader.MaxSupportedMinor.Member);

		private static LazyMember<int> MaxSupportedMajor = new LazyMember<int>(delegate()
		{
			string text = ExchangeVersion.MaxSupportedVersion.Member.ToString();
			int num = 1;
			int num2 = num;
			while (num2 < text.Length && char.IsDigit(text, num2))
			{
				num2++;
			}
			return int.Parse(text.Substring(num, num2 - num));
		});

		private static LazyMember<int> MaxSupportedMinor = new LazyMember<int>(delegate()
		{
			string text = ExchangeVersion.MaxSupportedVersion.Member.ToString();
			int length = text.Length;
			int num = length;
			while (num > 0 && char.IsDigit(text, num - 1))
			{
				num--;
			}
			return int.Parse(text.Substring(num, length - num));
		});

		private readonly string rawValue;

		private int major;

		private int minor;

		private int minMajor;

		private int minMinor;

		private ExchangeVersionType versionEnum;

		private bool numericVersionFound;

		private bool enumVersionFound;

		private bool minimumVersionFound;

		private string parseError;
	}
}
