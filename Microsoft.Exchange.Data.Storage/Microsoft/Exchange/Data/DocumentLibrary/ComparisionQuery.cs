using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ComparisionQuery<GPropType> : SinglePropertyQuery
	{
		protected ComparisionQuery(int index, GPropType propValue) : base(index)
		{
			if (propValue == null)
			{
				throw new ArgumentNullException("invalid propValue");
			}
			this.PropValue = propValue;
		}

		protected readonly GPropType PropValue;
	}
}
