using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetMailboxRegionalConfigurationData : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string DateFormat
		{
			get
			{
				return this.dateFormat;
			}
			set
			{
				this.dateFormat = value;
				base.TrackPropertyChanged("DateFormat");
			}
		}

		[DataMember]
		public string Language
		{
			get
			{
				return this.language;
			}
			set
			{
				this.language = value;
				base.TrackPropertyChanged("Language");
			}
		}

		[DataMember]
		public bool LocalizeDefaultFolderName
		{
			get
			{
				return this.localizeDefaultFolderName;
			}
			set
			{
				this.localizeDefaultFolderName = value;
				base.TrackPropertyChanged("LocalizeDefaultFolderName");
			}
		}

		[DataMember]
		public string TimeFormat
		{
			get
			{
				return this.timeFormat;
			}
			set
			{
				this.timeFormat = value;
				base.TrackPropertyChanged("TimeFormat");
			}
		}

		[DataMember]
		public string TimeZone
		{
			get
			{
				return this.timeZone;
			}
			set
			{
				this.timeZone = value;
				base.TrackPropertyChanged("TimeZone");
			}
		}

		private string dateFormat;

		private string language;

		private bool localizeDefaultFolderName;

		private string timeFormat;

		private string timeZone;
	}
}
