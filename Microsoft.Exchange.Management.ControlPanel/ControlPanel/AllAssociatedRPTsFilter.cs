using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AllAssociatedRPTsFilter : AllRPTsFilter
	{
		public AllAssociatedRPTsFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		public bool IncludeDefaultTags
		{
			get
			{
				return this.includeDefaultTags;
			}
			set
			{
				this.includeDefaultTags = value;
				if (this.includeDefaultTags)
				{
					base["Types"] = AllAssociatedRPTsFilter.retentionPolicyTagTypes;
					return;
				}
				base["Types"] = ElcFolderType.Personal;
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			base["Mailbox"] = RbacPrincipal.Current.ExecutingUserId;
		}

		public new const string RbacParameters = "?Types&Mailbox";

		private static readonly ElcFolderType[] retentionPolicyTagTypes = new ElcFolderType[]
		{
			ElcFolderType.Personal,
			ElcFolderType.All
		};

		private bool includeDefaultTags;
	}
}
