using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.ApplicationLogic.Compliance
{
	internal class FolderEnumerator : QueryResultsEnumerator
	{
		internal FolderEnumerator(QueryResult queryResult, Folder rootFolder, object[] rootFolderProperties) : base(queryResult)
		{
			this.rootFolder = rootFolder;
			this.rootFolderProperties = rootFolderProperties;
			this.isAtBegining = true;
		}

		public override bool MoveNext()
		{
			bool result = base.MoveNext();
			if (this.isAtBegining)
			{
				if (base.Current != null)
				{
					base.Current.Insert(0, this.rootFolderProperties);
				}
				this.isAtBegining = false;
			}
			return result;
		}

		public override void Dispose()
		{
			base.Dispose();
			if (this.rootFolder != null)
			{
				this.rootFolder.Dispose();
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.isAtBegining = true;
		}

		protected override void HandleException(Exception exception)
		{
			ExTraceGlobals.StorageTracer.TraceError<string, Exception>(0L, "{0}: Failed to get folder hierarchy because the folder was not found or was inaccessible. Exception: '{1}'", this.rootFolder.DisplayName, exception);
		}

		private readonly Folder rootFolder;

		private readonly object[] rootFolderProperties;

		private bool isAtBegining;
	}
}
