using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal abstract class ExchangeRpcServerBase : RpcServerBase
	{
		public virtual IProxyServer GetProxyServer()
		{
			return null;
		}

		public ExchangeRpcServerBase()
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ExchangeRpcServerBase()
		{
			ExchangeRpcServerBase.maxBuildMajor = short.MaxValue;
			ExchangeRpcServerBase.criticalBuildMajor = 0;
			ExchangeRpcServerBase.maxProductMinor = 255;
			ExchangeRpcServerBase.criticalProductMinor = 0;
			ExchangeRpcServerBase.maxProductMajor = 127;
			ExchangeRpcServerBase.criticalProductMajor = 6;
		}

		public static short criticalProductMajor = 6;

		public static short maxProductMajor = 127;

		public static short criticalProductMinor = 0;

		public static short maxProductMinor = 255;

		public static short criticalBuildMajor = 0;

		public static short maxBuildMajor = 32767;

		public static short criticalBuildMinor = 0;
	}
}
