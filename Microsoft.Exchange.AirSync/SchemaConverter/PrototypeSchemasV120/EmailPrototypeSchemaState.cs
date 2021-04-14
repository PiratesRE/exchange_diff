using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.SchemaConverter.XSO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.PrototypeSchemasV120
{
	internal class EmailPrototypeSchemaState : AirSyncXsoSchemaState
	{
		static EmailPrototypeSchemaState()
		{
			if (GlobalSettings.SupportedIPMTypes.Count > 0)
			{
				List<string> list = new List<string>(EmailPrototypeSchemaState.supportedClassTypes.Length + GlobalSettings.SupportedIPMTypes.Count);
				list.AddRange(EmailPrototypeSchemaState.supportedClassTypes);
				list.AddRange(GlobalSettings.SupportedIPMTypes);
				EmailPrototypeSchemaState.supportedClassFilter = AirSyncXsoSchemaState.BuildMessageClassFilter(list);
				return;
			}
			EmailPrototypeSchemaState.supportedClassFilter = AirSyncXsoSchemaState.BuildMessageClassFilter(EmailPrototypeSchemaState.supportedClassTypes);
		}

		public EmailPrototypeSchemaState(IdMapping idmapping) : base(EmailPrototypeSchemaState.supportedClassFilter)
		{
			base.InitConversionTable(2);
			this.CreatePropertyConversionTable(idmapping);
		}

		internal static QueryFilter SupportedClassQueryFilter
		{
			get
			{
				return EmailPrototypeSchemaState.supportedClassFilter;
			}
		}

		private void CreatePropertyConversionTable(IdMapping idmapping)
		{
			string xmlNodeNamespace = "Email:";
			string xmlNodeNamespace2 = "AirSyncBase:";
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "To", false),
				new XsoToProperty(PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "CC", false),
				new XsoCCProperty(PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "From", false),
				new XsoFromProperty(PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "Subject", false),
				new XsoEmailSubjectProperty(PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "ReplyTo", false),
				new XsoReplyToProperty(PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncUtcDateTimeProperty(xmlNodeNamespace, "DateReceived", AirSyncDateFormat.Punctuate, false),
				new XsoUtcDateTimeProperty(ItemSchema.ReceivedTime, PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "DisplayTo", false),
				new XsoStringProperty(ItemSchema.DisplayTo, PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "ThreadTopic", false),
				new XsoStringProperty(ItemSchema.ConversationTopic, PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncIntegerProperty(xmlNodeNamespace, "Importance", false),
				new XsoIntegerProperty(ItemSchema.Importance, PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncBooleanProperty(xmlNodeNamespace, "Read", false),
				new XsoReadFlagProperty(MessageItemSchema.IsRead)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSync12AttachmentsProperty(xmlNodeNamespace2, "Attachments", false),
				new XsoAttachments12Property(idmapping, PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncContentProperty(xmlNodeNamespace2, "Body", false),
				new XsoEmailBodyContentProperty(PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "MessageClass", false),
				new XsoMessageClassProperty(PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncMeetingRequestProperty(xmlNodeNamespace, "MeetingRequest", false, 120),
				new XsoMeetingRequestProperty(120)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncIntegerProperty(xmlNodeNamespace, "InternetCPID", false),
				new XsoIntegerProperty(BodySchema.InternetCpid, PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncFlagProperty(xmlNodeNamespace, "Flag", false),
				new XsoFlagProperty()
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "ContentClass", false),
				new XsoContentClassProperty(PropertyType.ReadOnly)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncIntegerProperty(xmlNodeNamespace2, "NativeBodyType", false),
				new XsoNativeBodyProperty()
			});
		}

		private static readonly string[] supportedClassTypes = new string[]
		{
			"IPM.NOTE",
			"IPM.INFOPATHFORM",
			"IPM.SCHEDULE.MEETING",
			"IPM.NOTIFICATION.MEETING",
			"IPM.POST",
			"IPM.OCTEL.VOICE",
			"IPM.VOICENOTES",
			"IPM.SHARING",
			"REPORT.IPM.NOTE.NDR",
			"REPORT.IPM.NOTE.DR",
			"REPORT.IPM.NOTE.DELAYED",
			"REPORT.IPM.NOTE.IPNRN",
			"REPORT.IPM.NOTE.IPNNRN",
			"REPORT.IPM.SCHEDULE.MEETING.REQUEST.NDR",
			"REPORT.IPM.SCHEDULE.MEETING.RESP.NEG.NDR",
			"REPORT.IPM.SCHEDULE.MEETING.RESP.POS.NDR",
			"REPORT.IPM.SCHEDULE.MEETING.RESP.TENT.NDR",
			"REPORT.IPM.SCHEDULE.MEETING.CANCELED.NDR",
			"REPORT.IPM.NOTE.SMIME.NDR",
			"REPORT.IPM.NOTE.SMIME.DR",
			"REPORT.IPM.NOTE.SMIME.IPNRN",
			"REPORT.IPM.NOTE.SMIME.IPNNRN",
			"REPORT.IPM.NOTE.SMIME.MULTIPARTSIGNED.NDR",
			"REPORT.IPM.NOTE.SMIME.MULTIPARTSIGNED.DR",
			"REPORT.IPM.NOTE.SMIME.MULTIPARTSIGNED.IPNRN",
			"REPORT.IPM.NOTE.SMIME.MULTIPARTSIGNED.IPNNRN"
		};

		private static readonly QueryFilter supportedClassFilter;
	}
}
