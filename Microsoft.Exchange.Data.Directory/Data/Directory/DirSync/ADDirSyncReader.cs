using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;

namespace Microsoft.Exchange.Data.Directory.DirSync
{
	internal class ADDirSyncReader<TResult> : ADGenericPagedReader<TResult> where TResult : ADDirSyncResult, new()
	{
		internal ADDirSyncReader(IDirectorySession session, QueryFilter filter, IEnumerable<PropertyDefinition> properties) : this(session, filter, 100, properties)
		{
		}

		internal ADDirSyncReader(IDirectorySession session, QueryFilter filter, int pageSize, IEnumerable<PropertyDefinition> properties) : base(session, ADDirSyncReader<TResult>.GetSearchRootForSession(session), QueryScope.SubTree, filter, null, pageSize, properties, false)
		{
			this.dirSyncRequestControl = new DirSyncRequestControl();
			this.dirSyncRequestControl.Option = (DirectorySynchronizationOptions)((ulong)int.MinValue);
			base.DirectoryControls.Add(this.dirSyncRequestControl);
		}

		internal TResult[] GetNextResultPage()
		{
			return this.GetNextPage();
		}

		protected override int SizeLimit
		{
			get
			{
				return base.PageSize;
			}
		}

		protected override SearchResultEntryCollection GetNextResultCollection()
		{
			this.dirSyncRequestControl.Cookie = base.Cookie;
			DirectoryControl directoryControl;
			SearchResultEntryCollection nextResultCollection = base.GetNextResultCollection(typeof(DirSyncResponseControl), out directoryControl);
			DirSyncResponseControl dirSyncResponseControl = directoryControl as DirSyncResponseControl;
			base.Cookie = dirSyncResponseControl.Cookie;
			base.RetrievedAllData = new bool?(!dirSyncResponseControl.MoreData);
			return nextResultCollection;
		}

		private static ADObjectId GetSearchRootForSession(IDirectorySession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (!session.UseConfigNC)
			{
				return session.GetDomainNamingContext();
			}
			return session.GetConfigurationNamingContext();
		}

		private const int DirSyncDefaultSizeLimit = 100;

		private readonly DirSyncRequestControl dirSyncRequestControl;
	}
}
