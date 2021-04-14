using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "GroupSearchDefinition", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	public class GroupSearchDefinition : SearchDefinition
	{
		[DataMember]
		public GroupType? GroupType
		{
			get
			{
				return this.GroupTypeField;
			}
			set
			{
				this.GroupTypeField = value;
			}
		}

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

		[DataMember]
		public bool? IsAgentRole
		{
			get
			{
				return this.IsAgentRoleField;
			}
			set
			{
				this.IsAgentRoleField = value;
			}
		}

		[DataMember]
		public Guid? UserObjectId
		{
			get
			{
				return this.UserObjectIdField;
			}
			set
			{
				this.UserObjectIdField = value;
			}
		}

		[DataMember]
		public string UserPrincipalName
		{
			get
			{
				return this.UserPrincipalNameField;
			}
			set
			{
				this.UserPrincipalNameField = value;
			}
		}

		private GroupType? GroupTypeField;

		private bool? HasErrorsOnlyField;

		private string[] IncludedPropertiesField;

		private bool? IsAgentRoleField;

		private Guid? UserObjectIdField;

		private string UserPrincipalNameField;
	}
}
