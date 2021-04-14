using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderAlreadyExistsException : MailboxReplicationPermanentException
	{
		public FolderAlreadyExistsException(string name) : base(MrsStrings.FolderAlreadyExists(name))
		{
			this.name = name;
		}

		public FolderAlreadyExistsException(string name, Exception innerException) : base(MrsStrings.FolderAlreadyExists(name), innerException)
		{
			this.name = name;
		}

		protected FolderAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
