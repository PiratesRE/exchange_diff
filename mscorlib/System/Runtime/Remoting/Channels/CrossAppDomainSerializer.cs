using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	internal static class CrossAppDomainSerializer
	{
		[SecurityCritical]
		internal static MemoryStream SerializeMessage(IMessage msg)
		{
			MemoryStream memoryStream = new MemoryStream();
			RemotingSurrogateSelector surrogateSelector = new RemotingSurrogateSelector();
			new BinaryFormatter
			{
				SurrogateSelector = surrogateSelector,
				Context = new StreamingContext(StreamingContextStates.CrossAppDomain)
			}.Serialize(memoryStream, msg, null, false);
			memoryStream.Position = 0L;
			return memoryStream;
		}

		[SecurityCritical]
		internal static MemoryStream SerializeMessageParts(ArrayList argsToSerialize)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			RemotingSurrogateSelector surrogateSelector = new RemotingSurrogateSelector();
			binaryFormatter.SurrogateSelector = surrogateSelector;
			binaryFormatter.Context = new StreamingContext(StreamingContextStates.CrossAppDomain);
			binaryFormatter.Serialize(memoryStream, argsToSerialize, null, false);
			memoryStream.Position = 0L;
			return memoryStream;
		}

		[SecurityCritical]
		internal static void SerializeObject(object obj, MemoryStream stm)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			RemotingSurrogateSelector surrogateSelector = new RemotingSurrogateSelector();
			binaryFormatter.SurrogateSelector = surrogateSelector;
			binaryFormatter.Context = new StreamingContext(StreamingContextStates.CrossAppDomain);
			binaryFormatter.Serialize(stm, obj, null, false);
		}

		[SecurityCritical]
		internal static MemoryStream SerializeObject(object obj)
		{
			MemoryStream memoryStream = new MemoryStream();
			CrossAppDomainSerializer.SerializeObject(obj, memoryStream);
			memoryStream.Position = 0L;
			return memoryStream;
		}

		[SecurityCritical]
		internal static IMessage DeserializeMessage(MemoryStream stm)
		{
			return CrossAppDomainSerializer.DeserializeMessage(stm, null);
		}

		[SecurityCritical]
		internal static IMessage DeserializeMessage(MemoryStream stm, IMethodCallMessage reqMsg)
		{
			if (stm == null)
			{
				throw new ArgumentNullException("stm");
			}
			stm.Position = 0L;
			return (IMessage)new BinaryFormatter
			{
				SurrogateSelector = null,
				Context = new StreamingContext(StreamingContextStates.CrossAppDomain)
			}.Deserialize(stm, null, false, true, reqMsg);
		}

		[SecurityCritical]
		internal static ArrayList DeserializeMessageParts(MemoryStream stm)
		{
			return (ArrayList)CrossAppDomainSerializer.DeserializeObject(stm);
		}

		[SecurityCritical]
		internal static object DeserializeObject(MemoryStream stm)
		{
			stm.Position = 0L;
			return new BinaryFormatter
			{
				Context = new StreamingContext(StreamingContextStates.CrossAppDomain)
			}.Deserialize(stm, null, false, true, null);
		}
	}
}
