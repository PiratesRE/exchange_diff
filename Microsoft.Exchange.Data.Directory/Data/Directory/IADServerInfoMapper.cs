using System;
using System.DirectoryServices.Protocols;

namespace Microsoft.Exchange.Data.Directory
{
	internal interface IADServerInfoMapper
	{
		ADRole GetADRole(ADServerInfo adServerInfo);

		string GetMappedFqdn(string serverFqdn);

		int GetMappedPortNumber(string serverFqdn, string forestFqdn, int portNumber);

		AuthType GetMappedAuthType(string serverFqdn, AuthType authType);
	}
}
