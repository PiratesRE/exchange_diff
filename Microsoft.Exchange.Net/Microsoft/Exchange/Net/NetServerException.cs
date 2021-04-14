using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net.LinkedIn;

namespace Microsoft.Exchange.Net
{
	internal static class NetServerException
	{
		static NetServerException()
		{
			NetServerException.stringIDs.Add(757455353U, "LinkedInRequestCookiesMissingOAuthSecret");
			NetServerException.stringIDs.Add(1078344448U, "InvalidMserveRequest");
			NetServerException.stringIDs.Add(703035395U, "EmptyAppAuthenticationResponse");
			NetServerException.stringIDs.Add(1478858759U, "EmptyRedirectUrlAuthenticationResponse");
			NetServerException.stringIDs.Add(1425604752U, "LinkedInQueryStringMissingOAuthVerifier");
			NetServerException.stringIDs.Add(3926725349U, "LinkedInQueryStringMissingOAuthToken");
		}

		public static LocalizedString InvalidOperationError(string serviceURI, LocalizedString exceptionMessage)
		{
			return new LocalizedString("InvalidOperationError", NetServerException.ResourceManager, new object[]
			{
				serviceURI,
				exceptionMessage
			});
		}

		public static LocalizedString LinkedInFailedToAuthenticateApp(LinkedInConfig configuration)
		{
			return new LocalizedString("LinkedInFailedToAuthenticateApp", NetServerException.ResourceManager, new object[]
			{
				configuration
			});
		}

		public static LocalizedString AppAuthenticationResponseTooLarge(long length)
		{
			return new LocalizedString("AppAuthenticationResponseTooLarge", NetServerException.ResourceManager, new object[]
			{
				length
			});
		}

