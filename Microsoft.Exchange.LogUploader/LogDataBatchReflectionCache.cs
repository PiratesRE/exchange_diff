using System;

namespace Microsoft.Exchange.LogUploader
{
	internal static class LogDataBatchReflectionCache<T> where T : LogDataBatch
	{
		internal static bool IsRawBatch
		{
			get
			{
				if (LogDataBatchReflectionCache<T>.attrCache == null)
				{
					object[] customAttributes = typeof(T).GetCustomAttributes(typeof(LogDataBatchAttribute), false);
					LogDataBatchReflectionCache<T>.attrCache = ((customAttributes.Length == 1) ? ((LogDataBatchAttribute)customAttributes[0]) : new LogDataBatchAttribute());
				}
				return LogDataBatchReflectionCache<T>.attrCache.IsRawBatch;
			}
		}

		internal static bool IsMessageBatch
		{
			get
			{
				if (LogDataBatchReflectionCache<T>.isMessageBatch == null)
				{
					LogDataBatchReflectionCache<T>.isMessageBatch = new bool?(typeof(MessageBatchBase).IsAssignableFrom(typeof(T)));
				}
				return LogDataBatchReflectionCache<T>.isMessageBatch.Value;
			}
		}

		private static LogDataBatchAttribute attrCache;

		private static bool? isMessageBatch;
	}
}
