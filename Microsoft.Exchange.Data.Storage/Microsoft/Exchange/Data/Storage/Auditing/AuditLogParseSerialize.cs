using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AuditLogParseSerialize
	{
		public static string GetAsString(IAuditLogRecord auditRecord)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			foreach (KeyValuePair<string, string> keyValuePair in auditRecord.GetDetails())
			{
				stringBuilder.AppendFormat("{0}{1} {2}\r\n", keyValuePair.Key, ":", keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		public static int SerializeMailboxAuditRecord(IAuditLogRecord auditRecord, MessageItem auditMessage)
		{
			int result = 0;
			BodyWriteConfiguration configuration = new BodyWriteConfiguration(BodyFormat.TextPlain);
			using (TextWriter textWriter = auditMessage.Body.OpenTextWriter(configuration))
			{
				string asString = AuditLogParseSerialize.GetAsString(auditRecord);
				textWriter.Write(asString);
				result = Encoding.Unicode.GetByteCount(asString);
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2219191613U);
			auditMessage.ClassName = "IPM.AuditLog";
			return result;
		}

		public static int SerializeAdminAuditRecord(IAuditLogRecord auditRecord, MessageItem auditMessage)
		{
			int result = 0;
			auditMessage.Subject = string.Format("{0} : {1}", auditRecord.UserId, auditRecord.Operation);
			BodyWriteConfiguration configuration = new BodyWriteConfiguration(BodyFormat.TextPlain);
			using (TextWriter textWriter = auditMessage.Body.OpenTextWriter(configuration))
			{
				string asString = AuditLogParseSerialize.GetAsString(auditRecord);
				textWriter.Write(asString);
				result = Encoding.Unicode.GetByteCount(asString);
			}
			auditMessage.From = new Participant(string.Format("{0}{1}", auditRecord.UserId, "audit"), string.Empty, string.Empty);
			auditMessage.Recipients.Add(new Participant(string.Format("{0}{1}", auditRecord.ObjectId, "audit"), string.Empty, string.Empty));
			auditMessage.ClassName = "IPM.AuditLog";
			return result;
		}

		public static MailboxAuditLogEvent ParseMailboxAuditRecord(MailboxAuditLogRecordId identity, string eventLog, string mailboxResolvedName, string guid, DateTime? lastAccessed)
		{
			MailboxAuditLogEvent mailboxAuditLogEvent = new MailboxAuditLogEvent(identity, mailboxResolvedName, guid, lastAccessed);
			using (StringReader stringReader = new StringReader(eventLog))
			{
				PropertyParseSchema[] array = AuditLogParseSerialize.MailboxAuditLogRecordParseSchema;
				string[] array2 = new string[2];
				string text;
				while ((text = stringReader.ReadLine()) != null)
				{
					if (text.Contains(".SourceItemId"))
					{
						MultiValuedProperty<MailboxAuditLogSourceItem> multiValuedProperty = mailboxAuditLogEvent[MailboxAuditLogEventSchema.SourceItems] as MultiValuedProperty<MailboxAuditLogSourceItem>;
						string itemId = text.Substring(text.IndexOf(":") + 1).Trim();
						if ((text = stringReader.ReadLine()) == null)
						{
							MailboxAuditLogSourceItem item = MailboxAuditLogSourceItem.Parse(itemId, null, null);
							multiValuedProperty.Add(item);
							break;
						}
						array2[0] = text;
						text = stringReader.ReadLine();
						array2[1] = text;
						string itemSubject = null;
						string itemFolderPathName = null;
						foreach (string text2 in array2)
						{
							if (text2 == null)
							{
								break;
							}
							if (text2.Contains(".SourceItemSubject"))
							{
								itemSubject = text2.Substring(text2.IndexOf(":") + 1).Trim();
							}
							if (text2.Contains(".SourceItemFolderPathName"))
							{
								itemFolderPathName = text2.Substring(text2.IndexOf(":") + 1).Trim();
							}
						}
						MailboxAuditLogSourceItem item2 = MailboxAuditLogSourceItem.Parse(itemId, itemSubject, itemFolderPathName);
						((MultiValuedProperty<MailboxAuditLogSourceItem>)mailboxAuditLogEvent[MailboxAuditLogEventSchema.SourceItems]).Add(item2);
						if (text == null)
						{
							break;
						}
					}
					else if (text.Contains(".SourceFolderId"))
					{
						MultiValuedProperty<MailboxAuditLogSourceFolder> multiValuedProperty2 = mailboxAuditLogEvent[MailboxAuditLogEventSchema.SourceFolders] as MultiValuedProperty<MailboxAuditLogSourceFolder>;
						string folderId = text.Substring(text.IndexOf(":") + 1).Trim();
						if ((text = stringReader.ReadLine()) == null)
						{
							MailboxAuditLogSourceFolder item3 = MailboxAuditLogSourceFolder.Parse(folderId, null);
							multiValuedProperty2.Add(item3);
							break;
						}
						string folderPathName = text.Substring(text.IndexOf(":") + 1).Trim();
						MailboxAuditLogSourceFolder item4 = MailboxAuditLogSourceFolder.Parse(folderId, folderPathName);
						multiValuedProperty2.Add(item4);
					}
					else
					{
						PropertyParseSchema[] array4 = array;
						int j = 0;
						while (j < array4.Length)
						{
							PropertyParseSchema propertyParseSchema = array4[j];
							if (text.StartsWith(propertyParseSchema.Label, StringComparison.OrdinalIgnoreCase) && text[propertyParseSchema.Label.Length] == ':')
							{
								object obj = propertyParseSchema.PropertyParser(text.Substring(propertyParseSchema.Label.Length + 1).Trim());
								if (obj != null)
								{
									mailboxAuditLogEvent[propertyParseSchema.Property] = obj;
									break;
								}
								break;
							}
							else
							{
								j++;
							}
						}
					}
				}
			}
			return mailboxAuditLogEvent;
		}

		public static void ParseAdminAuditLogRecord(ConfigurableObject auditEvent, ICollection<PropertyParseSchema> schema, string eventLog)
		{
			using (StringReader stringReader = new StringReader(eventLog))
			{
				string text;
				while ((text = stringReader.ReadLine()) != null)
				{
					foreach (PropertyParseSchema propertyParseSchema in schema)
					{
						if (text.StartsWith(propertyParseSchema.Label, StringComparison.OrdinalIgnoreCase) && text[propertyParseSchema.Label.Length] == ':')
						{
							object obj = propertyParseSchema.PropertyParser(text.Substring(propertyParseSchema.Label.Length + 1).Trim());
							if (obj != null)
							{
								auditEvent[propertyParseSchema.Property] = obj;
								break;
							}
							break;
						}
					}
				}
			}
		}

		public static object ParseBoolean(string line)
		{
			bool flag;
			if (bool.TryParse(line, out flag))
			{
				return flag;
			}
			return null;
		}

		private static PropertyParseSchema[] MailboxAuditLogRecordParseSchema
		{
			get
			{
				if (AuditLogParseSerialize.mailboxAuditLogRecordParseSchema == null)
				{
					PropertyParseSchema[] array = new PropertyParseSchema[26];
					array[0] = new PropertyParseSchema("Operation", MailboxAuditLogEventSchema.Operation, null);
					array[1] = new PropertyParseSchema("OperationResult", MailboxAuditLogEventSchema.OperationResult, null);
					array[2] = new PropertyParseSchema("LogonType", MailboxAuditLogEventSchema.LogonType, null);
					array[3] = new PropertyParseSchema("ExternalAccess", MailboxAuditLogEventSchema.ExternalAccess, (string line) => AuditLogParseSerialize.ParseBoolean(line));
					array[4] = new PropertyParseSchema("InternalLogonType", MailboxAuditLogEventSchema.InternalLogonType, null);
					array[5] = new PropertyParseSchema("DestFolderId", MailboxAuditLogEventSchema.DestFolderId, null);
					array[6] = new PropertyParseSchema("DestFolderPathName", MailboxAuditLogEventSchema.DestFolderPathName, null);
					array[7] = new PropertyParseSchema("FolderId", MailboxAuditLogEventSchema.FolderId, null);
					array[8] = new PropertyParseSchema("FolderPathName", MailboxAuditLogEventSchema.FolderPathName, null);
					array[9] = new PropertyParseSchema("ClientInfoString", MailboxAuditLogEventSchema.ClientInfoString, null);
					array[10] = new PropertyParseSchema("ClientVersion", MailboxAuditLogEventSchema.ClientVersion, null);
					array[11] = new PropertyParseSchema("ClientProcessName", MailboxAuditLogEventSchema.ClientProcessName, null);
					array[12] = new PropertyParseSchema("ClientMachineName", MailboxAuditLogEventSchema.ClientMachineName, null);
					array[13] = new PropertyParseSchema("ClientIPAddress", MailboxAuditLogEventSchema.ClientIPAddress, null);
					array[14] = new PropertyParseSchema("LogonUserSid", MailboxAuditLogEventSchema.LogonUserSid, null);
					array[15] = new PropertyParseSchema("LogonUserDisplayName", MailboxAuditLogEventSchema.LogonUserDisplayName, null);
					array[16] = new PropertyParseSchema("MailboxOwnerUPN", MailboxAuditLogEventSchema.MailboxOwnerUPN, null);
					array[17] = new PropertyParseSchema("MailboxOwnerSid", MailboxAuditLogEventSchema.MailboxOwnerSid, null);
					array[18] = new PropertyParseSchema("ItemId", MailboxAuditLogEventSchema.ItemId, null);
					array[19] = new PropertyParseSchema("ItemSubject", MailboxAuditLogEventSchema.ItemSubject, null);
					array[20] = new PropertyParseSchema("DestMailboxOwnerUPN", MailboxAuditLogEventSchema.DestMailboxOwnerUPN, null);
					array[21] = new PropertyParseSchema("DestMailboxOwnerSid", MailboxAuditLogEventSchema.DestMailboxOwnerSid, null);
					array[22] = new PropertyParseSchema("DestMailboxGuid", MailboxAuditLogEventSchema.DestMailboxGuid, null);
					array[23] = new PropertyParseSchema("DirtyProperties", MailboxAuditLogEventSchema.DirtyProperties, null);
					array[24] = new PropertyParseSchema("CrossMailboxOperation", MailboxAuditLogEventSchema.CrossMailboxOperation, (string line) => AuditLogParseSerialize.ParseBoolean(line));
					array[25] = new PropertyParseSchema("OriginatingServer", MailboxAuditLogEventSchema.OriginatingServer, null);
					AuditLogParseSerialize.mailboxAuditLogRecordParseSchema = array;
				}
				return AuditLogParseSerialize.mailboxAuditLogRecordParseSchema;
			}
		}

		private const string LabelValueSeparator = ":";

		public const string LogID = "audit";

		private static PropertyParseSchema[] mailboxAuditLogRecordParseSchema;
	}
}
