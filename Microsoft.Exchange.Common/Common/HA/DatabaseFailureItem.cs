using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Common.HA
{
	internal class DatabaseFailureItem
	{
		internal DatabaseFailureItem()
		{
		}

		internal FailureNameSpace NameSpace
		{
			get
			{
				return this.nameSpace;
			}
			set
			{
				this.nameSpace = value;
			}
		}

		internal FailureTag Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
			}
		}

		internal Guid Guid
		{
			get
			{
				return this.guid;
			}
			set
			{
				this.guid = value;
			}
		}

		internal string InstanceName
		{
			get
			{
				return this.instanceName;
			}
			set
			{
				this.instanceName = value;
			}
		}

		internal string ComponentName
		{
			get
			{
				return this.componentName;
			}
			set
			{
				this.componentName = value;
			}
		}

		internal IoErrorInfo IoError
		{
			get
			{
				return this.ioError;
			}
			set
			{
				this.ioError = value;
			}
		}

		internal NotificationEventInfo NotifyEvent
		{
			get
			{
				return this.notifyEvent;
			}
			set
			{
				this.notifyEvent = value;
			}
		}

		internal DateTime CreationTime
		{
			get
			{
				return this.creationTime;
			}
			set
			{
				this.creationTime = value;
			}
		}

		internal EventBookmark Bookmark
		{
			get
			{
				return this.bookMark;
			}
			set
			{
				this.bookMark = value;
			}
		}

		internal string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		internal DatabaseFailureItem(FailureNameSpace nameSpace, FailureTag tag, Guid guid)
		{
			this.NameSpace = nameSpace;
			this.Tag = tag;
			this.Guid = guid;
		}

		internal DatabaseFailureItem(FailureNameSpace nameSpace, FailureTag tag, Guid guid, string message)
		{
			this.NameSpace = nameSpace;
			this.Tag = tag;
			this.Guid = guid;
			this.Message = message;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (base.GetType() != obj.GetType())
			{
				return false;
			}
			DatabaseFailureItem databaseFailureItem = obj as DatabaseFailureItem;
			return this.NameSpace.Equals(databaseFailureItem.NameSpace) && this.Tag.Equals(databaseFailureItem.Tag) && this.Guid.Equals(databaseFailureItem.Guid) && string.Equals(this.InstanceName, databaseFailureItem.InstanceName, StringComparison.OrdinalIgnoreCase) && ((string.IsNullOrEmpty(this.ComponentName) && string.IsNullOrEmpty(databaseFailureItem.ComponentName)) || string.Equals(this.ComponentName, databaseFailureItem.ComponentName, StringComparison.OrdinalIgnoreCase)) && object.Equals(this.IoError, databaseFailureItem.IoError) && object.Equals(this.NotifyEvent, databaseFailureItem.NotifyEvent) && ((string.IsNullOrEmpty(this.Message) && string.IsNullOrEmpty(databaseFailureItem.Message)) || string.Equals(this.Message, databaseFailureItem.Message, StringComparison.OrdinalIgnoreCase));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.AppendFormat("CreationTime={0} ", this.CreationTime);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.AppendFormat("Namespace={0} ", this.NameSpace);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.AppendFormat("Tag={0} ", this.Tag);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.AppendFormat("Guid={0} ", this.Guid);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.AppendFormat("InstanceName={0} ", this.InstanceName);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.AppendFormat("ComponentName={0} ", this.ComponentName ?? "<null>");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("IoError=");
			if (this.IoError != null)
			{
				stringBuilder.Append(this.IoError.ToString());
			}
			else
			{
				stringBuilder.Append("<null>");
			}
			stringBuilder.AppendLine();
			stringBuilder.Append("NotificationEventInfo=");
			if (this.NotifyEvent != null)
			{
				stringBuilder.Append(this.NotifyEvent.ToString());
			}
			else
			{
				stringBuilder.Append("<null>");
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendFormat("Message={0} ", this.Message ?? "<null>");
			stringBuilder.Append(Environment.NewLine);
			return stringBuilder.ToString();
		}

		internal static DatabaseFailureItem Parse(string xml, bool isValidate)
		{
			DatabaseFailureItem databaseFailureItem = new DatabaseFailureItem();
			if (isValidate)
			{
				databaseFailureItem.Validate();
			}
			return databaseFailureItem;
		}

		internal static DatabaseFailureItem Parse(EventRecord record)
		{
			DatabaseFailureItem databaseFailureItem = new DatabaseFailureItem();
			IList<EventProperty> properties = record.Properties;
			if (properties == null || properties.Count == 0)
			{
				return databaseFailureItem;
			}
			DatabaseFailureItem.PropertyContainer propertyContainer = new DatabaseFailureItem.PropertyContainer(properties);
			int num = propertyContainer.Get<int>(EventRecordParameteIndex.Version);
			if (num != DatabaseFailureItem.ApiVersion)
			{
				throw new InvalidFailureItemException("Version");
			}
			databaseFailureItem.NameSpace = propertyContainer.GetEnum<FailureNameSpace>(EventRecordParameteIndex.Namespace);
			databaseFailureItem.Tag = propertyContainer.GetEnum<FailureTag>(EventRecordParameteIndex.Tag);
			databaseFailureItem.Guid = propertyContainer.Get<Guid>(EventRecordParameteIndex.Guid);
			databaseFailureItem.InstanceName = propertyContainer.Get<string>(EventRecordParameteIndex.InstanceName);
			databaseFailureItem.ComponentName = propertyContainer.Get<string>(EventRecordParameteIndex.ComponentName);
			databaseFailureItem.CreationTime = (record.TimeCreated ?? DateTime.MinValue);
			bool flag = propertyContainer.Get<bool>(EventRecordParameteIndex.IsIoErrorSpecified);
			bool flag2 = propertyContainer.Get<bool>(EventRecordParameteIndex.IsNotifyEventSpecified);
			if (flag)
			{
				databaseFailureItem.IoError = new IoErrorInfo
				{
					Category = propertyContainer.GetEnum<IoErrorCategory>(EventRecordParameteIndex.IoErrorCategory),
					FileName = propertyContainer.Get<string>(EventRecordParameteIndex.IoErrorFileName),
					Offset = propertyContainer.Get<long>(EventRecordParameteIndex.IoErrorOffset),
					Size = propertyContainer.Get<long>(EventRecordParameteIndex.IoErrorSize)
				};
			}
			if (flag2)
			{
				NotificationEventInfo notificationEventInfo = new NotificationEventInfo();
				notificationEventInfo.EventId = propertyContainer.Get<int>(EventRecordParameteIndex.NotifyeventId);
				uint num2 = propertyContainer.Get<uint>(EventRecordParameteIndex.NotifyeventParambufferSize);
				if (num2 > 0U)
				{
					byte[] bytes = propertyContainer.Get<byte[]>(EventRecordParameteIndex.NotifyeventParambuffer);
					UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
					string @string = unicodeEncoding.GetString(bytes);
					string text = @string;
					char[] separator = new char[1];
					string[] array = text.Split(separator);
					notificationEventInfo.Parameters = new string[array.Length - 1];
					for (int i = 0; i < array.Length - 1; i++)
					{
						notificationEventInfo.Parameters[i] = array[i];
					}
				}
				databaseFailureItem.NotifyEvent = notificationEventInfo;
			}
			databaseFailureItem.Bookmark = record.Bookmark;
			if (propertyContainer.IsIndexValid(EventRecordParameteIndex.Message))
			{
				databaseFailureItem.Message = propertyContainer.Get<string>(EventRecordParameteIndex.Message);
			}
			return databaseFailureItem;
		}

		internal static DatabaseFailureItem[] GetEntriesFromEventLog()
		{
			return DatabaseFailureItem.GetEntriesFromEventLog(DatabaseFailureItem.ChannelName, DatabaseFailureItem.DefaultQueryString);
		}

		internal static DatabaseFailureItem[] GetEntriesFromEventLog(string channelName, string queryString)
		{
			List<DatabaseFailureItem> list = new List<DatabaseFailureItem>();
			EventLogQuery eventQuery = new EventLogQuery(channelName, PathType.LogName, queryString);
			DatabaseFailureItem[] result;
			using (EventLogReader eventLogReader = new EventLogReader(eventQuery))
			{
				for (;;)
				{
					using (EventRecord eventRecord = eventLogReader.ReadEvent())
					{
						if (eventRecord != null)
						{
							DatabaseFailureItem databaseFailureItem = DatabaseFailureItem.Parse(eventRecord);
							if (databaseFailureItem != null)
							{
								list.Add(databaseFailureItem);
							}
							continue;
						}
					}
					break;
				}
				result = list.ToArray();
			}
			return result;
		}

		internal static IDisposable SetPublishDatabaseFailureItemTestHook(Action action)
		{
			return DatabaseFailureItem.publishDatabaseFailureItemTestHook.SetTestHook(action);
		}

		internal void Publish()
		{
			this.Publish(true, false);
		}

		internal void PublishDebug()
		{
			this.Publish(true, true);
		}

		internal void Publish(bool isValidate)
		{
			this.Publish(isValidate, false);
		}

		internal void Publish(bool isValidate, bool isDebugChannel)
		{
			if (DatabaseFailureItem.publishDatabaseFailureItemTestHook.Value != null)
			{
				DatabaseFailureItem.publishDatabaseFailureItemTestHook.Value();
			}
			if (isValidate)
			{
				this.Validate();
			}
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			IntPtr intPtr3 = IntPtr.Zero;
			ExDbFailureItemApi.HaDbFailureItem haDbFailureItem = default(ExDbFailureItemApi.HaDbFailureItem);
			try
			{
				haDbFailureItem.CbSize = Marshal.SizeOf(typeof(ExDbFailureItemApi.HaDbFailureItem));
				haDbFailureItem.NameSpace = this.NameSpace;
				haDbFailureItem.Tag = this.Tag;
				haDbFailureItem.Guid = this.Guid;
				if (this.InstanceName != null)
				{
					haDbFailureItem.InstanceName = this.InstanceName;
				}
				if (this.ComponentName != null)
				{
					haDbFailureItem.ComponentName = this.ComponentName;
				}
				if (this.IoError != null)
				{
					ExDbFailureItemApi.HaDbIoErrorInfo haDbIoErrorInfo = default(ExDbFailureItemApi.HaDbIoErrorInfo);
					haDbIoErrorInfo.CbSize = Marshal.SizeOf(typeof(ExDbFailureItemApi.HaDbIoErrorInfo));
					haDbIoErrorInfo.Category = this.IoError.Category;
					if (this.IoError.FileName != null)
					{
						haDbIoErrorInfo.FileName = this.IoError.FileName;
					}
					haDbIoErrorInfo.Offset = this.IoError.Offset;
					haDbIoErrorInfo.Size = this.IoError.Size;
					intPtr = Marshal.AllocHGlobal(haDbIoErrorInfo.CbSize);
					Marshal.StructureToPtr(haDbIoErrorInfo, intPtr, false);
				}
				haDbFailureItem.IoError = intPtr;
				if (this.NotifyEvent != null)
				{
					ExDbFailureItemApi.HaDbNotificationEventInfo haDbNotificationEventInfo = default(ExDbFailureItemApi.HaDbNotificationEventInfo);
					haDbNotificationEventInfo.CbSize = Marshal.SizeOf(typeof(ExDbFailureItemApi.HaDbNotificationEventInfo));
					haDbNotificationEventInfo.EventId = this.NotifyEvent.EventId;
					if (this.NotifyEvent.Parameters != null)
					{
						haDbNotificationEventInfo.NumParameters = this.NotifyEvent.Parameters.Length;
						intPtr2 = MarshalHelper.StringArrayToIntPtr(this.NotifyEvent.Parameters);
						haDbNotificationEventInfo.Parameters = intPtr2;
					}
					intPtr3 = Marshal.AllocHGlobal(haDbNotificationEventInfo.CbSize);
					Marshal.StructureToPtr(haDbNotificationEventInfo, intPtr3, false);
				}
				haDbFailureItem.NotificationEventInfo = intPtr3;
				if (this.Message != null)
				{
					haDbFailureItem.Message = this.Message;
				}
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2837851453U);
				int num = ExDbFailureItemApi.PublishFailureItemEx(isDebugChannel, ref haDbFailureItem);
				if (num != 0)
				{
					throw new ExDbApiException(new Win32Exception(num));
				}
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
				intPtr = IntPtr.Zero;
				if (intPtr2 != IntPtr.Zero)
				{
					MarshalHelper.FreeIntPtrOfMarshalledObjectsArray(intPtr2, this.NotifyEvent.Parameters.Length);
					intPtr2 = IntPtr.Zero;
				}
				Marshal.FreeHGlobal(intPtr3);
				intPtr3 = IntPtr.Zero;
			}
		}

		internal void Validate()
		{
			if (!Enum.IsDefined(this.NameSpace.GetType(), this.NameSpace))
			{
				throw new InvalidFailureItemException("NameSpace");
			}
			if (!Enum.IsDefined(this.Tag.GetType(), this.Tag))
			{
				throw new InvalidFailureItemException("Tag");
			}
			if (this.Guid == Guid.Empty)
			{
				throw new InvalidFailureItemException("Guid");
			}
			if (this.InstanceName == null)
			{
				throw new InvalidFailureItemException("InstanceName");
			}
			if (this.IoError != null)
			{
				if (!Enum.IsDefined(this.IoError.Category.GetType(), this.IoError.Category))
				{
					throw new InvalidFailureItemException("IoErrorInfo.Category");
				}
				if (this.IoError.FileName == null)
				{
					throw new InvalidFailureItemException("IoErrorInfo.FileName");
				}
			}
			if (this.NotifyEvent != null && this.NotifyEvent.EventId == -1)
			{
				throw new InvalidFailureItemException("NotificationEventInfo.EventId");
			}
		}

		internal static readonly int ApiVersion = 1;

		internal static readonly string ChannelName = "Microsoft-Exchange-MailboxDatabaseFailureItems/Operational";

		internal static readonly string DebugChannelName = "Microsoft-Exchange-MailboxDatabaseFailureItems/Debug";

		internal static readonly Guid ChannelGuid = new Guid("{08E893EA-4C11-4fff-A737-99B9EEDEE4F4}");

		internal static readonly string DefaultQueryString = "*[UserData/EventXML]";

		private static Hookable<Action> publishDatabaseFailureItemTestHook = Hookable<Action>.Create(true, null);

		private FailureNameSpace nameSpace;

		private FailureTag tag;

		private Guid guid;

		private string instanceName;

		private string componentName;

		private IoErrorInfo ioError;

		private NotificationEventInfo notifyEvent;

		private DateTime creationTime;

		private EventBookmark bookMark;

		private string message;

		internal class PropertyContainer
		{
			internal PropertyContainer(IList<EventProperty> props)
			{
				this.properties = props;
				this.propertiesCount = props.Count;
			}

			internal T Get<T>(EventRecordParameteIndex index)
			{
				if (index > (EventRecordParameteIndex)(this.propertiesCount - 1))
				{
					throw new InvalidFailureItemException(string.Format("index {0} is out of range (max={1})", (int)index, this.propertiesCount));
				}
				EventProperty eventProperty = this.properties[(int)index];
				if (eventProperty == null)
				{
					throw new InvalidFailureItemException(string.Format("property# {0} is null", index));
				}
				object value = eventProperty.Value;
				if (value == null)
				{
					throw new InvalidFailureItemException(string.Format("property value# {0} is null", index));
				}
				return (T)((object)value);
			}

			internal T GetEnum<T>(EventRecordParameteIndex index)
			{
				object obj = this.Get<object>(index);
				object obj2;
				if (obj is uint)
				{
					obj2 = (uint)obj;
				}
				else
				{
					obj2 = (int)obj;
				}
				return (T)((object)obj2);
			}

			internal bool IsIndexValid(EventRecordParameteIndex index)
			{
				return index <= (EventRecordParameteIndex)(this.propertiesCount - 1);
			}

			private readonly int propertiesCount;

			private readonly IList<EventProperty> properties;
		}
	}
}
