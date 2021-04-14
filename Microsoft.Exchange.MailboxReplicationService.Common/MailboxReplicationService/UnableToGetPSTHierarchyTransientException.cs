using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToGetPSTHierarchyTransientException : MailboxReplicationTransientException
	{
		public UnableToGetPSTHierarchyTransientException(string filePath) : base(MrsStrings.UnableToGetPSTHierarchy(filePath))
		{
			this.filePath = filePath;
		}

		public UnableToGetPSTHierarchyTransientException(string filePath, Exception innerException) : base(MrsStrings.UnableToGetPSTHierarchy(filePath), innerException)
		{
			this.filePath = filePath;
		}

		protected UnableToGetPSTHierarchyTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filePath = (string)info.GetValue("filePath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("filePath", this.filePath);
		}

		public string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		private readonly string filePath;
	}
}
