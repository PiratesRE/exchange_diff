using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Parent("application")]
	[DataContract(Name = "MediaRelayToken")]
	[Get(typeof(MediaRelayTokenResource))]
	internal class MediaRelayTokenResource : Resource
	{
		public MediaRelayTokenResource(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "OwnerUri", EmitDefaultValue = false)]
		public string OwnerUri
		{
			get
			{
				return base.GetValue<string>("OwnerUri");
			}
			set
			{
				base.SetValue<string>("OwnerUri", value);
			}
		}

		[DataMember(Name = "MediaRelays")]
		public ResourceCollection<MediaRelay> MediaRelays
		{
			get
			{
				return base.GetValue<ResourceCollection<MediaRelay>>("MediaRelays");
			}
			set
			{
				base.SetValue<ResourceCollection<MediaRelay>>("MediaRelays", value);
			}
		}

		[DataMember(Name = "UserName", EmitDefaultValue = false)]
		public string UserName
		{
			get
			{
				return base.GetValue<string>("UserName");
			}
			set
			{
				base.SetValue<string>("UserName", value);
			}
		}

		[DataMember(Name = "Password", EmitDefaultValue = false)]
		public string Password
		{
			get
			{
				return base.GetValue<string>("Password");
			}
			set
			{
				base.SetValue<string>("Password", value);
			}
		}

		[DataMember(Name = "Duration", EmitDefaultValue = false)]
		public int Duration
		{
			get
			{
				return base.GetValue<int>("Duration");
			}
			set
			{
				base.SetValue<int>("Duration", value);
			}
		}

		[DataMember(Name = "ValidTo", EmitDefaultValue = false)]
		public DateTime ValidTo
		{
			get
			{
				return base.GetValue<DateTime>("ValidTo");
			}
			set
			{
				base.SetValue<DateTime>("ValidTo", value);
			}
		}

		public const string Token = "mrastoken";
	}
}
