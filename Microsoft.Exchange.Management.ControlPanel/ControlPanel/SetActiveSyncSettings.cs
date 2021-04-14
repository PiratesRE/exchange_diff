using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetActiveSyncSettings : SetObjectProperties
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Set-ActiveSyncOrganizationSettings";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@C:OrganizationConfig";
			}
		}

		[DataMember]
		public string DefaultAccessLevel
		{
			get
			{
				return (string)base["DefaultAccessLevel"];
			}
			set
			{
				base["DefaultAccessLevel"] = value;
			}
		}

		[DataMember]
		public string[] AdminMailRecipients
		{
			get
			{
				return (string[])base[ActiveSyncOrganizationSettingsSchema.AdminMailRecipients];
			}
			set
			{
				base[ActiveSyncOrganizationSettingsSchema.AdminMailRecipients] = value;
			}
		}

		[DataMember]
		public string UserMailInsert
		{
			get
			{
				return (string)base[ActiveSyncOrganizationSettingsSchema.UserMailInsert];
			}
			set
			{
				base[ActiveSyncOrganizationSettingsSchema.UserMailInsert] = value;
			}
		}
	}
}
