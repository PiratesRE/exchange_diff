using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsRequest
{
	[DebuggerStepThrough]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[XmlType(AnonymousType = true, Namespace = "HMSETTINGS:")]
	[DesignerCategory("code")]
	[Serializable]
	public class ServiceSettingsTypeSafetyLevelRules
	{
		public object GetVersion
		{
			get
			{
				return this.getVersionField;
			}
			set
			{
				this.getVersionField = value;
			}
		}

		public object Get
		{
			get
			{
				return this.getField;
			}
			set
			{
				this.getField = value;
			}
		}

		private object getVersionField;

		private object getField;
	}
}
