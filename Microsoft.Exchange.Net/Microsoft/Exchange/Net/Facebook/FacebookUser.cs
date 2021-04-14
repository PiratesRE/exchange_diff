using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FacebookUser : IExtensibleDataObject
	{
		[DataMember(Name = "activities")]
		public FacebookActivityList ActivitiesList { get; set; }

		[DataMember(Name = "birthday")]
		public string Birthday { get; set; }

		[DataMember(Name = "education")]
		public List<FacebookEducationHistoryEntry> EducationHistory { get; set; }

		[DataMember(Name = "email")]
		public string EmailAddress { get; set; }

		[DataMember(Name = "id")]
		public string Id { get; set; }

		[DataMember(Name = "interests")]
		public FacebookInterestList InterestsList { get; set; }

		[DataMember(Name = "updated_time")]
		public string UpdatedTime { get; set; }

		[DataMember(Name = "first_name")]
		public string FirstName { get; set; }

		[DataMember(Name = "last_name")]
		public string LastName { get; set; }

		[DataMember(Name = "location")]
		public FacebookLocation Location { get; set; }

		[DataMember(Name = "mobile_phone")]
		public string MobilePhoneNumber { get; set; }

		[DataMember(Name = "picture")]
		public FacebookPicture Picture { get; set; }

		[DataMember(Name = "link")]
		public string ProfilePageUrl { get; set; }

		[DataMember(Name = "website")]
		public string Website { get; set; }

		[DataMember(Name = "work")]
		public List<FacebookWorkHistoryEntry> WorkHistory { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
