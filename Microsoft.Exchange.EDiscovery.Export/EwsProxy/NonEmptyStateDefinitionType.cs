using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class NonEmptyStateDefinitionType
	{
		[XmlElement("LocationBasedStateDefinition", typeof(LocationBasedStateDefinitionType))]
		[XmlElement("DeleteFromFolderStateDefinition", typeof(DeleteFromFolderStateDefinitionType))]
		[XmlElement("DeletedOccurrenceStateDefinition", typeof(DeletedOccurrenceStateDefinitionType))]
		public BaseCalendarItemStateDefinitionType Item
		{
			get
			{
				return this.itemField;
			}
			set
			{
				this.itemField = value;
			}
		}

		private BaseCalendarItemStateDefinitionType itemField;
	}
}
