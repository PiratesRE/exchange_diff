using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.CalendarDiagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Get", "CalendarDiagnosticLog", DefaultParameterSetName = "DoNotExportParameterSet")]
	public sealed class GetCalendarDiagnosticLog : GetRecipientObjectTask<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = true, ParameterSetName = "ExportToMsgParameterSet", ValueFromPipelineByPropertyName = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "DoNotExportParameterSet", ValueFromPipelineByPropertyName = true, Position = 0)]
		public new MailboxIdParameter Identity
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Subject
		{
			get
			{
				return (string)(base.Fields["Subject"] ?? string.Empty);
			}
			set
			{
				base.Fields["Subject"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string MeetingID
		{
			get
			{
				return (string)(base.Fields["MeetingID"] ?? string.Empty);
			}
			set
			{
				base.Fields["MeetingID"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime? StartDate
		{
			get
			{
				ExDateTime? exDateTime = (ExDateTime?)base.Fields["StartDate"];
				if (exDateTime == null)
				{
					exDateTime = new ExDateTime?(new ExDateTime(ExTimeZone.UtcTimeZone, DateTime.MinValue.ToUniversalTime()));
				}
				else
				{
					exDateTime = new ExDateTime?(new ExDateTime(ExTimeZone.CurrentTimeZone, exDateTime.Value.UtcTicks));
				}
				return new ExDateTime?(exDateTime.Value);
			}
			set
			{
				base.Fields["StartDate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime? EndDate
		{
			get
			{
				ExDateTime? exDateTime = (ExDateTime?)base.Fields["EndDate"];
				if (exDateTime == null)
				{
					exDateTime = new ExDateTime?(new ExDateTime(ExTimeZone.UtcTimeZone, DateTime.MaxValue.ToUniversalTime()));
				}
				else
				{
					exDateTime = new ExDateTime?(new ExDateTime(ExTimeZone.CurrentTimeZone, exDateTime.Value.UtcTicks));
				}
				return new ExDateTime?(exDateTime.Value);
			}
			set
			{
				base.Fields["EndDate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Latest
		{
			get
			{
				return (SwitchParameter)(base.Fields["GetLatest"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["GetLatest"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ExportToMsgParameterSet")]
		public string LogLocation
		{
			get
			{
				string fileName = string.Empty;
				if (!this.IsLogLocationInitialized)
				{
					if (!base.Fields.Contains("LogLocation"))
					{
						using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\CalendarLogs"))
						{
							if (registryKey != null)
							{
								string text = registryKey.GetValue("LogLocation", string.Empty) as string;
								if (!string.IsNullOrEmpty(text))
								{
									fileName = text;
									base.Fields["LogLocation"] = LocalLongFullPath.ConvertInvalidCharactersInPathName(fileName);
								}
							}
							goto IL_A4;
						}
					}
					fileName = (string)base.Fields["LogLocation"];
					base.Fields["LogLocation"] = LocalLongFullPath.ConvertInvalidCharactersInPathName(fileName);
					IL_A4:
					this.IsLogLocationInitialized = true;
				}
				return (string)base.Fields["LogLocation"];
			}
			set
			{
				base.Fields["LogLocation"] = value;
			}
		}

		private MailboxIdParameter LogMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["LogMailbox"];
			}
			set
			{
				base.Fields["LogMailbox"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			TaskLogger.LogEnter();
			try
			{
				if (this.LogLocation != null)
				{
					this.LogLocation = CalendarDiagnosticLogFileWriter.CheckAndCreateLogLocation(this.LogLocation);
				}
			}
			catch (SecurityException exception)
			{
				throw new ThrowTerminatingErrorException(new ErrorRecord(exception, string.Empty, ErrorCategory.PermissionDenied, this));
			}
			catch (ArgumentException exception2)
			{
				throw new ThrowTerminatingErrorException(new ErrorRecord(exception2, string.Empty, ErrorCategory.InvalidData, this));
			}
			catch (NotSupportedException exception3)
			{
				throw new ThrowTerminatingErrorException(new ErrorRecord(exception3, string.Empty, ErrorCategory.InvalidData, this));
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.OutputLogs();
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			this.itemId = null;
			this.objectId = null;
			LocalizedString? localizedString = null;
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!string.IsNullOrEmpty(this.MeetingID))
			{
				try
				{
					this.itemId = StoreObjectId.Deserialize(this.MeetingID);
				}
				catch (Exception)
				{
					if (!GlobalObjectId.TryParse(this.MeetingID, out this.objectId))
					{
						base.WriteError(new ManagementObjectNotFoundException(localizedString ?? base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(MailboxIdParameter).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ErrorCategory.InvalidData, null);
					}
				}
			}
			this.logSourceUser = base.GetDataObjects(this.Identity, base.OptionalIdentityData, out localizedString).FirstOrDefault<ADUser>();
			if (this.logSourceUser == null || base.HasErrors)
			{
				base.WriteError(new ManagementObjectNotFoundException(localizedString ?? base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(MailboxIdParameter).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ErrorCategory.InvalidData, null);
			}
			if (this.LogMailbox != null)
			{
				this.outputMailboxUser = base.GetDataObjects(this.LogMailbox, base.OptionalIdentityData, out localizedString).FirstOrDefault<ADUser>();
			}
			TaskLogger.LogExit();
		}

		private void OutputLogs()
		{
			TaskLogger.LogEnter();
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(base.SessionSettings, this.logSourceUser, RemotingOptions.AllowCrossSite);
			using (MailboxSession mailboxSession = StoreTasksHelper.OpenMailboxSession(exchangePrincipal, "Get-CalendarDiagnosticLogs"))
			{
				Dictionary<string, List<VersionedId>> allCalendarLogItems = this.GetAllCalendarLogItems(mailboxSession);
				if (allCalendarLogItems.Keys.Count == 0)
				{
					this.WriteWarning(Strings.CalendarDiagnosticLogsNotFound(this.Subject, mailboxSession.MailboxOwner.MailboxInfo.DisplayName));
					return;
				}
				MailboxSession mailboxSession2 = null;
				try
				{
					if (this.outputMailboxUser != null)
					{
						ExchangePrincipal principal = exchangePrincipal;
						mailboxSession2 = StoreTasksHelper.OpenMailboxSession(principal, "Get-CalendarDiagnosticLogs");
					}
					else
					{
						mailboxSession2 = mailboxSession;
					}
					SmtpAddress address = new SmtpAddress(exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
					foreach (KeyValuePair<string, List<VersionedId>> keyValuePair in allCalendarLogItems)
					{
						if (!string.IsNullOrEmpty(this.LogLocation))
						{
							this.diagnosticLogWriter = new CalendarDiagnosticLogFileWriter(this.LogLocation, mailboxSession.MailboxOwner.MailboxInfo.DisplayName, address.Domain);
						}
						base.WriteProgress(Strings.GetCalendarDiagnosticLog(this.Identity.ToString()), Strings.SavingCalendarLogs, 0);
						List<VersionedId> value = keyValuePair.Value;
						int count = value.Count;
						foreach (VersionedId storeId in value)
						{
							using (Item item = Item.Bind(mailboxSession, storeId))
							{
								if (!(item.LastModifiedTime > this.EndDate) && !(item.LastModifiedTime < this.StartDate))
								{
									if (!string.IsNullOrEmpty(this.LogLocation))
									{
										string text = null;
										if (Directory.Exists(this.LogLocation))
										{
											FileInfo fileInfo = this.diagnosticLogWriter.LogItem(item, out text);
											if (fileInfo == null && !string.IsNullOrEmpty(text))
											{
												base.WriteWarning(text);
											}
											else
											{
												base.WriteResult(new CalendarLog(item, fileInfo, (string)address));
											}
										}
									}
									else
									{
										base.WriteResult(new CalendarLog(item, (string)address));
									}
								}
							}
						}
					}
				}
				finally
				{
					if (mailboxSession2 != null)
					{
						mailboxSession2.Dispose();
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is StorageTransientException || exception is StoragePermanentException || exception is ObjectNotFoundException || exception is IOException || exception is UnauthorizedAccessException;
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			TaskLogger.LogExit();
		}

		private Dictionary<string, List<VersionedId>> GetAllCalendarLogItems(MailboxSession mailboxSession)
		{
			CalendarVersionStoreGateway calendarVersionStoreGateway = new CalendarVersionStoreGateway(default(CalendarVersionStoreQueryPolicy), true);
			Dictionary<string, List<VersionedId>> resultTree = new Dictionary<string, List<VersionedId>>();
			try
			{
				if (this.itemId != null)
				{
					using (Item item = Item.Bind(mailboxSession, this.itemId, ItemBindOption.LoadRequiredPropertiesOnly, GetCalendarDiagnosticLog.requiredIdProperties))
					{
						this.Subject = item.GetValueOrDefault<string>(ItemSchema.Subject);
						byte[] valueOrDefault = item.GetValueOrDefault<byte[]>(CalendarItemBaseSchema.CleanGlobalObjectId);
						if (valueOrDefault != null)
						{
							string cleanGoidKey = this.GetCleanGoidKey(valueOrDefault);
							calendarVersionStoreGateway.QueryByCleanGlobalObjectId(mailboxSession, new GlobalObjectId(valueOrDefault), "{735AA13D-42D0-4610-BDCB-3DF048D33957}", GetCalendarDiagnosticLog.requiredIdProperties, (PropertyBag propertyBag) => this.AddItemToResultTree(cleanGoidKey, resultTree, propertyBag), false, null, this.StartDate, this.EndDate);
						}
						else
						{
							base.WriteError(new InvalidOperationException(Strings.GetCalendarDiagnosticLogNoCleanGoidErrorMessage.ToString()), ErrorCategory.InvalidData, this);
						}
						goto IL_170;
					}
				}
				if (this.objectId != null)
				{
					calendarVersionStoreGateway.QueryByCleanGlobalObjectId(mailboxSession, this.objectId, "{735AA13D-42D0-4610-BDCB-3DF048D33957}", GetCalendarDiagnosticLog.requiredIdProperties, (PropertyBag propertyBag) => this.AddItemToResultTree(this.MeetingID, resultTree, propertyBag), false, null, this.StartDate, this.EndDate);
				}
				else
				{
					new HashSet<string>();
					calendarVersionStoreGateway.QueryBySubjectContains(mailboxSession, this.Subject, "{735AA13D-42D0-4610-BDCB-3DF048D33957}", GetCalendarDiagnosticLog.requiredIdProperties, delegate(PropertyBag propertyBag)
					{
						byte[] array;
						string cleanGoidKey = this.GetCleanGoidKey(propertyBag, out array);
						this.AddItemToResultTree(cleanGoidKey, resultTree, propertyBag);
					}, this.StartDate, this.EndDate);
				}
				IL_170:;
			}
			catch (CalendarVersionStoreNotPopulatedException exception)
			{
				base.WriteError(exception, ErrorCategory.OperationTimeout, this);
			}
			return resultTree;
		}

		private bool AddItemToResultTree(string cleanGoidKey, Dictionary<string, List<VersionedId>> resultTree, PropertyBag propertyBag)
		{
			List<VersionedId> list;
			if (!resultTree.TryGetValue(cleanGoidKey, out list))
			{
				list = new List<VersionedId>(1);
				resultTree.Add(cleanGoidKey, list);
				list.Add(propertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id));
			}
			else if (!this.Latest.IsPresent)
			{
				list.Add(propertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id));
			}
			return true;
		}

		private string GetCleanGoidKey(PropertyBag propertyBag, out byte[] cleanGoid)
		{
			cleanGoid = propertyBag.GetValueOrDefault<byte[]>(CalendarItemBaseSchema.CleanGlobalObjectId);
			return this.GetCleanGoidKey(cleanGoid);
		}

		private string GetCleanGoidKey(byte[] cleanGoid)
		{
			if (cleanGoid != null)
			{
				return GlobalObjectId.ByteArrayToHexString(cleanGoid);
			}
			return string.Empty;
		}

		private const string TaskName = "Get-CalendarDiagnosticLogs";

		private const string IdentityKey = "Identity";

		private const string SubjectKey = "Subject";

		private const string MeetingIDKey = "MeetingID";

		private const string LogLocationKey = "LogLocation";

		private const string LogMailboxKey = "LogMailbox";

		private const string StartDateKey = "StartDate";

		private const string EndDateKey = "EndDate";

		private const string GetLatestKey = "GetLatest";

		private const string DoNotExportKey = "DoNotExport";

		private const string DoNotExportParameterSet = "DoNotExportParameterSet";

		private const string ExportToMsgParameterSet = "ExportToMsgParameterSet";

		private const string LogRegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\CalendarLogs";

		private const string LogRegistryValue = "LogLocation";

		private const string RequiredPropertySetKey = "{735AA13D-42D0-4610-BDCB-3DF048D33957}";

		private ADUser logSourceUser;

		private ADUser outputMailboxUser;

		private bool IsLogLocationInitialized;

		private static StorePropertyDefinition[] requiredIdProperties = new StorePropertyDefinition[]
		{
			ItemSchema.Id,
			CalendarItemBaseSchema.CleanGlobalObjectId,
			ItemSchema.NormalizedSubject,
			CalendarItemBaseSchema.OriginalLastModifiedTime
		};

		private CalendarDiagnosticLogFileWriter diagnosticLogWriter;

		private StoreObjectId itemId;

		private GlobalObjectId objectId;
	}
}
