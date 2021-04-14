using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ErrorReadingFileException : MigrationTransientException
	{
		public ErrorReadingFileException(string file) : base(UpgradeHandlerStrings.ErrorReadingFile(file))
		{
			this.file = file;
		}

		public ErrorReadingFileException(string file, Exception innerException) : base(UpgradeHandlerStrings.ErrorReadingFile(file), innerException)
		{
			this.file = file;
		}

		protected ErrorReadingFileException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.file = (string)info.GetValue("file", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("file", this.file);
		}

		public string File
		{
			get
			{
				return this.file;
			}
		}

		private readonly string file;
	}
}
