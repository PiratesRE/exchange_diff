using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "ListRoleMemberResults", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class ListRoleMemberResults : ListResults
	{
		[DataMember]
		public RoleMember[] Results
		{
			get
			{
				return this.ResultsField;
			}
			set
			{
				this.ResultsField = value;
			}
		}

		private RoleMember[] ResultsField;
	}
}
