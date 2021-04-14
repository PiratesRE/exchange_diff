using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.OData
{
	internal abstract class ODataCommand : DisposeTrackableBase
	{
		public abstract object Execute();
	}
}
