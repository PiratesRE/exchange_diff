using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ModernGroupMemberComparerByAdObjectId : IEqualityComparer<ModernGroupMemberType>
	{
		private ModernGroupMemberComparerByAdObjectId()
		{
		}

		public bool Equals(ModernGroupMemberType member1, ModernGroupMemberType member2)
		{
			bool flag = member1 == null || member1.Persona == null;
			bool flag2 = member2 == null || member2.Persona == null;
			return (flag && flag2) || (!flag && !flag2 && member1.Persona.ADObjectId == member2.Persona.ADObjectId);
		}

		public int GetHashCode(ModernGroupMemberType member)
		{
			if (member == null || member.Persona == null)
			{
				return 0;
			}
			return member.Persona.ADObjectId.GetHashCode();
		}

		internal static readonly ModernGroupMemberComparerByAdObjectId Singleton = new ModernGroupMemberComparerByAdObjectId();
	}
}
