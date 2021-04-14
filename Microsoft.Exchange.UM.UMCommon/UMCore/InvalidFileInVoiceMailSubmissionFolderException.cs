using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidFileInVoiceMailSubmissionFolderException : LocalizedException
	{
		public InvalidFileInVoiceMailSubmissionFolderException(string file, string error) : base(Strings.InvalidFileInVoiceMailSubmissionFolder(file, error))
		{
			this.file = file;
			this.error = error;
		}

		public InvalidFileInVoiceMailSubmissionFolderException(string file, string error, Exception innerException) : base(Strings.InvalidFileInVoiceMailSubmissionFolder(file, error), innerException)
		{
			this.file = file;
			this.error = error;
		}

		protected InvalidFileInVoiceMailSubmissionFolderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.file = (string)info.GetValue("file", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("file", this.file);
			info.AddValue("error", this.error);
		}

		public string File
		{
			get
			{
				return this.file;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string file;

		private readonly string error;
	}
}
