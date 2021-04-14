using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	internal static class PropertySchemaPopulation
	{
		private static void Populate(StoreDatabase database)
		{
			PropertySchema.AddObjectSchema(database, ObjectType.Message, PropertySchemaPopulation.GenerateMessagePropertySchema(database));
			PropertySchema.AddObjectSchema(database, ObjectType.EmbeddedMessage, PropertySchemaPopulation.GenerateEmbeddedMessagePropertySchema(database));
			PropertySchema.AddObjectSchema(database, ObjectType.Folder, PropertySchemaPopulation.GenerateFolderPropertySchema(database));
			PropertySchema.AddObjectSchema(database, ObjectType.Attachment, PropertySchemaPopulation.GenerateAttachmentPropertySchema(database));
			PropertySchema.AddObjectSchema(database, ObjectType.AttachmentView, PropertySchemaPopulation.GenerateAttachmentViewPropertySchema(database));
			PropertySchema.AddObjectSchema(database, ObjectType.FolderView, PropertySchemaPopulation.GenerateFolderViewPropertySchema(database));
			PropertySchema.AddObjectSchema(database, ObjectType.Event, PropertySchemaPopulation.GenerateEventPropertySchema(database));
			PropertySchema.AddObjectSchema(database, ObjectType.Recipient, PropertySchemaPopulation.GenerateRecipientPropertySchema(database));
			PropertySchema.AddObjectSchema(database, ObjectType.Conversation, PropertySchemaPopulation.GenerateConversationPropertySchema(database));
			PropertySchema.AddObjectSchema(database, ObjectType.InferenceLog, PropertySchemaPopulation.GenerateInferenceLogPropertySchema(database));
			PropertySchema.AddObjectSchema(database, ObjectType.UserInfo, PropertySchemaPopulation.GenerateUserInfoPropertySchema(database));
		}

		public static ObjectPropertySchema GenerateMessagePropertySchema(StoreDatabase database)
		{
			Dictionary<StorePropTag, PropertyMapping> dictionary = new Dictionary<StorePropTag, PropertyMapping>(200);
			MessageTable messageTable = DatabaseSchema.MessageTable(database);
			if (messageTable == null)
			{
				return null;
			}
			Table table = messageTable.Table;
			ObjectPropertySchema objectPropertySchema = new ObjectPropertySchema();
			ObjectPropertySchema baseSchema = null;
			Func<IRowAccess, IRowPropertyBag> rowPropBagCreator = (IRowAccess rowAccess) => new RowPropertyBag(table, objectPropertySchema, PropTag.Message.MailboxNum, rowAccess);
			Column column;
			PropertyMapping propertyMapping;
			if (messageTable.MailboxPartitionNumber != null)
			{
				column = Factory.CreateMappedPropertyColumn(messageTable.MailboxPartitionNumber, PropTag.Message.MailboxPartitionNumber);
				propertyMapping = new PhysicalColumnPropertyMapping(PropTag.Message.MailboxPartitionNumber, column, null, null, null, (PhysicalColumn)column.ActualColumn, false, true, true, false, false);
			}
			else
			{
				column = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MailboxPartitionNumber, null), PropTag.Message.MailboxPartitionNumber);
				propertyMapping = new ConstantPropertyMapping(PropTag.Message.MailboxPartitionNumber, column, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.MailboxPartitionNumber, propertyMapping);
			Column column2;
			PropertyMapping propertyMapping2;
			if (messageTable.MailboxNumber != null)
			{
				column2 = Factory.CreateMappedPropertyColumn(messageTable.MailboxNumber, PropTag.Message.MailboxNumberInternal);
				propertyMapping2 = new PhysicalColumnPropertyMapping(PropTag.Message.MailboxNumberInternal, column2, null, null, null, (PhysicalColumn)column2.ActualColumn, false, true, true, false, false);
			}
			else
			{
				column2 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MailboxNumberInternal, null), PropTag.Message.MailboxNumberInternal);
				propertyMapping2 = new ConstantPropertyMapping(PropTag.Message.MailboxNumberInternal, column2, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.MailboxNumberInternal, propertyMapping2);
			Column column3 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructFunctionColumn(PropTag.Message.MailboxNum, typeof(int), 4, 0, messageTable.Table, new Func<object[], object>(PropertySchemaPopulation.ComputeMailboxNumber), "Exchange.ComputeMailboxNumber", new Column[]
			{
				column,
				column2
			}), PropTag.Message.MailboxNum);
			FunctionPropertyMapping value = new FunctionPropertyMapping(PropTag.Message.MailboxNum, column3, null, new Func<object[], object>(PropertySchemaPopulation.ComputeMailboxNumber), new PropertyMapping[]
			{
				propertyMapping,
				propertyMapping2
			}, true, true, false);
			dictionary.Add(PropTag.Message.MailboxNum, value);
			Column column4;
			PropertyMapping propertyMapping3;
			if (messageTable.MessageId != null)
			{
				column4 = Factory.CreateMappedPropertyColumn(messageTable.MessageId, PropTag.Message.MidBin);
				propertyMapping3 = new PhysicalColumnPropertyMapping(PropTag.Message.MidBin, column4, null, null, null, (PhysicalColumn)column4.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column4 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MidBin, null), PropTag.Message.MidBin);
				propertyMapping3 = new ConstantPropertyMapping(PropTag.Message.MidBin, column4, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.MidBin, propertyMapping3);
			Column column5 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.Mid, typeof(long), 8, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), "Exchange.ConvertExchangeIdToInt64", column4), PropTag.Message.Mid);
			ConversionPropertyMapping conversionPropertyMapping = new ConversionPropertyMapping(PropTag.Message.Mid, column5, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), PropTag.Message.MidBin, propertyMapping3, null, null, null, true, true, true);
			dictionary.Add(PropTag.Message.Mid, conversionPropertyMapping);
			Column column6;
			PropertyMapping propertyMapping4;
			if (messageTable.FolderId != null)
			{
				column6 = Factory.CreateMappedPropertyColumn(messageTable.FolderId, PropTag.Message.FidBin);
				propertyMapping4 = new PhysicalColumnPropertyMapping(PropTag.Message.FidBin, column6, null, null, null, (PhysicalColumn)column6.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column6 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.FidBin, null), PropTag.Message.FidBin);
				propertyMapping4 = new ConstantPropertyMapping(PropTag.Message.FidBin, column6, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.FidBin, propertyMapping4);
			Column column7 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.Fid, typeof(long), 8, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), "Exchange.ConvertExchangeIdToInt64", column6), PropTag.Message.Fid);
			ConversionPropertyMapping conversionPropertyMapping2 = new ConversionPropertyMapping(PropTag.Message.Fid, column7, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), PropTag.Message.FidBin, propertyMapping4, null, null, null, true, true, true);
			dictionary.Add(PropTag.Message.Fid, conversionPropertyMapping2);
			Column column8 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructFunctionColumn(PropTag.Message.EntryIdSvrEid, typeof(byte[]), 21, 0, messageTable.Table, new Func<object[], object>(PropertySchemaPopulation.ComputeMessageEntryId), "Exchange.ComputeMessageEntryId", new Column[]
			{
				column7,
				column5
			}), PropTag.Message.EntryIdSvrEid);
			FunctionPropertyMapping value2 = new FunctionPropertyMapping(PropTag.Message.EntryIdSvrEid, column8, null, new Func<object[], object>(PropertySchemaPopulation.ComputeMessageEntryId), new PropertyMapping[]
			{
				conversionPropertyMapping2,
				conversionPropertyMapping
			}, true, true, true);
			dictionary.Add(PropTag.Message.EntryIdSvrEid, value2);
			Column column9 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructFunctionColumn(PropTag.Message.EntryId, typeof(byte[]), 21, 0, messageTable.Table, new Func<object[], object>(PropertySchemaPopulation.ComputeMessageEntryId), "Exchange.ComputeMessageEntryId", new Column[]
			{
				column7,
				column5
			}), PropTag.Message.EntryId);
			FunctionPropertyMapping value3 = new FunctionPropertyMapping(PropTag.Message.EntryId, column9, null, new Func<object[], object>(PropertySchemaPopulation.ComputeMessageEntryId), new PropertyMapping[]
			{
				conversionPropertyMapping2,
				conversionPropertyMapping
			}, false, true, true);
			dictionary.Add(PropTag.Message.EntryId, value3);
			Column column10 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.ParentEntryId, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToFolderSvrEid), "Exchange.ConvertInt64ToFolderSvrEid", column7), PropTag.Message.ParentEntryId);
			ConversionPropertyMapping value4 = new ConversionPropertyMapping(PropTag.Message.ParentEntryId, column10, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToFolderSvrEid), PropTag.Message.Fid, conversionPropertyMapping2, null, null, null, false, true, true);
			dictionary.Add(PropTag.Message.ParentEntryId, value4);
			Column column11 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.ParentEntryIdSvrEid, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToFolderSvrEid), "Exchange.ConvertInt64ToFolderSvrEid", column7), PropTag.Message.ParentEntryIdSvrEid);
			ConversionPropertyMapping value5 = new ConversionPropertyMapping(PropTag.Message.ParentEntryIdSvrEid, column11, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToFolderSvrEid), PropTag.Message.Fid, conversionPropertyMapping2, null, null, null, true, true, true);
			dictionary.Add(PropTag.Message.ParentEntryIdSvrEid, value5);
			Column column12 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.RecordKeySvrEid, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToFolderSvrEid), "Exchange.ConvertInt64ToFolderSvrEid", column5), PropTag.Message.RecordKeySvrEid);
			ConversionPropertyMapping value6 = new ConversionPropertyMapping(PropTag.Message.RecordKeySvrEid, column12, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToFolderSvrEid), PropTag.Message.Mid, conversionPropertyMapping, null, null, null, true, true, true);
			dictionary.Add(PropTag.Message.RecordKeySvrEid, value6);
			Column column13 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.RecordKey, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToFolderSvrEid), "Exchange.ConvertInt64ToFolderSvrEid", column5), PropTag.Message.RecordKey);
			ConversionPropertyMapping value7 = new ConversionPropertyMapping(PropTag.Message.RecordKey, column13, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToFolderSvrEid), PropTag.Message.Mid, conversionPropertyMapping, null, null, null, false, true, true);
			dictionary.Add(PropTag.Message.RecordKey, value7);
			Column column14 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.InstanceId, typeof(long), 8, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), "Exchange.ConvertExchangeIdToInt64", column4), PropTag.Message.InstanceId);
			ConversionPropertyMapping value8 = new ConversionPropertyMapping(PropTag.Message.InstanceId, column14, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), PropTag.Message.MidBin, propertyMapping3, null, null, null, true, true, false);
			dictionary.Add(PropTag.Message.InstanceId, value8);
			Column column15;
			PropertyMapping propertyMapping5;
			if (messageTable.MessageDocumentId != null)
			{
				column15 = Factory.CreateMappedPropertyColumn(messageTable.MessageDocumentId, PropTag.Message.DocumentId);
				propertyMapping5 = new PhysicalColumnPropertyMapping(PropTag.Message.DocumentId, column15, null, null, null, (PhysicalColumn)column15.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column15 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.DocumentId, null), PropTag.Message.DocumentId);
				propertyMapping5 = new ConstantPropertyMapping(PropTag.Message.DocumentId, column15, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.DocumentId, propertyMapping5);
			PropertyMapping value9;
			if (messageTable.ConversationDocumentId != null)
			{
				Column column16 = Factory.CreateMappedPropertyColumn(messageTable.ConversationDocumentId, PropTag.Message.ConversationDocumentId);
				value9 = new PhysicalColumnPropertyMapping(PropTag.Message.ConversationDocumentId, column16, null, null, null, (PhysicalColumn)column16.ActualColumn, true, true, true, false, false);
			}
			else
			{
				Column column16 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.ConversationDocumentId, null), PropTag.Message.ConversationDocumentId);
				value9 = new ConstantPropertyMapping(PropTag.Message.ConversationDocumentId, column16, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.ConversationDocumentId, value9);
			PropertyColumn propertyColumn = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.InstanceNum, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping = new ComputedPropertyMapping(PropTag.Message.InstanceNum, propertyColumn, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageInstanceNum), null, null, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Message.InstanceNum, computedPropertyMapping);
			Column column17 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructFunctionColumn(PropTag.Message.InstanceKey, typeof(byte[]), 21, 0, messageTable.Table, new Func<object[], object>(PropertySchemaPopulation.ComputeMessageInstanceKey), "Exchange.ComputeMessageInstanceKey", new Column[]
			{
				column7,
				column5,
				propertyColumn
			}), PropTag.Message.InstanceKey);
			FunctionPropertyMapping value10 = new FunctionPropertyMapping(PropTag.Message.InstanceKey, column17, null, new Func<object[], object>(PropertySchemaPopulation.ComputeMessageInstanceKey), new PropertyMapping[]
			{
				conversionPropertyMapping2,
				conversionPropertyMapping,
				computedPropertyMapping
			}, false, true, true);
			dictionary.Add(PropTag.Message.InstanceKey, value10);
			Column column18 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructFunctionColumn(PropTag.Message.InstanceKeySvrEid, typeof(byte[]), 21, 0, messageTable.Table, new Func<object[], object>(PropertySchemaPopulation.ComputeMessageInstanceKey), "Exchange.ComputeMessageInstanceKey", new Column[]
			{
				column7,
				column5,
				propertyColumn
			}), PropTag.Message.InstanceKeySvrEid);
			FunctionPropertyMapping value11 = new FunctionPropertyMapping(PropTag.Message.InstanceKeySvrEid, column18, null, new Func<object[], object>(PropertySchemaPopulation.ComputeMessageInstanceKey), new PropertyMapping[]
			{
				conversionPropertyMapping2,
				conversionPropertyMapping,
				computedPropertyMapping
			}, true, true, true);
			dictionary.Add(PropTag.Message.InstanceKeySvrEid, value11);
			Column column19;
			PropertyMapping propertyMapping6;
			if (messageTable.SourceKey != null)
			{
				column19 = Factory.CreateMappedPropertyColumn(messageTable.SourceKey, PropTag.Message.InternalSourceKey);
				propertyMapping6 = new PhysicalColumnPropertyMapping(PropTag.Message.InternalSourceKey, column19, null, null, null, (PhysicalColumn)column19.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column19 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.InternalSourceKey, null), PropTag.Message.InternalSourceKey);
				propertyMapping6 = new ConstantPropertyMapping(PropTag.Message.InternalSourceKey, column19, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.InternalSourceKey, propertyMapping6);
			Column[] dependOn = new Column[]
			{
				column19,
				column4
			};
			PropertyColumn column20 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SourceKey, rowPropBagCreator, dependOn);
			ComputedPropertyMapping value12 = new ComputedPropertyMapping(PropTag.Message.SourceKey, column20, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSourceKey), new StorePropTag[]
			{
				PropTag.Message.InternalSourceKey,
				PropTag.Message.MidBin
			}, new PropertyMapping[]
			{
				propertyMapping6,
				propertyMapping3
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.SourceKey, value12);
			Column column21 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.ParentSourceKey, typeof(byte[]), 22, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo22ByteForm), "Exchange.ConvertExchangeIdTo22ByteForm", column6), PropTag.Message.ParentSourceKey);
			ConversionPropertyMapping value13 = new ConversionPropertyMapping(PropTag.Message.ParentSourceKey, column21, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo22ByteForm), PropTag.Message.FidBin, propertyMapping4, null, null, null, true, true, true);
			dictionary.Add(PropTag.Message.ParentSourceKey, value13);
			Column column22;
			PropertyMapping propertyMapping7;
			if (messageTable.LcnCurrent != null)
			{
				column22 = Factory.CreateMappedPropertyColumn(messageTable.LcnCurrent, PropTag.Message.ChangeNumberBin);
				propertyMapping7 = new PhysicalColumnPropertyMapping(PropTag.Message.ChangeNumberBin, column22, null, null, null, (PhysicalColumn)column22.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column22 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.ChangeNumberBin, null), PropTag.Message.ChangeNumberBin);
				propertyMapping7 = new ConstantPropertyMapping(PropTag.Message.ChangeNumberBin, column22, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.ChangeNumberBin, propertyMapping7);
			Column column23 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.ChangeNumber, typeof(long), 8, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), "Exchange.ConvertExchangeIdToInt64", column22), PropTag.Message.ChangeNumber);
			ConversionPropertyMapping conversionPropertyMapping3 = new ConversionPropertyMapping(PropTag.Message.ChangeNumber, column23, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), PropTag.Message.ChangeNumberBin, propertyMapping7, null, null, null, true, true, false);
			dictionary.Add(PropTag.Message.ChangeNumber, conversionPropertyMapping3);
			Column column24 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.Internal9ByteChangeNumber, typeof(byte[]), 9, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64To9ByteForm), "Exchange.ConvertInt64To9ByteForm", column23), PropTag.Message.Internal9ByteChangeNumber);
			ConversionPropertyMapping value14 = new ConversionPropertyMapping(PropTag.Message.Internal9ByteChangeNumber, column24, new Func<object, object>(PropertySchemaPopulation.ConvertInt64To9ByteForm), PropTag.Message.ChangeNumber, conversionPropertyMapping3, null, null, null, false, true, false);
			dictionary.Add(PropTag.Message.Internal9ByteChangeNumber, value14);
			Column column25;
			PropertyMapping propertyMapping8;
			if (messageTable.ChangeKey != null)
			{
				column25 = Factory.CreateMappedPropertyColumn(messageTable.ChangeKey, PropTag.Message.InternalChangeKey);
				propertyMapping8 = new PhysicalColumnPropertyMapping(PropTag.Message.InternalChangeKey, column25, null, null, null, (PhysicalColumn)column25.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column25 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.InternalChangeKey, null), PropTag.Message.InternalChangeKey);
				propertyMapping8 = new ConstantPropertyMapping(PropTag.Message.InternalChangeKey, column25, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.InternalChangeKey, propertyMapping8);
			Column[] dependOn2 = new Column[]
			{
				column25,
				column22
			};
			PropertyColumn column26 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ChangeKey, rowPropBagCreator, dependOn2);
			ComputedPropertyMapping value15 = new ComputedPropertyMapping(PropTag.Message.ChangeKey, column26, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageChangeKey), new StorePropTag[]
			{
				PropTag.Message.InternalChangeKey,
				PropTag.Message.ChangeNumberBin
			}, new PropertyMapping[]
			{
				propertyMapping8,
				propertyMapping7
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.ChangeKey, value15);
			Column column27 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.CnExport, typeof(byte[]), 24, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo24ByteForm), "Exchange.ConvertExchangeIdTo24ByteForm", column22), PropTag.Message.CnExport);
			ConversionPropertyMapping value16 = new ConversionPropertyMapping(PropTag.Message.CnExport, column27, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo24ByteForm), PropTag.Message.ChangeNumberBin, propertyMapping7, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageCnExport), null, null, true, true, false);
			dictionary.Add(PropTag.Message.CnExport, value16);
			Column column28 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.CnMvExport, typeof(byte[]), 24, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo24ByteForm), "Exchange.ConvertExchangeIdTo24ByteForm", column22), PropTag.Message.CnMvExport);
			ConversionPropertyMapping value17 = new ConversionPropertyMapping(PropTag.Message.CnMvExport, column28, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo24ByteForm), PropTag.Message.ChangeNumberBin, propertyMapping7, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageCnMvExport), null, null, true, true, false);
			dictionary.Add(PropTag.Message.CnMvExport, value17);
			Column column29;
			PropertyMapping propertyMapping9;
			if (messageTable.LcnReadUnread != null)
			{
				column29 = Factory.CreateMappedPropertyColumn(messageTable.LcnReadUnread, PropTag.Message.ReadCnNewBin);
				propertyMapping9 = new PhysicalColumnPropertyMapping(PropTag.Message.ReadCnNewBin, column29, null, null, null, (PhysicalColumn)column29.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column29 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.ReadCnNewBin, null), PropTag.Message.ReadCnNewBin);
				propertyMapping9 = new ConstantPropertyMapping(PropTag.Message.ReadCnNewBin, column29, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.ReadCnNewBin, propertyMapping9);
			Column column30 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.ReadCnNew, typeof(long), 8, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), "Exchange.ConvertExchangeIdToInt64", column29), PropTag.Message.ReadCnNew);
			ConversionPropertyMapping conversionPropertyMapping4 = new ConversionPropertyMapping(PropTag.Message.ReadCnNew, column30, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), PropTag.Message.ReadCnNewBin, propertyMapping9, null, null, null, true, true, false);
			dictionary.Add(PropTag.Message.ReadCnNew, conversionPropertyMapping4);
			Column column31 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.Internal9ByteReadCnNew, typeof(byte[]), 9, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64To9ByteForm), "Exchange.ConvertInt64To9ByteForm", column30), PropTag.Message.Internal9ByteReadCnNew);
			ConversionPropertyMapping value18 = new ConversionPropertyMapping(PropTag.Message.Internal9ByteReadCnNew, column31, new Func<object, object>(PropertySchemaPopulation.ConvertInt64To9ByteForm), PropTag.Message.ReadCnNew, conversionPropertyMapping4, null, null, null, false, true, false);
			dictionary.Add(PropTag.Message.Internal9ByteReadCnNew, value18);
			Column column32 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.ReadCnNewExport, typeof(byte[]), 24, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo24ByteForm), "Exchange.ConvertExchangeIdTo24ByteForm", column29), PropTag.Message.ReadCnNewExport);
			ConversionPropertyMapping value19 = new ConversionPropertyMapping(PropTag.Message.ReadCnNewExport, column32, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo24ByteForm), PropTag.Message.ReadCnNewBin, propertyMapping9, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReadCnNewExport), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadCnNewExport, value19);
			PropertyMapping value20;
			if (messageTable.GroupCns != null)
			{
				Column column33 = Factory.CreateMappedPropertyColumn(messageTable.GroupCns, PropTag.Message.PropGroupInfo);
				value20 = new PhysicalColumnPropertyMapping(PropTag.Message.PropGroupInfo, column33, null, null, null, (PhysicalColumn)column33.ActualColumn, true, true, true, false, false);
			}
			else
			{
				Column column33 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.PropGroupInfo, null), PropTag.Message.PropGroupInfo);
				value20 = new ConstantPropertyMapping(PropTag.Message.PropGroupInfo, column33, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.PropGroupInfo, value20);
			Column column34 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.PropertyGroupChangeMask, 0), PropTag.Message.PropertyGroupChangeMask);
			ConstantPropertyMapping value21 = new ConstantPropertyMapping(PropTag.Message.PropertyGroupChangeMask, column34, null, 0, false, true, false);
			dictionary.Add(PropTag.Message.PropertyGroupChangeMask, value21);
			PropertyColumn column35 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SearchKey, rowPropBagCreator, null);
			DefaultPropertyMapping value22 = new DefaultPropertyMapping(PropTag.Message.SearchKey, column35, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.SearchKey, value22);
			PropertyMapping value23;
			if (messageTable.RecipientList != null)
			{
				Column column36 = Factory.CreateMappedPropertyColumn(messageTable.RecipientList, PropTag.Message.MessageRecipientsMVBin);
				value23 = new PhysicalColumnPropertyMapping(PropTag.Message.MessageRecipientsMVBin, column36, null, null, null, (PhysicalColumn)column36.ActualColumn, true, false, false, false, false);
			}
			else
			{
				Column column36 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MessageRecipientsMVBin, null), PropTag.Message.MessageRecipientsMVBin);
				value23 = new ConstantPropertyMapping(PropTag.Message.MessageRecipientsMVBin, column36, null, null, false, false, false);
			}
			dictionary.Add(PropTag.Message.MessageRecipientsMVBin, value23);
			PropertyMapping value24;
			if (messageTable.SubobjectsBlob != null)
			{
				Column column37 = Factory.CreateMappedPropertyColumn(messageTable.SubobjectsBlob, PropTag.Message.ItemSubobjectsBin);
				value24 = new PhysicalColumnPropertyMapping(PropTag.Message.ItemSubobjectsBin, column37, null, null, null, (PhysicalColumn)column37.ActualColumn, true, false, false, false, false);
			}
			else
			{
				Column column37 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.ItemSubobjectsBin, null), PropTag.Message.ItemSubobjectsBin);
				value24 = new ConstantPropertyMapping(PropTag.Message.ItemSubobjectsBin, column37, null, null, false, false, false);
			}
			dictionary.Add(PropTag.Message.ItemSubobjectsBin, value24);
			Column column38;
			PropertyMapping propertyMapping10;
			if (messageTable.MessageClass != null)
			{
				column38 = Factory.CreateMappedPropertyColumn(messageTable.MessageClass, PropTag.Message.MessageClass);
				propertyMapping10 = new PhysicalColumnPropertyMapping(PropTag.Message.MessageClass, column38, null, null, null, (PhysicalColumn)column38.ActualColumn, true, true, true, true, false);
			}
			else
			{
				column38 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MessageClass, null), PropTag.Message.MessageClass);
				propertyMapping10 = new ConstantPropertyMapping(PropTag.Message.MessageClass, column38, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.MessageClass, propertyMapping10);
			PropertyMapping value25;
			if (messageTable.Importance != null)
			{
				Column column39 = Factory.CreateMappedPropertyColumn(messageTable.Importance, PropTag.Message.Importance);
				value25 = new PhysicalColumnPropertyMapping(PropTag.Message.Importance, column39, null, null, null, (PhysicalColumn)column39.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column39 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.Importance, null), PropTag.Message.Importance);
				value25 = new ConstantPropertyMapping(PropTag.Message.Importance, column39, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.Importance, value25);
			PropertyMapping value26;
			if (messageTable.Priority != null)
			{
				Column column40 = Factory.CreateMappedPropertyColumn(messageTable.Priority, PropTag.Message.Priority);
				value26 = new PhysicalColumnPropertyMapping(PropTag.Message.Priority, column40, null, null, null, (PhysicalColumn)column40.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column40 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.Priority, null), PropTag.Message.Priority);
				value26 = new ConstantPropertyMapping(PropTag.Message.Priority, column40, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.Priority, value26);
			Column column41;
			PropertyMapping propertyMapping11;
			if (messageTable.MailFlags != null)
			{
				column41 = Factory.CreateMappedPropertyColumn(messageTable.MailFlags, PropTag.Message.MailFlags);
				propertyMapping11 = new PhysicalColumnPropertyMapping(PropTag.Message.MailFlags, column41, null, null, null, (PhysicalColumn)column41.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column41 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MailFlags, null), PropTag.Message.MailFlags);
				propertyMapping11 = new ConstantPropertyMapping(PropTag.Message.MailFlags, column41, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.MailFlags, propertyMapping11);
			Column[] dependOn3 = new Column[]
			{
				column41
			};
			PropertyColumn column42 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptRequested, rowPropBagCreator, dependOn3);
			ComputedPropertyMapping value27 = new ComputedPropertyMapping(PropTag.Message.ReadReceiptRequested, column42, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReadReceiptRequested), new StorePropTag[]
			{
				PropTag.Message.MailFlags
			}, new PropertyMapping[]
			{
				propertyMapping11
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReadReceiptRequested), null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptRequested, value27);
			Column[] dependOn4 = new Column[]
			{
				column41
			};
			PropertyColumn column43 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.NonReceiptNotificationRequested, rowPropBagCreator, dependOn4);
			ComputedPropertyMapping value28 = new ComputedPropertyMapping(PropTag.Message.NonReceiptNotificationRequested, column43, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageNonReceiptNotificationRequested), new StorePropTag[]
			{
				PropTag.Message.MailFlags
			}, new PropertyMapping[]
			{
				propertyMapping11
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageNonReceiptNotificationRequested), null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.NonReceiptNotificationRequested, value28);
			PropertyMapping value29;
			if (messageTable.Sensitivity != null)
			{
				Column column44 = Factory.CreateMappedPropertyColumn(messageTable.Sensitivity, PropTag.Message.Sensitivity);
				value29 = new PhysicalColumnPropertyMapping(PropTag.Message.Sensitivity, column44, null, null, null, (PhysicalColumn)column44.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column44 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.Sensitivity, null), PropTag.Message.Sensitivity);
				value29 = new ConstantPropertyMapping(PropTag.Message.Sensitivity, column44, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.Sensitivity, value29);
			Column column45;
			PropertyMapping propertyMapping12;
			if (messageTable.SubjectPrefix != null)
			{
				column45 = Factory.CreateMappedPropertyColumn(messageTable.SubjectPrefix, PropTag.Message.SubjectPrefix);
				propertyMapping12 = new PhysicalColumnPropertyMapping(PropTag.Message.SubjectPrefix, column45, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSubjectPrefix), null, null, (PhysicalColumn)column45.ActualColumn, true, true, true, true, false);
			}
			else
			{
				column45 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.SubjectPrefix, null), PropTag.Message.SubjectPrefix);
				propertyMapping12 = new ConstantPropertyMapping(PropTag.Message.SubjectPrefix, column45, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.SubjectPrefix, propertyMapping12);
			PropertyColumn propertyColumn2 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.NormalizedSubject, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping = new DefaultPropertyMapping(PropTag.Message.NormalizedSubject, propertyColumn2, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageNormalizedSubject), null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.NormalizedSubject, defaultPropertyMapping);
			Column[] dependOn5 = new Column[]
			{
				column45,
				propertyColumn2
			};
			PropertyColumn column46 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.Subject, rowPropBagCreator, dependOn5);
			ComputedPropertyMapping value30 = new ComputedPropertyMapping(PropTag.Message.Subject, column46, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSubject), new StorePropTag[]
			{
				PropTag.Message.SubjectPrefix,
				PropTag.Message.NormalizedSubject
			}, new PropertyMapping[]
			{
				propertyMapping12,
				defaultPropertyMapping
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSubject), null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.Subject, value30);
			PropertyColumn propertyColumn3 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ConversationTopic, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping2 = new DefaultPropertyMapping(PropTag.Message.ConversationTopic, propertyColumn3, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.ConversationTopic, defaultPropertyMapping2);
			Column[] dependOn6 = new Column[]
			{
				propertyColumn3
			};
			PropertyColumn column47 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ConversationTopicHash, rowPropBagCreator, dependOn6);
			ComputedPropertyMapping value31 = new ComputedPropertyMapping(PropTag.Message.ConversationTopicHash, column47, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageConversationTopicHash), new StorePropTag[]
			{
				PropTag.Message.ConversationTopic
			}, new PropertyMapping[]
			{
				defaultPropertyMapping2
			}, null, null, null, false, true, false, false);
			dictionary.Add(PropTag.Message.ConversationTopicHash, value31);
			Column column48;
			PropertyMapping propertyMapping13;
			if (messageTable.ConversationIndexTracking != null)
			{
				column48 = Factory.CreateMappedPropertyColumn(messageTable.ConversationIndexTracking, PropTag.Message.InternalConversationIndexTracking);
				propertyMapping13 = new PhysicalColumnPropertyMapping(PropTag.Message.InternalConversationIndexTracking, column48, null, null, null, (PhysicalColumn)column48.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column48 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.InternalConversationIndexTracking, null), PropTag.Message.InternalConversationIndexTracking);
				propertyMapping13 = new ConstantPropertyMapping(PropTag.Message.InternalConversationIndexTracking, column48, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.InternalConversationIndexTracking, propertyMapping13);
			Column column49;
			PropertyMapping propertyMapping14;
			if (messageTable.ConversationIndex != null)
			{
				column49 = Factory.CreateMappedPropertyColumn(messageTable.ConversationIndex, PropTag.Message.InternalConversationIndex);
				propertyMapping14 = new PhysicalColumnPropertyMapping(PropTag.Message.InternalConversationIndex, column49, null, null, null, (PhysicalColumn)column49.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column49 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.InternalConversationIndex, null), PropTag.Message.InternalConversationIndex);
				propertyMapping14 = new ConstantPropertyMapping(PropTag.Message.InternalConversationIndex, column49, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.InternalConversationIndex, propertyMapping14);
			Column column50;
			PropertyMapping propertyMapping15;
			if (messageTable.ConversationId != null)
			{
				column50 = Factory.CreateMappedPropertyColumn(messageTable.ConversationId, PropTag.Message.ConversationItemConversationId);
				propertyMapping15 = new PhysicalColumnPropertyMapping(PropTag.Message.ConversationItemConversationId, column50, null, null, null, (PhysicalColumn)column50.ActualColumn, true, true, true, true, false);
			}
			else
			{
				column50 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.ConversationItemConversationId, null), PropTag.Message.ConversationItemConversationId);
				propertyMapping15 = new ConstantPropertyMapping(PropTag.Message.ConversationItemConversationId, column50, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.ConversationItemConversationId, propertyMapping15);
			Column[] dependOn7 = new Column[]
			{
				column49,
				column50
			};
			PropertyColumn column51 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ConversationIndex, rowPropBagCreator, dependOn7);
			ComputedPropertyMapping value32 = new ComputedPropertyMapping(PropTag.Message.ConversationIndex, column51, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageConversationIndex), new StorePropTag[]
			{
				PropTag.Message.InternalConversationIndex,
				PropTag.Message.ConversationItemConversationId
			}, new PropertyMapping[]
			{
				propertyMapping14,
				propertyMapping15
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageConversationIndex), null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.ConversationIndex, value32);
			Column[] dependOn8 = new Column[]
			{
				column48,
				column50
			};
			PropertyColumn column52 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ConversationIndexTracking, rowPropBagCreator, dependOn8);
			ComputedPropertyMapping value33 = new ComputedPropertyMapping(PropTag.Message.ConversationIndexTracking, column52, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageConversationIndexTracking), new StorePropTag[]
			{
				PropTag.Message.InternalConversationIndexTracking,
				PropTag.Message.ConversationItemConversationId
			}, new PropertyMapping[]
			{
				propertyMapping13,
				propertyMapping15
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageConversationIndexTracking), null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.ConversationIndexTracking, value33);
			PropertyColumn propertyColumn4 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.InternetMessageId, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping3 = new DefaultPropertyMapping(PropTag.Message.InternetMessageId, propertyColumn4, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.InternetMessageId, defaultPropertyMapping3);
			Column[] dependOn9 = new Column[]
			{
				propertyColumn4
			};
			PropertyColumn column53 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.InternetMessageIdHash, rowPropBagCreator, dependOn9);
			ComputedPropertyMapping value34 = new ComputedPropertyMapping(PropTag.Message.InternetMessageIdHash, column53, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageInternetMessageIdHash), new StorePropTag[]
			{
				PropTag.Message.InternetMessageId
			}, new PropertyMapping[]
			{
				defaultPropertyMapping3
			}, null, null, null, false, true, false, false);
			dictionary.Add(PropTag.Message.InternetMessageIdHash, value34);
			PropertyMapping value35;
			if (messageTable.ArticleNumber != null)
			{
				Column column54 = Factory.CreateMappedPropertyColumn(messageTable.ArticleNumber, PropTag.Message.InternetArticleNumber);
				value35 = new PhysicalColumnPropertyMapping(PropTag.Message.InternetArticleNumber, column54, null, null, null, (PhysicalColumn)column54.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column54 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.InternetArticleNumber, null), PropTag.Message.InternetArticleNumber);
				value35 = new ConstantPropertyMapping(PropTag.Message.InternetArticleNumber, column54, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.InternetArticleNumber, value35);
			PropertyMapping value36;
			if (messageTable.IMAPId != null)
			{
				Column column55 = Factory.CreateMappedPropertyColumn(messageTable.IMAPId, PropTag.Message.IMAPId);
				value36 = new PhysicalColumnPropertyMapping(PropTag.Message.IMAPId, column55, null, null, null, (PhysicalColumn)column55.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column55 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.IMAPId, null), PropTag.Message.IMAPId);
				value36 = new ConstantPropertyMapping(PropTag.Message.IMAPId, column55, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.IMAPId, value36);
			PropertyMapping value37;
			if (messageTable.CodePage != null)
			{
				Column column56 = Factory.CreateMappedPropertyColumn(messageTable.CodePage, PropTag.Message.MessageCodePage);
				value37 = new PhysicalColumnPropertyMapping(PropTag.Message.MessageCodePage, column56, null, null, null, (PhysicalColumn)column56.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column56 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MessageCodePage, null), PropTag.Message.MessageCodePage);
				value37 = new ConstantPropertyMapping(PropTag.Message.MessageCodePage, column56, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.MessageCodePage, value37);
			PropertyMapping value38;
			if (messageTable.CodePage != null)
			{
				Column column57 = Factory.CreateMappedPropertyColumn(messageTable.CodePage, PropTag.Message.CodePageId);
				value38 = new PhysicalColumnPropertyMapping(PropTag.Message.CodePageId, column57, null, null, null, (PhysicalColumn)column57.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column57 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.CodePageId, null), PropTag.Message.CodePageId);
				value38 = new ConstantPropertyMapping(PropTag.Message.CodePageId, column57, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.CodePageId, value38);
			PropertyMapping value39;
			if (messageTable.Status != null)
			{
				Column column58 = Factory.CreateMappedPropertyColumn(messageTable.Status, PropTag.Message.MsgStatus);
				value39 = new PhysicalColumnPropertyMapping(PropTag.Message.MsgStatus, column58, null, null, null, (PhysicalColumn)column58.ActualColumn, true, true, true, false, false);
			}
			else
			{
				Column column58 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MsgStatus, null), PropTag.Message.MsgStatus);
				value39 = new ConstantPropertyMapping(PropTag.Message.MsgStatus, column58, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.MsgStatus, value39);
			Column column59;
			PropertyMapping propertyMapping16;
			if (messageTable.IsRead != null)
			{
				column59 = Factory.CreateMappedPropertyColumn(messageTable.IsRead, PropTag.Message.IsReadColumn);
				propertyMapping16 = new PhysicalColumnPropertyMapping(PropTag.Message.IsReadColumn, column59, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageIsReadColumn), null, null, (PhysicalColumn)column59.ActualColumn, true, true, true, true, true);
			}
			else
			{
				column59 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.IsReadColumn, null), PropTag.Message.IsReadColumn);
				propertyMapping16 = new ConstantPropertyMapping(PropTag.Message.IsReadColumn, column59, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.IsReadColumn, propertyMapping16);
			Column column60;
			PropertyMapping propertyMapping17;
			if (messageTable.VirtualIsRead != null)
			{
				column60 = Factory.CreateMappedPropertyColumn(messageTable.VirtualIsRead, PropTag.Message.VirtualIsRead);
				propertyMapping17 = new PhysicalColumnPropertyMapping(PropTag.Message.VirtualIsRead, column60, null, null, null, (PhysicalColumn)column60.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column60 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.VirtualIsRead, null), PropTag.Message.VirtualIsRead);
				propertyMapping17 = new ConstantPropertyMapping(PropTag.Message.VirtualIsRead, column60, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.VirtualIsRead, propertyMapping17);
			Column[] dependOn10 = new Column[]
			{
				column60,
				column59,
				column23,
				column6
			};
			PropertyColumn propertyColumn5 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.Read, rowPropBagCreator, dependOn10);
			ComputedPropertyMapping computedPropertyMapping2 = new ComputedPropertyMapping(PropTag.Message.Read, propertyColumn5, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageRead), new StorePropTag[]
			{
				PropTag.Message.VirtualIsRead,
				PropTag.Message.IsReadColumn,
				PropTag.Message.ChangeNumber,
				PropTag.Message.FidBin
			}, new PropertyMapping[]
			{
				propertyMapping17,
				propertyMapping16,
				conversionPropertyMapping3,
				propertyMapping4
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageRead), null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.Read, computedPropertyMapping2);
			Column column61;
			PropertyMapping propertyMapping18;
			if (messageTable.IsHidden != null)
			{
				column61 = Factory.CreateMappedPropertyColumn(messageTable.IsHidden, PropTag.Message.Associated);
				propertyMapping18 = new PhysicalColumnPropertyMapping(PropTag.Message.Associated, column61, null, null, null, (PhysicalColumn)column61.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column61 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.Associated, null), PropTag.Message.Associated);
				propertyMapping18 = new ConstantPropertyMapping(PropTag.Message.Associated, column61, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.Associated, propertyMapping18);
			Column column62;
			PropertyMapping propertyMapping19;
			if (messageTable.HasAttachments != null)
			{
				column62 = Factory.CreateMappedPropertyColumn(messageTable.HasAttachments, PropTag.Message.HasAttach);
				propertyMapping19 = new PhysicalColumnPropertyMapping(PropTag.Message.HasAttach, column62, null, null, null, (PhysicalColumn)column62.ActualColumn, true, true, true, true, false);
			}
			else
			{
				column62 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.HasAttach, null), PropTag.Message.HasAttach);
				propertyMapping19 = new ConstantPropertyMapping(PropTag.Message.HasAttach, column62, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.HasAttach, propertyMapping19);
			Column column63;
			PropertyMapping propertyMapping20;
			if (messageTable.MessageFlagsActual != null)
			{
				column63 = Factory.CreateMappedPropertyColumn(messageTable.MessageFlagsActual, PropTag.Message.MessageFlagsActual);
				propertyMapping20 = new PhysicalColumnPropertyMapping(PropTag.Message.MessageFlagsActual, column63, null, null, null, (PhysicalColumn)column63.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column63 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MessageFlagsActual, null), PropTag.Message.MessageFlagsActual);
				propertyMapping20 = new ConstantPropertyMapping(PropTag.Message.MessageFlagsActual, column63, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.MessageFlagsActual, propertyMapping20);
			PropertyColumn propertyColumn6 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SubmitResponsibility, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping4 = new DefaultPropertyMapping(PropTag.Message.SubmitResponsibility, propertyColumn6, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.SubmitResponsibility, defaultPropertyMapping4);
			Column[] dependOn11 = new Column[]
			{
				column63,
				propertyColumn6
			};
			PropertyColumn column64 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SubmitFlags, rowPropBagCreator, dependOn11);
			ComputedPropertyMapping value40 = new ComputedPropertyMapping(PropTag.Message.SubmitFlags, column64, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSubmitFlags), new StorePropTag[]
			{
				PropTag.Message.MessageFlagsActual,
				PropTag.Message.SubmitResponsibility
			}, new PropertyMapping[]
			{
				propertyMapping20,
				defaultPropertyMapping4
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.SubmitFlags, value40);
			Column[] dependOn12 = new Column[]
			{
				column62,
				column61,
				propertyColumn5,
				column63,
				column41
			};
			PropertyColumn column65 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.MessageFlags, rowPropBagCreator, dependOn12);
			ComputedPropertyMapping value41 = new ComputedPropertyMapping(PropTag.Message.MessageFlags, column65, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageMessageFlags), new StorePropTag[]
			{
				PropTag.Message.HasAttach,
				PropTag.Message.Associated,
				PropTag.Message.Read,
				PropTag.Message.MessageFlagsActual,
				PropTag.Message.MailFlags
			}, new PropertyMapping[]
			{
				propertyMapping19,
				propertyMapping18,
				computedPropertyMapping2,
				propertyMapping20,
				propertyMapping11
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageMessageFlags), null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.MessageFlags, value41);
			Column[] dependOn13 = new Column[]
			{
				column63
			};
			PropertyColumn column66 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.IsIRMMessage, rowPropBagCreator, dependOn13);
			ComputedPropertyMapping value42 = new ComputedPropertyMapping(PropTag.Message.IsIRMMessage, column66, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageIsIRMMessage), new StorePropTag[]
			{
				PropTag.Message.MessageFlagsActual
			}, new PropertyMapping[]
			{
				propertyMapping20
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.IsIRMMessage, value42);
			Column[] dependOn14 = new Column[]
			{
				column61,
				column15,
				column49,
				column48,
				column50,
				propertyColumn3,
				column38
			};
			PropertyColumn propertyColumn7 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ConversationId, rowPropBagCreator, dependOn14);
			ComputedPropertyMapping computedPropertyMapping3 = new ComputedPropertyMapping(PropTag.Message.ConversationId, propertyColumn7, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageConversationId), new StorePropTag[]
			{
				PropTag.Message.Associated,
				PropTag.Message.DocumentId,
				PropTag.Message.InternalConversationIndex,
				PropTag.Message.InternalConversationIndexTracking,
				PropTag.Message.ConversationItemConversationId,
				PropTag.Message.ConversationTopic,
				PropTag.Message.MessageClass
			}, new PropertyMapping[]
			{
				propertyMapping18,
				propertyMapping5,
				propertyMapping14,
				propertyMapping13,
				propertyMapping15,
				defaultPropertyMapping2,
				propertyMapping10
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.ConversationId, computedPropertyMapping3);
			Column[] dependOn15 = new Column[]
			{
				propertyColumn7
			};
			PropertyColumn column67 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ConversationFamilyId, rowPropBagCreator, dependOn15);
			ComputedPropertyMapping value43 = new ComputedPropertyMapping(PropTag.Message.ConversationFamilyId, column67, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageConversationFamilyId), new StorePropTag[]
			{
				PropTag.Message.ConversationId
			}, new PropertyMapping[]
			{
				computedPropertyMapping3
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageConversationFamilyId), null, null, true, true, true, true);
			dictionary.Add(PropTag.Message.ConversationFamilyId, value43);
			Column[] dependOn16 = new Column[]
			{
				propertyColumn7
			};
			PropertyColumn column68 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ConversationIdHash, rowPropBagCreator, dependOn16);
			ComputedPropertyMapping value44 = new ComputedPropertyMapping(PropTag.Message.ConversationIdHash, column68, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageConversationIdHash), new StorePropTag[]
			{
				PropTag.Message.ConversationId
			}, new PropertyMapping[]
			{
				computedPropertyMapping3
			}, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Message.ConversationIdHash, value44);
			PropertyColumn column69 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.HasNamedProperties, rowPropBagCreator, null);
			ComputedPropertyMapping value45 = new ComputedPropertyMapping(PropTag.Message.HasNamedProperties, column69, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageHasNamedProperties), null, null, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Message.HasNamedProperties, value45);
			Column column70;
			PropertyMapping propertyMapping21;
			if (messageTable.Size != null)
			{
				column70 = Factory.CreateMappedPropertyColumn(messageTable.Size, PropTag.Message.MessageSize);
				propertyMapping21 = new PhysicalColumnPropertyMapping(PropTag.Message.MessageSize, column70, null, null, null, (PhysicalColumn)column70.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column70 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MessageSize, null), PropTag.Message.MessageSize);
				propertyMapping21 = new ConstantPropertyMapping(PropTag.Message.MessageSize, column70, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.MessageSize, propertyMapping21);
			PropertyColumn column71 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RowType, rowPropBagCreator, null);
			ComputedPropertyMapping value46 = new ComputedPropertyMapping(PropTag.Message.RowType, column71, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageRowType), null, null, null, null, null, false, false, true, false);
			dictionary.Add(PropTag.Message.RowType, value46);
			Column column72 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.MessageSize32, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column70), PropTag.Message.MessageSize32);
			ConversionPropertyMapping value47 = new ConversionPropertyMapping(PropTag.Message.MessageSize32, column72, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Message.MessageSize, propertyMapping21, null, null, null, true, true, true);
			dictionary.Add(PropTag.Message.MessageSize32, value47);
			Column column73;
			PropertyMapping propertyMapping22;
			if (messageTable.BodyType != null)
			{
				column73 = Factory.CreateMappedPropertyColumn(messageTable.BodyType, PropTag.Message.NativeBodyType);
				propertyMapping22 = new PhysicalColumnPropertyMapping(PropTag.Message.NativeBodyType, column73, null, null, null, (PhysicalColumn)column73.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column73 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.NativeBodyType, null), PropTag.Message.NativeBodyType);
				propertyMapping22 = new ConstantPropertyMapping(PropTag.Message.NativeBodyType, column73, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.NativeBodyType, propertyMapping22);
			Column column74;
			PropertyMapping propertyMapping23;
			if (messageTable.NativeBody != null)
			{
				column74 = Factory.CreateMappedPropertyColumn(messageTable.NativeBody, PropTag.Message.NativeBody);
				propertyMapping23 = new PhysicalColumnPropertyMapping(PropTag.Message.NativeBody, column74, null, null, null, (PhysicalColumn)column74.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column74 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.NativeBody, null), PropTag.Message.NativeBody);
				propertyMapping23 = new ConstantPropertyMapping(PropTag.Message.NativeBody, column74, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.NativeBody, propertyMapping23);
			Column[] dependOn17 = new Column[]
			{
				column73
			};
			PropertyColumn column75 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.NativeBodyInfo, rowPropBagCreator, dependOn17);
			ComputedPropertyMapping value48 = new ComputedPropertyMapping(PropTag.Message.NativeBodyInfo, column75, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageNativeBodyInfo), new StorePropTag[]
			{
				PropTag.Message.NativeBodyType
			}, new PropertyMapping[]
			{
				propertyMapping22
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.NativeBodyInfo, value48);
			Column[] dependOn18 = new Column[]
			{
				column73,
				column74
			};
			PropertyColumn column76 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.BodyUnicode, rowPropBagCreator, dependOn18);
			ComputedPropertyMapping value49 = new ComputedPropertyMapping(PropTag.Message.BodyUnicode, column76, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageBodyUnicode), new StorePropTag[]
			{
				PropTag.Message.NativeBodyType,
				PropTag.Message.NativeBody
			}, new PropertyMapping[]
			{
				propertyMapping22,
				propertyMapping23
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageBodyUnicode), new StreamGetterDelegate(PropertySchemaPopulation.GetMessageBodyUnicodeReadStream), new StreamGetterDelegate(PropertySchemaPopulation.GetMessageBodyUnicodeWriteStream), false, true, true, true);
			dictionary.Add(PropTag.Message.BodyUnicode, value49);
			Column[] dependOn19 = new Column[]
			{
				column73,
				column74
			};
			PropertyColumn column77 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RtfCompressed, rowPropBagCreator, dependOn19);
			ComputedPropertyMapping value50 = new ComputedPropertyMapping(PropTag.Message.RtfCompressed, column77, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageRtfCompressed), new StorePropTag[]
			{
				PropTag.Message.NativeBodyType,
				PropTag.Message.NativeBody
			}, new PropertyMapping[]
			{
				propertyMapping22,
				propertyMapping23
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageRtfCompressed), new StreamGetterDelegate(PropertySchemaPopulation.GetMessageRtfCompressedReadStream), new StreamGetterDelegate(PropertySchemaPopulation.GetMessageRtfCompressedWriteStream), false, true, true, true);
			dictionary.Add(PropTag.Message.RtfCompressed, value50);
			Column[] dependOn20 = new Column[]
			{
				column73,
				column74
			};
			PropertyColumn column78 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.BodyHtml, rowPropBagCreator, dependOn20);
			ComputedPropertyMapping value51 = new ComputedPropertyMapping(PropTag.Message.BodyHtml, column78, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageBodyHtml), new StorePropTag[]
			{
				PropTag.Message.NativeBodyType,
				PropTag.Message.NativeBody
			}, new PropertyMapping[]
			{
				propertyMapping22,
				propertyMapping23
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageBodyHtml), new StreamGetterDelegate(PropertySchemaPopulation.GetMessageBodyHtmlReadStream), new StreamGetterDelegate(PropertySchemaPopulation.GetMessageBodyHtmlWriteStream), false, true, true, true);
			dictionary.Add(PropTag.Message.BodyHtml, value51);
			Column[] dependOn21 = new Column[]
			{
				column73
			};
			PropertyColumn column79 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RTFInSync, rowPropBagCreator, dependOn21);
			ComputedPropertyMapping value52 = new ComputedPropertyMapping(PropTag.Message.RTFInSync, column79, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageRTFInSync), new StorePropTag[]
			{
				PropTag.Message.NativeBodyType
			}, new PropertyMapping[]
			{
				propertyMapping22
			}, null, null, null, true, true, true, true);
			dictionary.Add(PropTag.Message.RTFInSync, value52);
			PropertyColumn column80 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.Depth, rowPropBagCreator, null);
			ComputedPropertyMapping value53 = new ComputedPropertyMapping(PropTag.Message.Depth, column80, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageDepth), null, null, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Message.Depth, value53);
			PropertyColumn column81 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ContentCount, rowPropBagCreator, null);
			ComputedPropertyMapping value54 = new ComputedPropertyMapping(PropTag.Message.ContentCount, column81, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageContentCount), null, null, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Message.ContentCount, value54);
			PropertyColumn column82 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CategID, rowPropBagCreator, null);
			ComputedPropertyMapping value55 = new ComputedPropertyMapping(PropTag.Message.CategID, column82, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageCategID), null, null, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Message.CategID, value55);
			Column column83;
			PropertyMapping propertyMapping24;
			if (messageTable.VirtualUnreadMessageCount != null)
			{
				column83 = Factory.CreateMappedPropertyColumn(messageTable.VirtualUnreadMessageCount, PropTag.Message.VirtualUnreadMessageCount);
				propertyMapping24 = new PhysicalColumnPropertyMapping(PropTag.Message.VirtualUnreadMessageCount, column83, null, null, null, (PhysicalColumn)column83.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column83 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.VirtualUnreadMessageCount, null), PropTag.Message.VirtualUnreadMessageCount);
				propertyMapping24 = new ConstantPropertyMapping(PropTag.Message.VirtualUnreadMessageCount, column83, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.VirtualUnreadMessageCount, propertyMapping24);
			Column column84 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.UnreadCount, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column83), PropTag.Message.UnreadCount);
			ConversionPropertyMapping value56 = new ConversionPropertyMapping(PropTag.Message.UnreadCount, column84, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Message.VirtualUnreadMessageCount, propertyMapping24, null, null, null, true, true, false);
			dictionary.Add(PropTag.Message.UnreadCount, value56);
			PropertyColumn propertyColumn8 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.Preview, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping5 = new DefaultPropertyMapping(PropTag.Message.Preview, propertyColumn8, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.Preview, defaultPropertyMapping5);
			Column[] dependOn22 = new Column[]
			{
				propertyColumn5,
				propertyColumn8
			};
			PropertyColumn column85 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.PreviewUnread, rowPropBagCreator, dependOn22);
			ComputedPropertyMapping value57 = new ComputedPropertyMapping(PropTag.Message.PreviewUnread, column85, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessagePreviewUnread), new StorePropTag[]
			{
				PropTag.Message.Read,
				PropTag.Message.Preview
			}, new PropertyMapping[]
			{
				computedPropertyMapping2,
				defaultPropertyMapping5
			}, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Message.PreviewUnread, value57);
			PropertyMapping value58;
			if (messageTable.DateCreated != null)
			{
				Column column86 = Factory.CreateMappedPropertyColumn(messageTable.DateCreated, PropTag.Message.CreationTime);
				value58 = new PhysicalColumnPropertyMapping(PropTag.Message.CreationTime, column86, null, null, null, (PhysicalColumn)column86.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column86 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.CreationTime, null), PropTag.Message.CreationTime);
				value58 = new ConstantPropertyMapping(PropTag.Message.CreationTime, column86, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.CreationTime, value58);
			PropertyMapping value59;
			if (messageTable.LastModificationTime != null)
			{
				Column column87 = Factory.CreateMappedPropertyColumn(messageTable.LastModificationTime, PropTag.Message.LastModificationTime);
				value59 = new PhysicalColumnPropertyMapping(PropTag.Message.LastModificationTime, column87, null, null, null, (PhysicalColumn)column87.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column87 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.LastModificationTime, null), PropTag.Message.LastModificationTime);
				value59 = new ConstantPropertyMapping(PropTag.Message.LastModificationTime, column87, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.LastModificationTime, value59);
			Column column88;
			PropertyMapping propertyMapping25;
			if (messageTable.VersionHistory != null)
			{
				column88 = Factory.CreateMappedPropertyColumn(messageTable.VersionHistory, PropTag.Message.PCL);
				propertyMapping25 = new PhysicalColumnPropertyMapping(PropTag.Message.PCL, column88, null, null, null, (PhysicalColumn)column88.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column88 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.PCL, null), PropTag.Message.PCL);
				propertyMapping25 = new ConstantPropertyMapping(PropTag.Message.PCL, column88, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.PCL, propertyMapping25);
			PropertyMapping value60;
			if (messageTable.VersionHistory != null)
			{
				Column column89 = Factory.CreateMappedPropertyColumn(messageTable.VersionHistory, PropTag.Message.PredecessorChangeList);
				value60 = new PhysicalColumnPropertyMapping(PropTag.Message.PredecessorChangeList, column89, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessagePredecessorChangeList), null, null, (PhysicalColumn)column89.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column89 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.PredecessorChangeList, null), PropTag.Message.PredecessorChangeList);
				value60 = new ConstantPropertyMapping(PropTag.Message.PredecessorChangeList, column89, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.PredecessorChangeList, value60);
			Column column90 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.PclExport, typeof(byte[]), 0, 1073741823, table, new Func<object, object>(PropertySchemaPopulation.ConvertLXCNArrayToLTIDArray), "Exchange.ConvertLXCNArrayToLTIDArray", column88), PropTag.Message.PclExport);
			ConversionPropertyMapping value61 = new ConversionPropertyMapping(PropTag.Message.PclExport, column90, new Func<object, object>(PropertySchemaPopulation.ConvertLXCNArrayToLTIDArray), PropTag.Message.PCL, propertyMapping25, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessagePclExport), null, null, true, true, false);
			dictionary.Add(PropTag.Message.PclExport, value61);
			Column column91 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.DeletedOn, null), PropTag.Message.DeletedOn);
			ConstantPropertyMapping value62 = new ConstantPropertyMapping(PropTag.Message.DeletedOn, column91, null, null, true, true, true);
			dictionary.Add(PropTag.Message.DeletedOn, value62);
			PropertyMapping value63;
			if (messageTable.DateSent != null)
			{
				Column column92 = Factory.CreateMappedPropertyColumn(messageTable.DateSent, PropTag.Message.ClientSubmitTime);
				value63 = new PhysicalColumnPropertyMapping(PropTag.Message.ClientSubmitTime, column92, null, null, null, (PhysicalColumn)column92.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column92 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.ClientSubmitTime, null), PropTag.Message.ClientSubmitTime);
				value63 = new ConstantPropertyMapping(PropTag.Message.ClientSubmitTime, column92, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.ClientSubmitTime, value63);
			Column column93;
			PropertyMapping propertyMapping26;
			if (messageTable.DateReceived != null)
			{
				column93 = Factory.CreateMappedPropertyColumn(messageTable.DateReceived, PropTag.Message.MessageDeliveryTime);
				propertyMapping26 = new PhysicalColumnPropertyMapping(PropTag.Message.MessageDeliveryTime, column93, null, null, null, (PhysicalColumn)column93.ActualColumn, true, true, true, true, false);
			}
			else
			{
				column93 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MessageDeliveryTime, null), PropTag.Message.MessageDeliveryTime);
				propertyMapping26 = new ConstantPropertyMapping(PropTag.Message.MessageDeliveryTime, column93, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.MessageDeliveryTime, propertyMapping26);
			PropertyColumn column94 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.DisplayBcc, rowPropBagCreator, null);
			DefaultPropertyMapping value64 = new DefaultPropertyMapping(PropTag.Message.DisplayBcc, column94, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.DisplayBcc, value64);
			PropertyColumn column95 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.DisplayCc, rowPropBagCreator, null);
			DefaultPropertyMapping value65 = new DefaultPropertyMapping(PropTag.Message.DisplayCc, column95, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.DisplayCc, value65);
			PropertyMapping value66;
			if (messageTable.DisplayTo != null)
			{
				Column column96 = Factory.CreateMappedPropertyColumn(messageTable.DisplayTo, PropTag.Message.DisplayTo);
				value66 = new PhysicalColumnPropertyMapping(PropTag.Message.DisplayTo, column96, null, null, null, (PhysicalColumn)column96.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column96 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.DisplayTo, null), PropTag.Message.DisplayTo);
				value66 = new ConstantPropertyMapping(PropTag.Message.DisplayTo, column96, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.DisplayTo, value66);
			PropertyColumn propertyColumn9 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RenewTime, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping6 = new DefaultPropertyMapping(PropTag.Message.RenewTime, propertyColumn9, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.RenewTime, defaultPropertyMapping6);
			Column[] dependOn23 = new Column[]
			{
				propertyColumn9,
				column93
			};
			PropertyColumn column97 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.DeliveryOrRenewTime, rowPropBagCreator, dependOn23);
			ComputedPropertyMapping value67 = new ComputedPropertyMapping(PropTag.Message.DeliveryOrRenewTime, column97, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageDeliveryOrRenewTime), new StorePropTag[]
			{
				PropTag.Message.RenewTime,
				PropTag.Message.MessageDeliveryTime
			}, new PropertyMapping[]
			{
				defaultPropertyMapping6,
				propertyMapping26
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.DeliveryOrRenewTime, value67);
			PropertyColumn column98 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RichContentDeprecated, rowPropBagCreator, null);
			DefaultPropertyMapping value68 = new DefaultPropertyMapping(PropTag.Message.RichContentDeprecated, column98, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.RichContentDeprecated, value68);
			PropertyColumn column99 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RichContent, rowPropBagCreator, null);
			DefaultPropertyMapping value69 = new DefaultPropertyMapping(PropTag.Message.RichContent, column99, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.RichContent, value69);
			PropertyColumn column100 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ToGroupExpansionRecipients, rowPropBagCreator, null);
			ComputedPropertyMapping value70 = new ComputedPropertyMapping(PropTag.Message.ToGroupExpansionRecipients, column100, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageToGroupExpansionRecipients), null, null, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.ToGroupExpansionRecipients, value70);
			PropertyColumn column101 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CcGroupExpansionRecipients, rowPropBagCreator, null);
			ComputedPropertyMapping value71 = new ComputedPropertyMapping(PropTag.Message.CcGroupExpansionRecipients, column101, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageCcGroupExpansionRecipients), null, null, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.CcGroupExpansionRecipients, value71);
			PropertyColumn column102 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.BccGroupExpansionRecipients, rowPropBagCreator, null);
			ComputedPropertyMapping value72 = new ComputedPropertyMapping(PropTag.Message.BccGroupExpansionRecipients, column102, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageBccGroupExpansionRecipients), null, null, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.BccGroupExpansionRecipients, value72);
			PropertyMapping value73;
			if (messageTable.AnnotationToken != null)
			{
				Column column103 = Factory.CreateMappedPropertyColumn(messageTable.AnnotationToken, PropTag.Message.AnnotationToken);
				value73 = new PhysicalColumnPropertyMapping(PropTag.Message.AnnotationToken, column103, null, null, null, (PhysicalColumn)column103.ActualColumn, true, true, true, false, false);
			}
			else
			{
				Column column103 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.AnnotationToken, null), PropTag.Message.AnnotationToken);
				value73 = new ConstantPropertyMapping(PropTag.Message.AnnotationToken, column103, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.AnnotationToken, value73);
			Column column104 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.ChangeType, 2), PropTag.Message.ChangeType);
			ConstantPropertyMapping value74 = new ConstantPropertyMapping(PropTag.Message.ChangeType, column104, null, 2, false, true, false);
			dictionary.Add(PropTag.Message.ChangeType, value74);
			PropertyMapping value75;
			if (messageTable.UserConfigurationXmlStream != null)
			{
				Column column105 = Factory.CreateMappedPropertyColumn(messageTable.UserConfigurationXmlStream, PropTag.Message.UserConfigurationXmlStream);
				value75 = new PhysicalColumnPropertyMapping(PropTag.Message.UserConfigurationXmlStream, column105, null, null, null, (PhysicalColumn)column105.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column105 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.UserConfigurationXmlStream, null), PropTag.Message.UserConfigurationXmlStream);
				value75 = new ConstantPropertyMapping(PropTag.Message.UserConfigurationXmlStream, column105, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.UserConfigurationXmlStream, value75);
			PropertyMapping value76;
			if (messageTable.UserConfigurationStream != null)
			{
				Column column106 = Factory.CreateMappedPropertyColumn(messageTable.UserConfigurationStream, PropTag.Message.UserConfigurationStream);
				value76 = new PhysicalColumnPropertyMapping(PropTag.Message.UserConfigurationStream, column106, null, null, null, (PhysicalColumn)column106.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column106 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.UserConfigurationStream, null), PropTag.Message.UserConfigurationStream);
				value76 = new ConstantPropertyMapping(PropTag.Message.UserConfigurationStream, column106, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.UserConfigurationStream, value76);
			PropertyColumn column107 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.PredictedActions, rowPropBagCreator, null);
			DefaultPropertyMapping value77 = new DefaultPropertyMapping(PropTag.Message.PredictedActions, column107, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.PredictedActions, value77);
			PropertyColumn column108 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.IsClutter, rowPropBagCreator, null);
			DefaultPropertyMapping value78 = new DefaultPropertyMapping(PropTag.Message.IsClutter, column108, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Message.IsClutter, value78);
			PropertyColumn column109 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.FlagStatus, rowPropBagCreator, null);
			DefaultPropertyMapping value79 = new DefaultPropertyMapping(PropTag.Message.FlagStatus, column109, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.FlagStatus, value79);
			Column column110;
			PropertyMapping propertyMapping27;
			if (messageTable.VirtualParentDisplay != null)
			{
				column110 = Factory.CreateMappedPropertyColumn(messageTable.VirtualParentDisplay, PropTag.Message.VirtualParentDisplay);
				propertyMapping27 = new PhysicalColumnPropertyMapping(PropTag.Message.VirtualParentDisplay, column110, null, null, null, (PhysicalColumn)column110.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column110 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.VirtualParentDisplay, null), PropTag.Message.VirtualParentDisplay);
				propertyMapping27 = new ConstantPropertyMapping(PropTag.Message.VirtualParentDisplay, column110, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.VirtualParentDisplay, propertyMapping27);
			Column[] dependOn24 = new Column[]
			{
				column110,
				column7
			};
			PropertyColumn column111 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ParentDisplay, rowPropBagCreator, dependOn24);
			ComputedPropertyMapping value80 = new ComputedPropertyMapping(PropTag.Message.ParentDisplay, column111, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageParentDisplay), new StorePropTag[]
			{
				PropTag.Message.VirtualParentDisplay,
				PropTag.Message.Fid
			}, new PropertyMapping[]
			{
				propertyMapping27,
				conversionPropertyMapping2
			}, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Message.ParentDisplay, value80);
			PropertyColumn propertyColumn10 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping4 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingFlags, propertyColumn10, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSentRepresentingFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSentRepresentingFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingFlags, computedPropertyMapping4);
			Column[] dependOn25 = new Column[]
			{
				propertyColumn10
			};
			PropertyColumn propertyColumn11 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingEntryId, rowPropBagCreator, dependOn25);
			ComputedPropertyMapping computedPropertyMapping5 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingEntryId, propertyColumn11, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSentRepresentingEntryId), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping4
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSentRepresentingEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingEntryId, computedPropertyMapping5);
			Column[] dependOn26 = new Column[]
			{
				propertyColumn10,
				propertyColumn11
			};
			PropertyColumn propertyColumn12 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingAddressType, rowPropBagCreator, dependOn26);
			ComputedPropertyMapping computedPropertyMapping6 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingAddressType, propertyColumn12, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSentRepresentingAddressType), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags,
				PropTag.Message.SentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping4,
				computedPropertyMapping5
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSentRepresentingAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingAddressType, computedPropertyMapping6);
			Column[] dependOn27 = new Column[]
			{
				propertyColumn10,
				propertyColumn11
			};
			PropertyColumn propertyColumn13 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingEmailAddress, rowPropBagCreator, dependOn27);
			ComputedPropertyMapping computedPropertyMapping7 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingEmailAddress, propertyColumn13, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSentRepresentingEmailAddress), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags,
				PropTag.Message.SentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping4,
				computedPropertyMapping5
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSentRepresentingEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingEmailAddress, computedPropertyMapping7);
			Column[] dependOn28 = new Column[]
			{
				propertyColumn10,
				propertyColumn12,
				propertyColumn13,
				propertyColumn11
			};
			PropertyColumn propertyColumn14 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingSearchKey, rowPropBagCreator, dependOn28);
			ComputedPropertyMapping computedPropertyMapping8 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingSearchKey, propertyColumn14, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSentRepresentingSearchKey), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags,
				PropTag.Message.SentRepresentingAddressType,
				PropTag.Message.SentRepresentingEmailAddress,
				PropTag.Message.SentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping4,
				computedPropertyMapping6,
				computedPropertyMapping7,
				computedPropertyMapping5
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSentRepresentingSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingSearchKey, computedPropertyMapping8);
			Column[] dependOn29 = new Column[]
			{
				propertyColumn10,
				propertyColumn11,
				propertyColumn13
			};
			PropertyColumn propertyColumn15 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingSimpleDisplayName, rowPropBagCreator, dependOn29);
			ComputedPropertyMapping computedPropertyMapping9 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingSimpleDisplayName, propertyColumn15, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSentRepresentingSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags,
				PropTag.Message.SentRepresentingEntryId,
				PropTag.Message.SentRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping4,
				computedPropertyMapping5,
				computedPropertyMapping7
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSentRepresentingSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingSimpleDisplayName, computedPropertyMapping9);
			Column[] dependOn30 = new Column[]
			{
				propertyColumn10,
				propertyColumn11,
				propertyColumn15,
				propertyColumn13
			};
			PropertyColumn propertyColumn16 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingName, rowPropBagCreator, dependOn30);
			ComputedPropertyMapping computedPropertyMapping10 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingName, propertyColumn16, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSentRepresentingName), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags,
				PropTag.Message.SentRepresentingEntryId,
				PropTag.Message.SentRepresentingSimpleDisplayName,
				PropTag.Message.SentRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping4,
				computedPropertyMapping5,
				computedPropertyMapping9,
				computedPropertyMapping7
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSentRepresentingName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingName, computedPropertyMapping10);
			Column[] dependOn31 = new Column[]
			{
				propertyColumn10
			};
			PropertyColumn propertyColumn17 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingOrgAddressType, rowPropBagCreator, dependOn31);
			ComputedPropertyMapping computedPropertyMapping11 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingOrgAddressType, propertyColumn17, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSentRepresentingOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping4
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSentRepresentingOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingOrgAddressType, computedPropertyMapping11);
			Column[] dependOn32 = new Column[]
			{
				propertyColumn10
			};
			PropertyColumn propertyColumn18 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingOrgEmailAddr, rowPropBagCreator, dependOn32);
			ComputedPropertyMapping computedPropertyMapping12 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingOrgEmailAddr, propertyColumn18, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSentRepresentingOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping4
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSentRepresentingOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingOrgEmailAddr, computedPropertyMapping12);
			Column[] dependOn33 = new Column[]
			{
				propertyColumn10
			};
			PropertyColumn propertyColumn19 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingSID, rowPropBagCreator, dependOn33);
			ComputedPropertyMapping computedPropertyMapping13 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingSID, propertyColumn19, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSentRepresentingSID), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping4
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSentRepresentingSID), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingSID, computedPropertyMapping13);
			Column[] dependOn34 = new Column[]
			{
				propertyColumn10
			};
			PropertyColumn propertyColumn20 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingGuid, rowPropBagCreator, dependOn34);
			ComputedPropertyMapping computedPropertyMapping14 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingGuid, propertyColumn20, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSentRepresentingGuid), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping4
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSentRepresentingGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingGuid, computedPropertyMapping14);
			Column[] dependOn35 = new Column[]
			{
				propertyColumn10
			};
			PropertyColumn propertyColumn21 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderFlags, rowPropBagCreator, dependOn35);
			ComputedPropertyMapping computedPropertyMapping15 = new FullComputedPropertyMapping(PropTag.Message.SenderFlags, propertyColumn21, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSenderFlags), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping4
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSenderFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderFlags, computedPropertyMapping15);
			Column[] dependOn36 = new Column[]
			{
				propertyColumn21,
				propertyColumn11
			};
			PropertyColumn propertyColumn22 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderEntryId, rowPropBagCreator, dependOn36);
			ComputedPropertyMapping computedPropertyMapping16 = new FullComputedPropertyMapping(PropTag.Message.SenderEntryId, propertyColumn22, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSenderEntryId), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping15,
				computedPropertyMapping5
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSenderEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderEntryId, computedPropertyMapping16);
			Column[] dependOn37 = new Column[]
			{
				propertyColumn21,
				propertyColumn22,
				propertyColumn12
			};
			PropertyColumn propertyColumn23 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderAddressType, rowPropBagCreator, dependOn37);
			ComputedPropertyMapping computedPropertyMapping17 = new FullComputedPropertyMapping(PropTag.Message.SenderAddressType, propertyColumn23, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSenderAddressType), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SenderEntryId,
				PropTag.Message.SentRepresentingAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping15,
				computedPropertyMapping16,
				computedPropertyMapping6
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSenderAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderAddressType, computedPropertyMapping17);
			Column[] dependOn38 = new Column[]
			{
				propertyColumn21,
				propertyColumn22,
				propertyColumn13
			};
			PropertyColumn propertyColumn24 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderEmailAddress, rowPropBagCreator, dependOn38);
			ComputedPropertyMapping computedPropertyMapping18 = new FullComputedPropertyMapping(PropTag.Message.SenderEmailAddress, propertyColumn24, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSenderEmailAddress), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SenderEntryId,
				PropTag.Message.SentRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping15,
				computedPropertyMapping16,
				computedPropertyMapping7
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSenderEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderEmailAddress, computedPropertyMapping18);
			Column[] dependOn39 = new Column[]
			{
				propertyColumn21,
				propertyColumn23,
				propertyColumn24,
				propertyColumn22,
				propertyColumn14
			};
			PropertyColumn column112 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderSearchKey, rowPropBagCreator, dependOn39);
			ComputedPropertyMapping value81 = new FullComputedPropertyMapping(PropTag.Message.SenderSearchKey, column112, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSenderSearchKey), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SenderAddressType,
				PropTag.Message.SenderEmailAddress,
				PropTag.Message.SenderEntryId,
				PropTag.Message.SentRepresentingSearchKey
			}, new PropertyMapping[]
			{
				computedPropertyMapping15,
				computedPropertyMapping17,
				computedPropertyMapping18,
				computedPropertyMapping16,
				computedPropertyMapping8
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSenderSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderSearchKey, value81);
			Column[] dependOn40 = new Column[]
			{
				propertyColumn21,
				propertyColumn22,
				propertyColumn24,
				propertyColumn15
			};
			PropertyColumn propertyColumn25 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderSimpleDisplayName, rowPropBagCreator, dependOn40);
			ComputedPropertyMapping computedPropertyMapping19 = new FullComputedPropertyMapping(PropTag.Message.SenderSimpleDisplayName, propertyColumn25, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSenderSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SenderEntryId,
				PropTag.Message.SenderEmailAddress,
				PropTag.Message.SentRepresentingSimpleDisplayName
			}, new PropertyMapping[]
			{
				computedPropertyMapping15,
				computedPropertyMapping16,
				computedPropertyMapping18,
				computedPropertyMapping9
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSenderSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderSimpleDisplayName, computedPropertyMapping19);
			Column[] dependOn41 = new Column[]
			{
				propertyColumn21,
				propertyColumn22,
				propertyColumn25,
				propertyColumn24,
				propertyColumn16
			};
			PropertyColumn column113 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderName, rowPropBagCreator, dependOn41);
			ComputedPropertyMapping value82 = new FullComputedPropertyMapping(PropTag.Message.SenderName, column113, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSenderName), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SenderEntryId,
				PropTag.Message.SenderSimpleDisplayName,
				PropTag.Message.SenderEmailAddress,
				PropTag.Message.SentRepresentingName
			}, new PropertyMapping[]
			{
				computedPropertyMapping15,
				computedPropertyMapping16,
				computedPropertyMapping19,
				computedPropertyMapping18,
				computedPropertyMapping10
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSenderName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderName, value82);
			Column[] dependOn42 = new Column[]
			{
				propertyColumn21,
				propertyColumn17
			};
			PropertyColumn column114 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderOrgAddressType, rowPropBagCreator, dependOn42);
			ComputedPropertyMapping value83 = new FullComputedPropertyMapping(PropTag.Message.SenderOrgAddressType, column114, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSenderOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SentRepresentingOrgAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping15,
				computedPropertyMapping11
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSenderOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderOrgAddressType, value83);
			Column[] dependOn43 = new Column[]
			{
				propertyColumn21,
				propertyColumn18
			};
			PropertyColumn column115 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderOrgEmailAddr, rowPropBagCreator, dependOn43);
			ComputedPropertyMapping value84 = new FullComputedPropertyMapping(PropTag.Message.SenderOrgEmailAddr, column115, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSenderOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SentRepresentingOrgEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping15,
				computedPropertyMapping12
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSenderOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderOrgEmailAddr, value84);
			Column[] dependOn44 = new Column[]
			{
				propertyColumn21,
				propertyColumn19
			};
			PropertyColumn column116 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderSID, rowPropBagCreator, dependOn44);
			ComputedPropertyMapping value85 = new FullComputedPropertyMapping(PropTag.Message.SenderSID, column116, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSenderSID), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SentRepresentingSID
			}, new PropertyMapping[]
			{
				computedPropertyMapping15,
				computedPropertyMapping13
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSenderSID), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderSID, value85);
			Column[] dependOn45 = new Column[]
			{
				propertyColumn21,
				propertyColumn20
			};
			PropertyColumn column117 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderGuid, rowPropBagCreator, dependOn45);
			ComputedPropertyMapping value86 = new FullComputedPropertyMapping(PropTag.Message.SenderGuid, column117, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageSenderGuid), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SentRepresentingGuid
			}, new PropertyMapping[]
			{
				computedPropertyMapping15,
				computedPropertyMapping14
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageSenderGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderGuid, value86);
			PropertyColumn propertyColumn26 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping20 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingFlags, propertyColumn26, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSentRepresentingFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSentRepresentingFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingFlags, computedPropertyMapping20);
			Column[] dependOn46 = new Column[]
			{
				propertyColumn26
			};
			PropertyColumn propertyColumn27 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingEntryId, rowPropBagCreator, dependOn46);
			ComputedPropertyMapping computedPropertyMapping21 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingEntryId, propertyColumn27, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSentRepresentingEntryId), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping20
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSentRepresentingEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingEntryId, computedPropertyMapping21);
			Column[] dependOn47 = new Column[]
			{
				propertyColumn26,
				propertyColumn27
			};
			PropertyColumn propertyColumn28 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingAddressType, rowPropBagCreator, dependOn47);
			ComputedPropertyMapping computedPropertyMapping22 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingAddressType, propertyColumn28, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSentRepresentingAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags,
				PropTag.Message.OriginalSentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping20,
				computedPropertyMapping21
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSentRepresentingAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingAddressType, computedPropertyMapping22);
			Column[] dependOn48 = new Column[]
			{
				propertyColumn26,
				propertyColumn27
			};
			PropertyColumn propertyColumn29 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingEmailAddress, rowPropBagCreator, dependOn48);
			ComputedPropertyMapping computedPropertyMapping23 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingEmailAddress, propertyColumn29, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSentRepresentingEmailAddress), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags,
				PropTag.Message.OriginalSentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping20,
				computedPropertyMapping21
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSentRepresentingEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingEmailAddress, computedPropertyMapping23);
			Column[] dependOn49 = new Column[]
			{
				propertyColumn26,
				propertyColumn28,
				propertyColumn29,
				propertyColumn27
			};
			PropertyColumn propertyColumn30 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingSearchKey, rowPropBagCreator, dependOn49);
			ComputedPropertyMapping computedPropertyMapping24 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingSearchKey, propertyColumn30, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSentRepresentingSearchKey), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags,
				PropTag.Message.OriginalSentRepresentingAddressType,
				PropTag.Message.OriginalSentRepresentingEmailAddress,
				PropTag.Message.OriginalSentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping20,
				computedPropertyMapping22,
				computedPropertyMapping23,
				computedPropertyMapping21
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSentRepresentingSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingSearchKey, computedPropertyMapping24);
			Column[] dependOn50 = new Column[]
			{
				propertyColumn26,
				propertyColumn27,
				propertyColumn29
			};
			PropertyColumn propertyColumn31 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingSimpleDisplayName, rowPropBagCreator, dependOn50);
			ComputedPropertyMapping computedPropertyMapping25 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingSimpleDisplayName, propertyColumn31, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSentRepresentingSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags,
				PropTag.Message.OriginalSentRepresentingEntryId,
				PropTag.Message.OriginalSentRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping20,
				computedPropertyMapping21,
				computedPropertyMapping23
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSentRepresentingSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingSimpleDisplayName, computedPropertyMapping25);
			Column[] dependOn51 = new Column[]
			{
				propertyColumn26,
				propertyColumn27,
				propertyColumn31,
				propertyColumn29
			};
			PropertyColumn propertyColumn32 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingName, rowPropBagCreator, dependOn51);
			ComputedPropertyMapping computedPropertyMapping26 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingName, propertyColumn32, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSentRepresentingName), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags,
				PropTag.Message.OriginalSentRepresentingEntryId,
				PropTag.Message.OriginalSentRepresentingSimpleDisplayName,
				PropTag.Message.OriginalSentRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping20,
				computedPropertyMapping21,
				computedPropertyMapping25,
				computedPropertyMapping23
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSentRepresentingName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingName, computedPropertyMapping26);
			Column[] dependOn52 = new Column[]
			{
				propertyColumn26
			};
			PropertyColumn propertyColumn33 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingOrgAddressType, rowPropBagCreator, dependOn52);
			ComputedPropertyMapping computedPropertyMapping27 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingOrgAddressType, propertyColumn33, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSentRepresentingOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping20
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSentRepresentingOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingOrgAddressType, computedPropertyMapping27);
			Column[] dependOn53 = new Column[]
			{
				propertyColumn26
			};
			PropertyColumn propertyColumn34 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingOrgEmailAddr, rowPropBagCreator, dependOn53);
			ComputedPropertyMapping computedPropertyMapping28 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingOrgEmailAddr, propertyColumn34, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSentRepresentingOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping20
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSentRepresentingOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingOrgEmailAddr, computedPropertyMapping28);
			Column[] dependOn54 = new Column[]
			{
				propertyColumn26
			};
			PropertyColumn propertyColumn35 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingSid, rowPropBagCreator, dependOn54);
			ComputedPropertyMapping computedPropertyMapping29 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingSid, propertyColumn35, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSentRepresentingSid), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping20
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSentRepresentingSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingSid, computedPropertyMapping29);
			Column[] dependOn55 = new Column[]
			{
				propertyColumn26
			};
			PropertyColumn propertyColumn36 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingGuid, rowPropBagCreator, dependOn55);
			ComputedPropertyMapping computedPropertyMapping30 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingGuid, propertyColumn36, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSentRepresentingGuid), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping20
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSentRepresentingGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingGuid, computedPropertyMapping30);
			Column[] dependOn56 = new Column[]
			{
				propertyColumn26
			};
			PropertyColumn propertyColumn37 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderFlags, rowPropBagCreator, dependOn56);
			ComputedPropertyMapping computedPropertyMapping31 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderFlags, propertyColumn37, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSenderFlags), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping20
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSenderFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderFlags, computedPropertyMapping31);
			Column[] dependOn57 = new Column[]
			{
				propertyColumn37,
				propertyColumn27
			};
			PropertyColumn propertyColumn38 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderEntryId, rowPropBagCreator, dependOn57);
			ComputedPropertyMapping computedPropertyMapping32 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderEntryId, propertyColumn38, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSenderEntryId), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping31,
				computedPropertyMapping21
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSenderEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderEntryId, computedPropertyMapping32);
			Column[] dependOn58 = new Column[]
			{
				propertyColumn37,
				propertyColumn38,
				propertyColumn28
			};
			PropertyColumn propertyColumn39 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderAddressType, rowPropBagCreator, dependOn58);
			ComputedPropertyMapping computedPropertyMapping33 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderAddressType, propertyColumn39, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSenderAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSenderEntryId,
				PropTag.Message.OriginalSentRepresentingAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping31,
				computedPropertyMapping32,
				computedPropertyMapping22
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSenderAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderAddressType, computedPropertyMapping33);
			Column[] dependOn59 = new Column[]
			{
				propertyColumn37,
				propertyColumn38,
				propertyColumn29
			};
			PropertyColumn propertyColumn40 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderEmailAddress, rowPropBagCreator, dependOn59);
			ComputedPropertyMapping computedPropertyMapping34 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderEmailAddress, propertyColumn40, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSenderEmailAddress), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSenderEntryId,
				PropTag.Message.OriginalSentRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping31,
				computedPropertyMapping32,
				computedPropertyMapping23
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSenderEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderEmailAddress, computedPropertyMapping34);
			Column[] dependOn60 = new Column[]
			{
				propertyColumn37,
				propertyColumn39,
				propertyColumn40,
				propertyColumn38,
				propertyColumn30
			};
			PropertyColumn column118 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderSearchKey, rowPropBagCreator, dependOn60);
			ComputedPropertyMapping value87 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderSearchKey, column118, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSenderSearchKey), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSenderAddressType,
				PropTag.Message.OriginalSenderEmailAddress,
				PropTag.Message.OriginalSenderEntryId,
				PropTag.Message.OriginalSentRepresentingSearchKey
			}, new PropertyMapping[]
			{
				computedPropertyMapping31,
				computedPropertyMapping33,
				computedPropertyMapping34,
				computedPropertyMapping32,
				computedPropertyMapping24
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSenderSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderSearchKey, value87);
			Column[] dependOn61 = new Column[]
			{
				propertyColumn37,
				propertyColumn38,
				propertyColumn40,
				propertyColumn31
			};
			PropertyColumn propertyColumn41 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderSimpleDisplayName, rowPropBagCreator, dependOn61);
			ComputedPropertyMapping computedPropertyMapping35 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderSimpleDisplayName, propertyColumn41, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSenderSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSenderEntryId,
				PropTag.Message.OriginalSenderEmailAddress,
				PropTag.Message.OriginalSentRepresentingSimpleDisplayName
			}, new PropertyMapping[]
			{
				computedPropertyMapping31,
				computedPropertyMapping32,
				computedPropertyMapping34,
				computedPropertyMapping25
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSenderSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderSimpleDisplayName, computedPropertyMapping35);
			Column[] dependOn62 = new Column[]
			{
				propertyColumn37,
				propertyColumn38,
				propertyColumn41,
				propertyColumn40,
				propertyColumn32
			};
			PropertyColumn column119 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderName, rowPropBagCreator, dependOn62);
			ComputedPropertyMapping value88 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderName, column119, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSenderName), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSenderEntryId,
				PropTag.Message.OriginalSenderSimpleDisplayName,
				PropTag.Message.OriginalSenderEmailAddress,
				PropTag.Message.OriginalSentRepresentingName
			}, new PropertyMapping[]
			{
				computedPropertyMapping31,
				computedPropertyMapping32,
				computedPropertyMapping35,
				computedPropertyMapping34,
				computedPropertyMapping26
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSenderName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderName, value88);
			Column[] dependOn63 = new Column[]
			{
				propertyColumn37,
				propertyColumn33
			};
			PropertyColumn column120 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderOrgAddressType, rowPropBagCreator, dependOn63);
			ComputedPropertyMapping value89 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderOrgAddressType, column120, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSenderOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSentRepresentingOrgAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping31,
				computedPropertyMapping27
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSenderOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderOrgAddressType, value89);
			Column[] dependOn64 = new Column[]
			{
				propertyColumn37,
				propertyColumn34
			};
			PropertyColumn column121 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderOrgEmailAddr, rowPropBagCreator, dependOn64);
			ComputedPropertyMapping value90 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderOrgEmailAddr, column121, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSenderOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSentRepresentingOrgEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping31,
				computedPropertyMapping28
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSenderOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderOrgEmailAddr, value90);
			Column[] dependOn65 = new Column[]
			{
				propertyColumn37,
				propertyColumn35
			};
			PropertyColumn column122 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderSid, rowPropBagCreator, dependOn65);
			ComputedPropertyMapping value91 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderSid, column122, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSenderSid), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSentRepresentingSid
			}, new PropertyMapping[]
			{
				computedPropertyMapping31,
				computedPropertyMapping29
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSenderSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderSid, value91);
			Column[] dependOn66 = new Column[]
			{
				propertyColumn37,
				propertyColumn36
			};
			PropertyColumn column123 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderGuid, rowPropBagCreator, dependOn66);
			ComputedPropertyMapping value92 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderGuid, column123, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalSenderGuid), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSentRepresentingGuid
			}, new PropertyMapping[]
			{
				computedPropertyMapping31,
				computedPropertyMapping30
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalSenderGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderGuid, value92);
			PropertyColumn propertyColumn42 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdRepresentingFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping36 = new FullComputedPropertyMapping(PropTag.Message.RcvdRepresentingFlags, propertyColumn42, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageRcvdRepresentingFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageRcvdRepresentingFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdRepresentingFlags, computedPropertyMapping36);
			Column[] dependOn67 = new Column[]
			{
				propertyColumn42
			};
			PropertyColumn propertyColumn43 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingEntryId, rowPropBagCreator, dependOn67);
			ComputedPropertyMapping computedPropertyMapping37 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingEntryId, propertyColumn43, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedRepresentingEntryId), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping36
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedRepresentingEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingEntryId, computedPropertyMapping37);
			Column[] dependOn68 = new Column[]
			{
				propertyColumn42,
				propertyColumn43
			};
			PropertyColumn propertyColumn44 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingAddressType, rowPropBagCreator, dependOn68);
			ComputedPropertyMapping computedPropertyMapping38 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingAddressType, propertyColumn44, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedRepresentingAddressType), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags,
				PropTag.Message.ReceivedRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping36,
				computedPropertyMapping37
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedRepresentingAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingAddressType, computedPropertyMapping38);
			Column[] dependOn69 = new Column[]
			{
				propertyColumn42,
				propertyColumn43
			};
			PropertyColumn propertyColumn45 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingEmailAddress, rowPropBagCreator, dependOn69);
			ComputedPropertyMapping computedPropertyMapping39 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingEmailAddress, propertyColumn45, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedRepresentingEmailAddress), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags,
				PropTag.Message.ReceivedRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping36,
				computedPropertyMapping37
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedRepresentingEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingEmailAddress, computedPropertyMapping39);
			Column[] dependOn70 = new Column[]
			{
				propertyColumn42,
				propertyColumn44,
				propertyColumn45,
				propertyColumn43
			};
			PropertyColumn propertyColumn46 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingSearchKey, rowPropBagCreator, dependOn70);
			ComputedPropertyMapping computedPropertyMapping40 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingSearchKey, propertyColumn46, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedRepresentingSearchKey), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags,
				PropTag.Message.ReceivedRepresentingAddressType,
				PropTag.Message.ReceivedRepresentingEmailAddress,
				PropTag.Message.ReceivedRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping36,
				computedPropertyMapping38,
				computedPropertyMapping39,
				computedPropertyMapping37
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedRepresentingSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingSearchKey, computedPropertyMapping40);
			Column[] dependOn71 = new Column[]
			{
				propertyColumn42,
				propertyColumn43,
				propertyColumn45
			};
			PropertyColumn propertyColumn47 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingSimpleDisplayName, rowPropBagCreator, dependOn71);
			ComputedPropertyMapping computedPropertyMapping41 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingSimpleDisplayName, propertyColumn47, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedRepresentingSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags,
				PropTag.Message.ReceivedRepresentingEntryId,
				PropTag.Message.ReceivedRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping36,
				computedPropertyMapping37,
				computedPropertyMapping39
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedRepresentingSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingSimpleDisplayName, computedPropertyMapping41);
			Column[] dependOn72 = new Column[]
			{
				propertyColumn42,
				propertyColumn43,
				propertyColumn47,
				propertyColumn45
			};
			PropertyColumn propertyColumn48 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingName, rowPropBagCreator, dependOn72);
			ComputedPropertyMapping computedPropertyMapping42 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingName, propertyColumn48, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedRepresentingName), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags,
				PropTag.Message.ReceivedRepresentingEntryId,
				PropTag.Message.ReceivedRepresentingSimpleDisplayName,
				PropTag.Message.ReceivedRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping36,
				computedPropertyMapping37,
				computedPropertyMapping41,
				computedPropertyMapping39
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedRepresentingName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingName, computedPropertyMapping42);
			Column[] dependOn73 = new Column[]
			{
				propertyColumn42
			};
			PropertyColumn propertyColumn49 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdRepresentingOrgAddressType, rowPropBagCreator, dependOn73);
			ComputedPropertyMapping computedPropertyMapping43 = new FullComputedPropertyMapping(PropTag.Message.RcvdRepresentingOrgAddressType, propertyColumn49, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageRcvdRepresentingOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping36
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageRcvdRepresentingOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdRepresentingOrgAddressType, computedPropertyMapping43);
			Column[] dependOn74 = new Column[]
			{
				propertyColumn42
			};
			PropertyColumn propertyColumn50 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdRepresentingOrgEmailAddr, rowPropBagCreator, dependOn74);
			ComputedPropertyMapping computedPropertyMapping44 = new FullComputedPropertyMapping(PropTag.Message.RcvdRepresentingOrgEmailAddr, propertyColumn50, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageRcvdRepresentingOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping36
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageRcvdRepresentingOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdRepresentingOrgEmailAddr, computedPropertyMapping44);
			Column[] dependOn75 = new Column[]
			{
				propertyColumn42
			};
			PropertyColumn propertyColumn51 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdRepresentingSid, rowPropBagCreator, dependOn75);
			ComputedPropertyMapping computedPropertyMapping45 = new FullComputedPropertyMapping(PropTag.Message.RcvdRepresentingSid, propertyColumn51, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageRcvdRepresentingSid), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping36
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageRcvdRepresentingSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdRepresentingSid, computedPropertyMapping45);
			Column[] dependOn76 = new Column[]
			{
				propertyColumn42
			};
			PropertyColumn propertyColumn52 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingGuid, rowPropBagCreator, dependOn76);
			ComputedPropertyMapping computedPropertyMapping46 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingGuid, propertyColumn52, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedRepresentingGuid), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping36
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedRepresentingGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingGuid, computedPropertyMapping46);
			Column[] dependOn77 = new Column[]
			{
				propertyColumn42
			};
			PropertyColumn propertyColumn53 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdByFlags, rowPropBagCreator, dependOn77);
			ComputedPropertyMapping computedPropertyMapping47 = new FullComputedPropertyMapping(PropTag.Message.RcvdByFlags, propertyColumn53, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageRcvdByFlags), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping36
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageRcvdByFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdByFlags, computedPropertyMapping47);
			Column[] dependOn78 = new Column[]
			{
				propertyColumn53,
				propertyColumn43
			};
			PropertyColumn propertyColumn54 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedByEntryId, rowPropBagCreator, dependOn78);
			ComputedPropertyMapping computedPropertyMapping48 = new FullComputedPropertyMapping(PropTag.Message.ReceivedByEntryId, propertyColumn54, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedByEntryId), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping47,
				computedPropertyMapping37
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedByEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedByEntryId, computedPropertyMapping48);
			Column[] dependOn79 = new Column[]
			{
				propertyColumn53,
				propertyColumn54,
				propertyColumn44
			};
			PropertyColumn propertyColumn55 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedByAddressType, rowPropBagCreator, dependOn79);
			ComputedPropertyMapping computedPropertyMapping49 = new FullComputedPropertyMapping(PropTag.Message.ReceivedByAddressType, propertyColumn55, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedByAddressType), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedByEntryId,
				PropTag.Message.ReceivedRepresentingAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping47,
				computedPropertyMapping48,
				computedPropertyMapping38
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedByAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedByAddressType, computedPropertyMapping49);
			Column[] dependOn80 = new Column[]
			{
				propertyColumn53,
				propertyColumn54,
				propertyColumn45
			};
			PropertyColumn propertyColumn56 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedByEmailAddress, rowPropBagCreator, dependOn80);
			ComputedPropertyMapping computedPropertyMapping50 = new FullComputedPropertyMapping(PropTag.Message.ReceivedByEmailAddress, propertyColumn56, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedByEmailAddress), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedByEntryId,
				PropTag.Message.ReceivedRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping47,
				computedPropertyMapping48,
				computedPropertyMapping39
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedByEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedByEmailAddress, computedPropertyMapping50);
			Column[] dependOn81 = new Column[]
			{
				propertyColumn53,
				propertyColumn55,
				propertyColumn56,
				propertyColumn54,
				propertyColumn46
			};
			PropertyColumn column124 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedBySearchKey, rowPropBagCreator, dependOn81);
			ComputedPropertyMapping value93 = new FullComputedPropertyMapping(PropTag.Message.ReceivedBySearchKey, column124, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedBySearchKey), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedByAddressType,
				PropTag.Message.ReceivedByEmailAddress,
				PropTag.Message.ReceivedByEntryId,
				PropTag.Message.ReceivedRepresentingSearchKey
			}, new PropertyMapping[]
			{
				computedPropertyMapping47,
				computedPropertyMapping49,
				computedPropertyMapping50,
				computedPropertyMapping48,
				computedPropertyMapping40
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedBySearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedBySearchKey, value93);
			Column[] dependOn82 = new Column[]
			{
				propertyColumn53,
				propertyColumn54,
				propertyColumn56,
				propertyColumn47
			};
			PropertyColumn propertyColumn57 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedBySimpleDisplayName, rowPropBagCreator, dependOn82);
			ComputedPropertyMapping computedPropertyMapping51 = new FullComputedPropertyMapping(PropTag.Message.ReceivedBySimpleDisplayName, propertyColumn57, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedBySimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedByEntryId,
				PropTag.Message.ReceivedByEmailAddress,
				PropTag.Message.ReceivedRepresentingSimpleDisplayName
			}, new PropertyMapping[]
			{
				computedPropertyMapping47,
				computedPropertyMapping48,
				computedPropertyMapping50,
				computedPropertyMapping41
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedBySimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedBySimpleDisplayName, computedPropertyMapping51);
			Column[] dependOn83 = new Column[]
			{
				propertyColumn53,
				propertyColumn54,
				propertyColumn57,
				propertyColumn56,
				propertyColumn48
			};
			PropertyColumn column125 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedByName, rowPropBagCreator, dependOn83);
			ComputedPropertyMapping value94 = new FullComputedPropertyMapping(PropTag.Message.ReceivedByName, column125, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedByName), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedByEntryId,
				PropTag.Message.ReceivedBySimpleDisplayName,
				PropTag.Message.ReceivedByEmailAddress,
				PropTag.Message.ReceivedRepresentingName
			}, new PropertyMapping[]
			{
				computedPropertyMapping47,
				computedPropertyMapping48,
				computedPropertyMapping51,
				computedPropertyMapping50,
				computedPropertyMapping42
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedByName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedByName, value94);
			Column[] dependOn84 = new Column[]
			{
				propertyColumn53,
				propertyColumn49
			};
			PropertyColumn column126 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdByOrgAddressType, rowPropBagCreator, dependOn84);
			ComputedPropertyMapping value95 = new FullComputedPropertyMapping(PropTag.Message.RcvdByOrgAddressType, column126, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageRcvdByOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.RcvdRepresentingOrgAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping47,
				computedPropertyMapping43
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageRcvdByOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdByOrgAddressType, value95);
			Column[] dependOn85 = new Column[]
			{
				propertyColumn53,
				propertyColumn50
			};
			PropertyColumn column127 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdByOrgEmailAddr, rowPropBagCreator, dependOn85);
			ComputedPropertyMapping value96 = new FullComputedPropertyMapping(PropTag.Message.RcvdByOrgEmailAddr, column127, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageRcvdByOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.RcvdRepresentingOrgEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping47,
				computedPropertyMapping44
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageRcvdByOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdByOrgEmailAddr, value96);
			Column[] dependOn86 = new Column[]
			{
				propertyColumn53,
				propertyColumn51
			};
			PropertyColumn column128 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdBySid, rowPropBagCreator, dependOn86);
			ComputedPropertyMapping value97 = new FullComputedPropertyMapping(PropTag.Message.RcvdBySid, column128, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageRcvdBySid), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.RcvdRepresentingSid
			}, new PropertyMapping[]
			{
				computedPropertyMapping47,
				computedPropertyMapping45
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageRcvdBySid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdBySid, value97);
			Column[] dependOn87 = new Column[]
			{
				propertyColumn53,
				propertyColumn52
			};
			PropertyColumn column129 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedByGuid, rowPropBagCreator, dependOn87);
			ComputedPropertyMapping value98 = new FullComputedPropertyMapping(PropTag.Message.ReceivedByGuid, column129, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReceivedByGuid), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedRepresentingGuid
			}, new PropertyMapping[]
			{
				computedPropertyMapping47,
				computedPropertyMapping46
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReceivedByGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedByGuid, value98);
			PropertyColumn propertyColumn58 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping52 = new FullComputedPropertyMapping(PropTag.Message.CreatorFlags, propertyColumn58, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageCreatorFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageCreatorFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorFlags, computedPropertyMapping52);
			Column[] dependOn88 = new Column[]
			{
				propertyColumn58
			};
			PropertyColumn propertyColumn59 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorEntryId, rowPropBagCreator, dependOn88);
			ComputedPropertyMapping computedPropertyMapping53 = new FullComputedPropertyMapping(PropTag.Message.CreatorEntryId, propertyColumn59, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageCreatorEntryId), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping52
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageCreatorEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorEntryId, computedPropertyMapping53);
			Column[] dependOn89 = new Column[]
			{
				propertyColumn58,
				propertyColumn59
			};
			PropertyColumn propertyColumn60 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorAddressType, rowPropBagCreator, dependOn89);
			ComputedPropertyMapping computedPropertyMapping54 = new FullComputedPropertyMapping(PropTag.Message.CreatorAddressType, propertyColumn60, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageCreatorAddressType), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags,
				PropTag.Message.CreatorEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping52,
				computedPropertyMapping53
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageCreatorAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorAddressType, computedPropertyMapping54);
			Column[] dependOn90 = new Column[]
			{
				propertyColumn58,
				propertyColumn59
			};
			PropertyColumn propertyColumn61 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorEmailAddr, rowPropBagCreator, dependOn90);
			ComputedPropertyMapping computedPropertyMapping55 = new FullComputedPropertyMapping(PropTag.Message.CreatorEmailAddr, propertyColumn61, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageCreatorEmailAddr), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags,
				PropTag.Message.CreatorEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping52,
				computedPropertyMapping53
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageCreatorEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorEmailAddr, computedPropertyMapping55);
			Column[] dependOn91 = new Column[]
			{
				propertyColumn58,
				propertyColumn59,
				propertyColumn61
			};
			PropertyColumn propertyColumn62 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorSimpleDisplayName, rowPropBagCreator, dependOn91);
			ComputedPropertyMapping computedPropertyMapping56 = new FullComputedPropertyMapping(PropTag.Message.CreatorSimpleDisplayName, propertyColumn62, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageCreatorSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags,
				PropTag.Message.CreatorEntryId,
				PropTag.Message.CreatorEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping52,
				computedPropertyMapping53,
				computedPropertyMapping55
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageCreatorSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorSimpleDisplayName, computedPropertyMapping56);
			Column[] dependOn92 = new Column[]
			{
				propertyColumn58,
				propertyColumn59,
				propertyColumn62,
				propertyColumn61
			};
			PropertyColumn propertyColumn63 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorName, rowPropBagCreator, dependOn92);
			ComputedPropertyMapping computedPropertyMapping57 = new FullComputedPropertyMapping(PropTag.Message.CreatorName, propertyColumn63, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageCreatorName), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags,
				PropTag.Message.CreatorEntryId,
				PropTag.Message.CreatorSimpleDisplayName,
				PropTag.Message.CreatorEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping52,
				computedPropertyMapping53,
				computedPropertyMapping56,
				computedPropertyMapping55
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageCreatorName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorName, computedPropertyMapping57);
			Column[] dependOn93 = new Column[]
			{
				propertyColumn58
			};
			PropertyColumn propertyColumn64 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorOrgAddressType, rowPropBagCreator, dependOn93);
			ComputedPropertyMapping computedPropertyMapping58 = new FullComputedPropertyMapping(PropTag.Message.CreatorOrgAddressType, propertyColumn64, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageCreatorOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping52
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageCreatorOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorOrgAddressType, computedPropertyMapping58);
			Column[] dependOn94 = new Column[]
			{
				propertyColumn58
			};
			PropertyColumn propertyColumn65 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorOrgEmailAddr, rowPropBagCreator, dependOn94);
			ComputedPropertyMapping computedPropertyMapping59 = new FullComputedPropertyMapping(PropTag.Message.CreatorOrgEmailAddr, propertyColumn65, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageCreatorOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping52
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageCreatorOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorOrgEmailAddr, computedPropertyMapping59);
			Column[] dependOn95 = new Column[]
			{
				propertyColumn58
			};
			PropertyColumn propertyColumn66 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorSID, rowPropBagCreator, dependOn95);
			ComputedPropertyMapping computedPropertyMapping60 = new FullComputedPropertyMapping(PropTag.Message.CreatorSID, propertyColumn66, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageCreatorSID), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping52
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageCreatorSID), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorSID, computedPropertyMapping60);
			Column[] dependOn96 = new Column[]
			{
				propertyColumn58
			};
			PropertyColumn propertyColumn67 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorGuid, rowPropBagCreator, dependOn96);
			ComputedPropertyMapping computedPropertyMapping61 = new FullComputedPropertyMapping(PropTag.Message.CreatorGuid, propertyColumn67, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageCreatorGuid), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping52
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageCreatorGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorGuid, computedPropertyMapping61);
			Column[] dependOn97 = new Column[]
			{
				propertyColumn58
			};
			PropertyColumn propertyColumn68 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierFlags, rowPropBagCreator, dependOn97);
			ComputedPropertyMapping computedPropertyMapping62 = new FullComputedPropertyMapping(PropTag.Message.LastModifierFlags, propertyColumn68, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageLastModifierFlags), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping52
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageLastModifierFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierFlags, computedPropertyMapping62);
			Column[] dependOn98 = new Column[]
			{
				propertyColumn68,
				propertyColumn59
			};
			PropertyColumn propertyColumn69 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierEntryId, rowPropBagCreator, dependOn98);
			ComputedPropertyMapping computedPropertyMapping63 = new FullComputedPropertyMapping(PropTag.Message.LastModifierEntryId, propertyColumn69, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageLastModifierEntryId), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.CreatorEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping62,
				computedPropertyMapping53
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageLastModifierEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierEntryId, computedPropertyMapping63);
			Column[] dependOn99 = new Column[]
			{
				propertyColumn68,
				propertyColumn69,
				propertyColumn60
			};
			PropertyColumn propertyColumn70 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierAddressType, rowPropBagCreator, dependOn99);
			ComputedPropertyMapping computedPropertyMapping64 = new FullComputedPropertyMapping(PropTag.Message.LastModifierAddressType, propertyColumn70, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageLastModifierAddressType), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.LastModifierEntryId,
				PropTag.Message.CreatorAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping62,
				computedPropertyMapping63,
				computedPropertyMapping54
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageLastModifierAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierAddressType, computedPropertyMapping64);
			Column[] dependOn100 = new Column[]
			{
				propertyColumn68,
				propertyColumn69,
				propertyColumn61
			};
			PropertyColumn propertyColumn71 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierEmailAddr, rowPropBagCreator, dependOn100);
			ComputedPropertyMapping computedPropertyMapping65 = new FullComputedPropertyMapping(PropTag.Message.LastModifierEmailAddr, propertyColumn71, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageLastModifierEmailAddr), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.LastModifierEntryId,
				PropTag.Message.CreatorEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping62,
				computedPropertyMapping63,
				computedPropertyMapping55
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageLastModifierEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierEmailAddr, computedPropertyMapping65);
			Column[] dependOn101 = new Column[]
			{
				propertyColumn68,
				propertyColumn69,
				propertyColumn71,
				propertyColumn62
			};
			PropertyColumn propertyColumn72 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierSimpleDisplayName, rowPropBagCreator, dependOn101);
			ComputedPropertyMapping computedPropertyMapping66 = new FullComputedPropertyMapping(PropTag.Message.LastModifierSimpleDisplayName, propertyColumn72, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageLastModifierSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.LastModifierEntryId,
				PropTag.Message.LastModifierEmailAddr,
				PropTag.Message.CreatorSimpleDisplayName
			}, new PropertyMapping[]
			{
				computedPropertyMapping62,
				computedPropertyMapping63,
				computedPropertyMapping65,
				computedPropertyMapping56
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageLastModifierSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierSimpleDisplayName, computedPropertyMapping66);
			Column[] dependOn102 = new Column[]
			{
				propertyColumn68,
				propertyColumn69,
				propertyColumn72,
				propertyColumn71,
				propertyColumn63
			};
			PropertyColumn propertyColumn73 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierName, rowPropBagCreator, dependOn102);
			ComputedPropertyMapping computedPropertyMapping67 = new FullComputedPropertyMapping(PropTag.Message.LastModifierName, propertyColumn73, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageLastModifierName), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.LastModifierEntryId,
				PropTag.Message.LastModifierSimpleDisplayName,
				PropTag.Message.LastModifierEmailAddr,
				PropTag.Message.CreatorName
			}, new PropertyMapping[]
			{
				computedPropertyMapping62,
				computedPropertyMapping63,
				computedPropertyMapping66,
				computedPropertyMapping65,
				computedPropertyMapping57
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageLastModifierName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierName, computedPropertyMapping67);
			Column[] dependOn103 = new Column[]
			{
				propertyColumn68,
				propertyColumn64
			};
			PropertyColumn propertyColumn74 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierOrgAddressType, rowPropBagCreator, dependOn103);
			ComputedPropertyMapping computedPropertyMapping68 = new FullComputedPropertyMapping(PropTag.Message.LastModifierOrgAddressType, propertyColumn74, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageLastModifierOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.CreatorOrgAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping62,
				computedPropertyMapping58
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageLastModifierOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierOrgAddressType, computedPropertyMapping68);
			Column[] dependOn104 = new Column[]
			{
				propertyColumn68,
				propertyColumn65
			};
			PropertyColumn propertyColumn75 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierOrgEmailAddr, rowPropBagCreator, dependOn104);
			ComputedPropertyMapping computedPropertyMapping69 = new FullComputedPropertyMapping(PropTag.Message.LastModifierOrgEmailAddr, propertyColumn75, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageLastModifierOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.CreatorOrgEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping62,
				computedPropertyMapping59
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageLastModifierOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierOrgEmailAddr, computedPropertyMapping69);
			Column[] dependOn105 = new Column[]
			{
				propertyColumn68,
				propertyColumn66
			};
			PropertyColumn propertyColumn76 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierSid, rowPropBagCreator, dependOn105);
			ComputedPropertyMapping computedPropertyMapping70 = new FullComputedPropertyMapping(PropTag.Message.LastModifierSid, propertyColumn76, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageLastModifierSid), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.CreatorSID
			}, new PropertyMapping[]
			{
				computedPropertyMapping62,
				computedPropertyMapping60
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageLastModifierSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierSid, computedPropertyMapping70);
			Column[] dependOn106 = new Column[]
			{
				propertyColumn68,
				propertyColumn67
			};
			PropertyColumn propertyColumn77 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierGuid, rowPropBagCreator, dependOn106);
			ComputedPropertyMapping computedPropertyMapping71 = new FullComputedPropertyMapping(PropTag.Message.LastModifierGuid, propertyColumn77, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageLastModifierGuid), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.CreatorGuid
			}, new PropertyMapping[]
			{
				computedPropertyMapping62,
				computedPropertyMapping61
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageLastModifierGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierGuid, computedPropertyMapping71);
			PropertyColumn propertyColumn78 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping72 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptFlags, propertyColumn78, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReadReceiptFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReadReceiptFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptFlags, computedPropertyMapping72);
			Column[] dependOn107 = new Column[]
			{
				propertyColumn78
			};
			PropertyColumn propertyColumn79 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptEntryId, rowPropBagCreator, dependOn107);
			ComputedPropertyMapping computedPropertyMapping73 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptEntryId, propertyColumn79, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReadReceiptEntryId), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping72
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReadReceiptEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptEntryId, computedPropertyMapping73);
			Column[] dependOn108 = new Column[]
			{
				propertyColumn78,
				propertyColumn79
			};
			PropertyColumn propertyColumn80 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptAddressType, rowPropBagCreator, dependOn108);
			ComputedPropertyMapping computedPropertyMapping74 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptAddressType, propertyColumn80, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReadReceiptAddressType), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags,
				PropTag.Message.ReadReceiptEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping72,
				computedPropertyMapping73
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReadReceiptAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptAddressType, computedPropertyMapping74);
			Column[] dependOn109 = new Column[]
			{
				propertyColumn78,
				propertyColumn79
			};
			PropertyColumn propertyColumn81 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptEmailAddress, rowPropBagCreator, dependOn109);
			ComputedPropertyMapping computedPropertyMapping75 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptEmailAddress, propertyColumn81, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReadReceiptEmailAddress), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags,
				PropTag.Message.ReadReceiptEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping72,
				computedPropertyMapping73
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReadReceiptEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptEmailAddress, computedPropertyMapping75);
			Column[] dependOn110 = new Column[]
			{
				propertyColumn78,
				propertyColumn80,
				propertyColumn81,
				propertyColumn79
			};
			PropertyColumn column130 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptSearchKey, rowPropBagCreator, dependOn110);
			ComputedPropertyMapping value99 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptSearchKey, column130, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReadReceiptSearchKey), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags,
				PropTag.Message.ReadReceiptAddressType,
				PropTag.Message.ReadReceiptEmailAddress,
				PropTag.Message.ReadReceiptEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping72,
				computedPropertyMapping74,
				computedPropertyMapping75,
				computedPropertyMapping73
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReadReceiptSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptSearchKey, value99);
			Column[] dependOn111 = new Column[]
			{
				propertyColumn78,
				propertyColumn79,
				propertyColumn81
			};
			PropertyColumn propertyColumn82 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptSimpleDisplayName, rowPropBagCreator, dependOn111);
			ComputedPropertyMapping computedPropertyMapping76 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptSimpleDisplayName, propertyColumn82, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReadReceiptSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags,
				PropTag.Message.ReadReceiptEntryId,
				PropTag.Message.ReadReceiptEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping72,
				computedPropertyMapping73,
				computedPropertyMapping75
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReadReceiptSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptSimpleDisplayName, computedPropertyMapping76);
			Column[] dependOn112 = new Column[]
			{
				propertyColumn78,
				propertyColumn79,
				propertyColumn82,
				propertyColumn81
			};
			PropertyColumn column131 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptDisplayName, rowPropBagCreator, dependOn112);
			ComputedPropertyMapping value100 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptDisplayName, column131, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReadReceiptDisplayName), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags,
				PropTag.Message.ReadReceiptEntryId,
				PropTag.Message.ReadReceiptSimpleDisplayName,
				PropTag.Message.ReadReceiptEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping72,
				computedPropertyMapping73,
				computedPropertyMapping76,
				computedPropertyMapping75
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReadReceiptDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptDisplayName, value100);
			Column[] dependOn113 = new Column[]
			{
				propertyColumn78
			};
			PropertyColumn column132 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptOrgAddressType, rowPropBagCreator, dependOn113);
			ComputedPropertyMapping value101 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptOrgAddressType, column132, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReadReceiptOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping72
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReadReceiptOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptOrgAddressType, value101);
			Column[] dependOn114 = new Column[]
			{
				propertyColumn78
			};
			PropertyColumn column133 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptOrgEmailAddr, rowPropBagCreator, dependOn114);
			ComputedPropertyMapping value102 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptOrgEmailAddr, column133, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReadReceiptOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping72
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReadReceiptOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptOrgEmailAddr, value102);
			Column[] dependOn115 = new Column[]
			{
				propertyColumn78
			};
			PropertyColumn column134 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptSid, rowPropBagCreator, dependOn115);
			ComputedPropertyMapping value103 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptSid, column134, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReadReceiptSid), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping72
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReadReceiptSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptSid, value103);
			Column[] dependOn116 = new Column[]
			{
				propertyColumn78
			};
			PropertyColumn column135 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptGuid, rowPropBagCreator, dependOn116);
			ComputedPropertyMapping value104 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptGuid, column135, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReadReceiptGuid), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping72
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReadReceiptGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptGuid, value104);
			PropertyColumn propertyColumn83 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping77 = new FullComputedPropertyMapping(PropTag.Message.ReportFlags, propertyColumn83, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportFlags, computedPropertyMapping77);
			Column[] dependOn117 = new Column[]
			{
				propertyColumn83
			};
			PropertyColumn propertyColumn84 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportEntryId, rowPropBagCreator, dependOn117);
			ComputedPropertyMapping computedPropertyMapping78 = new FullComputedPropertyMapping(PropTag.Message.ReportEntryId, propertyColumn84, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportEntryId), new StorePropTag[]
			{
				PropTag.Message.ReportFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping77
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportEntryId, computedPropertyMapping78);
			Column[] dependOn118 = new Column[]
			{
				propertyColumn83,
				propertyColumn84
			};
			PropertyColumn propertyColumn85 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportAddressType, rowPropBagCreator, dependOn118);
			ComputedPropertyMapping computedPropertyMapping79 = new FullComputedPropertyMapping(PropTag.Message.ReportAddressType, propertyColumn85, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportAddressType), new StorePropTag[]
			{
				PropTag.Message.ReportFlags,
				PropTag.Message.ReportEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping77,
				computedPropertyMapping78
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportAddressType, computedPropertyMapping79);
			Column[] dependOn119 = new Column[]
			{
				propertyColumn83,
				propertyColumn84
			};
			PropertyColumn propertyColumn86 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportEmailAddress, rowPropBagCreator, dependOn119);
			ComputedPropertyMapping computedPropertyMapping80 = new FullComputedPropertyMapping(PropTag.Message.ReportEmailAddress, propertyColumn86, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportEmailAddress), new StorePropTag[]
			{
				PropTag.Message.ReportFlags,
				PropTag.Message.ReportEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping77,
				computedPropertyMapping78
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportEmailAddress, computedPropertyMapping80);
			Column[] dependOn120 = new Column[]
			{
				propertyColumn83,
				propertyColumn85,
				propertyColumn86,
				propertyColumn84
			};
			PropertyColumn column136 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportSearchKey, rowPropBagCreator, dependOn120);
			ComputedPropertyMapping value105 = new FullComputedPropertyMapping(PropTag.Message.ReportSearchKey, column136, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportSearchKey), new StorePropTag[]
			{
				PropTag.Message.ReportFlags,
				PropTag.Message.ReportAddressType,
				PropTag.Message.ReportEmailAddress,
				PropTag.Message.ReportEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping77,
				computedPropertyMapping79,
				computedPropertyMapping80,
				computedPropertyMapping78
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportSearchKey, value105);
			Column[] dependOn121 = new Column[]
			{
				propertyColumn83,
				propertyColumn84,
				propertyColumn86
			};
			PropertyColumn propertyColumn87 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportSimpleDisplayName, rowPropBagCreator, dependOn121);
			ComputedPropertyMapping computedPropertyMapping81 = new FullComputedPropertyMapping(PropTag.Message.ReportSimpleDisplayName, propertyColumn87, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.ReportFlags,
				PropTag.Message.ReportEntryId,
				PropTag.Message.ReportEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping77,
				computedPropertyMapping78,
				computedPropertyMapping80
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportSimpleDisplayName, computedPropertyMapping81);
			Column[] dependOn122 = new Column[]
			{
				propertyColumn83,
				propertyColumn84,
				propertyColumn87,
				propertyColumn86
			};
			PropertyColumn column137 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDisplayName, rowPropBagCreator, dependOn122);
			ComputedPropertyMapping value106 = new FullComputedPropertyMapping(PropTag.Message.ReportDisplayName, column137, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportDisplayName), new StorePropTag[]
			{
				PropTag.Message.ReportFlags,
				PropTag.Message.ReportEntryId,
				PropTag.Message.ReportSimpleDisplayName,
				PropTag.Message.ReportEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping77,
				computedPropertyMapping78,
				computedPropertyMapping81,
				computedPropertyMapping80
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDisplayName, value106);
			Column[] dependOn123 = new Column[]
			{
				propertyColumn83
			};
			PropertyColumn column138 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportOrgAddressType, rowPropBagCreator, dependOn123);
			ComputedPropertyMapping value107 = new FullComputedPropertyMapping(PropTag.Message.ReportOrgAddressType, column138, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.ReportFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping77
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportOrgAddressType, value107);
			Column[] dependOn124 = new Column[]
			{
				propertyColumn83
			};
			PropertyColumn column139 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportOrgEmailAddr, rowPropBagCreator, dependOn124);
			ComputedPropertyMapping value108 = new FullComputedPropertyMapping(PropTag.Message.ReportOrgEmailAddr, column139, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.ReportFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping77
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportOrgEmailAddr, value108);
			Column[] dependOn125 = new Column[]
			{
				propertyColumn83
			};
			PropertyColumn column140 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportSid, rowPropBagCreator, dependOn125);
			ComputedPropertyMapping value109 = new FullComputedPropertyMapping(PropTag.Message.ReportSid, column140, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportSid), new StorePropTag[]
			{
				PropTag.Message.ReportFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping77
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportSid, value109);
			Column[] dependOn126 = new Column[]
			{
				propertyColumn83
			};
			PropertyColumn column141 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportGuid, rowPropBagCreator, dependOn126);
			ComputedPropertyMapping value110 = new FullComputedPropertyMapping(PropTag.Message.ReportGuid, column141, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportGuid), new StorePropTag[]
			{
				PropTag.Message.ReportFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping77
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportGuid, value110);
			Column[] dependOn127 = new Column[]
			{
				propertyColumn68
			};
			PropertyColumn propertyColumn88 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorFlags, rowPropBagCreator, dependOn127);
			ComputedPropertyMapping computedPropertyMapping82 = new FullComputedPropertyMapping(PropTag.Message.OriginatorFlags, propertyColumn88, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginatorFlags), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping62
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginatorFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorFlags, computedPropertyMapping82);
			Column[] dependOn128 = new Column[]
			{
				propertyColumn88,
				propertyColumn69
			};
			PropertyColumn propertyColumn89 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorEntryId, rowPropBagCreator, dependOn128);
			ComputedPropertyMapping computedPropertyMapping83 = new FullComputedPropertyMapping(PropTag.Message.OriginatorEntryId, propertyColumn89, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginatorEntryId), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.LastModifierEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping82,
				computedPropertyMapping63
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginatorEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorEntryId, computedPropertyMapping83);
			Column[] dependOn129 = new Column[]
			{
				propertyColumn88,
				propertyColumn89,
				propertyColumn70
			};
			PropertyColumn propertyColumn90 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorAddressType, rowPropBagCreator, dependOn129);
			ComputedPropertyMapping computedPropertyMapping84 = new FullComputedPropertyMapping(PropTag.Message.OriginatorAddressType, propertyColumn90, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginatorAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.OriginatorEntryId,
				PropTag.Message.LastModifierAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping82,
				computedPropertyMapping83,
				computedPropertyMapping64
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginatorAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorAddressType, computedPropertyMapping84);
			Column[] dependOn130 = new Column[]
			{
				propertyColumn88,
				propertyColumn89,
				propertyColumn71
			};
			PropertyColumn propertyColumn91 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorEmailAddress, rowPropBagCreator, dependOn130);
			ComputedPropertyMapping computedPropertyMapping85 = new FullComputedPropertyMapping(PropTag.Message.OriginatorEmailAddress, propertyColumn91, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginatorEmailAddress), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.OriginatorEntryId,
				PropTag.Message.LastModifierEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping82,
				computedPropertyMapping83,
				computedPropertyMapping65
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginatorEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorEmailAddress, computedPropertyMapping85);
			Column[] dependOn131 = new Column[]
			{
				propertyColumn88,
				propertyColumn90,
				propertyColumn91,
				propertyColumn89
			};
			PropertyColumn column142 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorSearchKey, rowPropBagCreator, dependOn131);
			ComputedPropertyMapping value111 = new FullComputedPropertyMapping(PropTag.Message.OriginatorSearchKey, column142, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginatorSearchKey), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.OriginatorAddressType,
				PropTag.Message.OriginatorEmailAddress,
				PropTag.Message.OriginatorEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping82,
				computedPropertyMapping84,
				computedPropertyMapping85,
				computedPropertyMapping83
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginatorSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorSearchKey, value111);
			Column[] dependOn132 = new Column[]
			{
				propertyColumn88,
				propertyColumn89,
				propertyColumn91,
				propertyColumn72
			};
			PropertyColumn propertyColumn92 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorSimpleDisplayName, rowPropBagCreator, dependOn132);
			ComputedPropertyMapping computedPropertyMapping86 = new FullComputedPropertyMapping(PropTag.Message.OriginatorSimpleDisplayName, propertyColumn92, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginatorSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.OriginatorEntryId,
				PropTag.Message.OriginatorEmailAddress,
				PropTag.Message.LastModifierSimpleDisplayName
			}, new PropertyMapping[]
			{
				computedPropertyMapping82,
				computedPropertyMapping83,
				computedPropertyMapping85,
				computedPropertyMapping66
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginatorSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorSimpleDisplayName, computedPropertyMapping86);
			Column[] dependOn133 = new Column[]
			{
				propertyColumn88,
				propertyColumn89,
				propertyColumn92,
				propertyColumn91,
				propertyColumn73
			};
			PropertyColumn column143 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorName, rowPropBagCreator, dependOn133);
			ComputedPropertyMapping value112 = new FullComputedPropertyMapping(PropTag.Message.OriginatorName, column143, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginatorName), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.OriginatorEntryId,
				PropTag.Message.OriginatorSimpleDisplayName,
				PropTag.Message.OriginatorEmailAddress,
				PropTag.Message.LastModifierName
			}, new PropertyMapping[]
			{
				computedPropertyMapping82,
				computedPropertyMapping83,
				computedPropertyMapping86,
				computedPropertyMapping85,
				computedPropertyMapping67
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginatorName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorName, value112);
			Column[] dependOn134 = new Column[]
			{
				propertyColumn88,
				propertyColumn74
			};
			PropertyColumn column144 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorOrgAddressType, rowPropBagCreator, dependOn134);
			ComputedPropertyMapping value113 = new FullComputedPropertyMapping(PropTag.Message.OriginatorOrgAddressType, column144, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginatorOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.LastModifierOrgAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping82,
				computedPropertyMapping68
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginatorOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorOrgAddressType, value113);
			Column[] dependOn135 = new Column[]
			{
				propertyColumn88,
				propertyColumn75
			};
			PropertyColumn column145 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorOrgEmailAddr, rowPropBagCreator, dependOn135);
			ComputedPropertyMapping value114 = new FullComputedPropertyMapping(PropTag.Message.OriginatorOrgEmailAddr, column145, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginatorOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.LastModifierOrgEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping82,
				computedPropertyMapping69
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginatorOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorOrgEmailAddr, value114);
			Column[] dependOn136 = new Column[]
			{
				propertyColumn88,
				propertyColumn76
			};
			PropertyColumn column146 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorSid, rowPropBagCreator, dependOn136);
			ComputedPropertyMapping value115 = new FullComputedPropertyMapping(PropTag.Message.OriginatorSid, column146, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginatorSid), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.LastModifierSid
			}, new PropertyMapping[]
			{
				computedPropertyMapping82,
				computedPropertyMapping70
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginatorSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorSid, value115);
			Column[] dependOn137 = new Column[]
			{
				propertyColumn88,
				propertyColumn77
			};
			PropertyColumn column147 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorGuid, rowPropBagCreator, dependOn137);
			ComputedPropertyMapping value116 = new FullComputedPropertyMapping(PropTag.Message.OriginatorGuid, column147, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginatorGuid), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.LastModifierGuid
			}, new PropertyMapping[]
			{
				computedPropertyMapping82,
				computedPropertyMapping71
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginatorGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorGuid, value116);
			Column[] dependOn138 = new Column[]
			{
				propertyColumn68
			};
			PropertyColumn propertyColumn93 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorFlags, rowPropBagCreator, dependOn138);
			ComputedPropertyMapping computedPropertyMapping87 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorFlags, propertyColumn93, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalAuthorFlags), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping62
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalAuthorFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorFlags, computedPropertyMapping87);
			Column[] dependOn139 = new Column[]
			{
				propertyColumn93,
				propertyColumn69
			};
			PropertyColumn propertyColumn94 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorEntryId, rowPropBagCreator, dependOn139);
			ComputedPropertyMapping computedPropertyMapping88 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorEntryId, propertyColumn94, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalAuthorEntryId), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.LastModifierEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping87,
				computedPropertyMapping63
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalAuthorEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorEntryId, computedPropertyMapping88);
			Column[] dependOn140 = new Column[]
			{
				propertyColumn93,
				propertyColumn94,
				propertyColumn70
			};
			PropertyColumn propertyColumn95 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorAddressType, rowPropBagCreator, dependOn140);
			ComputedPropertyMapping computedPropertyMapping89 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorAddressType, propertyColumn95, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalAuthorAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.OriginalAuthorEntryId,
				PropTag.Message.LastModifierAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping87,
				computedPropertyMapping88,
				computedPropertyMapping64
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalAuthorAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorAddressType, computedPropertyMapping89);
			Column[] dependOn141 = new Column[]
			{
				propertyColumn93,
				propertyColumn94,
				propertyColumn71
			};
			PropertyColumn propertyColumn96 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorEmailAddress, rowPropBagCreator, dependOn141);
			ComputedPropertyMapping computedPropertyMapping90 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorEmailAddress, propertyColumn96, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalAuthorEmailAddress), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.OriginalAuthorEntryId,
				PropTag.Message.LastModifierEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping87,
				computedPropertyMapping88,
				computedPropertyMapping65
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalAuthorEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorEmailAddress, computedPropertyMapping90);
			Column[] dependOn142 = new Column[]
			{
				propertyColumn93,
				propertyColumn95,
				propertyColumn96,
				propertyColumn94
			};
			PropertyColumn column148 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorSearchKey, rowPropBagCreator, dependOn142);
			ComputedPropertyMapping value117 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorSearchKey, column148, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalAuthorSearchKey), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.OriginalAuthorAddressType,
				PropTag.Message.OriginalAuthorEmailAddress,
				PropTag.Message.OriginalAuthorEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping87,
				computedPropertyMapping89,
				computedPropertyMapping90,
				computedPropertyMapping88
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalAuthorSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorSearchKey, value117);
			Column[] dependOn143 = new Column[]
			{
				propertyColumn93,
				propertyColumn94,
				propertyColumn96,
				propertyColumn72
			};
			PropertyColumn propertyColumn97 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorSimpleDispName, rowPropBagCreator, dependOn143);
			ComputedPropertyMapping computedPropertyMapping91 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorSimpleDispName, propertyColumn97, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalAuthorSimpleDispName), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.OriginalAuthorEntryId,
				PropTag.Message.OriginalAuthorEmailAddress,
				PropTag.Message.LastModifierSimpleDisplayName
			}, new PropertyMapping[]
			{
				computedPropertyMapping87,
				computedPropertyMapping88,
				computedPropertyMapping90,
				computedPropertyMapping66
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalAuthorSimpleDispName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorSimpleDispName, computedPropertyMapping91);
			Column[] dependOn144 = new Column[]
			{
				propertyColumn93,
				propertyColumn94,
				propertyColumn97,
				propertyColumn96,
				propertyColumn73
			};
			PropertyColumn column149 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorName, rowPropBagCreator, dependOn144);
			ComputedPropertyMapping value118 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorName, column149, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalAuthorName), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.OriginalAuthorEntryId,
				PropTag.Message.OriginalAuthorSimpleDispName,
				PropTag.Message.OriginalAuthorEmailAddress,
				PropTag.Message.LastModifierName
			}, new PropertyMapping[]
			{
				computedPropertyMapping87,
				computedPropertyMapping88,
				computedPropertyMapping91,
				computedPropertyMapping90,
				computedPropertyMapping67
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalAuthorName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorName, value118);
			Column[] dependOn145 = new Column[]
			{
				propertyColumn93,
				propertyColumn74
			};
			PropertyColumn column150 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorOrgAddressType, rowPropBagCreator, dependOn145);
			ComputedPropertyMapping value119 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorOrgAddressType, column150, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalAuthorOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.LastModifierOrgAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping87,
				computedPropertyMapping68
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalAuthorOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorOrgAddressType, value119);
			Column[] dependOn146 = new Column[]
			{
				propertyColumn93,
				propertyColumn75
			};
			PropertyColumn column151 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorOrgEmailAddr, rowPropBagCreator, dependOn146);
			ComputedPropertyMapping value120 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorOrgEmailAddr, column151, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalAuthorOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.LastModifierOrgEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping87,
				computedPropertyMapping69
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalAuthorOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorOrgEmailAddr, value120);
			Column[] dependOn147 = new Column[]
			{
				propertyColumn93,
				propertyColumn76
			};
			PropertyColumn column152 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorSid, rowPropBagCreator, dependOn147);
			ComputedPropertyMapping value121 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorSid, column152, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalAuthorSid), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.LastModifierSid
			}, new PropertyMapping[]
			{
				computedPropertyMapping87,
				computedPropertyMapping70
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalAuthorSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorSid, value121);
			Column[] dependOn148 = new Column[]
			{
				propertyColumn93,
				propertyColumn77
			};
			PropertyColumn column153 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorGuid, rowPropBagCreator, dependOn148);
			ComputedPropertyMapping value122 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorGuid, column153, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageOriginalAuthorGuid), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.LastModifierGuid
			}, new PropertyMapping[]
			{
				computedPropertyMapping87,
				computedPropertyMapping71
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageOriginalAuthorGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorGuid, value122);
			PropertyColumn propertyColumn98 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping92 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationFlags, propertyColumn98, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportDestinationFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportDestinationFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationFlags, computedPropertyMapping92);
			Column[] dependOn149 = new Column[]
			{
				propertyColumn98
			};
			PropertyColumn propertyColumn99 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationEntryId, rowPropBagCreator, dependOn149);
			ComputedPropertyMapping computedPropertyMapping93 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationEntryId, propertyColumn99, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportDestinationEntryId), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping92
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportDestinationEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationEntryId, computedPropertyMapping93);
			Column[] dependOn150 = new Column[]
			{
				propertyColumn98,
				propertyColumn99
			};
			PropertyColumn propertyColumn100 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationAddressType, rowPropBagCreator, dependOn150);
			ComputedPropertyMapping computedPropertyMapping94 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationAddressType, propertyColumn100, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportDestinationAddressType), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags,
				PropTag.Message.ReportDestinationEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping92,
				computedPropertyMapping93
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportDestinationAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationAddressType, computedPropertyMapping94);
			Column[] dependOn151 = new Column[]
			{
				propertyColumn98,
				propertyColumn99
			};
			PropertyColumn propertyColumn101 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationEmailAddress, rowPropBagCreator, dependOn151);
			ComputedPropertyMapping computedPropertyMapping95 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationEmailAddress, propertyColumn101, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportDestinationEmailAddress), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags,
				PropTag.Message.ReportDestinationEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping92,
				computedPropertyMapping93
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportDestinationEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationEmailAddress, computedPropertyMapping95);
			Column[] dependOn152 = new Column[]
			{
				propertyColumn98,
				propertyColumn100,
				propertyColumn101,
				propertyColumn99
			};
			PropertyColumn column154 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationSearchKey, rowPropBagCreator, dependOn152);
			ComputedPropertyMapping value123 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationSearchKey, column154, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportDestinationSearchKey), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags,
				PropTag.Message.ReportDestinationAddressType,
				PropTag.Message.ReportDestinationEmailAddress,
				PropTag.Message.ReportDestinationEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping92,
				computedPropertyMapping94,
				computedPropertyMapping95,
				computedPropertyMapping93
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportDestinationSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationSearchKey, value123);
			Column[] dependOn153 = new Column[]
			{
				propertyColumn98,
				propertyColumn99,
				propertyColumn101
			};
			PropertyColumn propertyColumn102 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationSimpleDisplayName, rowPropBagCreator, dependOn153);
			ComputedPropertyMapping computedPropertyMapping96 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationSimpleDisplayName, propertyColumn102, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportDestinationSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags,
				PropTag.Message.ReportDestinationEntryId,
				PropTag.Message.ReportDestinationEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping92,
				computedPropertyMapping93,
				computedPropertyMapping95
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportDestinationSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationSimpleDisplayName, computedPropertyMapping96);
			Column[] dependOn154 = new Column[]
			{
				propertyColumn98,
				propertyColumn99,
				propertyColumn102,
				propertyColumn101
			};
			PropertyColumn column155 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationName, rowPropBagCreator, dependOn154);
			ComputedPropertyMapping value124 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationName, column155, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportDestinationName), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags,
				PropTag.Message.ReportDestinationEntryId,
				PropTag.Message.ReportDestinationSimpleDisplayName,
				PropTag.Message.ReportDestinationEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping92,
				computedPropertyMapping93,
				computedPropertyMapping96,
				computedPropertyMapping95
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportDestinationName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationName, value124);
			Column[] dependOn155 = new Column[]
			{
				propertyColumn98
			};
			PropertyColumn column156 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationOrgEmailType, rowPropBagCreator, dependOn155);
			ComputedPropertyMapping value125 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationOrgEmailType, column156, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportDestinationOrgEmailType), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping92
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportDestinationOrgEmailType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationOrgEmailType, value125);
			Column[] dependOn156 = new Column[]
			{
				propertyColumn98
			};
			PropertyColumn column157 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationOrgEmailAddr, rowPropBagCreator, dependOn156);
			ComputedPropertyMapping value126 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationOrgEmailAddr, column157, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportDestinationOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping92
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportDestinationOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationOrgEmailAddr, value126);
			Column[] dependOn157 = new Column[]
			{
				propertyColumn98
			};
			PropertyColumn column158 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationSid, rowPropBagCreator, dependOn157);
			ComputedPropertyMapping value127 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationSid, column158, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportDestinationSid), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping92
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportDestinationSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationSid, value127);
			Column[] dependOn158 = new Column[]
			{
				propertyColumn98
			};
			PropertyColumn column159 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationGuid, rowPropBagCreator, dependOn158);
			ComputedPropertyMapping value128 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationGuid, column159, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMessageReportDestinationGuid), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping92
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetMessageReportDestinationGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationGuid, value128);
			objectPropertySchema.Initialize(ObjectType.Message, table, dictionary, rowPropBagCreator, baseSchema);
			return objectPropertySchema;
		}

		public static ObjectPropertySchema GenerateEmbeddedMessagePropertySchema(StoreDatabase database)
		{
			Dictionary<StorePropTag, PropertyMapping> dictionary = new Dictionary<StorePropTag, PropertyMapping>(200);
			AttachmentTable attachmentTable = DatabaseSchema.AttachmentTable(database);
			if (attachmentTable == null)
			{
				return null;
			}
			Table table = attachmentTable.Table;
			ObjectPropertySchema objectPropertySchema = new ObjectPropertySchema();
			ObjectPropertySchema baseSchema = null;
			Func<IRowAccess, IRowPropertyBag> rowPropBagCreator = (IRowAccess rowAccess) => new RowPropertyBag(table, objectPropertySchema, PropTag.Message.MailboxNum, rowAccess);
			Column column;
			PropertyMapping propertyMapping;
			if (attachmentTable.MailboxPartitionNumber != null)
			{
				column = Factory.CreateMappedPropertyColumn(attachmentTable.MailboxPartitionNumber, PropTag.Message.MailboxPartitionNumber);
				propertyMapping = new PhysicalColumnPropertyMapping(PropTag.Message.MailboxPartitionNumber, column, null, null, null, (PhysicalColumn)column.ActualColumn, false, true, true, false, false);
			}
			else
			{
				column = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MailboxPartitionNumber, null), PropTag.Message.MailboxPartitionNumber);
				propertyMapping = new ConstantPropertyMapping(PropTag.Message.MailboxPartitionNumber, column, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.MailboxPartitionNumber, propertyMapping);
			Column column2;
			PropertyMapping propertyMapping2;
			if (attachmentTable.MailboxNumber != null)
			{
				column2 = Factory.CreateMappedPropertyColumn(attachmentTable.MailboxNumber, PropTag.Message.MailboxNumberInternal);
				propertyMapping2 = new PhysicalColumnPropertyMapping(PropTag.Message.MailboxNumberInternal, column2, null, null, null, (PhysicalColumn)column2.ActualColumn, false, true, true, false, false);
			}
			else
			{
				column2 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MailboxNumberInternal, null), PropTag.Message.MailboxNumberInternal);
				propertyMapping2 = new ConstantPropertyMapping(PropTag.Message.MailboxNumberInternal, column2, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.MailboxNumberInternal, propertyMapping2);
			Column column3 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructFunctionColumn(PropTag.Message.MailboxNum, typeof(int), 4, 0, attachmentTable.Table, new Func<object[], object>(PropertySchemaPopulation.ComputeMailboxNumber), "Exchange.ComputeMailboxNumber", new Column[]
			{
				column,
				column2
			}), PropTag.Message.MailboxNum);
			FunctionPropertyMapping value = new FunctionPropertyMapping(PropTag.Message.MailboxNum, column3, null, new Func<object[], object>(PropertySchemaPopulation.ComputeMailboxNumber), new PropertyMapping[]
			{
				propertyMapping,
				propertyMapping2
			}, true, true, false);
			dictionary.Add(PropTag.Message.MailboxNum, value);
			Column column4;
			PropertyMapping propertyMapping3;
			if (attachmentTable.AttachmentId != null)
			{
				column4 = Factory.CreateMappedPropertyColumn(attachmentTable.AttachmentId, PropTag.Message.MidBin);
				propertyMapping3 = new PhysicalColumnPropertyMapping(PropTag.Message.MidBin, column4, null, null, null, (PhysicalColumn)column4.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column4 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MidBin, null), PropTag.Message.MidBin);
				propertyMapping3 = new ConstantPropertyMapping(PropTag.Message.MidBin, column4, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.MidBin, propertyMapping3);
			Column column5 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.Mid, typeof(long), 8, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), "Exchange.ConvertExchangeIdToInt64", column4), PropTag.Message.Mid);
			ConversionPropertyMapping value2 = new ConversionPropertyMapping(PropTag.Message.Mid, column5, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), PropTag.Message.MidBin, propertyMapping3, null, null, null, true, true, true);
			dictionary.Add(PropTag.Message.Mid, value2);
			Column column6 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.EntryIdSvrEid, null), PropTag.Message.EntryIdSvrEid);
			ConstantPropertyMapping value3 = new ConstantPropertyMapping(PropTag.Message.EntryIdSvrEid, column6, null, null, true, true, true);
			dictionary.Add(PropTag.Message.EntryIdSvrEid, value3);
			Column column7 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.EntryId, null), PropTag.Message.EntryId);
			ConstantPropertyMapping value4 = new ConstantPropertyMapping(PropTag.Message.EntryId, column7, null, null, false, true, true);
			dictionary.Add(PropTag.Message.EntryId, value4);
			Column column8 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.InstanceId, typeof(long), 8, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), "Exchange.ConvertExchangeIdToInt64", column4), PropTag.Message.InstanceId);
			ConversionPropertyMapping value5 = new ConversionPropertyMapping(PropTag.Message.InstanceId, column8, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), PropTag.Message.MidBin, propertyMapping3, null, null, null, true, true, true);
			dictionary.Add(PropTag.Message.InstanceId, value5);
			Column column9 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.DocumentId, 0), PropTag.Message.DocumentId);
			ConstantPropertyMapping value6 = new ConstantPropertyMapping(PropTag.Message.DocumentId, column9, null, 0, true, true, false);
			dictionary.Add(PropTag.Message.DocumentId, value6);
			Column column10 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.InstanceKey, null), PropTag.Message.InstanceKey);
			ConstantPropertyMapping value7 = new ConstantPropertyMapping(PropTag.Message.InstanceKey, column10, null, null, false, true, true);
			dictionary.Add(PropTag.Message.InstanceKey, value7);
			Column column11 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.InstanceKeySvrEid, null), PropTag.Message.InstanceKeySvrEid);
			ConstantPropertyMapping value8 = new ConstantPropertyMapping(PropTag.Message.InstanceKeySvrEid, column11, null, null, true, true, true);
			dictionary.Add(PropTag.Message.InstanceKeySvrEid, value8);
			Column column12 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.InstanceNum, 0), PropTag.Message.InstanceNum);
			ConstantPropertyMapping value9 = new ConstantPropertyMapping(PropTag.Message.InstanceNum, column12, null, 0, true, true, false);
			dictionary.Add(PropTag.Message.InstanceNum, value9);
			Column[] dependOn = new Column[]
			{
				column4
			};
			PropertyColumn column13 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SourceKey, rowPropBagCreator, dependOn);
			ComputedPropertyMapping value10 = new ComputedPropertyMapping(PropTag.Message.SourceKey, column13, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSourceKey), new StorePropTag[]
			{
				PropTag.Message.MidBin
			}, new PropertyMapping[]
			{
				propertyMapping3
			}, null, null, null, true, true, true, true);
			dictionary.Add(PropTag.Message.SourceKey, value10);
			PropertyColumn column14 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SearchKey, rowPropBagCreator, null);
			DefaultPropertyMapping value11 = new DefaultPropertyMapping(PropTag.Message.SearchKey, column14, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.SearchKey, value11);
			PropertyMapping value12;
			if (attachmentTable.RecipientList != null)
			{
				Column column15 = Factory.CreateMappedPropertyColumn(attachmentTable.RecipientList, PropTag.Message.MessageRecipientsMVBin);
				value12 = new PhysicalColumnPropertyMapping(PropTag.Message.MessageRecipientsMVBin, column15, null, null, null, (PhysicalColumn)column15.ActualColumn, true, false, false, false, false);
			}
			else
			{
				Column column15 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MessageRecipientsMVBin, null), PropTag.Message.MessageRecipientsMVBin);
				value12 = new ConstantPropertyMapping(PropTag.Message.MessageRecipientsMVBin, column15, null, null, false, false, false);
			}
			dictionary.Add(PropTag.Message.MessageRecipientsMVBin, value12);
			PropertyMapping value13;
			if (attachmentTable.SubobjectsBlob != null)
			{
				Column column16 = Factory.CreateMappedPropertyColumn(attachmentTable.SubobjectsBlob, PropTag.Message.ItemSubobjectsBin);
				value13 = new PhysicalColumnPropertyMapping(PropTag.Message.ItemSubobjectsBin, column16, null, null, null, (PhysicalColumn)column16.ActualColumn, true, false, false, false, false);
			}
			else
			{
				Column column16 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.ItemSubobjectsBin, null), PropTag.Message.ItemSubobjectsBin);
				value13 = new ConstantPropertyMapping(PropTag.Message.ItemSubobjectsBin, column16, null, null, false, false, false);
			}
			dictionary.Add(PropTag.Message.ItemSubobjectsBin, value13);
			PropertyColumn column17 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.MessageClass, rowPropBagCreator, null);
			DefaultPropertyMapping value14 = new DefaultPropertyMapping(PropTag.Message.MessageClass, column17, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.MessageClass, value14);
			PropertyColumn propertyColumn = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SubjectPrefix, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping = new DefaultPropertyMapping(PropTag.Message.SubjectPrefix, propertyColumn, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.SubjectPrefix, defaultPropertyMapping);
			PropertyColumn propertyColumn2 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.NormalizedSubject, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping2 = new DefaultPropertyMapping(PropTag.Message.NormalizedSubject, propertyColumn2, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.NormalizedSubject, defaultPropertyMapping2);
			Column[] dependOn2 = new Column[]
			{
				propertyColumn,
				propertyColumn2
			};
			PropertyColumn column18 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.Subject, rowPropBagCreator, dependOn2);
			ComputedPropertyMapping value15 = new ComputedPropertyMapping(PropTag.Message.Subject, column18, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSubject), new StorePropTag[]
			{
				PropTag.Message.SubjectPrefix,
				PropTag.Message.NormalizedSubject
			}, new PropertyMapping[]
			{
				defaultPropertyMapping,
				defaultPropertyMapping2
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSubject), null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.Subject, value15);
			PropertyColumn column19 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.MsgStatus, rowPropBagCreator, null);
			DefaultPropertyMapping value16 = new DefaultPropertyMapping(PropTag.Message.MsgStatus, column19, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.MsgStatus, value16);
			PropertyColumn propertyColumn3 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.Read, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping3 = new DefaultPropertyMapping(PropTag.Message.Read, propertyColumn3, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.Read, defaultPropertyMapping3);
			PropertyColumn propertyColumn4 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.Associated, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping4 = new DefaultPropertyMapping(PropTag.Message.Associated, propertyColumn4, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.Associated, defaultPropertyMapping4);
			PropertyColumn propertyColumn5 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.HasAttach, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping5 = new DefaultPropertyMapping(PropTag.Message.HasAttach, propertyColumn5, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.HasAttach, defaultPropertyMapping5);
			Column column20;
			PropertyMapping propertyMapping4;
			if (attachmentTable.MailFlags != null)
			{
				column20 = Factory.CreateMappedPropertyColumn(attachmentTable.MailFlags, PropTag.Message.MailFlags);
				propertyMapping4 = new PhysicalColumnPropertyMapping(PropTag.Message.MailFlags, column20, null, null, null, (PhysicalColumn)column20.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column20 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MailFlags, null), PropTag.Message.MailFlags);
				propertyMapping4 = new ConstantPropertyMapping(PropTag.Message.MailFlags, column20, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Message.MailFlags, propertyMapping4);
			Column[] dependOn3 = new Column[]
			{
				column20
			};
			PropertyColumn column21 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptRequested, rowPropBagCreator, dependOn3);
			ComputedPropertyMapping value17 = new ComputedPropertyMapping(PropTag.Message.ReadReceiptRequested, column21, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReadReceiptRequested), new StorePropTag[]
			{
				PropTag.Message.MailFlags
			}, new PropertyMapping[]
			{
				propertyMapping4
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReadReceiptRequested), null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptRequested, value17);
			Column[] dependOn4 = new Column[]
			{
				column20
			};
			PropertyColumn column22 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.NonReceiptNotificationRequested, rowPropBagCreator, dependOn4);
			ComputedPropertyMapping value18 = new ComputedPropertyMapping(PropTag.Message.NonReceiptNotificationRequested, column22, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageNonReceiptNotificationRequested), new StorePropTag[]
			{
				PropTag.Message.MailFlags
			}, new PropertyMapping[]
			{
				propertyMapping4
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageNonReceiptNotificationRequested), null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.NonReceiptNotificationRequested, value18);
			Column column23;
			PropertyMapping propertyMapping5;
			if (attachmentTable.MessageFlagsActual != null)
			{
				column23 = Factory.CreateMappedPropertyColumn(attachmentTable.MessageFlagsActual, PropTag.Message.MessageFlagsActual);
				propertyMapping5 = new PhysicalColumnPropertyMapping(PropTag.Message.MessageFlagsActual, column23, null, null, null, (PhysicalColumn)column23.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column23 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MessageFlagsActual, null), PropTag.Message.MessageFlagsActual);
				propertyMapping5 = new ConstantPropertyMapping(PropTag.Message.MessageFlagsActual, column23, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.MessageFlagsActual, propertyMapping5);
			Column[] dependOn5 = new Column[]
			{
				propertyColumn5,
				propertyColumn4,
				propertyColumn3,
				column23
			};
			PropertyColumn column24 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.MessageFlags, rowPropBagCreator, dependOn5);
			ComputedPropertyMapping value19 = new ComputedPropertyMapping(PropTag.Message.MessageFlags, column24, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageMessageFlags), new StorePropTag[]
			{
				PropTag.Message.HasAttach,
				PropTag.Message.Associated,
				PropTag.Message.Read,
				PropTag.Message.MessageFlagsActual
			}, new PropertyMapping[]
			{
				defaultPropertyMapping5,
				defaultPropertyMapping4,
				defaultPropertyMapping3,
				propertyMapping5
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageMessageFlags), null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.MessageFlags, value19);
			PropertyColumn column25 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.HasNamedProperties, rowPropBagCreator, null);
			ComputedPropertyMapping value20 = new ComputedPropertyMapping(PropTag.Message.HasNamedProperties, column25, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageHasNamedProperties), null, null, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Message.HasNamedProperties, value20);
			Column column26;
			PropertyMapping propertyMapping6;
			if (attachmentTable.Size != null)
			{
				column26 = Factory.CreateMappedPropertyColumn(attachmentTable.Size, PropTag.Message.MessageSize);
				propertyMapping6 = new PhysicalColumnPropertyMapping(PropTag.Message.MessageSize, column26, null, null, null, (PhysicalColumn)column26.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column26 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MessageSize, null), PropTag.Message.MessageSize);
				propertyMapping6 = new ConstantPropertyMapping(PropTag.Message.MessageSize, column26, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.MessageSize, propertyMapping6);
			Column column27 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.MessageSize32, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column26), PropTag.Message.MessageSize32);
			ConversionPropertyMapping value21 = new ConversionPropertyMapping(PropTag.Message.MessageSize32, column27, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Message.MessageSize, propertyMapping6, null, null, null, true, true, true);
			dictionary.Add(PropTag.Message.MessageSize32, value21);
			Column column28;
			PropertyMapping propertyMapping7;
			if (attachmentTable.FullTextType != null)
			{
				column28 = Factory.CreateMappedPropertyColumn(attachmentTable.FullTextType, PropTag.Message.NativeBodyType);
				propertyMapping7 = new PhysicalColumnPropertyMapping(PropTag.Message.NativeBodyType, column28, null, null, null, (PhysicalColumn)column28.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column28 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.NativeBodyType, null), PropTag.Message.NativeBodyType);
				propertyMapping7 = new ConstantPropertyMapping(PropTag.Message.NativeBodyType, column28, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.NativeBodyType, propertyMapping7);
			Column column29;
			PropertyMapping propertyMapping8;
			if (attachmentTable.Content != null)
			{
				column29 = Factory.CreateMappedPropertyColumn(attachmentTable.Content, PropTag.Message.NativeBody);
				propertyMapping8 = new PhysicalColumnPropertyMapping(PropTag.Message.NativeBody, column29, null, null, null, (PhysicalColumn)column29.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column29 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.NativeBody, null), PropTag.Message.NativeBody);
				propertyMapping8 = new ConstantPropertyMapping(PropTag.Message.NativeBody, column29, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.NativeBody, propertyMapping8);
			Column[] dependOn6 = new Column[]
			{
				column28
			};
			PropertyColumn column30 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.NativeBodyInfo, rowPropBagCreator, dependOn6);
			ComputedPropertyMapping value22 = new ComputedPropertyMapping(PropTag.Message.NativeBodyInfo, column30, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageNativeBodyInfo), new StorePropTag[]
			{
				PropTag.Message.NativeBodyType
			}, new PropertyMapping[]
			{
				propertyMapping7
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Message.NativeBodyInfo, value22);
			Column[] dependOn7 = new Column[]
			{
				column28,
				column29
			};
			PropertyColumn column31 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.BodyUnicode, rowPropBagCreator, dependOn7);
			ComputedPropertyMapping value23 = new ComputedPropertyMapping(PropTag.Message.BodyUnicode, column31, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageBodyUnicode), new StorePropTag[]
			{
				PropTag.Message.NativeBodyType,
				PropTag.Message.NativeBody
			}, new PropertyMapping[]
			{
				propertyMapping7,
				propertyMapping8
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageBodyUnicode), new StreamGetterDelegate(PropertySchemaPopulation.GetEmbeddedMessageBodyUnicodeReadStream), new StreamGetterDelegate(PropertySchemaPopulation.GetEmbeddedMessageBodyUnicodeWriteStream), false, true, true, true);
			dictionary.Add(PropTag.Message.BodyUnicode, value23);
			Column[] dependOn8 = new Column[]
			{
				column28,
				column29
			};
			PropertyColumn column32 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RtfCompressed, rowPropBagCreator, dependOn8);
			ComputedPropertyMapping value24 = new ComputedPropertyMapping(PropTag.Message.RtfCompressed, column32, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageRtfCompressed), new StorePropTag[]
			{
				PropTag.Message.NativeBodyType,
				PropTag.Message.NativeBody
			}, new PropertyMapping[]
			{
				propertyMapping7,
				propertyMapping8
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageRtfCompressed), new StreamGetterDelegate(PropertySchemaPopulation.GetEmbeddedMessageRtfCompressedReadStream), new StreamGetterDelegate(PropertySchemaPopulation.GetEmbeddedMessageRtfCompressedWriteStream), false, true, true, true);
			dictionary.Add(PropTag.Message.RtfCompressed, value24);
			Column[] dependOn9 = new Column[]
			{
				column28,
				column29
			};
			PropertyColumn column33 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.BodyHtml, rowPropBagCreator, dependOn9);
			ComputedPropertyMapping value25 = new ComputedPropertyMapping(PropTag.Message.BodyHtml, column33, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageBodyHtml), new StorePropTag[]
			{
				PropTag.Message.NativeBodyType,
				PropTag.Message.NativeBody
			}, new PropertyMapping[]
			{
				propertyMapping7,
				propertyMapping8
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageBodyHtml), new StreamGetterDelegate(PropertySchemaPopulation.GetEmbeddedMessageBodyHtmlReadStream), new StreamGetterDelegate(PropertySchemaPopulation.GetEmbeddedMessageBodyHtmlWriteStream), false, true, true, true);
			dictionary.Add(PropTag.Message.BodyHtml, value25);
			Column[] dependOn10 = new Column[]
			{
				column28
			};
			PropertyColumn column34 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RTFInSync, rowPropBagCreator, dependOn10);
			ComputedPropertyMapping value26 = new ComputedPropertyMapping(PropTag.Message.RTFInSync, column34, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageRTFInSync), new StorePropTag[]
			{
				PropTag.Message.NativeBodyType
			}, new PropertyMapping[]
			{
				propertyMapping7
			}, null, null, null, true, true, true, true);
			dictionary.Add(PropTag.Message.RTFInSync, value26);
			PropertyMapping value27;
			if (attachmentTable.CreationTime != null)
			{
				Column column35 = Factory.CreateMappedPropertyColumn(attachmentTable.CreationTime, PropTag.Message.CreationTime);
				value27 = new PhysicalColumnPropertyMapping(PropTag.Message.CreationTime, column35, null, null, null, (PhysicalColumn)column35.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column35 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.CreationTime, null), PropTag.Message.CreationTime);
				value27 = new ConstantPropertyMapping(PropTag.Message.CreationTime, column35, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.CreationTime, value27);
			PropertyMapping value28;
			if (attachmentTable.LastModificationTime != null)
			{
				Column column36 = Factory.CreateMappedPropertyColumn(attachmentTable.LastModificationTime, PropTag.Message.LastModificationTime);
				value28 = new PhysicalColumnPropertyMapping(PropTag.Message.LastModificationTime, column36, null, null, null, (PhysicalColumn)column36.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column36 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.LastModificationTime, null), PropTag.Message.LastModificationTime);
				value28 = new ConstantPropertyMapping(PropTag.Message.LastModificationTime, column36, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Message.LastModificationTime, value28);
			PropertyColumn propertyColumn6 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.Preview, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping6 = new DefaultPropertyMapping(PropTag.Message.Preview, propertyColumn6, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Message.Preview, defaultPropertyMapping6);
			Column[] dependOn11 = new Column[]
			{
				propertyColumn3,
				propertyColumn6
			};
			PropertyColumn column37 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.PreviewUnread, rowPropBagCreator, dependOn11);
			ComputedPropertyMapping value29 = new ComputedPropertyMapping(PropTag.Message.PreviewUnread, column37, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessagePreviewUnread), new StorePropTag[]
			{
				PropTag.Message.Read,
				PropTag.Message.Preview
			}, new PropertyMapping[]
			{
				defaultPropertyMapping3,
				defaultPropertyMapping6
			}, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Message.PreviewUnread, value29);
			PropertyColumn propertyColumn7 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingFlags, propertyColumn7, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSentRepresentingFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSentRepresentingFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingFlags, computedPropertyMapping);
			Column[] dependOn12 = new Column[]
			{
				propertyColumn7
			};
			PropertyColumn propertyColumn8 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingEntryId, rowPropBagCreator, dependOn12);
			ComputedPropertyMapping computedPropertyMapping2 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingEntryId, propertyColumn8, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSentRepresentingEntryId), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSentRepresentingEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingEntryId, computedPropertyMapping2);
			Column[] dependOn13 = new Column[]
			{
				propertyColumn7,
				propertyColumn8
			};
			PropertyColumn propertyColumn9 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingAddressType, rowPropBagCreator, dependOn13);
			ComputedPropertyMapping computedPropertyMapping3 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingAddressType, propertyColumn9, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSentRepresentingAddressType), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags,
				PropTag.Message.SentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping,
				computedPropertyMapping2
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSentRepresentingAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingAddressType, computedPropertyMapping3);
			Column[] dependOn14 = new Column[]
			{
				propertyColumn7,
				propertyColumn8
			};
			PropertyColumn propertyColumn10 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingEmailAddress, rowPropBagCreator, dependOn14);
			ComputedPropertyMapping computedPropertyMapping4 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingEmailAddress, propertyColumn10, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSentRepresentingEmailAddress), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags,
				PropTag.Message.SentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping,
				computedPropertyMapping2
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSentRepresentingEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingEmailAddress, computedPropertyMapping4);
			Column[] dependOn15 = new Column[]
			{
				propertyColumn7,
				propertyColumn9,
				propertyColumn10,
				propertyColumn8
			};
			PropertyColumn propertyColumn11 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingSearchKey, rowPropBagCreator, dependOn15);
			ComputedPropertyMapping computedPropertyMapping5 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingSearchKey, propertyColumn11, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSentRepresentingSearchKey), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags,
				PropTag.Message.SentRepresentingAddressType,
				PropTag.Message.SentRepresentingEmailAddress,
				PropTag.Message.SentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping,
				computedPropertyMapping3,
				computedPropertyMapping4,
				computedPropertyMapping2
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSentRepresentingSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingSearchKey, computedPropertyMapping5);
			Column[] dependOn16 = new Column[]
			{
				propertyColumn7,
				propertyColumn8,
				propertyColumn10
			};
			PropertyColumn propertyColumn12 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingSimpleDisplayName, rowPropBagCreator, dependOn16);
			ComputedPropertyMapping computedPropertyMapping6 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingSimpleDisplayName, propertyColumn12, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSentRepresentingSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags,
				PropTag.Message.SentRepresentingEntryId,
				PropTag.Message.SentRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping,
				computedPropertyMapping2,
				computedPropertyMapping4
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSentRepresentingSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingSimpleDisplayName, computedPropertyMapping6);
			Column[] dependOn17 = new Column[]
			{
				propertyColumn7,
				propertyColumn8,
				propertyColumn12,
				propertyColumn10
			};
			PropertyColumn propertyColumn13 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingName, rowPropBagCreator, dependOn17);
			ComputedPropertyMapping computedPropertyMapping7 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingName, propertyColumn13, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSentRepresentingName), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags,
				PropTag.Message.SentRepresentingEntryId,
				PropTag.Message.SentRepresentingSimpleDisplayName,
				PropTag.Message.SentRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping,
				computedPropertyMapping2,
				computedPropertyMapping6,
				computedPropertyMapping4
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSentRepresentingName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingName, computedPropertyMapping7);
			Column[] dependOn18 = new Column[]
			{
				propertyColumn7
			};
			PropertyColumn propertyColumn14 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingOrgAddressType, rowPropBagCreator, dependOn18);
			ComputedPropertyMapping computedPropertyMapping8 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingOrgAddressType, propertyColumn14, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSentRepresentingOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSentRepresentingOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingOrgAddressType, computedPropertyMapping8);
			Column[] dependOn19 = new Column[]
			{
				propertyColumn7
			};
			PropertyColumn propertyColumn15 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingOrgEmailAddr, rowPropBagCreator, dependOn19);
			ComputedPropertyMapping computedPropertyMapping9 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingOrgEmailAddr, propertyColumn15, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSentRepresentingOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSentRepresentingOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingOrgEmailAddr, computedPropertyMapping9);
			Column[] dependOn20 = new Column[]
			{
				propertyColumn7
			};
			PropertyColumn propertyColumn16 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingSID, rowPropBagCreator, dependOn20);
			ComputedPropertyMapping computedPropertyMapping10 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingSID, propertyColumn16, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSentRepresentingSID), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSentRepresentingSID), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingSID, computedPropertyMapping10);
			Column[] dependOn21 = new Column[]
			{
				propertyColumn7
			};
			PropertyColumn propertyColumn17 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SentRepresentingGuid, rowPropBagCreator, dependOn21);
			ComputedPropertyMapping computedPropertyMapping11 = new FullComputedPropertyMapping(PropTag.Message.SentRepresentingGuid, propertyColumn17, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSentRepresentingGuid), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSentRepresentingGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SentRepresentingGuid, computedPropertyMapping11);
			Column[] dependOn22 = new Column[]
			{
				propertyColumn7
			};
			PropertyColumn propertyColumn18 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderFlags, rowPropBagCreator, dependOn22);
			ComputedPropertyMapping computedPropertyMapping12 = new FullComputedPropertyMapping(PropTag.Message.SenderFlags, propertyColumn18, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSenderFlags), new StorePropTag[]
			{
				PropTag.Message.SentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSenderFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderFlags, computedPropertyMapping12);
			Column[] dependOn23 = new Column[]
			{
				propertyColumn18,
				propertyColumn8
			};
			PropertyColumn propertyColumn19 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderEntryId, rowPropBagCreator, dependOn23);
			ComputedPropertyMapping computedPropertyMapping13 = new FullComputedPropertyMapping(PropTag.Message.SenderEntryId, propertyColumn19, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSenderEntryId), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping12,
				computedPropertyMapping2
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSenderEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderEntryId, computedPropertyMapping13);
			Column[] dependOn24 = new Column[]
			{
				propertyColumn18,
				propertyColumn19,
				propertyColumn9
			};
			PropertyColumn propertyColumn20 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderAddressType, rowPropBagCreator, dependOn24);
			ComputedPropertyMapping computedPropertyMapping14 = new FullComputedPropertyMapping(PropTag.Message.SenderAddressType, propertyColumn20, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSenderAddressType), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SenderEntryId,
				PropTag.Message.SentRepresentingAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping12,
				computedPropertyMapping13,
				computedPropertyMapping3
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSenderAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderAddressType, computedPropertyMapping14);
			Column[] dependOn25 = new Column[]
			{
				propertyColumn18,
				propertyColumn19,
				propertyColumn10
			};
			PropertyColumn propertyColumn21 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderEmailAddress, rowPropBagCreator, dependOn25);
			ComputedPropertyMapping computedPropertyMapping15 = new FullComputedPropertyMapping(PropTag.Message.SenderEmailAddress, propertyColumn21, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSenderEmailAddress), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SenderEntryId,
				PropTag.Message.SentRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping12,
				computedPropertyMapping13,
				computedPropertyMapping4
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSenderEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderEmailAddress, computedPropertyMapping15);
			Column[] dependOn26 = new Column[]
			{
				propertyColumn18,
				propertyColumn20,
				propertyColumn21,
				propertyColumn19,
				propertyColumn11
			};
			PropertyColumn column38 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderSearchKey, rowPropBagCreator, dependOn26);
			ComputedPropertyMapping value30 = new FullComputedPropertyMapping(PropTag.Message.SenderSearchKey, column38, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSenderSearchKey), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SenderAddressType,
				PropTag.Message.SenderEmailAddress,
				PropTag.Message.SenderEntryId,
				PropTag.Message.SentRepresentingSearchKey
			}, new PropertyMapping[]
			{
				computedPropertyMapping12,
				computedPropertyMapping14,
				computedPropertyMapping15,
				computedPropertyMapping13,
				computedPropertyMapping5
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSenderSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderSearchKey, value30);
			Column[] dependOn27 = new Column[]
			{
				propertyColumn18,
				propertyColumn19,
				propertyColumn21,
				propertyColumn12
			};
			PropertyColumn propertyColumn22 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderSimpleDisplayName, rowPropBagCreator, dependOn27);
			ComputedPropertyMapping computedPropertyMapping16 = new FullComputedPropertyMapping(PropTag.Message.SenderSimpleDisplayName, propertyColumn22, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSenderSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SenderEntryId,
				PropTag.Message.SenderEmailAddress,
				PropTag.Message.SentRepresentingSimpleDisplayName
			}, new PropertyMapping[]
			{
				computedPropertyMapping12,
				computedPropertyMapping13,
				computedPropertyMapping15,
				computedPropertyMapping6
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSenderSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderSimpleDisplayName, computedPropertyMapping16);
			Column[] dependOn28 = new Column[]
			{
				propertyColumn18,
				propertyColumn19,
				propertyColumn22,
				propertyColumn21,
				propertyColumn13
			};
			PropertyColumn column39 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderName, rowPropBagCreator, dependOn28);
			ComputedPropertyMapping value31 = new FullComputedPropertyMapping(PropTag.Message.SenderName, column39, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSenderName), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SenderEntryId,
				PropTag.Message.SenderSimpleDisplayName,
				PropTag.Message.SenderEmailAddress,
				PropTag.Message.SentRepresentingName
			}, new PropertyMapping[]
			{
				computedPropertyMapping12,
				computedPropertyMapping13,
				computedPropertyMapping16,
				computedPropertyMapping15,
				computedPropertyMapping7
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSenderName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderName, value31);
			Column[] dependOn29 = new Column[]
			{
				propertyColumn18,
				propertyColumn14
			};
			PropertyColumn column40 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderOrgAddressType, rowPropBagCreator, dependOn29);
			ComputedPropertyMapping value32 = new FullComputedPropertyMapping(PropTag.Message.SenderOrgAddressType, column40, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSenderOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SentRepresentingOrgAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping12,
				computedPropertyMapping8
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSenderOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderOrgAddressType, value32);
			Column[] dependOn30 = new Column[]
			{
				propertyColumn18,
				propertyColumn15
			};
			PropertyColumn column41 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderOrgEmailAddr, rowPropBagCreator, dependOn30);
			ComputedPropertyMapping value33 = new FullComputedPropertyMapping(PropTag.Message.SenderOrgEmailAddr, column41, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSenderOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SentRepresentingOrgEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping12,
				computedPropertyMapping9
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSenderOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderOrgEmailAddr, value33);
			Column[] dependOn31 = new Column[]
			{
				propertyColumn18,
				propertyColumn16
			};
			PropertyColumn column42 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderSID, rowPropBagCreator, dependOn31);
			ComputedPropertyMapping value34 = new FullComputedPropertyMapping(PropTag.Message.SenderSID, column42, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSenderSID), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SentRepresentingSID
			}, new PropertyMapping[]
			{
				computedPropertyMapping12,
				computedPropertyMapping10
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSenderSID), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderSID, value34);
			Column[] dependOn32 = new Column[]
			{
				propertyColumn18,
				propertyColumn17
			};
			PropertyColumn column43 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.SenderGuid, rowPropBagCreator, dependOn32);
			ComputedPropertyMapping value35 = new FullComputedPropertyMapping(PropTag.Message.SenderGuid, column43, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageSenderGuid), new StorePropTag[]
			{
				PropTag.Message.SenderFlags,
				PropTag.Message.SentRepresentingGuid
			}, new PropertyMapping[]
			{
				computedPropertyMapping12,
				computedPropertyMapping11
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageSenderGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.SenderGuid, value35);
			PropertyColumn propertyColumn23 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping17 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingFlags, propertyColumn23, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSentRepresentingFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSentRepresentingFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingFlags, computedPropertyMapping17);
			Column[] dependOn33 = new Column[]
			{
				propertyColumn23
			};
			PropertyColumn propertyColumn24 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingEntryId, rowPropBagCreator, dependOn33);
			ComputedPropertyMapping computedPropertyMapping18 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingEntryId, propertyColumn24, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSentRepresentingEntryId), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping17
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSentRepresentingEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingEntryId, computedPropertyMapping18);
			Column[] dependOn34 = new Column[]
			{
				propertyColumn23,
				propertyColumn24
			};
			PropertyColumn propertyColumn25 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingAddressType, rowPropBagCreator, dependOn34);
			ComputedPropertyMapping computedPropertyMapping19 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingAddressType, propertyColumn25, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSentRepresentingAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags,
				PropTag.Message.OriginalSentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping17,
				computedPropertyMapping18
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSentRepresentingAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingAddressType, computedPropertyMapping19);
			Column[] dependOn35 = new Column[]
			{
				propertyColumn23,
				propertyColumn24
			};
			PropertyColumn propertyColumn26 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingEmailAddress, rowPropBagCreator, dependOn35);
			ComputedPropertyMapping computedPropertyMapping20 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingEmailAddress, propertyColumn26, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSentRepresentingEmailAddress), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags,
				PropTag.Message.OriginalSentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping17,
				computedPropertyMapping18
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSentRepresentingEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingEmailAddress, computedPropertyMapping20);
			Column[] dependOn36 = new Column[]
			{
				propertyColumn23,
				propertyColumn25,
				propertyColumn26,
				propertyColumn24
			};
			PropertyColumn propertyColumn27 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingSearchKey, rowPropBagCreator, dependOn36);
			ComputedPropertyMapping computedPropertyMapping21 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingSearchKey, propertyColumn27, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSentRepresentingSearchKey), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags,
				PropTag.Message.OriginalSentRepresentingAddressType,
				PropTag.Message.OriginalSentRepresentingEmailAddress,
				PropTag.Message.OriginalSentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping17,
				computedPropertyMapping19,
				computedPropertyMapping20,
				computedPropertyMapping18
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSentRepresentingSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingSearchKey, computedPropertyMapping21);
			Column[] dependOn37 = new Column[]
			{
				propertyColumn23,
				propertyColumn24,
				propertyColumn26
			};
			PropertyColumn propertyColumn28 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingSimpleDisplayName, rowPropBagCreator, dependOn37);
			ComputedPropertyMapping computedPropertyMapping22 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingSimpleDisplayName, propertyColumn28, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSentRepresentingSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags,
				PropTag.Message.OriginalSentRepresentingEntryId,
				PropTag.Message.OriginalSentRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping17,
				computedPropertyMapping18,
				computedPropertyMapping20
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSentRepresentingSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingSimpleDisplayName, computedPropertyMapping22);
			Column[] dependOn38 = new Column[]
			{
				propertyColumn23,
				propertyColumn24,
				propertyColumn28,
				propertyColumn26
			};
			PropertyColumn propertyColumn29 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingName, rowPropBagCreator, dependOn38);
			ComputedPropertyMapping computedPropertyMapping23 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingName, propertyColumn29, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSentRepresentingName), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags,
				PropTag.Message.OriginalSentRepresentingEntryId,
				PropTag.Message.OriginalSentRepresentingSimpleDisplayName,
				PropTag.Message.OriginalSentRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping17,
				computedPropertyMapping18,
				computedPropertyMapping22,
				computedPropertyMapping20
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSentRepresentingName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingName, computedPropertyMapping23);
			Column[] dependOn39 = new Column[]
			{
				propertyColumn23
			};
			PropertyColumn propertyColumn30 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingOrgAddressType, rowPropBagCreator, dependOn39);
			ComputedPropertyMapping computedPropertyMapping24 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingOrgAddressType, propertyColumn30, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSentRepresentingOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping17
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSentRepresentingOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingOrgAddressType, computedPropertyMapping24);
			Column[] dependOn40 = new Column[]
			{
				propertyColumn23
			};
			PropertyColumn propertyColumn31 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingOrgEmailAddr, rowPropBagCreator, dependOn40);
			ComputedPropertyMapping computedPropertyMapping25 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingOrgEmailAddr, propertyColumn31, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSentRepresentingOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping17
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSentRepresentingOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingOrgEmailAddr, computedPropertyMapping25);
			Column[] dependOn41 = new Column[]
			{
				propertyColumn23
			};
			PropertyColumn propertyColumn32 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingSid, rowPropBagCreator, dependOn41);
			ComputedPropertyMapping computedPropertyMapping26 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingSid, propertyColumn32, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSentRepresentingSid), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping17
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSentRepresentingSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingSid, computedPropertyMapping26);
			Column[] dependOn42 = new Column[]
			{
				propertyColumn23
			};
			PropertyColumn propertyColumn33 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSentRepresentingGuid, rowPropBagCreator, dependOn42);
			ComputedPropertyMapping computedPropertyMapping27 = new FullComputedPropertyMapping(PropTag.Message.OriginalSentRepresentingGuid, propertyColumn33, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSentRepresentingGuid), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping17
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSentRepresentingGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSentRepresentingGuid, computedPropertyMapping27);
			Column[] dependOn43 = new Column[]
			{
				propertyColumn23
			};
			PropertyColumn propertyColumn34 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderFlags, rowPropBagCreator, dependOn43);
			ComputedPropertyMapping computedPropertyMapping28 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderFlags, propertyColumn34, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSenderFlags), new StorePropTag[]
			{
				PropTag.Message.OriginalSentRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping17
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSenderFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderFlags, computedPropertyMapping28);
			Column[] dependOn44 = new Column[]
			{
				propertyColumn34,
				propertyColumn24
			};
			PropertyColumn propertyColumn35 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderEntryId, rowPropBagCreator, dependOn44);
			ComputedPropertyMapping computedPropertyMapping29 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderEntryId, propertyColumn35, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSenderEntryId), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSentRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping28,
				computedPropertyMapping18
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSenderEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderEntryId, computedPropertyMapping29);
			Column[] dependOn45 = new Column[]
			{
				propertyColumn34,
				propertyColumn35,
				propertyColumn25
			};
			PropertyColumn propertyColumn36 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderAddressType, rowPropBagCreator, dependOn45);
			ComputedPropertyMapping computedPropertyMapping30 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderAddressType, propertyColumn36, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSenderAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSenderEntryId,
				PropTag.Message.OriginalSentRepresentingAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping28,
				computedPropertyMapping29,
				computedPropertyMapping19
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSenderAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderAddressType, computedPropertyMapping30);
			Column[] dependOn46 = new Column[]
			{
				propertyColumn34,
				propertyColumn35,
				propertyColumn26
			};
			PropertyColumn propertyColumn37 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderEmailAddress, rowPropBagCreator, dependOn46);
			ComputedPropertyMapping computedPropertyMapping31 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderEmailAddress, propertyColumn37, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSenderEmailAddress), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSenderEntryId,
				PropTag.Message.OriginalSentRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping28,
				computedPropertyMapping29,
				computedPropertyMapping20
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSenderEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderEmailAddress, computedPropertyMapping31);
			Column[] dependOn47 = new Column[]
			{
				propertyColumn34,
				propertyColumn36,
				propertyColumn37,
				propertyColumn35,
				propertyColumn27
			};
			PropertyColumn column44 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderSearchKey, rowPropBagCreator, dependOn47);
			ComputedPropertyMapping value36 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderSearchKey, column44, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSenderSearchKey), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSenderAddressType,
				PropTag.Message.OriginalSenderEmailAddress,
				PropTag.Message.OriginalSenderEntryId,
				PropTag.Message.OriginalSentRepresentingSearchKey
			}, new PropertyMapping[]
			{
				computedPropertyMapping28,
				computedPropertyMapping30,
				computedPropertyMapping31,
				computedPropertyMapping29,
				computedPropertyMapping21
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSenderSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderSearchKey, value36);
			Column[] dependOn48 = new Column[]
			{
				propertyColumn34,
				propertyColumn35,
				propertyColumn37,
				propertyColumn28
			};
			PropertyColumn propertyColumn38 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderSimpleDisplayName, rowPropBagCreator, dependOn48);
			ComputedPropertyMapping computedPropertyMapping32 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderSimpleDisplayName, propertyColumn38, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSenderSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSenderEntryId,
				PropTag.Message.OriginalSenderEmailAddress,
				PropTag.Message.OriginalSentRepresentingSimpleDisplayName
			}, new PropertyMapping[]
			{
				computedPropertyMapping28,
				computedPropertyMapping29,
				computedPropertyMapping31,
				computedPropertyMapping22
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSenderSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderSimpleDisplayName, computedPropertyMapping32);
			Column[] dependOn49 = new Column[]
			{
				propertyColumn34,
				propertyColumn35,
				propertyColumn38,
				propertyColumn37,
				propertyColumn29
			};
			PropertyColumn column45 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderName, rowPropBagCreator, dependOn49);
			ComputedPropertyMapping value37 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderName, column45, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSenderName), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSenderEntryId,
				PropTag.Message.OriginalSenderSimpleDisplayName,
				PropTag.Message.OriginalSenderEmailAddress,
				PropTag.Message.OriginalSentRepresentingName
			}, new PropertyMapping[]
			{
				computedPropertyMapping28,
				computedPropertyMapping29,
				computedPropertyMapping32,
				computedPropertyMapping31,
				computedPropertyMapping23
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSenderName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderName, value37);
			Column[] dependOn50 = new Column[]
			{
				propertyColumn34,
				propertyColumn30
			};
			PropertyColumn column46 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderOrgAddressType, rowPropBagCreator, dependOn50);
			ComputedPropertyMapping value38 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderOrgAddressType, column46, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSenderOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSentRepresentingOrgAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping28,
				computedPropertyMapping24
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSenderOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderOrgAddressType, value38);
			Column[] dependOn51 = new Column[]
			{
				propertyColumn34,
				propertyColumn31
			};
			PropertyColumn column47 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderOrgEmailAddr, rowPropBagCreator, dependOn51);
			ComputedPropertyMapping value39 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderOrgEmailAddr, column47, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSenderOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSentRepresentingOrgEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping28,
				computedPropertyMapping25
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSenderOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderOrgEmailAddr, value39);
			Column[] dependOn52 = new Column[]
			{
				propertyColumn34,
				propertyColumn32
			};
			PropertyColumn column48 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderSid, rowPropBagCreator, dependOn52);
			ComputedPropertyMapping value40 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderSid, column48, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSenderSid), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSentRepresentingSid
			}, new PropertyMapping[]
			{
				computedPropertyMapping28,
				computedPropertyMapping26
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSenderSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderSid, value40);
			Column[] dependOn53 = new Column[]
			{
				propertyColumn34,
				propertyColumn33
			};
			PropertyColumn column49 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalSenderGuid, rowPropBagCreator, dependOn53);
			ComputedPropertyMapping value41 = new FullComputedPropertyMapping(PropTag.Message.OriginalSenderGuid, column49, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalSenderGuid), new StorePropTag[]
			{
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSentRepresentingGuid
			}, new PropertyMapping[]
			{
				computedPropertyMapping28,
				computedPropertyMapping27
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalSenderGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalSenderGuid, value41);
			PropertyColumn propertyColumn39 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdRepresentingFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping33 = new FullComputedPropertyMapping(PropTag.Message.RcvdRepresentingFlags, propertyColumn39, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageRcvdRepresentingFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageRcvdRepresentingFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdRepresentingFlags, computedPropertyMapping33);
			Column[] dependOn54 = new Column[]
			{
				propertyColumn39
			};
			PropertyColumn propertyColumn40 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingEntryId, rowPropBagCreator, dependOn54);
			ComputedPropertyMapping computedPropertyMapping34 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingEntryId, propertyColumn40, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedRepresentingEntryId), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping33
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedRepresentingEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingEntryId, computedPropertyMapping34);
			Column[] dependOn55 = new Column[]
			{
				propertyColumn39,
				propertyColumn40
			};
			PropertyColumn propertyColumn41 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingAddressType, rowPropBagCreator, dependOn55);
			ComputedPropertyMapping computedPropertyMapping35 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingAddressType, propertyColumn41, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedRepresentingAddressType), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags,
				PropTag.Message.ReceivedRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping33,
				computedPropertyMapping34
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedRepresentingAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingAddressType, computedPropertyMapping35);
			Column[] dependOn56 = new Column[]
			{
				propertyColumn39,
				propertyColumn40
			};
			PropertyColumn propertyColumn42 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingEmailAddress, rowPropBagCreator, dependOn56);
			ComputedPropertyMapping computedPropertyMapping36 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingEmailAddress, propertyColumn42, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedRepresentingEmailAddress), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags,
				PropTag.Message.ReceivedRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping33,
				computedPropertyMapping34
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedRepresentingEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingEmailAddress, computedPropertyMapping36);
			Column[] dependOn57 = new Column[]
			{
				propertyColumn39,
				propertyColumn41,
				propertyColumn42,
				propertyColumn40
			};
			PropertyColumn propertyColumn43 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingSearchKey, rowPropBagCreator, dependOn57);
			ComputedPropertyMapping computedPropertyMapping37 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingSearchKey, propertyColumn43, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedRepresentingSearchKey), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags,
				PropTag.Message.ReceivedRepresentingAddressType,
				PropTag.Message.ReceivedRepresentingEmailAddress,
				PropTag.Message.ReceivedRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping33,
				computedPropertyMapping35,
				computedPropertyMapping36,
				computedPropertyMapping34
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedRepresentingSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingSearchKey, computedPropertyMapping37);
			Column[] dependOn58 = new Column[]
			{
				propertyColumn39,
				propertyColumn40,
				propertyColumn42
			};
			PropertyColumn propertyColumn44 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingSimpleDisplayName, rowPropBagCreator, dependOn58);
			ComputedPropertyMapping computedPropertyMapping38 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingSimpleDisplayName, propertyColumn44, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedRepresentingSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags,
				PropTag.Message.ReceivedRepresentingEntryId,
				PropTag.Message.ReceivedRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping33,
				computedPropertyMapping34,
				computedPropertyMapping36
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedRepresentingSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingSimpleDisplayName, computedPropertyMapping38);
			Column[] dependOn59 = new Column[]
			{
				propertyColumn39,
				propertyColumn40,
				propertyColumn44,
				propertyColumn42
			};
			PropertyColumn propertyColumn45 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingName, rowPropBagCreator, dependOn59);
			ComputedPropertyMapping computedPropertyMapping39 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingName, propertyColumn45, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedRepresentingName), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags,
				PropTag.Message.ReceivedRepresentingEntryId,
				PropTag.Message.ReceivedRepresentingSimpleDisplayName,
				PropTag.Message.ReceivedRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping33,
				computedPropertyMapping34,
				computedPropertyMapping38,
				computedPropertyMapping36
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedRepresentingName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingName, computedPropertyMapping39);
			Column[] dependOn60 = new Column[]
			{
				propertyColumn39
			};
			PropertyColumn propertyColumn46 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdRepresentingOrgAddressType, rowPropBagCreator, dependOn60);
			ComputedPropertyMapping computedPropertyMapping40 = new FullComputedPropertyMapping(PropTag.Message.RcvdRepresentingOrgAddressType, propertyColumn46, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageRcvdRepresentingOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping33
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageRcvdRepresentingOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdRepresentingOrgAddressType, computedPropertyMapping40);
			Column[] dependOn61 = new Column[]
			{
				propertyColumn39
			};
			PropertyColumn propertyColumn47 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdRepresentingOrgEmailAddr, rowPropBagCreator, dependOn61);
			ComputedPropertyMapping computedPropertyMapping41 = new FullComputedPropertyMapping(PropTag.Message.RcvdRepresentingOrgEmailAddr, propertyColumn47, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageRcvdRepresentingOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping33
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageRcvdRepresentingOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdRepresentingOrgEmailAddr, computedPropertyMapping41);
			Column[] dependOn62 = new Column[]
			{
				propertyColumn39
			};
			PropertyColumn propertyColumn48 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdRepresentingSid, rowPropBagCreator, dependOn62);
			ComputedPropertyMapping computedPropertyMapping42 = new FullComputedPropertyMapping(PropTag.Message.RcvdRepresentingSid, propertyColumn48, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageRcvdRepresentingSid), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping33
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageRcvdRepresentingSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdRepresentingSid, computedPropertyMapping42);
			Column[] dependOn63 = new Column[]
			{
				propertyColumn39
			};
			PropertyColumn propertyColumn49 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedRepresentingGuid, rowPropBagCreator, dependOn63);
			ComputedPropertyMapping computedPropertyMapping43 = new FullComputedPropertyMapping(PropTag.Message.ReceivedRepresentingGuid, propertyColumn49, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedRepresentingGuid), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping33
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedRepresentingGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedRepresentingGuid, computedPropertyMapping43);
			Column[] dependOn64 = new Column[]
			{
				propertyColumn39
			};
			PropertyColumn propertyColumn50 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdByFlags, rowPropBagCreator, dependOn64);
			ComputedPropertyMapping computedPropertyMapping44 = new FullComputedPropertyMapping(PropTag.Message.RcvdByFlags, propertyColumn50, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageRcvdByFlags), new StorePropTag[]
			{
				PropTag.Message.RcvdRepresentingFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping33
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageRcvdByFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdByFlags, computedPropertyMapping44);
			Column[] dependOn65 = new Column[]
			{
				propertyColumn50,
				propertyColumn40
			};
			PropertyColumn propertyColumn51 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedByEntryId, rowPropBagCreator, dependOn65);
			ComputedPropertyMapping computedPropertyMapping45 = new FullComputedPropertyMapping(PropTag.Message.ReceivedByEntryId, propertyColumn51, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedByEntryId), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedRepresentingEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping44,
				computedPropertyMapping34
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedByEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedByEntryId, computedPropertyMapping45);
			Column[] dependOn66 = new Column[]
			{
				propertyColumn50,
				propertyColumn51,
				propertyColumn41
			};
			PropertyColumn propertyColumn52 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedByAddressType, rowPropBagCreator, dependOn66);
			ComputedPropertyMapping computedPropertyMapping46 = new FullComputedPropertyMapping(PropTag.Message.ReceivedByAddressType, propertyColumn52, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedByAddressType), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedByEntryId,
				PropTag.Message.ReceivedRepresentingAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping44,
				computedPropertyMapping45,
				computedPropertyMapping35
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedByAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedByAddressType, computedPropertyMapping46);
			Column[] dependOn67 = new Column[]
			{
				propertyColumn50,
				propertyColumn51,
				propertyColumn42
			};
			PropertyColumn propertyColumn53 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedByEmailAddress, rowPropBagCreator, dependOn67);
			ComputedPropertyMapping computedPropertyMapping47 = new FullComputedPropertyMapping(PropTag.Message.ReceivedByEmailAddress, propertyColumn53, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedByEmailAddress), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedByEntryId,
				PropTag.Message.ReceivedRepresentingEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping44,
				computedPropertyMapping45,
				computedPropertyMapping36
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedByEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedByEmailAddress, computedPropertyMapping47);
			Column[] dependOn68 = new Column[]
			{
				propertyColumn50,
				propertyColumn52,
				propertyColumn53,
				propertyColumn51,
				propertyColumn43
			};
			PropertyColumn column50 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedBySearchKey, rowPropBagCreator, dependOn68);
			ComputedPropertyMapping value42 = new FullComputedPropertyMapping(PropTag.Message.ReceivedBySearchKey, column50, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedBySearchKey), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedByAddressType,
				PropTag.Message.ReceivedByEmailAddress,
				PropTag.Message.ReceivedByEntryId,
				PropTag.Message.ReceivedRepresentingSearchKey
			}, new PropertyMapping[]
			{
				computedPropertyMapping44,
				computedPropertyMapping46,
				computedPropertyMapping47,
				computedPropertyMapping45,
				computedPropertyMapping37
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedBySearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedBySearchKey, value42);
			Column[] dependOn69 = new Column[]
			{
				propertyColumn50,
				propertyColumn51,
				propertyColumn53,
				propertyColumn44
			};
			PropertyColumn propertyColumn54 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedBySimpleDisplayName, rowPropBagCreator, dependOn69);
			ComputedPropertyMapping computedPropertyMapping48 = new FullComputedPropertyMapping(PropTag.Message.ReceivedBySimpleDisplayName, propertyColumn54, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedBySimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedByEntryId,
				PropTag.Message.ReceivedByEmailAddress,
				PropTag.Message.ReceivedRepresentingSimpleDisplayName
			}, new PropertyMapping[]
			{
				computedPropertyMapping44,
				computedPropertyMapping45,
				computedPropertyMapping47,
				computedPropertyMapping38
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedBySimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedBySimpleDisplayName, computedPropertyMapping48);
			Column[] dependOn70 = new Column[]
			{
				propertyColumn50,
				propertyColumn51,
				propertyColumn54,
				propertyColumn53,
				propertyColumn45
			};
			PropertyColumn column51 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedByName, rowPropBagCreator, dependOn70);
			ComputedPropertyMapping value43 = new FullComputedPropertyMapping(PropTag.Message.ReceivedByName, column51, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedByName), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedByEntryId,
				PropTag.Message.ReceivedBySimpleDisplayName,
				PropTag.Message.ReceivedByEmailAddress,
				PropTag.Message.ReceivedRepresentingName
			}, new PropertyMapping[]
			{
				computedPropertyMapping44,
				computedPropertyMapping45,
				computedPropertyMapping48,
				computedPropertyMapping47,
				computedPropertyMapping39
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedByName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedByName, value43);
			Column[] dependOn71 = new Column[]
			{
				propertyColumn50,
				propertyColumn46
			};
			PropertyColumn column52 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdByOrgAddressType, rowPropBagCreator, dependOn71);
			ComputedPropertyMapping value44 = new FullComputedPropertyMapping(PropTag.Message.RcvdByOrgAddressType, column52, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageRcvdByOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.RcvdRepresentingOrgAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping44,
				computedPropertyMapping40
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageRcvdByOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdByOrgAddressType, value44);
			Column[] dependOn72 = new Column[]
			{
				propertyColumn50,
				propertyColumn47
			};
			PropertyColumn column53 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdByOrgEmailAddr, rowPropBagCreator, dependOn72);
			ComputedPropertyMapping value45 = new FullComputedPropertyMapping(PropTag.Message.RcvdByOrgEmailAddr, column53, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageRcvdByOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.RcvdRepresentingOrgEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping44,
				computedPropertyMapping41
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageRcvdByOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdByOrgEmailAddr, value45);
			Column[] dependOn73 = new Column[]
			{
				propertyColumn50,
				propertyColumn48
			};
			PropertyColumn column54 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.RcvdBySid, rowPropBagCreator, dependOn73);
			ComputedPropertyMapping value46 = new FullComputedPropertyMapping(PropTag.Message.RcvdBySid, column54, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageRcvdBySid), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.RcvdRepresentingSid
			}, new PropertyMapping[]
			{
				computedPropertyMapping44,
				computedPropertyMapping42
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageRcvdBySid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.RcvdBySid, value46);
			Column[] dependOn74 = new Column[]
			{
				propertyColumn50,
				propertyColumn49
			};
			PropertyColumn column55 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReceivedByGuid, rowPropBagCreator, dependOn74);
			ComputedPropertyMapping value47 = new FullComputedPropertyMapping(PropTag.Message.ReceivedByGuid, column55, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReceivedByGuid), new StorePropTag[]
			{
				PropTag.Message.RcvdByFlags,
				PropTag.Message.ReceivedRepresentingGuid
			}, new PropertyMapping[]
			{
				computedPropertyMapping44,
				computedPropertyMapping43
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReceivedByGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReceivedByGuid, value47);
			PropertyColumn propertyColumn55 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping49 = new FullComputedPropertyMapping(PropTag.Message.CreatorFlags, propertyColumn55, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageCreatorFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageCreatorFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorFlags, computedPropertyMapping49);
			Column[] dependOn75 = new Column[]
			{
				propertyColumn55
			};
			PropertyColumn propertyColumn56 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorEntryId, rowPropBagCreator, dependOn75);
			ComputedPropertyMapping computedPropertyMapping50 = new FullComputedPropertyMapping(PropTag.Message.CreatorEntryId, propertyColumn56, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageCreatorEntryId), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping49
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageCreatorEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorEntryId, computedPropertyMapping50);
			Column[] dependOn76 = new Column[]
			{
				propertyColumn55,
				propertyColumn56
			};
			PropertyColumn propertyColumn57 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorAddressType, rowPropBagCreator, dependOn76);
			ComputedPropertyMapping computedPropertyMapping51 = new FullComputedPropertyMapping(PropTag.Message.CreatorAddressType, propertyColumn57, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageCreatorAddressType), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags,
				PropTag.Message.CreatorEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping49,
				computedPropertyMapping50
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageCreatorAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorAddressType, computedPropertyMapping51);
			Column[] dependOn77 = new Column[]
			{
				propertyColumn55,
				propertyColumn56
			};
			PropertyColumn propertyColumn58 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorEmailAddr, rowPropBagCreator, dependOn77);
			ComputedPropertyMapping computedPropertyMapping52 = new FullComputedPropertyMapping(PropTag.Message.CreatorEmailAddr, propertyColumn58, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageCreatorEmailAddr), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags,
				PropTag.Message.CreatorEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping49,
				computedPropertyMapping50
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageCreatorEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorEmailAddr, computedPropertyMapping52);
			Column[] dependOn78 = new Column[]
			{
				propertyColumn55,
				propertyColumn56,
				propertyColumn58
			};
			PropertyColumn propertyColumn59 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorSimpleDisplayName, rowPropBagCreator, dependOn78);
			ComputedPropertyMapping computedPropertyMapping53 = new FullComputedPropertyMapping(PropTag.Message.CreatorSimpleDisplayName, propertyColumn59, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageCreatorSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags,
				PropTag.Message.CreatorEntryId,
				PropTag.Message.CreatorEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping49,
				computedPropertyMapping50,
				computedPropertyMapping52
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageCreatorSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorSimpleDisplayName, computedPropertyMapping53);
			Column[] dependOn79 = new Column[]
			{
				propertyColumn55,
				propertyColumn56,
				propertyColumn59,
				propertyColumn58
			};
			PropertyColumn propertyColumn60 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorName, rowPropBagCreator, dependOn79);
			ComputedPropertyMapping computedPropertyMapping54 = new FullComputedPropertyMapping(PropTag.Message.CreatorName, propertyColumn60, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageCreatorName), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags,
				PropTag.Message.CreatorEntryId,
				PropTag.Message.CreatorSimpleDisplayName,
				PropTag.Message.CreatorEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping49,
				computedPropertyMapping50,
				computedPropertyMapping53,
				computedPropertyMapping52
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageCreatorName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorName, computedPropertyMapping54);
			Column[] dependOn80 = new Column[]
			{
				propertyColumn55
			};
			PropertyColumn propertyColumn61 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorOrgAddressType, rowPropBagCreator, dependOn80);
			ComputedPropertyMapping computedPropertyMapping55 = new FullComputedPropertyMapping(PropTag.Message.CreatorOrgAddressType, propertyColumn61, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageCreatorOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping49
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageCreatorOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorOrgAddressType, computedPropertyMapping55);
			Column[] dependOn81 = new Column[]
			{
				propertyColumn55
			};
			PropertyColumn propertyColumn62 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorOrgEmailAddr, rowPropBagCreator, dependOn81);
			ComputedPropertyMapping computedPropertyMapping56 = new FullComputedPropertyMapping(PropTag.Message.CreatorOrgEmailAddr, propertyColumn62, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageCreatorOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping49
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageCreatorOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorOrgEmailAddr, computedPropertyMapping56);
			Column[] dependOn82 = new Column[]
			{
				propertyColumn55
			};
			PropertyColumn propertyColumn63 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorSID, rowPropBagCreator, dependOn82);
			ComputedPropertyMapping computedPropertyMapping57 = new FullComputedPropertyMapping(PropTag.Message.CreatorSID, propertyColumn63, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageCreatorSID), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping49
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageCreatorSID), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorSID, computedPropertyMapping57);
			Column[] dependOn83 = new Column[]
			{
				propertyColumn55
			};
			PropertyColumn propertyColumn64 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.CreatorGuid, rowPropBagCreator, dependOn83);
			ComputedPropertyMapping computedPropertyMapping58 = new FullComputedPropertyMapping(PropTag.Message.CreatorGuid, propertyColumn64, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageCreatorGuid), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping49
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageCreatorGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.CreatorGuid, computedPropertyMapping58);
			Column[] dependOn84 = new Column[]
			{
				propertyColumn55
			};
			PropertyColumn propertyColumn65 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierFlags, rowPropBagCreator, dependOn84);
			ComputedPropertyMapping computedPropertyMapping59 = new FullComputedPropertyMapping(PropTag.Message.LastModifierFlags, propertyColumn65, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageLastModifierFlags), new StorePropTag[]
			{
				PropTag.Message.CreatorFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping49
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageLastModifierFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierFlags, computedPropertyMapping59);
			Column[] dependOn85 = new Column[]
			{
				propertyColumn65,
				propertyColumn56
			};
			PropertyColumn propertyColumn66 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierEntryId, rowPropBagCreator, dependOn85);
			ComputedPropertyMapping computedPropertyMapping60 = new FullComputedPropertyMapping(PropTag.Message.LastModifierEntryId, propertyColumn66, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageLastModifierEntryId), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.CreatorEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping59,
				computedPropertyMapping50
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageLastModifierEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierEntryId, computedPropertyMapping60);
			Column[] dependOn86 = new Column[]
			{
				propertyColumn65,
				propertyColumn66,
				propertyColumn57
			};
			PropertyColumn propertyColumn67 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierAddressType, rowPropBagCreator, dependOn86);
			ComputedPropertyMapping computedPropertyMapping61 = new FullComputedPropertyMapping(PropTag.Message.LastModifierAddressType, propertyColumn67, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageLastModifierAddressType), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.LastModifierEntryId,
				PropTag.Message.CreatorAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping59,
				computedPropertyMapping60,
				computedPropertyMapping51
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageLastModifierAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierAddressType, computedPropertyMapping61);
			Column[] dependOn87 = new Column[]
			{
				propertyColumn65,
				propertyColumn66,
				propertyColumn58
			};
			PropertyColumn propertyColumn68 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierEmailAddr, rowPropBagCreator, dependOn87);
			ComputedPropertyMapping computedPropertyMapping62 = new FullComputedPropertyMapping(PropTag.Message.LastModifierEmailAddr, propertyColumn68, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageLastModifierEmailAddr), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.LastModifierEntryId,
				PropTag.Message.CreatorEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping59,
				computedPropertyMapping60,
				computedPropertyMapping52
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageLastModifierEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierEmailAddr, computedPropertyMapping62);
			Column[] dependOn88 = new Column[]
			{
				propertyColumn65,
				propertyColumn66,
				propertyColumn68,
				propertyColumn59
			};
			PropertyColumn propertyColumn69 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierSimpleDisplayName, rowPropBagCreator, dependOn88);
			ComputedPropertyMapping computedPropertyMapping63 = new FullComputedPropertyMapping(PropTag.Message.LastModifierSimpleDisplayName, propertyColumn69, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageLastModifierSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.LastModifierEntryId,
				PropTag.Message.LastModifierEmailAddr,
				PropTag.Message.CreatorSimpleDisplayName
			}, new PropertyMapping[]
			{
				computedPropertyMapping59,
				computedPropertyMapping60,
				computedPropertyMapping62,
				computedPropertyMapping53
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageLastModifierSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierSimpleDisplayName, computedPropertyMapping63);
			Column[] dependOn89 = new Column[]
			{
				propertyColumn65,
				propertyColumn66,
				propertyColumn69,
				propertyColumn68,
				propertyColumn60
			};
			PropertyColumn propertyColumn70 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierName, rowPropBagCreator, dependOn89);
			ComputedPropertyMapping computedPropertyMapping64 = new FullComputedPropertyMapping(PropTag.Message.LastModifierName, propertyColumn70, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageLastModifierName), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.LastModifierEntryId,
				PropTag.Message.LastModifierSimpleDisplayName,
				PropTag.Message.LastModifierEmailAddr,
				PropTag.Message.CreatorName
			}, new PropertyMapping[]
			{
				computedPropertyMapping59,
				computedPropertyMapping60,
				computedPropertyMapping63,
				computedPropertyMapping62,
				computedPropertyMapping54
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageLastModifierName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierName, computedPropertyMapping64);
			Column[] dependOn90 = new Column[]
			{
				propertyColumn65,
				propertyColumn61
			};
			PropertyColumn propertyColumn71 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierOrgAddressType, rowPropBagCreator, dependOn90);
			ComputedPropertyMapping computedPropertyMapping65 = new FullComputedPropertyMapping(PropTag.Message.LastModifierOrgAddressType, propertyColumn71, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageLastModifierOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.CreatorOrgAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping59,
				computedPropertyMapping55
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageLastModifierOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierOrgAddressType, computedPropertyMapping65);
			Column[] dependOn91 = new Column[]
			{
				propertyColumn65,
				propertyColumn62
			};
			PropertyColumn propertyColumn72 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierOrgEmailAddr, rowPropBagCreator, dependOn91);
			ComputedPropertyMapping computedPropertyMapping66 = new FullComputedPropertyMapping(PropTag.Message.LastModifierOrgEmailAddr, propertyColumn72, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageLastModifierOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.CreatorOrgEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping59,
				computedPropertyMapping56
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageLastModifierOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierOrgEmailAddr, computedPropertyMapping66);
			Column[] dependOn92 = new Column[]
			{
				propertyColumn65,
				propertyColumn63
			};
			PropertyColumn propertyColumn73 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierSid, rowPropBagCreator, dependOn92);
			ComputedPropertyMapping computedPropertyMapping67 = new FullComputedPropertyMapping(PropTag.Message.LastModifierSid, propertyColumn73, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageLastModifierSid), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.CreatorSID
			}, new PropertyMapping[]
			{
				computedPropertyMapping59,
				computedPropertyMapping57
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageLastModifierSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierSid, computedPropertyMapping67);
			Column[] dependOn93 = new Column[]
			{
				propertyColumn65,
				propertyColumn64
			};
			PropertyColumn propertyColumn74 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.LastModifierGuid, rowPropBagCreator, dependOn93);
			ComputedPropertyMapping computedPropertyMapping68 = new FullComputedPropertyMapping(PropTag.Message.LastModifierGuid, propertyColumn74, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageLastModifierGuid), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags,
				PropTag.Message.CreatorGuid
			}, new PropertyMapping[]
			{
				computedPropertyMapping59,
				computedPropertyMapping58
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageLastModifierGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.LastModifierGuid, computedPropertyMapping68);
			PropertyColumn propertyColumn75 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping69 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptFlags, propertyColumn75, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReadReceiptFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReadReceiptFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptFlags, computedPropertyMapping69);
			Column[] dependOn94 = new Column[]
			{
				propertyColumn75
			};
			PropertyColumn propertyColumn76 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptEntryId, rowPropBagCreator, dependOn94);
			ComputedPropertyMapping computedPropertyMapping70 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptEntryId, propertyColumn76, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReadReceiptEntryId), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping69
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReadReceiptEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptEntryId, computedPropertyMapping70);
			Column[] dependOn95 = new Column[]
			{
				propertyColumn75,
				propertyColumn76
			};
			PropertyColumn propertyColumn77 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptAddressType, rowPropBagCreator, dependOn95);
			ComputedPropertyMapping computedPropertyMapping71 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptAddressType, propertyColumn77, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReadReceiptAddressType), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags,
				PropTag.Message.ReadReceiptEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping69,
				computedPropertyMapping70
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReadReceiptAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptAddressType, computedPropertyMapping71);
			Column[] dependOn96 = new Column[]
			{
				propertyColumn75,
				propertyColumn76
			};
			PropertyColumn propertyColumn78 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptEmailAddress, rowPropBagCreator, dependOn96);
			ComputedPropertyMapping computedPropertyMapping72 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptEmailAddress, propertyColumn78, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReadReceiptEmailAddress), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags,
				PropTag.Message.ReadReceiptEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping69,
				computedPropertyMapping70
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReadReceiptEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptEmailAddress, computedPropertyMapping72);
			Column[] dependOn97 = new Column[]
			{
				propertyColumn75,
				propertyColumn77,
				propertyColumn78,
				propertyColumn76
			};
			PropertyColumn column56 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptSearchKey, rowPropBagCreator, dependOn97);
			ComputedPropertyMapping value48 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptSearchKey, column56, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReadReceiptSearchKey), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags,
				PropTag.Message.ReadReceiptAddressType,
				PropTag.Message.ReadReceiptEmailAddress,
				PropTag.Message.ReadReceiptEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping69,
				computedPropertyMapping71,
				computedPropertyMapping72,
				computedPropertyMapping70
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReadReceiptSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptSearchKey, value48);
			Column[] dependOn98 = new Column[]
			{
				propertyColumn75,
				propertyColumn76,
				propertyColumn78
			};
			PropertyColumn propertyColumn79 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptSimpleDisplayName, rowPropBagCreator, dependOn98);
			ComputedPropertyMapping computedPropertyMapping73 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptSimpleDisplayName, propertyColumn79, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReadReceiptSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags,
				PropTag.Message.ReadReceiptEntryId,
				PropTag.Message.ReadReceiptEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping69,
				computedPropertyMapping70,
				computedPropertyMapping72
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReadReceiptSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptSimpleDisplayName, computedPropertyMapping73);
			Column[] dependOn99 = new Column[]
			{
				propertyColumn75,
				propertyColumn76,
				propertyColumn79,
				propertyColumn78
			};
			PropertyColumn column57 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptDisplayName, rowPropBagCreator, dependOn99);
			ComputedPropertyMapping value49 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptDisplayName, column57, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReadReceiptDisplayName), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags,
				PropTag.Message.ReadReceiptEntryId,
				PropTag.Message.ReadReceiptSimpleDisplayName,
				PropTag.Message.ReadReceiptEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping69,
				computedPropertyMapping70,
				computedPropertyMapping73,
				computedPropertyMapping72
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReadReceiptDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptDisplayName, value49);
			Column[] dependOn100 = new Column[]
			{
				propertyColumn75
			};
			PropertyColumn column58 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptOrgAddressType, rowPropBagCreator, dependOn100);
			ComputedPropertyMapping value50 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptOrgAddressType, column58, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReadReceiptOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping69
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReadReceiptOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptOrgAddressType, value50);
			Column[] dependOn101 = new Column[]
			{
				propertyColumn75
			};
			PropertyColumn column59 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptOrgEmailAddr, rowPropBagCreator, dependOn101);
			ComputedPropertyMapping value51 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptOrgEmailAddr, column59, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReadReceiptOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping69
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReadReceiptOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptOrgEmailAddr, value51);
			Column[] dependOn102 = new Column[]
			{
				propertyColumn75
			};
			PropertyColumn column60 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptSid, rowPropBagCreator, dependOn102);
			ComputedPropertyMapping value52 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptSid, column60, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReadReceiptSid), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping69
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReadReceiptSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptSid, value52);
			Column[] dependOn103 = new Column[]
			{
				propertyColumn75
			};
			PropertyColumn column61 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReadReceiptGuid, rowPropBagCreator, dependOn103);
			ComputedPropertyMapping value53 = new FullComputedPropertyMapping(PropTag.Message.ReadReceiptGuid, column61, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReadReceiptGuid), new StorePropTag[]
			{
				PropTag.Message.ReadReceiptFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping69
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReadReceiptGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReadReceiptGuid, value53);
			PropertyColumn propertyColumn80 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping74 = new FullComputedPropertyMapping(PropTag.Message.ReportFlags, propertyColumn80, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportFlags, computedPropertyMapping74);
			Column[] dependOn104 = new Column[]
			{
				propertyColumn80
			};
			PropertyColumn propertyColumn81 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportEntryId, rowPropBagCreator, dependOn104);
			ComputedPropertyMapping computedPropertyMapping75 = new FullComputedPropertyMapping(PropTag.Message.ReportEntryId, propertyColumn81, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportEntryId), new StorePropTag[]
			{
				PropTag.Message.ReportFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping74
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportEntryId, computedPropertyMapping75);
			Column[] dependOn105 = new Column[]
			{
				propertyColumn80,
				propertyColumn81
			};
			PropertyColumn propertyColumn82 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportAddressType, rowPropBagCreator, dependOn105);
			ComputedPropertyMapping computedPropertyMapping76 = new FullComputedPropertyMapping(PropTag.Message.ReportAddressType, propertyColumn82, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportAddressType), new StorePropTag[]
			{
				PropTag.Message.ReportFlags,
				PropTag.Message.ReportEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping74,
				computedPropertyMapping75
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportAddressType, computedPropertyMapping76);
			Column[] dependOn106 = new Column[]
			{
				propertyColumn80,
				propertyColumn81
			};
			PropertyColumn propertyColumn83 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportEmailAddress, rowPropBagCreator, dependOn106);
			ComputedPropertyMapping computedPropertyMapping77 = new FullComputedPropertyMapping(PropTag.Message.ReportEmailAddress, propertyColumn83, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportEmailAddress), new StorePropTag[]
			{
				PropTag.Message.ReportFlags,
				PropTag.Message.ReportEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping74,
				computedPropertyMapping75
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportEmailAddress, computedPropertyMapping77);
			Column[] dependOn107 = new Column[]
			{
				propertyColumn80,
				propertyColumn82,
				propertyColumn83,
				propertyColumn81
			};
			PropertyColumn column62 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportSearchKey, rowPropBagCreator, dependOn107);
			ComputedPropertyMapping value54 = new FullComputedPropertyMapping(PropTag.Message.ReportSearchKey, column62, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportSearchKey), new StorePropTag[]
			{
				PropTag.Message.ReportFlags,
				PropTag.Message.ReportAddressType,
				PropTag.Message.ReportEmailAddress,
				PropTag.Message.ReportEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping74,
				computedPropertyMapping76,
				computedPropertyMapping77,
				computedPropertyMapping75
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportSearchKey, value54);
			Column[] dependOn108 = new Column[]
			{
				propertyColumn80,
				propertyColumn81,
				propertyColumn83
			};
			PropertyColumn propertyColumn84 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportSimpleDisplayName, rowPropBagCreator, dependOn108);
			ComputedPropertyMapping computedPropertyMapping78 = new FullComputedPropertyMapping(PropTag.Message.ReportSimpleDisplayName, propertyColumn84, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.ReportFlags,
				PropTag.Message.ReportEntryId,
				PropTag.Message.ReportEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping74,
				computedPropertyMapping75,
				computedPropertyMapping77
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportSimpleDisplayName, computedPropertyMapping78);
			Column[] dependOn109 = new Column[]
			{
				propertyColumn80,
				propertyColumn81,
				propertyColumn84,
				propertyColumn83
			};
			PropertyColumn column63 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDisplayName, rowPropBagCreator, dependOn109);
			ComputedPropertyMapping value55 = new FullComputedPropertyMapping(PropTag.Message.ReportDisplayName, column63, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportDisplayName), new StorePropTag[]
			{
				PropTag.Message.ReportFlags,
				PropTag.Message.ReportEntryId,
				PropTag.Message.ReportSimpleDisplayName,
				PropTag.Message.ReportEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping74,
				computedPropertyMapping75,
				computedPropertyMapping78,
				computedPropertyMapping77
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDisplayName, value55);
			Column[] dependOn110 = new Column[]
			{
				propertyColumn80
			};
			PropertyColumn column64 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportOrgAddressType, rowPropBagCreator, dependOn110);
			ComputedPropertyMapping value56 = new FullComputedPropertyMapping(PropTag.Message.ReportOrgAddressType, column64, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.ReportFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping74
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportOrgAddressType, value56);
			Column[] dependOn111 = new Column[]
			{
				propertyColumn80
			};
			PropertyColumn column65 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportOrgEmailAddr, rowPropBagCreator, dependOn111);
			ComputedPropertyMapping value57 = new FullComputedPropertyMapping(PropTag.Message.ReportOrgEmailAddr, column65, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.ReportFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping74
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportOrgEmailAddr, value57);
			Column[] dependOn112 = new Column[]
			{
				propertyColumn80
			};
			PropertyColumn column66 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportSid, rowPropBagCreator, dependOn112);
			ComputedPropertyMapping value58 = new FullComputedPropertyMapping(PropTag.Message.ReportSid, column66, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportSid), new StorePropTag[]
			{
				PropTag.Message.ReportFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping74
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportSid, value58);
			Column[] dependOn113 = new Column[]
			{
				propertyColumn80
			};
			PropertyColumn column67 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportGuid, rowPropBagCreator, dependOn113);
			ComputedPropertyMapping value59 = new FullComputedPropertyMapping(PropTag.Message.ReportGuid, column67, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportGuid), new StorePropTag[]
			{
				PropTag.Message.ReportFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping74
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportGuid, value59);
			Column[] dependOn114 = new Column[]
			{
				propertyColumn65
			};
			PropertyColumn propertyColumn85 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorFlags, rowPropBagCreator, dependOn114);
			ComputedPropertyMapping computedPropertyMapping79 = new FullComputedPropertyMapping(PropTag.Message.OriginatorFlags, propertyColumn85, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginatorFlags), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping59
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginatorFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorFlags, computedPropertyMapping79);
			Column[] dependOn115 = new Column[]
			{
				propertyColumn85,
				propertyColumn66
			};
			PropertyColumn propertyColumn86 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorEntryId, rowPropBagCreator, dependOn115);
			ComputedPropertyMapping computedPropertyMapping80 = new FullComputedPropertyMapping(PropTag.Message.OriginatorEntryId, propertyColumn86, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginatorEntryId), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.LastModifierEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping79,
				computedPropertyMapping60
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginatorEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorEntryId, computedPropertyMapping80);
			Column[] dependOn116 = new Column[]
			{
				propertyColumn85,
				propertyColumn86,
				propertyColumn67
			};
			PropertyColumn propertyColumn87 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorAddressType, rowPropBagCreator, dependOn116);
			ComputedPropertyMapping computedPropertyMapping81 = new FullComputedPropertyMapping(PropTag.Message.OriginatorAddressType, propertyColumn87, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginatorAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.OriginatorEntryId,
				PropTag.Message.LastModifierAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping79,
				computedPropertyMapping80,
				computedPropertyMapping61
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginatorAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorAddressType, computedPropertyMapping81);
			Column[] dependOn117 = new Column[]
			{
				propertyColumn85,
				propertyColumn86,
				propertyColumn68
			};
			PropertyColumn propertyColumn88 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorEmailAddress, rowPropBagCreator, dependOn117);
			ComputedPropertyMapping computedPropertyMapping82 = new FullComputedPropertyMapping(PropTag.Message.OriginatorEmailAddress, propertyColumn88, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginatorEmailAddress), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.OriginatorEntryId,
				PropTag.Message.LastModifierEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping79,
				computedPropertyMapping80,
				computedPropertyMapping62
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginatorEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorEmailAddress, computedPropertyMapping82);
			Column[] dependOn118 = new Column[]
			{
				propertyColumn85,
				propertyColumn87,
				propertyColumn88,
				propertyColumn86
			};
			PropertyColumn column68 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorSearchKey, rowPropBagCreator, dependOn118);
			ComputedPropertyMapping value60 = new FullComputedPropertyMapping(PropTag.Message.OriginatorSearchKey, column68, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginatorSearchKey), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.OriginatorAddressType,
				PropTag.Message.OriginatorEmailAddress,
				PropTag.Message.OriginatorEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping79,
				computedPropertyMapping81,
				computedPropertyMapping82,
				computedPropertyMapping80
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginatorSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorSearchKey, value60);
			Column[] dependOn119 = new Column[]
			{
				propertyColumn85,
				propertyColumn86,
				propertyColumn88,
				propertyColumn69
			};
			PropertyColumn propertyColumn89 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorSimpleDisplayName, rowPropBagCreator, dependOn119);
			ComputedPropertyMapping computedPropertyMapping83 = new FullComputedPropertyMapping(PropTag.Message.OriginatorSimpleDisplayName, propertyColumn89, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginatorSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.OriginatorEntryId,
				PropTag.Message.OriginatorEmailAddress,
				PropTag.Message.LastModifierSimpleDisplayName
			}, new PropertyMapping[]
			{
				computedPropertyMapping79,
				computedPropertyMapping80,
				computedPropertyMapping82,
				computedPropertyMapping63
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginatorSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorSimpleDisplayName, computedPropertyMapping83);
			Column[] dependOn120 = new Column[]
			{
				propertyColumn85,
				propertyColumn86,
				propertyColumn89,
				propertyColumn88,
				propertyColumn70
			};
			PropertyColumn column69 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorName, rowPropBagCreator, dependOn120);
			ComputedPropertyMapping value61 = new FullComputedPropertyMapping(PropTag.Message.OriginatorName, column69, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginatorName), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.OriginatorEntryId,
				PropTag.Message.OriginatorSimpleDisplayName,
				PropTag.Message.OriginatorEmailAddress,
				PropTag.Message.LastModifierName
			}, new PropertyMapping[]
			{
				computedPropertyMapping79,
				computedPropertyMapping80,
				computedPropertyMapping83,
				computedPropertyMapping82,
				computedPropertyMapping64
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginatorName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorName, value61);
			Column[] dependOn121 = new Column[]
			{
				propertyColumn85,
				propertyColumn71
			};
			PropertyColumn column70 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorOrgAddressType, rowPropBagCreator, dependOn121);
			ComputedPropertyMapping value62 = new FullComputedPropertyMapping(PropTag.Message.OriginatorOrgAddressType, column70, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginatorOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.LastModifierOrgAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping79,
				computedPropertyMapping65
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginatorOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorOrgAddressType, value62);
			Column[] dependOn122 = new Column[]
			{
				propertyColumn85,
				propertyColumn72
			};
			PropertyColumn column71 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorOrgEmailAddr, rowPropBagCreator, dependOn122);
			ComputedPropertyMapping value63 = new FullComputedPropertyMapping(PropTag.Message.OriginatorOrgEmailAddr, column71, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginatorOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.LastModifierOrgEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping79,
				computedPropertyMapping66
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginatorOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorOrgEmailAddr, value63);
			Column[] dependOn123 = new Column[]
			{
				propertyColumn85,
				propertyColumn73
			};
			PropertyColumn column72 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorSid, rowPropBagCreator, dependOn123);
			ComputedPropertyMapping value64 = new FullComputedPropertyMapping(PropTag.Message.OriginatorSid, column72, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginatorSid), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.LastModifierSid
			}, new PropertyMapping[]
			{
				computedPropertyMapping79,
				computedPropertyMapping67
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginatorSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorSid, value64);
			Column[] dependOn124 = new Column[]
			{
				propertyColumn85,
				propertyColumn74
			};
			PropertyColumn column73 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginatorGuid, rowPropBagCreator, dependOn124);
			ComputedPropertyMapping value65 = new FullComputedPropertyMapping(PropTag.Message.OriginatorGuid, column73, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginatorGuid), new StorePropTag[]
			{
				PropTag.Message.OriginatorFlags,
				PropTag.Message.LastModifierGuid
			}, new PropertyMapping[]
			{
				computedPropertyMapping79,
				computedPropertyMapping68
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginatorGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginatorGuid, value65);
			Column[] dependOn125 = new Column[]
			{
				propertyColumn65
			};
			PropertyColumn propertyColumn90 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorFlags, rowPropBagCreator, dependOn125);
			ComputedPropertyMapping computedPropertyMapping84 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorFlags, propertyColumn90, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalAuthorFlags), new StorePropTag[]
			{
				PropTag.Message.LastModifierFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping59
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalAuthorFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorFlags, computedPropertyMapping84);
			Column[] dependOn126 = new Column[]
			{
				propertyColumn90,
				propertyColumn66
			};
			PropertyColumn propertyColumn91 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorEntryId, rowPropBagCreator, dependOn126);
			ComputedPropertyMapping computedPropertyMapping85 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorEntryId, propertyColumn91, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalAuthorEntryId), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.LastModifierEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping84,
				computedPropertyMapping60
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalAuthorEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorEntryId, computedPropertyMapping85);
			Column[] dependOn127 = new Column[]
			{
				propertyColumn90,
				propertyColumn91,
				propertyColumn67
			};
			PropertyColumn propertyColumn92 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorAddressType, rowPropBagCreator, dependOn127);
			ComputedPropertyMapping computedPropertyMapping86 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorAddressType, propertyColumn92, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalAuthorAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.OriginalAuthorEntryId,
				PropTag.Message.LastModifierAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping84,
				computedPropertyMapping85,
				computedPropertyMapping61
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalAuthorAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorAddressType, computedPropertyMapping86);
			Column[] dependOn128 = new Column[]
			{
				propertyColumn90,
				propertyColumn91,
				propertyColumn68
			};
			PropertyColumn propertyColumn93 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorEmailAddress, rowPropBagCreator, dependOn128);
			ComputedPropertyMapping computedPropertyMapping87 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorEmailAddress, propertyColumn93, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalAuthorEmailAddress), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.OriginalAuthorEntryId,
				PropTag.Message.LastModifierEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping84,
				computedPropertyMapping85,
				computedPropertyMapping62
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalAuthorEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorEmailAddress, computedPropertyMapping87);
			Column[] dependOn129 = new Column[]
			{
				propertyColumn90,
				propertyColumn92,
				propertyColumn93,
				propertyColumn91
			};
			PropertyColumn column74 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorSearchKey, rowPropBagCreator, dependOn129);
			ComputedPropertyMapping value66 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorSearchKey, column74, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalAuthorSearchKey), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.OriginalAuthorAddressType,
				PropTag.Message.OriginalAuthorEmailAddress,
				PropTag.Message.OriginalAuthorEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping84,
				computedPropertyMapping86,
				computedPropertyMapping87,
				computedPropertyMapping85
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalAuthorSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorSearchKey, value66);
			Column[] dependOn130 = new Column[]
			{
				propertyColumn90,
				propertyColumn91,
				propertyColumn93,
				propertyColumn69
			};
			PropertyColumn propertyColumn94 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorSimpleDispName, rowPropBagCreator, dependOn130);
			ComputedPropertyMapping computedPropertyMapping88 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorSimpleDispName, propertyColumn94, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalAuthorSimpleDispName), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.OriginalAuthorEntryId,
				PropTag.Message.OriginalAuthorEmailAddress,
				PropTag.Message.LastModifierSimpleDisplayName
			}, new PropertyMapping[]
			{
				computedPropertyMapping84,
				computedPropertyMapping85,
				computedPropertyMapping87,
				computedPropertyMapping63
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalAuthorSimpleDispName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorSimpleDispName, computedPropertyMapping88);
			Column[] dependOn131 = new Column[]
			{
				propertyColumn90,
				propertyColumn91,
				propertyColumn94,
				propertyColumn93,
				propertyColumn70
			};
			PropertyColumn column75 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorName, rowPropBagCreator, dependOn131);
			ComputedPropertyMapping value67 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorName, column75, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalAuthorName), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.OriginalAuthorEntryId,
				PropTag.Message.OriginalAuthorSimpleDispName,
				PropTag.Message.OriginalAuthorEmailAddress,
				PropTag.Message.LastModifierName
			}, new PropertyMapping[]
			{
				computedPropertyMapping84,
				computedPropertyMapping85,
				computedPropertyMapping88,
				computedPropertyMapping87,
				computedPropertyMapping64
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalAuthorName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorName, value67);
			Column[] dependOn132 = new Column[]
			{
				propertyColumn90,
				propertyColumn71
			};
			PropertyColumn column76 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorOrgAddressType, rowPropBagCreator, dependOn132);
			ComputedPropertyMapping value68 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorOrgAddressType, column76, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalAuthorOrgAddressType), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.LastModifierOrgAddressType
			}, new PropertyMapping[]
			{
				computedPropertyMapping84,
				computedPropertyMapping65
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalAuthorOrgAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorOrgAddressType, value68);
			Column[] dependOn133 = new Column[]
			{
				propertyColumn90,
				propertyColumn72
			};
			PropertyColumn column77 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorOrgEmailAddr, rowPropBagCreator, dependOn133);
			ComputedPropertyMapping value69 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorOrgEmailAddr, column77, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalAuthorOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.LastModifierOrgEmailAddr
			}, new PropertyMapping[]
			{
				computedPropertyMapping84,
				computedPropertyMapping66
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalAuthorOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorOrgEmailAddr, value69);
			Column[] dependOn134 = new Column[]
			{
				propertyColumn90,
				propertyColumn73
			};
			PropertyColumn column78 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorSid, rowPropBagCreator, dependOn134);
			ComputedPropertyMapping value70 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorSid, column78, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalAuthorSid), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.LastModifierSid
			}, new PropertyMapping[]
			{
				computedPropertyMapping84,
				computedPropertyMapping67
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalAuthorSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorSid, value70);
			Column[] dependOn135 = new Column[]
			{
				propertyColumn90,
				propertyColumn74
			};
			PropertyColumn column79 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.OriginalAuthorGuid, rowPropBagCreator, dependOn135);
			ComputedPropertyMapping value71 = new FullComputedPropertyMapping(PropTag.Message.OriginalAuthorGuid, column79, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageOriginalAuthorGuid), new StorePropTag[]
			{
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.LastModifierGuid
			}, new PropertyMapping[]
			{
				computedPropertyMapping84,
				computedPropertyMapping68
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageOriginalAuthorGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.OriginalAuthorGuid, value71);
			PropertyColumn propertyColumn95 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationFlags, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping89 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationFlags, propertyColumn95, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportDestinationFlags), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportDestinationFlags), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationFlags, computedPropertyMapping89);
			Column[] dependOn136 = new Column[]
			{
				propertyColumn95
			};
			PropertyColumn propertyColumn96 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationEntryId, rowPropBagCreator, dependOn136);
			ComputedPropertyMapping computedPropertyMapping90 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationEntryId, propertyColumn96, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportDestinationEntryId), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping89
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportDestinationEntryId), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationEntryId, computedPropertyMapping90);
			Column[] dependOn137 = new Column[]
			{
				propertyColumn95,
				propertyColumn96
			};
			PropertyColumn propertyColumn97 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationAddressType, rowPropBagCreator, dependOn137);
			ComputedPropertyMapping computedPropertyMapping91 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationAddressType, propertyColumn97, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportDestinationAddressType), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags,
				PropTag.Message.ReportDestinationEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping89,
				computedPropertyMapping90
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportDestinationAddressType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationAddressType, computedPropertyMapping91);
			Column[] dependOn138 = new Column[]
			{
				propertyColumn95,
				propertyColumn96
			};
			PropertyColumn propertyColumn98 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationEmailAddress, rowPropBagCreator, dependOn138);
			ComputedPropertyMapping computedPropertyMapping92 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationEmailAddress, propertyColumn98, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportDestinationEmailAddress), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags,
				PropTag.Message.ReportDestinationEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping89,
				computedPropertyMapping90
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportDestinationEmailAddress), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationEmailAddress, computedPropertyMapping92);
			Column[] dependOn139 = new Column[]
			{
				propertyColumn95,
				propertyColumn97,
				propertyColumn98,
				propertyColumn96
			};
			PropertyColumn column80 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationSearchKey, rowPropBagCreator, dependOn139);
			ComputedPropertyMapping value72 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationSearchKey, column80, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportDestinationSearchKey), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags,
				PropTag.Message.ReportDestinationAddressType,
				PropTag.Message.ReportDestinationEmailAddress,
				PropTag.Message.ReportDestinationEntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping89,
				computedPropertyMapping91,
				computedPropertyMapping92,
				computedPropertyMapping90
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportDestinationSearchKey), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationSearchKey, value72);
			Column[] dependOn140 = new Column[]
			{
				propertyColumn95,
				propertyColumn96,
				propertyColumn98
			};
			PropertyColumn propertyColumn99 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationSimpleDisplayName, rowPropBagCreator, dependOn140);
			ComputedPropertyMapping computedPropertyMapping93 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationSimpleDisplayName, propertyColumn99, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportDestinationSimpleDisplayName), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags,
				PropTag.Message.ReportDestinationEntryId,
				PropTag.Message.ReportDestinationEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping89,
				computedPropertyMapping90,
				computedPropertyMapping92
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportDestinationSimpleDisplayName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationSimpleDisplayName, computedPropertyMapping93);
			Column[] dependOn141 = new Column[]
			{
				propertyColumn95,
				propertyColumn96,
				propertyColumn99,
				propertyColumn98
			};
			PropertyColumn column81 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationName, rowPropBagCreator, dependOn141);
			ComputedPropertyMapping value73 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationName, column81, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportDestinationName), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags,
				PropTag.Message.ReportDestinationEntryId,
				PropTag.Message.ReportDestinationSimpleDisplayName,
				PropTag.Message.ReportDestinationEmailAddress
			}, new PropertyMapping[]
			{
				computedPropertyMapping89,
				computedPropertyMapping90,
				computedPropertyMapping93,
				computedPropertyMapping92
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportDestinationName), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationName, value73);
			Column[] dependOn142 = new Column[]
			{
				propertyColumn95
			};
			PropertyColumn column82 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationOrgEmailType, rowPropBagCreator, dependOn142);
			ComputedPropertyMapping value74 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationOrgEmailType, column82, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportDestinationOrgEmailType), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping89
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportDestinationOrgEmailType), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationOrgEmailType, value74);
			Column[] dependOn143 = new Column[]
			{
				propertyColumn95
			};
			PropertyColumn column83 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationOrgEmailAddr, rowPropBagCreator, dependOn143);
			ComputedPropertyMapping value75 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationOrgEmailAddr, column83, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportDestinationOrgEmailAddr), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping89
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportDestinationOrgEmailAddr), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationOrgEmailAddr, value75);
			Column[] dependOn144 = new Column[]
			{
				propertyColumn95
			};
			PropertyColumn column84 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationSid, rowPropBagCreator, dependOn144);
			ComputedPropertyMapping value76 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationSid, column84, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportDestinationSid), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping89
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportDestinationSid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationSid, value76);
			Column[] dependOn145 = new Column[]
			{
				propertyColumn95
			};
			PropertyColumn column85 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Message.ReportDestinationGuid, rowPropBagCreator, dependOn145);
			ComputedPropertyMapping value77 = new FullComputedPropertyMapping(PropTag.Message.ReportDestinationGuid, column85, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetEmbeddedMessageReportDestinationGuid), new StorePropTag[]
			{
				PropTag.Message.ReportDestinationFlags
			}, new PropertyMapping[]
			{
				computedPropertyMapping89
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetEmbeddedMessageReportDestinationGuid), null, null, true, true, true);
			dictionary.Add(PropTag.Message.ReportDestinationGuid, value77);
			objectPropertySchema.Initialize(ObjectType.EmbeddedMessage, table, dictionary, rowPropBagCreator, baseSchema);
			return objectPropertySchema;
		}

		public static ObjectPropertySchema GenerateFolderPropertySchema(StoreDatabase database)
		{
			Dictionary<StorePropTag, PropertyMapping> dictionary = new Dictionary<StorePropTag, PropertyMapping>(200);
			FolderTable folderTable = DatabaseSchema.FolderTable(database);
			if (folderTable == null)
			{
				return null;
			}
			Table table = folderTable.Table;
			ObjectPropertySchema objectPropertySchema = new ObjectPropertySchema();
			ObjectPropertySchema baseSchema = null;
			Func<IRowAccess, IRowPropertyBag> rowPropBagCreator = (IRowAccess rowAccess) => new RowPropertyBag(table, objectPropertySchema, PropTag.Folder.MailboxNum, rowAccess);
			Column column;
			PropertyMapping propertyMapping;
			if (folderTable.MailboxPartitionNumber != null)
			{
				column = Factory.CreateMappedPropertyColumn(folderTable.MailboxPartitionNumber, PropTag.Folder.MailboxPartitionNumber);
				propertyMapping = new PhysicalColumnPropertyMapping(PropTag.Folder.MailboxPartitionNumber, column, null, null, null, (PhysicalColumn)column.ActualColumn, false, true, true, false, false);
			}
			else
			{
				column = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.MailboxPartitionNumber, null), PropTag.Folder.MailboxPartitionNumber);
				propertyMapping = new ConstantPropertyMapping(PropTag.Folder.MailboxPartitionNumber, column, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Folder.MailboxPartitionNumber, propertyMapping);
			Column column2;
			PropertyMapping propertyMapping2;
			if (folderTable.MailboxNumber != null)
			{
				column2 = Factory.CreateMappedPropertyColumn(folderTable.MailboxNumber, PropTag.Folder.MailboxNumberInternal);
				propertyMapping2 = new PhysicalColumnPropertyMapping(PropTag.Folder.MailboxNumberInternal, column2, null, null, null, (PhysicalColumn)column2.ActualColumn, false, true, true, false, false);
			}
			else
			{
				column2 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.MailboxNumberInternal, null), PropTag.Folder.MailboxNumberInternal);
				propertyMapping2 = new ConstantPropertyMapping(PropTag.Folder.MailboxNumberInternal, column2, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Folder.MailboxNumberInternal, propertyMapping2);
			Column column3 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructFunctionColumn(PropTag.Folder.MailboxNum, typeof(int), 4, 0, folderTable.Table, new Func<object[], object>(PropertySchemaPopulation.ComputeMailboxNumber), "Exchange.ComputeMailboxNumber", new Column[]
			{
				column,
				column2
			}), PropTag.Folder.MailboxNum);
			FunctionPropertyMapping functionPropertyMapping = new FunctionPropertyMapping(PropTag.Folder.MailboxNum, column3, null, new Func<object[], object>(PropertySchemaPopulation.ComputeMailboxNumber), new PropertyMapping[]
			{
				propertyMapping,
				propertyMapping2
			}, true, true, false);
			dictionary.Add(PropTag.Folder.MailboxNum, functionPropertyMapping);
			Column column4;
			PropertyMapping propertyMapping3;
			if (folderTable.FolderId != null)
			{
				column4 = Factory.CreateMappedPropertyColumn(folderTable.FolderId, PropTag.Folder.FidBin);
				propertyMapping3 = new PhysicalColumnPropertyMapping(PropTag.Folder.FidBin, column4, null, null, null, (PhysicalColumn)column4.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column4 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.FidBin, null), PropTag.Folder.FidBin);
				propertyMapping3 = new ConstantPropertyMapping(PropTag.Folder.FidBin, column4, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Folder.FidBin, propertyMapping3);
			Column column5 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.Fid, typeof(long), 8, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), "Exchange.ConvertExchangeIdToInt64", column4), PropTag.Folder.Fid);
			ConversionPropertyMapping conversionPropertyMapping = new ConversionPropertyMapping(PropTag.Folder.Fid, column5, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), PropTag.Folder.FidBin, propertyMapping3, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.Fid, conversionPropertyMapping);
			Column column6;
			PropertyMapping propertyMapping4;
			if (folderTable.ParentFolderId != null)
			{
				column6 = Factory.CreateMappedPropertyColumn(folderTable.ParentFolderId, PropTag.Folder.ParentFidBin);
				propertyMapping4 = new PhysicalColumnPropertyMapping(PropTag.Folder.ParentFidBin, column6, null, null, null, (PhysicalColumn)column6.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column6 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.ParentFidBin, null), PropTag.Folder.ParentFidBin);
				propertyMapping4 = new ConstantPropertyMapping(PropTag.Folder.ParentFidBin, column6, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Folder.ParentFidBin, propertyMapping4);
			Column column7 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.ParentFid, typeof(long), 8, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), "Exchange.ConvertExchangeIdToInt64", column6), PropTag.Folder.ParentFid);
			ConversionPropertyMapping value = new ConversionPropertyMapping(PropTag.Folder.ParentFid, column7, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), PropTag.Folder.ParentFidBin, propertyMapping4, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.ParentFid, value);
			Column column8 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.EntryId, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), "Exchange.ConvertExchangeIdToFolderSvrEid", column4), PropTag.Folder.EntryId);
			ConversionPropertyMapping value2 = new ConversionPropertyMapping(PropTag.Folder.EntryId, column8, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), PropTag.Folder.FidBin, propertyMapping3, null, null, null, false, true, true);
			dictionary.Add(PropTag.Folder.EntryId, value2);
			Column column9 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.EntryIdSvrEid, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), "Exchange.ConvertExchangeIdToFolderSvrEid", column4), PropTag.Folder.EntryIdSvrEid);
			ConversionPropertyMapping value3 = new ConversionPropertyMapping(PropTag.Folder.EntryIdSvrEid, column9, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), PropTag.Folder.FidBin, propertyMapping3, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.EntryIdSvrEid, value3);
			Column column10 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.ParentEntryId, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), "Exchange.ConvertExchangeIdToFolderSvrEid", column6), PropTag.Folder.ParentEntryId);
			ConversionPropertyMapping value4 = new ConversionPropertyMapping(PropTag.Folder.ParentEntryId, column10, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), PropTag.Folder.ParentFidBin, propertyMapping4, null, null, null, false, true, true);
			dictionary.Add(PropTag.Folder.ParentEntryId, value4);
			Column column11 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.ParentEntryIdSvrEid, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), "Exchange.ConvertExchangeIdToFolderSvrEid", column6), PropTag.Folder.ParentEntryIdSvrEid);
			ConversionPropertyMapping value5 = new ConversionPropertyMapping(PropTag.Folder.ParentEntryIdSvrEid, column11, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), PropTag.Folder.ParentFidBin, propertyMapping4, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.ParentEntryIdSvrEid, value5);
			Column column12 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.RecordKeySvrEid, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), "Exchange.ConvertExchangeIdToFolderSvrEid", column4), PropTag.Folder.RecordKeySvrEid);
			ConversionPropertyMapping value6 = new ConversionPropertyMapping(PropTag.Folder.RecordKeySvrEid, column12, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), PropTag.Folder.FidBin, propertyMapping3, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.RecordKeySvrEid, value6);
			Column column13 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.RecordKey, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), "Exchange.ConvertExchangeIdToFolderSvrEid", column4), PropTag.Folder.RecordKey);
			ConversionPropertyMapping value7 = new ConversionPropertyMapping(PropTag.Folder.RecordKey, column13, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), PropTag.Folder.FidBin, propertyMapping3, null, null, null, false, true, true);
			dictionary.Add(PropTag.Folder.RecordKey, value7);
			Column column14 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.InstanceKey, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), "Exchange.ConvertExchangeIdToFolderSvrEid", column4), PropTag.Folder.InstanceKey);
			ConversionPropertyMapping value8 = new ConversionPropertyMapping(PropTag.Folder.InstanceKey, column14, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), PropTag.Folder.FidBin, propertyMapping3, null, null, null, false, true, true);
			dictionary.Add(PropTag.Folder.InstanceKey, value8);
			Column column15 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.InstanceKeySvrEid, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), "Exchange.ConvertExchangeIdToFolderSvrEid", column4), PropTag.Folder.InstanceKeySvrEid);
			ConversionPropertyMapping value9 = new ConversionPropertyMapping(PropTag.Folder.InstanceKeySvrEid, column15, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToFolderSvrEid), PropTag.Folder.FidBin, propertyMapping3, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.InstanceKeySvrEid, value9);
			Column column16;
			PropertyMapping propertyMapping5;
			if (folderTable.SourceKey != null)
			{
				column16 = Factory.CreateMappedPropertyColumn(folderTable.SourceKey, PropTag.Folder.InternalSourceKey);
				propertyMapping5 = new PhysicalColumnPropertyMapping(PropTag.Folder.InternalSourceKey, column16, null, null, null, (PhysicalColumn)column16.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column16 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.InternalSourceKey, null), PropTag.Folder.InternalSourceKey);
				propertyMapping5 = new ConstantPropertyMapping(PropTag.Folder.InternalSourceKey, column16, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Folder.InternalSourceKey, propertyMapping5);
			Column[] dependOn = new Column[]
			{
				column16,
				column4
			};
			PropertyColumn column17 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.SourceKey, rowPropBagCreator, dependOn);
			ComputedPropertyMapping value10 = new ComputedPropertyMapping(PropTag.Folder.SourceKey, column17, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetFolderSourceKey), new StorePropTag[]
			{
				PropTag.Folder.InternalSourceKey,
				PropTag.Folder.FidBin
			}, new PropertyMapping[]
			{
				propertyMapping5,
				propertyMapping3
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Folder.SourceKey, value10);
			Column column18 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.ParentSourceKey, typeof(byte[]), 22, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo22ByteForm), "Exchange.ConvertExchangeIdTo22ByteForm", column6), PropTag.Folder.ParentSourceKey);
			ConversionPropertyMapping value11 = new ConversionPropertyMapping(PropTag.Folder.ParentSourceKey, column18, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo22ByteForm), PropTag.Folder.ParentFidBin, propertyMapping4, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.ParentSourceKey, value11);
			Column column19;
			PropertyMapping propertyMapping6;
			if (folderTable.LcnCurrent != null)
			{
				column19 = Factory.CreateMappedPropertyColumn(folderTable.LcnCurrent, PropTag.Folder.ChangeNumberBin);
				propertyMapping6 = new PhysicalColumnPropertyMapping(PropTag.Folder.ChangeNumberBin, column19, null, null, null, (PhysicalColumn)column19.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column19 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.ChangeNumberBin, null), PropTag.Folder.ChangeNumberBin);
				propertyMapping6 = new ConstantPropertyMapping(PropTag.Folder.ChangeNumberBin, column19, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Folder.ChangeNumberBin, propertyMapping6);
			Column column20 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.ChangeNumber, typeof(long), 8, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), "Exchange.ConvertExchangeIdToInt64", column19), PropTag.Folder.ChangeNumber);
			ConversionPropertyMapping value12 = new ConversionPropertyMapping(PropTag.Folder.ChangeNumber, column20, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), PropTag.Folder.ChangeNumberBin, propertyMapping6, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.ChangeNumber, value12);
			Column column21;
			PropertyMapping propertyMapping7;
			if (folderTable.ChangeKey != null)
			{
				column21 = Factory.CreateMappedPropertyColumn(folderTable.ChangeKey, PropTag.Folder.InternalChangeKey);
				propertyMapping7 = new PhysicalColumnPropertyMapping(PropTag.Folder.InternalChangeKey, column21, null, null, null, (PhysicalColumn)column21.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column21 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.InternalChangeKey, null), PropTag.Folder.InternalChangeKey);
				propertyMapping7 = new ConstantPropertyMapping(PropTag.Folder.InternalChangeKey, column21, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Folder.InternalChangeKey, propertyMapping7);
			Column[] dependOn2 = new Column[]
			{
				column21,
				column19
			};
			PropertyColumn column22 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.ChangeKey, rowPropBagCreator, dependOn2);
			ComputedPropertyMapping value13 = new ComputedPropertyMapping(PropTag.Folder.ChangeKey, column22, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetFolderChangeKey), new StorePropTag[]
			{
				PropTag.Folder.InternalChangeKey,
				PropTag.Folder.ChangeNumberBin
			}, new PropertyMapping[]
			{
				propertyMapping7,
				propertyMapping6
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Folder.ChangeKey, value13);
			Column column23 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.CnExport, typeof(byte[]), 24, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo24ByteForm), "Exchange.ConvertExchangeIdTo24ByteForm", column19), PropTag.Folder.CnExport);
			ConversionPropertyMapping value14 = new ConversionPropertyMapping(PropTag.Folder.CnExport, column23, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo24ByteForm), PropTag.Folder.ChangeNumberBin, propertyMapping6, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetFolderCnExport), null, null, true, true, true);
			dictionary.Add(PropTag.Folder.CnExport, value14);
			Column column24 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.CnMvExport, typeof(byte[]), 24, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo24ByteForm), "Exchange.ConvertExchangeIdTo24ByteForm", column19), PropTag.Folder.CnMvExport);
			ConversionPropertyMapping value15 = new ConversionPropertyMapping(PropTag.Folder.CnMvExport, column24, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdTo24ByteForm), PropTag.Folder.ChangeNumberBin, propertyMapping6, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetFolderCnMvExport), null, null, true, true, false);
			dictionary.Add(PropTag.Folder.CnMvExport, value15);
			Column column25;
			PropertyMapping propertyMapping8;
			if (folderTable.DisplayName != null)
			{
				column25 = Factory.CreateMappedPropertyColumn(folderTable.DisplayName, PropTag.Folder.DisplayName);
				propertyMapping8 = new PhysicalColumnPropertyMapping(PropTag.Folder.DisplayName, column25, null, null, null, (PhysicalColumn)column25.ActualColumn, true, true, true, true, false);
			}
			else
			{
				column25 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.DisplayName, null), PropTag.Folder.DisplayName);
				propertyMapping8 = new ConstantPropertyMapping(PropTag.Folder.DisplayName, column25, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Folder.DisplayName, propertyMapping8);
			PropertyMapping value16;
			if (folderTable.Comment != null)
			{
				Column column26 = Factory.CreateMappedPropertyColumn(folderTable.Comment, PropTag.Folder.Comment);
				value16 = new PhysicalColumnPropertyMapping(PropTag.Folder.Comment, column26, null, null, null, (PhysicalColumn)column26.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column26 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.Comment, null), PropTag.Folder.Comment);
				value16 = new ConstantPropertyMapping(PropTag.Folder.Comment, column26, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Folder.Comment, value16);
			PropertyMapping value17;
			if (folderTable.ContainerClass != null)
			{
				Column column27 = Factory.CreateMappedPropertyColumn(folderTable.ContainerClass, PropTag.Folder.ContainerClass);
				value17 = new PhysicalColumnPropertyMapping(PropTag.Folder.ContainerClass, column27, null, null, null, (PhysicalColumn)column27.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column27 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.ContainerClass, null), PropTag.Folder.ContainerClass);
				value17 = new ConstantPropertyMapping(PropTag.Folder.ContainerClass, column27, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Folder.ContainerClass, value17);
			PropertyMapping value18;
			if (folderTable.DisplayType != null)
			{
				Column column28 = Factory.CreateMappedPropertyColumn(folderTable.DisplayType, PropTag.Folder.DisplayType);
				value18 = new PhysicalColumnPropertyMapping(PropTag.Folder.DisplayType, column28, null, null, null, (PhysicalColumn)column28.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column28 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.DisplayType, null), PropTag.Folder.DisplayType);
				value18 = new ConstantPropertyMapping(PropTag.Folder.DisplayType, column28, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Folder.DisplayType, value18);
			Column column29;
			PropertyMapping propertyMapping9;
			if (folderTable.QueryCriteria != null)
			{
				column29 = Factory.CreateMappedPropertyColumn(folderTable.QueryCriteria, PropTag.Folder.QueryCriteriaInternal);
				propertyMapping9 = new PhysicalColumnPropertyMapping(PropTag.Folder.QueryCriteriaInternal, column29, null, null, null, (PhysicalColumn)column29.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column29 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.QueryCriteriaInternal, null), PropTag.Folder.QueryCriteriaInternal);
				propertyMapping9 = new ConstantPropertyMapping(PropTag.Folder.QueryCriteriaInternal, column29, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Folder.QueryCriteriaInternal, propertyMapping9);
			Column[] dependOn3 = new Column[]
			{
				column6,
				column29
			};
			PropertyColumn propertyColumn = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.FolderType, rowPropBagCreator, dependOn3);
			ComputedPropertyMapping computedPropertyMapping = new ComputedPropertyMapping(PropTag.Folder.FolderType, propertyColumn, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetFolderFolderType), new StorePropTag[]
			{
				PropTag.Folder.ParentFidBin,
				PropTag.Folder.QueryCriteriaInternal
			}, new PropertyMapping[]
			{
				propertyMapping4,
				propertyMapping9
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Folder.FolderType, computedPropertyMapping);
			PropertyColumn propertyColumn2 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.IPMFolder, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping = new DefaultPropertyMapping(PropTag.Folder.IPMFolder, propertyColumn2, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Folder.IPMFolder, defaultPropertyMapping);
			PropertyColumn propertyColumn3 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.HasRules, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping2 = new DefaultPropertyMapping(PropTag.Folder.HasRules, propertyColumn3, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Folder.HasRules, defaultPropertyMapping2);
			Column[] dependOn4 = new Column[]
			{
				propertyColumn2,
				propertyColumn,
				propertyColumn3
			};
			PropertyColumn column30 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.FolderFlags, rowPropBagCreator, dependOn4);
			ComputedPropertyMapping value19 = new ComputedPropertyMapping(PropTag.Folder.FolderFlags, column30, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetFolderFolderFlags), new StorePropTag[]
			{
				PropTag.Folder.IPMFolder,
				PropTag.Folder.FolderType,
				PropTag.Folder.HasRules
			}, new PropertyMapping[]
			{
				defaultPropertyMapping,
				computedPropertyMapping,
				defaultPropertyMapping2
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Folder.FolderFlags, value19);
			PropertyMapping value20;
			if (folderTable.CreatorSid != null)
			{
				Column column31 = Factory.CreateMappedPropertyColumn(folderTable.CreatorSid, PropTag.Folder.CreatorSID);
				value20 = new PhysicalColumnPropertyMapping(PropTag.Folder.CreatorSID, column31, null, null, null, (PhysicalColumn)column31.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column31 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.CreatorSID, null), PropTag.Folder.CreatorSID);
				value20 = new ConstantPropertyMapping(PropTag.Folder.CreatorSID, column31, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Folder.CreatorSID, value20);
			PropertyMapping value21;
			if (folderTable.CreationTime != null)
			{
				Column column32 = Factory.CreateMappedPropertyColumn(folderTable.CreationTime, PropTag.Folder.CreationTime);
				value21 = new PhysicalColumnPropertyMapping(PropTag.Folder.CreationTime, column32, null, null, null, (PhysicalColumn)column32.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column32 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.CreationTime, null), PropTag.Folder.CreationTime);
				value21 = new ConstantPropertyMapping(PropTag.Folder.CreationTime, column32, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Folder.CreationTime, value21);
			PropertyColumn column33 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.DisablePerUserRead, rowPropBagCreator, null);
			DefaultPropertyMapping value22 = new DefaultPropertyMapping(PropTag.Folder.DisablePerUserRead, column33, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetFolderDisablePerUserRead), null, null, true, true, true, false);
			dictionary.Add(PropTag.Folder.DisablePerUserRead, value22);
			PropertyMapping value23;
			if (folderTable.LastModificationTime != null)
			{
				Column column34 = Factory.CreateMappedPropertyColumn(folderTable.LastModificationTime, PropTag.Folder.LastModificationTime);
				value23 = new PhysicalColumnPropertyMapping(PropTag.Folder.LastModificationTime, column34, null, null, null, (PhysicalColumn)column34.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column34 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.LastModificationTime, null), PropTag.Folder.LastModificationTime);
				value23 = new ConstantPropertyMapping(PropTag.Folder.LastModificationTime, column34, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Folder.LastModificationTime, value23);
			PropertyMapping value24;
			if (folderTable.LocalCommitTimeMax != null)
			{
				Column column35 = Factory.CreateMappedPropertyColumn(folderTable.LocalCommitTimeMax, PropTag.Folder.LocalCommitTimeMax);
				value24 = new PhysicalColumnPropertyMapping(PropTag.Folder.LocalCommitTimeMax, column35, null, null, null, (PhysicalColumn)column35.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column35 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.LocalCommitTimeMax, null), PropTag.Folder.LocalCommitTimeMax);
				value24 = new ConstantPropertyMapping(PropTag.Folder.LocalCommitTimeMax, column35, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Folder.LocalCommitTimeMax, value24);
			Column column36;
			PropertyMapping propertyMapping10;
			if (folderTable.VersionHistory != null)
			{
				column36 = Factory.CreateMappedPropertyColumn(folderTable.VersionHistory, PropTag.Folder.PCL);
				propertyMapping10 = new PhysicalColumnPropertyMapping(PropTag.Folder.PCL, column36, null, null, null, (PhysicalColumn)column36.ActualColumn, true, false, true, true, false);
			}
			else
			{
				column36 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.PCL, null), PropTag.Folder.PCL);
				propertyMapping10 = new ConstantPropertyMapping(PropTag.Folder.PCL, column36, null, null, false, true, true);
			}
			dictionary.Add(PropTag.Folder.PCL, propertyMapping10);
			PropertyMapping value25;
			if (folderTable.VersionHistory != null)
			{
				Column column37 = Factory.CreateMappedPropertyColumn(folderTable.VersionHistory, PropTag.Folder.PredecessorChangeList);
				value25 = new PhysicalColumnPropertyMapping(PropTag.Folder.PredecessorChangeList, column37, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetFolderPredecessorChangeList), null, null, (PhysicalColumn)column37.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column37 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.PredecessorChangeList, null), PropTag.Folder.PredecessorChangeList);
				value25 = new ConstantPropertyMapping(PropTag.Folder.PredecessorChangeList, column37, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Folder.PredecessorChangeList, value25);
			Column column38 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.PclExport, typeof(byte[]), 0, 1073741823, table, new Func<object, object>(PropertySchemaPopulation.ConvertLXCNArrayToLTIDArray), "Exchange.ConvertLXCNArrayToLTIDArray", column36), PropTag.Folder.PclExport);
			ConversionPropertyMapping value26 = new ConversionPropertyMapping(PropTag.Folder.PclExport, column38, new Func<object, object>(PropertySchemaPopulation.ConvertLXCNArrayToLTIDArray), PropTag.Folder.PCL, propertyMapping10, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetFolderPclExport), null, null, true, true, true);
			dictionary.Add(PropTag.Folder.PclExport, value26);
			Column column39 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.DeletedOn, null), PropTag.Folder.DeletedOn);
			ConstantPropertyMapping value27 = new ConstantPropertyMapping(PropTag.Folder.DeletedOn, column39, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.DeletedOn, value27);
			PropertyColumn column40 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.HasNamedProperties, rowPropBagCreator, null);
			ComputedPropertyMapping value28 = new ComputedPropertyMapping(PropTag.Folder.HasNamedProperties, column40, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetFolderHasNamedProperties), null, null, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Folder.HasNamedProperties, value28);
			PropertyMapping value29;
			if (folderTable.AclTableAndSecurityDescriptor != null)
			{
				Column column41 = Factory.CreateMappedPropertyColumn(folderTable.AclTableAndSecurityDescriptor, PropTag.Folder.AclTableAndSecurityDescriptor);
				value29 = new PhysicalColumnPropertyMapping(PropTag.Folder.AclTableAndSecurityDescriptor, column41, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetFolderAclTableAndSecurityDescriptor), null, null, (PhysicalColumn)column41.ActualColumn, true, true, true, false, true);
			}
			else
			{
				Column column41 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.AclTableAndSecurityDescriptor, null), PropTag.Folder.AclTableAndSecurityDescriptor);
				value29 = new ConstantPropertyMapping(PropTag.Folder.AclTableAndSecurityDescriptor, column41, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Folder.AclTableAndSecurityDescriptor, value29);
			PropertyColumn column42 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.NTSecurityDescriptor, rowPropBagCreator, null);
			ComputedPropertyMapping value30 = new ComputedPropertyMapping(PropTag.Folder.NTSecurityDescriptor, column42, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetFolderNTSecurityDescriptor), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetFolderNTSecurityDescriptor), null, null, false, true, true, true);
			dictionary.Add(PropTag.Folder.NTSecurityDescriptor, value30);
			PropertyColumn column43 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.FreeBusyNTSD, rowPropBagCreator, null);
			ComputedPropertyMapping value31 = new ComputedPropertyMapping(PropTag.Folder.FreeBusyNTSD, column43, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetFolderFreeBusyNTSD), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetFolderFreeBusyNTSD), null, null, false, true, true, true);
			dictionary.Add(PropTag.Folder.FreeBusyNTSD, value31);
			Column column44 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.Access, 63), PropTag.Folder.Access);
			ConstantPropertyMapping value32 = new ConstantPropertyMapping(PropTag.Folder.Access, column44, null, 63, true, true, true);
			dictionary.Add(PropTag.Folder.Access, value32);
			PropertyColumn column45 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.SecureOrigination, rowPropBagCreator, null);
			DefaultPropertyMapping value33 = new DefaultPropertyMapping(PropTag.Folder.SecureOrigination, column45, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Folder.SecureOrigination, value33);
			PropertyColumn propertyColumn4 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.ModeratorRuleCount, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping3 = new DefaultPropertyMapping(PropTag.Folder.ModeratorRuleCount, propertyColumn4, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Folder.ModeratorRuleCount, defaultPropertyMapping3);
			Column column46 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.HasModeratorRules, typeof(bool), 1, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt32ToBooleanNotZero), "Exchange.ConvertInt32ToBooleanNotZero", propertyColumn4), PropTag.Folder.HasModeratorRules);
			ConversionPropertyMapping value34 = new ConversionPropertyMapping(PropTag.Folder.HasModeratorRules, column46, new Func<object, object>(PropertySchemaPopulation.ConvertInt32ToBooleanNotZero), PropTag.Folder.ModeratorRuleCount, defaultPropertyMapping3, null, null, null, false, true, true);
			dictionary.Add(PropTag.Folder.HasModeratorRules, value34);
			Column column47;
			PropertyMapping propertyMapping11;
			if (folderTable.MessageCount != null)
			{
				column47 = Factory.CreateMappedPropertyColumn(folderTable.MessageCount, PropTag.Folder.ContentCountInt64);
				propertyMapping11 = new PhysicalColumnPropertyMapping(PropTag.Folder.ContentCountInt64, column47, null, null, null, (PhysicalColumn)column47.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column47 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.ContentCountInt64, null), PropTag.Folder.ContentCountInt64);
				propertyMapping11 = new ConstantPropertyMapping(PropTag.Folder.ContentCountInt64, column47, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Folder.ContentCountInt64, propertyMapping11);
			Column column48 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.ContentCount, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column47), PropTag.Folder.ContentCount);
			ConversionPropertyMapping value35 = new ConversionPropertyMapping(PropTag.Folder.ContentCount, column48, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Folder.ContentCountInt64, propertyMapping11, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.ContentCount, value35);
			PropertyMapping value36;
			if (folderTable.UnreadMessageCount != null)
			{
				Column column49 = Factory.CreateMappedPropertyColumn(folderTable.UnreadMessageCount, PropTag.Folder.UnreadCountInt64);
				value36 = new PhysicalColumnPropertyMapping(PropTag.Folder.UnreadCountInt64, column49, null, null, null, (PhysicalColumn)column49.ActualColumn, true, false, true, false, false);
			}
			else
			{
				Column column49 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.UnreadCountInt64, null), PropTag.Folder.UnreadCountInt64);
				value36 = new ConstantPropertyMapping(PropTag.Folder.UnreadCountInt64, column49, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Folder.UnreadCountInt64, value36);
			Column column50;
			PropertyMapping propertyMapping12;
			if (folderTable.VirtualUnreadMessageCount != null)
			{
				column50 = Factory.CreateMappedPropertyColumn(folderTable.VirtualUnreadMessageCount, PropTag.Folder.VirtualUnreadMessageCount);
				propertyMapping12 = new PhysicalColumnPropertyMapping(PropTag.Folder.VirtualUnreadMessageCount, column50, null, null, null, (PhysicalColumn)column50.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column50 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.VirtualUnreadMessageCount, null), PropTag.Folder.VirtualUnreadMessageCount);
				propertyMapping12 = new ConstantPropertyMapping(PropTag.Folder.VirtualUnreadMessageCount, column50, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Folder.VirtualUnreadMessageCount, propertyMapping12);
			Column column51 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.UnreadCount, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column50), PropTag.Folder.UnreadCount);
			ConversionPropertyMapping value37 = new ConversionPropertyMapping(PropTag.Folder.UnreadCount, column51, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Folder.VirtualUnreadMessageCount, propertyMapping12, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.UnreadCount, value37);
			Column column52;
			PropertyMapping propertyMapping13;
			if (folderTable.HiddenItemCount != null)
			{
				column52 = Factory.CreateMappedPropertyColumn(folderTable.HiddenItemCount, PropTag.Folder.AssociatedContentCountInt64);
				propertyMapping13 = new PhysicalColumnPropertyMapping(PropTag.Folder.AssociatedContentCountInt64, column52, null, null, null, (PhysicalColumn)column52.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column52 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.AssociatedContentCountInt64, null), PropTag.Folder.AssociatedContentCountInt64);
				propertyMapping13 = new ConstantPropertyMapping(PropTag.Folder.AssociatedContentCountInt64, column52, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Folder.AssociatedContentCountInt64, propertyMapping13);
			Column column53 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.AssociatedContentCount, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column52), PropTag.Folder.AssociatedContentCount);
			ConversionPropertyMapping value38 = new ConversionPropertyMapping(PropTag.Folder.AssociatedContentCount, column53, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Folder.AssociatedContentCountInt64, propertyMapping13, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.AssociatedContentCount, value38);
			Column column54;
			PropertyMapping propertyMapping14;
			if (folderTable.TotalDeletedCount != null)
			{
				column54 = Factory.CreateMappedPropertyColumn(folderTable.TotalDeletedCount, PropTag.Folder.DeletedCountTotalInt64);
				propertyMapping14 = new PhysicalColumnPropertyMapping(PropTag.Folder.DeletedCountTotalInt64, column54, null, null, null, (PhysicalColumn)column54.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column54 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.DeletedCountTotalInt64, null), PropTag.Folder.DeletedCountTotalInt64);
				propertyMapping14 = new ConstantPropertyMapping(PropTag.Folder.DeletedCountTotalInt64, column54, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Folder.DeletedCountTotalInt64, propertyMapping14);
			Column column55 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.DeletedCountTotal, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column54), PropTag.Folder.DeletedCountTotal);
			ConversionPropertyMapping value39 = new ConversionPropertyMapping(PropTag.Folder.DeletedCountTotal, column55, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Folder.DeletedCountTotalInt64, propertyMapping14, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.DeletedCountTotal, value39);
			Column column56;
			PropertyMapping propertyMapping15;
			if (folderTable.MessageHasAttachCount != null)
			{
				column56 = Factory.CreateMappedPropertyColumn(folderTable.MessageHasAttachCount, PropTag.Folder.NormalMsgWithAttachCountInt64);
				propertyMapping15 = new PhysicalColumnPropertyMapping(PropTag.Folder.NormalMsgWithAttachCountInt64, column56, null, null, null, (PhysicalColumn)column56.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column56 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.NormalMsgWithAttachCountInt64, null), PropTag.Folder.NormalMsgWithAttachCountInt64);
				propertyMapping15 = new ConstantPropertyMapping(PropTag.Folder.NormalMsgWithAttachCountInt64, column56, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Folder.NormalMsgWithAttachCountInt64, propertyMapping15);
			Column column57 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.NormalMsgWithAttachCount, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column56), PropTag.Folder.NormalMsgWithAttachCount);
			ConversionPropertyMapping value40 = new ConversionPropertyMapping(PropTag.Folder.NormalMsgWithAttachCount, column57, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Folder.NormalMsgWithAttachCountInt64, propertyMapping15, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.NormalMsgWithAttachCount, value40);
			Column column58;
			PropertyMapping propertyMapping16;
			if (folderTable.HiddenItemHasAttachCount != null)
			{
				column58 = Factory.CreateMappedPropertyColumn(folderTable.HiddenItemHasAttachCount, PropTag.Folder.AssocMsgWithAttachCountInt64);
				propertyMapping16 = new PhysicalColumnPropertyMapping(PropTag.Folder.AssocMsgWithAttachCountInt64, column58, null, null, null, (PhysicalColumn)column58.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column58 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.AssocMsgWithAttachCountInt64, null), PropTag.Folder.AssocMsgWithAttachCountInt64);
				propertyMapping16 = new ConstantPropertyMapping(PropTag.Folder.AssocMsgWithAttachCountInt64, column58, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Folder.AssocMsgWithAttachCountInt64, propertyMapping16);
			Column column59 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.AssocMsgWithAttachCount, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column58), PropTag.Folder.AssocMsgWithAttachCount);
			ConversionPropertyMapping value41 = new ConversionPropertyMapping(PropTag.Folder.AssocMsgWithAttachCount, column59, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Folder.AssocMsgWithAttachCountInt64, propertyMapping16, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.AssocMsgWithAttachCount, value41);
			Column column60;
			PropertyMapping propertyMapping17;
			if (folderTable.MessageAttachCount != null)
			{
				column60 = Factory.CreateMappedPropertyColumn(folderTable.MessageAttachCount, PropTag.Folder.AttachOnNormalMsgCtInt64);
				propertyMapping17 = new PhysicalColumnPropertyMapping(PropTag.Folder.AttachOnNormalMsgCtInt64, column60, null, null, null, (PhysicalColumn)column60.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column60 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.AttachOnNormalMsgCtInt64, null), PropTag.Folder.AttachOnNormalMsgCtInt64);
				propertyMapping17 = new ConstantPropertyMapping(PropTag.Folder.AttachOnNormalMsgCtInt64, column60, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Folder.AttachOnNormalMsgCtInt64, propertyMapping17);
			Column column61 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.AttachOnNormalMsgCt, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column60), PropTag.Folder.AttachOnNormalMsgCt);
			ConversionPropertyMapping value42 = new ConversionPropertyMapping(PropTag.Folder.AttachOnNormalMsgCt, column61, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Folder.AttachOnNormalMsgCtInt64, propertyMapping17, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.AttachOnNormalMsgCt, value42);
			Column column62;
			PropertyMapping propertyMapping18;
			if (folderTable.HiddenItemAttachCount != null)
			{
				column62 = Factory.CreateMappedPropertyColumn(folderTable.HiddenItemAttachCount, PropTag.Folder.AttachOnAssocMsgCtInt64);
				propertyMapping18 = new PhysicalColumnPropertyMapping(PropTag.Folder.AttachOnAssocMsgCtInt64, column62, null, null, null, (PhysicalColumn)column62.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column62 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.AttachOnAssocMsgCtInt64, null), PropTag.Folder.AttachOnAssocMsgCtInt64);
				propertyMapping18 = new ConstantPropertyMapping(PropTag.Folder.AttachOnAssocMsgCtInt64, column62, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Folder.AttachOnAssocMsgCtInt64, propertyMapping18);
			Column column63 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.AttachOnAssocMsgCt, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column62), PropTag.Folder.AttachOnAssocMsgCt);
			ConversionPropertyMapping value43 = new ConversionPropertyMapping(PropTag.Folder.AttachOnAssocMsgCt, column63, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Folder.AttachOnAssocMsgCtInt64, propertyMapping18, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.AttachOnAssocMsgCt, value43);
			Column column64;
			PropertyMapping propertyMapping19;
			if (folderTable.FolderCount != null)
			{
				column64 = Factory.CreateMappedPropertyColumn(folderTable.FolderCount, PropTag.Folder.FolderChildCountInt64);
				propertyMapping19 = new PhysicalColumnPropertyMapping(PropTag.Folder.FolderChildCountInt64, column64, null, null, null, (PhysicalColumn)column64.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column64 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.FolderChildCountInt64, null), PropTag.Folder.FolderChildCountInt64);
				propertyMapping19 = new ConstantPropertyMapping(PropTag.Folder.FolderChildCountInt64, column64, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Folder.FolderChildCountInt64, propertyMapping19);
			Column column65 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.FolderChildCount, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column64), PropTag.Folder.FolderChildCount);
			ConversionPropertyMapping value44 = new ConversionPropertyMapping(PropTag.Folder.FolderChildCount, column65, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Folder.FolderChildCountInt64, propertyMapping19, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.FolderChildCount, value44);
			Column column66 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.Subfolders, typeof(bool), 1, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToBooleanNotZero), "Exchange.ConvertInt64ToBooleanNotZero", column64), PropTag.Folder.Subfolders);
			ConversionPropertyMapping value45 = new ConversionPropertyMapping(PropTag.Folder.Subfolders, column66, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToBooleanNotZero), PropTag.Folder.FolderChildCountInt64, propertyMapping19, null, null, null, true, true, true);
			dictionary.Add(PropTag.Folder.Subfolders, value45);
			PropertyColumn column67 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.DeletedFolderCount, rowPropBagCreator, null);
			DefaultPropertyMapping value46 = new DefaultPropertyMapping(PropTag.Folder.DeletedFolderCount, column67, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Folder.DeletedFolderCount, value46);
			Column column68;
			PropertyMapping propertyMapping20;
			if (folderTable.MessageSize != null)
			{
				column68 = Factory.CreateMappedPropertyColumn(folderTable.MessageSize, PropTag.Folder.NormalMessageSize);
				propertyMapping20 = new PhysicalColumnPropertyMapping(PropTag.Folder.NormalMessageSize, column68, null, null, null, (PhysicalColumn)column68.ActualColumn, true, true, true, true, false);
			}
			else
			{
				column68 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.NormalMessageSize, null), PropTag.Folder.NormalMessageSize);
				propertyMapping20 = new ConstantPropertyMapping(PropTag.Folder.NormalMessageSize, column68, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Folder.NormalMessageSize, propertyMapping20);
			Column column69 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.NormalMessageSize32, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column68), PropTag.Folder.NormalMessageSize32);
			ConversionPropertyMapping value47 = new ConversionPropertyMapping(PropTag.Folder.NormalMessageSize32, column69, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Folder.NormalMessageSize, propertyMapping20, null, null, null, false, true, true);
			dictionary.Add(PropTag.Folder.NormalMessageSize32, value47);
			Column column70;
			PropertyMapping propertyMapping21;
			if (folderTable.HiddenItemSize != null)
			{
				column70 = Factory.CreateMappedPropertyColumn(folderTable.HiddenItemSize, PropTag.Folder.AssociatedMessageSize);
				propertyMapping21 = new PhysicalColumnPropertyMapping(PropTag.Folder.AssociatedMessageSize, column70, null, null, null, (PhysicalColumn)column70.ActualColumn, true, true, true, true, false);
			}
			else
			{
				column70 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.AssociatedMessageSize, null), PropTag.Folder.AssociatedMessageSize);
				propertyMapping21 = new ConstantPropertyMapping(PropTag.Folder.AssociatedMessageSize, column70, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Folder.AssociatedMessageSize, propertyMapping21);
			Column column71 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.AssociatedMessageSize32, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column70), PropTag.Folder.AssociatedMessageSize32);
			ConversionPropertyMapping value48 = new ConversionPropertyMapping(PropTag.Folder.AssociatedMessageSize32, column71, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Folder.AssociatedMessageSize, propertyMapping21, null, null, null, false, true, true);
			dictionary.Add(PropTag.Folder.AssociatedMessageSize32, value48);
			Column[] dependOn5 = new Column[]
			{
				column68,
				column70
			};
			PropertyColumn propertyColumn5 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.MessageSize, rowPropBagCreator, dependOn5);
			ComputedPropertyMapping computedPropertyMapping2 = new ComputedPropertyMapping(PropTag.Folder.MessageSize, propertyColumn5, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetFolderMessageSize), new StorePropTag[]
			{
				PropTag.Folder.NormalMessageSize,
				PropTag.Folder.AssociatedMessageSize
			}, new PropertyMapping[]
			{
				propertyMapping20,
				propertyMapping21
			}, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Folder.MessageSize, computedPropertyMapping2);
			Column column72 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Folder.MessageSize32, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", propertyColumn5), PropTag.Folder.MessageSize32);
			ConversionPropertyMapping value49 = new ConversionPropertyMapping(PropTag.Folder.MessageSize32, column72, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Folder.MessageSize, computedPropertyMapping2, null, null, null, false, true, true);
			dictionary.Add(PropTag.Folder.MessageSize32, value49);
			PropertyMapping value50;
			if (folderTable.NormalItemPromotedColumns != null)
			{
				Column column73 = Factory.CreateMappedPropertyColumn(folderTable.NormalItemPromotedColumns, PropTag.Folder.PromotedProperties);
				value50 = new PhysicalColumnPropertyMapping(PropTag.Folder.PromotedProperties, column73, null, null, null, (PhysicalColumn)column73.ActualColumn, true, false, false, false, false);
			}
			else
			{
				Column column73 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.PromotedProperties, null), PropTag.Folder.PromotedProperties);
				value50 = new ConstantPropertyMapping(PropTag.Folder.PromotedProperties, column73, null, null, false, false, false);
			}
			dictionary.Add(PropTag.Folder.PromotedProperties, value50);
			PropertyMapping value51;
			if (folderTable.HiddenItemPromotedColumns != null)
			{
				Column column74 = Factory.CreateMappedPropertyColumn(folderTable.HiddenItemPromotedColumns, PropTag.Folder.HiddenPromotedProperties);
				value51 = new PhysicalColumnPropertyMapping(PropTag.Folder.HiddenPromotedProperties, column74, null, null, null, (PhysicalColumn)column74.ActualColumn, true, false, false, false, false);
			}
			else
			{
				Column column74 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.HiddenPromotedProperties, null), PropTag.Folder.HiddenPromotedProperties);
				value51 = new ConstantPropertyMapping(PropTag.Folder.HiddenPromotedProperties, column74, null, null, false, false, false);
			}
			dictionary.Add(PropTag.Folder.HiddenPromotedProperties, value51);
			Column column75 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.Rights, -1), PropTag.Folder.Rights);
			ConstantPropertyMapping value52 = new ConstantPropertyMapping(PropTag.Folder.Rights, column75, null, -1, true, true, true);
			dictionary.Add(PropTag.Folder.Rights, value52);
			PropertyMapping value53;
			if (folderTable.NextArticleNumber != null)
			{
				Column column76 = Factory.CreateMappedPropertyColumn(folderTable.NextArticleNumber, PropTag.Folder.ArticleNumNext);
				value53 = new PhysicalColumnPropertyMapping(PropTag.Folder.ArticleNumNext, column76, null, null, null, (PhysicalColumn)column76.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column76 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.ArticleNumNext, null), PropTag.Folder.ArticleNumNext);
				value53 = new ConstantPropertyMapping(PropTag.Folder.ArticleNumNext, column76, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Folder.ArticleNumNext, value53);
			Column[] dependOn6 = new Column[]
			{
				column3,
				column5
			};
			PropertyColumn column77 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.CorrelationId, rowPropBagCreator, dependOn6);
			ComputedPropertyMapping value54 = new ComputedPropertyMapping(PropTag.Folder.CorrelationId, column77, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetFolderCorrelationId), new StorePropTag[]
			{
				PropTag.Folder.MailboxNum,
				PropTag.Folder.Fid
			}, new PropertyMapping[]
			{
				functionPropertyMapping,
				conversionPropertyMapping
			}, null, null, null, false, true, false, false);
			dictionary.Add(PropTag.Folder.CorrelationId, value54);
			PropertyColumn propertyColumn6 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.FolderAdminFlags, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping4 = new DefaultPropertyMapping(PropTag.Folder.FolderAdminFlags, propertyColumn6, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Folder.FolderAdminFlags, defaultPropertyMapping4);
			Column[] dependOn7 = new Column[]
			{
				propertyColumn2,
				propertyColumn,
				propertyColumn6,
				column25
			};
			PropertyColumn column78 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.PartOfContentIndexing, rowPropBagCreator, dependOn7);
			ComputedPropertyMapping value55 = new ComputedPropertyMapping(PropTag.Folder.PartOfContentIndexing, column78, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetFolderPartOfContentIndexing), new StorePropTag[]
			{
				PropTag.Folder.IPMFolder,
				PropTag.Folder.FolderType,
				PropTag.Folder.FolderAdminFlags,
				PropTag.Folder.DisplayName
			}, new PropertyMapping[]
			{
				defaultPropertyMapping,
				computedPropertyMapping,
				defaultPropertyMapping4,
				propertyMapping8
			}, null, null, null, false, true, false, false);
			dictionary.Add(PropTag.Folder.PartOfContentIndexing, value55);
			PropertyColumn column79 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Folder.PropertyGroupMappingId, rowPropBagCreator, null);
			ComputedPropertyMapping value56 = new ComputedPropertyMapping(PropTag.Folder.PropertyGroupMappingId, column79, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetFolderPropertyGroupMappingId), null, null, null, null, null, false, false, false, false);
			dictionary.Add(PropTag.Folder.PropertyGroupMappingId, value56);
			objectPropertySchema.Initialize(ObjectType.Folder, table, dictionary, rowPropBagCreator, baseSchema);
			return objectPropertySchema;
		}

		public static ObjectPropertySchema GenerateAttachmentPropertySchema(StoreDatabase database)
		{
			Dictionary<StorePropTag, PropertyMapping> dictionary = new Dictionary<StorePropTag, PropertyMapping>(200);
			AttachmentTable attachmentTable = DatabaseSchema.AttachmentTable(database);
			if (attachmentTable == null)
			{
				return null;
			}
			Table table = attachmentTable.Table;
			ObjectPropertySchema objectPropertySchema = new ObjectPropertySchema();
			ObjectPropertySchema baseSchema = null;
			Func<IRowAccess, IRowPropertyBag> rowPropBagCreator = (IRowAccess rowAccess) => new RowPropertyBag(table, objectPropertySchema, PropTag.Attachment.MailboxNum, rowAccess);
			Column column;
			PropertyMapping propertyMapping;
			if (attachmentTable.MailboxPartitionNumber != null)
			{
				column = Factory.CreateMappedPropertyColumn(attachmentTable.MailboxPartitionNumber, PropTag.Attachment.MailboxPartitionNumber);
				propertyMapping = new PhysicalColumnPropertyMapping(PropTag.Attachment.MailboxPartitionNumber, column, null, null, null, (PhysicalColumn)column.ActualColumn, false, true, true, false, false);
			}
			else
			{
				column = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.MailboxPartitionNumber, null), PropTag.Attachment.MailboxPartitionNumber);
				propertyMapping = new ConstantPropertyMapping(PropTag.Attachment.MailboxPartitionNumber, column, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Attachment.MailboxPartitionNumber, propertyMapping);
			Column column2;
			PropertyMapping propertyMapping2;
			if (attachmentTable.MailboxNumber != null)
			{
				column2 = Factory.CreateMappedPropertyColumn(attachmentTable.MailboxNumber, PropTag.Attachment.MailboxNumberInternal);
				propertyMapping2 = new PhysicalColumnPropertyMapping(PropTag.Attachment.MailboxNumberInternal, column2, null, null, null, (PhysicalColumn)column2.ActualColumn, false, true, true, false, false);
			}
			else
			{
				column2 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.MailboxNumberInternal, null), PropTag.Attachment.MailboxNumberInternal);
				propertyMapping2 = new ConstantPropertyMapping(PropTag.Attachment.MailboxNumberInternal, column2, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Attachment.MailboxNumberInternal, propertyMapping2);
			Column column3 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructFunctionColumn(PropTag.Attachment.MailboxNum, typeof(int), 4, 0, attachmentTable.Table, new Func<object[], object>(PropertySchemaPopulation.ComputeMailboxNumber), "Exchange.ComputeMailboxNumber", new Column[]
			{
				column,
				column2
			}), PropTag.Attachment.MailboxNum);
			FunctionPropertyMapping value = new FunctionPropertyMapping(PropTag.Attachment.MailboxNum, column3, null, new Func<object[], object>(PropertySchemaPopulation.ComputeMailboxNumber), new PropertyMapping[]
			{
				propertyMapping,
				propertyMapping2
			}, true, true, false);
			dictionary.Add(PropTag.Attachment.MailboxNum, value);
			PropertyMapping value2;
			if (attachmentTable.Name != null)
			{
				Column column4 = Factory.CreateMappedPropertyColumn(attachmentTable.Name, PropTag.Attachment.DisplayName);
				value2 = new PhysicalColumnPropertyMapping(PropTag.Attachment.DisplayName, column4, null, null, null, (PhysicalColumn)column4.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column4 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.DisplayName, null), PropTag.Attachment.DisplayName);
				value2 = new ConstantPropertyMapping(PropTag.Attachment.DisplayName, column4, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Attachment.DisplayName, value2);
			PropertyMapping value3;
			if (attachmentTable.CreationTime != null)
			{
				Column column5 = Factory.CreateMappedPropertyColumn(attachmentTable.CreationTime, PropTag.Attachment.CreationTime);
				value3 = new PhysicalColumnPropertyMapping(PropTag.Attachment.CreationTime, column5, null, null, null, (PhysicalColumn)column5.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column5 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.CreationTime, null), PropTag.Attachment.CreationTime);
				value3 = new ConstantPropertyMapping(PropTag.Attachment.CreationTime, column5, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Attachment.CreationTime, value3);
			PropertyMapping value4;
			if (attachmentTable.LastModificationTime != null)
			{
				Column column6 = Factory.CreateMappedPropertyColumn(attachmentTable.LastModificationTime, PropTag.Attachment.LastModificationTime);
				value4 = new PhysicalColumnPropertyMapping(PropTag.Attachment.LastModificationTime, column6, null, null, null, (PhysicalColumn)column6.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column6 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.LastModificationTime, null), PropTag.Attachment.LastModificationTime);
				value4 = new ConstantPropertyMapping(PropTag.Attachment.LastModificationTime, column6, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Attachment.LastModificationTime, value4);
			PropertyMapping value5;
			if (attachmentTable.Content != null)
			{
				Column column7 = Factory.CreateMappedPropertyColumn(attachmentTable.Content, PropTag.Attachment.Content);
				value5 = new PhysicalColumnPropertyMapping(PropTag.Attachment.Content, column7, null, null, null, (PhysicalColumn)column7.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column7 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.Content, null), PropTag.Attachment.Content);
				value5 = new ConstantPropertyMapping(PropTag.Attachment.Content, column7, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Attachment.Content, value5);
			PropertyMapping value6;
			if (attachmentTable.Content != null)
			{
				Column column8 = Factory.CreateMappedPropertyColumn(attachmentTable.Content, PropTag.Attachment.ContentObj);
				value6 = new PhysicalColumnPropertyMapping(PropTag.Attachment.ContentObj, column8, null, null, null, (PhysicalColumn)column8.ActualColumn, true, false, true, true, false);
			}
			else
			{
				Column column8 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.ContentObj, null), PropTag.Attachment.ContentObj);
				value6 = new ConstantPropertyMapping(PropTag.Attachment.ContentObj, column8, null, null, false, true, true);
			}
			dictionary.Add(PropTag.Attachment.ContentObj, value6);
			PropertyMapping value7;
			if (attachmentTable.AttachmentMethod != null)
			{
				Column column9 = Factory.CreateMappedPropertyColumn(attachmentTable.AttachmentMethod, PropTag.Attachment.AttachMethod);
				value7 = new PhysicalColumnPropertyMapping(PropTag.Attachment.AttachMethod, column9, null, null, null, (PhysicalColumn)column9.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column9 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.AttachMethod, null), PropTag.Attachment.AttachMethod);
				value7 = new ConstantPropertyMapping(PropTag.Attachment.AttachMethod, column9, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Attachment.AttachMethod, value7);
			Column column10;
			PropertyMapping propertyMapping3;
			if (attachmentTable.AttachmentId != null)
			{
				column10 = Factory.CreateMappedPropertyColumn(attachmentTable.AttachmentId, PropTag.Attachment.AttachmentIdBin);
				propertyMapping3 = new PhysicalColumnPropertyMapping(PropTag.Attachment.AttachmentIdBin, column10, null, null, null, (PhysicalColumn)column10.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column10 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.AttachmentIdBin, null), PropTag.Attachment.AttachmentIdBin);
				propertyMapping3 = new ConstantPropertyMapping(PropTag.Attachment.AttachmentIdBin, column10, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Attachment.AttachmentIdBin, propertyMapping3);
			Column column11 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Attachment.AttachmentId, typeof(long), 8, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), "Exchange.ConvertExchangeIdToInt64", column10), PropTag.Attachment.AttachmentId);
			ConversionPropertyMapping value8 = new ConversionPropertyMapping(PropTag.Attachment.AttachmentId, column11, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), PropTag.Attachment.AttachmentIdBin, propertyMapping3, null, null, null, true, true, false);
			dictionary.Add(PropTag.Attachment.AttachmentId, value8);
			PropertyMapping value9;
			if (attachmentTable.RenderingPosition != null)
			{
				Column column12 = Factory.CreateMappedPropertyColumn(attachmentTable.RenderingPosition, PropTag.Attachment.RenderingPosition);
				value9 = new PhysicalColumnPropertyMapping(PropTag.Attachment.RenderingPosition, column12, null, null, null, (PhysicalColumn)column12.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column12 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.RenderingPosition, null), PropTag.Attachment.RenderingPosition);
				value9 = new ConstantPropertyMapping(PropTag.Attachment.RenderingPosition, column12, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Attachment.RenderingPosition, value9);
			PropertyColumn column13 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Attachment.Language, rowPropBagCreator, null);
			ComputedPropertyMapping value10 = new ComputedPropertyMapping(PropTag.Attachment.Language, column13, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetAttachmentLanguage), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetAttachmentLanguage), null, null, false, true, true, true);
			dictionary.Add(PropTag.Attachment.Language, value10);
			Column column14;
			PropertyMapping propertyMapping4;
			if (attachmentTable.Size != null)
			{
				column14 = Factory.CreateMappedPropertyColumn(attachmentTable.Size, PropTag.Attachment.AttachSizeInt64);
				propertyMapping4 = new PhysicalColumnPropertyMapping(PropTag.Attachment.AttachSizeInt64, column14, null, null, null, (PhysicalColumn)column14.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column14 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.AttachSizeInt64, null), PropTag.Attachment.AttachSizeInt64);
				propertyMapping4 = new ConstantPropertyMapping(PropTag.Attachment.AttachSizeInt64, column14, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Attachment.AttachSizeInt64, propertyMapping4);
			Column column15 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Attachment.AttachSize, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column14), PropTag.Attachment.AttachSize);
			ConversionPropertyMapping value11 = new ConversionPropertyMapping(PropTag.Attachment.AttachSize, column15, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Attachment.AttachSizeInt64, propertyMapping4, null, null, null, true, true, true);
			dictionary.Add(PropTag.Attachment.AttachSize, value11);
			PropertyMapping value12;
			if (attachmentTable.ContentId != null)
			{
				Column column16 = Factory.CreateMappedPropertyColumn(attachmentTable.ContentId, PropTag.Attachment.ContentId);
				value12 = new PhysicalColumnPropertyMapping(PropTag.Attachment.ContentId, column16, null, null, null, (PhysicalColumn)column16.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column16 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.ContentId, null), PropTag.Attachment.ContentId);
				value12 = new ConstantPropertyMapping(PropTag.Attachment.ContentId, column16, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Attachment.ContentId, value12);
			PropertyMapping value13;
			if (attachmentTable.ContentType != null)
			{
				Column column17 = Factory.CreateMappedPropertyColumn(attachmentTable.ContentType, PropTag.Attachment.ContentType);
				value13 = new PhysicalColumnPropertyMapping(PropTag.Attachment.ContentType, column17, null, null, null, (PhysicalColumn)column17.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column17 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.ContentType, null), PropTag.Attachment.ContentType);
				value13 = new ConstantPropertyMapping(PropTag.Attachment.ContentType, column17, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Attachment.ContentType, value13);
			PropertyColumn column18 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Attachment.HasNamedProperties, rowPropBagCreator, null);
			ComputedPropertyMapping value14 = new ComputedPropertyMapping(PropTag.Attachment.HasNamedProperties, column18, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetAttachmentHasNamedProperties), null, null, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Attachment.HasNamedProperties, value14);
			Column column19 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Attachment.Mid, typeof(long), 8, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), "Exchange.ConvertExchangeIdToInt64", column10), PropTag.Attachment.Mid);
			ConversionPropertyMapping value15 = new ConversionPropertyMapping(PropTag.Attachment.Mid, column19, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), PropTag.Attachment.AttachmentIdBin, propertyMapping3, null, null, null, true, true, false);
			dictionary.Add(PropTag.Attachment.Mid, value15);
			PropertyMapping value16;
			if (attachmentTable.SubobjectsBlob != null)
			{
				Column column20 = Factory.CreateMappedPropertyColumn(attachmentTable.SubobjectsBlob, PropTag.Attachment.ItemSubobjectsBin);
				value16 = new PhysicalColumnPropertyMapping(PropTag.Attachment.ItemSubobjectsBin, column20, null, null, null, (PhysicalColumn)column20.ActualColumn, true, false, false, false, false);
			}
			else
			{
				Column column20 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.ItemSubobjectsBin, null), PropTag.Attachment.ItemSubobjectsBin);
				value16 = new ConstantPropertyMapping(PropTag.Attachment.ItemSubobjectsBin, column20, null, null, false, false, false);
			}
			dictionary.Add(PropTag.Attachment.ItemSubobjectsBin, value16);
			objectPropertySchema.Initialize(ObjectType.Attachment, table, dictionary, rowPropBagCreator, baseSchema);
			return objectPropertySchema;
		}

		public static ObjectPropertySchema GenerateAttachmentViewPropertySchema(StoreDatabase database)
		{
			Dictionary<StorePropTag, PropertyMapping> dictionary = new Dictionary<StorePropTag, PropertyMapping>(200);
			AttachmentTableFunctionTableFunction attachmentTableFunctionTableFunction = DatabaseSchema.AttachmentTableFunctionTableFunction(database);
			Table tableFunction = attachmentTableFunctionTableFunction.TableFunction;
			ObjectPropertySchema objectPropertySchema = new ObjectPropertySchema();
			ObjectPropertySchema objectSchema = PropertySchema.GetObjectSchema(database, ObjectType.Attachment);
			Func<IRowAccess, IRowPropertyBag> rowPropBagCreator = null;
			PropertyMapping value;
			if (attachmentTableFunctionTableFunction.AttachmentNumber != null)
			{
				Column column = Factory.CreateMappedPropertyColumn(attachmentTableFunctionTableFunction.AttachmentNumber, PropTag.Attachment.AttachNum);
				value = new PhysicalColumnPropertyMapping(PropTag.Attachment.AttachNum, column, null, null, null, (PhysicalColumn)column.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Attachment.AttachNum, null), PropTag.Attachment.AttachNum);
				value = new ConstantPropertyMapping(PropTag.Attachment.AttachNum, column, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Attachment.AttachNum, value);
			objectPropertySchema.Initialize(ObjectType.AttachmentView, tableFunction, dictionary, rowPropBagCreator, objectSchema);
			return objectPropertySchema;
		}

		public static ObjectPropertySchema GenerateFolderViewPropertySchema(StoreDatabase database)
		{
			Dictionary<StorePropTag, PropertyMapping> dictionary = new Dictionary<StorePropTag, PropertyMapping>(200);
			FolderHierarchyBlobTableFunction folderHierarchyBlobTableFunction = DatabaseSchema.FolderHierarchyBlobTableFunction(database);
			Table tableFunction = folderHierarchyBlobTableFunction.TableFunction;
			ObjectPropertySchema objectPropertySchema = new ObjectPropertySchema();
			ObjectPropertySchema objectSchema = PropertySchema.GetObjectSchema(database, ObjectType.Folder);
			Func<IRowAccess, IRowPropertyBag> rowPropBagCreator = null;
			PropertyMapping value;
			if (folderHierarchyBlobTableFunction.Depth != null)
			{
				Column column = Factory.CreateMappedPropertyColumn(folderHierarchyBlobTableFunction.Depth, PropTag.Folder.Depth);
				value = new PhysicalColumnPropertyMapping(PropTag.Folder.Depth, column, null, null, null, (PhysicalColumn)column.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Folder.Depth, null), PropTag.Folder.Depth);
				value = new ConstantPropertyMapping(PropTag.Folder.Depth, column, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Folder.Depth, value);
			objectPropertySchema.Initialize(ObjectType.FolderView, tableFunction, dictionary, rowPropBagCreator, objectSchema);
			return objectPropertySchema;
		}

		public static ObjectPropertySchema GenerateEventPropertySchema(StoreDatabase database)
		{
			Dictionary<StorePropTag, PropertyMapping> dictionary = new Dictionary<StorePropTag, PropertyMapping>(200);
			EventsTable eventsTable = DatabaseSchema.EventsTable(database);
			if (eventsTable == null)
			{
				return null;
			}
			Table table = eventsTable.Table;
			ObjectPropertySchema objectPropertySchema = new ObjectPropertySchema();
			ObjectPropertySchema baseSchema = null;
			Func<IRowAccess, IRowPropertyBag> rowPropBagCreator = (IRowAccess rowAccess) => new RowPropertyBag(table, objectPropertySchema, PropTag.Event.MailboxNum, rowAccess);
			PropertyMapping value;
			if (eventsTable.EventCounter != null)
			{
				Column column = Factory.CreateMappedPropertyColumn(eventsTable.EventCounter, PropTag.Event.EventCounter);
				value = new PhysicalColumnPropertyMapping(PropTag.Event.EventCounter, column, null, null, null, (PhysicalColumn)column.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventCounter, null), PropTag.Event.EventCounter);
				value = new ConstantPropertyMapping(PropTag.Event.EventCounter, column, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventCounter, value);
			PropertyMapping value2;
			if (eventsTable.CreateTime != null)
			{
				Column column2 = Factory.CreateMappedPropertyColumn(eventsTable.CreateTime, PropTag.Event.EventCreatedTime);
				value2 = new PhysicalColumnPropertyMapping(PropTag.Event.EventCreatedTime, column2, null, null, null, (PhysicalColumn)column2.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column2 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventCreatedTime, null), PropTag.Event.EventCreatedTime);
				value2 = new ConstantPropertyMapping(PropTag.Event.EventCreatedTime, column2, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventCreatedTime, value2);
			PropertyMapping value3;
			if (eventsTable.TransactionId != null)
			{
				Column column3 = Factory.CreateMappedPropertyColumn(eventsTable.TransactionId, PropTag.Event.EventTransacId);
				value3 = new PhysicalColumnPropertyMapping(PropTag.Event.EventTransacId, column3, null, null, null, (PhysicalColumn)column3.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column3 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventTransacId, null), PropTag.Event.EventTransacId);
				value3 = new ConstantPropertyMapping(PropTag.Event.EventTransacId, column3, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventTransacId, value3);
			PropertyMapping value4;
			if (eventsTable.EventType != null)
			{
				Column column4 = Factory.CreateMappedPropertyColumn(eventsTable.EventType, PropTag.Event.EventMask);
				value4 = new PhysicalColumnPropertyMapping(PropTag.Event.EventMask, column4, null, null, null, (PhysicalColumn)column4.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column4 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventMask, null), PropTag.Event.EventMask);
				value4 = new ConstantPropertyMapping(PropTag.Event.EventMask, column4, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventMask, value4);
			PropertyMapping value5;
			if (eventsTable.ClientType != null)
			{
				Column column5 = Factory.CreateMappedPropertyColumn(eventsTable.ClientType, PropTag.Event.EventClientType);
				value5 = new PhysicalColumnPropertyMapping(PropTag.Event.EventClientType, column5, null, null, null, (PhysicalColumn)column5.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column5 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventClientType, null), PropTag.Event.EventClientType);
				value5 = new ConstantPropertyMapping(PropTag.Event.EventClientType, column5, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventClientType, value5);
			PropertyMapping value6;
			if (eventsTable.Flags != null)
			{
				Column column6 = Factory.CreateMappedPropertyColumn(eventsTable.Flags, PropTag.Event.EventFlags);
				value6 = new PhysicalColumnPropertyMapping(PropTag.Event.EventFlags, column6, null, null, null, (PhysicalColumn)column6.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column6 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventFlags, null), PropTag.Event.EventFlags);
				value6 = new ConstantPropertyMapping(PropTag.Event.EventFlags, column6, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventFlags, value6);
			PropertyMapping value7;
			if (eventsTable.ObjectClass != null)
			{
				Column column7 = Factory.CreateMappedPropertyColumn(eventsTable.ObjectClass, PropTag.Event.EventMessageClass);
				value7 = new PhysicalColumnPropertyMapping(PropTag.Event.EventMessageClass, column7, null, null, null, (PhysicalColumn)column7.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column7 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventMessageClass, null), PropTag.Event.EventMessageClass);
				value7 = new ConstantPropertyMapping(PropTag.Event.EventMessageClass, column7, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventMessageClass, value7);
			PropertyMapping value8;
			if (eventsTable.Fid != null)
			{
				Column column8 = Factory.CreateMappedPropertyColumn(eventsTable.Fid, PropTag.Event.EventFid);
				value8 = new PhysicalColumnPropertyMapping(PropTag.Event.EventFid, column8, null, null, null, (PhysicalColumn)column8.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column8 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventFid, null), PropTag.Event.EventFid);
				value8 = new ConstantPropertyMapping(PropTag.Event.EventFid, column8, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventFid, value8);
			PropertyMapping value9;
			if (eventsTable.Mid != null)
			{
				Column column9 = Factory.CreateMappedPropertyColumn(eventsTable.Mid, PropTag.Event.EventMid);
				value9 = new PhysicalColumnPropertyMapping(PropTag.Event.EventMid, column9, null, null, null, (PhysicalColumn)column9.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column9 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventMid, null), PropTag.Event.EventMid);
				value9 = new ConstantPropertyMapping(PropTag.Event.EventMid, column9, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventMid, value9);
			PropertyMapping value10;
			if (eventsTable.ParentFid != null)
			{
				Column column10 = Factory.CreateMappedPropertyColumn(eventsTable.ParentFid, PropTag.Event.EventFidParent);
				value10 = new PhysicalColumnPropertyMapping(PropTag.Event.EventFidParent, column10, null, null, null, (PhysicalColumn)column10.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column10 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventFidParent, null), PropTag.Event.EventFidParent);
				value10 = new ConstantPropertyMapping(PropTag.Event.EventFidParent, column10, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventFidParent, value10);
			PropertyMapping value11;
			if (eventsTable.OldFid != null)
			{
				Column column11 = Factory.CreateMappedPropertyColumn(eventsTable.OldFid, PropTag.Event.EventFidOld);
				value11 = new PhysicalColumnPropertyMapping(PropTag.Event.EventFidOld, column11, null, null, null, (PhysicalColumn)column11.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column11 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventFidOld, null), PropTag.Event.EventFidOld);
				value11 = new ConstantPropertyMapping(PropTag.Event.EventFidOld, column11, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventFidOld, value11);
			PropertyMapping value12;
			if (eventsTable.OldMid != null)
			{
				Column column12 = Factory.CreateMappedPropertyColumn(eventsTable.OldMid, PropTag.Event.EventMidOld);
				value12 = new PhysicalColumnPropertyMapping(PropTag.Event.EventMidOld, column12, null, null, null, (PhysicalColumn)column12.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column12 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventMidOld, null), PropTag.Event.EventMidOld);
				value12 = new ConstantPropertyMapping(PropTag.Event.EventMidOld, column12, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventMidOld, value12);
			PropertyMapping value13;
			if (eventsTable.OldParentFid != null)
			{
				Column column13 = Factory.CreateMappedPropertyColumn(eventsTable.OldParentFid, PropTag.Event.EventFidOldParent);
				value13 = new PhysicalColumnPropertyMapping(PropTag.Event.EventFidOldParent, column13, null, null, null, (PhysicalColumn)column13.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column13 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventFidOldParent, null), PropTag.Event.EventFidOldParent);
				value13 = new ConstantPropertyMapping(PropTag.Event.EventFidOldParent, column13, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventFidOldParent, value13);
			PropertyMapping value14;
			if (eventsTable.ItemCount != null)
			{
				Column column14 = Factory.CreateMappedPropertyColumn(eventsTable.ItemCount, PropTag.Event.EventItemCount);
				value14 = new PhysicalColumnPropertyMapping(PropTag.Event.EventItemCount, column14, null, null, null, (PhysicalColumn)column14.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column14 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventItemCount, null), PropTag.Event.EventItemCount);
				value14 = new ConstantPropertyMapping(PropTag.Event.EventItemCount, column14, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventItemCount, value14);
			PropertyMapping value15;
			if (eventsTable.UnreadCount != null)
			{
				Column column15 = Factory.CreateMappedPropertyColumn(eventsTable.UnreadCount, PropTag.Event.EventUnreadCount);
				value15 = new PhysicalColumnPropertyMapping(PropTag.Event.EventUnreadCount, column15, null, null, null, (PhysicalColumn)column15.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column15 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventUnreadCount, null), PropTag.Event.EventUnreadCount);
				value15 = new ConstantPropertyMapping(PropTag.Event.EventUnreadCount, column15, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventUnreadCount, value15);
			PropertyMapping value16;
			if (eventsTable.ExtendedFlags != null)
			{
				Column column16 = Factory.CreateMappedPropertyColumn(eventsTable.ExtendedFlags, PropTag.Event.EventExtendedFlags);
				value16 = new PhysicalColumnPropertyMapping(PropTag.Event.EventExtendedFlags, column16, null, null, null, (PhysicalColumn)column16.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column16 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventExtendedFlags, null), PropTag.Event.EventExtendedFlags);
				value16 = new ConstantPropertyMapping(PropTag.Event.EventExtendedFlags, column16, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventExtendedFlags, value16);
			PropertyMapping value17;
			if (eventsTable.Sid != null)
			{
				Column column17 = Factory.CreateMappedPropertyColumn(eventsTable.Sid, PropTag.Event.EventSid);
				value17 = new PhysicalColumnPropertyMapping(PropTag.Event.EventSid, column17, null, null, null, (PhysicalColumn)column17.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column17 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventSid, null), PropTag.Event.EventSid);
				value17 = new ConstantPropertyMapping(PropTag.Event.EventSid, column17, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventSid, value17);
			PropertyMapping value18;
			if (eventsTable.DocumentId != null)
			{
				Column column18 = Factory.CreateMappedPropertyColumn(eventsTable.DocumentId, PropTag.Event.EventDocId);
				value18 = new PhysicalColumnPropertyMapping(PropTag.Event.EventDocId, column18, null, null, null, (PhysicalColumn)column18.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column18 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.EventDocId, null), PropTag.Event.EventDocId);
				value18 = new ConstantPropertyMapping(PropTag.Event.EventDocId, column18, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Event.EventDocId, value18);
			PropertyMapping value19;
			if (eventsTable.MailboxNumber != null)
			{
				Column column19 = Factory.CreateMappedPropertyColumn(eventsTable.MailboxNumber, PropTag.Event.MailboxNum);
				value19 = new PhysicalColumnPropertyMapping(PropTag.Event.MailboxNum, column19, null, null, null, (PhysicalColumn)column19.ActualColumn, true, true, true, false, false);
			}
			else
			{
				Column column19 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Event.MailboxNum, null), PropTag.Event.MailboxNum);
				value19 = new ConstantPropertyMapping(PropTag.Event.MailboxNum, column19, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Event.MailboxNum, value19);
			PropertyColumn column20 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Event.EventMailboxGuid, rowPropBagCreator, null);
			DefaultPropertyMapping value20 = new DefaultPropertyMapping(PropTag.Event.EventMailboxGuid, column20, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Event.EventMailboxGuid, value20);
			objectPropertySchema.Initialize(ObjectType.Event, table, dictionary, rowPropBagCreator, baseSchema);
			return objectPropertySchema;
		}

		public static ObjectPropertySchema GenerateRecipientPropertySchema(StoreDatabase database)
		{
			Dictionary<StorePropTag, PropertyMapping> dictionary = new Dictionary<StorePropTag, PropertyMapping>(200);
			RecipientTableFunctionTableFunction recipientTableFunctionTableFunction = DatabaseSchema.RecipientTableFunctionTableFunction(database);
			Table table = recipientTableFunctionTableFunction.TableFunction;
			ObjectPropertySchema objectPropertySchema = new ObjectPropertySchema();
			ObjectPropertySchema baseSchema = null;
			Func<IRowAccess, IRowPropertyBag> rowPropBagCreator = (IRowAccess rowAccess) => new RowPropertyBag(table, objectPropertySchema, StorePropTag.Invalid, rowAccess);
			Column column = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Recipient.RowId, 0), PropTag.Recipient.RowId);
			ConstantPropertyMapping value = new ConstantPropertyMapping(PropTag.Recipient.RowId, column, null, 0, true, true, true);
			dictionary.Add(PropTag.Recipient.RowId, value);
			PropertyColumn column2 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.RecipientType, rowPropBagCreator, null);
			ComputedPropertyMapping value2 = new ComputedPropertyMapping(PropTag.Recipient.RecipientType, column2, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetRecipientRecipientType), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetRecipientRecipientType), null, null, false, true, true, true);
			dictionary.Add(PropTag.Recipient.RecipientType, value2);
			PropertyColumn column3 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.DisplayName, rowPropBagCreator, null);
			DefaultPropertyMapping value3 = new DefaultPropertyMapping(PropTag.Recipient.DisplayName, column3, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetRecipientDisplayName), null, null, true, true, true, false);
			dictionary.Add(PropTag.Recipient.DisplayName, value3);
			PropertyColumn column4 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.UserDN, rowPropBagCreator, null);
			DefaultPropertyMapping value4 = new DefaultPropertyMapping(PropTag.Recipient.UserDN, column4, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Recipient.UserDN, value4);
			PropertyColumn column5 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.AddressType, rowPropBagCreator, null);
			ComputedPropertyMapping value5 = new ComputedPropertyMapping(PropTag.Recipient.AddressType, column5, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetRecipientAddressType), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetRecipientAddressType), null, null, false, true, true, true);
			dictionary.Add(PropTag.Recipient.AddressType, value5);
			PropertyColumn column6 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.EmailAddress, rowPropBagCreator, null);
			ComputedPropertyMapping value6 = new ComputedPropertyMapping(PropTag.Recipient.EmailAddress, column6, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetRecipientEmailAddress), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetRecipientEmailAddress), null, null, false, true, true, true);
			dictionary.Add(PropTag.Recipient.EmailAddress, value6);
			PropertyColumn column7 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.SendInternetEncoding, rowPropBagCreator, null);
			DefaultPropertyMapping value7 = new DefaultPropertyMapping(PropTag.Recipient.SendInternetEncoding, column7, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Recipient.SendInternetEncoding, value7);
			PropertyColumn propertyColumn = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.EntryId, rowPropBagCreator, null);
			ComputedPropertyMapping computedPropertyMapping = new ComputedPropertyMapping(PropTag.Recipient.EntryId, propertyColumn, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetRecipientEntryId), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetRecipientEntryId), null, null, false, true, true, true);
			dictionary.Add(PropTag.Recipient.EntryId, computedPropertyMapping);
			Column[] dependOn = new Column[]
			{
				propertyColumn
			};
			PropertyColumn column8 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.EntryIdSvrEid, rowPropBagCreator, dependOn);
			ComputedPropertyMapping value8 = new ComputedPropertyMapping(PropTag.Recipient.EntryIdSvrEid, column8, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetRecipientEntryIdSvrEid), new StorePropTag[]
			{
				PropTag.Recipient.EntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping
			}, null, null, null, false, false, true, true);
			dictionary.Add(PropTag.Recipient.EntryIdSvrEid, value8);
			Column[] dependOn2 = new Column[]
			{
				propertyColumn
			};
			PropertyColumn column9 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.RecordKey, rowPropBagCreator, dependOn2);
			ComputedPropertyMapping value9 = new ComputedPropertyMapping(PropTag.Recipient.RecordKey, column9, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetRecipientRecordKey), new StorePropTag[]
			{
				PropTag.Recipient.EntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Recipient.RecordKey, value9);
			Column[] dependOn3 = new Column[]
			{
				propertyColumn
			};
			PropertyColumn column10 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.RecipientEntryId, rowPropBagCreator, dependOn3);
			ComputedPropertyMapping value10 = new ComputedPropertyMapping(PropTag.Recipient.RecipientEntryId, column10, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetRecipientRecipientEntryId), new StorePropTag[]
			{
				PropTag.Recipient.EntryId
			}, new PropertyMapping[]
			{
				computedPropertyMapping
			}, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetRecipientRecipientEntryId), null, null, false, true, true, true);
			dictionary.Add(PropTag.Recipient.RecipientEntryId, value10);
			PropertyColumn column11 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.Responsibility, rowPropBagCreator, null);
			ComputedPropertyMapping value11 = new ComputedPropertyMapping(PropTag.Recipient.Responsibility, column11, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetRecipientResponsibility), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetRecipientResponsibility), null, null, false, true, true, true);
			dictionary.Add(PropTag.Recipient.Responsibility, value11);
			PropertyColumn column12 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.SendRichInfo, rowPropBagCreator, null);
			ComputedPropertyMapping value12 = new ComputedPropertyMapping(PropTag.Recipient.SendRichInfo, column12, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetRecipientSendRichInfo), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetRecipientSendRichInfo), null, null, false, true, true, true);
			dictionary.Add(PropTag.Recipient.SendRichInfo, value12);
			PropertyColumn column13 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.DisplayType, rowPropBagCreator, null);
			ComputedPropertyMapping value13 = new ComputedPropertyMapping(PropTag.Recipient.DisplayType, column13, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetRecipientDisplayType), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetRecipientDisplayType), null, null, false, true, true, true);
			dictionary.Add(PropTag.Recipient.DisplayType, value13);
			PropertyColumn column14 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.ObjectType, rowPropBagCreator, null);
			ComputedPropertyMapping value14 = new ComputedPropertyMapping(PropTag.Recipient.ObjectType, column14, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetRecipientObjectType), null, null, null, null, null, true, true, true, true);
			dictionary.Add(PropTag.Recipient.ObjectType, value14);
			PropertyColumn column15 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Recipient.SearchKey, rowPropBagCreator, null);
			ComputedPropertyMapping value15 = new ComputedPropertyMapping(PropTag.Recipient.SearchKey, column15, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetRecipientSearchKey), null, null, new Func<Context, ISimplePropertyBag, object, ErrorCode>(PropertySchemaPopulation.SetRecipientSearchKey), null, null, false, true, true, true);
			dictionary.Add(PropTag.Recipient.SearchKey, value15);
			objectPropertySchema.Initialize(ObjectType.Recipient, table, dictionary, rowPropBagCreator, baseSchema);
			return objectPropertySchema;
		}

		public static ObjectPropertySchema GenerateConversationPropertySchema(StoreDatabase database)
		{
			Dictionary<StorePropTag, PropertyMapping> dictionary = new Dictionary<StorePropTag, PropertyMapping>(200);
			MessageTable messageTable = DatabaseSchema.MessageTable(database);
			if (messageTable == null)
			{
				return null;
			}
			Table table = messageTable.Table;
			ObjectPropertySchema objectPropertySchema = new ObjectPropertySchema();
			ObjectPropertySchema objectSchema = PropertySchema.GetObjectSchema(database, ObjectType.Message);
			Func<IRowAccess, IRowPropertyBag> rowPropBagCreator = null;
			Column column;
			PropertyMapping propertyMapping;
			if (messageTable.MessageId != null)
			{
				column = Factory.CreateMappedPropertyColumn(messageTable.MessageId, PropTag.Message.MidBin);
				propertyMapping = new PhysicalColumnPropertyMapping(PropTag.Message.MidBin, column, null, null, null, (PhysicalColumn)column.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Message.MidBin, null), PropTag.Message.MidBin);
				propertyMapping = new ConstantPropertyMapping(PropTag.Message.MidBin, column, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Message.MidBin, propertyMapping);
			Column column2 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.Mid, typeof(long), 8, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), "Exchange.ConvertExchangeIdToInt64", column), PropTag.Message.Mid);
			ConversionPropertyMapping conversionPropertyMapping = new ConversionPropertyMapping(PropTag.Message.Mid, column2, new Func<object, object>(PropertySchemaPopulation.ConvertExchangeIdToInt64), PropTag.Message.MidBin, propertyMapping, null, null, null, true, true, true);
			dictionary.Add(PropTag.Message.Mid, conversionPropertyMapping);
			Column column3 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.EntryIdSvrEid, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToFolderSvrEid), "Exchange.ConvertInt64ToFolderSvrEid", column2), PropTag.Message.EntryIdSvrEid);
			ConversionPropertyMapping value = new ConversionPropertyMapping(PropTag.Message.EntryIdSvrEid, column3, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToFolderSvrEid), PropTag.Message.Mid, conversionPropertyMapping, null, null, null, true, true, true);
			dictionary.Add(PropTag.Message.EntryIdSvrEid, value);
			Column column4 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Message.EntryId, typeof(byte[]), 21, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToFolderSvrEid), "Exchange.ConvertInt64ToFolderSvrEid", column2), PropTag.Message.EntryId);
			ConversionPropertyMapping value2 = new ConversionPropertyMapping(PropTag.Message.EntryId, column4, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToFolderSvrEid), PropTag.Message.Mid, conversionPropertyMapping, null, null, null, false, true, true);
			dictionary.Add(PropTag.Message.EntryId, value2);
			objectPropertySchema.Initialize(ObjectType.Conversation, table, dictionary, rowPropBagCreator, objectSchema);
			return objectPropertySchema;
		}

		public static ObjectPropertySchema GenerateInferenceLogPropertySchema(StoreDatabase database)
		{
			Dictionary<StorePropTag, PropertyMapping> dictionary = new Dictionary<StorePropTag, PropertyMapping>(200);
			InferenceLogTable inferenceLogTable = DatabaseSchema.InferenceLogTable(database);
			if (inferenceLogTable == null)
			{
				return null;
			}
			Table table = inferenceLogTable.Table;
			ObjectPropertySchema objectPropertySchema = new ObjectPropertySchema();
			ObjectPropertySchema baseSchema = null;
			Func<IRowAccess, IRowPropertyBag> rowPropBagCreator = (IRowAccess rowAccess) => new RowPropertyBag(table, objectPropertySchema, PropTag.InferenceLog.MailboxNum, rowAccess);
			PropertyMapping value;
			if (inferenceLogTable.RowId != null)
			{
				Column column = Factory.CreateMappedPropertyColumn(inferenceLogTable.RowId, PropTag.InferenceLog.RowId);
				value = new PhysicalColumnPropertyMapping(PropTag.InferenceLog.RowId, column, null, null, null, (PhysicalColumn)column.ActualColumn, false, true, true, true, false);
			}
			else
			{
				Column column = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.InferenceLog.RowId, null), PropTag.InferenceLog.RowId);
				value = new ConstantPropertyMapping(PropTag.InferenceLog.RowId, column, null, null, true, true, true);
			}
			dictionary.Add(PropTag.InferenceLog.RowId, value);
			Column column2;
			PropertyMapping propertyMapping;
			if (inferenceLogTable.MailboxPartitionNumber != null)
			{
				column2 = Factory.CreateMappedPropertyColumn(inferenceLogTable.MailboxPartitionNumber, PropTag.InferenceLog.MailboxPartitionNumber);
				propertyMapping = new PhysicalColumnPropertyMapping(PropTag.InferenceLog.MailboxPartitionNumber, column2, null, null, null, (PhysicalColumn)column2.ActualColumn, false, true, true, false, false);
			}
			else
			{
				column2 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.InferenceLog.MailboxPartitionNumber, null), PropTag.InferenceLog.MailboxPartitionNumber);
				propertyMapping = new ConstantPropertyMapping(PropTag.InferenceLog.MailboxPartitionNumber, column2, null, null, true, true, false);
			}
			dictionary.Add(PropTag.InferenceLog.MailboxPartitionNumber, propertyMapping);
			Column column3;
			PropertyMapping propertyMapping2;
			if (inferenceLogTable.MailboxNumber != null)
			{
				column3 = Factory.CreateMappedPropertyColumn(inferenceLogTable.MailboxNumber, PropTag.InferenceLog.MailboxNumberInternal);
				propertyMapping2 = new PhysicalColumnPropertyMapping(PropTag.InferenceLog.MailboxNumberInternal, column3, null, null, null, (PhysicalColumn)column3.ActualColumn, false, true, true, false, false);
			}
			else
			{
				column3 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.InferenceLog.MailboxNumberInternal, null), PropTag.InferenceLog.MailboxNumberInternal);
				propertyMapping2 = new ConstantPropertyMapping(PropTag.InferenceLog.MailboxNumberInternal, column3, null, null, true, true, false);
			}
			dictionary.Add(PropTag.InferenceLog.MailboxNumberInternal, propertyMapping2);
			Column column4 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructFunctionColumn(PropTag.InferenceLog.MailboxNum, typeof(int), 4, 0, inferenceLogTable.Table, new Func<object[], object>(PropertySchemaPopulation.ComputeMailboxNumber), "Exchange.ComputeMailboxNumber", new Column[]
			{
				column2,
				column3
			}), PropTag.InferenceLog.MailboxNum);
			FunctionPropertyMapping value2 = new FunctionPropertyMapping(PropTag.InferenceLog.MailboxNum, column4, null, new Func<object[], object>(PropertySchemaPopulation.ComputeMailboxNumber), new PropertyMapping[]
			{
				propertyMapping,
				propertyMapping2
			}, true, true, false);
			dictionary.Add(PropTag.InferenceLog.MailboxNum, value2);
			PropertyMapping value3;
			if (inferenceLogTable.CreateTime != null)
			{
				Column column5 = Factory.CreateMappedPropertyColumn(inferenceLogTable.CreateTime, PropTag.InferenceLog.InferenceTimeStamp);
				value3 = new PhysicalColumnPropertyMapping(PropTag.InferenceLog.InferenceTimeStamp, column5, null, null, null, (PhysicalColumn)column5.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column5 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.InferenceLog.InferenceTimeStamp, null), PropTag.InferenceLog.InferenceTimeStamp);
				value3 = new ConstantPropertyMapping(PropTag.InferenceLog.InferenceTimeStamp, column5, null, null, true, true, true);
			}
			dictionary.Add(PropTag.InferenceLog.InferenceTimeStamp, value3);
			objectPropertySchema.Initialize(ObjectType.InferenceLog, table, dictionary, rowPropBagCreator, baseSchema);
			return objectPropertySchema;
		}

		public static ObjectPropertySchema GenerateUserInfoPropertySchema(StoreDatabase database)
		{
			Dictionary<StorePropTag, PropertyMapping> dictionary = new Dictionary<StorePropTag, PropertyMapping>(200);
			UserInfoTable userInfoTable = DatabaseSchema.UserInfoTable(database);
			if (userInfoTable == null)
			{
				return null;
			}
			Table table = userInfoTable.Table;
			ObjectPropertySchema objectPropertySchema = new ObjectPropertySchema();
			ObjectPropertySchema baseSchema = null;
			Func<IRowAccess, IRowPropertyBag> rowPropBagCreator = (IRowAccess rowAccess) => new RowPropertyBag(table, objectPropertySchema, StorePropTag.Invalid, rowAccess);
			PropertyMapping value;
			if (userInfoTable.UserGuid != null)
			{
				Column column = Factory.CreateMappedPropertyColumn(userInfoTable.UserGuid, PropTag.UserInfo.UserInformationGuid);
				value = new PhysicalColumnPropertyMapping(PropTag.UserInfo.UserInformationGuid, column, null, null, null, (PhysicalColumn)column.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.UserInfo.UserInformationGuid, null), PropTag.UserInfo.UserInformationGuid);
				value = new ConstantPropertyMapping(PropTag.UserInfo.UserInformationGuid, column, null, null, true, true, true);
			}
			dictionary.Add(PropTag.UserInfo.UserInformationGuid, value);
			PropertyMapping value2;
			if (userInfoTable.CreationTime != null)
			{
				Column column2 = Factory.CreateMappedPropertyColumn(userInfoTable.CreationTime, PropTag.UserInfo.UserInformationCreationTime);
				value2 = new PhysicalColumnPropertyMapping(PropTag.UserInfo.UserInformationCreationTime, column2, null, null, null, (PhysicalColumn)column2.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column2 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.UserInfo.UserInformationCreationTime, null), PropTag.UserInfo.UserInformationCreationTime);
				value2 = new ConstantPropertyMapping(PropTag.UserInfo.UserInformationCreationTime, column2, null, null, true, true, true);
			}
			dictionary.Add(PropTag.UserInfo.UserInformationCreationTime, value2);
			PropertyMapping value3;
			if (userInfoTable.LastModificationTime != null)
			{
				Column column3 = Factory.CreateMappedPropertyColumn(userInfoTable.LastModificationTime, PropTag.UserInfo.UserInformationLastModificationTime);
				value3 = new PhysicalColumnPropertyMapping(PropTag.UserInfo.UserInformationLastModificationTime, column3, null, null, null, (PhysicalColumn)column3.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column3 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.UserInfo.UserInformationLastModificationTime, null), PropTag.UserInfo.UserInformationLastModificationTime);
				value3 = new ConstantPropertyMapping(PropTag.UserInfo.UserInformationLastModificationTime, column3, null, null, true, true, true);
			}
			dictionary.Add(PropTag.UserInfo.UserInformationLastModificationTime, value3);
			PropertyMapping value4;
			if (userInfoTable.ChangeNumber != null)
			{
				Column column4 = Factory.CreateMappedPropertyColumn(userInfoTable.ChangeNumber, PropTag.UserInfo.UserInformationChangeNumber);
				value4 = new PhysicalColumnPropertyMapping(PropTag.UserInfo.UserInformationChangeNumber, column4, null, null, null, (PhysicalColumn)column4.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column4 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.UserInfo.UserInformationChangeNumber, null), PropTag.UserInfo.UserInformationChangeNumber);
				value4 = new ConstantPropertyMapping(PropTag.UserInfo.UserInformationChangeNumber, column4, null, null, true, true, true);
			}
			dictionary.Add(PropTag.UserInfo.UserInformationChangeNumber, value4);
			PropertyMapping value5;
			if (userInfoTable.LastInteractiveLogonTime != null)
			{
				Column column5 = Factory.CreateMappedPropertyColumn(userInfoTable.LastInteractiveLogonTime, PropTag.UserInfo.UserInformationLastInteractiveLogonTime);
				value5 = new PhysicalColumnPropertyMapping(PropTag.UserInfo.UserInformationLastInteractiveLogonTime, column5, null, null, null, (PhysicalColumn)column5.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column5 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.UserInfo.UserInformationLastInteractiveLogonTime, null), PropTag.UserInfo.UserInformationLastInteractiveLogonTime);
				value5 = new ConstantPropertyMapping(PropTag.UserInfo.UserInformationLastInteractiveLogonTime, column5, null, null, true, true, true);
			}
			dictionary.Add(PropTag.UserInfo.UserInformationLastInteractiveLogonTime, value5);
			objectPropertySchema.Initialize(ObjectType.UserInfo, table, dictionary, rowPropBagCreator, baseSchema);
			return objectPropertySchema;
		}

		private static object ConvertExchangeIdToInt64(object inputObject)
		{
			object result = null;
			if (inputObject != null)
			{
				byte[] bytes = (byte[])inputObject;
				long num = ExchangeIdHelpers.Convert26ByteToLong(bytes);
				result = num;
			}
			return result;
		}

		private static object ConvertExchangeIdTo22ByteForm(object inputObject)
		{
			object result = null;
			if (inputObject != null)
			{
				byte[] bytes = (byte[])inputObject;
				byte[] array = ExchangeIdHelpers.Convert26ByteTo22Byte(bytes);
				result = array;
			}
			return result;
		}

		private static object ConvertExchangeIdTo24ByteForm(object inputObject)
		{
			object result = null;
			if (inputObject != null)
			{
				byte[] bytes = (byte[])inputObject;
				byte[] array = ExchangeIdHelpers.Convert26ByteTo24Byte(bytes);
				result = array;
			}
			return result;
		}

		private static object ConvertInt64ToFolderSvrEid(object inputObject)
		{
			object result = null;
			if (inputObject != null)
			{
				long fid = (long)inputObject;
				byte[] array = ExchangeIdHelpers.BuildOursServerEntryId(fid, 0L, 0);
				result = array;
			}
			return result;
		}

		private static object ConvertExchangeIdToFolderSvrEid(object inputObject)
		{
			object result = null;
			if (inputObject != null)
			{
				byte[] fid = (byte[])inputObject;
				byte[] array = ExchangeIdHelpers.Convert26ByteToFolderSvrEid(fid);
				result = array;
			}
			return result;
		}

		private static object ConvertInt64To9ByteForm(object inputObject)
		{
			object result = null;
			if (inputObject != null)
			{
				long shortTermId = (long)inputObject;
				byte[] array = ExchangeIdHelpers.ConvertLongTo9Byte(shortTermId);
				result = array;
			}
			return result;
		}

		private static object TruncateBinaryToFitInIndex(object inputObject)
		{
			object result = null;
			if (inputObject != null)
			{
				byte[] input = (byte[])inputObject;
				byte[] array = ValueHelper.TruncateBinaryValue(input, 255);
				result = array;
			}
			return result;
		}

		private static object TruncateStringToFitInIndex(object inputObject)
		{
			object result = null;
			if (inputObject != null)
			{
				string input = (string)inputObject;
				string text = ValueHelper.TruncateStringValue(input, 127);
				result = text;
			}
			return result;
		}

		private static object ConvertInt64ToInt32(object inputObject)
		{
			object result = null;
			if (inputObject != null)
			{
				long num = (long)inputObject;
				int num2 = (int)num;
				result = num2;
			}
			return result;
		}

		private static object ConvertInt64ToBooleanNotZero(object inputObject)
		{
			object result = null;
			if (inputObject != null)
			{
				long num = (long)inputObject;
				bool flag = num != 0L;
				result = flag;
			}
			return result;
		}

		private static object ConvertInt32ToBooleanNotZero(object inputObject)
		{
			object result = null;
			if (inputObject != null)
			{
				int num = (int)inputObject;
				bool flag = num != 0;
				result = flag;
			}
			return result;
		}

		private static object ConvertGuidToBinary(object inputObject)
		{
			object result = null;
			if (inputObject != null)
			{
				byte[] array = ((Guid)inputObject).ToByteArray();
				result = array;
			}
			return result;
		}

		private static object ConvertLXCNArrayToLTIDArray(object inputObject)
		{
			object result = null;
			if (inputObject != null)
			{
				byte[] array = (byte[])inputObject;
				PCL pcl = new PCL(0);
				if (array != null)
				{
					pcl.LoadBinaryLXCN(array);
				}
				byte[] array2 = pcl.DumpBinaryLTID();
				result = array2;
			}
			return result;
		}

		private static object ComputeMailboxNumber(object[] arguments)
		{
			int num = 0;
			object obj = arguments[num++];
			object obj2 = arguments[num++];
			object result;
			if (obj2 != null)
			{
				result = obj2;
			}
			else
			{
				result = obj;
			}
			return result;
		}

		private static object ComputeMessageInstanceKey(object[] arguments)
		{
			int num = 0;
			object obj = arguments[num++];
			object obj2 = arguments[num++];
			object obj3 = arguments[num++];
			object result;
			if (obj2 == null || obj == null)
			{
				result = null;
			}
			else
			{
				result = ExchangeIdHelpers.BuildOursServerEntryId((long)obj, (long)obj2, (int)obj3);
			}
			return result;
		}

		private static object ComputeMessageEntryId(object[] arguments)
		{
			int num = 0;
			object obj = arguments[num++];
			object obj2 = arguments[num++];
			object result;
			if (obj2 == null || obj == null)
			{
				result = null;
			}
			else
			{
				result = ExchangeIdHelpers.BuildOursServerEntryId((long)obj, (long)obj2, 0);
			}
			return result;
		}

		public static object GetMessageInstanceNum(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			object obj = null;
			if (bag is IInstanceNumberOverride)
			{
				obj = ((IInstanceNumberOverride)bag).GetInstanceNumberOverride();
			}
			if (obj == null)
			{
				obj = 0;
			}
			return obj;
		}

		public static object GetMessageSourceKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Message.InternalSourceKey);
			if (array == null)
			{
				byte[] array2 = (byte[])bag.GetPropertyValue(context, PropTag.Message.MidBin);
				array = ((array2 == null) ? null : ExchangeIdHelpers.Convert26ByteTo22Byte(array2));
			}
			return array;
		}

		public static object GetMessageChangeKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Message.InternalChangeKey);
			if (array == null)
			{
				byte[] array2 = (byte[])bag.GetPropertyValue(context, PropTag.Message.ChangeNumberBin);
				array = ((array2 == null) ? null : ExchangeIdHelpers.Convert26ByteTo22Byte(array2));
			}
			return array;
		}

		public static ErrorCode SetMessageCnExport(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode result = ErrorCode.NoError;
			if (value == null)
			{
				result = ErrorCode.CreateNoAccess((LID)37656U);
			}
			else
			{
				byte[] array = (byte[])value;
				if (array.Length != 24)
				{
					result = ErrorCode.CreateInvalidParameter((LID)54040U);
				}
				else
				{
					ExchangeId exchangeId = ExchangeId.CreateFrom24ByteArray(context, bag.ReplidGuidMap, array);
					result = bag.SetProperty(context, PropTag.Message.ChangeNumberBin, exchangeId.To26ByteArray()).Propagate((LID)30396U);
				}
			}
			return result;
		}

		public static ErrorCode SetMessageCnMvExport(Context context, ISimplePropertyBag bag, object value)
		{
			return ErrorCode.NoError;
		}

		public static ErrorCode SetMessageReadCnNewExport(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode result = ErrorCode.NoError;
			if (value == null)
			{
				result = ErrorCode.CreateNoAccess((LID)41752U);
			}
			else
			{
				byte[] array = (byte[])value;
				if (array.Length != 24)
				{
					result = ErrorCode.CreateInvalidParameter((LID)58136U);
				}
				else
				{
					ExchangeId exchangeId = ExchangeId.CreateFrom24ByteArray(context, bag.ReplidGuidMap, array);
					result = bag.SetProperty(context, PropTag.Message.ReadCnNewBin, exchangeId.To26ByteArray()).Propagate((LID)30612U);
				}
			}
			return result;
		}

		public static object GetMessageReadReceiptRequested(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			bool flag = PropertyBagHelpers.TestPropertyFlags(context, bag, PropTag.Message.MailFlags, 2, 2);
			return flag;
		}

		public static ErrorCode SetMessageReadReceiptRequested(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			MailFlags mailFlags = MailFlags.NeedsReadNotification;
			if (value != null && (bool)value)
			{
				mailFlags |= MailFlags.NeedsNotReadNotification;
			}
			PropertyBagHelpers.SetPropertyFlags(context, bag, PropTag.Message.MailFlags, value, (short)mailFlags);
			return noError;
		}

		public static object GetMessageNonReceiptNotificationRequested(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			bool flag = PropertyBagHelpers.TestPropertyFlags(context, bag, PropTag.Message.MailFlags, 4, 4);
			return flag;
		}

		public static ErrorCode SetMessageNonReceiptNotificationRequested(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			PropertyBagHelpers.SetPropertyFlags(context, bag, PropTag.Message.MailFlags, value, 4);
			return noError;
		}

		public static ErrorCode SetMessageSubjectPrefix(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			PhysicalColumn subjectPrefix = DatabaseSchema.MessageTable(context.Database).SubjectPrefix;
			int num;
			if (subjectPrefix.TryGetColumnMaxSize(out num) && value != null && ((string)value).Length > num)
			{
				value = ((string)value).Substring(0, num);
			}
			bag.SetPhysicalColumn(context, subjectPrefix, value);
			return noError;
		}

		public static ErrorCode SetMessageNormalizedSubject(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			if (value != null && ((string)value).Length > 1024)
			{
				value = ((string)value).Substring(0, 1024);
			}
			bag.SetBlobProperty(context, PropTag.Message.NormalizedSubject, value);
			return noError;
		}

		public static object GetMessageSubject(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			string text = (string)bag.GetPropertyValue(context, PropTag.Message.SubjectPrefix);
			string text2 = (string)bag.GetPropertyValue(context, PropTag.Message.NormalizedSubject);
			if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
			{
				return null;
			}
			return (string.IsNullOrEmpty(text) ? "" : text) + (string.IsNullOrEmpty(text2) ? "" : text2);
		}

		public static ErrorCode SetMessageSubject(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			string text = (string)value;
			if (string.IsNullOrEmpty(text))
			{
				bag.SetProperty(context, PropTag.Message.SubjectPrefix, null);
				bag.SetProperty(context, PropTag.Message.NormalizedSubject, null);
			}
			else
			{
				int num = Math.Min(text.Length, 4);
				int num2 = -1;
				for (int i = 0; i < num; i++)
				{
					if (text[i] == ':' && i + 1 < text.Length && text[i + 1] == ' ')
					{
						num2 = i + 1;
						break;
					}
					if (!char.IsLetter(text[i]))
					{
						break;
					}
				}
				string value2;
				string value3;
				if (num2 > 0)
				{
					value2 = text.Substring(0, num2 + 1);
					value3 = text.Substring(num2 + 1);
				}
				else
				{
					value2 = null;
					value3 = text;
				}
				bag.SetProperty(context, PropTag.Message.SubjectPrefix, value2);
				bag.SetProperty(context, PropTag.Message.NormalizedSubject, value3);
			}
			return noError;
		}

		public static object GetMessageConversationTopicHash(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			string text = (string)bag.GetPropertyValue(context, PropTag.Message.ConversationTopic);
			if (text != null)
			{
				return HashHelpers.GetConversationTopicHash(text);
			}
			return null;
		}

		public static object GetMessageConversationIndex(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Message.ConversationItemConversationId);
			if (array != null)
			{
				return ConversationIdHelpers.GenerateConversationIndex(array);
			}
			return bag.GetPropertyValue(context, PropTag.Message.InternalConversationIndex);
		}

		public static ErrorCode SetMessageConversationIndex(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return bag.SetProperty(context, PropTag.Message.InternalConversationIndex, value);
		}

		public static object GetMessageConversationIndexTracking(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Message.ConversationItemConversationId);
			if (array != null)
			{
				return true;
			}
			return bag.GetPropertyValue(context, PropTag.Message.InternalConversationIndexTracking);
		}

		public static ErrorCode SetMessageConversationIndexTracking(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return bag.SetProperty(context, PropTag.Message.InternalConversationIndexTracking, value);
		}

		public static object GetMessageInternetMessageIdHash(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			string text = (string)bag.GetPropertyValue(context, PropTag.Message.InternetMessageId);
			if (text != null)
			{
				return HashHelpers.GetInternetMessageIdHash(text);
			}
			return null;
		}

		public static ErrorCode SetMessageIsReadColumn(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			if (value != null && (bool)value)
			{
				PropertyBagHelpers.SetPropertyFlags(context, bag, PropTag.Message.MessageFlagsActual, value, 1024);
			}
			return noError;
		}

		public static object GetMessageRead(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return bag.GetPropertyValue(context, PropTag.Message.VirtualIsRead);
		}

		public static ErrorCode SetMessageRead(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			TopMessage topMessage = bag as TopMessage;
			if (topMessage == null || !topMessage.ParentFolder.IsPerUserReadUnreadTrackingEnabled)
			{
				return bag.SetProperty(context, PropTag.Message.IsReadColumn, value).Propagate((LID)30480U);
			}
			if ((bool?)value != null && ((bool?)value).Value)
			{
				topMessage.AddToPerUser(context, !topMessage.IsNew && !topMessage.IsDirty);
				return ErrorCode.NoError;
			}
			topMessage.RemoveFromPerUser(context, !topMessage.IsNew && !topMessage.IsDirty);
			return ErrorCode.NoError;
		}

		public static object GetMessageSubmitFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			MessageFlags messageFlags = (MessageFlags)((int)bag.GetPropertyValue(context, PropTag.Message.MessageFlagsActual));
			if ((messageFlags & MessageFlags.Submit) == MessageFlags.None)
			{
				return null;
			}
			object propertyValue = bag.GetPropertyValue(context, PropTag.Message.SubmitResponsibility);
			SubmissionResponsibility submissionResponsibility = (propertyValue == null) ? SubmissionResponsibility.None : ((SubmissionResponsibility)((int)propertyValue));
			bool flag = (submissionResponsibility & SubmissionResponsibility.PreProcessingDone) == SubmissionResponsibility.PreProcessingDone;
			bool flag2 = (submissionResponsibility & SubmissionResponsibility.MdbDone) == SubmissionResponsibility.MdbDone;
			SubmissionState submissionState = SubmissionState.None;
			if (flag)
			{
				if (!flag2)
				{
					submissionState = SubmissionState.Locked;
				}
			}
			else
			{
				submissionState = SubmissionState.PreProcessing;
			}
			return (int)submissionState;
		}

		public static object GetMessageMessageFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			MessageFlags messageFlags = (MessageFlags)((int)bag.GetPropertyValue(context, PropTag.Message.MessageFlagsActual));
			if ((bool)bag.GetPropertyValue(context, PropTag.Message.HasAttach))
			{
				messageFlags |= MessageFlags.HasAttachment;
			}
			if ((bool)bag.GetPropertyValue(context, PropTag.Message.Associated))
			{
				messageFlags |= MessageFlags.Associated;
			}
			if ((bool)bag.GetPropertyValue(context, PropTag.Message.Read))
			{
				messageFlags |= MessageFlags.Read;
			}
			if (PropertyBagHelpers.TestPropertyFlags(context, bag, PropTag.Message.MailFlags, 34, 2))
			{
				messageFlags |= MessageFlags.ReadNotificationPending;
			}
			if (PropertyBagHelpers.TestPropertyFlags(context, bag, PropTag.Message.MailFlags, 36, 4))
			{
				messageFlags |= MessageFlags.NonReadNotificationPending;
			}
			return (int)messageFlags;
		}

		public static ErrorCode SetMessageMessageFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode result = ErrorCode.NoError;
			if (value == null)
			{
				result = ErrorCode.CreateNoAccess((LID)33560U);
			}
			else
			{
				MessageFlags messageFlags = (MessageFlags)((int)value);
				bag.SetProperty(context, PropTag.Message.Read, (messageFlags & MessageFlags.Read) != MessageFlags.None);
				messageFlags &= ~(MessageFlags.Read | MessageFlags.HasAttachment | MessageFlags.Associated | MessageFlags.ReadNotificationPending | MessageFlags.NonReadNotificationPending);
				if (!((Message)bag).Mailbox.GetCreatedByMove(context))
				{
					messageFlags &= ~MessageFlags.EverRead;
					object propertyValue = bag.GetPropertyValue(context, PropTag.Message.MessageFlagsActual);
					MessageFlags messageFlags2 = (MessageFlags)((propertyValue == null) ? 0 : ((int)propertyValue));
					messageFlags |= (messageFlags2 & MessageFlags.EverRead);
				}
				bag.SetProperty(context, PropTag.Message.MessageFlagsActual, (int)messageFlags);
			}
			return result;
		}

		public static object GetMessageIsIRMMessage(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			MessageFlags messageFlags = (MessageFlags)((int)bag.GetPropertyValue(context, PropTag.Message.MessageFlagsActual));
			if ((messageFlags & MessageFlags.Irm) == MessageFlags.None)
			{
				return SerializedValue.BoxedFalse;
			}
			return SerializedValue.BoxedTrue;
		}

		public static object GetMessageConversationId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Message.ConversationItemConversationId);
			if (array != null)
			{
				return array;
			}
			bool messageIsAssociated = (bool)bag.GetPropertyValue(context, PropTag.Message.Associated);
			byte[] conversationIndex = (byte[])bag.GetPropertyValue(context, PropTag.Message.InternalConversationIndex);
			int? documentId = (int?)bag.GetPropertyValue(context, PropTag.Message.DocumentId);
			object propertyValue = bag.GetPropertyValue(context, PropTag.Message.InternalConversationIndexTracking);
			bool? conversationIndexTracking;
			if (propertyValue == null)
			{
				conversationIndexTracking = null;
			}
			else
			{
				conversationIndexTracking = new bool?((bool)propertyValue);
			}
			string conversationTopic = (string)bag.GetPropertyValue(context, PropTag.Message.ConversationTopic);
			string messageClass = (string)bag.GetPropertyValue(context, PropTag.Message.MessageClass);
			return ConversationIdHelpers.FabricateConversationId(messageIsAssociated, conversationIndex, conversationIndexTracking, conversationTopic, messageClass, documentId);
		}

		public static object GetMessageConversationFamilyId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return (byte[])bag.GetPropertyValue(context, PropTag.Message.ConversationId);
		}

		public static ErrorCode SetMessageConversationFamilyId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			byte[] y = (byte[])bag.GetPropertyValue(context, PropTag.Message.ConversationId);
			if (ValueHelper.ArraysEqual<byte>((byte[])value, y))
			{
				bag.SetBlobProperty(context, PropTag.Message.ConversationFamilyId, null);
			}
			else
			{
				bag.SetBlobProperty(context, PropTag.Message.ConversationFamilyId, value);
			}
			return noError;
		}

		public static object GetMessageConversationIdHash(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Message.ConversationId);
			if (array != null)
			{
				return HashHelpers.GetConversationIdHash(array);
			}
			return null;
		}

		public static object GetMessageHasNamedProperties(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			bool result = false;
			bag.EnumerateBlobProperties(context, delegate(StorePropTag propTag, object value)
			{
				if (propTag.IsNamedProperty)
				{
					result = true;
					return false;
				}
				return true;
			}, false);
			return result;
		}

		public static object GetMessageRowType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return null;
		}

		public static object GetMessageNativeBodyInfo(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			short? num = (short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType);
			short? num2 = num;
			int? num3 = (num2 != null) ? new int?((int)num2.GetValueOrDefault()) : null;
			if (num3 == null)
			{
				return 0;
			}
			return (int)num.Value;
		}

		public static object GetMessageBodyUnicode(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			if (!((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 1))
			{
				return null;
			}
			object propertyValue = bag.GetPropertyValue(context, PropTag.Message.NativeBody);
			byte[] array = propertyValue as byte[];
			if (array != null)
			{
				return Encoding.Unicode.GetString(array, 0, array.Length);
			}
			return propertyValue;
		}

		public static ErrorCode SetMessageBodyUnicode(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			if (value != null)
			{
				string s = (string)value;
				byte[] bytes = Encoding.Unicode.GetBytes(s);
				bag.SetProperty(context, PropTag.Message.NativeBody, bytes);
				bag.SetProperty(context, PropTag.Message.NativeBodyType, 1);
			}
			else if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 1)
			{
				bag.SetProperty(context, PropTag.Message.NativeBody, null);
				bag.SetProperty(context, PropTag.Message.NativeBodyType, null);
			}
			return noError;
		}

		public static ErrorCode GetMessageBodyUnicodeReadStream(Context context, ISimplePropertyBag bag, out Stream stream)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			stream = null;
			if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 1)
			{
				errorCode = bag.OpenPropertyReadStream(context, PropTag.Message.NativeBody, out stream);
			}
			if (stream == null && errorCode == ErrorCode.NoError)
			{
				errorCode = ErrorCode.CreateNotFound((LID)51992U);
			}
			return errorCode;
		}

		public static ErrorCode GetMessageBodyUnicodeWriteStream(Context context, ISimplePropertyBag bag, out Stream stream)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			stream = null;
			bag.SetProperty(context, PropTag.Message.NativeBodyType, 1);
			errorCode = bag.OpenPropertyWriteStream(context, PropTag.Message.NativeBody, out stream);
			if (stream == null && errorCode == ErrorCode.NoError)
			{
				errorCode = ErrorCode.CreateNotFound((LID)62232U);
			}
			return errorCode;
		}

		public static object GetMessageRtfCompressed(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 2)
			{
				return bag.GetPropertyValue(context, PropTag.Message.NativeBody);
			}
			return null;
		}

		public static ErrorCode SetMessageRtfCompressed(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			if (value != null)
			{
				bag.SetProperty(context, PropTag.Message.NativeBody, value);
				bag.SetProperty(context, PropTag.Message.NativeBodyType, 2);
			}
			else if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 2)
			{
				bag.SetProperty(context, PropTag.Message.NativeBody, null);
				bag.SetProperty(context, PropTag.Message.NativeBodyType, null);
			}
			return noError;
		}

		public static ErrorCode GetMessageRtfCompressedReadStream(Context context, ISimplePropertyBag bag, out Stream stream)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			stream = null;
			if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 2)
			{
				errorCode = bag.OpenPropertyReadStream(context, PropTag.Message.NativeBody, out stream);
			}
			if (stream == null && errorCode == ErrorCode.NoError)
			{
				errorCode = ErrorCode.CreateNotFound((LID)51992U);
			}
			return errorCode;
		}

		public static ErrorCode GetMessageRtfCompressedWriteStream(Context context, ISimplePropertyBag bag, out Stream stream)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			stream = null;
			bag.SetProperty(context, PropTag.Message.NativeBodyType, 2);
			errorCode = bag.OpenPropertyWriteStream(context, PropTag.Message.NativeBody, out stream);
			if (stream == null && errorCode == ErrorCode.NoError)
			{
				errorCode = ErrorCode.CreateNotFound((LID)62232U);
			}
			return errorCode;
		}

		public static object GetMessageBodyHtml(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 3)
			{
				return bag.GetPropertyValue(context, PropTag.Message.NativeBody);
			}
			return null;
		}

		public static ErrorCode SetMessageBodyHtml(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			if (value != null)
			{
				bag.SetProperty(context, PropTag.Message.NativeBody, value);
				bag.SetProperty(context, PropTag.Message.NativeBodyType, 3);
			}
			else if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 3)
			{
				bag.SetProperty(context, PropTag.Message.NativeBody, null);
				bag.SetProperty(context, PropTag.Message.NativeBodyType, null);
			}
			return noError;
		}

		public static ErrorCode GetMessageBodyHtmlReadStream(Context context, ISimplePropertyBag bag, out Stream stream)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			stream = null;
			if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 3)
			{
				errorCode = bag.OpenPropertyReadStream(context, PropTag.Message.NativeBody, out stream);
			}
			if (stream == null && errorCode == ErrorCode.NoError)
			{
				errorCode = ErrorCode.CreateNotFound((LID)51992U);
			}
			return errorCode;
		}

		public static ErrorCode GetMessageBodyHtmlWriteStream(Context context, ISimplePropertyBag bag, out Stream stream)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			stream = null;
			bag.SetProperty(context, PropTag.Message.NativeBodyType, 3);
			errorCode = bag.OpenPropertyWriteStream(context, PropTag.Message.NativeBody, out stream);
			if (stream == null && errorCode == ErrorCode.NoError)
			{
				errorCode = ErrorCode.CreateNotFound((LID)62232U);
			}
			return errorCode;
		}

		public static object GetMessageRTFInSync(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			short? num = (short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType);
			if (num == 2)
			{
				return true;
			}
			if (num == 3)
			{
				return false;
			}
			return null;
		}

		public static object GetMessageDepth(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return null;
		}

		public static object GetMessageContentCount(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return null;
		}

		public static object GetMessageCategID(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return null;
		}

		public static object GetMessagePreviewUnread(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			if (!(bool)bag.GetPropertyValue(context, PropTag.Message.Read))
			{
				return bag.GetPropertyValue(context, PropTag.Message.Preview);
			}
			return null;
		}

		public static ErrorCode SetMessagePredecessorChangeList(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			PCL pcl = new PCL(0);
			if (value != null)
			{
				byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Message.PCL);
				if (array != null)
				{
					bool assertCondition = pcl.TryLoadBinaryLXCN(array);
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(assertCondition, "stored value of PCL is corrupt");
				}
				if (!pcl.TryLoadBinaryLXCN((byte[])value))
				{
					return ErrorCode.CreateInvalidParameter((LID)30024U);
				}
			}
			bag.SetProperty(context, PropTag.Message.PCL, pcl.DumpBinaryLXCN());
			return noError;
		}

		public static ErrorCode SetMessagePclExport(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			PCL pcl = new PCL(0);
			if (value != null)
			{
				byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Message.PCL);
				if (array != null)
				{
					bool assertCondition = pcl.TryLoadBinaryLXCN(array);
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(assertCondition, "stored value of PCL is corrupt");
				}
				if (!pcl.TryLoadBinaryLTID((byte[])value))
				{
					return ErrorCode.CreateInvalidParameter((LID)30248U);
				}
			}
			bag.SetProperty(context, PropTag.Message.PCL, pcl.DumpBinaryLXCN());
			return noError;
		}

		public static object GetMessageDeliveryOrRenewTime(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			DateTime? dateTime = (DateTime?)bag.GetPropertyValue(context, PropTag.Message.RenewTime);
			if (dateTime != null)
			{
				return dateTime.Value;
			}
			return bag.GetPropertyValue(context, PropTag.Message.MessageDeliveryTime);
		}

		public static object GetMessageToGroupExpansionRecipients(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return null;
		}

		public static object GetMessageCcGroupExpansionRecipients(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return null;
		}

		public static object GetMessageBccGroupExpansionRecipients(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return null;
		}

		public static object GetMessageParentDisplay(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return bag.GetPropertyValue(context, PropTag.Message.VirtualParentDisplay);
		}

		public static object GetMessageSentRepresentingFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetMessageSentRepresentingFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetMessageSentRepresentingEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetMessageSentRepresentingEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetMessageSentRepresentingAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetMessageSentRepresentingAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetMessageSentRepresentingEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetMessageSentRepresentingEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetMessageSentRepresentingSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetMessageSentRepresentingSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetMessageSentRepresentingSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetMessageSentRepresentingSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetMessageSentRepresentingName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetMessageSentRepresentingName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetMessageSentRepresentingOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetMessageSentRepresentingOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetMessageSentRepresentingOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetMessageSentRepresentingOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetMessageSentRepresentingSID(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetMessageSentRepresentingSID(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetMessageSentRepresentingGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetMessageSentRepresentingGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetMessageSenderFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetMessageSenderFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetMessageSenderEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetMessageSenderEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetMessageSenderAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetMessageSenderAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetMessageSenderEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetMessageSenderEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetMessageSenderSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetMessageSenderSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetMessageSenderSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetMessageSenderSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetMessageSenderName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetMessageSenderName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetMessageSenderOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetMessageSenderOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetMessageSenderOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetMessageSenderOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetMessageSenderSID(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetMessageSenderSID(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetMessageSenderGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetMessageSenderGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetMessageOriginalSentRepresentingFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetMessageOriginalSentRepresentingFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetMessageOriginalSentRepresentingEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetMessageOriginalSentRepresentingEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetMessageOriginalSentRepresentingAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetMessageOriginalSentRepresentingAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetMessageOriginalSentRepresentingEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetMessageOriginalSentRepresentingEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetMessageOriginalSentRepresentingSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetMessageOriginalSentRepresentingSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetMessageOriginalSentRepresentingSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetMessageOriginalSentRepresentingSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetMessageOriginalSentRepresentingName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetMessageOriginalSentRepresentingName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetMessageOriginalSentRepresentingOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetMessageOriginalSentRepresentingOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetMessageOriginalSentRepresentingOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetMessageOriginalSentRepresentingOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetMessageOriginalSentRepresentingSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetMessageOriginalSentRepresentingSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetMessageOriginalSentRepresentingGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetMessageOriginalSentRepresentingGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetMessageOriginalSenderFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetMessageOriginalSenderFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetMessageOriginalSenderEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetMessageOriginalSenderEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetMessageOriginalSenderAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetMessageOriginalSenderAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetMessageOriginalSenderEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetMessageOriginalSenderEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetMessageOriginalSenderSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetMessageOriginalSenderSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetMessageOriginalSenderSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetMessageOriginalSenderSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetMessageOriginalSenderName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetMessageOriginalSenderName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetMessageOriginalSenderOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetMessageOriginalSenderOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetMessageOriginalSenderOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetMessageOriginalSenderOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetMessageOriginalSenderSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetMessageOriginalSenderSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetMessageOriginalSenderGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetMessageOriginalSenderGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetMessageRcvdRepresentingFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetMessageRcvdRepresentingFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetMessageReceivedRepresentingEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetMessageReceivedRepresentingEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetMessageReceivedRepresentingAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetMessageReceivedRepresentingAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetMessageReceivedRepresentingEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetMessageReceivedRepresentingEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetMessageReceivedRepresentingSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetMessageReceivedRepresentingSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetMessageReceivedRepresentingSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetMessageReceivedRepresentingSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetMessageReceivedRepresentingName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetMessageReceivedRepresentingName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetMessageRcvdRepresentingOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetMessageRcvdRepresentingOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetMessageRcvdRepresentingOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetMessageRcvdRepresentingOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetMessageRcvdRepresentingSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetMessageRcvdRepresentingSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetMessageReceivedRepresentingGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetMessageReceivedRepresentingGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetMessageRcvdByFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetMessageRcvdByFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetMessageReceivedByEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetMessageReceivedByEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetMessageReceivedByAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetMessageReceivedByAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetMessageReceivedByEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetMessageReceivedByEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetMessageReceivedBySearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetMessageReceivedBySearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetMessageReceivedBySimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetMessageReceivedBySimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetMessageReceivedByName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetMessageReceivedByName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetMessageRcvdByOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetMessageRcvdByOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetMessageRcvdByOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetMessageRcvdByOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetMessageRcvdBySid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetMessageRcvdBySid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetMessageReceivedByGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetMessageReceivedByGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetMessageCreatorFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetMessageCreatorFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetMessageCreatorEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetMessageCreatorEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetMessageCreatorAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetMessageCreatorAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetMessageCreatorEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetMessageCreatorEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetMessageCreatorSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetMessageCreatorSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetMessageCreatorName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetMessageCreatorName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetMessageCreatorOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetMessageCreatorOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetMessageCreatorOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetMessageCreatorOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetMessageCreatorSID(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetMessageCreatorSID(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetMessageCreatorGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetMessageCreatorGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetMessageLastModifierFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetMessageLastModifierFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetMessageLastModifierEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetMessageLastModifierEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetMessageLastModifierAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetMessageLastModifierAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetMessageLastModifierEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetMessageLastModifierEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetMessageLastModifierSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetMessageLastModifierSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetMessageLastModifierName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetMessageLastModifierName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetMessageLastModifierOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetMessageLastModifierOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetMessageLastModifierOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetMessageLastModifierOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetMessageLastModifierSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetMessageLastModifierSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetMessageLastModifierGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetMessageLastModifierGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetMessageReadReceiptFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetMessageReadReceiptFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetMessageReadReceiptEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetMessageReadReceiptEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetMessageReadReceiptAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetMessageReadReceiptAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetMessageReadReceiptEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetMessageReadReceiptEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetMessageReadReceiptSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetMessageReadReceiptSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetMessageReadReceiptSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetMessageReadReceiptSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetMessageReadReceiptDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetMessageReadReceiptDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetMessageReadReceiptOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetMessageReadReceiptOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetMessageReadReceiptOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetMessageReadReceiptOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetMessageReadReceiptSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetMessageReadReceiptSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetMessageReadReceiptGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetMessageReadReceiptGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetMessageReportFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetMessageReportFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetMessageReportEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetMessageReportEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetMessageReportAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetMessageReportAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetMessageReportEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetMessageReportEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetMessageReportSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetMessageReportSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetMessageReportSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetMessageReportSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetMessageReportDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetMessageReportDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetMessageReportOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetMessageReportOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetMessageReportOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetMessageReportOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetMessageReportSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetMessageReportSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetMessageReportGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetMessageReportGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetMessageOriginatorFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginatorFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetMessageOriginatorEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginatorEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetMessageOriginatorAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginatorAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetMessageOriginatorEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginatorEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetMessageOriginatorSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginatorSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetMessageOriginatorSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginatorSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetMessageOriginatorName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginatorName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetMessageOriginatorOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginatorOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetMessageOriginatorOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginatorOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetMessageOriginatorSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginatorSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetMessageOriginatorGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginatorGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetMessageOriginalAuthorFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginalAuthorFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetMessageOriginalAuthorEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginalAuthorEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetMessageOriginalAuthorAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginalAuthorAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetMessageOriginalAuthorEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginalAuthorEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetMessageOriginalAuthorSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginalAuthorSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetMessageOriginalAuthorSimpleDispName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginalAuthorSimpleDispName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetMessageOriginalAuthorName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginalAuthorName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetMessageOriginalAuthorOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginalAuthorOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetMessageOriginalAuthorOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginalAuthorOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetMessageOriginalAuthorSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginalAuthorSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetMessageOriginalAuthorGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetMessageOriginalAuthorGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetMessageReportDestinationFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetMessageReportDestinationFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetMessageReportDestinationEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetMessageReportDestinationEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetMessageReportDestinationAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetMessageReportDestinationAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetMessageReportDestinationEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetMessageReportDestinationEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetMessageReportDestinationSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetMessageReportDestinationSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetMessageReportDestinationSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetMessageReportDestinationSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetMessageReportDestinationName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetMessageReportDestinationName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetMessageReportDestinationOrgEmailType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetMessageReportDestinationOrgEmailType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetMessageReportDestinationOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetMessageReportDestinationOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetMessageReportDestinationSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetMessageReportDestinationSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetMessageReportDestinationGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetMessageReportDestinationGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static ErrorCode InitializeMessage(Context context, ISimplePropertyBag bag)
		{
			bag.SetProperty(context, PropTag.Message.Importance, 1);
			bag.SetProperty(context, PropTag.Message.Priority, 0);
			bag.SetProperty(context, PropTag.Message.ReadReceiptRequested, false);
			bag.SetProperty(context, PropTag.Message.NonReceiptNotificationRequested, false);
			bag.SetProperty(context, PropTag.Message.Sensitivity, 0);
			PropertyBagHelpers.SetPropertyFlags(context, bag, PropTag.Message.MessageFlagsActual, true, 8);
			bag.SetProperty(context, PropTag.Message.DisplayBcc, string.Empty);
			bag.SetProperty(context, PropTag.Message.DisplayCc, string.Empty);
			bag.SetProperty(context, PropTag.Message.DisplayTo, string.Empty);
			return ErrorCode.NoError;
		}

		public static object GetEmbeddedMessageSourceKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Message.MidBin);
			if (array != null)
			{
				return ExchangeIdHelpers.Convert26ByteTo22Byte(array);
			}
			return null;
		}

		public static object GetEmbeddedMessageSubject(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			string text = (string)bag.GetPropertyValue(context, PropTag.Message.SubjectPrefix);
			string text2 = (string)bag.GetPropertyValue(context, PropTag.Message.NormalizedSubject);
			if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
			{
				return null;
			}
			return (string.IsNullOrEmpty(text) ? "" : text) + (string.IsNullOrEmpty(text2) ? "" : text2);
		}

		public static ErrorCode SetEmbeddedMessageSubject(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			string text = (string)value;
			if (string.IsNullOrEmpty(text))
			{
				bag.SetProperty(context, PropTag.Message.SubjectPrefix, null);
				bag.SetProperty(context, PropTag.Message.NormalizedSubject, null);
			}
			else
			{
				int num = Math.Min(text.Length, 4);
				int num2 = -1;
				for (int i = 0; i < num; i++)
				{
					if (text[i] == ':' && i + 1 < text.Length && text[i + 1] == ' ')
					{
						num2 = i + 1;
						break;
					}
					if (!char.IsLetter(text[i]))
					{
						break;
					}
				}
				string value2;
				string value3;
				if (num2 > 0)
				{
					value2 = text.Substring(0, num2 + 1);
					value3 = text.Substring(num2 + 1);
				}
				else
				{
					value2 = null;
					value3 = text;
				}
				bag.SetProperty(context, PropTag.Message.SubjectPrefix, value2);
				bag.SetProperty(context, PropTag.Message.NormalizedSubject, value3);
			}
			return noError;
		}

		public static object GetEmbeddedMessageReadReceiptRequested(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return PropertySchemaPopulation.GetMessageReadReceiptRequested(context, bag);
		}

		public static ErrorCode SetEmbeddedMessageReadReceiptRequested(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return PropertySchemaPopulation.SetMessageReadReceiptRequested(context, bag, value).Propagate((LID)29760U);
		}

		public static object GetEmbeddedMessageNonReceiptNotificationRequested(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return PropertySchemaPopulation.GetMessageNonReceiptNotificationRequested(context, bag);
		}

		public static ErrorCode SetEmbeddedMessageNonReceiptNotificationRequested(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return PropertySchemaPopulation.SetMessageNonReceiptNotificationRequested(context, bag, value).Propagate((LID)29764U);
		}

		public static object GetEmbeddedMessageMessageFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return PropertySchemaPopulation.GetMessageMessageFlags(context, bag);
		}

		public static ErrorCode SetEmbeddedMessageMessageFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return PropertySchemaPopulation.SetMessageMessageFlags(context, bag, value).Propagate((LID)49944U);
		}

		public static object GetEmbeddedMessageHasNamedProperties(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			bool result = false;
			bag.EnumerateBlobProperties(context, delegate(StorePropTag propTag, object value)
			{
				if (propTag.IsNamedProperty)
				{
					result = true;
					return false;
				}
				return true;
			}, false);
			return result;
		}

		public static object GetEmbeddedMessageNativeBodyInfo(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			short? num = (short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType);
			short? num2 = num;
			int? num3 = (num2 != null) ? new int?((int)num2.GetValueOrDefault()) : null;
			if (num3 == null)
			{
				return 0;
			}
			return (int)num.Value;
		}

		public static object GetEmbeddedMessageBodyUnicode(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			if (!((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 1))
			{
				return null;
			}
			object propertyValue = bag.GetPropertyValue(context, PropTag.Message.NativeBody);
			byte[] array = propertyValue as byte[];
			if (array != null)
			{
				return Encoding.Unicode.GetString(array, 0, array.Length);
			}
			return propertyValue;
		}

		public static ErrorCode SetEmbeddedMessageBodyUnicode(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			if (value != null)
			{
				string s = (string)value;
				byte[] bytes = Encoding.Unicode.GetBytes(s);
				bag.SetProperty(context, PropTag.Message.NativeBody, bytes);
				bag.SetProperty(context, PropTag.Message.NativeBodyType, 1);
			}
			else if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 1)
			{
				bag.SetProperty(context, PropTag.Message.NativeBody, null);
				bag.SetProperty(context, PropTag.Message.NativeBodyType, null);
			}
			return noError;
		}

		public static ErrorCode GetEmbeddedMessageBodyUnicodeReadStream(Context context, ISimplePropertyBag bag, out Stream stream)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			stream = null;
			if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 1)
			{
				errorCode = bag.OpenPropertyReadStream(context, PropTag.Message.NativeBody, out stream);
			}
			if (stream == null && errorCode == ErrorCode.NoError)
			{
				errorCode = ErrorCode.CreateNotFound((LID)51992U);
			}
			return errorCode;
		}

		public static ErrorCode GetEmbeddedMessageBodyUnicodeWriteStream(Context context, ISimplePropertyBag bag, out Stream stream)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			stream = null;
			bag.SetProperty(context, PropTag.Message.NativeBodyType, 1);
			errorCode = bag.OpenPropertyWriteStream(context, PropTag.Message.NativeBody, out stream);
			if (stream == null && errorCode == ErrorCode.NoError)
			{
				errorCode = ErrorCode.CreateNotFound((LID)62232U);
			}
			return errorCode;
		}

		public static object GetEmbeddedMessageRtfCompressed(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 2)
			{
				return bag.GetPropertyValue(context, PropTag.Message.NativeBody);
			}
			return null;
		}

		public static ErrorCode SetEmbeddedMessageRtfCompressed(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			if (value != null)
			{
				bag.SetProperty(context, PropTag.Message.NativeBody, value);
				bag.SetProperty(context, PropTag.Message.NativeBodyType, 2);
			}
			else if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 2)
			{
				bag.SetProperty(context, PropTag.Message.NativeBody, null);
				bag.SetProperty(context, PropTag.Message.NativeBodyType, null);
			}
			return noError;
		}

		public static ErrorCode GetEmbeddedMessageRtfCompressedReadStream(Context context, ISimplePropertyBag bag, out Stream stream)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			stream = null;
			if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 2)
			{
				errorCode = bag.OpenPropertyReadStream(context, PropTag.Message.NativeBody, out stream);
			}
			if (stream == null && errorCode == ErrorCode.NoError)
			{
				errorCode = ErrorCode.CreateNotFound((LID)51992U);
			}
			return errorCode;
		}

		public static ErrorCode GetEmbeddedMessageRtfCompressedWriteStream(Context context, ISimplePropertyBag bag, out Stream stream)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			stream = null;
			bag.SetProperty(context, PropTag.Message.NativeBodyType, 2);
			errorCode = bag.OpenPropertyWriteStream(context, PropTag.Message.NativeBody, out stream);
			if (stream == null && errorCode == ErrorCode.NoError)
			{
				errorCode = ErrorCode.CreateNotFound((LID)62232U);
			}
			return errorCode;
		}

		public static object GetEmbeddedMessageBodyHtml(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 3)
			{
				return bag.GetPropertyValue(context, PropTag.Message.NativeBody);
			}
			return null;
		}

		public static ErrorCode SetEmbeddedMessageBodyHtml(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			if (value != null)
			{
				bag.SetProperty(context, PropTag.Message.NativeBody, value);
				bag.SetProperty(context, PropTag.Message.NativeBodyType, 3);
			}
			else if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 3)
			{
				bag.SetProperty(context, PropTag.Message.NativeBody, null);
				bag.SetProperty(context, PropTag.Message.NativeBodyType, null);
			}
			return noError;
		}

		public static ErrorCode GetEmbeddedMessageBodyHtmlReadStream(Context context, ISimplePropertyBag bag, out Stream stream)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			stream = null;
			if ((short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType) == 3)
			{
				errorCode = bag.OpenPropertyReadStream(context, PropTag.Message.NativeBody, out stream);
			}
			if (stream == null && errorCode == ErrorCode.NoError)
			{
				errorCode = ErrorCode.CreateNotFound((LID)51992U);
			}
			return errorCode;
		}

		public static ErrorCode GetEmbeddedMessageBodyHtmlWriteStream(Context context, ISimplePropertyBag bag, out Stream stream)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			stream = null;
			bag.SetProperty(context, PropTag.Message.NativeBodyType, 3);
			errorCode = bag.OpenPropertyWriteStream(context, PropTag.Message.NativeBody, out stream);
			if (stream == null && errorCode == ErrorCode.NoError)
			{
				errorCode = ErrorCode.CreateNotFound((LID)62232U);
			}
			return errorCode;
		}

		public static object GetEmbeddedMessageRTFInSync(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			short? num = (short?)bag.GetPropertyValue(context, PropTag.Message.NativeBodyType);
			if (num == 2)
			{
				return true;
			}
			if (num == 3)
			{
				return false;
			}
			return null;
		}

		public static object GetEmbeddedMessagePreviewUnread(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			if (!(bool)bag.GetPropertyValue(context, PropTag.Message.Read))
			{
				return bag.GetPropertyValue(context, PropTag.Message.Preview);
			}
			return null;
		}

		public static object GetEmbeddedMessageSentRepresentingFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageSentRepresentingFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetEmbeddedMessageSentRepresentingEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageSentRepresentingEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetEmbeddedMessageSentRepresentingAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageSentRepresentingAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetEmbeddedMessageSentRepresentingEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageSentRepresentingEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetEmbeddedMessageSentRepresentingSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageSentRepresentingSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetEmbeddedMessageSentRepresentingSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageSentRepresentingSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetEmbeddedMessageSentRepresentingName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageSentRepresentingName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetEmbeddedMessageSentRepresentingOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageSentRepresentingOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetEmbeddedMessageSentRepresentingOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageSentRepresentingOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetEmbeddedMessageSentRepresentingSID(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageSentRepresentingSID(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetEmbeddedMessageSentRepresentingGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.SentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageSentRepresentingGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.SentRepresenting, value);
		}

		public static object GetEmbeddedMessageSenderFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageSenderFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetEmbeddedMessageSenderEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageSenderEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetEmbeddedMessageSenderAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageSenderAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetEmbeddedMessageSenderEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageSenderEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetEmbeddedMessageSenderSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageSenderSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetEmbeddedMessageSenderSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageSenderSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetEmbeddedMessageSenderName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageSenderName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetEmbeddedMessageSenderOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageSenderOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetEmbeddedMessageSenderOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageSenderOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetEmbeddedMessageSenderSID(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageSenderSID(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetEmbeddedMessageSenderGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.Sender, AddressInfoTags.SentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageSenderGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.Sender, value);
		}

		public static object GetEmbeddedMessageOriginalSentRepresentingFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSentRepresentingFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetEmbeddedMessageOriginalSentRepresentingEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSentRepresentingEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetEmbeddedMessageOriginalSentRepresentingAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSentRepresentingAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetEmbeddedMessageOriginalSentRepresentingEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSentRepresentingEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetEmbeddedMessageOriginalSentRepresentingSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSentRepresentingSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetEmbeddedMessageOriginalSentRepresentingSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSentRepresentingSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetEmbeddedMessageOriginalSentRepresentingName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSentRepresentingName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetEmbeddedMessageOriginalSentRepresentingOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSentRepresentingOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetEmbeddedMessageOriginalSentRepresentingOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSentRepresentingOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetEmbeddedMessageOriginalSentRepresentingSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSentRepresentingSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetEmbeddedMessageOriginalSentRepresentingGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.OriginalSentRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSentRepresentingGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.OriginalSentRepresenting, value);
		}

		public static object GetEmbeddedMessageOriginalSenderFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSenderFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetEmbeddedMessageOriginalSenderEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSenderEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetEmbeddedMessageOriginalSenderAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSenderAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetEmbeddedMessageOriginalSenderEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSenderEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetEmbeddedMessageOriginalSenderSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSenderSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetEmbeddedMessageOriginalSenderSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSenderSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetEmbeddedMessageOriginalSenderName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSenderName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetEmbeddedMessageOriginalSenderOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSenderOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetEmbeddedMessageOriginalSenderOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSenderOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetEmbeddedMessageOriginalSenderSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSenderSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetEmbeddedMessageOriginalSenderGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.OriginalSender, AddressInfoTags.OriginalSentRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageOriginalSenderGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.OriginalSender, value);
		}

		public static object GetEmbeddedMessageRcvdRepresentingFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageRcvdRepresentingFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetEmbeddedMessageReceivedRepresentingEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageReceivedRepresentingEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetEmbeddedMessageReceivedRepresentingAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageReceivedRepresentingAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetEmbeddedMessageReceivedRepresentingEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageReceivedRepresentingEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetEmbeddedMessageReceivedRepresentingSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageReceivedRepresentingSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetEmbeddedMessageReceivedRepresentingSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageReceivedRepresentingSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetEmbeddedMessageReceivedRepresentingName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageReceivedRepresentingName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetEmbeddedMessageRcvdRepresentingOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageRcvdRepresentingOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetEmbeddedMessageRcvdRepresentingOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageRcvdRepresentingOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetEmbeddedMessageRcvdRepresentingSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageRcvdRepresentingSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetEmbeddedMessageReceivedRepresentingGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.ReceivedRepresenting, null);
		}

		public static ErrorCode SetEmbeddedMessageReceivedRepresentingGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.ReceivedRepresenting, value);
		}

		public static object GetEmbeddedMessageRcvdByFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageRcvdByFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetEmbeddedMessageReceivedByEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageReceivedByEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetEmbeddedMessageReceivedByAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageReceivedByAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetEmbeddedMessageReceivedByEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageReceivedByEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetEmbeddedMessageReceivedBySearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageReceivedBySearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetEmbeddedMessageReceivedBySimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageReceivedBySimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetEmbeddedMessageReceivedByName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageReceivedByName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetEmbeddedMessageRcvdByOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageRcvdByOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetEmbeddedMessageRcvdByOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageRcvdByOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetEmbeddedMessageRcvdBySid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageRcvdBySid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetEmbeddedMessageReceivedByGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.ReceivedBy, AddressInfoTags.ReceivedRepresenting);
		}

		public static ErrorCode SetEmbeddedMessageReceivedByGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.ReceivedBy, value);
		}

		public static object GetEmbeddedMessageCreatorFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetEmbeddedMessageCreatorFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetEmbeddedMessageCreatorEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetEmbeddedMessageCreatorEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetEmbeddedMessageCreatorAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetEmbeddedMessageCreatorAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetEmbeddedMessageCreatorEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetEmbeddedMessageCreatorEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetEmbeddedMessageCreatorSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetEmbeddedMessageCreatorSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetEmbeddedMessageCreatorName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetEmbeddedMessageCreatorName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetEmbeddedMessageCreatorOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetEmbeddedMessageCreatorOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetEmbeddedMessageCreatorOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetEmbeddedMessageCreatorOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetEmbeddedMessageCreatorSID(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetEmbeddedMessageCreatorSID(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetEmbeddedMessageCreatorGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.Creator, null);
		}

		public static ErrorCode SetEmbeddedMessageCreatorGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.Creator, value);
		}

		public static object GetEmbeddedMessageLastModifierFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetEmbeddedMessageLastModifierFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetEmbeddedMessageLastModifierEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetEmbeddedMessageLastModifierEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetEmbeddedMessageLastModifierAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetEmbeddedMessageLastModifierAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetEmbeddedMessageLastModifierEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetEmbeddedMessageLastModifierEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetEmbeddedMessageLastModifierSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetEmbeddedMessageLastModifierSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetEmbeddedMessageLastModifierName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetEmbeddedMessageLastModifierName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetEmbeddedMessageLastModifierOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetEmbeddedMessageLastModifierOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetEmbeddedMessageLastModifierOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetEmbeddedMessageLastModifierOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetEmbeddedMessageLastModifierSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetEmbeddedMessageLastModifierSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetEmbeddedMessageLastModifierGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.LastModifier, AddressInfoTags.Creator);
		}

		public static ErrorCode SetEmbeddedMessageLastModifierGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.LastModifier, value);
		}

		public static object GetEmbeddedMessageReadReceiptFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetEmbeddedMessageReadReceiptFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetEmbeddedMessageReadReceiptEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetEmbeddedMessageReadReceiptEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetEmbeddedMessageReadReceiptAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetEmbeddedMessageReadReceiptAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetEmbeddedMessageReadReceiptEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetEmbeddedMessageReadReceiptEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetEmbeddedMessageReadReceiptSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetEmbeddedMessageReadReceiptSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetEmbeddedMessageReadReceiptSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetEmbeddedMessageReadReceiptSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetEmbeddedMessageReadReceiptDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetEmbeddedMessageReadReceiptDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetEmbeddedMessageReadReceiptOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetEmbeddedMessageReadReceiptOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetEmbeddedMessageReadReceiptOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetEmbeddedMessageReadReceiptOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetEmbeddedMessageReadReceiptSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetEmbeddedMessageReadReceiptSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetEmbeddedMessageReadReceiptGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.ReadReceipt, null);
		}

		public static ErrorCode SetEmbeddedMessageReadReceiptGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.ReadReceipt, value);
		}

		public static object GetEmbeddedMessageReportFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetEmbeddedMessageReportFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetEmbeddedMessageReportEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetEmbeddedMessageReportEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetEmbeddedMessageReportAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetEmbeddedMessageReportAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetEmbeddedMessageReportEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetEmbeddedMessageReportEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetEmbeddedMessageReportSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetEmbeddedMessageReportSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetEmbeddedMessageReportSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetEmbeddedMessageReportSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetEmbeddedMessageReportDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetEmbeddedMessageReportDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetEmbeddedMessageReportOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetEmbeddedMessageReportOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetEmbeddedMessageReportOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetEmbeddedMessageReportOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetEmbeddedMessageReportSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetEmbeddedMessageReportSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetEmbeddedMessageReportGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.Report, null);
		}

		public static ErrorCode SetEmbeddedMessageReportGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.Report, value);
		}

		public static object GetEmbeddedMessageOriginatorFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginatorFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetEmbeddedMessageOriginatorEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginatorEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetEmbeddedMessageOriginatorAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginatorAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetEmbeddedMessageOriginatorEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginatorEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetEmbeddedMessageOriginatorSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginatorSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetEmbeddedMessageOriginatorSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginatorSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetEmbeddedMessageOriginatorName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginatorName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetEmbeddedMessageOriginatorOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginatorOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetEmbeddedMessageOriginatorOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginatorOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetEmbeddedMessageOriginatorSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginatorSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetEmbeddedMessageOriginatorGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.Originator, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginatorGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.Originator, value);
		}

		public static object GetEmbeddedMessageOriginalAuthorFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginalAuthorFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetEmbeddedMessageOriginalAuthorEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginalAuthorEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetEmbeddedMessageOriginalAuthorAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginalAuthorAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetEmbeddedMessageOriginalAuthorEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginalAuthorEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetEmbeddedMessageOriginalAuthorSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginalAuthorSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetEmbeddedMessageOriginalAuthorSimpleDispName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginalAuthorSimpleDispName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetEmbeddedMessageOriginalAuthorName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginalAuthorName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetEmbeddedMessageOriginalAuthorOrgAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginalAuthorOrgAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetEmbeddedMessageOriginalAuthorOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginalAuthorOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetEmbeddedMessageOriginalAuthorSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginalAuthorSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetEmbeddedMessageOriginalAuthorGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.OriginalAuthor, AddressInfoTags.LastModifier);
		}

		public static ErrorCode SetEmbeddedMessageOriginalAuthorGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.OriginalAuthor, value);
		}

		public static object GetEmbeddedMessageReportDestinationFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetFlags(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetEmbeddedMessageReportDestinationFlags(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetFlags(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetEmbeddedMessageReportDestinationEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEntryId(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetEmbeddedMessageReportDestinationEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEntryId(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetEmbeddedMessageReportDestinationAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetAddressType(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetEmbeddedMessageReportDestinationAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetAddressType(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetEmbeddedMessageReportDestinationEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetEmailAddress(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetEmbeddedMessageReportDestinationEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetEmailAddress(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetEmbeddedMessageReportDestinationSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSearchKey(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetEmbeddedMessageReportDestinationSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSearchKey(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetEmbeddedMessageReportDestinationSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSimpleDisplayName(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetEmbeddedMessageReportDestinationSimpleDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSimpleDisplayName(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetEmbeddedMessageReportDestinationName(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetDisplayName(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetEmbeddedMessageReportDestinationName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetDisplayName(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetEmbeddedMessageReportDestinationOrgEmailType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalAddressType(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetEmbeddedMessageReportDestinationOrgEmailType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalAddressType(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetEmbeddedMessageReportDestinationOrgEmailAddr(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetOriginalEmailAddress(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetEmbeddedMessageReportDestinationOrgEmailAddr(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetOriginalEmailAddress(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetEmbeddedMessageReportDestinationSid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetSid(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetEmbeddedMessageReportDestinationSid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetSid(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static object GetEmbeddedMessageReportDestinationGuid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return AddressInfoGetter.GetGuid(context, bag, AddressInfoTags.ReportDestination, null);
		}

		public static ErrorCode SetEmbeddedMessageReportDestinationGuid(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return AddressInfoSetter.SetGuid(context, bag, AddressInfoTags.ReportDestination, value);
		}

		public static ErrorCode InitializeEmbeddedMessage(Context context, ISimplePropertyBag bag)
		{
			bag.SetProperty(context, PropTag.Message.SearchKey, Guid.NewGuid().ToByteArray());
			bag.SetProperty(context, PropTag.Message.MsgStatus, 0);
			bag.SetProperty(context, PropTag.Message.Read, false);
			bag.SetProperty(context, PropTag.Message.Associated, false);
			bag.SetProperty(context, PropTag.Message.HasAttach, false);
			bag.SetProperty(context, PropTag.Message.ReadReceiptRequested, false);
			bag.SetProperty(context, PropTag.Message.NonReceiptNotificationRequested, false);
			bag.SetProperty(context, PropTag.Message.MessageFlags, 8);
			bag.SetProperty(context, PropTag.Message.CreationTime, ((EmbeddedMessage)bag).Mailbox.UtcNow);
			bag.SetProperty(context, PropTag.Message.LastModificationTime, ((EmbeddedMessage)bag).Mailbox.UtcNow);
			return ErrorCode.NoError;
		}

		public static object GetFolderSourceKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Folder.InternalSourceKey);
			if (array == null)
			{
				byte[] array2 = (byte[])bag.GetPropertyValue(context, PropTag.Folder.FidBin);
				array = ((array2 == null) ? null : ExchangeIdHelpers.Convert26ByteTo22Byte(array2));
			}
			return array;
		}

		public static object GetFolderChangeKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Folder.InternalChangeKey);
			if (array == null)
			{
				byte[] array2 = (byte[])bag.GetPropertyValue(context, PropTag.Folder.ChangeNumberBin);
				array = ((array2 == null) ? null : ExchangeIdHelpers.Convert26ByteTo22Byte(array2));
			}
			return array;
		}

		public static ErrorCode SetFolderCnExport(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode result = ErrorCode.NoError;
			if (value == null)
			{
				result = ErrorCode.CreateNoAccess((LID)48152U);
			}
			else
			{
				byte[] array = (byte[])value;
				if (array.Length != 24)
				{
					result = ErrorCode.CreateInvalidParameter((LID)64536U);
				}
				else
				{
					ExchangeId exchangeId = ExchangeId.CreateFrom24ByteArray(context, bag.ReplidGuidMap, array);
					bag.SetProperty(context, PropTag.Folder.ChangeNumberBin, exchangeId.To26ByteArray());
				}
			}
			return result;
		}

		public static ErrorCode SetFolderCnMvExport(Context context, ISimplePropertyBag bag, object value)
		{
			return ErrorCode.NoError;
		}

		public static object GetFolderFolderType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Folder.ParentFidBin);
			if (array == null)
			{
				return 0;
			}
			if (ValueHelper.ArraysEqual<byte>(array, ExchangeId.Zero.To26ByteArray()))
			{
				return 0;
			}
			if (bag.GetPropertyValue(context, PropTag.Folder.QueryCriteriaInternal) != null)
			{
				return 2;
			}
			return 1;
		}

		public static object GetFolderFolderFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			FolderFlags folderFlags = (FolderFlags)0;
			object propertyValue = bag.GetPropertyValue(context, PropTag.Folder.IPMFolder);
			if (propertyValue != null && (bool)propertyValue)
			{
				folderFlags |= FolderFlags.Ipm;
			}
			object propertyValue2 = bag.GetPropertyValue(context, PropTag.Folder.FolderType);
			if (propertyValue2 != null)
			{
				FolderType folderType = (FolderType)((int)propertyValue2);
				if (folderType == FolderType.Search)
				{
					folderFlags |= FolderFlags.Search;
				}
				else if (folderType == FolderType.Normal)
				{
					folderFlags |= FolderFlags.Normal;
				}
			}
			object propertyValue3 = bag.GetPropertyValue(context, PropTag.Folder.HasRules);
			if (propertyValue3 != null && (bool)propertyValue3)
			{
				folderFlags |= FolderFlags.Rules;
			}
			return (int)folderFlags;
		}

		public static ErrorCode SetFolderDisablePerUserRead(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			Folder folder = bag as Folder;
			bool isPerUserReadUnreadTrackingEnabled = folder.IsPerUserReadUnreadTrackingEnabled;
			folder.SetBlobProperty(context, PropTag.Folder.DisablePerUserRead, value);
			folder.UpdatePerUserReadUnreadTrackingEnabled(context);
			if (folder.IsPerUserReadUnreadTrackingEnabled != isPerUserReadUnreadTrackingEnabled)
			{
				if (isPerUserReadUnreadTrackingEnabled)
				{
					PerUser.DeleteAllResidentEntriesForFolder(context, folder);
					folder.SetProperty(context, PropTag.Folder.CnsetIn, null);
					long num = folder.BuildUnreadCount(context);
					folder.SetColumn(context, folder.FolderTable.UnreadMessageCount, num);
				}
				else
				{
					folder.SetCnsetIn(context, folder.BuildCnsetIn(context));
				}
			}
			return noError;
		}

		public static ErrorCode SetFolderPredecessorChangeList(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			PCL pcl = new PCL(0);
			if (value != null)
			{
				byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Folder.PCL);
				if (array != null)
				{
					bool assertCondition = pcl.TryLoadBinaryLXCN(array);
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(assertCondition, "stored value of PCL is corrupt");
				}
				if (!pcl.TryLoadBinaryLXCN((byte[])value))
				{
					return ErrorCode.CreateInvalidParameter((LID)30024U);
				}
			}
			bag.SetProperty(context, PropTag.Folder.PCL, pcl.DumpBinaryLXCN());
			return noError;
		}

		public static ErrorCode SetFolderPclExport(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			PCL pcl = new PCL(0);
			if (value != null)
			{
				byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Folder.PCL);
				if (array != null)
				{
					bool assertCondition = pcl.TryLoadBinaryLXCN(array);
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(assertCondition, "stored value of PCL is corrupt");
				}
				if (!pcl.TryLoadBinaryLTID((byte[])value))
				{
					return ErrorCode.CreateInvalidParameter((LID)30248U);
				}
			}
			bag.SetProperty(context, PropTag.Folder.PCL, pcl.DumpBinaryLXCN());
			return noError;
		}

		public static object GetFolderHasNamedProperties(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			bool result = false;
			bag.EnumerateBlobProperties(context, delegate(StorePropTag propTag, object value)
			{
				if (propTag.IsNamedProperty)
				{
					result = true;
					return false;
				}
				return true;
			}, false);
			return result;
		}

		public static ErrorCode SetFolderAclTableAndSecurityDescriptor(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode result = ErrorCode.NoError;
			SearchFolder searchFolder = bag as SearchFolder;
			if (searchFolder != null && searchFolder.Mailbox.SharedState.MailboxTypeDetail != MailboxInfo.MailboxTypeDetail.GroupMailbox && !FolderSecurity.AclTableAndSecurityDescriptorProperty.IsEmpty((byte[])bag.GetPropertyValue(context, PropTag.Folder.AclTableAndSecurityDescriptor)))
			{
				result = ErrorCode.CreateNoAccess((LID)61200U);
			}
			((Folder)bag).BumpAclTableVersion();
			return result;
		}

		public static object GetFolderNTSecurityDescriptor(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] buffer = (byte[])bag.GetPropertyValue(context, PropTag.Folder.AclTableAndSecurityDescriptor);
			FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = AclTableHelper.Parse(context, buffer);
			if (aclTableAndSecurityDescriptorProperty.SecurityDescriptor != null)
			{
				return PropertyValueHelpers.FormatSdForTransfer(aclTableAndSecurityDescriptorProperty.SecurityDescriptor);
			}
			return null;
		}

		public static ErrorCode SetFolderNTSecurityDescriptor(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return ErrorCode.CreateNoAccess((LID)51632U);
		}

		public static object GetFolderFreeBusyNTSD(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] buffer = (byte[])bag.GetPropertyValue(context, PropTag.Folder.AclTableAndSecurityDescriptor);
			FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = AclTableHelper.Parse(context, buffer);
			if (aclTableAndSecurityDescriptorProperty.FreeBusySecurityDescriptor != null)
			{
				return PropertyValueHelpers.FormatSdForTransfer(aclTableAndSecurityDescriptorProperty.FreeBusySecurityDescriptor);
			}
			SecurityDescriptor securityDescriptor = aclTableAndSecurityDescriptorProperty.ComputeFreeBusySdFromFolderSd();
			if (securityDescriptor != null)
			{
				return PropertyValueHelpers.FormatSdForTransfer(securityDescriptor);
			}
			return null;
		}

		public static ErrorCode SetFolderFreeBusyNTSD(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return ErrorCode.CreateNoAccess((LID)45488U);
		}

		public static object GetFolderMessageSize(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return (long)bag.GetPropertyValue(context, PropTag.Folder.NormalMessageSize) + (long)bag.GetPropertyValue(context, PropTag.Folder.AssociatedMessageSize);
		}

		public static object GetFolderCorrelationId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			int mailboxNumber = (int)bag.GetPropertyValue(context, PropTag.Folder.MailboxNum);
			long fid = (long)bag.GetPropertyValue(context, PropTag.Folder.Fid);
			return CorrelationIdHelper.GetCorrelationId(mailboxNumber, fid);
		}

		public static object GetFolderPartOfContentIndexing(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			object propertyValue = bag.GetPropertyValue(context, PropTag.Folder.IPMFolder);
			if (propertyValue != null && (bool)propertyValue)
			{
				FolderType folderType = FolderType.Normal;
				object propertyValue2 = bag.GetPropertyValue(context, PropTag.Folder.FolderType);
				if (propertyValue2 != null)
				{
					folderType = (FolderType)((int)propertyValue2);
				}
				return folderType == FolderType.Normal;
			}
			object propertyValue3 = bag.GetPropertyValue(context, PropTag.Folder.FolderAdminFlags);
			string a = (string)bag.GetPropertyValue(context, PropTag.Folder.DisplayName);
			if (propertyValue3 != null)
			{
				FolderAdminFlags folderAdminFlags = (FolderAdminFlags)((int)propertyValue3);
				if ((folderAdminFlags & FolderAdminFlags.DumpsterFolder) == FolderAdminFlags.DumpsterFolder && a != "Calendar Logging")
				{
					return true;
				}
			}
			return a == "UMReportingData";
		}

		public static object GetFolderPropertyGroupMappingId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return MessagePropGroups.CurrentGroupMappingId;
		}

		public static ErrorCode InitializeFolder(Context context, ISimplePropertyBag bag)
		{
			bag.SetProperty(context, PropTag.Folder.CreationTime, ((Folder)bag).Mailbox.UtcNow);
			bag.SetProperty(context, PropTag.Folder.LastModificationTime, ((Folder)bag).Mailbox.UtcNow);
			bag.SetProperty(context, PropTag.Folder.SecureOrigination, false);
			bag.SetProperty(context, PropTag.Folder.ContentCountInt64, 0L);
			bag.SetProperty(context, PropTag.Folder.UnreadCountInt64, 0L);
			bag.SetProperty(context, PropTag.Folder.AssociatedContentCountInt64, 0L);
			bag.SetProperty(context, PropTag.Folder.DeletedCountTotalInt64, 0L);
			bag.SetProperty(context, PropTag.Folder.NormalMsgWithAttachCountInt64, 0L);
			bag.SetProperty(context, PropTag.Folder.AssocMsgWithAttachCountInt64, 0L);
			bag.SetProperty(context, PropTag.Folder.AttachOnNormalMsgCtInt64, 0L);
			bag.SetProperty(context, PropTag.Folder.AttachOnAssocMsgCtInt64, 0L);
			bag.SetProperty(context, PropTag.Folder.FolderChildCountInt64, 0L);
			bag.SetProperty(context, PropTag.Folder.DeletedFolderCount, 0);
			bag.SetProperty(context, PropTag.Folder.NormalMessageSize, 0L);
			bag.SetProperty(context, PropTag.Folder.AssociatedMessageSize, 0L);
			return ErrorCode.NoError;
		}

		public static object GetAttachmentLanguage(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return "EnUs";
		}

		public static ErrorCode SetAttachmentLanguage(Context context, ISimplePropertyBag bag, object value)
		{
			return ErrorCode.NoError;
		}

		public static object GetAttachmentHasNamedProperties(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			bool result = false;
			bag.EnumerateBlobProperties(context, delegate(StorePropTag propTag, object value)
			{
				if (propTag.IsNamedProperty)
				{
					result = true;
					return false;
				}
				return true;
			}, false);
			return result;
		}

		public static ErrorCode InitializeAttachment(Context context, ISimplePropertyBag bag)
		{
			bag.SetProperty(context, PropTag.Attachment.CreationTime, ((Attachment)bag).Mailbox.UtcNow);
			bag.SetProperty(context, PropTag.Attachment.LastModificationTime, ((Attachment)bag).Mailbox.UtcNow);
			return ErrorCode.NoError;
		}

		public static ErrorCode InitializeAttachmentView(Context context, ISimplePropertyBag bag)
		{
			return ErrorCode.NoError;
		}

		public static ErrorCode InitializeFolderView(Context context, ISimplePropertyBag bag)
		{
			return ErrorCode.NoError;
		}

		public static ErrorCode InitializeEvent(Context context, ISimplePropertyBag bag)
		{
			return ErrorCode.NoError;
		}

		public static object GetRecipientRecipientType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			object blobPropertyValue = bag.GetBlobPropertyValue(context, PropTag.Recipient.RecipientType);
			if (blobPropertyValue == null)
			{
				return 1;
			}
			return blobPropertyValue;
		}

		public static ErrorCode SetRecipientRecipientType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			Recipient recipient = bag as Recipient;
			if (recipient != null)
			{
				RecipientType type = (RecipientType)((int)bag.GetPropertyValue(context, PropTag.Recipient.RecipientType));
				recipient.InvalidateMessageComputedProperty(type);
				RecipientType type2 = (RecipientType)((int)value);
				recipient.InvalidateMessageComputedProperty(type2);
			}
			bag.SetBlobProperty(context, PropTag.Recipient.RecipientType, value);
			return noError;
		}

		public static ErrorCode SetRecipientDisplayName(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			Recipient recipient = bag as Recipient;
			if (recipient != null)
			{
				RecipientType type = (RecipientType)((int)bag.GetPropertyValue(context, PropTag.Recipient.RecipientType));
				recipient.InvalidateMessageComputedProperty(type);
			}
			bag.SetBlobProperty(context, PropTag.Recipient.DisplayName, value);
			return noError;
		}

		public static object GetRecipientAddressType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			string text = (string)bag.GetBlobPropertyValue(context, PropTag.Recipient.AddressType);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			object blobPropertyValue = bag.GetBlobPropertyValue(context, PropTag.Recipient.EntryId);
			if (blobPropertyValue == null)
			{
				return null;
			}
			string text2 = null;
			string text3 = null;
			Eidt eidt;
			if (AddressBookEID.IsAddressBookEntryId(context, (byte[])blobPropertyValue, out eidt, out text2))
			{
				return "EX";
			}
			MapiAPIFlags mapiAPIFlags;
			if (AddressBookEID.IsOneOffEntryId(context, (byte[])blobPropertyValue, out mapiAPIFlags, ref text, ref text2, ref text3))
			{
				return text;
			}
			return null;
		}

		public static ErrorCode SetRecipientAddressType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			string text = value as string;
			if (!string.IsNullOrEmpty(text) && text.Length < 64)
			{
				bag.SetBlobProperty(context, PropTag.Recipient.AddressType, text);
			}
			else
			{
				bag.SetBlobProperty(context, PropTag.Recipient.AddressType, null);
			}
			if (string.Compare(text, "EX", StringComparison.OrdinalIgnoreCase) == 0)
			{
				bag.SetBlobProperty(context, PropTag.Recipient.UserDN, bag.GetBlobPropertyValue(context, PropTag.Recipient.EmailAddress) as string);
			}
			else
			{
				bag.SetBlobProperty(context, PropTag.Recipient.UserDN, null);
			}
			return noError;
		}

		public static object GetRecipientEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			string text = (string)bag.GetBlobPropertyValue(context, PropTag.Recipient.EmailAddress);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			object blobPropertyValue = bag.GetBlobPropertyValue(context, PropTag.Recipient.EntryId);
			if (blobPropertyValue == null)
			{
				return null;
			}
			string text2 = null;
			string text3 = null;
			Eidt eidt;
			MapiAPIFlags mapiAPIFlags;
			if (AddressBookEID.IsAddressBookEntryId(context, (byte[])blobPropertyValue, out eidt, out text) || AddressBookEID.IsOneOffEntryId(context, (byte[])blobPropertyValue, out mapiAPIFlags, ref text2, ref text, ref text3))
			{
				return text;
			}
			return null;
		}

		public static ErrorCode SetRecipientEmailAddress(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			string text = (string)bag.GetPropertyValue(context, PropTag.Recipient.AddressType);
			if (!string.IsNullOrEmpty(text))
			{
				if (string.Compare(text, "EX", StringComparison.OrdinalIgnoreCase) == 0)
				{
					bag.SetBlobProperty(context, PropTag.Recipient.UserDN, value);
				}
				else
				{
					bag.SetBlobProperty(context, PropTag.Recipient.UserDN, null);
				}
			}
			bag.SetBlobProperty(context, PropTag.Recipient.EmailAddress, value);
			return noError;
		}

		public static object GetRecipientEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] array = bag.GetBlobPropertyValue(context, PropTag.Recipient.EntryId) as byte[];
			if (array != null && array.Length != 0)
			{
				return array;
			}
			string text = (string)bag.GetPropertyValue(context, PropTag.Recipient.UserDN);
			if (!string.IsNullOrEmpty(text))
			{
				int num = (int)bag.GetPropertyValue(context, PropTag.Recipient.ObjectType);
				return AddressBookEID.MakeAddressBookEntryID(text, num != 6);
			}
			string emailAddr = (string)bag.GetPropertyValue(context, PropTag.Recipient.EmailAddress);
			string displayName = (string)bag.GetPropertyValue(context, PropTag.Recipient.DisplayName);
			string addrType = (string)bag.GetPropertyValue(context, PropTag.Recipient.AddressType);
			bool getSendRichInfo = (bool)bag.GetPropertyValue(context, PropTag.Recipient.SendRichInfo);
			int getSendInternetEncoding = 0;
			object propertyValue = bag.GetPropertyValue(context, PropTag.Recipient.SendInternetEncoding);
			if (propertyValue != null)
			{
				getSendInternetEncoding = (int)propertyValue;
			}
			return AddressBookEID.MakeOneOffEntryID(addrType, emailAddr, displayName, getSendRichInfo, getSendInternetEncoding);
		}

		public static ErrorCode SetRecipientEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			byte[] array = value as byte[];
			if (array == null || array.Length == 0)
			{
				value = null;
			}
			bag.SetBlobProperty(context, PropTag.Recipient.EntryId, value);
			return noError;
		}

		public static object GetRecipientEntryIdSvrEid(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return bag.GetPropertyValue(context, PropTag.Recipient.EntryId);
		}

		public static object GetRecipientRecordKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return bag.GetPropertyValue(context, PropTag.Recipient.EntryId);
		}

		public static object GetRecipientRecipientEntryId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return bag.GetPropertyValue(context, PropTag.Recipient.EntryId);
		}

		public static ErrorCode SetRecipientRecipientEntryId(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			return bag.SetProperty(context, PropTag.Recipient.EntryId, value).Propagate((LID)40185U);
		}

		public static object GetRecipientResponsibility(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			bool? flag = bag.GetBlobPropertyValue(context, PropTag.Recipient.Responsibility) as bool?;
			if (flag == null || !flag.Value)
			{
				return SerializedValue.BoxedFalse;
			}
			return SerializedValue.BoxedTrue;
		}

		public static ErrorCode SetRecipientResponsibility(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			bag.SetBlobProperty(context, PropTag.Recipient.Responsibility, value);
			return noError;
		}

		public static object GetRecipientSendRichInfo(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			bool? flag = bag.GetBlobPropertyValue(context, PropTag.Recipient.SendRichInfo) as bool?;
			if (flag == null || !flag.Value)
			{
				return SerializedValue.BoxedFalse;
			}
			return SerializedValue.BoxedTrue;
		}

		public static ErrorCode SetRecipientSendRichInfo(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			bag.SetBlobProperty(context, PropTag.Recipient.SendRichInfo, value);
			return noError;
		}

		public static object GetRecipientDisplayType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			int? num = bag.GetBlobPropertyValue(context, PropTag.Recipient.DisplayType) as int?;
			return (num != null) ? num.Value : 0;
		}

		public static ErrorCode SetRecipientDisplayType(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			bag.SetBlobProperty(context, PropTag.Recipient.DisplayType, value);
			return noError;
		}

		public static object GetRecipientObjectType(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return 6;
		}

		public static object GetRecipientSearchKey(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] array = bag.GetBlobPropertyValue(context, PropTag.Recipient.SearchKey) as byte[];
			if (array == null)
			{
				string addrType = (string)bag.GetPropertyValue(context, PropTag.Recipient.AddressType);
				string emailAddr = (string)bag.GetPropertyValue(context, PropTag.Recipient.EmailAddress);
				array = AddressBookEID.MakeSearchKey(addrType, emailAddr);
			}
			return array;
		}

		public static ErrorCode SetRecipientSearchKey(Context context, ISimplePropertyBag bag, object value)
		{
			ErrorCode noError = ErrorCode.NoError;
			bag.SetBlobProperty(context, PropTag.Recipient.SearchKey, value);
			return noError;
		}

		public static ErrorCode InitializeRecipient(Context context, ISimplePropertyBag bag)
		{
			return ErrorCode.NoError;
		}

		public static ErrorCode InitializeConversation(Context context, ISimplePropertyBag bag)
		{
			return ErrorCode.NoError;
		}

		public static ErrorCode InitializeInferenceLog(Context context, ISimplePropertyBag bag)
		{
			return ErrorCode.NoError;
		}

		public static ErrorCode InitializeUserInfo(Context context, ISimplePropertyBag bag)
		{
			return ErrorCode.NoError;
		}

		internal static void MountEventHandler(StoreDatabase database)
		{
			PropertySchemaPopulation.Populate(database);
		}

		public static PropertyColumn ConstructPropertyColumn(Table table, StorePropTag propertyTag, Func<IRowAccess, IRowPropertyBag> rowPropBagCreator, Column[] dependOn)
		{
			return PropertySchemaPopulation.ConstructPropertyColumn(table, propertyTag, rowPropBagCreator, dependOn);
		}

		public static ConversionColumn ConstructConversionColumn(StorePropTag propertyTag, Type type, int size, int maxLength, Table table, Func<object, object> conversionFunction, string functionName, Column argumentColumn)
		{
			return PropertySchemaPopulation.ConstructConversionColumn(propertyTag, type, size, maxLength, table, conversionFunction, functionName, argumentColumn);
		}

		public static ConstantColumn ConstructConstantColumn(StorePropTag propertyTag, object propertyValue)
		{
			return PropertySchemaPopulation.ConstructConstantColumn(propertyTag, propertyValue);
		}

		public static FunctionColumn ConstructFunctionColumn(StorePropTag propertyTag, Type type, int size, int maxLength, Table table, Func<object[], object> function, string functionName, Column[] argumentColumns)
		{
			return PropertySchemaPopulation.ConstructFunctionColumn(propertyTag, type, size, maxLength, table, function, functionName, argumentColumns);
		}
	}
}
