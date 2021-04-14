using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.PST;
using Microsoft.Exchange.PST.Common;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class PstWriter : IContextualBatchDataWriter<List<ItemInformation>>, IBatchDataWriter<List<ItemInformation>>, IDisposable
	{
		public PstWriter(PstTarget target, IProgressController progressController)
		{
			this.progressController = progressController;
			this.target = target;
			this.timer = new Stopwatch();
			this.targetFolderProvider = new PstTargetFolderProvider();
			this.pstMBFileCount = 1;
			this.unsearchableMBTotalSize = 0L;
			this.searchableMBTotalSize = 0L;
			this.pstPFFileCount = 1;
			this.unsearchablePFTotalSize = 0L;
			this.searchablePFTotalSize = 0L;
		}

		public void EnterDataContext(DataContext context)
		{
			Tracer.TraceInformation("PstWriter.EnterDataContext: {0}", new object[]
			{
				context.SourceId
			});
			LocalFileHelper.CallFileOperation(delegate
			{
				string path = context.IsUnsearchable ? this.target.ExportContext.TargetLocation.UnsearchableExportLocation : this.target.ExportContext.TargetLocation.ExportLocation;
				DirectoryInfo directoryInfo = new DirectoryInfo(path);
				if (!directoryInfo.Exists)
				{
					Tracer.TraceInformation("PstWriter.EnterDataContext: Creating directory '{0}'", new object[]
					{
						directoryInfo.FullName
					});
					Directory.CreateDirectory(directoryInfo.FullName);
				}
			}, ExportErrorType.FailedToCreateExportLocation);
			this.dataContext = context;
			this.timer.Restart();
			this.targetFolderProvider.Reset(this.dataContext);
			if (this.dataContext.IsPublicFolder)
			{
				long num = 0L;
				foreach (ItemId itemId in this.dataContext.ItemIdList.ReadItemIds())
				{
					num += (long)((ulong)itemId.Size);
				}
				if (this.dataContext.IsUnsearchable)
				{
					if (this.unsearchablePFTotalSize > 0L && this.unsearchablePFTotalSize + num > ConstantProvider.PSTSizeLimitInBytes && num < ConstantProvider.PSTSizeLimitInBytes)
					{
						this.ReOpenPFDataContext();
						this.unsearchablePFTotalSize = 0L;
					}
				}
				else if (this.searchablePFTotalSize > 0L && this.searchablePFTotalSize + num > ConstantProvider.PSTSizeLimitInBytes && num < ConstantProvider.PSTSizeLimitInBytes)
				{
					this.ReOpenPFDataContext();
					this.searchablePFTotalSize = 0L;
				}
				this.currentPFRecord = (this.dataContext.IsUnsearchable ? this.pfUnsearchableExportResult : this.pfExportResult);
				if (this.currentPFRecord == null && this.dataContext.ItemCount > 0)
				{
					this.EnsurePFPstFileCreated();
				}
				if (this.currentPFRecord != null && this.pstPFSession == null)
				{
					this.pstPFSession = PstWriter.CreatePstSession(this.currentPFRecord.ExportFile.Path);
					Tracer.TraceInformation("PstWriter.EnterDataContext: PST pf session created.", new object[0]);
					return;
				}
			}
			else
			{
				this.pstMBFileCount = 1;
				this.unsearchableMBTotalSize = 0L;
				this.searchableMBTotalSize = 0L;
				this.currentRootRecord = (this.dataContext.IsUnsearchable ? this.rootUnsearchableExportResult : this.rootExportResult);
				if (this.currentRootRecord == null && this.dataContext.ItemCount > 0)
				{
					this.EnsurePstFileCreated();
				}
				if (this.currentRootRecord != null)
				{
					this.pstSession = PstWriter.CreatePstSession(this.currentRootRecord.ExportFile.Path);
					Tracer.TraceInformation("PstWriter.EnterDataContext: PST session created.", new object[0]);
				}
			}
		}

		public void ExitDataContext(bool errorHappened)
		{
			if (this.dataContext != null)
			{
				Tracer.TraceInformation("PstWriter.EnterDataContext: SourceId:{0}; errorHappened:{1}", new object[]
				{
					this.dataContext.SourceId,
					errorHappened
				});
				if (this.pstSession != null)
				{
					Tracer.TraceInformation("PstWriter.ExitDataContext: Closing PST session", new object[0]);
					this.pstSession.Close();
					this.pstSession = null;
					Tracer.TraceInformation("PstWriter.ExitDataContext: PST session closed", new object[0]);
				}
				if (this.target.ExportContext.ExportMetadata.IncludeDuplicates && !this.dataContext.IsPublicFolder)
				{
					if (!errorHappened)
					{
						this.ReportFileInformation(this.currentRootRecord);
					}
					this.currentRootRecord = null;
					this.rootExportResult = null;
					this.rootUnsearchableExportResult = null;
				}
				this.dataContext = null;
			}
		}

		public void ExitPFDataContext(bool errorHappened)
		{
			Tracer.TraceInformation("PstWriter.EnterDataContext: errorHappened:{0}", new object[]
			{
				errorHappened
			});
			if (this.pstPFSession != null)
			{
				Tracer.TraceInformation("PstWriter.ExitDataContext: Closing public folder PST session", new object[0]);
				this.pstPFSession.Close();
				this.pstPFSession = null;
				Tracer.TraceInformation("PstWriter.ExitDataContext: PST public folder session closed", new object[0]);
			}
			if (this.currentPFRecord != null)
			{
				if (!errorHappened)
				{
					this.ReportFileInformation(this.currentPFRecord);
				}
				this.currentPFRecord = null;
				this.pfExportResult = null;
				this.pfUnsearchableExportResult = null;
			}
		}

		public void ReOpenMBDataContext()
		{
			Tracer.TraceInformation("PstWriter.ReOpenMBDataContext: SourceId:{0}", new object[]
			{
				this.dataContext.SourceId
			});
			if (this.pstSession != null)
			{
				Tracer.TraceInformation("PstWriter.ReOpenMBDataContext: Closing mailbox PST session", new object[0]);
				this.pstSession.Close();
				this.pstSession = null;
				Tracer.TraceInformation("PstWriter.ReOpenMBDataContext: PST mailbox session closed", new object[0]);
			}
			if (this.currentRootRecord != null)
			{
				this.ReportFileInformation(this.currentRootRecord);
				this.currentRootRecord = null;
				if (!this.dataContext.IsUnsearchable)
				{
					this.rootExportResult = null;
				}
				else
				{
					this.rootUnsearchableExportResult = null;
				}
			}
			this.currentRootRecord = (this.dataContext.IsUnsearchable ? this.rootUnsearchableExportResult : this.rootExportResult);
			if (this.currentRootRecord == null && this.dataContext.ItemCount > 0)
			{
				this.EnsurePstFileCreated();
			}
			if (this.currentRootRecord != null && this.pstSession == null)
			{
				this.pstSession = PstWriter.CreatePstSession(this.currentRootRecord.ExportFile.Path);
				this.targetFolderProvider.Reset(this.dataContext);
				Tracer.TraceInformation("PstWriter.ReOpenMBDataContext: PST session created.", new object[0]);
			}
		}

		public void ReOpenPFDataContext()
		{
			Tracer.TraceInformation("PstWriter.ReOpenPFDataContext: SourceId:{0}", new object[]
			{
				this.dataContext.SourceId
			});
			if (this.pstPFSession != null)
			{
				Tracer.TraceInformation("PstWriter.ReOpenPFDataContext: Closing public folder PST session", new object[0]);
				this.pstPFSession.Close();
				this.pstPFSession = null;
				Tracer.TraceInformation("PstWriter.ReOpenPFDataContext: PST public folder session closed", new object[0]);
			}
			if (this.currentPFRecord != null)
			{
				this.ReportFileInformation(this.currentPFRecord);
				this.currentPFRecord = null;
				if (!this.dataContext.IsUnsearchable)
				{
					this.pfExportResult = null;
				}
				else
				{
					this.pfUnsearchableExportResult = null;
				}
			}
			this.currentPFRecord = (this.dataContext.IsUnsearchable ? this.pfUnsearchableExportResult : this.pfExportResult);
			if (this.currentPFRecord == null && this.dataContext.ItemCount > 0)
			{
				this.EnsurePFPstFileCreated();
			}
			if (this.currentPFRecord != null && this.pstPFSession == null)
			{
				this.pstPFSession = PstWriter.CreatePstSession(this.currentPFRecord.ExportFile.Path);
				this.targetFolderProvider.Reset(this.dataContext);
				Tracer.TraceInformation("PstWriter.ReOpenPFDataContext: PST pf session created.", new object[0]);
			}
		}

		public void WriteDataBatch(List<ItemInformation> dataBatch)
		{
			Tracer.TraceInformation("PstWriter.WriteDataBatch: Writing data batch for mailbox '{0}', item count: {1}", new object[]
			{
				this.dataContext.SourceId,
				dataBatch.Count
			});
			ProgressRecord progressRecord = new ProgressRecord(this.dataContext);
			PSTSession pstsession = this.dataContext.IsPublicFolder ? this.pstPFSession : this.pstSession;
			try
			{
				foreach (ItemInformation itemInformation in dataBatch)
				{
					if (this.progressController.IsStopRequested)
					{
						Tracer.TraceInformation("PstWriter.WriteDataBatch: Stop requested when processing mailbox '{0}'", new object[]
						{
							this.dataContext.SourceId
						});
						break;
					}
					try
					{
						if (this.dataContext.IsPublicFolder)
						{
							if (this.dataContext.IsUnsearchable)
							{
								if (this.unsearchablePFTotalSize > 0L && this.unsearchablePFTotalSize + (long)((ulong)itemInformation.Id.Size) > ConstantProvider.PSTSizeLimitInBytes)
								{
									this.ReOpenPFDataContext();
									pstsession = this.pstPFSession;
									this.unsearchablePFTotalSize = 0L;
								}
								this.unsearchablePFTotalSize += (long)((ulong)itemInformation.Id.Size);
							}
							else
							{
								if (this.searchablePFTotalSize > 0L && this.searchablePFTotalSize + (long)((ulong)itemInformation.Id.Size) > ConstantProvider.PSTSizeLimitInBytes)
								{
									this.ReOpenPFDataContext();
									pstsession = this.pstPFSession;
									this.searchablePFTotalSize = 0L;
								}
								this.searchablePFTotalSize += (long)((ulong)itemInformation.Id.Size);
							}
						}
						else if (this.dataContext.IsUnsearchable)
						{
							if (this.unsearchableMBTotalSize > 0L && this.unsearchableMBTotalSize + (long)((ulong)itemInformation.Id.Size) > ConstantProvider.PSTSizeLimitInBytes)
							{
								this.ReOpenMBDataContext();
								pstsession = this.pstSession;
								this.unsearchableMBTotalSize = 0L;
							}
							this.unsearchableMBTotalSize += (long)((ulong)itemInformation.Id.Size);
						}
						else
						{
							if (this.searchableMBTotalSize > 0L && this.searchableMBTotalSize + (long)((ulong)itemInformation.Id.Size) > ConstantProvider.PSTSizeLimitInBytes)
							{
								this.ReOpenMBDataContext();
								pstsession = this.pstSession;
								this.searchableMBTotalSize = 0L;
							}
							this.searchableMBTotalSize += (long)((ulong)itemInformation.Id.Size);
						}
						if (itemInformation.Error == null)
						{
							if (itemInformation.Id.IsDuplicate)
							{
								progressRecord.ReportItemExported(itemInformation.Id, null, null);
							}
							else
							{
								IFolder parentFolder = this.targetFolderProvider.GetParentFolder(pstsession, itemInformation.Id.ParentFolder, this.target.ExportContext.ExportMetadata.IncludeDuplicates);
								if (parentFolder == null)
								{
									if (!this.dataContext.IsPublicFolder)
									{
										progressRecord.ReportItemError(itemInformation.Id, this.currentRootRecord, ExportErrorType.ParentFolderNotFound, string.Format(CultureInfo.CurrentCulture, "Parent folder '{0}' is missing, this may caused by newly created folder", new object[]
										{
											itemInformation.Id.ParentFolder
										}));
										Tracer.TraceError("PstWriter.WriteDataBatch: Failed to get the parent folder of current message to export. Mailbox: '{0}', Message Id: '{1}', Folder Path: '{2}'", new object[]
										{
											this.dataContext.SourceId,
											itemInformation.Id.Id,
											itemInformation.Id.ParentFolder
										});
									}
									else
									{
										progressRecord.ReportItemError(itemInformation.Id, this.currentPFRecord, ExportErrorType.ParentFolderNotFound, string.Format(CultureInfo.CurrentCulture, "Parent folder '{0}' is missing, this may caused by newly created folder", new object[]
										{
											itemInformation.Id.ParentFolder
										}));
										Tracer.TraceError("PstWriter.WriteDataBatch: Failed to get the parent folder of current message to export. Mailbox: '{0}', Message Id: '{1}', Folder Path: '{2}'", new object[]
										{
											this.dataContext.SourceId,
											itemInformation.Id.Id,
											itemInformation.Id.ParentFolder
										});
									}
								}
								else
								{
									IMessage message = PstWriter.CreatePstMessage(pstsession, parentFolder, itemInformation, !this.target.ExportContext.ExportMetadata.IncludeDuplicates);
									if (!this.dataContext.IsPublicFolder)
									{
										progressRecord.ReportItemExported(itemInformation.Id, this.currentRootRecord.ExportFile.Name + '/' + PstWriter.CreateEntryIdFromNodeId(pstsession.MessageStore.Guid, message.Id), this.currentRootRecord);
									}
									else
									{
										progressRecord.ReportItemExported(itemInformation.Id, this.currentPFRecord.ExportFile.Name + '/' + PstWriter.CreateEntryIdFromNodeId(pstsession.MessageStore.Guid, message.Id), this.currentPFRecord);
									}
								}
							}
						}
						else if (!this.dataContext.IsPublicFolder)
						{
							progressRecord.ReportItemError(itemInformation.Id, this.currentRootRecord, itemInformation.Error.ErrorType, itemInformation.Error.Message);
						}
						else
						{
							progressRecord.ReportItemError(itemInformation.Id, this.currentPFRecord, itemInformation.Error.ErrorType, itemInformation.Error.Message);
						}
					}
					catch (ExportException ex)
					{
						Tracer.TraceError("PstWriter.WriteDataBatch: Exception happed. Mailbox:'{0}'. Exception:'{1}'", new object[]
						{
							this.dataContext.SourceId,
							ex
						});
						if (!this.dataContext.IsPublicFolder)
						{
							progressRecord.ReportItemError(itemInformation.Id, this.currentRootRecord, ex.ErrorType, ex.Message);
						}
						else
						{
							progressRecord.ReportItemError(itemInformation.Id, this.currentPFRecord, ex.ErrorType, ex.Message);
						}
					}
				}
			}
			finally
			{
				this.timer.Stop();
				progressRecord.ReportDuration(this.timer.Elapsed);
				this.progressController.ReportProgress(progressRecord);
				this.timer.Restart();
			}
		}

		public void Close()
		{
			Tracer.TraceInformation("PstWriter.Close: Closing PstWriter", new object[0]);
			if (!this.target.ExportContext.ExportMetadata.IncludeDuplicates)
			{
				if (!this.progressController.IsStopRequested)
				{
					this.ReportFileInformation(this.rootExportResult);
					this.ReportFileInformation(this.rootUnsearchableExportResult);
					this.ReportFileInformation(this.pfExportResult);
					this.ReportFileInformation(this.pfUnsearchableExportResult);
				}
				this.rootExportResult = null;
				this.rootUnsearchableExportResult = null;
				this.currentRootRecord = null;
				this.pfExportResult = null;
				this.pfUnsearchableExportResult = null;
				this.currentPFRecord = null;
				if (this.pstPFSession != null)
				{
					Tracer.TraceInformation("PstWriter.Close: Closing public folder PST session", new object[0]);
					this.pstPFSession.Close();
					this.pstPFSession = null;
					Tracer.TraceInformation("PstWriter.Close: PST public folder session closed", new object[0]);
				}
			}
		}

		void IDisposable.Dispose()
		{
			this.Close();
		}

		internal static IMessage CreatePstMessage(IPST pstSession, IFolder pstFolder, ItemInformation item, bool removeMetadata)
		{
			IMessage message = pstFolder.AddMessage();
			ExtractContext extractContext = new ExtractContext(pstSession, item);
			extractContext.EnterMessageContext(message);
			FastTransferStreamExtractor fastTransferStreamExtractor = new FastTransferStreamExtractor(extractContext, removeMetadata);
			fastTransferStreamExtractor.Extract();
			extractContext.ExitMessageContext();
			return message;
		}

		internal static PSTSession CreatePstSession(string pstFilePath)
		{
			Tracer.TraceInformation("PstWriter.CreatePstSession: Creating PST session for PST file '{0}'", new object[]
			{
				pstFilePath
			});
			PSTSession pstsession = new PSTSession(new Tracer.TraceMethod(Tracer.TraceInformation), new Tracer.TraceMethod(Tracer.TraceWarning), new Tracer.TraceMethod(Tracer.TraceError));
			try
			{
				pstsession.Open(pstFilePath, true, true, false);
			}
			catch (PSTExceptionBase pstexceptionBase)
			{
				Tracer.TraceError("PstWriter.CreatePstSession: Exception: {0}", new object[]
				{
					pstexceptionBase
				});
				throw new ExportException(ExportErrorType.FailedToOpenPstFile, pstexceptionBase);
			}
			return pstsession;
		}

		private static string CreateEntryIdFromNodeId(Guid guid, uint nodeId)
		{
			byte[] array = new byte[24];
			Array.Copy(guid.ToByteArray(), 0, array, 4, 16);
			Array.Copy(BitConverter.GetBytes(nodeId), 0, array, 20, 4);
			return Convert.ToBase64String(array);
		}

		private void EnsurePstFileCreated()
		{
			if (this.currentRootRecord == null)
			{
				if (this.target.ExportContext.ExportMetadata.IncludeDuplicates)
				{
					this.CreatePstFile(this.target.GetPstFilePath(this.dataContext.SourceName, this.dataContext.IsUnsearchable, this.pstMBFileCount));
				}
				else
				{
					this.CreatePstFile(this.target.GetPstFilePath(null, this.dataContext.IsUnsearchable, this.pstMBFileCount));
				}
				this.pstMBFileCount++;
			}
		}

		private void EnsurePFPstFileCreated()
		{
			if (this.currentPFRecord == null)
			{
				this.CreatePstFile(this.target.GetPFPstFilePath(this.dataContext.IsUnsearchable, this.pstPFFileCount));
				this.pstPFFileCount++;
			}
		}

		private void CreatePstFile(string filePath)
		{
			PstWriter.CreatePstSession(filePath).Close();
			ProgressRecord progressRecord = new ProgressRecord();
			ExportRecord exportRecord = new ExportRecord
			{
				ExportFile = new ExportFile
				{
					Name = Path.GetFileName(filePath),
					Path = filePath
				},
				SourceId = null,
				Id = Path.GetFileNameWithoutExtension(filePath),
				OriginalPath = null,
				Parent = null,
				Title = null,
				DocumentType = "File",
				RelationshipType = "None",
				IsUnsearchable = this.dataContext.IsUnsearchable
			};
			if (this.dataContext.IsPublicFolder)
			{
				this.currentPFRecord = exportRecord;
				if (this.dataContext.IsUnsearchable)
				{
					this.pfUnsearchableExportResult = this.currentPFRecord;
				}
				else
				{
					this.pfExportResult = this.currentPFRecord;
				}
				progressRecord.ReportRootRecord(this.currentPFRecord);
			}
			else
			{
				this.currentRootRecord = exportRecord;
				if (this.dataContext.IsUnsearchable)
				{
					this.rootUnsearchableExportResult = this.currentRootRecord;
				}
				else
				{
					this.rootExportResult = this.currentRootRecord;
				}
				progressRecord.ReportRootRecord(this.currentRootRecord);
			}
			this.progressController.ReportProgress(progressRecord);
		}

		private void ReportFileInformation(ExportRecord rootExportRecord)
		{
			if (rootExportRecord != null)
			{
				byte[] array;
				using (SHA256CryptoServiceProvider sha256CryptoServiceProvider = new SHA256CryptoServiceProvider())
				{
					using (FileStream fileStream = new FileStream(rootExportRecord.ExportFile.Path, FileMode.Open, FileAccess.Read, FileShare.Read, 16777216))
					{
						array = sha256CryptoServiceProvider.ComputeHash(fileStream);
						rootExportRecord.ExportFile.Size = (ulong)fileStream.Length;
					}
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2", CultureInfo.InvariantCulture));
				}
				rootExportRecord.ExportFile.Hash = "SHA256:" + stringBuilder.ToString();
				ProgressRecord progressRecord = new ProgressRecord();
				progressRecord.ReportRootRecord(rootExportRecord);
				this.progressController.ReportProgress(progressRecord);
			}
		}

		private readonly Stopwatch timer;

		private readonly PstTargetFolderProvider targetFolderProvider;

		private readonly PstTarget target;

		private readonly IProgressController progressController;

		private DataContext dataContext;

		private ExportRecord currentRootRecord;

		private ExportRecord rootExportResult;

		private ExportRecord rootUnsearchableExportResult;

		private ExportRecord currentPFRecord;

		private ExportRecord pfExportResult;

		private ExportRecord pfUnsearchableExportResult;

		private PSTSession pstSession;

		private PSTSession pstPFSession;

		private int pstPFFileCount;

		private int pstMBFileCount;

		private long unsearchableMBTotalSize;

		private long searchableMBTotalSize;

		private long unsearchablePFTotalSize;

		private long searchablePFTotalSize;
	}
}
