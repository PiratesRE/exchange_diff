using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Mapi;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal static class AggregationHelper
	{
		internal static bool IsMailboxRole
		{
			get
			{
				if (AggregationHelper.isMailboxRole == null)
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\MailboxRole"))
					{
						AggregationHelper.isMailboxRole = new bool?(registryKey != null);
					}
				}
				return AggregationHelper.isMailboxRole.Value;
			}
		}

		public static void FilterPropertyDefinitionsByBackendSource(IEnumerable<PropertyDefinition> properties, MbxReadMode mbxReadMode, out List<ADPropertyDefinition> adProps, out List<MServPropertyDefinition> mservProps, out List<MbxPropertyDefinition> mbxProps)
		{
			adProps = new List<ADPropertyDefinition>();
			mservProps = new List<MServPropertyDefinition>();
			mbxProps = new List<MbxPropertyDefinition>();
			bool flag = !ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("ConsumerMbxLookupDisabled");
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				ADPropertyDefinition adpropertyDefinition = propertyDefinition as ADPropertyDefinition;
				if (propertyDefinition != null)
				{
					if (adpropertyDefinition.MServPropertyDefinition != null)
					{
						mservProps.Add((MServPropertyDefinition)adpropertyDefinition.MServPropertyDefinition);
					}
					if (adpropertyDefinition.MbxPropertyDefinition != null && mbxReadMode != MbxReadMode.NoMbxRead && flag)
					{
						mbxProps.Add((MbxPropertyDefinition)adpropertyDefinition.MbxPropertyDefinition);
					}
					adProps.Add(adpropertyDefinition);
				}
			}
		}

		[Conditional("DEBUG")]
		private static void CheckPropertyDefinitionsForConsistency(ADPropertyDefinition adProp, SimpleProviderPropertyDefinition propertyDefinition, bool checkReadonly = false)
		{
			if (adProp == null)
			{
				throw new ArgumentNullException("adProp");
			}
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			if (adProp.Name != propertyDefinition.Name)
			{
				throw new ArgumentException(string.Format("Underlying property definition for ADPropertyDefinition {0} has non-matching name: {1}", adProp.Name, propertyDefinition.Name));
			}
			if (adProp.IsMultivalued != propertyDefinition.IsMultivalued)
			{
				throw new ArgumentException(string.Format("ADPropertyDefinition {0} and underlying property definition must have the same IsMultivalued value", adProp.Name));
			}
			if (adProp.Type != propertyDefinition.Type && adProp.Type != Nullable.GetUnderlyingType(propertyDefinition.Type))
			{
				throw new ArgumentException(string.Format("ADPropertyDefinition {0}: underlying property definition must have either the same Type, or Nullable version of it", adProp.Name));
			}
			if (checkReadonly && adProp.IsReadOnly)
			{
				throw new ArgumentException(string.Format("ADPropertyDefinition {0} has underlying property definition but is read-only", adProp.Name));
			}
		}

		public static ADRawEntry PerformMservLookupByPuid(ulong puid, bool isReadOnly, List<MServPropertyDefinition> properties)
		{
			new ADPropertyBag();
			ADRawEntry result;
			using (MservRecipientSession mservRecipientSession = new MservRecipientSession(isReadOnly))
			{
				result = mservRecipientSession.FindADRawEntryByPuid(puid, properties);
			}
			return result;
		}

		public static ADRawEntry PerformMbxLookupByPuid(ADObjectId resultId, Guid mdbGuid, bool isReadOnly, List<MbxPropertyDefinition> properties)
		{
			ulong puid;
			if (ConsumerIdentityHelper.TryGetPuidFromGuid(resultId.ObjectGuid, out puid))
			{
				return MbxRecipientSession.FindADRawEntryByPuid(puid, mdbGuid, isReadOnly, properties);
			}
			throw new ArgumentException("resultId");
		}

		public static ADRawEntry PerformMservLookupByMemberName(SmtpAddress memberName, bool isReadOnly, List<MServPropertyDefinition> properties)
		{
			if (!properties.Contains(MServRecipientSchema.NetID))
			{
				properties.Add(MServRecipientSchema.NetID);
			}
			new ADPropertyBag();
			ADRawEntry result;
			using (MservRecipientSession mservRecipientSession = new MservRecipientSession(isReadOnly))
			{
				result = mservRecipientSession.FindADRawEntryByEmailAddress(memberName.ToString(), properties);
			}
			return result;
		}

		public static void PerformMservModification(ADPropertyBag mservPropertyBag)
		{
			using (MservRecipientSession mservRecipientSession = new MservRecipientSession(false))
			{
				ADRawEntry instanceToSave = new ADRawEntry(mservPropertyBag);
				mservRecipientSession.Save(instanceToSave);
			}
		}

		public static void PerformMbxModification(Guid mdbGuid, Guid mbxGuid, ADPropertyBag properties, bool isNew)
		{
			if (isNew)
			{
				MbxRecipientSession.CreateUserInformationRecord(mdbGuid, mbxGuid, properties);
				return;
			}
			try
			{
				MbxRecipientSession.UpdateUserInformationRecord(mdbGuid, mbxGuid, properties, null);
			}
			catch (ADDriverStoreAccessPermanentException ex)
			{
				if (ex.InnerException == null || !(ex.InnerException is MapiExceptionUserInformationNotFound))
				{
					throw;
				}
				MbxRecipientSession.CreateUserInformationRecord(mdbGuid, mbxGuid, properties);
			}
		}

		public static ADRawEntry PerformADLookup(ADObjectId identity, List<ADPropertyDefinition> properties)
		{
			ADUser aduser = (ADUser)TemplateTenantConfiguration.GetLocalTempateUser().Clone();
			ADPropertyBag adpropertyBag = new ADPropertyBag();
			foreach (ADPropertyDefinition adpropertyDefinition in properties)
			{
				adpropertyBag.SetField(adpropertyDefinition, aduser[adpropertyDefinition]);
				if (adpropertyDefinition.IsCalculated)
				{
					foreach (ProviderPropertyDefinition providerPropertyDefinition in adpropertyDefinition.SupportingProperties)
					{
						ADPropertyDefinition adpropertyDefinition2 = (ADPropertyDefinition)providerPropertyDefinition;
						adpropertyBag.SetField(adpropertyDefinition2, aduser[adpropertyDefinition2]);
					}
				}
			}
			ADRawEntry adrawEntry = new ADRawEntry(adpropertyBag);
			adrawEntry.SetId(identity);
			adrawEntry.ValidateRead();
			adrawEntry.ResetChangeTracking(true);
			return adrawEntry;
		}

		private static bool? isMailboxRole = null;
	}
}
