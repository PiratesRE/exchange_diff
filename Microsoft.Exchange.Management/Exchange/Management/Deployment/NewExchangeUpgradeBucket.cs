using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("New", "ExchangeUpgradeBucket", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class NewExchangeUpgradeBucket : NewSystemConfigurationObjectTask<ExchangeUpgradeBucket>
	{
		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxMailboxes
		{
			get
			{
				return (Unlimited<int>)base.Fields[ExchangeUpgradeBucketSchema.MaxMailboxes];
			}
			set
			{
				base.Fields[ExchangeUpgradeBucketSchema.MaxMailboxes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Description
		{
			get
			{
				return (string)base.Fields[ExchangeUpgradeBucketSchema.Description];
			}
			set
			{
				base.Fields[ExchangeUpgradeBucketSchema.Description] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)base.Fields[ExchangeUpgradeBucketSchema.Enabled];
			}
			set
			{
				base.Fields[ExchangeUpgradeBucketSchema.Enabled] = value;
			}
		}

		[ValidateRange(1, 999)]
		[Parameter(Mandatory = false)]
		public int Priority
		{
			get
			{
				return (int)base.Fields[ExchangeUpgradeBucketSchema.Priority];
			}
			set
			{
				base.Fields[ExchangeUpgradeBucketSchema.Priority] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string SourceVersion
		{
			get
			{
				return (string)base.Fields[ExchangeUpgradeBucketSchema.SourceVersion];
			}
			set
			{
				base.Fields[ExchangeUpgradeBucketSchema.SourceVersion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StartDate
		{
			get
			{
				return (DateTime?)base.Fields[ExchangeUpgradeBucketSchema.StartDate];
			}
			set
			{
				base.Fields[ExchangeUpgradeBucketSchema.StartDate] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string TargetVersion
		{
			get
			{
				return (string)base.Fields[ExchangeUpgradeBucketSchema.TargetVersion];
			}
			set
			{
				base.Fields[ExchangeUpgradeBucketSchema.TargetVersion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateCount(0, 4)]
		public OrganizationUpgradeStage[] DisabledUpgradeStages
		{
			get
			{
				return (OrganizationUpgradeStage[])base.Fields[ExchangeUpgradeBucketSchema.DisabledUpgradeStages];
			}
			set
			{
				base.Fields[ExchangeUpgradeBucketSchema.DisabledUpgradeStages] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewExchangeUpgradeBucket(this.DataObject.Name);
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is ArgumentException;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ExchangeUpgradeBucket exchangeUpgradeBucket = (ExchangeUpgradeBucket)base.PrepareDataObject();
			exchangeUpgradeBucket.SetId(this.ConfigurationSession, base.Name);
			if (base.Fields.IsChanged(ExchangeUpgradeBucketSchema.Enabled))
			{
				exchangeUpgradeBucket.Enabled = this.Enabled;
			}
			if (base.Fields.IsChanged(ExchangeUpgradeBucketSchema.MaxMailboxes))
			{
				exchangeUpgradeBucket.MaxMailboxes = this.MaxMailboxes;
			}
			if (base.Fields.IsChanged(ExchangeUpgradeBucketSchema.Description))
			{
				exchangeUpgradeBucket.Description = this.Description;
			}
			if (base.Fields.IsChanged(ExchangeUpgradeBucketSchema.Priority))
			{
				exchangeUpgradeBucket.Priority = this.Priority;
			}
			if (base.Fields.IsChanged(ExchangeUpgradeBucketSchema.SourceVersion))
			{
				exchangeUpgradeBucket.SourceVersion = NewExchangeUpgradeBucket.TranslateKnownVersion(this.SourceVersion);
			}
			if (base.Fields.IsChanged(ExchangeUpgradeBucketSchema.StartDate))
			{
				exchangeUpgradeBucket.StartDate = this.StartDate;
			}
			if (base.Fields.IsChanged(ExchangeUpgradeBucketSchema.TargetVersion))
			{
				exchangeUpgradeBucket.TargetVersion = NewExchangeUpgradeBucket.TranslateKnownVersion(this.TargetVersion);
			}
			if (base.Fields.IsChanged(ExchangeUpgradeBucketSchema.DisabledUpgradeStages))
			{
				exchangeUpgradeBucket.DisabledUpgradeStages = this.DisabledUpgradeStages;
			}
			TaskLogger.LogExit();
			return exchangeUpgradeBucket;
		}

		private static string TranslateKnownVersion(string sourceVersion)
		{
			if ("R5".Equals(sourceVersion, StringComparison.OrdinalIgnoreCase))
			{
				return "14.1.225.*";
			}
			if ("R6".Equals(sourceVersion, StringComparison.OrdinalIgnoreCase))
			{
				return "14.16.*";
			}
			return sourceVersion;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			UpgradeBucketTaskHelper.ValidateSourceAndTargetVersions(this.DataObject.SourceVersion, this.DataObject.TargetVersion, new Task.ErrorLoggerDelegate(base.WriteError));
			TaskLogger.LogExit();
		}
	}
}
