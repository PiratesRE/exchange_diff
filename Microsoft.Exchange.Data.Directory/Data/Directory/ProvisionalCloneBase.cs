using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[AttributeUsage(AttributeTargets.Property)]
	internal class ProvisionalCloneBase : Attribute
	{
		public ProvisionalCloneBase(CloneSet contexts)
		{
			this.CloneSet = contexts;
		}

		public CloneSet CloneSet;
	}
}
