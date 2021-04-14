using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime
{
	public class AddressHeader : Header, IEnumerable<AddressItem>, IEnumerable
	{
		public AddressHeader(string name) : this(name, Header.GetHeaderId(name, true))
		{
			Type type = Header.TypeFromHeaderId(base.HeaderId);
			if (base.HeaderId != HeaderId.Unknown && type != typeof(AddressHeader))
			{
				throw new ArgumentException(Strings.NameNotValidForThisHeaderType(name, "AddressHeader", type.Name));
			}
		}

		internal AddressHeader(string name, HeaderId headerId) : base(name, headerId)
		{
		}

		public sealed override string Value
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotSupportedException(Strings.UnicodeMimeHeaderAddressNotSupported);
			}
		}

		public sealed override bool RequiresSMTPUTF8
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				for (MimeNode mimeNode = base.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling)
				{
					AddressItem addressItem = mimeNode as AddressItem;
					if (addressItem != null && addressItem.RequiresSMTPUTF8)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal override byte[] RawValue
		{
			get
			{
				return null;
			}
			set
			{
				base.RawValue = value;
			}
		}

		internal override void RawValueAboutToChange()
		{
			this.parsed = true;
			base.InternalRemoveAll();
			if (this.parser != null)
			{
				this.parser.Reset();
			}
			this.parsed = false;
		}

		public override bool TryGetValue(out string value)
		{
			value = null;
			return false;
		}

		public new MimeNode.Enumerator<AddressItem> GetEnumerator()
		{
			return new MimeNode.Enumerator<AddressItem>(this);
		}

		IEnumerator<AddressItem> IEnumerable<AddressItem>.GetEnumerator()
		{
			return new MimeNode.Enumerator<AddressItem>(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new MimeNode.Enumerator<AddressItem>(this);
		}

		public sealed override MimeNode Clone()
		{
			AddressHeader addressHeader = new AddressHeader(base.Name, base.HeaderId);
			this.CopyTo(addressHeader);
			return addressHeader;
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
			AddressHeader addressHeader = destination as AddressHeader;
			if (addressHeader == null)
			{
				throw new ArgumentException(Strings.CantCopyToDifferentObjectType);
			}
			base.CopyTo(destination);
			addressHeader.parsed = this.parsed;
			addressHeader.parser = ((this.parser == null) ? null : new MimeAddressParser(addressHeader.Lines, this.parser));
		}

		public static AddressHeader Parse(string name, string value, AddressParserFlags flags)
		{
			AddressHeader addressHeader = new AddressHeader(name);
			if (!string.IsNullOrEmpty(value))
			{
				byte[] array = ByteString.StringToBytes(value, true);
				addressHeader.parser = new MimeAddressParser();
				addressHeader.parser.Initialize(new MimeStringList(array, 0, array.Length), AddressParserFlags.None != (flags & AddressParserFlags.IgnoreComments), AddressParserFlags.None != (flags & AddressParserFlags.AllowSquareBrackets), true);
				addressHeader.staticParsing = true;
				addressHeader.Parse();
			}
			return addressHeader;
		}

		public sealed override bool IsValueValid(string value)
		{
			return false;
		}

		internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
		{
			long num = base.WriteName(stream, ref scratchBuffer);
			currentLineLength.IncrementBy((int)num);
			if (!base.IsDirty && base.RawLength != 0)
			{
				if (base.IsProtected)
				{
					num += Header.WriteLines(base.Lines, stream);
					currentLineLength.SetAs(0);
					return num;
				}
				if (base.InternalLastChild == null)
				{
					bool flag = false;
					if (!base.IsHeaderLineTooLong(num, out flag))
					{
						num += Header.WriteLines(base.Lines, stream);
						currentLineLength.SetAs(0);
						return num;
					}
				}
			}
			if (!this.parsed)
			{
				this.Parse();
			}
			MimeNode mimeNode = base.FirstChild;
			int num2 = 0;
			while (mimeNode != null)
			{
				if (1 < ++num2)
				{
					stream.Write(MimeString.Comma, 0, MimeString.Comma.Length);
					num += (long)MimeString.Comma.Length;
					currentLineLength.IncrementBy(MimeString.Comma.Length);
				}
				num += mimeNode.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
				mimeNode = mimeNode.NextSibling;
			}
			return num + Header.WriteLineEnd(stream, ref currentLineLength);
		}

		internal override void RemoveAllUnparsed()
		{
			this.parsed = true;
		}

		internal override MimeNode ParseNextChild()
		{
			if (this.parsed)
			{
				return null;
			}
			MimeNode internalLastChild = base.InternalLastChild;
			MimeNode mimeNode;
			if (internalLastChild is MimeGroup)
			{
				while (internalLastChild.ParseNextChild() != null)
				{
				}
				mimeNode = internalLastChild.InternalNextSibling;
			}
			else
			{
				mimeNode = this.ParseNextMailBox(false);
			}
			this.parsed = (mimeNode == null);
			return mimeNode;
		}

		internal override void CheckChildrenLimit(int countLimit, int bytesLimit)
		{
			if (this.parser == null)
			{
				this.parser = new MimeAddressParser();
			}
			if (!this.parser.Initialized)
			{
				DecodingOptions headerDecodingOptions = base.GetHeaderDecodingOptions();
				this.parser.Initialize(base.Lines, false, false, headerDecodingOptions.AllowUTF8);
			}
			int i;
			for (i = 0; i <= countLimit; i++)
			{
				MimeStringList mimeStringList = default(MimeStringList);
				MimeStringList mimeStringList2 = default(MimeStringList);
				if (AddressParserResult.End == this.parser.ParseNextMailbox(ref mimeStringList, ref mimeStringList2))
				{
					this.parser.Reset();
					return;
				}
				if (mimeStringList.Length > bytesLimit)
				{
					throw new MimeException(Strings.TooManyTextValueBytes(mimeStringList.Length, bytesLimit));
				}
			}
			throw new MimeException(Strings.TooManyAddressItems(i, countLimit));
		}

		internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
		{
			if (!(newChild is MimeRecipient) && !(newChild is MimeGroup))
			{
				throw new ArgumentException(Strings.NewChildNotRecipientOrGroup, "newChild");
			}
			return refChild;
		}

		internal override void AppendLine(MimeString line, bool markDirty)
		{
			base.AppendLine(line, markDirty);
			this.parsed = false;
		}

		internal MimeNode ParseNextMailBox(bool fromGroup)
		{
			if (this.parsed)
			{
				return null;
			}
			DecodingOptions headerDecodingOptions = base.GetHeaderDecodingOptions();
			if (this.parser == null)
			{
				this.parser = new MimeAddressParser();
			}
			if (!this.parser.Initialized)
			{
				this.parser.Initialize(base.Lines, false, false, headerDecodingOptions.AllowUTF8);
			}
			MimeStringList displayNameFragments = default(MimeStringList);
			MimeStringList mimeStringList = default(MimeStringList);
			AddressParserResult addressParserResult = this.parser.ParseNextMailbox(ref displayNameFragments, ref mimeStringList);
			switch (addressParserResult)
			{
			case AddressParserResult.Mailbox:
			case AddressParserResult.GroupInProgress:
			{
				MimeRecipient mimeRecipient = new MimeRecipient(ref mimeStringList, ref displayNameFragments);
				if (this.staticParsing)
				{
					MimeRecipient.ConvertDisplayNameBack(mimeRecipient, displayNameFragments, headerDecodingOptions.AllowUTF8);
				}
				if (addressParserResult == AddressParserResult.GroupInProgress)
				{
					MimeGroup mimeGroup = base.InternalLastChild as MimeGroup;
					mimeGroup.InternalInsertAfter(mimeRecipient, mimeGroup.InternalLastChild);
					return mimeRecipient;
				}
				base.InternalInsertAfter(mimeRecipient, base.InternalLastChild);
				if (!fromGroup)
				{
					return mimeRecipient;
				}
				return null;
			}
			case AddressParserResult.GroupStart:
			{
				MimeGroup mimeGroup = new MimeGroup(ref displayNameFragments);
				if (this.staticParsing)
				{
					MimeRecipient.ConvertDisplayNameBack(mimeGroup, displayNameFragments, headerDecodingOptions.AllowUTF8);
				}
				base.InternalInsertAfter(mimeGroup, base.InternalLastChild);
				return mimeGroup;
			}
			case AddressParserResult.End:
				return null;
			default:
				return null;
			}
		}

		private void Parse()
		{
			while (!this.parsed)
			{
				this.ParseNextChild();
			}
			if (this.staticParsing)
			{
				this.staticParsing = false;
			}
		}

		internal override void ForceParse()
		{
			this.Parse();
		}

		internal const bool AllowUTF8Value = true;

		private bool staticParsing;

		private bool parsed;

		private MimeAddressParser parser;
	}
}
