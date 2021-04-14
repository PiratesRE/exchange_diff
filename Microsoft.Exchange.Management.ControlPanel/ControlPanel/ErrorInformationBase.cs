using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(ShouldContinueExceptionDetails))]
	[DataContract]
	public class ErrorInformationBase
	{
		public ErrorInformationBase()
		{
		}

		public ErrorInformationBase(Exception exception)
		{
			this.Exception = exception;
			if (string.IsNullOrEmpty(this.Message))
			{
				this.Message = exception.Message;
			}
			if (ErrorHandlingUtil.CanShowDebugInfo(this.Exception))
			{
				this.CallStack = this.Exception.ToTraceString();
			}
			IExceptionDetails exceptionDetails = this.Exception as IExceptionDetails;
			if (exceptionDetails != null)
			{
				this.Details = exceptionDetails.Details;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public virtual string Message { get; protected set; }

		[DataMember(EmitDefaultValue = false)]
		public string CallStack { get; protected set; }

		[DataMember(EmitDefaultValue = false)]
		public object Details { get; private set; }

		public virtual Exception Exception { get; protected set; }

		internal void Translate(Identity translationIdentity)
		{
			this.Translate(translationIdentity, this.Message);
		}

		internal void Translate(Identity translationIdentity, string newMsg)
		{
			this.Message = PowerShellMessageTranslator.Instance.Translate(translationIdentity, this.Exception, newMsg);
		}
	}
}
