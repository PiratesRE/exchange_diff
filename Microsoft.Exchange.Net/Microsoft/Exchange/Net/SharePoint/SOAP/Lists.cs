using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.SharePoint.SOAP
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[WebServiceBinding(Name = "ListsSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
	internal class Lists : WsAsyncProxyWrapper
	{
		public event GetListCompletedEventHandler GetListCompleted;

		public event GetListAndViewCompletedEventHandler GetListAndViewCompleted;

		public event DeleteListCompletedEventHandler DeleteListCompleted;

		public event AddListCompletedEventHandler AddListCompleted;

		public event AddListFromFeatureCompletedEventHandler AddListFromFeatureCompleted;

		public event UpdateListCompletedEventHandler UpdateListCompleted;

		public event GetListCollectionCompletedEventHandler GetListCollectionCompleted;

		public event GetListItemsCompletedEventHandler GetListItemsCompleted;

		public event GetListItemChangesCompletedEventHandler GetListItemChangesCompleted;

		public event GetListItemChangesWithKnowledgeCompletedEventHandler GetListItemChangesWithKnowledgeCompleted;

		public event GetListItemChangesSinceTokenCompletedEventHandler GetListItemChangesSinceTokenCompleted;

		public event UpdateListItemsCompletedEventHandler UpdateListItemsCompleted;

		public event UpdateListItemsWithKnowledgeCompletedEventHandler UpdateListItemsWithKnowledgeCompleted;

		public event AddDiscussionBoardItemCompletedEventHandler AddDiscussionBoardItemCompleted;

		public event AddWikiPageCompletedEventHandler AddWikiPageCompleted;

		public event GetVersionCollectionCompletedEventHandler GetVersionCollectionCompleted;

		public event AddAttachmentCompletedEventHandler AddAttachmentCompleted;

		public event GetAttachmentCollectionCompletedEventHandler GetAttachmentCollectionCompleted;

		public event DeleteAttachmentCompletedEventHandler DeleteAttachmentCompleted;

		public event CheckOutFileCompletedEventHandler CheckOutFileCompleted;

		public event UndoCheckOutCompletedEventHandler UndoCheckOutCompleted;

		public event CheckInFileCompletedEventHandler CheckInFileCompleted;

		public event GetListContentTypesCompletedEventHandler GetListContentTypesCompleted;

		public event GetListContentTypesAndPropertiesCompletedEventHandler GetListContentTypesAndPropertiesCompleted;

		public event GetListContentTypeCompletedEventHandler GetListContentTypeCompleted;

		public event CreateContentTypeCompletedEventHandler CreateContentTypeCompleted;

		public event UpdateContentTypeCompletedEventHandler UpdateContentTypeCompleted;

		public event DeleteContentTypeCompletedEventHandler DeleteContentTypeCompleted;

		public event UpdateContentTypeXmlDocumentCompletedEventHandler UpdateContentTypeXmlDocumentCompleted;

		public event UpdateContentTypesXmlDocumentCompletedEventHandler UpdateContentTypesXmlDocumentCompleted;

		public event DeleteContentTypeXmlDocumentCompletedEventHandler DeleteContentTypeXmlDocumentCompleted;

		public event ApplyContentTypeToListCompletedEventHandler ApplyContentTypeToListCompleted;

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetList", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetList(string listName)
		{
			object[] array = base.Invoke("GetList", new object[]
			{
				listName
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginGetList(string listName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetList", new object[]
			{
				listName
			}, callback, asyncState);
		}

		public XmlNode EndGetList(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void GetListAsync(string listName)
		{
			this.GetListAsync(listName, null);
		}

		public void GetListAsync(string listName, object userState)
		{
			if (this.GetListOperationCompleted == null)
			{
				this.GetListOperationCompleted = new SendOrPostCallback(this.OnGetListOperationCompleted);
			}
			base.InvokeAsync("GetList", new object[]
			{
				listName
			}, this.GetListOperationCompleted, userState);
		}

		private void OnGetListOperationCompleted(object arg)
		{
			if (this.GetListCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetListCompleted(this, new GetListCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListAndView", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetListAndView(string listName, string viewName)
		{
			object[] array = base.Invoke("GetListAndView", new object[]
			{
				listName,
				viewName
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginGetListAndView(string listName, string viewName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetListAndView", new object[]
			{
				listName,
				viewName
			}, callback, asyncState);
		}

		public XmlNode EndGetListAndView(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void GetListAndViewAsync(string listName, string viewName)
		{
			this.GetListAndViewAsync(listName, viewName, null);
		}

		public void GetListAndViewAsync(string listName, string viewName, object userState)
		{
			if (this.GetListAndViewOperationCompleted == null)
			{
				this.GetListAndViewOperationCompleted = new SendOrPostCallback(this.OnGetListAndViewOperationCompleted);
			}
			base.InvokeAsync("GetListAndView", new object[]
			{
				listName,
				viewName
			}, this.GetListAndViewOperationCompleted, userState);
		}

		private void OnGetListAndViewOperationCompleted(object arg)
		{
			if (this.GetListAndViewCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetListAndViewCompleted(this, new GetListAndViewCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/DeleteList", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void DeleteList(string listName)
		{
			base.Invoke("DeleteList", new object[]
			{
				listName
			});
		}

		public IAsyncResult BeginDeleteList(string listName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DeleteList", new object[]
			{
				listName
			}, callback, asyncState);
		}

		public void EndDeleteList(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void DeleteListAsync(string listName)
		{
			this.DeleteListAsync(listName, null);
		}

		public void DeleteListAsync(string listName, object userState)
		{
			if (this.DeleteListOperationCompleted == null)
			{
				this.DeleteListOperationCompleted = new SendOrPostCallback(this.OnDeleteListOperationCompleted);
			}
			base.InvokeAsync("DeleteList", new object[]
			{
				listName
			}, this.DeleteListOperationCompleted, userState);
		}

		private void OnDeleteListOperationCompleted(object arg)
		{
			if (this.DeleteListCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeleteListCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/AddList", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode AddList(string listName, string description, int templateID)
		{
			object[] array = base.Invoke("AddList", new object[]
			{
				listName,
				description,
				templateID
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginAddList(string listName, string description, int templateID, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddList", new object[]
			{
				listName,
				description,
				templateID
			}, callback, asyncState);
		}

		public XmlNode EndAddList(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void AddListAsync(string listName, string description, int templateID)
		{
			this.AddListAsync(listName, description, templateID, null);
		}

		public void AddListAsync(string listName, string description, int templateID, object userState)
		{
			if (this.AddListOperationCompleted == null)
			{
				this.AddListOperationCompleted = new SendOrPostCallback(this.OnAddListOperationCompleted);
			}
			base.InvokeAsync("AddList", new object[]
			{
				listName,
				description,
				templateID
			}, this.AddListOperationCompleted, userState);
		}

		private void OnAddListOperationCompleted(object arg)
		{
			if (this.AddListCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddListCompleted(this, new AddListCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/AddListFromFeature", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode AddListFromFeature(string listName, string description, Guid featureID, int templateID)
		{
			object[] array = base.Invoke("AddListFromFeature", new object[]
			{
				listName,
				description,
				featureID,
				templateID
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginAddListFromFeature(string listName, string description, Guid featureID, int templateID, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddListFromFeature", new object[]
			{
				listName,
				description,
				featureID,
				templateID
			}, callback, asyncState);
		}

		public XmlNode EndAddListFromFeature(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void AddListFromFeatureAsync(string listName, string description, Guid featureID, int templateID)
		{
			this.AddListFromFeatureAsync(listName, description, featureID, templateID, null);
		}

		public void AddListFromFeatureAsync(string listName, string description, Guid featureID, int templateID, object userState)
		{
			if (this.AddListFromFeatureOperationCompleted == null)
			{
				this.AddListFromFeatureOperationCompleted = new SendOrPostCallback(this.OnAddListFromFeatureOperationCompleted);
			}
			base.InvokeAsync("AddListFromFeature", new object[]
			{
				listName,
				description,
				featureID,
				templateID
			}, this.AddListFromFeatureOperationCompleted, userState);
		}

		private void OnAddListFromFeatureOperationCompleted(object arg)
		{
			if (this.AddListFromFeatureCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddListFromFeatureCompleted(this, new AddListFromFeatureCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateList", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode UpdateList(string listName, XmlNode listProperties, XmlNode newFields, XmlNode updateFields, XmlNode deleteFields, string listVersion)
		{
			object[] array = base.Invoke("UpdateList", new object[]
			{
				listName,
				listProperties,
				newFields,
				updateFields,
				deleteFields,
				listVersion
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginUpdateList(string listName, XmlNode listProperties, XmlNode newFields, XmlNode updateFields, XmlNode deleteFields, string listVersion, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateList", new object[]
			{
				listName,
				listProperties,
				newFields,
				updateFields,
				deleteFields,
				listVersion
			}, callback, asyncState);
		}

		public XmlNode EndUpdateList(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void UpdateListAsync(string listName, XmlNode listProperties, XmlNode newFields, XmlNode updateFields, XmlNode deleteFields, string listVersion)
		{
			this.UpdateListAsync(listName, listProperties, newFields, updateFields, deleteFields, listVersion, null);
		}

		public void UpdateListAsync(string listName, XmlNode listProperties, XmlNode newFields, XmlNode updateFields, XmlNode deleteFields, string listVersion, object userState)
		{
			if (this.UpdateListOperationCompleted == null)
			{
				this.UpdateListOperationCompleted = new SendOrPostCallback(this.OnUpdateListOperationCompleted);
			}
			base.InvokeAsync("UpdateList", new object[]
			{
				listName,
				listProperties,
				newFields,
				updateFields,
				deleteFields,
				listVersion
			}, this.UpdateListOperationCompleted, userState);
		}

		private void OnUpdateListOperationCompleted(object arg)
		{
			if (this.UpdateListCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateListCompleted(this, new UpdateListCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListCollection", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetListCollection()
		{
			object[] array = base.Invoke("GetListCollection", new object[0]);
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginGetListCollection(AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetListCollection", new object[0], callback, asyncState);
		}

		public XmlNode EndGetListCollection(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void GetListCollectionAsync()
		{
			this.GetListCollectionAsync(null);
		}

		public void GetListCollectionAsync(object userState)
		{
			if (this.GetListCollectionOperationCompleted == null)
			{
				this.GetListCollectionOperationCompleted = new SendOrPostCallback(this.OnGetListCollectionOperationCompleted);
			}
			base.InvokeAsync("GetListCollection", new object[0], this.GetListCollectionOperationCompleted, userState);
		}

		private void OnGetListCollectionOperationCompleted(object arg)
		{
			if (this.GetListCollectionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetListCollectionCompleted(this, new GetListCollectionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListItems", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetListItems(string listName, string viewName, XmlNode query, XmlNode viewFields, string rowLimit, XmlNode queryOptions, string webID)
		{
			object[] array = base.Invoke("GetListItems", new object[]
			{
				listName,
				viewName,
				query,
				viewFields,
				rowLimit,
				queryOptions,
				webID
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginGetListItems(string listName, string viewName, XmlNode query, XmlNode viewFields, string rowLimit, XmlNode queryOptions, string webID, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetListItems", new object[]
			{
				listName,
				viewName,
				query,
				viewFields,
				rowLimit,
				queryOptions,
				webID
			}, callback, asyncState);
		}

		public XmlNode EndGetListItems(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void GetListItemsAsync(string listName, string viewName, XmlNode query, XmlNode viewFields, string rowLimit, XmlNode queryOptions, string webID)
		{
			this.GetListItemsAsync(listName, viewName, query, viewFields, rowLimit, queryOptions, webID, null);
		}

		public void GetListItemsAsync(string listName, string viewName, XmlNode query, XmlNode viewFields, string rowLimit, XmlNode queryOptions, string webID, object userState)
		{
			if (this.GetListItemsOperationCompleted == null)
			{
				this.GetListItemsOperationCompleted = new SendOrPostCallback(this.OnGetListItemsOperationCompleted);
			}
			base.InvokeAsync("GetListItems", new object[]
			{
				listName,
				viewName,
				query,
				viewFields,
				rowLimit,
				queryOptions,
				webID
			}, this.GetListItemsOperationCompleted, userState);
		}

		private void OnGetListItemsOperationCompleted(object arg)
		{
			if (this.GetListItemsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetListItemsCompleted(this, new GetListItemsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListItemChanges", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetListItemChanges(string listName, XmlNode viewFields, string since, XmlNode contains)
		{
			object[] array = base.Invoke("GetListItemChanges", new object[]
			{
				listName,
				viewFields,
				since,
				contains
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginGetListItemChanges(string listName, XmlNode viewFields, string since, XmlNode contains, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetListItemChanges", new object[]
			{
				listName,
				viewFields,
				since,
				contains
			}, callback, asyncState);
		}

		public XmlNode EndGetListItemChanges(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void GetListItemChangesAsync(string listName, XmlNode viewFields, string since, XmlNode contains)
		{
			this.GetListItemChangesAsync(listName, viewFields, since, contains, null);
		}

		public void GetListItemChangesAsync(string listName, XmlNode viewFields, string since, XmlNode contains, object userState)
		{
			if (this.GetListItemChangesOperationCompleted == null)
			{
				this.GetListItemChangesOperationCompleted = new SendOrPostCallback(this.OnGetListItemChangesOperationCompleted);
			}
			base.InvokeAsync("GetListItemChanges", new object[]
			{
				listName,
				viewFields,
				since,
				contains
			}, this.GetListItemChangesOperationCompleted, userState);
		}

		private void OnGetListItemChangesOperationCompleted(object arg)
		{
			if (this.GetListItemChangesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetListItemChangesCompleted(this, new GetListItemChangesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListItemChangesWithKnowledge", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetListItemChangesWithKnowledge(string listName, string viewName, XmlNode query, XmlNode viewFields, string rowLimit, XmlNode queryOptions, string syncScope, XmlNode knowledge, XmlNode contains)
		{
			object[] array = base.Invoke("GetListItemChangesWithKnowledge", new object[]
			{
				listName,
				viewName,
				query,
				viewFields,
				rowLimit,
				queryOptions,
				syncScope,
				knowledge,
				contains
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginGetListItemChangesWithKnowledge(string listName, string viewName, XmlNode query, XmlNode viewFields, string rowLimit, XmlNode queryOptions, string syncScope, XmlNode knowledge, XmlNode contains, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetListItemChangesWithKnowledge", new object[]
			{
				listName,
				viewName,
				query,
				viewFields,
				rowLimit,
				queryOptions,
				syncScope,
				knowledge,
				contains
			}, callback, asyncState);
		}

		public XmlNode EndGetListItemChangesWithKnowledge(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void GetListItemChangesWithKnowledgeAsync(string listName, string viewName, XmlNode query, XmlNode viewFields, string rowLimit, XmlNode queryOptions, string syncScope, XmlNode knowledge, XmlNode contains)
		{
			this.GetListItemChangesWithKnowledgeAsync(listName, viewName, query, viewFields, rowLimit, queryOptions, syncScope, knowledge, contains, null);
		}

		public void GetListItemChangesWithKnowledgeAsync(string listName, string viewName, XmlNode query, XmlNode viewFields, string rowLimit, XmlNode queryOptions, string syncScope, XmlNode knowledge, XmlNode contains, object userState)
		{
			if (this.GetListItemChangesWithKnowledgeOperationCompleted == null)
			{
				this.GetListItemChangesWithKnowledgeOperationCompleted = new SendOrPostCallback(this.OnGetListItemChangesWithKnowledgeOperationCompleted);
			}
			base.InvokeAsync("GetListItemChangesWithKnowledge", new object[]
			{
				listName,
				viewName,
				query,
				viewFields,
				rowLimit,
				queryOptions,
				syncScope,
				knowledge,
				contains
			}, this.GetListItemChangesWithKnowledgeOperationCompleted, userState);
		}

		private void OnGetListItemChangesWithKnowledgeOperationCompleted(object arg)
		{
			if (this.GetListItemChangesWithKnowledgeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetListItemChangesWithKnowledgeCompleted(this, new GetListItemChangesWithKnowledgeCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListItemChangesSinceToken", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetListItemChangesSinceToken(string listName, string viewName, XmlNode query, XmlNode viewFields, string rowLimit, XmlNode queryOptions, string changeToken, XmlNode contains)
		{
			object[] array = base.Invoke("GetListItemChangesSinceToken", new object[]
			{
				listName,
				viewName,
				query,
				viewFields,
				rowLimit,
				queryOptions,
				changeToken,
				contains
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginGetListItemChangesSinceToken(string listName, string viewName, XmlNode query, XmlNode viewFields, string rowLimit, XmlNode queryOptions, string changeToken, XmlNode contains, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetListItemChangesSinceToken", new object[]
			{
				listName,
				viewName,
				query,
				viewFields,
				rowLimit,
				queryOptions,
				changeToken,
				contains
			}, callback, asyncState);
		}

		public XmlNode EndGetListItemChangesSinceToken(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void GetListItemChangesSinceTokenAsync(string listName, string viewName, XmlNode query, XmlNode viewFields, string rowLimit, XmlNode queryOptions, string changeToken, XmlNode contains)
		{
			this.GetListItemChangesSinceTokenAsync(listName, viewName, query, viewFields, rowLimit, queryOptions, changeToken, contains, null);
		}

		public void GetListItemChangesSinceTokenAsync(string listName, string viewName, XmlNode query, XmlNode viewFields, string rowLimit, XmlNode queryOptions, string changeToken, XmlNode contains, object userState)
		{
			if (this.GetListItemChangesSinceTokenOperationCompleted == null)
			{
				this.GetListItemChangesSinceTokenOperationCompleted = new SendOrPostCallback(this.OnGetListItemChangesSinceTokenOperationCompleted);
			}
			base.InvokeAsync("GetListItemChangesSinceToken", new object[]
			{
				listName,
				viewName,
				query,
				viewFields,
				rowLimit,
				queryOptions,
				changeToken,
				contains
			}, this.GetListItemChangesSinceTokenOperationCompleted, userState);
		}

		private void OnGetListItemChangesSinceTokenOperationCompleted(object arg)
		{
			if (this.GetListItemChangesSinceTokenCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetListItemChangesSinceTokenCompleted(this, new GetListItemChangesSinceTokenCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateListItems", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode UpdateListItems(string listName, XmlNode updates)
		{
			object[] array = base.Invoke("UpdateListItems", new object[]
			{
				listName,
				updates
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginUpdateListItems(string listName, XmlNode updates, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateListItems", new object[]
			{
				listName,
				updates
			}, callback, asyncState);
		}

		public XmlNode EndUpdateListItems(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void UpdateListItemsAsync(string listName, XmlNode updates)
		{
			this.UpdateListItemsAsync(listName, updates, null);
		}

		public void UpdateListItemsAsync(string listName, XmlNode updates, object userState)
		{
			if (this.UpdateListItemsOperationCompleted == null)
			{
				this.UpdateListItemsOperationCompleted = new SendOrPostCallback(this.OnUpdateListItemsOperationCompleted);
			}
			base.InvokeAsync("UpdateListItems", new object[]
			{
				listName,
				updates
			}, this.UpdateListItemsOperationCompleted, userState);
		}

		private void OnUpdateListItemsOperationCompleted(object arg)
		{
			if (this.UpdateListItemsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateListItemsCompleted(this, new UpdateListItemsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateListItemsWithKnowledge", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode UpdateListItemsWithKnowledge(string listName, XmlNode updates, string syncScope, XmlNode knowledge)
		{
			object[] array = base.Invoke("UpdateListItemsWithKnowledge", new object[]
			{
				listName,
				updates,
				syncScope,
				knowledge
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginUpdateListItemsWithKnowledge(string listName, XmlNode updates, string syncScope, XmlNode knowledge, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateListItemsWithKnowledge", new object[]
			{
				listName,
				updates,
				syncScope,
				knowledge
			}, callback, asyncState);
		}

		public XmlNode EndUpdateListItemsWithKnowledge(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void UpdateListItemsWithKnowledgeAsync(string listName, XmlNode updates, string syncScope, XmlNode knowledge)
		{
			this.UpdateListItemsWithKnowledgeAsync(listName, updates, syncScope, knowledge, null);
		}

		public void UpdateListItemsWithKnowledgeAsync(string listName, XmlNode updates, string syncScope, XmlNode knowledge, object userState)
		{
			if (this.UpdateListItemsWithKnowledgeOperationCompleted == null)
			{
				this.UpdateListItemsWithKnowledgeOperationCompleted = new SendOrPostCallback(this.OnUpdateListItemsWithKnowledgeOperationCompleted);
			}
			base.InvokeAsync("UpdateListItemsWithKnowledge", new object[]
			{
				listName,
				updates,
				syncScope,
				knowledge
			}, this.UpdateListItemsWithKnowledgeOperationCompleted, userState);
		}

		private void OnUpdateListItemsWithKnowledgeOperationCompleted(object arg)
		{
			if (this.UpdateListItemsWithKnowledgeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateListItemsWithKnowledgeCompleted(this, new UpdateListItemsWithKnowledgeCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/AddDiscussionBoardItem", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode AddDiscussionBoardItem(string listName, [XmlElement(DataType = "base64Binary")] byte[] message)
		{
			object[] array = base.Invoke("AddDiscussionBoardItem", new object[]
			{
				listName,
				message
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginAddDiscussionBoardItem(string listName, byte[] message, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddDiscussionBoardItem", new object[]
			{
				listName,
				message
			}, callback, asyncState);
		}

		public XmlNode EndAddDiscussionBoardItem(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void AddDiscussionBoardItemAsync(string listName, byte[] message)
		{
			this.AddDiscussionBoardItemAsync(listName, message, null);
		}

		public void AddDiscussionBoardItemAsync(string listName, byte[] message, object userState)
		{
			if (this.AddDiscussionBoardItemOperationCompleted == null)
			{
				this.AddDiscussionBoardItemOperationCompleted = new SendOrPostCallback(this.OnAddDiscussionBoardItemOperationCompleted);
			}
			base.InvokeAsync("AddDiscussionBoardItem", new object[]
			{
				listName,
				message
			}, this.AddDiscussionBoardItemOperationCompleted, userState);
		}

		private void OnAddDiscussionBoardItemOperationCompleted(object arg)
		{
			if (this.AddDiscussionBoardItemCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddDiscussionBoardItemCompleted(this, new AddDiscussionBoardItemCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/AddWikiPage", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode AddWikiPage(string strListName, string listRelPageUrl, string wikiContent)
		{
			object[] array = base.Invoke("AddWikiPage", new object[]
			{
				strListName,
				listRelPageUrl,
				wikiContent
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginAddWikiPage(string strListName, string listRelPageUrl, string wikiContent, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddWikiPage", new object[]
			{
				strListName,
				listRelPageUrl,
				wikiContent
			}, callback, asyncState);
		}

		public XmlNode EndAddWikiPage(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void AddWikiPageAsync(string strListName, string listRelPageUrl, string wikiContent)
		{
			this.AddWikiPageAsync(strListName, listRelPageUrl, wikiContent, null);
		}

		public void AddWikiPageAsync(string strListName, string listRelPageUrl, string wikiContent, object userState)
		{
			if (this.AddWikiPageOperationCompleted == null)
			{
				this.AddWikiPageOperationCompleted = new SendOrPostCallback(this.OnAddWikiPageOperationCompleted);
			}
			base.InvokeAsync("AddWikiPage", new object[]
			{
				strListName,
				listRelPageUrl,
				wikiContent
			}, this.AddWikiPageOperationCompleted, userState);
		}

		private void OnAddWikiPageOperationCompleted(object arg)
		{
			if (this.AddWikiPageCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddWikiPageCompleted(this, new AddWikiPageCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetVersionCollection", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetVersionCollection(string strlistID, string strlistItemID, string strFieldName)
		{
			object[] array = base.Invoke("GetVersionCollection", new object[]
			{
				strlistID,
				strlistItemID,
				strFieldName
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginGetVersionCollection(string strlistID, string strlistItemID, string strFieldName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetVersionCollection", new object[]
			{
				strlistID,
				strlistItemID,
				strFieldName
			}, callback, asyncState);
		}

		public XmlNode EndGetVersionCollection(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void GetVersionCollectionAsync(string strlistID, string strlistItemID, string strFieldName)
		{
			this.GetVersionCollectionAsync(strlistID, strlistItemID, strFieldName, null);
		}

		public void GetVersionCollectionAsync(string strlistID, string strlistItemID, string strFieldName, object userState)
		{
			if (this.GetVersionCollectionOperationCompleted == null)
			{
				this.GetVersionCollectionOperationCompleted = new SendOrPostCallback(this.OnGetVersionCollectionOperationCompleted);
			}
			base.InvokeAsync("GetVersionCollection", new object[]
			{
				strlistID,
				strlistItemID,
				strFieldName
			}, this.GetVersionCollectionOperationCompleted, userState);
		}

		private void OnGetVersionCollectionOperationCompleted(object arg)
		{
			if (this.GetVersionCollectionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetVersionCollectionCompleted(this, new GetVersionCollectionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/AddAttachment", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string AddAttachment(string listName, string listItemID, string fileName, [XmlElement(DataType = "base64Binary")] byte[] attachment)
		{
			object[] array = base.Invoke("AddAttachment", new object[]
			{
				listName,
				listItemID,
				fileName,
				attachment
			});
			return (string)array[0];
		}

		public IAsyncResult BeginAddAttachment(string listName, string listItemID, string fileName, byte[] attachment, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddAttachment", new object[]
			{
				listName,
				listItemID,
				fileName,
				attachment
			}, callback, asyncState);
		}

		public string EndAddAttachment(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (string)array[0];
		}

		public void AddAttachmentAsync(string listName, string listItemID, string fileName, byte[] attachment)
		{
			this.AddAttachmentAsync(listName, listItemID, fileName, attachment, null);
		}

		public void AddAttachmentAsync(string listName, string listItemID, string fileName, byte[] attachment, object userState)
		{
			if (this.AddAttachmentOperationCompleted == null)
			{
				this.AddAttachmentOperationCompleted = new SendOrPostCallback(this.OnAddAttachmentOperationCompleted);
			}
			base.InvokeAsync("AddAttachment", new object[]
			{
				listName,
				listItemID,
				fileName,
				attachment
			}, this.AddAttachmentOperationCompleted, userState);
		}

		private void OnAddAttachmentOperationCompleted(object arg)
		{
			if (this.AddAttachmentCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddAttachmentCompleted(this, new AddAttachmentCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetAttachmentCollection", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetAttachmentCollection(string listName, string listItemID)
		{
			object[] array = base.Invoke("GetAttachmentCollection", new object[]
			{
				listName,
				listItemID
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginGetAttachmentCollection(string listName, string listItemID, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetAttachmentCollection", new object[]
			{
				listName,
				listItemID
			}, callback, asyncState);
		}

		public XmlNode EndGetAttachmentCollection(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void GetAttachmentCollectionAsync(string listName, string listItemID)
		{
			this.GetAttachmentCollectionAsync(listName, listItemID, null);
		}

		public void GetAttachmentCollectionAsync(string listName, string listItemID, object userState)
		{
			if (this.GetAttachmentCollectionOperationCompleted == null)
			{
				this.GetAttachmentCollectionOperationCompleted = new SendOrPostCallback(this.OnGetAttachmentCollectionOperationCompleted);
			}
			base.InvokeAsync("GetAttachmentCollection", new object[]
			{
				listName,
				listItemID
			}, this.GetAttachmentCollectionOperationCompleted, userState);
		}

		private void OnGetAttachmentCollectionOperationCompleted(object arg)
		{
			if (this.GetAttachmentCollectionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetAttachmentCollectionCompleted(this, new GetAttachmentCollectionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/DeleteAttachment", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void DeleteAttachment(string listName, string listItemID, string url)
		{
			base.Invoke("DeleteAttachment", new object[]
			{
				listName,
				listItemID,
				url
			});
		}

		public IAsyncResult BeginDeleteAttachment(string listName, string listItemID, string url, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DeleteAttachment", new object[]
			{
				listName,
				listItemID,
				url
			}, callback, asyncState);
		}

		public void EndDeleteAttachment(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void DeleteAttachmentAsync(string listName, string listItemID, string url)
		{
			this.DeleteAttachmentAsync(listName, listItemID, url, null);
		}

		public void DeleteAttachmentAsync(string listName, string listItemID, string url, object userState)
		{
			if (this.DeleteAttachmentOperationCompleted == null)
			{
				this.DeleteAttachmentOperationCompleted = new SendOrPostCallback(this.OnDeleteAttachmentOperationCompleted);
			}
			base.InvokeAsync("DeleteAttachment", new object[]
			{
				listName,
				listItemID,
				url
			}, this.DeleteAttachmentOperationCompleted, userState);
		}

		private void OnDeleteAttachmentOperationCompleted(object arg)
		{
			if (this.DeleteAttachmentCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeleteAttachmentCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/CheckOutFile", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool CheckOutFile(string pageUrl, string checkoutToLocal, string lastmodified)
		{
			object[] array = base.Invoke("CheckOutFile", new object[]
			{
				pageUrl,
				checkoutToLocal,
				lastmodified
			});
			return (bool)array[0];
		}

		public IAsyncResult BeginCheckOutFile(string pageUrl, string checkoutToLocal, string lastmodified, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CheckOutFile", new object[]
			{
				pageUrl,
				checkoutToLocal,
				lastmodified
			}, callback, asyncState);
		}

		public bool EndCheckOutFile(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (bool)array[0];
		}

		public void CheckOutFileAsync(string pageUrl, string checkoutToLocal, string lastmodified)
		{
			this.CheckOutFileAsync(pageUrl, checkoutToLocal, lastmodified, null);
		}

		public void CheckOutFileAsync(string pageUrl, string checkoutToLocal, string lastmodified, object userState)
		{
			if (this.CheckOutFileOperationCompleted == null)
			{
				this.CheckOutFileOperationCompleted = new SendOrPostCallback(this.OnCheckOutFileOperationCompleted);
			}
			base.InvokeAsync("CheckOutFile", new object[]
			{
				pageUrl,
				checkoutToLocal,
				lastmodified
			}, this.CheckOutFileOperationCompleted, userState);
		}

		private void OnCheckOutFileOperationCompleted(object arg)
		{
			if (this.CheckOutFileCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CheckOutFileCompleted(this, new CheckOutFileCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UndoCheckOut", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool UndoCheckOut(string pageUrl)
		{
			object[] array = base.Invoke("UndoCheckOut", new object[]
			{
				pageUrl
			});
			return (bool)array[0];
		}

		public IAsyncResult BeginUndoCheckOut(string pageUrl, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UndoCheckOut", new object[]
			{
				pageUrl
			}, callback, asyncState);
		}

		public bool EndUndoCheckOut(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (bool)array[0];
		}

		public void UndoCheckOutAsync(string pageUrl)
		{
			this.UndoCheckOutAsync(pageUrl, null);
		}

		public void UndoCheckOutAsync(string pageUrl, object userState)
		{
			if (this.UndoCheckOutOperationCompleted == null)
			{
				this.UndoCheckOutOperationCompleted = new SendOrPostCallback(this.OnUndoCheckOutOperationCompleted);
			}
			base.InvokeAsync("UndoCheckOut", new object[]
			{
				pageUrl
			}, this.UndoCheckOutOperationCompleted, userState);
		}

		private void OnUndoCheckOutOperationCompleted(object arg)
		{
			if (this.UndoCheckOutCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UndoCheckOutCompleted(this, new UndoCheckOutCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/CheckInFile", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool CheckInFile(string pageUrl, string comment, string CheckinType)
		{
			object[] array = base.Invoke("CheckInFile", new object[]
			{
				pageUrl,
				comment,
				CheckinType
			});
			return (bool)array[0];
		}

		public IAsyncResult BeginCheckInFile(string pageUrl, string comment, string CheckinType, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CheckInFile", new object[]
			{
				pageUrl,
				comment,
				CheckinType
			}, callback, asyncState);
		}

		public bool EndCheckInFile(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (bool)array[0];
		}

		public void CheckInFileAsync(string pageUrl, string comment, string CheckinType)
		{
			this.CheckInFileAsync(pageUrl, comment, CheckinType, null);
		}

		public void CheckInFileAsync(string pageUrl, string comment, string CheckinType, object userState)
		{
			if (this.CheckInFileOperationCompleted == null)
			{
				this.CheckInFileOperationCompleted = new SendOrPostCallback(this.OnCheckInFileOperationCompleted);
			}
			base.InvokeAsync("CheckInFile", new object[]
			{
				pageUrl,
				comment,
				CheckinType
			}, this.CheckInFileOperationCompleted, userState);
		}

		private void OnCheckInFileOperationCompleted(object arg)
		{
			if (this.CheckInFileCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CheckInFileCompleted(this, new CheckInFileCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListContentTypes", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetListContentTypes(string listName, string contentTypeId)
		{
			object[] array = base.Invoke("GetListContentTypes", new object[]
			{
				listName,
				contentTypeId
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginGetListContentTypes(string listName, string contentTypeId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetListContentTypes", new object[]
			{
				listName,
				contentTypeId
			}, callback, asyncState);
		}

		public XmlNode EndGetListContentTypes(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void GetListContentTypesAsync(string listName, string contentTypeId)
		{
			this.GetListContentTypesAsync(listName, contentTypeId, null);
		}

		public void GetListContentTypesAsync(string listName, string contentTypeId, object userState)
		{
			if (this.GetListContentTypesOperationCompleted == null)
			{
				this.GetListContentTypesOperationCompleted = new SendOrPostCallback(this.OnGetListContentTypesOperationCompleted);
			}
			base.InvokeAsync("GetListContentTypes", new object[]
			{
				listName,
				contentTypeId
			}, this.GetListContentTypesOperationCompleted, userState);
		}

		private void OnGetListContentTypesOperationCompleted(object arg)
		{
			if (this.GetListContentTypesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetListContentTypesCompleted(this, new GetListContentTypesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListContentTypesAndProperties", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetListContentTypesAndProperties(string listName, string contentTypeId, string propertyPrefix, bool includeWebProperties, [XmlIgnore] bool includeWebPropertiesSpecified)
		{
			object[] array = base.Invoke("GetListContentTypesAndProperties", new object[]
			{
				listName,
				contentTypeId,
				propertyPrefix,
				includeWebProperties,
				includeWebPropertiesSpecified
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginGetListContentTypesAndProperties(string listName, string contentTypeId, string propertyPrefix, bool includeWebProperties, bool includeWebPropertiesSpecified, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetListContentTypesAndProperties", new object[]
			{
				listName,
				contentTypeId,
				propertyPrefix,
				includeWebProperties,
				includeWebPropertiesSpecified
			}, callback, asyncState);
		}

		public XmlNode EndGetListContentTypesAndProperties(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void GetListContentTypesAndPropertiesAsync(string listName, string contentTypeId, string propertyPrefix, bool includeWebProperties, bool includeWebPropertiesSpecified)
		{
			this.GetListContentTypesAndPropertiesAsync(listName, contentTypeId, propertyPrefix, includeWebProperties, includeWebPropertiesSpecified, null);
		}

		public void GetListContentTypesAndPropertiesAsync(string listName, string contentTypeId, string propertyPrefix, bool includeWebProperties, bool includeWebPropertiesSpecified, object userState)
		{
			if (this.GetListContentTypesAndPropertiesOperationCompleted == null)
			{
				this.GetListContentTypesAndPropertiesOperationCompleted = new SendOrPostCallback(this.OnGetListContentTypesAndPropertiesOperationCompleted);
			}
			base.InvokeAsync("GetListContentTypesAndProperties", new object[]
			{
				listName,
				contentTypeId,
				propertyPrefix,
				includeWebProperties,
				includeWebPropertiesSpecified
			}, this.GetListContentTypesAndPropertiesOperationCompleted, userState);
		}

		private void OnGetListContentTypesAndPropertiesOperationCompleted(object arg)
		{
			if (this.GetListContentTypesAndPropertiesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetListContentTypesAndPropertiesCompleted(this, new GetListContentTypesAndPropertiesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListContentType", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetListContentType(string listName, string contentTypeId)
		{
			object[] array = base.Invoke("GetListContentType", new object[]
			{
				listName,
				contentTypeId
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginGetListContentType(string listName, string contentTypeId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetListContentType", new object[]
			{
				listName,
				contentTypeId
			}, callback, asyncState);
		}

		public XmlNode EndGetListContentType(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void GetListContentTypeAsync(string listName, string contentTypeId)
		{
			this.GetListContentTypeAsync(listName, contentTypeId, null);
		}

		public void GetListContentTypeAsync(string listName, string contentTypeId, object userState)
		{
			if (this.GetListContentTypeOperationCompleted == null)
			{
				this.GetListContentTypeOperationCompleted = new SendOrPostCallback(this.OnGetListContentTypeOperationCompleted);
			}
			base.InvokeAsync("GetListContentType", new object[]
			{
				listName,
				contentTypeId
			}, this.GetListContentTypeOperationCompleted, userState);
		}

		private void OnGetListContentTypeOperationCompleted(object arg)
		{
			if (this.GetListContentTypeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetListContentTypeCompleted(this, new GetListContentTypeCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/CreateContentType", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string CreateContentType(string listName, string displayName, string parentType, XmlNode fields, XmlNode contentTypeProperties, string addToView)
		{
			object[] array = base.Invoke("CreateContentType", new object[]
			{
				listName,
				displayName,
				parentType,
				fields,
				contentTypeProperties,
				addToView
			});
			return (string)array[0];
		}

		public IAsyncResult BeginCreateContentType(string listName, string displayName, string parentType, XmlNode fields, XmlNode contentTypeProperties, string addToView, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateContentType", new object[]
			{
				listName,
				displayName,
				parentType,
				fields,
				contentTypeProperties,
				addToView
			}, callback, asyncState);
		}

		public string EndCreateContentType(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (string)array[0];
		}

		public void CreateContentTypeAsync(string listName, string displayName, string parentType, XmlNode fields, XmlNode contentTypeProperties, string addToView)
		{
			this.CreateContentTypeAsync(listName, displayName, parentType, fields, contentTypeProperties, addToView, null);
		}

		public void CreateContentTypeAsync(string listName, string displayName, string parentType, XmlNode fields, XmlNode contentTypeProperties, string addToView, object userState)
		{
			if (this.CreateContentTypeOperationCompleted == null)
			{
				this.CreateContentTypeOperationCompleted = new SendOrPostCallback(this.OnCreateContentTypeOperationCompleted);
			}
			base.InvokeAsync("CreateContentType", new object[]
			{
				listName,
				displayName,
				parentType,
				fields,
				contentTypeProperties,
				addToView
			}, this.CreateContentTypeOperationCompleted, userState);
		}

		private void OnCreateContentTypeOperationCompleted(object arg)
		{
			if (this.CreateContentTypeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateContentTypeCompleted(this, new CreateContentTypeCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateContentType", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode UpdateContentType(string listName, string contentTypeId, XmlNode contentTypeProperties, XmlNode newFields, XmlNode updateFields, XmlNode deleteFields, string addToView)
		{
			object[] array = base.Invoke("UpdateContentType", new object[]
			{
				listName,
				contentTypeId,
				contentTypeProperties,
				newFields,
				updateFields,
				deleteFields,
				addToView
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginUpdateContentType(string listName, string contentTypeId, XmlNode contentTypeProperties, XmlNode newFields, XmlNode updateFields, XmlNode deleteFields, string addToView, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateContentType", new object[]
			{
				listName,
				contentTypeId,
				contentTypeProperties,
				newFields,
				updateFields,
				deleteFields,
				addToView
			}, callback, asyncState);
		}

		public XmlNode EndUpdateContentType(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void UpdateContentTypeAsync(string listName, string contentTypeId, XmlNode contentTypeProperties, XmlNode newFields, XmlNode updateFields, XmlNode deleteFields, string addToView)
		{
			this.UpdateContentTypeAsync(listName, contentTypeId, contentTypeProperties, newFields, updateFields, deleteFields, addToView, null);
		}

		public void UpdateContentTypeAsync(string listName, string contentTypeId, XmlNode contentTypeProperties, XmlNode newFields, XmlNode updateFields, XmlNode deleteFields, string addToView, object userState)
		{
			if (this.UpdateContentTypeOperationCompleted == null)
			{
				this.UpdateContentTypeOperationCompleted = new SendOrPostCallback(this.OnUpdateContentTypeOperationCompleted);
			}
			base.InvokeAsync("UpdateContentType", new object[]
			{
				listName,
				contentTypeId,
				contentTypeProperties,
				newFields,
				updateFields,
				deleteFields,
				addToView
			}, this.UpdateContentTypeOperationCompleted, userState);
		}

		private void OnUpdateContentTypeOperationCompleted(object arg)
		{
			if (this.UpdateContentTypeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateContentTypeCompleted(this, new UpdateContentTypeCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/DeleteContentType", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode DeleteContentType(string listName, string contentTypeId)
		{
			object[] array = base.Invoke("DeleteContentType", new object[]
			{
				listName,
				contentTypeId
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginDeleteContentType(string listName, string contentTypeId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DeleteContentType", new object[]
			{
				listName,
				contentTypeId
			}, callback, asyncState);
		}

		public XmlNode EndDeleteContentType(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void DeleteContentTypeAsync(string listName, string contentTypeId)
		{
			this.DeleteContentTypeAsync(listName, contentTypeId, null);
		}

		public void DeleteContentTypeAsync(string listName, string contentTypeId, object userState)
		{
			if (this.DeleteContentTypeOperationCompleted == null)
			{
				this.DeleteContentTypeOperationCompleted = new SendOrPostCallback(this.OnDeleteContentTypeOperationCompleted);
			}
			base.InvokeAsync("DeleteContentType", new object[]
			{
				listName,
				contentTypeId
			}, this.DeleteContentTypeOperationCompleted, userState);
		}

		private void OnDeleteContentTypeOperationCompleted(object arg)
		{
			if (this.DeleteContentTypeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeleteContentTypeCompleted(this, new DeleteContentTypeCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateContentTypeXmlDocument", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode UpdateContentTypeXmlDocument(string listName, string contentTypeId, XmlNode newDocument)
		{
			object[] array = base.Invoke("UpdateContentTypeXmlDocument", new object[]
			{
				listName,
				contentTypeId,
				newDocument
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginUpdateContentTypeXmlDocument(string listName, string contentTypeId, XmlNode newDocument, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateContentTypeXmlDocument", new object[]
			{
				listName,
				contentTypeId,
				newDocument
			}, callback, asyncState);
		}

		public XmlNode EndUpdateContentTypeXmlDocument(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void UpdateContentTypeXmlDocumentAsync(string listName, string contentTypeId, XmlNode newDocument)
		{
			this.UpdateContentTypeXmlDocumentAsync(listName, contentTypeId, newDocument, null);
		}

		public void UpdateContentTypeXmlDocumentAsync(string listName, string contentTypeId, XmlNode newDocument, object userState)
		{
			if (this.UpdateContentTypeXmlDocumentOperationCompleted == null)
			{
				this.UpdateContentTypeXmlDocumentOperationCompleted = new SendOrPostCallback(this.OnUpdateContentTypeXmlDocumentOperationCompleted);
			}
			base.InvokeAsync("UpdateContentTypeXmlDocument", new object[]
			{
				listName,
				contentTypeId,
				newDocument
			}, this.UpdateContentTypeXmlDocumentOperationCompleted, userState);
		}

		private void OnUpdateContentTypeXmlDocumentOperationCompleted(object arg)
		{
			if (this.UpdateContentTypeXmlDocumentCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateContentTypeXmlDocumentCompleted(this, new UpdateContentTypeXmlDocumentCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateContentTypesXmlDocument", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode UpdateContentTypesXmlDocument(string listName, XmlNode newDocument)
		{
			object[] array = base.Invoke("UpdateContentTypesXmlDocument", new object[]
			{
				listName,
				newDocument
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginUpdateContentTypesXmlDocument(string listName, XmlNode newDocument, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateContentTypesXmlDocument", new object[]
			{
				listName,
				newDocument
			}, callback, asyncState);
		}

		public XmlNode EndUpdateContentTypesXmlDocument(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void UpdateContentTypesXmlDocumentAsync(string listName, XmlNode newDocument)
		{
			this.UpdateContentTypesXmlDocumentAsync(listName, newDocument, null);
		}

		public void UpdateContentTypesXmlDocumentAsync(string listName, XmlNode newDocument, object userState)
		{
			if (this.UpdateContentTypesXmlDocumentOperationCompleted == null)
			{
				this.UpdateContentTypesXmlDocumentOperationCompleted = new SendOrPostCallback(this.OnUpdateContentTypesXmlDocumentOperationCompleted);
			}
			base.InvokeAsync("UpdateContentTypesXmlDocument", new object[]
			{
				listName,
				newDocument
			}, this.UpdateContentTypesXmlDocumentOperationCompleted, userState);
		}

		private void OnUpdateContentTypesXmlDocumentOperationCompleted(object arg)
		{
			if (this.UpdateContentTypesXmlDocumentCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateContentTypesXmlDocumentCompleted(this, new UpdateContentTypesXmlDocumentCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/DeleteContentTypeXmlDocument", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode DeleteContentTypeXmlDocument(string listName, string contentTypeId, string documentUri)
		{
			object[] array = base.Invoke("DeleteContentTypeXmlDocument", new object[]
			{
				listName,
				contentTypeId,
				documentUri
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginDeleteContentTypeXmlDocument(string listName, string contentTypeId, string documentUri, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DeleteContentTypeXmlDocument", new object[]
			{
				listName,
				contentTypeId,
				documentUri
			}, callback, asyncState);
		}

		public XmlNode EndDeleteContentTypeXmlDocument(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void DeleteContentTypeXmlDocumentAsync(string listName, string contentTypeId, string documentUri)
		{
			this.DeleteContentTypeXmlDocumentAsync(listName, contentTypeId, documentUri, null);
		}

		public void DeleteContentTypeXmlDocumentAsync(string listName, string contentTypeId, string documentUri, object userState)
		{
			if (this.DeleteContentTypeXmlDocumentOperationCompleted == null)
			{
				this.DeleteContentTypeXmlDocumentOperationCompleted = new SendOrPostCallback(this.OnDeleteContentTypeXmlDocumentOperationCompleted);
			}
			base.InvokeAsync("DeleteContentTypeXmlDocument", new object[]
			{
				listName,
				contentTypeId,
				documentUri
			}, this.DeleteContentTypeXmlDocumentOperationCompleted, userState);
		}

		private void OnDeleteContentTypeXmlDocumentOperationCompleted(object arg)
		{
			if (this.DeleteContentTypeXmlDocumentCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeleteContentTypeXmlDocumentCompleted(this, new DeleteContentTypeXmlDocumentCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/ApplyContentTypeToList", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode ApplyContentTypeToList(string webUrl, string contentTypeId, string listName)
		{
			object[] array = base.Invoke("ApplyContentTypeToList", new object[]
			{
				webUrl,
				contentTypeId,
				listName
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginApplyContentTypeToList(string webUrl, string contentTypeId, string listName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ApplyContentTypeToList", new object[]
			{
				webUrl,
				contentTypeId,
				listName
			}, callback, asyncState);
		}

		public XmlNode EndApplyContentTypeToList(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void ApplyContentTypeToListAsync(string webUrl, string contentTypeId, string listName)
		{
			this.ApplyContentTypeToListAsync(webUrl, contentTypeId, listName, null);
		}

		public void ApplyContentTypeToListAsync(string webUrl, string contentTypeId, string listName, object userState)
		{
			if (this.ApplyContentTypeToListOperationCompleted == null)
			{
				this.ApplyContentTypeToListOperationCompleted = new SendOrPostCallback(this.OnApplyContentTypeToListOperationCompleted);
			}
			base.InvokeAsync("ApplyContentTypeToList", new object[]
			{
				webUrl,
				contentTypeId,
				listName
			}, this.ApplyContentTypeToListOperationCompleted, userState);
		}

		private void OnApplyContentTypeToListOperationCompleted(object arg)
		{
			if (this.ApplyContentTypeToListCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ApplyContentTypeToListCompleted(this, new ApplyContentTypeToListCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		private SendOrPostCallback GetListOperationCompleted;

		private SendOrPostCallback GetListAndViewOperationCompleted;

		private SendOrPostCallback DeleteListOperationCompleted;

		private SendOrPostCallback AddListOperationCompleted;

		private SendOrPostCallback AddListFromFeatureOperationCompleted;

		private SendOrPostCallback UpdateListOperationCompleted;

		private SendOrPostCallback GetListCollectionOperationCompleted;

		private SendOrPostCallback GetListItemsOperationCompleted;

		private SendOrPostCallback GetListItemChangesOperationCompleted;

		private SendOrPostCallback GetListItemChangesWithKnowledgeOperationCompleted;

		private SendOrPostCallback GetListItemChangesSinceTokenOperationCompleted;

		private SendOrPostCallback UpdateListItemsOperationCompleted;

		private SendOrPostCallback UpdateListItemsWithKnowledgeOperationCompleted;

		private SendOrPostCallback AddDiscussionBoardItemOperationCompleted;

		private SendOrPostCallback AddWikiPageOperationCompleted;

		private SendOrPostCallback GetVersionCollectionOperationCompleted;

		private SendOrPostCallback AddAttachmentOperationCompleted;

		private SendOrPostCallback GetAttachmentCollectionOperationCompleted;

		private SendOrPostCallback DeleteAttachmentOperationCompleted;

		private SendOrPostCallback CheckOutFileOperationCompleted;

		private SendOrPostCallback UndoCheckOutOperationCompleted;

		private SendOrPostCallback CheckInFileOperationCompleted;

		private SendOrPostCallback GetListContentTypesOperationCompleted;

		private SendOrPostCallback GetListContentTypesAndPropertiesOperationCompleted;

		private SendOrPostCallback GetListContentTypeOperationCompleted;

		private SendOrPostCallback CreateContentTypeOperationCompleted;

		private SendOrPostCallback UpdateContentTypeOperationCompleted;

		private SendOrPostCallback DeleteContentTypeOperationCompleted;

		private SendOrPostCallback UpdateContentTypeXmlDocumentOperationCompleted;

		private SendOrPostCallback UpdateContentTypesXmlDocumentOperationCompleted;

		private SendOrPostCallback DeleteContentTypeXmlDocumentOperationCompleted;

		private SendOrPostCallback ApplyContentTypeToListOperationCompleted;
	}
}
