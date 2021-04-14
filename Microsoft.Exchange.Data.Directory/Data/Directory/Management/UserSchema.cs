using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class UserSchema : OrgPersonPresentationObjectSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADUserSchema>();
		}

		public static readonly ADPropertyDefinition IsSecurityPrincipal = ADMailboxRecipientSchema.IsSecurityPrincipal;

		public static readonly ADPropertyDefinition SamAccountName = ADMailboxRecipientSchema.SamAccountName;

		public static readonly ADPropertyDefinition Sid = ADMailboxRecipientSchema.Sid;

		public static readonly ADPropertyDefinition SidHistory = ADMailboxRecipientSchema.SidHistory;

		public static readonly ADPropertyDefinition UserPrincipalName = ADUserSchema.UserPrincipalName;

		public static readonly ADPropertyDefinition ResetPasswordOnNextLogon = ADUserSchema.ResetPasswordOnNextLogon;

		public static readonly ADPropertyDefinition OrganizationalUnit = ADRecipientSchema.OrganizationalUnit;

		public static readonly ADPropertyDefinition CertificateSubject = ADUserSchema.CertificateSubject;

		public static readonly ADPropertyDefinition RemotePowerShellEnabled = ADRecipientSchema.RemotePowerShellEnabled;

		public static readonly ADPropertyDefinition UserAccountControl = ADUserSchema.UserAccountControl;

		public static readonly ADPropertyDefinition IsLinked = ADRecipientSchema.IsLinked;

		public static readonly ADPropertyDefinition LinkedMasterAccount = ADRecipientSchema.LinkedMasterAccount;

		public static readonly ADPropertyDefinition WindowsLiveID = ADRecipientSchema.WindowsLiveID;

		public static readonly ADPropertyDefinition NetID = ADUserSchema.NetID;

		public static readonly ADPropertyDefinition ConsumerNetID = ADUserSchema.ConsumerNetID;

		public static readonly ADPropertyDefinition OriginalNetID = ADUserSchema.OriginalNetID;

		public static readonly ADPropertyDefinition LEOEnabled = ADRecipientSchema.LEOEnabled;

		public static readonly ADPropertyDefinition ExternalDirectoryObjectId = ADRecipientSchema.ExternalDirectoryObjectId;

		public static readonly ADPropertyDefinition SKUAssigned = ADRecipientSchema.SKUAssigned;

		public static readonly ADPropertyDefinition IsSoftDeletedByRemove = ADRecipientSchema.IsSoftDeletedByRemove;

		public static readonly ADPropertyDefinition IsSoftDeletedByDisable = ADRecipientSchema.IsSoftDeletedByDisable;

		public static readonly ADPropertyDefinition WhenSoftDeleted = ADRecipientSchema.WhenSoftDeleted;

		public static readonly ADPropertyDefinition PreviousRecipientTypeDetails = ADRecipientSchema.PreviousRecipientTypeDetails;

		public static readonly ADPropertyDefinition UpgradeRequest = ADRecipientSchema.UpgradeRequest;

		public static readonly ADPropertyDefinition UpgradeStatus = ADRecipientSchema.UpgradeStatus;

		public static readonly ADPropertyDefinition UpgradeDetails = ADRecipientSchema.UpgradeDetails;

		public static readonly ADPropertyDefinition UpgradeMessage = ADRecipientSchema.UpgradeMessage;

		public static readonly ADPropertyDefinition UpgradeStage = ADRecipientSchema.UpgradeStage;

		public static readonly ADPropertyDefinition UpgradeStageTimeStamp = ADRecipientSchema.UpgradeStageTimeStamp;

		public static readonly ADPropertyDefinition InPlaceHoldsRaw = ADRecipientSchema.InPlaceHoldsRaw;

		public static readonly ADPropertyDefinition MailboxRelease = ADUserSchema.MailboxRelease;

		public static readonly ADPropertyDefinition ArchiveRelease = ADUserSchema.ArchiveRelease;

		public static readonly ADPropertyDefinition MailboxProvisioningConstraint = ADRecipientSchema.MailboxProvisioningConstraint;

		public static readonly ADPropertyDefinition MailboxProvisioningPreferences = ADRecipientSchema.MailboxProvisioningPreferences;
	}
}
