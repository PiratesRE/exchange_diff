using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class ImportContactListRequest : BaseJsonRequest
	{
		public ImportContactListRequest()
		{
			this.ImportedContactList = new ImportContactList();
		}

		[DataMember(IsRequired = true)]
		public ImportContactList ImportedContactList { get; set; }

		public override string ToString()
		{
			return string.Format("ImportContactListRequest: {0}", this.ImportedContactList);
		}
	}
}
