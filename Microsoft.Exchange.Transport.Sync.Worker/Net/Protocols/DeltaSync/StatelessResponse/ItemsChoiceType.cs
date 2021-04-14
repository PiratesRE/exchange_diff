using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessResponse
{
	[XmlType(Namespace = "DeltaSyncV2:", IncludeInSchema = false)]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[Serializable]
	public enum ItemsChoiceType
	{
		Completed,
		ReminderDate,
		State,
		Title
	}
}
