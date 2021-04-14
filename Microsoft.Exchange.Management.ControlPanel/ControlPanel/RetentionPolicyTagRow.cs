using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RetentionPolicyTagRow : RetentionPolicyTagBaseRow
	{
		public RetentionPolicyTagRow(PresentationRetentionPolicyTag rpt) : base(rpt)
		{
		}

		[DataMember]
		public string RetentionPolicyTagType
		{
			get
			{
				return base.OptionalTag ? OwaOptionStrings.RetentionTypeOptional : OwaOptionStrings.RetentionTypeRequired;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		protected override string GetLocalizedRetentionActionType()
		{
			if (base.DefaultTag)
			{
				return (base.RetentionPolicyTag.RetentionAction == RetentionActionType.MoveToArchive) ? OwaOptionStrings.RetentionActionTypeDefaultArchive : OwaOptionStrings.RetentionActionTypeDefaultDelete;
			}
			return base.GetLocalizedRetentionActionType();
		}
	}
}
