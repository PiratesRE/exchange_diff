using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class MailInfo
	{
		public DateTime DocumentTime { get; set; }

		public ulong Key { get; set; }

		public LshFingerprint Fingerprint { get; set; }

		public ulong SenderDomainHash { get; set; }

		public ulong SenderHash { get; set; }

		public ulong[] RecipientsDomainHash { get; set; }

		public int RecipientNumber { get; set; }

		public ulong SubjectHash { get; set; }

		public ulong ClientIpHash { get; set; }

		public ulong ClientIp24Hash { get; set; }

		public DirectionEnum EmailDirection { get; set; }

		public bool SenFeed { get; set; }

		public bool HoneypotFeed { get; set; }

		public bool FnFeed { get; set; }

		public bool ThirdPartyFeed { get; set; }

		public bool SewrFeed { get; set; }

		public bool SpamVerdict { get; set; }
	}
}
