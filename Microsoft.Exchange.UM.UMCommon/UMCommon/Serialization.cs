using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Compliance.Serialization.Formatters;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal static class Serialization
	{
		public static byte[] ObjectToBytes(object obj)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DiagnosticTracer, null, "In Serialization.ObjectToBytes", new object[0]);
			MemoryStream memoryStream = new MemoryStream();
			byte[] array = null;
			try
			{
				BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
				if (obj == null)
				{
					return null;
				}
				binaryFormatter.Serialize(memoryStream, obj);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				int num = (int)memoryStream.Length;
				array = new byte[num];
				memoryStream.Read(array, 0, num);
			}
			catch (SerializationException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.DiagnosticTracer, null, "In Serialization.ObjectToBytes, Got Error = {0}", new object[]
				{
					ex
				});
			}
			finally
			{
				memoryStream.Close();
			}
			return array;
		}

		public static object BytesToObject(byte[] mbinaryData)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DiagnosticTracer, 0, "In Serialization.BytesToObject", new object[0]);
			MemoryStream memoryStream = new MemoryStream();
			object result = null;
			try
			{
				if (mbinaryData == null || mbinaryData.Length == 0)
				{
					return null;
				}
				memoryStream.Write(mbinaryData, 0, mbinaryData.Length);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				result = TypedBinaryFormatter.DeserializeObject(memoryStream, SerializationTypeBinder.Instance);
			}
			catch (SerializationException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.DiagnosticTracer, null, "In Serialization.BytesToObject, Got Error = {0}", new object[]
				{
					ex
				});
			}
			finally
			{
				memoryStream.Close();
			}
			return result;
		}
	}
}
