using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal static class PropertySchemaPopulation
	{
		private static void Populate(StoreDatabase database)
		{
			PropertySchema.AddObjectSchema(database, ObjectType.Mailbox, PropertySchemaPopulation.GenerateMailboxPropertySchema(database));
		}

		public static ObjectPropertySchema GenerateMailboxPropertySchema(StoreDatabase database)
		{
			Dictionary<StorePropTag, PropertyMapping> dictionary = new Dictionary<StorePropTag, PropertyMapping>(200);
			MailboxTable mailboxTable = DatabaseSchema.MailboxTable(database);
			if (mailboxTable == null)
			{
				return null;
			}
			Table table = mailboxTable.Table;
			ObjectPropertySchema objectPropertySchema = new ObjectPropertySchema();
			ObjectPropertySchema baseSchema = null;
			Func<IRowAccess, IRowPropertyBag> rowPropBagCreator = (IRowAccess rowAccess) => new RowPropertyBag(table, objectPropertySchema, PropTag.Mailbox.MailboxNum, rowAccess);
			PropertyMapping value;
			if (mailboxTable.MailboxNumber != null)
			{
				Column column = Factory.CreateMappedPropertyColumn(mailboxTable.MailboxNumber, PropTag.Mailbox.MailboxNum);
				value = new PhysicalColumnPropertyMapping(PropTag.Mailbox.MailboxNum, column, null, null, null, (PhysicalColumn)column.ActualColumn, false, true, true, false, false);
			}
			else
			{
				Column column = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.MailboxNum, null), PropTag.Mailbox.MailboxNum);
				value = new ConstantPropertyMapping(PropTag.Mailbox.MailboxNum, column, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Mailbox.MailboxNum, value);
			PropertyMapping value2;
			if (mailboxTable.MailboxPartitionNumber != null)
			{
				Column column2 = Factory.CreateMappedPropertyColumn(mailboxTable.MailboxPartitionNumber, PropTag.Mailbox.MailboxPartitionNumber);
				value2 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.MailboxPartitionNumber, column2, null, null, null, (PhysicalColumn)column2.ActualColumn, false, true, true, false, false);
			}
			else
			{
				Column column2 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.MailboxPartitionNumber, null), PropTag.Mailbox.MailboxPartitionNumber);
				value2 = new ConstantPropertyMapping(PropTag.Mailbox.MailboxPartitionNumber, column2, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Mailbox.MailboxPartitionNumber, value2);
			Column column3;
			PropertyMapping propertyMapping;
			if (mailboxTable.MailboxGuid != null)
			{
				column3 = Factory.CreateMappedPropertyColumn(mailboxTable.MailboxGuid, PropTag.Mailbox.MailboxDSGuidGuid);
				propertyMapping = new PhysicalColumnPropertyMapping(PropTag.Mailbox.MailboxDSGuidGuid, column3, null, null, null, (PhysicalColumn)column3.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column3 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.MailboxDSGuidGuid, null), PropTag.Mailbox.MailboxDSGuidGuid);
				propertyMapping = new ConstantPropertyMapping(PropTag.Mailbox.MailboxDSGuidGuid, column3, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Mailbox.MailboxDSGuidGuid, propertyMapping);
			Column column4 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Mailbox.MailboxDSGuid, typeof(byte[]), 16, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertGuidToBinary), "Exchange.ConvertGuidToBinary", column3), PropTag.Mailbox.MailboxDSGuid);
			ConversionPropertyMapping value3 = new ConversionPropertyMapping(PropTag.Mailbox.MailboxDSGuid, column4, new Func<object, object>(PropertySchemaPopulation.ConvertGuidToBinary), PropTag.Mailbox.MailboxDSGuidGuid, propertyMapping, null, null, null, true, true, true);
			dictionary.Add(PropTag.Mailbox.MailboxDSGuid, value3);
			PropertyMapping value4;
			if (mailboxTable.UnifiedMailboxGuid != null)
			{
				Column column5 = Factory.CreateMappedPropertyColumn(mailboxTable.UnifiedMailboxGuid, PropTag.Mailbox.UnifiedMailboxGuidGuid);
				value4 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.UnifiedMailboxGuidGuid, column5, null, null, null, (PhysicalColumn)column5.ActualColumn, true, false, true, false, false);
			}
			else
			{
				Column column5 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.UnifiedMailboxGuidGuid, null), PropTag.Mailbox.UnifiedMailboxGuidGuid);
				value4 = new ConstantPropertyMapping(PropTag.Mailbox.UnifiedMailboxGuidGuid, column5, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Mailbox.UnifiedMailboxGuidGuid, value4);
			PropertyMapping value5;
			if (mailboxTable.DisplayName != null)
			{
				Column column6 = Factory.CreateMappedPropertyColumn(mailboxTable.DisplayName, PropTag.Mailbox.DisplayName);
				value5 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.DisplayName, column6, null, null, null, (PhysicalColumn)column6.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column6 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.DisplayName, null), PropTag.Mailbox.DisplayName);
				value5 = new ConstantPropertyMapping(PropTag.Mailbox.DisplayName, column6, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.DisplayName, value5);
			PropertyMapping value6;
			if (mailboxTable.SimpleDisplayName != null)
			{
				Column column7 = Factory.CreateMappedPropertyColumn(mailboxTable.SimpleDisplayName, PropTag.Mailbox.SimpleDisplayName);
				value6 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.SimpleDisplayName, column7, null, null, null, (PhysicalColumn)column7.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column7 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.SimpleDisplayName, null), PropTag.Mailbox.SimpleDisplayName);
				value6 = new ConstantPropertyMapping(PropTag.Mailbox.SimpleDisplayName, column7, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.SimpleDisplayName, value6);
			PropertyMapping value7;
			if (mailboxTable.Comment != null)
			{
				Column column8 = Factory.CreateMappedPropertyColumn(mailboxTable.Comment, PropTag.Mailbox.Comment);
				value7 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.Comment, column8, null, null, null, (PhysicalColumn)column8.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column8 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.Comment, null), PropTag.Mailbox.Comment);
				value7 = new ConstantPropertyMapping(PropTag.Mailbox.Comment, column8, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.Comment, value7);
			Column column9;
			PropertyMapping propertyMapping2;
			if (mailboxTable.TenantHint != null)
			{
				column9 = Factory.CreateMappedPropertyColumn(mailboxTable.TenantHint, PropTag.Mailbox.InternalTenantHint);
				propertyMapping2 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.InternalTenantHint, column9, null, null, null, (PhysicalColumn)column9.ActualColumn, true, true, true, false, false);
			}
			else
			{
				column9 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.InternalTenantHint, null), PropTag.Mailbox.InternalTenantHint);
				propertyMapping2 = new ConstantPropertyMapping(PropTag.Mailbox.InternalTenantHint, column9, null, null, true, true, false);
			}
			dictionary.Add(PropTag.Mailbox.InternalTenantHint, propertyMapping2);
			Column[] dependOn = new Column[]
			{
				column9
			};
			PropertyColumn column10 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Mailbox.TenantHint, rowPropBagCreator, dependOn);
			ComputedPropertyMapping value8 = new ComputedPropertyMapping(PropTag.Mailbox.TenantHint, column10, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMailboxTenantHint), new StorePropTag[]
			{
				PropTag.Mailbox.InternalTenantHint
			}, new PropertyMapping[]
			{
				propertyMapping2
			}, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Mailbox.TenantHint, value8);
			PropertyMapping value9;
			if (mailboxTable.MailboxOwnerDisplayName != null)
			{
				Column column11 = Factory.CreateMappedPropertyColumn(mailboxTable.MailboxOwnerDisplayName, PropTag.Mailbox.MailboxOwnerName);
				value9 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.MailboxOwnerName, column11, null, null, null, (PhysicalColumn)column11.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column11 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.MailboxOwnerName, null), PropTag.Mailbox.MailboxOwnerName);
				value9 = new ConstantPropertyMapping(PropTag.Mailbox.MailboxOwnerName, column11, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.MailboxOwnerName, value9);
			PropertyMapping value10;
			if (mailboxTable.OofState != null)
			{
				Column column12 = Factory.CreateMappedPropertyColumn(mailboxTable.OofState, PropTag.Mailbox.OofState);
				value10 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.OofState, column12, null, null, null, (PhysicalColumn)column12.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column12 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.OofState, null), PropTag.Mailbox.OofState);
				value10 = new ConstantPropertyMapping(PropTag.Mailbox.OofState, column12, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.OofState, value10);
			PropertyMapping value11;
			if (mailboxTable.DeletedOn != null)
			{
				Column column13 = Factory.CreateMappedPropertyColumn(mailboxTable.DeletedOn, PropTag.Mailbox.DateDiscoveredAbsentInDS);
				value11 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.DateDiscoveredAbsentInDS, column13, null, null, null, (PhysicalColumn)column13.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column13 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.DateDiscoveredAbsentInDS, null), PropTag.Mailbox.DateDiscoveredAbsentInDS);
				value11 = new ConstantPropertyMapping(PropTag.Mailbox.DateDiscoveredAbsentInDS, column13, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.DateDiscoveredAbsentInDS, value11);
			PropertyMapping value12;
			if (mailboxTable.DeletedOn != null)
			{
				Column column14 = Factory.CreateMappedPropertyColumn(mailboxTable.DeletedOn, PropTag.Mailbox.DeletedOn);
				value12 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.DeletedOn, column14, null, null, null, (PhysicalColumn)column14.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column14 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.DeletedOn, null), PropTag.Mailbox.DeletedOn);
				value12 = new ConstantPropertyMapping(PropTag.Mailbox.DeletedOn, column14, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.DeletedOn, value12);
			PropertyMapping value13;
			if (mailboxTable.OwnerLegacyDN != null)
			{
				Column column15 = Factory.CreateMappedPropertyColumn(mailboxTable.OwnerLegacyDN, PropTag.Mailbox.EmailAddress);
				value13 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.EmailAddress, column15, null, null, null, (PhysicalColumn)column15.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column15 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.EmailAddress, null), PropTag.Mailbox.EmailAddress);
				value13 = new ConstantPropertyMapping(PropTag.Mailbox.EmailAddress, column15, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.EmailAddress, value13);
			PropertyMapping value14;
			if (mailboxTable.OwnerLegacyDN != null)
			{
				Column column16 = Factory.CreateMappedPropertyColumn(mailboxTable.OwnerLegacyDN, PropTag.Mailbox.MailboxOwnerDN);
				value14 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.MailboxOwnerDN, column16, null, null, null, (PhysicalColumn)column16.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column16 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.MailboxOwnerDN, null), PropTag.Mailbox.MailboxOwnerDN);
				value14 = new ConstantPropertyMapping(PropTag.Mailbox.MailboxOwnerDN, column16, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.MailboxOwnerDN, value14);
			PropertyColumn column17 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Mailbox.HasNamedProperties, rowPropBagCreator, null);
			ComputedPropertyMapping value15 = new ComputedPropertyMapping(PropTag.Mailbox.HasNamedProperties, column17, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMailboxHasNamedProperties), null, null, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Mailbox.HasNamedProperties, value15);
			PropertyColumn column18 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Mailbox.CISearchEnabled, rowPropBagCreator, null);
			ComputedPropertyMapping value16 = new ComputedPropertyMapping(PropTag.Mailbox.CISearchEnabled, column18, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMailboxCISearchEnabled), null, null, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Mailbox.CISearchEnabled, value16);
			Column column19;
			PropertyMapping propertyMapping3;
			if (mailboxTable.MessageCount != null)
			{
				column19 = Factory.CreateMappedPropertyColumn(mailboxTable.MessageCount, PropTag.Mailbox.ContentCountInt64);
				propertyMapping3 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.ContentCountInt64, column19, null, null, null, (PhysicalColumn)column19.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column19 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.ContentCountInt64, null), PropTag.Mailbox.ContentCountInt64);
				propertyMapping3 = new ConstantPropertyMapping(PropTag.Mailbox.ContentCountInt64, column19, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Mailbox.ContentCountInt64, propertyMapping3);
			Column column20 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Mailbox.ContentCount, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column19), PropTag.Mailbox.ContentCount);
			ConversionPropertyMapping value17 = new ConversionPropertyMapping(PropTag.Mailbox.ContentCount, column20, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Mailbox.ContentCountInt64, propertyMapping3, null, null, null, true, true, true);
			dictionary.Add(PropTag.Mailbox.ContentCount, value17);
			Column column21;
			PropertyMapping propertyMapping4;
			if (mailboxTable.HiddenMessageCount != null)
			{
				column21 = Factory.CreateMappedPropertyColumn(mailboxTable.HiddenMessageCount, PropTag.Mailbox.AssociatedContentCountInt64);
				propertyMapping4 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.AssociatedContentCountInt64, column21, null, null, null, (PhysicalColumn)column21.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column21 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.AssociatedContentCountInt64, null), PropTag.Mailbox.AssociatedContentCountInt64);
				propertyMapping4 = new ConstantPropertyMapping(PropTag.Mailbox.AssociatedContentCountInt64, column21, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Mailbox.AssociatedContentCountInt64, propertyMapping4);
			Column column22 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Mailbox.AssociatedContentCount, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column21), PropTag.Mailbox.AssociatedContentCount);
			ConversionPropertyMapping value18 = new ConversionPropertyMapping(PropTag.Mailbox.AssociatedContentCount, column22, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Mailbox.AssociatedContentCountInt64, propertyMapping4, null, null, null, true, true, true);
			dictionary.Add(PropTag.Mailbox.AssociatedContentCount, value18);
			Column column23;
			PropertyMapping propertyMapping5;
			if (mailboxTable.MessageDeletedCount != null)
			{
				column23 = Factory.CreateMappedPropertyColumn(mailboxTable.MessageDeletedCount, PropTag.Mailbox.DeletedMsgCountInt64);
				propertyMapping5 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.DeletedMsgCountInt64, column23, null, null, null, (PhysicalColumn)column23.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column23 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.DeletedMsgCountInt64, null), PropTag.Mailbox.DeletedMsgCountInt64);
				propertyMapping5 = new ConstantPropertyMapping(PropTag.Mailbox.DeletedMsgCountInt64, column23, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Mailbox.DeletedMsgCountInt64, propertyMapping5);
			Column column24 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Mailbox.DeletedMsgCount, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column23), PropTag.Mailbox.DeletedMsgCount);
			ConversionPropertyMapping value19 = new ConversionPropertyMapping(PropTag.Mailbox.DeletedMsgCount, column24, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Mailbox.DeletedMsgCountInt64, propertyMapping5, null, null, null, true, true, true);
			dictionary.Add(PropTag.Mailbox.DeletedMsgCount, value19);
			Column column25;
			PropertyMapping propertyMapping6;
			if (mailboxTable.HiddenMessageDeletedCount != null)
			{
				column25 = Factory.CreateMappedPropertyColumn(mailboxTable.HiddenMessageDeletedCount, PropTag.Mailbox.DeletedAssocMsgCountInt64);
				propertyMapping6 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.DeletedAssocMsgCountInt64, column25, null, null, null, (PhysicalColumn)column25.ActualColumn, true, false, true, false, false);
			}
			else
			{
				column25 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.DeletedAssocMsgCountInt64, null), PropTag.Mailbox.DeletedAssocMsgCountInt64);
				propertyMapping6 = new ConstantPropertyMapping(PropTag.Mailbox.DeletedAssocMsgCountInt64, column25, null, null, false, true, false);
			}
			dictionary.Add(PropTag.Mailbox.DeletedAssocMsgCountInt64, propertyMapping6);
			Column column26 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Mailbox.DeletedAssocMsgCount, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column25), PropTag.Mailbox.DeletedAssocMsgCount);
			ConversionPropertyMapping value20 = new ConversionPropertyMapping(PropTag.Mailbox.DeletedAssocMsgCount, column26, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Mailbox.DeletedAssocMsgCountInt64, propertyMapping6, null, null, null, true, true, true);
			dictionary.Add(PropTag.Mailbox.DeletedAssocMsgCount, value20);
			Column column27;
			PropertyMapping propertyMapping7;
			if (mailboxTable.MessageSize != null)
			{
				column27 = Factory.CreateMappedPropertyColumn(mailboxTable.MessageSize, PropTag.Mailbox.NormalMessageSize);
				propertyMapping7 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.NormalMessageSize, column27, null, null, null, (PhysicalColumn)column27.ActualColumn, true, true, true, true, false);
			}
			else
			{
				column27 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.NormalMessageSize, null), PropTag.Mailbox.NormalMessageSize);
				propertyMapping7 = new ConstantPropertyMapping(PropTag.Mailbox.NormalMessageSize, column27, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.NormalMessageSize, propertyMapping7);
			Column column28 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Mailbox.NormalMessageSize32, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column27), PropTag.Mailbox.NormalMessageSize32);
			ConversionPropertyMapping value21 = new ConversionPropertyMapping(PropTag.Mailbox.NormalMessageSize32, column28, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Mailbox.NormalMessageSize, propertyMapping7, null, null, null, false, true, true);
			dictionary.Add(PropTag.Mailbox.NormalMessageSize32, value21);
			Column column29;
			PropertyMapping propertyMapping8;
			if (mailboxTable.HiddenMessageSize != null)
			{
				column29 = Factory.CreateMappedPropertyColumn(mailboxTable.HiddenMessageSize, PropTag.Mailbox.AssociatedMessageSize);
				propertyMapping8 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.AssociatedMessageSize, column29, null, null, null, (PhysicalColumn)column29.ActualColumn, true, true, true, true, false);
			}
			else
			{
				column29 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.AssociatedMessageSize, null), PropTag.Mailbox.AssociatedMessageSize);
				propertyMapping8 = new ConstantPropertyMapping(PropTag.Mailbox.AssociatedMessageSize, column29, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.AssociatedMessageSize, propertyMapping8);
			Column column30 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Mailbox.AssociatedMessageSize32, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column29), PropTag.Mailbox.AssociatedMessageSize32);
			ConversionPropertyMapping value22 = new ConversionPropertyMapping(PropTag.Mailbox.AssociatedMessageSize32, column30, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Mailbox.AssociatedMessageSize, propertyMapping8, null, null, null, false, true, true);
			dictionary.Add(PropTag.Mailbox.AssociatedMessageSize32, value22);
			Column[] dependOn2 = new Column[]
			{
				column27,
				column29
			};
			PropertyColumn propertyColumn = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Mailbox.MessageSize, rowPropBagCreator, dependOn2);
			ComputedPropertyMapping computedPropertyMapping = new ComputedPropertyMapping(PropTag.Mailbox.MessageSize, propertyColumn, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMailboxMessageSize), new StorePropTag[]
			{
				PropTag.Mailbox.NormalMessageSize,
				PropTag.Mailbox.AssociatedMessageSize
			}, new PropertyMapping[]
			{
				propertyMapping7,
				propertyMapping8
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Mailbox.MessageSize, computedPropertyMapping);
			Column column31 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Mailbox.MessageSize32, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", propertyColumn), PropTag.Mailbox.MessageSize32);
			ConversionPropertyMapping value23 = new ConversionPropertyMapping(PropTag.Mailbox.MessageSize32, column31, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Mailbox.MessageSize, computedPropertyMapping, null, null, null, false, true, true);
			dictionary.Add(PropTag.Mailbox.MessageSize32, value23);
			Column column32;
			PropertyMapping propertyMapping9;
			if (mailboxTable.MessageDeletedSize != null)
			{
				column32 = Factory.CreateMappedPropertyColumn(mailboxTable.MessageDeletedSize, PropTag.Mailbox.DeletedNormalMessageSize);
				propertyMapping9 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.DeletedNormalMessageSize, column32, null, null, null, (PhysicalColumn)column32.ActualColumn, true, true, true, true, false);
			}
			else
			{
				column32 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.DeletedNormalMessageSize, null), PropTag.Mailbox.DeletedNormalMessageSize);
				propertyMapping9 = new ConstantPropertyMapping(PropTag.Mailbox.DeletedNormalMessageSize, column32, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.DeletedNormalMessageSize, propertyMapping9);
			PropertyMapping value24;
			if (mailboxTable.MailboxDatabaseVersion != null)
			{
				Column column33 = Factory.CreateMappedPropertyColumn(mailboxTable.MailboxDatabaseVersion, PropTag.Mailbox.MailboxDatabaseVersion);
				value24 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.MailboxDatabaseVersion, column33, null, null, null, (PhysicalColumn)column33.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column33 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.MailboxDatabaseVersion, null), PropTag.Mailbox.MailboxDatabaseVersion);
				value24 = new ConstantPropertyMapping(PropTag.Mailbox.MailboxDatabaseVersion, column33, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.MailboxDatabaseVersion, value24);
			Column column34 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Mailbox.DeletedNormalMessageSize32, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column32), PropTag.Mailbox.DeletedNormalMessageSize32);
			ConversionPropertyMapping value25 = new ConversionPropertyMapping(PropTag.Mailbox.DeletedNormalMessageSize32, column34, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Mailbox.DeletedNormalMessageSize, propertyMapping9, null, null, null, false, true, true);
			dictionary.Add(PropTag.Mailbox.DeletedNormalMessageSize32, value25);
			Column column35;
			PropertyMapping propertyMapping10;
			if (mailboxTable.HiddenMessageDeletedSize != null)
			{
				column35 = Factory.CreateMappedPropertyColumn(mailboxTable.HiddenMessageDeletedSize, PropTag.Mailbox.DeletedAssociatedMessageSize);
				propertyMapping10 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.DeletedAssociatedMessageSize, column35, null, null, null, (PhysicalColumn)column35.ActualColumn, true, true, true, true, false);
			}
			else
			{
				column35 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.DeletedAssociatedMessageSize, null), PropTag.Mailbox.DeletedAssociatedMessageSize);
				propertyMapping10 = new ConstantPropertyMapping(PropTag.Mailbox.DeletedAssociatedMessageSize, column35, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.DeletedAssociatedMessageSize, propertyMapping10);
			Column column36 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Mailbox.DeletedAssociatedMessageSize32, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", column35), PropTag.Mailbox.DeletedAssociatedMessageSize32);
			ConversionPropertyMapping value26 = new ConversionPropertyMapping(PropTag.Mailbox.DeletedAssociatedMessageSize32, column36, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Mailbox.DeletedAssociatedMessageSize, propertyMapping10, null, null, null, false, true, true);
			dictionary.Add(PropTag.Mailbox.DeletedAssociatedMessageSize32, value26);
			Column[] dependOn3 = new Column[]
			{
				column32,
				column35
			};
			PropertyColumn propertyColumn2 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Mailbox.DeletedMessageSize, rowPropBagCreator, dependOn3);
			ComputedPropertyMapping computedPropertyMapping2 = new ComputedPropertyMapping(PropTag.Mailbox.DeletedMessageSize, propertyColumn2, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMailboxDeletedMessageSize), new StorePropTag[]
			{
				PropTag.Mailbox.DeletedNormalMessageSize,
				PropTag.Mailbox.DeletedAssociatedMessageSize
			}, new PropertyMapping[]
			{
				propertyMapping9,
				propertyMapping10
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Mailbox.DeletedMessageSize, computedPropertyMapping2);
			Column column37 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConversionColumn(PropTag.Mailbox.DeletedMessageSize32, typeof(int), 4, 0, table, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), "Exchange.ConvertInt64ToInt32", propertyColumn2), PropTag.Mailbox.DeletedMessageSize32);
			ConversionPropertyMapping value27 = new ConversionPropertyMapping(PropTag.Mailbox.DeletedMessageSize32, column37, new Func<object, object>(PropertySchemaPopulation.ConvertInt64ToInt32), PropTag.Mailbox.DeletedMessageSize, computedPropertyMapping2, null, null, null, false, true, true);
			dictionary.Add(PropTag.Mailbox.DeletedMessageSize32, value27);
			PropertyMapping value28;
			if (mailboxTable.LastLogonTime != null)
			{
				Column column38 = Factory.CreateMappedPropertyColumn(mailboxTable.LastLogonTime, PropTag.Mailbox.LastLogonTime);
				value28 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.LastLogonTime, column38, null, null, null, (PhysicalColumn)column38.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column38 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.LastLogonTime, null), PropTag.Mailbox.LastLogonTime);
				value28 = new ConstantPropertyMapping(PropTag.Mailbox.LastLogonTime, column38, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.LastLogonTime, value28);
			PropertyMapping value29;
			if (mailboxTable.LastLogoffTime != null)
			{
				Column column39 = Factory.CreateMappedPropertyColumn(mailboxTable.LastLogoffTime, PropTag.Mailbox.LastLogoffTime);
				value29 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.LastLogoffTime, column39, null, null, null, (PhysicalColumn)column39.ActualColumn, true, true, true, true, false);
			}
			else
			{
				Column column39 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.LastLogoffTime, null), PropTag.Mailbox.LastLogoffTime);
				value29 = new ConstantPropertyMapping(PropTag.Mailbox.LastLogoffTime, column39, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.LastLogoffTime, value29);
			Column column40;
			PropertyMapping propertyMapping11;
			if (mailboxTable.Status != null)
			{
				column40 = Factory.CreateMappedPropertyColumn(mailboxTable.Status, PropTag.Mailbox.MailboxStatus);
				propertyMapping11 = new PhysicalColumnPropertyMapping(PropTag.Mailbox.MailboxStatus, column40, null, null, null, (PhysicalColumn)column40.ActualColumn, true, true, true, true, false);
			}
			else
			{
				column40 = Factory.CreateMappedPropertyColumn(PropertySchemaPopulation.ConstructConstantColumn(PropTag.Mailbox.MailboxStatus, null), PropTag.Mailbox.MailboxStatus);
				propertyMapping11 = new ConstantPropertyMapping(PropTag.Mailbox.MailboxStatus, column40, null, null, true, true, true);
			}
			dictionary.Add(PropTag.Mailbox.MailboxStatus, propertyMapping11);
			PropertyColumn propertyColumn3 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Mailbox.MailboxFlags, rowPropBagCreator, null);
			DefaultPropertyMapping defaultPropertyMapping = new DefaultPropertyMapping(PropTag.Mailbox.MailboxFlags, propertyColumn3, null, null, null, false, true, true, false);
			dictionary.Add(PropTag.Mailbox.MailboxFlags, defaultPropertyMapping);
			Column[] dependOn4 = new Column[]
			{
				column40,
				propertyColumn3
			};
			PropertyColumn column41 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Mailbox.MailboxMiscFlags, rowPropBagCreator, dependOn4);
			ComputedPropertyMapping value30 = new ComputedPropertyMapping(PropTag.Mailbox.MailboxMiscFlags, column41, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMailboxMailboxMiscFlags), new StorePropTag[]
			{
				PropTag.Mailbox.MailboxStatus,
				PropTag.Mailbox.MailboxFlags
			}, new PropertyMapping[]
			{
				propertyMapping11,
				defaultPropertyMapping
			}, null, null, null, false, true, true, true);
			dictionary.Add(PropTag.Mailbox.MailboxMiscFlags, value30);
			PropertyColumn column42 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Mailbox.InferenceClientActivityFlags, rowPropBagCreator, null);
			DefaultPropertyMapping value31 = new DefaultPropertyMapping(PropTag.Mailbox.InferenceClientActivityFlags, column42, null, null, null, true, true, true, false);
			dictionary.Add(PropTag.Mailbox.InferenceClientActivityFlags, value31);
			PropertyColumn column43 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Mailbox.ExtendedRuleSizeLimit, rowPropBagCreator, null);
			DefaultPropertyMapping value32 = new DefaultPropertyMapping(PropTag.Mailbox.ExtendedRuleSizeLimit, column43, null, null, null, false, false, true, false);
			dictionary.Add(PropTag.Mailbox.ExtendedRuleSizeLimit, value32);
			PropertyColumn column44 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Mailbox.CodePageId, rowPropBagCreator, null);
			DefaultPropertyMapping value33 = new DefaultPropertyMapping(PropTag.Mailbox.CodePageId, column44, null, null, null, false, false, true, false);
			dictionary.Add(PropTag.Mailbox.CodePageId, value33);
			PropertyColumn column45 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Mailbox.InferenceUserCapabilityFlags, rowPropBagCreator, null);
			DefaultPropertyMapping value34 = new DefaultPropertyMapping(PropTag.Mailbox.InferenceUserCapabilityFlags, column45, null, null, null, false, false, true, false);
			dictionary.Add(PropTag.Mailbox.InferenceUserCapabilityFlags, value34);
			PropertyColumn column46 = PropertySchemaPopulation.ConstructPropertyColumn(table, PropTag.Mailbox.PropertyGroupMappingId, rowPropBagCreator, null);
			ComputedPropertyMapping value35 = new ComputedPropertyMapping(PropTag.Mailbox.PropertyGroupMappingId, column46, new Func<Context, ISimpleReadOnlyPropertyBag, object>(PropertySchemaPopulation.GetMailboxPropertyGroupMappingId), null, null, null, null, null, false, false, false, false);
			dictionary.Add(PropTag.Mailbox.PropertyGroupMappingId, value35);
			objectPropertySchema.Initialize(ObjectType.Mailbox, table, dictionary, rowPropBagCreator, baseSchema);
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

		public static object GetMailboxTenantHint(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			byte[] array = (byte[])bag.GetPropertyValue(context, PropTag.Mailbox.InternalTenantHint);
			if (array == null || array.Length == 0)
			{
				return TenantHint.RootOrgBlob;
			}
			return array;
		}

		public static object GetMailboxHasNamedProperties(Context context, ISimpleReadOnlyPropertyBag bag)
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

		public static object GetMailboxCISearchEnabled(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return true;
		}

		public static object GetMailboxMessageSize(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return (long)bag.GetPropertyValue(context, PropTag.Mailbox.NormalMessageSize) + (long)bag.GetPropertyValue(context, PropTag.Mailbox.AssociatedMessageSize);
		}

		public static object GetMailboxDeletedMessageSize(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return (long)bag.GetPropertyValue(context, PropTag.Mailbox.DeletedNormalMessageSize) + (long)bag.GetPropertyValue(context, PropTag.Mailbox.DeletedAssociatedMessageSize);
		}

		public static object GetMailboxMailboxMiscFlags(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			object propertyValue = bag.GetPropertyValue(context, PropTag.Mailbox.MailboxFlags);
			int num = (propertyValue != null) ? ((int)propertyValue) : 0;
			propertyValue = bag.GetPropertyValue(context, PropTag.Mailbox.MailboxStatus);
			short num2 = (short)propertyValue;
			if (4 == num2)
			{
				num |= 128;
			}
			else if (3 == num2)
			{
				num |= 64;
			}
			return num;
		}

		public static object GetMailboxPropertyGroupMappingId(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return MessagePropGroups.CurrentGroupMappingId;
		}

		public static ErrorCode InitializeMailbox(Context context, ISimplePropertyBag bag)
		{
			bag.SetProperty(context, PropTag.Mailbox.ContentCountInt64, 0L);
			bag.SetProperty(context, PropTag.Mailbox.AssociatedContentCountInt64, 0L);
			bag.SetProperty(context, PropTag.Mailbox.DeletedMsgCountInt64, 0L);
			bag.SetProperty(context, PropTag.Mailbox.DeletedAssocMsgCountInt64, 0L);
			bag.SetProperty(context, PropTag.Mailbox.NormalMessageSize, 0L);
			bag.SetProperty(context, PropTag.Mailbox.AssociatedMessageSize, 0L);
			bag.SetProperty(context, PropTag.Mailbox.DeletedNormalMessageSize, 0L);
			bag.SetProperty(context, PropTag.Mailbox.DeletedAssociatedMessageSize, 0L);
			return ErrorCode.NoError;
		}

		internal static void MountEventHandler(StoreDatabase database)
		{
			PropertySchemaPopulation.Populate(database);
		}

		public static PropertyColumn ConstructPropertyColumn(Table table, StorePropTag propertyTag, Func<IRowAccess, IRowPropertyBag> rowPropBagCreator, Column[] dependOn)
		{
			string name = "p" + propertyTag.PropTag.ToString("X8");
			if (propertyTag.PropType == PropertyType.Unspecified || propertyTag.PropType == PropertyType.Null)
			{
				throw new StoreException((LID)46760U, ErrorCodeValue.UnexpectedType, "PT_UNSPECIFIED/PT_NULL property type is not supported. propTag=" + propertyTag.ToString());
			}
			Type type = PropertyTypeHelper.ClrTypeFromPropType(propertyTag.PropType);
			int size = PropertyTypeHelper.SizeFromPropType(propertyTag.PropType);
			int maxLength = PropertyTypeHelper.MaxLengthFromPropType(propertyTag.PropType);
			return Factory.CreatePropertyColumn(name, type, size, maxLength, table, propertyTag, rowPropBagCreator, dependOn);
		}

		public static ConversionColumn ConstructConversionColumn(StorePropTag propertyTag, Type type, int size, int maxLength, Table table, Func<object, object> conversionFunction, string functionName, Column argumentColumn)
		{
			string name = string.Format("p{0:X8}", propertyTag.PropTag);
			return Factory.CreateConversionColumn(name, type, size, maxLength, table, conversionFunction, functionName, argumentColumn);
		}

		public static ConstantColumn ConstructConstantColumn(StorePropTag propertyTag, object propertyValue)
		{
			string name = string.Format("p{0:X8}", propertyTag.PropTag);
			return Factory.CreateConstantColumn(name, PropertyTypeHelper.ClrTypeFromPropType(propertyTag.PropType), Visibility.Public, PropertyTypeHelper.SizeFromPropType(propertyTag.PropType), PropertyTypeHelper.MaxLengthFromPropType(propertyTag.PropType), propertyValue);
		}

		public static FunctionColumn ConstructFunctionColumn(StorePropTag propertyTag, Type type, int size, int maxLength, Table table, Func<object[], object> function, string functionName, Column[] argumentColumns)
		{
			string name = string.Format("p{0:X8}", propertyTag.PropTag);
			return Factory.CreateFunctionColumn(name, type, size, maxLength, table, function, functionName, argumentColumns);
		}
	}
}
