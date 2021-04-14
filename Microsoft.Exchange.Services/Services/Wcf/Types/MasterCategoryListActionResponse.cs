using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MasterCategoryListActionResponse
	{
		public MasterCategoryListActionResponse(CategoryType[] masterCategoryList)
		{
			this.WasSuccessful = true;
			this.MasterList = masterCategoryList;
		}

		public MasterCategoryListActionResponse(MasterCategoryListActionError errorCode)
		{
			this.WasSuccessful = false;
			this.ErrorCode = errorCode;
		}

		[DataMember]
		public CategoryType[] MasterList { get; set; }

		[DataMember]
		public bool WasSuccessful { get; set; }

		[DataMember]
		public MasterCategoryListActionError ErrorCode { get; set; }
	}
}
