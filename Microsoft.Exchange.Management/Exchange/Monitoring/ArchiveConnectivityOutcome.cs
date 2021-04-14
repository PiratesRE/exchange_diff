using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class ArchiveConnectivityOutcome : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ArchiveConnectivityOutcome.schema;
			}
		}

		private new bool IsValid
		{
			get
			{
				return true;
			}
		}

		public ArchiveConnectivityOutcome(string userSmtp, string primaryFAI, string primaryLastProcessedTime, string archiveDomain, string archiveDatabase, string archiveFAI, string archiveLastProcessedTime, string complianceConfiguration, string mrmProperties) : base(new SimpleProviderPropertyBag())
		{
			this.Identity = userSmtp;
			this.PrimaryMRMConfiguration = primaryFAI;
			this.PrimaryLastProcessedTime = primaryLastProcessedTime;
			this.ArchiveDomain = archiveDomain;
			this.ArchiveDatabase = archiveDatabase;
			this.ArchiveMRMConfiguration = archiveFAI;
			this.ArchiveLastProcessedTime = archiveLastProcessedTime;
			this.ComplianceConfiguration = complianceConfiguration;
			this.ItemMRMProperties = mrmProperties;
			this.Result = new ArchiveConnectivityResult(ArchiveConnectivityResultEnum.Undefined);
		}

		internal void Update(ArchiveConnectivityResultEnum resultEnum, string error)
		{
			lock (this.thisLock)
			{
				this.Result = new ArchiveConnectivityResult(resultEnum);
				this.Error = (error ?? string.Empty);
			}
		}

		public new string Identity
		{
			get
			{
				return (string)this[ArchiveConnectivityResultOutcomeSchema.UserSmtp];
			}
			internal set
			{
				this[ArchiveConnectivityResultOutcomeSchema.UserSmtp] = value;
			}
		}

		public string PrimaryMRMConfiguration
		{
			get
			{
				return (string)this[ArchiveConnectivityResultOutcomeSchema.PrimaryMRMConfiguration];
			}
			internal set
			{
				this[ArchiveConnectivityResultOutcomeSchema.PrimaryMRMConfiguration] = value;
			}
		}

		public string PrimaryLastProcessedTime
		{
			get
			{
				return (string)this[ArchiveConnectivityResultOutcomeSchema.PrimaryLastProcessedTime];
			}
			internal set
			{
				this[ArchiveConnectivityResultOutcomeSchema.PrimaryLastProcessedTime] = value;
			}
		}

		public string ArchiveDomain
		{
			get
			{
				return (string)this[ArchiveConnectivityResultOutcomeSchema.ArchiveDomain];
			}
			internal set
			{
				this[ArchiveConnectivityResultOutcomeSchema.ArchiveDomain] = value;
			}
		}

		public string ArchiveDatabase
		{
			get
			{
				return (string)this[ArchiveConnectivityResultOutcomeSchema.ArchiveDatabase];
			}
			internal set
			{
				this[ArchiveConnectivityResultOutcomeSchema.ArchiveDatabase] = value;
			}
		}

		public string ArchiveMRMConfiguration
		{
			get
			{
				return (string)this[ArchiveConnectivityResultOutcomeSchema.ArchiveMRMConfiguration];
			}
			internal set
			{
				this[ArchiveConnectivityResultOutcomeSchema.ArchiveMRMConfiguration] = value;
			}
		}

		public string ArchiveLastProcessedTime
		{
			get
			{
				return (string)this[ArchiveConnectivityResultOutcomeSchema.ArchiveLastProcessedTime];
			}
			internal set
			{
				this[ArchiveConnectivityResultOutcomeSchema.ArchiveLastProcessedTime] = value;
			}
		}

		public string ComplianceConfiguration
		{
			get
			{
				return (string)this[ArchiveConnectivityResultOutcomeSchema.ComplianceConfiguration];
			}
			internal set
			{
				this[ArchiveConnectivityResultOutcomeSchema.ComplianceConfiguration] = value;
			}
		}

		public string ItemMRMProperties
		{
			get
			{
				return (string)this[ArchiveConnectivityResultOutcomeSchema.ItemMRMProperties];
			}
			internal set
			{
				this[ArchiveConnectivityResultOutcomeSchema.ItemMRMProperties] = value;
			}
		}

		public ArchiveConnectivityResult Result
		{
			get
			{
				return (ArchiveConnectivityResult)this[ArchiveConnectivityResultOutcomeSchema.Result];
			}
			internal set
			{
				this[ArchiveConnectivityResultOutcomeSchema.Result] = value;
			}
		}

		public string Error
		{
			get
			{
				return (string)this[ArchiveConnectivityResultOutcomeSchema.Error];
			}
			internal set
			{
				this[ArchiveConnectivityResultOutcomeSchema.Error] = value;
			}
		}

		[NonSerialized]
		private object thisLock = new object();

		private static ArchiveConnectivityResultOutcomeSchema schema = ObjectSchema.GetInstance<ArchiveConnectivityResultOutcomeSchema>();
	}
}
