using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class SinglePropertyQuery : Query
	{
		protected SinglePropertyQuery(int index)
		{
			this.Index = index;
		}

		protected readonly int Index;
	}
}
