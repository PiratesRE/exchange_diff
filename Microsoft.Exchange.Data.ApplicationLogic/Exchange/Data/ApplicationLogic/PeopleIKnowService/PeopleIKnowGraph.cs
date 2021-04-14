using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.PeopleIKnowService
{
	[DataContract]
	public sealed class PeopleIKnowGraph : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData { get; set; }

		[DataMember(Name = "RelevantPeople")]
		public RelevantPerson[] RelevantPeople;
	}
}
