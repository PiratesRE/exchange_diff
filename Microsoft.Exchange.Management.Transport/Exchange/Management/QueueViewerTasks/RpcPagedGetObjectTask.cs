using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.QueueViewer;
using Microsoft.Exchange.Transport.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	public abstract class RpcPagedGetObjectTask<ObjectType> : PagedGetObjectTask<ObjectType> where ObjectType : PagedDataObject
	{
		[Parameter(Mandatory = false, ParameterSetName = "Filter", ValueFromPipeline = true)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		protected SwitchParameter IncludeDetails
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeDetails"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeDetails"] = value;
			}
		}

		protected SwitchParameter IncludeLatencyInfo
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeLatencyInfo"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeLatencyInfo"] = value;
			}
		}

		protected Server TargetServer
		{
			get
			{
				return this.targetServer;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				TaskLogger.LogExit();
				return;
			}
			if (this.Server == null)
			{
				this.Server = ServerIdParameter.Parse("localhost");
			}
			this.ResolveTargetServer();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			ObjectType[] array = null;
			int totalCount = 0;
			int num = 0;
			try
			{
				int num2 = 3;
				QueryFilter queryFilter = DateTimeConverter.ConvertQueryFilter(this.innerFilter);
				while (num2-- > 0)
				{
					try
					{
						if (base.BookmarkObject != null)
						{
							ObjectType bookmarkObject = base.BookmarkObject;
							bookmarkObject.ConvertDatesToUniversalTime();
						}
						QueueViewerInputObject queueViewerInputObject = new QueueViewerInputObject((base.BookmarkIndex <= 0) ? -1 : (base.BookmarkIndex - 1), base.BookmarkObject, base.IncludeBookmark, this.IncludeLatencyInfo.IsPresent, this.IncludeDetails.IsPresent, base.ResultSize.IsUnlimited ? int.MaxValue : base.ResultSize.Value, queryFilter, base.SearchForward, base.SortOrder);
						using (QueueViewerClient<ObjectType> queueViewerClient = new QueueViewerClient<ObjectType>((string)this.Server))
						{
							if (!VersionedQueueViewerClient.UsePropertyBagBasedAPI(this.targetServer))
							{
								PagedDataObject bookmarkObject2 = null;
								if (typeof(ObjectType) == typeof(ExtensibleQueueInfo))
								{
									if (base.Filter != null)
									{
										this.innerFilter = new MonadFilter(base.Filter, this, ObjectSchema.GetInstance<QueueInfoSchema>()).InnerFilter;
									}
									this.InitializeInnerFilter<QueueInfo>(null, QueueInfoSchema.Identity);
									if (base.BookmarkObject != null)
									{
										bookmarkObject2 = new QueueInfo(base.BookmarkObject as ExtensibleQueueInfo);
									}
								}
								else
								{
									if (base.Filter != null)
									{
										this.innerFilter = new MonadFilter(base.Filter, this, ObjectSchema.GetInstance<MessageInfoSchema>()).InnerFilter;
									}
									this.InitializeInnerFilter<MessageInfo>(MessageInfoSchema.Identity, MessageInfoSchema.Queue);
									if (base.BookmarkObject != null)
									{
										bookmarkObject2 = new MessageInfo(base.BookmarkObject as ExtensibleMessageInfo);
									}
								}
								queueViewerInputObject.QueryFilter = this.innerFilter;
								List<SortOrderEntry> list = null;
								if (queueViewerInputObject.SortOrder != null)
								{
									list = new List<SortOrderEntry>();
									foreach (QueueViewerSortOrderEntry queueViewerSortOrderEntry in queueViewerInputObject.SortOrder)
									{
										list.Add(SortOrderEntry.Parse(queueViewerSortOrderEntry.ToString()));
									}
								}
								array = queueViewerClient.GetQueueViewerObjectPage(queueViewerInputObject.QueryFilter, (queueViewerInputObject.SortOrder != null) ? list.ToArray() : null, queueViewerInputObject.SearchForward, queueViewerInputObject.PageSize, bookmarkObject2, queueViewerInputObject.BookmarkIndex, queueViewerInputObject.IncludeBookmark, queueViewerInputObject.IncludeDetails, queueViewerInputObject.IncludeComponentLatencyInfo, ref totalCount, ref num);
							}
							else if (VersionedQueueViewerClient.UseCustomSerialization(this.targetServer))
							{
								array = queueViewerClient.GetPropertyBagBasedQueueViewerObjectPageCustomSerialization(queueViewerInputObject, ref totalCount, ref num);
							}
							else
							{
								array = queueViewerClient.GetPropertyBagBasedQueueViewerObjectPage(queueViewerInputObject, ref totalCount, ref num);
							}
							break;
						}
					}
					catch (RpcException ex)
					{
						if ((ex.ErrorCode != 1753 && ex.ErrorCode != 1727) || num2 == 0)
						{
							throw;
						}
					}
				}
				if (base.ReturnPageInfo)
				{
					base.WriteObject(new PagedPositionInfo(num + 1, totalCount));
				}
				DateTimeConverter.ConvertResultSet<ObjectType>(array);
				foreach (ObjectType dataObject in array)
				{
					base.WriteResult(dataObject);
				}
			}
			catch (ParsingNonFilterablePropertyException ex2)
			{
				base.WriteError(ex2, ErrorCategory.InvalidArgument, ex2.PropertyName);
			}
			catch (QueueViewerException ex3)
			{
				base.WriteError(ErrorMapper.GetLocalizedException(ex3.ErrorCode, null, this.Server), ErrorCategory.InvalidOperation, null);
			}
			catch (RpcException ex4)
			{
				base.WriteError(ErrorMapper.GetLocalizedException(ex4.ErrorCode, null, this.Server), ErrorCategory.InvalidOperation, null);
			}
		}

		internal abstract void InitializeInnerFilter<Object>(QueueViewerPropertyDefinition<Object> messageIdentity, QueueViewerPropertyDefinition<Object> queueIdentity) where Object : PagedDataObject;

		private void ResolveTargetServer()
		{
			try
			{
				this.targetServer = VersionedQueueViewerClient.GetServer((string)this.Server);
			}
			catch (QueueViewerException)
			{
				base.WriteError(new LocalizedException(Strings.ErrorServerNotFound((string)this.Server)), ErrorCategory.InvalidArgument, null);
			}
		}

		private Server targetServer;
	}
}
