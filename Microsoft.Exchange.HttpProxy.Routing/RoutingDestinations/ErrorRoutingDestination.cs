using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations
{
	internal class ErrorRoutingDestination : RoutingDestinationBase
	{
		public ErrorRoutingDestination(string message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			this.message = message;
		}

		public string Message
		{
			get
			{
				return this.message;
			}
		}

		public override RoutingItemType RoutingItemType
		{
			get
			{
				return RoutingItemType.Error;
			}
		}

		public override string Value
		{
			get
			{
				return this.Message;
			}
		}

		public override IList<string> Properties
		{
			get
			{
				return Array<string>.Empty;
			}
		}

		public static bool TryParse(string value, IList<string> properties, out ErrorRoutingDestination destination)
		{
			destination = null;
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			destination = new ErrorRoutingDestination(value);
			return true;
		}

		private readonly string message;
	}
}
