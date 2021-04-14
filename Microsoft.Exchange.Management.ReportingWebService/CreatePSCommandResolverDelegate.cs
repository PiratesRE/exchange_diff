using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.ReportingWebService.PowerShell;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal delegate IPSCommandResolver CreatePSCommandResolverDelegate(IEnumerable<string> snapIns);
}
