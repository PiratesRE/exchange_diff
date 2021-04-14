using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Auditing;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal class AuditLogRecord : ItemPropertiesBase, IAuditLogRecord
	{
		[DataMember]
		public int RecordTypeInt { get; set; }

		[DataMember]
		public DateTime CreationTime { get; set; }

		[DataMember]
		public string Operation { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ObjectId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string UserId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public Tuple<string, string>[] Details { get; set; }

		IEnumerable<KeyValuePair<string, string>> IAuditLogRecord.GetDetails()
		{
			if (this.Details != null)
			{
				foreach (Tuple<string, string> detail in this.Details)
				{
					yield return new KeyValuePair<string, string>(detail.Item1, detail.Item2);
				}
			}
			yield break;
		}

		AuditLogRecordType IAuditLogRecord.RecordType
		{
			get
			{
				return (AuditLogRecordType)this.RecordTypeInt;
			}
		}

		public override void Apply(MailboxSession session, Item item)
		{
			BodyWriteConfiguration configuration = new BodyWriteConfiguration(BodyFormat.TextPlain);
			using (TextWriter textWriter = item.Body.OpenTextWriter(configuration))
			{
				string asString = AuditLogParseSerialize.GetAsString(this);
				textWriter.Write(asString);
			}
			item.ClassName = "IPM.AuditLog";
		}
	}
}
