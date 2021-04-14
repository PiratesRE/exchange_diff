using System;
using System.Net;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	[Serializable]
	internal class AirSyncPermanentException : LocalizedException
	{
		internal AirSyncPermanentException(LocalizedString message, Exception innerException, bool logEvent) : this(message, innerException, HttpStatusCode.InternalServerError, null, StatusCode.ServerError, logEvent)
		{
		}

		internal AirSyncPermanentException(bool logEvent) : this(new LocalizedString(string.Empty), null, HttpStatusCode.InternalServerError, null, StatusCode.ServerError, logEvent)
		{
		}

		internal AirSyncPermanentException(StatusCode airSyncStatusCode, XmlDocument xmlResponse, Exception innerException, bool logEvent) : this(new LocalizedString(string.Empty), innerException, HttpStatusCode.OK, xmlResponse, airSyncStatusCode, logEvent)
		{
		}

		internal AirSyncPermanentException(StatusCode airSyncStatusCode, XmlDocument xmlResponse, LocalizedString message, bool logEvent) : this(message, null, HttpStatusCode.OK, xmlResponse, airSyncStatusCode, logEvent)
		{
		}

		internal AirSyncPermanentException(StatusCode airSyncStatusCode, XmlDocument xmlResponse, LocalizedString message, Exception innerException, bool logEvent) : this(message, innerException, HttpStatusCode.OK, xmlResponse, airSyncStatusCode, logEvent)
		{
		}

		internal AirSyncPermanentException(StatusCode airSyncStatusCode, LocalizedString message, bool logEvent) : this(message, null, HttpStatusCode.OK, null, airSyncStatusCode, logEvent)
		{
		}

		internal AirSyncPermanentException(StatusCode airSyncStatusCode, bool logEvent) : this(new LocalizedString(string.Empty), null, HttpStatusCode.OK, null, airSyncStatusCode, logEvent)
		{
		}

		internal AirSyncPermanentException(StatusCode airSyncStatusCode, LocalizedString message, Exception innerException, bool logEvent) : this(message, innerException, HttpStatusCode.OK, null, airSyncStatusCode, logEvent)
		{
		}

		internal AirSyncPermanentException(StatusCode airSyncStatusCode, Exception innerException, bool logEvent) : this(new LocalizedString(string.Empty), innerException, HttpStatusCode.OK, null, airSyncStatusCode, logEvent)
		{
		}

		internal AirSyncPermanentException(HttpStatusCode httpStatusCode, StatusCode airSyncStatusCode, Exception innerException, bool logEvent) : this(new LocalizedString(string.Empty), innerException, httpStatusCode, null, airSyncStatusCode, logEvent)
		{
		}

		internal AirSyncPermanentException(HttpStatusCode httpStatusCode, StatusCode airSyncStatusCode, LocalizedString message, bool logEvent) : this(message, null, httpStatusCode, null, airSyncStatusCode, logEvent)
		{
		}

		internal AirSyncPermanentException(HttpStatusCode httpStatusCode, StatusCode airSyncStatusCode, LocalizedString message, Exception innerException, bool logEvent) : this(message, innerException, httpStatusCode, null, airSyncStatusCode, logEvent)
		{
		}

		protected AirSyncPermanentException(SerializationInfo info, StreamingContext context)
		{
			this.httpStatusCode = HttpStatusCode.InternalServerError;
			this.logStackTraceToEventLog = true;
			base..ctor(info, context);
			AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "AirSyncPermanentException is being deserialized");
			this.httpStatusCode = (HttpStatusCode)info.GetValue("httpStatusCode", typeof(HttpStatusCode));
			this.airSyncStatusCode = (StatusCode)info.GetValue("airSyncStatusCode", typeof(StatusCode));
			foreach (SerializationEntry serializationEntry in info)
			{
				if (serializationEntry.Name == "xmlResponse")
				{
					this.xmlResponse = (XmlDocument)serializationEntry.Value;
				}
			}
		}

		private AirSyncPermanentException(LocalizedString message, Exception innerException, HttpStatusCode httpStatusCode, XmlDocument xmlResponse, StatusCode airSyncStatusCode, bool logEvent)
		{
			this.httpStatusCode = HttpStatusCode.InternalServerError;
			this.logStackTraceToEventLog = true;
			base..ctor(message, innerException);
			this.httpStatusCode = httpStatusCode;
			this.xmlResponse = xmlResponse;
			this.airSyncStatusCode = airSyncStatusCode;
			if (message.IsEmpty)
			{
				this.logExceptionToEventLog = false;
			}
			else
			{
				this.logExceptionToEventLog = logEvent;
			}
			AirSyncUtility.ExceptionToStringHelper arg = new AirSyncUtility.ExceptionToStringHelper(this);
			AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.RequestsTracer, this, "AirSyncPermanentException: {0}", arg);
		}

		internal int AirSyncStatusCodeInInt
		{
			get
			{
				return (int)this.airSyncStatusCode;
			}
		}

		internal StatusCode AirSyncStatusCode
		{
			get
			{
				return this.airSyncStatusCode;
			}
		}

		internal HttpStatusCode HttpStatusCode
		{
			get
			{
				return this.httpStatusCode;
			}
		}

		protected internal bool LogExceptionToEventLog
		{
			get
			{
				return this.logExceptionToEventLog;
			}
			protected set
			{
				this.logExceptionToEventLog = value;
			}
		}

		internal bool LogStackTraceToEventLog
		{
			get
			{
				return this.logStackTraceToEventLog;
			}
			set
			{
				this.logStackTraceToEventLog = value;
			}
		}

		internal XmlDocument XmlResponse
		{
			get
			{
				return this.xmlResponse;
			}
		}

		internal string ErrorStringForProtocolLogger { get; set; }

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.ErrorStringForProtocolLogger))
			{
				return base.ToString();
			}
			LocalizedString localizedString = base.LocalizedString;
			if (!base.LocalizedString.IsEmpty)
			{
				return string.Format("{0}\r\n{1}\r\n{2}", base.ToString(), base.LocalizedString, this.ErrorStringForProtocolLogger);
			}
			return string.Format("{0}\r\n{1}", base.ToString(), this.ErrorStringForProtocolLogger);
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
		}

		private StatusCode airSyncStatusCode;

		private HttpStatusCode httpStatusCode;

		private bool logExceptionToEventLog;

		private bool logStackTraceToEventLog;

		[OptionalField]
		private XmlDocument xmlResponse;
	}
}
