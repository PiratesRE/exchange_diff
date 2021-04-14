using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations
{
	internal class UnknownRoutingDestination : RoutingDestinationBase
	{
		public UnknownRoutingDestination(string type, string value, IList<string> properties)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			this.type = type;
			this.value = value;
			this.properties = properties;
		}

		public override RoutingItemType RoutingItemType
		{
			get
			{
				return RoutingItemType.Unknown;
			}
		}

		public override IList<string> Properties
		{
			get
			{
				return this.properties;
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

		private readonly IList<string> properties;

		private readonly string value;
	}
}
