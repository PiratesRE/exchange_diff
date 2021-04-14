using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "MakeMeAvailableSettings")]
	internal class MakeMeAvailableSettings : Resource
	{
		public MakeMeAvailableSettings(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "PhoneNumber", EmitDefaultValue = false)]
		public string PhoneNumber
		{
			get
			{
				return base.GetValue<string>("PhoneNumber");
			}
			set
			{
				base.SetValue<string>("PhoneNumber", value);
			}
		}

		[DataMember(Name = "SupportedModalities", EmitDefaultValue = false)]
		public ModalityType[] SupportedModalities
		{
			get
			{
				return base.GetValue<ModalityType[]>("SupportedModalities");
			}
			set
			{
				base.SetValue<ModalityType[]>("SupportedModalities", value);
			}
		}

		[DataMember(Name = "AudioPreference", EmitDefaultValue = false)]
		public AudioPreference AudioPreference
		{
			get
			{
				return base.GetValue<AudioPreference>("AudioPreference");
			}
			set
			{
				base.SetValue<AudioPreference>("AudioPreference", value);
			}
		}

		[DataMember(Name = "InactiveInterval", EmitDefaultValue = false)]
		public TimeSpan InactiveInterval
		{
			get
			{
				return base.GetValue<TimeSpan>("InactiveInterval");
			}
			set
			{
				base.SetValue<TimeSpan>("InactiveInterval", value);
			}
		}

		[DataMember(Name = "AwayInterval", EmitDefaultValue = false)]
		public TimeSpan AwayInterval
		{
			get
			{
				return base.GetValue<TimeSpan>("AwayInterval");
			}
			set
			{
				base.SetValue<TimeSpan>("AwayInterval", value);
			}
		}
	}
}
