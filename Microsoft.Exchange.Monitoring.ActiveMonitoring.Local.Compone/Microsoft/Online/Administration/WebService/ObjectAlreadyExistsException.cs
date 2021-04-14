using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[KnownType(typeof(MemberAlreadyExistsException))]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ObjectAlreadyExistsException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[KnownType(typeof(GroupAlreadyExistsException))]
	[KnownType(typeof(DomainNameConflictException))]
	[KnownType(typeof(UserAlreadyExistsException))]
	[KnownType(typeof(DomainAlreadyExistsInOldSystemException))]
	[KnownType(typeof(DomainAlreadyExistsException))]
	public class ObjectAlreadyExistsException : DataOperationException
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
