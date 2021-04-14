using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	public abstract class IncrementalAttributeBase
	{
		[DataMember]
		public abstract bool Changed { get; protected set; }

		public abstract object GetObjectValue();
	}
}
