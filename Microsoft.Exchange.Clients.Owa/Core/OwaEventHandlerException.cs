using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaEventHandlerException : OwaTransientException
	{
		public OwaEventHandlerException() : base(null)
		{
		}

		public OwaEventHandlerException(string message, string description, OwaEventHandlerErrorCode errorCode, bool hideDebugInformation, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
			this.description = description;
			this.errorCode = errorCode;
			this.hideDebugInformation = hideDebugInformation;
		}

		public OwaEventHandlerException(string message, string description, OwaEventHandlerErrorCode errorCode, Exception innerException, object thisObject) : this(message, description, errorCode, false, innerException, thisObject)
		{
		}

		public OwaEventHandlerException(string message, string description, Exception innerException) : this(message, description, OwaEventHandlerErrorCode.NotSet, innerException, null)
		{
		}

		public OwaEventHandlerException(string message, string description) : this(message, description, OwaEventHandlerErrorCode.NotSet, null, null)
		{
		}

		public OwaEventHandlerException(string message, string description, OwaEventHandlerErrorCode errorCode, bool hideDebugInformation) : this(message, description, errorCode, hideDebugInformation, null, null)
		{
		}

		public OwaEventHandlerException(string message, string description, bool hideDebugInformation) : this(message, description, OwaEventHandlerErrorCode.NotSet, hideDebugInformation, null, null)
		{
		}

		public OwaEventHandlerException(string message, string description, OwaEventHandlerErrorCode errorCode) : this(message, description, errorCode, null, null)
		{
		}

		public OwaEventHandlerException(string message) : this(message, message, OwaEventHandlerErrorCode.NotSet, null, null)
		{
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public bool HideDebugInformation
		{
			get
			{
				return this.hideDebugInformation;
			}
		}

		public new OwaEventHandlerErrorCode ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		private string description = string.Empty;

		private OwaEventHandlerErrorCode errorCode;

		private bool hideDebugInformation;
	}
}
