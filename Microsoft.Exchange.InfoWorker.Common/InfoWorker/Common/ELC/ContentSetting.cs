using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class ContentSetting
	{
		internal ContentSetting()
		{
		}

		internal ContentSetting(ElcContentSettings elcContentSettings)
		{
			this.guid = elcContentSettings.Guid;
			this.name = elcContentSettings.Name;
			this.messageClass = elcContentSettings.MessageClass;
			this.ageLimitForRetention = elcContentSettings.AgeLimitForRetention;
			this.retentionEnabled = elcContentSettings.RetentionEnabled;
			this.retentionAction = elcContentSettings.RetentionAction;
			this.journalingEnabled = elcContentSettings.JournalingEnabled;
			this.triggerForRetention = elcContentSettings.TriggerForRetention;
			if (elcContentSettings.MoveToDestinationFolder != null)
			{
				this.moveToDestinationFolder = new Guid?(elcContentSettings.MoveToDestinationFolder.ObjectGuid);
				this.moveToDestinationFolderName = elcContentSettings.MoveToDestinationFolder.Name;
			}
			this.managedFolderName = elcContentSettings.ManagedFolderName;
			this.addressForJournaling = elcContentSettings.AddressForJournaling;
			this.labelForJournaling = elcContentSettings.LabelForJournaling;
			this.messageFormatForJournaling = elcContentSettings.MessageFormatForJournaling;
		}

		internal Guid Guid
		{
			get
			{
				return this.guid;
			}
			set
			{
				this.guid = value;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal string MessageClass
		{
			get
			{
				return this.messageClass;
			}
			set
			{
				this.messageClass = value;
			}
		}

		internal EnhancedTimeSpan? AgeLimitForRetention
		{
			get
			{
				return this.ageLimitForRetention;
			}
			set
			{
				this.ageLimitForRetention = value;
			}
		}

		internal bool RetentionEnabled
		{
			get
			{
				return this.retentionEnabled;
			}
			set
			{
				this.retentionEnabled = value;
			}
		}

		internal RetentionActionType RetentionAction
		{
			get
			{
				return this.retentionAction;
			}
			set
			{
				this.retentionAction = value;
			}
		}

		internal bool JournalingEnabled
		{
			get
			{
				return this.journalingEnabled;
			}
			set
			{
				this.journalingEnabled = value;
			}
		}

		internal RetentionDateType TriggerForRetention
		{
			get
			{
				return this.triggerForRetention;
			}
			set
			{
				this.triggerForRetention = value;
			}
		}

		internal Guid? MoveToDestinationFolder
		{
			get
			{
				return this.moveToDestinationFolder;
			}
			set
			{
				this.moveToDestinationFolder = value;
			}
		}

		internal string MoveToDestinationFolderName
		{
			get
			{
				return this.moveToDestinationFolderName;
			}
			set
			{
				this.moveToDestinationFolderName = value;
			}
		}

		internal string ManagedFolderName
		{
			get
			{
				return this.managedFolderName;
			}
			set
			{
				this.managedFolderName = value;
			}
		}

		internal ADObjectId AddressForJournaling
		{
			get
			{
				return this.addressForJournaling;
			}
			set
			{
				this.addressForJournaling = value;
			}
		}

		internal string LabelForJournaling
		{
			get
			{
				return this.labelForJournaling;
			}
			set
			{
				this.labelForJournaling = value;
			}
		}

		internal JournalingFormat MessageFormatForJournaling
		{
			get
			{
				return this.messageFormatForJournaling;
			}
			set
			{
				this.messageFormatForJournaling = value;
			}
		}

		private Guid guid;

		private string name;

		private string messageClass;

		private EnhancedTimeSpan? ageLimitForRetention;

		private bool retentionEnabled;

		private RetentionActionType retentionAction;

		private bool journalingEnabled;

		private RetentionDateType triggerForRetention;

		private Guid? moveToDestinationFolder;

		private string moveToDestinationFolderName;

		private string managedFolderName;

		private ADObjectId addressForJournaling;

		private string labelForJournaling;

		private JournalingFormat messageFormatForJournaling;
	}
}
