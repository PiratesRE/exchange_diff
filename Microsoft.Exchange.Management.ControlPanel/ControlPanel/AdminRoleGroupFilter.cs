using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AdminRoleGroupFilter : SearchTextFilter
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-RoleGroup";
			}
		}

		[DataMember]
		internal string ManagementScope { get; set; }

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}

		protected override string[] FilterableProperties
		{
			get
			{
				return AdminRoleGroupFilter.filterableProperties;
			}
		}

		protected override SearchTextFilterType FilterType
		{
			get
			{
				return SearchTextFilterType.Contains;
			}
		}

		private static string[] filterableProperties = new string[]
		{
			"Name"
		};
	}
}
