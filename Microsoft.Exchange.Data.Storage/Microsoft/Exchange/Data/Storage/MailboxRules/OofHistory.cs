using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OofHistory : DisposeTrackableBase
	{
		public bool Testing
		{
			get
			{
				return this.testing;
			}
			set
			{
				this.testing = value;
			}
		}

		public OofHistory(byte[] senderAddress, byte[] globalRuleId, IRuleEvaluationContext context)
		{
			this.senderAddress = senderAddress;
			this.globalRuleId = globalRuleId;
			this.context = context;
		}

		public bool TryInitialize()
		{
			if (!this.TryOpenHistoryFolder())
			{
				return false;
			}
			this.OpenHistoryStream();
			return true;
		}

		public void TestInitialize(Stream stream)
		{
			this.oofHistoryStream = stream;
			this.reader = new OofHistoryReader();
			this.reader.Initialize(this.oofHistoryStream);
		}

		public bool ShouldSendOofReply()
		{
			if (this.isNew)
			{
				return true;
			}
			try
			{
				this.reader = new OofHistoryReader();
				this.reader.Initialize(this.oofHistoryStream);
				while (this.reader.HasMoreEntries)
				{
					this.reader.ReadEntry();
					if (this.MatchEntryProperties())
					{
						return false;
					}
				}
			}
			catch (OofHistoryCorruptionException ex)
			{
				this.Trace<string>("OOF history data corruption detected: {0}", ex.Message);
				if (this.context != null && !this.testing)
				{
					this.context.LogEvent(this.context.OofHistoryCorruption, ex.GetType().ToString(), new object[]
					{
						((MailboxSession)this.context.StoreSession).MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString()
					});
				}
				this.CloseHistoryStream();
				this.oofHistoryFolder.DeleteProps(new PropTag[]
				{
					PropTag.OofHistory
				});
				this.OpenHistoryStream();
				this.reader = new OofHistoryReader();
				this.reader.Initialize(this.oofHistoryStream);
			}
			return true;
		}

		public void AppendEntry()
		{
			int num = (this.isNew || this.reader == null) ? 0 : this.reader.EntryCount;
			if (num >= 10000)
			{
				this.Trace<int>("History data reached the limit of {0} entries, append aborted.", 10000);
				return;
			}
			int num2 = this.senderAddress.Length;
			if (num2 > 1000)
			{
				this.Trace<int, ushort>("Sender address size {0} is over the limit of {1} bytes, append aborted.", num2, 1000);
				return;
			}
			int num3 = this.globalRuleId.Length;
			if (num3 > 1000)
			{
				this.Trace<int, ushort>("Global rule id size is over the limit of {0} bytes, append aborted.", num3, 1000);
				return;
			}
			OofHistoryWriter oofHistoryWriter = new OofHistoryWriter();
			oofHistoryWriter.Initialize(this.oofHistoryStream);
			oofHistoryWriter.AppendEntry(num + 1, this.senderAddress, this.globalRuleId);
			this.isNew = false;
		}

		public void DumpHistory(TextWriter writer)
		{
			this.reader = new OofHistoryReader();
			this.reader.Initialize(this.oofHistoryStream);
			while (this.reader.HasMoreEntries)
			{
				this.reader.ReadEntry();
				IList<byte> currentEntryRuleIdBytes = this.reader.CurrentEntryRuleIdBytes;
				byte[] array = new byte[currentEntryRuleIdBytes.Count];
				currentEntryRuleIdBytes.CopyTo(array, 0);
				string arg = BitConverter.ToString(array);
				IList<byte> currentEntryAddressBytes = this.reader.CurrentEntryAddressBytes;
				byte[] array2 = new byte[currentEntryAddressBytes.Count];
				currentEntryAddressBytes.CopyTo(array2, 0);
				string @string = Encoding.ASCII.GetString(array2);
				writer.WriteLine("{0} - {1}", arg, @string);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OofHistory>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (this.oofHistoryStream != null)
			{
				this.CloseHistoryStream();
			}
			if (this.oofHistoryFolder != null)
			{
				this.oofHistoryFolder.Dispose();
				this.oofHistoryFolder = null;
			}
		}

		private static bool MatchEntryProperty(IList<byte> expectedPropertyData, IList<byte> actualPropertyData)
		{
			if (expectedPropertyData.Count != actualPropertyData.Count)
			{
				return false;
			}
			ushort num = 0;
			while ((int)num < actualPropertyData.Count)
			{
				if (expectedPropertyData[(int)num] != actualPropertyData[(int)num])
				{
					return false;
				}
				num += 1;
			}
			return true;
		}

		private bool MatchEntryProperties()
		{
			return OofHistory.MatchEntryProperty(this.senderAddress, this.reader.CurrentEntryAddressBytes) && OofHistory.MatchEntryProperty(this.globalRuleId, this.reader.CurrentEntryRuleIdBytes);
		}

		private bool TryOpenHistoryFolder()
		{
			bool result;
			try
			{
				MapiStore mapiStore = this.context.StoreSession.Mailbox.MapiStore;
				using (MapiFolder nonIpmSubtreeFolder = mapiStore.GetNonIpmSubtreeFolder())
				{
					this.oofHistoryFolder = nonIpmSubtreeFolder.OpenSubFolderByName("Freebusy Data");
				}
				this.Trace("OOF history folder opened.");
				result = true;
			}
			catch (MapiExceptionNotFound mapiExceptionNotFound)
			{
				this.context.TraceError<string>("Unable to open the OOF history folder {0}, OOF history operation skipped.", "Freebusy Data");
				this.context.LogEvent(this.context.OofHistoryFolderMissing, mapiExceptionNotFound.GetType().ToString(), new object[]
				{
					((MailboxSession)this.context.StoreSession).MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString()
				});
				result = false;
			}
			return result;
		}

		private void OpenHistoryStream()
		{
			if (this.oofHistoryFolder == null)
			{
				throw new InvalidOperationException("OOF history folder must be opened first.");
			}
			try
			{
				OpenPropertyFlags flags = OpenPropertyFlags.Modify | OpenPropertyFlags.DeferredErrors;
				this.oofHistoryStream = this.oofHistoryFolder.OpenStream(PropTag.OofHistory, flags);
				this.Trace("OOF history property opened.");
				this.LockStreamWithRetry();
			}
			catch (MapiExceptionNotFound)
			{
				this.Trace("OOF history property does not exist, creating new OOF history stream.");
				if (this.oofHistoryStream != null)
				{
					this.oofHistoryStream.Dispose();
					this.oofHistoryStream = null;
				}
				OpenPropertyFlags flags2 = OpenPropertyFlags.Create | OpenPropertyFlags.Modify | OpenPropertyFlags.DeferredErrors;
				this.oofHistoryStream = this.oofHistoryFolder.OpenStream(PropTag.OofHistory, flags2);
				this.LockStreamWithRetry();
				this.PrepareNewOofHistoryStream();
			}
		}

		private void CloseHistoryStream()
		{
			try
			{
				if (this.streamLocked)
				{
					MapiStream mapiStream = this.oofHistoryStream as MapiStream;
					mapiStream.UnlockRegion(0L, 1L, 1);
				}
			}
			finally
			{
				this.oofHistoryStream.Dispose();
				this.oofHistoryStream = null;
			}
		}

		private void PrepareNewOofHistoryStream()
		{
			this.Trace("Resetting / Initializing OOF history stream.");
			OofHistoryWriter.Reset(this.oofHistoryStream);
			this.oofHistoryStream.Position = 0L;
			this.isNew = true;
		}

		private void LockStreamWithRetry()
		{
			MapiStream mapiStream = this.oofHistoryStream as MapiStream;
			if (mapiStream == null)
			{
				return;
			}
			for (int i = 0; i < 6; i++)
			{
				try
				{
					mapiStream.LockRegion(0L, 1L, 1);
					this.streamLocked = true;
					this.Trace("Oof history stream locked.");
					break;
				}
				catch (MapiExceptionLockViolation argument)
				{
					if (i == 5)
					{
						throw;
					}
					this.context.TraceError<int, MapiExceptionLockViolation>("Failed to lock on attempt {0}. Exception encountered is {1}.", i, argument);
					Thread.Sleep(100);
				}
			}
		}

		private void Trace(string message)
		{
			if (this.context != null)
			{
				this.context.TraceDebug(message);
			}
		}

		private void Trace<T>(string format, T argument)
		{
			if (this.context != null)
			{
				this.context.TraceDebug<T>(format, argument);
			}
		}

		private void Trace<T1, T2>(string format, T1 argument1, T2 argument2)
		{
			if (this.context != null)
			{
				this.context.TraceDebug<T1, T2>(format, argument1, argument2);
			}
		}

		public const ushort MaxPropertyValueLength = 1000;

		internal const int HistoryEntryLimit = 10000;

		internal const int LockRetrySleepMilliSeconds = 100;

		internal const string OofHistoryFolderName = "Freebusy Data";

		private const int LockRetryLimit = 6;

		private IRuleEvaluationContext context;

		private byte[] senderAddress;

		private byte[] globalRuleId;

		private bool isNew;

		private OofHistoryReader reader;

		private MapiFolder oofHistoryFolder;

		private Stream oofHistoryStream;

		private bool streamLocked;

		private bool testing;

		public enum PropId : byte
		{
			SenderAddress = 1,
			GlobalRuleId
		}
	}
}
