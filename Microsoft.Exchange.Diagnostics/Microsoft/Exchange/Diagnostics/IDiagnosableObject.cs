using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Diagnostics
{
	internal interface IDiagnosableObject
	{
		string HashableIdentity { get; }

		XElement GetDiagnosticInfo(string argument);
	}
}
