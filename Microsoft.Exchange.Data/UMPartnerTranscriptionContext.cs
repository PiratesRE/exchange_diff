using System;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data
{
	internal class UMPartnerTranscriptionContext : UMPartnerContext
	{
		public string CallId
		{
			get
			{
				return (string)base[UMPartnerTranscriptionContext.Schema.CallId];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.CallId] = value;
			}
		}

		public string SessionId
		{
			get
			{
				return (string)base[UMPartnerTranscriptionContext.Schema.SessionId];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.SessionId] = value;
			}
		}

		public ExDateTime CreationTime
		{
			get
			{
				return (ExDateTime)base[UMPartnerTranscriptionContext.Schema.CreationTime];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.CreationTime] = value;
			}
		}

		public SmtpAddress PartnerAddress
		{
			get
			{
				return (SmtpAddress)base[UMPartnerTranscriptionContext.Schema.PartnerAddress];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.PartnerAddress] = value;
			}
		}

		public string PartnerTranscriptionAttachmentName
		{
			get
			{
				return (string)base[UMPartnerTranscriptionContext.Schema.PartnerTranscriptionAttachmentName];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.PartnerTranscriptionAttachmentName] = value;
			}
		}

		public string PartnerAudioAttachmentName
		{
			get
			{
				return (string)base[UMPartnerTranscriptionContext.Schema.PartnerAudioAttachmentName];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.PartnerAudioAttachmentName] = value;
			}
		}

		public string PcmAudioAttachmentName
		{
			get
			{
				return (string)base[UMPartnerTranscriptionContext.Schema.PcmAudioAttachmentName];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.PcmAudioAttachmentName] = value;
			}
		}

		public string IpmAttachmentName
		{
			get
			{
				return (string)base[UMPartnerTranscriptionContext.Schema.IpmAttachmentName];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.IpmAttachmentName] = value;
			}
		}

		public int PartnerMaxDeliveryDelay
		{
			get
			{
				return (int)base[UMPartnerTranscriptionContext.Schema.PartnerMaxDeliveryDelay];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.PartnerMaxDeliveryDelay] = value;
			}
		}

		public int Duration
		{
			get
			{
				return (int)base[UMPartnerTranscriptionContext.Schema.Duration];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.Duration] = value;
			}
		}

		public string AudioCodec
		{
			get
			{
				return (string)base[UMPartnerTranscriptionContext.Schema.AudioCodec];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.AudioCodec] = value;
			}
		}

		public string CallingParty
		{
			get
			{
				return (string)base[UMPartnerTranscriptionContext.Schema.CallingParty];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.CallingParty] = value;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return (CultureInfo)base[UMPartnerTranscriptionContext.Schema.Culture];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.Culture] = value;
			}
		}

		public string Subject
		{
			get
			{
				return (string)base[UMPartnerTranscriptionContext.Schema.Subject];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.Subject] = value;
			}
		}

		public bool IsImportant
		{
			get
			{
				return (bool)base[UMPartnerTranscriptionContext.Schema.IsImportant];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.IsImportant] = value;
			}
		}

		public bool IsCallAnsweringMessage
		{
			get
			{
				return (bool)base[UMPartnerTranscriptionContext.Schema.IsCallAnsweringMessage];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.IsCallAnsweringMessage] = value;
			}
		}

		public Guid CallerGuid
		{
			get
			{
				return (Guid)base[UMPartnerTranscriptionContext.Schema.CallerGuid];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.CallerGuid] = value;
			}
		}

		public string CallerName
		{
			get
			{
				return (string)base[UMPartnerTranscriptionContext.Schema.CallerName];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.CallerName] = value;
			}
		}

		public string CallerIdDisplayName
		{
			get
			{
				return (string)base[UMPartnerTranscriptionContext.Schema.CallerIdDisplayName];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.CallerIdDisplayName] = value;
			}
		}

		public string UMDialPlanLanguage
		{
			get
			{
				return (string)base[UMPartnerTranscriptionContext.Schema.UMDialPlanLanguage];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.UMDialPlanLanguage] = value;
			}
		}

		public string CallerInformedOfAnalysis
		{
			get
			{
				return (string)base[UMPartnerTranscriptionContext.Schema.CallerInformedOfAnalysis];
			}
			set
			{
				base[UMPartnerTranscriptionContext.Schema.CallerInformedOfAnalysis] = value;
			}
		}

		protected override UMPartnerContext.UMPartnerContextSchema ContextSchema
		{
			get
			{
				return UMPartnerTranscriptionContext.Schema;
			}
		}

		private static readonly UMPartnerTranscriptionContext.UMPartnerTranscriptionContextSchema Schema = new UMPartnerTranscriptionContext.UMPartnerTranscriptionContextSchema();

		private class UMPartnerTranscriptionContextSchema : UMPartnerContext.UMPartnerContextSchema
		{
			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition CallId = new UMPartnerContext.UMPartnerContextPropertyDefinition("CallId", typeof(string), string.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition SessionId = new UMPartnerContext.UMPartnerContextPropertyDefinition("SessionId", typeof(string), string.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition CreationTime = new UMPartnerContext.UMPartnerContextPropertyDefinition("CreationTime", typeof(ExDateTime), ExDateTime.MinValue);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition PartnerAddress = new UMPartnerContext.UMPartnerContextPropertyDefinition("PartnerAddress", typeof(SmtpAddress), SmtpAddress.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition PartnerMaxDeliveryDelay = new UMPartnerContext.UMPartnerContextPropertyDefinition("PartnerMaxDeliveryDelay", typeof(int), 0);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition PartnerTranscriptionAttachmentName = new UMPartnerContext.UMPartnerContextPropertyDefinition("PartnerTranscriptionAttachmentName", typeof(string), string.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition PartnerAudioAttachmentName = new UMPartnerContext.UMPartnerContextPropertyDefinition("PartnerAudioAttachmentName", typeof(string), string.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition PcmAudioAttachmentName = new UMPartnerContext.UMPartnerContextPropertyDefinition("PcmAudioAttachmentName", typeof(string), string.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition IpmAttachmentName = new UMPartnerContext.UMPartnerContextPropertyDefinition("IpmAttachmentName", typeof(string), string.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition Duration = new UMPartnerContext.UMPartnerContextPropertyDefinition("Duration", typeof(int), 0);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition AudioCodec = new UMPartnerContext.UMPartnerContextPropertyDefinition("AudioCodec", typeof(string), string.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition CallingParty = new UMPartnerContext.UMPartnerContextPropertyDefinition("CallingParty", typeof(string), string.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition Culture = new UMPartnerContext.UMPartnerContextPropertyDefinition("Culture", typeof(CultureInfo), CultureInfo.InvariantCulture);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition Subject = new UMPartnerContext.UMPartnerContextPropertyDefinition("Subject", typeof(string), string.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition IsImportant = new UMPartnerContext.UMPartnerContextPropertyDefinition("IsImportant", typeof(bool), false);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition IsCallAnsweringMessage = new UMPartnerContext.UMPartnerContextPropertyDefinition("IsCallAnsweringMessage", typeof(bool), false);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition CallerGuid = new UMPartnerContext.UMPartnerContextPropertyDefinition("CallerGuid", typeof(Guid), Guid.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition CallerName = new UMPartnerContext.UMPartnerContextPropertyDefinition("CallerName", typeof(string), string.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition CallerIdDisplayName = new UMPartnerContext.UMPartnerContextPropertyDefinition("CallerIdDisplayName", typeof(string), string.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition UMDialPlanLanguage = new UMPartnerContext.UMPartnerContextPropertyDefinition("UMDialPlanLanguage", typeof(string), string.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition CallerInformedOfAnalysis = new UMPartnerContext.UMPartnerContextPropertyDefinition("CallerInformedOfAnalysis", typeof(string), string.Empty);
		}
	}
}
