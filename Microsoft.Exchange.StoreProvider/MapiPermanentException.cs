using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class MapiPermanentException : LocalizedException
	{
		internal MapiPermanentException(string realExceptionClassName, string message, int hresult, int ec, DiagnosticContext diagCtx, Exception innerException) : base(new LocalizedString(string.Concat(new string[]
		{
			realExceptionClassName,
			": ",
			message,
			" (hr=0x",
			hresult.ToString("x"),
			", ec=",
			ec.ToString(),
			")",
			(diagCtx != null) ? ("\n" + diagCtx.ToString()) : ""
		})), innerException)
		{
			this.lowLevelError = ec;
			base.ErrorCode = hresult;
			this.diagContext = ((diagCtx != null) ? diagCtx : new DiagnosticContext());
		}

		internal MapiPermanentException(string realExceptionClassName, string message) : base(new LocalizedString(realExceptionClassName + ": " + message))
		{
			this.lowLevelError = 0;
			base.ErrorCode = -2147467259;
			this.diagContext = new DiagnosticContext();
		}

		internal MapiPermanentException(string realExceptionClassName, string message, Exception innerException) : base(new LocalizedString(realExceptionClassName + ": " + message), innerException)
		{
			this.lowLevelError = 0;
			base.ErrorCode = -2147467259;
			this.diagContext = new DiagnosticContext();
		}

		protected MapiPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.lowLevelError = info.GetInt32("lowLevelError");
			this.diagContext = new DiagnosticContext(info, context);
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("lowLevelError", this.lowLevelError);
			((ISerializable)this.diagContext).GetObjectData(info, context);
			base.GetObjectData(info, context);
		}

		public int LowLevelError
		{
			get
			{
				return this.lowLevelError;
			}
		}

		public new int HResult
		{
			get
			{
				return base.ErrorCode;
			}
		}

		public DiagnosticContext DiagCtx
		{
			get
			{
				return this.diagContext;
			}
		}

		private int lowLevelError;

		private DiagnosticContext diagContext;
	}
}
