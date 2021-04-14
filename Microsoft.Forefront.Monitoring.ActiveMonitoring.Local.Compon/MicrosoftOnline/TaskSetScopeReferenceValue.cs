using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class TaskSetScopeReferenceValue
	{
		[XmlArrayItem("ScopeReference", IsNullable = false)]
		public ScopeReferencesScopeReference[] ScopeReferences
		{
			get
			{
				return this.scopeReferencesField;
			}
			set
			{
				this.scopeReferencesField = value;
			}
		}

		[XmlAttribute]
		public string TaskSetId
		{
			get
			{
				return this.taskSetIdField;
			}
			set
			{
				this.taskSetIdField = value;
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

		private ScopeReferencesScopeReference[] scopeReferencesField;

		private string taskSetIdField;

		private bool builtInField;
	}
}
