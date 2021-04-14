using System;

namespace Microsoft.Exchange.Net
{
	internal class SoaRecord
	{
		public SoaRecord(string primaryServer, string administrator, int serialNumber, int refresh, int retry, int expire, int defaultTimeToLive)
		{
			this.primaryServer = primaryServer;
			this.administrator = administrator;
			this.serialNumber = serialNumber;
			this.refresh = refresh;
			this.retry = retry;
			this.expire = expire;
			this.defaultTimeToLive = defaultTimeToLive;
		}

		private SoaRecord()
		{
		}

		public string PrimaryServer
		{
			get
			{
				return this.primaryServer;
			}
		}

		public string Administrator
		{
			get
			{
				return this.administrator;
			}
		}

		public int SerialNumber
		{
			get
			{
				return this.serialNumber;
			}
		}

		public int Refresh
		{
			get
			{
				return this.refresh;
			}
		}

		public int Retry
		{
			get
			{
				return this.retry;
			}
		}

		public int Expire
		{
			get
			{
				return this.expire;
			}
		}

		public int DefaultTimeToLive
		{
			get
			{
				return this.defaultTimeToLive;
			}
		}

		private readonly string primaryServer;

		private readonly string administrator;

		private readonly int serialNumber;

		private readonly int refresh;

		private readonly int retry;

		private readonly int expire;

		private readonly int defaultTimeToLive;
	}
}
