using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class InstantMessageUtilities
	{
		public static string GetExtraWatsonData(IUserContext userContext)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("OWA Version: ");
			stringBuilder.Append(Globals.ApplicationVersion);
			stringBuilder.AppendLine();
			if (userContext != null && !Globals.DisableBreadcrumbs)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(userContext.DumpBreadcrumbs());
			}
			return stringBuilder.ToString();
		}

		internal static void SetSignedOutFlag(MailboxSession session, bool signedOut)
		{
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					using (Folder folder = InstantMessageUtilities.SafeFolderBind(session, DefaultFolderType.Root, new PropertyDefinition[]
					{
						ViewStateProperties.SignedOutOfIM
					}))
					{
						if (folder != null)
						{
							folder[ViewStateProperties.SignedOutOfIM] = signedOut;
							folder.Save();
						}
					}
				});
			}
			catch (GrayException ex)
			{
				Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging.ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessageUtilities.SetSignedOutFlag failed. Exception message is {0}.", new object[]
				{
					ex
				});
			}
		}

		internal static bool IsSignedOut(MailboxSession session)
		{
			bool signedOut = false;
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					using (Folder folder = InstantMessageUtilities.SafeFolderBind(session, DefaultFolderType.Root, new PropertyDefinition[]
					{
						ViewStateProperties.SignedOutOfIM
					}))
					{
						if (folder != null)
						{
							bool defaultValue = false;
							signedOut = InstantMessageUtilities.GetFolderProperty<bool>(folder, ViewStateProperties.SignedOutOfIM, defaultValue);
						}
					}
				});
			}
			catch (GrayException ex)
			{
				Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging.ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessageUtilities.IsSignedOut failed. Exception message is {0}.", new object[]
				{
					ex
				});
			}
			return signedOut;
		}

		internal static string GetSipUri(string emailAddress, IUserContext userContext)
		{
			IRecipientSession recipientSession = InstantMessageUtilities.CreateADRecipientSession(Culture.GetUserCulture().LCID, true, ConsistencyMode.IgnoreInvalid, true, userContext.ExchangePrincipal, false, userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN);
			try
			{
				SmtpProxyAddress proxyAddress = new SmtpProxyAddress(emailAddress, true);
				ADRecipient adrecipient = recipientSession.FindByProxyAddress(proxyAddress);
				if (adrecipient != null)
				{
					string sipUri = ADPersonToContactConverter.GetSipUri(adrecipient);
					Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.InstantMessagingTracer.TraceDebug<string, string>(0L, "SIPUri for {0}: {1}", emailAddress, sipUri);
					return sipUri;
				}
			}
			catch (NonUniqueRecipientException ex)
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.InstantMessagingTracer.TraceError<string>(0L, "Failed to get unique recipient: Error: {0}", ex.Message);
			}
			Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.InstantMessagingTracer.TraceError<string>(0L, "Failed to get SIP Address for: {0}", emailAddress);
			return null;
		}

		internal static bool IsSipUri(string address)
		{
			return !string.IsNullOrWhiteSpace(address) && address.StartsWith("sip:");
		}

		internal static string ToSipFormat(string imAddress)
		{
			if (imAddress == null)
			{
				throw new ArgumentNullException("imAddress");
			}
			return InstantMessageUtilities.PrefixString("sip:", imAddress);
		}

		internal static string ToGroupFormat(string groupId)
		{
			if (groupId == null)
			{
				throw new ArgumentNullException("groupId");
			}
			return InstantMessageUtilities.PrefixString("grp:", groupId);
		}

		internal static string FromSipFormat(string sipAddress)
		{
			if (string.IsNullOrEmpty(sipAddress))
			{
				return string.Empty;
			}
			if (sipAddress.Length > "sip:".Length && string.Compare(sipAddress.Substring(0, "sip:".Length), "sip:", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return sipAddress.Substring("sip:".Length);
			}
			return sipAddress;
		}

		internal static void SendWatsonReport(string methodName, IUserContext userContext, Exception exception)
		{
			Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging.ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "{0} failed. {1}", new object[]
			{
				methodName,
				(exception.Message != null) ? exception.Message : string.Empty
			});
			InstantMessageUtilities.SendInstantMessageWatsonReport(userContext, exception);
		}

		internal static void SendInstantMessageWatsonReport(IUserContext userContext, Exception exception)
		{
			Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging.ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "Exception: Type: {0} Error: {1}.", new object[]
			{
				exception.GetType(),
				exception.Message
			});
			if (Globals.SendWatsonReports)
			{
				Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging.ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "Sending watson report");
				ReportOptions options = (exception is AccessViolationException || exception is InvalidProgramException || exception is TypeInitializationException) ? ReportOptions.ReportTerminateAfterSend : ReportOptions.None;
				ExWatson.AddExtraData(InstantMessageUtilities.GetExtraWatsonData(userContext));
				ExWatson.SendReport(exception, options, null);
			}
			if (exception is AccessViolationException)
			{
				Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging.ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "Shutting down OWA due to unrecoverable exception");
				ErrorHandlerUtilities.TerminateProcess();
				return;
			}
			if ((exception is InvalidProgramException || exception is TypeInitializationException) && Interlocked.Exchange(ref InstantMessageUtilities.queuedDelayedRestart, 1) == 0)
			{
				new Thread(new ThreadStart(InstantMessageUtilities.DelayedRestartUponUnexecutableCode)).Start();
			}
		}

		internal static HashSet<string> GetExpandedGroups(MailboxSession session)
		{
			string[] groupIds = null;
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					using (Folder folder = InstantMessageUtilities.SafeFolderBind(session, DefaultFolderType.Root, new PropertyDefinition[]
					{
						ViewStateProperties.ExpandedGroups
					}))
					{
						if (folder != null)
						{
							string[] defaultValue = null;
							groupIds = InstantMessageUtilities.GetFolderProperty<string[]>(folder, ViewStateProperties.ExpandedGroups, defaultValue);
							if (groupIds != null)
							{
								if (groupIds.Length == 1 && groupIds[0] == string.Empty)
								{
									groupIds = new string[0];
								}
							}
						}
					}
				});
			}
			catch (GrayException ex)
			{
				Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging.ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessageUtilities.GetExpandedGroups failed. Exception {0}.", new object[]
				{
					ex
				});
			}
			if (groupIds == null)
			{
				return null;
			}
			return new HashSet<string>(groupIds);
		}

		internal static ProxySettings[] GetProxySettings(string[] upns, IUserContext userContext)
		{
			List<ProxySettings> list = new List<ProxySettings>();
			IRecipientSession recipientSession = InstantMessageUtilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, userContext.ExchangePrincipal, userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN);
			Result<OWAMiniRecipient>[] array = recipientSession.FindOWAMiniRecipientByUserPrincipalName(upns);
			for (int i = 0; i < array.Length; i++)
			{
				OWAMiniRecipient data = array[i].Data;
				string text = data.UserPrincipalName.ToString();
				if (string.IsNullOrEmpty(text))
				{
					list.Add(new ProxySettings(text, null));
				}
				else
				{
					list.Add(new ProxySettings(text, InstantMessageUtilities.GetProxyAddressesForRecipient(data)));
				}
			}
			return list.ToArray();
		}

		internal static T ParseEnumValue<T>(string value, T defaultValue)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return defaultValue;
			}
			return (T)((object)Enum.Parse(typeof(T), value));
		}

		internal static IRecipientSession CreateADRecipientSession(ConsistencyMode consistencyMode, ExchangePrincipal exchangePrincipal, ADObjectId directorySearchRoot)
		{
			return InstantMessageUtilities.CreateADRecipientSession(true, consistencyMode, exchangePrincipal, directorySearchRoot);
		}

		private static string[] GetProxyAddressesForRecipient(OWAMiniRecipient owaMiniRecipient)
		{
			ProxyAddressCollection emailAddresses = owaMiniRecipient.EmailAddresses;
			string[] array;
			if (emailAddresses != null && emailAddresses.Count > 0)
			{
				array = new string[emailAddresses.Count];
				for (int i = 0; i < emailAddresses.Count; i++)
				{
					array[i] = emailAddresses[i].AddressString;
				}
			}
			else
			{
				array = new string[0];
			}
			return array;
		}

		private static string PrefixString(string prefix, string original)
		{
			if (original.Length > prefix.Length && string.Compare(original.Substring(0, prefix.Length), prefix, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return original;
			}
			return prefix + original;
		}

		private static StoreObjectId TryGetDefaultFolderId(MailboxSession session, DefaultFolderType type)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return session.GetDefaultFolderId(type);
		}

		private static Folder SafeFolderBind(MailboxSession mailboxSession, DefaultFolderType defaultFolderType, params PropertyDefinition[] returnProperties)
		{
			StoreObjectId storeObjectId = InstantMessageUtilities.TryGetDefaultFolderId(mailboxSession, defaultFolderType);
			Folder result = null;
			if (storeObjectId != null)
			{
				try
				{
					result = Folder.Bind(mailboxSession, defaultFolderType, returnProperties);
				}
				catch (ObjectNotFoundException ex)
				{
					Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging.ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "Failed to bind to folder: Error: {0}", new object[]
					{
						ex.Message
					});
				}
			}
			return result;
		}

		private static Folder SafeFolderBind(MailboxSession mailboxSession, StoreObjectId folderId, params PropertyDefinition[] returnProperties)
		{
			Folder result = null;
			try
			{
				result = Folder.Bind(mailboxSession, folderId, returnProperties);
			}
			catch (ObjectNotFoundException ex)
			{
				Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging.ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "Failed to bind to folder: Error: {0}", new object[]
				{
					ex.Message
				});
			}
			return result;
		}

		private static IRecipientSession CreateADRecipientSession(bool readOnly, ConsistencyMode consistencyMode, ExchangePrincipal exchangePrincipal, ADObjectId directorySearchRoot)
		{
			return InstantMessageUtilities.CreateADRecipientSession(CultureInfo.CurrentCulture.LCID, readOnly, consistencyMode, false, exchangePrincipal, true, directorySearchRoot);
		}

		private static IRecipientSession CreateADRecipientSession(int lcid, bool readOnly, ConsistencyMode consistencyMode, bool useDirectorySearchRoot, ExchangePrincipal exchangePrincipal, ADObjectId directorySearchRoot)
		{
			return InstantMessageUtilities.CreateADRecipientSession(lcid, readOnly, consistencyMode, useDirectorySearchRoot, exchangePrincipal, true, directorySearchRoot);
		}

		private static IRecipientSession CreateADRecipientSession(int lcid, bool readOnly, ConsistencyMode consistencyMode, bool useDirectorySearchRoot, ExchangePrincipal exchangePrincipal, bool scopeToGal, ADObjectId directorySearchRoot)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(exchangePrincipal.MailboxInfo.OrganizationId);
			adsessionSettings.AccountingObject = null;
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, useDirectorySearchRoot ? directorySearchRoot : null, lcid, readOnly, consistencyMode, null, adsessionSettings, 788, "CreateADRecipientSession", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\im\\InstantMessageUtilities.cs");
		}

		private static T GetFolderProperty<T>(Folder folder, PropertyDefinition propertyDefinition, T defaultValue)
		{
			object obj = folder.TryGetProperty(propertyDefinition);
			if (obj is PropertyError || obj == null)
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		private static void DelayedRestartUponUnexecutableCode()
		{
			Thread.Sleep(90000);
			OwaDiagnostics.Logger.LogEvent(ClientsEventLogConstants.Tuple_OwaRestartingAfterFailedLoad, string.Empty, new object[0]);
			ErrorHandlerUtilities.TerminateProcess();
		}

		internal const string DefaultMessageFormat = "text/plain;charset=utf-8";

		internal const int MaxConversationCount = 20;

		internal const int MailboxSessionLockTimeout = 30000;

		private const string SipPrefix = "sip:";

		private const string GroupPrefix = "grp:";

		private static int queuedDelayedRestart;
	}
}
