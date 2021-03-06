using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UpdateMasterCategoryListResponse : MasterCategoryListActionResponse
	{
		public UpdateMasterCategoryListResponse(CategoryType[] masterCategoryList) : base(masterCategoryList)
		{
		}
	}
}
