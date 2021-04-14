using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncGroupSchema : SyncRecipientSchema
	{
		public override DirectoryObjectClass DirectoryObjectClass
		{
			get
			{
				return DirectoryObjectClass.Group;
			}
		}

		public static SyncPropertyDefinition GroupType = new SyncPropertyDefinition(ADGroupSchema.GroupType, null, typeof(object), SyncPropertyDefinitionFlags.Ignore, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition CoManagedBy = new SyncPropertyDefinition(ADGroupSchema.CoManagedBy, "MSExchCoManagedByLink", typeof(SyncLink), typeof(MSExchCoManagedByLink), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition IsHierarchicalGroup = new SyncPropertyDefinition(ADGroupSchema.IsOrganizationalGroup, "MSOrgIsOrganizational", typeof(DirectoryPropertyBooleanSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition MailEnabled = new SyncPropertyDefinition("MailEnabled", "MailEnabled", typeof(bool), typeof(bool), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.AlwaysReturned, SyncPropertyDefinition.InitialSyncPropertySetVersion, false);

		public static SyncPropertyDefinition ManagedBy = new SyncPropertyDefinition(ADGroupSchema.RawManagedBy, "ManagedBy", typeof(SyncLink), typeof(ManagedBy), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Members = new SyncPropertyDefinition(ADGroupSchema.Members, "Member", typeof(SyncLink), typeof(Member), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ReportToManagerEnabled = new SyncPropertyDefinition(ADGroupSchema.ReportToManagerEnabled, "ReportToOwner", typeof(DirectoryPropertyBooleanSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ReportToOriginatorEnabled = new SyncPropertyDefinition(ADGroupSchema.ReportToOriginatorEnabled, "ReportToOriginator", typeof(bool), typeof(DirectoryPropertyBooleanSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition SecurityEnabled = new SyncPropertyDefinition("SecurityEnabled", "SecurityEnabled", typeof(bool), typeof(DirectoryPropertyBooleanSingle), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.Calculated | SyncPropertyDefinitionFlags.PersistDefaultValue, SyncPropertyDefinition.InitialSyncPropertySetVersion, false, new SyncPropertyDefinition[]
		{
			SyncGroupSchema.GroupType
		}, ADObject.FlagGetterDelegate(SyncGroupSchema.GroupType, int.MinValue), new SetterDelegate(SyncGroup.SecurityEnabledSetter));

		public static SyncPropertyDefinition SendOofMessageToOriginatorEnabled = new SyncPropertyDefinition(ADGroupSchema.SendOofMessageToOriginatorEnabled, "OofReplyToOriginator", typeof(bool), typeof(DirectoryPropertyBooleanSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition WellKnownObject = new SyncPropertyDefinition("WellKnownObject", "WellKnownObject", typeof(string), typeof(DirectoryPropertyStringSingleLength1To40), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.AlwaysReturned, SyncPropertyDefinition.InitialSyncPropertySetVersion, string.Empty);

		public new static SyncPropertyDefinition RecipientTypeDetailsValue = new SyncPropertyDefinition(ADRecipientSchema.RecipientTypeDetailsValue, "MSExchRecipientTypeDetails", typeof(DirectoryPropertyInt64Single), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.AlwaysReturned, SyncPropertyDefinition.SyncPropertySetVersion14);

		public static SyncPropertyDefinition Creator = new SyncPropertyDefinition("Creator", "CreatedOnBehalfOf", typeof(string), typeof(DirectoryPropertyReferenceUserAndServicePrincipalSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.SyncPropertySetVersion15, string.Empty);

		public static SyncPropertyDefinition SharePointResources = new SyncPropertyDefinition("SharePointResources", "SharepointResources", typeof(string), typeof(DirectoryPropertyStringLength1To1024), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.SyncPropertySetVersion15, null);

		public static SyncPropertyDefinition Owners = new SyncPropertyDefinition(ADUserSchema.Owners, "Owner", typeof(SyncLink), typeof(Owner), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.SyncPropertySetVersion15);

		public static SyncPropertyDefinition IsPublic = new SyncPropertyDefinition("IsPublic", "IsPublic", typeof(bool), typeof(bool), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.SyncPropertySetVersion15, false);

		public static SyncPropertyDefinition Description = new SyncPropertyDefinition(ADRecipientSchema.Description, "Description", typeof(string), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.SyncPropertySetVersion17);
	}
}
