using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Flags]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
	[Serializable]
	public enum SyncOptions
	{
		None = 1,
		DelayUntilContextIsProvisioned = 2,
		SkipExchangeSpecificFiltering = 4,
		SkipBackfillOnRevisionUpdate = 8,
		SkipBackfill = 16
	}
}
