using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "phoneDialInInformationResource")]
	internal class PhoneDialInInformationResource : OnlineMeetingCapabilityResource
	{
		public PhoneDialInInformationResource(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "defaultRegion", EmitDefaultValue = false)]
		public string DefaultRegion
		{
			get
			{
				return base.GetValue<string>("defaultRegion");
			}
			set
			{
				base.SetValue<string>("defaultRegion", value);
			}
		}

		[DataMember(Name = "dialInRegion", EmitDefaultValue = false)]
		public ResourceCollection<DialInRegionResource> DialInRegions
		{
			get
			{
				return base.GetValue<ResourceCollection<DialInRegionResource>>("dialInRegion");
			}
			set
			{
				base.SetValue<ResourceCollection<DialInRegionResource>>("dialInRegion", value);
			}
		}

		[DataMember(Name = "externalDirectoryUri", EmitDefaultValue = false)]
		public string ExternalDirectoryUri
		{
			get
			{
				return base.GetValue<string>("externalDirectoryUri");
			}
			set
			{
				base.SetValue<string>("externalDirectoryUri", value);
			}
		}

		[DataMember(Name = "internalDirectoryUri", EmitDefaultValue = false)]
		public string InternalDirectoryUri
		{
			get
			{
				return base.GetValue<string>("internalDirectoryUri");
			}
			set
			{
				base.SetValue<string>("internalDirectoryUri", value);
			}
		}

		[DataMember(Name = "conferenceId", EmitDefaultValue = false)]
		public string ConferenceId
		{
			get
			{
				return base.GetValue<string>("conferenceId");
			}
			set
			{
				base.SetValue<string>("conferenceId", value);
			}
		}

		[DataMember(Name = "isAudioConferenceProviderEnabled", EmitDefaultValue = false)]
		public bool IsAudioConferenceProviderEnabled
		{
			get
			{
				return base.GetValue<bool>("isAudioConferenceProviderEnabled");
			}
			set
			{
				base.SetValue<bool>("isAudioConferenceProviderEnabled", value);
			}
		}

		[DataMember(Name = "participantPassCode", EmitDefaultValue = false)]
		public string ParticipantPassCode
		{
			get
			{
				return base.GetValue<string>("participantPassCode");
			}
			set
			{
				base.SetValue<string>("participantPassCode", value);
			}
		}

		[DataMember(Name = "tollFreeNumbers", EmitDefaultValue = false)]
		public string[] TollFreeNumbers
		{
			get
			{
				return base.GetValue<string[]>("tollFreeNumbers");
			}
			set
			{
				base.SetValue<string[]>("tollFreeNumbers", value);
			}
		}

		[DataMember(Name = "tollNumber", EmitDefaultValue = false)]
		public string TollNumber
		{
			get
			{
				return base.GetValue<string>("tollNumber");
			}
			set
			{
				base.SetValue<string>("tollNumber", value);
			}
		}

		public const string Token = "phoneDialInInformation";

		internal static class PropertyNames
		{
			public const string DefaultRegion = "defaultRegion";

			public const string DialInRegion = "dialInRegion";

			public const string ExternalDirectoryUri = "externalDirectoryUri";

			public const string InternalDirectoryUri = "internalDirectoryUri";

			public const string ConferenceId = "conferenceId";

			public const string IsAudioConferenceProviderEnabled = "isAudioConferenceProviderEnabled";

			public const string ParticipantPassCode = "participantPassCode";

			public const string TollFreeNumbers = "tollFreeNumbers";

			public const string TollNumber = "tollNumber";
		}
	}
}
