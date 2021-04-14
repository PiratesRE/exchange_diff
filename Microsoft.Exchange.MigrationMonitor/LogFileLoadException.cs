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
	internal class LogFileLoadException : LocalizedException
	{
		public LogFileLoadException(string dirName) : base(MigrationMonitorStrings.ErrorLogFileLoad(dirName))
		{
			this.dirName = dirName;
		}

		public LogFileLoadException(string dirName, Exception innerException) : base(MigrationMonitorStrings.ErrorLogFileLoad(dirName), innerException)
		{
			this.dirName = dirName;
		}

		protected LogFileLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dirName = (string)info.GetValue("dirName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dirName", this.dirName);
		}

		public string DirName
		{
			get
			{
				return this.dirName;
			}
		}

		private readonly string dirName;
	}
}
