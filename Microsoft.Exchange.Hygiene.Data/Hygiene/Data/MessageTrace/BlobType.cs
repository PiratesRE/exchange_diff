using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal struct BlobType
	{
		public BlobType(string value)
		{
			this.value = value;
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		private readonly string value;
	}
}
