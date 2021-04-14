using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Entities.People.DataProviders
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ADContactDataProvider
	{
		internal ADContactDataProvider(IRecipientSession recipientSession, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.recipientSession = recipientSession;
			this.tracer = tracer;
		}

		internal Result<ADRawEntry>[] GetBatchADObjects(ProxyAddress[] proxyAddresses)
		{
			ArgumentValidator.ThrowIfNull("proxyAddresses", proxyAddresses);
			if (proxyAddresses.Length == 0)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "ADContactDataProvider.GetBatchADObjects: no proxy addresses to get AD objects");
				return Array<Result<ADRawEntry>>.Empty;
			}
			ADPersonToContactConverterSet organizationalContactProperties = ADPersonToContactConverterSet.OrganizationalContactProperties;
			return this.recipientSession.FindByProxyAddresses(proxyAddresses, organizationalContactProperties.ADProperties);
		}

		private readonly IRecipientSession recipientSession;

		private readonly ITracer tracer;
	}
}
