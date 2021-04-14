using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[KnownType(typeof(TooManyUnverifiedDomainException))]
	[KnownType(typeof(UniquenessValidationException))]
	[KnownType(typeof(InvalidPasswordWeakException))]
	[KnownType(typeof(PropertyDomainValidationException))]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "PropertyValidationException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[KnownType(typeof(StringLengthValidationException))]
	[KnownType(typeof(StringSyntaxValidationException))]
	[KnownType(typeof(InvalidPasswordException))]
	[DebuggerStepThrough]
	[KnownType(typeof(InvalidPasswordContainMemberNameException))]
	[KnownType(typeof(IncorrectPasswordException))]
	[KnownType(typeof(ServicePrincipalCredentialNotSettableException))]
	[KnownType(typeof(ItemCountValidationException))]
	[KnownType(typeof(TooManySearchResultsException))]
	[KnownType(typeof(PropertyNotSettableException))]
	public class PropertyValidationException : MsolAdministrationException
	{
		[DataMember]
		public string ParentObjectType
		{
			get
			{
				return this.ParentObjectTypeField;
			}
			set
			{
				this.ParentObjectTypeField = value;
			}
		}

		[DataMember]
		public string PropertyName
		{
			get
			{
				return this.PropertyNameField;
			}
			set
			{
				this.PropertyNameField = value;
			}
		}

		private string ParentObjectTypeField;

		private string PropertyNameField;
	}
}
