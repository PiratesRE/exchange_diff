using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	internal sealed class PhysicalAccessPerformanceCountersInstance : PerformanceCounterInstance
	{
		internal PhysicalAccessPerformanceCountersInstance(string instanceName, PhysicalAccessPerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeIS Physical Access")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NumberOfQueriesPerSec = new ExPerformanceCounter(base.CategoryName, "Number of Queries per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfQueriesPerSec, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfQueriesPerSec);
				this.NumberOfInsertsPerSec = new ExPerformanceCounter(base.CategoryName, "Number of Inserts per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfInsertsPerSec, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfInsertsPerSec);
				this.NumberOfUpdatesPerSec = new ExPerformanceCounter(base.CategoryName, "Number of Updates per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfUpdatesPerSec, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfUpdatesPerSec);
				this.NumberOfDeletesPerSec = new ExPerformanceCounter(base.CategoryName, "Number of Deletes per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfDeletesPerSec, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDeletesPerSec);
				this.NumberOfOthersPerSec = new ExPerformanceCounter(base.CategoryName, "Number of Others per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfOthersPerSec, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOthersPerSec);
				this.OffPageBlobHitsPerSec = new ExPerformanceCounter(base.CategoryName, "Number of off-page blob hits per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.OffPageBlobHitsPerSec, new ExPerformanceCounter[0]);
				list.Add(this.OffPageBlobHitsPerSec);
				this.OtherRowsReadRate = new ExPerformanceCounter(base.CategoryName, "Other - Rows read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.OtherRowsReadRate, new ExPerformanceCounter[0]);
				list.Add(this.OtherRowsReadRate);
				this.OtherRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Other - Seeks per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.OtherRowsSeekRate, new ExPerformanceCounter[0]);
				list.Add(this.OtherRowsSeekRate);
				this.OtherRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Other - Rows accepted per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.OtherRowsAcceptRate, new ExPerformanceCounter[0]);
				list.Add(this.OtherRowsAcceptRate);
				this.OtherRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Other - Rows written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.OtherRowsWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.OtherRowsWriteRate);
				this.OtherBytesReadRate = new ExPerformanceCounter(base.CategoryName, "Other - Bytes read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.OtherBytesReadRate, new ExPerformanceCounter[0]);
				list.Add(this.OtherBytesReadRate);
				this.OtherBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Other - Bytes written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.OtherBytesWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.OtherBytesWriteRate);
				this.TableFunctionRowsReadRate = new ExPerformanceCounter(base.CategoryName, "TableFunction - Rows read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TableFunctionRowsReadRate, new ExPerformanceCounter[0]);
				list.Add(this.TableFunctionRowsReadRate);
				this.TableFunctionRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "TableFunction - Rows accepted per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TableFunctionRowsAcceptRate, new ExPerformanceCounter[0]);
				list.Add(this.TableFunctionRowsAcceptRate);
				this.TableFunctionBytesReadRate = new ExPerformanceCounter(base.CategoryName, "TableFunction - Bytes read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TableFunctionBytesReadRate, new ExPerformanceCounter[0]);
				list.Add(this.TableFunctionBytesReadRate);
				this.TempRowsReadRate = new ExPerformanceCounter(base.CategoryName, "Temp - Rows read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TempRowsReadRate, new ExPerformanceCounter[0]);
				list.Add(this.TempRowsReadRate);
				this.TempRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Temp - Seeks per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TempRowsSeekRate, new ExPerformanceCounter[0]);
				list.Add(this.TempRowsSeekRate);
				this.TempRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Temp - Rows accepted per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TempRowsAcceptRate, new ExPerformanceCounter[0]);
				list.Add(this.TempRowsAcceptRate);
				this.TempRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Temp - Rows written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TempRowsWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.TempRowsWriteRate);
				this.TempBytesReadRate = new ExPerformanceCounter(base.CategoryName, "Temp - Bytes read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TempBytesReadRate, new ExPerformanceCounter[0]);
				list.Add(this.TempBytesReadRate);
				this.TempBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Temp - Bytes written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TempBytesWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.TempBytesWriteRate);
				this.LazyIndexRowsReadRate = new ExPerformanceCounter(base.CategoryName, "LazyIndex - Rows read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LazyIndexRowsReadRate, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexRowsReadRate);
				this.LazyIndexRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "LazyIndex - Seeks per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LazyIndexRowsSeekRate, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexRowsSeekRate);
				this.LazyIndexRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "LazyIndex - Rows accepted per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LazyIndexRowsAcceptRate, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexRowsAcceptRate);
				this.LazyIndexRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "LazyIndex - Rows written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LazyIndexRowsWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexRowsWriteRate);
				this.LazyIndexBytesReadRate = new ExPerformanceCounter(base.CategoryName, "LazyIndex - Bytes read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LazyIndexBytesReadRate, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexBytesReadRate);
				this.LazyIndexBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "LazyIndex - Bytes written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LazyIndexBytesWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexBytesWriteRate);
				this.FolderRowsReadRate = new ExPerformanceCounter(base.CategoryName, "Folder - Rows read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.FolderRowsReadRate, new ExPerformanceCounter[0]);
				list.Add(this.FolderRowsReadRate);
				this.FolderRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Folder - Seeks per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.FolderRowsSeekRate, new ExPerformanceCounter[0]);
				list.Add(this.FolderRowsSeekRate);
				this.FolderRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Folder - Rows accepted per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.FolderRowsAcceptRate, new ExPerformanceCounter[0]);
				list.Add(this.FolderRowsAcceptRate);
				this.FolderRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Folder - Rows written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.FolderRowsWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.FolderRowsWriteRate);
				this.FolderBytesReadRate = new ExPerformanceCounter(base.CategoryName, "Folder - Bytes read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.FolderBytesReadRate, new ExPerformanceCounter[0]);
				list.Add(this.FolderBytesReadRate);
				this.FolderBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Folder - Bytes written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.FolderBytesWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.FolderBytesWriteRate);
				this.MessageRowsReadRate = new ExPerformanceCounter(base.CategoryName, "Message - Rows read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessageRowsReadRate, new ExPerformanceCounter[0]);
				list.Add(this.MessageRowsReadRate);
				this.MessageRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Message - Seeks per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessageRowsSeekRate, new ExPerformanceCounter[0]);
				list.Add(this.MessageRowsSeekRate);
				this.MessageRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Message - Rows accepted per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessageRowsAcceptRate, new ExPerformanceCounter[0]);
				list.Add(this.MessageRowsAcceptRate);
				this.MessageRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Message - Rows written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessageRowsWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.MessageRowsWriteRate);
				this.MessageBytesReadRate = new ExPerformanceCounter(base.CategoryName, "Message - Bytes read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessageBytesReadRate, new ExPerformanceCounter[0]);
				list.Add(this.MessageBytesReadRate);
				this.MessageBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Message - Bytes written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessageBytesWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.MessageBytesWriteRate);
				this.AttachmentRowsReadRate = new ExPerformanceCounter(base.CategoryName, "Attachment - Rows read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AttachmentRowsReadRate, new ExPerformanceCounter[0]);
				list.Add(this.AttachmentRowsReadRate);
				this.AttachmentRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Attachment - Seeks per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AttachmentRowsSeekRate, new ExPerformanceCounter[0]);
				list.Add(this.AttachmentRowsSeekRate);
				this.AttachmentRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Attachment - Rows accepted per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AttachmentRowsAcceptRate, new ExPerformanceCounter[0]);
				list.Add(this.AttachmentRowsAcceptRate);
				this.AttachmentRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Attachment - Rows written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AttachmentRowsWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.AttachmentRowsWriteRate);
				this.AttachmentBytesReadRate = new ExPerformanceCounter(base.CategoryName, "Attachment - Bytes read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AttachmentBytesReadRate, new ExPerformanceCounter[0]);
				list.Add(this.AttachmentBytesReadRate);
				this.AttachmentBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Attachment - Bytes written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AttachmentBytesWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.AttachmentBytesWriteRate);
				this.PseudoIndexMaintenanceRowsReadRate = new ExPerformanceCounter(base.CategoryName, "PseudoIndexMaintenance - Rows read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PseudoIndexMaintenanceRowsReadRate, new ExPerformanceCounter[0]);
				list.Add(this.PseudoIndexMaintenanceRowsReadRate);
				this.PseudoIndexMaintenanceRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "PseudoIndexMaintenance - Seeks per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PseudoIndexMaintenanceRowsSeekRate, new ExPerformanceCounter[0]);
				list.Add(this.PseudoIndexMaintenanceRowsSeekRate);
				this.PseudoIndexMaintenanceRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "PseudoIndexMaintenance - Rows accepted per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PseudoIndexMaintenanceRowsAcceptRate, new ExPerformanceCounter[0]);
				list.Add(this.PseudoIndexMaintenanceRowsAcceptRate);
				this.PseudoIndexMaintenanceRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "PseudoIndexMaintenance - Rows written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PseudoIndexMaintenanceRowsWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.PseudoIndexMaintenanceRowsWriteRate);
				this.PseudoIndexMaintenanceBytesReadRate = new ExPerformanceCounter(base.CategoryName, "PseudoIndexMaintenance - Bytes read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PseudoIndexMaintenanceBytesReadRate, new ExPerformanceCounter[0]);
				list.Add(this.PseudoIndexMaintenanceBytesReadRate);
				this.PseudoIndexMaintenanceBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "PseudoIndexMaintenance - Bytes written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PseudoIndexMaintenanceBytesWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.PseudoIndexMaintenanceBytesWriteRate);
				this.EventsRowsReadRate = new ExPerformanceCounter(base.CategoryName, "Events - Rows read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.EventsRowsReadRate, new ExPerformanceCounter[0]);
				list.Add(this.EventsRowsReadRate);
				this.EventsRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Events - Seeks per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.EventsRowsSeekRate, new ExPerformanceCounter[0]);
				list.Add(this.EventsRowsSeekRate);
				this.EventsRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Events - Rows accepted per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.EventsRowsAcceptRate, new ExPerformanceCounter[0]);
				list.Add(this.EventsRowsAcceptRate);
				this.EventsRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Events - Rows written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.EventsRowsWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.EventsRowsWriteRate);
				this.EventsBytesReadRate = new ExPerformanceCounter(base.CategoryName, "Events - Bytes read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.EventsBytesReadRate, new ExPerformanceCounter[0]);
				list.Add(this.EventsBytesReadRate);
				this.EventsBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Events - Bytes written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.EventsBytesWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.EventsBytesWriteRate);
				this.RowsReadRate = new ExPerformanceCounter(base.CategoryName, "Total - Rows read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RowsReadRate, new ExPerformanceCounter[0]);
				list.Add(this.RowsReadRate);
				this.RowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Total - Seeks per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RowsSeekRate, new ExPerformanceCounter[0]);
				list.Add(this.RowsSeekRate);
				this.RowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Total - Rows accepted per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RowsAcceptRate, new ExPerformanceCounter[0]);
				list.Add(this.RowsAcceptRate);
				this.RowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Total - Rows written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RowsWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.RowsWriteRate);
				this.BytesReadRate = new ExPerformanceCounter(base.CategoryName, "Total - Bytes read per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.BytesReadRate, new ExPerformanceCounter[0]);
				list.Add(this.BytesReadRate);
				this.BytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Total - Bytes written per second", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.BytesWriteRate, new ExPerformanceCounter[0]);
				list.Add(this.BytesWriteRate);
				long num = this.NumberOfQueriesPerSec.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal PhysicalAccessPerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchangeIS Physical Access")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NumberOfQueriesPerSec = new ExPerformanceCounter(base.CategoryName, "Number of Queries per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfQueriesPerSec);
				this.NumberOfInsertsPerSec = new ExPerformanceCounter(base.CategoryName, "Number of Inserts per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfInsertsPerSec);
				this.NumberOfUpdatesPerSec = new ExPerformanceCounter(base.CategoryName, "Number of Updates per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfUpdatesPerSec);
				this.NumberOfDeletesPerSec = new ExPerformanceCounter(base.CategoryName, "Number of Deletes per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDeletesPerSec);
				this.NumberOfOthersPerSec = new ExPerformanceCounter(base.CategoryName, "Number of Others per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOthersPerSec);
				this.OffPageBlobHitsPerSec = new ExPerformanceCounter(base.CategoryName, "Number of off-page blob hits per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OffPageBlobHitsPerSec);
				this.OtherRowsReadRate = new ExPerformanceCounter(base.CategoryName, "Other - Rows read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OtherRowsReadRate);
				this.OtherRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Other - Seeks per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OtherRowsSeekRate);
				this.OtherRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Other - Rows accepted per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OtherRowsAcceptRate);
				this.OtherRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Other - Rows written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OtherRowsWriteRate);
				this.OtherBytesReadRate = new ExPerformanceCounter(base.CategoryName, "Other - Bytes read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OtherBytesReadRate);
				this.OtherBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Other - Bytes written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OtherBytesWriteRate);
				this.TableFunctionRowsReadRate = new ExPerformanceCounter(base.CategoryName, "TableFunction - Rows read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TableFunctionRowsReadRate);
				this.TableFunctionRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "TableFunction - Rows accepted per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TableFunctionRowsAcceptRate);
				this.TableFunctionBytesReadRate = new ExPerformanceCounter(base.CategoryName, "TableFunction - Bytes read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TableFunctionBytesReadRate);
				this.TempRowsReadRate = new ExPerformanceCounter(base.CategoryName, "Temp - Rows read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TempRowsReadRate);
				this.TempRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Temp - Seeks per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TempRowsSeekRate);
				this.TempRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Temp - Rows accepted per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TempRowsAcceptRate);
				this.TempRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Temp - Rows written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TempRowsWriteRate);
				this.TempBytesReadRate = new ExPerformanceCounter(base.CategoryName, "Temp - Bytes read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TempBytesReadRate);
				this.TempBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Temp - Bytes written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TempBytesWriteRate);
				this.LazyIndexRowsReadRate = new ExPerformanceCounter(base.CategoryName, "LazyIndex - Rows read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexRowsReadRate);
				this.LazyIndexRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "LazyIndex - Seeks per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexRowsSeekRate);
				this.LazyIndexRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "LazyIndex - Rows accepted per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexRowsAcceptRate);
				this.LazyIndexRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "LazyIndex - Rows written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexRowsWriteRate);
				this.LazyIndexBytesReadRate = new ExPerformanceCounter(base.CategoryName, "LazyIndex - Bytes read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexBytesReadRate);
				this.LazyIndexBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "LazyIndex - Bytes written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexBytesWriteRate);
				this.FolderRowsReadRate = new ExPerformanceCounter(base.CategoryName, "Folder - Rows read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FolderRowsReadRate);
				this.FolderRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Folder - Seeks per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FolderRowsSeekRate);
				this.FolderRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Folder - Rows accepted per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FolderRowsAcceptRate);
				this.FolderRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Folder - Rows written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FolderRowsWriteRate);
				this.FolderBytesReadRate = new ExPerformanceCounter(base.CategoryName, "Folder - Bytes read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FolderBytesReadRate);
				this.FolderBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Folder - Bytes written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FolderBytesWriteRate);
				this.MessageRowsReadRate = new ExPerformanceCounter(base.CategoryName, "Message - Rows read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MessageRowsReadRate);
				this.MessageRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Message - Seeks per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MessageRowsSeekRate);
				this.MessageRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Message - Rows accepted per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MessageRowsAcceptRate);
				this.MessageRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Message - Rows written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MessageRowsWriteRate);
				this.MessageBytesReadRate = new ExPerformanceCounter(base.CategoryName, "Message - Bytes read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MessageBytesReadRate);
				this.MessageBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Message - Bytes written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MessageBytesWriteRate);
				this.AttachmentRowsReadRate = new ExPerformanceCounter(base.CategoryName, "Attachment - Rows read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AttachmentRowsReadRate);
				this.AttachmentRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Attachment - Seeks per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AttachmentRowsSeekRate);
				this.AttachmentRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Attachment - Rows accepted per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AttachmentRowsAcceptRate);
				this.AttachmentRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Attachment - Rows written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AttachmentRowsWriteRate);
				this.AttachmentBytesReadRate = new ExPerformanceCounter(base.CategoryName, "Attachment - Bytes read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AttachmentBytesReadRate);
				this.AttachmentBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Attachment - Bytes written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AttachmentBytesWriteRate);
				this.PseudoIndexMaintenanceRowsReadRate = new ExPerformanceCounter(base.CategoryName, "PseudoIndexMaintenance - Rows read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PseudoIndexMaintenanceRowsReadRate);
				this.PseudoIndexMaintenanceRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "PseudoIndexMaintenance - Seeks per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PseudoIndexMaintenanceRowsSeekRate);
				this.PseudoIndexMaintenanceRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "PseudoIndexMaintenance - Rows accepted per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PseudoIndexMaintenanceRowsAcceptRate);
				this.PseudoIndexMaintenanceRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "PseudoIndexMaintenance - Rows written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PseudoIndexMaintenanceRowsWriteRate);
				this.PseudoIndexMaintenanceBytesReadRate = new ExPerformanceCounter(base.CategoryName, "PseudoIndexMaintenance - Bytes read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PseudoIndexMaintenanceBytesReadRate);
				this.PseudoIndexMaintenanceBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "PseudoIndexMaintenance - Bytes written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PseudoIndexMaintenanceBytesWriteRate);
				this.EventsRowsReadRate = new ExPerformanceCounter(base.CategoryName, "Events - Rows read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.EventsRowsReadRate);
				this.EventsRowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Events - Seeks per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.EventsRowsSeekRate);
				this.EventsRowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Events - Rows accepted per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.EventsRowsAcceptRate);
				this.EventsRowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Events - Rows written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.EventsRowsWriteRate);
				this.EventsBytesReadRate = new ExPerformanceCounter(base.CategoryName, "Events - Bytes read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.EventsBytesReadRate);
				this.EventsBytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Events - Bytes written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.EventsBytesWriteRate);
				this.RowsReadRate = new ExPerformanceCounter(base.CategoryName, "Total - Rows read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RowsReadRate);
				this.RowsSeekRate = new ExPerformanceCounter(base.CategoryName, "Total - Seeks per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RowsSeekRate);
				this.RowsAcceptRate = new ExPerformanceCounter(base.CategoryName, "Total - Rows accepted per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RowsAcceptRate);
				this.RowsWriteRate = new ExPerformanceCounter(base.CategoryName, "Total - Rows written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RowsWriteRate);
				this.BytesReadRate = new ExPerformanceCounter(base.CategoryName, "Total - Bytes read per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BytesReadRate);
				this.BytesWriteRate = new ExPerformanceCounter(base.CategoryName, "Total - Bytes written per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BytesWriteRate);
				long num = this.NumberOfQueriesPerSec.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter NumberOfQueriesPerSec;

		public readonly ExPerformanceCounter NumberOfInsertsPerSec;

		public readonly ExPerformanceCounter NumberOfUpdatesPerSec;

		public readonly ExPerformanceCounter NumberOfDeletesPerSec;

		public readonly ExPerformanceCounter NumberOfOthersPerSec;

		public readonly ExPerformanceCounter OffPageBlobHitsPerSec;

		public readonly ExPerformanceCounter OtherRowsReadRate;

		public readonly ExPerformanceCounter OtherRowsSeekRate;

		public readonly ExPerformanceCounter OtherRowsAcceptRate;

		public readonly ExPerformanceCounter OtherRowsWriteRate;

		public readonly ExPerformanceCounter OtherBytesReadRate;

		public readonly ExPerformanceCounter OtherBytesWriteRate;

		public readonly ExPerformanceCounter TableFunctionRowsReadRate;

		public readonly ExPerformanceCounter TableFunctionRowsAcceptRate;

		public readonly ExPerformanceCounter TableFunctionBytesReadRate;

		public readonly ExPerformanceCounter TempRowsReadRate;

		public readonly ExPerformanceCounter TempRowsSeekRate;

		public readonly ExPerformanceCounter TempRowsAcceptRate;

		public readonly ExPerformanceCounter TempRowsWriteRate;

		public readonly ExPerformanceCounter TempBytesReadRate;

		public readonly ExPerformanceCounter TempBytesWriteRate;

		public readonly ExPerformanceCounter LazyIndexRowsReadRate;

		public readonly ExPerformanceCounter LazyIndexRowsSeekRate;

		public readonly ExPerformanceCounter LazyIndexRowsAcceptRate;

		public readonly ExPerformanceCounter LazyIndexRowsWriteRate;

		public readonly ExPerformanceCounter LazyIndexBytesReadRate;

		public readonly ExPerformanceCounter LazyIndexBytesWriteRate;

		public readonly ExPerformanceCounter FolderRowsReadRate;

		public readonly ExPerformanceCounter FolderRowsSeekRate;

		public readonly ExPerformanceCounter FolderRowsAcceptRate;

		public readonly ExPerformanceCounter FolderRowsWriteRate;

		public readonly ExPerformanceCounter FolderBytesReadRate;

		public readonly ExPerformanceCounter FolderBytesWriteRate;

		public readonly ExPerformanceCounter MessageRowsReadRate;

		public readonly ExPerformanceCounter MessageRowsSeekRate;

		public readonly ExPerformanceCounter MessageRowsAcceptRate;

		public readonly ExPerformanceCounter MessageRowsWriteRate;

		public readonly ExPerformanceCounter MessageBytesReadRate;

		public readonly ExPerformanceCounter MessageBytesWriteRate;

		public readonly ExPerformanceCounter AttachmentRowsReadRate;

		public readonly ExPerformanceCounter AttachmentRowsSeekRate;

		public readonly ExPerformanceCounter AttachmentRowsAcceptRate;

		public readonly ExPerformanceCounter AttachmentRowsWriteRate;

		public readonly ExPerformanceCounter AttachmentBytesReadRate;

		public readonly ExPerformanceCounter AttachmentBytesWriteRate;

		public readonly ExPerformanceCounter PseudoIndexMaintenanceRowsReadRate;

		public readonly ExPerformanceCounter PseudoIndexMaintenanceRowsSeekRate;

		public readonly ExPerformanceCounter PseudoIndexMaintenanceRowsAcceptRate;

		public readonly ExPerformanceCounter PseudoIndexMaintenanceRowsWriteRate;

		public readonly ExPerformanceCounter PseudoIndexMaintenanceBytesReadRate;

		public readonly ExPerformanceCounter PseudoIndexMaintenanceBytesWriteRate;

		public readonly ExPerformanceCounter EventsRowsReadRate;

		public readonly ExPerformanceCounter EventsRowsSeekRate;

		public readonly ExPerformanceCounter EventsRowsAcceptRate;

		public readonly ExPerformanceCounter EventsRowsWriteRate;

		public readonly ExPerformanceCounter EventsBytesReadRate;

		public readonly ExPerformanceCounter EventsBytesWriteRate;

		public readonly ExPerformanceCounter RowsReadRate;

		public readonly ExPerformanceCounter RowsSeekRate;

		public readonly ExPerformanceCounter RowsAcceptRate;

		public readonly ExPerformanceCounter RowsWriteRate;

		public readonly ExPerformanceCounter BytesReadRate;

		public readonly ExPerformanceCounter BytesWriteRate;
	}
}
