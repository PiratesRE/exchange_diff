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
	public class GetDelegateType : BaseDelegateType
	{
		[XmlArrayItem("UserId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public UserIdType[] UserIds
		{
			get
			{
				return this.userIdsField;
			}
			set
			{
				this.userIdsField = value;
			}
		}

		[XmlAttribute]
		public bool IncludePermissions
		{
			get
			{
				return this.includePermissionsField;
			}
			set
			{
				this.includePermissionsField = value;
			}
		}

		private UserIdType[] userIdsField;

		private bool includePermissionsField;
	}
}
