using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "applicationSettings")]
	internal class ApplicationSettings : Resource
	{
		public ApplicationSettings(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "userAgent", EmitDefaultValue = false)]
		public string UserAgent
		{
			get
			{
				return base.GetValue<string>("userAgent");
			}
			set
			{
				base.SetValue<string>("userAgent", value);
			}
		}

		[DataMember(Name = "endpointId", EmitDefaultValue = false)]
		public string EndpointId
		{
			get
			{
				return base.GetValue<string>("endpointId");
			}
			set
			{
				base.SetValue<string>("endpointId", value);
			}
		}

		[DataMember(Name = "culture", EmitDefaultValue = false)]
		public string Culture
		{
			get
			{
				return base.GetValue<string>("culture");
			}
			set
			{
				base.SetValue<string>("culture", value);
			}
		}

		[DataMember(Name = "type", EmitDefaultValue = false)]
		public ApplicationType ApplicationType
		{
			get
			{
				return base.GetValue<ApplicationType>("type");
			}
			set
			{
				base.SetValue<ApplicationType>("type", value);
			}
		}

		public bool IsInternalApplication { get; set; }
	}
}
