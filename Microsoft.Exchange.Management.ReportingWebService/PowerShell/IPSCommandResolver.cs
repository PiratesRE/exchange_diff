using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ReportingWebService.PowerShell
{
	internal interface IPSCommandResolver
	{
		ReadOnlyCollection<PSTypeName> GetOutputType(string commandName);
	}
}
