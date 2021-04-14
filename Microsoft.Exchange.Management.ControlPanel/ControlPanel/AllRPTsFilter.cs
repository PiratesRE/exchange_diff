using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AllRPTsFilter : WebServiceParameters
	{
		public AllRPTsFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-RetentionPolicyTag";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@C:OrganizationConfig";
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			base["Types"] = ElcFolderType.Personal;
		}

		public const string RbacParameters = "?Types";
	}
}
