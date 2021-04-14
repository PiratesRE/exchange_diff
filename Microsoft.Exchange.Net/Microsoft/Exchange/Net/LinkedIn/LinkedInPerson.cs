using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[DataContract]
	public class LinkedInPerson : IExtensibleDataObject
	{
		[DataMember(Name = "id")]
		public string Id { get; set; }

		[DataMember(Name = "firstName")]
		public string FirstName { get; set; }

		[DataMember(Name = "lastName")]
		public string LastName { get; set; }

		[DataMember(Name = "headline")]
		public string Headline { get; set; }

		[DataMember(Name = "emailAddress")]
		public string EmailAddress { get; set; }

		[DataMember(Name = "threeCurrentPositions")]
		public LinkedInPositionsList ThreeCurrentPositions { get; set; }

		[DataMember(Name = "phoneNumbers")]
		public LinkedInPhoneNumberList PhoneNumbers { get; set; }

		[DataMember(Name = "pictureUrl")]
		public string PictureUrl { get; set; }

		[DataMember(Name = "imAccounts")]
		public LinkedInIMAccounts IMAccounts { get; set; }

		[DataMember(Name = "dateOfBirth")]
		public LinkedInBirthDate Birthdate { get; set; }

		[DataMember(Name = "educations")]
		public LinkedInSchoolList SchoolList { get; set; }

		[DataMember(Name = "publicProfileUrl")]
		public string PublicProfileUrl { get; set; }

		[DataMember(Name = "pictureUrls")]
		public LinkedInPictureUrls PictureUrls { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
