using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class PstTarget : ITarget
	{
		public PstTarget(IExportContext exportContext)
		{
			this.ExportContext = exportContext;
		}

		public IExportContext ExportContext { get; private set; }

		public ExportSettings ExportSettings { get; set; }

		public IStatusLog GetStatusLog()
		{
			return new PstStatusLog(this.GetStatusFilePath());
		}

		public IItemIdList CreateItemIdList(string mailboxId, bool isUnsearchable)
		{
			return new LocalFileItemIdList(mailboxId, this.GetItemIdListFilePath(mailboxId, isUnsearchable), isUnsearchable);
		}

		public void RemoveItemIdList(string mailboxId, bool isUnsearchable)
		{
			LocalFileHelper.RemoveFile(this.GetItemIdListFilePath(mailboxId, isUnsearchable), ExportErrorType.FailedToRemoveItemIdList);
		}

		public IContextualBatchDataWriter<List<ItemInformation>> CreateDataWriter(IProgressController progressController)
		{
			return new PstWriter(this, progressController);
		}

		public string GetStatusFilePath()
		{
			return Path.Combine(this.ExportContext.TargetLocation.WorkingLocation, Uri.EscapeDataString(this.ExportContext.ExportMetadata.ExportName) + ".status");
		}

		public string GetPstFilePath(string sourceName, bool isUnsearchable, int pstMBFileCount)
		{
			string text = string.IsNullOrEmpty(sourceName) ? string.Empty : ("-" + sourceName);
			string text2 = string.Empty;
			if (pstMBFileCount > 1)
			{
				text2 = string.Format(CultureInfo.CurrentCulture, "{0}{1}-{2}-{3}", new object[]
				{
					this.ExportContext.ExportMetadata.ExportName,
					text,
					this.ExportSettings.ExportTime.ToString("MM.dd.yyyy-HHmmtt", CultureInfo.InvariantCulture),
					pstMBFileCount
				});
			}
			else
			{
				text2 = string.Format(CultureInfo.CurrentCulture, "{0}{1}-{2}", new object[]
				{
					this.ExportContext.ExportMetadata.ExportName,
					text,
					this.ExportSettings.ExportTime.ToString("MM.dd.yyyy-HHmmtt", CultureInfo.InvariantCulture)
				});
			}
			string path = ((text2.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) ? Uri.EscapeDataString(text2) : text2) + (isUnsearchable ? "_unsearchable.pst" : ".pst");
			return Path.Combine(isUnsearchable ? this.ExportContext.TargetLocation.UnsearchableExportLocation : this.ExportContext.TargetLocation.ExportLocation, path);
		}

		public string GetPFPstFilePath(bool isUnsearchable, int pstPFFileCount)
		{
			string text = "-Public Folders";
			string text2 = string.Format(CultureInfo.CurrentCulture, "{0}{1}-{2}-{3}", new object[]
			{
				this.ExportContext.ExportMetadata.ExportName,
				text,
				this.ExportSettings.ExportTime.ToString("MM.dd.yyyy-HHmmtt", CultureInfo.InvariantCulture),
				pstPFFileCount
			});
			string path = ((text2.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) ? Uri.EscapeDataString(text2) : text2) + (isUnsearchable ? "_unsearchable.pst" : ".pst");
			return Path.Combine(isUnsearchable ? this.ExportContext.TargetLocation.UnsearchableExportLocation : this.ExportContext.TargetLocation.ExportLocation, path);
		}

		public string GetItemIdListFilePath(string sourceId, bool isUnsearchable)
		{
			if (isUnsearchable)
			{
				return Path.Combine(this.ExportContext.TargetLocation.WorkingLocation, Uri.EscapeDataString(sourceId) + this.ExportSettings.ExportTime.Ticks + ".unsearchable.itemlist");
			}
			return Path.Combine(this.ExportContext.TargetLocation.WorkingLocation, Uri.EscapeDataString(sourceId) + this.ExportSettings.ExportTime.Ticks + ".itemlist");
		}

		public void Rollback(SourceInformationCollection allSourceInformation)
		{
			Tracer.TraceInformation("PstTarget.Rollback - Start", new object[0]);
			Tracer.TraceInformation("PstTarget.Rollback IncludeDuplicates {0}, IncludeSearchableItems {1}, IncludeUnsearchableItems {2}", new object[]
			{
				this.ExportContext.ExportMetadata.IncludeDuplicates ? "true" : "false",
				this.ExportContext.ExportMetadata.IncludeSearchableItems ? "true" : "false",
				this.ExportContext.ExportMetadata.IncludeUnsearchableItems ? "true" : "false"
			});
			foreach (SourceInformation sourceInformation in allSourceInformation.Values)
			{
				Tracer.TraceInformation("PstTarget.Rollback source Id {0}, source Name {1}", new object[]
				{
					sourceInformation.Configuration.Id,
					sourceInformation.Configuration.Name
				});
				bool isPublicFolder = sourceInformation.Configuration.Id.StartsWith("\\");
				if (this.ExportContext.ExportMetadata.IncludeDuplicates)
				{
					if (this.ExportContext.ExportMetadata.IncludeSearchableItems)
					{
						this.RemoveFileForSource(sourceInformation.Configuration.Name, false, isPublicFolder);
					}
					if (this.ExportContext.ExportMetadata.IncludeUnsearchableItems)
					{
						this.RemoveFileForSource(sourceInformation.Configuration.Name, true, isPublicFolder);
					}
				}
				if (this.ExportContext.ExportMetadata.IncludeSearchableItems)
				{
					string itemIdListFilePath = this.GetItemIdListFilePath(sourceInformation.Configuration.Id, false);
					LocalFileHelper.RemoveFile(itemIdListFilePath, ExportErrorType.FailedToRollbackResultsInTargetLocation);
				}
				if (this.ExportContext.ExportMetadata.IncludeUnsearchableItems)
				{
					string itemIdListFilePath2 = this.GetItemIdListFilePath(sourceInformation.Configuration.Id, true);
					LocalFileHelper.RemoveFile(itemIdListFilePath2, ExportErrorType.FailedToRollbackResultsInTargetLocation);
				}
			}
			if (!this.ExportContext.ExportMetadata.IncludeDuplicates)
			{
				if (this.ExportContext.ExportMetadata.IncludeSearchableItems)
				{
					this.RemoveFileForSource(null, false, false);
					this.RemoveFileForSource(null, false, true);
				}
				if (this.ExportContext.ExportMetadata.IncludeUnsearchableItems)
				{
					this.RemoveFileForSource(null, true, false);
					this.RemoveFileForSource(null, true, true);
				}
			}
			Tracer.TraceInformation("PstTarget.Rollback - End", new object[0]);
		}

		public void CheckLocation()
		{
			if (string.IsNullOrEmpty(this.ExportContext.TargetLocation.WorkingLocation))
			{
				throw new ArgumentNullException("WorkingLocation");
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(this.ExportContext.TargetLocation.WorkingLocation);
			if (!directoryInfo.Exists)
			{
				Directory.CreateDirectory(directoryInfo.FullName);
			}
			if (string.IsNullOrEmpty(this.ExportContext.TargetLocation.ExportLocation) && this.ExportContext.ExportMetadata.IncludeSearchableItems)
			{
				throw new ArgumentNullException("ExportLocation");
			}
			if (string.IsNullOrEmpty(this.ExportContext.TargetLocation.UnsearchableExportLocation) && this.ExportContext.ExportMetadata.IncludeUnsearchableItems)
			{
				throw new ArgumentNullException("UnsearchableExportLocation");
			}
		}

		public void CheckInitialStatus(SourceInformationCollection allSourceInformation, OperationStatus status)
		{
			foreach (SourceInformation sourceInformation in allSourceInformation.Values)
			{
				bool isPublicFolder = sourceInformation.Configuration.Id.StartsWith("\\");
				if (this.ExportContext.ExportMetadata.IncludeDuplicates)
				{
					if (this.ExportContext.ExportMetadata.IncludeSearchableItems)
					{
						this.ValidateDataFile(sourceInformation.Configuration.Name, false, isPublicFolder);
					}
					if (this.ExportContext.ExportMetadata.IncludeUnsearchableItems)
					{
						this.ValidateDataFile(sourceInformation.Configuration.Name, true, isPublicFolder);
					}
				}
				if (!this.ExportContext.IsResume || !sourceInformation.Status.IsSearchCompleted(this.ExportContext.ExportMetadata.IncludeSearchableItems, this.ExportContext.ExportMetadata.IncludeUnsearchableItems))
				{
					if (this.ExportContext.ExportMetadata.IncludeSearchableItems)
					{
						string itemIdListFilePath = this.GetItemIdListFilePath(sourceInformation.Configuration.Id, false);
						LocalFileHelper.RemoveFile(itemIdListFilePath, ExportErrorType.FailedToCleanupCorruptedStatusLog);
					}
					if (this.ExportContext.ExportMetadata.IncludeUnsearchableItems)
					{
						string itemIdListFilePath2 = this.GetItemIdListFilePath(sourceInformation.Configuration.Id, true);
						LocalFileHelper.RemoveFile(itemIdListFilePath2, ExportErrorType.FailedToCleanupCorruptedStatusLog);
					}
				}
			}
			if (!this.ExportContext.ExportMetadata.IncludeDuplicates)
			{
				if (this.ExportContext.ExportMetadata.IncludeSearchableItems)
				{
					this.ValidateDataFile(null, false, false);
				}
				if (this.ExportContext.ExportMetadata.IncludeUnsearchableItems)
				{
					this.ValidateDataFile(null, true, false);
				}
			}
		}

		private void ValidateDataFile(string sourceName, bool isUnsearchable, bool isPublicFolder)
		{
			string text = string.Empty;
			if (isPublicFolder)
			{
				text = this.GetPFPstFilePath(isUnsearchable, 1);
			}
			else
			{
				text = this.GetPstFilePath(sourceName, isUnsearchable, 1);
			}
			if (File.Exists(text))
			{
				try
				{
					PstWriter.CreatePstSession(text).Close();
				}
				catch (ExportException ex)
				{
					Tracer.TraceError("PstTarget.ValidateDateFile: Failed to create PST session. Exception: {0}", new object[]
					{
						ex
					});
					throw new ExportException(ExportErrorType.FailedToOpenExistingPstFile, text);
				}
			}
		}

		private void RemoveFileForSource(string source, bool isUnsearchable, bool isPublicFolder)
		{
			Tracer.TraceInformation("PstTarget.RemoveFileForSource ", new object[0]);
			int num = 1000;
			string text = string.Empty;
			bool flag = true;
			int num2 = 1;
			while (flag)
			{
				if (isPublicFolder)
				{
					text = this.GetPFPstFilePath(isUnsearchable, num2++);
				}
				else
				{
					text = this.GetPstFilePath(source, isUnsearchable, num2++);
				}
				if (File.Exists(text))
				{
					try
					{
						LocalFileHelper.RemoveFile(text, ExportErrorType.FailedToRollbackResultsInTargetLocation);
						goto IL_81;
					}
					catch (ExportException ex)
					{
						Tracer.TraceError("PstTarget.RemoveFileForSource: Failed FileName: {0}, Exception: {1}", new object[]
						{
							text,
							ex.ToString()
						});
						goto IL_81;
					}
					goto IL_7F;
				}
				goto IL_7F;
				IL_81:
				if (num2 > num)
				{
					Tracer.TraceError("PstTarget.RemoveFileForSource: Exceeded fileCount limit of {0} files for source {1}.", new object[]
					{
						num2,
						source
					});
					return;
				}
				continue;
				IL_7F:
				flag = false;
				goto IL_81;
			}
		}
	}
}
