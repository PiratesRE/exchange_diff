using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Get(typeof(OnlineMeetingExtensionResource))]
	[Parent("onlineMeeting")]
	[DataContract(Name = "OnlineMeetingExtensionResource")]
	internal class OnlineMeetingExtensionResource : Resource
	{
		public OnlineMeetingExtensionResource(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "id", EmitDefaultValue = false)]
		public string Id
		{
			get
			{
				return base.GetValue<string>("Id");
			}
			set
			{
				base.SetValue<string>("Id", value);
			}
		}

		[DataMember(Name = "type", EmitDefaultValue = false)]
		public OnlineMeetingExtensionType? Type
		{
			get
			{
				return base.GetValue<OnlineMeetingExtensionType?>("type");
			}
			set
			{
				base.SetValue<OnlineMeetingExtensionType?>("type", value);
			}
		}

		public ICollection<string> PropertyNames
		{
			get
			{
				return base.Keys;
			}
		}

		public T GetPropertyValue<T>(string name)
		{
			return base.GetValue<T>(name);
		}

		public void SetPropertyValue<T>(string name, T value)
		{
			base.SetValue<T>(name, value);
		}

		public const string Token = "onlineMeetingExtension";
	}
}
