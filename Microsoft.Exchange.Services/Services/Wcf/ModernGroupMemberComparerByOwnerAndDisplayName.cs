using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ModernGroupMemberComparerByOwnerAndDisplayName : IComparer<ModernGroupMemberType>
	{
		private ModernGroupMemberComparerByOwnerAndDisplayName()
		{
		}

		public int Compare(ModernGroupMemberType member1, ModernGroupMemberType member2)
		{
			bool flag = member1 == null;
			bool flag2 = member2 == null;
			if (flag && flag2)
			{
				return 0;
			}
			if (flag)
			{
				return 1;
			}
			if (flag2)
			{
				return -1;
			}
			if (member1.IsOwner == member2.IsOwner)
			{
				return ModernGroupMemberComparerByDisplayName.Singleton.Compare(member1, member2);
			}
			if (member1.IsOwner)
			{
				return -1;
			}
			return 1;
		}

		internal static readonly ModernGroupMemberComparerByOwnerAndDisplayName Singleton = new ModernGroupMemberComparerByOwnerAndDisplayName();
	}
}
