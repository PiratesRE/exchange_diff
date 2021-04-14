using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11")]
	[Flags]
	[Serializable]
	public enum ServiceInstanceMoveOptions
	{
		OutOfContextAndFaultInWithAutoComplete = 1,
		OutOfContextWithPauseBeforeFaultin = 2,
		FaultinWithPauseBeforeOutOfContext = 4
	}
}
