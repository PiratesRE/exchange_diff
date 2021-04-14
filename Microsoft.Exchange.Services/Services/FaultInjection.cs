using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Mapi;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.Services
{
	internal class FaultInjection
	{
		public static void GenerateFault(FaultInjection.LIDs faultLid)
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest((uint)faultLid);
		}

		public static T TraceTest<T>(FaultInjection.LIDs faultLid)
		{
			T result = default(T);
			ExTraceGlobals.FaultInjectionTracer.TraceTest<T>((uint)faultLid, ref result);
			return result;
		}

		public static void FaultInjectionPoint(FaultInjection.LIDs faultLid, Action productAction, Action faultInjectionAction)
		{
			if (FaultInjection.TraceTest<bool>(faultLid))
			{
				faultInjectionAction();
				return;
			}
			productAction();
		}

		public static Exception Callback(string exceptionType)
		{
			Exception result = null;
			if (exceptionType != null && exceptionType != null)
			{
				if (<PrivateImplementationDetails>{07C235D2-EA05-4020-8C99-D4258F03250B}.$$method0x600019e-1 == null)
				{
					<PrivateImplementationDetails>{07C235D2-EA05-4020-8C99-D4258F03250B}.$$method0x600019e-1 = new Dictionary<string, int>(28)
					{
						{
							"Microsoft.Exchange.Data.Storage.EventNotFoundException",
							0
						},
						{
							"Microsoft.Exchange.Data.Storage.ReadEventsFailedException",
							1
						},
						{
							"Microsoft.Exchange.Data.Storage.ReadEventsFailedTransientException",
							2
						},
						{
							"Microsoft.Exchange.Data.Directory.SuitabilityDirectoryException",
							3
						},
						{
							"Microsoft.Exchange.Data.Directory.ADTransientException",
							4
						},
						{
							"System.Net.WebException",
							5
						},
						{
							"Microsoft.Exchange.Data.Directory.ADPossibleOperationException",
							6
						},
						{
							"Microsoft.Exchange.Data.Storage.StorageTransientException",
							7
						},
						{
							"Microsoft.Exchange.Data.Storage.MailboxInSiteFailoverException",
							8
						},
						{
							"Microsoft.Exchange.Data.Directory.SystemConfiguration.OverBudgetException",
							9
						},
						{
							"Microsoft.Exchange.Data.Storage.AccessDeniedException",
							10
						},
						{
							"Microsoft.Exchange.Data.Storage.AccountDisabledException",
							11
						},
						{
							"Microsoft.Exchange.Data.Storage.ConnectionFailedPermanentException",
							12
						},
						{
							"Microsoft.Exchange.Data.Storage.CorruptDataException",
							13
						},
						{
							"Microsoft.Exchange.Data.Storage.ObjectNotFoundException",
							14
						},
						{
							"Microsoft.Exchange.Data.Storage.ServerNotFoundException",
							15
						},
						{
							"Microsoft.Exchange.Data.Storage.ServerNotInSiteException",
							16
						},
						{
							"Microsoft.Exchange.Data.Storage.FinalEventException",
							17
						},
						{
							"Microsoft.Mapi.MapiExceptionAmbiguousAlias",
							18
						},
						{
							"System.SystemException",
							19
						},
						{
							"System.IO.IOException",
							20
						},
						{
							"Microsoft.Mapi.MapiExceptionCorruptData",
							21
						},
						{
							"Microsoft.Exchange.Data.Storage.IllegalCrossServerConnectionException",
							22
						},
						{
							"Microsoft.Mapi.MapiExceptionShutoffQuotaExceeded",
							23
						},
						{
							"Microsoft.Mapi.MapiExceptionNoSupport",
							24
						},
						{
							"Microsoft.Exchange.Data.TextConverters.TextConvertersException",
							25
						},
						{
							"Microsoft.Mapi.MapiExceptionNoAccess",
							26
						},
						{
							"Microsoft.Mapi.MapiExceptionTimeout",
							27
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{07C235D2-EA05-4020-8C99-D4258F03250B}.$$method0x600019e-1.TryGetValue(exceptionType, out num))
				{
					switch (num)
					{
					case 0:
						result = new EventNotFoundException(new LocalizedString("EventNotFoundException"));
						break;
					case 1:
						result = new ReadEventsFailedException(new LocalizedString("ReadEventsFailedException"), null);
						break;
					case 2:
						result = new ReadEventsFailedTransientException(new LocalizedString("ReadEventsFailedTransientException"), null);
						break;
					case 3:
						result = new SuitabilityDirectoryException("FQDN", 1, "SuitabilityDirectoryException");
						break;
					case 4:
						result = new ADTransientException(new LocalizedString("ADTransientException"));
						break;
					case 5:
						result = new WebException("The request was aborted: The request was canceled.", null, WebExceptionStatus.RequestCanceled, null);
						break;
					case 6:
						result = new ADPossibleOperationException(new LocalizedString("ADPossibleOperationException"));
						break;
					case 7:
						result = new StorageTransientException(new LocalizedString("StorageTransientException"));
						break;
					case 8:
						result = new MailboxInSiteFailoverException(new LocalizedString("ConstMailboxInSiteFailoverException"));
						break;
					case 9:
						result = new OverBudgetException();
						break;
					case 10:
						result = new AccessDeniedException(new LocalizedString("AccessDeniedException"));
						break;
					case 11:
						result = new AccountDisabledException(new LocalizedString("AccountDisabledException"));
						break;
					case 12:
						result = new ConnectionFailedPermanentException(new LocalizedString("ConnectionFailedPermanentException"));
						break;
					case 13:
						result = new CorruptDataException(new LocalizedString("CorruptDataException"));
						break;
					case 14:
						result = new ObjectNotFoundException(new LocalizedString("ObjectNotFoundException"));
						break;
					case 15:
						result = new ServerNotFoundException("Server not Found", "ServerName");
						break;
					case 16:
						result = new ServerNotInSiteException("Server not in site", "ServerName");
						break;
					case 17:
					{
						MapiEventNative mapiEventNative = default(MapiEventNative);
						Event finalEvent = new Event(Guid.NewGuid(), new MapiEvent(ref mapiEventNative));
						result = new FinalEventException(finalEvent);
						break;
					}
					case 18:
					{
						MapiExceptionAmbiguousAlias innerException = new MapiExceptionAmbiguousAlias("MapiExceptionAmbiguousAlias", 0, 0, null, null);
						result = new StoragePermanentException(new LocalizedString("MapiExceptionAmbiguousAlias"), innerException);
						break;
					}
					case 19:
						result = new SystemException("SystemException");
						break;
					case 20:
						result = new IOException("IOException");
						break;
					case 21:
						result = new MapiExceptionCorruptData("MapiExceptionCorruptData", 0, 0, null, null);
						break;
					case 22:
						result = new IllegalCrossServerConnectionException(new LocalizedString("IllegalCrossServerConnectionException"));
						break;
					case 23:
						result = new MapiExceptionShutoffQuotaExceeded("MapiExceptionShutoffQuotaExceeded", 0, 0, null, null);
						break;
					case 24:
						result = new MapiExceptionNoSupport("MapiExceptionNoSupport", 0, 0, null, null);
						break;
					case 25:
						result = new TextConvertersException("TextConvertersException");
						break;
					case 26:
					{
						MapiExceptionNoAccess innerException2 = new MapiExceptionNoAccess("MapiExceptionNoAccess", 0, 0, null, null);
						result = new StoragePermanentException(new LocalizedString("MapiExceptionNoAccess"), innerException2);
						break;
					}
					case 27:
					{
						Thread.Sleep(2718);
						MapiExceptionTimeout exception = MapiExceptionHelper.TimeoutException("MapiExceptionTimeout", null);
						result = StorageGlobals.TranslateMapiException(new LocalizedString("MapiExceptionTimeout"), exception, null, null, "MapiExceptionTimeout", new object[0]);
						break;
					}
					}
				}
			}
			return result;
		}

		internal enum LIDs : uint
		{
			PushSubscriptionTermination = 2833657149U,
			ADExceptionDuringCallContextConstructor = 3286641981U,
			CallContextCreateCallContext = 3789958461U,
			ADTransientExceptionLogProxyFailure = 3454414141U,
			WebExceptionDuringXmlReaderCreate = 4259720509U,
			CallContextDispose = 3559271741U,
			GetInboxRules = 3274059069U,
			ServiceDiscoveryExceptionOnGetSite = 2703633725U,
			UpdateInboxRules = 2972069181U,
			EventSinkReadEventsFailure = 3534105917U,
			EwsBasicAuthWindowsPrincipalMappingError = 2544250173U,
			IOExceptionWhileProxyingBodyContent = 3594923325U,
			MapiExceptionWhileGettingPermissionSet = 3024497981U,
			WebClientQueryStringPropertyBase_GetMailboxParameter = 3158715709U,
			GetUserAvailability_GetUserAvailabilityFromRequest = 3309710653U,
			LogDatapoint = 3804638525U,
			FindCountLimit_ChangeValue = 3913690429U,
			SearchTimeoutInMilliseconds_ChangeValue = 3200658749U,
			ActAsUserRequirement_ChangeValue = 2328243517U,
			ConvertId_SleepTime_ChangeValue = 2647010621U,
			SkipTokenSerializationCheck_ChangeValue = 3167104317U,
			UploadItems_FastTransferProxyRequestFailure = 3351653693U,
			UploadItems_MapiMessageSaveChangesFailure = 3217435965U,
			GetFolder_IllegalCrossServerConnection = 3116772669U,
			ConversationPreviewFailure = 3137744189U,
			BodyConversionFailure = 3842387261U,
			NormalizedBodyConversionFailure = 2231774525U,
			UniqueBodyConversionFailure = 3305516349U,
			MapiExceptionNoAccessError = 3976604989U,
			GetItemCalendarIdRaiseException = 2969972029U,
			GetConversationItemsMailSubjectRaiseException = 4043713853U,
			GetItemMapiTimeoutError = 3238407485U,
			MultiStepServiceCommandPreExecuteMapiTimeoutError = 3716558141U,
			GetConversationItemsItemPartLoadFailed = 4177931581U,
			GetAttachmentSizeLimit_ChangeValue = 2659593533U,
			OAuthIdentityActAsUserNullSid = 2475044157U,
			AddAggregatedAccountNewSyncRequest_ChangeValue = 3951439165U,
			RemoveggregatedAccountNewSyncRequest_ChangeValue = 2340826429U,
			MapiExceptionMaxObjsExceededInGetItem_ChangeValue = 4186320189U
		}
	}
}
