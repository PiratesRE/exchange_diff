using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data.DataProvider;
using Microsoft.Exchange.Hygiene.Data.Reporting;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal sealed class MessageTraceSession : HygieneSession, IMessageTraceQuery, IMessageTraceSession
	{
		public MessageTraceSession()
		{
			this.DataProvider = ConfigDataProviderFactory.Default.Create(DatabaseType.Mtrt);
		}

		public int GetNumberOfPersistentCopiesPerPartition(int physicalInstanceId)
		{
			IPartitionedDataProvider partitionedDataProvider = this.DataProvider as IPartitionedDataProvider;
			if (partitionedDataProvider == null)
			{
				throw new NotSupportedException(string.Format("Not supported for DataProvider type: {0}", this.DataProvider.GetType()));
			}
			return partitionedDataProvider.GetNumberOfPersistentCopiesPerPartition(physicalInstanceId);
		}

		public int GetNumberOfPhysicalPartitions()
		{
			IPartitionedDataProvider partitionedDataProvider = this.DataProvider as IPartitionedDataProvider;
			if (partitionedDataProvider == null)
			{
				throw new NotSupportedException(string.Format("Not supported for DataProvider type: {0}", this.DataProvider.GetType()));
			}
			return partitionedDataProvider.GetNumberOfPhysicalPartitions();
		}

		public object GetPartitionId(string hashKey)
		{
			IHashBucket hashBucket = this.DataProvider as IHashBucket;
			if (hashBucket == null)
			{
				throw new NotSupportedException(string.Format("Not supported for DataProvider type: {0}", this.DataProvider.GetType()));
			}
			int logicalHash = hashBucket.GetLogicalHash(hashKey);
			return hashBucket.GetPhysicalInstanceIdByHashValue(logicalHash);
		}

		public Dictionary<int, bool[]> GetStatusOfAllPhysicalPartitionCopies()
		{
			IPartitionedDataProvider partitionedDataProvider = this.DataProvider as IPartitionedDataProvider;
			if (partitionedDataProvider == null)
			{
				throw new NotSupportedException(string.Format("Not supported for DataProvider type: {0}", this.DataProvider.GetType()));
			}
			return partitionedDataProvider.GetStatusOfAllPhysicalPartitionCopies();
		}

		public void Save(RawLogBatch rawLogBatch)
		{
			if (rawLogBatch == null)
			{
				throw new ArgumentNullException("rawLogBatch");
			}
			this.DataProvider.Save(rawLogBatch);
		}

		public void Save(MessageTraceBatch messageTraceBatch)
		{
			if (messageTraceBatch == null)
			{
				throw new ArgumentNullException("messageTraceBatch");
			}
			IHashBucket hashBucket = this.DataProvider as IHashBucket;
			if (hashBucket != null)
			{
				Dictionary<object, List<MessageTrace>> dictionary = DalHelper.SplitByPhysicalInstance<MessageTrace>(hashBucket, MessageTraceSchema.OrganizationalUnitRootProperty, messageTraceBatch.ToList<MessageTrace>(), CommonMessageTraceSchema.HashBucketProperty);
				foreach (object obj in dictionary.Keys)
				{
					dictionary[obj].Sort();
					IEnumerable<List<MessageTrace>> optimizedBathesForSave = this.GetOptimizedBathesForSave(dictionary[obj]);
					foreach (List<MessageTrace> messageList in optimizedBathesForSave)
					{
						MessageTraceForSaveDataSet instance = MessageTraceForSaveDataSet.CreateDataSet(obj, null, messageList, messageTraceBatch.PersistentStoreCopyId);
						this.DataProvider.Save(instance);
					}
				}
				return;
			}
			if (messageTraceBatch.OrganizationalUnitRoot == null)
			{
				throw new ArgumentException("MessageTraceBatch.OrganizationalUnitRoot should have valid TenantId");
			}
			foreach (MessageTrace messageTrace in messageTraceBatch)
			{
				messageTrace[CommonMessageTraceSchema.HashBucketProperty] = 1;
			}
			MessageTraceForSaveDataSet instance2 = MessageTraceForSaveDataSet.CreateDataSet(null, messageTraceBatch.OrganizationalUnitRoot, messageTraceBatch, null);
			this.DataProvider.Save(instance2);
		}

		public bool Save(UserAddressBatch userAddresses, bool ignorePartialDbFailures = false)
		{
			bool result = true;
			object obj = userAddresses[UserAddressBatchSchema.FssCopyIdProp];
			if (ignorePartialDbFailures && obj == null)
			{
				result = this.SaveAllCopies<UserAddressBatch>(userAddresses, ((Guid)userAddresses[UserAddressBatchSchema.OrganizationalUnitRootProperty]).ToString());
			}
			else
			{
				this.DataProvider.Save(userAddresses);
			}
			return result;
		}

		public bool Save(QuarantinedMessageRecipientBatch recipientBatch, bool ignorePartialDbFailures = false)
		{
			bool result = true;
			object obj = recipientBatch[QuarantinedMessageRecipientBatchSchema.FssCopyIdProp];
			if (ignorePartialDbFailures && obj == null)
			{
				result = this.SaveAllCopies<QuarantinedMessageRecipientBatch>(recipientBatch, ((Guid)recipientBatch[QuarantinedMessageRecipientBatchSchema.OrganizationalUnitRootProperty]).ToString());
			}
			else
			{
				this.DataProvider.Save(recipientBatch);
			}
			return result;
		}

		public bool Save(QuarantinedMessageRecipient quarantinedMessageRecipient, bool ignorePartialDbFailures = false)
		{
			bool result = true;
			object obj = quarantinedMessageRecipient[QuarantinedMessageRecipientSchema.FssCopyIdProp];
			if (ignorePartialDbFailures && obj == null)
			{
				result = this.SaveAllCopies<QuarantinedMessageRecipient>(quarantinedMessageRecipient, ((Guid)quarantinedMessageRecipient[QuarantinedMessageRecipientSchema.OrganizationalUnitRootProperty]).ToString());
			}
			else
			{
				this.DataProvider.Save(quarantinedMessageRecipient);
			}
			return result;
		}

		public void Save(UnifiedPolicyTraceBatch traceBatch)
		{
			if (traceBatch == null)
			{
				throw new ArgumentNullException("traceBatch");
			}
			IHashBucket hashBucket = this.DataProvider as IHashBucket;
			if (hashBucket != null)
			{
				Dictionary<object, List<UnifiedPolicyTrace>> dictionary = DalHelper.SplitByPhysicalInstance<UnifiedPolicyTrace>(hashBucket, UnifiedPolicyCommonSchema.OrganizationalUnitRootProperty, traceBatch.ToList<UnifiedPolicyTrace>(), UnifiedPolicyCommonSchema.HashBucketProperty);
				foreach (object obj in dictionary.Keys)
				{
					dictionary[obj].Sort();
					UnifiedPolicyForSaveDataSet instance = UnifiedPolicyForSaveDataSet.CreateDataSet(obj, dictionary[obj], traceBatch.PersistentStoreCopyId);
					this.DataProvider.Save(instance);
				}
			}
		}

		public void SaveTenantSetting<T>(T instance, DateTime? whenChangedAtSourceUTC) where T : ADObject, new()
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (whenChangedAtSourceUTC != null)
			{
				MessageTraceTenantSettingFacade<T> messageTraceTenantSettingFacade = new MessageTraceTenantSettingFacade<T>(instance);
				messageTraceTenantSettingFacade[MessageTraceTenantSettingFacade<T>.WhenChangedAtSourceProp] = whenChangedAtSourceUTC.Value;
			}
			this.DataProvider.Save(instance);
		}

		public void Delete(IConfigurable configurable)
		{
			if (configurable == null)
			{
				throw new ArgumentNullException("configurable");
			}
			this.DataProvider.Delete(configurable);
		}

		public T Find<T>(ADObjectId organizationalUnitRoot, ADObjectId id) where T : ADObject, new()
		{
			if (organizationalUnitRoot == null)
			{
				throw new ArgumentNullException("organizationalUnitRoot");
			}
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, organizationalUnitRoot.ObjectGuid),
				new ComparisonFilter(ComparisonOperator.Equal, DalHelper.SettingTypeProp, typeof(T).Name),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, id.ObjectGuid)
			});
			return (T)((object)this.DataProvider.Find<T>(filter, null, false, null).FirstOrDefault<IConfigurable>());
		}

		public IEnumerable<AggTrafficData> FindTenantSpamDigestHistory(Guid organizationalUnitRootId, DateTime startDatetime, DateTime endDatetime, int pageSize = 1000)
		{
			int num = Convert.ToInt32(startDatetime.ToString("yyyyMMdd"));
			int num2 = Convert.ToInt32(endDatetime.ToString("yyyyMMdd"));
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, CommonReportingSchema.OrganizationalUnitRootProperty, organizationalUnitRootId),
				new ComparisonFilter(ComparisonOperator.Equal, CommonReportingSchema.StartDateKeyProperty, num),
				new ComparisonFilter(ComparisonOperator.Equal, CommonReportingSchema.EndDateKeyProperty, num2),
				new ComparisonFilter(ComparisonOperator.Equal, CommonReportingSchema.TrafficTypeProperty, "SpamDigestMail"),
				new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PageSizeProp, pageSize)
			});
			return this.DataProvider.Find<AggTrafficData>(filter, null, false, null).Cast<AggTrafficData>().Cache<AggTrafficData>();
		}

		public IPagedReader<EsnTenant> FindPagedEsnTenants(int partitionNumber, DateTime upperBoundary, int pageSize = 100)
		{
			object[] allPhysicalPartitions = ((IPartitionedDataProvider)this.DataProvider).GetAllPhysicalPartitions();
			if (partitionNumber < 0 || partitionNumber >= allPhysicalPartitions.Length)
			{
				return null;
			}
			return new ConfigDataProviderPagedReader<EsnTenant>(this.DataProvider, null, QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PhysicalInstanceKeyProp, allPhysicalPartitions[partitionNumber]),
				new ComparisonFilter(ComparisonOperator.Equal, EsnTenantSchema.UpperBoundaryQueryProperty, upperBoundary)
			}), null, pageSize);
		}

		public IEnumerable<UnsentSpamDigestMessage> FindPagedUnsentDigestMessages(Guid organizationalUnitRoot, DateTime upperBoundary, ref string pageCookie, out bool complete, int pageSize = 1000)
		{
			QueryFilter pagingQueryFilter = PagingHelper.GetPagingQueryFilter(QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, UnsentSpamDigestMessageSchema.OrganizationalUnitRootProperty, organizationalUnitRoot),
				new ComparisonFilter(ComparisonOperator.Equal, UnsentSpamDigestMessageSchema.UpperBoundaryQueryProperty, upperBoundary)
			}), pageCookie);
			IEnumerable<UnsentSpamDigestMessage> result = this.DataProvider.FindPaged<UnsentSpamDigestMessage>(pagingQueryFilter, null, false, null, pageSize).Cast<UnsentSpamDigestMessage>().Cache<UnsentSpamDigestMessage>();
			pageCookie = PagingHelper.GetProcessedCookie(pagingQueryFilter, out complete);
			return result;
		}

		public IPagedReader<UnsentSpamDigestMessage> FindPagedUnsentDigestMessages(DateTime upperBoundary, int pageSize = 0, int defaultESNFrequency = 3)
		{
			List<IPagedReader<UnsentSpamDigestMessage>> list = new List<IPagedReader<UnsentSpamDigestMessage>>();
			foreach (object propertyValue in ((IPartitionedDataProvider)this.DataProvider).GetAllPhysicalPartitions())
			{
				list.Add(new ConfigDataProviderPagedReader<UnsentSpamDigestMessage>(this.DataProvider, null, QueryFilter.AndTogether(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PhysicalInstanceKeyProp, propertyValue),
					new ComparisonFilter(ComparisonOperator.Equal, UnsentSpamDigestMessageSchema.UpperBoundaryQueryProperty, upperBoundary),
					new ComparisonFilter(ComparisonOperator.Equal, UnsentSpamDigestMessageSchema.DefaultESNFrequencyQueryProperty, defaultESNFrequency)
				}), null, pageSize));
			}
			return new CompositePagedReader<UnsentSpamDigestMessage>(list.ToArray());
		}

		public IPagedReader<UnsentSpamDigestMessage> FindPagedUnsentDigestMessages(int partitionNumber, DateTime upperBoundary, int pageSize = 0, int defaultESNFrequency = 3)
		{
			object[] allPhysicalPartitions = ((IPartitionedDataProvider)this.DataProvider).GetAllPhysicalPartitions();
			if (partitionNumber < 0 || partitionNumber >= allPhysicalPartitions.Length)
			{
				return null;
			}
			return new ConfigDataProviderPagedReader<UnsentSpamDigestMessage>(this.DataProvider, null, QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PhysicalInstanceKeyProp, allPhysicalPartitions[partitionNumber]),
				new ComparisonFilter(ComparisonOperator.Equal, UnsentSpamDigestMessageSchema.UpperBoundaryQueryProperty, upperBoundary),
				new ComparisonFilter(ComparisonOperator.Equal, UnsentSpamDigestMessageSchema.DefaultESNFrequencyQueryProperty, defaultESNFrequency)
			}), null, pageSize);
		}

		public MessageTrace Read(Guid organizationalUnitRoot, Guid exMessageId)
		{
			if (exMessageId == Guid.Empty)
			{
				throw new ArgumentNullException("exMessageId");
			}
			MessageTraceDataSet messageTraceDataSet = (MessageTraceDataSet)this.DataProvider.Find<MessageTraceDataSet>(MessageTraceSession.BuildQueryFilter(organizationalUnitRoot, exMessageId), null, false, null).FirstOrDefault<IConfigurable>();
			if (messageTraceDataSet == null)
			{
				return null;
			}
			return messageTraceDataSet.ConvertToMessageTraceObject();
		}

		public IEnumerable<QuarantinedMessageDetail> GetQuarantinedMessageDetails(Guid organizationalUnitRoot, Guid exMessageId, Guid? eventId = null, string recipientAddress = null)
		{
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				MessageTraceSession.BuildQueryFilter(organizationalUnitRoot, exMessageId),
				new ComparisonFilter(ComparisonOperator.Equal, QuarantinedMessageCommonSchema.EventIdProperty, eventId),
				new ComparisonFilter(ComparisonOperator.Equal, QuarantinedMessageCommonSchema.RecipientAddressProperty, recipientAddress)
			});
			IConfigurable[] source = this.DataProvider.Find<QuarantinedMessageDetail>(filter, null, false, null);
			return source.Cast<QuarantinedMessageDetail>();
		}

		public IEnumerable<TReportObject> FindReportObject<TReportObject>(QueryFilter filter) where TReportObject : IConfigurable, new()
		{
			IConfigurable[] source = this.DataProvider.Find<TReportObject>(filter, null, false, null);
			return source.Cast<TReportObject>();
		}

		public MessageTrace[] FindPagedTrace(Guid organizationalUnitRoot, DateTime start, DateTime end, string fromEmailPrefix = null, string fromEmailDomain = null, string toEmailPrefix = null, string toEmailDomain = null, string clientMessageId = null, int rowIndex = 0, int rowCount = -1)
		{
			QueryFilter filter = MessageTraceSession.BuildQueryFilter(organizationalUnitRoot, start, end, fromEmailDomain, fromEmailPrefix, toEmailDomain, toEmailPrefix, clientMessageId);
			IConfigurable[] array = this.DataProvider.Find<MessageTrace>(filter, null, false, null);
			MessageTrace[] array2 = new MessageTrace[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = (MessageTrace)array[i];
			}
			return array2;
		}

		internal static QueryFilter BuildQueryFilter(Guid organizationalUnitRoot, DateTime startTime, DateTime endTime, string fromEmailDomain = null, string fromEmailPrefix = null, string toEmailDomain = null, string toEmailPrefix = null, string clientMessageId = null)
		{
			List<QueryFilter> list = new List<QueryFilter>(8);
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, MessageTraceSchema.OrganizationalUnitRootProperty, organizationalUnitRoot));
			list.Add(new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, MessageTraceSchema.StartTimeQueryProperty, startTime));
			list.Add(new ComparisonFilter(ComparisonOperator.LessThan, MessageTraceSchema.EndTimeQueryProperty, endTime));
			if (fromEmailPrefix != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, MessageTraceSchema.FromEmailPrefixProperty, fromEmailPrefix));
			}
			if (fromEmailDomain != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, MessageTraceSchema.FromEmailDomainProperty, fromEmailDomain));
			}
			if (toEmailPrefix != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, MessageRecipientSchema.ToEmailPrefixProperty, toEmailPrefix));
			}
			if (toEmailDomain != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, MessageRecipientSchema.ToEmailDomainProperty, toEmailDomain));
			}
			if (clientMessageId != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, MessageTraceSchema.ClientMessageIdProperty, clientMessageId));
			}
			return new AndFilter(list.ToArray());
		}

		internal static QueryFilter BuildQueryFilter(Guid organizationalUnitRoot, Guid exMessageId)
		{
			return QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, MessageTraceSchema.OrganizationalUnitRootProperty, organizationalUnitRoot),
				new ComparisonFilter(ComparisonOperator.Equal, MessageTraceSchema.ExMessageIdProperty, exMessageId)
			});
		}

		internal IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			return this.DataProvider.Find<T>(filter, rootId, deepSearch, sortBy);
		}

		internal void Save(IEnumerable<AggTrafficData> aggTrafficDataBatch, int? persistentStoreCopyId = null)
		{
			if (aggTrafficDataBatch == null)
			{
				throw new ArgumentNullException("aggTrafficDataBatch");
			}
			IHashBucket hashBucket = this.DataProvider as IHashBucket;
			if (hashBucket == null)
			{
				throw new NotSupportedException(string.Format("Save(AggTrafficData) is not supported for DataProvider: {0}", this.DataProvider.GetType()));
			}
			foreach (AggTrafficData aggTrafficData in aggTrafficDataBatch)
			{
				aggTrafficData[CommonReportingSchema.DomainHashKeyProp] = (string.IsNullOrWhiteSpace(aggTrafficData.TenantDomain) ? MessageTraceEntityBase.EmptyEmailDomainHashKey : DalHelper.GetSHA1Hash(aggTrafficData.TenantDomain));
			}
			Dictionary<object, List<AggTrafficData>> dictionary = DalHelper.SplitByPhysicalInstance<AggTrafficData>(hashBucket, CommonReportingSchema.OrganizationalUnitRootProperty, aggTrafficDataBatch, DalHelper.HashBucketProp);
			foreach (object obj in dictionary.Keys)
			{
				AggTrafficDatas aggTrafficDatas = new AggTrafficDatas(dictionary[obj]);
				aggTrafficDatas[DalHelper.PhysicalInstanceKeyProp] = obj;
				if (persistentStoreCopyId != null)
				{
					aggTrafficDatas[DalHelper.FssCopyIdProp] = persistentStoreCopyId;
				}
				this.DataProvider.Save(aggTrafficDatas);
			}
		}

		internal void Save(IEnumerable<AggTopTrafficData> aggTopTrafficDataBatch, int? persistentStoreCopyId = null)
		{
			if (aggTopTrafficDataBatch == null)
			{
				throw new ArgumentNullException("aggTopTrafficDataBatch");
			}
			IHashBucket hashBucket = this.DataProvider as IHashBucket;
			if (hashBucket == null)
			{
				throw new NotSupportedException(string.Format("Save(AggTopTrafficData) is not supported for DataProvider: {0}", this.DataProvider.GetType()));
			}
			foreach (AggTopTrafficData aggTopTrafficData in aggTopTrafficDataBatch)
			{
				aggTopTrafficData[CommonReportingSchema.DomainHashKeyProp] = new byte[0];
			}
			Dictionary<object, List<AggTopTrafficData>> dictionary = DalHelper.SplitByPhysicalInstance<AggTopTrafficData>(hashBucket, CommonReportingSchema.OrganizationalUnitRootProperty, aggTopTrafficDataBatch, DalHelper.HashBucketProp);
			foreach (object obj in dictionary.Keys)
			{
				AggTopTrafficDatas aggTopTrafficDatas = new AggTopTrafficDatas(dictionary[obj]);
				aggTopTrafficDatas[DalHelper.PhysicalInstanceKeyProp] = obj;
				if (persistentStoreCopyId != null)
				{
					aggTopTrafficDatas[DalHelper.FssCopyIdProp] = persistentStoreCopyId;
				}
				this.DataProvider.Save(aggTopTrafficDatas);
			}
		}

		internal void Save(IEnumerable<AggPolicyTrafficData> aggPolicyTrafficDataBatch, int? persistentStoreCopyId = null)
		{
			if (aggPolicyTrafficDataBatch == null)
			{
				throw new ArgumentNullException("aggPolicyTrafficDataBatch");
			}
			IHashBucket hashBucket = this.DataProvider as IHashBucket;
			if (hashBucket == null)
			{
				throw new NotSupportedException(string.Format("Save(AggPolicyTrafficData) is not supported for DataProvider: {0}", this.DataProvider.GetType()));
			}
			foreach (AggPolicyTrafficData aggPolicyTrafficData in aggPolicyTrafficDataBatch)
			{
				aggPolicyTrafficData[CommonReportingSchema.DomainHashKeyProp] = new byte[0];
				aggPolicyTrafficData[CommonReportingSchema.DataSourceProperty] = (aggPolicyTrafficData.DataSource ?? "EXO");
			}
			Dictionary<object, List<AggPolicyTrafficData>> dictionary = DalHelper.SplitByPhysicalInstance<AggPolicyTrafficData>(hashBucket, CommonReportingSchema.OrganizationalUnitRootProperty, aggPolicyTrafficDataBatch, DalHelper.HashBucketProp);
			foreach (object obj in dictionary.Keys)
			{
				AggPolicyTrafficDatas aggPolicyTrafficDatas = new AggPolicyTrafficDatas(dictionary[obj]);
				aggPolicyTrafficDatas[DalHelper.PhysicalInstanceKeyProp] = obj;
				if (persistentStoreCopyId != null)
				{
					aggPolicyTrafficDatas[DalHelper.FssCopyIdProp] = persistentStoreCopyId;
				}
				this.DataProvider.Save(aggPolicyTrafficDatas);
			}
		}

		public void Save(IEnumerable<MessageTrafficTypeMapping> messageTrafficTypeMappingBatch, int? persistentStoreCopyId = null)
		{
			if (messageTrafficTypeMappingBatch == null)
			{
				throw new ArgumentNullException("messageTrafficTypeMappingBatch");
			}
			IHashBucket hashBucket = this.DataProvider as IHashBucket;
			if (hashBucket == null)
			{
				throw new NotSupportedException(string.Format("Save(MessageTrafficTypeMapping) is not supported for DataProvider: {0}", this.DataProvider.GetType()));
			}
			foreach (MessageTrafficTypeMapping messageTrafficTypeMapping in messageTrafficTypeMappingBatch)
			{
				messageTrafficTypeMapping[CommonReportingSchema.DomainHashKeyProp] = new byte[0];
				messageTrafficTypeMapping[CommonReportingSchema.DataSourceProperty] = (messageTrafficTypeMapping.DataSource ?? "EXO");
			}
			Dictionary<object, List<MessageTrafficTypeMapping>> dictionary = DalHelper.SplitByPhysicalInstance<MessageTrafficTypeMapping>(hashBucket, CommonReportingSchema.OrganizationalUnitRootProperty, messageTrafficTypeMappingBatch, DalHelper.HashBucketProp);
			foreach (object obj in dictionary.Keys)
			{
				MessageTrafficTypeMappings messageTrafficTypeMappings = new MessageTrafficTypeMappings(dictionary[obj]);
				messageTrafficTypeMappings[DalHelper.PhysicalInstanceKeyProp] = obj;
				if (persistentStoreCopyId != null)
				{
					messageTrafficTypeMappings[DalHelper.FssCopyIdProp] = persistentStoreCopyId;
				}
				this.DataProvider.Save(messageTrafficTypeMappings);
			}
		}

		internal void Save(IEnumerable<DeviceData> deviceDataBatch, int? persistentStoreCopyId = null)
		{
			if (deviceDataBatch == null)
			{
				throw new ArgumentNullException("deviceDataBatch");
			}
			IHashBucket hashBucket = this.DataProvider as IHashBucket;
			if (hashBucket == null)
			{
				foreach (DeviceData deviceData in deviceDataBatch)
				{
					deviceData[DeviceCommonSchema.HashBucketProperty] = 1;
				}
				using (IEnumerator<DeviceData> enumerator2 = deviceDataBatch.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						DeviceData instance = enumerator2.Current;
						this.DataProvider.Save(instance);
					}
					return;
				}
			}
			Dictionary<object, List<DeviceData>> dictionary = DalHelper.SplitByPhysicalInstance<DeviceData>(hashBucket, DeviceCommonSchema.OrganizationalUnitRootProperty, deviceDataBatch, DeviceCommonSchema.HashBucketProperty);
			foreach (object obj in dictionary.Keys)
			{
				DeviceDataBatch deviceDataBatch2 = new DeviceDataBatch(dictionary[obj]);
				deviceDataBatch2[DalHelper.PhysicalInstanceKeyProp] = obj;
				if (persistentStoreCopyId != null)
				{
					deviceDataBatch2[DalHelper.FssCopyIdProp] = persistentStoreCopyId;
				}
				this.DataProvider.Save(deviceDataBatch2);
			}
		}

		public void Save(IConfigurable obj)
		{
			this.DataProvider.Save(obj);
		}

		private bool SaveAllCopies<T>(T obj, string partitionKey) where T : IConfigurable, IPropertyBag
		{
			bool result = true;
			List<TransientDALException> list = null;
			if (partitionKey == null)
			{
				throw new ArgumentNullException("partitionKey");
			}
			int physicalInstanceId = (int)this.GetPartitionId(partitionKey);
			int physicalPartitionCopyCount = this.GetPhysicalPartitionCopyCount(physicalInstanceId);
			for (int i = 0; i < physicalPartitionCopyCount; i++)
			{
				try
				{
					obj[DalHelper.FssCopyIdProp] = i;
					this.DataProvider.Save(obj);
				}
				catch (TransientDALException item)
				{
					result = false;
					if (list == null)
					{
						list = new List<TransientDALException>();
					}
					list.Add(item);
				}
			}
			if (list != null && list.Count == physicalPartitionCopyCount)
			{
				throw new AggregateException(list.ToArray());
			}
			return result;
		}

		private int GetPhysicalPartitionCopyCount(int physicalInstanceId)
		{
			IPartitionedDataProvider partitionedDataProvider = this.DataProvider as IPartitionedDataProvider;
			if (partitionedDataProvider == null)
			{
				throw new NotSupportedException("GetPhysicalPartitionCopyCount may not be called from an environment that does not use a partitioned data provider.");
			}
			return partitionedDataProvider.GetNumberOfPersistentCopiesPerPartition(physicalInstanceId);
		}

		private IEnumerable<List<MessageTrace>> GetOptimizedBathesForSave(List<MessageTrace> mesageList)
		{
			List<MessageTrace> defaultList = new List<MessageTrace>();
			foreach (MessageTrace msg in mesageList)
			{
				if (msg.Recipients.Count > SessionConfiguration.Instance.PerMessageRecipientSaveThreshold)
				{
					if (msg.Recipients.Count > SessionConfiguration.Instance.PerMessageRecipientSplitSaveThreshold)
					{
						foreach (MessageTrace splitMessage in this.SplitMessageForOptimizedSave(msg, SessionConfiguration.Instance.PerMessageRecipientSplitSaveThreshold))
						{
							yield return new List<MessageTrace>
							{
								splitMessage
							};
						}
					}
					else
					{
						yield return new List<MessageTrace>
						{
							msg
						};
					}
				}
				else
				{
					defaultList.Add(msg);
				}
			}
			yield return defaultList;
			yield break;
		}

		private IEnumerable<MessageTrace> SplitMessageForOptimizedSave(MessageTrace sourceMessage, int recipientSplitThreshold)
		{
			Dictionary<Guid, Dictionary<Guid, MessageRecipientStatus>> eventRecipientStatusMap = this.GetMessageEventRecipientStatusMap(sourceMessage);
			List<MessageRecipient> recipientList = new List<MessageRecipient>();
			foreach (MessageRecipient recipient in sourceMessage.Recipients)
			{
				recipientList.Add(recipient);
				if (recipientList.Count == recipientSplitThreshold)
				{
					yield return this.GetSplitMessage(sourceMessage, recipientList, eventRecipientStatusMap);
					recipientList.Clear();
				}
			}
			if (recipientList.Count > 0)
			{
				yield return this.GetSplitMessage(sourceMessage, recipientList, eventRecipientStatusMap);
			}
			yield break;
		}

		private MessageTrace GetSplitMessage(MessageTrace sourceMessage, List<MessageRecipient> recipientList, Dictionary<Guid, Dictionary<Guid, MessageRecipientStatus>> eventRecipientStatusMap)
		{
			MessageTrace splitMessage = new MessageTrace();
			splitMessage.ExMessageId = sourceMessage.ExMessageId;
			splitMessage.ClientMessageId = sourceMessage.ClientMessageId;
			splitMessage.OrganizationalUnitRoot = sourceMessage.OrganizationalUnitRoot;
			splitMessage.Direction = sourceMessage.Direction;
			splitMessage.FromEmailPrefix = sourceMessage.FromEmailPrefix;
			splitMessage.FromEmailDomain = sourceMessage.FromEmailDomain;
			splitMessage.IPAddress = sourceMessage.IPAddress;
			sourceMessage.GetExtendedPropertiesEnumerable().ToList<MessageProperty>().ForEach(delegate(MessageProperty r)
			{
				splitMessage.AddExtendedProperty(r);
			});
			sourceMessage.Events.ForEach(delegate(MessageEvent r)
			{
				splitMessage.Add(this.GetSplitMessageEvent(r, recipientList, eventRecipientStatusMap[r.EventId]));
			});
			sourceMessage.Classifications.ForEach(delegate(MessageClassification r)
			{
				splitMessage.Add(r);
			});
			sourceMessage.ClientInformation.ForEach(delegate(MessageClientInformation r)
			{
				splitMessage.Add(r);
			});
			recipientList.ForEach(delegate(MessageRecipient r)
			{
				splitMessage.Add(r);
			});
			return splitMessage;
		}

		private MessageEvent GetSplitMessageEvent(MessageEvent sourceEvent, List<MessageRecipient> recipientList, Dictionary<Guid, MessageRecipientStatus> recipientStatusMap)
		{
			MessageEvent splitEvent = new MessageEvent();
			splitEvent.ExMessageId = sourceEvent.ExMessageId;
			splitEvent.EventId = sourceEvent.EventId;
			splitEvent.TimeStamp = sourceEvent.TimeStamp;
			splitEvent.EventType = sourceEvent.EventType;
			splitEvent.EventSource = sourceEvent.EventSource;
			sourceEvent.GetExtendedPropertiesEnumerable().ToList<MessageEventProperty>().ForEach(delegate(MessageEventProperty r)
			{
				splitEvent.AddExtendedProperty(r);
			});
			sourceEvent.Rules.ForEach(delegate(MessageEventRule r)
			{
				splitEvent.Add(r);
			});
			sourceEvent.SourceItems.ForEach(delegate(MessageEventSourceItem r)
			{
				splitEvent.Add(r);
			});
			foreach (MessageRecipient messageRecipient in recipientList)
			{
				if (recipientStatusMap.ContainsKey(messageRecipient.RecipientId))
				{
					splitEvent.Add(recipientStatusMap[messageRecipient.RecipientId]);
				}
			}
			return splitEvent;
		}

		private Dictionary<Guid, Dictionary<Guid, MessageRecipientStatus>> GetMessageEventRecipientStatusMap(MessageTrace message)
		{
			Dictionary<Guid, Dictionary<Guid, MessageRecipientStatus>> dictionary = new Dictionary<Guid, Dictionary<Guid, MessageRecipientStatus>>();
			foreach (MessageEvent messageEvent in message.Events)
			{
				Dictionary<Guid, MessageRecipientStatus> statusDict = new Dictionary<Guid, MessageRecipientStatus>();
				messageEvent.Statuses.ForEach(delegate(MessageRecipientStatus r)
				{
					statusDict[r.RecipientId] = r;
				});
				dictionary[messageEvent.EventId] = statusDict;
			}
			return dictionary;
		}

		private const string spamDigestMailTrafficType = "SpamDigestMail";

		internal readonly IConfigDataProvider DataProvider;
	}
}
