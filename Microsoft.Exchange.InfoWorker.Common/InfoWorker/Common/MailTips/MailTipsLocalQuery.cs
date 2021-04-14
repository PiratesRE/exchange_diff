using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.MailTips;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	internal class MailTipsLocalQuery : LocalQuery
	{
		internal MailTipsLocalQuery(ClientContext clientContext, DateTime deadline, IBudget callerBudget) : base(clientContext, deadline)
		{
			this.callerBudget = callerBudget;
		}

		internal override BaseQueryResult GetData(BaseQuery query)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			MailTipsQuery mailTipsQuery = (MailTipsQuery)query;
			mailTipsQuery.LatencyTracker = new Dictionary<string, long>(4);
			EmailAddress email = query.Email;
			MailTips mailTips = new MailTips(query.RecipientData);
			mailTips.Permission = mailTipsQuery.Permission;
			mailTips.MarkAsPending(MailTipTypes.OutOfOfficeMessage | MailTipTypes.MailboxFullStatus);
			int traceId = this.clientContext.GetHashCode();
			MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)traceId, "{0} / {1}: Getting mailbox MailTips...", TraceContext.Get(), mailTips.EmailAddress);
			MailTipsQueryResult mailTipsQueryResult = null;
			DateTime utcNow = DateTime.UtcNow;
			if (utcNow > this.deadline)
			{
				MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, TimeSpan>((long)traceId, "{0} / {1}: Timeout expired before opening mailbox session {2}", TraceContext.Get(), mailTips.EmailAddress, utcNow - this.deadline);
				return this.HandleException(email, stopwatch, mailTipsQuery, new TimeoutExpiredException("Opening-Mailbox-Session"));
			}
			MailboxSession session = null;
			bool outOfOfficeSuccess = false;
			bool mailboxFullSuccess = false;
			try
			{
				mailTipsQueryResult = this.RunUnderExceptionHandler(email, stopwatch, mailTipsQuery, delegate
				{
					Stopwatch stopwatch = Stopwatch.StartNew();
					ExchangePrincipal exchangePrincipal = mailTipsQuery.ExchangePrincipal;
					MailboxAccessInfo accessInfo = new MailboxAccessInfo(new WindowsPrincipal(WindowsIdentity.GetCurrent()));
					session = MailboxSession.ConfigurableOpen(exchangePrincipal, accessInfo, CultureInfo.InvariantCulture, "Client=MSExchangeRPC;Action=MailTips", LogonType.Admin, MailTipsLocalQuery.MailboxPropertyDefinitions, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.SuppressFolderIdPrefetch | MailboxSession.InitializationFlags.DeferDefaultFolderIdInitialization | MailboxSession.InitializationFlags.IgnoreForcedFolderInit, MailTipsLocalQuery.MailboxDefaultFolderTypes);
					session.AccountingObject = this.callerBudget;
					mailTipsQuery.LatencyTracker["OpenSession"] = stopwatch.ElapsedMilliseconds;
					stopwatch.Stop();
					MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, long>((long)traceId, "{0} / {1}: MailboxSession opened in {2} milliseconds", TraceContext.Get(), mailTips.EmailAddress, stopwatch.ElapsedMilliseconds);
					return null;
				});
				if (mailTipsQueryResult != null)
				{
					MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)traceId, "{0} / {1}: Unable to open mailbox session", TraceContext.Get(), mailTips.EmailAddress);
					return mailTipsQueryResult;
				}
				utcNow = DateTime.UtcNow;
				if (utcNow > this.deadline)
				{
					MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, TimeSpan>((long)traceId, "{0} / {1}: Timeout expired before getting mailbox-full {2}", TraceContext.Get(), mailTips.EmailAddress, utcNow - this.deadline);
					return this.HandleException(email, stopwatch, mailTipsQuery, new TimeoutExpiredException("Getting-MailboxFull"));
				}
				mailTipsQueryResult = this.RunUnderExceptionHandler(email, stopwatch, mailTipsQuery, delegate
				{
					mailboxFullSuccess = this.GetMailboxFullStatus(traceId, session, mailTips);
					mailTipsQuery.LatencyTracker["GetMailboxFull"] = stopwatch.ElapsedMilliseconds;
					return null;
				});
				if (mailTipsQueryResult != null)
				{
					MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)traceId, "{0} / {1}: Unable to get mailbox-full, but will try to get OOF.", TraceContext.Get(), mailTips.EmailAddress);
				}
				utcNow = DateTime.UtcNow;
				if (utcNow > this.deadline)
				{
					MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, TimeSpan>((long)traceId, "{0} / {1}: Timeout expired before getting out-of-office message {2}", TraceContext.Get(), mailTips.EmailAddress, utcNow - this.deadline);
					return this.HandleException(email, stopwatch, mailTipsQuery, new TimeoutExpiredException("Getting-OutOfOffice"));
				}
				mailTipsQueryResult = this.RunUnderExceptionHandler(email, stopwatch, mailTipsQuery, delegate
				{
					outOfOfficeSuccess = this.GetOutOfOfficeMessage(traceId, session, mailTips);
					mailTipsQuery.LatencyTracker["GetOOF"] = stopwatch.ElapsedMilliseconds;
					return null;
				});
				if (mailTipsQueryResult != null)
				{
					MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)traceId, "{0} / {1}: Unable to get OOF, returning an error.", TraceContext.Get(), mailTips.EmailAddress);
					return mailTipsQueryResult;
				}
			}
			finally
			{
				if (session != null)
				{
					session.Dispose();
				}
				mailTipsQuery.LatencyTracker["DisposeSession"] = stopwatch.ElapsedMilliseconds;
				if (mailTipsQueryResult != null)
				{
					mailTips.Exception = mailTipsQueryResult.ExceptionInfo;
				}
				MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, bool>((long)traceId, "{0} / {1}: OutOfOffice message success: {2}", TraceContext.Get(), mailTips.EmailAddress, outOfOfficeSuccess);
				MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, bool>((long)traceId, "{0} / {1}: MailboxFull success: {2}", TraceContext.Get(), mailTips.EmailAddress, mailboxFullSuccess);
				if (!outOfOfficeSuccess)
				{
					mailTips.MarkAsUnavailable(MailTipTypes.OutOfOfficeMessage);
				}
				if (!mailboxFullSuccess)
				{
					mailTips.MarkAsUnavailable(MailTipTypes.MailboxFullStatus);
				}
				MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)traceId, "{0} / {1}: Returning MailTipsQueryResult", TraceContext.Get(), mailTips.EmailAddress);
				mailTipsQueryResult = new MailTipsQueryResult(mailTips);
				stopwatch.Stop();
				MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, long>((long)traceId, "{0} / {1}: LocalQuery took {2} milliseconds", TraceContext.Get(), mailTips.EmailAddress, stopwatch.ElapsedMilliseconds);
			}
			return mailTipsQueryResult;
		}

		internal void ParseUserOofSettings(UserOofSettings oofSettings, MailTips mailTips, int traceId)
		{
			if (oofSettings.OofState == OofState.Disabled)
			{
				MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)traceId, "{0} / {1}: OutOfOffice is disabled", TraceContext.Get(), mailTips.EmailAddress);
				mailTips.OutOfOfficeMessage = string.Empty;
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (oofSettings.OofState == OofState.Scheduled && (utcNow > oofSettings.EndTime || utcNow < oofSettings.StartTime))
			{
				MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)traceId, "{0} / {1}: OutOfOffice is Scheduled, but we're outside the scheduled interval", TraceContext.Get(), mailTips.EmailAddress);
				mailTips.OutOfOfficeMessage = string.Empty;
				return;
			}
			ReplyBody replyBody;
			if (MailTipsAccessLevel.All != mailTips.Permission.AccessLevel)
			{
				MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)traceId, "{0} / {1}: MailTipsAccessLevel is not All, retrieving external auto reply.", TraceContext.Get(), mailTips.EmailAddress);
				replyBody = oofSettings.ExternalReply;
			}
			else if (this.clientContext is InternalClientContext)
			{
				MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)traceId, "{0} / {1}: InternalClientContext detected, retrieving internal auto reply.", TraceContext.Get(), mailTips.EmailAddress);
				replyBody = oofSettings.InternalReply;
			}
			else
			{
				ExternalClientContext externalClientContext = (ExternalClientContext)this.clientContext;
				if (mailTips.Configuration.Domains.IsInternal(externalClientContext.EmailAddress.Domain))
				{
					MailTipsLocalQuery.GetMailTipsTracer.TraceDebug((long)traceId, "{0} / {1}: ExternalClientContext detected and caller domain {2} is internal to recipient organization {3}, retrieving internal auto reply.", new object[]
					{
						TraceContext.Get(),
						mailTips.EmailAddress,
						externalClientContext.EmailAddress.Domain,
						mailTips.Configuration.OrganizationConfiguration.Configuration.OrganizationId
					});
					replyBody = oofSettings.InternalReply;
				}
				else
				{
					MailTipsLocalQuery.GetMailTipsTracer.TraceDebug((long)traceId, "{0} / {1}: ExternalClientContext detected and caller domain {2} is not internal to recipient organization {3}, retrieving external auto reply.", new object[]
					{
						TraceContext.Get(),
						mailTips.EmailAddress,
						externalClientContext.EmailAddress.Domain,
						mailTips.Configuration.OrganizationConfiguration.Configuration.OrganizationId
					});
					replyBody = oofSettings.ExternalReply;
				}
			}
			if (replyBody == null || replyBody.RawMessage == null || RuleGenerator.IsEmptyString(replyBody.RawMessage))
			{
				MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)traceId, "{0} / {1}: OutOfOfficeMessage is null or empty", TraceContext.Get(), mailTips.EmailAddress);
				mailTips.OutOfOfficeMessage = string.Empty;
				return;
			}
			MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, string>((long)traceId, "{0} / {1}: OutOfOffice message has been retrieved: {2}", TraceContext.Get(), mailTips.EmailAddress, replyBody.Message);
			string outOfOfficeMessage = MailTipsUtility.MakeSafeHtml(traceId, replyBody.Message);
			mailTips.OutOfOfficeMessage = outOfOfficeMessage;
			mailTips.OutOfOfficeMessageLanguage = replyBody.LanguageTag;
			if (oofSettings.OofState == OofState.Scheduled)
			{
				mailTips.OutOfOfficeDuration = oofSettings.Duration;
			}
		}

		private MailTipsQueryResult RunUnderExceptionHandler(EmailAddress emailAddress, Stopwatch stopwatch, MailTipsQuery mailTipsQuery, MailTipsLocalQuery.MailTipsLocalQueryDelegate method)
		{
			MailTipsQueryResult result;
			try
			{
				method();
				result = null;
			}
			catch (ConnectionFailedPermanentException exception)
			{
				result = this.HandleConnectionException(emailAddress, stopwatch, mailTipsQuery, exception);
			}
			catch (ObjectNotFoundException exception2)
			{
				result = this.HandleConnectionException(emailAddress, stopwatch, mailTipsQuery, exception2);
			}
			catch (ConnectionFailedTransientException exception3)
			{
				result = this.HandleConnectionException(emailAddress, stopwatch, mailTipsQuery, exception3);
			}
			catch (AccountDisabledException exception4)
			{
				result = this.HandleConnectionException(emailAddress, stopwatch, mailTipsQuery, exception4);
			}
			catch (VirusScanInProgressException innerException)
			{
				LocalizedString localizedString = Strings.descVirusScanInProgress(emailAddress.ToString());
				LocalizedException exception5 = new LocalizedException(localizedString, innerException);
				result = this.HandleException(emailAddress, stopwatch, mailTipsQuery, exception5);
			}
			catch (VirusDetectedException innerException2)
			{
				LocalizedString localizedString2 = Strings.descVirusDetected(emailAddress.ToString());
				LocalizedException exception6 = new LocalizedException(localizedString2, innerException2);
				result = this.HandleException(emailAddress, stopwatch, mailTipsQuery, exception6);
			}
			catch (AuthzException innerException3)
			{
				result = this.HandleException(emailAddress, stopwatch, mailTipsQuery, new Win32InteropException(innerException3));
			}
			catch (StoragePermanentException exception7)
			{
				result = this.HandleConnectionException(emailAddress, stopwatch, mailTipsQuery, exception7);
			}
			catch (StorageTransientException exception8)
			{
				result = this.HandleConnectionException(emailAddress, stopwatch, mailTipsQuery, exception8);
			}
			catch (ADTransientException exception9)
			{
				result = this.HandleConnectionException(emailAddress, stopwatch, mailTipsQuery, exception9);
			}
			catch (ADExternalException exception10)
			{
				result = this.HandleConnectionException(emailAddress, stopwatch, mailTipsQuery, exception10);
			}
			catch (TransientException exception11)
			{
				result = this.HandleConnectionException(emailAddress, stopwatch, mailTipsQuery, exception11);
			}
			return result;
		}

		private MailTipsQueryResult HandleConnectionException(EmailAddress emailAddress, Stopwatch stopwatch, MailTipsQuery mailTipsQuery, Exception exception)
		{
			MailTipsLocalQuery.GetMailTipsTracer.TraceError<object, EmailAddress, Exception>((long)this.GetHashCode(), "{0} / {1}: Database connection failed for mailbox, with exception {2}.", TraceContext.Get(), emailAddress, exception);
			this.LogEvent(emailAddress, stopwatch, mailTipsQuery, exception);
			MailTipsPerfCounters.IntraSiteMailTipsFailuresPerSecond.Increment();
			return new MailTipsQueryResult(new MailboxLogonFailedException(exception));
		}

		private MailTipsQueryResult HandleException(EmailAddress emailAddress, Stopwatch stopwatch, MailTipsQuery mailTipsQuery, LocalizedException exception)
		{
			MailTipsLocalQuery.GetMailTipsTracer.TraceError<object, EmailAddress, LocalizedException>((long)this.GetHashCode(), "{0} / {1}: Exception getting MailTips: {2}", TraceContext.Get(), emailAddress, exception);
			this.LogEvent(emailAddress, stopwatch, mailTipsQuery, exception);
			MailTipsPerfCounters.IntraSiteMailTipsFailuresPerSecond.Increment();
			return new MailTipsQueryResult(exception);
		}

		private void LogEvent(EmailAddress emailAddress, Stopwatch stopwatch, MailTipsQuery mailTipsQuery, Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder(mailTipsQuery.LatencyTracker.Count * 20 + 20);
			foreach (string text in mailTipsQuery.LatencyTracker.Keys)
			{
				stringBuilder.Append(text);
				stringBuilder.Append(":");
				stringBuilder.Append(mailTipsQuery.LatencyTracker[text]);
				stringBuilder.Append(",");
			}
			stringBuilder.Append(" total:");
			stringBuilder.Append(stopwatch.ElapsedMilliseconds);
			MailTips.Logger.LogEvent(InfoWorkerEventLogConstants.Tuple_MailTipsMailboxQueryFailed, exception.GetType().FullName, new object[]
			{
				Globals.ProcessId,
				emailAddress,
				stringBuilder,
				exception
			});
		}

		private bool GetOutOfOfficeMessage(int traceId, MailboxSession session, MailTips mailTips)
		{
			bool flag = false;
			MailTipsLocalQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)traceId, "{0} / {1}: Getting OutOfOffice", TraceContext.Get(), mailTips.EmailAddress);
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				object obj = session.Mailbox.TryGetProperty(MailboxSchema.MailboxOofState);
				bool flag2 = true;
				if (!(obj is PropertyError))
				{
					flag2 = (bool)obj;
				}
				if (flag2)
				{
					UserOofSettings userOofSettings = UserOofSettings.GetUserOofSettings(session);
					this.ParseUserOofSettings(userOofSettings, mailTips, traceId);
				}
				flag = true;
			}
			catch (InvalidParameterException ex)
			{
				MailTipsLocalQuery.GetMailTipsTracer.TraceDebug((long)traceId, "{0} / {1}: GetOofMailTip InvalidParameterException: {2}, {3}", new object[]
				{
					TraceContext.Get(),
					mailTips.EmailAddress,
					ex.GetType().Name,
					ex.Message
				});
				mailTips.OutOfOfficeMessage = null;
				flag = true;
			}
			finally
			{
				stopwatch.Stop();
				MailTipsPerfCounters.OutOfOfficeAnsweredWithinOneSecond_Base.Increment();
				if (stopwatch.ElapsedMilliseconds < 1000L)
				{
					MailTipsPerfCounters.OutOfOfficeAnsweredWithinOneSecond.Increment();
					MailTipsPerfCounters.OutOfOfficeAnsweredWithinThreeSeconds.Increment();
					MailTipsPerfCounters.OutOfOfficeAnsweredWithinTenSeconds.Increment();
				}
				else if (stopwatch.ElapsedMilliseconds < 3000L)
				{
					MailTipsPerfCounters.OutOfOfficeAnsweredWithinThreeSeconds.Increment();
					MailTipsPerfCounters.OutOfOfficeAnsweredWithinTenSeconds.Increment();
				}
				else if (stopwatch.ElapsedMilliseconds < 10000L)
				{
					MailTipsPerfCounters.OutOfOfficeAnsweredWithinTenSeconds.Increment();
				}
				if (flag)
				{
					MailTipsPerfCounters.OutOfOfficePositiveResponses.Increment();
				}
			}
			return flag;
		}

		private bool GetMailboxFullStatus(int traceId, MailboxSession session, MailTips mailTips)
		{
			bool flag = false;
			MailTipsLocalQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)traceId, "{0} / {1}: Getting OutOfOffice", TraceContext.Get(), mailTips.EmailAddress);
			Stopwatch stopwatch = Stopwatch.StartNew();
			bool result;
			try
			{
				Mailbox mailbox = session.Mailbox;
				object obj = mailbox.TryGetProperty(MailboxSchema.QuotaUsedExtended);
				if (obj is PropertyError)
				{
					MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, object>((long)traceId, "{0} / {1}: GetMailboxFullStatus: {2}", TraceContext.Get(), mailTips.EmailAddress, obj);
					flag = false;
					result = flag;
				}
				else
				{
					ulong num = (ulong)((long)obj);
					obj = mailbox.TryGetProperty(MailboxSchema.QuotaProhibitReceive);
					if (obj is PropertyError)
					{
						MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, object>((long)traceId, "{0} / {1}: GetMailboxFullStatus: {2}", TraceContext.Get(), mailTips.EmailAddress, obj);
						flag = false;
						result = flag;
					}
					else
					{
						int num2 = (int)obj;
						if (num2 < 0)
						{
							mailTips.MailboxFull = false;
							flag = true;
							result = flag;
						}
						else
						{
							ulong num3 = (ulong)((long)num2);
							ulong num4 = num3 * 1024UL;
							bool flag2 = num >= num4;
							MailTipsLocalQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, bool>((long)traceId, "{0} / {1}: GetMailboxFullStatus: {2}", TraceContext.Get(), mailTips.EmailAddress, flag2);
							mailTips.MailboxFull = flag2;
							flag = true;
							result = flag;
						}
					}
				}
			}
			finally
			{
				stopwatch.Stop();
				MailTipsPerfCounters.MailboxFullAnsweredWithinOneSecond_Base.Increment();
				if (stopwatch.ElapsedMilliseconds < 1000L)
				{
					MailTipsPerfCounters.MailboxFullAnsweredWithinOneSecond.Increment();
					MailTipsPerfCounters.MailboxFullAnsweredWithinThreeSeconds.Increment();
					MailTipsPerfCounters.MailboxFullAnsweredWithinTenSeconds.Increment();
				}
				else if (stopwatch.ElapsedMilliseconds < 3000L)
				{
					MailTipsPerfCounters.MailboxFullAnsweredWithinThreeSeconds.Increment();
					MailTipsPerfCounters.MailboxFullAnsweredWithinTenSeconds.Increment();
				}
				else if (stopwatch.ElapsedMilliseconds < 10000L)
				{
					MailTipsPerfCounters.MailboxFullAnsweredWithinTenSeconds.Increment();
				}
				if (flag)
				{
					MailTipsPerfCounters.MailboxFullPositiveResponses.Increment();
				}
			}
			return result;
		}

		private static readonly Microsoft.Exchange.Diagnostics.Trace GetMailTipsTracer = ExTraceGlobals.GetMailTipsTracer;

		private static readonly PropertyDefinition[] MailboxPropertyDefinitions = new PropertyDefinition[]
		{
			MailboxSchema.MailboxOofState,
			MailboxSchema.UserOofSettingsItemId,
			MailboxSchema.QuotaProhibitReceive,
			MailboxSchema.QuotaUsedExtended,
			MailboxSchema.LocaleId
		};

		private static readonly DefaultFolderType[] MailboxDefaultFolderTypes = new DefaultFolderType[]
		{
			DefaultFolderType.Configuration,
			DefaultFolderType.System
		};

		private IBudget callerBudget;

		private delegate MailTipsQueryResult MailTipsLocalQueryDelegate();
	}
}
