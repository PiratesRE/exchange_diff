using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class SecurityContextsQueryTarget : IStoreSimpleQueryTarget<CountedClientSecurityContext>, IStoreQueryTargetBase<CountedClientSecurityContext>
	{
		private SecurityContextsQueryTarget()
		{
			StoreQueryTargets.Register<CountedClientSecurityContext>(this, Visibility.Public);
		}

		public static SecurityContextsQueryTarget Create()
		{
			return new SecurityContextsQueryTarget();
		}

		string IStoreQueryTargetBase<CountedClientSecurityContext>.Name
		{
			get
			{
				return "CountedSecurityContext";
			}
		}

		Type[] IStoreQueryTargetBase<CountedClientSecurityContext>.ParameterTypes
		{
			get
			{
				return Array<Type>.Empty;
			}
		}

		IEnumerable<CountedClientSecurityContext> IStoreSimpleQueryTarget<CountedClientSecurityContext>.GetRows(object[] parameters)
		{
			IEnumerable<SecurityContextKey> keysCopy = SecurityContextManager.GetKeysInDictionary();
			foreach (SecurityContextKey key in keysCopy)
			{
				CountedClientSecurityContext info = SecurityContextManager.GetValueForKey(key);
				if (info != null)
				{
					yield return info;
				}
			}
			yield break;
		}
	}
}
