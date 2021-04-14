using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RpcCommon
	{
		public static byte[] ConvertRpcParametersToByteArray(Dictionary<string, object> parameters)
		{
			if (parameters == null || parameters.Count < 1)
			{
				throw new ArgumentNullException("parameters");
			}
			return RpcCommon.ObjectToBytes(parameters);
		}

		public static Dictionary<string, object> ConvertByteArrayToRpcParameters(byte[] data)
		{
			if (data == null || data.Length < 1)
			{
				throw new ArgumentNullException("data");
			}
			Dictionary<string, object> dictionary = RpcCommon.BytesToObject(data) as Dictionary<string, object>;
			if (dictionary == null)
			{
				dictionary = new Dictionary<string, object>();
			}
			return dictionary;
		}

		private static byte[] ObjectToBytes(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			byte[] bytesFromBuffer;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				binaryFormatter.Serialize(memoryStream, obj);
				bytesFromBuffer = Util.GetBytesFromBuffer(memoryStream);
			}
			return bytesFromBuffer;
		}

		private static object BytesToObject(byte[] binaryData)
		{
			if (binaryData == null || binaryData.Length == 0)
			{
				return null;
			}
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			object result;
			using (MemoryStream memoryStream = new MemoryStream(binaryData, false))
			{
				result = binaryFormatter.Deserialize(memoryStream);
			}
			return result;
		}

		public const int CurrentRpcVersion = 1;
	}
}
