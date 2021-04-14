using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "StringLengthValidationException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class StringLengthValidationException : PropertyValidationException
	{
		[DataMember]
		public int? MaxLength
		{
			get
			{
				return this.MaxLengthField;
			}
			set
			{
				this.MaxLengthField = value;
			}
		}

		[DataMember]
		public int? MinLength
		{
			get
			{
				return this.MinLengthField;
			}
			set
			{
				this.MinLengthField = value;
			}
		}

		private int? MaxLengthField;

		private int? MinLengthField;
	}
}
