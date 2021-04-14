using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAnchorService : IDisposable
	{
		bool Initialize(AnchorContext context);

		void Start();

		IEnumerable<IDiagnosable> GetDiagnosableComponents();
	}
}
