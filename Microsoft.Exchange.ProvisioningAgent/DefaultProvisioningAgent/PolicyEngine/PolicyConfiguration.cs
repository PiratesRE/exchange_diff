using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.DefaultProvisioningAgent.PolicyEngine
{
	internal static class PolicyConfiguration
	{
		static PolicyConfiguration()
		{
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("enable-Mailbox", typeof(Mailbox));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("enable-MailContact", typeof(MailContact));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("enable-MailUser", typeof(MailUser));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("Enable-MailPublicFolder", typeof(ADPublicFolder));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("enable-RemoteMailbox", typeof(RemoteMailbox));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("enable-DistributionGroup", typeof(DistributionGroup));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("new-Mailbox", typeof(Mailbox));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("new-MailContact", typeof(MailContact));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("new-MailUser", typeof(MailUser));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("new-RemoteMailbox", typeof(RemoteMailbox));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("new-SyncMailbox", typeof(SyncMailbox));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("new-SyncMailContact", typeof(SyncMailContact));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("new-SyncMailUser", typeof(SyncMailUser));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("New-SyncMailPublicFolder", typeof(ADPublicFolder));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("new-DistributionGroup", typeof(DistributionGroup));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("new-SyncDistributionGroup", typeof(SyncDistributionGroup));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("new-DynamicDistributionGroup", typeof(DynamicDistributionGroup));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("undo-SoftDeletedMailbox", typeof(Mailbox));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("undo-SyncSoftDeletedMailbox", typeof(SyncMailbox));
			PolicyConfiguration.Task2DefaultObjectTypeDictionary.Add("undo-SyncSoftDeletedMailUser", typeof(SyncMailUser));
			PolicyConfiguration.ObjectType2PolicyEntryDictionary.Add(typeof(Mailbox), new PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>());
			PolicyConfiguration.ObjectType2PolicyEntryDictionary.Add(typeof(MailContact), new PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>());
			PolicyConfiguration.ObjectType2PolicyEntryDictionary.Add(typeof(MailUser), new PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>());
			PolicyConfiguration.ObjectType2PolicyEntryDictionary.Add(typeof(RemoteMailbox), new PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>());
			PolicyConfiguration.ObjectType2PolicyEntryDictionary.Add(typeof(SyncMailbox), new PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>());
			PolicyConfiguration.ObjectType2PolicyEntryDictionary.Add(typeof(SyncMailContact), new PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>());
			PolicyConfiguration.ObjectType2PolicyEntryDictionary.Add(typeof(SyncMailUser), new PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>());
			PolicyConfiguration.ObjectType2PolicyEntryDictionary.Add(typeof(DistributionGroup), new PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>());
			PolicyConfiguration.ObjectType2PolicyEntryDictionary.Add(typeof(SyncDistributionGroup), new PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>());
			PolicyConfiguration.ObjectType2PolicyEntryDictionary.Add(typeof(DynamicDistributionGroup), new PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>());
			PolicyConfiguration.ObjectType2PolicyEntryDictionary.Add(typeof(MailPublicFolder), new PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>());
			PolicyConfiguration.ObjectType2PolicyEntryDictionary.Add(typeof(ADPublicFolder), new PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>());
		}

		public static readonly Dictionary<string, Type> Task2DefaultObjectTypeDictionary = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

		public static readonly Dictionary<Type, PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>> ObjectType2PolicyEntryDictionary = new Dictionary<Type, PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy>>();
	}
}
