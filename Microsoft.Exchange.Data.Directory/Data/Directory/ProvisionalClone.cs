using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	[AttributeUsage(AttributeTargets.Property)]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ProvisionalClone : ProvisionalCloneBase
	{
		public ProvisionalClone(CloneSet contexts = CloneSet.CloneExtendedSet) : base(contexts)
		{
		}
	}
}
