using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Common.ImportContacts
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CsvImportContact : ImportContactBase
	{
		public CsvImportContact(MailboxSession mbxSession) : base(mbxSession)
		{
		}

		protected CsvImportContact()
		{
		}

		public int ImportContactsFromCSV(ImportContactsCsvSchema schema, Stream csvStream, Encoding encoding)
		{
			SyncUtilities.ThrowIfArgumentNull("schema", schema);
			SyncUtilities.ThrowIfArgumentNull("csvStream", csvStream);
			SyncUtilities.ThrowIfArgumentNull("encoding", encoding);
			int num;
			List<ImportContactObject> list = this.ReadContactsFromCSV(schema, csvStream, encoding, out num);
			CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)10UL, "Contacts Collected: {0}, Skipped: {1}. Saving to mailbox.", new object[]
			{
				list.Count,
				num
			});
			return base.SaveContactsToMailbox(list, schema.Culture);
		}

		protected int CountRecognizedColumns(ImportContactsCsvSchema schema, CsvRow header)
		{
			SyncUtilities.ThrowIfArgumentNull("schema", schema);
			SyncUtilities.ThrowIfArgumentNull("header", header);
			if (schema.ColumnsMapping == null)
			{
				throw new InvalidOperationException("Schema object needs to have valid ColumnsMapping object.");
			}
			int num = 0;
			for (int i = 0; i < header.ColumnMap.Count; i++)
			{
				if (schema.ColumnsMapping.ContainsKey(header.ColumnMap[i]))
				{
					num++;
				}
			}
			return num;
		}

		protected List<ImportContactObject> ReadContactsFromCSV(ImportContactsCsvSchema schema, Stream csvStream, Encoding encoding, out int rowsSkipped)
		{
			rowsSkipped = 0;
			int capacity = 50;
			List<ImportContactObject> list = new List<ImportContactObject>(capacity);
			try
			{
				using (IEnumerator<CsvRow> enumerator = schema.Read(csvStream, encoding, false, false).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						if (this.CountRecognizedColumns(schema, enumerator.Current) == 0)
						{
							throw new ContactCsvFileContainsNoKnownColumnsException();
						}
						while (enumerator.MoveNext())
						{
							CsvRow csvRow = enumerator.Current;
							CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)11UL, "Processing Row {0} of the csv file.", new object[]
							{
								csvRow.Index
							});
							ImportContactObject importContactObject = new ImportContactObject(csvRow.Index);
							foreach (KeyValuePair<string, string> keyValuePair in csvRow.GetExistingValues())
							{
								importContactObject.AddProperty(keyValuePair.Key, keyValuePair.Value, schema.ColumnsMapping, schema.Culture);
							}
							if (importContactObject.PropertyCount > 0)
							{
								list.Add(importContactObject);
							}
							else
							{
								CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)12UL, "Skipping Row {0} because it has no properties.", new object[]
								{
									csvRow.Index
								});
								rowsSkipped++;
							}
						}
					}
				}
			}
			catch (CsvDuplicatedColumnException innerException)
			{
				throw new InvalidCsvFileFormatException(innerException);
			}
			catch (CsvWrongNumberOfColumnsException innerException2)
			{
				throw new InvalidCsvFileFormatException(innerException2);
			}
			catch (CsvTooManyRowsException innerException3)
			{
				throw new ContactCsvFileTooLargeException(2000, innerException3);
			}
			if (list.Count <= 0)
			{
				throw new ContactCsvFileEmptyException();
			}
			return list;
		}
	}
}
