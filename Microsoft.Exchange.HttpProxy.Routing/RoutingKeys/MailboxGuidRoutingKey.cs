using System;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingKeys
{
	internal class MailboxGuidRoutingKey : RoutingKeyBase
	{
		public MailboxGuidRoutingKey(Guid mailboxGuid, string tenantDomain)
		{
			if (tenantDomain == null)
			{
				throw new ArgumentNullException("tenantDomain");
			}
			this.mailboxGuid = mailboxGuid;
			this.tenantDomain = tenantDomain;
		}

		public override RoutingItemType RoutingItemType
		{
			get
			{
				return RoutingItemType.MailboxGuid;
			}
		}

		public override string Value
		{
			get
			{
				return this.MailboxGuid + '@' + this.TenantDomain;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public string TenantDomain
		{
			get
			{
				return this.tenantDomain;
			}
		}

		public static bool TryParse(string value, out MailboxGuidRoutingKey key)
		{
			key = null;
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int num = value.IndexOf('@');
			if (num == -1)
			{
				return false;
			}
			string input = value.Substring(0, num);
			Guid guid;
			if (!Guid.TryParse(input, out guid))
			{
				return false;
			}
			key = new MailboxGuidRoutingKey(guid, value.Substring(num + 1));
			return true;
		}

		private const char Separator = '@';

		private readonly Guid mailboxGuid;

		private readonly string tenantDomain;
	}
}
