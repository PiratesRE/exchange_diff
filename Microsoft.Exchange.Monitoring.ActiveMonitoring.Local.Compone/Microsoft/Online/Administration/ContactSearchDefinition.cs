using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "ContactSearchDefinition", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class ContactSearchDefinition : SearchDefinition
	{
		[DataMember]
		public bool? HasErrorsOnly
		{
			get
			{
				return this.HasErrorsOnlyField;
			}
			set
			{
				this.HasErrorsOnlyField = value;
			}
		}

		[DataMember]
		public string[] IncludedProperties
		{
			get
			{
				return this.IncludedPropertiesField;
			}
			set
			{
				this.IncludedPropertiesField = value;
			}
		}

		private bool? HasErrorsOnlyField;

		private string[] IncludedPropertiesField;
	}
}
