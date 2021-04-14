using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnexpectedErrorPermanentException : MailboxReplicationPermanentException
	{
		public UnexpectedErrorPermanentException(int hr) : base(MrsStrings.UnexpectedError(hr))
		{
			this.hr = hr;
		}

		public UnexpectedErrorPermanentException(int hr, Exception innerException) : base(MrsStrings.UnexpectedError(hr), innerException)
		{
			this.hr = hr;
		}

		protected UnexpectedErrorPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.hr = (int)info.GetValue("hr", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("hr", this.hr);
		}

		public int Hr
		{
			get
			{
				return this.hr;
			}
		}

		private readonly int hr;
	}
}
