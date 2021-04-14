using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Tracking;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport.Logging.Search;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Tracking
{
	internal static class Utils
	{
		internal static bool AccessCheck(ADObjectId accessRequestedForId, ADObjectId executingUserId, IRecipientSession session, out string diagnosticText)
		{
			diagnosticText = string.Empty;
			if (executingUserId == null)
			{
				diagnosticText = "ExecutingUserId is null, access denied. BypassDelegateChecking must be specified to use message-tracking.";
				return false;
			}
			ADUser aduser = session.Read(executingUserId) as ADUser;
			if (aduser == null)
			{
				diagnosticText = string.Format("Could not find user with ID {0}.  Aborting authorization check.", executingUserId);
				return false;
			}
			if (executingUserId.Equals(accessRequestedForId))
			{
				ExTraceGlobals.TaskTracer.TraceDebug<string>(0L, "Authenticated user {0} tracking as self.", aduser.Alias);
				return true;
			}
			ADUser accessRequestedForADUser = (ADUser)session.Read(accessRequestedForId);
			bool flag = Utils.CheckFullAccessPermissions(aduser, accessRequestedForADUser, session);
			if (!flag)
			{
				diagnosticText = "Access check failed.";
			}
			return flag;
		}

		internal static bool CheckFullAccessPermissions(ADUser executingAdUser, ADUser accessRequestedForADUser, IRecipientSession session)
		{
			ExTraceGlobals.TaskTracer.TraceDebug<string, string>(0L, "Checking if {0} has full access for mailbox {1}", executingAdUser.Alias, accessRequestedForADUser.Alias);
			ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
			DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(accessRequestedForADUser.Database.ObjectGuid);
			RawSecurityDescriptor rawSecurityDescriptor = null;
			using (MapiMessageStoreSession mapiMessageStoreSession = new MapiMessageStoreSession(serverForDatabase.ServerLegacyDN, Server.GetSystemAttendantLegacyDN(serverForDatabase.ServerLegacyDN), Fqdn.Parse(serverForDatabase.ServerFqdn)))
			{
				MailboxId mailboxId = new MailboxId(new DatabaseId(accessRequestedForADUser.Database.ObjectGuid), accessRequestedForADUser.ExchangeGuid);
				try
				{
					rawSecurityDescriptor = mapiMessageStoreSession.GetMailboxSecurityDescriptor(mailboxId);
				}
				catch (MailboxNotFoundException)
				{
					ExTraceGlobals.TaskTracer.TraceDebug<MailboxId>(0L, "Could not find mailbox {0} when attempting to read its security descriptor.", mailboxId);
					return false;
				}
			}
			byte[] array = new byte[rawSecurityDescriptor.BinaryLength];
			rawSecurityDescriptor.GetBinaryForm(array, 0);
			ActiveDirectorySecurity activeDirectorySecurity = new ActiveDirectorySecurity();
			activeDirectorySecurity.SetSecurityDescriptorBinaryForm(array);
			int num = AuthzAuthorization.CheckGenericPermission(executingAdUser.Sid, rawSecurityDescriptor, AccessMask.CreateChild);
			return (num & 1) == 1;
		}

		internal static T FindById<T>(IIdentityParameter idParameter, IConfigDataProvider dataProvider) where T : IConfigurable, new()
		{
			IEnumerable<T> objects = idParameter.GetObjects<T>(null, dataProvider);
			T result;
			using (IEnumerator<T> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new ManagementObjectNotFoundException(Strings.ErrorManagementObjectNotFound(idParameter.ToString()));
				}
				result = enumerator.Current;
				if (enumerator.MoveNext())
				{
					throw new ManagementObjectAmbiguousException(Strings.ErrorManagementObjectAmbiguous(idParameter.ToString()));
				}
			}
			return result;
		}

		internal static ClientSecurityContext GetSecurityContextForUser(ISecurityAccessToken executingUser, DelegatedPrincipal delegatedPrincipal, ADUser trackedUser)
		{
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			ExTraceGlobals.TaskTracer.TraceDebug<string, string, bool>(0L, "executing-user={0}, tracked-user={1}, ismultitenancyenabled={2}", (executingUser != null) ? executingUser.UserSid.ToString() : delegatedPrincipal.ToString(), trackedUser.Sid.Value, enabled);
			if (!enabled || (executingUser != null && string.Equals(executingUser.UserSid, trackedUser.Sid.Value, StringComparison.OrdinalIgnoreCase)))
			{
				ExTraceGlobals.TaskTracer.TraceDebug(0L, "executing-user == tracked-user or we are not running in a Multi Tenant environment.");
				return new ClientSecurityContext(executingUser, AuthzFlags.AuthzSkipTokenGroups);
			}
			WindowsIdentity identity;
			try
			{
				ExTraceGlobals.TaskTracer.TraceDebug(0L, "executing-user != tracked-user");
				if (string.IsNullOrEmpty(trackedUser.UserPrincipalName))
				{
					ExTraceGlobals.TaskTracer.TraceError<ADObjectId>(0L, "Null/Empty UPN for user {0}", trackedUser.Id);
					Strings.TrackingErrorUserObjectCorrupt(trackedUser.Id.ToString(), "UserPrincipalName");
					string data = string.Format("Missing UserPrincipalName attribute for user {0}", trackedUser.Id.ToString());
					TrackingError trackingError = new TrackingError(ErrorCode.InvalidADData, string.Empty, data, string.Empty);
					throw new TrackingFatalException(trackingError, null, false);
				}
				identity = new WindowsIdentity(trackedUser.UserPrincipalName);
			}
			catch (UnauthorizedAccessException ex)
			{
				ExTraceGlobals.TaskTracer.TraceError<string, UnauthorizedAccessException>(0L, "Not authorized to get WindowsIdentity for {0}, Exception: {1}", trackedUser.UserPrincipalName, ex);
				TrackingError trackingError2 = new TrackingError(ErrorCode.UnexpectedErrorPermanent, string.Empty, string.Format("Cannot logon as {0}", trackedUser.Id.ToString()), ex.ToString());
				throw new TrackingFatalException(trackingError2, ex, false);
			}
			catch (SecurityException arg)
			{
				ExTraceGlobals.TaskTracer.TraceError<string, SecurityException>(0L, "Not authorized to get WindowsIdentity for {0}, falling back to ExecutingUser, Exception: {1}", trackedUser.UserPrincipalName, arg);
				return new ClientSecurityContext(executingUser, AuthzFlags.AuthzSkipTokenGroups);
			}
			return new ClientSecurityContext(identity);
		}

		internal static bool AreAnyErrorsLocalToThisForest(List<TrackingError> errors)
		{
			if (errors != null)
			{
				foreach (TrackingError trackingError in errors)
				{
					if (StringComparer.OrdinalIgnoreCase.Equals(ServerCache.Instance.GetLocalServer().Domain, trackingError.Domain))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal static void WriteDiagnostics(Task task, DiagnosticsContext diagnosticsContext, bool piiRedactionEnabled)
		{
			StringBuilder stringBuilder = null;
			List<ICollection<KeyValuePair<string, object>>> data = diagnosticsContext.Data;
			if (data == null)
			{
				return;
			}
			bool flag = ServerCache.Instance.WriteToStatsLogs && ServerCache.Instance.HostId == HostId.ECPApplicationPool;
			foreach (ICollection<KeyValuePair<string, object>> collection in data)
			{
				if (flag)
				{
					CommonDiagnosticsLog.Instance.LogEvent(CommonDiagnosticsLog.Source.DeliveryReports, collection);
				}
				else
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(256);
					}
					int count = collection.Count;
					int num = 0;
					foreach (KeyValuePair<string, object> keyValuePair in collection)
					{
						string arg;
						if (piiRedactionEnabled && Utils.DiagnosticKeysToRedact.Contains(keyValuePair.Key))
						{
							arg = Utils.RedactPiiString(keyValuePair.Value as string, task);
						}
						else
						{
							arg = (keyValuePair.Value as string);
						}
						stringBuilder.AppendFormat("{0}={1}", keyValuePair.Key, arg);
						if (++num < count)
						{
							stringBuilder.Append(",");
						}
					}
					string text = stringBuilder.ToString();
					task.WriteDebug(text);
					stringBuilder.Length = 0;
					ExTraceGlobals.TaskTracer.TraceDebug<string>(0L, "DEBUG: {0}", text);
				}
			}
		}

		internal static void HandleTrackingException(DirectoryContext directoryContext, TrackingBaseException ex, Task task, bool isMultiMessageSearch, bool isRunningAsAdmin)
		{
			if (directoryContext == null)
			{
				ErrorCodeInformationAttribute errorCodeInformationAttribute;
				ErrorCode errorCode;
				string value = ex.TrackingError.ToErrorMessage(isMultiMessageSearch, out errorCodeInformationAttribute, out errorCode);
				if (!errorCodeInformationAttribute.ShowToIWUser && isRunningAsAdmin)
				{
					return;
				}
				Utils.HandleError(task, ex.ToString(), new LocalizedString(value), isRunningAsAdmin || errorCodeInformationAttribute.ShowDetailToIW, ex is TrackingTransientException, ErrorCategory.NotSpecified, null);
			}
			if (!ex.IsAlreadyLogged)
			{
				directoryContext.Errors.Errors.Add(ex.TrackingError);
			}
		}

		internal static void HandleError(Task task, string debugMessage, LocalizedString message, bool overrideDefaultMessage, bool isTransientError, ErrorCategory errorCategory, object target)
		{
			ExTraceGlobals.SearchLibraryTracer.TraceError((long)task.GetHashCode(), debugMessage);
			task.WriteDebug(debugMessage);
			LocalizedString message2;
			if (overrideDefaultMessage)
			{
				message2 = message;
			}
			else if (isTransientError)
			{
				message2 = Strings.TrackingTransientError;
			}
			else
			{
				message2 = Strings.TrackingPermanentError;
			}
			task.WriteError(new TrackingLocalizedException(message2), errorCategory, target);
		}

		internal static void WriteWarnings(Task task, DirectoryContext directoryContext, bool isAdminMode, List<TrackingError> errors)
		{
			SearchMessageTrackingReport searchMessageTrackingReport = task as SearchMessageTrackingReport;
			bool flag = searchMessageTrackingReport != null && !searchMessageTrackingReport.IsOwaJumpOffPointRequest;
			StringBuilder stringBuilder = null;
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			foreach (TrackingError trackingError in errors)
			{
				ErrorCodeInformationAttribute errorCodeInformationAttribute = null;
				ErrorCode errorCode;
				string text = trackingError.ToErrorMessage(flag, out errorCodeInformationAttribute, out errorCode);
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				else
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(trackingError.ErrorCode);
				if (!(task is GetMessageTrackingReport) || errorCode != ErrorCode.LegacySender)
				{
					if (task.IsDebugOn)
					{
						task.WriteDebug(trackingError.ToString());
					}
					if (!string.IsNullOrEmpty(text) && !hashSet.Contains(text))
					{
						hashSet.Add(text);
						if (isAdminMode || errorCodeInformationAttribute.ShowDetailToIW)
						{
							task.WriteWarning(text);
						}
						else if (errorCodeInformationAttribute.ShowToIWUser)
						{
							if (errorCodeInformationAttribute.IsTransientError)
							{
								task.WriteWarning(flag ? Strings.TrackingTransientErrorMultiMessageSearch : Strings.TrackingTransientError);
							}
							else
							{
								task.WriteWarning(flag ? Strings.TrackingPermanentErrorMultiMessageSearch : Strings.TrackingPermanentError);
							}
						}
						if (!isAdminMode)
						{
							break;
						}
					}
				}
			}
			if (directoryContext != null && stringBuilder != null)
			{
				directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Errors, stringBuilder.ToString());
			}
		}

		internal static HashSet<string> GetAllSmtpProxiesForRecipientFilters(ICollection<string> filters, IRecipientSession session)
		{
			HashSet<string> hashSet = null;
			if (filters == null || filters.Count == 0)
			{
				return null;
			}
			int num = Math.Min(filters.Count, 256);
			List<ProxyAddress> list = new List<ProxyAddress>(num);
			foreach (string text in filters)
			{
				if (!string.IsNullOrEmpty(text) && SmtpAddress.IsValidSmtpAddress(text) && SmtpAddress.NullReversePath != (SmtpAddress)text)
				{
					ExTraceGlobals.TaskTracer.TraceDebug<string>(0L, "Adding filter '{0}' for proxy lookup.", text);
					list.Add(ProxyAddress.Parse(text));
					if (list.Count > num)
					{
						ExTraceGlobals.TaskTracer.TraceDebug<int>(0L, "Not going to get proxy for all filters.  Total number of filters is {0}.", filters.Count);
						break;
					}
				}
			}
			if (list.Count == 0)
			{
				ExTraceGlobals.TaskTracer.TraceDebug(0L, "No filters need to be looked up, none are addresses.");
				return null;
			}
			ProxyAddress[] array = list.ToArray();
			Result<ADRawEntry>[] array2 = session.FindByProxyAddresses(array, Utils.emailAddressesProperty);
			if (array2 == null)
			{
				return null;
			}
			int i = 0;
			while (i < array2.Length)
			{
				Result<ADRawEntry> result = array2[i];
				if (result.Data == null)
				{
					goto IL_1A5;
				}
				ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)result.Data[ADRecipientSchema.EmailAddresses];
				if (proxyAddressCollection != null)
				{
					using (MultiValuedProperty<ProxyAddress>.Enumerator enumerator2 = proxyAddressCollection.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							ProxyAddress proxyAddress = enumerator2.Current;
							if (ProxyAddressPrefix.Smtp.Equals(proxyAddress.Prefix) && !string.Equals(proxyAddress.AddressString, list[i].AddressString, StringComparison.OrdinalIgnoreCase))
							{
								if (hashSet == null)
								{
									hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
								}
								hashSet.Add(proxyAddress.AddressString);
							}
						}
						goto IL_1C2;
					}
					goto IL_1A5;
				}
				IL_1C2:
				i++;
				continue;
				IL_1A5:
				ExTraceGlobals.TaskTracer.TraceDebug<ProxyAddress, ProviderError>(0L, "{0} cannot be found up due to {1}.", array[i], result.Error);
				goto IL_1C2;
			}
			return hashSet;
		}

		internal static TimeSpan GetTimeout(bool isAdmin)
		{
			double value = (double)(isAdmin ? ServerCache.Instance.HelpdeskTimeoutSeconds : ServerCache.Instance.IWTimeoutSeconds);
			return TimeSpan.FromSeconds(value);
		}

		internal static string RedactPiiString(string original, Task context)
		{
			if (string.IsNullOrEmpty(original))
			{
				return original;
			}
			string original2;
			string redacted;
			string result = SuppressingPiiData.Redact(original, out original2, out redacted);
			Utils.AddPiiToMap(context.ExchangeRunspaceConfig, original2, redacted);
			return result;
		}

		internal static SmtpAddress RedactPiiSmtpAddress(SmtpAddress original, Task context)
		{
			if (SmtpAddress.Empty == original)
			{
				return original;
			}
			string original2;
			string redacted;
			SmtpAddress result = SuppressingPiiData.Redact(original, out original2, out redacted);
			Utils.AddPiiToMap(context.ExchangeRunspaceConfig, original2, redacted);
			return result;
		}

		internal static void RedactPiiStringArray(string[] original, Task context)
		{
			if (original == null)
			{
				return;
			}
			for (int i = 0; i < original.Length; i++)
			{
				original[i] = Utils.RedactPiiString(original[i], context);
			}
		}

		internal static void RedactEventData(KeyValuePair<string, object>[] eventData, HashSet<string> keysToRedact, Task context)
		{
			if (eventData == null)
			{
				return;
			}
			for (int i = 0; i < eventData.Length; i++)
			{
				if (keysToRedact.Contains(eventData[i].Key))
				{
					eventData[i] = new KeyValuePair<string, object>(eventData[i].Key, Utils.RedactPiiString(eventData[i].Value as string, context));
				}
			}
		}

		internal static void RedactRecipientTrackingEvents(RecipientTrackingEvent[] events, Task context)
		{
			if (events == null || events.Length == 0)
			{
				return;
			}
			foreach (RecipientTrackingEvent recipientTrackingEvent in events)
			{
				recipientTrackingEvent.RecipientAddress = Utils.RedactPiiSmtpAddress(recipientTrackingEvent.RecipientAddress, context);
				recipientTrackingEvent.RecipientDisplayName = Utils.RedactPiiString(recipientTrackingEvent.RecipientDisplayName, context);
			}
		}

		internal static bool TryResolveRedactedString(string redacted, Task context, out string resolved)
		{
			if (string.IsNullOrEmpty(redacted) || !PiiMapManager.ContainsRedactedPiiValue(redacted))
			{
				resolved = redacted;
				return false;
			}
			resolved = PiiMapManager.Instance.ResolveRedactedValue(redacted);
			if (resolved == redacted)
			{
				context.WriteWarning(string.Format("Failed to resolve redacted input parameter: {0}", redacted));
				return false;
			}
			return true;
		}

		internal static bool TryResolveRedactedStringArray(string[] redacted, Task context)
		{
			if (redacted == null || redacted.Length == 0)
			{
				return false;
			}
			bool result = false;
			for (int i = 0; i < redacted.Length; i++)
			{
				string text;
				if (Utils.TryResolveRedactedString(redacted[i], context, out text))
				{
					redacted[i] = text;
					result = true;
				}
			}
			return result;
		}

		internal static bool TryResolveRedactedSmtpAddress(SmtpAddress redacted, Task context, out SmtpAddress resolved)
		{
			string redacted2 = redacted.ToString();
			string address;
			if (Utils.TryResolveRedactedString(redacted2, context, out address))
			{
				resolved = new SmtpAddress(address);
				return true;
			}
			resolved = redacted;
			return false;
		}

		internal static bool TryResolveRedactedSmtpAddressArray(SmtpAddress[] redacted, Task context)
		{
			if (redacted == null || redacted.Length == 0)
			{
				return false;
			}
			bool result = false;
			for (int i = 0; i < redacted.Length; i++)
			{
				SmtpAddress smtpAddress;
				if (Utils.TryResolveRedactedSmtpAddress(redacted[i], context, out smtpAddress))
				{
					redacted[i] = smtpAddress;
					result = true;
				}
			}
			return result;
		}

		private static void AddPiiToMap(ExchangeRunspaceConfiguration config, string original, string redacted)
		{
			if (config != null)
			{
				if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(redacted))
				{
					return;
				}
				PiiMap orAdd = PiiMapManager.Instance.GetOrAdd(config.PiiMapId);
				if (orAdd != null)
				{
					orAdd[redacted] = original.ToString();
					return;
				}
			}
			else
			{
				ExTraceGlobals.TaskTracer.TraceDebug(0L, "Original value is not added to PII map. ExchangeRunspaceConfiguration is not set.");
			}
		}

		private static PropertyDefinition[] displayNameProperty = new PropertyDefinition[]
		{
			ADRecipientSchema.DisplayName
		};

		private static PropertyDefinition[] emailAddressesProperty = new PropertyDefinition[]
		{
			ADRecipientSchema.EmailAddresses
		};

		private static HashSet<string> DiagnosticKeysToRedact = new HashSet<string>
		{
			DiagnosticProperty.Usr.ToString()
		};
	}
}
