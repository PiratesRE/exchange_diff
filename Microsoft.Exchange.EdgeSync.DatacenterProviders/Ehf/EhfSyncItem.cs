using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Text;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Datacenter;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfSyncItem
	{
		protected EhfSyncItem(ExSearchResultEntry entry, EdgeSyncDiag diagSession)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			if (diagSession == null)
			{
				throw new ArgumentNullException("diagSession");
			}
			this.entry = entry;
			this.diagSession = diagSession;
		}

		public ExSearchResultEntry ADEntry
		{
			get
			{
				return this.entry;
			}
		}

		public string DistinguishedName
		{
			get
			{
				return this.entry.DistinguishedName;
			}
		}

		public bool IsDeleted
		{
			get
			{
				return this.entry.IsDeleted;
			}
		}

		protected EdgeSyncDiag DiagSession
		{
			get
			{
				return this.diagSession;
			}
		}

		private bool ContainsCurrentSyncErrors
		{
			get
			{
				return this.syncErrors != null && this.syncErrors.Count > 0;
			}
		}

		private bool ContainsPreviousSyncErrors
		{
			get
			{
				return this.entry.GetAttribute("msExchEdgeSyncCookies") != null;
			}
		}

		public void AddSyncError(string errorMessage)
		{
			if (this.syncErrors == null)
			{
				this.syncErrors = new List<string>();
			}
			this.syncErrors.Add(errorMessage);
		}

		public void AddSyncError(string messageFormat, params object[] args)
		{
			string diagString = EdgeSyncDiag.GetDiagString(messageFormat, args);
			this.AddSyncError(diagString);
		}

		public bool EventLogAndTryStoreSyncErrors(EhfADAdapter adapter)
		{
			this.EventLogSyncErrors();
			return this.entry.IsDeleted || this.TryStoreSyncErrors(adapter);
		}

		protected static int GetFlagsValue(string flagsAttrName, ExSearchResultEntry resultEntry, EhfSyncItem syncItem)
		{
			DirectoryAttribute attribute = resultEntry.GetAttribute(flagsAttrName);
			if (attribute == null)
			{
				return 0;
			}
			string text = (string)attribute[0];
			if (string.IsNullOrEmpty(text))
			{
				return 0;
			}
			int result;
			if (!int.TryParse(text, out result))
			{
				syncItem.AddSyncError(syncItem.DiagSession.LogAndTraceError("Unable to parse flags value ({0}) of attribute {1} for AD object ({2}); using default value 0", new object[]
				{
					text,
					flagsAttrName,
					resultEntry.DistinguishedName
				}));
			}
			return result;
		}

		protected Guid GetObjectGuid()
		{
			return this.entry.GetObjectGuid();
		}

		protected int GetFlagsValue(string flagsAttrName)
		{
			return EhfSyncItem.GetFlagsValue(flagsAttrName, this.entry, this);
		}

		private void EventLogSyncErrors()
		{
			if (!this.ContainsCurrentSyncErrors)
			{
				return;
			}
			string text;
			if (this.syncErrors.Count == 1)
			{
				text = this.syncErrors[0];
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string value in this.syncErrors)
				{
					stringBuilder.AppendLine(value);
				}
				text = stringBuilder.ToString();
			}
			this.DiagSession.EventLog.LogEvent(EdgeSyncEventLogConstants.Tuple_EhfEntrySyncFailure, null, new object[]
			{
				this.DistinguishedName,
				text
			});
		}

		private bool TryStoreSyncErrors(EhfADAdapter adapter)
		{
			object[] array = null;
			string text = null;
			if (this.ContainsCurrentSyncErrors)
			{
				array = new object[this.syncErrors.Count];
				for (int i = 0; i < this.syncErrors.Count; i++)
				{
					array[i] = this.syncErrors[i];
				}
				text = "save";
			}
			else if (this.ContainsPreviousSyncErrors)
			{
				text = "remove";
			}
			if (text == null)
			{
				return true;
			}
			Guid objectGuid = this.GetObjectGuid();
			try
			{
				adapter.SetAttributeValues(objectGuid, "msExchEdgeSyncCookies", array);
			}
			catch (ExDirectoryException ex)
			{
				if (ex.ResultCode == ResultCode.NoSuchObject)
				{
					this.DiagSession.LogAndTraceException(ex, "NoSuchObject error occurred while trying to {0} sync errors for AD object <{1}>:<{2}>; ignoring the error", new object[]
					{
						text,
						this.DistinguishedName,
						objectGuid
					});
					return true;
				}
				this.DiagSession.LogAndTraceException(ex, "Exception occurred while trying to {0} sync errors for AD object <{1}>:<{2}>", new object[]
				{
					text,
					this.DistinguishedName,
					objectGuid
				});
				this.DiagSession.EventLog.LogEvent(EdgeSyncEventLogConstants.Tuple_EhfFailedUpdateSyncErrors, this.DistinguishedName, new object[]
				{
					this.DistinguishedName,
					text,
					ex
				});
				return false;
			}
			this.DiagSession.Tracer.TraceDebug<string, string, Guid>((long)this.DiagSession.GetHashCode(), "Successfully {0}d sync errors for AD object <{1}>:<{2}>", text, this.DistinguishedName, objectGuid);
			return true;
		}

		private ExSearchResultEntry entry;

		private EdgeSyncDiag diagSession;

		private List<string> syncErrors;
	}
}
