using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(RootItemIdType))]
	[XmlInclude(typeof(AttachmentIdType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(OccurrenceItemIdType))]
	[XmlInclude(typeof(ItemIdType))]
	[XmlInclude(typeof(RecurringMasterItemIdRangesType))]
	[XmlInclude(typeof(RequestAttachmentIdType))]
	[XmlInclude(typeof(RecurringMasterItemIdType))]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public abstract class BaseItemIdType
	{
	}
}
