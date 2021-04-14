using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[KnownType(typeof(BindingConfiguration))]
	[DataContract]
	public class GetObjectResultForBinding
	{
		[DataMember]
		public BindingConfiguration GetObjectResult { get; set; }
	}
}
