using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsTileNotification : WnsXmlNotification
	{
		public WnsTileNotification(string appId, OrganizationId tenantId, string deviceUri, WnsTileVisual visual, int? timeToLive = null, string tag = null, WnsCachePolicy? cachePolicy = null) : base(appId, tenantId, deviceUri)
		{
			this.Visual = visual;
			this.CachePolicy = cachePolicy;
			this.TimeToLive = timeToLive;
		}

		public WnsTileVisual Visual { get; private set; }

		public string Tag { get; private set; }

		public WnsCachePolicy? CachePolicy { get; private set; }

		public int? TimeToLive { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			base.ValidateTimeToLive(this.TimeToLive, errors);
			base.ValidateTemplate(this.Visual.Binding, errors);
		}

		protected override void WriteWnsXmlPayload(WnsPayloadWriter wpw)
		{
			wpw.WriteElementStart("tile", true);
			wpw.WriteAttributesEnd();
			this.Visual.WriteWnsPayload(wpw);
			wpw.WriteElementEnd();
		}

		protected override void PrepareWnsRequest(WnsRequest wnsRequest)
		{
			base.PrepareWnsRequest(wnsRequest);
			wnsRequest.WnsType = "wns/tile";
			if (this.CachePolicy != null)
			{
				wnsRequest.WnsCachePolicy = this.CachePolicy.Value.ToString();
			}
			if (this.TimeToLive != null)
			{
				wnsRequest.WnsTimeToLive = this.TimeToLive.ToString();
			}
			if (string.IsNullOrWhiteSpace(this.Tag))
			{
				wnsRequest.WnsTag = this.Tag;
			}
		}

		protected override string InternalToFullString()
		{
			return string.Format("{0}; tag: {1}; cache:{2}; ttl:{3}", new object[]
			{
				base.InternalToFullString(),
				this.Tag.ToNullableString(),
				this.CachePolicy.ToNullableString<WnsCachePolicy>(),
				this.TimeToLive.ToNullableString<int>()
			});
		}
	}
}
