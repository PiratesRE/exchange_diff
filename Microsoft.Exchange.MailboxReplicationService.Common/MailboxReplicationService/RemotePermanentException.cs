using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class RemotePermanentException : MailboxReplicationPermanentException, IMRSRemoteException
	{
		public RemotePermanentException(LocalizedString msg, Exception innerException) : base(msg, innerException)
		{
		}

		protected RemotePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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

		internal static void Serialize(SerializationInfo info, StreamingContext context, string originalFailureType, int mapiLowLevelError, WellKnownException[] wkeClasses, string remoteStackTrace)
		{
			info.AddValue("originalFailureType", originalFailureType);
			info.AddValue("mapiLowLevelError", mapiLowLevelError);
			info.AddValue("wkeCount", wkeClasses.Length);
			for (int i = 0; i < wkeClasses.Length; i++)
			{
				info.AddValue(string.Format("wke{0}", i), (int)wkeClasses[i]);
			}
			info.AddValue("remoteStackTrace", remoteStackTrace);
		}

		internal static void Deserialize(SerializationInfo info, StreamingContext context, out string originalFailureType, out int mapiLowLevelError, out WellKnownException[] wkeClasses, out string remoteStackTrace)
		{
			originalFailureType = info.GetString("originalFailureType");
			mapiLowLevelError = info.GetInt32("mapiLowLevelError");
			int @int = info.GetInt32("wkeCount");
			wkeClasses = new WellKnownException[@int];
			for (int i = 0; i < @int; i++)
			{
				wkeClasses[i] = (WellKnownException)info.GetInt32(string.Format("wke{0}", i));
			}
			try
			{
				remoteStackTrace = info.GetString("remoteStackTrace");
			}
			catch (SerializationException)
			{
				remoteStackTrace = null;
			}
		}

		private string originalFailureType;

		private WellKnownException[] wkeClasses;

		private int mapiLowLevelError;

		private string remoteStackTrace;
	}
}
