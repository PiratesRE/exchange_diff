using System;
using System.Data;
using System.Net;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	public class QueueViewerMessagesResultPane : QueueViewerResultPaneBase
	{
		static QueueViewerMessagesResultPane()
		{
			QueueViewerMessagesResultPane.iconLibrary = new IconLibrary();
			QueueViewerMessagesResultPane.iconLibrary.Icons.Add(MessageStatus.Active, Icons.MessagesActive);
			QueueViewerMessagesResultPane.iconLibrary.Icons.Add(MessageStatus.Suspended, Icons.MessagesFrozen);
			QueueViewerMessagesResultPane.iconLibrary.Icons.Add(MessageStatus.Ready, Icons.MessagesQueued);
			QueueViewerMessagesResultPane.iconLibrary.Icons.Add(MessageStatus.Retry, Icons.MessagesRetry);
			QueueViewerMessagesResultPane.iconLibrary.Icons.Add(MessageStatus.PendingSuspend, Icons.MessagesFrozen);
			QueueViewerMessagesResultPane.iconLibrary.Icons.Add(MessageStatus.PendingRemove, Icons.MessagesPendingDelete);
		}

		public QueueViewerMessagesResultPane()
		{
			base.Name = "QueueViewerMessagesResultPane";
			base.ObjectList.Name = "messageList";
			base.ObjectList.ListView.AutoGenerateColumns = false;
			ExchangeColumnHeader exchangeColumnHeader = new ExchangeColumnHeader();
			exchangeColumnHeader.Name = "FromAddress";
			exchangeColumnHeader.Text = Strings.FromAddressColumn;
			exchangeColumnHeader.Default = true;
			ExchangeColumnHeader exchangeColumnHeader2 = new ExchangeColumnHeader();
			exchangeColumnHeader2.Name = "Status";
			exchangeColumnHeader2.Text = Strings.StatusColumn;
			exchangeColumnHeader2.Default = true;
			ExchangeColumnHeader exchangeColumnHeader3 = new ExchangeColumnHeader();
			exchangeColumnHeader3.Name = "Size";
			exchangeColumnHeader3.Text = Strings.SizeColumn;
			exchangeColumnHeader3.Default = true;
			exchangeColumnHeader3.CustomFormatter = DisplayFormats.ByteQuantifiedSizeAsKb;
			ExchangeColumnHeader exchangeColumnHeader4 = new ExchangeColumnHeader();
			exchangeColumnHeader4.Name = "SCL";
			exchangeColumnHeader4.Text = Strings.SCLColumn;
			exchangeColumnHeader4.Default = true;
			ExchangeColumnHeader exchangeColumnHeader5 = new ExchangeColumnHeader();
			exchangeColumnHeader5.Name = "Queue";
			exchangeColumnHeader5.Text = Strings.QueueIdColumn;
			exchangeColumnHeader5.Default = true;
			ExchangeColumnHeader exchangeColumnHeader6 = new ExchangeColumnHeader();
			exchangeColumnHeader6.Name = "MessageSourceName";
			exchangeColumnHeader6.Text = Strings.MessageSourceNameColumn;
			exchangeColumnHeader6.Default = true;
			ExchangeColumnHeader exchangeColumnHeader7 = new ExchangeColumnHeader();
			exchangeColumnHeader7.Name = "LastError";
			exchangeColumnHeader7.Text = Strings.LastErrorColumn;
			exchangeColumnHeader7.Default = true;
			ExchangeColumnHeader exchangeColumnHeader8 = new ExchangeColumnHeader();
			exchangeColumnHeader8.Name = "Subject";
			exchangeColumnHeader8.Text = Strings.SubjectColumn;
			exchangeColumnHeader8.Default = true;
			ExchangeColumnHeader exchangeColumnHeader9 = new ExchangeColumnHeader();
			exchangeColumnHeader9.Name = "SourceIP";
			exchangeColumnHeader9.Text = Strings.SourceIPColumn;
			ExchangeColumnHeader exchangeColumnHeader10 = new ExchangeColumnHeader();
			exchangeColumnHeader10.Name = "InternetMessageId";
			exchangeColumnHeader10.Text = Strings.InternetMessageIdColumn;
			ExchangeColumnHeader exchangeColumnHeader11 = new ExchangeColumnHeader();
			exchangeColumnHeader11.Name = "ExpirationTime";
			exchangeColumnHeader11.Text = Strings.ExpirationTimeColumn;
			ExchangeColumnHeader exchangeColumnHeader12 = new ExchangeColumnHeader();
			exchangeColumnHeader12.Name = "DateReceived";
			exchangeColumnHeader12.Text = Strings.DateReceivedColumn;
			base.ObjectList.ListView.AvailableColumns.AddRange(new ExchangeColumnHeader[]
			{
				exchangeColumnHeader,
				exchangeColumnHeader2,
				exchangeColumnHeader3,
				exchangeColumnHeader4,
				exchangeColumnHeader5,
				exchangeColumnHeader6,
				exchangeColumnHeader8,
				exchangeColumnHeader7,
				exchangeColumnHeader9,
				exchangeColumnHeader10,
				exchangeColumnHeader11,
				exchangeColumnHeader12
			});
			base.ObjectList.FilterControl.ObjectSchema = ObjectSchema.GetInstance<ExtensibleMessageInfoSchema>();
			base.ObjectList.FilterControl.PropertiesToFilter.Add(new FilterablePropertyDescription(ExtensibleMessageInfoSchema.FromAddress, Strings.FromAddressColumn, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual,
				PropertyFilterOperator.Contains
			}));
			base.ObjectList.FilterControl.PropertiesToFilter.Add(new FilterablePropertyDescription(ExtensibleMessageInfoSchema.Status, Strings.StatusColumn, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual
			}));
			FilterablePropertyDescription filterablePropertyDescription = new FilterablePropertyDescription(ExtensibleMessageInfoSchema.Size, Strings.SizeColumn, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual,
				PropertyFilterOperator.GreaterThan,
				PropertyFilterOperator.GreaterThanOrEqual,
				PropertyFilterOperator.LessThan,
				PropertyFilterOperator.LessThanOrEqual
			});
			filterablePropertyDescription.ColumnType = typeof(ByteQuantifiedSize);
			filterablePropertyDescription.FormatMode = 6;
			base.ObjectList.FilterControl.PropertiesToFilter.Add(filterablePropertyDescription);
			base.ObjectList.FilterControl.PropertiesToFilter.Add(new FilterablePropertyDescription(ExtensibleMessageInfoSchema.SCL, Strings.SCLColumn, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual,
				PropertyFilterOperator.GreaterThan,
				PropertyFilterOperator.GreaterThanOrEqual,
				PropertyFilterOperator.LessThan,
				PropertyFilterOperator.LessThanOrEqual
			}));
			base.ObjectList.FilterControl.PropertiesToFilter.Add(new FilterablePropertyDescription(ExtensibleMessageInfoSchema.Queue, Strings.QueueIdColumn, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual,
				PropertyFilterOperator.Contains
			}));
			base.ObjectList.FilterControl.PropertiesToFilter.Add(new FilterablePropertyDescription(ExtensibleMessageInfoSchema.MessageSourceName, Strings.MessageSourceNameColumn, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual,
				PropertyFilterOperator.Contains
			}));
			base.ObjectList.FilterControl.PropertiesToFilter.Add(new FilterablePropertyDescription(ExtensibleMessageInfoSchema.LastError, Strings.LastErrorColumn, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual,
				PropertyFilterOperator.Contains,
				PropertyFilterOperator.Present,
				PropertyFilterOperator.NotPresent
			}));
			base.ObjectList.FilterControl.PropertiesToFilter.Add(new FilterablePropertyDescription(ExtensibleMessageInfoSchema.Subject, Strings.SubjectColumn, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual,
				PropertyFilterOperator.Contains,
				PropertyFilterOperator.Present,
				PropertyFilterOperator.NotPresent
			}));
			base.ObjectList.FilterControl.PropertiesToFilter.Add(new FilterablePropertyDescription(ExtensibleMessageInfoSchema.SourceIP, Strings.SourceIPColumn, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual
			}));
			base.ObjectList.FilterControl.PropertiesToFilter.Add(new FilterablePropertyDescription(ExtensibleMessageInfoSchema.InternetMessageId, Strings.InternetMessageIdColumn, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual,
				PropertyFilterOperator.Contains
			}));
			base.ObjectList.FilterControl.PropertiesToFilter.Add(new FilterablePropertyDescription(ExtensibleMessageInfoSchema.ExpirationTime, Strings.ExpirationTimeColumn, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.GreaterThan,
				PropertyFilterOperator.GreaterThanOrEqual,
				PropertyFilterOperator.LessThan,
				PropertyFilterOperator.LessThanOrEqual
			}));
			base.ObjectList.FilterControl.PropertiesToFilter.Add(new FilterablePropertyDescription(ExtensibleMessageInfoSchema.DateReceived, Strings.DateReceivedColumn, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.GreaterThan,
				PropertyFilterOperator.GreaterThanOrEqual,
				PropertyFilterOperator.LessThan,
				PropertyFilterOperator.LessThanOrEqual
			}));
			base.ObjectList.ListView.SelectionNameProperty = "Subject";
			base.ObjectList.ListView.SortProperty = "FromAddress";
			base.ObjectList.ListView.ImagePropertyName = "Status";
			base.ListControl.IconLibrary = QueueViewerMessagesResultPane.iconLibrary;
			base.SubscribedRefreshCategories.Add(RefreshCategories.Message);
		}

		protected override void SetupCommandsProfile()
		{
			base.SetupCommandsProfile();
			ResultsCommandProfile resultsCommandProfile = new ResultsCommandProfile
			{
				Command = new Command
				{
					Name = "freezeMessage",
					Icon = Icons.MessagesFrozen,
					Text = Strings.SuspendCommandText,
					Description = LocalizedString.Empty
				},
				Setting = new ResultsCommandSetting
				{
					Operation = CommandOperation.Update,
					IsSelectionCommand = true,
					UseSingleRowRefresh = false
				},
				Action = new MessagesTaskCommandAction
				{
					CommandText = "Suspend-Message"
				},
				UpdatingUtil = new SelectionCommandVisibilityBindingUtil
				{
					PropertyName = QueueViewerMessagesResultPane.CanSuspendString,
					TrueValue = true,
					AllowMixedValues = true
				}
			};
			ResultsCommandProfile resultsCommandProfile2 = new ResultsCommandProfile
			{
				Command = new Command
				{
					Name = "unfreezeMessage",
					Icon = Icons.MessagesQueued,
					Text = Strings.ResumeCommandText,
					Description = LocalizedString.Empty
				},
				Setting = new ResultsCommandSetting
				{
					Operation = CommandOperation.Update,
					IsSelectionCommand = true,
					UseSingleRowRefresh = false
				},
				Action = new MessagesTaskCommandAction
				{
					CommandText = "Resume-Message"
				},
				UpdatingUtil = new SelectionCommandVisibilityBindingUtil
				{
					PropertyName = QueueViewerMessagesResultPane.CanResumeString,
					TrueValue = true,
					AllowMixedValues = true
				}
			};
			ResultsCommandProfile resultsCommandProfile3 = ResultsCommandProfile.CreateSeparator();
			ResultsCommandProfile resultsCommandProfile4 = new ResultsCommandProfile
			{
				Command = new Command
				{
					Name = "deleteNDRMessage",
					Icon = Icons.MessagesPendingDelete,
					Text = Strings.RemoveMessageNDRCommandText,
					Description = LocalizedString.Empty
				},
				Setting = new ResultsCommandSetting
				{
					Operation = CommandOperation.Delete,
					IsSelectionCommand = true,
					IsRemoveCommand = true,
					UseSingleRowRefresh = false
				},
				Action = new MessagesTaskCommandAction
				{
					CommandText = "Remove-Message",
					Parameters = new MonadParameterCollection
					{
						new MonadParameter
						{
							ParameterName = "WithNDR",
							Value = true
						}
					},
					SingleSelectionConfirmation = new SingleSelectionMessageDelegate(Strings.RemoveMessageNDRConfirmSingleText),
					MultipleSelectionConfirmation = new MultipleSelectionMessageDelegate(Strings.RemoveMessageNDRConfirmMultiText)
				},
				UpdatingUtil = new SelectionCommandVisibilityBindingUtil
				{
					PropertyName = QueueViewerMessagesResultPane.CanRemoveString,
					TrueValue = true,
					AllowMixedValues = true
				}
			};
			ResultsCommandProfile resultsCommandProfile5 = new ResultsCommandProfile
			{
				Command = new Command
				{
					Name = "deleteMessage",
					Icon = Icons.MessagesPendingDelete,
					Text = Strings.RemoveMessageNoNDRCommandText,
					Description = LocalizedString.Empty
				},
				Setting = new ResultsCommandSetting
				{
					Operation = CommandOperation.Delete,
					IsSelectionCommand = true,
					UseSingleRowRefresh = false
				},
				Action = new MessagesTaskCommandAction
				{
					CommandText = "Remove-Message",
					Parameters = new MonadParameterCollection
					{
						new MonadParameter
						{
							ParameterName = "WithNDR",
							Value = false
						}
					},
					SingleSelectionConfirmation = new SingleSelectionMessageDelegate(Strings.RemoveMessageNoNDRConfirmSingleText),
					MultipleSelectionConfirmation = new MultipleSelectionMessageDelegate(Strings.RemoveMessageNoNDRConfirmMultiText)
				},
				UpdatingUtil = new SelectionCommandVisibilityBindingUtil
				{
					PropertyName = QueueViewerMessagesResultPane.CanRemoveString,
					TrueValue = true,
					AllowMixedValues = true
				}
			};
			base.CommandsProfile.CustomSelectionCommands.AddRange(new ResultsCommandProfile[]
			{
				resultsCommandProfile,
				resultsCommandProfile2,
				resultsCommandProfile3,
				resultsCommandProfile4,
				resultsCommandProfile5
			});
			ResultsCommandProfile item = new ResultsCommandProfile
			{
				Command = new Command
				{
					Name = "Properties",
					Icon = Icons.Properties,
					Text = Strings.ShowPropertiesCommand
				},
				Setting = new ResultsCommandSetting
				{
					Operation = CommandOperation.Update,
					IsSelectionCommand = true,
					IsPropertiesCommand = true,
					RequiresSingleSelection = true
				},
				Action = new ShowQueueViewerMessagePropertiesCommandAction()
			};
			base.CommandsProfile.ShowSelectionPropertiesCommands.Add(item);
		}

		protected override void SetUpDatasourceColumns()
		{
			DataColumn dataColumn = new DataColumn("FromAddress", typeof(string));
			base.Datasource.Columns.Add(dataColumn);
			dataColumn = new DataColumn("Status", typeof(EnumObject));
			base.Datasource.Columns.Add(dataColumn);
			dataColumn = new DataColumn("Size", typeof(ByteQuantifiedSize));
			base.Datasource.Columns.Add(dataColumn);
			dataColumn = new DataColumn("SCL", typeof(int));
			base.Datasource.Columns.Add(dataColumn);
			dataColumn = new DataColumn("Queue", typeof(QueueIdentity));
			base.Datasource.Columns.Add(dataColumn);
			dataColumn = new DataColumn("MessageSourceName", typeof(string));
			base.Datasource.Columns.Add(dataColumn);
			dataColumn = new DataColumn("LastError", typeof(string));
			base.Datasource.Columns.Add(dataColumn);
			dataColumn = new DataColumn("Subject", typeof(string));
			base.Datasource.Columns.Add(dataColumn);
			dataColumn = new DataColumn("SourceIP", typeof(IPAddress));
			base.Datasource.Columns.Add(dataColumn);
			dataColumn = new DataColumn("InternetMessageId", typeof(string));
			base.Datasource.Columns.Add(dataColumn);
			dataColumn = new DataColumn("ExpirationTime", typeof(DateTime));
			base.Datasource.Columns.Add(dataColumn);
			dataColumn = new DataColumn("DateReceived", typeof(DateTime));
			base.Datasource.Columns.Add(dataColumn);
			dataColumn = new DataColumn("Identity");
			base.Datasource.Columns.Add(dataColumn);
			base.Datasource.Table.PrimaryKey = new DataColumn[]
			{
				dataColumn
			};
			base.Datasource.Columns.Add(new DataColumn(QueueViewerMessagesResultPane.CanSuspendString, typeof(bool)));
			base.Datasource.Columns.Add(new DataColumn(QueueViewerMessagesResultPane.CanResumeString, typeof(bool)));
			base.Datasource.Columns.Add(new DataColumn(QueueViewerMessagesResultPane.CanRemoveString, typeof(bool)));
			base.Datasource.Table.RowChanged += this.Table_RowChanged;
		}

		private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
		{
			if (e.Action == DataRowAction.Add || e.Action == DataRowAction.Change)
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				DataRow row = e.Row;
				MessageStatus messageStatus = (MessageStatus)((EnumObject)row["Status"]);
				QueueIdentity queueIdentity = (QueueIdentity)row["Queue"];
				QueueType type = queueIdentity.Type;
				if (MessageStatus.PendingRemove != messageStatus && type != QueueType.Submission)
				{
					flag = true;
				}
				if (type != QueueType.Submission)
				{
					if (MessageStatus.Suspended == messageStatus)
					{
						flag3 = true;
					}
					else
					{
						flag2 = true;
					}
				}
				QueueViewerMessagesResultPane.UpdateColumnIfChanged(row, QueueViewerMessagesResultPane.CanSuspendString, flag2);
				QueueViewerMessagesResultPane.UpdateColumnIfChanged(row, QueueViewerMessagesResultPane.CanResumeString, flag3);
				QueueViewerMessagesResultPane.UpdateColumnIfChanged(row, QueueViewerMessagesResultPane.CanRemoveString, flag);
			}
		}

		private static void UpdateColumnIfChanged(DataRow row, string columnName, object value)
		{
			if (!value.Equals(row[columnName]))
			{
				row[columnName] = value;
			}
		}

		public override string SelectionHelpTopic
		{
			get
			{
				return SelectionHelpTopics.MessageInfo;
			}
		}

		private static readonly IconLibrary iconLibrary;

		private static readonly string CanRemoveString = "CanRemove";

		private static readonly string CanSuspendString = "CanSuspend";

		private static readonly string CanResumeString = "CanResume";
	}
}
