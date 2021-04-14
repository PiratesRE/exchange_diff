using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class ImportContactListResponse : OptionsResponseBase
	{
		[DataMember(IsRequired = true)]
		public int NumberOfContactsImported { get; set; }

		public override string ToString()
		{
			return string.Format("ImportContactListResponse: {0}", this.NumberOfContactsImported);
		}
	}
}
