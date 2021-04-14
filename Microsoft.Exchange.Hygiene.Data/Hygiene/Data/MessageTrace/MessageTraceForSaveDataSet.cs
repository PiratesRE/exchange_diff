using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal sealed class MessageTraceForSaveDataSet : ConfigurablePropertyBag, IValidateable
	{
		static MessageTraceForSaveDataSet()
		{
			MessageTraceForSaveDataSet.mapTableToTvpColumnInfo = new Dictionary<HygienePropertyDefinition, HygienePropertyDefinition[]>();
			foreach (TvpInfo tvpInfo in MessageTraceForSaveDataSet.tvpPrototypeList)
			{
				MessageTraceForSaveDataSet.mapTableToTvpColumnInfo.Add(tvpInfo.TableName, tvpInfo.Columns);
			}
			MessageTraceForSaveDataSet.mappingInfoList = new List<MessageTraceForSaveDataSet.MappingInfo>();
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageRecipientsTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageRecipientPropertiesTableProperty,
				ParentKeyProperty = MessageRecipientSchema.RecipientIdProperty,
				ChildKeyProperty = PropertyBase.ParentIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EmailHashKeyProperty, CommonMessageTraceSchema.EmailHashKeyProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageRecipientsTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageRecipientStatusTableProperty,
				ParentKeyProperty = MessageRecipientSchema.RecipientIdProperty,
				ChildKeyProperty = MessageRecipientStatusSchema.RecipientIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EmailHashKeyProperty, CommonMessageTraceSchema.EmailHashKeyProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageEventsTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageEventPropertiesTableProperty,
				ParentKeyProperty = MessageEventSchema.EventIdProperty,
				ChildKeyProperty = PropertyBase.ParentIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EventHashKeyProperty, CommonMessageTraceSchema.EventHashKeyProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageEventsTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageEventRulesTableProperty,
				ParentKeyProperty = MessageEventSchema.EventIdProperty,
				ChildKeyProperty = MessageEventRuleSchema.EventIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EventHashKeyProperty, CommonMessageTraceSchema.EventHashKeyProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageEventsTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageEventSourceItemsTableProperty,
				ParentKeyProperty = MessageEventSchema.EventIdProperty,
				ChildKeyProperty = MessageEventSourceItemSchema.EventIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EventHashKeyProperty, CommonMessageTraceSchema.EventHashKeyProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageEventsTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageRecipientStatusTableProperty,
				ParentKeyProperty = MessageEventSchema.EventIdProperty,
				ChildKeyProperty = MessageRecipientStatusSchema.EventIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EventHashKeyProperty, CommonMessageTraceSchema.EventHashKeyProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageEventRulesTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageActionTableProperty,
				ParentKeyProperty = MessageEventRuleSchema.EventRuleIdProperty,
				ChildKeyProperty = MessageActionSchema.EventRuleIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(MessageEventRuleSchema.RuleIdProperty, CommonMessageTraceSchema.RuleIdProperty),
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EventHashKeyProperty, CommonMessageTraceSchema.EventHashKeyProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageEventRulesTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageEventRulePropertiesTableProperty,
				ParentKeyProperty = MessageEventRuleSchema.EventRuleIdProperty,
				ChildKeyProperty = PropertyBase.ParentIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(MessageEventRuleSchema.RuleIdProperty, PropertyBase.ParentObjectIdProperty),
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EventHashKeyProperty, CommonMessageTraceSchema.EventHashKeyProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageEventRulesTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageEventRuleClassificationsTableProperty,
				ParentKeyProperty = MessageEventRuleSchema.EventRuleIdProperty,
				ChildKeyProperty = MessageEventRuleClassificationSchema.EventRuleIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.RuleIdProperty, CommonMessageTraceSchema.RuleIdProperty),
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EventHashKeyProperty, CommonMessageTraceSchema.EventHashKeyProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageEventRuleClassificationsTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageEventRuleClassificationPropertiesTableProperty,
				ParentKeyProperty = MessageEventRuleClassificationSchema.EventRuleClassificationIdProperty,
				ChildKeyProperty = PropertyBase.ParentIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.RuleIdProperty, PropertyBase.ParentObjectIdProperty),
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(MessageEventRuleClassificationSchema.DataClassificationIdProperty, PropertyBase.RefObjectIdProperty),
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EventHashKeyProperty, CommonMessageTraceSchema.EventHashKeyProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageEventSourceItemsTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageEventSourceItemPropertiesTableProperty,
				ParentKeyProperty = MessageEventSourceItemSchema.SourceItemIdProperty,
				ChildKeyProperty = PropertyBase.ParentIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(MessageEventSourceItemSchema.NameProperty, PropertyBase.RefNameProperty),
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EventHashKeyProperty, CommonMessageTraceSchema.EventHashKeyProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageClassificationsTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageClassificationPropertiesTableProperty,
				ParentKeyProperty = MessageClassificationSchema.ClassificationIdProperty,
				ChildKeyProperty = PropertyBase.ParentIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(MessageClassificationSchema.DataClassificationIdProperty, PropertyBase.ParentObjectIdProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageClientInformationTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageClientInformationPropertiesTableProperty,
				ParentKeyProperty = MessageClientInformationSchema.ClientInformationIdProperty,
				ChildKeyProperty = PropertyBase.ParentIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(MessageClientInformationSchema.DataClassificationIdProperty, PropertyBase.ParentObjectIdProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageRecipientStatusTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageRecipientStatusPropertiesTableProperty,
				ParentKeyProperty = MessageRecipientStatusSchema.RecipientStatusIdProperty,
				ChildKeyProperty = PropertyBase.ParentIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EventHashKeyProperty, CommonMessageTraceSchema.EventHashKeyProperty),
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EmailHashKeyProperty, CommonMessageTraceSchema.EmailHashKeyProperty)
				}
			});
			MessageTraceForSaveDataSet.mappingInfoList.Add(new MessageTraceForSaveDataSet.MappingInfo
			{
				ParentTableProperty = MessageTraceDataSetSchema.MessageActionTableProperty,
				ChildTableProperty = MessageTraceDataSetSchema.MessageActionPropertiesTableProperty,
				ParentKeyProperty = MessageActionSchema.RuleActionIdProperty,
				ChildKeyProperty = PropertyBase.ParentIdProperty,
				PropertyMappings = new List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>>
				{
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.RuleIdProperty, PropertyBase.ParentObjectIdProperty),
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(CommonMessageTraceSchema.EventHashKeyProperty, CommonMessageTraceSchema.EventHashKeyProperty),
					new Tuple<HygienePropertyDefinition, HygienePropertyDefinition>(MessageActionSchema.NameProperty, PropertyBase.RefNameProperty)
				}
			});
		}

		public override ObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		public object PhysicalPartionId
		{
			get
			{
				return this[CommonMessageTraceSchema.PhysicalInstanceKeyProp];
			}
			set
			{
				this[CommonMessageTraceSchema.PhysicalInstanceKeyProp] = value;
			}
		}

		public object FssCopyId
		{
			get
			{
				return this[CommonMessageTraceSchema.FssCopyIdProp];
			}
			set
			{
				this[CommonMessageTraceSchema.FssCopyIdProp] = value;
			}
		}

		public static MessageTraceForSaveDataSet CreateDataSet(object partitionId, Guid? organizationId, IEnumerable<MessageTrace> messageList, int? fssCopyId = null)
		{
			if (partitionId == null && (organizationId == null || organizationId == null))
			{
				throw new ArgumentException("CreateDataSet call is invalid. Both partitionId and organizationId cannot be null");
			}
			if (partitionId != null && organizationId != null && organizationId != null)
			{
				throw new ArgumentException("CreateDataSet call is invalid. Shouldn't set both partitionId and organizationId.");
			}
			MessageTraceForSaveDataSet.FillConfigurablePropertyBagFromGraph fillConfigurablePropertyBagFromGraph = new MessageTraceForSaveDataSet.FillConfigurablePropertyBagFromGraph();
			foreach (MessageTrace messageTrace in messageList)
			{
				MessageTraceForSaveDataSet.SetMessageProperties(messageTrace);
				messageTrace.Accept(fillConfigurablePropertyBagFromGraph);
			}
			if (partitionId != null)
			{
				fillConfigurablePropertyBagFromGraph.PropertyBag.PhysicalPartionId = (int)partitionId;
			}
			if (organizationId != null && organizationId != null)
			{
				fillConfigurablePropertyBagFromGraph.PropertyBag[CommonMessageTraceSchema.OrganizationalUnitRootProperty] = organizationId.Value;
			}
			if (fssCopyId != null)
			{
				fillConfigurablePropertyBagFromGraph.PropertyBag.FssCopyId = fssCopyId;
			}
			fillConfigurablePropertyBagFromGraph.PropertyBag.identity = new ConfigObjectId(Guid.NewGuid().ToString());
			fillConfigurablePropertyBagFromGraph.PropertyBag.ValidateObject();
			return fillConfigurablePropertyBagFromGraph.PropertyBag;
		}

		public override Type GetSchemaType()
		{
			return typeof(MessageTraceDataSetSchema);
		}

		public void ValidateObject()
		{
			this.RemoveDuplicates();
		}

		public int GetDatasize()
		{
			int num = 0;
			foreach (HygienePropertyDefinition propertyDefinition in MessageTraceForSaveDataSet.tvpDataTables)
			{
				DataTable dataTable = this[propertyDefinition] as DataTable;
				if (dataTable != null)
				{
					num += dataTable.Rows.Count;
				}
			}
			return num;
		}

		private static void SetProperties(DataTable parentTable, DataTable childTable, HygienePropertyDefinition parentKey, HygienePropertyDefinition childKey, List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>> propertyMappings)
		{
			if (!parentTable.Columns.Contains(parentKey.Name) || !childTable.Columns.Contains(childKey.Name) || propertyMappings == null || propertyMappings.Count == 0)
			{
				return;
			}
			var enumerable = from childRow in childTable.AsEnumerable()
			join parentRow in parentTable.AsEnumerable() on childRow[childKey.Name] equals parentRow[parentKey.Name]
			select new
			{
				ChildRow = childRow,
				ParentRow = parentRow
			};
			foreach (var <>f__AnonymousType in enumerable)
			{
				foreach (Tuple<HygienePropertyDefinition, HygienePropertyDefinition> tuple in propertyMappings)
				{
					if (childTable.Columns.Contains(tuple.Item2.Name))
					{
						<>f__AnonymousType.ChildRow[tuple.Item2.Name] = <>f__AnonymousType.ParentRow[tuple.Item1.Name];
					}
				}
			}
		}

		private static void SetMessageProperties(DataTable messageTable)
		{
			if (messageTable == null)
			{
				return;
			}
			foreach (object obj in messageTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (dataRow[MessageTraceSchema.IPAddressProperty.Name] is string)
				{
					dataRow[CommonMessageTraceSchema.IPHashKeyProperty.Name] = DalHelper.GetSHA1Hash((dataRow[MessageTraceSchema.IPAddressProperty.Name] as string).ToLower());
				}
				dataRow[MessageTraceSchema.FromEmailPrefixProperty.Name] = MessageTraceEntityBase.StandardizeEmailPrefix(dataRow[MessageTraceSchema.FromEmailPrefixProperty.Name] as string);
				dataRow[MessageTraceSchema.FromEmailDomainProperty.Name] = MessageTraceEntityBase.StandardizeEmailDomain(dataRow[MessageTraceSchema.FromEmailDomainProperty.Name] as string);
				dataRow[CommonMessageTraceSchema.EmailDomainHashKeyProperty.Name] = (MessageTraceEntityBase.GetEmailDomainHashKey(dataRow[MessageTraceSchema.FromEmailDomainProperty.Name] as string) ?? MessageTraceEntityBase.EmptyEmailDomainHashKey);
				dataRow[CommonMessageTraceSchema.EmailHashKeyProperty.Name] = (MessageTraceEntityBase.GetEmailHashKey(dataRow[MessageTraceSchema.FromEmailPrefixProperty.Name] as string, dataRow[MessageTraceSchema.FromEmailDomainProperty.Name] as string) ?? MessageTraceEntityBase.EmptyEmailHashKey);
			}
		}

		private static void SetMessageProperties(MessageTrace messageTrace)
		{
			Guid exMessageId = messageTrace.ExMessageId;
			if (messageTrace.ExMessageId == Guid.Empty)
			{
				throw new ArgumentException("MessageTrace object has an invalid ExMessageId (null or empty)");
			}
			if (messageTrace.IPAddress == null)
			{
				messageTrace[CommonMessageTraceSchema.IPHashKeyProperty] = null;
			}
			else
			{
				messageTrace[CommonMessageTraceSchema.IPHashKeyProperty] = DalHelper.GetSHA1Hash(messageTrace.IPAddress.ToString().ToLower());
			}
			messageTrace.FromEmailPrefix = MessageTraceEntityBase.StandardizeEmailPrefix(messageTrace.FromEmailPrefix);
			messageTrace.FromEmailDomain = MessageTraceEntityBase.StandardizeEmailDomain(messageTrace.FromEmailDomain);
			messageTrace[CommonMessageTraceSchema.EmailDomainHashKeyProperty] = (MessageTraceEntityBase.GetEmailDomainHashKey(messageTrace.FromEmailDomain) ?? MessageTraceEntityBase.EmptyEmailDomainHashKey);
			messageTrace[CommonMessageTraceSchema.EmailHashKeyProperty] = (MessageTraceEntityBase.GetEmailHashKey(messageTrace.FromEmailPrefix, messageTrace.FromEmailDomain) ?? MessageTraceEntityBase.EmptyEmailHashKey);
		}

		private static TvpInfo CreateTvpInfoPrototype(HygienePropertyDefinition tableName, HygienePropertyDefinition[] columnDefinitions)
		{
			HygienePropertyDefinition[] array = new HygienePropertyDefinition[columnDefinitions.Length];
			DataTable dataTable = new DataTable();
			DataColumnCollection columns = dataTable.Columns;
			dataTable.TableName = tableName.Name;
			foreach (HygienePropertyDefinition hygienePropertyDefinition in columnDefinitions)
			{
				if (!hygienePropertyDefinition.IsCalculated)
				{
					DataColumn dataColumn = columns.Add(hygienePropertyDefinition.Name, (hygienePropertyDefinition.Type == typeof(byte[])) ? hygienePropertyDefinition.Type : DalHelper.ConvertToStoreType(hygienePropertyDefinition));
					array[dataColumn.Ordinal] = hygienePropertyDefinition;
				}
			}
			dataTable.BeginLoadData();
			return new TvpInfo(tableName, dataTable, array);
		}

		private static MessageTraceForSaveDataSet CreateMessageTraceForSaveDataSet()
		{
			MessageTraceForSaveDataSet messageTraceForSaveDataSet = new MessageTraceForSaveDataSet();
			foreach (TvpInfo tvpInfo in MessageTraceForSaveDataSet.tvpPrototypeList)
			{
				messageTraceForSaveDataSet[tvpInfo.TableName] = tvpInfo.Tvp.Clone();
			}
			return messageTraceForSaveDataSet;
		}

		private void SyncProperties()
		{
			foreach (HygienePropertyDefinition hygienePropertyDefinition in MessageTraceForSaveDataSet.mapTableToTvpColumnInfo.Keys)
			{
				DataTable table = this[hygienePropertyDefinition] as DataTable;
				if (table != null)
				{
					foreach (HygienePropertyDefinition hygienePropertyDefinition2 in MessageTraceForSaveDataSet.mapTableToTvpColumnInfo[hygienePropertyDefinition])
					{
						if (!table.Columns.Contains(hygienePropertyDefinition2.Name))
						{
							table.Columns.Add(hygienePropertyDefinition2.Name, (hygienePropertyDefinition2.Type == typeof(byte[])) ? hygienePropertyDefinition2.Type : DalHelper.ConvertToStoreType(hygienePropertyDefinition2));
						}
					}
					int i;
					for (i = 0; i < table.Columns.Count; i++)
					{
						if (MessageTraceForSaveDataSet.mapTableToTvpColumnInfo[hygienePropertyDefinition].FirstOrDefault((HygienePropertyDefinition propertyDefinition) => string.Compare(propertyDefinition.Name, table.Columns[i].ColumnName, StringComparison.OrdinalIgnoreCase) == 0) == null)
						{
							table.Columns.RemoveAt(i--);
						}
					}
				}
			}
		}

		private void RemoveDuplicates()
		{
			DataTable table = this[MessageTraceDataSetSchema.MessageEventsTableProperty] as DataTable;
			if (this.RemoveDuplicates(table, MessageTraceForSaveDataSet.eventComparer))
			{
				DataTable table2 = this[MessageTraceDataSetSchema.MessageEventPropertiesTableProperty] as DataTable;
				this.RemoveDuplicates(table2, MessageTraceForSaveDataSet.propertyComparer);
			}
			DataTable table3 = this[MessageTraceDataSetSchema.MessageRecipientStatusTableProperty] as DataTable;
			if (this.RemoveDuplicates(table3, MessageTraceForSaveDataSet.recipientStatusComparer))
			{
				DataTable table4 = this[MessageTraceDataSetSchema.MessageRecipientStatusPropertiesTableProperty] as DataTable;
				this.RemoveDuplicates(table4, MessageTraceForSaveDataSet.propertyComparer);
			}
			DataTable table5 = this[MessageTraceDataSetSchema.MessageEventSourceItemsTableProperty] as DataTable;
			if (this.RemoveDuplicates(table5, MessageTraceForSaveDataSet.eventSourceItemComparer))
			{
				DataTable table6 = this[MessageTraceDataSetSchema.MessageEventSourceItemPropertiesTableProperty] as DataTable;
				this.RemoveDuplicates(table6, MessageTraceForSaveDataSet.propertyComparer);
			}
			DataTable table7 = this[MessageTraceDataSetSchema.MessageEventRulesTableProperty] as DataTable;
			if (this.RemoveDuplicates(table7, MessageTraceForSaveDataSet.eventRuleComparer))
			{
				DataTable table8 = this[MessageTraceDataSetSchema.MessageEventRulePropertiesTableProperty] as DataTable;
				this.RemoveDuplicates(table8, MessageTraceForSaveDataSet.propertyComparer);
			}
			DataTable table9 = this[MessageTraceDataSetSchema.MessageActionTableProperty] as DataTable;
			if (this.RemoveDuplicates(table9, MessageTraceForSaveDataSet.actionComparer))
			{
				DataTable table10 = this[MessageTraceDataSetSchema.MessageActionPropertiesTableProperty] as DataTable;
				this.RemoveDuplicates(table10, MessageTraceForSaveDataSet.propertyComparer);
			}
			DataTable table11 = this[MessageTraceDataSetSchema.MessageEventRuleClassificationsTableProperty] as DataTable;
			if (this.RemoveDuplicates(table11, MessageTraceForSaveDataSet.eventRuleClassificationComparer))
			{
				DataTable table12 = this[MessageTraceDataSetSchema.MessageEventRuleClassificationPropertiesTableProperty] as DataTable;
				this.RemoveDuplicates(table12, MessageTraceForSaveDataSet.propertyComparer);
			}
			DataTable table13 = this[MessageTraceDataSetSchema.MessageRecipientsTableProperty] as DataTable;
			if (this.RemoveDuplicates(table13, MessageTraceForSaveDataSet.recipientsComparer))
			{
				DataTable table14 = this[MessageTraceDataSetSchema.MessageRecipientPropertiesTableProperty] as DataTable;
				this.RemoveDuplicates(table14, MessageTraceForSaveDataSet.propertyComparer);
			}
			DataTable table15 = this[MessageTraceDataSetSchema.MessageClassificationsTableProperty] as DataTable;
			if (this.RemoveDuplicates(table15, MessageTraceForSaveDataSet.classificationsComparer))
			{
				DataTable table16 = this[MessageTraceDataSetSchema.MessageClassificationPropertiesTableProperty] as DataTable;
				this.RemoveDuplicates(table16, MessageTraceForSaveDataSet.propertyComparer);
			}
			DataTable table17 = this[MessageTraceDataSetSchema.MessageClientInformationTableProperty] as DataTable;
			if (this.RemoveDuplicates(table17, MessageTraceForSaveDataSet.clientInformationsComparer))
			{
				DataTable table18 = this[MessageTraceDataSetSchema.MessageClientInformationPropertiesTableProperty] as DataTable;
				this.RemoveDuplicates(table18, MessageTraceForSaveDataSet.propertyComparer);
			}
		}

		private bool RemoveDuplicates(DataTable table, MessageTraceForSaveDataSet.MessageRowComparer comparer)
		{
			if (table == null || table.Rows.Count == 0)
			{
				return false;
			}
			bool result = false;
			HashSet<DataRow> hashSet = new HashSet<DataRow>(comparer);
			int i = 0;
			while (i < table.Rows.Count)
			{
				if (!hashSet.Contains(table.Rows[i]))
				{
					hashSet.Add(table.Rows[i++]);
				}
				else
				{
					table.Rows.RemoveAt(i);
					result = true;
				}
			}
			return result;
		}

		private static TvpInfo[] tvpPrototypeList = new TvpInfo[]
		{
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessagesTableProperty, MessageTrace.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessagePropertiesTableProperty, MessageProperty.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageActionTableProperty, MessageAction.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageActionPropertiesTableProperty, MessageActionProperty.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageEventsTableProperty, MessageEvent.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageEventPropertiesTableProperty, MessageEventProperty.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageEventRulesTableProperty, MessageEventRule.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageEventRulePropertiesTableProperty, MessageEventRuleProperty.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageEventSourceItemsTableProperty, MessageEventSourceItem.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageEventSourceItemPropertiesTableProperty, MessageEventSourceItemProperty.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageRecipientsTableProperty, MessageRecipient.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageRecipientPropertiesTableProperty, MessageRecipientProperty.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageRecipientStatusTableProperty, MessageRecipientStatus.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageRecipientStatusPropertiesTableProperty, MessageRecipientStatusProperty.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageClassificationsTableProperty, MessageClassification.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageClassificationPropertiesTableProperty, MessageClassificationProperty.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageEventRuleClassificationsTableProperty, MessageEventRuleClassification.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageEventRuleClassificationPropertiesTableProperty, MessageEventRuleClassificationProperty.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageClientInformationTableProperty, MessageClientInformation.Properties),
			MessageTraceForSaveDataSet.CreateTvpInfoPrototype(MessageTraceDataSetSchema.MessageClientInformationPropertiesTableProperty, MessageClientInformationProperty.Properties)
		};

		private static HygienePropertyDefinition[] tvpDataTables = new HygienePropertyDefinition[]
		{
			MessageTraceDataSetSchema.MessagesTableProperty,
			MessageTraceDataSetSchema.MessagePropertiesTableProperty,
			MessageTraceDataSetSchema.MessageActionTableProperty,
			MessageTraceDataSetSchema.MessageActionPropertiesTableProperty,
			MessageTraceDataSetSchema.MessageEventsTableProperty,
			MessageTraceDataSetSchema.MessageEventPropertiesTableProperty,
			MessageTraceDataSetSchema.MessageEventRulesTableProperty,
			MessageTraceDataSetSchema.MessageEventRulePropertiesTableProperty,
			MessageTraceDataSetSchema.MessageEventSourceItemsTableProperty,
			MessageTraceDataSetSchema.MessageEventSourceItemPropertiesTableProperty,
			MessageTraceDataSetSchema.MessageRecipientsTableProperty,
			MessageTraceDataSetSchema.MessageRecipientPropertiesTableProperty,
			MessageTraceDataSetSchema.MessageRecipientStatusTableProperty,
			MessageTraceDataSetSchema.MessageRecipientStatusPropertiesTableProperty,
			MessageTraceDataSetSchema.MessageClassificationsTableProperty,
			MessageTraceDataSetSchema.MessageClassificationPropertiesTableProperty,
			MessageTraceDataSetSchema.MessageEventRuleClassificationsTableProperty,
			MessageTraceDataSetSchema.MessageEventRuleClassificationPropertiesTableProperty,
			MessageTraceDataSetSchema.MessageClientInformationTableProperty,
			MessageTraceDataSetSchema.MessageClientInformationPropertiesTableProperty
		};

		private static Dictionary<HygienePropertyDefinition, HygienePropertyDefinition[]> mapTableToTvpColumnInfo;

		private static List<MessageTraceForSaveDataSet.MappingInfo> mappingInfoList;

		private static MessageTraceForSaveDataSet.MessageEventRowComparer eventComparer = new MessageTraceForSaveDataSet.MessageEventRowComparer();

		private static MessageTraceForSaveDataSet.MessageRecipientStatusRowComparer recipientStatusComparer = new MessageTraceForSaveDataSet.MessageRecipientStatusRowComparer();

		private static MessageTraceForSaveDataSet.MessageEventSourceItemRowComparer eventSourceItemComparer = new MessageTraceForSaveDataSet.MessageEventSourceItemRowComparer();

		private static MessageTraceForSaveDataSet.MessageEventRuleRowComparer eventRuleComparer = new MessageTraceForSaveDataSet.MessageEventRuleRowComparer();

		private static MessageTraceForSaveDataSet.MessageActionRowComparer actionComparer = new MessageTraceForSaveDataSet.MessageActionRowComparer();

		private static MessageTraceForSaveDataSet.MessageEventRuleClassificationRowComparer eventRuleClassificationComparer = new MessageTraceForSaveDataSet.MessageEventRuleClassificationRowComparer();

		private static MessageTraceForSaveDataSet.MessageRecipientRowComparer recipientsComparer = new MessageTraceForSaveDataSet.MessageRecipientRowComparer();

		private static MessageTraceForSaveDataSet.MessageClassificationRowComparer classificationsComparer = new MessageTraceForSaveDataSet.MessageClassificationRowComparer();

		private static MessageTraceForSaveDataSet.MessageClientInformationRowComparer clientInformationsComparer = new MessageTraceForSaveDataSet.MessageClientInformationRowComparer();

		private static MessageTraceForSaveDataSet.MessagePropertyRowComparer propertyComparer = new MessageTraceForSaveDataSet.MessagePropertyRowComparer();

		private ObjectId identity;

		private class FillConfigurablePropertyBagFromGraph : MessageTraceVisitorBase
		{
			public FillConfigurablePropertyBagFromGraph()
			{
				this.bag = MessageTraceForSaveDataSet.CreateMessageTraceForSaveDataSet();
			}

			public MessageTraceForSaveDataSet PropertyBag
			{
				get
				{
					return this.bag;
				}
			}

			public override void Visit(MessageTrace messageTraceInfo)
			{
				this.lastVisitedTrace = messageTraceInfo;
				this.SerializeObjectToDataTable<MessageTrace>(messageTraceInfo, MessageTraceDataSetSchema.MessagesTableProperty);
			}

			public override void Visit(MessageProperty messageProperty)
			{
				this.ApplyMessageProperties(messageProperty);
				this.SerializeObjectToDataTable<MessageProperty>(messageProperty, MessageTraceDataSetSchema.MessagePropertiesTableProperty);
			}

			public override void Visit(MessageEvent messageEvent)
			{
				this.ApplyMessageProperties(messageEvent);
				string inputString = string.Format("{0}{1:yyyy-MM-dd hh:mm:ss.fffffff}{2}", (int)messageEvent.EventType, messageEvent.TimeStamp, (int)messageEvent.EventSource);
				messageEvent[CommonMessageTraceSchema.EventHashKeyProperty] = DalHelper.GetMDHash(inputString);
				this.messageEventIdMap[messageEvent.EventId] = messageEvent;
				this.SerializeObjectToDataTable<MessageEvent>(messageEvent, MessageTraceDataSetSchema.MessageEventsTableProperty);
			}

			public override void Visit(MessageEventProperty messageEventProperty)
			{
				this.ApplyMessageProperties(messageEventProperty);
				this.ApplyMessageEventProperties(this.messageEventIdMap[messageEventProperty.EventId], messageEventProperty);
				this.SerializeObjectToDataTable<MessageEventProperty>(messageEventProperty, MessageTraceDataSetSchema.MessageEventPropertiesTableProperty);
			}

			public override void Visit(MessageRecipient messageRecipient)
			{
				this.ApplyMessageProperties(messageRecipient);
				messageRecipient.ToEmailPrefix = MessageTraceEntityBase.StandardizeEmailPrefix(messageRecipient.ToEmailPrefix);
				messageRecipient.ToEmailDomain = MessageTraceEntityBase.StandardizeEmailDomain(messageRecipient.ToEmailDomain);
				messageRecipient[CommonMessageTraceSchema.EmailDomainHashKeyProperty] = MessageTraceEntityBase.GetEmailDomainHashKey(messageRecipient.ToEmailDomain);
				messageRecipient[CommonMessageTraceSchema.EmailHashKeyProperty] = MessageTraceEntityBase.GetEmailHashKey(messageRecipient.ToEmailPrefix, messageRecipient.ToEmailDomain);
				this.recipientIdMap[messageRecipient.RecipientId] = messageRecipient;
				this.SerializeObjectToDataTable<MessageRecipient>(messageRecipient, MessageTraceDataSetSchema.MessageRecipientsTableProperty);
			}

			public override void Visit(MessageRecipientProperty messageRecipientProperty)
			{
				this.ApplyMessageProperties(messageRecipientProperty);
				this.ApplyRecipientProperties(this.recipientIdMap[messageRecipientProperty.RecipientId], messageRecipientProperty);
				this.SerializeObjectToDataTable<MessageRecipientProperty>(messageRecipientProperty, MessageTraceDataSetSchema.MessageRecipientPropertiesTableProperty);
			}

			public override void Visit(MessageEventRule messageEventRule)
			{
				this.ApplyMessageProperties(messageEventRule);
				this.messageEventRuleIdMap[messageEventRule.EventRuleId] = messageEventRule;
				this.ApplyMessageEventProperties(this.messageEventIdMap[messageEventRule.EventId], messageEventRule);
				this.messageEventRuleIdMap[messageEventRule.EventRuleId] = messageEventRule;
				this.SerializeObjectToDataTable<MessageEventRule>(messageEventRule, MessageTraceDataSetSchema.MessageEventRulesTableProperty);
			}

			public override void Visit(MessageEventRuleProperty messageEventRuleProperty)
			{
				this.ApplyMessageProperties(messageEventRuleProperty);
				this.ApplyMessageEventRuleExProperties(this.messageEventRuleIdMap[messageEventRuleProperty.EventRuleId], messageEventRuleProperty);
				this.SerializeObjectToDataTable<MessageEventRuleProperty>(messageEventRuleProperty, MessageTraceDataSetSchema.MessageEventRulePropertiesTableProperty);
			}

			public override void Visit(MessageEventRuleClassification messageEventRuleClassification)
			{
				this.ApplyMessageProperties(messageEventRuleClassification);
				this.ApplyMessageEventRuleProperties(this.messageEventRuleIdMap[messageEventRuleClassification.EventRuleId], messageEventRuleClassification);
				this.messageEventRuleClassificationIdMap[messageEventRuleClassification.EventRuleClassificationId] = messageEventRuleClassification;
				this.SerializeObjectToDataTable<MessageEventRuleClassification>(messageEventRuleClassification, MessageTraceDataSetSchema.MessageEventRuleClassificationsTableProperty);
			}

			public override void Visit(MessageEventRuleClassificationProperty messageEventRuleClassificationProperty)
			{
				this.ApplyMessageProperties(messageEventRuleClassificationProperty);
				this.ApplyMessageEventRuleClassificationExProperties(this.messageEventRuleClassificationIdMap[messageEventRuleClassificationProperty.EventRuleClassificationId], messageEventRuleClassificationProperty);
				this.SerializeObjectToDataTable<MessageEventRuleClassificationProperty>(messageEventRuleClassificationProperty, MessageTraceDataSetSchema.MessageEventRuleClassificationPropertiesTableProperty);
			}

			public override void Visit(MessageEventSourceItem messageEventSourceItem)
			{
				this.ApplyMessageProperties(messageEventSourceItem);
				this.ApplyMessageEventProperties(this.messageEventIdMap[messageEventSourceItem.EventId], messageEventSourceItem);
				this.messageEventSourceItemIdMap[messageEventSourceItem.SourceItemId] = messageEventSourceItem;
				this.SerializeObjectToDataTable<MessageEventSourceItem>(messageEventSourceItem, MessageTraceDataSetSchema.MessageEventSourceItemsTableProperty);
			}

			public override void Visit(MessageEventSourceItemProperty messageEventSourceItemProperty)
			{
				this.ApplyMessageProperties(messageEventSourceItemProperty);
				this.ApplyMessageEventSourceItemExProperties(this.messageEventSourceItemIdMap[messageEventSourceItemProperty.SourceItemId], messageEventSourceItemProperty);
				this.SerializeObjectToDataTable<MessageEventSourceItemProperty>(messageEventSourceItemProperty, MessageTraceDataSetSchema.MessageEventSourceItemPropertiesTableProperty);
			}

			public override void Visit(MessageClassification messageClassification)
			{
				this.ApplyMessageProperties(messageClassification);
				this.messageClassificationIdMap[messageClassification.ClassificationId] = messageClassification;
				this.SerializeObjectToDataTable<MessageClassification>(messageClassification, MessageTraceDataSetSchema.MessageClassificationsTableProperty);
			}

			public override void Visit(MessageClassificationProperty messageClassificationProperty)
			{
				this.ApplyMessageProperties(messageClassificationProperty);
				this.ApplyMessageClassificationExProperty(this.messageClassificationIdMap[messageClassificationProperty.ClassificationId], messageClassificationProperty);
				this.SerializeObjectToDataTable<MessageClassificationProperty>(messageClassificationProperty, MessageTraceDataSetSchema.MessageClassificationPropertiesTableProperty);
			}

			public override void Visit(MessageClientInformation messageClientInformation)
			{
				this.ApplyMessageProperties(messageClientInformation);
				this.messageClientInformationIdMap[messageClientInformation.ClientInformationId] = messageClientInformation;
				this.SerializeObjectToDataTable<MessageClientInformation>(messageClientInformation, MessageTraceDataSetSchema.MessageClientInformationTableProperty);
			}

			public override void Visit(MessageClientInformationProperty messageClientInformationProperty)
			{
				this.ApplyMessageProperties(messageClientInformationProperty);
				this.ApplyMessageClientInformationExProperties(this.messageClientInformationIdMap[messageClientInformationProperty.ClientInformationId], messageClientInformationProperty);
				this.SerializeObjectToDataTable<MessageClientInformationProperty>(messageClientInformationProperty, MessageTraceDataSetSchema.MessageClientInformationPropertiesTableProperty);
			}

			public override void Visit(MessageRecipientStatus recipientStatus)
			{
				this.ApplyMessageProperties(recipientStatus);
				this.ApplyRecipientProperties(this.recipientIdMap[recipientStatus.RecipientId], recipientStatus);
				this.ApplyMessageEventProperties(this.messageEventIdMap[recipientStatus.EventId], recipientStatus);
				this.messageRecipientStatusIdMap[recipientStatus.RecipientStatusId] = recipientStatus;
				this.SerializeObjectToDataTable<MessageRecipientStatus>(recipientStatus, MessageTraceDataSetSchema.MessageRecipientStatusTableProperty);
			}

			public override void Visit(MessageRecipientStatusProperty recipientStatusProperty)
			{
				this.ApplyMessageProperties(recipientStatusProperty);
				this.ApplyMessageRecipientStatusExProperties(this.messageRecipientStatusIdMap[recipientStatusProperty.RecipientStatusId], recipientStatusProperty);
				this.SerializeObjectToDataTable<MessageRecipientStatusProperty>(recipientStatusProperty, MessageTraceDataSetSchema.MessageRecipientStatusPropertiesTableProperty);
			}

			public override void Visit(MessageAction messageAction)
			{
				this.ApplyMessageProperties(messageAction);
				if (messageAction.EventRuleId != Guid.Empty)
				{
					this.ApplyMessageEventRuleProperties(this.messageEventRuleIdMap[messageAction.EventRuleId], messageAction);
				}
				this.messageActionIdMap[messageAction.RuleActionId] = messageAction;
				this.SerializeObjectToDataTable<MessageAction>(messageAction, MessageTraceDataSetSchema.MessageActionTableProperty);
			}

			public override void Visit(MessageActionProperty messageActionProperty)
			{
				this.ApplyMessageProperties(messageActionProperty);
				this.ApplyMessageActionExProperties(this.messageActionIdMap[messageActionProperty.RuleActionId], messageActionProperty);
				this.SerializeObjectToDataTable<MessageActionProperty>(messageActionProperty, MessageTraceDataSetSchema.MessageActionPropertiesTableProperty);
			}

			private void ApplyMessageProperties(MessageTraceEntityBase entity)
			{
				entity[CommonMessageTraceSchema.ExMessageIdProperty] = this.lastVisitedTrace.ExMessageId;
				entity[CommonMessageTraceSchema.HashBucketProperty] = this.lastVisitedTrace[CommonMessageTraceSchema.HashBucketProperty];
			}

			private void ApplyMessageEventProperties(MessageEvent parentEvent, MessageTraceEntityBase entity)
			{
				entity[CommonMessageTraceSchema.EventIdProperty] = parentEvent.EventId;
				entity[CommonMessageTraceSchema.EventHashKeyProperty] = parentEvent[CommonMessageTraceSchema.EventHashKeyProperty];
			}

			private void ApplyMessageEventRuleProperties(MessageEventRule parentEventRule, MessageTraceEntityBase entity)
			{
				entity[MessageEventRuleSchema.EventRuleIdProperty] = parentEventRule.EventRuleId;
				entity[CommonMessageTraceSchema.RuleIdProperty] = parentEventRule.RuleId;
				entity[CommonMessageTraceSchema.EventHashKeyProperty] = parentEventRule[CommonMessageTraceSchema.EventHashKeyProperty];
			}

			private void ApplyMessageEventRuleExProperties(MessageEventRule parentEventRule, MessageEventRuleProperty entity)
			{
				entity[PropertyBase.ParentObjectIdProperty] = parentEventRule.RuleId;
				entity[CommonMessageTraceSchema.EventHashKeyProperty] = parentEventRule[CommonMessageTraceSchema.EventHashKeyProperty];
			}

			private void ApplyMessageActionExProperties(MessageAction parentAction, MessageActionProperty entity)
			{
				entity[PropertyBase.ParentObjectIdProperty] = parentAction[CommonMessageTraceSchema.RuleIdProperty];
				entity[CommonMessageTraceSchema.EventHashKeyProperty] = parentAction[CommonMessageTraceSchema.EventHashKeyProperty];
				entity[PropertyBase.RefNameProperty] = parentAction.Name;
			}

			private void ApplyMessageEventRuleClassificationExProperties(MessageEventRuleClassification parentRuleClassification, MessageEventRuleClassificationProperty entity)
			{
				entity[PropertyBase.ParentObjectIdProperty] = parentRuleClassification[CommonMessageTraceSchema.RuleIdProperty];
				entity[PropertyBase.RefObjectIdProperty] = parentRuleClassification.DataClassificationId;
				entity[CommonMessageTraceSchema.EventHashKeyProperty] = parentRuleClassification[CommonMessageTraceSchema.EventHashKeyProperty];
			}

			private void ApplyMessageEventSourceItemExProperties(MessageEventSourceItem parentSourceItem, MessageEventSourceItemProperty entity)
			{
				entity[PropertyBase.RefNameProperty] = parentSourceItem.Name;
				entity[CommonMessageTraceSchema.EventHashKeyProperty] = parentSourceItem[CommonMessageTraceSchema.EventHashKeyProperty];
			}

			private void ApplyRecipientProperties(MessageRecipient parentRecipient, MessageTraceEntityBase entity)
			{
				entity[MessageRecipientSchema.RecipientIdProperty] = parentRecipient.RecipientId;
				entity[CommonMessageTraceSchema.EmailHashKeyProperty] = parentRecipient[CommonMessageTraceSchema.EmailHashKeyProperty];
			}

			private void ApplyMessageRecipientStatusExProperties(MessageRecipientStatus parentRecipientStatus, MessageRecipientStatusProperty entity)
			{
				entity[CommonMessageTraceSchema.EventHashKeyProperty] = parentRecipientStatus[CommonMessageTraceSchema.EventHashKeyProperty];
				entity[CommonMessageTraceSchema.EmailHashKeyProperty] = parentRecipientStatus[CommonMessageTraceSchema.EmailHashKeyProperty];
			}

			private void ApplyMessageClassificationExProperty(MessageClassification parentClassification, MessageClassificationProperty entity)
			{
				entity[PropertyBase.ParentObjectIdProperty] = parentClassification.DataClassificationId;
			}

			private void ApplyMessageClientInformationExProperties(MessageClientInformation parentClientInformation, MessageClientInformationProperty entity)
			{
				entity[PropertyBase.ParentObjectIdProperty] = parentClientInformation.DataClassificationId;
			}

			private void SerializeObjectToDataTable<T>(T source, HygienePropertyDefinition tableDefinition) where T : MessageTraceEntityBase
			{
				DataTable dataTable = this.bag[tableDefinition] as DataTable;
				HygienePropertyDefinition[] columns = MessageTraceForSaveDataSet.mapTableToTvpColumnInfo[tableDefinition];
				DataRow row = dataTable.NewRow();
				this.PopulateRow(row, columns, source);
				dataTable.Rows.Add(row);
			}

			private void PopulateRow(DataRow row, HygienePropertyDefinition[] columns, MessageTraceEntityBase dataSource)
			{
				for (int i = 0; i < columns.Length; i++)
				{
					HygienePropertyDefinition hygienePropertyDefinition = columns[i];
					if (hygienePropertyDefinition != null && !hygienePropertyDefinition.IsCalculated)
					{
						object obj = dataSource[hygienePropertyDefinition];
						if (obj != hygienePropertyDefinition.DefaultValue)
						{
							row[i] = obj;
						}
					}
				}
			}

			private MessageTraceForSaveDataSet bag;

			private MessageTrace lastVisitedTrace;

			private Dictionary<Guid, MessageRecipient> recipientIdMap = new Dictionary<Guid, MessageRecipient>();

			private Dictionary<Guid, MessageEvent> messageEventIdMap = new Dictionary<Guid, MessageEvent>();

			private Dictionary<Guid, MessageEventRule> messageEventRuleIdMap = new Dictionary<Guid, MessageEventRule>();

			private Dictionary<Guid, MessageEventRuleClassification> messageEventRuleClassificationIdMap = new Dictionary<Guid, MessageEventRuleClassification>();

			private Dictionary<Guid, MessageAction> messageActionIdMap = new Dictionary<Guid, MessageAction>();

			private Dictionary<Guid, MessageRecipientStatus> messageRecipientStatusIdMap = new Dictionary<Guid, MessageRecipientStatus>();

			private Dictionary<Guid, MessageEventSourceItem> messageEventSourceItemIdMap = new Dictionary<Guid, MessageEventSourceItem>();

			private Dictionary<Guid, MessageClientInformation> messageClientInformationIdMap = new Dictionary<Guid, MessageClientInformation>();

			private Dictionary<Guid, MessageClassification> messageClassificationIdMap = new Dictionary<Guid, MessageClassification>();
		}

		private sealed class MappingInfo
		{
			public HygienePropertyDefinition ParentTableProperty { get; internal set; }

			public HygienePropertyDefinition ChildTableProperty { get; internal set; }

			public HygienePropertyDefinition ParentKeyProperty { get; internal set; }

			public HygienePropertyDefinition ChildKeyProperty { get; internal set; }

			public List<Tuple<HygienePropertyDefinition, HygienePropertyDefinition>> PropertyMappings { get; internal set; }
		}

		private abstract class MessageRowComparer : IEqualityComparer<DataRow>
		{
			public virtual bool Equals(DataRow x, DataRow y)
			{
				throw new NotImplementedException();
			}

			public virtual int GetHashCode(DataRow row)
			{
				return row.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name).GetHashCode();
			}

			public virtual bool CanCompare(DataRow x, DataRow y)
			{
				return x.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) == y.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name);
			}
		}

		private sealed class MessageEventRowComparer : MessageTraceForSaveDataSet.MessageRowComparer
		{
			public override bool Equals(DataRow x, DataRow y)
			{
				return x.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) == y.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) && x.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name).SequenceEqual(y.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name));
			}

			public override int GetHashCode(DataRow row)
			{
				Guid guid = row.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name);
				byte[] value = row.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name);
				return guid.GetHashCode() ^ (int)DalHelper.FastHash(value);
			}
		}

		private sealed class MessageEventSourceItemRowComparer : MessageTraceForSaveDataSet.MessageRowComparer
		{
			public override bool Equals(DataRow x, DataRow y)
			{
				return x.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) == y.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) && x.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name).SequenceEqual(y.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name)) && string.Compare(x.Field(MessageEventSourceItemSchema.NameProperty.Name), y.Field(MessageEventSourceItemSchema.NameProperty.Name), StringComparison.OrdinalIgnoreCase) == 0;
			}

			public override int GetHashCode(DataRow row)
			{
				Guid guid = row.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name);
				byte[] value = row.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name);
				string text = row.Field(MessageEventSourceItemSchema.NameProperty.Name);
				return guid.GetHashCode() ^ (int)DalHelper.FastHash(value) ^ (int)DalHelper.FastHash(text.ToLowerInvariant());
			}
		}

		private sealed class MessageRecipientStatusRowComparer : MessageTraceForSaveDataSet.MessageRowComparer
		{
			public override bool Equals(DataRow x, DataRow y)
			{
				return x.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) == y.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) && x.Field(CommonMessageTraceSchema.EmailHashKeyProperty.Name).SequenceEqual(y.Field(CommonMessageTraceSchema.EmailHashKeyProperty.Name)) && x.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name).SequenceEqual(y.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name));
			}

			public override int GetHashCode(DataRow row)
			{
				Guid guid = row.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name);
				byte[] value = row.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name);
				byte[] value2 = row.Field(CommonMessageTraceSchema.EmailHashKeyProperty.Name);
				return guid.GetHashCode() ^ (int)DalHelper.FastHash(value) ^ (int)DalHelper.FastHash(value2);
			}
		}

		private sealed class MessageEventRuleRowComparer : MessageTraceForSaveDataSet.MessageRowComparer
		{
			public override bool Equals(DataRow x, DataRow y)
			{
				return x.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) == y.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) && x.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name).SequenceEqual(y.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name)) && x.Field(CommonMessageTraceSchema.RuleIdProperty.Name) == y.Field(CommonMessageTraceSchema.RuleIdProperty.Name) && string.Equals(x.Field(MessageEventRuleSchema.RuleTypeProperty.Name), y.Field(MessageEventRuleSchema.RuleTypeProperty.Name), StringComparison.OrdinalIgnoreCase);
			}

			public override int GetHashCode(DataRow row)
			{
				Guid guid = row.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name);
				byte[] value = row.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name);
				Guid guid2 = row.Field(CommonMessageTraceSchema.RuleIdProperty.Name);
				string value2 = row.Field(MessageEventRuleSchema.RuleTypeProperty.Name) ?? string.Empty;
				return guid.GetHashCode() ^ (int)DalHelper.FastHash(value) ^ guid2.GetHashCode() ^ (int)DalHelper.FastHash(value2);
			}
		}

		private sealed class MessageActionRowComparer : MessageTraceForSaveDataSet.MessageRowComparer
		{
			public override bool Equals(DataRow x, DataRow y)
			{
				return x.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) == y.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) && x.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name).SequenceEqual(y.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name)) && x.Field(CommonMessageTraceSchema.RuleIdProperty.Name) == y.Field(CommonMessageTraceSchema.RuleIdProperty.Name) && string.Compare(x.Field(MessageActionSchema.NameProperty.Name), y.Field(MessageActionSchema.NameProperty.Name), StringComparison.OrdinalIgnoreCase) == 0;
			}

			public override int GetHashCode(DataRow row)
			{
				Guid guid = row.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name);
				byte[] value = row.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name);
				Guid guid2 = row.Field(CommonMessageTraceSchema.RuleIdProperty.Name);
				string text = row.Field(MessageActionSchema.NameProperty.Name);
				return guid.GetHashCode() ^ (int)DalHelper.FastHash(value) ^ guid2.GetHashCode() ^ (int)DalHelper.FastHash(text.ToLowerInvariant());
			}
		}

		private sealed class MessageEventRuleClassificationRowComparer : MessageTraceForSaveDataSet.MessageRowComparer
		{
			public override bool Equals(DataRow x, DataRow y)
			{
				return x.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) == y.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) && x.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name).SequenceEqual(y.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name)) && x.Field(CommonMessageTraceSchema.RuleIdProperty.Name) == y.Field(CommonMessageTraceSchema.RuleIdProperty.Name) && x.Field(CommonMessageTraceSchema.DataClassificationIdProperty.Name) == y.Field(CommonMessageTraceSchema.DataClassificationIdProperty.Name);
			}

			public override int GetHashCode(DataRow row)
			{
				Guid guid = row.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name);
				byte[] value = row.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name);
				Guid guid2 = row.Field(CommonMessageTraceSchema.RuleIdProperty.Name);
				Guid guid3 = row.Field(CommonMessageTraceSchema.DataClassificationIdProperty.Name);
				return guid.GetHashCode() ^ (int)DalHelper.FastHash(value) ^ guid2.GetHashCode() ^ guid3.GetHashCode();
			}
		}

		private sealed class MessageClassificationRowComparer : MessageTraceForSaveDataSet.MessageRowComparer
		{
			public override bool Equals(DataRow x, DataRow y)
			{
				return x.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) == y.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) && x.Field(CommonMessageTraceSchema.DataClassificationIdProperty.Name) == y.Field(CommonMessageTraceSchema.DataClassificationIdProperty.Name);
			}

			public override int GetHashCode(DataRow row)
			{
				Guid guid = row.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name);
				Guid guid2 = row.Field(CommonMessageTraceSchema.DataClassificationIdProperty.Name);
				return guid.GetHashCode() ^ guid2.GetHashCode();
			}
		}

		private sealed class MessageClientInformationRowComparer : MessageTraceForSaveDataSet.MessageRowComparer
		{
			public override bool Equals(DataRow x, DataRow y)
			{
				return x.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) == y.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) && x.Field(CommonMessageTraceSchema.DataClassificationIdProperty.Name) == y.Field(CommonMessageTraceSchema.DataClassificationIdProperty.Name);
			}

			public override int GetHashCode(DataRow row)
			{
				Guid guid = row.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name);
				Guid guid2 = row.Field(CommonMessageTraceSchema.DataClassificationIdProperty.Name);
				return guid.GetHashCode() ^ guid2.GetHashCode();
			}
		}

		private sealed class MessageRecipientRowComparer : MessageTraceForSaveDataSet.MessageRowComparer
		{
			public override bool Equals(DataRow x, DataRow y)
			{
				return x.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) == y.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) && x.Field(CommonMessageTraceSchema.EmailHashKeyProperty.Name).SequenceEqual(y.Field(CommonMessageTraceSchema.EmailHashKeyProperty.Name));
			}

			public override int GetHashCode(DataRow row)
			{
				Guid guid = row.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name);
				byte[] value = row.Field(CommonMessageTraceSchema.EmailHashKeyProperty.Name);
				return guid.GetHashCode() ^ (int)DalHelper.FastHash(value);
			}
		}

		private sealed class MessagePropertyRowComparer : MessageTraceForSaveDataSet.MessageRowComparer
		{
			public override bool Equals(DataRow x, DataRow y)
			{
				return !(x.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name) != y.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name)) && MessageTraceForSaveDataSet.MessagePropertyRowComparer.Equals(x.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name), y.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name)) && !(x.Field(PropertyBase.ParentObjectIdProperty.Name) != y.Field(PropertyBase.ParentObjectIdProperty.Name)) && !(x.Field(PropertyBase.RefObjectIdProperty.Name) != y.Field(PropertyBase.RefObjectIdProperty.Name)) && string.Compare(x.Field(PropertyBase.PropertyNameProperty.Name), y.Field(PropertyBase.PropertyNameProperty.Name), StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(x.Field(PropertyBase.RefNameProperty.Name), y.Field(PropertyBase.RefNameProperty.Name), StringComparison.OrdinalIgnoreCase) == 0 && x.Field(PropertyBase.PropertyIndexProperty.Name) == y.Field(PropertyBase.PropertyIndexProperty.Name);
			}

			public override int GetHashCode(DataRow row)
			{
				Guid guid = row.Field(CommonMessageTraceSchema.ExMessageIdProperty.Name);
				byte[] value = row.Field(CommonMessageTraceSchema.EventHashKeyProperty.Name) ?? MessageTraceForSaveDataSet.MessagePropertyRowComparer.emptyByteArray;
				Guid guid2 = row.Field(PropertyBase.ParentObjectIdProperty.Name) ?? Guid.Empty;
				Guid guid3 = row.Field(PropertyBase.RefObjectIdProperty.Name) ?? Guid.Empty;
				string text = row.Field(PropertyBase.RefNameProperty.Name) ?? string.Empty;
				string text2 = row.Field(PropertyBase.PropertyNameProperty.Name) ?? string.Empty;
				int num = row.Field(PropertyBase.PropertyIndexProperty.Name);
				return guid.GetHashCode() ^ (int)DalHelper.FastHash(value) ^ guid2.GetHashCode() ^ guid3.GetHashCode() ^ (int)DalHelper.FastHash(text.ToLowerInvariant()) ^ (int)DalHelper.FastHash(text2.ToLowerInvariant()) ^ num;
			}

			private static bool Equals(byte[] leftArray, byte[] rightArray)
			{
				return (leftArray == null && rightArray == null) || (leftArray != null && rightArray != null && leftArray.SequenceEqual(rightArray));
			}

			private static readonly byte[] emptyByteArray = new byte[0];
		}
	}
}
