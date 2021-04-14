using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.Analysis.Features;

namespace Microsoft.Exchange.Management.Deployment
{
	internal interface IManagedMethodProvider
	{
		Dictionary<string, object[]> CheckDNS(string ipAddress, string svrFQDN);

		Dictionary<string, object[]> PortAvailable(string svrName, Dictionary<string, List<string>> commands);

		string GetComputerNameEx(ValidationConstant.ComputerNameFormat computerNameFormat);

		string GetUserNameEx(ValidationConstant.ExtendedNameFormat extendedNameType);
	}
}
