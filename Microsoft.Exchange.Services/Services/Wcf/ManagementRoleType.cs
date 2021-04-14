using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[XmlRoot(ElementName = "ManagementRole", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class ManagementRoleType
	{
		[XmlArray(Order = 0)]
		[XmlArrayItem("Role", IsNullable = false)]
		[DataMember(Name = "UserRoles", IsRequired = false)]
		public string[] UserRoles
		{
			get
			{
				return this.userRolesField;
			}
			set
			{
				this.userRolesField = value;
			}
		}

		[XmlArray(Order = 1)]
		[XmlArrayItem("Role", IsNullable = false)]
		[DataMember(Name = "ApplicationRoles", IsRequired = false)]
		public string[] ApplicationRoles
		{
			get
			{
				return this.applicationRolesField;
			}
			set
			{
				this.applicationRolesField = value;
			}
		}

		internal bool HasUserRoles
		{
			get
			{
				return this.UserRoleTypes != null && this.UserRoleTypes.Length > 0;
			}
		}

		internal bool HasApplicationRoles
		{
			get
			{
				return this.ApplicationRoles != null && this.ApplicationRoles.Length > 0;
			}
		}

		internal RoleType[] UserRoleTypes
		{
			get
			{
				this.ValidateAndConvert();
				return this.userRoleTypes;
			}
		}

		internal RoleType[] ApplicationRoleTypes
		{
			get
			{
				this.ValidateAndConvert();
				return this.applicationRoleTypes;
			}
			set
			{
				this.applicationRoleTypes = value;
			}
		}

		internal void ValidateAndConvert()
		{
			if (this.validated)
			{
				return;
			}
			try
			{
				if (this.userRolesField != null)
				{
					this.userRoleTypes = (from role in this.userRolesField
					select (RoleType)Enum.Parse(typeof(RoleType), role, true)).Distinct<RoleType>().ToArray<RoleType>();
				}
				if (this.applicationRolesField != null)
				{
					this.applicationRoleTypes = (from role in this.applicationRolesField
					select (RoleType)Enum.Parse(typeof(RoleType), role, true)).Distinct<RoleType>().ToArray<RoleType>();
				}
				this.validated = true;
			}
			catch (ArgumentException arg)
			{
				ExTraceGlobals.AuthorizationTracer.TraceDebug<ArgumentException>(0L, "[ManagementRoleType.ValidateAndConvert] hit argument exception: {0}", arg);
				throw new InvalidManagementRoleHeaderException((CoreResources.IDs)2448725207U);
			}
		}

		private bool validated;

		private string[] userRolesField;

		private string[] applicationRolesField;

		private RoleType[] userRoleTypes;

		private RoleType[] applicationRoleTypes;
	}
}
