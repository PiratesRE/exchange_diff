using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	internal class DiagnosticCommand
	{
		public DiagnosticCommand(DiagnosticType command, Guid entry) : this(command, null, new Guid[]
		{
			entry
		})
		{
		}

		public DiagnosticCommand(DiagnosticType command, Guid orgId, Guid entry) : this(command, new Guid[]
		{
			orgId
		}, new Guid[]
		{
			entry
		})
		{
		}

		public DiagnosticCommand(DiagnosticType command, ICollection<Guid> entries) : this(command, null, entries)
		{
		}

		public DiagnosticCommand(DiagnosticType command, ICollection<Guid> orgIds, ICollection<Guid> entries)
		{
			if (entries == null)
			{
				throw new ArgumentNullException("entries");
			}
			this.Command = command;
			this.Organizations = orgIds;
			this.CacheEntries = entries;
		}

		public DiagnosticType Command { get; private set; }

		public ICollection<Guid> Organizations { get; private set; }

		public ICollection<Guid> CacheEntries { get; private set; }

		public bool IsGlobalCacheEntryCommand
		{
			get
			{
				return this.Organizations == null;
			}
		}

		public static DiagnosticCommand TryFromReceivedData(byte[] buffer, int bufLen, out Exception ex)
		{
			ex = null;
			DiagnosticCommand result = null;
			try
			{
				result = DiagnosticCommand.FromReceivedData(buffer, bufLen);
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			return result;
		}

		public static DiagnosticCommand FromReceivedData(byte[] buffer, int bufLen)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (bufLen <= 0)
			{
				throw new ArgumentException("bufLen is less than zero.");
			}
			if (buffer.Length < bufLen)
			{
				throw new ArgumentException("The buffer is too small.");
			}
			string @string = Encoding.UTF8.GetString(buffer, 0, bufLen);
			string[] array = @string.Split(new char[]
			{
				';'
			});
			if (array == null || array.Length < 3)
			{
				throw new ArgumentException("Received diagnostic command is invalid.");
			}
			if (!array[1].StartsWith("c:", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(string.Format("Invalid ProvisioningCache diagnostic command received, failed to parse the command {0}", array[1]));
			}
			DiagnosticType command;
			if (!Enum.TryParse<DiagnosticType>(array[1].Substring(array[1].IndexOf(':') + 1), true, out command))
			{
				throw new ArgumentException(string.Format("Invalid diagnostic command type received: {0}", array[1]));
			}
			List<Guid> orgIds = null;
			if (!array[2].StartsWith("o:global", StringComparison.OrdinalIgnoreCase))
			{
				if (string.IsNullOrWhiteSpace(array[2].Substring("o:".Length)))
				{
					orgIds = new List<Guid>();
				}
				else
				{
					orgIds = DiagnosticCommand.ParseGuidLists(array[2], "o:", "Org Ids");
				}
			}
			List<Guid> entries;
			if (array.Length > 3 && !string.IsNullOrWhiteSpace(array[3]))
			{
				entries = DiagnosticCommand.ParseGuidLists(array[3], "e:", "Cache Entries");
			}
			else
			{
				entries = new List<Guid>();
			}
			return new DiagnosticCommand(command, orgIds, entries);
		}

		private static List<Guid> ParseGuidLists(string guidList, string prefix, string usageType)
		{
			if (!guidList.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(string.Format("Invalid ProvisioningCache diagnostic command received, failed to parse the {0} : {1}", usageType, guidList));
			}
			string[] array = guidList.Substring(guidList.IndexOf(':') + 1).Split(new char[]
			{
				','
			});
			if (array == null || array.Length == 0)
			{
				throw new ArgumentException(string.Format("No {0} received in ProvisioningCache diagnostic command, invalid string: {1}", usageType, guidList));
			}
			List<Guid> list = new List<Guid>();
			foreach (string input in array)
			{
				Guid item;
				if (!Guid.TryParse(input, out item))
				{
					throw new ArgumentException(string.Format("Invalid {0} received in ProvisioningCache diagnostic command: {1}", usageType, guidList));
				}
				list.Add(item);
			}
			return list;
		}

		public byte[] ToSendMessage()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("v:").Append(DiagnosticCommand.Version).Append(';');
			stringBuilder.Append("c:").Append(this.Command.ToString()).Append(';');
			if (this.Organizations == null)
			{
				stringBuilder.Append("o:global").Append(';');
			}
			else if (this.Organizations.Count > 0)
			{
				DiagnosticCommand.AppendGuidList(stringBuilder, "o:", this.Organizations);
			}
			else
			{
				stringBuilder.Append("o:").Append(';');
			}
			if (this.CacheEntries.Count > 0)
			{
				DiagnosticCommand.AppendGuidList(stringBuilder, "e:", this.CacheEntries);
			}
			string s = stringBuilder.ToString().TrimEnd(new char[]
			{
				';'
			});
			return Encoding.UTF8.GetBytes(s);
		}

		private static void AppendGuidList(StringBuilder sb, string prefix, ICollection<Guid> guids)
		{
			sb.Append(prefix);
			foreach (Guid guid in guids)
			{
				sb.Append(guid.ToString()).Append(',');
			}
			sb.Replace(',', ';', sb.Length - 1, 1);
		}

		private const char Colon = ':';

		private const char Comma = ',';

		private const char Separator = ';';

		private const string VersionPrefix = "v:";

		private const string CommandPrefix = "c:";

		private const string OrganizationPrefix = "o:";

		private const string GlobalCachePrefix = "o:global";

		private const string CacheEntryPrefix = "e:";

		public static readonly int Version = 1;
	}
}
