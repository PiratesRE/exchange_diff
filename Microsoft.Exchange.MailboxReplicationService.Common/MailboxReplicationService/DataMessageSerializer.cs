using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class DataMessageSerializer
	{
		static DataMessageSerializer()
		{
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FlushMessage.Deserialize), FlushMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyImportBufferMessage.Deserialize), FxProxyImportBufferMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyGetObjectDataRequestMessage.Deserialize), FxProxyGetObjectDataRequestMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyGetObjectDataResponseMessage.Deserialize), FxProxyGetObjectDataResponseMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolOpenFolderMessage.Deserialize), FxProxyPoolOpenFolderMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolCloseEntryMessage.Deserialize), FxProxyPoolCloseEntryMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolOpenItemMessage.Deserialize), FxProxyPoolOpenItemMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolCreateFolderMessage.Deserialize), FxProxyPoolCreateFolderMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolSetExtendedAclMessage.Deserialize), FxProxyPoolSetExtendedAclMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolCreateItemMessage.Deserialize), FxProxyPoolCreateItemMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolSetPropsMessage.Deserialize), FxProxyPoolSetPropsMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolSetItemPropertiesMessage.Deserialize), FxProxyPoolSetItemPropertiesMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolSaveChangesMessage.Deserialize), FxProxyPoolSaveChangesMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolWriteToMimeMessage.Deserialize), FxProxyPoolWriteToMimeMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolDeleteItemMessage.Deserialize), FxProxyPoolDeleteItemMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolGetFolderDataRequestMessage.Deserialize), FxProxyPoolGetFolderDataRequestMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolGetFolderDataResponseMessage.Deserialize), FxProxyPoolGetFolderDataResponseMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolGetUploadedIDsRequestMessage.Deserialize), FxProxyPoolGetUploadedIDsRequestMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(FxProxyPoolGetUploadedIDsResponseMessage.Deserialize), FxProxyPoolGetUploadedIDsResponseMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(BufferBatchMessage.Deserialize), BufferBatchMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(PagedDataMessage.Deserialize), PagedDataMessage.SupportedOpcodes);
			DataMessageSerializer.RegisterType(new DataMessageSerializer.CreateDataMessageDelegate(MessageExportResultsMessage.Deserialize), MessageExportResultsMessage.SupportedOpcodes);
		}

		public static IDataMessage Deserialize(int opcode, byte[] data, bool useCompression)
		{
			DataMessageSerializer.CreateDataMessageDelegate createDataMessageDelegate;
			if (DataMessageSerializer.createMessageByOpcode.TryGetValue((DataMessageOpcode)opcode, out createDataMessageDelegate))
			{
				return createDataMessageDelegate((DataMessageOpcode)opcode, data, useCompression);
			}
			throw new InputDataIsInvalidPermanentException();
		}

		private static void RegisterType(DataMessageSerializer.CreateDataMessageDelegate createDelegate, DataMessageOpcode[] opcodes)
		{
			foreach (DataMessageOpcode key in opcodes)
			{
				DataMessageSerializer.createMessageByOpcode.Add(key, createDelegate);
			}
		}

		private static Dictionary<DataMessageOpcode, DataMessageSerializer.CreateDataMessageDelegate> createMessageByOpcode = new Dictionary<DataMessageOpcode, DataMessageSerializer.CreateDataMessageDelegate>();

		private delegate IDataMessage CreateDataMessageDelegate(DataMessageOpcode opcode, byte[] data, bool useCompression);
	}
}
