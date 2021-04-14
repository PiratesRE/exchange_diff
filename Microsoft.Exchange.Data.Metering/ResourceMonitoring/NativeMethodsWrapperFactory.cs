using System;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	public class NativeMethodsWrapperFactory
	{
		internal static Func<INativeMethodsWrapper> CreateNativeMethodsWrapperFunc
		{
			get
			{
				return NativeMethodsWrapperFactory.createNativeMethodsWrapperFunc;
			}
			set
			{
				NativeMethodsWrapperFactory.createNativeMethodsWrapperFunc = value;
			}
		}

		internal static INativeMethodsWrapper CreateNativeMethodsWrapper()
		{
			return NativeMethodsWrapperFactory.CreateNativeMethodsWrapperFunc();
		}

		private static INativeMethodsWrapper CreateRealNativeMethodsWrapper()
		{
			return new NativeMethodsWrapper();
		}

		private static Func<INativeMethodsWrapper> createNativeMethodsWrapperFunc = new Func<INativeMethodsWrapper>(NativeMethodsWrapperFactory.CreateRealNativeMethodsWrapper);
	}
}
