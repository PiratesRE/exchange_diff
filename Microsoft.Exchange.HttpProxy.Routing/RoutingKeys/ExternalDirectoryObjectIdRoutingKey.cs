using System;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingKeys
{
	public class ExternalDirectoryObjectIdRoutingKey : RoutingKeyBase
	{
		public ExternalDirectoryObjectIdRoutingKey(Guid userGuid, Guid tenantGuid)
		{
			this.userGuid = userGuid;
			this.tenantGuid = tenantGuid;
		}

		public override RoutingItemType RoutingItemType
		{
			get
			{
				return RoutingItemType.ExternalDirectoryObjectId;
			}
		}

		public override string Value
		{
			get
			{
				return this.UserGuid + '@' + this.TenantGuid;
			}
		}

		public Guid UserGuid
		{
			get
			{
				return this.userGuid;
			}
		}

		public Guid TenantGuid
		{
			get
			{
				return this.tenantGuid;
			}
		}

		public static bool TryParse(string value, out ExternalDirectoryObjectIdRoutingKey key)
		{
			key = null;
			if (string.IsNullOrEmpty(value))
			{
				return false;
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
			if (num + 1 == value.Length)
			{
				return false;
			}
			string input2 = value.Substring(num + 1);
			Guid guid2;
			if (!Guid.TryParse(input2, out guid2))
			{
				return false;
			}
			key = new ExternalDirectoryObjectIdRoutingKey(guid, guid2);
			return true;
		}

		private const char Separator = '@';

		private readonly Guid userGuid;

		private readonly Guid tenantGuid;
	}
}
