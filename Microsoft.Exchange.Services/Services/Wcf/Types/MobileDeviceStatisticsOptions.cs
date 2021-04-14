using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MobileDeviceStatisticsOptions : OptionsPropertyChangeTracker
	{
		[DataMember]
		public bool ActiveSync
		{
			get
			{
				return this.activeSyncEnabled;
			}
			set
			{
				this.activeSyncEnabled = value;
				base.TrackPropertyChanged("ActiveSync");
			}
		}

		[DataMember]
		public bool GetMailboxLog
		{
			get
			{
				return this.getMailboxLog;
			}
			set
			{
				this.getMailboxLog = value;
				base.TrackPropertyChanged("GetMailboxLog");
			}
		}

		[DataMember]
		public bool ShowRecoveryPassword
		{
			get
			{
				return this.showRecoveryPwd;
			}
			set
			{
				this.showRecoveryPwd = value;
				base.TrackPropertyChanged("ShowRecoveryPassword");
			}
		}

		private bool activeSyncEnabled;

		private bool getMailboxLog;

		private bool showRecoveryPwd;
	}
}
