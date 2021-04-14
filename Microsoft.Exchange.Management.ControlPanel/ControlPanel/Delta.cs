using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public struct Delta<T>
	{
		public List<T> AddedObjects;

		public List<T> RemovedObjects;

		public List<T> UnchangedObjects;
	}
}
