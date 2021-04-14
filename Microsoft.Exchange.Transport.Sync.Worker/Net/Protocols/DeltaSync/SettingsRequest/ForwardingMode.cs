﻿using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[Serializable]
	public enum ForwardingMode
	{
		[XmlEnum(Name = "NoForwarding")]
		NoForwarding,
		[XmlEnum(Name = "ForwardOnly")]
		ForwardOnly,
		[XmlEnum(Name = "StoreAndForward")]
		StoreAndForward
	}
}
