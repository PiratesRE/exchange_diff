using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationTransientException : TransientException
	{
		public MigrationTransientException(LocalizedString localizedErrorMessage) : base(localizedErrorMessage)
		{
			this.InternalError = string.Empty;
		}

		public MigrationTransientException(LocalizedString localizedErrorMessage, Exception innerException) : base(localizedErrorMessage, innerException)
		{
			this.InternalError = string.Empty;
		}

		public MigrationTransientException(LocalizedString localizedErrorMessage, string internalError, Exception ex) : base(localizedErrorMessage, ex)
		{
			this.InternalError = internalError;
		}

		public MigrationTransientException(LocalizedString localizedErrorMessage, string internalError) : base(localizedErrorMessage)
		{
			this.InternalError = internalError;
		}

		protected MigrationTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.InternalError = info.GetString("InternalError");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("InternalError", this.InternalError);
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

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.InternalError))
			{
				return "internal error:" + this.InternalError + base.ToString();
			}
			return base.ToString();
		}
	}
}
