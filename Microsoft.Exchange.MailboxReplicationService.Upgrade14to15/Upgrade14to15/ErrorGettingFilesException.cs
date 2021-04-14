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
	internal class ErrorGettingFilesException : MigrationTransientException
	{
		public ErrorGettingFilesException(string directory) : base(UpgradeHandlerStrings.ErrorGettingFiles(directory))
		{
			this.directory = directory;
		}

		public ErrorGettingFilesException(string directory, Exception innerException) : base(UpgradeHandlerStrings.ErrorGettingFiles(directory), innerException)
		{
			this.directory = directory;
		}

		protected ErrorGettingFilesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.directory = (string)info.GetValue("directory", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("directory", this.directory);
		}

		public string Directory
		{
			get
			{
				return this.directory;
			}
		}

		private readonly string directory;
	}
}
