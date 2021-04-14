using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	public class AsyncLogWrapperFactory
	{
		internal static Func<string, LogHeaderFormatter, string, IAsyncLogWrapper> CreateAsyncLogWrapperFunc
		{
			get
			{
				return AsyncLogWrapperFactory.createAsyncLogWrapperFunc;
			}
			set
			{
				AsyncLogWrapperFactory.createAsyncLogWrapperFunc = value;
			}
		}

		internal static IAsyncLogWrapper CreateAsyncLogWrapper(string logFileName, LogHeaderFormatter logHeaderFormatter, string logComponentName)
		{
			return AsyncLogWrapperFactory.CreateAsyncLogWrapperFunc(logFileName, logHeaderFormatter, logComponentName);
		}

		private static IAsyncLogWrapper CreateRealAsyncLogWrapper(string logFileName, LogHeaderFormatter logHeaderFormatter, string logComponentName)
		{
			return new AsyncLogWrapper(logFileName, logHeaderFormatter, logComponentName);
		}

		private static Func<string, LogHeaderFormatter, string, IAsyncLogWrapper> createAsyncLogWrapperFunc = new Func<string, LogHeaderFormatter, string, IAsyncLogWrapper>(AsyncLogWrapperFactory.CreateRealAsyncLogWrapper);
	}
}
