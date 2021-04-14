using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Routing;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal class CompositeAddressFinder : IAddressFinder
	{
		public CompositeAddressFinder(params IAddressFinder[] addressFinders)
		{
			ArgumentValidator.ThrowIfNull("addressFinders", addressFinders);
			this.addressFinders = addressFinders;
		}

		IRoutingKey[] IAddressFinder.Find(AddressFinderSource source, IAddressFinderDiagnostics diagnostics)
		{
			AddressFinderHelper.ThrowIfNull(source, diagnostics);
			ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[CompositeAddressFinder::Find]: Number of addressFinders {0}.", this.addressFinders.Length);
			if (this.addressFinders.Length == 0)
			{
				return AddressFinderHelper.EmptyRoutingKeyArray;
			}
			List<IRoutingKey> list = new List<IRoutingKey>(this.addressFinders.Length);
			foreach (IAddressFinder addressFinder in this.addressFinders)
			{
				if (addressFinder != null)
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<Type>((long)this.GetHashCode(), "[CompositeAddressFinder::Find]: addressFinder {0}.", addressFinder.GetType());
					IRoutingKey[] collection = addressFinder.Find(source, diagnostics);
					if (!collection.IsNullOrEmpty())
					{
						list.AddRange(collection);
					}
				}
				else
				{
					ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[CompositeAddressFinder::Find]: addressFinder is null.");
				}
			}
			if (list.Count == 0)
			{
				return AddressFinderHelper.EmptyRoutingKeyArray;
			}
			return list.ToArray();
		}

		private readonly IAddressFinder[] addressFinders;
	}
}
