using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Provisioning.CompanyManagement
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "CompanyNotFoundFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Provisioning.CompanyManagement")]
	[DebuggerStepThrough]
	public class CompanyNotFoundFault : CompanyManagementFault
	{
		[DataMember(IsRequired = true, EmitDefaultValue = false)]
		public Guid ContextId
		{
			get
			{
				return this.ContextIdField;
			}
			set
			{
				this.ContextIdField = value;
			}
		}

		private Guid ContextIdField;
	}
}
