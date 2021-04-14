using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.MailboxUpgraders;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class ReceiveFolder
	{
		private ReceiveFolder(string messageClass, ExchangeId folderId, DateTime lastModificationTime)
		{
			this.messageClass = messageClass;
			this.folderId = folderId;
			this.lastModificationTime = lastModificationTime;
		}

		public string MessageClass
		{
			get
			{
				return this.messageClass;
			}
		}

		public ExchangeId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public DateTime LastModificationTime
		{
			get
			{
				return this.lastModificationTime;
			}
		}

		public static void Initialize()
		{
			FixReceiveFolderPK.InitializeUpgraderAction(new Action<Context, StoreDatabase>(ReceiveFolder.PerformDatabaseUpgrade), new Action<Context, StoreDatabase>(ReceiveFolder.InitInMemoryDatabaseSchema));
			UpgradeReceiveFolderContent.InitializeUpgraderAction(new Action<Context, Mailbox>(ReceiveFolder.PerformMailboxUpgrade));
		}

		public static ReceiveFolder GetReceiveFolder(Context context, Mailbox mailbox, string messageClass)
		{
			messageClass = messageClass.ToUpperInvariant();
			return ReceiveFolder.GetReceiveFolder(context, mailbox, messageClass, false);
		}

		public static IList<ReceiveFolder> GetReceiveFolders(Context context, Mailbox mailbox)
		{
			ICollection<string> allReceiveFolderMessageClasses = ReceiveFolder.GetAllReceiveFolderMessageClasses(context, mailbox);
			List<ReceiveFolder> list = new List<ReceiveFolder>(allReceiveFolderMessageClasses.Count);
			foreach (string text in allReceiveFolderMessageClasses)
			{
				list.Add(ReceiveFolder.GetReceiveFolder(context, mailbox, text, true));
			}
			return list;
		}

		public static void SetReceiveFolder(Context context, Mailbox mailbox, string messageClass, ExchangeId folderId)
		{
			messageClass = messageClass.ToUpperInvariant();
			object value = ReceiveFolder.KeyFromMessageClass(context, mailbox, messageClass);
			Table properReceiveFolderTable = ReceiveFolder.GetProperReceiveFolderTable(context, mailbox);
			ReceiveFolder receiveFolder = ReceiveFolder.GetReceiveFolder(context, mailbox, messageClass, true);
			if (receiveFolder != null)
			{
				if (receiveFolder.FolderId == folderId)
				{
					return;
				}
				if (folderId.IsNullOrZero)
				{
					Folder folder = Folder.OpenFolder(context, mailbox, receiveFolder.FolderId);
					int num = (int)folder.GetPropertyValue(context, PropTag.Folder.SetReceiveCount);
					folder.SetProperty(context, PropTag.Folder.SetReceiveCount, num - 1);
					folder.Save(context);
					using (DataRow dataRow = Factory.OpenDataRow(context.Culture, context, properReceiveFolderTable, true, new ColumnValue[]
					{
						new ColumnValue(properReceiveFolderTable.Columns[0], mailbox.MailboxNumber),
						new ColumnValue(properReceiveFolderTable.Columns[1], value)
					}))
					{
						dataRow.Delete(context);
					}
					return;
				}
			}
			if (folderId.IsValid)
			{
				Folder folder2 = Folder.OpenFolder(context, mailbox, folderId);
				if (folder2 == null)
				{
					throw new ObjectNotFoundException((LID)61432U, mailbox.MailboxGuid, string.Format("Folder {0} not found", folderId));
				}
				if (receiveFolder != null)
				{
					Folder folder3 = Folder.OpenFolder(context, mailbox, receiveFolder.FolderId);
					int num2 = (int)folder3.GetPropertyValue(context, PropTag.Folder.SetReceiveCount);
					folder3.SetProperty(context, PropTag.Folder.SetReceiveCount, num2 - 1);
					using (DataRow dataRow2 = Factory.OpenDataRow(context.Culture, context, properReceiveFolderTable, true, new ColumnValue[]
					{
						new ColumnValue(properReceiveFolderTable.Columns[0], mailbox.MailboxNumber),
						new ColumnValue(properReceiveFolderTable.Columns[1], value)
					}))
					{
						dataRow2.SetValue(context, properReceiveFolderTable.Columns[2], folderId.To26ByteArray());
						dataRow2.SetValue(context, properReceiveFolderTable.Columns[3], mailbox.UtcNow);
						dataRow2.Flush(context);
						goto IL_2E2;
					}
				}
				using (DataRow dataRow3 = Factory.CreateDataRow(context.Culture, context, properReceiveFolderTable, true, new ColumnValue[]
				{
					new ColumnValue(properReceiveFolderTable.Columns[0], mailbox.MailboxNumber),
					new ColumnValue(properReceiveFolderTable.Columns[1], value)
				}))
				{
					dataRow3.SetValue(context, properReceiveFolderTable.Columns[2], folderId.To26ByteArray());
					dataRow3.SetValue(context, properReceiveFolderTable.Columns[3], mailbox.UtcNow);
					dataRow3.Flush(context);
				}
				IL_2E2:
				int num3 = 0;
				if (folder2.GetPropertyValue(context, PropTag.Folder.SetReceiveCount) != null)
				{
					num3 = (int)folder2.GetPropertyValue(context, PropTag.Folder.SetReceiveCount);
				}
				folder2.SetProperty(context, PropTag.Folder.SetReceiveCount, num3 + 1);
			}
		}

		internal static void RemoveAllReceiveFolders(Context context, Mailbox mailbox)
		{
			Table properReceiveFolderTable = ReceiveFolder.GetProperReceiveFolderTable(context, mailbox);
			mailbox.RemoveMailboxEntriesFromTable(context, properReceiveFolderTable);
		}

		private static ReceiveFolder GetReceiveFolder(Context context, Mailbox mailbox, string messageClass, bool exactMatch)
		{
			Table properReceiveFolderTable = ReceiveFolder.GetProperReceiveFolderTable(context, mailbox);
			string text = messageClass;
			if (!exactMatch)
			{
				int num = -1;
				foreach (string text2 in ReceiveFolder.GetAllReceiveFolderMessageClasses(context, mailbox))
				{
					int num2 = ReceiveFolder.ReceiveFolderMessageClassMatch(text2, messageClass);
					if (num2 > num)
					{
						num = num2;
						text = text2;
					}
				}
			}
			ReceiveFolder result = null;
			if (text != null)
			{
				object value = ReceiveFolder.KeyFromMessageClass(context, mailbox, text);
				using (DataRow dataRow = Factory.OpenDataRow(context.Culture, context, properReceiveFolderTable, true, new ColumnValue[]
				{
					new ColumnValue(properReceiveFolderTable.Columns[0], mailbox.MailboxNumber),
					new ColumnValue(properReceiveFolderTable.Columns[1], value)
				}))
				{
					if (dataRow != null && !dataRow.IsDead)
					{
						messageClass = ReceiveFolder.MessageClassFromKey(context, mailbox, dataRow.GetValue(context, properReceiveFolderTable.Columns[1]));
						byte[] bytes = (byte[])dataRow.GetValue(context, properReceiveFolderTable.Columns[2]);
						ExchangeId exchangeId = ExchangeId.CreateFrom26ByteArray(context, mailbox.ReplidGuidMap, bytes);
						DateTime dateTime = (DateTime)dataRow.GetValue(context, properReceiveFolderTable.Columns[3]);
						result = new ReceiveFolder(messageClass, exchangeId, dateTime);
					}
				}
			}
			return result;
		}

		private static ICollection<string> GetAllReceiveFolderMessageClasses(Context context, Mailbox mailbox)
		{
			List<string> list = new List<string>(10);
			Table properReceiveFolderTable = ReceiveFolder.GetProperReceiveFolderTable(context, mailbox);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxNumber
			});
			Column[] columnsToFetch = new Column[]
			{
				properReceiveFolderTable.Columns[1]
			};
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, properReceiveFolderTable, properReceiveFolderTable.PrimaryKeyIndex, columnsToFetch, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						string item = ReceiveFolder.MessageClassFromKey(context, mailbox, reader.GetValue(properReceiveFolderTable.Columns[1]));
						list.Add(item);
					}
				}
			}
			return list;
		}

		private static int ReceiveFolderMessageClassMatch(string value, string pattern)
		{
			int result = 0;
			if (!string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(pattern) && pattern.StartsWith(value, StringComparison.OrdinalIgnoreCase) && (pattern.Length == value.Length || pattern[value.Length] == '.'))
				{
					result = value.Length;
				}
				else
				{
					result = -1;
				}
			}
			return result;
		}

		private static void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			ReceiveFolder2Table receiveFolder2Table = DatabaseSchema.ReceiveFolder2Table(database);
			receiveFolder2Table.Table.MinVersion = FixReceiveFolderPK.Instance.To.Value;
		}

		private static void PerformDatabaseUpgrade(Context context, StoreDatabase database)
		{
			ReceiveFolder2Table receiveFolder2Table = DatabaseSchema.ReceiveFolder2Table(database);
			receiveFolder2Table.Table.CreateTable(context, FixReceiveFolderPK.Instance.To.Value);
		}

		private static void PerformMailboxUpgrade(Context context, Mailbox mailbox)
		{
			ReceiveFolderTable receiveFolderTable = DatabaseSchema.ReceiveFolderTable(context.Database);
			ReceiveFolder2Table receiveFolder2Table = DatabaseSchema.ReceiveFolder2Table(context.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxNumber
			});
			Column[] columnsToFetch = new Column[]
			{
				receiveFolderTable.MessageClass,
				receiveFolderTable.FolderId,
				receiveFolderTable.LastModificationTime,
				receiveFolderTable.ExtensionBlob
			};
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, receiveFolderTable.Table, receiveFolderTable.Table.PrimaryKeyIndex, columnsToFetch, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						string @string = reader.GetString(receiveFolderTable.MessageClass);
						byte[] binary = reader.GetBinary(receiveFolderTable.FolderId);
						DateTime dateTime = reader.GetDateTime(receiveFolderTable.LastModificationTime);
						byte[] binary2 = reader.GetBinary(receiveFolderTable.ExtensionBlob);
						byte[] array = new byte[@string.Length];
						ParseSerialize.SerializeAsciiString(@string, array, 0);
						using (DataRow dataRow = Factory.CreateDataRow(context.Culture, context, receiveFolder2Table.Table, true, new ColumnValue[]
						{
							new ColumnValue(receiveFolder2Table.MailboxNumber, mailbox.MailboxNumber),
							new ColumnValue(receiveFolder2Table.MessageClass, array)
						}))
						{
							dataRow.SetValue(context, receiveFolder2Table.FolderId, binary);
							dataRow.SetValue(context, receiveFolder2Table.LastModificationTime, dateTime);
							if (binary2 != null)
							{
								dataRow.SetValue(context, receiveFolder2Table.ExtensionBlob, binary2);
							}
							dataRow.Flush(context);
						}
					}
				}
			}
			mailbox.RemoveMailboxEntriesFromTable(context, receiveFolderTable.Table);
		}

		private static Table GetProperReceiveFolderTable(Context context, Mailbox mailbox)
		{
			Table table;
			if (UpgradeReceiveFolderContent.IsReady(context, mailbox))
			{
				table = DatabaseSchema.ReceiveFolder2Table(mailbox.Database).Table;
			}
			else
			{
				table = DatabaseSchema.ReceiveFolderTable(mailbox.Database).Table;
			}
			return table;
		}

		private static string MessageClassFromKey(Context context, Mailbox mailbox, object keyValue)
		{
			string result;
			if (UpgradeReceiveFolderContent.IsReady(context, mailbox))
			{
				byte[] array = (byte[])keyValue;
				result = ParseSerialize.ParseAsciiString(array, 0, array.Length);
			}
			else
			{
				result = (string)keyValue;
			}
			return result;
		}

		private static object KeyFromMessageClass(Context context, Mailbox mailbox, string messageClass)
		{
			if (UpgradeReceiveFolderContent.IsReady(context, mailbox))
			{
				byte[] array = new byte[messageClass.Length];
				ParseSerialize.SerializeAsciiString(messageClass, array, 0);
				return array;
			}
			return messageClass;
		}

		private const int ColumnIndexMailboxNumber = 0;

		private const int ColumnIndexMessageClass = 1;

		private const int ColumnIndexFolderId = 2;

		private const int ColumnIndexLastModificationTime = 3;

		private const int ColumnIndexExtensionBlob = 4;

		private readonly ExchangeId folderId;

		private readonly string messageClass;

		private readonly DateTime lastModificationTime;
	}
}
