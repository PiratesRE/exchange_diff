using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PlatformTelephonyAddress
	{
		public PlatformTelephonyAddress(string name, PlatformSipUri uri)
		{
			this.Name = name;
			this.Uri = uri;
		}

		public string Name { get; private set; }

		public PlatformSipUri Uri { get; private set; }
	}
}
