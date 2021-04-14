using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SecurityPrincipalPickerFilter : SearchTextFilter
	{
		public SecurityPrincipalPickerFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-SecurityPrincipal";
			}
		}

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
				return SecurityPrincipalPickerFilter.filterableProperties;
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			SecurityPrincipalType[] value = new SecurityPrincipalType[]
			{
				SecurityPrincipalType.UniversalSecurityGroup,
				SecurityPrincipalType.User
			};
			base["Types"] = value;
			base["RoleGroupAssignable"] = true;
		}

		protected override SearchTextFilterType FilterType
		{
			get
			{
				if (DDIHelper.IsFFO())
				{
					return SearchTextFilterType.Equals;
				}
				return base.FilterType;
			}
		}

		public new const string RbacParameters = "?ResultSize&Filter&Types&RoleGroupAssignable";

		internal const string GetCmdlet = "Get-SecurityPrincipal";

		private static string[] filterableProperties = new string[]
		{
			"Name",
			"DisplayName"
		};
	}
}
