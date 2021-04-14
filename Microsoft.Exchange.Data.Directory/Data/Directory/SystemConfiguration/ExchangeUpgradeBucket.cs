using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ExchangeUpgradeBucket : ADConfigurationObject
	{
		internal override ADObjectId ParentPath
		{
			get
			{
				return ExchangeUpgradeBucket.ContainerPath;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ExchangeUpgradeBucket.MostDerivedClass;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ExchangeUpgradeBucket.SchemaInstance;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal static object TargetVersionGetterDelegate(IPropertyBag bag)
		{
			return ExchangeUpgradeBucket.GetVersionStringFromInteger((long)bag[ExchangeUpgradeBucketSchema.RawTargetVersion]);
		}

		internal static void TargetVersionSetterDelegate(object value, IPropertyBag bag)
		{
			bag[ExchangeUpgradeBucketSchema.RawTargetVersion] = ExchangeUpgradeBucket.GetVersionIntegerFromString((string)value);
		}

		internal static object SourceVersionGetterDelegate(IPropertyBag bag)
		{
			return ExchangeUpgradeBucket.GetVersionStringFromInteger((long)bag[ExchangeUpgradeBucketSchema.RawSourceVersion]);
		}

		internal static void SourceVersionSetterDelegate(object value, IPropertyBag bag)
		{
			bag[ExchangeUpgradeBucketSchema.RawSourceVersion] = ExchangeUpgradeBucket.GetVersionIntegerFromString((string)value);
		}

		private static string GetVersionStringFromInteger(long versionInteger)
		{
			ExchangeBuild exchangeBuild = new ExchangeBuild(versionInteger);
			string text = exchangeBuild.Major.ToString() + ".";
			text += ((exchangeBuild.Minor == byte.MaxValue) ? "*" : (exchangeBuild.Minor.ToString() + "."));
			if (exchangeBuild.Minor != 255)
			{
				text += ((exchangeBuild.Build == ushort.MaxValue) ? "*" : (exchangeBuild.Build.ToString() + "."));
			}
			if (exchangeBuild.Build != 65535)
			{
				text += ((exchangeBuild.BuildRevision == 1023) ? "*" : exchangeBuild.BuildRevision.ToString());
			}
			return text;
		}

		private static long GetVersionIntegerFromString(string versionString)
		{
			string[] array = versionString.Split(new char[]
			{
				'.'
			});
			byte b;
			if (!byte.TryParse(array[0], out b) || b == 255)
			{
				throw new ArgumentException(DirectoryStrings.ExchangeUpgradeBucketInvalidVersionFormat(versionString));
			}
			byte maxValue = byte.MaxValue;
			if (array[1] != "*" && (!byte.TryParse(array[1], out maxValue) || maxValue == 255))
			{
				throw new ArgumentException(DirectoryStrings.ExchangeUpgradeBucketInvalidVersionFormat(versionString));
			}
			ushort maxValue2 = ushort.MaxValue;
			ushort num = 1023;
			if (array.Length > 2 && array[2] != "*" && (!ushort.TryParse(array[2], out maxValue2) || maxValue2 == 65535))
			{
				throw new ArgumentException(DirectoryStrings.ExchangeUpgradeBucketInvalidVersionFormat(versionString));
			}
			if (array.Length > 3 && array[3] != "*" && (!ushort.TryParse(array[3], out num) || num >= 1023))
			{
				throw new ArgumentException(DirectoryStrings.ExchangeUpgradeBucketInvalidVersionFormat(versionString));
			}
			ExchangeBuild exchangeBuild = new ExchangeBuild(b, maxValue, maxValue2, num);
			return exchangeBuild.ToInt64();
		}

		[Parameter(Mandatory = false)]
		public ExchangeUpgradeBucketStatus Status
		{
			get
			{
				return (ExchangeUpgradeBucketStatus)this[ExchangeUpgradeBucketSchema.Status];
			}
			set
			{
				this[ExchangeUpgradeBucketSchema.Status] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this[ExchangeUpgradeBucketSchema.Enabled];
			}
			set
			{
				this[ExchangeUpgradeBucketSchema.Enabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StartDate
		{
			get
			{
				return (DateTime?)this[ExchangeUpgradeBucketSchema.StartDate];
			}
			set
			{
				this[ExchangeUpgradeBucketSchema.StartDate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxMailboxes
		{
			get
			{
				if (this[ExchangeUpgradeBucketSchema.MaxMailboxes] != null)
				{
					return (int)this[ExchangeUpgradeBucketSchema.MaxMailboxes];
				}
				return Unlimited<int>.UnlimitedValue;
			}
			set
			{
				this[ExchangeUpgradeBucketSchema.MaxMailboxes] = (value.IsUnlimited ? null : value.Value);
			}
		}

		[ValidateRange(1, 999)]
		[Parameter(Mandatory = false)]
		public int Priority
		{
			get
			{
				return (int)this[ExchangeUpgradeBucketSchema.Priority];
			}
			set
			{
				this[ExchangeUpgradeBucketSchema.Priority] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Description
		{
			get
			{
				return (string)this[ExchangeUpgradeBucketSchema.Description];
			}
			set
			{
				this[ExchangeUpgradeBucketSchema.Description] = value;
			}
		}

		public string SourceVersion
		{
			get
			{
				return (string)this[ExchangeUpgradeBucketSchema.SourceVersion];
			}
			set
			{
				this[ExchangeUpgradeBucketSchema.SourceVersion] = value;
			}
		}

		public string TargetVersion
		{
			get
			{
				return (string)this[ExchangeUpgradeBucketSchema.TargetVersion];
			}
			set
			{
				this[ExchangeUpgradeBucketSchema.TargetVersion] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Organizations
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ExchangeUpgradeBucketSchema.Organizations];
			}
		}

		public int OrganizationCount
		{
			get
			{
				return this.Organizations.Count;
			}
		}

		public int MailboxCount
		{
			get
			{
				return (int)this[ExchangeUpgradeBucketSchema.MailboxCount];
			}
			internal set
			{
				this[ExchangeUpgradeBucketSchema.MailboxCount] = value;
			}
		}

		[ValidateCount(0, 4)]
		[Parameter(Mandatory = false)]
		public OrganizationUpgradeStage[] DisabledUpgradeStages
		{
			get
			{
				int num = (int)this[ExchangeUpgradeBucketSchema.DisabledUpgradeStages];
				List<OrganizationUpgradeStage> list = new List<OrganizationUpgradeStage>();
				for (int i = 0; i <= 3; i++)
				{
					int num2 = 1 << i;
					if ((num & num2) > 0)
					{
						list.Add((OrganizationUpgradeStage)i);
					}
				}
				return list.ToArray();
			}
			set
			{
				int num = 0;
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						OrganizationUpgradeStage organizationUpgradeStage = value[i];
						num |= 1 << (int)organizationUpgradeStage;
					}
				}
				this[ExchangeUpgradeBucketSchema.DisabledUpgradeStages] = num;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationUpgradeStageStatus StartUpgradeStatus
		{
			get
			{
				return (OrganizationUpgradeStageStatus)this[ExchangeUpgradeBucketSchema.StartUpgradeStatus];
			}
			set
			{
				this[ExchangeUpgradeBucketSchema.StartUpgradeStatus] = (int)value;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationUpgradeStageStatus UpgradeOrganizationMailboxesStatus
		{
			get
			{
				return (OrganizationUpgradeStageStatus)this[ExchangeUpgradeBucketSchema.UpgradeOrganizationMailboxesStatus];
			}
			set
			{
				this[ExchangeUpgradeBucketSchema.UpgradeOrganizationMailboxesStatus] = (int)value;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationUpgradeStageStatus UpgradeUserMailboxesStatus
		{
			get
			{
				return (OrganizationUpgradeStageStatus)this[ExchangeUpgradeBucketSchema.UpgradeUserMailboxesStatus];
			}
			set
			{
				this[ExchangeUpgradeBucketSchema.UpgradeUserMailboxesStatus] = (int)value;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationUpgradeStageStatus CompleteUpgradeStatus
		{
			get
			{
				return (OrganizationUpgradeStageStatus)this[ExchangeUpgradeBucketSchema.CompleteUpgradeStatus];
			}
			set
			{
				this[ExchangeUpgradeBucketSchema.CompleteUpgradeStatus] = (int)value;
			}
		}

		private static readonly ADObjectId ContainerPath = new ADObjectId("CN=UpgradeOrchestration,CN=Global Settings");

		private static readonly string MostDerivedClass = "msExchOrganizationUpgradePolicy";

		private static readonly ExchangeUpgradeBucketSchema SchemaInstance = ObjectSchema.GetInstance<ExchangeUpgradeBucketSchema>();
	}
}
