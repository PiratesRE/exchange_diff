using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsImage
	{
		public WnsImage()
		{
		}

		public WnsImage(string source, string alt = null, bool? addImageQuery = null)
		{
			this.Source = source;
			this.Alt = alt;
			this.AddImageQuery = addImageQuery;
		}

		public string Source { get; private set; }

		public string Alt { get; private set; }

		public bool? AddImageQuery { get; private set; }

		public override string ToString()
		{
			return string.Format("{{src:{0}; alt:{1}; addImageQuery:{2}}}", this.Source.ToNullableString(), this.Alt.ToNullableString(), this.AddImageQuery.ToNullableString<bool>());
		}

		internal void WriteWnsPayload(int id, WnsPayloadWriter wpw)
		{
			ArgumentValidator.ThrowIfZeroOrNegative("id", id);
			ArgumentValidator.ThrowIfNull("wpw", wpw);
			wpw.WriteElementStart("image", false);
			wpw.WriteAttribute("id", id);
			wpw.WriteUriAttribute("src", this.Source, false);
			wpw.WriteAttribute("alt", this.Alt, true);
			wpw.WriteAttribute<bool>("addImageQuery", this.AddImageQuery, true);
			wpw.WriteElementEnd();
		}
	}
}
