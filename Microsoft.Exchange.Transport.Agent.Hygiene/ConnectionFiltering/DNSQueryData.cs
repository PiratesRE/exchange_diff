using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Agent.ConnectionFiltering
{
	internal class DNSQueryData
	{
		public DNSQueryData(IPAddress hostIP, object source, ReceiveEventArgs eventArgs, bool safeListEnabled, bool blockListEnabled)
		{
			this.HostIP = hostIP;
			this.ReverseIP = DNSQueryData.ConvertToReverseIP(hostIP);
			this.Source = source;
			this.EventArgs = eventArgs;
			this.SafeListEnabled = safeListEnabled;
			this.BlockListEnabled = blockListEnabled;
			if (source is ReceiveCommandEventSource)
			{
				this.CurrentEvent = SMTPEvent.RcptTo;
				return;
			}
			this.CurrentEvent = SMTPEvent.EOH;
		}

		public static string ConvertToReverseIP(IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.AddressFamily != AddressFamily.InterNetwork)
			{
				throw new ArgumentException("address must be IPv4.", "address");
			}
			byte[] addressBytes = address.GetAddressBytes();
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", new object[]
			{
				addressBytes[3],
				addressBytes[2],
				addressBytes[1],
				addressBytes[0]
			});
		}

		public IPAddress HostIP;

		public string ReverseIP;

		public SMTPEvent CurrentEvent;

		public object Source;

		public ReceiveEventArgs EventArgs;

		public IPListProvider Provider;

		public bool SafeListEnabled;

		public bool BlockListEnabled;
	}
}
