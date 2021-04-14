using System;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Search
{
	internal class MessageSearcher : IDisposable
	{
		internal Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		internal MessageSearcher(MapiStore store, SearchTestResult searchResult, MonitorHelper monitor, StopClass threadExit)
		{
			if (store == null)
			{
				throw new ArgumentNullException("store");
			}
			if (searchResult == null)
			{
				throw new ArgumentNullException("searchResult");
			}
			this.store = store;
			this.searchResult = searchResult;
			this.databaseGuid = searchResult.DatabaseGuid;
			this.searchString = DateTime.UtcNow.Ticks.ToString();
			this.monitor = monitor;
			this.threadExit = threadExit;
			this.ReadSleepTestHook();
		}

		void IDisposable.Dispose()
		{
			TestSearch.TestSearchTracer.TraceDebug((long)this.GetHashCode(), "Disposing MessageSearcher");
			if (!this.disposed)
			{
				lock (this)
				{
					if (this.folderId != null)
					{
						this.parentFolder.DeleteFolder(this.folderId, DeleteFolderFlags.DeleteMessages | DeleteFolderFlags.DelSubFolders | DeleteFolderFlags.ForceHardDelete);
						this.folderId = null;
					}
					if (this.searchFolder != null)
					{
						this.searchFolder.Dispose();
						this.searchFolder = null;
					}
					if (this.testFolder != null)
					{
						this.testFolder.Dispose();
						this.testFolder = null;
					}
					if (this.parentFolder != null)
					{
						this.parentFolder.Dispose();
						this.parentFolder = null;
					}
					if (this.store != null)
					{
						this.store.Dispose();
						this.store = null;
					}
					this.disposed = true;
					GC.SuppressFinalize(this);
				}
			}
		}

		private uint CreateMsgWithAttachement(out byte[] entryId)
		{
			ASCIIEncoding asciiencoding = new ASCIIEncoding();
			uint @int;
			using (MapiMessage mapiMessage = this.testFolder.CreateMessage())
			{
				PropValue[] props = new PropValue[]
				{
					new PropValue(PropTag.Subject, string.Format("CITestSearch: {0}.", this.searchString)),
					new PropValue(PropTag.Body, string.Format("The unique search string in the body is: {0}.", this.searchString)),
					new PropValue(PropTag.MessageDeliveryTime, (DateTime)ExDateTime.Now)
				};
				this.threadExit.CheckStop();
				mapiMessage.SetProps(props);
				int num;
				using (MapiAttach mapiAttach = mapiMessage.CreateAttach(out num))
				{
					string s = string.Format("This is a test msg created by test-search task (MSExchangeSearch {0}).It will be deleted soon...", this.searchString);
					byte[] bytes = asciiencoding.GetBytes(s);
					using (MapiStream mapiStream = mapiAttach.OpenStream(PropTag.AttachDataBin, OpenPropertyFlags.Create))
					{
						mapiStream.Write(bytes, 0, bytes.Length);
						mapiStream.Flush();
						this.threadExit.CheckStop();
					}
					props = new PropValue[]
					{
						new PropValue(PropTag.AttachFileName, "CITestSearch.txt"),
						new PropValue(PropTag.AttachMethod, AttachMethods.ByValue)
					};
					mapiAttach.SetProps(props);
					mapiAttach.SaveChanges();
				}
				this.threadExit.CheckStop();
				mapiMessage.SaveChanges();
				entryId = mapiMessage.GetProp(PropTag.EntryId).GetBytes();
				@int = (uint)mapiMessage.GetProp(PropTag.DocumentId).GetInt();
			}
			return @int;
		}

		private void SearchMapiNotificationHandler(MapiNotification notification)
		{
			if (notification.NotificationType == AdviseFlags.SearchComplete)
			{
				this.SearchComplete.Set();
			}
		}

		private bool FoundMessage(int docIdToLookFor)
		{
			this.AddMonitoringEvent(this.searchResult, Strings.TestSearchFindMessage(this.searchResult.Database, this.searchResult.Mailbox));
			try
			{
				using (MapiTable contentsTable = this.searchFolder.GetContentsTable(ContentsTableFlags.DeferredErrors))
				{
					this.threadExit.CheckStop();
					Restriction restriction = new Restriction.PropertyRestriction(Restriction.RelOp.Equal, PropTag.DocumentId, docIdToLookFor);
					if (contentsTable.QueryOneValue(PropTag.DocumentId, restriction) != null)
					{
						return true;
					}
				}
			}
			catch (MapiExceptionNotFound)
			{
				return false;
			}
			return false;
		}

		private bool SearchSucceeded(Restriction res)
		{
			bool result;
			lock (this)
			{
				if (res == null)
				{
					throw new ArgumentNullException("res");
				}
				this.threadExit.CheckStop();
				byte[] bytes = this.searchFolder.GetProp(PropTag.EntryId).GetBytes();
				this.searchFolder.GetProp(PropTag.ContainerClass).GetString();
				this.SearchComplete.Reset();
				this.threadExit.CheckStop();
				MapiNotificationHandle mapiNotificationHandle = this.store.Advise(bytes, AdviseFlags.SearchComplete, new MapiNotificationHandler(this.SearchMapiNotificationHandler), (MapiNotificationClientFlags)0);
				try
				{
					this.threadExit.CheckStop();
					this.searchFolder.SetSearchCriteria(res, new byte[][]
					{
						this.folderId
					}, SearchCriteriaFlags.Restart | SearchCriteriaFlags.Foreground);
					this.threadExit.CheckStop();
					bool flag2 = this.SearchComplete.WaitOne(MessageSearcher.SearchCompleteTimeout, true);
					if (flag2)
					{
						this.threadExit.CheckStop();
						Thread.Sleep(this.testSearchStallInSeconds);
						return this.FoundMessage((int)this.documentId);
					}
				}
				finally
				{
					if (mapiNotificationHandle != null)
					{
						this.store.Unadvise(mapiNotificationHandle);
					}
				}
				result = false;
			}
			return result;
		}

		private void DeleteOldTestFolders()
		{
			using (MapiTable hierarchyTable = this.parentFolder.GetHierarchyTable())
			{
				PropValue[][] array = hierarchyTable.QueryAllRows(Restriction.Content(PropTag.DisplayName, "test-exchangesearch-folder-", ContentFlags.Prefix), new PropTag[]
				{
					PropTag.EntryId
				});
				foreach (PropValue[] array3 in array)
				{
					this.threadExit.CheckStop();
					this.parentFolder.DeleteFolder(array3[0].GetBytes(), DeleteFolderFlags.DeleteMessages | DeleteFolderFlags.DelSubFolders | DeleteFolderFlags.ForceHardDelete);
				}
			}
		}

		internal void InitializeSearch()
		{
			lock (this)
			{
				this.threadExit.CheckStop();
				this.AddMonitoringEvent(this.searchResult, Strings.TestSearchCurrentMailbox(this.searchResult.Mailbox));
				this.AddMonitoringEvent(this.searchResult, Strings.TestSearchGetNonIpmSubTreeFolder(this.searchResult.Database, this.searchResult.Mailbox));
				this.parentFolder = this.store.GetNonIpmSubtreeFolder();
				this.threadExit.CheckStop();
				Thread.Sleep(this.testInitStallInSeconds);
				this.DeleteOldTestFolders();
				string folderName = "test-exchangesearch-folder-" + Guid.NewGuid();
				this.threadExit.CheckStop();
				this.AddMonitoringEvent(this.searchResult, Strings.TestSearchCreateFolder(this.searchResult.Database, this.searchResult.Mailbox));
				this.testFolder = this.parentFolder.CreateFolder(folderName, null, true);
				this.folderId = this.testFolder.GetProp(PropTag.EntryId).GetBytes();
				this.threadExit.CheckStop();
				this.testFolder.EmptyFolder(EmptyFolderFlags.ForceHardDelete);
				this.threadExit.CheckStop();
				this.AddMonitoringEvent(this.searchResult, Strings.TestSearchCreateMessage(this.searchResult.Database, this.searchResult.Mailbox));
				byte[] entryId;
				this.documentId = this.CreateMsgWithAttachement(out entryId);
				this.searchResult.EntryId = entryId;
				this.searchResult.DocumentId = this.documentId;
				long ticks = ExDateTime.Now.LocalTime.Ticks;
				string folderName2 = string.Format("test-{0}", ticks);
				this.threadExit.CheckStop();
				this.AddMonitoringEvent(this.searchResult, Strings.TestSearchCreateSearchFolder(this.searchResult.Database, this.searchResult.Mailbox));
				this.searchFolder = this.testFolder.CreateSearchFolder(folderName2, "", false);
				this.threadExit.CheckStop();
			}
		}

		internal bool DoSearch()
		{
			return this.SearchSucceeded(Restriction.Content(PropTag.Subject, this.searchString, ContentFlags.Prefix));
		}

		private void AddMonitoringEvent(SearchTestResult result, LocalizedString msg)
		{
			if (this.monitor != null)
			{
				this.monitor.AddMonitoringEvent(result, msg);
			}
		}

		private void ReadSleepTestHook()
		{
			RegistryKey registryKey2;
			RegistryKey registryKey = registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ExSearch\\TestHook", RegistryKeyPermissionCheck.ReadSubTree);
			try
			{
				if (registryKey != null)
				{
					this.testInitStallInSeconds = (int)registryKey.GetValue("MessageSearcherInitStallInSeconds", 0);
					this.testSearchStallInSeconds = (int)registryKey.GetValue("MessageSearcherSearchStallInSeconds", 0);
				}
			}
			finally
			{
				if (registryKey2 != null)
				{
					((IDisposable)registryKey2).Dispose();
				}
			}
		}

		private const string TestFolderNamePrefix = "test-exchangesearch-folder-";

		private const string TestHookKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ExSearch\\TestHook";

		private const string InitStallKey = "MessageSearcherInitStallInSeconds";

		private const string SearchStallKey = "MessageSearcherSearchStallInSeconds";

		private MapiStore store;

		private SearchTestResult searchResult;

		private MapiFolder parentFolder;

		private MapiFolder testFolder;

		private MapiFolder searchFolder;

		private readonly Guid databaseGuid;

		private byte[] folderId;

		private uint documentId;

		private readonly string searchString;

		private MonitorHelper monitor;

		private StopClass threadExit;

		private int testInitStallInSeconds;

		private int testSearchStallInSeconds;

		private static TimeSpan SearchCompleteTimeout = new TimeSpan(0, 0, 2);

		private ManualResetEvent SearchComplete = new ManualResetEvent(false);

		private bool disposed;
	}
}
