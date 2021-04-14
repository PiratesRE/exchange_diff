using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Transport
{
	internal class BifInfo
	{
		public bool? SendTNEF
		{
			get
			{
				if (this.HasField(BifInfo.BitInfoFieldFlag.TNEF))
				{
					return new bool?(this.sendTNEF);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.sendTNEF = false;
					this.ClearFieldFlag(BifInfo.BitInfoFieldFlag.TNEF);
					return;
				}
				this.sendTNEF = value.Value;
				this.SetFieldFlag(BifInfo.BitInfoFieldFlag.TNEF);
			}
		}

		public uint? SendInternetEncoding
		{
			get
			{
				if (this.HasField(BifInfo.BitInfoFieldFlag.SendInternetEncoding))
				{
					return new uint?(this.sendInternetEncoding);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.sendInternetEncoding = 0U;
					this.ClearFieldFlag(BifInfo.BitInfoFieldFlag.SendInternetEncoding);
					return;
				}
				this.sendInternetEncoding = value.Value;
				this.SetFieldFlag(BifInfo.BitInfoFieldFlag.SendInternetEncoding);
			}
		}

		public bool? FriendlyNames
		{
			get
			{
				if (this.HasField(BifInfo.BitInfoFieldFlag.FriendlyNames))
				{
					return new bool?(this.friendlyNames);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.friendlyNames = false;
					this.ClearFieldFlag(BifInfo.BitInfoFieldFlag.FriendlyNames);
					return;
				}
				this.friendlyNames = value.Value;
				this.SetFieldFlag(BifInfo.BitInfoFieldFlag.FriendlyNames);
			}
		}

		public int? LineWrap
		{
			get
			{
				if (this.HasField(BifInfo.BitInfoFieldFlag.LineWrap))
				{
					return new int?(this.lineWrap);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.lineWrap = 0;
					this.ClearFieldFlag(BifInfo.BitInfoFieldFlag.LineWrap);
					return;
				}
				this.lineWrap = value.Value;
				this.SetFieldFlag(BifInfo.BitInfoFieldFlag.LineWrap);
			}
		}

		public string CharSet
		{
			get
			{
				if (this.HasField(BifInfo.BitInfoFieldFlag.CharSet))
				{
					return this.charSet;
				}
				return string.Empty;
			}
			set
			{
				this.charSet = value;
				if (value == null)
				{
					this.ClearFieldFlag(BifInfo.BitInfoFieldFlag.CharSet);
					return;
				}
				this.SetFieldFlag(BifInfo.BitInfoFieldFlag.CharSet);
			}
		}

		public uint? SuppressFlag
		{
			get
			{
				if (this.HasField(BifInfo.BitInfoFieldFlag.AutoResponseSuppress))
				{
					return new uint?(this.suppressFlag);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.suppressFlag = 0U;
					this.ClearFieldFlag(BifInfo.BitInfoFieldFlag.AutoResponseSuppress);
					return;
				}
				this.suppressFlag = value.Value;
				this.SetFieldFlag(BifInfo.BitInfoFieldFlag.AutoResponseSuppress);
			}
		}

		public BifSenderType? SenderType
		{
			get
			{
				if (this.HasField(BifInfo.BitInfoFieldFlag.SenderType))
				{
					return new BifSenderType?(this.senderType);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.senderType = BifSenderType.Originator;
					this.ClearFieldFlag(BifInfo.BitInfoFieldFlag.SenderType);
					return;
				}
				this.senderType = value.Value;
				this.SetFieldFlag(BifInfo.BitInfoFieldFlag.SenderType);
			}
		}

		public string DlOwnerDN
		{
			get
			{
				if (this.HasField(BifInfo.BitInfoFieldFlag.DlOwnerDN))
				{
					return this.dlOwnerDN;
				}
				return string.Empty;
			}
			set
			{
				this.dlOwnerDN = value;
				if (value == null)
				{
					this.ClearFieldFlag(BifInfo.BitInfoFieldFlag.DlOwnerDN);
					return;
				}
				this.SetFieldFlag(BifInfo.BitInfoFieldFlag.DlOwnerDN);
			}
		}

		public string SenderDN
		{
			get
			{
				if (this.HasField(BifInfo.BitInfoFieldFlag.SenderDN))
				{
					return this.senderDN;
				}
				return string.Empty;
			}
			set
			{
				this.senderDN = value;
				if (value == null)
				{
					this.ClearFieldFlag(BifInfo.BitInfoFieldFlag.SenderDN);
					return;
				}
				this.SetFieldFlag(BifInfo.BitInfoFieldFlag.SenderDN);
			}
		}

		public bool HasContentConversionOptions
		{
			get
			{
				return this.SendTNEF != null || this.SendInternetEncoding != null || this.FriendlyNames != null || this.LineWrap != null || !string.IsNullOrEmpty(this.CharSet);
			}
		}

		public static BifInfo FromByteArray(byte[] bifInfoBlob)
		{
			if (bifInfoBlob.Length != 916)
			{
				throw new BifInfoException("invalid data size");
			}
			BifInfo bifInfo = new BifInfo();
			int num = 0;
			bifInfo.fieldFlags = (BifInfo.BitInfoFieldFlag)BitConverter.ToUInt32(bifInfoBlob, num);
			num += 4;
			bifInfo.sendTNEF = (BitConverter.ToInt32(bifInfoBlob, num) != 0);
			num += 4;
			bifInfo.sendInternetEncoding = BitConverter.ToUInt32(bifInfoBlob, num);
			num += 4;
			bifInfo.friendlyNames = (BitConverter.ToInt32(bifInfoBlob, num) != 0);
			num += 4;
			bifInfo.lineWrap = BitConverter.ToInt32(bifInfoBlob, num);
			num += 4;
			bifInfo.charSet = BifInfo.ConvertUTF8BytesToString(bifInfoBlob, num, 256);
			num += 256;
			bifInfo.suppressFlag = BitConverter.ToUInt32(bifInfoBlob, num);
			num += 4;
			bifInfo.senderType = (BifSenderType)BitConverter.ToUInt32(bifInfoBlob, num);
			num += 4;
			bifInfo.dlOwnerDN = BifInfo.ConvertUTF8BytesToString(bifInfoBlob, num, 316);
			num += 316;
			bifInfo.senderDN = BifInfo.ConvertUTF8BytesToString(bifInfoBlob, num, 316);
			return bifInfo;
		}

		public static bool IsValidDN(string dn)
		{
			return Encoding.UTF8.GetByteCount(dn) < 316;
		}

		public static bool IsValidCharSetName(string charset)
		{
			return Encoding.UTF8.GetByteCount(charset) < 256;
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[916];
			int num = 0;
			num += ExBitConverter.Write((uint)this.fieldFlags, array, num);
			num += ExBitConverter.Write(Convert.ToInt32(this.sendTNEF), array, num);
			num += ExBitConverter.Write(this.sendInternetEncoding, array, num);
			num += ExBitConverter.Write(Convert.ToInt32(this.friendlyNames), array, num);
			num += ExBitConverter.Write(this.lineWrap, array, num);
			num += BifInfo.WriteStringAsBytes(this.charSet, array, num, 256);
			num += ExBitConverter.Write(this.suppressFlag, array, num);
			num += ExBitConverter.Write((uint)this.senderType, array, num);
			num += BifInfo.WriteStringAsBytes(this.dlOwnerDN, array, num, 316);
			num += BifInfo.WriteStringAsBytes(this.senderDN, array, num, 316);
			return array;
		}

		public override bool Equals(object obj)
		{
			BifInfo bifInfo = obj as BifInfo;
			return bifInfo != null && (this.fieldFlags == bifInfo.fieldFlags && this.sendTNEF == bifInfo.sendTNEF && this.sendInternetEncoding == bifInfo.sendInternetEncoding && this.friendlyNames == bifInfo.friendlyNames && this.lineWrap == bifInfo.lineWrap && this.suppressFlag == bifInfo.suppressFlag && this.senderType == bifInfo.senderType && string.Equals(this.CharSet, bifInfo.CharSet) && string.Equals(this.DlOwnerDN, bifInfo.DlOwnerDN)) && string.Equals(this.SenderDN, bifInfo.SenderDN);
		}

		public override int GetHashCode()
		{
			return (int)(this.sendInternetEncoding ^ (uint)this.lineWrap ^ this.suppressFlag);
		}

		public string GetContentConversionOptionsString()
		{
			if (!this.HasContentConversionOptions)
			{
				return string.Empty;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3};{4}", new object[]
			{
				this.SendTNEF.ToString(),
				(this.SendInternetEncoding != null) ? this.SendInternetEncoding.Value.ToString("X8") : string.Empty,
				this.FriendlyNames.ToString(),
				(this.LineWrap != null) ? this.LineWrap.Value.ToString(CultureInfo.InvariantCulture) : string.Empty,
				this.CharSet
			});
		}

		public void CopyFromContentConversionOptionsString(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return;
			}
			string[] array = str.Split(BifInfo.FieldSeparator);
			if (array.Length != 5)
			{
				return;
			}
			bool value;
			if (bool.TryParse(array[0], out value))
			{
				this.SendTNEF = new bool?(value);
			}
			uint value2;
			if (uint.TryParse(array[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value2))
			{
				this.SendInternetEncoding = new uint?(value2);
			}
			bool value3;
			if (bool.TryParse(array[2], out value3))
			{
				this.FriendlyNames = new bool?(value3);
			}
			int value4;
			if (int.TryParse(array[3], out value4))
			{
				this.LineWrap = new int?(value4);
			}
			Charset charset;
			if (Charset.TryGetCharset(array[4], out charset))
			{
				this.CharSet = array[4];
			}
		}

		private static int WriteStringAsBytes(string str, byte[] targetArray, int offset, int bytesToUseInTargetArray)
		{
			int num;
			if (str == null)
			{
				num = 0;
			}
			else
			{
				num = Encoding.UTF8.GetBytes(str, 0, str.Length, targetArray, offset);
			}
			if (num >= bytesToUseInTargetArray)
			{
				throw new ArgumentException("Input string is too long.");
			}
			for (int i = num; i < bytesToUseInTargetArray; i++)
			{
				targetArray[offset + i] = 0;
			}
			return bytesToUseInTargetArray;
		}

		private static string ConvertUTF8BytesToString(byte[] byteArray, int offset, int maximumLength)
		{
			int num = Array.IndexOf<byte>(byteArray, 0, offset, maximumLength);
			return Encoding.UTF8.GetString(byteArray, offset, num - offset);
		}

		private void ClearFieldFlag(BifInfo.BitInfoFieldFlag fieldFlag)
		{
			this.fieldFlags &= ~fieldFlag;
		}

		private void SetFieldFlag(BifInfo.BitInfoFieldFlag fieldFlag)
		{
			this.fieldFlags |= fieldFlag;
		}

		private bool HasField(BifInfo.BitInfoFieldFlag fieldFlag)
		{
			return (this.fieldFlags & fieldFlag) != (BifInfo.BitInfoFieldFlag)0U;
		}

		private const int MaximumDNLength = 316;

		private const int MaximumCharsetLength = 256;

		private const int BifInfoSize = 916;

		private static readonly char[] FieldSeparator = new char[]
		{
			';'
		};

		private BifInfo.BitInfoFieldFlag fieldFlags;

		private bool sendTNEF;

		private uint sendInternetEncoding;

		private bool friendlyNames;

		private int lineWrap;

		private string charSet;

		private uint suppressFlag;

		private BifSenderType senderType;

		private string dlOwnerDN;

		private string senderDN;

		[Flags]
		private enum BitInfoFieldFlag : uint
		{
			TNEF = 1U,
			SendInternetEncoding = 2U,
			FriendlyNames = 4U,
			LineWrap = 8U,
			CharSet = 16U,
			SenderType = 4096U,
			DlOwnerDN = 8192U,
			SenderDN = 16384U,
			AutoResponseSuppress = 32768U,
			ExpansionDsn = 16777216U
		}
	}
}
