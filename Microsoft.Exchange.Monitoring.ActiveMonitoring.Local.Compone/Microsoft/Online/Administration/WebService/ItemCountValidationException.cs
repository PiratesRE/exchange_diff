using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[KnownType(typeof(TooManyUnverifiedDomainException))]
	[KnownType(typeof(TooManySearchResultsException))]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ItemCountValidationException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class ItemCountValidationException : PropertyValidationException
	{
		[DataMember]
		public int? MaxCount
		{
			get
			{
				return this.MaxCountField;
			}
			set
			{
				this.MaxCountField = value;
			}
		}

		[DataMember]
		public int? MinCount
		{
			get
			{
				return this.MinCountField;
			}
			set
			{
				this.MinCountField = value;
			}
		}

		private int? MaxCountField;

		private int? MinCountField;
	}
}
