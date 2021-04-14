using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.Shell
{
	[DataContract(Name = "WorkloadAuthenticationId", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public enum WorkloadAuthenticationId
	{
		[EnumMember]
		None,
		[EnumMember]
		Exchange,
		[EnumMember]
		Sharepoint,
		[EnumMember]
		FIM,
		[EnumMember]
		Lync,
		[EnumMember]
		OfficeDotCom,
		[EnumMember]
		CRM,
		[EnumMember]
		Forefront,
		[EnumMember]
		OnRamp,
		[EnumMember]
		AADUX,
		[EnumMember]
		AdminPortal,
		[EnumMember]
		Apps,
		[EnumMember]
		Yammer,
		[EnumMember]
		Project,
		[EnumMember]
		Pulse,
		[EnumMember]
		PowerBI,
		[EnumMember]
		ExchangeAdmin,
		[EnumMember]
		ExchangeIWOptions,
		[EnumMember]
		ComplianceCenter,
		[EnumMember]
		WAC
	}
}
