using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsToastVisual : WnsVisual
	{
		public WnsToastVisual(WnsToastBinding binding, string language = null, string baseUri = null, WnsBranding? branding = null, bool? addImageQuery = null) : base(language, baseUri, branding, addImageQuery)
		{
			this.Binding = binding;
		}

		public WnsToastBinding Binding { get; private set; }

		public override string ToString()
		{
			return string.Format("{{binding:{0}; {1}}}", this.Binding.ToString(), base.ToString());
		}

		internal override void WriteWnsBindings(WnsPayloadWriter wpw)
		{
			this.Binding.WriteWnsPayload(wpw);
		}
	}
}
