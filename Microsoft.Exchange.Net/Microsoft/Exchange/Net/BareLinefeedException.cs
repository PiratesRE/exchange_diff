using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	public class BareLinefeedException : LocalizedException
	{
		public long Position
		{
			get
			{
				return this.position;
			}
		}

		public BareLinefeedException() : this(-1)
		{
		}

		public BareLinefeedException(int position) : base(NetException.DataContainsBareLinefeeds)
		{
			this.position = (long)position;
		}

		private long position;
	}
}
