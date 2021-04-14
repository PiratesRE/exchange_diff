using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingKeys
{
	internal class SmtpRoutingKey : RoutingKeyBase
	{
		public SmtpRoutingKey(SmtpAddress smtpAddress)
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
				return RoutingItemType.Smtp;
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

		public static bool TryParse(string value, out SmtpRoutingKey key)
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
			key = new SmtpRoutingKey(new SmtpAddress(value));
			return true;
		}

		private readonly SmtpAddress smtpAddress;
	}
}
