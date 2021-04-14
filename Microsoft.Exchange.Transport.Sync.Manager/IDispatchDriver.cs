using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDispatchDriver
	{
		event EventHandler<EventArgs> PrimingEvent;

		void AddDiagnosticInfoTo(XElement componentElement);
	}
}
