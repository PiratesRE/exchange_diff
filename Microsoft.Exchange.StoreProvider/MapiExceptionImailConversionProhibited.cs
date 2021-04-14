using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionImailConversionProhibited : LocalizedException
	{
		internal MapiExceptionImailConversionProhibited(string message, int hresult, int ec, DiagnosticContext diagCtx, Exception innerException) : base(new LocalizedString(string.Concat(new string[]
		{
			"MapiExceptionImailConversionProhibited: ",
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

		internal MapiExceptionImailConversionProhibited(string message) : base(new LocalizedString("MapiExceptionImailConversionProhibited: " + message))
		{
			this.lowLevelError = 0;
			base.ErrorCode = -2147467259;
			this.diagContext = new DiagnosticContext();
		}

		internal MapiExceptionImailConversionProhibited(string message, Exception innerException) : base(new LocalizedString("MapiExceptionImailConversionProhibited: " + message), innerException)
		{
			this.lowLevelError = 0;
			base.ErrorCode = -2147467259;
			this.diagContext = new DiagnosticContext();
		}

		private MapiExceptionImailConversionProhibited(SerializationInfo info, StreamingContext context) : base(info, context)
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
