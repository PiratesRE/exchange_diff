using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Rpc.QueueViewer;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport.QueueViewer
{
	internal sealed class QueueViewerServer : QueueViewerRpcServer
	{
		public override byte[] GetPropertyBagBasedQueueViewerObjectPageCustomSerialization(QVObjectType objectType, byte[] inputObjectBytes, ref int totalCount, ref int pageOffset)
		{
			Version targetVersion;
			QueueViewerInputObject queueViewerInputObject;
			try
			{
				queueViewerInputObject = Serialization.BytesToInputObject(objectType, inputObjectBytes, out targetVersion);
			}
			catch (SerializationException)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
			}
			return this.GetQueueViewerObjectPage(objectType, targetVersion, queueViewerInputObject.QueryFilter, queueViewerInputObject.SortOrder, queueViewerInputObject.SearchForward, queueViewerInputObject.PageSize, (PagedDataObject)queueViewerInputObject.BookmarkObject, queueViewerInputObject.BookmarkIndex, queueViewerInputObject.IncludeBookmark, queueViewerInputObject.IncludeDetails, queueViewerInputObject.IncludeComponentLatencyInfo, QueueViewerServer.ResultObjectType.PagedCustom, ref totalCount, ref pageOffset);
		}

		public override byte[] GetPropertyBagBasedQueueViewerObjectPage(QVObjectType objectType, byte[] inputObjectBytes, ref int totalCount, ref int pageOffset)
		{
			QueueViewerInputObject queueViewerInputObject;
			try
			{
				queueViewerInputObject = (QueueViewerInputObject)Serialization.BytesToObject(inputObjectBytes);
			}
			catch (SerializationException)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
			}
			return this.GetQueueViewerObjectPage(objectType, Serialization.Version, queueViewerInputObject.QueryFilter, queueViewerInputObject.SortOrder, queueViewerInputObject.SearchForward, queueViewerInputObject.PageSize, (PagedDataObject)queueViewerInputObject.BookmarkObject, queueViewerInputObject.BookmarkIndex, queueViewerInputObject.IncludeBookmark, queueViewerInputObject.IncludeDetails, queueViewerInputObject.IncludeComponentLatencyInfo, QueueViewerServer.ResultObjectType.PagedBinary, ref totalCount, ref pageOffset);
		}

		public override byte[] GetQueueViewerObjectPage(QVObjectType objectType, byte[] queryFilterBytes, byte[] sortOrderBytes, bool searchForward, int pageSize, byte[] bookmarkObjectBytes, int bookmarkIndex, bool includeBookmark, bool includeDetails, byte[] propertyBagBytes, ref int totalCount, ref int pageOffset)
		{
			List<QueueViewerSortOrderEntry> list = null;
			QueryFilter queryFilter;
			SortOrderEntry[] array;
			PagedDataObject pagedDataObject;
			try
			{
				queryFilter = (QueryFilter)Serialization.BytesToObject(queryFilterBytes);
				array = (SortOrderEntry[])Serialization.BytesToObject(sortOrderBytes);
				pagedDataObject = (PagedDataObject)Serialization.BytesToObject(bookmarkObjectBytes);
			}
			catch (SerializationException)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
			}
			if (array != null)
			{
				list = new List<QueueViewerSortOrderEntry>();
				foreach (SortOrderEntry sortOrderEntry in array)
				{
					list.Add(QueueViewerSortOrderEntry.Parse(sortOrderEntry.ToString()));
				}
			}
			if (pagedDataObject != null)
			{
				if (objectType == QVObjectType.MessageInfo)
				{
					pagedDataObject = this.GetExtensibleMessageInfo(pagedDataObject as MessageInfo);
				}
				else if (objectType == QVObjectType.QueueInfo)
				{
					pagedDataObject = this.GetExtensibleQueueInfo(pagedDataObject as QueueInfo);
				}
			}
			bool includeComponentLatencyInfo = false;
			if (propertyBagBytes != null)
			{
				MdbefPropertyCollection mdbefPropertyCollection = MdbefPropertyCollection.Create(propertyBagBytes, 0, propertyBagBytes.Length);
				object obj;
				if (mdbefPropertyCollection.TryGetValue(2483093515U, out obj) && obj is bool)
				{
					includeComponentLatencyInfo = (bool)obj;
				}
			}
			return this.GetQueueViewerObjectPage(objectType, Serialization.Version, queryFilter, (list != null) ? list.ToArray() : null, searchForward, pageSize, pagedDataObject, bookmarkIndex, includeBookmark, includeDetails, includeComponentLatencyInfo, QueueViewerServer.ResultObjectType.Legacy, ref totalCount, ref pageOffset);
		}

		public override void FreezeQueue(byte[] queueIdentityBytes, byte[] queryFilterBytes)
		{
			try
			{
				if (queueIdentityBytes != null)
				{
					QueueIdentity queueIdentity;
					try
					{
						queueIdentity = (QueueIdentity)Serialization.BytesToObject(queueIdentityBytes);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
					if (!QueueManager.FreezeQueue(queueIdentity))
					{
						throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
					}
				}
				else
				{
					if (queryFilterBytes == null)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_AMBIGUOUS_PARAMETER_SET);
					}
					QueryFilter queryFilter;
					try
					{
						queryFilter = (QueryFilter)Serialization.BytesToObject(queryFilterBytes);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
					PagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema> pagingEngine = QueueViewerServer.NewPagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema>(queryFilter, null, true, int.MaxValue, null, -1, false, false, false);
					int num = 0;
					int arg;
					int num2;
					ExtensibleQueueInfo[] queueInfoPage = Components.QueueManager.GetQueueInfoPage(pagingEngine, out arg, out num2);
					foreach (ExtensibleQueueInfo extensibleQueueInfo in queueInfoPage)
					{
						try
						{
							if (QueueManager.FreezeQueue((QueueIdentity)extensibleQueueInfo.Identity))
							{
								num++;
							}
						}
						catch (QueueViewerException)
						{
						}
					}
					ExTraceGlobals.QueuingTracer.TraceDebug<int, int>((long)this.GetHashCode(), "{0} queue(s) were frozen out of {1} that matched the filter", num, arg);
				}
			}
			catch (QueueViewerException)
			{
				throw;
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
			}
		}

		public override void UnfreezeQueue(byte[] queueIdentityBytes, byte[] queryFilterBytes)
		{
			try
			{
				if (queueIdentityBytes != null)
				{
					QueueIdentity queueIdentity;
					try
					{
						queueIdentity = (QueueIdentity)Serialization.BytesToObject(queueIdentityBytes);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
					if (!QueueManager.UnfreezeQueue(queueIdentity))
					{
						throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
					}
				}
				else
				{
					if (queryFilterBytes == null)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_AMBIGUOUS_PARAMETER_SET);
					}
					QueryFilter queryFilter;
					try
					{
						queryFilter = (QueryFilter)Serialization.BytesToObject(queryFilterBytes);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
					PagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema> pagingEngine = QueueViewerServer.NewPagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema>(queryFilter, null, true, int.MaxValue, null, -1, false, false, false);
					int num = 0;
					int arg;
					int num2;
					ExtensibleQueueInfo[] queueInfoPage = Components.QueueManager.GetQueueInfoPage(pagingEngine, out arg, out num2);
					foreach (ExtensibleQueueInfo extensibleQueueInfo in queueInfoPage)
					{
						try
						{
							if (QueueManager.UnfreezeQueue((QueueIdentity)extensibleQueueInfo.Identity))
							{
								num++;
							}
						}
						catch (QueueViewerException)
						{
						}
					}
					ExTraceGlobals.QueuingTracer.TraceDebug<int, int>((long)this.GetHashCode(), "{0} queue(s) were unfrozen out of {1} that matched the filter", num, arg);
				}
			}
			catch (QueueViewerException)
			{
				throw;
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
			}
		}

		public override void RetryQueue(byte[] queueIdentityBytes, byte[] queryFilterBytes, bool resubmit)
		{
			bool flag = false;
			string text = string.Empty;
			bool asynchronousRetryQueue = Components.TransportAppConfig.QueueConfiguration.AsynchronousRetryQueue;
			try
			{
				Monitor.TryEnter(QueueViewerServer.RetryQueueInProgressSyncObject, ref flag);
				if (!flag || (this.pendingRetryQueueTask != null && !this.pendingRetryQueueTask.IsCompleted))
				{
					throw new QueueViewerException(QVErrorCode.QV_E_QUEUE_RESUBMIT_IN_PROGRESS);
				}
				List<QueueIdentity> queueIdentities;
				if (queueIdentityBytes != null)
				{
					QueueIdentity queueIdentity;
					try
					{
						queueIdentity = (QueueIdentity)Serialization.BytesToObject(queueIdentityBytes);
						if (Components.TransportAppConfig.QueueConfiguration.AsynchronousRetryQueue)
						{
							QueueManager.PreAsyncRetryQueueValidate(queueIdentity, resubmit);
						}
						text = string.Format("-Identity {0} -Resubmit:${1}", queueIdentity, resubmit);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
					queueIdentities = new List<QueueIdentity>(1)
					{
						queueIdentity
					};
				}
				else
				{
					if (queryFilterBytes == null)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_AMBIGUOUS_PARAMETER_SET);
					}
					QueryFilter queryFilter;
					try
					{
						queryFilter = (QueryFilter)Serialization.BytesToObject(queryFilterBytes);
						text = string.Format("-Filter {{{0}}} -Resubmit:${1}", queryFilter, resubmit);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
					PagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema> pagingEngine = QueueViewerServer.NewPagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema>(queryFilter, null, true, int.MaxValue, null, -1, false, false, false);
					int num;
					int num2;
					ExtensibleQueueInfo[] queueInfoPage = Components.QueueManager.GetQueueInfoPage(pagingEngine, out num, out num2);
					queueIdentities = (from queueInfo in queueInfoPage
					select queueInfo.QueueIdentity).ToList<QueueIdentity>();
				}
				this.pendingRetryQueueTask = this.RetryQueueHelperAsync(queueIdentities, resubmit, text);
				if (!asynchronousRetryQueue)
				{
					this.pendingRetryQueueTask.Wait(Components.TransportAppConfig.QueueConfiguration.SynchronousRetryQueueTimeout);
				}
			}
			catch (AggregateException ex)
			{
				Exception ex2 = ex.InnerExceptions.FirstOrDefault((Exception iex) => !(iex is QueueViewerException)) ?? ex.InnerExceptions[0];
				QueueManager.EventLogger.LogEvent(TransportEventLogConstants.Tuple_QueueViewerExceptionDuringAsyncRetryQueue, null, new object[]
				{
					text,
					ex2.Message,
					ex2.StackTrace
				});
				if (!(ex2 is QueueViewerException))
				{
					ExWatson.SendReportAndCrashOnAnotherThread(ex2);
				}
				else if (!asynchronousRetryQueue && queueIdentityBytes != null)
				{
					throw ex2;
				}
			}
			catch (QueueViewerException)
			{
				throw;
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(QueueViewerServer.RetryQueueInProgressSyncObject);
				}
			}
		}

		public override int ReadMessageBody(byte[] mailItemIdBytes, byte[] buffer, int position, int count)
		{
			int result;
			try
			{
				if (mailItemIdBytes == null)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_AMBIGUOUS_PARAMETER_SET);
				}
				MessageIdentity messageIdentity;
				try
				{
					messageIdentity = (MessageIdentity)Serialization.BytesToObject(mailItemIdBytes);
				}
				catch (SerializationException)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
				}
				if (messageIdentity.IsFullySpecified)
				{
					int num;
					if (!Components.QueueManager.ReadMessageBody(messageIdentity, buffer, position, count, out num))
					{
						throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
					}
					result = num;
				}
				else
				{
					QueryFilter queryFilter = new TextFilter(ExtensibleMessageInfoSchema.Identity, messageIdentity.ToString(), MatchOptions.FullString, MatchFlags.Default);
					PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine = QueueViewerServer.NewPagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema>(queryFilter, null, true, int.MaxValue, null, -1, false, false, false);
					int num2 = 0;
					int num3;
					ExtensibleMessageInfo[] messageInfoPage = Components.QueueManager.GetMessageInfoPage(pagingEngine, out num3, out num2);
					if (messageInfoPage.Length == 0)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
					}
					if (messageInfoPage.Length > 1)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH);
					}
					int num;
					if (!Components.QueueManager.ReadMessageBody((MessageIdentity)messageInfoPage[0].Identity, buffer, position, count, out num))
					{
						throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
					}
					result = num;
				}
			}
			catch (QueueViewerException)
			{
				throw;
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
				result = 0;
			}
			return result;
		}

		public override void FreezeMessage(byte[] mailItemIdBytes, byte[] queryFilterBytes)
		{
			if ((mailItemIdBytes == null && queryFilterBytes == null) || (mailItemIdBytes != null && queryFilterBytes != null))
			{
				throw new QueueViewerException(QVErrorCode.QV_E_AMBIGUOUS_PARAMETER_SET);
			}
			try
			{
				QueryFilter queryFilter = null;
				bool flag = false;
				if (mailItemIdBytes != null)
				{
					MessageIdentity messageIdentity;
					try
					{
						messageIdentity = (MessageIdentity)Serialization.BytesToObject(mailItemIdBytes);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
					if (messageIdentity.IsFullySpecified)
					{
						if (!QueueManager.FreezeMessage(messageIdentity))
						{
							throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
						}
						return;
					}
					else
					{
						queryFilter = new TextFilter(ExtensibleMessageInfoSchema.Identity, messageIdentity.ToString(), MatchOptions.FullString, MatchFlags.Default);
						flag = true;
					}
				}
				if (queryFilterBytes != null)
				{
					try
					{
						queryFilter = (QueryFilter)Serialization.BytesToObject(queryFilterBytes);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
				}
				PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine = QueueViewerServer.NewPagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema>(queryFilter, null, true, int.MaxValue, null, -1, false, false, false);
				int num = 0;
				int arg;
				int num2;
				ExtensibleMessageInfo[] messageInfoPage = Components.QueueManager.GetMessageInfoPage(pagingEngine, out arg, out num2);
				if (flag)
				{
					if (messageInfoPage.Length == 0)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
					}
					if (messageInfoPage.Length > 1)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH);
					}
				}
				foreach (ExtensibleMessageInfo extensibleMessageInfo in messageInfoPage)
				{
					try
					{
						if (QueueManager.FreezeMessage((MessageIdentity)extensibleMessageInfo.Identity))
						{
							num++;
						}
					}
					catch (QueueViewerException)
					{
						if (flag)
						{
							throw;
						}
					}
				}
				ExTraceGlobals.QueuingTracer.TraceDebug<int, int>((long)this.GetHashCode(), "{0} message(s) were frozen out of {1} that matched the filter", num, arg);
			}
			catch (QueueViewerException)
			{
				throw;
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
			}
		}

		public override void UnfreezeMessage(byte[] mailItemIdBytes, byte[] queryFilterBytes)
		{
			if ((mailItemIdBytes == null && queryFilterBytes == null) || (mailItemIdBytes != null && queryFilterBytes != null))
			{
				throw new QueueViewerException(QVErrorCode.QV_E_AMBIGUOUS_PARAMETER_SET);
			}
			try
			{
				QueryFilter queryFilter = null;
				bool flag = false;
				if (mailItemIdBytes != null)
				{
					MessageIdentity messageIdentity;
					try
					{
						messageIdentity = (MessageIdentity)Serialization.BytesToObject(mailItemIdBytes);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
					if (messageIdentity.IsFullySpecified)
					{
						if (!Components.QueueManager.UnfreezeMessage(messageIdentity))
						{
							throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
						}
						return;
					}
					else
					{
						queryFilter = new TextFilter(ExtensibleMessageInfoSchema.Identity, messageIdentity.ToString(), MatchOptions.FullString, MatchFlags.Default);
						flag = true;
					}
				}
				if (queryFilterBytes != null)
				{
					try
					{
						queryFilter = (QueryFilter)Serialization.BytesToObject(queryFilterBytes);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
				}
				PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine = QueueViewerServer.NewPagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema>(queryFilter, null, true, int.MaxValue, null, -1, false, false, false);
				int num = 0;
				int arg;
				int num2;
				ExtensibleMessageInfo[] messageInfoPage = Components.QueueManager.GetMessageInfoPage(pagingEngine, out arg, out num2);
				if (flag)
				{
					if (messageInfoPage.Length == 0)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
					}
					if (messageInfoPage.Length > 1)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH);
					}
				}
				foreach (ExtensibleMessageInfo extensibleMessageInfo in messageInfoPage)
				{
					try
					{
						if (Components.QueueManager.UnfreezeMessage((MessageIdentity)extensibleMessageInfo.Identity))
						{
							num++;
						}
					}
					catch (QueueViewerException)
					{
						if (flag)
						{
							throw;
						}
					}
				}
				ExTraceGlobals.QueuingTracer.TraceDebug<int, int>((long)this.GetHashCode(), "{0} message(s) were unfrozen out of {1} that matched the filter", num, arg);
			}
			catch (QueueViewerException)
			{
				throw;
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
			}
		}

		public override void DeleteMessage(byte[] mailItemIdBytes, byte[] queryFilterBytes, bool withNDR)
		{
			if ((mailItemIdBytes == null && queryFilterBytes == null) || (mailItemIdBytes != null && queryFilterBytes != null))
			{
				throw new QueueViewerException(QVErrorCode.QV_E_AMBIGUOUS_PARAMETER_SET);
			}
			try
			{
				QueryFilter queryFilter = null;
				bool singleMessage = false;
				if (mailItemIdBytes != null)
				{
					MessageIdentity messageIdentity;
					try
					{
						messageIdentity = (MessageIdentity)Serialization.BytesToObject(mailItemIdBytes);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
					if (messageIdentity.IsFullySpecified)
					{
						if (!Components.QueueManager.DeleteMessage(messageIdentity, withNDR))
						{
							throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
						}
						return;
					}
					else
					{
						queryFilter = new TextFilter(ExtensibleMessageInfoSchema.Identity, messageIdentity.ToString(), MatchOptions.FullString, MatchFlags.Default);
						singleMessage = true;
					}
				}
				if (queryFilterBytes != null)
				{
					try
					{
						queryFilter = (QueryFilter)Serialization.BytesToObject(queryFilterBytes);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
				}
				int totalCount = 0;
				int messagesDeleted = 0;
				QueueViewerServer.ApplyActionToMessages(queryFilter, singleMessage, delegate(ExtensibleMessageInfo messageInfo)
				{
					if (Components.QueueManager.DeleteMessage((MessageIdentity)messageInfo.Identity, withNDR))
					{
						messagesDeleted++;
					}
					totalCount++;
				});
				ExTraceGlobals.QueuingTracer.TraceDebug<int, int>((long)this.GetHashCode(), "{0} message(s) were unfrozen out of {1} that matched the filter", messagesDeleted, totalCount);
			}
			catch (QueueViewerException)
			{
				throw;
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
			}
		}

		public override void RedirectMessage(byte[] targetServersBytes)
		{
			try
			{
				Components.QueueManager.RedirectMessage(this.DeserializeObject<MultiValuedProperty<Fqdn>>(targetServersBytes));
			}
			catch (QueueViewerException)
			{
				throw;
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
			}
		}

		public override void SetMessage(byte[] mailItemIdBytes, byte[] queryFilterBytes, byte[] inputPropertiesBytes, bool resubmit)
		{
			if ((mailItemIdBytes == null && queryFilterBytes == null) || (mailItemIdBytes != null && queryFilterBytes != null))
			{
				throw new QueueViewerException(QVErrorCode.QV_E_AMBIGUOUS_PARAMETER_SET);
			}
			if (inputPropertiesBytes == null)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
			}
			try
			{
				QueryFilter queryFilter = null;
				bool singleMessage = false;
				ExtensibleMessageInfo properties;
				try
				{
					properties = Serialization.BytesToSingleMessageObject(inputPropertiesBytes);
				}
				catch (SerializationException)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
				}
				if (mailItemIdBytes != null)
				{
					MessageIdentity messageIdentity;
					try
					{
						messageIdentity = Serialization.BytesToMessageId(mailItemIdBytes);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
					if (messageIdentity.IsFullySpecified)
					{
						if (!Components.QueueManager.SetMessage(messageIdentity, properties, resubmit))
						{
							throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
						}
						return;
					}
					else
					{
						queryFilter = new TextFilter(ExtensibleMessageInfoSchema.Identity, messageIdentity.ToString(), MatchOptions.FullString, MatchFlags.Default);
						singleMessage = true;
					}
				}
				if (queryFilterBytes != null)
				{
					try
					{
						queryFilter = Serialization.BytesToFilterObject(queryFilterBytes);
					}
					catch (SerializationException)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
					}
				}
				int messagesUpdated = 0;
				int totalCount = 0;
				QueueViewerServer.ApplyActionToMessages(queryFilter, singleMessage, delegate(ExtensibleMessageInfo messageInfo)
				{
					if (Components.QueueManager.SetMessage((MessageIdentity)messageInfo.Identity, properties, resubmit))
					{
						messagesUpdated++;
					}
					totalCount++;
				});
				ExTraceGlobals.QueuingTracer.TraceDebug<int, int>((long)this.GetHashCode(), "{0} message(s) were updated out of {1} that matched the filter", messagesUpdated, totalCount);
			}
			catch (QueueViewerException)
			{
				throw;
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
			}
		}

		private static PagingEngine<ObjectType, SchemaType> NewPagingEngine<ObjectType, SchemaType>(QueryFilter queryFilter, QueueViewerSortOrderEntry[] sortOrder, bool searchForward, int pageSize, ObjectType bookmarkObject, int bookmarkIndex, bool includeBookmark, bool includeDetails, bool includeComponentLatencyInfo) where ObjectType : PagedDataObject where SchemaType : PagedObjectSchema, new()
		{
			PagingEngine<ObjectType, SchemaType> pagingEngine = new PagingEngine<ObjectType, SchemaType>();
			pagingEngine.SetFilter(queryFilter);
			pagingEngine.SetSortOrder(sortOrder);
			pagingEngine.SearchForward = searchForward;
			pagingEngine.PageSize = pageSize;
			pagingEngine.BookmarkObject = bookmarkObject;
			pagingEngine.BookmarkIndex = bookmarkIndex;
			pagingEngine.IncludeBookmark = includeBookmark;
			pagingEngine.IncludeDetails = includeDetails;
			pagingEngine.IncludeComponentLatencyInfo = includeComponentLatencyInfo;
			return pagingEngine;
		}

		private static void ApplyActionToMessages(QueryFilter queryFilter, bool singleMessage, Action<ExtensibleMessageInfo> messageAction)
		{
			PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine = QueueViewerServer.NewPagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema>(queryFilter, null, true, int.MaxValue, null, -1, false, false, false);
			int num;
			int num2;
			ExtensibleMessageInfo[] messageInfoPage = Components.QueueManager.GetMessageInfoPage(pagingEngine, out num, out num2);
			if (singleMessage)
			{
				if (messageInfoPage.Length == 0)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
				}
				if (messageInfoPage.Length > 1)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH);
				}
			}
			foreach (ExtensibleMessageInfo obj in messageInfoPage)
			{
				try
				{
					messageAction(obj);
				}
				catch (QueueViewerException)
				{
					if (singleMessage)
					{
						throw;
					}
				}
			}
		}

		private byte[] GetQueueViewerObjectPage(QVObjectType objectType, Version targetVersion, QueryFilter queryFilter, QueueViewerSortOrderEntry[] sortOrder, bool searchForward, int pageSize, PagedDataObject bookmarkObject, int bookmarkIndex, bool includeBookmark, bool includeDetails, bool includeComponentLatencyInfo, QueueViewerServer.ResultObjectType resultObjectType, ref int totalCount, ref int pageOffset)
		{
			byte[] result;
			try
			{
				if (bookmarkIndex != -1 && bookmarkObject != null)
				{
					throw new ArgumentException("BookmarkIndex and BookmarkObject are exclusive arguments", "bookmarkIndex, bookmarkObject");
				}
				if (pageSize < 0)
				{
					throw new ArgumentException("PageSize must be greater than 0", "pageSize");
				}
				byte[] array = null;
				PagedDataObject[] array2;
				if (objectType == QVObjectType.MessageInfo)
				{
					QueryFilter queryFilter2 = queryFilter;
					if (queryFilter == null)
					{
						queryFilter2 = new CSharpFilter<ExtensibleMessageInfo>((ExtensibleMessageInfo messageInfo) => messageInfo.Queue.Type != QueueType.Shadow);
					}
					QueryFilter queryFilter3 = queryFilter2;
					PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine = QueueViewerServer.NewPagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema>(queryFilter3, sortOrder, searchForward, pageSize, (ExtensibleMessageInfo)bookmarkObject, bookmarkIndex, includeBookmark, includeDetails, includeComponentLatencyInfo);
					array2 = Components.QueueManager.GetMessageInfoPage(pagingEngine, out totalCount, out pageOffset);
					if (resultObjectType == QueueViewerServer.ResultObjectType.Legacy)
					{
						List<MessageInfo> list = new List<MessageInfo>();
						foreach (PagedDataObject pagedDataObject in array2)
						{
							list.Add(new MessageInfo((ExtensibleMessageInfo)pagedDataObject));
						}
						array = Serialization.ObjectToBytes(list.ToArray());
					}
				}
				else
				{
					if (objectType != QVObjectType.QueueInfo)
					{
						throw new ArgumentException("Unknown generic argument type", "objectType");
					}
					PagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema> pagingEngine2 = QueueViewerServer.NewPagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema>(queryFilter, sortOrder, searchForward, pageSize, (ExtensibleQueueInfo)bookmarkObject, bookmarkIndex, includeBookmark, false, false);
					array2 = Components.QueueManager.GetQueueInfoPage(pagingEngine2, out totalCount, out pageOffset);
					if (resultObjectType == QueueViewerServer.ResultObjectType.Legacy)
					{
						List<QueueInfo> list2 = new List<QueueInfo>();
						foreach (PagedDataObject pagedDataObject2 in array2)
						{
							list2.Add(new QueueInfo((ExtensibleQueueInfo)pagedDataObject2));
						}
						array = Serialization.ObjectToBytes(list2.ToArray());
					}
				}
				if (resultObjectType == QueueViewerServer.ResultObjectType.PagedCustom)
				{
					array = Serialization.PagedObjectToBytes(array2, targetVersion);
				}
				else if (resultObjectType == QueueViewerServer.ResultObjectType.PagedBinary)
				{
					array = Serialization.ObjectToBytes(array2);
				}
				result = array;
			}
			catch (QueueViewerException)
			{
				throw;
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
				result = null;
			}
			return result;
		}

		private T DeserializeObject<T>(byte[] serializedData)
		{
			if (serializedData == null)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
			}
			T result;
			try
			{
				result = (T)((object)Serialization.BytesToObject(serializedData));
			}
			catch (SerializationException)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_CLIENT_DATA);
			}
			return result;
		}

		private ExtensibleMessageInfo GetExtensibleMessageInfo(MessageInfo legacyMessageInfo)
		{
			MessageIdentity messageIdentity = (MessageIdentity)legacyMessageInfo.Identity;
			return new PropertyBagBasedMessageInfo(messageIdentity.InternalId, messageIdentity.QueueIdentity)
			{
				Subject = legacyMessageInfo.Subject,
				InternetMessageId = legacyMessageInfo.InternetMessageId,
				FromAddress = legacyMessageInfo.FromAddress,
				Status = legacyMessageInfo.Status,
				Size = legacyMessageInfo.Size,
				MessageSourceName = legacyMessageInfo.MessageSourceName,
				SourceIP = legacyMessageInfo.SourceIP,
				SCL = legacyMessageInfo.SCL,
				DateReceived = legacyMessageInfo.DateReceived,
				ExpirationTime = legacyMessageInfo.ExpirationTime,
				LastErrorCode = legacyMessageInfo.LastErrorCode,
				LastError = legacyMessageInfo.LastError,
				RetryCount = legacyMessageInfo.RetryCount,
				Recipients = legacyMessageInfo.Recipients,
				ComponentLatency = legacyMessageInfo.ComponentLatency,
				MessageLatency = legacyMessageInfo.MessageLatency
			};
		}

		private ExtensibleQueueInfo GetExtensibleQueueInfo(QueueInfo legacyQueueInfo)
		{
			return new PropertyBagBasedQueueInfo((QueueIdentity)legacyQueueInfo.Identity)
			{
				DeliveryType = legacyQueueInfo.DeliveryType,
				NextHopConnector = legacyQueueInfo.NextHopConnector,
				Status = legacyQueueInfo.Status,
				MessageCount = legacyQueueInfo.MessageCount,
				LastError = legacyQueueInfo.LastError,
				LastRetryTime = legacyQueueInfo.LastRetryTime,
				NextRetryTime = legacyQueueInfo.NextRetryTime
			};
		}

		private async System.Threading.Tasks.Task RetryQueueHelperAsync(IList<QueueIdentity> queueIdentities, bool resubmit, string parameterString)
		{
			Task<bool>[] retryTasks = new Task<bool>[queueIdentities.Count];
			Parallel.For(0, queueIdentities.Count, delegate(int i)
			{
				retryTasks[i] = QueueManager.RetryQueueAsync(queueIdentities[i], resubmit);
			});
			await System.Threading.Tasks.Task.WhenAll<bool>(retryTasks);
			int queuesRetried = retryTasks.Count((Task<bool> retryTask) => retryTask.Result);
			ExTraceGlobals.QueuingTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "Asynchronous Retry-Queue with '{0}', results with {1} queue(s) were retried out of {2}", parameterString, queuesRetried, queueIdentities.Count<QueueIdentity>());
		}

		private static readonly object RetryQueueInProgressSyncObject = new object();

		private System.Threading.Tasks.Task pendingRetryQueueTask;

		private enum ResultObjectType
		{
			Legacy,
			PagedBinary,
			PagedCustom
		}
	}
}
