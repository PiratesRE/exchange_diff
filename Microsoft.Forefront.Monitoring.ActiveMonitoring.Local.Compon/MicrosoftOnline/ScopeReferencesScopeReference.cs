using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[Serializable]
	public class ScopeReferencesScopeReference
	{
		[XmlAttribute]
		public string ScopeId
		{
			get
			{
				return this.scopeIdField;
			}
			set
			{
				this.scopeIdField = value;
			}
		}

		[XmlAttribute]
		public bool BuiltIn
		{
			get
			{
				return this.builtInField;
			}
			set
			{
				this.builtInField = value;
			}
		}

		private string scopeIdField;

		private bool builtInField;
	}
}
