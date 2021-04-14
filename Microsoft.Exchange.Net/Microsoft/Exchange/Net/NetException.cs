using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	internal static class NetException
	{
		static NetException()
		{
			NetException.stringIDs.Add(2888966445U, "NotFederated");
			NetException.stringIDs.Add(3146200830U, "InvalidMaxBufferCount");
			NetException.stringIDs.Add(2620764921U, "InvalidWmaFormat");
			NetException.stringIDs.Add(4272875403U, "SmallCapacity");
			NetException.stringIDs.Add(1883331241U, "LargeBuffer");
			NetException.stringIDs.Add(4266227918U, "BufferOverflow");
			NetException.stringIDs.Add(2543924840U, "ProcessRunnerTimeout");
			NetException.stringIDs.Add(774122735U, "DSSAndRSA");
			NetException.stringIDs.Add(222712050U, "AuthzUnableToDoAccessCheckFromNullOrInvalidHandle");
			NetException.stringIDs.Add(4239692632U, "EmptyServerList");
			NetException.stringIDs.Add(1300380106U, "IAsyncResultMismatch");
			NetException.stringIDs.Add(4130279271U, "UnknownPropertyType");
			NetException.stringIDs.Add(724957639U, "FindLineError");
			NetException.stringIDs.Add(1939069342U, "InvalidDateValue");
			NetException.stringIDs.Add(2078991984U, "LargeParameter");
			NetException.stringIDs.Add(2642084527U, "AuthzIdentityNotSet");
			NetException.stringIDs.Add(335518370U, "AuthManagerNotInitialized");
			NetException.stringIDs.Add(3109722150U, "ApplicationUriMissing");
			NetException.stringIDs.Add(3853787433U, "CorruptStringSize");
			NetException.stringIDs.Add(523155784U, "DuplicateItem");
			NetException.stringIDs.Add(3497416644U, "BufferMismatch");
			NetException.stringIDs.Add(282247621U, "CannotHandleUnsecuredRedirection");
			NetException.stringIDs.Add(2788838128U, "EndAlreadyCalled");
			NetException.stringIDs.Add(1998366202U, "CollectionChanged");
			NetException.stringIDs.Add(3363535402U, "BadOffset");
			NetException.stringIDs.Add(1479752888U, "AlreadyInitialized");
			NetException.stringIDs.Add(4293010739U, "SignatureDoesNotMatch");
			NetException.stringIDs.Add(1000630245U, "NoTokenContext");
			NetException.stringIDs.Add(574942079U, "InvalidUnicodeString");
			NetException.stringIDs.Add(2031043979U, "DomainsMissing");
			NetException.stringIDs.Add(4205481756U, "OnlySSLSupported");
			NetException.stringIDs.Add(1391971810U, "StoreTypeUnsupported");
			NetException.stringIDs.Add(1787132391U, "MultipleOfAlignmentFactor");
			NetException.stringIDs.Add(3739156314U, "GetUserSettingsGeneralFailure");
			NetException.stringIDs.Add(3669818989U, "NoResponseFromHttpServer");
			NetException.stringIDs.Add(4039752388U, "NoContext");
			NetException.stringIDs.Add(1520602905U, "NegativeIndex");
			NetException.stringIDs.Add(3658473395U, "SmallBuffer");
			NetException.stringIDs.Add(2711616649U, "ReceiveInProgress");
			NetException.stringIDs.Add(994785501U, "NotInitialized");
			NetException.stringIDs.Add(2115127879U, "InvalidDuration");
			NetException.stringIDs.Add(1323030597U, "EmptyCertSubject");
			NetException.stringIDs.Add(3037031059U, "UnknownAuthMechanism");
			NetException.stringIDs.Add(2092010860U, "SeekOrigin");
			NetException.stringIDs.Add(2702236218U, "aadTransportFailureException");
			NetException.stringIDs.Add(1228304272U, "EmptyFQDNList");
			NetException.stringIDs.Add(2649873638U, "ResolveInProgress");
			NetException.stringIDs.Add(4160234858U, "EmptyCertThumbprint");
			NetException.stringIDs.Add(3484386229U, "AuthzInitializeContextFromDuplicateAuthZFailed");
			NetException.stringIDs.Add(1344176929U, "StringContainsInvalidCharacters");
			NetException.stringIDs.Add(1018641786U, "UnexpectedUserResponses");
			NetException.stringIDs.Add(2422771817U, "LogonData");
			NetException.stringIDs.Add(1024372311U, "AuthFailureException");
			NetException.stringIDs.Add(2234054046U, "ImmutableStream");
			NetException.stringIDs.Add(3706950651U, "NegativeCapacity");
			NetException.stringIDs.Add(3076215569U, "UnsupportedFilter");
			NetException.stringIDs.Add(1610756644U, "InvalidIPType");
			NetException.stringIDs.Add(3156409200U, "TlsProtocolFailureException");
			NetException.stringIDs.Add(1791693013U, "CorruptArraySize");
			NetException.stringIDs.Add(1612735551U, "AuthzGetInformationFromContextReturnedSuccessForSize");
			NetException.stringIDs.Add(2043762708U, "CapacityOverflow");
			NetException.stringIDs.Add(1871965737U, "ExportAndArchive");
			NetException.stringIDs.Add(1441393770U, "InternalOperationFailure");
			NetException.stringIDs.Add(3175999590U, "MissingNullTerminator");
			NetException.stringIDs.Add(2134260535U, "DSSNotSupported");
			NetException.stringIDs.Add(2821016632U, "InvalidNumberOfBytes");
			NetException.stringIDs.Add(2396394977U, "CouldNotAllocateFragment");
			NetException.stringIDs.Add(565660828U, "OutOfRange");
			NetException.stringIDs.Add(115723314U, "ResponseMissingErrorCode");
			NetException.stringIDs.Add(1151742595U, "IllegalContainedAccess");
			NetException.stringIDs.Add(2305890504U, "UseReportEncryptedBytesFilled");
			NetException.stringIDs.Add(4177743836U, "SendInProgress");
			NetException.stringIDs.Add(976065123U, "LargeIndex");
			NetException.stringIDs.Add(1274073424U, "InvalidSize");
			NetException.stringIDs.Add(1896894556U, "EmptyResponse");
			NetException.stringIDs.Add(1753773471U, "CollectionEmpty");
			NetException.stringIDs.Add(2455122056U, "NegativeParameter");
			NetException.stringIDs.Add(907185727U, "MustBeTlsForAuthException");
			NetException.stringIDs.Add(1927799671U, "WrongType");
			NetException.stringIDs.Add(1586250006U, "DataContainsBareLinefeeds");
			NetException.stringIDs.Add(1945685814U, "ClosedStream");
			NetException.stringIDs.Add(2499062057U, "MissingPrimaryGroupSid");
			NetException.stringIDs.Add(1163610383U, "ReadPastEnd");
			NetException.stringIDs.Add(1890952107U, "TlsAlreadyNegotiated");
			NetException.stringIDs.Add(2126643456U, "DestinationIndexOutOfRange");
			NetException.stringIDs.Add(3093826520U, "TruncatedData");
		}

		public static LocalizedString NotFederated
		{
			get
			{
				return new LocalizedString("NotFederated", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMaxBufferCount
		{
			get
			{
				return new LocalizedString("InvalidMaxBufferCount", "ExBFC10C", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthzUnableToGetTokenFromNullOrInvalidHandle(string clientContext)
		{
			return new LocalizedString("AuthzUnableToGetTokenFromNullOrInvalidHandle", "", false, false, NetException.ResourceManager, new object[]
			{
				clientContext
			});
		}

		public static LocalizedString InvalidWmaFormat
		{
			get
			{
				return new LocalizedString("InvalidWmaFormat", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmallCapacity
		{
			get
			{
				return new LocalizedString("SmallCapacity", "ExA4724A", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsApiFailureException(string error)
		{
			return new LocalizedString("TlsApiFailureException", "", false, false, NetException.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString LargeBuffer
		{
			get
			{
				return new LocalizedString("LargeBuffer", "ExCA1CF4", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BufferOverflow
		{
			get
			{
				return new LocalizedString("BufferOverflow", "ExB97FC1", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CertificateSubjectNotFound(string name)
		{
			return new LocalizedString("CertificateSubjectNotFound", "", false, false, NetException.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ProcessRunnerTimeout
		{
			get
			{
				return new LocalizedString("ProcessRunnerTimeout", "ExEBFF7D", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSSAndRSA
		{
			get
			{
				return new LocalizedString("DSSAndRSA", "Ex72FA3A", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthzUnableToDoAccessCheckFromNullOrInvalidHandle
		{
			get
			{
				return new LocalizedString("AuthzUnableToDoAccessCheckFromNullOrInvalidHandle", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmptyServerList
		{
			get
			{
				return new LocalizedString("EmptyServerList", "Ex295FBA", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IAsyncResultMismatch
		{
			get
			{
				return new LocalizedString("IAsyncResultMismatch", "Ex38C37F", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownPropertyType
		{
			get
			{
				return new LocalizedString("UnknownPropertyType", "ExB8CF30", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FindLineError
		{
			get
			{
				return new LocalizedString("FindLineError", "ExE043FA", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSmtpServerResponseException(string response)
		{
			return new LocalizedString("InvalidSmtpServerResponseException", "", false, false, NetException.ResourceManager, new object[]
			{
				response
			});
		}

		public static LocalizedString UnmanagedAlloc(int size)
		{
			return new LocalizedString("UnmanagedAlloc", "ExF11D00", false, true, NetException.ResourceManager, new object[]
			{
				size
			});
		}

		public static LocalizedString InvalidDateValue
		{
			get
			{
				return new LocalizedString("InvalidDateValue", "ExADE97C", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidWaveFormat(string s)
		{
			return new LocalizedString("InvalidWaveFormat", "", false, false, NetException.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString DomainNotPresentInResponse(string domain, string domainsFromResponse)
		{
			return new LocalizedString("DomainNotPresentInResponse", "", false, false, NetException.ResourceManager, new object[]
			{
				domain,
				domainsFromResponse
			});
		}

		public static LocalizedString LargeParameter
		{
			get
			{
				return new LocalizedString("LargeParameter", "Ex5EFE8D", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedAutodiscoverResult(string result)
		{
			return new LocalizedString("UnexpectedAutodiscoverResult", "", false, false, NetException.ResourceManager, new object[]
			{
				result
			});
		}

		public static LocalizedString AuthzUnableToPerformAccessCheck(string clientContext)
		{
			return new LocalizedString("AuthzUnableToPerformAccessCheck", "", false, false, NetException.ResourceManager, new object[]
			{
				clientContext
			});
		}

		public static LocalizedString AuthzIdentityNotSet
		{
			get
			{
				return new LocalizedString("AuthzIdentityNotSet", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthManagerNotInitialized
		{
			get
			{
				return new LocalizedString("AuthManagerNotInitialized", "Ex2FF5B8", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApplicationUriMissing
		{
			get
			{
				return new LocalizedString("ApplicationUriMissing", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CorruptStringSize
		{
			get
			{
				return new LocalizedString("CorruptStringSize", "Ex3C16B8", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicateItem
		{
			get
			{
				return new LocalizedString("DuplicateItem", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MalformedLocationHeader(string locationHeader)
		{
			return new LocalizedString("MalformedLocationHeader", "", false, false, NetException.ResourceManager, new object[]
			{
				locationHeader
			});
		}

		public static LocalizedString UnexpectedSmtpServerResponseException(int expectedResponseCode, int actualResponseCode, string wholeResponse)
		{
			return new LocalizedString("UnexpectedSmtpServerResponseException", "", false, false, NetException.ResourceManager, new object[]
			{
				expectedResponseCode,
				actualResponseCode,
				wholeResponse
			});
		}

		public static LocalizedString FailedToConnectToSMTPServerException(string server)
		{
			return new LocalizedString("FailedToConnectToSMTPServerException", "", false, false, NetException.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString BufferMismatch
		{
			get
			{
				return new LocalizedString("BufferMismatch", "ExBFABE7", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogonAsNetworkServiceFailed(string error)
		{
			return new LocalizedString("LogonAsNetworkServiceFailed", "", false, false, NetException.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString CannotHandleUnsecuredRedirection
		{
			get
			{
				return new LocalizedString("CannotHandleUnsecuredRedirection", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EndAlreadyCalled
		{
			get
			{
				return new LocalizedString("EndAlreadyCalled", "ExF0F933", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CollectionChanged
		{
			get
			{
				return new LocalizedString("CollectionChanged", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSid(string invalidSid)
		{
			return new LocalizedString("InvalidSid", "ExC35A6F", false, true, NetException.ResourceManager, new object[]
			{
				invalidSid
			});
		}

		public static LocalizedString UnsupportedAudioFormat(string fileName)
		{
			return new LocalizedString("UnsupportedAudioFormat", "", false, false, NetException.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString BadOffset
		{
			get
			{
				return new LocalizedString("BadOffset", "Ex78B5D0", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlreadyInitialized
		{
			get
			{
				return new LocalizedString("AlreadyInitialized", "Ex757617", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SignatureDoesNotMatch
		{
			get
			{
				return new LocalizedString("SignatureDoesNotMatch", "Ex01B475", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoTokenContext
		{
			get
			{
				return new LocalizedString("NoTokenContext", "Ex41300D", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidUnicodeString
		{
			get
			{
				return new LocalizedString("InvalidUnicodeString", "ExEA88A8", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainsMissing
		{
			get
			{
				return new LocalizedString("DomainsMissing", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AudioConversionFailed(string reason)
		{
			return new LocalizedString("AudioConversionFailed", "", false, false, NetException.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString OnlySSLSupported
		{
			get
			{
				return new LocalizedString("OnlySSLSupported", "Ex96B89E", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotConnectedToSMTPServerException(string server)
		{
			return new LocalizedString("NotConnectedToSMTPServerException", "", false, false, NetException.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString StoreTypeUnsupported
		{
			get
			{
				return new LocalizedString("StoreTypeUnsupported", "ExE9E5F7", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MultipleOfAlignmentFactor
		{
			get
			{
				return new LocalizedString("MultipleOfAlignmentFactor", "ExD313AE", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetUserSettingsGeneralFailure
		{
			get
			{
				return new LocalizedString("GetUserSettingsGeneralFailure", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoResponseFromHttpServer
		{
			get
			{
				return new LocalizedString("NoResponseFromHttpServer", "Ex61FC70", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoContext
		{
			get
			{
				return new LocalizedString("NoContext", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NegativeIndex
		{
			get
			{
				return new LocalizedString("NegativeIndex", "ExB4DA6B", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmallBuffer
		{
			get
			{
				return new LocalizedString("SmallBuffer", "Ex69366B", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReceiveInProgress
		{
			get
			{
				return new LocalizedString("ReceiveInProgress", "ExFF7646", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthzInitializeContextFromTokenFailed(string clientContext)
		{
			return new LocalizedString("AuthzInitializeContextFromTokenFailed", "", false, false, NetException.ResourceManager, new object[]
			{
				clientContext
			});
		}

		public static LocalizedString NotInitialized
		{
			get
			{
				return new LocalizedString("NotInitialized", "ExC4DE2C", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDuration
		{
			get
			{
				return new LocalizedString("InvalidDuration", "Ex883DF3", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidFlags(int flags)
		{
			return new LocalizedString("InvalidFlags", "Ex43D52A", false, true, NetException.ResourceManager, new object[]
			{
				flags
			});
		}

		public static LocalizedString AuthzGetInformationFromContextFailed(string clientContext)
		{
			return new LocalizedString("AuthzGetInformationFromContextFailed", "", false, false, NetException.ResourceManager, new object[]
			{
				clientContext
			});
		}

		public static LocalizedString AuthApiFailureException(string error)
		{
			return new LocalizedString("AuthApiFailureException", "", false, false, NetException.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString EmptyCertSubject
		{
			get
			{
				return new LocalizedString("EmptyCertSubject", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthzInitializeContextFromSidFailed(string clientContext)
		{
			return new LocalizedString("AuthzInitializeContextFromSidFailed", "", false, false, NetException.ResourceManager, new object[]
			{
				clientContext
			});
		}

		public static LocalizedString UnknownAuthMechanism
		{
			get
			{
				return new LocalizedString("UnknownAuthMechanism", "ExB2EA4B", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeekOrigin
		{
			get
			{
				return new LocalizedString("SeekOrigin", "Ex83058E", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString aadTransportFailureException
		{
			get
			{
				return new LocalizedString("aadTransportFailureException", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmptyFQDNList
		{
			get
			{
				return new LocalizedString("EmptyFQDNList", "Ex995D96", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResolveInProgress
		{
			get
			{
				return new LocalizedString("ResolveInProgress", "Ex9C21C2", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmptyCertThumbprint
		{
			get
			{
				return new LocalizedString("EmptyCertThumbprint", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthzInitializeContextFromDuplicateAuthZFailed
		{
			get
			{
				return new LocalizedString("AuthzInitializeContextFromDuplicateAuthZFailed", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StringContainsInvalidCharacters
		{
			get
			{
				return new LocalizedString("StringContainsInvalidCharacters", "ExD765BA", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedUserResponses
		{
			get
			{
				return new LocalizedString("UnexpectedUserResponses", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedStatusCode(string statusCode)
		{
			return new LocalizedString("UnexpectedStatusCode", "", false, false, NetException.ResourceManager, new object[]
			{
				statusCode
			});
		}

		public static LocalizedString LogonData
		{
			get
			{
				return new LocalizedString("LogonData", "ExBF27A7", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthFailureException
		{
			get
			{
				return new LocalizedString("AuthFailureException", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImmutableStream
		{
			get
			{
				return new LocalizedString("ImmutableStream", "Ex545DFA", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NegativeCapacity
		{
			get
			{
				return new LocalizedString("NegativeCapacity", "Ex28A2C5", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedFilter
		{
			get
			{
				return new LocalizedString("UnsupportedFilter", "Ex0D5214", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidIPType
		{
			get
			{
				return new LocalizedString("InvalidIPType", "Ex261EBD", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsProtocolFailureException
		{
			get
			{
				return new LocalizedString("TlsProtocolFailureException", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CorruptArraySize
		{
			get
			{
				return new LocalizedString("CorruptArraySize", "ExDE1375", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthzGetInformationFromContextReturnedSuccessForSize
		{
			get
			{
				return new LocalizedString("AuthzGetInformationFromContextReturnedSuccessForSize", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CapacityOverflow
		{
			get
			{
				return new LocalizedString("CapacityOverflow", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExportAndArchive
		{
			get
			{
				return new LocalizedString("ExportAndArchive", "Ex54F1AE", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedResponseType(string responseType)
		{
			return new LocalizedString("UnexpectedResponseType", "", false, false, NetException.ResourceManager, new object[]
			{
				responseType
			});
		}

		public static LocalizedString InternalOperationFailure
		{
			get
			{
				return new LocalizedString("InternalOperationFailure", "ExD62529", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingNullTerminator
		{
			get
			{
				return new LocalizedString("MissingNullTerminator", "ExA381A7", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSSNotSupported
		{
			get
			{
				return new LocalizedString("DSSNotSupported", "Ex610A51", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CertificateThumbprintNotFound(string name)
		{
			return new LocalizedString("CertificateThumbprintNotFound", "", false, false, NetException.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InvalidNumberOfBytes
		{
			get
			{
				return new LocalizedString("InvalidNumberOfBytes", "Ex186A41", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CouldNotAllocateFragment
		{
			get
			{
				return new LocalizedString("CouldNotAllocateFragment", "ExC1DDAE", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutOfRange
		{
			get
			{
				return new LocalizedString("OutOfRange", "ExEA934B", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiscoveryFailed(string domain)
		{
			return new LocalizedString("DiscoveryFailed", "", false, false, NetException.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString UnsupportedAuthMechanismException(string authMechanism)
		{
			return new LocalizedString("UnsupportedAuthMechanismException", "", false, false, NetException.ResourceManager, new object[]
			{
				authMechanism
			});
		}

		public static LocalizedString InvalidUserForGetUserSettings(string user)
		{
			return new LocalizedString("InvalidUserForGetUserSettings", "", false, false, NetException.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString InvalidFQDN(string name)
		{
			return new LocalizedString("InvalidFQDN", "Ex260214", false, true, NetException.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString CertificateIssuerNotfound(string name)
		{
			return new LocalizedString("CertificateIssuerNotfound", "", false, false, NetException.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString AlreadyConnectedToSMTPServerException(string server)
		{
			return new LocalizedString("AlreadyConnectedToSMTPServerException", "", false, false, NetException.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ResponseMissingErrorCode
		{
			get
			{
				return new LocalizedString("ResponseMissingErrorCode", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IllegalContainedAccess
		{
			get
			{
				return new LocalizedString("IllegalContainedAccess", "ExF09B3E", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInResponse(string errorCode)
		{
			return new LocalizedString("ErrorInResponse", "", false, false, NetException.ResourceManager, new object[]
			{
				errorCode
			});
		}

		public static LocalizedString UseReportEncryptedBytesFilled
		{
			get
			{
				return new LocalizedString("UseReportEncryptedBytesFilled", "ExA6BD4C", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendInProgress
		{
			get
			{
				return new LocalizedString("SendInProgress", "ExC9F7D4", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LargeIndex
		{
			get
			{
				return new LocalizedString("LargeIndex", "ExA3051C", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSize
		{
			get
			{
				return new LocalizedString("InvalidSize", "ExC7703B", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmptyResponse
		{
			get
			{
				return new LocalizedString("EmptyResponse", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WildcardNotSupported(string name)
		{
			return new LocalizedString("WildcardNotSupported", "Ex2F5706", false, true, NetException.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString NonHttpsLocationHeader(string locationHeader)
		{
			return new LocalizedString("NonHttpsLocationHeader", "", false, false, NetException.ResourceManager, new object[]
			{
				locationHeader
			});
		}

		public static LocalizedString CollectionEmpty
		{
			get
			{
				return new LocalizedString("CollectionEmpty", "Ex76100C", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NegativeParameter
		{
			get
			{
				return new LocalizedString("NegativeParameter", "ExAC35B0", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MustBeTlsForAuthException
		{
			get
			{
				return new LocalizedString("MustBeTlsForAuthException", "", false, false, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedPathLocationHeader(string locationHeader)
		{
			return new LocalizedString("UnexpectedPathLocationHeader", "", false, false, NetException.ResourceManager, new object[]
			{
				locationHeader
			});
		}

		public static LocalizedString WrongType
		{
			get
			{
				return new LocalizedString("WrongType", "Ex05E06B", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApplicationUriMalformed(string applicaitonUri)
		{
			return new LocalizedString("ApplicationUriMalformed", "", false, false, NetException.ResourceManager, new object[]
			{
				applicaitonUri
			});
		}

		public static LocalizedString DataContainsBareLinefeeds
		{
			get
			{
				return new LocalizedString("DataContainsBareLinefeeds", "ExC3EF57", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthzAddSidsToContextFailed(string clientContext)
		{
			return new LocalizedString("AuthzAddSidsToContextFailed", "", false, false, NetException.ResourceManager, new object[]
			{
				clientContext
			});
		}

		public static LocalizedString ClosedStream
		{
			get
			{
				return new LocalizedString("ClosedStream", "Ex1EDA64", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingPrimaryGroupSid
		{
			get
			{
				return new LocalizedString("MissingPrimaryGroupSid", "ExF7732F", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadPastEnd
		{
			get
			{
				return new LocalizedString("ReadPastEnd", "Ex17A55D", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsAlreadyNegotiated
		{
			get
			{
				return new LocalizedString("TlsAlreadyNegotiated", "Ex769CCF", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DestinationIndexOutOfRange
		{
			get
			{
				return new LocalizedString("DestinationIndexOutOfRange", "Ex24A377", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TruncatedData
		{
			get
			{
				return new LocalizedString("TruncatedData", "Ex613FB4", false, true, NetException.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(NetException.IDs key)
		{
			return new LocalizedString(NetException.stringIDs[(uint)key], NetException.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(86);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Net.NetException", typeof(NetException).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			NotFederated = 2888966445U,
			InvalidMaxBufferCount = 3146200830U,
			InvalidWmaFormat = 2620764921U,
			SmallCapacity = 4272875403U,
			LargeBuffer = 1883331241U,
			BufferOverflow = 4266227918U,
			ProcessRunnerTimeout = 2543924840U,
			DSSAndRSA = 774122735U,
			AuthzUnableToDoAccessCheckFromNullOrInvalidHandle = 222712050U,
			EmptyServerList = 4239692632U,
			IAsyncResultMismatch = 1300380106U,
			UnknownPropertyType = 4130279271U,
			FindLineError = 724957639U,
			InvalidDateValue = 1939069342U,
			LargeParameter = 2078991984U,
			AuthzIdentityNotSet = 2642084527U,
			AuthManagerNotInitialized = 335518370U,
			ApplicationUriMissing = 3109722150U,
			CorruptStringSize = 3853787433U,
			DuplicateItem = 523155784U,
			BufferMismatch = 3497416644U,
			CannotHandleUnsecuredRedirection = 282247621U,
			EndAlreadyCalled = 2788838128U,
			CollectionChanged = 1998366202U,
			BadOffset = 3363535402U,
			AlreadyInitialized = 1479752888U,
			SignatureDoesNotMatch = 4293010739U,
			NoTokenContext = 1000630245U,
			InvalidUnicodeString = 574942079U,
			DomainsMissing = 2031043979U,
			OnlySSLSupported = 4205481756U,
			StoreTypeUnsupported = 1391971810U,
			MultipleOfAlignmentFactor = 1787132391U,
			GetUserSettingsGeneralFailure = 3739156314U,
			NoResponseFromHttpServer = 3669818989U,
			NoContext = 4039752388U,
			NegativeIndex = 1520602905U,
			SmallBuffer = 3658473395U,
			ReceiveInProgress = 2711616649U,
			NotInitialized = 994785501U,
			InvalidDuration = 2115127879U,
			EmptyCertSubject = 1323030597U,
			UnknownAuthMechanism = 3037031059U,
			SeekOrigin = 2092010860U,
			aadTransportFailureException = 2702236218U,
			EmptyFQDNList = 1228304272U,
			ResolveInProgress = 2649873638U,
			EmptyCertThumbprint = 4160234858U,
			AuthzInitializeContextFromDuplicateAuthZFailed = 3484386229U,
			StringContainsInvalidCharacters = 1344176929U,
			UnexpectedUserResponses = 1018641786U,
			LogonData = 2422771817U,
			AuthFailureException = 1024372311U,
			ImmutableStream = 2234054046U,
			NegativeCapacity = 3706950651U,
			UnsupportedFilter = 3076215569U,
			InvalidIPType = 1610756644U,
			TlsProtocolFailureException = 3156409200U,
			CorruptArraySize = 1791693013U,
			AuthzGetInformationFromContextReturnedSuccessForSize = 1612735551U,
			CapacityOverflow = 2043762708U,
			ExportAndArchive = 1871965737U,
			InternalOperationFailure = 1441393770U,
			MissingNullTerminator = 3175999590U,
			DSSNotSupported = 2134260535U,
			InvalidNumberOfBytes = 2821016632U,
			CouldNotAllocateFragment = 2396394977U,
			OutOfRange = 565660828U,
			ResponseMissingErrorCode = 115723314U,
			IllegalContainedAccess = 1151742595U,
			UseReportEncryptedBytesFilled = 2305890504U,
			SendInProgress = 4177743836U,
			LargeIndex = 976065123U,
			InvalidSize = 1274073424U,
			EmptyResponse = 1896894556U,
			CollectionEmpty = 1753773471U,
			NegativeParameter = 2455122056U,
			MustBeTlsForAuthException = 907185727U,
			WrongType = 1927799671U,
			DataContainsBareLinefeeds = 1586250006U,
			ClosedStream = 1945685814U,
			MissingPrimaryGroupSid = 2499062057U,
			ReadPastEnd = 1163610383U,
			TlsAlreadyNegotiated = 1890952107U,
			DestinationIndexOutOfRange = 2126643456U,
			TruncatedData = 3093826520U
		}

		private enum ParamIDs
		{
			AuthzUnableToGetTokenFromNullOrInvalidHandle,
			TlsApiFailureException,
			CertificateSubjectNotFound,
			InvalidSmtpServerResponseException,
			UnmanagedAlloc,
			InvalidWaveFormat,
			DomainNotPresentInResponse,
			UnexpectedAutodiscoverResult,
			AuthzUnableToPerformAccessCheck,
			MalformedLocationHeader,
			UnexpectedSmtpServerResponseException,
			FailedToConnectToSMTPServerException,
			LogonAsNetworkServiceFailed,
			InvalidSid,
			UnsupportedAudioFormat,
			AudioConversionFailed,
			NotConnectedToSMTPServerException,
			AuthzInitializeContextFromTokenFailed,
			InvalidFlags,
			AuthzGetInformationFromContextFailed,
			AuthApiFailureException,
			AuthzInitializeContextFromSidFailed,
			UnexpectedStatusCode,
			UnexpectedResponseType,
			CertificateThumbprintNotFound,
			DiscoveryFailed,
			UnsupportedAuthMechanismException,
			InvalidUserForGetUserSettings,
			InvalidFQDN,
			CertificateIssuerNotfound,
			AlreadyConnectedToSMTPServerException,
			ErrorInResponse,
			WildcardNotSupported,
			NonHttpsLocationHeader,
			UnexpectedPathLocationHeader,
			ApplicationUriMalformed,
			AuthzAddSidsToContextFailed
		}
	}
}
