using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RetentionPolicyTagBaseRow : BaseRow
	{
		public RetentionPolicyTagBaseRow(PresentationRetentionPolicyTag rpt) : base(rpt)
		{
			this.RetentionPolicyTag = rpt;
		}

		public PresentationRetentionPolicyTag RetentionPolicyTag { get; set; }

		[DataMember]
		public string DisplayName
		{
			get
			{
				return this.RetentionPolicyTag.GetLocalizedFolderName(new CultureInfo[]
				{
					Thread.CurrentThread.CurrentUICulture
				}.AsEnumerable<CultureInfo>());
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
				if (this.RetentionPolicyTag.AgeLimitForRetention == null || !this.RetentionPolicyTag.RetentionEnabled)
				{
					return int.MaxValue;
				}
				return this.RetentionPolicyTag.AgeLimitForRetention.Value.Days;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RetentionPeriod
		{
			get
			{
				if (this.RetentionPolicyTag.AgeLimitForRetention != null && this.RetentionPolicyTag.RetentionEnabled)
				{
					return this.RetentionDuration(this.RetentionPolicyTag.AgeLimitForRetention.Value.Days);
				}
				return OwaOptionStrings.Unlimited;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RetentionAction
		{
			get
			{
				return this.GetLocalizedRetentionAction(this.RetentionPolicyTag.RetentionEnabled, this.RetentionPolicyTag.RetentionAction);
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
				return this.GetLocalizedRetentionActionType();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool OptionalTag { get; set; }

		public bool DefaultTag
		{
			get
			{
				return this.RetentionPolicyTag.Type == ElcFolderType.All;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public static string GetSortProperty(string displayProperty)
		{
			string result = displayProperty;
			if (displayProperty == "RetentionPeriod")
			{
				result = "RetentionDays";
			}
			return result;
		}

		public override bool Equals(object obj)
		{
			return obj != null && (object.ReferenceEquals(obj, this) || (base.GetType() == obj.GetType() && this.RetentionPolicyTag.Guid == (obj as RetentionPolicyTagBaseRow).RetentionPolicyTag.Guid));
		}

		public override int GetHashCode()
		{
			return this.RetentionPolicyTag.Guid.GetHashCode();
		}

		protected virtual string GetLocalizedRetentionActionType()
		{
			return (this.RetentionPolicyTag.RetentionAction == RetentionActionType.MoveToArchive) ? OwaOptionStrings.RetentionActionTypeArchive : OwaOptionStrings.RetentionActionTypeDelete;
		}

		private string RetentionDuration(int days)
		{
			if (days == 0)
			{
				return OwaOptionStrings.RPTExpireNever;
			}
			if (days <= 90)
			{
				return string.Format((days > 1) ? OwaOptionStrings.RPTDays : OwaOptionStrings.RPTDay, days);
			}
			int num = days / 365;
			int num2 = days % 365 / 30;
			string text = string.Format((num2 > 1) ? OwaOptionStrings.RPTMonths : OwaOptionStrings.RPTMonth, num2);
			string text2 = string.Format((num > 1) ? OwaOptionStrings.RPTYears : OwaOptionStrings.RPTYear, num);
			if (num != 0 && num2 != 0)
			{
				return string.Format(OwaOptionStrings.RPTYearsMonths, text2, text);
			}
			if (num != 0)
			{
				return text2;
			}
			if (num2 != 0)
			{
				return text;
			}
			return string.Empty;
		}

		private string GetLocalizedRetentionAction(bool retentionEnabled, RetentionActionType retentionActionType)
		{
			string result = LocalizedDescriptionAttribute.FromEnum(typeof(RetentionActionType), retentionActionType);
			bool flag = retentionActionType == RetentionActionType.MoveToArchive;
			if (retentionActionType == RetentionActionType.DeleteAndAllowRecovery)
			{
				result = OwaOptionStrings.RetentionActionDeleteAndAllowRecovery;
			}
			if (!retentionEnabled)
			{
				if (flag)
				{
					result = OwaOptionStrings.RetentionActionNeverMove;
				}
				else
				{
					result = OwaOptionStrings.RetentionActionNeverDelete;
				}
			}
			return result;
		}
	}
}
