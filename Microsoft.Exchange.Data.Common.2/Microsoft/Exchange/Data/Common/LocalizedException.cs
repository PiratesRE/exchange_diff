using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Common
{
	[Serializable]
	public class LocalizedException : Exception, ILocalizedException, ILocalizedString
	{
		internal static void TraceException(string formatString, params object[] formatObjects)
		{
			LocalizedException.TraceExceptionDelegate traceExceptionCallback = LocalizedException.TraceExceptionCallback;
			if (traceExceptionCallback != null)
			{
				LocalizedException.TraceExceptionCallback(formatString, formatObjects);
			}
		}

		public LocalizedException(LocalizedString localizedString) : this(localizedString, null)
		{
			LocalizedException.TraceException("Created LocalizedException({0})", new object[]
			{
				localizedString
			});
		}

		public LocalizedException(LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
			this.localizedString = localizedString;
			LocalizedException.TraceException("Created LocalizedException({0}, innerException)", new object[]
			{
				localizedString
			});
		}

		public override string Message
		{
			get
			{
				return this.LocalizedString.ToString(this.FormatProvider);
			}
		}

		public IFormatProvider FormatProvider
		{
			get
			{
				return this.formatProvider;
			}
			set
			{
				this.formatProvider = value;
			}
		}

		public LocalizedString LocalizedString
		{
			get
			{
				return this.localizedString;
			}
		}

		public int ErrorCode
		{
			get
			{
				return base.HResult;
			}
			set
			{
				base.HResult = value;
			}
		}

		public string StringId
		{
			get
			{
				return this.localizedString.StringId;
			}
		}

		public ReadOnlyCollection<object> StringFormatParameters
		{
			get
			{
				return this.localizedString.FormatParameters;
			}
		}

		public static int GenerateErrorCode(Exception e)
		{
			int num = LocalizedException.InternalGenerateErrorCode(e);
			int num2 = 0;
			if (e.InnerException != null)
			{
				Exception innerException = e.InnerException;
				while (innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
				}
				num2 = LocalizedException.InternalGenerateErrorCode(innerException);
			}
			return num ^ num2;
		}

		private static int InternalGenerateErrorCode(Exception e)
		{
			StackTrace stackTrace = new StackTrace(e);
			int hashCode = stackTrace.ToString().GetHashCode();
			int hashCode2 = e.GetType().GetHashCode();
			int num = 0;
			ILocalizedString localizedString = e as ILocalizedString;
			if (localizedString != null)
			{
				num = localizedString.LocalizedString.GetHashCode();
			}
			return num ^ hashCode ^ hashCode2;
		}

		protected LocalizedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.localizedString = (LocalizedString)info.GetValue("localizedString", typeof(LocalizedString));
			LocalizedException.TraceException("Created LocalizedException(info, context)", new object[0]);
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("localizedString", this.LocalizedString);
		}

		internal static LocalizedException.TraceExceptionDelegate TraceExceptionCallback;

		private IFormatProvider formatProvider;

		private LocalizedString localizedString;

		internal delegate void TraceExceptionDelegate(string formatString, params object[] formatObjects);
	}
}
