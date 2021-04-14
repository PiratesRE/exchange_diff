using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal struct RecipientEncoding : IEquatable<RecipientEncoding>, IComparable<RecipientEncoding>
	{
		public RecipientEncoding(bool? tnefEnabled, int? internetEncoding)
		{
			this = new RecipientEncoding(tnefEnabled, internetEncoding, null);
		}

		public RecipientEncoding(bool? tnefEnabled, int? internetEncoding, string characterSet)
		{
			this.tnefEnabled = tnefEnabled;
			this.encoding = internetEncoding;
			this.displaySenderName = false;
			this.useSimpleDisplayName = false;
			this.byteEncoderTypeFor7BitCharsets = ByteEncoderTypeFor7BitCharsetsEnum.Undefined;
			this.preferredInternetCodePageForShiftJis = PreferredInternetCodePageForShiftJisEnum.Undefined;
			this.requiredCharsetCoverage = null;
			this.characterSet = ((characterSet == null) ? string.Empty : characterSet);
		}

		public ByteEncoderTypeFor7BitCharsetsEnum ByteEncoderTypeFor7BitCharsets
		{
			get
			{
				return this.byteEncoderTypeFor7BitCharsets;
			}
			set
			{
				this.byteEncoderTypeFor7BitCharsets = value;
			}
		}

		public string CharacterSet
		{
			get
			{
				return this.characterSet;
			}
			set
			{
				this.characterSet = value;
			}
		}

		public bool? TNEFEnabled
		{
			get
			{
				return this.tnefEnabled;
			}
			set
			{
				this.tnefEnabled = value;
			}
		}

		public int? InternetEncoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				this.encoding = value;
			}
		}

		public bool DisplaySenderName
		{
			get
			{
				return this.displaySenderName;
			}
			set
			{
				this.displaySenderName = value;
			}
		}

		public PreferredInternetCodePageForShiftJisEnum PreferredInternetCodePageForShiftJis
		{
			get
			{
				return this.preferredInternetCodePageForShiftJis;
			}
			set
			{
				this.preferredInternetCodePageForShiftJis = value;
			}
		}

		public int? RequiredCharsetCoverage
		{
			get
			{
				return this.requiredCharsetCoverage;
			}
			set
			{
				this.requiredCharsetCoverage = value;
			}
		}

		public bool UseSimpleDisplayName
		{
			get
			{
				return this.useSimpleDisplayName;
			}
			set
			{
				this.useSimpleDisplayName = value;
			}
		}

		public bool IsMimeEncoding
		{
			get
			{
				return this.encoding != null && (this.encoding.Value & 262144) != 0;
			}
		}

		public InternetMessageFormat InternetMessageFormat
		{
			get
			{
				if (this.encoding == null)
				{
					return InternetMessageFormat.Mime;
				}
				if ((this.encoding.Value & 262144) != 0)
				{
					return InternetMessageFormat.Mime;
				}
				if ((this.encoding.Value & 2228224) != 0)
				{
					return InternetMessageFormat.Uuencode;
				}
				return InternetMessageFormat.Binhex;
			}
		}

		public InternetTextFormat InternetTextFormat
		{
			get
			{
				if (this.encoding != null)
				{
					if (this.encoding.Value == 917504)
					{
						return InternetTextFormat.HtmlOnly;
					}
					if (this.encoding.Value == 1441792)
					{
						return InternetTextFormat.HtmlAndTextAlternative;
					}
				}
				return InternetTextFormat.TextOnly;
			}
		}

		public bool Equals(RecipientEncoding other)
		{
			return this.tnefEnabled.Equals(other.tnefEnabled) && this.displaySenderName == other.displaySenderName && this.useSimpleDisplayName == other.useSimpleDisplayName && this.encoding.Equals(other.encoding) && string.Equals(this.characterSet, other.characterSet) && this.requiredCharsetCoverage == other.requiredCharsetCoverage && this.preferredInternetCodePageForShiftJis == other.preferredInternetCodePageForShiftJis && this.byteEncoderTypeFor7BitCharsets == other.byteEncoderTypeFor7BitCharsets;
		}

		public int CompareTo(RecipientEncoding other)
		{
			int num = Nullable.Compare<bool>(this.tnefEnabled, other.tnefEnabled);
			if (num != 0)
			{
				return num;
			}
			num = Nullable.Compare<int>(this.encoding, other.encoding);
			if (num != 0)
			{
				return num;
			}
			if (this.displaySenderName != other.displaySenderName)
			{
				if (!this.displaySenderName)
				{
					return -1;
				}
				return 1;
			}
			else if (this.useSimpleDisplayName != other.useSimpleDisplayName)
			{
				if (!this.useSimpleDisplayName)
				{
					return -1;
				}
				return 1;
			}
			else
			{
				num = Nullable.Compare<int>(this.requiredCharsetCoverage, other.requiredCharsetCoverage);
				if (num != 0)
				{
					return num;
				}
				num = this.byteEncoderTypeFor7BitCharsets.CompareTo(other.byteEncoderTypeFor7BitCharsets);
				if (num != 0)
				{
					return num;
				}
				num = this.preferredInternetCodePageForShiftJis.CompareTo(other.preferredInternetCodePageForShiftJis);
				if (num != 0)
				{
					return num;
				}
				return string.CompareOrdinal(this.characterSet, other.characterSet);
			}
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is RecipientEncoding && this.Equals((RecipientEncoding)obj);
		}

		public override int GetHashCode()
		{
			int num = this.encoding ?? 0;
			if (this.tnefEnabled != null)
			{
				num <<= 1;
				num |= (this.tnefEnabled.Value ? 1 : 0);
			}
			num <<= 1;
			num |= (this.displaySenderName ? 1 : 0);
			num <<= 1;
			num |= (this.useSimpleDisplayName ? 1 : 0);
			if (this.characterSet != null)
			{
				num = (num << 1 ^ this.characterSet.GetHashCode());
			}
			num = (num << 1 ^ this.byteEncoderTypeFor7BitCharsets.GetHashCode());
			num = (num << 1 ^ this.preferredInternetCodePageForShiftJis.GetHashCode());
			if (this.requiredCharsetCoverage != null)
			{
				num = (num << 1 ^ this.requiredCharsetCoverage.GetHashCode());
			}
			return num;
		}

		private string characterSet;

		private ByteEncoderTypeFor7BitCharsetsEnum byteEncoderTypeFor7BitCharsets;

		private bool? tnefEnabled;

		private int? encoding;

		private bool displaySenderName;

		private PreferredInternetCodePageForShiftJisEnum preferredInternetCodePageForShiftJis;

		private int? requiredCharsetCoverage;

		private bool useSimpleDisplayName;
	}
}
