using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class CalendarVersionStoreNotPopulatedException : StorageTransientException
	{
		public bool IsCreated { get; private set; }

		public SearchState FolderState { get; private set; }

		public TimeSpan WaitTimeBeforeThrow { get; private set; }

		public CalendarVersionStoreNotPopulatedException(bool isCreated, SearchState folderState, TimeSpan waitTime) : this(isCreated, folderState, waitTime, null)
		{
		}

		public CalendarVersionStoreNotPopulatedException(bool isCreated, SearchState folderState, TimeSpan waitTime, Exception innerException) : base(ServerStrings.CVSPopulationTimedout, innerException)
		{
			this.IsCreated = isCreated;
			this.FolderState = folderState;
			this.WaitTimeBeforeThrow = waitTime;
		}

		protected CalendarVersionStoreNotPopulatedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info != null)
			{
				this.IsCreated = info.GetBoolean("IsCreated");
				this.FolderState = (SearchState)info.GetValue("FolderState", typeof(SearchState));
				this.WaitTimeBeforeThrow = (TimeSpan)info.GetValue("WaitTimeBeforeThrow", typeof(TimeSpan));
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("IsCreated", this.IsCreated, typeof(bool));
			info.AddValue("FolderState", this.FolderState, typeof(SearchState));
			info.AddValue("WaitTimeBeforeThrow", this.WaitTimeBeforeThrow, typeof(TimeSpan));
		}

		private const string IsCreatedKey = "IsCreated";

		private const string FolderStateKey = "FolderState";

		private const string WaitTimeBeforeThrowKey = "WaitTimeBeforeThrow";
	}
}
