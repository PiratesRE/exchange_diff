using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class ApnsAlert
	{
		public ApnsAlert(string body = null, string launchImage = null, string actionLocKey = null, string locKey = null, string[] locArgs = null)
		{
			this.Body = body;
			this.LaunchImage = launchImage;
			this.ActionLocKey = actionLocKey;
			this.LocKey = locKey;
			this.LocArgs = locArgs;
		}

		[DataMember(Name = "body", EmitDefaultValue = false, Order = 1)]
		public string Body { get; private set; }

		[DataMember(Name = "launch-image", EmitDefaultValue = false, Order = 2)]
		public string LaunchImage { get; private set; }

		[DataMember(Name = "action-loc-key", EmitDefaultValue = false, Order = 3)]
		public string ActionLocKey { get; private set; }

		[DataMember(Name = "loc-key", EmitDefaultValue = false, Order = 4)]
		public string LocKey { get; private set; }

		[DataMember(Name = "loc-args", EmitDefaultValue = false, Order = 5)]
		public string[] LocArgs { get; private set; }

		public string ToJson()
		{
			return JsonConverter.Serialize<ApnsAlert>(this, null);
		}

		public override string ToString()
		{
			if (this.toStringCache == null)
			{
				this.toStringCache = string.Format("{{body:{0}; launch-image:{1}; action-loc-key:{2}; loc-key:{3}; loc-args:{4}}}", new object[]
				{
					this.Body.ToNullableString(),
					this.LaunchImage.ToNullableString(),
					this.ActionLocKey.ToNullableString(),
					this.LocKey.ToNullableString(),
					this.LocArgs.ToNullableString(null)
				});
			}
			return this.toStringCache;
		}

		private string toStringCache;
	}
}
