using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "RequiredPropertyNotSetException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class RequiredPropertyNotSetException : MsolAdministrationException
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
