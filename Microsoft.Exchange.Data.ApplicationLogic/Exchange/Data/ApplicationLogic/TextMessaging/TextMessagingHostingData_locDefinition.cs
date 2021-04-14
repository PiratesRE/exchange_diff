using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.TextMessaging
{
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[DebuggerStepThrough]
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[Serializable]
	public class TextMessagingHostingData_locDefinition
	{
		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public string _locDefault_loc
		{
			get
			{
				return this._locDefault_locField;
			}
			set
			{
				this._locDefault_locField = value;
			}
		}

		private string _locDefault_locField;
	}
}
