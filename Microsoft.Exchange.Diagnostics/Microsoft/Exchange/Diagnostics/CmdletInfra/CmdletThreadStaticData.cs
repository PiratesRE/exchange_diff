using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal static class CmdletThreadStaticData
	{
		internal static int? CacheHitCount
		{
			get
			{
				return CmdletThreadStaticData.GetValue<int>("CacheHitCount", 0);
			}
			set
			{
				CmdletThreadStaticData.SetValue<int?>("CacheHitCount", value);
			}
		}

		internal static int? CacheMissCount
		{
			get
			{
				return CmdletThreadStaticData.GetValue<int>("CacheMissCount", 0);
			}
			set
			{
				CmdletThreadStaticData.SetValue<int?>("CacheMissCount", value);
			}
		}

		private static bool OnlyOneActiveCmdletInCurrentThread
		{
			get
			{
				return CmdletThreadStaticData.registeredCmdletUniqueIds != null && CmdletThreadStaticData.registeredCmdletUniqueIds.Count == 1;
			}
		}

		internal static bool TryGetCurrentCmdletUniqueId(out Guid cmdletUniqueId)
		{
			cmdletUniqueId = Guid.Empty;
			if (!CmdletThreadStaticData.OnlyOneActiveCmdletInCurrentThread)
			{
				return false;
			}
			cmdletUniqueId = CmdletThreadStaticData.registeredCmdletUniqueIds[0];
			return true;
		}

		internal static void RegisterCmdletUniqueId(Guid cmdletUniqueId)
		{
			if (CmdletThreadStaticData.registeredCmdletUniqueIds == null)
			{
				CmdletThreadStaticData.registeredCmdletUniqueIds = new List<Guid>();
			}
			CmdletThreadStaticData.registeredCmdletUniqueIds.Add(cmdletUniqueId);
		}

		internal static void UnRegisterCmdletUniqueId(Guid cmdletUniqueId)
		{
			if (CmdletThreadStaticData.registeredCmdletUniqueIds == null)
			{
				return;
			}
			CmdletThreadStaticData.registeredCmdletUniqueIds.Remove(cmdletUniqueId);
			if (CmdletThreadStaticData.currentThreadValueDic != null)
			{
				CmdletThreadStaticData.currentThreadValueDic.Clear();
			}
		}

		private static T? GetValue<T>(string key, T defaultValue) where T : struct
		{
			if (!CmdletThreadStaticData.OnlyOneActiveCmdletInCurrentThread)
			{
				return null;
			}
			object obj;
			if (CmdletThreadStaticData.currentThreadValueDic == null || !CmdletThreadStaticData.currentThreadValueDic.TryGetValue(key, out obj))
			{
				return new T?(defaultValue);
			}
			return new T?((T)((object)obj));
		}

		private static void SetValue<T>(string key, T value)
		{
			if (!CmdletThreadStaticData.OnlyOneActiveCmdletInCurrentThread)
			{
				return;
			}
			if (CmdletThreadStaticData.currentThreadValueDic == null)
			{
				CmdletThreadStaticData.currentThreadValueDic = new Dictionary<string, object>();
			}
			CmdletThreadStaticData.currentThreadValueDic[key] = value;
		}

		[ThreadStatic]
		private static List<Guid> registeredCmdletUniqueIds;

		[ThreadStatic]
		private static Dictionary<string, object> currentThreadValueDic;
	}
}
