using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Diagnostics
{
	internal interface IDiagnosable
	{
		string GetDiagnosticComponentName();

		XElement GetDiagnosticInfo(DiagnosableParameters parameters);
	}
}
