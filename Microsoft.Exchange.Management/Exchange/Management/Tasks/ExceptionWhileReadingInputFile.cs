using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExceptionWhileReadingInputFile : LocalizedException
	{
		public ExceptionWhileReadingInputFile(string filename, string exMessage) : base(Strings.ExceptionWhileReadingInputFile(filename, exMessage))
		{
			this.filename = filename;
			this.exMessage = exMessage;
		}

		public ExceptionWhileReadingInputFile(string filename, string exMessage, Exception innerException) : base(Strings.ExceptionWhileReadingInputFile(filename, exMessage), innerException)
		{
			this.filename = filename;
			this.exMessage = exMessage;
		}

		protected ExceptionWhileReadingInputFile(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filename = (string)info.GetValue("filename", typeof(string));
			this.exMessage = (string)info.GetValue("exMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("filename", this.filename);
			info.AddValue("exMessage", this.exMessage);
		}

		public string Filename
		{
			get
			{
				return this.filename;
			}
		}

		public string ExMessage
		{
			get
			{
				return this.exMessage;
			}
		}

		private readonly string filename;

		private readonly string exMessage;
	}
}
