using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.StsUpdate;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Update
{
	internal class VerifyContent
	{
		public VerifyContent()
		{
			this.recordsArray = new ArrayList();
		}

		public bool IsFullDownload
		{
			get
			{
				return this.fileHeader.FileType == VerifyContent.UpdateFileType.FullUpdate;
			}
		}

		public string MajorVersion
		{
			get
			{
				return this.fileHeader.MajorVersion;
			}
		}

		public string MinorVersion
		{
			get
			{
				return this.fileHeader.MinorVersion;
			}
		}

		public DateTime ExpirationTime
		{
			get
			{
				return this.fileHeader.ExpTime;
			}
		}

		public ArrayList SenderRecords
		{
			get
			{
				return this.recordsArray;
			}
		}

		public bool Parse(byte[] data)
		{
			if (StsUpdateAgent.EndProcessing)
			{
				return true;
			}
			if (data == null)
			{
				ExTraceGlobals.OnDownloadTracer.TraceError((long)this.GetHashCode(), "Empty data bytes in VerifyContent.Parse");
				return false;
			}
			MemoryStream memoryStream = null;
			BinaryReader binaryReader = null;
			try
			{
				memoryStream = new MemoryStream(data);
				binaryReader = new BinaryReader(memoryStream);
				if (binaryReader.PeekChar() != -1)
				{
					VerifyContent.UpdateFileType updateFileType = (VerifyContent.UpdateFileType)binaryReader.ReadByte();
					if (!VerifyContent.ValidateUpdateFileType(updateFileType))
					{
						ExTraceGlobals.OnDownloadTracer.TraceError((long)this.GetHashCode(), "VerifyContent.Parse: corrupt data file.");
						StsUpdateAgent.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_UpdateAgentFileNotLoaded, null, null);
						return false;
					}
					string text = binaryReader.ReadString();
					string text2 = binaryReader.ReadString();
					int num = binaryReader.ReadInt32();
					int nRecords = binaryReader.ReadInt32();
					if (StsUpdateAgent.CompareAndSetCurrentVersion(text, text2) == StsUpdateAgent.VersionComparison.Eq)
					{
						return false;
					}
					ExTraceGlobals.OnDownloadTracer.TraceDebug((long)this.GetHashCode(), "Download file header, type:{0}, majorVer:{1}, minorVer:{2}, expTime:{3}", new object[]
					{
						updateFileType,
						text,
						text2,
						num
					});
					this.fileHeader = new VerifyContent.Header(updateFileType, text, text2, num);
					if (this.ParseRecords(binaryReader, nRecords) && !StsUpdateAgent.EndProcessing)
					{
						this.StoreRecords();
					}
				}
			}
			catch (EndOfStreamException ex)
			{
				ExTraceGlobals.OnDownloadTracer.TraceError<string>((long)this.GetHashCode(), "EndOfStreamException in Parse, error: {0}", ex.Message);
			}
			catch (IOException ex2)
			{
				ExTraceGlobals.OnDownloadTracer.TraceError<string>((long)this.GetHashCode(), "IOException in Parse, error: {0}", ex2.Message);
			}
			finally
			{
				if (binaryReader != null)
				{
					binaryReader.Close();
				}
				if (memoryStream != null)
				{
					memoryStream.Close();
				}
			}
			return true;
		}

		private void StoreRecords()
		{
			Database.TruncateSenderReputationTable(ExTraceGlobals.DatabaseTracer);
			foreach (object obj in this.SenderRecords)
			{
				VerifyContent.SenderReputationRecord senderReputationRecord = (VerifyContent.SenderReputationRecord)obj;
				if (StsUpdateAgent.EndProcessing)
				{
					return;
				}
				switch (senderReputationRecord.RecordType)
				{
				case VerifyContent.UpdateRecordType.Add:
				case VerifyContent.UpdateRecordType.Update:
					Database.UpdateSenderReputationData(senderReputationRecord.SenderAddrHash, senderReputationRecord.Srl, senderReputationRecord.IsOpenProxy, this.ExpirationTime, ExTraceGlobals.DatabaseTracer);
					break;
				case VerifyContent.UpdateRecordType.Delete:
					Database.DeleteSenderReputationData(senderReputationRecord.SenderAddrHash, ExTraceGlobals.DatabaseTracer);
					break;
				}
			}
			Database.UpdateConfiguration("majorversion", this.MajorVersion, ExTraceGlobals.DatabaseTracer);
			Database.UpdateConfiguration("minorversion", this.MinorVersion, ExTraceGlobals.DatabaseTracer);
			StsUpdateAgent.PerformanceCounters.TotalUpdate();
		}

		private bool ParseRecords(BinaryReader reader, int nRecords)
		{
			if (reader == null)
			{
				return false;
			}
			bool result;
			try
			{
				int num = 0;
				while (num < nRecords && !StsUpdateAgent.EndProcessing)
				{
					int num2 = 0;
					bool proxy = false;
					int num3 = (int)reader.ReadByte();
					if (num3 < 10)
					{
						ExTraceGlobals.OnDownloadTracer.TraceError<int>((long)this.GetHashCode(), "Invalid record length: {0}", num3);
						return false;
					}
					byte[] senderAddrHash = reader.ReadBytes(CMD5.HashSize());
					byte b = reader.ReadByte();
					VerifyContent.UpdateRecordType updateRecordType = (VerifyContent.UpdateRecordType)((b & 224) >> 5);
					if (!VerifyContent.ValidateUpdateRecordType(updateRecordType))
					{
						ExTraceGlobals.OnDownloadTracer.TraceError<int>((long)this.GetHashCode(), "Invalid record type: {0}", (b & 224) >> 5);
					}
					else
					{
						if (updateRecordType != VerifyContent.UpdateRecordType.Delete)
						{
							proxy = ((b & 1) == 1);
							num2 = (b & 30) >> 1;
							if (num2 == 15)
							{
								num2 = -1;
							}
							if (num2 < -1 || num2 > 9)
							{
								ExTraceGlobals.OnDownloadTracer.TraceError<int>((long)this.GetHashCode(), "Invalid srl value: {0}", num2);
								return false;
							}
						}
						if (this.IsFullDownload && updateRecordType != VerifyContent.UpdateRecordType.Add)
						{
							ExTraceGlobals.OnDownloadTracer.TraceError<int>((long)this.GetHashCode(), "Received record type: {0} in full download", num3);
							return false;
						}
						VerifyContent.SenderReputationRecord senderReputationRecord = null;
						if (StsUpdateAgent.EndProcessing)
						{
							return true;
						}
						switch (updateRecordType)
						{
						case VerifyContent.UpdateRecordType.Add:
						case VerifyContent.UpdateRecordType.Update:
							senderReputationRecord = new VerifyContent.SenderReputationRecord(updateRecordType, senderAddrHash, num2, proxy);
							break;
						case VerifyContent.UpdateRecordType.Delete:
							senderReputationRecord = new VerifyContent.SenderReputationRecord(senderAddrHash);
							break;
						}
						if (senderReputationRecord != null)
						{
							this.recordsArray.Add(senderReputationRecord);
						}
					}
					reader.ReadBytes(num3 - 1 - CMD5.HashSize() - 1);
					num++;
				}
				if (reader.PeekChar() != -1)
				{
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch (EndOfStreamException ex)
			{
				ExTraceGlobals.OnDownloadTracer.TraceError<string>((long)this.GetHashCode(), "EndOfStreamException in ParseRecords, error: {0}", ex.Message);
				result = false;
			}
			catch (IOException ex2)
			{
				ExTraceGlobals.OnDownloadTracer.TraceError<string>((long)this.GetHashCode(), "IOException in Parse, error: {0}", ex2.Message);
				result = false;
			}
			return result;
		}

		private static bool ValidateUpdateFileType(VerifyContent.UpdateFileType updateFileType)
		{
			return updateFileType >= VerifyContent.UpdateFileType.Incremental && updateFileType <= VerifyContent.UpdateFileType.FullUpdate;
		}

		private static bool ValidateUpdateRecordType(VerifyContent.UpdateRecordType updateRecordType)
		{
			return updateRecordType >= VerifyContent.UpdateRecordType.Add && updateRecordType <= VerifyContent.UpdateRecordType.Update;
		}

		private VerifyContent.Header fileHeader;

		private ArrayList recordsArray;

		private struct Header
		{
			public Header(VerifyContent.UpdateFileType fileType, string majorVersion, string minorVersion, int expTimeInMinutes)
			{
				this.FileType = fileType;
				this.MajorVersion = majorVersion;
				this.MinorVersion = minorVersion;
				this.ExpTime = DateTime.UtcNow.AddMinutes((double)expTimeInMinutes);
			}

			public VerifyContent.UpdateFileType FileType;

			public string MajorVersion;

			public string MinorVersion;

			public DateTime ExpTime;
		}

		private class SenderReputationRecord
		{
			public SenderReputationRecord(VerifyContent.UpdateRecordType recType, byte[] senderAddrHash, int srl, bool proxy)
			{
				this.recType = recType;
				this.srl = srl;
				this.proxy = proxy;
				this.senderAddrHash = senderAddrHash;
			}

			public SenderReputationRecord(byte[] senderAddrHash)
			{
				this.recType = VerifyContent.UpdateRecordType.Delete;
				this.senderAddrHash = senderAddrHash;
			}

			public byte[] SenderAddrHash
			{
				get
				{
					return this.senderAddrHash;
				}
			}

			public VerifyContent.UpdateRecordType RecordType
			{
				get
				{
					return this.recType;
				}
			}

			public bool IsOpenProxy
			{
				get
				{
					return this.proxy;
				}
			}

			public int Srl
			{
				get
				{
					return this.srl;
				}
			}

			private VerifyContent.UpdateRecordType recType;

			private byte[] senderAddrHash;

			private int srl;

			private bool proxy;
		}

		private enum UpdateFileType : byte
		{
			Incremental,
			FullUpdate
		}

		private enum UpdateRecordType : byte
		{
			Add = 1,
			Delete,
			Update
		}
	}
}
