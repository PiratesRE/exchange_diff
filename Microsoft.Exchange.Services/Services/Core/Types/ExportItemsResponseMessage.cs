using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("ExportItemsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ExportItemsResponseMessage : ResponseMessage
	{
		public ExportItemsResponseMessage()
		{
		}

		internal ExportItemsResponseMessage(ServiceResultCode code, ServiceError error, XmlNode item) : base(code, error)
		{
			this.itemXML = item;
		}

		internal ExportItemsResponseMessage(int itemIndex, ServiceCommandBase serviceCommand)
		{
			this.itemIndex = itemIndex;
			this.serviceCommand = serviceCommand;
		}

		[XmlAnyElement("ItemId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public XmlNode ItemId
		{
			get
			{
				base.ExecuteServiceCommandIfRequired();
				return this.itemId;
			}
			set
			{
				this.itemId = value;
			}
		}

		[XmlAnyElement("Data", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public XmlNode Data
		{
			get
			{
				base.ExecuteServiceCommandIfRequired();
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		protected override void InternalExecuteServiceCommand()
		{
			if (this.serviceCommand != null)
			{
				ServiceResult<XmlNode> serviceResult = this.CheckBatchProcessingErrorAndExecute();
				this.PopulateServiceResult(serviceResult);
			}
		}

		private ServiceResult<XmlNode> CheckBatchProcessingErrorAndExecute()
		{
			ExportItems exportItems = (ExportItems)this.serviceCommand;
			bool flag = exportItems.IsPreviousResultStopBatchProcessingError(this.itemIndex);
			ServiceResult<XmlNode> result;
			if (flag)
			{
				result = new ServiceResult<XmlNode>(ServiceResultCode.Warning, null, ServiceError.CreateBatchProcessingStoppedError());
			}
			else
			{
				result = exportItems.ExportItemsResult(this.itemIndex);
			}
			return result;
		}

		private void PopulateServiceResult(ServiceResult<XmlNode> serviceResult)
		{
			base.Initialize(serviceResult.Code, serviceResult.Error);
			this.itemXML = serviceResult.Value;
			if (this.itemXML != null)
			{
				foreach (object obj in this.itemXML.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.LocalName == "ItemId")
					{
						this.itemId = xmlNode;
					}
					else if (xmlNode.LocalName == "Data")
					{
						this.data = xmlNode;
					}
				}
			}
		}

		private int itemIndex;

		private ServiceCommandBase serviceCommand;

		private XmlNode itemXML;

		private XmlNode itemId;

		private XmlNode data;
	}
}
