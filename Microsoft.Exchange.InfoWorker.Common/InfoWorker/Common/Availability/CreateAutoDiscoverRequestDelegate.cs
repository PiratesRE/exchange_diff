using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal delegate AutoDiscoverRequestOperation CreateAutoDiscoverRequestDelegate(Application application, ClientContext clientContext, RequestLogger requestLogger, Uri targetUri, AutoDiscoverAuthenticator authenticator, EmailAddress[] emailAddresses, UriSource uriSource, AutodiscoverType autodiscoverType);
}
