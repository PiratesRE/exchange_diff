using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(LocationBasedStateDefinitionType))]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(DeleteFromFolderStateDefinitionType))]
	[XmlInclude(typeof(DeletedOccurrenceStateDefinitionType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class BaseCalendarItemStateDefinitionType
	{
	}
}
