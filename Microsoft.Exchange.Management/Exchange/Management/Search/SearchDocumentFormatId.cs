using System;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Search
{
	[Serializable]
	public class SearchDocumentFormatId : ObjectId
	{
		public SearchDocumentFormatId(string id)
		{
			this.identity = id;
		}

		public SearchDocumentFormatId(byte[] id)
		{
			this.identity = Encoding.Unicode.GetString(id);
		}

		public override byte[] GetBytes()
		{
			return Encoding.Unicode.GetBytes(this.identity);
		}

		public override string ToString()
		{
			return this.identity;
		}

		private readonly string identity;
	}
}
