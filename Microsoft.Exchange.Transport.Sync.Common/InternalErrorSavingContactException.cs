using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InternalErrorSavingContactException : ImportContactsException
	{
		public InternalErrorSavingContactException(int failedContactIndex, int contactsSaved) : base(Strings.InternalErrorSavingContact(failedContactIndex, contactsSaved))
		{
			this.failedContactIndex = failedContactIndex;
			this.contactsSaved = contactsSaved;
		}

		public InternalErrorSavingContactException(int failedContactIndex, int contactsSaved, Exception innerException) : base(Strings.InternalErrorSavingContact(failedContactIndex, contactsSaved), innerException)
		{
			this.failedContactIndex = failedContactIndex;
			this.contactsSaved = contactsSaved;
		}

		protected InternalErrorSavingContactException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.failedContactIndex = (int)info.GetValue("failedContactIndex", typeof(int));
			this.contactsSaved = (int)info.GetValue("contactsSaved", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("failedContactIndex", this.failedContactIndex);
			info.AddValue("contactsSaved", this.contactsSaved);
		}

		public int FailedContactIndex
		{
			get
			{
				return this.failedContactIndex;
			}
		}

		public int ContactsSaved
		{
			get
			{
				return this.contactsSaved;
			}
		}

		private readonly int failedContactIndex;

		private readonly int contactsSaved;
	}
}
