using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[KnownType(typeof(InvalidContextException))]
	[KnownType(typeof(GroupMemberNotFoundException))]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ObjectNotFoundException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[KnownType(typeof(ServicePrincipalNotFoundException))]
	[KnownType(typeof(UserNotFoundException))]
	[KnownType(typeof(CompanyNotFoundException))]
	[KnownType(typeof(RoleNotFoundException))]
	[KnownType(typeof(RoleMemberNotFoundException))]
	[KnownType(typeof(SubscriptionNotFoundException))]
	[KnownType(typeof(DomainNotFoundException))]
	[KnownType(typeof(ContractNotFoundException))]
	[KnownType(typeof(ContactNotFoundException))]
	[KnownType(typeof(GroupNotFoundException))]
	public class ObjectNotFoundException : DataOperationException
	{
		[DataMember]
		public string ObjectKey
		{
			get
			{
				return this.ObjectKeyField;
			}
			set
			{
				this.ObjectKeyField = value;
			}
		}

		private string ObjectKeyField;
	}
}
