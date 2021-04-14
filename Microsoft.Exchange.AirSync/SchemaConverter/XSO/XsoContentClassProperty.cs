using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoContentClassProperty : XsoStringProperty
	{
		public XsoContentClassProperty(PropertyType type) : base(null, type, new PropertyDefinition[]
		{
			StoreObjectSchema.ContentClass
		})
		{
		}

		public override string StringData
		{
			get
			{
				if (this.IsItemDelegated())
				{
					return "urn:content-classes:message";
				}
				string text = base.XsoItem.GetValueOrDefault<string>(StoreObjectSchema.ContentClass);
				if (BodyConversionUtilities.IsMessageRestrictedAndDecoded((Item)base.XsoItem) || BodyConversionUtilities.IsIRMFailedToDecode((Item)base.XsoItem))
				{
					text = null;
				}
				if (text != null)
				{
					return text;
				}
				string className = base.XsoItem.ClassName;
				if (XsoContentClassProperty.ContentClassMapping.TryGetValue(className, out text))
				{
					return text;
				}
				if (className.StartsWith("ipm.appointment.", StringComparison.OrdinalIgnoreCase))
				{
					return "urn:content-classes:appointment";
				}
				if (className.StartsWith("ipm.schedule.meeting.", StringComparison.OrdinalIgnoreCase))
				{
					return "urn:content-classes:calendarmessage";
				}
				if (className.StartsWith("ipm.contact", StringComparison.OrdinalIgnoreCase))
				{
					return "urn:content-classes:person";
				}
				if (className.StartsWith("ipm.note.rpmsg.microsoft.voicemail", StringComparison.OrdinalIgnoreCase) && BodyConversionUtilities.IsMessageRestrictedAndDecoded((Item)base.XsoItem))
				{
					return "voice";
				}
				if (className.StartsWith("ipm.note.", StringComparison.OrdinalIgnoreCase))
				{
					return "urn:content-classes:message";
				}
				if (className.StartsWith("ipm.document.", StringComparison.OrdinalIgnoreCase))
				{
					return "urn:content-classes:document";
				}
				if (className.StartsWith("ipm.", StringComparison.OrdinalIgnoreCase))
				{
					return "urn:content-classes:message";
				}
				if (className.StartsWith("report.dr", StringComparison.OrdinalIgnoreCase))
				{
					return "urn:content-classes:dsn";
				}
				if (className.StartsWith("report.ndr", StringComparison.OrdinalIgnoreCase))
				{
					return "urn:content-classes:dsn";
				}
				if (className.StartsWith("report.ipnndr", StringComparison.OrdinalIgnoreCase))
				{
					return "urn:content-classes:mdn";
				}
				if (className.StartsWith("report.ipnnrn", StringComparison.OrdinalIgnoreCase))
				{
					return "urn:content-classes:mdn";
				}
				if (className.StartsWith("report.ipnrn", StringComparison.OrdinalIgnoreCase))
				{
					return "urn:content-classes:mdn";
				}
				if (className.StartsWith("report.", StringComparison.OrdinalIgnoreCase))
				{
					return "urn:content-classes:message";
				}
				return "urn:content-classes:document";
			}
		}

		private static Dictionary<string, string> ContentClassMapping
		{
			get
			{
				if (XsoContentClassProperty.contentClassMapping == null)
				{
					lock (typeof(XsoContentClassProperty))
					{
						if (XsoContentClassProperty.contentClassMapping == null)
						{
							XsoContentClassProperty.contentClassMapping = new Dictionary<string, string>(82, StringComparer.OrdinalIgnoreCase);
							XsoContentClassProperty.contentClassMapping.Add("ipc", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipm", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipf", "urn:content-classes:folder");
							XsoContentClassProperty.contentClassMapping.Add("ipf.appointment", "urn:content-classes:calendarfolder");
							XsoContentClassProperty.contentClassMapping.Add("ipf.contact", "urn:content-classes:contactfolder");
							XsoContentClassProperty.contentClassMapping.Add("ipf.journal", "urn:content-classes:journalfolder");
							XsoContentClassProperty.contentClassMapping.Add("ipf.note", "urn:content-classes:mailfolder");
							XsoContentClassProperty.contentClassMapping.Add("ipf.stickynote", "urn:content-classes:notefolder");
							XsoContentClassProperty.contentClassMapping.Add("ipf.task", "urn:content-classes:taskfolder");
							XsoContentClassProperty.contentClassMapping.Add("ipm.activity", "urn:content-classes:activity");
							XsoContentClassProperty.contentClassMapping.Add("ipm.appointment", "urn:content-classes:appointment");
							XsoContentClassProperty.contentClassMapping.Add("ipm.conflict.resolution.message", "http://content-classes.microsoft.com/exchange/conflict");
							XsoContentClassProperty.contentClassMapping.Add("ipm.contact", "urn:content-classes:person");
							XsoContentClassProperty.contentClassMapping.Add("ipm.contentclassdef", "urn:content-classes:contentclassdef ");
							XsoContentClassProperty.contentClassMapping.Add("ipm.distlist", "urn:content-classes:group");
							XsoContentClassProperty.contentClassMapping.Add("ipm.document", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipm.document.*doc", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipm.document.excel.sheet.5", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipm.document.excel.sheet.8", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipm.document.microsoft internet mail message", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.document.msproject.project.4_1", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipm.document.msproject.project.8", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipm.document.powerpoint.show.4", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipm.document.powerpoint.show.7", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipm.document.powerpoint.show.8", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipm.document.textfile", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipm.document.word.document.6", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipm.document.word.document.8", "urn:content-classes:document");
							XsoContentClassProperty.contentClassMapping.Add("ipm.microsoft.keyexchange", "http://content-classes.microsoft.com/exchange/keyexchange");
							XsoContentClassProperty.contentClassMapping.Add("ipm.microsoft.scheduledata.freebusy", "urn:content-classes:freebusy");
							XsoContentClassProperty.contentClassMapping.Add("ipm.note", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.note.exchange.security.enrollment", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.note.imc.notification", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.note.p772", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.note.rfc822.mime", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.note.rules.ooftemplate.microsoft", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.note.rules.replytemplate.microsoft", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.note.secure", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.note.secure.service.reply", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.note.secure.sign", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.note.smime", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.note.smime.multipartsigned", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.note.storagequotawarning", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.object", "urn:content-classes:object ");
							XsoContentClassProperty.contentClassMapping.Add("ipm.organization", "urn:content-classes:organization");
							XsoContentClassProperty.contentClassMapping.Add("ipm.outlook.recal", "urn:content-classes:recallmessage");
							XsoContentClassProperty.contentClassMapping.Add("ipm.post", "urn:content-classes:message");
							XsoContentClassProperty.contentClassMapping.Add("ipm.propertydef", "urn:content-classes:propertydef ");
							XsoContentClassProperty.contentClassMapping.Add("ipm.recall.report", "urn:content-classes:recallreport");
							XsoContentClassProperty.contentClassMapping.Add("ipm.recall.report.failure", "urn:content-classes:recallreport");
							XsoContentClassProperty.contentClassMapping.Add("ipm.recall.report.success", "urn:content-classes:recallreport");
							XsoContentClassProperty.contentClassMapping.Add("ipm.report", "urn:content-classes:reportmessage");
							XsoContentClassProperty.contentClassMapping.Add("ipm.schedule.meeting.canceled", "urn:content-classes:calendarmessage");
							XsoContentClassProperty.contentClassMapping.Add("ipm.schedule.meeting.request", "urn:content-classes:calendarmessage");
							XsoContentClassProperty.contentClassMapping.Add("ipm.schedule.meeting.resp.neg", "urn:content-classes:calendarmessage");
							XsoContentClassProperty.contentClassMapping.Add("ipm.schedule.meeting.resp.pos", "urn:content-classes:calendarmessage");
							XsoContentClassProperty.contentClassMapping.Add("ipm.schedule.meeting.resp.tent", "urn:content-classes:calendarmessage");
							XsoContentClassProperty.contentClassMapping.Add("ipm.stickynote", "urn:content-classes:note");
							XsoContentClassProperty.contentClassMapping.Add("ipm.task", "urn:content-classes:task");
							XsoContentClassProperty.contentClassMapping.Add("ipm.taskrequest", "urn:content-classes:calendarmessage");
							XsoContentClassProperty.contentClassMapping.Add("ipm.taskrequest.accept", "urn:content-classes:calendarmessage");
							XsoContentClassProperty.contentClassMapping.Add("ipm.taskrequest.decline", "urn:content-classes:calendarmessage");
							XsoContentClassProperty.contentClassMapping.Add("ipm.taskrequest.update", "urn:content-classes:calendarmessage");
							XsoContentClassProperty.contentClassMapping.Add("report", "urn:content-classes:reportmessage");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.note.dr", "urn:content-classes:dsn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.note.ipnndr", "urn:content-classes:mdn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.note.ipnnrn", "urn:content-classes:mdn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.note.ipnrn", "urn:content-classes:mdn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.note.ndr", "urn:content-classes:dsn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.recall.report.failure.ndr", "urn:content-classes:dsn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.schedule.meeting.canceled.ndr", "urn:content-classes:dsn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.schedule.meeting.request.dr", "urn:content-classes:dsn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.schedule.meeting.request.ipnnrn", "urn:content-classes:mdn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.schedule.meeting.request.ipnrn", "urn:content-classes:mdn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.schedule.meeting.request.ndr", "urn:content-classes:dsn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.schedule.meeting.resp.neg.ndr", "urn:content-classes:dsn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.schedule.meeting.resp.pos.ndr", "urn:content-classes:dsn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.schedule.meeting.resp.tent.ndr", "urn:content-classes:dsn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.taskrequest.accept.ndr", "urn:content-classes:dsn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.taskrequest.decline.ndr", "urn:content-classes:dsn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.taskrequest.ndr", "urn:content-classes:dsn");
							XsoContentClassProperty.contentClassMapping.Add("report.ipm.taskrequest.update.ndr", "urn:content-classes:dsn");
						}
					}
				}
				return XsoContentClassProperty.contentClassMapping;
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			throw new ConversionException("Content-class is a read-only property and should not be set!");
		}

		private static Dictionary<string, string> contentClassMapping;
	}
}
