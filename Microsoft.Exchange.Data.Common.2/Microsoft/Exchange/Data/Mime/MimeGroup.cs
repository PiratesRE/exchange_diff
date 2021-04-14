using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Mime
{
	public class MimeGroup : AddressItem, IEnumerable<MimeRecipient>, IEnumerable
	{
		public MimeGroup()
		{
		}

		public MimeGroup(string displayName) : base(displayName)
		{
		}

		internal MimeGroup(ref MimeStringList displayName) : base(ref displayName)
		{
		}

		public new MimeNode.Enumerator<MimeRecipient> GetEnumerator()
		{
			return new MimeNode.Enumerator<MimeRecipient>(this);
		}

		IEnumerator<MimeRecipient> IEnumerable<MimeRecipient>.GetEnumerator()
		{
			return new MimeNode.Enumerator<MimeRecipient>(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new MimeNode.Enumerator<MimeRecipient>(this);
		}

		public sealed override MimeNode Clone()
		{
			MimeGroup mimeGroup = new MimeGroup();
			this.CopyTo(mimeGroup);
			return mimeGroup;
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
			MimeGroup mimeGroup = destination as MimeGroup;
			if (mimeGroup == null)
			{
				throw new ArgumentException(Strings.CantCopyToDifferentObjectType);
			}
			while (mimeGroup.ParseNextChild() != null)
			{
			}
			while (this.ParseNextChild() != null)
			{
			}
			base.CopyTo(destination);
		}

		internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
		{
			if (!(newChild is MimeRecipient))
			{
				throw new ArgumentException(Strings.NewChildIsNotARecipient);
			}
			return refChild;
		}

		internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
		{
			MimeNode nextSibling = base.NextSibling;
			MimeStringList displayNameToWrite = base.GetDisplayNameToWrite(encodingOptions);
			long num = 0L;
			if (displayNameToWrite.GetLength(4026531839U) != 0)
			{
				int num2 = 1;
				if (base.FirstChild == null)
				{
					num2++;
				}
				if (base.NextSibling != null)
				{
					num2++;
				}
				num += Header.QuoteAndFold(stream, displayNameToWrite, 4026531839U, base.IsQuotingRequired(displayNameToWrite, encodingOptions.AllowUTF8), true, encodingOptions.AllowUTF8, num2, ref currentLineLength, ref scratchBuffer);
				stream.Write(MimeString.Colon, 0, MimeString.Colon.Length);
				num += (long)MimeString.Colon.Length;
				currentLineLength.IncrementBy(MimeString.Colon.Length);
			}
			MimeNode mimeNode = base.FirstChild;
			int num3 = 0;
			while (mimeNode != null)
			{
				if (1 < ++num3)
				{
					stream.Write(MimeString.Comma, 0, MimeString.Comma.Length);
					num += (long)MimeString.Comma.Length;
					currentLineLength.IncrementBy(MimeString.Comma.Length);
				}
				num += mimeNode.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
				mimeNode = mimeNode.NextSibling;
			}
			stream.Write(MimeString.Semicolon, 0, MimeString.Semicolon.Length);
			num += (long)MimeString.Semicolon.Length;
			currentLineLength.IncrementBy(MimeString.Semicolon.Length);
			return num;
		}

		internal override MimeNode ParseNextChild()
		{
			MimeNode mimeNode = null;
			if (!this.parsed && base.Parent != null)
			{
				AddressHeader addressHeader = base.Parent as AddressHeader;
				if (addressHeader != null)
				{
					mimeNode = addressHeader.ParseNextMailBox(true);
				}
			}
			this.parsed = (mimeNode == null);
			return mimeNode;
		}

		private bool parsed;
	}
}