		public static LocalizedString LinkedInRequestCookiesMissingOAuthSecret
		{
			get
			{
				return new LocalizedString("LinkedInRequestCookiesMissingOAuthSecret", NetServerException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMserveRequest
		{
			get
			{
				return new LocalizedString("InvalidMserveRequest", NetServerException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedInInvalidOAuthResponseMissingToken(string response)
		{
			return new LocalizedString("LinkedInInvalidOAuthResponseMissingToken", NetServerException.ResourceManager, new object[]
			{
				response
			});
		}

		public static LocalizedString InvalidAppAuthenticationResponse(string response, FacebookAuthenticatorConfig configuration)
		{
			return new LocalizedString("InvalidAppAuthenticationResponse", NetServerException.ResourceManager, new object[]
			{
				response,
				configuration
			});
		}

		public static LocalizedString EmptyAppAuthenticationResponse
		{
			get
			{
				return new LocalizedString("EmptyAppAuthenticationResponse", NetServerException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MserveCacheEndpointNotFound(string endpoint, string error)
		{
			return new LocalizedString("MserveCacheEndpointNotFound", NetServerException.ResourceManager, new object[]
			{
				endpoint,
				error
			});
		}

		public static LocalizedString EmptyRedirectUrlAuthenticationResponse
		{
			get
			{
				return new LocalizedString("EmptyRedirectUrlAuthenticationResponse", NetServerException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MserveCacheTimeoutError(string error)
		{
			return new LocalizedString("MserveCacheTimeoutError", NetServerException.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ExceptionWithStack(LocalizedString exceptionMessage, string stackTrace)
		{
			return new LocalizedString("ExceptionWithStack", NetServerException.ResourceManager, new object[]
			{
				exceptionMessage,
				stackTrace
			});
		}

		public static LocalizedString NestedExceptionMsg(LocalizedString message, LocalizedString innerMessage)
		{
			return new LocalizedString("NestedExceptionMsg", NetServerException.ResourceManager, new object[]
			{
				message,
				innerMessage
			});
		}

		public static LocalizedString LinkedInQueryStringMissingOAuthVerifier
		{
			get
			{
				return new LocalizedString("LinkedInQueryStringMissingOAuthVerifier", NetServerException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedAppAuthenticationResponse(HttpStatusCode code, string body, FacebookAuthenticatorConfig configuration)
		{
			return new LocalizedString("UnexpectedAppAuthenticationResponse", NetServerException.ResourceManager, new object[]
			{
				code,
				body,
				configuration
			});
		}

		public static LocalizedString InvalidDataError(string serviceURI, LocalizedString exceptionMessage)
		{
			return new LocalizedString("InvalidDataError", NetServerException.ResourceManager, new object[]
			{
				serviceURI,
				exceptionMessage
			});
		}

		public static LocalizedString EndpointNotFoundError(string serviceURI, LocalizedString exceptionMessage)
		{
			return new LocalizedString("EndpointNotFoundError", NetServerException.ResourceManager, new object[]
			{
				serviceURI,
				exceptionMessage
			});
		}

		public static LocalizedString QuotaExceededError(string serviceURI, LocalizedString exceptionMessage)
		{
			return new LocalizedString("QuotaExceededError", NetServerException.ResourceManager, new object[]
			{
				serviceURI,
				exceptionMessage
			});
		}

		public static LocalizedString LinkedInQueryStringMissingOAuthToken
		{
			get
			{
				return new LocalizedString("LinkedInQueryStringMissingOAuthToken", NetServerException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedInInvalidOAuthResponseMissingTokenSecret(string response)
		{
			return new LocalizedString("LinkedInInvalidOAuthResponseMissingTokenSecret", NetServerException.ResourceManager, new object[]
			{
				response
			});
		}

		public static LocalizedString CommunicationError(string serviceURI, LocalizedString exceptionMessage)
		{
			return new LocalizedString("CommunicationError", NetServerException.ResourceManager, new object[]
			{
				serviceURI,
				exceptionMessage
			});
		}

		public static LocalizedString PermanentMserveCacheError(string error)
		{
			return new LocalizedString("PermanentMserveCacheError", NetServerException.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString LinkedInUnexpectedAppAuthenticationResponse(HttpStatusCode code, string body, LinkedInConfig configuration)
		{
			return new LocalizedString("LinkedInUnexpectedAppAuthenticationResponse", NetServerException.ResourceManager, new object[]
			{
				code,
				body,
				configuration
			});
		}

		public static LocalizedString InvalidAppAuthorizationCode(string errorMessage, FacebookAuthenticatorConfig configuration)
		{
			return new LocalizedString("InvalidAppAuthorizationCode", NetServerException.ResourceManager, new object[]
			{
				errorMessage,
				configuration
			});
		}

		public static LocalizedString FailedToAuthenticateApp(FacebookAuthenticatorConfig configuration)
		{
			return new LocalizedString("FailedToAuthenticateApp", NetServerException.ResourceManager, new object[]
			{
				configuration
			});
		}

		public static LocalizedString TransientMserveCacheError(string error)
		{
			return new LocalizedString("TransientMserveCacheError", NetServerException.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString UnexpectedCharSetInAppAuthenticationResponse(string charset)
		{
			return new LocalizedString("UnexpectedCharSetInAppAuthenticationResponse", NetServerException.ResourceManager, new object[]
			{
				charset
			});
		}

		public static LocalizedString TimeoutError(string serviceURI, LocalizedString exceptionMessage)
		{
			return new LocalizedString("TimeoutError", NetServerException.ResourceManager, new object[]
			{
				serviceURI,
				exceptionMessage
			});
		}

		public static LocalizedString ProcessRunnerToStringArgumentOutOfRangeException(string variableName, string errorMessage, string executableName, int exitCode)
		{
			return new LocalizedString("ProcessRunnerToStringArgumentOutOfRangeException", NetServerException.ResourceManager, new object[]
			{
				variableName,
				errorMessage,
				executableName,
				exitCode
			});
		}

		public static LocalizedString GetLocalizedString(NetServerException.IDs key)
		{
			return new LocalizedString(NetServerException.stringIDs[(uint)key], NetServerException.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(6);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Net.NetServerException", typeof(NetServerException).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			LinkedInRequestCookiesMissingOAuthSecret = 757455353U,
			InvalidMserveRequest = 1078344448U,
			EmptyAppAuthenticationResponse = 703035395U,
			EmptyRedirectUrlAuthenticationResponse = 1478858759U,
			LinkedInQueryStringMissingOAuthVerifier = 1425604752U,
			LinkedInQueryStringMissingOAuthToken = 3926725349U
		}

		private enum ParamIDs
		{
			InvalidOperationError,
			LinkedInFailedToAuthenticateApp,
			AppAuthenticationResponseTooLarge,
			LinkedInInvalidOAuthResponseMissingToken,
			InvalidAppAuthenticationResponse,
			MserveCacheEndpointNotFound,
			MserveCacheTimeoutError,
			ExceptionWithStack,
			NestedExceptionMsg,
			UnexpectedAppAuthenticationResponse,
			InvalidDataError,
			EndpointNotFoundError,
			QuotaExceededError,
			LinkedInInvalidOAuthResponseMissingTokenSecret,
			CommunicationError,
			PermanentMserveCacheError,
			LinkedInUnexpectedAppAuthenticationResponse,
			InvalidAppAuthorizationCode,
			FailedToAuthenticateApp,
			TransientMserveCacheError,
			UnexpectedCharSetInAppAuthenticationResponse,
			TimeoutError,
			ProcessRunnerToStringArgumentOutOfRangeException
		}
	}
}
