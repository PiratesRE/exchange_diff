using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class XmlValueSearchForUsers
	{
		public SearchForUsersValue SearchForUsers
		{
			get
			{
				return this.searchForUsersField;
			}
			set
			{
				this.searchForUsersField = value;
			}
		}

		private SearchForUsersValue searchForUsersField;
	}
}
