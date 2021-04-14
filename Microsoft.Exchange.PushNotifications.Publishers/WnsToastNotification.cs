using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsToastNotification : WnsXmlNotification
	{
		public WnsToastNotification(string appId, OrganizationId tenantId, string deviceUri, WnsToastVisual visual, WnsToastDuration? duration = null, string launch = null, WnsAudio audio = null) : base(appId, tenantId, deviceUri)
		{
			this.Visual = visual;
			this.Duration = duration;
			this.Launch = launch;
			this.Audio = audio;
		}

		public WnsToastVisual Visual { get; private set; }

		public WnsToastDuration? Duration { get; private set; }

		public string Launch { get; private set; }

		public WnsAudio Audio { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			base.ValidateTemplate(this.Visual.Binding, errors);
		}

		protected override void WriteWnsXmlPayload(WnsPayloadWriter wpw)
		{
			ArgumentValidator.ThrowIfNull("wpw", wpw);
			wpw.WriteElementStart("toast", true);
			wpw.WriteAttribute<WnsToastDuration>("duration", this.Duration, true);
			wpw.WriteAttribute("launch", this.Launch, true);
			wpw.WriteAttributesEnd();
			this.Visual.WriteWnsPayload(wpw);
			if (this.Audio != null)
			{
				this.Audio.WriteWnsPayload(wpw);
			}
			wpw.WriteElementEnd();
		}

		protected override void PrepareWnsRequest(WnsRequest wnsRequest)
		{
			base.PrepareWnsRequest(wnsRequest);
			wnsRequest.WnsType = "wns/toast";
		}

		protected override string InternalToFullString()
		{
			return string.Format("{0}; visual:{1}; duration:{2}; launch:{3}; audio:{4}", new object[]
			{
				base.InternalToFullString(),
				this.Visual.ToNullableString(null),
				this.Duration.ToNullableString<WnsToastDuration>(),
				this.Launch.ToNullableString(),
				this.Audio.ToNullableString(null)
			});
		}
	}
}
