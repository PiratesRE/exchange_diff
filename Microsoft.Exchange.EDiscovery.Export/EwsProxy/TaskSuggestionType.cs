using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class TaskSuggestionType : EntityType
	{
		public string TaskString
		{
			get
			{
				return this.taskStringField;
			}
			set
			{
				this.taskStringField = value;
			}
		}

		[XmlArrayItem("EmailUser", IsNullable = false)]
		public EmailUserType[] Assignees
		{
			get
			{
				return this.assigneesField;
			}
			set
			{
				this.assigneesField = value;
			}
		}

		private string taskStringField;

		private EmailUserType[] assigneesField;
	}
}
