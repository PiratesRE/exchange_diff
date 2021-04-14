using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class BaseRoleGroupParameters : SetObjectProperties
	{
		[DataMember]
		public string Name
		{
			get
			{
				return (string)base[ADObjectSchema.Name];
			}
			set
			{
				base[ADObjectSchema.Name] = value;
			}
		}

		[DataMember]
		public string Description
		{
			get
			{
				return (string)base[ADRecipientSchema.Description];
			}
			set
			{
				base[ADRecipientSchema.Description] = value.Trim();
			}
		}

		[DataMember]
		public AggregatedScope AggregatedScope { get; set; }

		public bool IsScopeModified
		{
			get
			{
				return this.AggregatedScope != null;
			}
		}

		public string ManagementScopeId
		{
			get
			{
				if (this.IsScopeModified)
				{
					return this.AggregatedScope.ID;
				}
				return null;
			}
		}

		public bool IsOrganizationalUnit
		{
			get
			{
				return this.IsScopeModified && this.AggregatedScope.IsOrganizationalUnit;
			}
		}

		public ManagementScopeRow ManagementScopeRow { get; set; }

		public AdminRoleGroupObject OriginalObject { get; set; }

		public ExtendedOrganizationalUnit OrganizationalUnitRow { get; set; }

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}
	}
}
