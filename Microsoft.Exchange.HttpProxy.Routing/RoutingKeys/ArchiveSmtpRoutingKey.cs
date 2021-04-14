using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingKeys
{
	internal class ArchiveSmtpRoutingKey : RoutingKeyBase
	{
		public ArchiveSmtpRoutingKey(SmtpAddress smtpAddress)
		{
			if (!smtpAddress.IsValidAddress)
			{
				throw new ArgumentException("The SMTP address is not a valid address", "smtpAddress");
			}
			this.smtpAddress = smtpAddress;
		}

		public override RoutingItemType RoutingItemType
		{
			get
			{
				return RoutingItemType.ArchiveSmtp;
			}
		}

		public override string Value
		{
			get
			{
				return this.SmtpAddress.ToString();
			}
		}

		public SmtpAddress SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
		}

		public static bool TryParse(string value, out ArchiveSmtpRoutingKey key)
		{
			key = null;
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!SmtpAddress.IsValidSmtpAddress(value))
			{
				return false;
			}
			key = new ArchiveSmtpRoutingKey(new SmtpAddress(value));
			return true;
		}

		private readonly SmtpAddress smtpAddress;
	}
}
