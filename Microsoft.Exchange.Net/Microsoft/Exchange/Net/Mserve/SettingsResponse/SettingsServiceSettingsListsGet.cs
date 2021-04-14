using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsResponse
{
	[DebuggerStepThrough]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[XmlType(AnonymousType = true, Namespace = "HMSETTINGS:")]
	[DesignerCategory("code")]
	[Serializable]
	public class SettingsServiceSettingsListsGet
	{
		[XmlArrayItem("List", IsNullable = false)]
		public ListsGetResponseTypeList[] Lists
		{
			get
			{
				return this.listsField;
			}
			set
			{
				this.listsField = value;
			}
		}

		private ListsGetResponseTypeList[] listsField;
	}
}
