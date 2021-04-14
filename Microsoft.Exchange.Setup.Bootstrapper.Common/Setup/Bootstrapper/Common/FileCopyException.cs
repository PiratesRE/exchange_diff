using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Bootstrapper.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FileCopyException : LocalizedException
	{
		public FileCopyException(string srcDir, string dstDir) : base(Strings.FileCopyFailed(srcDir, dstDir))
		{
			this.srcDir = srcDir;
			this.dstDir = dstDir;
		}

		public FileCopyException(string srcDir, string dstDir, Exception innerException) : base(Strings.FileCopyFailed(srcDir, dstDir), innerException)
		{
			this.srcDir = srcDir;
			this.dstDir = dstDir;
		}

		protected FileCopyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.srcDir = (string)info.GetValue("srcDir", typeof(string));
			this.dstDir = (string)info.GetValue("dstDir", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("srcDir", this.srcDir);
			info.AddValue("dstDir", this.dstDir);
		}

		public string SrcDir
		{
			get
			{
				return this.srcDir;
			}
		}

		public string DstDir
		{
			get
			{
				return this.dstDir;
			}
		}

		private readonly string srcDir;

		private readonly string dstDir;
	}
}
