using System;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime
{
	public abstract class AddressItem : MimeNode
	{
		internal AddressItem()
		{
		}

		internal AddressItem(string displayName)
		{
			this.decodedDisplayName = displayName;
		}

		internal AddressItem(ref MimeStringList displayName)
		{
			this.displayNameFragments.TakeOver(ref displayName);
		}

		public string DisplayName
		{
			get
			{
				DecodingResults decodingResults;
				if (this.decodedDisplayName == null && !this.TryGetDisplayName(base.GetHeaderDecodingOptions(), out decodingResults, out this.decodedDisplayName))
				{
					MimeCommon.ThrowDecodingFailedException(ref decodingResults);
				}
				return this.decodedDisplayName;
			}
			set
			{
				this.displayNameFragments.Reset();
				this.decodedDisplayName = value;
				this.SetDirty();
			}
		}

		public virtual bool RequiresSMTPUTF8
		{
			get
			{
				return false;
			}
		}

		internal string DecodedDisplayName
		{
			set
			{
				this.decodedDisplayName = value;
			}
		}

		public override void CopyTo(object destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (destination == this)
			{
				return;
			}
			AddressItem addressItem = destination as AddressItem;
			if (addressItem == null)
			{
				throw new ArgumentException(Strings.CantCopyToDifferentObjectType);
			}
			base.CopyTo(destination);
			addressItem.displayNameFragments = this.displayNameFragments.Clone();
			addressItem.decodedDisplayName = this.decodedDisplayName;
		}

		public bool TryGetDisplayName(out string displayName)
		{
			DecodingResults decodingResults;
			return this.TryGetDisplayName(base.GetHeaderDecodingOptions(), out decodingResults, out displayName);
		}

		public bool TryGetDisplayName(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string displayName)
		{
			if (this.displayNameFragments.Count == 0)
			{
				displayName = this.decodedDisplayName;
				decodingResults = default(DecodingResults);
				return true;
			}
			if (decodingOptions.Charset == null)
			{
				decodingOptions.Charset = base.GetDefaultHeaderDecodingCharset(null, null);
			}
			return MimeCommon.TryDecodeValue(this.displayNameFragments, 4026531839U, decodingOptions, out decodingResults, out displayName);
		}

		private bool IsQuotingRequired(string displayName, bool allowUTF8)
		{
			MimeString mimeStr = new MimeString(displayName);
			return this.IsQuotingRequired(mimeStr, allowUTF8);
		}

		private bool IsQuotingRequired(MimeString mimeStr, bool allowUTF8)
		{
			AddressItem.WriteState writeState = AddressItem.WriteState.Begin;
			MimeString mimeString = new MimeString(AddressItem.WordBreakBytes, 0, AddressItem.WordBreakBytes.Length);
			int num;
			int num2;
			byte[] data = mimeStr.GetData(out num, out num2);
			while (num2 != 0)
			{
				switch (writeState)
				{
				case AddressItem.WriteState.Begin:
				{
					int num3 = 0;
					int num4 = MimeScan.FindEndOf(MimeScan.Token.Atom, data, num, num2, out num3, allowUTF8);
					if (num4 == 0)
					{
						if (num2 <= 3 || num != 0 || !mimeString.HasPrefixEq(data, 0, 3))
						{
							return true;
						}
						num += 3;
						num2 -= 3;
						writeState = AddressItem.WriteState.Begin;
					}
					else
					{
						num += num4;
						num2 -= num4;
						writeState = AddressItem.WriteState.Atom;
					}
					break;
				}
				case AddressItem.WriteState.Atom:
					if ((num2 < 2 || data[num] != 32) && (num2 < 1 || data[num] != 46))
					{
						return true;
					}
					num++;
					num2--;
					writeState = AddressItem.WriteState.Begin;
					break;
				}
			}
			return false;
		}

		internal bool IsQuotingRequired(MimeStringList displayNameFragments, bool allowUTF8)
		{
			for (int num = 0; num != displayNameFragments.Count; num++)
			{
				MimeString mimeStr = displayNameFragments[num];
				if ((mimeStr.Mask & 4026531839U) != 0U && this.IsQuotingRequired(mimeStr, allowUTF8))
				{
					return true;
				}
			}
			return false;
		}

		internal string QuoteString(string inputString)
		{
			StringBuilder stringBuilder = new StringBuilder(inputString.Length + 2);
			stringBuilder.Append("\"");
			foreach (char c in inputString)
			{
				if (c < '\u0080' && MimeScan.IsEscapingRequired((byte)c))
				{
					stringBuilder.Append("\\");
				}
				stringBuilder.Append(c);
			}
			stringBuilder.Append("\"");
			return stringBuilder.ToString();
		}

		internal void ResetDisplayNameFragments()
		{
			this.displayNameFragments.Reset();
		}

		internal MimeStringList GetDisplayNameToWrite(EncodingOptions encodingOptions)
		{
			MimeStringList result = this.displayNameFragments;
			if (result.GetLength(4026531839U) == 0 && this.decodedDisplayName != null && this.decodedDisplayName.Length != 0)
			{
				string value;
				if ((byte)(encodingOptions.EncodingFlags & EncodingFlags.QuoteDisplayNameBeforeRfc2047Encoding) != 0 && this.IsQuotingRequired(this.decodedDisplayName, encodingOptions.AllowUTF8) && MimeCommon.IsEncodingRequired(this.decodedDisplayName, encodingOptions.AllowUTF8))
				{
					value = this.QuoteString(this.decodedDisplayName);
				}
				else
				{
					value = this.decodedDisplayName;
				}
				result = MimeCommon.EncodeValue(value, encodingOptions, ValueEncodingStyle.Phrase);
				this.displayNameFragments = result;
			}
			else if ((byte)(EncodingFlags.ForceReencode & encodingOptions.EncodingFlags) != 0)
			{
				result = MimeCommon.EncodeValue(this.DisplayName, encodingOptions, ValueEncodingStyle.Phrase);
			}
			return result;
		}

		internal static readonly byte[] WordBreakBytes = ByteString.StringToBytes(" =?", true);

		private MimeStringList displayNameFragments;

		private string decodedDisplayName;

		private enum WriteState
		{
			Begin,
			Atom
		}
	}
}
