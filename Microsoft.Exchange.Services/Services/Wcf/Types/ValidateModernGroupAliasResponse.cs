using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Name = "ValidateModernGroupAliasResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ValidateModernGroupAliasResponse : BaseJsonResponse
	{
		internal ValidateModernGroupAliasResponse(string alias, bool isAliasUnique)
		{
			this.Alias = alias;
			this.IsAliasUnique = isAliasUnique;
		}

		[DataMember(Name = "IsAliasUnique", IsRequired = false)]
		public bool IsAliasUnique { get; set; }

		[DataMember(Name = "Alias", IsRequired = false)]
		public string Alias { get; set; }
	}
}
