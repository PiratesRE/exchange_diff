using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(CASMailbox))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetCASMailbox : OptionsPropertyChangeTracker
	{
		[DataMember]
		public bool ActiveSyncDebugLogging
		{
			get
			{
				return this.activeSyncDebugLogging;
			}
			set
			{
				this.activeSyncDebugLogging = value;
				base.TrackPropertyChanged("ActiveSyncDebugLogging");
			}
		}

		[DataMember]
		public bool ImapForceICalForCalendarRetrievalOption
		{
			get
			{
				return this.imapForceICalForCalendarRetrievalOption;
			}
			set
			{
				this.imapForceICalForCalendarRetrievalOption = value;
				base.TrackPropertyChanged("ImapForceICalForCalendarRetrievalOption");
			}
		}

		[DataMember]
		public bool ImapSuppressReadReceipt
		{
			get
			{
				return this.imapSuppressReadReceipt;
			}
			set
			{
				this.imapSuppressReadReceipt = value;
				base.TrackPropertyChanged("ImapSuppressReadReceipt");
			}
		}

		[DataMember]
		public bool PopForceICalForCalendarRetrievalOption
		{
			get
			{
				return this.popForceICalForCalendarRetrievalOption;
			}
			set
			{
				this.popForceICalForCalendarRetrievalOption = value;
				base.TrackPropertyChanged("PopForceICalForCalendarRetrievalOption");
			}
		}

		[DataMember]
		public bool PopSuppressReadReceipt
		{
			get
			{
				return this.popSuppressReadReceipt;
			}
			set
			{
				this.popSuppressReadReceipt = value;
				base.TrackPropertyChanged("PopSuppressReadReceipt");
			}
		}

		private bool activeSyncDebugLogging;

		private bool imapForceICalForCalendarRetrievalOption;

		private bool imapSuppressReadReceipt;

		private bool popForceICalForCalendarRetrievalOption;

		private bool popSuppressReadReceipt;
	}
}
