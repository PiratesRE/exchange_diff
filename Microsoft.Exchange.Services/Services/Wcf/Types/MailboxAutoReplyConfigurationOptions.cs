using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MailboxAutoReplyConfigurationOptions : OptionsPropertyChangeTracker
	{
		[DataMember]
		public OofState AutoReplyState
		{
			get
			{
				return this.autoReplyState;
			}
			set
			{
				this.autoReplyState = value;
				base.TrackPropertyChanged("AutoReplyState");
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false)]
		public string EndTime
		{
			get
			{
				return this.endTime;
			}
			set
			{
				this.endTime = value;
				base.TrackPropertyChanged("EndTime");
			}
		}

		[DataMember]
		public ExternalAudience ExternalAudience
		{
			get
			{
				return this.externalAudience;
			}
			set
			{
				this.externalAudience = value;
				base.TrackPropertyChanged("ExternalAudience");
			}
		}

		[DataMember]
		public string ExternalMessage
		{
			get
			{
				return this.externalMessage;
			}
			set
			{
				if (value != this.externalMessage)
				{
					this.externalMessage = value;
					this.ExternalMessageText = DataConversionUtils.ConvertHtmlToText(this.externalMessage);
				}
				base.TrackPropertyChanged("ExternalMessage");
			}
		}

		[DataMember]
		public string InternalMessage
		{
			get
			{
				return this.internalMessage;
			}
			set
			{
				if (value != this.internalMessage)
				{
					this.internalMessage = value;
					this.InternalMessageText = DataConversionUtils.ConvertHtmlToText(this.internalMessage);
				}
				base.TrackPropertyChanged("InternalMessage");
			}
		}

		[DataMember]
		public string ExternalMessageText
		{
			get
			{
				return this.externalMessageText;
			}
			private set
			{
				this.externalMessageText = value;
			}
		}

		[DataMember]
		public string InternalMessageText
		{
			get
			{
				return this.internalMessageText;
			}
			private set
			{
				this.internalMessageText = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string StartTime
		{
			get
			{
				return this.startTime;
			}
			set
			{
				this.startTime = value;
				base.TrackPropertyChanged("StartTime");
			}
		}

		private OofState autoReplyState;

		private string endTime;

		private ExternalAudience externalAudience;

		private string externalMessage;

		private string internalMessage;

		private string externalMessageText;

		private string internalMessageText;

		private string startTime;
	}
}
