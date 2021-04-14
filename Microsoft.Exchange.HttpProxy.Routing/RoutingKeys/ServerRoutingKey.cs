using System;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingKeys
{
	internal class ServerRoutingKey : RoutingKeyBase
	{
		public ServerRoutingKey(string fqdn) : this(fqdn, null)
		{
		}

		public ServerRoutingKey(string fqdn, int? version)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentException("The FQDN should not be null or empty.", "fqdn");
			}
			this.fqdn = fqdn;
			this.version = version;
		}

		public override RoutingItemType RoutingItemType
		{
			get
			{
				return RoutingItemType.Server;
			}
		}

		public override string Value
		{
			get
			{
				if (this.version != null)
				{
					return this.fqdn + '~' + this.version;
				}
				return this.fqdn;
			}
		}

		public string Server
		{
			get
			{
				return this.fqdn;
			}
		}

		public int? Version
		{
			get
			{
				return this.version;
			}
		}

		public static bool TryParse(string value, out ServerRoutingKey key)
		{
			key = null;
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			string text;
			string text2;
			Utilities.GetTwoSubstrings(value, '~', out text, out text2);
			if (string.IsNullOrEmpty(text2))
			{
				key = new ServerRoutingKey(text);
				return true;
			}
			int value2;
			if (!int.TryParse(text2, out value2))
			{
				return false;
			}
			key = new ServerRoutingKey(text, new int?(value2));
			return true;
		}

		private const char Separator = '~';

		private readonly string fqdn;

		private readonly int? version;
	}
}
