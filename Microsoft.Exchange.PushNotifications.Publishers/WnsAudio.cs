using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsAudio
	{
		public WnsAudio(WnsSound? sound = null, bool? loop = null, bool? silent = null)
		{
			this.Sound = sound;
			this.Loop = loop;
			this.Silent = silent;
		}

		public WnsSound? Sound { get; private set; }

		public bool? Loop { get; private set; }

		public bool? Silent { get; private set; }

		public override string ToString()
		{
			return string.Format("{{src:{0}; loop:{1}; silent:{2}}}", this.Sound.ToNullableString<WnsSound>(), this.Loop.ToNullableString<bool>(), this.Silent.ToNullableString<bool>());
		}

		internal void WriteWnsPayload(WnsPayloadWriter wpw)
		{
			ArgumentValidator.ThrowIfNull("wpw", wpw);
			wpw.WriteElementStart("audio", false);
			wpw.WriteSoundAttribute("src", this.Sound, true);
			wpw.WriteAttribute<bool>("loop", this.Loop, true);
			wpw.WriteAttribute<bool>("silent", this.Silent, true);
			wpw.WriteElementEnd();
		}
	}
}
