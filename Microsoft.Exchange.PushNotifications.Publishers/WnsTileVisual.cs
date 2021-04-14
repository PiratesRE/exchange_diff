using System;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsTileVisual : WnsVisual
	{
		public WnsTileVisual(WnsTileBinding binding, WnsTileBinding[] extraBindings = null, string language = null, string baseUri = null, WnsBranding? branding = null, bool? addImageQuery = null) : base(language, baseUri, branding, addImageQuery)
		{
			this.Binding = binding;
			this.ExtraBindings = extraBindings;
		}

		public WnsTileBinding Binding { get; private set; }

		public WnsTileBinding[] ExtraBindings { get; private set; }

		public override string ToString()
		{
			return string.Format("{{binding:{0}; extraBindings:{1}; {2}}}", this.Binding.ToString(), this.ExtraBindings.ToNullableString(null), base.ToString());
		}

		internal override void WriteWnsBindings(WnsPayloadWriter wpw)
		{
			this.Binding.WriteWnsPayload(wpw);
			if (this.ExtraBindings != null)
			{
				foreach (WnsTileBinding wnsTileBinding in this.ExtraBindings)
				{
					wnsTileBinding.WriteWnsPayload(wpw);
				}
			}
		}
	}
}
