using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.PeopleIKnowService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PeopleIKnowJsonSerializer : IPeopleIKnowSerializer
	{
		private PeopleIKnowJsonSerializer()
		{
		}

		public string Serialize(PeopleIKnowGraph peopleIKnowGraph)
		{
			ArgumentValidator.ThrowIfNull("peopleIKnowGraph", peopleIKnowGraph);
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(PeopleIKnowGraph), PeopleIKnowJsonSerializer.KnownTypes);
			string @string;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject(memoryStream, peopleIKnowGraph);
				@string = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			return @string;
		}

		public PeopleIKnowGraph Deserialize(string serializedPeopleIKnow)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(PeopleIKnowGraph), PeopleIKnowJsonSerializer.KnownTypes);
			PeopleIKnowGraph result;
			using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedPeopleIKnow)))
			{
				result = (PeopleIKnowGraph)dataContractJsonSerializer.ReadObject(memoryStream);
			}
			return result;
		}

		private static readonly Type[] KnownTypes = new Type[]
		{
			typeof(RelevantPerson[]),
			typeof(RelevantPerson)
		};

		internal static readonly PeopleIKnowJsonSerializer Singleton = new PeopleIKnowJsonSerializer();
	}
}
