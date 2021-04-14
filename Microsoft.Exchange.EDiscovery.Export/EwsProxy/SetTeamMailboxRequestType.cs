using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SetTeamMailboxRequestType : BaseRequestType
	{
		public EmailAddressType EmailAddress
		{
			get
			{
				return this.emailAddressField;
			}
			set
			{
				this.emailAddressField = value;
			}
		}

		public string SharePointSiteUrl
		{
			get
			{
				return this.sharePointSiteUrlField;
			}
			set
			{
				this.sharePointSiteUrlField = value;
			}
		}

		public TeamMailboxLifecycleStateType State
		{
			get
			{
				return this.stateField;
			}
			set
			{
				this.stateField = value;
			}
		}

		private EmailAddressType emailAddressField;

		private string sharePointSiteUrlField;

		private TeamMailboxLifecycleStateType stateField;
	}
}
