using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Rpc.Cluster
{
	internal abstract class SerializationServices
	{
		public unsafe static int Serialize(object input, byte** _ppb, int* _pcb)
		{
			return <Module>.GetUnmanagedBytes(SerializationServices.Serialize(input), _ppb, _pcb);
		}

		public static byte[] Serialize(object input)
		{
			if (input == null)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			byte[] buffer;
			try
			{
				ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null).Serialize(memoryStream, input);
				buffer = memoryStream.GetBuffer();
			}
			finally
			{
				memoryStream.Close();
			}
			return buffer;
		}

		public unsafe static T Deserialize<T>(byte* _pb, int _cb) where T : class
		{
			byte[] array = new byte[_cb];
			if (_cb > 0)
			{
				Marshal.Copy((IntPtr)((void*)_pb), array, 0, _cb);
			}
			return SerializationServices.Deserialize<T>(array);
		}

		public static T Deserialize<T>(byte[] serializedBytes) where T : class
		{
			T result = default(T);
			if (serializedBytes != null && serializedBytes.Length > 0)
			{
				MemoryStream serializationStream = new MemoryStream(serializedBytes);
				result = (T)((object)ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null).Deserialize(serializationStream));
			}
			return result;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public unsafe static bool TryDeserialize<T>(byte* _pb, int _cb, ref T outputObject, ref Exception deserializeEx) where T : class
		{
			try
			{
				T t = SerializationServices.Deserialize<T>(<Module>.MakeManagedBytes(_pb, _cb));
				outputObject = t;
				return true;
			}
			catch (SerializationException ex)
			{
				deserializeEx = ex;
			}
			catch (TargetInvocationException ex2)
			{
				deserializeEx = ex2;
			}
			catch (DecoderFallbackException ex3)
			{
				deserializeEx = ex3;
			}
			return false;
		}

		public SerializationServices()
		{
		}
	}
}
