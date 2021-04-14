using System;
using System.Text;

namespace Microsoft.Exchange.Common.Cache
{
	internal class CachableString : CachableItem
	{
		public string StringItem { get; private set; }

		public CachableString(string value)
		{
			this.StringItem = value;
		}

		public override long ItemSize
		{
			get
			{
				return (long)Encoding.Unicode.GetByteCount(this.StringItem);
			}
		}
	}
}
