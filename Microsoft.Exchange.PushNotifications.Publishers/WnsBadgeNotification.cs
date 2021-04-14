using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsBadgeNotification : WnsXmlNotification
	{
		public WnsBadgeNotification(string appId, OrganizationId tenantId, string deviceUri, int numericValue, int? timeToLive = null, WnsCachePolicy? cachePolicy = 1) : this(appId, tenantId, deviceUri, new int?(numericValue), null, timeToLive, null)
		{
		}

		public WnsBadgeNotification(string appId, OrganizationId tenantId, string deviceUri, WnsGlyph glyphValue, int? timeToLive = null, WnsCachePolicy? cachePolicy = 1) : this(appId, tenantId, deviceUri, null, new WnsGlyph?(glyphValue), timeToLive, null)
		{
		}

		private WnsBadgeNotification(string appId, OrganizationId tenantId, string deviceUri, int? numericValue, WnsGlyph? glyphValue, int? timeToLive = null, WnsCachePolicy? cachePolicy = null) : base(appId, tenantId, deviceUri)
		{
			this.NumericValue = numericValue;
			this.GlyphValue = glyphValue;
			this.CachePolicy = cachePolicy;
			this.TimeToLive = timeToLive;
		}

		public int? NumericValue { get; private set; }

		public WnsGlyph? GlyphValue { get; private set; }

		public WnsCachePolicy? CachePolicy { get; private set; }

		public int? TimeToLive { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			base.ValidateTimeToLive(this.TimeToLive, errors);
		}

		protected override void WriteWnsXmlPayload(WnsPayloadWriter wpw)
		{
			wpw.WriteElementStart("badge", false);
			if (this.GlyphValue != null)
			{
				wpw.WriteAttribute<WnsGlyph>("value", this.GlyphValue, false);
			}
			else
			{
				wpw.WriteAttribute<int>("value", this.NumericValue, false);
			}
			wpw.WriteElementEnd();
		}

		protected override void PrepareWnsRequest(WnsRequest wnsRequest)
		{
			base.PrepareWnsRequest(wnsRequest);
			wnsRequest.WnsType = "wns/badge";
			if (this.CachePolicy != null)
			{
				wnsRequest.WnsCachePolicy = this.CachePolicy.Value.ToString();
			}
			if (this.TimeToLive != null)
			{
				wnsRequest.WnsTimeToLive = this.TimeToLive.ToString();
			}
		}

		protected override string InternalToFullString()
		{
			return string.Format("{0}; num:{1}; glyph:{2}; cache:{3}; ttl:{4}", new object[]
			{
				base.InternalToFullString(),
				this.NumericValue.ToNullableString<int>(),
				this.TimeToLive.ToNullableString<int>(),
				this.CachePolicy.ToNullableString<WnsCachePolicy>(),
				this.TimeToLive.ToNullableString<int>()
			});
		}
	}
}
