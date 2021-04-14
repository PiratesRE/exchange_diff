using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Exchange.Net
{
	internal class TargetHost
	{
		public TargetHost(string targetName, IPAddress[] addresses, TimeSpan timeToLive) : this(targetName, new List<IPAddress>(addresses), timeToLive)
		{
		}

		public TargetHost(string targetName, List<IPAddress> addresses, TimeSpan timeToLive)
		{
			this.name = targetName;
			this.addresses = addresses;
			this.TimeToLive = timeToLive;
		}

		private TargetHost()
		{
		}

		public List<IPAddress> IPAddresses
		{
			get
			{
				return this.addresses;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public TimeSpan TimeToLive
		{
			get
			{
				return this.timeToLive;
			}
			private set
			{
				this.timeToLive = value;
			}
		}

		public override string ToString()
		{
			return string.Format("Name={0};TTL={1};IPs={2}", this.name, this.timeToLive, (this.addresses == null) ? string.Empty : string.Join<IPAddress>(",", this.addresses));
		}

		private List<IPAddress> addresses;

		private string name;

		private TimeSpan timeToLive;
	}
}
