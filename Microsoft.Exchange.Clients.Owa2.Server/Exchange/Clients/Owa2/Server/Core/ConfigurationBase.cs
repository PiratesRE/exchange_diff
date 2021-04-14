using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class ConfigurationBase
	{
		public AttachmentPolicy AttachmentPolicy
		{
			get
			{
				return this.attachmentPolicy;
			}
			protected set
			{
				this.attachmentPolicy = value;
			}
		}

		public ulong SegmentationFlags
		{
			get
			{
				return this.segmentationFlags;
			}
			protected set
			{
				this.segmentationFlags = value;
			}
		}

		public string DefaultTheme
		{
			get
			{
				return this.defaultTheme;
			}
			protected set
			{
				this.defaultTheme = value;
			}
		}

		public bool UseGB18030
		{
			get
			{
				return this.useGB18030;
			}
			protected set
			{
				this.useGB18030 = value;
			}
		}

		public bool UseISO885915
		{
			get
			{
				return this.useISO885915;
			}
			protected set
			{
				this.useISO885915 = value;
			}
		}

		public OutboundCharsetOptions OutboundCharset
		{
			get
			{
				return this.outboundCharset;
			}
			protected set
			{
				this.outboundCharset = value;
			}
		}

		public InstantMessagingTypeOptions InstantMessagingType
		{
			get
			{
				return this.instantMessagingType;
			}
			protected set
			{
				this.instantMessagingType = value;
			}
		}

		public bool InstantMessagingEnabled { get; protected set; }

		public bool PlacesEnabled { get; protected set; }

		public bool WeatherEnabled { get; protected set; }

		public bool AllowCopyContactsToDeviceAddressBook { get; protected set; }

		public AllowOfflineOnEnum AllowOfflineOn { get; protected set; }

		public bool RecoverDeletedItemsEnabled { get; protected set; }

		public bool GroupCreationEnabled { get; protected set; }

		protected static AttachmentPolicyLevel AttachmentActionToPolicyLevel(AttachmentBlockingActions? action)
		{
			AttachmentBlockingActions valueOrDefault = action.GetValueOrDefault();
			if (action != null)
			{
				switch (valueOrDefault)
				{
				case AttachmentBlockingActions.Allow:
					return AttachmentPolicyLevel.Allow;
				case AttachmentBlockingActions.ForceSave:
					return AttachmentPolicyLevel.ForceSave;
				case AttachmentBlockingActions.Block:
					return AttachmentPolicyLevel.Block;
				}
			}
			return AttachmentPolicyLevel.Block;
		}

		protected static ulong SetSegmentationFlags(int segmentationBits1, int segmentationBits2)
		{
			return (ulong)segmentationBits1 + (ulong)((ulong)((long)segmentationBits2) << 32);
		}

		private AttachmentPolicy attachmentPolicy;

		private string defaultTheme;

		private bool useGB18030;

		private bool useISO885915;

		private OutboundCharsetOptions outboundCharset = OutboundCharsetOptions.AutoDetect;

		private ulong segmentationFlags;

		private InstantMessagingTypeOptions instantMessagingType;
	}
}
