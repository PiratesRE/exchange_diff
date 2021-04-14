using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Common;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class MailboxShape
	{
		public MailboxShape(long messagesPerFolderCountWarningQuota, long messagesPerFolderCountReceiveQuota, long dumpsterMessagesPerFolderCountWarningQuota, long dumpsterMessagesPerFolderCountReceiveQuota, long folderHierarchyChildrenCountWarningQuota, long folderHierarchyChildrenCountReceiveQuota, long folderHierarchyDepthWarningQuota, long folderHierarchyDepthReceiveQuota, long foldersCountWarningQuota, long foldersCountReceiveQuota, long namedPropertiesCountQuota)
		{
			this.messagesPerFolderCountWarningQuota = messagesPerFolderCountWarningQuota;
			this.messagesPerFolderCountReceiveQuota = messagesPerFolderCountReceiveQuota;
			this.dumpsterMessagesPerFolderCountWarningQuota = dumpsterMessagesPerFolderCountWarningQuota;
			this.dumpsterMessagesPerFolderCountReceiveQuota = dumpsterMessagesPerFolderCountReceiveQuota;
			this.folderHierarchyChildrenCountWarningQuota = folderHierarchyChildrenCountWarningQuota;
			this.folderHierarchyChildrenCountReceiveQuota = folderHierarchyChildrenCountReceiveQuota;
			this.folderHierarchyDepthWarningQuota = folderHierarchyDepthWarningQuota;
			this.folderHierarchyDepthReceiveQuota = folderHierarchyDepthReceiveQuota;
			this.foldersCountWarningQuota = foldersCountWarningQuota;
			this.foldersCountReceiveQuota = foldersCountReceiveQuota;
			this.namedPropertiesCountQuota = namedPropertiesCountQuota;
		}

		public long MessagesPerFolderCountWarningQuota
		{
			get
			{
				return this.messagesPerFolderCountWarningQuota;
			}
		}

		public long MessagesPerFolderCountReceiveQuota
		{
			get
			{
				return this.messagesPerFolderCountReceiveQuota;
			}
		}

		public long DumpsterMessagesPerFolderCountWarningQuota
		{
			get
			{
				return this.dumpsterMessagesPerFolderCountWarningQuota;
			}
		}

		public long DumpsterMessagesPerFolderCountReceiveQuota
		{
			get
			{
				return this.dumpsterMessagesPerFolderCountReceiveQuota;
			}
		}

		public long FolderHierarchyChildrenCountWarningQuota
		{
			get
			{
				return this.folderHierarchyChildrenCountWarningQuota;
			}
		}

		public long FolderHierarchyChildrenCountReceiveQuota
		{
			get
			{
				return this.folderHierarchyChildrenCountReceiveQuota;
			}
		}

		public long FolderHierarchyDepthWarningQuota
		{
			get
			{
				return this.folderHierarchyDepthWarningQuota;
			}
		}

		public long FolderHierarchyDepthReceiveQuota
		{
			get
			{
				return this.folderHierarchyDepthReceiveQuota;
			}
		}

		public long FoldersCountWarningQuota
		{
			get
			{
				return this.foldersCountWarningQuota;
			}
		}

		public long FoldersCountReceiveQuota
		{
			get
			{
				return this.foldersCountReceiveQuota;
			}
		}

		public long NamedPropertiesCountQuota
		{
			get
			{
				return this.namedPropertiesCountQuota;
			}
		}

		public static bool TryParse(string input, out Dictionary<Guid, MailboxShape> mailboxShapeConfiguration)
		{
			mailboxShapeConfiguration = new Dictionary<Guid, MailboxShape>();
			if (string.IsNullOrEmpty(input))
			{
				if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ConfigurationTracer.TraceInformation<string>(0, 0L, "No mailbox shape configuration in: {0}", input);
				}
				return true;
			}
			string[] array = input.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 0)
			{
				if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ConfigurationTracer.TraceInformation<string>(0, 0L, "No mailbox shape configuration in: {0}", input);
				}
				return true;
			}
			foreach (string text in array)
			{
				if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ConfigurationTracer.TraceInformation<string>(0, 0L, "Try parse mailbox shape configuration: {0}", text);
				}
				Guid guid;
				MailboxShape value;
				if (!MailboxShape.TryParseSingleInstance(text, out guid, out value))
				{
					if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed to parse mailbox shape configuration: {0}", text);
					}
					mailboxShapeConfiguration = null;
					return false;
				}
				try
				{
					mailboxShapeConfiguration.Add(guid, value);
				}
				catch (ArgumentException exception)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
					if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.ConfigurationTracer.TraceError<Guid>(0L, "Skip overwrite for mailbox {0}", guid);
					}
				}
			}
			return true;
		}

		internal static bool TryParseSingleInstance(string input, out Guid mailboxGuid, out MailboxShape mailboxShape)
		{
			mailboxGuid = Guid.Empty;
			mailboxShape = null;
			string[] array = input.Split(null, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 9 && array.Length != 12)
			{
				if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed parsing mailbox configuration: {0}", input);
				}
				return false;
			}
			if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.ConfigurationTracer.TraceInformation<string>(0, 0L, "Parsing mailbox configuration: {0}", input);
			}
			if (!Guid.TryParse(array[0], out mailboxGuid))
			{
				if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed parsing: {0} as Guid", array[0]);
				}
				return false;
			}
			long num;
			if (!long.TryParse(array[1], out num))
			{
				if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed parsing: {0} as long", array[1]);
				}
				return false;
			}
			long num2;
			if (!long.TryParse(array[2], out num2))
			{
				if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed parsing: {0} as long", array[2]);
				}
				return false;
			}
			long num3;
			if (!long.TryParse(array[3], out num3))
			{
				if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed parsing: {0} as long", array[3]);
				}
				return false;
			}
			long num4;
			if (!long.TryParse(array[4], out num4))
			{
				if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed parsing: {0} as long", array[4]);
				}
				return false;
			}
			long num5;
			if (!long.TryParse(array[5], out num5))
			{
				if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed parsing: {0} as long", array[5]);
				}
				return false;
			}
			long num6;
			if (!long.TryParse(array[6], out num6))
			{
				if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed parsing: {0} as long", array[6]);
				}
				return false;
			}
			long num7;
			if (!long.TryParse(array[7], out num7))
			{
				if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed parsing: {0} as long", array[7]);
				}
				return false;
			}
			long num8;
			if (!long.TryParse(array[8], out num8))
			{
				if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed parsing: {0} as long", array[8]);
				}
				return false;
			}
			long num9;
			long num10;
			long num11;
			if (array.Length > 9)
			{
				if (!long.TryParse(array[9], out num9))
				{
					if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed parsing: {0} as long", array[9]);
					}
					return false;
				}
				if (!long.TryParse(array[10], out num10))
				{
					if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed parsing: {0} as long", array[10]);
					}
					return false;
				}
				if (!long.TryParse(array[11], out num11))
				{
					if (ExTraceGlobals.ConfigurationTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Failed parsing: {0} as long", array[11]);
					}
					return false;
				}
			}
			else
			{
				num9 = (ConfigurationSchema.FoldersCountWarningQuota.Value.IsUnlimited ? long.MaxValue : ConfigurationSchema.FoldersCountWarningQuota.Value.Value);
				num10 = (ConfigurationSchema.FoldersCountReceiveQuota.Value.IsUnlimited ? long.MaxValue : ConfigurationSchema.FoldersCountReceiveQuota.Value.Value);
				num11 = (long)((ulong)ConfigurationSchema.MAPINamedPropsQuota.Value);
			}
			mailboxShape = new MailboxShape(num, num2, num3, num4, num5, num6, num7, num8, num9, num10, num11);
			return true;
		}

		private readonly long messagesPerFolderCountWarningQuota;

		private readonly long messagesPerFolderCountReceiveQuota;

		private readonly long dumpsterMessagesPerFolderCountWarningQuota;

		private readonly long dumpsterMessagesPerFolderCountReceiveQuota;

		private readonly long folderHierarchyChildrenCountWarningQuota;

		private readonly long folderHierarchyChildrenCountReceiveQuota;

		private readonly long folderHierarchyDepthWarningQuota;

		private readonly long folderHierarchyDepthReceiveQuota;

		private readonly long foldersCountWarningQuota;

		private readonly long foldersCountReceiveQuota;

		private readonly long namedPropertiesCountQuota;
	}
}
