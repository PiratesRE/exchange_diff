using System;

namespace Microsoft.Exchange.Diagnostics
{
	public static class CombGuidGenerator
	{
		public static Guid NewGuid()
		{
			return CombGuidGenerator.NewGuid(Guid.NewGuid(), DateTime.UtcNow);
		}

		public static Guid NewGuid(DateTime dateTime)
		{
			return CombGuidGenerator.NewGuid(Guid.NewGuid(), dateTime);
		}

		public static Guid NewGuid(Guid guid, DateTime dateTime)
		{
			byte[] array = guid.ToByteArray();
			byte[] bytes = BitConverter.GetBytes(dateTime.Ticks);
			Array.Reverse(bytes);
			Array.Copy(bytes, 0, array, 10, 6);
			Array.Copy(bytes, 6, array, 8, 2);
			return new Guid(array);
		}

		public static DateTime ExtractDateTimeFromCombGuid(Guid guid)
		{
			long ticksFromGuid = CombGuidGenerator.GetTicksFromGuid(guid);
			return new DateTime(ticksFromGuid, DateTimeKind.Utc);
		}

		public static bool IsCombGuid(Guid guid)
		{
			long ticksFromGuid = CombGuidGenerator.GetTicksFromGuid(guid);
			return DateTime.MinValue.Ticks <= ticksFromGuid && ticksFromGuid <= DateTime.MaxValue.Ticks;
		}

		private static long GetTicksFromGuid(Guid guid)
		{
			byte[] sourceArray = guid.ToByteArray();
			byte[] array = new byte[8];
			Array.Copy(sourceArray, 10, array, 0, 6);
			Array.Copy(sourceArray, 8, array, 6, 2);
			Array.Reverse(array);
			return BitConverter.ToInt64(array, 0);
		}
	}
}
