using System;
using System.IO;

namespace Microsoft.Exchange.Data.Mime
{
	public abstract class MimeOutputFilter
	{
		public virtual bool FilterPart(MimePart part, Stream stream)
		{
			return false;
		}

		public virtual bool FilterHeaderList(HeaderList headerList, Stream stream)
		{
			return false;
		}

		public virtual bool FilterHeader(Header header, Stream stream)
		{
			return false;
		}

		public virtual bool FilterPartBody(MimePart part, Stream stream)
		{
			return false;
		}

		public virtual void ClosePart(MimePart part, Stream stream)
		{
		}
	}
}
