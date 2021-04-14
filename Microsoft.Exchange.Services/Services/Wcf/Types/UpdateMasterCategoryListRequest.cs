using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UpdateMasterCategoryListRequest
	{
		[DataMember(Name = "AddCategoryList", IsRequired = false)]
		public CategoryType[] AddCategoryList { get; set; }

		[DataMember(Name = "RemoveCategoryList", IsRequired = false)]
		public string[] RemoveCategoryList { get; set; }

		[DataMember(Name = "ChangeCategoryColorList", IsRequired = false)]
		public CategoryType[] ChangeCategoryColorList { get; set; }
	}
}
