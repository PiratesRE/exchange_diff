using System;

namespace Microsoft.Exchange.Transport.Configuration
{
	public delegate void ConfigurationUpdateHandler<TCache>(TCache update) where TCache : class;
}
