using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class IdentityCollectionRequest : BaseJsonRequest
	{
		public IdentityCollectionRequest()
		{
			this.IdentityCollection = new IdentityCollection();
		}

		[DataMember(IsRequired = true)]
		public IdentityCollection IdentityCollection { get; set; }

		public override string ToString()
		{
			return string.Format("IdentityCollectionRequest: {0}", this.IdentityCollection);
		}
	}
}
