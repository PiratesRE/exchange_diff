using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "SetGroupRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class SetGroupRequest : Request
	{
		[DataMember]
		public Group Group
		{
			get
			{
				return this.GroupField;
			}
			set
			{
				this.GroupField = value;
			}
		}

		private Group GroupField;
	}
}
