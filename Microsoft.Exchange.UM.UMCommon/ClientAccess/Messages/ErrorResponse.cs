using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.Prompts.Provisioning;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class ErrorResponse : ResponseBase
	{
		internal ErrorResponse()
		{
		}

		internal ErrorResponse(string errorType)
		{
			this.errorType = errorType;
		}

		internal ErrorResponse(Exception exception)
		{
			this.errorType = exception.GetType().FullName;
		}

		public string ErrorType
		{
			get
			{
				return this.errorType;
			}
			set
			{
				this.errorType = value;
			}
		}

		internal Exception GetException()
		{
			ErrorResponse.CreateExceptionDelegate createExceptionDelegate;
			if (ErrorResponse.errorTable.TryGetValue(this.errorType, out createExceptionDelegate))
			{
				return createExceptionDelegate();
			}
			CallIdTracer.TraceError(ExTraceGlobals.ClientAccessTracer, this, "GetException: Cannot create exception of type {0}.", new object[]
			{
				this.errorType
			});
			return new ClientAccessException();
		}

		private static Dictionary<string, ErrorResponse.CreateExceptionDelegate> CreateErrorTable()
		{
			Dictionary<string, ErrorResponse.CreateExceptionDelegate> dictionary = new Dictionary<string, ErrorResponse.CreateExceptionDelegate>();
			dictionary[typeof(IPGatewayNotFoundException).FullName] = (() => new IPGatewayNotFoundException());
			dictionary[typeof(InvalidCallIdException).FullName] = (() => new InvalidCallIdException());
			dictionary[typeof(DialingRulesException).FullName] = (() => new DialingRulesException());
			dictionary[typeof(InvalidPhoneNumberException).FullName] = (() => new InvalidPhoneNumberException());
			dictionary[typeof(InvalidSipUriException).FullName] = (() => new InvalidSipUriException());
			dictionary[typeof(UserNotUmEnabledException).FullName] = (() => new UserNotUmEnabledException(string.Empty));
			dictionary[typeof(OverPlayOnPhoneCallLimitException).FullName] = (() => new OverPlayOnPhoneCallLimitException());
			dictionary[typeof(InvalidFileNameException).FullName] = (() => new InvalidFileNameException(128));
			dictionary[typeof(NoCallerIdToUseException).FullName] = (() => new NoCallerIdToUseException());
			dictionary[typeof(InvalidUMAutoAttendantException).FullName] = (() => new InvalidUMAutoAttendantException());
			dictionary[typeof(AudioDataIsOversizeException).FullName] = (() => new AudioDataIsOversizeException(5, 5L));
			dictionary[typeof(SourceFileNotFoundException).FullName] = (() => new SourceFileNotFoundException(string.Empty));
			dictionary[typeof(DeleteContentException).FullName] = (() => new DeleteContentException(string.Empty));
			dictionary[typeof(PublishingPointException).FullName] = (() => new PublishingPointException(string.Empty));
			dictionary[typeof(EWSUMMailboxAccessException).FullName] = (() => new EWSUMMailboxAccessException(string.Empty));
			return dictionary;
		}

		private static Dictionary<string, ErrorResponse.CreateExceptionDelegate> errorTable = ErrorResponse.CreateErrorTable();

		private string errorType;

		private delegate Exception CreateExceptionDelegate();
	}
}
