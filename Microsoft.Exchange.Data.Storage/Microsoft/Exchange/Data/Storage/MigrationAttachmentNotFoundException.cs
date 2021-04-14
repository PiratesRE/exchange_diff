using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationAttachmentNotFoundException : MigrationPermanentException
	{
		public MigrationAttachmentNotFoundException(string attachment) : base(ServerStrings.MigrationAttachmentNotFound(attachment))
		{
			this.attachment = attachment;
		}

		public MigrationAttachmentNotFoundException(string attachment, Exception innerException) : base(ServerStrings.MigrationAttachmentNotFound(attachment), innerException)
		{
			this.attachment = attachment;
		}

		protected MigrationAttachmentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.attachment = (string)info.GetValue("attachment", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("attachment", this.attachment);
		}

		public string Attachment
		{
			get
			{
				return this.attachment;
			}
		}

		private readonly string attachment;
	}
}
