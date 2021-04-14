using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[KnownType(typeof(InvalidPasswordContainMemberNameException))]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "StringSyntaxValidationException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[KnownType(typeof(IncorrectPasswordException))]
	[DebuggerStepThrough]
	[KnownType(typeof(InvalidPasswordException))]
	[KnownType(typeof(InvalidPasswordWeakException))]
	public class StringSyntaxValidationException : PropertyValidationException
	{
		[DataMember]
		public string RegularExpression
		{
			get
			{
				return this.RegularExpressionField;
			}
			set
			{
				this.RegularExpressionField = value;
			}
		}

		private string RegularExpressionField;
	}
}
