using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class GlobalNamedPropertyMap
	{
		private static GlobalNamedPropertyMap._CriticalConsistencyBlock Critical()
		{
			return new GlobalNamedPropertyMap._CriticalConsistencyBlock
			{
				CriticalBlockActive = true
			};
		}

		public static void Initialize()
		{
			GlobalNamedPropertyMap.nameToDefinitionMap = new Dictionary<StorePropName, StoreNamedPropInfo>(4096);
			GlobalNamedPropertyMap.lockObject = new object();
		}

		internal static void AddPropDefinition(Context context, ref StoreNamedPropInfo namedPropertyInfo)
		{
			StorePropName propName = namedPropertyInfo.PropName;
			using (GlobalNamedPropertyMap._CriticalConsistencyBlock criticalConsistencyBlock = GlobalNamedPropertyMap.Critical())
			{
				using (LockManager.Lock(GlobalNamedPropertyMap.lockObject, context.Diagnostics))
				{
					if (GlobalNamedPropertyMap.nameToDefinitionMap.Count > 50000)
					{
						GlobalNamedPropertyMap.nameToDefinitionMap.Clear();
					}
					StoreNamedPropInfo storeNamedPropInfo;
					if (!GlobalNamedPropertyMap.nameToDefinitionMap.TryGetValue(propName, out storeNamedPropInfo))
					{
						GlobalNamedPropertyMap.nameToDefinitionMap[propName] = namedPropertyInfo;
					}
					else
					{
						namedPropertyInfo = storeNamedPropInfo;
					}
				}
				criticalConsistencyBlock.Success();
			}
		}

		internal static bool GetDefinitionFromName(Context context, StorePropName propName, out StoreNamedPropInfo namedPropertyInfo)
		{
			bool result = false;
			namedPropertyInfo = null;
			using (GlobalNamedPropertyMap._CriticalConsistencyBlock criticalConsistencyBlock = GlobalNamedPropertyMap.Critical())
			{
				using (LockManager.Lock(GlobalNamedPropertyMap.lockObject, context.Diagnostics))
				{
					if (GlobalNamedPropertyMap.nameToDefinitionMap.TryGetValue(propName, out namedPropertyInfo))
					{
						result = true;
					}
				}
				criticalConsistencyBlock.Success();
			}
			return result;
		}

		private static Dictionary<StorePropName, StoreNamedPropInfo> nameToDefinitionMap;

		private static object lockObject;

		private struct _CriticalConsistencyBlock : IDisposable
		{
			public void Dispose()
			{
				if (this.CriticalBlockActive)
				{
					Globals.AssertRetail(false, "forced crash");
				}
			}

			public void Success()
			{
				this.CriticalBlockActive = false;
			}

			public bool CriticalBlockActive;
		}
	}
}
