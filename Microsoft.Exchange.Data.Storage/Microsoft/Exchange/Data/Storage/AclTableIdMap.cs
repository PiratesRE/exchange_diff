using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AclTableIdMap
	{
		public long GetIdForSecurityIdentifier(SecurityIdentifier securityIdentifier, List<SecurityIdentifier> sidHistory)
		{
			long num;
			if (this.securityIdentifierToIdMap.TryGetValue(securityIdentifier, out num))
			{
				return num;
			}
			if (sidHistory != null)
			{
				foreach (SecurityIdentifier key in sidHistory)
				{
					if (this.securityIdentifierToIdMap.TryGetValue(key, out num))
					{
						return num;
					}
				}
			}
			long num2;
			this.nextId = (num2 = this.nextId) + 1L;
			num = num2;
			this.securityIdentifierToIdMap[securityIdentifier] = num;
			if (sidHistory != null)
			{
				foreach (SecurityIdentifier key2 in sidHistory)
				{
					this.securityIdentifierToIdMap[key2] = num;
				}
			}
			return num;
		}

		private readonly Dictionary<SecurityIdentifier, long> securityIdentifierToIdMap = new Dictionary<SecurityIdentifier, long>();

		private long nextId = 1000L;
	}
}
