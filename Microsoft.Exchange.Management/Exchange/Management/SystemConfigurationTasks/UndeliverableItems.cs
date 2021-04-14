using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class UndeliverableItems
	{
		public UndeliverableItems(string undeliverableFolderPath, string replayFolderPath, Task.TaskWarningLoggingDelegate warningWriter, Task.TaskErrorLoggingDelegate errorWriter)
		{
			this.undeliverableFolderPath = undeliverableFolderPath;
			this.replayFolderPath = replayFolderPath;
			this.warningWriter = warningWriter;
			this.errorWriter = errorWriter;
		}

		public string UndeliverableFolderPath
		{
			get
			{
				return this.undeliverableFolderPath;
			}
		}

		public string ReplayFolderPath
		{
			get
			{
				return this.replayFolderPath;
			}
		}

		public List<MalwareFilterRecoveryItem> GetAllItems()
		{
			List<MalwareFilterRecoveryItem> list = new List<MalwareFilterRecoveryItem>();
			string[] files = Directory.GetFiles(this.undeliverableFolderPath, "*.frf");
			foreach (string fileName in files)
			{
				FileInfo fileInfo = new FileInfo(fileName);
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
				list.Add(new MalwareFilterRecoveryItem(fileNameWithoutExtension, fileInfo.LastWriteTimeUtc));
			}
			return list;
		}

		public MalwareFilterRecoveryItem FindItem(string identity)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentException("FindItem does not accept null or empty string as a parameter.");
			}
			string text = Path.Combine(this.undeliverableFolderPath, identity) + ".frf";
			if (File.Exists(text))
			{
				FileInfo fileInfo = new FileInfo(text);
				return new MalwareFilterRecoveryItem(identity, fileInfo.LastWriteTimeUtc);
			}
			this.errorWriter(new ManagementObjectNotFoundException(Strings.ErrorRecoveryItemNotFoundByIdentity(identity)), ErrorCategory.ObjectNotFound, identity);
			return null;
		}

		public void RemoveItem(string identity)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentException("RemoveItem does not accept null or empty string as a parameter.");
			}
			string path = Path.Combine(this.undeliverableFolderPath, identity) + ".frf";
			if (!File.Exists(path))
			{
				this.errorWriter(new ManagementObjectNotFoundException(Strings.ErrorRecoveryItemNotFoundByIdentity(identity)), ErrorCategory.ObjectNotFound, identity);
				return;
			}
			File.Delete(path);
		}

		public void ReplayItem(string identity, bool remove)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("identity");
			}
			string path = Path.Combine(this.undeliverableFolderPath, identity) + ".frf";
			if (!File.Exists(path))
			{
				this.errorWriter(new ManagementObjectNotFoundException(Strings.ErrorRecoveryItemNotFoundByIdentity(identity)), ErrorCategory.ObjectNotFound, identity);
				return;
			}
			byte[] array = File.ReadAllBytes(path);
			array = UndeliverableItems.Decode(array);
			path = Path.Combine(this.replayFolderPath, identity) + ".frf";
			path = Path.ChangeExtension(path, ".eml");
			if (File.Exists(path))
			{
				this.warningWriter(Strings.WarningMessageExistsInReplayQueue(identity));
				return;
			}
			try
			{
				using (Stream stream = File.Open(path, FileMode.CreateNew))
				{
					stream.Write(array, 0, array.Length);
				}
				if (remove)
				{
					this.RemoveItem(identity);
				}
			}
			catch (Exception exception)
			{
				this.errorWriter(exception, ErrorCategory.InvalidOperation, null);
			}
		}

		private static byte[] Decode(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			byte[] array = new byte[buffer.Length];
			for (int i = 0; i < buffer.Length; i++)
			{
				byte[] array2 = array;
				int num = i;
				int num2 = i;
				array2[num] = (buffer[num2] ^= UndeliverableItems.obfuscateValue);
			}
			return array;
		}

		private readonly string undeliverableFolderPath;

		private readonly string replayFolderPath;

		private Task.TaskWarningLoggingDelegate warningWriter;

		private Task.TaskErrorLoggingDelegate errorWriter;

		private static byte obfuscateValue = byte.MaxValue;
	}
}
