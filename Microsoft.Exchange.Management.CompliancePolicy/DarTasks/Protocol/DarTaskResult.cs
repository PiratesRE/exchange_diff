using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol
{
	[Serializable]
	public class DarTaskResult
	{
		public string DarTaskId { get; set; }

		public TaskStoreObject[] DarTasks { get; set; }

		public TaskAggregateStoreObject[] DarTaskAggregates { get; set; }

		public string LocalizedError { get; set; }

		public string LocalizedInformation { get; set; }

		public static DarTaskResult Nothing()
		{
			return new DarTaskResult();
		}

		public static DarTaskResult GetResultObject(byte[] data)
		{
			ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			DarTaskResult darTaskResult = DarTaskResult.ObjectFromBytes<DarTaskResult>(data);
			if (darTaskResult.LocalizedError != null)
			{
				throw new DataSourceOperationException(new LocalizedString(darTaskResult.LocalizedError));
			}
			return darTaskResult;
		}

		public static T ObjectFromBytes<T>(byte[] data)
		{
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			T result;
			using (MemoryStream memoryStream = new MemoryStream(data, false))
			{
				result = (T)((object)binaryFormatter.Deserialize(memoryStream));
			}
			return result;
		}

		public static byte[] ObjectToBytes<T>(T obj)
		{
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			byte[] buffer;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				binaryFormatter.Serialize(memoryStream, obj);
				buffer = memoryStream.GetBuffer();
			}
			return buffer;
		}

		public byte[] ToBytes()
		{
			return DarTaskResult.ObjectToBytes<DarTaskResult>(this);
		}
	}
}
