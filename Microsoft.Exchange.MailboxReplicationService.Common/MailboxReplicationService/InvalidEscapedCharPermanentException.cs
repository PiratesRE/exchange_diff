using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidEscapedCharPermanentException : FolderFilterPermanentException
	{
		public InvalidEscapedCharPermanentException(string folderPath, int charPosition) : base(MrsStrings.InvalidEscapedChar(folderPath, charPosition))
		{
			this.folderPath = folderPath;
			this.charPosition = charPosition;
		}

		public InvalidEscapedCharPermanentException(string folderPath, int charPosition, Exception innerException) : base(MrsStrings.InvalidEscapedChar(folderPath, charPosition), innerException)
		{
			this.folderPath = folderPath;
			this.charPosition = charPosition;
		}

		protected InvalidEscapedCharPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folderPath = (string)info.GetValue("folderPath", typeof(string));
			this.charPosition = (int)info.GetValue("charPosition", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folderPath", this.folderPath);
			info.AddValue("charPosition", this.charPosition);
		}

		public string FolderPath
		{
			get
			{
				return this.folderPath;
			}
		}

		public int CharPosition
		{
			get
			{
				return this.charPosition;
			}
		}

		private readonly string folderPath;

		private readonly int charPosition;
	}
}
