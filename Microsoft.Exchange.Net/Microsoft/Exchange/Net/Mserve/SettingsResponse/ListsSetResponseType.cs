using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsResponse
{
	[XmlType(Namespace = "HMSETTINGS:")]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ListsSetResponseType
	{
		public int Status
		{
			get
			{
				return this.statusField;
			}
			set
			{
				this.statusField = value;
			}
		}

		[XmlElement("List")]
		public ListsSetResponseTypeList[] List
		{
			get
			{
				return this.listField;
			}
			set
			{
				this.listField = value;
			}
		}

		private int statusField;

		private ListsSetResponseTypeList[] listField;
	}
}
