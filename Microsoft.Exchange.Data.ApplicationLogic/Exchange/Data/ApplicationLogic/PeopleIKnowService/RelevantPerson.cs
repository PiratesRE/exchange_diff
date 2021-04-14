using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.PeopleIKnowService
{
	[DataContract]
	public sealed class RelevantPerson : IExtensibleDataObject
	{
		[DataMember(Name = "E")]
		public string EmailAddress { get; set; }

		[DataMember(Name = "R")]
		public int RelevanceScore { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
