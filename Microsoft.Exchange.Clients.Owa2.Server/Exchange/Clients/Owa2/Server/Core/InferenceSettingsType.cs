using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Clutter;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class InferenceSettingsType : UserConfigurationBaseType
	{
		public InferenceSettingsType() : base(InferenceSettingsType.ConfigurationName)
		{
		}

		[DataMember]
		public bool? IsClutterUIEnabled
		{
			get
			{
				return (bool?)base[UserConfigurationPropertyId.IsClutterUIEnabled];
			}
			set
			{
				base[UserConfigurationPropertyId.IsClutterUIEnabled] = value;
			}
		}

		internal override UserConfigurationPropertySchemaBase Schema
		{
			get
			{
				return InferenceSettingsPropertySchema.Instance;
			}
		}

		internal override IUserConfigurationAccessStrategy CreateConfigurationAccessStrategy()
		{
			return new DefaultFolderConfigurationAccessStrategy(DefaultFolderType.Inbox);
		}

		internal static bool GetFeatureSupportedState(MailboxSession mailboxSession, UserContext userContext)
		{
			bool enabled = userContext.FeaturesManager.ServerSettings.InferenceUI.Enabled;
			bool flag = false;
			if (enabled)
			{
				flag = true;
			}
			return enabled && flag;
		}

		internal bool TryMigrateUserOptionsValue(MailboxSession mailboxSession, UserOptionsType userOptions)
		{
			bool result = false;
			if ((userOptions.UserOptionsMigrationState & UserOptionsMigrationState.ShowInferenceUiElementsMigrated) == UserOptionsMigrationState.None)
			{
				try
				{
					if (this.IsClutterUIEnabled == null && !userOptions.ShowInferenceUiElements)
					{
						ExTraceGlobals.UserOptionsDataTracer.TraceDebug((long)this.GetHashCode(), "Updating Inference.Settings IsClutterUIEnabled for upgrade scenario in TryMigrateUserOptionsValue.");
						this.IsClutterUIEnabled = new bool?(userOptions.ShowInferenceUiElements);
						base.Commit(mailboxSession, new UserConfigurationPropertyDefinition[]
						{
							this.Schema.GetPropertyDefinition(UserConfigurationPropertyId.IsClutterUIEnabled)
						});
					}
					ExTraceGlobals.UserOptionsDataTracer.TraceDebug((long)this.GetHashCode(), "Updating OWA.Settings UserOptionsMigrationState to reflect new ShowInferenceUiElementsMigrated value in TryMigrateUserOptionsValue.");
					userOptions.UserOptionsMigrationState |= UserOptionsMigrationState.ShowInferenceUiElementsMigrated;
					userOptions.Commit(mailboxSession, new UserConfigurationPropertyDefinition[]
					{
						UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.UserOptionsMigrationState)
					});
					result = true;
				}
				catch (StoragePermanentException arg)
				{
					ExTraceGlobals.UserOptionsDataTracer.TraceError<StoragePermanentException, string>(0L, "Permanent error while trying to migrate Inference settings in TryMigrateUserOptionsValue. Error: {0}. User: {1}", arg, mailboxSession.DisplayAddress);
				}
				catch (StorageTransientException arg2)
				{
					ExTraceGlobals.UserOptionsDataTracer.TraceError<StorageTransientException, string>(0L, "Transient error while trying to migrate Inference settings in TryMigrateUserOptionsValue. Error: {0}. User: {1}.", arg2, mailboxSession.DisplayAddress);
				}
				catch (Exception arg3)
				{
					ExTraceGlobals.UserContextCallTracer.TraceError<Exception, string>(0L, "Unexpected error while trying to migrate Inference settings in TryMigrateUserOptionsValue. Error: {0}. User: {1}.", arg3, mailboxSession.DisplayAddress);
				}
			}
			return result;
		}

		internal void LoadAll(MailboxSession session)
		{
			IList<UserConfigurationPropertyDefinition> properties = new List<UserConfigurationPropertyDefinition>(base.OptionProperties.Keys);
			base.Load(session, properties, true);
		}

		internal static void UpdateUserPreferenceFlag(MailboxSession mailboxSession, UserContext userContext, bool enable)
		{
			if (userContext.FeaturesManager.ClientServerSettings.FolderBasedClutter.Enabled)
			{
				InferenceSettingsType.UpdateClutterClassificationEnabled(mailboxSession, userContext.FeaturesManager.ConfigurationSnapshot, enable);
				return;
			}
			if (InferenceSettingsType.GetFeatureSupportedState(mailboxSession, userContext))
			{
				new InferenceSettingsType
				{
					IsClutterUIEnabled = new bool?(enable)
				}.Commit(mailboxSession, new UserConfigurationPropertyDefinition[]
				{
					InferenceSettingsPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.IsClutterUIEnabled)
				});
			}
		}

		internal static void UpdateClutterClassificationEnabled(MailboxSession mailboxSession, VariantConfigurationSnapshot configurationSnapshot, bool enable)
		{
			bool flag = ClutterUtilities.IsClassificationEnabled(mailboxSession, configurationSnapshot);
			if (!flag && enable)
			{
				ClutterUtilities.OptUserIn(mailboxSession, configurationSnapshot, new FrontEndLocator());
				return;
			}
			if (flag && !enable)
			{
				ClutterUtilities.OptUserOut(mailboxSession, configurationSnapshot, new FrontEndLocator());
			}
		}

		internal static void ReadFolderBasedClutterSettings(MailboxSession mailboxSession, VariantConfigurationSnapshot configurationSnapshot, OwaUserConfiguration userConfiguration)
		{
			userConfiguration.SegmentationSettings.PredictedActions = ClutterUtilities.IsClutterEnabled(mailboxSession, configurationSnapshot);
			userConfiguration.UserOptions.ShowInferenceUiElements = ClutterUtilities.IsClassificationEnabled(mailboxSession, configurationSnapshot);
		}

		private static string ConfigurationName = "Inference.Settings";
	}
}
