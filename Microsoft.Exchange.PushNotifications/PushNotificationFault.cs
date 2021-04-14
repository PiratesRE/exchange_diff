using System;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	public class PushNotificationFault : IExtensibleDataObject
	{
		public PushNotificationFault(Exception ex) : this(ex, false, 0, true)
		{
		}

		public PushNotificationFault(Exception ex, int backOffTimeInMilliseconds, bool includeOriginatingServer) : this(ex, false, backOffTimeInMilliseconds, includeOriginatingServer)
		{
		}

		public PushNotificationFault(Exception ex, bool includeExceptionDetails, int backOffTimeInMilliseconds = 0, bool includeOriginatingServer = true)
		{
			ArgumentValidator.ThrowIfNull("ex", ex);
			if (ex is LocalizedException)
			{
				this.Message = ((LocalizedException)ex).LocalizedString;
			}
			else
			{
				this.Message = ex.Message;
			}
			this.CanRetry = (ex is PushNotificationTransientException);
			if (includeExceptionDetails)
			{
				this.StackTrace = ex.StackTrace;
				if (ex.InnerException != null)
				{
					this.InnerFault = new PushNotificationFault(ex.InnerException, includeExceptionDetails, 0, true);
				}
			}
			this.BackOffTimeInMilliseconds = backOffTimeInMilliseconds;
			if (includeOriginatingServer)
			{
				this.OriginatingServer = Environment.MachineName;
			}
		}

		[DataMember]
		public string Message { get; private set; }

		[DataMember]
		public bool CanRetry { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public string StackTrace { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public PushNotificationFault InnerFault { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public int BackOffTimeInMilliseconds { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public string OriginatingServer { get; private set; }

		public ExtensionDataObject ExtensionData { get; set; }

		public override string ToString()
		{
			return this.Message;
		}

		public string ToFullString()
		{
			if (this.toFullStringCache == null)
			{
				StringBuilder stringBuilder = new StringBuilder("{");
				PushNotificationFault.ExceptionToFullString(this, ref stringBuilder);
				if (this.InnerFault != null)
				{
					stringBuilder.Append("\",InnerException\":{");
					PushNotificationFault.ExceptionToFullString(this.InnerFault, ref stringBuilder);
					stringBuilder.Append("}");
				}
				if (this.BackOffTimeInMilliseconds != 0)
				{
					stringBuilder.AppendFormat(",\"BackOffTimeInMilliseconds\":{0}", this.BackOffTimeInMilliseconds);
				}
				if (!string.IsNullOrEmpty(this.OriginatingServer))
				{
					stringBuilder.AppendFormat(",\"OriginatingServer\":{0}", this.OriginatingServer);
				}
				stringBuilder.Append("}");
				this.toFullStringCache = stringBuilder.ToString();
			}
			return this.toFullStringCache;
		}

		private static void ExceptionToFullString(PushNotificationFault notificationFault, ref StringBuilder builder)
		{
			builder.AppendFormat("\"Message\":{0},\"CanRetry\":{1}", notificationFault.Message, notificationFault.CanRetry);
			if (!string.IsNullOrEmpty(notificationFault.StackTrace))
			{
				builder.AppendFormat(",\"StackTrace\":{0}", notificationFault.StackTrace);
			}
		}

		private string toFullStringCache;
	}
}
