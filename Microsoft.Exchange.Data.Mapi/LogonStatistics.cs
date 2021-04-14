using System;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public sealed class LogonStatistics : LogonStatisticsEntry
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return LogonStatistics.schema;
			}
		}

		public uint? AdapterSpeed
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.AdapterSpeed];
			}
		}

		public string ClientIPAddress
		{
			get
			{
				return (string)this[LogonStatisticsSchema.ClientIPAddress];
			}
		}

		public ClientMode ClientMode
		{
			get
			{
				return (ClientMode)this[LogonStatisticsSchema.ClientMode];
			}
		}

		public string ClientName
		{
			get
			{
				return (string)this[LogonStatisticsSchema.ClientName];
			}
		}

		public string ClientVersion
		{
			get
			{
				return (string)this[LogonStatisticsSchema.ClientVersion];
			}
		}

		public uint? CodePage
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.CodePage];
			}
		}

		public uint? CurrentOpenAttachments
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.CurrentOpenAttachments];
			}
		}

		public uint? CurrentOpenFolders
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.CurrentOpenFolders];
			}
		}

		public uint? CurrentOpenMessages
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.CurrentOpenMessages];
			}
		}

		public uint? FolderOperationCount
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.FolderOperationCount];
			}
		}

		public string FullMailboxDirectoryName
		{
			get
			{
				return (string)this[LogonStatisticsSchema.FullMailboxDirectoryName];
			}
		}

		public string FullUserDirectoryName
		{
			get
			{
				return (string)this[LogonStatisticsSchema.FullUserDirectoryName];
			}
		}

		public string HostAddress
		{
			get
			{
				return (string)this[LogonStatisticsSchema.HostAddress];
			}
		}

		public DateTime? LastAccessTime
		{
			get
			{
				return (DateTime?)this[LogonStatisticsSchema.LastAccessTime];
			}
		}

		public uint? Latency
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.Latency];
			}
		}

		public uint? LocaleID
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.LocaleID];
			}
		}

		public DateTime? LogonTime
		{
			get
			{
				return (DateTime?)this[LogonStatisticsSchema.LogonTime];
			}
		}

		public string MACAddress
		{
			get
			{
				return (string)this[LogonStatisticsSchema.MACAddress];
			}
		}

		public uint? MessagingOperationCount
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.MessagingOperationCount];
			}
		}

		public uint? OtherOperationCount
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.OtherOperationCount];
			}
		}

		public uint? ProgressOperationCount
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.ProgressOperationCount];
			}
		}

		public uint? RPCCallsSucceeded
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.RPCCallsSucceeded];
			}
		}

		public uint? StreamOperationCount
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.StreamOperationCount];
			}
		}

		public uint? TableOperationCount
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.TableOperationCount];
			}
		}

		public uint? TotalOperationCount
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.TotalOperationCount];
			}
		}

		public uint? TransferOperationCount
		{
			get
			{
				return (uint?)this[LogonStatisticsSchema.TransferOperationCount];
			}
		}

		public string UserName
		{
			get
			{
				return (string)this[LogonStatisticsSchema.UserName];
			}
		}

		public string Windows2000Account
		{
			get
			{
				return (string)this[LogonStatisticsSchema.Windows2000Account];
			}
		}

		public string ApplicationId
		{
			get
			{
				return (string)this[LogonStatisticsSchema.ApplicationId];
			}
		}

		public string SessionId
		{
			get
			{
				long? num = (long?)this[LogonStatisticsSchema.SessionId];
				if (num != null)
				{
					return num.Value.ToString("x");
				}
				return string.Empty;
			}
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
			internal set
			{
				this.serverName = value;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
			internal set
			{
				this.databaseName = value;
			}
		}

		private string serverName;

		private string databaseName;

		private static MapiObjectSchema schema = ObjectSchema.GetInstance<LogonStatisticsSchema>();
	}
}
