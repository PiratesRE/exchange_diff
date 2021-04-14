using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ViewRetentionPolicyTagRow : OptionalRetentionPolicyTagRow
	{
		public ViewRetentionPolicyTagRow(PresentationRetentionPolicyTag rpt) : base(rpt)
		{
		}

		[DataMember]
		public string AppliesTo
		{
			get
			{
				return base.RetentionPolicyTag.MessageClassDisplayName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RetentionPolicyTagTypeDescription
		{
			get
			{
				string result = string.Empty;
				if (!base.OptionalTag)
				{
					if (base.DefaultTag)
					{
						result = OwaOptionStrings.DefaultRetentionTagDescription;
					}
					else
					{
						result = OwaOptionStrings.RetentionTypeRequiredDescription;
					}
				}
				return result;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DescriptionLabel
		{
			get
			{
				string description = base.Description;
				if (!string.IsNullOrEmpty(description))
				{
					return OwaOptionStrings.ViewRPTDescriptionLabel;
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RetentionPeriodDetail
		{
			get
			{
				if (base.RetentionPolicyTag.AgeLimitForRetention != null && base.RetentionPolicyTag.RetentionEnabled)
				{
					return base.RetentionPeriod;
				}
				return OwaOptionStrings.RPTNone;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
