using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Reflection;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MrsPSHandler : DisposeTrackableBase
	{
		public MrsPSHandler(string prefix)
		{
			MrsPSHandler.CheckPSRunspaceInitialized();
			this.mrsCommandInteractionHandler = new MrsPSHandler.MrsCommandInteractionHandler(prefix);
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				this.MonadConnection = new MonadConnection("timeout=30", this.mrsCommandInteractionHandler);
				disposeGuard.Add<MonadConnection>(this.MonadConnection);
				this.MonadConnection.Open();
				disposeGuard.Success();
			}
		}

		public MonadConnection MonadConnection { get; private set; }

		public List<ReportEntry> ReportEntries
		{
			get
			{
				return this.mrsCommandInteractionHandler.ReportEntries;
			}
		}

		public List<Exception> ExceptionsReported
		{
			get
			{
				return this.mrsCommandInteractionHandler.ExceptionsReported;
			}
		}

		public MonadCommand GetCommand(MrsCmdlet mrsCmdlet)
		{
			return new MonadCommand(MrsPSHandler.cmdletToString[mrsCmdlet], this.MonadConnection);
		}

		private static void CheckPSRunspaceInitialized()
		{
			if (!MrsPSHandler.psRunspaceInitialized)
			{
				lock (MrsPSHandler.psLocker)
				{
					if (!MrsPSHandler.psRunspaceInitialized)
					{
						List<CmdletConfigurationEntry> list = new List<CmdletConfigurationEntry>();
						foreach (KeyValuePair<string, MrsPSHandler.MrsCmdletInfo[]> keyValuePair in MrsPSHandler.cmdlets)
						{
							string text = Path.Combine(ConfigurationContext.Setup.BinPath, keyValuePair.Key);
							if (File.Exists(text))
							{
								Assembly assembly = Assembly.LoadFrom(text);
								foreach (MrsPSHandler.MrsCmdletInfo mrsCmdletInfo in keyValuePair.Value)
								{
									list.Add(new CmdletConfigurationEntry(mrsCmdletInfo.Cmdlet, assembly.GetType(mrsCmdletInfo.ClassName, true, true), "Microsoft.Exchange.ServerStatus-Help.xml"));
									MrsPSHandler.cmdletToString.Add(mrsCmdletInfo.MrsCmdlet, mrsCmdletInfo.Cmdlet);
								}
							}
						}
						MonadRunspaceConfiguration.AddArray(list.ToArray());
						MrsPSHandler.psRunspaceInitialized = true;
					}
				}
			}
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.MonadConnection != null)
			{
				this.MonadConnection.Dispose();
				this.MonadConnection = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MrsPSHandler>(this);
		}

		private static readonly Dictionary<string, MrsPSHandler.MrsCmdletInfo[]> cmdlets = new Dictionary<string, MrsPSHandler.MrsCmdletInfo[]>
		{
			{
				"Microsoft.Exchange.Management.dll",
				new MrsPSHandler.MrsCmdletInfo[]
				{
					new MrsPSHandler.MrsCmdletInfo(MrsCmdlet.UpdateMovedMailbox, "Update-MovedMailbox", "Microsoft.Exchange.Management.RecipientTasks.UpdateMovedMailbox"),
					new MrsPSHandler.MrsCmdletInfo(MrsCmdlet.SetOrganization, "Set-Organization", "Microsoft.Exchange.Management.Deployment.SetOrganization"),
					new MrsPSHandler.MrsCmdletInfo(MrsCmdlet.GetPublicFolderMoveRequest, "Get-PublicFolderMoveRequest", "Microsoft.Exchange.Management.RecipientTasks.GetPublicFolderMoveRequest"),
					new MrsPSHandler.MrsCmdletInfo(MrsCmdlet.GetMoveRequest, "Get-MoveRequest", "Microsoft.Exchange.Management.RecipientTasks.GetMoveRequest")
				}
			},
			{
				"Microsoft.Exchange.Management.Recipient.dll",
				new MrsPSHandler.MrsCmdletInfo[]
				{
					new MrsPSHandler.MrsCmdletInfo(MrsCmdlet.SetConsumerMailbox, "Set-ConsumerMailbox", "Microsoft.Exchange.Management.RecipientTasks.SetConsumerMailbox"),
					new MrsPSHandler.MrsCmdletInfo(MrsCmdlet.GetMailbox, "Get-Mailbox", "Microsoft.Exchange.Management.RecipientTasks.GetMailbox")
				}
			}
		};

		private static readonly object psLocker = new object();

		private static Dictionary<MrsCmdlet, string> cmdletToString = new Dictionary<MrsCmdlet, string>();

		private static bool psRunspaceInitialized = false;

		private readonly MrsPSHandler.MrsCommandInteractionHandler mrsCommandInteractionHandler;

		private class MrsCmdletInfo
		{
			public MrsCmdletInfo(MrsCmdlet mrsCmdlet, string cmdlet, string className)
			{
				this.MrsCmdlet = mrsCmdlet;
				this.Cmdlet = cmdlet;
				this.ClassName = className;
			}

			public string Cmdlet { get; private set; }

			public string ClassName { get; private set; }

			public MrsCmdlet MrsCmdlet { get; private set; }
		}

		private class MrsCommandInteractionHandler : CommandInteractionHandler
		{
			public MrsCommandInteractionHandler(string prefix)
			{
				this.prefix = prefix;
				this.ReportEntries = new List<ReportEntry>();
				this.ExceptionsReported = new List<Exception>();
			}

			public List<ReportEntry> ReportEntries { get; private set; }

			public List<Exception> ExceptionsReported { get; private set; }

			public override void ReportVerboseOutput(string message)
			{
				base.ReportVerboseOutput(message);
				MrsTracer.Common.Debug("{0}", new object[]
				{
					string.Format("{0} - {1}", this.prefix, message)
				});
				ReportEntry reportEntry = new ReportEntry(LocalizedString.Empty, ReportEntryType.Debug);
				reportEntry.DebugData = message;
				this.ReportEntries.Add(reportEntry);
			}

			public override void ReportWarning(WarningReportEventArgs e)
			{
				base.ReportWarning(e);
				MrsTracer.Common.Warning("{0}", new object[]
				{
					string.Format("{0} - {1}", this.prefix, e.WarningMessage)
				});
				this.ReportEntries.Add(new ReportEntry(new LocalizedString(e.WarningMessage)));
			}

			public override void ReportException(Exception ex)
			{
				base.ReportException(ex);
				this.ExceptionsReported.Add(ex);
				LocalizedString localizedString = CommonUtils.FullExceptionMessage(ex);
				MrsTracer.Common.Error("{0}", new object[]
				{
					string.Format("{0} - {1}", this.prefix, localizedString)
				});
				this.ReportEntries.Add(new ReportEntry(localizedString, ReportEntryType.Error, ex, ReportEntryFlags.None));
			}

			public override void ReportErrors(ErrorReportEventArgs e)
			{
				base.ReportErrors(e);
				this.ExceptionsReported.Add(e.ErrorRecord.Exception);
				LocalizedString localizedString = CommonUtils.FullExceptionMessage(e.ErrorRecord.Exception);
				MrsTracer.Common.Error("{0}", new object[]
				{
					string.Format("{0} - {1}", this.prefix, localizedString)
				});
				this.ReportEntries.Add(new ReportEntry(localizedString, ReportEntryType.Error, e.ErrorRecord.Exception, ReportEntryFlags.None));
			}

			public override ConfirmationChoice ShowConfirmationDialog(string message, ConfirmationChoice defaultChoice)
			{
				MrsTracer.Common.Warning("{0}", new object[]
				{
					string.Format("{0} - Automatically Confirmed - {1}", this.prefix, message)
				});
				this.ReportEntries.Add(new ReportEntry(new LocalizedString(message)));
				return ConfirmationChoice.Yes;
			}

			private readonly string prefix;
		}
	}
}
