using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class OwaUserConfiguration
	{
		[DataMember]
		public UserOptionsType UserOptions
		{
			get
			{
				return this.userOptions;
			}
			set
			{
				this.userOptions = value;
			}
		}

		[DataMember]
		public SessionSettingsType SessionSettings
		{
			get
			{
				return this.sessionSettings;
			}
			set
			{
				this.sessionSettings = value;
			}
		}

		[DataMember]
		public string InlineExploreContent
		{
			get
			{
				return this.inlineExploreContent;
			}
			set
			{
				this.inlineExploreContent = value;
			}
		}

		[DataMember]
		public SegmentationSettingsType SegmentationSettings
		{
			get
			{
				return this.segmentationSettings;
			}
			set
			{
				this.segmentationSettings = value;
			}
		}

		[DataMember]
		public AttachmentPolicyType AttachmentPolicy
		{
			get
			{
				return this.attachmentPolicy;
			}
			set
			{
				this.attachmentPolicy = value;
			}
		}

		[DataMember]
		public PolicySettingsType PolicySettings
		{
			get
			{
				return this.policySettings;
			}
			set
			{
				this.policySettings = value;
			}
		}

		[DataMember]
		public MobileDevicePolicySettingsType MobileDevicePolicySettings
		{
			get
			{
				return this.mobileDevicePolicySettings;
			}
			set
			{
				this.mobileDevicePolicySettings = value;
			}
		}

		[DataMember]
		public ApplicationSettingsType ApplicationSettings
		{
			get
			{
				return this.applicationSettings;
			}
			set
			{
				this.applicationSettings = value;
			}
		}

		[DataMember]
		public OwaViewStateConfiguration ViewStateConfiguration
		{
			get
			{
				return this.viewStateConfiguration;
			}
			set
			{
				this.viewStateConfiguration = value;
			}
		}

		[DataMember]
		public MasterCategoryListType MasterCategoryList
		{
			get
			{
				return this.masterCategoryList;
			}
			set
			{
				this.masterCategoryList = value;
			}
		}

		[DataMember]
		public SmimeAdminSettingsType SmimeAdminSettings
		{
			get
			{
				return this.smimeAdminSettings;
			}
			set
			{
				this.smimeAdminSettings = value;
			}
		}

		[DataMember]
		public uint MailTipsLargeAudienceThreshold { get; set; }

		[DataMember]
		public RetentionPolicyTag[] RetentionPolicyTags { get; set; }

		[DataMember]
		public int MaxRecipientsPerMessage { get; set; }

		[DataMember]
		public bool RecoverDeletedItemsEnabled { get; set; }

		[DataMember]
		public string[] FlightConfiguration { get; set; }

		[DataMember]
		public bool PublicComputersDetectionEnabled { get; set; }

		[DataMember]
		public bool PolicyTipsEnabled { get; set; }

		private UserOptionsType userOptions;

		private SessionSettingsType sessionSettings;

		private SegmentationSettingsType segmentationSettings;

		private AttachmentPolicyType attachmentPolicy;

		private PolicySettingsType policySettings;

		private MobileDevicePolicySettingsType mobileDevicePolicySettings;

		private ApplicationSettingsType applicationSettings;

		private OwaViewStateConfiguration viewStateConfiguration;

		private MasterCategoryListType masterCategoryList;

		private string inlineExploreContent;

		private SmimeAdminSettingsType smimeAdminSettings;
	}
}
