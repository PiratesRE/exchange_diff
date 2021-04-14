using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public abstract class AnchorLocalizedExceptionBase : LocalizedException
	{
		protected AnchorLocalizedExceptionBase(LocalizedString localizedErrorMessage, string internalError, Exception ex) : base(localizedErrorMessage, ex)
		{
			this.InternalError = internalError;
		}

		protected AnchorLocalizedExceptionBase(LocalizedString localizedErrorMessage, string internalError) : base(localizedErrorMessage)
		{
			this.InternalError = internalError;
		}

		protected AnchorLocalizedExceptionBase(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.InternalError = info.GetString("InternalError");
		}

		public string InternalError
		{
			get
			{
				return (string)this.Data["InternalError"];
			}
			internal set
			{
				this.Data["InternalError"] = value;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("InternalError", this.InternalError);
		}

		public override string ToString()
		{
			return "internal error:" + this.InternalError + base.ToString();
		}

		private const string InternalErrorKey = "InternalError";
	}
}
