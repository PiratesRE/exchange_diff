using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class DataConverter<T, TNative, TData> where T : IDataConverter<TNative, TData>, new()
	{
		public static TData GetData(TNative native)
		{
			if (native == null)
			{
				return default(TData);
			}
			return DataConverter<T, TNative, TData>.instance.GetDataRepresentation(native);
		}

		public static TNative GetNative(TData data)
		{
			if (data == null)
			{
				return default(TNative);
			}
			return DataConverter<T, TNative, TData>.instance.GetNativeRepresentation(data);
		}

		public static TData[] GetData(TNative[] a)
		{
			if (a == null)
			{
				return null;
			}
			TData[] array = new TData[a.Length];
			for (int i = 0; i < a.Length; i++)
			{
				array[i] = DataConverter<T, TNative, TData>.instance.GetDataRepresentation(a[i]);
			}
			return array;
		}

		public static TNative[] GetNative(TData[] a)
		{
			if (a == null)
			{
				return null;
			}
			TNative[] array = new TNative[a.Length];
			for (int i = 0; i < a.Length; i++)
			{
				array[i] = DataConverter<T, TNative, TData>.instance.GetNativeRepresentation(a[i]);
			}
			return array;
		}

		private static T instance = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
	}
}
