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
	public class GetDiscoverySearchConfigurationType : BaseRequestType
	{
		public string SearchId
		{
			get
			{
				return this.searchIdField;
			}
			set
			{
				this.searchIdField = value;
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

		public bool InPlaceHoldConfigurationOnly
		{
			get
			{
				return this.inPlaceHoldConfigurationOnlyField;
			}
			set
			{
				this.inPlaceHoldConfigurationOnlyField = value;
			}
		}

		[XmlIgnore]
		public bool InPlaceHoldConfigurationOnlySpecified
		{
			get
			{
				return this.inPlaceHoldConfigurationOnlyFieldSpecified;
			}
			set
			{
				this.inPlaceHoldConfigurationOnlyFieldSpecified = value;
			}
		}

		private string searchIdField;

		private bool expandGroupMembershipField;

		private bool expandGroupMembershipFieldSpecified;

		private bool inPlaceHoldConfigurationOnlyField;

		private bool inPlaceHoldConfigurationOnlyFieldSpecified;
	}
}
