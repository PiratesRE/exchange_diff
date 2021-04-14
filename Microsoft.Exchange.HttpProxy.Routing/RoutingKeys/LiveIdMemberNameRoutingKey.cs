using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingKeys
{
	public class LiveIdMemberNameRoutingKey : RoutingKeyBase
	{
		public LiveIdMemberNameRoutingKey(SmtpAddress liveIdMemberName, string organizationContext)
		{
			if (!liveIdMemberName.IsValidAddress)
			{
				throw new ArgumentException("The liveid member name is not valid", "liveIdMemberName");
			}
			if (!string.IsNullOrEmpty(organizationContext) && !SmtpAddress.IsValidDomain(organizationContext))
			{
				throw new ArgumentException("The organizationContext is not valid", "organizationContext");
			}
			this.liveIdMemberName = liveIdMemberName;
			this.organizationContext = organizationContext;
		}

		public override RoutingItemType RoutingItemType
		{
			get
			{
				return RoutingItemType.LiveIdMemberName;
			}
		}

		public override string Value
		{
			get
			{
				return this.LiveIdMemberName + ';' + this.OrganizationContext;
			}
		}

		public SmtpAddress LiveIdMemberName
		{
			get
			{
				return this.liveIdMemberName;
			}
		}

		public string OrganizationContext
		{
			get
			{
				return this.organizationContext;
			}
		}

		public string OrganizationDomain
		{
			get
			{
				string domain = this.LiveIdMemberName.Domain;
				if (!string.IsNullOrEmpty(this.OrganizationContext))
				{
					domain = this.OrganizationContext;
				}
				return domain;
			}
		}

		public static bool TryParse(string value, out LiveIdMemberNameRoutingKey key)
		{
			key = null;
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			int num = value.IndexOf(';');
			if (num == -1)
			{
				return false;
			}
			string address = value.Substring(0, num);
			if (!SmtpAddress.IsValidSmtpAddress(address))
			{
				return false;
			}
			string domain = null;
			if (num + 1 < value.Length)
			{
				domain = value.Substring(num + 1);
				if (!SmtpAddress.IsValidDomain(domain))
				{
					return false;
				}
			}
			key = new LiveIdMemberNameRoutingKey(new SmtpAddress(address), domain);
			return true;
		}

		private const char Separator = ';';

		private readonly SmtpAddress liveIdMemberName;

		private readonly string organizationContext;
	}
}
