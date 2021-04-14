using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime
{
	public class MimeRecipient : AddressItem
	{
		public MimeRecipient()
		{
		}

		public MimeRecipient(string displayName, string email) : base(displayName)
		{
			if (email == null)
			{
				throw new ArgumentNullException("email");
			}
			this.emailAddressFragments.Append(new MimeString(email));
		}

		internal MimeRecipient(ref MimeStringList address, ref MimeStringList displayName) : base(ref displayName)
		{
			this.emailAddressFragments.TakeOverAppend(ref address);
		}

		public string Email
		{
			get
			{
				byte[] sz = this.emailAddressFragments.GetSz();
				if (sz != null)
				{
					return ByteString.BytesToString(sz, true);
				}
				return string.Empty;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (!MimeAddressParser.IsWellFormedAddress(value, true))
				{
					throw new ArgumentException("Address string must be well-formed", "value");
				}
				this.emailAddressFragments.Reset();
				this.emailAddressFragments.Append(new MimeString(value));
				this.SetDirty();
			}
		}

		public sealed override bool RequiresSMTPUTF8
		{
			get
			{
				return !MimeString.IsPureASCII(this.emailAddressFragments);
			}
		}

		public static MimeRecipient Parse(string address, AddressParserFlags flags)
		{
			MimeRecipient mimeRecipient = new MimeRecipient();
			if (!string.IsNullOrEmpty(address))
			{
				byte[] array = ByteString.StringToBytes(address, true);
				MimeAddressParser mimeAddressParser = new MimeAddressParser();
				mimeAddressParser.Initialize(new MimeStringList(array, 0, array.Length), AddressParserFlags.None != (flags & AddressParserFlags.IgnoreComments), AddressParserFlags.None != (flags & AddressParserFlags.AllowSquareBrackets), true);
				MimeStringList displayNameFragments = default(MimeStringList);
				mimeAddressParser.ParseNextMailbox(ref displayNameFragments, ref mimeRecipient.emailAddressFragments);
				MimeRecipient.ConvertDisplayNameBack(mimeRecipient, displayNameFragments, true);
			}
			return mimeRecipient;
		}

		public static bool IsEmailValid(string email)
		{
			return MimeRecipient.IsEmailValid(email, false);
		}

		public static bool IsEmailValid(string email, bool allowUTF8)
		{
			return MimeAddressParser.IsWellFormedAddress(email, allowUTF8);
		}

		public sealed override MimeNode Clone()
		{
			MimeRecipient mimeRecipient = new MimeRecipient();
			this.CopyTo(mimeRecipient);
			return mimeRecipient;
		}

		public sealed override void CopyTo(object destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (destination == this)
			{
				return;
			}
			MimeRecipient mimeRecipient = destination as MimeRecipient;
			if (mimeRecipient == null)
			{
				throw new ArgumentException(Strings.CantCopyToDifferentObjectType);
			}
			base.CopyTo(destination);
			mimeRecipient.emailAddressFragments = this.emailAddressFragments.Clone();
		}

		internal static void ConvertDisplayNameBack(AddressItem addressItem, MimeStringList displayNameFragments, bool allowUTF8)
		{
			byte[] sz = displayNameFragments.GetSz(4026531839U);
			if (sz == null)
			{
				addressItem.DecodedDisplayName = null;
				return;
			}
			string decodedDisplayName = ByteString.BytesToString(sz, allowUTF8);
			addressItem.DecodedDisplayName = decodedDisplayName;
		}

		internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
		{
			throw new NotSupportedException(Strings.RecipientsCannotHaveChildNodes);
		}

		internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
		{
			MimeStringList displayNameToWrite = base.GetDisplayNameToWrite(encodingOptions);
			long num = 0L;
			int num2 = 0;
			if (base.NextSibling != null)
			{
				num2++;
			}
			else if (base.Parent is MimeGroup)
			{
				num2++;
				if (base.Parent.NextSibling != null)
				{
					num2++;
				}
			}
			byte[] sz = this.emailAddressFragments.GetSz();
			int num3 = ByteString.BytesToCharCount(sz, encodingOptions.AllowUTF8);
			if (displayNameToWrite.GetLength(4026531839U) != 0)
			{
				num += Header.QuoteAndFold(stream, displayNameToWrite, 4026531839U, base.IsQuotingRequired(displayNameToWrite, encodingOptions.AllowUTF8), true, encodingOptions.AllowUTF8, (num3 == 0) ? num2 : 0, ref currentLineLength, ref scratchBuffer);
			}
			if (num3 != 0)
			{
				int num4 = (1 < currentLineLength.InChars) ? 1 : 0;
				if (1 < currentLineLength.InChars)
				{
					if (currentLineLength.InChars + num3 + 2 + num2 + num4 > 78)
					{
						num += Header.WriteLineEnd(stream, ref currentLineLength);
						stream.Write(Header.LineStartWhitespace, 0, Header.LineStartWhitespace.Length);
						num += (long)Header.LineStartWhitespace.Length;
						currentLineLength.IncrementBy(Header.LineStartWhitespace.Length);
					}
					else
					{
						stream.Write(MimeString.Space, 0, MimeString.Space.Length);
						num += (long)MimeString.Space.Length;
						currentLineLength.IncrementBy(MimeString.Space.Length);
					}
				}
				stream.Write(MimeString.LessThan, 0, MimeString.LessThan.Length);
				num += (long)MimeString.LessThan.Length;
				currentLineLength.IncrementBy(MimeString.LessThan.Length);
				stream.Write(sz, 0, sz.Length);
				num += (long)sz.Length;
				currentLineLength.IncrementBy(num3, sz.Length);
				stream.Write(MimeString.GreaterThan, 0, MimeString.GreaterThan.Length);
				num += (long)MimeString.GreaterThan.Length;
				currentLineLength.IncrementBy(MimeString.GreaterThan.Length);
			}
			return num;
		}

		private MimeStringList emailAddressFragments;
	}
}
