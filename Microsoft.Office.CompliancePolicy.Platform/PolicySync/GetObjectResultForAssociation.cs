using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[KnownType(typeof(AssociationConfiguration))]
	[DataContract]
	public class GetObjectResultForAssociation
	{
		[DataMember]
		public AssociationConfiguration GetObjectResult { get; set; }
	}
}
