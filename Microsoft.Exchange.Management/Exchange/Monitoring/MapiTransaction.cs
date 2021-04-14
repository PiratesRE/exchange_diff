using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Monitoring
{
	internal class MapiTransaction : IComparable
	{
		internal Server TargetServer
		{
			get
			{
				return this.targetServer;
			}
		}

		internal Database Database
		{
			get
			{
				return this.database;
			}
		}

		internal ADRecipient ADRecipient
		{
			get
			{
				return this.adRecipient;
			}
		}

		internal string DiagnosticContext
		{
			get
			{
				return this.diagnosticContext;
			}
		}

		internal MapiTransaction(Server targetServer, Database database, ADRecipient adRecipient, bool isArchiveMailbox, bool isDatabaseCopyActive)
		{
			if (targetServer == null)
			{
				throw new ArgumentNullException("targetServer");
			}
			if (database == null)
			{
				throw new ArgumentNullException("database");
			}
			this.targetServer = targetServer;
			this.database = database;
			this.adRecipient = adRecipient;
			this.isArchiveMailbox = isArchiveMailbox;
			this.isDatabaseCopyActive = isDatabaseCopyActive;
		}

		private string ShortErrorMsgFromException(Exception exception)
		{
			string result = string.Empty;
			if (exception.InnerException != null)
			{
				result = Strings.MapiTransactionShortErrorMsgFromExceptionWithInnerException(exception.GetType().ToString(), exception.Message, exception.InnerException.GetType().ToString(), exception.InnerException.Message);
			}
			else
			{
				result = Strings.MapiTransactionShortErrorMsgFromException(exception.GetType().ToString(), exception.Message);
			}
			return result;
		}

		private string GetErrorStringBasedOnDatabaseCopyState()
		{
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=StoreActiveMonitoring", this.targetServer.Name, null, null, null))
			{
				MdbStatus[] array = exRpcAdmin.ListMdbStatus(new Guid[]
				{
					this.database.Guid
				});
				if (array.Length != 0)
				{
					if (this.isDatabaseCopyActive)
					{
						if ((array[0].Status & MdbStatusFlags.Online) != MdbStatusFlags.Online)
						{
							return Strings.MapiTransactionDiagnosticTargetDatabaseDismounted;
						}
					}
					else if ((array[0].Status & MdbStatusFlags.AttachedReadOnly) != MdbStatusFlags.AttachedReadOnly)
					{
						return Strings.MapiTransactionDiagnosticTargetDatabaseNotAttached;
					}
				}
			}
			return null;
		}

		private string GetDiagnosticContext(Exception ex)
		{
			if (ex == null)
			{
				throw new ArgumentNullException("ex");
			}
			string result = string.Empty;
			Exception innerException = ex.InnerException;
			MapiPermanentException ex2 = innerException as MapiPermanentException;
			MapiRetryableException ex3 = innerException as MapiRetryableException;
			if (ex2 != null)
			{
				result = ex2.DiagCtx.ToCompactString();
			}
			else if (ex3 != null)
			{
				result = ex3.DiagCtx.ToCompactString();
			}
			return result;
		}

		private string DiagnoseMapiOperationException(LocalizedException exception, out MapiTransactionResultEnum result)
		{
			result = MapiTransactionResultEnum.Failure;
			bool flag;
			bool flag2;
			string result2 = this.DiagnoseMapiOperationException(exception, out flag, out flag2);
			if (flag)
			{
				result = MapiTransactionResultEnum.MdbMoved;
			}
			else if (flag2)
			{
				result = MapiTransactionResultEnum.StoreNotRunning;
			}
			this.diagnosticContext = this.GetDiagnosticContext(exception);
			return result2;
		}

		internal string DiagnoseMapiOperationException(LocalizedException exception, out bool targetDatabaseMoved, out bool storeNotRunning)
		{
			targetDatabaseMoved = false;
			storeNotRunning = false;
			try
			{
				if (this.isDatabaseCopyActive)
				{
					DatabaseLocationInfo serverForDatabase = RecipientTaskHelper.GetActiveManagerInstance().GetServerForDatabase(this.database.Guid);
					if (string.Compare(serverForDatabase.ServerFqdn, this.targetServer.Fqdn, StringComparison.OrdinalIgnoreCase) != 0)
					{
						targetDatabaseMoved = true;
						return Strings.MapiTransactionDiagnosticTargetDatabaseNotOnTargetServer(serverForDatabase.ServerFqdn, this.targetServer.Fqdn);
					}
				}
			}
			catch (ObjectNotFoundException exception2)
			{
				return Strings.MapiTransactionDiagnosticFailedToGetMdbLocation(this.ShortErrorMsgFromException(exception2));
			}
			using (ServiceController serviceController = new ServiceController("MSExchangeIS", this.targetServer.Fqdn))
			{
				try
				{
					if (serviceController.Status != ServiceControllerStatus.Running)
					{
						storeNotRunning = true;
						return Strings.MapiTransactionDiagnosticStoreServiceIsNotRunning;
					}
				}
				catch (InvalidOperationException exception3)
				{
					return Strings.MapiTransactionDiagnosticStoreServiceCheckFailure(this.ShortErrorMsgFromException(exception3));
				}
			}
			try
			{
				string errorStringBasedOnDatabaseCopyState = this.GetErrorStringBasedOnDatabaseCopyState();
				if (!string.IsNullOrWhiteSpace(errorStringBasedOnDatabaseCopyState))
				{
					return errorStringBasedOnDatabaseCopyState;
				}
			}
			catch (MapiPermanentException exception4)
			{
				return Strings.MapiTransactionDiagnosticStoreStateCheckFailure(this.ShortErrorMsgFromException(exception4));
			}
			catch (MapiRetryableException exception5)
			{
				return Strings.MapiTransactionDiagnosticStoreStateCheckFailure(this.ShortErrorMsgFromException(exception5));
			}
			return this.ShortErrorMsgFromException(exception);
		}

		private void Execute(object transactionOutcome)
		{
			this.Execute((MapiTransactionOutcome)transactionOutcome);
		}

		private void Execute(MapiTransactionOutcome transactionOutcome)
		{
			ExchangePrincipal mailboxOwner = null;
			MapiTransactionResultEnum resultEnum = MapiTransactionResultEnum.Failure;
			string error = string.Empty;
			TimeSpan latency = TimeSpan.Zero;
			Guid? mailboxGuid = null;
			MailboxMiscFlags? mailboxMiscFlags = null;
			try
			{
				if (this.adRecipient == null)
				{
					try
					{
						string errorStringBasedOnDatabaseCopyState = this.GetErrorStringBasedOnDatabaseCopyState();
						if (!string.IsNullOrWhiteSpace(errorStringBasedOnDatabaseCopyState))
						{
							error = errorStringBasedOnDatabaseCopyState;
						}
						else
						{
							error = Strings.MapiTransactionErrorMsgNoMailbox;
						}
					}
					catch (MapiPermanentException ex)
					{
						error = Strings.MapiTransactionDiagnosticStoreStateCheckFailure(this.ShortErrorMsgFromException(ex));
						this.diagnosticContext = ex.DiagCtx.ToCompactString();
					}
					catch (MapiRetryableException ex2)
					{
						error = Strings.MapiTransactionDiagnosticStoreStateCheckFailure(this.ShortErrorMsgFromException(ex2));
						this.diagnosticContext = ex2.DiagCtx.ToCompactString();
					}
					transactionOutcome.Update(MapiTransactionResultEnum.Failure, TimeSpan.Zero, error, mailboxGuid, mailboxMiscFlags, this.isDatabaseCopyActive);
				}
				else
				{
					try
					{
						if (this.adRecipient is ADSystemMailbox)
						{
							mailboxOwner = ExchangePrincipal.FromADSystemMailbox(ADSessionSettings.FromRootOrgScopeSet(), (ADSystemMailbox)this.adRecipient, this.targetServer);
						}
						else
						{
							ADSessionSettings adSessionSettings = this.adRecipient.OrganizationId.ToADSessionSettings();
							mailboxOwner = ExchangePrincipal.FromMailboxData(adSessionSettings, this.adRecipient.DisplayName, this.targetServer.Fqdn, this.targetServer.ExchangeLegacyDN, this.adRecipient.LegacyExchangeDN, this.isArchiveMailbox ? ((ADUser)this.adRecipient).ArchiveGuid : ((ADUser)this.adRecipient).ExchangeGuid, this.database.Guid, this.adRecipient.PrimarySmtpAddress.ToString(), this.adRecipient.Id, new List<CultureInfo>(), Array<Guid>.Empty, RecipientType.Invalid, RemotingOptions.AllowCrossSite);
						}
					}
					catch (ObjectNotFoundException ex3)
					{
						transactionOutcome.Update(MapiTransactionResultEnum.Failure, TimeSpan.Zero, this.ShortErrorMsgFromException(ex3), mailboxGuid, mailboxMiscFlags, this.isDatabaseCopyActive);
						this.diagnosticContext = this.GetDiagnosticContext(ex3);
						return;
					}
					MailboxSession mailboxSession = null;
					Stopwatch stopwatch = Stopwatch.StartNew();
					try
					{
						if (!this.transactionTimeouted)
						{
							try
							{
								mailboxSession = MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, "Client=StoreActiveMonitoring;Action=Test-MapiConnectivity", false, false, !this.isDatabaseCopyActive);
							}
							catch (StorageTransientException exception)
							{
								error = this.DiagnoseMapiOperationException(exception, out resultEnum);
								return;
							}
							catch (StoragePermanentException exception2)
							{
								error = this.DiagnoseMapiOperationException(exception2, out resultEnum);
								return;
							}
							if (!this.transactionTimeouted)
							{
								using (Folder.Bind(mailboxSession, DefaultFolderType.Inbox, new PropertyDefinition[]
								{
									FolderSchema.ItemCount
								}))
								{
									resultEnum = MapiTransactionResultEnum.Success;
									error = string.Empty;
								}
								mailboxSession.Mailbox.Load(new PropertyDefinition[]
								{
									MailboxSchema.MailboxGuid,
									MailboxSchema.MailboxMiscFlags
								});
								byte[] array = mailboxSession.Mailbox.TryGetProperty(MailboxSchema.MailboxGuid) as byte[];
								object obj = mailboxSession.Mailbox.TryGetProperty(MailboxSchema.MailboxMiscFlags);
								if (array != null && array.Length == 16)
								{
									mailboxGuid = new Guid?(new Guid(array));
								}
								if (obj is int)
								{
									mailboxMiscFlags = new MailboxMiscFlags?((MailboxMiscFlags)obj);
								}
								latency = stopwatch.Elapsed;
							}
						}
					}
					finally
					{
						if (mailboxSession != null)
						{
							mailboxSession.Dispose();
						}
					}
				}
			}
			catch (Exception exception3)
			{
				error = this.ShortErrorMsgFromException(exception3);
			}
			finally
			{
				lock (this.timeoutOperationLock)
				{
					if (!this.transactionTimeouted)
					{
						transactionOutcome.Update(resultEnum, latency, error, mailboxGuid, mailboxMiscFlags, this.isDatabaseCopyActive);
					}
				}
			}
		}

		internal MapiTransactionOutcome TimedExecute(int timeOutMilliseconds)
		{
			Thread thread = new Thread(new ParameterizedThreadStart(this.Execute));
			MapiTransactionOutcome mapiTransactionOutcome = new MapiTransactionOutcome(this.targetServer, this.database, this.adRecipient);
			this.transactionTimeouted = false;
			try
			{
				thread.Start(mapiTransactionOutcome);
				if (!thread.Join(timeOutMilliseconds))
				{
					lock (this.timeoutOperationLock)
					{
						this.transactionTimeouted = true;
					}
					if (!thread.Join(250))
					{
						thread.Abort();
					}
				}
				if (mapiTransactionOutcome.Latency.TotalMilliseconds > (double)timeOutMilliseconds || this.transactionTimeouted)
				{
					mapiTransactionOutcome.Update(MapiTransactionResultEnum.Failure, TimeSpan.Zero, Strings.MapiTransactionErrorMsgTimeout((double)timeOutMilliseconds / 1000.0), null, null, this.isDatabaseCopyActive);
				}
			}
			catch (ThreadAbortException)
			{
				mapiTransactionOutcome.Update(MapiTransactionResultEnum.Failure, TimeSpan.Zero, Strings.MapiTransactionAbortedMsg, null, null, this.isDatabaseCopyActive);
			}
			return mapiTransactionOutcome;
		}

		public int CompareTo(object obj)
		{
			MapiTransaction mapiTransaction = obj as MapiTransaction;
			if (mapiTransaction == null)
			{
				throw new ArgumentException();
			}
			int num = string.Compare(this.database.ServerName, mapiTransaction.database.ServerName, true, CultureInfo.CurrentCulture);
			if (num != 0)
			{
				return num;
			}
			return string.Compare(this.database.Name, mapiTransaction.database.Name, true, CultureInfo.CurrentCulture);
		}

		private const string MapiClientIdAndAction = "Client=StoreActiveMonitoring;Action=Test-MapiConnectivity";

		private Server targetServer;

		private Database database;

		private ADRecipient adRecipient;

		private string diagnosticContext;

		private readonly bool isArchiveMailbox;

		private readonly bool isDatabaseCopyActive;

		private volatile bool transactionTimeouted;

		private object timeoutOperationLock = new object();
	}
}
