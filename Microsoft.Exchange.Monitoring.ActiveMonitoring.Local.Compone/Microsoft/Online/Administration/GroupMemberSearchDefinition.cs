using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "GroupMemberSearchDefinition", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class GroupMemberSearchDefinition : SearchDefinition
	{
		[DataMember]
		public Guid GroupObjectId
		{
			get
			{
				return this.GroupObjectIdField;
			}
			set
			{
				this.GroupObjectIdField = value;
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

		private Guid GroupObjectIdField;

		private string[] IncludedPropertiesField;
	}
}
