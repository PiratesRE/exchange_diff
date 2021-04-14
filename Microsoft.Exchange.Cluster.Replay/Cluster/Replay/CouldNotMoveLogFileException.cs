using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotMoveLogFileException : TransientException
	{
		public CouldNotMoveLogFileException(string oldpath, string newpath) : base(ReplayStrings.CouldNotMoveLogFile(oldpath, newpath))
		{
			this.oldpath = oldpath;
			this.newpath = newpath;
		}

		public CouldNotMoveLogFileException(string oldpath, string newpath, Exception innerException) : base(ReplayStrings.CouldNotMoveLogFile(oldpath, newpath), innerException)
		{
			this.oldpath = oldpath;
			this.newpath = newpath;
		}

		protected CouldNotMoveLogFileException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.oldpath = (string)info.GetValue("oldpath", typeof(string));
			this.newpath = (string)info.GetValue("newpath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("oldpath", this.oldpath);
			info.AddValue("newpath", this.newpath);
		}

		public string Oldpath
		{
			get
			{
				return this.oldpath;
			}
		}

		public string Newpath
		{
			get
			{
				return this.newpath;
			}
		}

		private readonly string oldpath;

		private readonly string newpath;
	}
}
