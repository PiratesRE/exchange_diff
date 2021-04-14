using System;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class UploadItems : MultiStepServiceCommand<UploadItemsRequest, XmlNode>
	{
		public UploadItems(CallContext callContext, UploadItemsRequest request) : base(callContext, request)
		{
			if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010SP1))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<ExchangeVersion>(0L, "[UploadItems::UploadItems] UploadItems is only supported on or after E14SP1. The request was from {0}. Failing request.", ExchangeVersion.Current);
				throw new InvalidRequestException();
			}
			this.items = request.Items;
			ServiceCommandBase.ThrowIfNull(this.items, "items", "UploadItems::UploadItems");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			UploadItemsResponse uploadItemsResponse = new UploadItemsResponse();
			uploadItemsResponse.BuildForResults<XmlNode>(base.Results);
			return uploadItemsResponse;
		}

		internal override int StepCount
		{
			get
			{
				return this.items.Nodes.Count;
			}
		}

		internal override ServiceResult<XmlNode> Execute()
		{
			bool isAssociated = false;
			XmlNode xmlNode = this.items.Nodes[base.CurrentStep];
			ServiceCommandBase.ThrowIfNull(xmlNode, "itemXml", "UploadItems::Execute");
			XmlAttribute xmlAttribute = xmlNode.Attributes["CreateAction"];
			XmlAttribute xmlAttribute2 = xmlNode.Attributes["IsAssociated"];
			if (xmlAttribute2 != null)
			{
				try
				{
					isAssociated = BooleanConverter.Parse(xmlAttribute2.Value);
				}
				catch (FormatException innerException)
				{
					throw new InvalidValueForPropertyException(new PropertyUri(PropertyUriEnum.IsAssociated), innerException);
				}
			}
			string value;
			if ((value = xmlAttribute.Value) != null && !(value == "CreateNew"))
			{
				if (value == "Update")
				{
					return this.UpdateOrCreateItem(xmlNode, true, false, isAssociated);
				}
				if (value == "UpdateOrCreate")
				{
					return this.UpdateOrCreateItem(xmlNode, true, true, isAssociated);
				}
			}
			return this.UpdateOrCreateItem(xmlNode, false, true, isAssociated);
		}

		private ServiceResult<XmlNode> UpdateOrCreateItem(XmlNode itemXml, bool tryUpdate, bool okToCreate, bool isAssociated)
		{
			XmlNode xmlNode = null;
			XmlNode xmlNode2 = null;
			XmlNode xmlNode3 = null;
			StoreObjectId storeObjectId = null;
			Item item = null;
			IdAndSession idAndSession = null;
			XmlNodeList childNodes = itemXml.ChildNodes;
			for (int i = 0; i < childNodes.Count; i++)
			{
				string localName;
				if ((localName = childNodes.Item(i).LocalName) != null)
				{
					if (!(localName == "ParentFolderId"))
					{
						if (!(localName == "ItemId"))
						{
							if (localName == "Data")
							{
								xmlNode3 = childNodes.Item(i);
								xmlNode3.Normalize();
							}
						}
						else
						{
							xmlNode = childNodes.Item(i);
							xmlNode.Normalize();
						}
					}
					else
					{
						xmlNode2 = childNodes.Item(i);
						xmlNode2.Normalize();
					}
				}
			}
			try
			{
				idAndSession = base.IdConverter.ConvertXmlToIdAndSessionReadOnly((XmlElement)xmlNode2, BasicTypes.Folder);
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new ParentFolderNotFoundException(innerException);
			}
			if (tryUpdate && !this.TryOpenForOverwrite((XmlElement)xmlNode, idAndSession, out item))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "UploadItems::UpdateOrCreateItem attempt to open item for overwrite failed.");
				if (!okToCreate)
				{
					throw new ObjectNotFoundException(new LocalizedString("UploadItems: Unable to open existing item for update"));
				}
			}
			if (item == null)
			{
				if (!isAssociated)
				{
					item = Item.Create(idAndSession.Session, string.Empty, idAndSession.Id);
				}
				else
				{
					item = MessageItem.CreateAssociated(idAndSession.Session, idAndSession.Id);
				}
			}
			try
			{
				using (MapiFxProxy fxProxyCollector = item.MapiMessage.GetFxProxyCollector())
				{
					byte[] array = Base64StringConverter.Parse(xmlNode3.InnerText);
					int j = 0;
					int length = array.GetLength(0);
					while (j < length)
					{
						if (j + 8 > length)
						{
							throw new CorruptDataException(new LocalizedString("FastTransferUpload, Uploaded Data"));
						}
						int num = BitConverter.ToInt32(array, j);
						j += 4;
						int num2 = BitConverter.ToInt32(array, j);
						j += 4;
						if (j + num2 > length)
						{
							throw new CorruptDataException(new LocalizedString("FastTransferUpload, Uploaded Data"));
						}
						byte[] array2 = (num2 > 0) ? new byte[num2] : null;
						if (num2 > 0)
						{
							Array.ConstrainedCopy(array, j, array2, 0, num2);
						}
						FaultInjection.GenerateFault((FaultInjection.LIDs)3351653693U);
						FxOpcodes fxOpcodes = (FxOpcodes)num;
						if (fxOpcodes == FxOpcodes.TransferBuffer && array2 == null)
						{
							string value = xmlNode2.Attributes.GetNamedItem("Id").Value;
							string value2 = "null";
							if (xmlNode != null)
							{
								value2 = xmlNode.Attributes.GetNamedItem("Id").Value;
							}
							base.CallContext.ProtocolLog.AppendGenericError("BufferInfo", string.Format("[Length:{0}; Index:{1}]", length, j));
							base.CallContext.ProtocolLog.AppendGenericError("ItemID", value2);
							base.CallContext.ProtocolLog.AppendGenericError("ParentFolderID", value);
							throw new CorruptDataException(new LocalizedString("UploadItems - corrupted data encountered"));
						}
						fxProxyCollector.ProcessRequest(fxOpcodes, array2);
						j += num2;
					}
				}
				FaultInjection.GenerateFault((FaultInjection.LIDs)3217435965U);
				item.MapiMessage.SaveChanges();
				this.objectsChanged++;
				byte[] entryId = (byte[])item.MapiMessage.GetProp(PropTag.EntryId).Value;
				storeObjectId = StoreObjectId.FromProviderSpecificId(entryId);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UploadItems::UpdateOrCreateItem encountered exception - Class: {0}, Message: {1}.", ex.GetType().FullName, ex.Message);
				if (ex is CorruptDataException)
				{
					throw;
				}
				if (ex is MapiExceptionNoSupport || ex is MapiExceptionCallFailed || ex is MapiExceptionCorruptData || ex is MapiExceptionInvalidParameter)
				{
					throw new CorruptDataException(new LocalizedString("FastTransferUpload"), ex);
				}
				if (ex is MapiPermanentException || ex is MapiRetryableException)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSaveChanges, (LocalizedException)ex, idAndSession.Session, item, "EWS::UploadItems: MapiException = {0}.", new object[]
					{
						ex
					});
				}
				throw;
			}
			finally
			{
				item.Dispose();
			}
			if (storeObjectId == null)
			{
				throw new CorruptDataException(new LocalizedString("Store Id"));
			}
			XmlElement xmlElement = ServiceXml.CreateElement(base.XmlDocument, "ItemId", "http://schemas.microsoft.com/exchange/services/2006/messages");
			using (Item item2 = Item.Bind(idAndSession.Session, storeObjectId))
			{
				IdAndSession idAndSession2 = IdAndSession.CreateFromItem(item2);
				IdConverter.CreateStoreIdXml(xmlElement, idAndSession2.Id, idAndSession2, "ItemId", "http://schemas.microsoft.com/exchange/services/2006/messages");
			}
			return new ServiceResult<XmlNode>(xmlElement);
		}

		private bool TryOpenForOverwrite(XmlElement idNode, IdAndSession parentFolderIdAndSession, out Item xsoItem)
		{
			bool flag = false;
			xsoItem = null;
			try
			{
				IdAndSession idAndSession = base.IdConverter.ConvertXmlToIdAndSessionReadOnly(idNode, BasicTypes.Item);
				ItemResponseShape responseShape = new ItemResponseShape(ShapeEnum.IdOnly, BodyResponseType.Best, false, null);
				ToXmlPropertyList propertyList = XsoDataConverter.GetPropertyList(idAndSession.Id, idAndSession.Session, responseShape);
				xsoItem = idAndSession.GetRootXsoItem(propertyList.GetPropertyDefinitions());
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "[UploadItems::TryOpenForOverwrite item to be opened for overwrite doesn't exist.");
				return false;
			}
			catch (NonExistentMailboxGuidException)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "[UploadItems::TryOpenForOverwrite item to be opened for overwrite has wrong mailbox guid.");
				return false;
			}
			catch (NonExistentMailboxException)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "[UploadItems::TryOpenForOverwrite nonexistent mailbox for item to be opened for overwrite.");
				return false;
			}
			try
			{
				byte[] entryId = (byte[])xsoItem.MapiMessage.GetProp(PropTag.ParentEntryId).Value;
				StoreObjectId id = StoreObjectId.FromProviderSpecificId(entryId);
				if (parentFolderIdAndSession.Id.Equals(id))
				{
					using (MapiTable attachmentTable = xsoItem.MapiMessage.GetAttachmentTable())
					{
						attachmentTable.SetColumns(new PropTag[]
						{
							PropTag.AttachNum
						});
						for (;;)
						{
							PropValue[][] array = attachmentTable.QueryRows(100);
							if (array.GetLength(0) == 0)
							{
								break;
							}
							for (int i = 0; i < array.Length; i++)
							{
								xsoItem.MapiMessage.DeleteAttach((int)array[i][0].Value);
							}
						}
					}
					using (MapiTable recipientTable = xsoItem.MapiMessage.GetRecipientTable())
					{
						recipientTable.SetColumns(new PropTag[]
						{
							PropTag.RowId
						});
						for (;;)
						{
							PropValue[][] array2 = recipientTable.QueryRows(100);
							if (array2.GetLength(0) == 0)
							{
								break;
							}
							for (int j = 0; j < array2.Length; j++)
							{
								xsoItem.MapiMessage.ModifyRecipients(ModifyRecipientsFlags.RemoveRecipients, new AdrEntry[]
								{
									new AdrEntry(new PropValue[]
									{
										array2[j][0]
									})
								});
							}
						}
					}
					PropTag[] propList = xsoItem.MapiMessage.GetPropList();
					xsoItem.MapiMessage.DeleteProps(propList);
					flag = true;
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UploadItems::TryOpenForOverwrite encountered exception - Class: {0}, Message: {1}.", ex.GetType().FullName, ex.Message);
				throw;
			}
			finally
			{
				if (!flag)
				{
					xsoItem.Dispose();
					xsoItem = null;
				}
			}
			return flag;
		}

		private XmlNodeArray items;
	}
}
