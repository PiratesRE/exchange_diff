using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal static class RopFactory
	{
		internal static void CreateRops(byte[] inputBuffer, int inputIndex, int inputSize, IParseLogonTracker logonTracker, ServerObjectHandleTable serverObjectHandleTable, ref List<Rop> ropList)
		{
			using (Reader reader = Reader.CreateBufferReader(new ArraySegment<byte>(inputBuffer, inputIndex, inputSize)))
			{
				while (reader.Position < reader.Length)
				{
					long position = reader.Position;
					RopId ropId = (RopId)reader.PeekByte(0L);
					Rop rop;
					if (!RopFactory.TryCreateRop(ropId, out rop))
					{
						throw new BufferParseException(string.Format("Invalid rop type found: {0}", ropId));
					}
					rop.ParseInput(reader, serverObjectHandleTable, logonTracker);
					long position2 = reader.Position;
					if (ropList == null)
					{
						ropList = new List<Rop>();
					}
					ropList.Add(rop);
				}
			}
		}

		internal static bool TryCreateRop(RopId ropId, out Rop rop)
		{
			Rop.CreateRopDelegate createRopDelegate;
			if (RopFactory.ropDictionary.TryGetValue(ropId, out createRopDelegate))
			{
				rop = createRopDelegate();
				return true;
			}
			rop = null;
			return false;
		}

		private static Dictionary<RopId, Rop.CreateRopDelegate> PopulateRopDictionary()
		{
			Assembly assembly = typeof(RopFactory).GetTypeInfo().Assembly;
			Type typeFromHandle = typeof(Rop);
			Dictionary<RopId, Rop.CreateRopDelegate> dictionary = new Dictionary<RopId, Rop.CreateRopDelegate>(new RopIdComparer());
			foreach (TypeInfo typeInfo in assembly.DefinedTypes)
			{
				if (!typeInfo.IsAbstract && typeInfo.IsSubclassOf(typeFromHandle))
				{
					FieldInfo declaredField = typeInfo.GetDeclaredField("RopType");
					RopId key = (RopId)declaredField.GetValue(null);
					MethodInfo declaredMethod = typeInfo.GetDeclaredMethod("CreateRop");
					Rop.CreateRopDelegate value = (Rop.CreateRopDelegate)declaredMethod.CreateDelegate(typeof(Rop.CreateRopDelegate));
					dictionary[key] = value;
				}
			}
			return dictionary;
		}

		private const int MaxInputBufferSize = 32768;

		private static Dictionary<RopId, Rop.CreateRopDelegate> ropDictionary = RopFactory.PopulateRopDictionary();
	}
}
