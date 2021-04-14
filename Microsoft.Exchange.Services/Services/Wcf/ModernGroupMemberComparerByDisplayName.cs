using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ModernGroupMemberComparerByDisplayName : IComparer<ModernGroupMemberType>
	{
		private ModernGroupMemberComparerByDisplayName()
		{
		}

		public int Compare(ModernGroupMemberType member1, ModernGroupMemberType member2)
		{
			bool flag = false;
			bool flag2 = false;
			if (member1 == null || member1.Persona == null)
			{
				flag = true;
			}
			if (member2 == null || member2.Persona == null)
			{
				flag2 = true;
			}
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
			return member1.Persona.DisplayName.CompareTo(member2.Persona.DisplayName);
		}

		internal static readonly ModernGroupMemberComparerByDisplayName Singleton = new ModernGroupMemberComparerByDisplayName();
	}
}
