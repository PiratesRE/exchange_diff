using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public class PresentationRetentionPolicyTag : RetentionPolicyTag
	{
		public PresentationRetentionPolicyTag(RetentionPolicyTag retentionPolicyTag)
		{
			this.propertyBag = retentionPolicyTag.propertyBag;
			this.m_Session = retentionPolicyTag.m_Session;
			this.contentSettings = retentionPolicyTag.GetELCContentSettings().FirstOrDefault<ElcContentSettings>();
		}

		public PresentationRetentionPolicyTag() : this(new RetentionPolicyTag(), new ElcContentSettings())
		{
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.contentSettings != null)
			{
				errors.AddRange(this.contentSettings.Validate());
			}
		}

		public PresentationRetentionPolicyTag(RetentionPolicyTag retentionPolicyTag, ElcContentSettings contentSettings)
		{
			this.propertyBag = retentionPolicyTag.propertyBag;
			this.m_Session = retentionPolicyTag.m_Session;
			this.contentSettings = contentSettings;
		}

		public string MessageClassDisplayName
		{
			get
			{
				if (this.contentSettings != null)
				{
					return (string)this.contentSettings[ElcContentSettingsSchema.MessageClassDisplayName];
				}
				return null;
			}
		}

		public string MessageClass
		{
			get
			{
				if (this.contentSettings != null)
				{
					return (string)this.contentSettings[ElcContentSettingsSchema.MessageClass];
				}
				return null;
			}
		}

		public string Description
		{
			get
			{
				if (this.contentSettings != null)
				{
					return this.contentSettings.Description;
				}
				return null;
			}
		}

		public bool RetentionEnabled
		{
			get
			{
				return this.contentSettings != null && (bool)this.contentSettings[ElcContentSettingsSchema.RetentionEnabled];
			}
			set
			{
				if (this.contentSettings != null)
				{
					this.contentSettings.RetentionEnabled = value;
				}
			}
		}

		public RetentionActionType RetentionAction
		{
			get
			{
				if (this.contentSettings != null)
				{
					return (RetentionActionType)this.contentSettings[ElcContentSettingsSchema.RetentionAction];
				}
				return RetentionActionType.DeleteAndAllowRecovery;
			}
			set
			{
				if (this.contentSettings != null)
				{
					this.contentSettings.RetentionAction = value;
				}
			}
		}

		public EnhancedTimeSpan? AgeLimitForRetention
		{
			get
			{
				if (this.contentSettings != null)
				{
					return (EnhancedTimeSpan?)this.contentSettings[ElcContentSettingsSchema.AgeLimitForRetention];
				}
				return null;
			}
			set
			{
				if (this.contentSettings != null)
				{
					this.contentSettings.AgeLimitForRetention = value;
				}
			}
		}

		public ADObjectId MoveToDestinationFolder
		{
			get
			{
				if (this.contentSettings != null)
				{
					return (ADObjectId)this.contentSettings[ElcContentSettingsSchema.MoveToDestinationFolder];
				}
				return null;
			}
		}

		public RetentionDateType TriggerForRetention
		{
			get
			{
				if (this.contentSettings != null)
				{
					return (RetentionDateType)this.contentSettings[ElcContentSettingsSchema.TriggerForRetention];
				}
				return RetentionDateType.WhenDelivered;
			}
		}

		public JournalingFormat MessageFormatForJournaling
		{
			get
			{
				if (this.contentSettings != null)
				{
					return (JournalingFormat)this.contentSettings[ElcContentSettingsSchema.MessageFormatForJournaling];
				}
				return JournalingFormat.UseMsg;
			}
		}

		public bool JournalingEnabled
		{
			get
			{
				return this.contentSettings != null && (bool)this.contentSettings[ElcContentSettingsSchema.JournalingEnabled];
			}
		}

		public ADObjectId AddressForJournaling
		{
			get
			{
				if (this.contentSettings != null)
				{
					return (ADObjectId)this.contentSettings[ElcContentSettingsSchema.AddressForJournaling];
				}
				return null;
			}
		}

		public string LabelForJournaling
		{
			get
			{
				if (this.contentSettings != null)
				{
					return (string)this.contentSettings[ElcContentSettingsSchema.LabelForJournaling];
				}
				return null;
			}
		}

		private ElcContentSettings contentSettings;
	}
}
