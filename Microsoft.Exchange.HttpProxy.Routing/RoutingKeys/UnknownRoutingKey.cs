using System;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingKeys
{
	internal class UnknownRoutingKey : RoutingKeyBase
	{
		public UnknownRoutingKey(string type, string value)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.type = type;
			this.value = value;
		}

		public override RoutingItemType RoutingItemType
		{
			get
			{
				return RoutingItemType.Unknown;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		public override string Value
		{
			get
			{
				return this.value;
			}
		}

		private readonly string type;

		private readonly string value;
	}
}
