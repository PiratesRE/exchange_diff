using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.ELC;
using Microsoft.Exchange.InfoWorker.Common.Search;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.UM.ClientAccess;
using Microsoft.Exchange.UM.Prompts.Provisioning;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Services.Core
{
	internal static class ServiceErrors
	{
		internal static ExceptionMapper GetExceptionMapper()
		{
			return ServiceErrors.exceptionMapper;
		}

		static ServiceErrors()
		{
			ExceptionMappingBase.Attributes attributes = ExceptionMappingBase.Attributes.StopsBatchProcessing;
			ExceptionMappingBase.Attributes attributes2 = ExceptionMappingBase.Attributes.TryInnerExceptionForExceptionMapping;
			ExceptionMappingBase.Attributes attributes3 = ExceptionMappingBase.Attributes.IsUnmappedException;
			ExceptionMappingBase.Attributes attributes4 = ExceptionMappingBase.Attributes.ReportException;
			ExceptionMapper chainedExceptionMapper = new ExceptionMapper(new ExceptionMappingBase[]
			{
				new StaticExceptionMapping(typeof(StorageTransientException), attributes2 | attributes3, ResponseCodeType.ErrorInternalServerTransientError, (CoreResources.IDs)3995283118U),
				new StaticExceptionMapping(typeof(StoragePermanentException), attributes2 | attributes3 | attributes4, ResponseCodeType.ErrorInternalServerError, CoreResources.IDs.ErrorInternalServerError),
				new StaticExceptionMapping(typeof(TransientException), attributes3, ResponseCodeType.ErrorInternalServerTransientError, (CoreResources.IDs)3995283118U),
				new StaticExceptionMapping(typeof(LocalizedException), attributes3 | attributes4, ResponseCodeType.ErrorInternalServerError, CoreResources.IDs.ErrorInternalServerError)
			});
			List<ExceptionMappingBase> list = new List<ExceptionMappingBase>
			{
				new FilterNotSupportedExceptionMapping(),
				new IllegalCrossServerConnectionExceptionMapping(),
				new OccurrenceCrossingBoundaryExceptionMapping(),
				new RecurrenceHasNoOccurrenceExceptionMapping(),
				new StaticExceptionMapping(typeof(AccessDeniedException), attributes2 | attributes, ResponseCodeType.ErrorAccessDenied, (CoreResources.IDs)3579904699U),
				new StaticExceptionMapping(typeof(AccountDisabledException), attributes, ResponseCodeType.ErrorAccountDisabled, CoreResources.IDs.ErrorAccountDisabled),
				new StaticExceptionMapping(typeof(ADTransientException), attributes, ResponseCodeType.ErrorADUnavailable, CoreResources.IDs.ErrorADUnavailable),
				new StaticExceptionMapping(typeof(ADPossibleOperationException), attributes, ResponseCodeType.ErrorADOperation, (CoreResources.IDs)4038759526U),
				new StaticExceptionMapping(typeof(AttachmentExceededException), ResponseCodeType.ErrorAttachmentSizeLimitExceeded, CoreResources.IDs.ErrorAttachmentSizeLimitExceeded),
				new StaticExceptionMapping(typeof(CalendarProcessingException), ResponseCodeType.ErrorCorruptData, CoreResources.IDs.ErrorCorruptData),
				new StaticExceptionMapping(typeof(ConnectionFailedPermanentException), attributes2 | attributes, ResponseCodeType.ErrorConnectionFailed, (CoreResources.IDs)3500594897U),
				new StaticExceptionMapping(typeof(ConnectionFailedTransientException), attributes2 | attributes, ResponseCodeType.ErrorConnectionFailed, (CoreResources.IDs)3500594897U),
				new StaticExceptionMapping(typeof(ConversionFailedException), ResponseCodeType.ErrorContentConversionFailed, (CoreResources.IDs)4227151856U),
				new StaticExceptionMapping(typeof(ConversationItemQueryException), ExchangeVersion.Exchange2010SP1, ResponseCodeType.ErrorInvalidOperation, (CoreResources.IDs)3629808665U),
				new StaticExceptionMapping(typeof(CorrelationFailedException), ExchangeVersion.Exchange2007SP1, ResponseCodeType.ErrorInvalidOperation, (CoreResources.IDs)2611688746U),
				new StaticExceptionMapping(typeof(CorruptDataException), ResponseCodeType.ErrorCorruptData, CoreResources.IDs.ErrorCorruptData),
				new StaticExceptionMapping(typeof(DataValidationException), ResponseCodeType.ErrorMailboxConfiguration, CoreResources.IDs.ErrorMailboxConfiguration),
				new StaticExceptionMapping(typeof(DataSourceOperationException), ResponseCodeType.ErrorDataSourceOperation, (CoreResources.IDs)2697731302U),
				new StaticExceptionMapping(typeof(EventNotFoundException), ResponseCodeType.ErrorEventNotFound, CoreResources.IDs.ErrorEventNotFound),
				new StaticExceptionMapping(typeof(FinalEventException), ResponseCodeType.ErrorInvalidSubscription, CoreResources.IDs.ErrorInvalidSubscription),
				new StaticExceptionMapping(typeof(InvalidEventWatermarkException), ResponseCodeType.ErrorInvalidWatermark, (CoreResources.IDs)3312780993U),
				new StaticExceptionMapping(typeof(InvalidExternalSharingInitiatorException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorInvalidExternalSharingInitiator, (CoreResources.IDs)4028591235U),
				new StaticExceptionMapping(typeof(InvalidParentFolderException), ResponseCodeType.ErrorInvalidParentFolder, (CoreResources.IDs)3659985571U),
				new StaticExceptionMapping(typeof(InvalidReceiveMeetingMessageCopiesException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorDelegateValidationFailed, CoreResources.IDs.ErrorDelegateMustBeCalendarEditorToGetMeetingMessages),
				new StaticExceptionMapping(typeof(InvalidRecipientsException), ResponseCodeType.ErrorInvalidRecipients, CoreResources.IDs.ErrorInvalidRecipients),
				new StaticExceptionMapping(typeof(InvalidSharingDataException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorInvalidSharingData, CoreResources.IDs.ErrorInvalidSharingData),
				new StaticExceptionMapping(typeof(InvalidSharingMessageException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorInvalidSharingMessage, CoreResources.IDs.ErrorInvalidSharingMessage),
				new StaticExceptionMapping(typeof(InvalidSidException), ResponseCodeType.ErrorInvalidSid, (CoreResources.IDs)2718542415U),
				new StaticExceptionMapping(typeof(MailboxUnavailableException), attributes, ResponseCodeType.ErrorMailboxStoreUnavailable, CoreResources.IDs.ErrorMailboxStoreUnavailable),
				new StaticExceptionMapping(typeof(MapiExceptionCorruptStore), attributes, ResponseCodeType.ErrorMailboxStoreUnavailable, CoreResources.IDs.ErrorMailboxStoreUnavailable),
				new StaticExceptionMapping(typeof(MapiExceptionExiting), attributes, ResponseCodeType.ErrorMailboxStoreUnavailable, CoreResources.IDs.ErrorMailboxStoreUnavailable),
				new StaticExceptionMapping(typeof(MapiExceptionJetErrorInvalidSesid), attributes, ResponseCodeType.ErrorInternalServerTransientError, (CoreResources.IDs)3995283118U),
				new StaticExceptionMapping(typeof(MapiExceptionMailboxInTransit), attributes, ResponseCodeType.ErrorMailboxMoveInProgress, CoreResources.IDs.ErrorMailboxMoveInProgress),
				new StaticExceptionMapping(typeof(MapiExceptionMdbOffline), attributes, ResponseCodeType.ErrorMailboxStoreUnavailable, CoreResources.IDs.ErrorMailboxStoreUnavailable),
				new StaticExceptionMapping(typeof(MapiExceptionNetworkError), attributes, ResponseCodeType.ErrorMailboxStoreUnavailable, CoreResources.IDs.ErrorMailboxStoreUnavailable),
				new StaticExceptionMapping(typeof(MapiExceptionNoCreateRight), ResponseCodeType.ErrorCreateItemAccessDenied, CoreResources.IDs.ErrorCreateItemAccessDenied),
				new StaticExceptionMapping(typeof(MapiExceptionNoCreateSubfolderRight), ResponseCodeType.ErrorCreateSubfolderAccessDenied, (CoreResources.IDs)4062262029U),
				new StaticExceptionMapping(typeof(MapiExceptionNotEnoughMemory), ResponseCodeType.ErrorNotEnoughMemory, (CoreResources.IDs)3719196410U),
				new StaticExceptionMapping(typeof(MapiExceptionPasswordChangeRequired), attributes, ResponseCodeType.ErrorPasswordChangeRequired, (CoreResources.IDs)3093510304U),
				new StaticExceptionMapping(typeof(MapiExceptionPasswordExpired), attributes, ResponseCodeType.ErrorPasswordExpired, CoreResources.IDs.ErrorPasswordExpired),
				new StaticExceptionMapping(typeof(MapiExceptionPublicRoot), attributes, ExchangeVersion.Exchange2007SP1, ResponseCodeType.ErrorOperationNotAllowedWithPublicFolderRoot, (CoreResources.IDs)2440725179U),
				new StaticExceptionMapping(typeof(MapiExceptionRuleVersion), ResponseCodeType.ErrorItemSave, (CoreResources.IDs)2540872182U),
				new StaticExceptionMapping(typeof(MapiExceptionStreamSizeError), ResponseCodeType.ErrorItemPropertyRequestFailed, CoreResources.IDs.ErrorItemPropertyRequestFailed),
				new StaticExceptionMapping(typeof(MapiExceptionTooComplex), ResponseCodeType.ErrorRestrictionTooComplex, CoreResources.IDs.ErrorRestrictionTooComplex),
				new StaticExceptionMapping(typeof(MapiExceptionWrongMailbox), attributes, ResponseCodeType.ErrorMailboxMoveInProgress, CoreResources.IDs.ErrorMailboxMoveInProgress),
				new MapiExceptionMaxObjsExceededMapping(),
				new StaticExceptionMapping(typeof(MessageTooBigException), ResponseCodeType.ErrorMessageSizeExceeded, (CoreResources.IDs)2913173341U),
				new StaticExceptionMapping(typeof(MessageSubmissionExceededException), ResponseCodeType.ErrorMessageSizeExceeded, (CoreResources.IDs)2913173341U),
				new StaticExceptionMapping(typeof(NoMoreConnectionsException), attributes, ResponseCodeType.ErrorMailboxStoreUnavailable, CoreResources.IDs.ErrorMailboxStoreUnavailable),
				new StaticExceptionMapping(typeof(NoExternalEwsAvailableException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorSharingNoExternalEwsAvailable, (CoreResources.IDs)4047718788U),
				new StaticExceptionMapping(typeof(NotAllowedExternalSharingByPolicyException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorNotAllowedExternalSharingByPolicy, (CoreResources.IDs)2890296403U),
				new StaticExceptionMapping(typeof(NotSupportedSharingMessageException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorNotSupportedSharingMessage, (CoreResources.IDs)3991730990U),
				new StaticExceptionMapping(typeof(NotSupportedWithServerVersionException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorWrongServerVersion, (CoreResources.IDs)3533302998U),
				new StaticExceptionMapping(typeof(ObjectExistedException), ResponseCodeType.ErrorFolderExists, CoreResources.IDs.ErrorFolderExists),
				new StaticExceptionMapping(typeof(ObjectNotFoundException), ResponseCodeType.ErrorItemNotFound, (CoreResources.IDs)4005418156U),
				new StaticExceptionMapping(typeof(ObjectValidationException), ResponseCodeType.ErrorPropertyValidationFailure, (CoreResources.IDs)3967923828U),
				new StaticExceptionMapping(typeof(ObjectNotInitializedException), ResponseCodeType.ErrorSearchFolderNotInitialized, CoreResources.IDs.ErrorSearchFolderNotInitialized),
				new StaticExceptionMapping(typeof(OccurrenceNotFoundException), ExchangeVersion.Exchange2007SP1, ResponseCodeType.ErrorInvalidOperation, CoreResources.IDs.MessageOccurrenceNotFound),
				new StaticExceptionMapping(typeof(OccurrenceTimeSpanTooBigException), ResponseCodeType.ErrorOccurrenceTimeSpanTooBig, CoreResources.IDs.ErrorOccurrenceTimeSpanTooBig),
				new StaticExceptionMapping(typeof(OrganizationNotFederatedException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorOrganizationNotFederated, CoreResources.IDs.ErrorOrganizationNotFederated),
				new StaticExceptionMapping(typeof(QuotaExceededException), attributes, ResponseCodeType.ErrorQuotaExceeded, (CoreResources.IDs)3654265673U),
				new StaticExceptionMapping(typeof(ReadEventsFailedException), ResponseCodeType.ErrorReadEventsFailed, (CoreResources.IDs)3577190220U),
				new StaticExceptionMapping(typeof(ReadEventsFailedTransientException), ResponseCodeType.ErrorReadEventsFailed, (CoreResources.IDs)3577190220U),
				new StaticExceptionMapping(typeof(RecoverableItemsAccessDeniedException), ResponseCodeType.ErrorItemSave, (CoreResources.IDs)2339310738U),
				new StaticExceptionMapping(typeof(RecurrenceEndDateTooBigException), ResponseCodeType.ErrorRecurrenceEndDateTooBig, (CoreResources.IDs)2652436543U),
				new StaticExceptionMapping(typeof(RecurrenceFormatException), ExchangeVersion.Exchange2007SP1, ResponseCodeType.ErrorInvalidOperation, (CoreResources.IDs)3854873845U),
				new StaticExceptionMapping(typeof(RecurrenceStartDateTooSmallException), ExchangeVersion.Exchange2007SP1, ResponseCodeType.ErrorInvalidOperation, CoreResources.IDs.MessageRecurrenceStartDateTooSmall),
				new StaticExceptionMapping(typeof(ResourcesException), attributes, ResponseCodeType.ErrorInsufficientResources, CoreResources.IDs.ErrorInsufficientResources),
				new StaticExceptionMapping(typeof(SaveConflictException), ResponseCodeType.ErrorIrresolvableConflict, CoreResources.IDs.ErrorIrresolvableConflict),
				new StaticExceptionMapping(typeof(SendAsDeniedException), ResponseCodeType.ErrorSendAsDenied, (CoreResources.IDs)4260694481U),
				new StaticExceptionMapping(typeof(ServerPausedException), attributes, ResponseCodeType.ErrorMailboxStoreUnavailable, CoreResources.IDs.ErrorMailboxStoreUnavailable),
				new StaticExceptionMapping(typeof(SubmissionQuotaExceededException), attributes, ExchangeVersion.Exchange2010, ResponseCodeType.ErrorSubmissionQuotaExceeded, CoreResources.IDs.ErrorSubmissionQuotaExceeded),
				new StaticExceptionMapping(typeof(VirusDetectedException), ResponseCodeType.ErrorVirusDetected, (CoreResources.IDs)3705244005U),
				new StaticExceptionMapping(typeof(VirusScanInProgressException), ResponseCodeType.ErrorServerBusy, (CoreResources.IDs)3655513582U),
				new StaticExceptionMapping(typeof(VirusMessageDeletedException), ResponseCodeType.ErrorVirusMessageDeleted, CoreResources.IDs.ErrorVirusMessageDeleted),
				new StaticExceptionMapping(typeof(WrongServerException), attributes, ResponseCodeType.ErrorMailboxMoveInProgress, CoreResources.IDs.ErrorMailboxMoveInProgress),
				new StaticExceptionMapping(typeof(RulesTooBigException), ResponseCodeType.ErrorRulesOverQuota, CoreResources.IDs.RuleErrorRulesOverQuota),
				new StaticExceptionMapping(typeof(InvalidIdMalformedException), ResponseCodeType.ErrorInvalidIdMalformed, (CoreResources.IDs)3107705007U),
				new TimeZoneExceptionMapping(),
				new UnsupportedPropertyForSortGroupExceptionMapping(),
				new StaticExceptionMapping(typeof(TooManyObjectsOpenedException), ExchangeVersion.Exchange2013, ResponseCodeType.ErrorTooManyObjectsOpened, CoreResources.IDs.ErrorTooManyObjectsOpened),
				new StaticExceptionMapping(typeof(TenantAccessBlockedException), ExchangeVersion.ExchangeV2_14, ResponseCodeType.ErrorOrganizationAccessBlocked, (CoreResources.IDs)3276585407U),
				new StaticExceptionMapping(typeof(InvalidLicenseException), ExchangeVersion.ExchangeV2_14, ResponseCodeType.ErrorInvalidLicense, CoreResources.IDs.ErrorInvalidLicense),
				new StaticExceptionMapping(typeof(Microsoft.Exchange.Entities.DataProviders.InvalidRequestException), ResponseCodeType.ErrorInvalidRequest, (CoreResources.IDs)3784063568U)
			};
			ServiceErrors.AddMappings(list);
			ServiceErrors.exceptionMapper = new ExceptionMapper(chainedExceptionMapper, list);
		}

		public static ServiceError GetServiceError(LocalizedException exception)
		{
			return ServiceErrors.exceptionMapper.GetServiceError(exception);
		}

		public static ServiceError GetServiceError(LocalizedException exception, ExchangeVersion currentExchangeVersion)
		{
			return ServiceErrors.exceptionMapper.GetServiceError(exception, currentExchangeVersion);
		}

		public static void CheckAndThrowFaultExceptionOnRequestLevelErrors<SingleItemType>(params ServiceResult<SingleItemType>[] results)
		{
			ServiceError serviceError = null;
			int num = 0;
			int num2 = 0;
			if (results == null)
			{
				return;
			}
			foreach (ServiceResult<SingleItemType> serviceResult in results)
			{
				if (serviceResult != null)
				{
					if (serviceResult.Error != null && serviceResult.Error.MessageKey != ResponseCodeType.NoError)
					{
						if (ServiceErrors.GroupResponseIgnoredCodeTypes.Contains(serviceResult.Error.MessageKey))
						{
							num2++;
							goto IL_93;
						}
						if (ServiceErrors.GroupResponseCodeTypes.Contains(serviceResult.Error.MessageKey))
						{
							if (serviceError == null)
							{
								serviceError = serviceResult.Error;
							}
							else if (serviceError.MessageKey != serviceResult.Error.MessageKey)
							{
								return;
							}
							num++;
							goto IL_93;
						}
					}
					return;
				}
				IL_93:;
			}
			if (serviceError != null && serviceError.LocalizedException != null)
			{
				RequestDetailsLogger.Current.AppendGenericInfo("ErrorsRemapped", num + ':' + num2);
				throw FaultExceptionUtilities.CreateFault(serviceError.LocalizedException, FaultParty.Receiver);
			}
		}

		private static void AddMappings(List<ExceptionMappingBase> exceptionMappingList)
		{
			ExceptionMappingBase.Attributes attributes = ExceptionMappingBase.Attributes.StopsBatchProcessing;
			ExceptionMappingBase.Attributes attributes2 = ExceptionMappingBase.Attributes.TryInnerExceptionForExceptionMapping;
			exceptionMappingList.Add(new MailboxCrossSiteFailoverExceptionMapping());
			exceptionMappingList.Add(new MailboxFailoverExceptionMapping(typeof(MailboxInSiteFailoverException)));
			exceptionMappingList.Add(new MailboxFailoverExceptionMapping(typeof(MailboxOfflineException)));
			exceptionMappingList.Add(new OverBudgetExceptionMapping());
			exceptionMappingList.Add(new PrincipalNotAllowedByPolicyExceptionMapping());
			exceptionMappingList.Add(new ResourceUnhealthyExceptionMapping());
			exceptionMappingList.Add(new RightsNotAllowedByPolicyExceptionMapping());
			exceptionMappingList.Add(new SharingSynchronizationExceptionMapping());
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(ADOperationException), attributes, ResponseCodeType.ErrorADOperation, (CoreResources.IDs)4038759526U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(ADFilterException), ResponseCodeType.ErrorADSessionFilter, CoreResources.IDs.ErrorADSessionFilter));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(ClientAccessException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorUnifiedMessagingRequestFailed, (CoreResources.IDs)2346704662U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(ContentIndexingNotEnabledException), attributes2 | attributes, ResponseCodeType.ErrorContentIndexingNotEnabled, (CoreResources.IDs)3975089319U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(DelayCacheFullException), ExceptionMappingBase.Attributes.StopsBatchProcessing, ResponseCodeType.ErrorServerBusy, (CoreResources.IDs)3655513582U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(DeleteContentException), ExchangeVersion.Exchange2012, ResponseCodeType.ErrorDeleteUnifiedMessagingPromptFailed, CoreResources.IDs.ErrorDeleteUnifiedMessagingPromptFailed));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(DialPlanNotFoundException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorUnifiedMessagingDialPlanNotFound, CoreResources.IDs.ErrorUnifiedMessagingDialPlanNotFound));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(DialingRulesException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorPhoneNumberNotDialable, (CoreResources.IDs)4266358168U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(ELCDuplicateFolderNamesArgumentException), ResponseCodeType.ErrorDuplicateInputFolderNames, CoreResources.IDs.ErrorDuplicateInputFolderNames));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(ELCNoMatchingOrgFoldersException), ResponseCodeType.ErrorManagedFolderNotFound, (CoreResources.IDs)2306155022U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(ELCOrgFolderExistsException), ResponseCodeType.ErrorManagedFolderAlreadyExists, CoreResources.IDs.ErrorManagedFolderAlreadyExists));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(ELCPartialCompletionException), ResponseCodeType.ErrorCreateManagedFolderPartialCompletion, CoreResources.IDs.ErrorCreateManagedFolderPartialCompletion));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(ELCRootFailureException), ResponseCodeType.ErrorManagedFoldersRootFailure, (CoreResources.IDs)2580909644U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(InvalidADRecipientException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorCallerIsInvalidADAccount, CoreResources.IDs.ErrorCallerIsInvalidADAccount));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(InvalidCallIdException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorInvalidPhoneCallId, (CoreResources.IDs)3978299680U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(InvalidExternalSharingSubscriberException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorInvalidExternalSharingSubscriber, (CoreResources.IDs)3133201118U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(InvalidFederatedOrganizationIdException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorInvalidFederatedOrganizationId, (CoreResources.IDs)3060608191U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(InvalidObjectIdException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorInvalidIdMalformed, (CoreResources.IDs)3107705007U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(InvalidPhoneNumberException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorInvalidPhoneNumber, (CoreResources.IDs)3260461220U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(InvalidPrincipalException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorUnifiedMessagingRequestFailed, (CoreResources.IDs)2346704662U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(InvalidResponseException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorUnifiedMessagingRequestFailed, (CoreResources.IDs)2346704662U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(InvalidSipUriException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorInvalidSIPUri, CoreResources.IDs.ErrorInvalidSIPUri));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(IPGatewayNotFoundException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorIPGatewayNotFound, (CoreResources.IDs)2252936850U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(OverPlayOnPhoneCallLimitException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorServerBusy, (CoreResources.IDs)3655513582U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(PublishingPointException), ExchangeVersion.Exchange2012, ResponseCodeType.ErrorPromptPublishingOperationFailed, (CoreResources.IDs)2217412679U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(SourceFileNotFoundException), ExchangeVersion.Exchange2012, ResponseCodeType.ErrorUnifiedMessagingPromptNotFound, (CoreResources.IDs)3135900505U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(UnableToFindUMReportDataException), ExchangeVersion.Exchange2012, ResponseCodeType.ErrorUnifiedMessagingReportDataNotFound, CoreResources.IDs.ErrorUnifiedMessagingReportDataNotFound));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(UMRecipientValidationException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorUnifiedMessagingRequestFailed, (CoreResources.IDs)2346704662U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(UMServerNotFoundException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorUnifiedMessagingServerNotFound, CoreResources.IDs.ErrorUnifiedMessagingServerNotFound));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(UserNotUmEnabledException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorUserNotUnifiedMessagingEnabled, (CoreResources.IDs)4142344047U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(UserWithoutFederatedProxyAddressException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorUserWithoutFederatedProxyAddress, (CoreResources.IDs)3060608191U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(ProxyRequestNotAllowedException), ExchangeVersion.Exchange2010SP1, ResponseCodeType.ErrorProxyRequestNotAllowed, (CoreResources.IDs)4264440001U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(SearchMailboxException), ExchangeVersion.Exchange2010SP1, ResponseCodeType.ErrorInternalServerError, (CoreResources.IDs)2933656041U));
			exceptionMappingList.Add(new StaticExceptionMapping(typeof(SearchFolderTimeoutException), ExchangeVersion.Exchange2010SP1, ResponseCodeType.ErrorInternalServerError, (CoreResources.IDs)2447591155U));
			exceptionMappingList.Add(new RightsManagementExceptionMapping(typeof(RightsManagementPermanentException), ExchangeVersion.Exchange2012, ResponseCodeType.ErrorRightsManagementPermanentException));
			exceptionMappingList.Add(new RightsManagementExceptionMapping(typeof(RightsManagementTransientException), ExchangeVersion.Exchange2012, ResponseCodeType.ErrorRightsManagementTransientException));
			exceptionMappingList.Add(new ExchangeConfigurationExceptionMapping(typeof(ExchangeConfigurationException), ExchangeVersion.Exchange2012, ResponseCodeType.ErrorExchangeConfigurationException));
		}

		private static readonly ExceptionMapper exceptionMapper;

		private static HashSet<ResponseCodeType> GroupResponseCodeTypes = new HashSet<ResponseCodeType>
		{
			ResponseCodeType.ErrorInternalServerTransientError,
			ResponseCodeType.ErrorServerBusy
		};

		private static HashSet<ResponseCodeType> GroupResponseIgnoredCodeTypes = new HashSet<ResponseCodeType>
		{
			ResponseCodeType.ErrorBatchProcessingStopped
		};
	}
}
