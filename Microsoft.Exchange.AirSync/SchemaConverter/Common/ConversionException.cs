using System;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	[Serializable]
	internal class ConversionException : LocalizedException
	{
		public ConversionException() : this(HttpStatusCode.InternalServerError, null, null)
		{
		}

		public ConversionException(string message) : this(HttpStatusCode.InternalServerError, message, null)
		{
		}

		public ConversionException(string message, Exception innerException) : this(HttpStatusCode.InternalServerError, message, innerException)
		{
		}

		public ConversionException(HttpStatusCode httpStatusCode, string message) : this(httpStatusCode, message, null)
		{
		}

		public ConversionException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(new LocalizedString(message), innerException)
		{
			AirSyncDiagnostics.TraceError<HttpStatusCode, string, string>(ExTraceGlobals.CommonTracer, this, "ConversionException has been thrown. HttpStatusCode:'{0}', Message:'{1}', InnerException: '{2}'", httpStatusCode, (message != null) ? message : string.Empty, (innerException != null) ? innerException.Message : string.Empty);
			this.httpStatusCode = httpStatusCode;
			this.httpStatusCodeIsSet = true;
		}

		public ConversionException(int airSyncStatusCode) : this(airSyncStatusCode, null, null)
		{
		}

		public ConversionException(int airSyncStatusCode, string message) : this(airSyncStatusCode, message, null)
		{
		}

		public ConversionException(int airSyncStatusCode, Exception innerException) : this(airSyncStatusCode, null, innerException)
		{
		}

		public ConversionException(int airSyncStatusCode, string message, Exception innerException) : base(new LocalizedString(message), innerException)
		{
			AirSyncDiagnostics.TraceWarning<int, string, string>(ExTraceGlobals.CommonTracer, this, "ConversionException has been thrown. ConversionStatusCode:'{0}', Message:'{1}', InnerException: '{2}'", airSyncStatusCode, (message != null) ? message : string.Empty, (innerException != null) ? innerException.Message : string.Empty);
			this.airSyncStatusCode = airSyncStatusCode;
		}

		protected ConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			AirSyncDiagnostics.TraceError(ExTraceGlobals.CommonTracer, this, "ConversionException is being deserialized");
			this.httpStatusCode = (HttpStatusCode)info.GetValue("httpStatusCode", typeof(HttpStatusCode));
			this.airSyncStatusCode = (int)info.GetValue("airSyncStatusCode", typeof(int));
			this.httpStatusCodeIsSet = (bool)info.GetValue("isHttpStatusCodeSet", typeof(bool));
		}

		public int ConversionStatusCode
		{
			get
			{
				AirSyncDiagnostics.Assert(!this.httpStatusCodeIsSet);
				return this.airSyncStatusCode;
			}
		}

		public HttpStatusCode HttpStatusCode
		{
			get
			{
				AirSyncDiagnostics.Assert(this.httpStatusCodeIsSet);
				return this.httpStatusCode;
			}
		}

		public bool IsHttpStatusCodeSet
		{
			get
			{
				return this.httpStatusCodeIsSet;
			}
		}

		private int airSyncStatusCode;

		private HttpStatusCode httpStatusCode;

		private bool httpStatusCodeIsSet;
	}
}
