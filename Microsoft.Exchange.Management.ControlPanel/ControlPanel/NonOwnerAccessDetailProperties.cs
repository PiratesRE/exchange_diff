using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NonOwnerAccessDetailProperties : AuditLogChangeDetailProperties
	{
		protected override string DetailsPaneId
		{
			get
			{
				return "NonOwnerAccessDetailsPane";
			}
		}

		protected override string HeaderLabelId
		{
			get
			{
				return "lblMailboxChanged";
			}
		}

		protected override string GetDetailsPaneHeader()
		{
			PowerShellResults<NonOwnerAccessDetailRow> powerShellResults = base.Results as PowerShellResults<NonOwnerAccessDetailRow>;
			if (powerShellResults != null && powerShellResults.Output.Length > 0 && powerShellResults.Succeeded)
			{
				return string.Format("{0}{1}", Strings.MailboxAccessed, powerShellResults.Output[0].MailboxAuditLogEvent.MailboxResolvedOwnerName);
			}
			return string.Empty;
		}

		protected override void RenderChanges()
		{
			PowerShellResults<NonOwnerAccessDetailRow> powerShellResults = base.Results as PowerShellResults<NonOwnerAccessDetailRow>;
			if (powerShellResults != null && powerShellResults.Output.Length > 0 && powerShellResults.Succeeded)
			{
				Table table = new Table();
				table.CssClass = "PropertyForm";
				int num = 0;
				while (num < powerShellResults.Output.Length && num < 500)
				{
					NonOwnerAccessDetailRow nonOwnerAccessDetailRow = powerShellResults.Output[num];
					MailboxAuditLogEvent mailboxAuditLogEvent = nonOwnerAccessDetailRow.MailboxAuditLogEvent;
					table.Rows.Add(base.GetDetailRowForTable(Strings.TimeOfOperation, mailboxAuditLogEvent.LastAccessed.Value.ToUniversalTime().UtcToUserDateTimeString()));
					table.Rows.Add(base.GetDetailRowForTable(Strings.OperationPerformedBy, mailboxAuditLogEvent.LogonUserDisplayName));
					table.Rows.Add(base.GetDetailRowForTable(Strings.LogonType, this.GetLocalizedLogonType(mailboxAuditLogEvent.LogonType)));
					table.Rows.Add(base.GetDetailRowForTable(Strings.OperationType, this.GetLocalizedAction(mailboxAuditLogEvent.Operation)));
					if (mailboxAuditLogEvent.Operation.Equals(Enum.GetName(typeof(MailboxAuditOperations), MailboxAuditOperations.Move), StringComparison.InvariantCultureIgnoreCase) || mailboxAuditLogEvent.Operation.Equals(Enum.GetName(typeof(MailboxAuditOperations), MailboxAuditOperations.MoveToDeletedItems), StringComparison.InvariantCultureIgnoreCase) || mailboxAuditLogEvent.Operation.Equals(Enum.GetName(typeof(MailboxAuditOperations), MailboxAuditOperations.SoftDelete), StringComparison.InvariantCultureIgnoreCase) || mailboxAuditLogEvent.Operation.Equals(Enum.GetName(typeof(MailboxAuditOperations), MailboxAuditOperations.HardDelete), StringComparison.InvariantCultureIgnoreCase))
					{
						List<string> list = new List<string>();
						List<string> list2 = new List<string>();
						if (mailboxAuditLogEvent.SourceFolders.Added.Length > 0)
						{
							foreach (MailboxAuditLogSourceFolder mailboxAuditLogSourceFolder in mailboxAuditLogEvent.SourceFolders.Added)
							{
								if (!list2.Contains(this.FixFolderPathName(mailboxAuditLogSourceFolder.SourceFolderPathName)))
								{
									list.Add(this.FixFolderPathName(mailboxAuditLogSourceFolder.SourceFolderPathName));
								}
							}
							list2.Add(this.FixFolderPathName(mailboxAuditLogEvent.FolderPathName));
						}
						else
						{
							foreach (MailboxAuditLogSourceItem mailboxAuditLogSourceItem in mailboxAuditLogEvent.SourceItems.Added)
							{
								list.Add(mailboxAuditLogSourceItem.SourceItemSubject);
								list2.Add(this.FixFolderPathName(mailboxAuditLogSourceItem.SourceItemFolderPathName));
							}
						}
						string information = string.Join(Strings.Separator, list.ToArray());
						string information2 = string.Join(Strings.Separator, list2.ToArray());
						table.Rows.Add(base.GetDetailRowForTable(Strings.ItemSubject, information));
						table.Rows.Add(base.GetDetailRowForTable(Strings.SourceFolder, information2));
						if (mailboxAuditLogEvent.Operation.Equals(Enum.GetName(typeof(MailboxAuditOperations), MailboxAuditOperations.Move), StringComparison.InvariantCultureIgnoreCase))
						{
							string information3 = this.FixFolderPathName(mailboxAuditLogEvent.DestFolderPathName);
							table.Rows.Add(base.GetDetailRowForTable(Strings.DestinationFolder, information3));
						}
					}
					else if (mailboxAuditLogEvent.Operation.Equals(Enum.GetName(typeof(MailboxAuditOperations), MailboxAuditOperations.FolderBind), StringComparison.InvariantCultureIgnoreCase))
					{
						table.Rows.Add(base.GetDetailRowForTable(Strings.FolderName, this.FixFolderPathName(mailboxAuditLogEvent.FolderPathName)));
					}
					else
					{
						table.Rows.Add(base.GetDetailRowForTable(Strings.ItemSubject, mailboxAuditLogEvent.ItemSubject));
						if (mailboxAuditLogEvent.Operation.Equals(Enum.GetName(typeof(MailboxAuditOperations), MailboxAuditOperations.Create), StringComparison.InvariantCultureIgnoreCase))
						{
							table.Rows.Add(base.GetDetailRowForTable(Strings.FolderName, this.FixFolderPathName(mailboxAuditLogEvent.FolderPathName)));
						}
					}
					table.Rows.Add(base.GetDetailRowForTable(Strings.OperationStatus, this.GetLocalizedStatus(mailboxAuditLogEvent.OperationResult)));
					table.Rows.Add(base.GetEmptyRowForTable());
					num++;
				}
				this.detailsPane.Controls.Add(table);
			}
		}

		private string GetLocalizedAction(string action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			MailboxAuditOperations mailboxAuditOperations = (MailboxAuditOperations)Enum.Parse(typeof(MailboxAuditOperations), action, true);
			MailboxAuditOperations mailboxAuditOperations2 = mailboxAuditOperations;
			if (mailboxAuditOperations2 <= MailboxAuditOperations.HardDelete)
			{
				if (mailboxAuditOperations2 <= MailboxAuditOperations.MoveToDeletedItems)
				{
					switch (mailboxAuditOperations2)
					{
					case MailboxAuditOperations.Update:
						return Strings.MailboxOperationUpdate;
					case MailboxAuditOperations.Copy:
						return Strings.MailboxOperationCopy;
					case MailboxAuditOperations.Update | MailboxAuditOperations.Copy:
						break;
					case MailboxAuditOperations.Move:
						return Strings.MailboxOperationMove;
					default:
						if (mailboxAuditOperations2 == MailboxAuditOperations.MoveToDeletedItems)
						{
							return Strings.MailboxOperationMoveToDeletedItems;
						}
						break;
					}
				}
				else
				{
					if (mailboxAuditOperations2 == MailboxAuditOperations.SoftDelete)
					{
						return Strings.MailboxOperationSoftDelete;
					}
					if (mailboxAuditOperations2 == MailboxAuditOperations.HardDelete)
					{
						return Strings.MailboxOperationHardDelete;
					}
				}
			}
			else if (mailboxAuditOperations2 <= MailboxAuditOperations.SendAs)
			{
				if (mailboxAuditOperations2 == MailboxAuditOperations.FolderBind)
				{
					return Strings.MailboxOperationFolderBind;
				}
				if (mailboxAuditOperations2 == MailboxAuditOperations.SendAs)
				{
					return Strings.MailboxOperationSendAs;
				}
			}
			else
			{
				if (mailboxAuditOperations2 == MailboxAuditOperations.SendOnBehalf)
				{
					return Strings.MailboxOperationSendOnBehalf;
				}
				if (mailboxAuditOperations2 == MailboxAuditOperations.MessageBind)
				{
					return Strings.MailboxOperationMessageBind;
				}
				if (mailboxAuditOperations2 == MailboxAuditOperations.Create)
				{
					return Strings.MailboxOperationCreate;
				}
			}
			throw new FaultException(new ArgumentException("NonOwnerAccessValue").Message);
		}

		private string GetLocalizedLogonType(string logonType)
		{
			if (logonType == null)
			{
				throw new ArgumentNullException(logonType);
			}
			string a;
			if ((a = logonType.ToLower()) != null)
			{
				if (a == "admin")
				{
					return Strings.LogonTypeAdmin;
				}
				if (a == "delegate")
				{
					return Strings.LogonTypeDelegate;
				}
				if (a == "external")
				{
					return Strings.LogonTypeExternal;
				}
			}
			throw new FaultException(new ArgumentException("LogonTypeValue").Message);
		}

		private string GetLocalizedStatus(string status)
		{
			if (status.Equals(OperationResult.Succeeded.ToString(), StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.Succeeded;
			}
			if (status.Equals(OperationResult.Failed.ToString(), StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.Failed;
			}
			if (status.Equals(OperationResult.PartiallySucceeded.ToString(), StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.PartiallySucceeded;
			}
			return Strings.Unknown;
		}

		private string FixFolderPathName(string folderName)
		{
			folderName = folderName.Trim();
			if (folderName.StartsWith("/") || folderName.StartsWith("\\"))
			{
				folderName = folderName.TrimStart(new char[]
				{
					'/',
					'\\'
				});
			}
			return folderName;
		}

		private const string Delegate = "delegate";

		private const string Admin = "admin";

		private const string External = "external";
	}
}
