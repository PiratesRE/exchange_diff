using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class PropertyExistenceType
	{
		[DataMember(EmitDefaultValue = false)]
		public bool ExtractedAddresses { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ExtractedContacts { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ExtractedEmails { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ExtractedKeywords { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ExtractedMeetings { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ExtractedPhones { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ExtractedTasks { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ExtractedUrls { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ReplyToNames { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ReplyToBlob { get; set; }
	}
}
