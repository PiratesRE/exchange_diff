using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Cobalt;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class CobaltStore : DisposeTrackableBase
	{
		public string OwnerEmailAddress
		{
			get
			{
				return this.userAddress;
			}
		}

		public int EditorCount
		{
			get
			{
				return this.editorCount;
			}
		}

		internal CobaltStoreSaver Saver
		{
			get
			{
				return this.saver;
			}
		}

		internal string WorkingDirectory
		{
			get
			{
				return this.workingDirectory;
			}
		}

		static CobaltStore()
		{
			CobaltHostServices.Initialize();
		}

		public CobaltStore(string parentDirectory, string userId, string correlationId, bool diagnosticsEnabled, int memoryBudget)
		{
			if (string.IsNullOrEmpty(parentDirectory))
			{
				throw new ArgumentException("CobaltStore parent directory must be specified.");
			}
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentException("CobaltStore user ID must be specified.");
			}
			if (string.IsNullOrEmpty(correlationId))
			{
				throw new ArgumentException("CobaltStore correlation ID must be specified.");
			}
			this.userAddress = userId;
			this.correlationId = correlationId;
			this.diagnosticsEnabled = diagnosticsEnabled;
			this.workingDirectory = Path.Combine(parentDirectory, this.correlationId);
			Directory.CreateDirectory(this.workingDirectory);
			this.storeDisposalEscrow = new DisposalEscrow("CobaltStore");
			this.InitializePartitionBlobStores(memoryBudget);
			this.synchronizationObject = new object();
			this.saver = new CobaltStoreSaver();
		}

		public override string ToString()
		{
			return this.userAddress + "-" + this.correlationId;
		}

		public void Save(Stream stream)
		{
			this.SaveDiagnosticStream(stream, "Original.bin");
			lock (this.synchronizationObject)
			{
				using (DisposalEscrow disposalEscrow = new DisposalEscrow("CobaltStore.Save"))
				{
					CobaltFile cobaltFile = this.CreateCobaltFile(disposalEscrow, true);
					CobaltFilePartition cobaltFilePartition = cobaltFile.GetCobaltFilePartition(FilePartitionId.Content);
					Metrics metrics;
					cobaltFilePartition.SetStream(GenericFda.ContentStreamId, new BytesFromStream(stream), ref metrics);
					cobaltFilePartition.CommitChanges();
					this.SaveDiagnosticDocument(cobaltFile, "AfterSave.bin");
				}
			}
		}

		public Stream GetDocumentStream()
		{
			Stream result;
			lock (this.synchronizationObject)
			{
				using (DisposalEscrow disposalEscrow = new DisposalEscrow("CobaltStore.GetDocumentStream"))
				{
					CobaltFile cobaltFile = this.CreateCobaltFile(disposalEscrow, false);
					CobaltFilePartition cobaltFilePartition = cobaltFile.GetCobaltFilePartition(FilePartitionId.Content);
					Bytes stream = cobaltFilePartition.GetStream(CobaltFilePartition.ContentStreamId);
					Stream stream2 = new StreamFromBytes(stream, 0UL);
					Stream stream3 = new StreamDisposalWrapper(stream2, disposalEscrow.Transfer("GetDocumentStream-StreamDisposalWrapper"));
					result = stream3;
				}
			}
			return result;
		}

		public void SaveFailed(Exception exception)
		{
			this.permanentException = exception;
		}

		public void ProcessRequest(Stream requestStream, Stream responseStream, Action<Enum, string> logDetail)
		{
			if (this.permanentException != null)
			{
				throw new CobaltStore.OrphanedCobaltStoreException(string.Format("The attachment is no longer available for CobaltStore {0}.", this.correlationId), this.permanentException);
			}
			try
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				if (!Monitor.TryEnter(this.synchronizationObject, TimeSpan.FromSeconds(15.0)))
				{
					throw new Exception("Unable to acquire CobaltStore lock.");
				}
				stopwatch.Stop();
				logDetail(WacRequestHandlerMetadata.LockWaitTime, stopwatch.ElapsedMilliseconds.ToString());
				using (DisposalEscrow disposalEscrow = new DisposalEscrow("CobaltStore.ProcessRequest"))
				{
					using (Stream stream = new MemoryStream(65536))
					{
						requestStream.CopyTo(stream, 65536);
						stream.Position = 0L;
						logDetail(WacRequestHandlerMetadata.CobaltRequestLength, stream.Length.ToString());
						CobaltFile cobaltFile = this.CreateCobaltFile(disposalEscrow, false);
						this.SaveDiagnosticDocument(cobaltFile, "BeforeRoundTrip.bin");
						this.SaveDiagnosticStream(requestStream, "Request.xml");
						using (DisposableAtomFromStream disposableAtomFromStream = new DisposableAtomFromStream(stream))
						{
							Roundtrip roundtrip = cobaltFile.CobaltEndpoint.CreateRoundtrip();
							bool exceptionThrown = false;
							Atom atom = null;
							Stopwatch stopwatch2 = new Stopwatch();
							stopwatch2.Start();
							try
							{
								object obj;
								ProtocolVersion protocolVersion;
								roundtrip.DeserializeInputFromProtocol(disposableAtomFromStream, ref obj, ref protocolVersion);
								roundtrip.Execute();
								cobaltFile.CommitChanges();
								atom = roundtrip.SerializeOutputToProtocol(1, obj, null);
							}
							catch (Exception)
							{
								exceptionThrown = true;
								throw;
							}
							finally
							{
								stopwatch2.Stop();
								logDetail(WacRequestHandlerMetadata.CobaltTime, stopwatch2.ElapsedMilliseconds.ToString());
								this.LogBlobStoreMetrics(logDetail, cobaltFile);
								this.LogRequestDetails(logDetail, roundtrip, exceptionThrown);
							}
							this.UpdateEditorCount(roundtrip.RequestBatch);
							this.SaveDiagnosticDocument(cobaltFile, "AfterRoundTrip.bin");
							atom.CopyTo(responseStream);
							logDetail(WacRequestHandlerMetadata.CobaltResponseLength, atom.Length.ToString());
							if (this.diagnosticsEnabled)
							{
								using (MemoryStream memoryStream = new MemoryStream())
								{
									atom.CopyTo(memoryStream);
									this.SaveDiagnosticStream(memoryStream, "Response.xml");
								}
							}
						}
					}
				}
			}
			finally
			{
				if (Monitor.IsEntered(this.synchronizationObject))
				{
					Monitor.Exit(this.synchronizationObject);
				}
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.storeDisposalEscrow != null)
				{
					this.storeDisposalEscrow.Dispose();
					this.storeDisposalEscrow = null;
				}
				if (this.saver != null)
				{
					this.saver.Dispose();
					this.saver = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CobaltStore>(this);
		}

		private void SaveDiagnosticDocument(CobaltFile cobaltFile, string fileName)
		{
			if (!this.diagnosticsEnabled)
			{
				return;
			}
			CobaltFilePartition cobaltFilePartition = cobaltFile.GetCobaltFilePartition(FilePartitionId.Content);
			Bytes stream = cobaltFilePartition.GetStream(CobaltFilePartition.ContentStreamId);
			string filePath = this.GetFilePath(fileName);
			using (StreamFromBytes streamFromBytes = new StreamFromBytes(stream, 0UL))
			{
				using (FileStream fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
				{
					streamFromBytes.CopyTo(fileStream);
				}
			}
		}

		private string GetFilePath(string fileName)
		{
			string path = WacUtilities.GetCurrentTimeForFileName() + "-" + fileName;
			return Path.Combine(this.workingDirectory, path);
		}

		private void InitializePartitionBlobStores(int memoryBudget)
		{
			TemporaryHostBlobStore.Config config = new TemporaryHostBlobStore.Config();
			config.AllocateBsn = (() => (ulong)Interlocked.Increment(ref CobaltStore.bsn));
			config.StartBsn = (() => (ulong)CobaltStore.bsn);
			this.contentBlobStore = this.CreateBlobStore(config, memoryBudget, "Content");
			this.metadataBlobStore = this.CreateBlobStore(config, memoryBudget, "Metadata");
			this.editorTableBlobStore = this.CreateBlobStore(config, memoryBudget, "Editors");
			this.convertedDocumentBlobStore = this.CreateBlobStore(config, memoryBudget, "WWConv");
			this.updateBlobStore = this.CreateBlobStore(config, memoryBudget, "WWUpdate");
		}

		private HostBlobStore CreateBlobStore(TemporaryHostBlobStore.Config configuration, int memoryBudget, string directoryName)
		{
			string text = Path.Combine(this.workingDirectory, directoryName);
			Directory.CreateDirectory(text);
			TemporaryHostBlobStore temporaryHostBlobStore = new TemporaryHostBlobStore(configuration, this.storeDisposalEscrow, "CobaltStore." + directoryName, (long)memoryBudget, text, false);
			this.blobStores.Add(temporaryHostBlobStore);
			return temporaryHostBlobStore;
		}

		private CobaltFile CreateCobaltFile(DisposalEscrow disposalEscrow, bool isNewFile)
		{
			if (CobaltStore.partitionNames == null)
			{
				Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
				Dictionary<Guid, string> dictionary2 = dictionary;
				FilePartitionId content = FilePartitionId.Content;
				dictionary2.Add(content.GuidId, "C");
				Dictionary<Guid, string> dictionary3 = dictionary;
				FilePartitionId coauthMetadata = FilePartitionId.CoauthMetadata;
				dictionary3.Add(coauthMetadata.GuidId, "CAM");
				Dictionary<Guid, string> dictionary4 = dictionary;
				FilePartitionId o14EditorsTable = FilePartitionId.O14EditorsTable;
				dictionary4.Add(o14EditorsTable.GuidId, "O14E");
				Dictionary<Guid, string> dictionary5 = dictionary;
				FilePartitionId wordWacConvertedDocument = FilePartitionId.WordWacConvertedDocument;
				dictionary5.Add(wordWacConvertedDocument.GuidId, "WWCD");
				Dictionary<Guid, string> dictionary6 = dictionary;
				FilePartitionId wordWacUpdate = FilePartitionId.WordWacUpdate;
				dictionary6.Add(wordWacUpdate.GuidId, "WWU");
				CobaltStore.partitionNames = dictionary;
			}
			CobaltFilePartitionConfig value = new CobaltFilePartitionConfig
			{
				PartitionId = new FilePartitionId?(FilePartitionId.Content),
				HostBlobStore = this.contentBlobStore,
				Schema = 2,
				IsNewFile = isNewFile
			};
			CobaltFilePartitionConfig value2 = new CobaltFilePartitionConfig
			{
				PartitionId = new FilePartitionId?(FilePartitionId.CoauthMetadata),
				HostBlobStore = this.metadataBlobStore,
				Schema = 2,
				IsNewFile = isNewFile
			};
			CobaltFilePartitionConfig value3 = new CobaltFilePartitionConfig
			{
				PartitionId = new FilePartitionId?(FilePartitionId.O14EditorsTable),
				HostBlobStore = this.editorTableBlobStore,
				Schema = 2,
				IsNewFile = isNewFile
			};
			CobaltFilePartitionConfig value4 = new CobaltFilePartitionConfig
			{
				PartitionId = new FilePartitionId?(FilePartitionId.WordWacConvertedDocument),
				HostBlobStore = this.convertedDocumentBlobStore,
				Schema = 2,
				IsNewFile = isNewFile
			};
			CobaltFilePartitionConfig value5 = new CobaltFilePartitionConfig
			{
				PartitionId = new FilePartitionId?(FilePartitionId.WordWacUpdate),
				HostBlobStore = this.updateBlobStore,
				Schema = 2,
				IsNewFile = isNewFile
			};
			return new CobaltFile(disposalEscrow, new Dictionary<FilePartitionId, CobaltFilePartitionConfig>
			{
				{
					FilePartitionId.Content,
					value
				},
				{
					FilePartitionId.CoauthMetadata,
					value2
				},
				{
					FilePartitionId.O14EditorsTable,
					value3
				},
				{
					FilePartitionId.WordWacConvertedDocument,
					value4
				},
				{
					FilePartitionId.WordWacUpdate,
					value5
				}
			}, new CobaltServerLockingStore(this), null);
		}

		private void SaveDiagnosticStream(Stream input, string fileName)
		{
			if (!this.diagnosticsEnabled)
			{
				return;
			}
			string filePath = this.GetFilePath(fileName);
			long position = input.Position;
			input.Position = 0L;
			using (Stream stream = File.Open(filePath, FileMode.Create, FileAccess.Write))
			{
				input.CopyTo(stream);
			}
			input.Position = position;
		}

		private static string GetRequestNameAndPartition(Request request)
		{
			string str;
			if (!CobaltStore.partitionNames.TryGetValue(request.PartitionId.GuidId, out str))
			{
				str = request.PartitionId.GuidId.ToString();
			}
			return request.GetType().Name + "." + str;
		}

		private void LogRequestDetails(Action<Enum, string> logDetail, Roundtrip roundtrip, bool exceptionThrown)
		{
			string arg = string.Join(" ", from request in roundtrip.RequestBatch.Requests
			select CobaltStore.GetRequestNameAndPartition(request));
			logDetail(WacRequestHandlerMetadata.CobaltOperations, arg);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (Request request2 in roundtrip.RequestBatch.Requests)
			{
				if (request2.Failed)
				{
					flag = true;
					stringBuilder.Append(request2.GetType().Name);
					stringBuilder.Append(" failed. ");
					try
					{
						string conciseLoggingStatement = Log.GetConciseLoggingStatement(request2, this.userAddress);
						if (exceptionThrown || request2.Failed)
						{
							stringBuilder.AppendLine(conciseLoggingStatement);
						}
					}
					catch (ErrorException)
					{
						stringBuilder.AppendLine("Concise logging not supported.");
					}
				}
			}
			try
			{
				roundtrip.ThrowIfAnyError();
			}
			catch (Exception ex)
			{
				exceptionThrown = true;
				stringBuilder.AppendLine("ThrowIfAnyError: " + ex.ToString());
			}
			if (exceptionThrown || flag)
			{
				logDetail(WacRequestHandlerMetadata.ErrorDetails, stringBuilder.ToString());
			}
		}

		private void LogBlobStoreMetrics(Action<Enum, string> logDetail, CobaltFile cobaltFile)
		{
			int num = 0;
			int num2 = 0;
			ulong num3 = 0UL;
			ulong num4 = 0UL;
			long num5 = 0L;
			long num6 = 0L;
			foreach (HostBlobStore hostBlobStore in this.blobStores)
			{
				HostBlobStoreMetrics metrics = hostBlobStore.GetMetrics();
				if (metrics != null)
				{
					num += metrics.ReadBlobsFound + metrics.ReadBlobsNotFound;
					num2 += metrics.WrittenBlobs;
					num3 += metrics.ReadBlobBytes;
					num4 += metrics.WrittenBlobBytes;
					TemporaryHostBlobStoreMetrics tempHostBlobStoreMetrics = ((TemporaryHostBlobStore)hostBlobStore).TempHostBlobStoreMetrics;
					num5 += (long)tempHostBlobStoreMetrics.NumberOfBlobsOnDisk;
					num6 += tempHostBlobStoreMetrics.TotalSizeOfBlobsOnDisk;
				}
			}
			logDetail(WacRequestHandlerMetadata.CobaltReads, num.ToString());
			logDetail(WacRequestHandlerMetadata.CobaltWrites, num2.ToString());
			logDetail(WacRequestHandlerMetadata.CobaltBytesRead, num3.ToString());
			logDetail(WacRequestHandlerMetadata.CobaltBytesWritten, num4.ToString());
			logDetail(WacRequestHandlerMetadata.DiskBlobCount, num5.ToString());
			logDetail(WacRequestHandlerMetadata.DiskBlobSize, num6.ToString());
		}

		private void UpdateEditorCount(RequestBatch batch)
		{
			foreach (Request request in batch.Requests)
			{
				if (request is JoinCoauthoringRequest)
				{
					this.editorCount++;
				}
				else if (request is ExitCoauthoringRequest)
				{
					this.Saver.SaveAndThrowExceptions();
					this.editorCount--;
				}
			}
		}

		internal const string CobaltSubdirectory = "OwaCobalt";

		private const int myBlockSize = 65536;

		private static long bsn = 0L;

		private static IReadOnlyDictionary<Guid, string> partitionNames;

		private readonly string userAddress;

		private readonly string correlationId;

		private readonly string workingDirectory;

		private readonly bool diagnosticsEnabled;

		private HostBlobStore contentBlobStore;

		private HostBlobStore metadataBlobStore;

		private HostBlobStore editorTableBlobStore;

		private HostBlobStore convertedDocumentBlobStore;

		private HostBlobStore updateBlobStore;

		private List<TemporaryHostBlobStore> blobStores = new List<TemporaryHostBlobStore>(5);

		private DisposalEscrow storeDisposalEscrow;

		private object synchronizationObject;

		private CobaltStoreSaver saver;

		private Exception permanentException;

		private int editorCount;

		private class OrphanedCobaltStoreException : Exception
		{
			public OrphanedCobaltStoreException(string message, Exception exception) : base(message, exception)
			{
			}
		}
	}
}
