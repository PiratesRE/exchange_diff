using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderFilterInvalidPermanentException : MailboxReplicationPermanentException
	{
		public FolderFilterInvalidPermanentException(LocalizedString errorMessage) : base(Strings.ErrorFolderFilterInvalid(errorMessage))
		{
			this.errorMessage = errorMessage;
		}

		public FolderFilterInvalidPermanentException(LocalizedString errorMessage, Exception innerException) : base(Strings.ErrorFolderFilterInvalid(errorMessage), innerException)
		{
			this.errorMessage = errorMessage;
		}

		protected FolderFilterInvalidPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorMessage = (LocalizedString)info.GetValue("errorMessage", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public LocalizedString ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly LocalizedString errorMessage;
	}
}
