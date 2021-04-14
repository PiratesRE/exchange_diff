using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ExRpcModule
	{
		public static void Bind()
		{
			if (ExRpcModule.exrpcInited)
			{
				return;
			}
			lock (ExRpcModule.syncLock)
			{
				if (!ExRpcModule.exrpcInited)
				{
					if (ComponentTrace<MapiNetTags>.CheckEnabled(60))
					{
						ComponentTrace<MapiNetTags>.Trace(9010, 60, 0L, "ExRpcModule.Bind");
					}
					try
					{
						int num = NativeMethods.EcInitProvider(out ExRpcModule.pPerfData);
						if (num != 0)
						{
							throw MapiExceptionHelper.LowLevelInitializationFailureException(string.Format("ec={0} (0x{1})", num.ToString(), num.ToString("x")));
						}
						ExRpcModule.exrpcInited = true;
					}
					catch (DllNotFoundException)
					{
						throw MapiExceptionHelper.LowLevelInitializationFailureException("Unable to load exrpc32.dll or one of its dependent DLLs (extrace.dll, exchmem.dll, msvcr90.dll, etc)");
					}
				}
			}
		}

		public static IExRpcManageInterface GetExRpcManage()
		{
			ExRpcModule.Bind();
			SafeExRpcManageHandle result = null;
			int num = NativeMethods.EcGetExRpcManage(out result);
			if (num != 0)
			{
				MapiExceptionHelper.ThrowIfError("Unable to create ExRpcManage interface.", num);
			}
			return result;
		}

		private static bool exrpcInited = false;

		internal static IntPtr pPerfData;

		private static object syncLock = new object();
	}
}
