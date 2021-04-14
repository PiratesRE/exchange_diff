using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidPKConfigFormatException : LocalizedException
	{
		public InvalidPKConfigFormatException(string file) : base(Strings.InvalidPKConfigFormat(file))
		{
			this.file = file;
		}

		public InvalidPKConfigFormatException(string file, Exception innerException) : base(Strings.InvalidPKConfigFormat(file), innerException)
		{
			this.file = file;
		}

		protected InvalidPKConfigFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
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
