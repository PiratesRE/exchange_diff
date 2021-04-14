using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileOpenException : TransientException
	{
		public FileOpenException(string fileName, string errMsg) : base(ReplayStrings.FileOpenError(fileName, errMsg))
		{
			this.fileName = fileName;
			this.errMsg = errMsg;
		}

		public FileOpenException(string fileName, string errMsg, Exception innerException) : base(ReplayStrings.FileOpenError(fileName, errMsg), innerException)
		{
			this.fileName = fileName;
			this.errMsg = errMsg;
		}

		protected FileOpenException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fileName = (string)info.GetValue("fileName", typeof(string));
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fileName", this.fileName);
			info.AddValue("errMsg", this.errMsg);
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string fileName;

		private readonly string errMsg;
	}
}
