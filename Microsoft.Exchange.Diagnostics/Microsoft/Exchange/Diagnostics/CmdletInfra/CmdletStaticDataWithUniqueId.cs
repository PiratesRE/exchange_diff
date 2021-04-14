using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal static class CmdletStaticDataWithUniqueId<T>
	{
		private static Dictionary<Guid, T> Data
		{
			get
			{
				Dictionary<Guid, T> result;
				if ((result = CmdletStaticDataWithUniqueId<T>.data) == null)
				{
					result = (CmdletStaticDataWithUniqueId<T>.data = new Dictionary<Guid, T>());
				}
				return result;
			}
		}

		internal static bool ContainsKey(Guid cmdletUniqueId)
		{
			return CmdletStaticDataWithUniqueId<T>.Data.ContainsKey(cmdletUniqueId);
		}

		internal static T Get(Guid cmdletUniqueId)
		{
			T result;
			CmdletStaticDataWithUniqueId<T>.TryGet(cmdletUniqueId, out result);
			return result;
		}

		internal static bool TryGet(Guid cmdletUniqueId, out T output)
		{
			return CmdletStaticDataWithUniqueId<T>.Data.TryGetValue(cmdletUniqueId, out output);
		}

		internal static void Set(Guid cmdletUniqueId, T value)
		{
			CmdletStaticDataWithUniqueId<T>.Data[cmdletUniqueId] = value;
		}

		internal static bool Remove(Guid cmdletUniqueId)
		{
			return CmdletStaticDataWithUniqueId<T>.Data.Remove(cmdletUniqueId);
		}

		[ThreadStatic]
		private static Dictionary<Guid, T> data;
	}
}
