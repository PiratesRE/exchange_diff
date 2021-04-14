using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://www.ccs.com/TestServices/")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[Serializable]
	public class ProvisionInfo
	{
		public Guid ContextId
		{
			get
			{
				return this.contextIdField;
			}
			set
			{
				this.contextIdField = value;
			}
		}

		public string ShortLivedToken
		{
			get
			{
				return this.shortLivedTokenField;
			}
			set
			{
				this.shortLivedTokenField = value;
			}
		}

		public string FirstAdminUserNetId
		{
			get
			{
				return this.firstAdminUserNetIdField;
			}
			set
			{
				this.firstAdminUserNetIdField = value;
			}
		}

		private Guid contextIdField;

		private string shortLivedTokenField;

		private string firstAdminUserNetIdField;
	}
}
