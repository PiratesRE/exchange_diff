using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[Serializable]
	public class UpdateGroupMailboxType : BaseRequestType
	{
		public string GroupSmtpAddress;

		public string ExecutingUserSmtpAddress;

		public string DomainController;

		public GroupMailboxConfigurationActionType ForceConfigurationAction;

		public GroupMemberIdentifierType MemberIdentifierType;

		[XmlIgnore]
		public bool MemberIdentifierTypeSpecified;

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] AddedMembers;

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] RemovedMembers;

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] AddedPendingMembers;

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] RemovedPendingMembers;

		public int PermissionsVersion;

		[XmlIgnore]
		public bool PermissionsVersionSpecified;
	}
}
