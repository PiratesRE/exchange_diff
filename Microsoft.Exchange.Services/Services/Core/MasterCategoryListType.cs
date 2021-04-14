using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class MasterCategoryListType
	{
		[DataMember]
		public CategoryType[] MasterList { get; set; }

		[DataMember]
		public CategoryType[] DefaultList { get; set; }
	}
}
