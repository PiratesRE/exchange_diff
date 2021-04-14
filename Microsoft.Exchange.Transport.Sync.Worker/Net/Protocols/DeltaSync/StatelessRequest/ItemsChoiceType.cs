using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[XmlType(Namespace = "DeltaSyncV2:", IncludeInSchema = false)]
	[Serializable]
	public enum ItemsChoiceType
	{
		Completed,
		ReminderDate,
		State,
		Title
	}
}
