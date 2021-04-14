using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	internal class RegistrationPropertyError
	{
		public RegistrationPropertyError(PropertyDefinition propDef, PropertyErrorCode errorCode)
		{
			this.PropertyDefinition = propDef;
			this.ErrorCode = errorCode;
			this.Exception = null;
		}

		public RegistrationPropertyError(PropertyDefinition propDef, Exception exception)
		{
			this.PropertyDefinition = propDef;
			this.ErrorCode = PropertyErrorCode.Exception;
			this.Exception = exception;
		}

		public PropertyErrorCode ErrorCode { get; private set; }

		public PropertyDefinition PropertyDefinition { get; private set; }

		public Exception Exception { get; private set; }

		public override string ToString()
		{
			return string.Format("[PropertyError] Property: {0}[{1}], Code: {2}, Exception: {3}", new object[]
			{
				this.PropertyDefinition.Name,
				this.PropertyDefinition.Type.Name,
				this.ErrorCode,
				(this.Exception == null) ? "NULL" : this.Exception.ToString()
			});
		}
	}
}
