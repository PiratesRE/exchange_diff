using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RetentionPolicyTagResolverRow : AdObjectResolverRow
	{
		internal ElcContentSettings ContentSettings
		{
			get
			{
				return this.contentSettings;
			}
			set
			{
				this.contentSettings = value;
			}
		}

		public RetentionPolicyTagResolverRow(ADRawEntry aDRawEntry) : base(aDRawEntry)
		{
		}

		[DataMember]
		public string Name
		{
			get
			{
				return (string)base.ADRawEntry[ADObjectSchema.Name];
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Type
		{
			get
			{
				return RetentionUtils.GetLocalizedType((ElcFolderType)base.ADRawEntry[RetentionPolicyTagSchema.Type]);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RetentionPeriodDays
		{
			get
			{
				bool flag = this.contentSettings != null && this.contentSettings.RetentionEnabled;
				EnhancedTimeSpan? enhancedTimeSpan = (this.contentSettings != null) ? this.contentSettings.AgeLimitForRetention : null;
				if (enhancedTimeSpan != null && flag)
				{
					int days = enhancedTimeSpan.Value.Days;
					return string.Format((days > 1) ? Strings.RPTDays : Strings.RPTDay, days);
				}
				return Strings.Unlimited;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int RetentionDays
		{
			get
			{
				bool flag = this.contentSettings != null && this.contentSettings.RetentionEnabled;
				EnhancedTimeSpan? enhancedTimeSpan = (this.contentSettings != null) ? this.contentSettings.AgeLimitForRetention : null;
				if (enhancedTimeSpan != null && flag)
				{
					return enhancedTimeSpan.Value.Days;
				}
				return int.MaxValue;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RetentionPolicyActionType
		{
			get
			{
				RetentionActionType retentionActionType = (this.contentSettings != null) ? this.contentSettings.RetentionAction : RetentionActionType.DeleteAndAllowRecovery;
				return RetentionUtils.GetLocalizedRetentionActionType(retentionActionType);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public new static PropertyDefinition[] Properties = new List<PropertyDefinition>(AdObjectResolverRow.Properties)
		{
			ADObjectSchema.Name,
			RetentionPolicyTagSchema.Type
		}.ToArray();

		private ElcContentSettings contentSettings;
	}
}
