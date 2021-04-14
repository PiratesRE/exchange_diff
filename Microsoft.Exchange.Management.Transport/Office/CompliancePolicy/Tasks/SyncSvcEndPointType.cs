using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal enum SyncSvcEndPointType
	{
		[EnumMember]
		RestOAuth,
		[EnumMember]
		SoapOAuth,
		[EnumMember]
		SoapCert
	}
}
