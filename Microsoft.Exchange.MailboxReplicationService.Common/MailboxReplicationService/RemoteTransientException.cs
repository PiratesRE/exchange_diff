using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class RemoteTransientException : MailboxReplicationTransientException, IMRSRemoteException
	{
		public RemoteTransientException(LocalizedString msg, Exception innerException) : base(msg, innerException)
		{
		}

		protected RemoteTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			RemotePermanentException.Deserialize(info, context, out this.originalFailureType, out this.mapiLowLevelError, out this.wkeClasses, out this.remoteStackTrace);
		}

		string IMRSRemoteException.OriginalFailureType
		{
			get
			{
				return this.originalFailureType;
			}
			set
			{
				this.originalFailureType = value;
			}
		}

		WellKnownException[] IMRSRemoteException.WKEClasses
		{
			get
			{
				return this.wkeClasses;
			}
			set
			{
				this.wkeClasses = value;
			}
		}

		int IMRSRemoteException.MapiLowLevelError
		{
			get
			{
				return this.mapiLowLevelError;
			}
			set
			{
				this.mapiLowLevelError = value;
			}
		}

		string IMRSRemoteException.RemoteStackTrace
		{
			get
			{
				return this.remoteStackTrace;
			}
			set
			{
				this.remoteStackTrace = value;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			RemotePermanentException.Serialize(info, context, this.originalFailureType, this.mapiLowLevelError, this.wkeClasses, this.remoteStackTrace);
		}

		private string originalFailureType;

		private WellKnownException[] wkeClasses;

		private int mapiLowLevelError;

		private string remoteStackTrace;
	}
}
