using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security.AntiXss;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AirSync
{
	internal static class AirSyncUtility
	{
		internal static string[] PiiTags
		{
			get
			{
				return AirSyncUtility.piiTags;
			}
		}

		internal static string DefaultAcceptedDomainName
		{
			get
			{
				OrganizationId currentOrganizationId = Command.CurrentOrganizationId;
				string text;
				if (AirSyncUtility.defaultAcceptedDomainTable.TryGetValue(currentOrganizationId, out text))
				{
					return text;
				}
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(currentOrganizationId), 168, "DefaultAcceptedDomainName", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\AirSyncUtility.cs");
				AcceptedDomain defaultAcceptedDomain = tenantOrTopologyConfigurationSession.GetDefaultAcceptedDomain();
				if (Command.CurrentCommand != null)
				{
					Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, tenantOrTopologyConfigurationSession.LastUsedDc);
				}
				if (defaultAcceptedDomain != null)
				{
					text = defaultAcceptedDomain.DomainName.ToString();
					if (!string.IsNullOrEmpty(text))
					{
						AirSyncUtility.defaultAcceptedDomainTable.Add(currentOrganizationId, text);
					}
				}
				if (string.IsNullOrEmpty(text) && Command.CurrentCommand != null)
				{
					Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoAcceptedDomainFor" + currentOrganizationId);
				}
				return text;
			}
		}

		internal static InboundConversionOptions GetInboundConversionOptions()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(Command.CurrentOrganizationId), 210, "GetInboundConversionOptions", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\AirSyncUtility.cs");
			InboundConversionOptions inboundConversionOptions = new InboundConversionOptions(AirSyncUtility.DefaultAcceptedDomainName);
			inboundConversionOptions.UserADSession = tenantOrRootOrgRecipientSession;
			inboundConversionOptions.LoadPerOrganizationCharsetDetectionOptions(Command.CurrentOrganizationId);
			return inboundConversionOptions;
		}

		internal static OutboundConversionOptions GetOutboundConversionOptions()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(Command.CurrentOrganizationId), 229, "GetOutboundConversionOptions", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\AirSyncUtility.cs");
			OutboundConversionOptions outboundConversionOptions = new OutboundConversionOptions(AirSyncUtility.DefaultAcceptedDomainName);
			outboundConversionOptions.ClearCategories = false;
			outboundConversionOptions.LoadPerOrganizationCharsetDetectionOptions(Command.CurrentOrganizationId);
			outboundConversionOptions.AllowPartialStnefConversion = true;
			outboundConversionOptions.DemoteBcc = true;
			outboundConversionOptions.UserADSession = tenantOrRootOrgRecipientSession;
			if (Command.CurrentCommand != null)
			{
				Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, tenantOrRootOrgRecipientSession.LastUsedDc);
			}
			return outboundConversionOptions;
		}

		internal static bool TryGetPropertyFromBag<T>(IStorePropertyBag propertyBag, PropertyDefinition propDef, out T value)
		{
			object obj = null;
			try
			{
				obj = propertyBag.TryGetProperty(propDef);
			}
			catch (NotInBagPropertyErrorException)
			{
				AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, null, "[AirSyncUtility.TryGetPropertyFromBag] NotInBag exception for property {0}.  Returning default value.", propDef.Name);
				value = default(T);
				return false;
			}
			if (obj is T)
			{
				value = (T)((object)obj);
				return true;
			}
			PropertyError propertyError = obj as PropertyError;
			if (propertyError != null)
			{
				AirSyncDiagnostics.TraceError<Type, string, PropertyErrorCode>(ExTraceGlobals.RequestsTracer, null, "[AirSyncUtility.TryGetPropertyFromBag] Expected property of type {0} in bag for propDef {1}, but encountered error {2}.", typeof(T), propDef.Name, propertyError.PropertyErrorCode);
			}
			else
			{
				try
				{
					value = (T)((object)obj);
					return true;
				}
				catch (InvalidCastException ex)
				{
					AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, null, "[AirSyncUtility.TryGetPropertyFromBag] Tried to cast property '{0}' with value '{1}' to type '{2}', but the cast failed with error '{3}'.", new object[]
					{
						propDef.Name,
						(obj == null) ? "<NULL>" : obj,
						typeof(T).FullName,
						ex
					});
				}
			}
			value = default(T);
			return false;
		}

		internal static int ParseVersionString(string versionString)
		{
			switch (versionString)
			{
			case "":
				return 0;
			case "Unknown":
				return -1;
			case "Unexpected":
				return -1;
			case "Unspecified":
				return 0;
			case "1.0":
				return 10;
			case "2.0":
				return 20;
			case "2.1":
				return 21;
			case "2.5":
				return 25;
			case "12.0":
				return 120;
			case "12.1":
				return 121;
			case "14.0":
				return 140;
			case "14.1":
				return 141;
			case "16.0":
				return 160;
			case null:
				break;
			default:
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Invalid version string '{0}', returning -1", versionString);
				return -1;
				break;
			}
			return 0;
		}

		internal static string BuildOuterXml(XmlDocument xmlDocument)
		{
			return AirSyncUtility.BuildOuterXml(xmlDocument, false);
		}

		internal static string BuildOuterXml(XmlDocument xmlDocument, bool truncatePII)
		{
			StringBuilder stringBuilder = new StringBuilder(4096);
			stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
			if (xmlDocument.HasChildNodes)
			{
				if (xmlDocument.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
				{
					AirSyncUtility.BuildOuterXml(stringBuilder, xmlDocument.FirstChild.NextSibling, 0, null, truncatePII);
				}
				else
				{
					AirSyncUtility.BuildOuterXml(stringBuilder, xmlDocument.FirstChild, 0, null, truncatePII);
				}
			}
			return stringBuilder.ToString();
		}

		internal static string GetClassNameFromType(string folderType)
		{
			string result = null;
			switch (folderType)
			{
			case "1":
				result = string.Empty;
				break;
			case "12":
				result = "IPF.Note";
				break;
			case "13":
				result = "IPF.Appointment";
				break;
			case "14":
				result = "IPF.Contact";
				break;
			case "15":
				result = "IPF.Task";
				break;
			case "16":
				result = "IPF.Journal";
				break;
			case "17":
				result = "IPF.StickyNote";
				break;
			}
			return result;
		}

		internal static string GetAirSyncFolderTypeClass(ISyncItemId folderId)
		{
			MailboxSyncItemId mailboxSyncItemId = folderId as MailboxSyncItemId;
			if (mailboxSyncItemId != null)
			{
				return AirSyncUtility.GetAirSyncFolderTypeClass((StoreObjectId)mailboxSyncItemId.NativeId);
			}
			if (folderId is RecipientInfoCacheSyncItemId)
			{
				return "RecipientInfoCache";
			}
			return null;
		}

		internal static string GetAirSyncFolderTypeClass(StoreObjectId folderId)
		{
			string result;
			switch (folderId.ObjectType)
			{
			case StoreObjectType.CalendarFolder:
				result = "Calendar";
				break;
			case StoreObjectType.ContactsFolder:
				result = "Contacts";
				break;
			case StoreObjectType.TasksFolder:
				result = "Tasks";
				break;
			case StoreObjectType.NotesFolder:
				result = "Notes";
				break;
			default:
				result = "Email";
				break;
			}
			return result;
		}

		internal static string GetAirSyncFolderType(MailboxSession mailboxSession, StoreObjectId folderId)
		{
			if (folderId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox)))
			{
				return "2";
			}
			if (folderId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts)))
			{
				return "3";
			}
			if (folderId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems)))
			{
				return "4";
			}
			if (folderId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.SentItems)))
			{
				return "5";
			}
			if (folderId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.Outbox)))
			{
				return "6";
			}
			return "1";
		}

		internal static string GetAirSyncFolderType(MailboxSession mailboxSession, bool sharedFolder, Folder folder)
		{
			return AirSyncUtility.GetAirSyncFolderType(mailboxSession, sharedFolder, folder.Id, folder.ClassName);
		}

		internal static string GetAirSyncDefaultFolderType(MailboxSession mailboxSession, StoreId folderId)
		{
			switch (mailboxSession.IsDefaultFolderType(folderId))
			{
			case DefaultFolderType.Calendar:
				return "8";
			case DefaultFolderType.Contacts:
				return "9";
			case DefaultFolderType.DeletedItems:
				return "4";
			case DefaultFolderType.Drafts:
				return "3";
			case DefaultFolderType.Inbox:
				return "2";
			case DefaultFolderType.Journal:
				return "11";
			case DefaultFolderType.Notes:
				return "10";
			case DefaultFolderType.Outbox:
				return "6";
			case DefaultFolderType.SentItems:
				return "5";
			case DefaultFolderType.Tasks:
				return "7";
			}
			return null;
		}

		internal static string GetAirSyncFolderType(MailboxSession mailboxSession, bool sharedFolder, StoreId folderId, string folderClassName)
		{
			string airSyncDefaultFolderType = AirSyncUtility.GetAirSyncDefaultFolderType(mailboxSession, folderId);
			if (!string.IsNullOrEmpty(airSyncDefaultFolderType))
			{
				return airSyncDefaultFolderType;
			}
			switch (folderClassName)
			{
			case "":
				return "1";
			case "IPF.Note":
			case "IPF.Note.OutlookHomepage":
				return "12";
			case "IPF.Appointment":
				if (!sharedFolder)
				{
					return "13";
				}
				return "20";
			case "IPF.Contact":
				if (!sharedFolder)
				{
					return "14";
				}
				return "21";
			case "IPF.Task":
				if (!sharedFolder)
				{
					return "15";
				}
				return "22";
			case "IPF.Journal":
				if (!sharedFolder)
				{
					return "16";
				}
				return "23";
			case "IPF.StickyNote":
				if (!sharedFolder)
				{
					return "17";
				}
				return "24";
			}
			if (folderClassName.StartsWith("IPF.Note.", StringComparison.OrdinalIgnoreCase) || folderClassName.Equals("IPF.Note", StringComparison.OrdinalIgnoreCase))
			{
				return "12";
			}
			if (folderClassName.StartsWith("IPF.Appointment.", StringComparison.OrdinalIgnoreCase) || folderClassName.Equals("IPF.Appointment", StringComparison.OrdinalIgnoreCase))
			{
				if (!sharedFolder)
				{
					return "13";
				}
				return "20";
			}
			else if (folderClassName.StartsWith("IPF.Contact.", StringComparison.OrdinalIgnoreCase) || folderClassName.Equals("IPF.Contact", StringComparison.OrdinalIgnoreCase))
			{
				if (!sharedFolder)
				{
					return "14";
				}
				return "21";
			}
			else if (folderClassName.StartsWith("IPF.Journal.", StringComparison.OrdinalIgnoreCase) || folderClassName.Equals("IPF.Journal", StringComparison.OrdinalIgnoreCase))
			{
				if (!sharedFolder)
				{
					return "16";
				}
				return "23";
			}
			else if (folderClassName.StartsWith("IPF.Task.", StringComparison.OrdinalIgnoreCase) || folderClassName.Equals("IPF.Task", StringComparison.OrdinalIgnoreCase))
			{
				if (!sharedFolder)
				{
					return "15";
				}
				return "22";
			}
			else
			{
				if (!folderClassName.StartsWith("IPF.StickyNote.", StringComparison.OrdinalIgnoreCase) && !folderClassName.Equals("IPF.StickyNote", StringComparison.OrdinalIgnoreCase))
				{
					return "18";
				}
				if (!sharedFolder)
				{
					return "17";
				}
				return "24";
			}
		}

		internal static SyncCollection.CollectionTypes GetCollectionType(string shortId)
		{
			if (string.IsNullOrEmpty(shortId))
			{
				return SyncCollection.CollectionTypes.Unknown;
			}
			int num;
			if (int.TryParse(shortId, out num))
			{
				return SyncCollection.CollectionTypes.Mailbox;
			}
			if (shortId != null && shortId == "RI")
			{
				return SyncCollection.CollectionTypes.RecipientInfoCache;
			}
			return SyncCollection.CollectionTypes.Unknown;
		}

		internal static bool IsVirtualFolder(SyncCollection collection)
		{
			SyncCollection.CollectionTypes collectionType = AirSyncUtility.GetCollectionType(collection.CollectionId);
			return collectionType != SyncCollection.CollectionTypes.Mailbox && collectionType != SyncCollection.CollectionTypes.Unknown;
		}

		internal static bool IsProtectedVoicemailItem(Item item)
		{
			string valueOrDefault = item.GetValueOrDefault<string>(StoreObjectSchema.ItemClass);
			return valueOrDefault.StartsWith("IPM.Note.RPMSG.Microsoft.Voicemail", StringComparison.OrdinalIgnoreCase);
		}

		internal static CustomSyncState GetOrCreateGlobalSyncState(SyncStateStorage syncStateStorage)
		{
			GlobalSyncStateInfo syncStateInfo = new GlobalSyncStateInfo();
			CustomSyncState customSyncState = syncStateStorage.GetCustomSyncState(syncStateInfo, new PropertyDefinition[0]);
			if (customSyncState == null)
			{
				customSyncState = syncStateStorage.CreateCustomSyncState(syncStateInfo);
			}
			return customSyncState;
		}

		internal static CustomSyncState GetOrCreateSyncStatusSyncState(SyncStateStorage syncStateStorage)
		{
			SyncStatusSyncStateInfo syncStateInfo = new SyncStatusSyncStateInfo();
			AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "AirSyncUtility:GetSyncStatus Sync State");
			CustomSyncState customSyncState = syncStateStorage.GetCustomSyncState(syncStateInfo, new PropertyDefinition[0]);
			if (customSyncState == null)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "AirSyncUtility: SyncStatus sync state not found. Create new.");
				customSyncState = syncStateStorage.CreateCustomSyncState(syncStateInfo);
			}
			return customSyncState;
		}

		internal static string ExceptionToLocString(Exception ex)
		{
			Exception ex2 = ex;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(EASServerStrings.ExBegin);
			int num = 0;
			while (ex2 != null)
			{
				stringBuilder.Append(EASServerStrings.ExType(ex2.GetType().FullName));
				stringBuilder.Append(EASServerStrings.ExMessage(ex2.Message));
				stringBuilder.Append(EASServerStrings.ExLevel(num.ToString(CultureInfo.InvariantCulture)));
				if (ex2 is AirSyncPermanentException)
				{
					AirSyncPermanentException ex3 = ex2 as AirSyncPermanentException;
					stringBuilder.Append(EASServerStrings.HttpStatusCode((int)ex3.HttpStatusCode));
					stringBuilder.Append(EASServerStrings.SyncStatusCode(ex3.AirSyncStatusCodeInInt));
					stringBuilder.Append(EASServerStrings.XmlResponse((ex3.XmlResponse == null) ? EASServerStrings.NoXmlResponse : AirSyncUtility.BuildOuterXml(ex3.XmlResponse)));
				}
				stringBuilder.Append(EASServerStrings.ExStackTrace(ex2.StackTrace));
				ex2 = ex2.InnerException;
				if (ex2 != null)
				{
					stringBuilder.Append(EASServerStrings.ExInner);
				}
				num++;
			}
			stringBuilder.Append(EASServerStrings.ExEnd);
			return stringBuilder.ToString();
		}

		internal static bool HandleNonCriticalException(Exception ex, bool needWatson)
		{
			if (!AirSyncUtility.IsCriticalException(ex) && !(ex is AirSyncFatalException))
			{
				AirSyncUtility.UpdateExceptionPerfCounter(ex);
				if (GlobalSettings.SendWatsonReport && needWatson && !(ex is LocalizedException))
				{
					AirSyncDiagnostics.SendWatson(ex, false);
				}
				else
				{
					AirSyncDiagnostics.TraceError<Exception>(ExTraceGlobals.RequestsTracer, null, "Non-critical exception is caught and handled: {0}", ex);
				}
				return true;
			}
			AirSyncDiagnostics.TraceError<Exception>(ExTraceGlobals.RequestsTracer, null, "Critical exception is not handled: {0}", ex);
			return false;
		}

		internal static void LogCompressedStackTrace(Exception e, IAirSyncContext context)
		{
			if (GlobalSettings.LogCompressedExceptionDetails)
			{
				string compressedStackTrace = ExceptionTools.GetCompressedStackTrace(e);
				context.ProtocolLogger.AppendValue(ProtocolLoggerData.ExceptionStackTrace, compressedStackTrace);
			}
		}

		internal static void ProcessException(Exception ex)
		{
			AirSyncUtility.ProcessException(ex, null, null);
		}

		internal static void ProcessException(Exception ex, object thisObject, IAirSyncContext context)
		{
			string text = "Unknown user";
			string text2 = string.Empty;
			string empty = string.Empty;
			string text3 = null;
			HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
			StatusCode statusCode = StatusCode.ServerError;
			bool flag = true;
			if (context != null)
			{
				AirSyncUtility.LogCompressedStackTrace(ex, context);
				context.Response.XmlDocument = null;
				if (context.User != null)
				{
					text2 = context.User.ServerFullyQualifiedDomainName;
					text = context.User.Name;
				}
				AirSyncUtility.UpdateExceptionPerfCounter(ex);
				context.Response.Clear();
				context.Response.HttpStatusCode = HttpStatusCode.InternalServerError;
				if (GlobalSettings.WriteExceptionDiagnostics || (context.User != null && context.User.IsMonitoringTestUser))
				{
					context.Response.AppendHeader("X-ExceptionDiagnostics", ex.ToString());
				}
			}
			try
			{
				AirSyncUtility.UpdateAirSyncRequestCache((context != null && context.ActivityScope == null) ? default(Guid) : context.ActivityScope.ActivityId, ex, text, (context != null && context.Request != null && context.Request.CommandType != CommandType.Options) ? context.DeviceIdentity.ToString() : "Unknown device");
			}
			catch (Exception ex2)
			{
				AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, null, "Error while call UpdateAirSyncRequestCache! ex = ", ex2.ToString());
			}
			AirSyncUtility.UpdateExceptionPerfCounter(ex);
			bool flag2 = true;
			bool flag3 = true;
			bool flag4 = true;
			bool flag5 = false;
			AirSyncPermanentException ex3 = ex as AirSyncPermanentException;
			DirectoryOperationException ex4 = ex as DirectoryOperationException;
			for (Exception ex5 = (ex == null) ? null : ex.InnerException; ex5 != null; ex5 = ex5.InnerException)
			{
				RightsManagementException ex6 = ex5 as RightsManagementException;
				if (ex6 != null && (ex6.FailureCode == RightsManagementFailureCode.InternalLicensingDisabled || ex6.FailureCode == RightsManagementFailureCode.ExternalLicensingDisabled))
				{
					ex3 = new AirSyncPermanentException(StatusCode.IRM_FeatureDisabled, false);
					ex3.ErrorStringForProtocolLogger = "asuPe" + ex6.FailureCode.ToString();
				}
			}
			if (ex3 != null)
			{
				if (ex3.XmlResponse != null && context != null)
				{
					context.Response.HttpStatusCode = HttpStatusCode.OK;
					context.Response.XmlDocument = ex3.XmlResponse;
					flag = false;
				}
				else
				{
					httpStatusCode = ex3.HttpStatusCode;
					statusCode = ex3.AirSyncStatusCode;
				}
				flag2 = false;
				flag4 = ex3.LogExceptionToEventLog;
				text3 = ex3.ErrorStringForProtocolLogger;
				if (ex3.AirSyncStatusCode > StatusCode.None && context != null)
				{
					context.ProtocolLogger.SetValue(ProtocolLoggerData.StatusCode, (int)ex3.AirSyncStatusCode);
				}
				if (ex3 is DiscoveryInfoMissingException && flag4)
				{
					AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_DiscoveryInfoMissingError, text2, new string[]
					{
						text2,
						text
					});
					text3 = "DiscoveryInfoMissing";
					flag4 = false;
				}
			}
			else if (ex is MailboxInSiteFailoverException)
			{
				flag2 = false;
				flag4 = false;
				text3 = "MailboxInSiteFailoverException";
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.ServerErrorRetryLater;
			}
			else if (ex is MailboxCrossSiteFailoverException)
			{
				flag2 = false;
				flag4 = false;
				text3 = "MailboxCrossSiteFailoverException";
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.ServerErrorRetryLater;
			}
			else if (ex is MailboxOfflineException)
			{
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_MailboxOffline, new string[]
				{
					text2
				});
				text3 = "MailboxOffline";
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.MailboxServerOffline;
				flag4 = false;
				flag2 = false;
			}
			else if (ex is AccessDeniedException)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_MailboxAccessDenied, "MailboxAccessDenied: " + text2 + text, new string[]
				{
					text,
					text2
				});
				text3 = "AccessDenied";
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.AccessDenied;
				flag4 = false;
				flag2 = false;
			}
			else if (ex is AccountDisabledException)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_AccountDisabled, "AccountDisabled: " + text2 + text, new string[]
				{
					text,
					text2
				});
				text3 = "AccountDisabled";
				httpStatusCode = HttpStatusCode.Forbidden;
				statusCode = StatusCode.AccountDisabled;
				flag4 = false;
				flag2 = false;
			}
			else if (ex is QuotaExceededException)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_MailboxQuotaExceeded, "MailboxQuotaExceeded: " + text2 + text, new string[]
				{
					text,
					text2
				});
				text3 = "QuotaExceeded";
				httpStatusCode = (HttpStatusCode)507;
				statusCode = StatusCode.MailboxQuotaExceeded;
				flag4 = false;
				flag2 = false;
			}
			else if (ex is WrongServerException)
			{
				if (context != null)
				{
					if (!context.User.IsMonitoringTestUser)
					{
						AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_MailboxConnectionFailed, text, new string[]
						{
							text,
							text2,
							ex.ToString()
						});
					}
					context.Response.AppendHeader("X-BEServerException", "Microsoft.Exchange.Data.Storage.IllegalCrossServerConnectionException");
					string value = ((WrongServerException)ex).RightServerToString();
					if (!string.IsNullOrEmpty(value))
					{
						context.Response.AppendHeader(WellKnownHeader.XDBMountedOnServer, value);
					}
				}
				text3 = "WrongServerExceptionGoingTo+" + text2;
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.ServerErrorRetryLater;
				flag4 = false;
				flag2 = false;
			}
			else if (ex is ConnectionFailedTransientException)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_ConnectionFailed, text2, new string[]
				{
					text2,
					ex.ToString()
				});
				text3 = "ConnectionFailedTransient";
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.ServerErrorRetryLater;
				flag4 = false;
				flag2 = false;
			}
			else if (ex is InvalidLicenseException)
			{
				text3 = "InvalidLicenseException";
				httpStatusCode = HttpStatusCode.Forbidden;
				statusCode = StatusCode.AccessDenied;
				flag4 = false;
				flag2 = false;
			}
			else if (ex is ConnectionFailedPermanentException)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_MailboxConnectionFailed, text, new string[]
				{
					text,
					text2,
					ex.ToString()
				});
				text3 = "ConnectionFailedPermanent";
				httpStatusCode = HttpStatusCode.InternalServerError;
				statusCode = StatusCode.ServerError;
				flag4 = false;
				flag2 = false;
			}
			else if (ex is WbxmlException)
			{
				flag2 = false;
				flag4 = false;
				text3 = "InvalidWBXML";
				httpStatusCode = HttpStatusCode.BadRequest;
				statusCode = StatusCode.InvalidWBXML;
			}
			else if (ex is CorruptSyncStateException)
			{
				httpStatusCode = HttpStatusCode.InternalServerError;
				statusCode = StatusCode.SyncStateCorrupt;
				flag4 = true;
				flag2 = false;
				CorruptSyncStateException ex7 = ex as CorruptSyncStateException;
				string arg = (ex7 != null) ? ex7.SyncStateName : "<NoName>";
				string arg2 = (ex.InnerException != null) ? ex.InnerException.GetType().Name : "<NoInnerEx>";
				text3 = string.Format("CorruptSyncState_{0}_{1}", arg, arg2);
			}
			else if (ex is SyncStateNotFoundException)
			{
				text3 = "SyncStateNotFound";
				httpStatusCode = HttpStatusCode.InternalServerError;
				statusCode = StatusCode.SyncStateNotFound;
				flag4 = false;
				flag2 = false;
			}
			else if (ex is SyncStateExistedException)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_SyncStateExisted, "SyncStateExistedException: " + text + ex.Message, new string[]
				{
					ex.ToString()
				});
				text3 = "SyncStateExisted";
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.SyncStateAlreadyExists;
				flag4 = true;
				flag2 = false;
			}
			else if (ex is InvalidSyncStateVersionException)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_InvalidSyncStateVersion, "InvalidSyncStateVersion: " + text + ex.Message, new string[]
				{
					ex.ToString()
				});
				if (AirSyncUtility.TryDeleteAllSyncStateIfNeeded(Command.CurrentCommand.SyncStateStorage, out text3))
				{
					context.Response.SetErrorResponse(HttpStatusCode.ServiceUnavailable, StatusCode.ServerErrorRetryLater);
					if (string.IsNullOrEmpty(text3))
					{
						text3 = "InvalidSyncState:Retry";
					}
					httpStatusCode = HttpStatusCode.ServiceUnavailable;
					statusCode = StatusCode.ServerErrorRetryLater;
				}
				else
				{
					text3 = "InvalidSyncState:BackendIsNewer";
					httpStatusCode = HttpStatusCode.InternalServerError;
					statusCode = StatusCode.SyncStateVersionInvalid;
				}
				flag4 = true;
				flag2 = false;
			}
			else if (ex is StorageTransientException)
			{
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.ServerErrorRetryLater;
				flag4 = false;
				flag2 = false;
				text3 = AirSyncUtility.BuildErrorStringFromException(ex, "StorageTransient");
			}
			else if (ex is StoragePermanentException)
			{
				httpStatusCode = HttpStatusCode.InternalServerError;
				statusCode = StatusCode.ServerError;
				flag4 = true;
				flag2 = false;
				text3 = AirSyncUtility.BuildErrorStringFromException(ex, "StoragePermanent");
			}
			else if (ex is ServiceDiscoveryTransientException)
			{
				text3 = "ServiceDiscoveryTransient";
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.ServerErrorRetryLater;
				flag4 = false;
				flag2 = false;
			}
			else if (ex is ServiceDiscoveryPermanentException)
			{
				text3 = "ServiceDiscoveryPermanent";
				httpStatusCode = HttpStatusCode.InternalServerError;
				statusCode = StatusCode.ServerError;
				flag4 = true;
				flag2 = false;
			}
			else if (ex is MapiExceptionExiting)
			{
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_MailboxOffline, new string[]
				{
					text2
				});
				flag2 = false;
				flag4 = false;
				text3 = "MapiExiting";
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.MailboxServerOffline;
			}
			else if (ex is ADTransientException)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_ActiveDirectoryTransientError, "ActiveDirectoryTransientError: " + text2 + text, new string[]
				{
					text,
					ex.ToString()
				});
				flag2 = false;
				flag4 = false;
				text3 = "ADTransient";
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.ServerErrorRetryLater;
			}
			else if (ex is ADOperationException)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_ActiveDirectoryOperationError, "ActiveDirectoryOperationError: " + text2 + text, new string[]
				{
					text,
					ex.ToString()
				});
				flag2 = false;
				flag4 = false;
				text3 = "ADOperation";
				httpStatusCode = HttpStatusCode.InternalServerError;
				statusCode = StatusCode.ServerError;
			}
			else if (ex is ADExternalException)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_ActiveDirectoryExternalError, "ActiveDirectoryExternalError: " + text2 + text, new string[]
				{
					text,
					ex.ToString()
				});
				text3 = "ADExternalException";
				httpStatusCode = HttpStatusCode.InternalServerError;
				statusCode = StatusCode.ServerError;
				flag4 = false;
				flag2 = false;
			}
			else if (ex is DataValidationException)
			{
				text3 = "DataValidation";
				httpStatusCode = HttpStatusCode.Forbidden;
				statusCode = StatusCode.AccessDenied;
				flag3 = true;
				flag4 = true;
				flag2 = false;
			}
			else if (ex4 != null)
			{
				DirectoryResponse response = ex4.Response;
				ResultCode resultCode = response.ResultCode;
				ResultCode resultCode2 = resultCode;
				if (resultCode2 == ResultCode.InsufficientAccessRights)
				{
					flag2 = false;
					flag4 = true;
					text3 = "DirectoryInsufficientAccessRights";
					httpStatusCode = HttpStatusCode.Forbidden;
					statusCode = StatusCode.ActiveDirectoryAccessDenied;
				}
				else
				{
					text3 = "DirectoryFailure";
					httpStatusCode = HttpStatusCode.InternalServerError;
					statusCode = StatusCode.ServerError;
				}
			}
			else if (ex is ResourceUnhealthyException)
			{
				flag2 = false;
				flag4 = false;
				text3 = "ResourceUnhealthy";
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.ServerErrorRetryLater;
			}
			else if (ex is TransientException)
			{
				flag2 = false;
				flag4 = false;
				text3 = "TransientError";
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.ServerErrorRetryLater;
			}
			else if (ex is HttpException)
			{
				flag2 = false;
				if (((HttpException)ex).WebEventCode == 3004)
				{
					flag4 = false;
					text3 = "MaxRequestLengthExceeded";
					statusCode = StatusCode.SendQuotaExceeded;
				}
				else
				{
					flag4 = true;
					text3 = "HttpLayerFailure";
					statusCode = StatusCode.ServerError;
				}
				httpStatusCode = HttpStatusCode.InternalServerError;
			}
			else if (ex is ConversionException)
			{
				flag2 = true;
				flag4 = false;
				text3 = "SchemaConversionFailure";
				httpStatusCode = HttpStatusCode.InternalServerError;
				statusCode = StatusCode.ServerError;
			}
			else if (ex is OverBudgetException)
			{
				flag2 = false;
				flag4 = false;
				text3 = "OverBudget";
				if (context != null)
				{
					TimeSpan timeSpan = TimeSpan.FromMilliseconds((double)(ex as OverBudgetException).BackoffTime);
					if (timeSpan.TotalSeconds > 0.0 && context.Request.Version < 140)
					{
						context.Response.AppendHeader("Retry-After", timeSpan.TotalSeconds.ToString(), false);
					}
					if (timeSpan.TotalSeconds > 0.0)
					{
						BackOffValue backOffValue = new BackOffValue
						{
							BackOffDuration = timeSpan.TotalSeconds,
							BackOffReason = "OutOfBudget",
							BackOffType = BackOffType.High
						};
						context.Response.AppendHeader("X-MS-BackOffDuration", backOffValue.ToString(), false);
						if (GlobalSettings.AddBackOffReasonHeader)
						{
							context.Response.AppendHeader("X-MS-BackOffReason", "OutOfBudget");
						}
						context.ProtocolLogger.SetValue(ProtocolLoggerData.SuggestedBackOffValue, backOffValue.ToString());
						context.ProtocolLogger.SetValue(ProtocolLoggerData.BackOffReason, "OutOfBudget");
						AirSyncDiagnostics.TraceInfo<double, string>(ExTraceGlobals.RequestsTracer, null, "[AirSyncUtility.ProcessException]. Add BackOffReason & Header. BackOffReason:{0}, BackOffDuration:{1}", timeSpan.TotalSeconds, "OutOfBudget");
					}
					AirSyncUtility.RecordDeviceBehavior(thisObject, context, AutoblockThresholdType.OutOfBudgets);
				}
				httpStatusCode = HttpStatusCode.ServiceUnavailable;
				statusCode = StatusCode.ServerErrorRetryLater;
			}
			else if (ex is AirSyncFatalException)
			{
				AirSyncFatalException ex8 = (AirSyncFatalException)ex;
				flag5 = true;
				flag4 = true;
				flag3 = true;
				flag2 = ex8.WatsonReportEnabled;
				text3 = (string.IsNullOrEmpty(ex8.LoggerString) ? "AirSyncFatalException" : ex8.LoggerString);
				flag = false;
			}
			else
			{
				flag5 = AirSyncUtility.IsCriticalException(ex);
				flag2 = true;
				flag3 = true;
				text3 = ex.GetType().FullName;
				flag4 = flag5;
				if (!flag5)
				{
					httpStatusCode = HttpStatusCode.InternalServerError;
					statusCode = StatusCode.ServerError;
				}
				else
				{
					flag = false;
				}
			}
			if (context != null)
			{
				if (flag)
				{
					context.Response.SetErrorResponse(httpStatusCode, statusCode);
				}
				if (context.ProtocolLogger != null && text3 != null)
				{
					context.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, text3);
				}
			}
			MailboxLogger mailboxLogger = (Command.CurrentCommand != null) ? Command.CurrentCommand.MailboxLogger : null;
			if (mailboxLogger != null)
			{
				RightsManagementException ex9 = ex.InnerException as RightsManagementException;
				if (ex9 != null)
				{
					mailboxLogger.SetData(MailboxLogDataName.IRM_FailureCode, ex9.FailureCode);
				}
				else if (ex.InnerException is ExchangeConfigurationException)
				{
					mailboxLogger.SetData(MailboxLogDataName.IRM_FailureCode, "asuPeExchangeConfigurationException");
				}
			}
			if (context != null)
			{
				context.SetDiagnosticValue(ConditionalHandlerSchema.Exception, ex);
				context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.EasStatus, statusCode);
				context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.HttpStatus, httpStatusCode);
			}
			if (flag3 || flag4)
			{
				string text4 = AirSyncUtility.ExceptionToString(ex);
				if (flag3)
				{
					AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, thisObject, "{0}", text4);
				}
				if (flag4)
				{
					text4 = AirSyncUtility.ExceptionToLocString(ex);
					if (flag5)
					{
						if (ex is AirSyncFatalException)
						{
							AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_AirSyncFatalException, new string[]
							{
								text4
							});
						}
						else
						{
							AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_AirSyncUnhandledException, new string[]
							{
								empty,
								text4
							});
						}
					}
					else
					{
						AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_AirSyncException, new string[]
						{
							empty,
							text4
						});
					}
				}
			}
			if (GlobalSettings.SendWatsonReport && flag2)
			{
				AirSyncDiagnostics.SendWatson(ex, flag5);
				if (context != null)
				{
					AirSyncUtility.RecordDeviceBehavior(thisObject, context, AutoblockThresholdType.Watsons);
				}
			}
			if (flag5)
			{
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					throw new AirSyncFatalException(EASServerStrings.UnhandledException, "RethrowUnhandledException", false, ex);
				});
			}
			IncorrectUrlRequestException ex10 = ex as IncorrectUrlRequestException;
			if (context != null && ex10 != null && ex10.HeaderName != null && ex10.HeaderValue != null)
			{
				context.Response.AppendHeader(ex10.HeaderName, ex10.HeaderValue);
			}
		}

		private static string BuildErrorStringFromException(Exception ex, string prefix)
		{
			StringBuilder stringBuilder = new StringBuilder(prefix);
			for (Exception ex2 = ex; ex2 != null; ex2 = ex2.InnerException)
			{
				RightsManagementException ex3 = ex2 as RightsManagementException;
				if (ex3 != null)
				{
					stringBuilder.Append(": " + ex3.FailureCode.ToString());
				}
				else
				{
					stringBuilder.Append(": " + ex2.GetType().Name);
				}
			}
			return stringBuilder.ToString();
		}

		internal static bool IsCriticalException(Exception ex)
		{
			return ex is OutOfMemoryException || ex is StackOverflowException || ex is AccessViolationException || ex is InvalidProgramException || ex is CannotUnloadAppDomainException || ex is BadImageFormatException || ex is TypeInitializationException || ex is TypeLoadException;
		}

		internal static string ExceptionToString(Exception ex)
		{
			Exception ex2 = ex;
			StringBuilder stringBuilder = new StringBuilder(512);
			stringBuilder.Append("--- Exception start ---\r\n");
			int num = 0;
			while (ex2 != null)
			{
				stringBuilder.AppendFormat("Exception type: {0}\r\n", ex2.GetType().FullName);
				stringBuilder.AppendFormat("Exception message: {0}\r\n", ex2.Message);
				stringBuilder.AppendFormat("Exception level: {0}\r\n", num.ToString(CultureInfo.InvariantCulture));
				if (ex2 is AirSyncPermanentException)
				{
					AirSyncPermanentException ex3 = ex2 as AirSyncPermanentException;
					stringBuilder.AppendFormat("HttpStatusCode: {0}\r\n", ex3.HttpStatusCode);
					stringBuilder.AppendFormat("AirSyncStatusCode: {0}\r\n", ex3.AirSyncStatusCode);
					stringBuilder.AppendFormat("ProtocolLoggerString: {0}\r\n", ex3.ErrorStringForProtocolLogger);
					stringBuilder.AppendFormat("XmlResponse: \r\n{0}\r\n", (ex3.XmlResponse == null) ? "[No XmlResponse]" : AirSyncUtility.BuildOuterXml(ex3.XmlResponse));
				}
				stringBuilder.AppendFormat("Exception stack trace: {0}\r\n", ex2.StackTrace);
				ex2 = ex2.InnerException;
				if (ex2 != null)
				{
					stringBuilder.Append("Inner exception follows...\r\n");
				}
				num++;
			}
			stringBuilder.Append("--- Exception end ---");
			return stringBuilder.ToString();
		}

		internal static void UpdateAirSyncRequestCache(Guid requestId, Exception ex, string userEMail, string deviceID)
		{
			ErrorDetail errorDetail = new ErrorDetail();
			errorDetail.ErrorType = ex.GetType().ToString();
			errorDetail.ErrorMessage = ex.Message;
			errorDetail.StackTrace = ExceptionTools.GetCompressedStackTrace(ex);
			errorDetail.DeviceID = deviceID;
			errorDetail.UserEmail = userEMail;
			ActiveSyncRequestData activeSyncRequestData = ActiveSyncRequestCache.Instance.Get(requestId);
			activeSyncRequestData.HasErrors = true;
			if (activeSyncRequestData.ErrorDetails == null)
			{
				activeSyncRequestData.ErrorDetails = new List<ErrorDetail>();
			}
			activeSyncRequestData.ErrorDetails.Add(errorDetail);
		}

		internal static void EnableOutlookExtensionsFeature(byte[] outlookExtensions, OutlookExtension feature)
		{
			int num = (OutlookExtension)outlookExtensions.Length - feature / OutlookExtension.TrueMessageRead - OutlookExtension.SystemCategories;
			if (num < 0)
			{
				return;
			}
			bool flag;
			if (!Constants.FeatureAccessMap.TryGetValue(feature, out flag))
			{
				return;
			}
			int num2 = (int)(feature % OutlookExtension.TrueMessageRead);
			if (flag && (outlookExtensions[num] & (byte)(1 << num2)) != 1)
			{
				int num3 = num;
				outlookExtensions[num3] |= (byte)(1 << num2);
			}
		}

		public static string HtmlEncode(string input, bool useNamedEntities = false)
		{
			return AntiXssEncoder.HtmlEncode(input, useNamedEntities);
		}

		public static string HtmlEncode(LocalizedString input, CultureInfo cultureInfo, bool useNamedEntities = false)
		{
			return AntiXssEncoder.HtmlEncode(input.ToString(cultureInfo), useNamedEntities);
		}

		private static void BuildOuterXml(StringBuilder builder, XmlNode node, int tabCount, string defaultNamespace)
		{
			AirSyncUtility.BuildOuterXml(builder, node, tabCount, defaultNamespace, false);
		}

		private static void BuildOuterXml(StringBuilder builder, XmlNode node, int tabCount, string defaultNamespace, bool truncatePII)
		{
			builder.Append("\r\n");
			for (int i = 0; i < tabCount; i++)
			{
				builder.Append("\t");
			}
			builder.Append("<");
			builder.Append(node.Name);
			if (truncatePII && "Body".Equals(node.Name, StringComparison.Ordinal))
			{
				if (node.HasChildNodes)
				{
					int num = -1;
					XmlNode xmlNode = node["EstimatedDataSize"];
					if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText) && !int.TryParse(xmlNode.InnerText, out num))
					{
						num = -1;
					}
					if (num == -1)
					{
						XmlNode xmlNode2 = node["Data"];
						if (xmlNode2 != null && xmlNode2.InnerText != null)
						{
							num = xmlNode2.InnerText.Length;
						}
					}
					if (num != -1)
					{
						builder.Append("=");
						builder.Append(num);
						builder.Append(" bytes");
					}
				}
				builder.Append("/>");
				return;
			}
			if (defaultNamespace == null || string.Compare(node.NamespaceURI, defaultNamespace, StringComparison.Ordinal) != 0)
			{
				builder.Append(" xmlns=\"");
				builder.Append(node.NamespaceURI);
				builder.Append("\"");
			}
			AirSyncBlobXmlNode airSyncBlobXmlNode = node as AirSyncBlobXmlNode;
			if (!node.HasChildNodes && (airSyncBlobXmlNode == null || (airSyncBlobXmlNode.ByteArray == null && airSyncBlobXmlNode.Stream == null)))
			{
				builder.Append("/>");
				return;
			}
			if (truncatePII)
			{
				foreach (string text in AirSyncUtility.PiiTags)
				{
					if (text.Equals(node.Name, StringComparison.Ordinal))
					{
						if (node.InnerText != null)
						{
							builder.Append(" bytes=\"");
							builder.Append(node.InnerText.Length);
							builder.Append("\"");
						}
						builder.Append("/>");
						return;
					}
				}
			}
			builder.Append(">");
			bool flag = false;
			if (airSyncBlobXmlNode != null)
			{
				if (airSyncBlobXmlNode.ByteArray != null)
				{
					builder.Append(HexConverter.ByteArrayToHexString(airSyncBlobXmlNode.ByteArray));
					flag = true;
				}
				else if (airSyncBlobXmlNode.Stream != null)
				{
					if (airSyncBlobXmlNode.Stream.CanSeek)
					{
						airSyncBlobXmlNode.Stream.Seek(0L, SeekOrigin.Begin);
					}
					StreamReader streamReader = new StreamReader(airSyncBlobXmlNode.Stream, Encoding.UTF8);
					builder.Append(streamReader.ReadToEnd());
					flag = true;
				}
			}
			else
			{
				foreach (object obj in node.ChildNodes)
				{
					XmlNode xmlNode3 = (XmlNode)obj;
					switch (xmlNode3.NodeType)
					{
					case XmlNodeType.Element:
						tabCount++;
						AirSyncUtility.BuildOuterXml(builder, xmlNode3, tabCount, node.NamespaceURI, truncatePII);
						tabCount--;
						continue;
					case XmlNodeType.Text:
						for (int k = 0; k < xmlNode3.Value.Length; k++)
						{
							char c = xmlNode3.Value[k];
							if (c != '"')
							{
								switch (c)
								{
								case '&':
									builder.Append("&amp;");
									break;
								case '\'':
									builder.Append("&apos;");
									break;
								default:
									switch (c)
									{
									case '<':
										builder.Append("&lt;");
										goto IL_35B;
									case '>':
										builder.Append("&gt;");
										goto IL_35B;
									}
									builder.Append(xmlNode3.Value[k]);
									break;
								}
							}
							else
							{
								builder.Append("&quot;");
							}
							IL_35B:;
						}
						flag = true;
						continue;
					}
					AirSyncDiagnostics.Assert(false, "Unexpected/unsupported XmlNodeType {0}!", new object[]
					{
						xmlNode3.NodeType
					});
					AirSyncDiagnostics.TraceError<XmlNodeType>(ExTraceGlobals.RequestsTracer, null, "AirSyncUtility.BuildOuterXml() Unexpected XmlNodeType {0}", xmlNode3.NodeType);
				}
			}
			if (!flag)
			{
				builder.Append("\r\n");
				for (int l = 0; l < tabCount; l++)
				{
					builder.Append("\t");
				}
			}
			builder.Append("</" + node.Name + ">");
		}

		private static void UpdateExceptionPerfCounter(Exception ex)
		{
			if (ex is StorageTransientException)
			{
				RatePerfCounters.IncrementExceptionPerfCounter(2);
				return;
			}
			if (ex is StoragePermanentException)
			{
				RatePerfCounters.IncrementExceptionPerfCounter(3);
				return;
			}
			if (ex is ADTransientException || ex is ServiceDiscoveryTransientException)
			{
				RatePerfCounters.IncrementExceptionPerfCounter(4);
				return;
			}
			if (ex is ServiceDiscoveryPermanentException || ex is ADOperationException || ex is ADExternalException)
			{
				RatePerfCounters.IncrementExceptionPerfCounter(5);
				return;
			}
			if (ex is ConnectionFailedTransientException)
			{
				RatePerfCounters.IncrementExceptionPerfCounter(0);
				return;
			}
			if (ex is MailboxOfflineException)
			{
				RatePerfCounters.IncrementExceptionPerfCounter(1);
				return;
			}
			if (ex is TransientException)
			{
				RatePerfCounters.IncrementExceptionPerfCounter(6);
			}
		}

		private static bool TryDeleteAllSyncStateIfNeeded(SyncStateStorage syncStateStorage, out string errorStringForProtocolLogger)
		{
			if (syncStateStorage == null)
			{
				throw new ArgumentNullException("syncStateStorage");
			}
			errorStringForProtocolLogger = "InvalidSyncStateVersion";
			try
			{
				if (syncStateStorage.DeleteAllSyncStatesIfMoved())
				{
					errorStringForProtocolLogger = "InvalidSyncStateVersion_MbxMoved";
					return true;
				}
				FolderCommand folderCommand = Command.CurrentCommand as FolderCommand;
				if (folderCommand != null)
				{
					FolderCommand.FolderRequest folderRequest = folderCommand.ParseRequest();
					if (folderRequest.SyncKey == 0)
					{
						AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "Folder sync 0 detected! Deleting all SyncState...");
						errorStringForProtocolLogger = "InvalidSyncStateVersion_FldSync0";
						syncStateStorage.DeleteAllSyncStates();
						return true;
					}
				}
				if (Command.CurrentCommand is ProvisionCommand)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "ProvisionCommand detected! Deleting all SyncState...");
					errorStringForProtocolLogger = "InvalidSyncStateVersion_Provision";
					syncStateStorage.DeleteAllSyncStates();
					return true;
				}
			}
			catch (Exception ex)
			{
				AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, null, "Error trying to open SyncStatusSyncState! ex = ", ex.ToString());
				errorStringForProtocolLogger = "FailedDeleteAllSyncState:" + ex.Message;
			}
			return false;
		}

		private static void RecordDeviceBehavior(object thisObject, IAirSyncContext context, AutoblockThresholdType autoblockThresholdType)
		{
			try
			{
				AirSyncDiagnostics.TraceDebug<AutoblockThresholdType>(ExTraceGlobals.RequestsTracer, thisObject, "RecordDeviceBehavior of type {0}", autoblockThresholdType);
				if (context.Request.CommandType == CommandType.Options)
				{
					AirSyncDiagnostics.TraceDebug<AutoblockThresholdType>(ExTraceGlobals.RequestsTracer, thisObject, "RecordDeviceBehavior Ignoring Options request {0}", autoblockThresholdType);
				}
				else
				{
					if (context.DeviceBehavior != null)
					{
						switch (autoblockThresholdType)
						{
						case AutoblockThresholdType.Watsons:
							context.DeviceBehavior.RecordWatson();
							break;
						case AutoblockThresholdType.OutOfBudgets:
							context.DeviceBehavior.RecordOutOfBudget();
							break;
						}
					}
					else
					{
						Command command = thisObject as Command;
						if (command != null && command.User != null)
						{
							DeviceBehavior deviceBehavior;
							if (DeviceBehaviorCache.TryGetValue(command.User.ADUser.OriginalId.ObjectGuid, context.Request.DeviceIdentity, out deviceBehavior))
							{
								deviceBehavior.ProtocolLogger = command.ProtocolLogger;
								switch (autoblockThresholdType)
								{
								case AutoblockThresholdType.Watsons:
									deviceBehavior.RecordWatson();
									break;
								case AutoblockThresholdType.OutOfBudgets:
									deviceBehavior.RecordOutOfBudget();
									break;
								}
								deviceBehavior.ProtocolLogger = null;
								goto IL_23D;
							}
							GlobalInfo globalInfo = command.GlobalInfo;
							GlobalInfo globalInfo2 = null;
							bool flag = false;
							deviceBehavior = command.Context.DeviceBehavior;
							try
							{
								if (deviceBehavior == null)
								{
									if (globalInfo == null)
									{
										if (command.MailboxSession == null)
										{
											AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, thisObject, "RDB:OpenMailboxSession");
											command.OpenMailboxSession(command.User);
										}
										if (command.SyncStateStorage == null)
										{
											AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, thisObject, "RDB:OpenSyncStorage");
											command.OpenSyncStorage(false);
											flag = true;
										}
										AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, thisObject, "RDB:GlobalInfo.LoadFromMailbox");
										globalInfo2 = GlobalInfo.LoadFromMailbox(command.MailboxSession, command.SyncStateStorage, command.ProtocolLogger);
										globalInfo = globalInfo2;
									}
									deviceBehavior = DeviceBehavior.GetDeviceBehavior(command.User.ADUser.OriginalId.ObjectGuid, context.Request.DeviceIdentity, globalInfo, thisObject, command.ProtocolLogger);
								}
								if (deviceBehavior != null)
								{
									switch (autoblockThresholdType)
									{
									case AutoblockThresholdType.Watsons:
										deviceBehavior.RecordWatson();
										break;
									case AutoblockThresholdType.OutOfBudgets:
										deviceBehavior.RecordOutOfBudget();
										break;
									}
								}
								goto IL_23D;
							}
							finally
							{
								if (globalInfo2 != null)
								{
									AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, thisObject, "RDB:globalInfo.SaveToMailbox,Dispose");
									try
									{
										globalInfo.SaveToMailbox();
									}
									catch (LocalizedException arg)
									{
										AirSyncDiagnostics.TraceError<LocalizedException>(ExTraceGlobals.RequestsTracer, thisObject, "Failed to save global info: {0}", arg);
									}
									globalInfo2.Dispose();
								}
								if (flag)
								{
									AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, thisObject, "RDB:CommitSyncStatusSyncState");
									command.CommitSyncStatusSyncState();
								}
							}
						}
						AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, thisObject, "Unable to record behavior - this object is not a Command.");
					}
					IL_23D:;
				}
			}
			catch (Exception ex)
			{
				if (!AirSyncUtility.HandleNonCriticalException(ex, false))
				{
					throw;
				}
			}
		}

		public static bool AreNotNullOrEmptyAndStartsWith(string str1, string str2)
		{
			return !string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2) && str1.StartsWith(str2, StringComparison.OrdinalIgnoreCase);
		}

		public static bool AreNotNullOrEmptyAndContains(string str1, string str2)
		{
			return !string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2) && str1.IndexOf(str2, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public static bool AreNotNullOrEmptyAndRegexMatches(string input, string pattern, RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)
		{
			return !string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(pattern) && Regex.IsMatch(input, pattern, regexOptions);
		}

		public static string ConvertSytemCategoryIdToKeywordsFormat(int systemCategoryId)
		{
			if (systemCategoryId % 2 != 1)
			{
				throw new ArgumentException("System category id must be odd");
			}
			return string.Format(CultureInfo.InvariantCulture, "__SystemCategory{0}__", new object[]
			{
				systemCategoryId
			});
		}

		public static void ReplaceOrAddNode(this XmlNode parentNode, string nodeToReplaceOrAdd, string newInnerText, string namespaceUri)
		{
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(parentNode.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("X", namespaceUri);
			XmlNode xmlNode = parentNode.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "X:{0}", new object[]
			{
				nodeToReplaceOrAdd
			}), xmlNamespaceManager);
			if (xmlNode != null)
			{
				xmlNode.InnerText = newInnerText;
				return;
			}
			XmlElement xmlElement = parentNode.OwnerDocument.CreateElement(nodeToReplaceOrAdd, namespaceUri);
			xmlElement.InnerText = newInnerText;
			parentNode.AppendChild(xmlElement);
		}

		public static void ReplaceAnnotationWithExtensionIfExists(XmlNode xmlNode, string annotationName, string annotationGroup, string namespaceUri = "WindowsLive:")
		{
			AnnotationsManager requestAnnotations = Command.CurrentCommand.RequestAnnotations;
			if (requestAnnotations.ContainsAnnotation(annotationName, annotationGroup))
			{
				xmlNode.ReplaceOrAddNode(annotationName, requestAnnotations.FetchAnnotation(annotationName, annotationGroup), namespaceUri);
			}
		}

		private const RegexOptions DeviceFilterRegexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;

		private static string[] piiTags = new string[]
		{
			"AccountName",
			"Alias",
			"Anniversary",
			"AssistantName",
			"AssistantPhoneNumber",
			"Email",
			"Name",
			"Birthday",
			"Body",
			"BusinessAddressCity",
			"BusinessAddressCountry",
			"BusinessAddressPostalCode",
			"BusinessAddressState",
			"BusinessAddressStreet",
			"BusinessFaxNumber",
			"BusinessPhoneNumber",
			"Business2PhoneNumber",
			"CarPhoneNumber",
			"Category",
			"CC",
			"Child",
			"CompanyMainPhone",
			"CompanyName",
			"CompressedRTF",
			"Rtf",
			"CustomerId",
			"Data",
			"Department",
			"DisplayTo",
			"Email1Address",
			"Email2Address",
			"Email3Address",
			"FileAs",
			"FirstName",
			"From",
			"GovernmentId",
			"HomeAddressCity",
			"HomeAddressCountry",
			"HomeFaxNumber",
			"HomePhoneNumber",
			"Home2PhoneNumber",
			"HomeAddressPostalCode",
			"HomeAddressState",
			"HomeAddressStreet",
			"IMAddress",
			"IMAddress2",
			"IMAddress3",
			"JobTitle",
			"LastName",
			"Location",
			"ManagerName",
			"MiddleName",
			"MIMEData",
			"MMS",
			"MobilePhoneNumber",
			"NickName",
			"OfficeLocation",
			"Organizer",
			"OrganizerEmail",
			"OrganizerName",
			"OtherAddressCity",
			"OtherAddressCountry",
			"OtherAddressPostalCode",
			"OtherAddressState",
			"OtherAddressStreet",
			"PagerNumber",
			"Picture",
			"Preview",
			"RadioPhoneNumber",
			"ReplyTo",
			"Sender",
			"Spouse",
			"Subject",
			"Suffix",
			"ThreadTopic",
			"Title",
			"To",
			"WebPage",
			"YomiCompanyName",
			"YomiFirstName",
			"YomiLastName"
		};

		private static MruDictionaryCache<OrganizationId, string> defaultAcceptedDomainTable = new MruDictionaryCache<OrganizationId, string>(5, 50000, 5);

		internal struct XsoFilters
		{
			internal static readonly ComparisonFilter NonHidden = new ComparisonFilter(ComparisonOperator.NotEqual, FolderSchema.IsHidden, true);

			internal static readonly ComparisonFilter MailFolder = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ContainerClass, "IPF.Note");

			internal static readonly NotFilter SpecialMailFolder = new NotFilter(new ExistsFilter(StoreObjectSchema.ContainerClass));

			internal static readonly OrFilter AllMailFolder = new OrFilter(new QueryFilter[]
			{
				AirSyncUtility.XsoFilters.MailFolder,
				AirSyncUtility.XsoFilters.SpecialMailFolder
			});

			internal static readonly AndFilter GetHierarchyFilter = new AndFilter(new QueryFilter[]
			{
				AirSyncUtility.XsoFilters.NonHidden,
				AirSyncUtility.XsoFilters.AllMailFolder
			});
		}

		public class ExceptionToStringHelper
		{
			public ExceptionToStringHelper(Exception ex)
			{
				this.exception = ex;
			}

			public override string ToString()
			{
				try
				{
					return AirSyncUtility.ExceptionToString(this.exception);
				}
				catch
				{
				}
				return "ExceptionToString() call failed";
			}

			private Exception exception;
		}

		internal static class FolderType
		{
			internal const string UserFolder = "1";

			internal const string StandardInbox = "2";

			internal const string StandardDrafts = "3";

			internal const string StandardDeletedItems = "4";

			internal const string StandardSentItems = "5";

			internal const string StandardOutbox = "6";

			internal const string StandardTasks = "7";

			internal const string StandardCalendar = "8";

			internal const string StandardContacts = "9";

			internal const string StandardNotes = "10";

			internal const string StandardJournal = "11";

			internal const string UserMail = "12";

			internal const string UserCalendar = "13";

			internal const string UserContacts = "14";

			internal const string UserTasks = "15";

			internal const string UserJournal = "16";

			internal const string UserNotes = "17";

			internal const string UnknownFolder = "18";

			internal const string RecipientInfoFolder = "19";

			internal const string SharedCalendar = "20";

			internal const string SharedContacts = "21";

			internal const string SharedTasks = "22";

			internal const string SharedJournal = "23";

			internal const string SharedNotes = "24";
		}
	}
}
