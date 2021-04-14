using System;
using System.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.HttpProxy.Routing;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal static class AddressFinderHelper
	{
		public static IRoutingKey[] EmptyRoutingKeyArray
		{
			get
			{
				return AddressFinderHelper.StaticEmptyRoutingKeyArray;
			}
		}

		public static IRoutingKey[] GetRoutingKeyArray(params IRoutingKey[] routingKeys)
		{
			return routingKeys;
		}

		public static bool IsNullOrEmpty(this ICollection collection)
		{
			return collection == null || collection.Count == 0;
		}

		public static void ThrowIfNull(AddressFinderSource source, IAddressFinderDiagnostics diagnostics)
		{
			ArgumentValidator.ThrowIfNull("source", source);
			ArgumentValidator.ThrowIfNull("diagnostics", diagnostics);
		}

		private static readonly IRoutingKey[] StaticEmptyRoutingKeyArray = new IRoutingKey[0];
	}
}
