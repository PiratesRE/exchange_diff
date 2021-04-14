using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Interop;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Common.ImportContacts
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OutlookCsvImportContact : CsvImportContact
	{
		public OutlookCsvImportContact(MailboxSession mbxSession) : base(mbxSession)
		{
		}

		public int ImportContactsFromOutlookCSV(Stream csvStream)
		{
			SyncUtilities.ThrowIfArgumentNull("csvStream", csvStream);
			Dictionary<string, ImportContactProperties> columnsMapping;
			Encoding encoding;
			CultureInfo culture;
			this.DetectLanguage(csvStream, out columnsMapping, out encoding, out culture);
			ImportContactsCsvSchema schema = new ImportContactsCsvSchema(columnsMapping, culture);
			return base.ImportContactsFromCSV(schema, csvStream, encoding);
		}

		protected void DetectLanguage(Stream csvStream, out Dictionary<string, ImportContactProperties> selectedMapping, out Encoding selectedEncoding, out CultureInfo selectedCulture)
		{
			selectedMapping = null;
			selectedEncoding = null;
			selectedCulture = null;
			int num = 0;
			if (csvStream.Length <= 0L)
			{
				throw new ContactCsvFileEmptyException();
			}
			int headerSize;
			Encoding[] array = this.GetHeaderEncoding(csvStream, out headerSize);
			array = this.FillMissingEncodings(array);
			foreach (Encoding encoding in array)
			{
				csvStream.Seek(0L, SeekOrigin.Begin);
				CsvRow? csvRow = ImportContactsCsvSchema.ReadCSVHeader(csvStream, encoding);
				if (csvRow == null)
				{
					throw new ContactCsvFileEmptyException();
				}
				csvStream.Seek(0L, SeekOrigin.Begin);
				List<OutlookCsvLanguage> languagesForCodepage = OutlookCsvLanguageSelect.Instance.GetLanguagesForCodepage(encoding.WindowsCodePage);
				foreach (OutlookCsvLanguage outlookCsvLanguage in languagesForCodepage)
				{
					ImportContactsCsvSchema schema = new ImportContactsCsvSchema(outlookCsvLanguage.ColumnsMapping, outlookCsvLanguage.Culture);
					int num2 = base.CountRecognizedColumns(schema, csvRow.Value);
					if (num2 > num)
					{
						CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)18UL, "Selecting Language {0} with codepage {1} and {2} known columns.", new object[]
						{
							outlookCsvLanguage.Culture.DisplayName,
							encoding.WindowsCodePage,
							num2
						});
						num = num2;
						selectedMapping = outlookCsvLanguage.ColumnsMapping;
						selectedEncoding = encoding;
						selectedCulture = outlookCsvLanguage.Culture;
					}
				}
			}
			if (selectedMapping == null)
			{
				CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)19UL, "No Language found for the detected codepage and read columns.", new object[0]);
				throw new ContactCsvFileContainsNoKnownColumnsException();
			}
			if (selectedEncoding.WindowsCodePage == OutlookCsvLanguageSelect.Instance.EnglishUS.CodePage && selectedMapping == OutlookCsvLanguageSelect.Instance.EnglishUS.ColumnsMapping)
			{
				Encoding bodyEncoding = this.GetBodyEncoding(csvStream, headerSize);
				if (bodyEncoding != null)
				{
					selectedEncoding = bodyEncoding;
				}
			}
		}

		protected Encoding[] FillMissingEncodings(Encoding[] encodingArray)
		{
			List<Encoding> list = new List<Encoding>(encodingArray.Length);
			bool flag = false;
			foreach (Encoding encoding in encodingArray)
			{
				list.Add(encoding);
				int windowsCodePage = encoding.WindowsCodePage;
				if (windowsCodePage != 932)
				{
					if (windowsCodePage == 1252)
					{
						if (!this.ContainsEncodingWithCodepage(encodingArray, 1253))
						{
							list.Add(Encoding.GetEncoding(1253));
							flag = true;
						}
						if (!this.ContainsEncodingWithCodepage(encodingArray, 1250))
						{
							list.Add(Encoding.GetEncoding(1250));
							flag = true;
						}
					}
				}
				else if (!this.ContainsEncodingWithCodepage(encodingArray, 936))
				{
					list.Add(Encoding.GetEncoding(936));
					flag = true;
				}
			}
			if (!flag)
			{
				return encodingArray;
			}
			return list.ToArray();
		}

		protected int GetFirstLineSizeInStream(Stream csvStream)
		{
			Stream stream = Streams.CreateSuppressCloseWrapper(csvStream);
			int length;
			using (StreamReader streamReader = new StreamReader(stream, Encoding.ASCII))
			{
				string text = streamReader.ReadLine();
				length = text.Length;
			}
			return length;
		}

		protected Encoding[] GetHeaderEncoding(Stream csvStream, out int headerSize)
		{
			csvStream.Seek(0L, SeekOrigin.Begin);
			headerSize = this.GetFirstLineSizeInStream(csvStream);
			CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)1280UL, "Detected header size : {0}.", new object[]
			{
				headerSize
			});
			csvStream.Seek(0L, SeekOrigin.Begin);
			int num = Math.Min(headerSize, 4096);
			if (num <= 0)
			{
				CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)1281UL, "Stream contains no header.", new object[0]);
				throw new ContactCsvFileEmptyException();
			}
			byte[] array = new byte[num];
			csvStream.Read(array, 0, num);
			csvStream.Seek(0L, SeekOrigin.Begin);
			CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)1282UL, "Detecting header encoding.", new object[0]);
			Encoding[] bytesEncoding = this.GetBytesEncoding(array);
			if (bytesEncoding.Length == 0)
			{
				CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)25UL, "No codepage foung in the list, using system default codepage.", new object[0]);
				return new Encoding[]
				{
					Encoding.Default
				};
			}
			return bytesEncoding;
		}

		protected Encoding GetBodyEncoding(Stream csvStream, int headerSize)
		{
			csvStream.Seek((long)headerSize, SeekOrigin.Begin);
			int num = (int)Math.Min(csvStream.Length - (long)headerSize, 4096L);
			if (num <= 0)
			{
				CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)1283UL, "Stream contains no body.", new object[0]);
				throw new ContactCsvFileEmptyException();
			}
			byte[] array = new byte[num];
			csvStream.Read(array, 0, num);
			csvStream.Seek(0L, SeekOrigin.Begin);
			CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)1284UL, "Detecting body encoding.", new object[0]);
			Encoding[] bytesEncoding = this.GetBytesEncoding(array);
			if (bytesEncoding.Length == 0)
			{
				return null;
			}
			return bytesEncoding[0];
		}

		protected Encoding[] GetBytesEncoding(byte[] byteArray)
		{
			int num = 100;
			List<Encoding> list = new List<Encoding>(num);
			CMultiLanguage cmultiLanguage = null;
			try
			{
				cmultiLanguage = new CMultiLanguage();
				if (cmultiLanguage == null)
				{
					CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)20UL, "Failure to create COM multilanguage object.", new object[0]);
					throw new InternalErrorSavingContactException(0, 0);
				}
				IMultiLanguage2 multiLanguage = (IMultiLanguage2)cmultiLanguage;
				int num2 = num;
				int num3 = byteArray.Length;
				DetectEncodingInfo[] array = new DetectEncodingInfo[num];
				lock (OutlookCsvImportContact.syncRoot)
				{
					multiLanguage.DetectInputCodepage(MLDETECTCP.MLDETECTCP_NONE, 0U, byteArray, ref num3, array, ref num2);
				}
				if (num2 > 0)
				{
					CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)21UL, "Detected codepages : {0}.", new object[]
					{
						num2
					});
					for (int i = 0; i < num2; i++)
					{
						CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)22UL, "Codepage : {0}, Confidence : {1}, Doc Percentage : {2}.", new object[]
						{
							array[i].CodePage,
							array[i].Confidence,
							array[i].DocPercent
						});
						Encoding encoding = Encoding.GetEncoding((int)array[i].CodePage);
						if (encoding.WindowsCodePage != encoding.CodePage)
						{
							bool flag2 = false;
							foreach (Encoding encoding2 in list)
							{
								if (encoding2.CodePage == encoding.WindowsCodePage)
								{
									flag2 = true;
									break;
								}
							}
							if (!flag2)
							{
								list.Add(Encoding.GetEncoding(encoding.WindowsCodePage));
							}
						}
						list.Add(encoding);
					}
				}
				else
				{
					CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)23UL, "No codepage detected by DetectInputCodepage method.", new object[0]);
				}
			}
			catch (COMException ex)
			{
				CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)24UL, "Caught COMException while detecting codepage: {0}.", new object[]
				{
					ex
				});
			}
			finally
			{
				if (cmultiLanguage != null)
				{
					Marshal.FinalReleaseComObject(cmultiLanguage);
				}
			}
			return list.ToArray();
		}

		private bool ContainsEncodingWithCodepage(Encoding[] encodingArray, int windowsCodePage)
		{
			foreach (Encoding encoding in encodingArray)
			{
				if (encoding.WindowsCodePage == windowsCodePage)
				{
					return true;
				}
			}
			return false;
		}

		private const int CodepageDetectionFileSampleSize = 4096;

		private static readonly object syncRoot = new object();
	}
}
