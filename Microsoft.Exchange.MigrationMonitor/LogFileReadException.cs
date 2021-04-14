using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class LogFileReadException : LocalizedException
	{
		public LogFileReadException(string fileName) : base(MigrationMonitorStrings.ErrorLogFileRead(fileName))
		{
			this.fileName = fileName;
		}

		public LogFileReadException(string fileName, Exception innerException) : base(MigrationMonitorStrings.ErrorLogFileRead(fileName), innerException)
		{
			this.fileName = fileName;
		}

		protected LogFileReadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fileName = (string)info.GetValue("fileName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fileName", this.fileName);
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		private readonly string fileName;
	}
}
