using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Kes
{
	internal class ReputationListSettings : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.ReputationListSettingsID.ToString());
			}
		}

		public int ReputationListSettingsID
		{
			get
			{
				return (int)this[ReputationListSettings.ReputationListSettingsIDProperty];
			}
			set
			{
				this[ReputationListSettings.ReputationListSettingsIDProperty] = value;
			}
		}

		public string Filename
		{
			get
			{
				return (string)this[ReputationListSettings.FilenameProperty];
			}
			set
			{
				this[ReputationListSettings.FilenameProperty] = value;
			}
		}

		public string CommandOptions
		{
			get
			{
				return (string)this[ReputationListSettings.CommandOptionsProperty];
			}
			set
			{
				this[ReputationListSettings.CommandOptionsProperty] = value;
			}
		}

		public string ShareLocation
		{
			get
			{
				return (string)this[ReputationListSettings.ShareLocationProperty];
			}
			set
			{
				this[ReputationListSettings.ShareLocationProperty] = value;
			}
		}

		public bool? IsEnabled
		{
			get
			{
				return (bool?)this[ReputationListSettings.IsEnabledProperty];
			}
			set
			{
				this[ReputationListSettings.IsEnabledProperty] = value;
			}
		}

		public byte? ReputationListTypeID
		{
			get
			{
				return (byte?)this[ReputationListSettings.ReputationListTypeIDProperty];
			}
			set
			{
				this[ReputationListSettings.ReputationListTypeIDProperty] = value;
			}
		}

		public byte? ReputationListID
		{
			get
			{
				return (byte?)this[ReputationListSettings.ReputationListIDProperty];
			}
			set
			{
				this[ReputationListSettings.ReputationListIDProperty] = value;
			}
		}

		public int? Score
		{
			get
			{
				return (int?)this[ReputationListSettings.ScoreProperty];
			}
			set
			{
				this[ReputationListSettings.ScoreProperty] = value;
			}
		}

		public DateTime? LastFileModifiedDatetime
		{
			get
			{
				return (DateTime?)this[ReputationListSettings.LastFileModifiedDatetimeProperty];
			}
			set
			{
				this[ReputationListSettings.LastFileModifiedDatetimeProperty] = value;
			}
		}

		public DateTime? LastDownloadedDatetime
		{
			get
			{
				return (DateTime?)this[ReputationListSettings.LastDownloadedDatetimeProperty];
			}
			set
			{
				this[ReputationListSettings.LastDownloadedDatetimeProperty] = value;
			}
		}

		public DateTime? LastGeneratedDatetime
		{
			get
			{
				return (DateTime?)this[ReputationListSettings.LastGeneratedDatetimeProperty];
			}
			set
			{
				this[ReputationListSettings.LastGeneratedDatetimeProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition ReputationListSettingsIDProperty = new HygienePropertyDefinition("i_ReputationListSettingsId", typeof(int?));

		public static readonly HygienePropertyDefinition FilenameProperty = new HygienePropertyDefinition("nvc_Filename", typeof(string));

		public static readonly HygienePropertyDefinition CommandOptionsProperty = new HygienePropertyDefinition("nvc_CommandOptions", typeof(string));

		public static readonly HygienePropertyDefinition ShareLocationProperty = new HygienePropertyDefinition("nvc_ShareLocation", typeof(string));

		public static readonly HygienePropertyDefinition IsEnabledProperty = new HygienePropertyDefinition("f_IsEnabled", typeof(bool?));

		public static readonly HygienePropertyDefinition ReputationListTypeIDProperty = new HygienePropertyDefinition("ti_ReputationListTypeId", typeof(byte?));

		public static readonly HygienePropertyDefinition ReputationListIDProperty = new HygienePropertyDefinition("ti_ReputationListId", typeof(byte?));

		public static readonly HygienePropertyDefinition ScoreProperty = new HygienePropertyDefinition("i_Score", typeof(int?));

		public static readonly HygienePropertyDefinition LastFileModifiedDatetimeProperty = new HygienePropertyDefinition("dt_LastFileModifiedDatetime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition LastDownloadedDatetimeProperty = new HygienePropertyDefinition("dt_LastDownloadedDatetime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition LastGeneratedDatetimeProperty = new HygienePropertyDefinition("dt_LastGeneratedDatetime", typeof(DateTime?));
	}
}
