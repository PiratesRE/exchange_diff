using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Parent("user")]
	[Get(typeof(PoliciesResource))]
	[DataContract(Name = "Policies")]
	internal class PoliciesResource : Resource
	{
		public PoliciesResource(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "SendFeedbackUrl", EmitDefaultValue = false)]
		public string SendFeedbackUrl
		{
			get
			{
				return base.GetValue<string>("SendFeedbackUrl");
			}
			set
			{
				base.SetValue<string>("SendFeedbackUrl", value);
			}
		}

		[DataMember(Name = "OnlineFeedbackUrl", EmitDefaultValue = false)]
		public string OnlineFeedbackUrl
		{
			get
			{
				return base.GetValue<string>("OnlineFeedbackUrl");
			}
			set
			{
				base.SetValue<string>("OnlineFeedbackUrl", value);
			}
		}

		[DataMember(Name = "EnableSQM", EmitDefaultValue = false)]
		public bool? EnableSQM
		{
			get
			{
				return base.GetValue<bool?>("EnableSQM");
			}
			set
			{
				base.SetValue<bool?>("EnableSQM", value);
			}
		}

		[DataMember(Name = "EnableLogging", EmitDefaultValue = false)]
		public bool? EnableLogging
		{
			get
			{
				return base.GetValue<bool?>("EnableLogging");
			}
			set
			{
				base.SetValue<bool?>("EnableLogging", value);
			}
		}

		[DataMember(Name = "LoggingLevel", EmitDefaultValue = false)]
		public string LoggingLevel
		{
			get
			{
				return base.GetValue<string>("LoggingLevel");
			}
			set
			{
				base.SetValue<string>("LoggingLevel", value);
			}
		}

		[DataMember(Name = "DisableEmoticons", EmitDefaultValue = false)]
		public bool? DisableEmoticons
		{
			get
			{
				return base.GetValue<bool?>("DisableEmoticons");
			}
			set
			{
				base.SetValue<bool?>("DisableEmoticons", value);
			}
		}

		[DataMember(Name = "EnableMultiviewJoin", EmitDefaultValue = false)]
		public bool? EnableMultiviewJoin
		{
			get
			{
				return base.GetValue<bool?>("EnableMultiviewJoin");
			}
			set
			{
				base.SetValue<bool?>("EnableMultiviewJoin", value);
			}
		}

		[DataMember(Name = "DisableHtmlIM", EmitDefaultValue = false)]
		public bool? DisableHtmlIM
		{
			get
			{
				return base.GetValue<bool?>("DisableHtmlIM");
			}
			set
			{
				base.SetValue<bool?>("DisableHtmlIM", value);
			}
		}

		[DataMember(Name = "EnterpriseVoiceEnabled", EmitDefaultValue = false)]
		public bool? EnterpriseVoiceEnabled
		{
			get
			{
				return base.GetValue<bool?>("EnterpriseVoiceEnabled");
			}
			set
			{
				base.SetValue<bool?>("EnterpriseVoiceEnabled", value);
			}
		}

		[DataMember(Name = "ExchangeUMEnabled", EmitDefaultValue = false)]
		public bool? ExchangeUMEnabled
		{
			get
			{
				return base.GetValue<bool?>("ExchangeUMEnabled");
			}
			set
			{
				base.SetValue<bool?>("ExchangeUMEnabled", value);
			}
		}

		[DataMember(Name = "VoicemailUri", EmitDefaultValue = false)]
		public string VoicemailUri
		{
			get
			{
				return base.GetValue<string>("VoicemailUri");
			}
			set
			{
				base.SetValue<string>("VoicemailUri", value);
			}
		}

		public const string Token = "policies";
	}
}
