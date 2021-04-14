using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RootFolderInvalidPermanentException : MailboxReplicationPermanentException
	{
		public RootFolderInvalidPermanentException(LocalizedString errorMessage) : base(Strings.ErrorRootFolderInvalid(errorMessage))
		{
			this.errorMessage = errorMessage;
		}

		public RootFolderInvalidPermanentException(LocalizedString errorMessage, Exception innerException) : base(Strings.ErrorRootFolderInvalid(errorMessage), innerException)
		{
			this.errorMessage = errorMessage;
		}

		protected RootFolderInvalidPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
