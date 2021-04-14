using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ModernGroupMember")]
	[Serializable]
	public class ModernGroupMemberType : ItemType
	{
		[DataMember]
		public Persona Persona { get; set; }

		[DataMember]
		public bool IsOwner { get; set; }
	}
}
