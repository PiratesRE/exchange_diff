using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SourceFileOpenException : LocalizedException
	{
		public SourceFileOpenException(string fileName) : base(Strings.SourceFileOpenException(fileName))
		{
			this.fileName = fileName;
		}

		public SourceFileOpenException(string fileName, Exception innerException) : base(Strings.SourceFileOpenException(fileName), innerException)
		{
			this.fileName = fileName;
		}

		protected SourceFileOpenException(SerializationInfo info, StreamingContext context) : base(info, context)
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
