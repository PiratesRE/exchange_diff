using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class GetSearchableMailboxesType : BaseRequestType
	{
		public string SearchFilter
		{
			get
			{
				return this.searchFilterField;
			}
			set
			{
				this.searchFilterField = value;
			}
		}

		public bool ExpandGroupMembership
		{
			get
			{
				return this.expandGroupMembershipField;
			}
			set
			{
				this.expandGroupMembershipField = value;
			}
		}

		[XmlIgnore]
		public bool ExpandGroupMembershipSpecified
		{
			get
			{
				return this.expandGroupMembershipFieldSpecified;
			}
			set
			{
				this.expandGroupMembershipFieldSpecified = value;
			}
		}

		private string searchFilterField;

		private bool expandGroupMembershipField;

		private bool expandGroupMembershipFieldSpecified;
	}
}
