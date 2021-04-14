using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Serialization;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class ControlData
	{
		public ControlData(DateTime lastProcessedDate)
		{
			this.LastProcessedDate = lastProcessedDate;
		}

		public DateTime LastProcessedDate { get; private set; }

		public static ControlData CreateFromByteArray(byte[] serializedData)
		{
			if (serializedData == null || serializedData.Length == 0)
			{
				return ControlData.Empty;
			}
			ControlData result;
			try
			{
				int num = 0;
				uint num2 = Serialization.DeserializeUInt32(serializedData, ref num);
				if (num2 > 1U)
				{
					return ControlData.Empty;
				}
				DateTime lastProcessedDate = Serialization.DeserializeDateTime(serializedData, ref num);
				result = new ControlData(lastProcessedDate);
			}
			catch (SerializationException)
			{
				result = ControlData.Empty;
			}
			return result;
		}

		public static ControlData Create(DateTime lastProcessedDate)
		{
			return new ControlData(lastProcessedDate);
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[12];
			int num = 0;
			Serialization.SerializeUInt32(array, ref num, 1U);
			Serialization.SerializeDateTime(array, ref num, this.LastProcessedDate);
			return array;
		}

		private const uint CurrentVersion = 1U;

		public static readonly ControlData Empty = new ControlData(DateTime.MinValue);
	}
}
