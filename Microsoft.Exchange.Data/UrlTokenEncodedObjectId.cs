using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class UrlTokenEncodedObjectId : ObjectId
	{
		public UrlTokenEncodedObjectId(string rawValue)
		{
			this.urlTokenEncodedValue = UrlTokenConverter.UrlTokenEncode(rawValue);
		}

		public override byte[] GetBytes()
		{
			if (this.urlTokenEncodedValue == null)
			{
				return new byte[0];
			}
			return Encoding.UTF8.GetBytes(this.urlTokenEncodedValue);
		}

		public override string ToString()
		{
			return this.urlTokenEncodedValue;
		}

		private readonly string urlTokenEncodedValue;
	}
}
