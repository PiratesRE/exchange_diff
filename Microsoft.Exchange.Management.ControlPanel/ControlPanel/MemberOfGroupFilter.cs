using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MemberOfGroupFilter : GroupFilter
	{
		protected override string AdditionalFilterText
		{
			get
			{
				string text = (RbacPrincipal.Current.ExecutingUserId != null) ? RbacPrincipal.Current.ExecutingUserId.DistinguishedName : string.Empty;
				return string.Format("Members -eq '{0}'", text.Replace("'", "''"));
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:MyGAL";
			}
		}
	}
}
