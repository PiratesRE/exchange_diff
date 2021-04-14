using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnifiedMessagingMailboxPolicyHasProtectUnauthenticatedVoiceMailSetToException : LocalizedException
	{
		public UnifiedMessagingMailboxPolicyHasProtectUnauthenticatedVoiceMailSetToException(string policyName, DRMProtectionOptions drmProtectionOption) : base(Strings.UnifiedMessagingMailboxPolicyHasProtectUnauthenticatedVoiceMailSetTo(policyName, drmProtectionOption))
		{
			this.policyName = policyName;
			this.drmProtectionOption = drmProtectionOption;
		}

		public UnifiedMessagingMailboxPolicyHasProtectUnauthenticatedVoiceMailSetToException(string policyName, DRMProtectionOptions drmProtectionOption, Exception innerException) : base(Strings.UnifiedMessagingMailboxPolicyHasProtectUnauthenticatedVoiceMailSetTo(policyName, drmProtectionOption), innerException)
		{
			this.policyName = policyName;
			this.drmProtectionOption = drmProtectionOption;
		}

		protected UnifiedMessagingMailboxPolicyHasProtectUnauthenticatedVoiceMailSetToException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.policyName = (string)info.GetValue("policyName", typeof(string));
			this.drmProtectionOption = (DRMProtectionOptions)info.GetValue("drmProtectionOption", typeof(DRMProtectionOptions));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("policyName", this.policyName);
			info.AddValue("drmProtectionOption", this.drmProtectionOption);
		}

		public string PolicyName
		{
			get
			{
				return this.policyName;
			}
		}

		public DRMProtectionOptions DrmProtectionOption
		{
			get
			{
				return this.drmProtectionOption;
			}
		}

		private readonly string policyName;

		private readonly DRMProtectionOptions drmProtectionOption;
	}
}
