using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IPeopleConnectApplicationConfigADReader
	{
		AuthServer ReadFacebookAuthServer();

		AuthServer ReadLinkedInAuthServer();

		string ReadWebProxyUri();

		string ReadFacebookGraphApiEndpoint();

		string ReadLinkedInProfileEndpoint();

		string ReadLinkedInConnectionsEndpoint();

		string ReadLinkedInInvalidateTokenEndpoint();
	}
}
