using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConfigurationItemSchema : ItemSchema
	{
		public new static ConfigurationItemSchema Instance
		{
			get
			{
				if (ConfigurationItemSchema.instance == null)
				{
					ConfigurationItemSchema.instance = new ConfigurationItemSchema();
				}
				return ConfigurationItemSchema.instance;
			}
		}

		internal override void CoreObjectUpdate(CoreItem coreItem, CoreItemOperation operation)
		{
			base.CoreObjectUpdate(coreItem, operation);
			this.PrepareAggregatedUserConfigurationUpdate(coreItem);
		}

		internal override void CoreObjectUpdateComplete(CoreItem coreItem, SaveResult saveResult)
		{
			base.CoreObjectUpdateComplete(coreItem, saveResult);
			this.CompleteAggregatedUserConfigurationUpdates(coreItem, saveResult);
		}

		private void PrepareAggregatedUserConfigurationUpdate(CoreItem coreItem)
		{
			if (!coreItem.CoreObjectUpdateContext.ContainsKey(this) && this.IsEnabledForConfigurationAggregation(coreItem))
			{
				IList<IAggregatedUserConfigurationWriter> writers = AggregatedUserConfiguration.GetWriters(AggregatedUserConfigurationSchema.Instance, coreItem.Session as IMailboxSession, coreItem);
				if (writers != null)
				{
					coreItem.CoreObjectUpdateContext[this] = writers;
					foreach (IAggregatedUserConfigurationWriter aggregatedUserConfigurationWriter in writers)
					{
						aggregatedUserConfigurationWriter.Prepare();
					}
				}
			}
		}

		private void CompleteAggregatedUserConfigurationUpdates(CoreItem coreItem, SaveResult saveResult)
		{
			if (saveResult == SaveResult.Success || saveResult == SaveResult.SuccessWithConflictResolution)
			{
				object obj = null;
				if (coreItem.CoreObjectUpdateContext.TryGetValue(this, out obj))
				{
					coreItem.CoreObjectUpdateContext.Remove(this);
					IList<IAggregatedUserConfigurationWriter> list = obj as IList<IAggregatedUserConfigurationWriter>;
					if (list != null)
					{
						foreach (IAggregatedUserConfigurationWriter aggregatedUserConfigurationWriter in list)
						{
							aggregatedUserConfigurationWriter.Commit();
						}
					}
				}
			}
		}

		private bool IsEnabledForConfigurationAggregation(CoreItem coreItem)
		{
			return coreItem.Session != null && coreItem.Session.MailboxOwner != null && ConfigurationItemSchema.IsEnabledForConfigurationAggregation(coreItem.Session.MailboxOwner);
		}

		internal static bool IsEnabledForConfigurationAggregation(IExchangePrincipal principal)
		{
			bool result = false;
			if (principal != null && !principal.ObjectId.IsNullOrEmpty())
			{
				result = principal.GetConfiguration().DataStorage.UserConfigurationAggregation.Enabled;
			}
			return result;
		}

		[Autoload]
		internal static readonly StorePropertyDefinition UserConfigurationType = InternalSchema.UserConfigurationType;

		[Autoload]
		internal static readonly StorePropertyDefinition UserConfigurationDictionary = InternalSchema.UserConfigurationDictionary;

		[Autoload]
		internal static readonly StorePropertyDefinition UserConfigurationStream = InternalSchema.UserConfigurationStream;

		[Autoload]
		public static readonly StorePropertyDefinition UserConfigurationXml = InternalSchema.UserConfigurationXml;

		private static ConfigurationItemSchema instance = null;
	}
}
