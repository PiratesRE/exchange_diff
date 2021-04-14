using System;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Mime
{
	public class AsciiTextHeader : Header
	{
		public AsciiTextHeader(string name, string value) : this(name, Header.GetHeaderId(name, true))
		{
			this.Value = value;
		}

		internal AsciiTextHeader(string name, HeaderId headerId) : base(name, headerId)
		{
		}

		public sealed override string Value
		{
			get
			{
				return base.GetRawValue(true);
			}
			set
			{
				base.SetRawValue(value, true, true);
			}
		}

		public sealed override bool RequiresSMTPUTF8
		{
			get
			{
				return !MimeString.IsPureASCII(base.Lines);
			}
		}

		public sealed override MimeNode Clone()
		{
			AsciiTextHeader asciiTextHeader = new AsciiTextHeader(base.Name, base.HeaderId);
			this.CopyTo(asciiTextHeader);
			return asciiTextHeader;
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
			if (!(destination is AsciiTextHeader))
			{
				base.CopyTo(destination);
				return;
			}
			base.CopyTo(destination);
		}

		public sealed override bool IsValueValid(string value)
		{
			return true;
		}

		internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
		{
			throw new NotSupportedException(Strings.AddingChildrenToAsciiTextHeader);
		}

		internal const bool AllowUTF8Value = true;
	}
}
