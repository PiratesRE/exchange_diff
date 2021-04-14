using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Security;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[CLSCompliant(false)]
	[XmlInclude(typeof(BaseFolderIdType))]
	[XmlInclude(typeof(BaseRequestType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(BaseCalendarItemStateDefinitionType))]
	[XmlInclude(typeof(RuleOperationType))]
	[XmlInclude(typeof(BaseEmailAddressType))]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[WebServiceBinding(Name = "PrivateExchangeServiceBinding", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlInclude(typeof(AttendeeConflictData))]
	[XmlInclude(typeof(ServiceConfiguration))]
	[XmlInclude(typeof(DirectoryEntryType))]
	[XmlInclude(typeof(BaseResponseMessageType))]
	[XmlInclude(typeof(RecurrencePatternBaseType))]
	[XmlInclude(typeof(BaseSubscriptionRequestType))]
	[XmlInclude(typeof(MailboxLocatorType))]
	[XmlInclude(typeof(BaseGroupByType))]
	[XmlInclude(typeof(RecurrenceRangeBaseType))]
	[XmlInclude(typeof(BasePagingType))]
	[XmlInclude(typeof(BaseItemIdType))]
	[XmlInclude(typeof(ChangeDescriptionType))]
	[XmlInclude(typeof(AttachmentType))]
	[XmlInclude(typeof(BasePermissionType))]
	[XmlInclude(typeof(BaseFolderType))]
	public class PrivateExchangeServiceBinding : CustomSoapHttpClientProtocol
	{
		public PrivateExchangeServiceBinding()
		{
		}

		public event CreateUMPromptCompletedEventHandler CreateUMPromptCompleted;

		public event DeleteUMPromptsCompletedEventHandler DeleteUMPromptsCompleted;

		public event GetUMPromptCompletedEventHandler GetUMPromptCompleted;

		public event GetUMPromptNamesCompletedEventHandler GetUMPromptNamesCompleted;

		public event GetClientExtensionCompletedEventHandler GetClientExtensionCompleted;

		public event SetClientExtensionCompletedEventHandler SetClientExtensionCompleted;

		public event StartFindInGALSpeechRecognitionCompletedEventHandler StartFindInGALSpeechRecognitionCompleted;

		public event CompleteFindInGALSpeechRecognitionCompletedEventHandler CompleteFindInGALSpeechRecognitionCompleted;

		public event CreateUMCallDataRecordCompletedEventHandler CreateUMCallDataRecordCompleted;

		public event GetUMCallDataRecordsCompletedEventHandler GetUMCallDataRecordsCompleted;

		public event GetUMCallSummaryCompletedEventHandler GetUMCallSummaryCompleted;

		public event InitUMMailboxCompletedEventHandler InitUMMailboxCompleted;

		public event ResetUMMailboxCompletedEventHandler ResetUMMailboxCompleted;

		public event ValidateUMPinCompletedEventHandler ValidateUMPinCompleted;

		public event SaveUMPinCompletedEventHandler SaveUMPinCompleted;

		public event GetUMPinCompletedEventHandler GetUMPinCompleted;

		public event GetClientIntentCompletedEventHandler GetClientIntentCompleted;

		public event GetUMSubscriberCallAnsweringDataCompletedEventHandler GetUMSubscriberCallAnsweringDataCompleted;

		public event UpdateMailboxAssociationCompletedEventHandler UpdateMailboxAssociationCompleted;

		public event UpdateGroupMailboxCompletedEventHandler UpdateGroupMailboxCompleted;

		public event PostModernGroupItemCompletedEventHandler PostModernGroupItemCompleted;

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/CreateUMPrompt", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("CreateUMPromptResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public CreateUMPromptResponseMessageType CreateUMPrompt([XmlElement("CreateUMPrompt", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] CreateUMPromptType CreateUMPrompt1)
		{
			object[] array = this.Invoke("CreateUMPrompt", new object[]
			{
				CreateUMPrompt1
			});
			return (CreateUMPromptResponseMessageType)array[0];
		}

		public IAsyncResult BeginCreateUMPrompt(CreateUMPromptType CreateUMPrompt1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateUMPrompt", new object[]
			{
				CreateUMPrompt1
			}, callback, asyncState);
		}

		public CreateUMPromptResponseMessageType EndCreateUMPrompt(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CreateUMPromptResponseMessageType)array[0];
		}

		public void CreateUMPromptAsync(CreateUMPromptType CreateUMPrompt1)
		{
			this.CreateUMPromptAsync(CreateUMPrompt1, null);
		}

		public void CreateUMPromptAsync(CreateUMPromptType CreateUMPrompt1, object userState)
		{
			if (this.CreateUMPromptOperationCompleted == null)
			{
				this.CreateUMPromptOperationCompleted = new SendOrPostCallback(this.OnCreateUMPromptOperationCompleted);
			}
			base.InvokeAsync("CreateUMPrompt", new object[]
			{
				CreateUMPrompt1
			}, this.CreateUMPromptOperationCompleted, userState);
		}

		private void OnCreateUMPromptOperationCompleted(object arg)
		{
			if (this.CreateUMPromptCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateUMPromptCompleted(this, new CreateUMPromptCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/DeleteUMPrompts", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("DeleteUMPromptsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public DeleteUMPromptsResponseMessageType DeleteUMPrompts([XmlElement("DeleteUMPrompts", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] DeleteUMPromptsType DeleteUMPrompts1)
		{
			object[] array = this.Invoke("DeleteUMPrompts", new object[]
			{
				DeleteUMPrompts1
			});
			return (DeleteUMPromptsResponseMessageType)array[0];
		}

		public IAsyncResult BeginDeleteUMPrompts(DeleteUMPromptsType DeleteUMPrompts1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DeleteUMPrompts", new object[]
			{
				DeleteUMPrompts1
			}, callback, asyncState);
		}

		public DeleteUMPromptsResponseMessageType EndDeleteUMPrompts(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DeleteUMPromptsResponseMessageType)array[0];
		}

		public void DeleteUMPromptsAsync(DeleteUMPromptsType DeleteUMPrompts1)
		{
			this.DeleteUMPromptsAsync(DeleteUMPrompts1, null);
		}

		public void DeleteUMPromptsAsync(DeleteUMPromptsType DeleteUMPrompts1, object userState)
		{
			if (this.DeleteUMPromptsOperationCompleted == null)
			{
				this.DeleteUMPromptsOperationCompleted = new SendOrPostCallback(this.OnDeleteUMPromptsOperationCompleted);
			}
			base.InvokeAsync("DeleteUMPrompts", new object[]
			{
				DeleteUMPrompts1
			}, this.DeleteUMPromptsOperationCompleted, userState);
		}

		private void OnDeleteUMPromptsOperationCompleted(object arg)
		{
			if (this.DeleteUMPromptsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeleteUMPromptsCompleted(this, new DeleteUMPromptsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetUMPrompt", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetUMPromptResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetUMPromptResponseMessageType GetUMPrompt([XmlElement("GetUMPrompt", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetUMPromptType GetUMPrompt1)
		{
			object[] array = this.Invoke("GetUMPrompt", new object[]
			{
				GetUMPrompt1
			});
			return (GetUMPromptResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetUMPrompt(GetUMPromptType GetUMPrompt1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetUMPrompt", new object[]
			{
				GetUMPrompt1
			}, callback, asyncState);
		}

		public GetUMPromptResponseMessageType EndGetUMPrompt(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetUMPromptResponseMessageType)array[0];
		}

		public void GetUMPromptAsync(GetUMPromptType GetUMPrompt1)
		{
			this.GetUMPromptAsync(GetUMPrompt1, null);
		}

		public void GetUMPromptAsync(GetUMPromptType GetUMPrompt1, object userState)
		{
			if (this.GetUMPromptOperationCompleted == null)
			{
				this.GetUMPromptOperationCompleted = new SendOrPostCallback(this.OnGetUMPromptOperationCompleted);
			}
			base.InvokeAsync("GetUMPrompt", new object[]
			{
				GetUMPrompt1
			}, this.GetUMPromptOperationCompleted, userState);
		}

		private void OnGetUMPromptOperationCompleted(object arg)
		{
			if (this.GetUMPromptCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetUMPromptCompleted(this, new GetUMPromptCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetUMPromptNames", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("GetUMPromptNamesResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetUMPromptNamesResponseMessageType GetUMPromptNames([XmlElement("GetUMPromptNames", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetUMPromptNamesType GetUMPromptNames1)
		{
			object[] array = this.Invoke("GetUMPromptNames", new object[]
			{
				GetUMPromptNames1
			});
			return (GetUMPromptNamesResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetUMPromptNames(GetUMPromptNamesType GetUMPromptNames1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetUMPromptNames", new object[]
			{
				GetUMPromptNames1
			}, callback, asyncState);
		}

		public GetUMPromptNamesResponseMessageType EndGetUMPromptNames(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetUMPromptNamesResponseMessageType)array[0];
		}

		public void GetUMPromptNamesAsync(GetUMPromptNamesType GetUMPromptNames1)
		{
			this.GetUMPromptNamesAsync(GetUMPromptNames1, null);
		}

		public void GetUMPromptNamesAsync(GetUMPromptNamesType GetUMPromptNames1, object userState)
		{
			if (this.GetUMPromptNamesOperationCompleted == null)
			{
				this.GetUMPromptNamesOperationCompleted = new SendOrPostCallback(this.OnGetUMPromptNamesOperationCompleted);
			}
			base.InvokeAsync("GetUMPromptNames", new object[]
			{
				GetUMPromptNames1
			}, this.GetUMPromptNamesOperationCompleted, userState);
		}

		private void OnGetUMPromptNamesOperationCompleted(object arg)
		{
			if (this.GetUMPromptNamesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetUMPromptNamesCompleted(this, new GetUMPromptNamesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetClientExtension", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("GetClientExtensionResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ClientExtensionResponseType GetClientExtension([XmlElement("GetClientExtension", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetClientExtensionType GetClientExtension1)
		{
			object[] array = this.Invoke("GetClientExtension", new object[]
			{
				GetClientExtension1
			});
			return (ClientExtensionResponseType)array[0];
		}

		public IAsyncResult BeginGetClientExtension(GetClientExtensionType GetClientExtension1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetClientExtension", new object[]
			{
				GetClientExtension1
			}, callback, asyncState);
		}

		public ClientExtensionResponseType EndGetClientExtension(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ClientExtensionResponseType)array[0];
		}

		public void GetClientExtensionAsync(GetClientExtensionType GetClientExtension1)
		{
			this.GetClientExtensionAsync(GetClientExtension1, null);
		}

		public void GetClientExtensionAsync(GetClientExtensionType GetClientExtension1, object userState)
		{
			if (this.GetClientExtensionOperationCompleted == null)
			{
				this.GetClientExtensionOperationCompleted = new SendOrPostCallback(this.OnGetClientExtensionOperationCompleted);
			}
			base.InvokeAsync("GetClientExtension", new object[]
			{
				GetClientExtension1
			}, this.GetClientExtensionOperationCompleted, userState);
		}

		private void OnGetClientExtensionOperationCompleted(object arg)
		{
			if (this.GetClientExtensionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetClientExtensionCompleted(this, new GetClientExtensionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/SetClientExtension", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("SetClientExtensionResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SetClientExtensionResponseType SetClientExtension([XmlElement("SetClientExtension", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] SetClientExtensionType SetClientExtension1)
		{
			object[] array = this.Invoke("SetClientExtension", new object[]
			{
				SetClientExtension1
			});
			return (SetClientExtensionResponseType)array[0];
		}

		public IAsyncResult BeginSetClientExtension(SetClientExtensionType SetClientExtension1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SetClientExtension", new object[]
			{
				SetClientExtension1
			}, callback, asyncState);
		}

		public SetClientExtensionResponseType EndSetClientExtension(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (SetClientExtensionResponseType)array[0];
		}

		public void SetClientExtensionAsync(SetClientExtensionType SetClientExtension1)
		{
			this.SetClientExtensionAsync(SetClientExtension1, null);
		}

		public void SetClientExtensionAsync(SetClientExtensionType SetClientExtension1, object userState)
		{
			if (this.SetClientExtensionOperationCompleted == null)
			{
				this.SetClientExtensionOperationCompleted = new SendOrPostCallback(this.OnSetClientExtensionOperationCompleted);
			}
			base.InvokeAsync("SetClientExtension", new object[]
			{
				SetClientExtension1
			}, this.SetClientExtensionOperationCompleted, userState);
		}

		private void OnSetClientExtensionOperationCompleted(object arg)
		{
			if (this.SetClientExtensionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetClientExtensionCompleted(this, new SetClientExtensionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/StartFindInGALSpeechRecognition", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("StartFindInGALSpeechRecognitionResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public StartFindInGALSpeechRecognitionResponseMessageType StartFindInGALSpeechRecognition([XmlElement("StartFindInGALSpeechRecognition", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] StartFindInGALSpeechRecognitionType StartFindInGALSpeechRecognition1)
		{
			object[] array = this.Invoke("StartFindInGALSpeechRecognition", new object[]
			{
				StartFindInGALSpeechRecognition1
			});
			return (StartFindInGALSpeechRecognitionResponseMessageType)array[0];
		}

		public IAsyncResult BeginStartFindInGALSpeechRecognition(StartFindInGALSpeechRecognitionType StartFindInGALSpeechRecognition1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("StartFindInGALSpeechRecognition", new object[]
			{
				StartFindInGALSpeechRecognition1
			}, callback, asyncState);
		}

		public StartFindInGALSpeechRecognitionResponseMessageType EndStartFindInGALSpeechRecognition(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (StartFindInGALSpeechRecognitionResponseMessageType)array[0];
		}

		public void StartFindInGALSpeechRecognitionAsync(StartFindInGALSpeechRecognitionType StartFindInGALSpeechRecognition1)
		{
			this.StartFindInGALSpeechRecognitionAsync(StartFindInGALSpeechRecognition1, null);
		}

		public void StartFindInGALSpeechRecognitionAsync(StartFindInGALSpeechRecognitionType StartFindInGALSpeechRecognition1, object userState)
		{
			if (this.StartFindInGALSpeechRecognitionOperationCompleted == null)
			{
				this.StartFindInGALSpeechRecognitionOperationCompleted = new SendOrPostCallback(this.OnStartFindInGALSpeechRecognitionOperationCompleted);
			}
			base.InvokeAsync("StartFindInGALSpeechRecognition", new object[]
			{
				StartFindInGALSpeechRecognition1
			}, this.StartFindInGALSpeechRecognitionOperationCompleted, userState);
		}

		private void OnStartFindInGALSpeechRecognitionOperationCompleted(object arg)
		{
			if (this.StartFindInGALSpeechRecognitionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.StartFindInGALSpeechRecognitionCompleted(this, new StartFindInGALSpeechRecognitionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/CompleteFindInGALSpeechRecognition", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("CompleteFindInGALSpeechRecognitionResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public CompleteFindInGALSpeechRecognitionResponseMessageType CompleteFindInGALSpeechRecognition([XmlElement("CompleteFindInGALSpeechRecognition", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] CompleteFindInGALSpeechRecognitionType CompleteFindInGALSpeechRecognition1)
		{
			object[] array = this.Invoke("CompleteFindInGALSpeechRecognition", new object[]
			{
				CompleteFindInGALSpeechRecognition1
			});
			return (CompleteFindInGALSpeechRecognitionResponseMessageType)array[0];
		}

		public IAsyncResult BeginCompleteFindInGALSpeechRecognition(CompleteFindInGALSpeechRecognitionType CompleteFindInGALSpeechRecognition1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CompleteFindInGALSpeechRecognition", new object[]
			{
				CompleteFindInGALSpeechRecognition1
			}, callback, asyncState);
		}

		public CompleteFindInGALSpeechRecognitionResponseMessageType EndCompleteFindInGALSpeechRecognition(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CompleteFindInGALSpeechRecognitionResponseMessageType)array[0];
		}

		public void CompleteFindInGALSpeechRecognitionAsync(CompleteFindInGALSpeechRecognitionType CompleteFindInGALSpeechRecognition1)
		{
			this.CompleteFindInGALSpeechRecognitionAsync(CompleteFindInGALSpeechRecognition1, null);
		}

		public void CompleteFindInGALSpeechRecognitionAsync(CompleteFindInGALSpeechRecognitionType CompleteFindInGALSpeechRecognition1, object userState)
		{
			if (this.CompleteFindInGALSpeechRecognitionOperationCompleted == null)
			{
				this.CompleteFindInGALSpeechRecognitionOperationCompleted = new SendOrPostCallback(this.OnCompleteFindInGALSpeechRecognitionOperationCompleted);
			}
			base.InvokeAsync("CompleteFindInGALSpeechRecognition", new object[]
			{
				CompleteFindInGALSpeechRecognition1
			}, this.CompleteFindInGALSpeechRecognitionOperationCompleted, userState);
		}

		private void OnCompleteFindInGALSpeechRecognitionOperationCompleted(object arg)
		{
			if (this.CompleteFindInGALSpeechRecognitionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CompleteFindInGALSpeechRecognitionCompleted(this, new CompleteFindInGALSpeechRecognitionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/CreateUMCallDataRecord", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("CreateUMCallDataRecordResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public CreateUMCallDataRecordResponseMessageType CreateUMCallDataRecord([XmlElement("CreateUMCallDataRecord", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] CreateUMCallDataRecordType CreateUMCallDataRecord1)
		{
			object[] array = this.Invoke("CreateUMCallDataRecord", new object[]
			{
				CreateUMCallDataRecord1
			});
			return (CreateUMCallDataRecordResponseMessageType)array[0];
		}

		public IAsyncResult BeginCreateUMCallDataRecord(CreateUMCallDataRecordType CreateUMCallDataRecord1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateUMCallDataRecord", new object[]
			{
				CreateUMCallDataRecord1
			}, callback, asyncState);
		}

		public CreateUMCallDataRecordResponseMessageType EndCreateUMCallDataRecord(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CreateUMCallDataRecordResponseMessageType)array[0];
		}

		public void CreateUMCallDataRecordAsync(CreateUMCallDataRecordType CreateUMCallDataRecord1)
		{
			this.CreateUMCallDataRecordAsync(CreateUMCallDataRecord1, null);
		}

		public void CreateUMCallDataRecordAsync(CreateUMCallDataRecordType CreateUMCallDataRecord1, object userState)
		{
			if (this.CreateUMCallDataRecordOperationCompleted == null)
			{
				this.CreateUMCallDataRecordOperationCompleted = new SendOrPostCallback(this.OnCreateUMCallDataRecordOperationCompleted);
			}
			base.InvokeAsync("CreateUMCallDataRecord", new object[]
			{
				CreateUMCallDataRecord1
			}, this.CreateUMCallDataRecordOperationCompleted, userState);
		}

		private void OnCreateUMCallDataRecordOperationCompleted(object arg)
		{
			if (this.CreateUMCallDataRecordCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateUMCallDataRecordCompleted(this, new CreateUMCallDataRecordCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetUMCallDataRecords", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetUMCallDataRecordsResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetUMCallDataRecordsResponseMessageType GetUMCallDataRecords([XmlElement("GetUMCallDataRecords", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetUMCallDataRecordsType GetUMCallDataRecords1)
		{
			object[] array = this.Invoke("GetUMCallDataRecords", new object[]
			{
				GetUMCallDataRecords1
			});
			return (GetUMCallDataRecordsResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetUMCallDataRecords(GetUMCallDataRecordsType GetUMCallDataRecords1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetUMCallDataRecords", new object[]
			{
				GetUMCallDataRecords1
			}, callback, asyncState);
		}

		public GetUMCallDataRecordsResponseMessageType EndGetUMCallDataRecords(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetUMCallDataRecordsResponseMessageType)array[0];
		}

		public void GetUMCallDataRecordsAsync(GetUMCallDataRecordsType GetUMCallDataRecords1)
		{
			this.GetUMCallDataRecordsAsync(GetUMCallDataRecords1, null);
		}

		public void GetUMCallDataRecordsAsync(GetUMCallDataRecordsType GetUMCallDataRecords1, object userState)
		{
			if (this.GetUMCallDataRecordsOperationCompleted == null)
			{
				this.GetUMCallDataRecordsOperationCompleted = new SendOrPostCallback(this.OnGetUMCallDataRecordsOperationCompleted);
			}
			base.InvokeAsync("GetUMCallDataRecords", new object[]
			{
				GetUMCallDataRecords1
			}, this.GetUMCallDataRecordsOperationCompleted, userState);
		}

		private void OnGetUMCallDataRecordsOperationCompleted(object arg)
		{
			if (this.GetUMCallDataRecordsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetUMCallDataRecordsCompleted(this, new GetUMCallDataRecordsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetUMCallSummary", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetUMCallSummaryResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetUMCallSummaryResponseMessageType GetUMCallSummary([XmlElement("GetUMCallSummary", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetUMCallSummaryType GetUMCallSummary1)
		{
			object[] array = this.Invoke("GetUMCallSummary", new object[]
			{
				GetUMCallSummary1
			});
			return (GetUMCallSummaryResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetUMCallSummary(GetUMCallSummaryType GetUMCallSummary1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetUMCallSummary", new object[]
			{
				GetUMCallSummary1
			}, callback, asyncState);
		}

		public GetUMCallSummaryResponseMessageType EndGetUMCallSummary(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetUMCallSummaryResponseMessageType)array[0];
		}

		public void GetUMCallSummaryAsync(GetUMCallSummaryType GetUMCallSummary1)
		{
			this.GetUMCallSummaryAsync(GetUMCallSummary1, null);
		}

		public void GetUMCallSummaryAsync(GetUMCallSummaryType GetUMCallSummary1, object userState)
		{
			if (this.GetUMCallSummaryOperationCompleted == null)
			{
				this.GetUMCallSummaryOperationCompleted = new SendOrPostCallback(this.OnGetUMCallSummaryOperationCompleted);
			}
			base.InvokeAsync("GetUMCallSummary", new object[]
			{
				GetUMCallSummary1
			}, this.GetUMCallSummaryOperationCompleted, userState);
		}

		private void OnGetUMCallSummaryOperationCompleted(object arg)
		{
			if (this.GetUMCallSummaryCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetUMCallSummaryCompleted(this, new GetUMCallSummaryCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/InitUMMailbox", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("InitUMMailboxResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public InitUMMailboxResponseMessageType InitUMMailbox([XmlElement("InitUMMailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] InitUMMailboxType InitUMMailbox1)
		{
			object[] array = this.Invoke("InitUMMailbox", new object[]
			{
				InitUMMailbox1
			});
			return (InitUMMailboxResponseMessageType)array[0];
		}

		public IAsyncResult BeginInitUMMailbox(InitUMMailboxType InitUMMailbox1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("InitUMMailbox", new object[]
			{
				InitUMMailbox1
			}, callback, asyncState);
		}

		public InitUMMailboxResponseMessageType EndInitUMMailbox(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (InitUMMailboxResponseMessageType)array[0];
		}

		public void InitUMMailboxAsync(InitUMMailboxType InitUMMailbox1)
		{
			this.InitUMMailboxAsync(InitUMMailbox1, null);
		}

		public void InitUMMailboxAsync(InitUMMailboxType InitUMMailbox1, object userState)
		{
			if (this.InitUMMailboxOperationCompleted == null)
			{
				this.InitUMMailboxOperationCompleted = new SendOrPostCallback(this.OnInitUMMailboxOperationCompleted);
			}
			base.InvokeAsync("InitUMMailbox", new object[]
			{
				InitUMMailbox1
			}, this.InitUMMailboxOperationCompleted, userState);
		}

		private void OnInitUMMailboxOperationCompleted(object arg)
		{
			if (this.InitUMMailboxCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.InitUMMailboxCompleted(this, new InitUMMailboxCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/ResetUMMailbox", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("ResetUMMailboxResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ResetUMMailboxResponseMessageType ResetUMMailbox([XmlElement("ResetUMMailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] ResetUMMailboxType ResetUMMailbox1)
		{
			object[] array = this.Invoke("ResetUMMailbox", new object[]
			{
				ResetUMMailbox1
			});
			return (ResetUMMailboxResponseMessageType)array[0];
		}

		public IAsyncResult BeginResetUMMailbox(ResetUMMailboxType ResetUMMailbox1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ResetUMMailbox", new object[]
			{
				ResetUMMailbox1
			}, callback, asyncState);
		}

		public ResetUMMailboxResponseMessageType EndResetUMMailbox(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ResetUMMailboxResponseMessageType)array[0];
		}

		public void ResetUMMailboxAsync(ResetUMMailboxType ResetUMMailbox1)
		{
			this.ResetUMMailboxAsync(ResetUMMailbox1, null);
		}

		public void ResetUMMailboxAsync(ResetUMMailboxType ResetUMMailbox1, object userState)
		{
			if (this.ResetUMMailboxOperationCompleted == null)
			{
				this.ResetUMMailboxOperationCompleted = new SendOrPostCallback(this.OnResetUMMailboxOperationCompleted);
			}
			base.InvokeAsync("ResetUMMailbox", new object[]
			{
				ResetUMMailbox1
			}, this.ResetUMMailboxOperationCompleted, userState);
		}

		private void OnResetUMMailboxOperationCompleted(object arg)
		{
			if (this.ResetUMMailboxCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ResetUMMailboxCompleted(this, new ResetUMMailboxCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/ValidateUMPin", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("ValidateUMPinResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ValidateUMPinResponseMessageType ValidateUMPin([XmlElement("ValidateUMPin", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] ValidateUMPinType ValidateUMPin1)
		{
			object[] array = this.Invoke("ValidateUMPin", new object[]
			{
				ValidateUMPin1
			});
			return (ValidateUMPinResponseMessageType)array[0];
		}

		public IAsyncResult BeginValidateUMPin(ValidateUMPinType ValidateUMPin1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ValidateUMPin", new object[]
			{
				ValidateUMPin1
			}, callback, asyncState);
		}

		public ValidateUMPinResponseMessageType EndValidateUMPin(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ValidateUMPinResponseMessageType)array[0];
		}

		public void ValidateUMPinAsync(ValidateUMPinType ValidateUMPin1)
		{
			this.ValidateUMPinAsync(ValidateUMPin1, null);
		}

		public void ValidateUMPinAsync(ValidateUMPinType ValidateUMPin1, object userState)
		{
			if (this.ValidateUMPinOperationCompleted == null)
			{
				this.ValidateUMPinOperationCompleted = new SendOrPostCallback(this.OnValidateUMPinOperationCompleted);
			}
			base.InvokeAsync("ValidateUMPin", new object[]
			{
				ValidateUMPin1
			}, this.ValidateUMPinOperationCompleted, userState);
		}

		private void OnValidateUMPinOperationCompleted(object arg)
		{
			if (this.ValidateUMPinCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ValidateUMPinCompleted(this, new ValidateUMPinCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/SaveUMPin", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("SaveUMPinResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SaveUMPinResponseMessageType SaveUMPin([XmlElement("SaveUMPin", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] SaveUMPinType SaveUMPin1)
		{
			object[] array = this.Invoke("SaveUMPin", new object[]
			{
				SaveUMPin1
			});
			return (SaveUMPinResponseMessageType)array[0];
		}

		public IAsyncResult BeginSaveUMPin(SaveUMPinType SaveUMPin1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SaveUMPin", new object[]
			{
				SaveUMPin1
			}, callback, asyncState);
		}

		public SaveUMPinResponseMessageType EndSaveUMPin(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (SaveUMPinResponseMessageType)array[0];
		}

		public void SaveUMPinAsync(SaveUMPinType SaveUMPin1)
		{
			this.SaveUMPinAsync(SaveUMPin1, null);
		}

		public void SaveUMPinAsync(SaveUMPinType SaveUMPin1, object userState)
		{
			if (this.SaveUMPinOperationCompleted == null)
			{
				this.SaveUMPinOperationCompleted = new SendOrPostCallback(this.OnSaveUMPinOperationCompleted);
			}
			base.InvokeAsync("SaveUMPin", new object[]
			{
				SaveUMPin1
			}, this.SaveUMPinOperationCompleted, userState);
		}

		private void OnSaveUMPinOperationCompleted(object arg)
		{
			if (this.SaveUMPinCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SaveUMPinCompleted(this, new SaveUMPinCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetUMPin", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("GetUMPinResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetUMPinResponseMessageType GetUMPin([XmlElement("GetUMPin", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetUMPinType GetUMPin1)
		{
			object[] array = this.Invoke("GetUMPin", new object[]
			{
				GetUMPin1
			});
			return (GetUMPinResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetUMPin(GetUMPinType GetUMPin1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetUMPin", new object[]
			{
				GetUMPin1
			}, callback, asyncState);
		}

		public GetUMPinResponseMessageType EndGetUMPin(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetUMPinResponseMessageType)array[0];
		}

		public void GetUMPinAsync(GetUMPinType GetUMPin1)
		{
			this.GetUMPinAsync(GetUMPin1, null);
		}

		public void GetUMPinAsync(GetUMPinType GetUMPin1, object userState)
		{
			if (this.GetUMPinOperationCompleted == null)
			{
				this.GetUMPinOperationCompleted = new SendOrPostCallback(this.OnGetUMPinOperationCompleted);
			}
			base.InvokeAsync("GetUMPin", new object[]
			{
				GetUMPin1
			}, this.GetUMPinOperationCompleted, userState);
		}

		private void OnGetUMPinOperationCompleted(object arg)
		{
			if (this.GetUMPinCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetUMPinCompleted(this, new GetUMPinCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetClientIntent", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("GetClientIntentResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetClientIntentResponseMessageType GetClientIntent([XmlElement("GetClientIntent", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetClientIntentType GetClientIntent1)
		{
			object[] array = this.Invoke("GetClientIntent", new object[]
			{
				GetClientIntent1
			});
			return (GetClientIntentResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetClientIntent(GetClientIntentType GetClientIntent1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetClientIntent", new object[]
			{
				GetClientIntent1
			}, callback, asyncState);
		}

		public GetClientIntentResponseMessageType EndGetClientIntent(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetClientIntentResponseMessageType)array[0];
		}

		public void GetClientIntentAsync(GetClientIntentType GetClientIntent1)
		{
			this.GetClientIntentAsync(GetClientIntent1, null);
		}

		public void GetClientIntentAsync(GetClientIntentType GetClientIntent1, object userState)
		{
			if (this.GetClientIntentOperationCompleted == null)
			{
				this.GetClientIntentOperationCompleted = new SendOrPostCallback(this.OnGetClientIntentOperationCompleted);
			}
			base.InvokeAsync("GetClientIntent", new object[]
			{
				GetClientIntent1
			}, this.GetClientIntentOperationCompleted, userState);
		}

		private void OnGetClientIntentOperationCompleted(object arg)
		{
			if (this.GetClientIntentCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetClientIntentCompleted(this, new GetClientIntentCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetUMSubscriberCallAnsweringData", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("GetUMSubscriberCallAnsweringDataResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetUMSubscriberCallAnsweringDataResponseMessageType GetUMSubscriberCallAnsweringData([XmlElement("GetUMSubscriberCallAnsweringData", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetUMSubscriberCallAnsweringDataType GetUMSubscriberCallAnsweringData1)
		{
			object[] array = this.Invoke("GetUMSubscriberCallAnsweringData", new object[]
			{
				GetUMSubscriberCallAnsweringData1
			});
			return (GetUMSubscriberCallAnsweringDataResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetUMSubscriberCallAnsweringData(GetUMSubscriberCallAnsweringDataType GetUMSubscriberCallAnsweringData1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetUMSubscriberCallAnsweringData", new object[]
			{
				GetUMSubscriberCallAnsweringData1
			}, callback, asyncState);
		}

		public GetUMSubscriberCallAnsweringDataResponseMessageType EndGetUMSubscriberCallAnsweringData(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetUMSubscriberCallAnsweringDataResponseMessageType)array[0];
		}

		public void GetUMSubscriberCallAnsweringDataAsync(GetUMSubscriberCallAnsweringDataType GetUMSubscriberCallAnsweringData1)
		{
			this.GetUMSubscriberCallAnsweringDataAsync(GetUMSubscriberCallAnsweringData1, null);
		}

		public void GetUMSubscriberCallAnsweringDataAsync(GetUMSubscriberCallAnsweringDataType GetUMSubscriberCallAnsweringData1, object userState)
		{
			if (this.GetUMSubscriberCallAnsweringDataOperationCompleted == null)
			{
				this.GetUMSubscriberCallAnsweringDataOperationCompleted = new SendOrPostCallback(this.OnGetUMSubscriberCallAnsweringDataOperationCompleted);
			}
			base.InvokeAsync("GetUMSubscriberCallAnsweringData", new object[]
			{
				GetUMSubscriberCallAnsweringData1
			}, this.GetUMSubscriberCallAnsweringDataOperationCompleted, userState);
		}

		private void OnGetUMSubscriberCallAnsweringDataOperationCompleted(object arg)
		{
			if (this.GetUMSubscriberCallAnsweringDataCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetUMSubscriberCallAnsweringDataCompleted(this, new GetUMSubscriberCallAnsweringDataCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/UpdateMailboxAssociation", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("UpdateMailboxAssociationResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public UpdateMailboxAssociationResponseType UpdateMailboxAssociation([XmlElement("UpdateMailboxAssociation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] UpdateMailboxAssociationType UpdateMailboxAssociation1)
		{
			object[] array = this.Invoke("UpdateMailboxAssociation", new object[]
			{
				UpdateMailboxAssociation1
			});
			return (UpdateMailboxAssociationResponseType)array[0];
		}

		public IAsyncResult BeginUpdateMailboxAssociation(UpdateMailboxAssociationType UpdateMailboxAssociation1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateMailboxAssociation", new object[]
			{
				UpdateMailboxAssociation1
			}, callback, asyncState);
		}

		public UpdateMailboxAssociationResponseType EndUpdateMailboxAssociation(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (UpdateMailboxAssociationResponseType)array[0];
		}

		public void UpdateMailboxAssociationAsync(UpdateMailboxAssociationType UpdateMailboxAssociation1)
		{
			this.UpdateMailboxAssociationAsync(UpdateMailboxAssociation1, null);
		}

		public void UpdateMailboxAssociationAsync(UpdateMailboxAssociationType UpdateMailboxAssociation1, object userState)
		{
			if (this.UpdateMailboxAssociationOperationCompleted == null)
			{
				this.UpdateMailboxAssociationOperationCompleted = new SendOrPostCallback(this.OnUpdateMailboxAssociationOperationCompleted);
			}
			base.InvokeAsync("UpdateMailboxAssociation", new object[]
			{
				UpdateMailboxAssociation1
			}, this.UpdateMailboxAssociationOperationCompleted, userState);
		}

		private void OnUpdateMailboxAssociationOperationCompleted(object arg)
		{
			if (this.UpdateMailboxAssociationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateMailboxAssociationCompleted(this, new UpdateMailboxAssociationCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/UpdateGroupMailbox", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("UpdateGroupMailboxResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public UpdateGroupMailboxResponseType UpdateGroupMailbox([XmlElement("UpdateGroupMailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] UpdateGroupMailboxType UpdateGroupMailbox1)
		{
			object[] array = this.Invoke("UpdateGroupMailbox", new object[]
			{
				UpdateGroupMailbox1
			});
			return (UpdateGroupMailboxResponseType)array[0];
		}

		public IAsyncResult BeginUpdateGroupMailbox(UpdateGroupMailboxType UpdateGroupMailbox1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateGroupMailbox", new object[]
			{
				UpdateGroupMailbox1
			}, callback, asyncState);
		}

		public UpdateGroupMailboxResponseType EndUpdateGroupMailbox(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (UpdateGroupMailboxResponseType)array[0];
		}

		public void UpdateGroupMailboxAsync(UpdateGroupMailboxType UpdateGroupMailbox1)
		{
			this.UpdateGroupMailboxAsync(UpdateGroupMailbox1, null);
		}

		public void UpdateGroupMailboxAsync(UpdateGroupMailboxType UpdateGroupMailbox1, object userState)
		{
			if (this.UpdateGroupMailboxOperationCompleted == null)
			{
				this.UpdateGroupMailboxOperationCompleted = new SendOrPostCallback(this.OnUpdateGroupMailboxOperationCompleted);
			}
			base.InvokeAsync("UpdateGroupMailbox", new object[]
			{
				UpdateGroupMailbox1
			}, this.UpdateGroupMailboxOperationCompleted, userState);
		}

		private void OnUpdateGroupMailboxOperationCompleted(object arg)
		{
			if (this.UpdateGroupMailboxCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateGroupMailboxCompleted(this, new UpdateGroupMailboxCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/PostModernGroupItem", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("PostModernGroupItemResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public PostModernGroupItemResponseType PostModernGroupItem([XmlElement("PostModernGroupItem", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] PostModernGroupItemType PostModernGroupItem1)
		{
			object[] array = this.Invoke("PostModernGroupItem", new object[]
			{
				PostModernGroupItem1
			});
			return (PostModernGroupItemResponseType)array[0];
		}

		public IAsyncResult BeginPostModernGroupItem(PostModernGroupItemType PostModernGroupItem1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("PostModernGroupItem", new object[]
			{
				PostModernGroupItem1
			}, callback, asyncState);
		}

		public PostModernGroupItemResponseType EndPostModernGroupItem(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (PostModernGroupItemResponseType)array[0];
		}

		public void PostModernGroupItemAsync(PostModernGroupItemType PostModernGroupItem1)
		{
			this.PostModernGroupItemAsync(PostModernGroupItem1, null);
		}

		public void PostModernGroupItemAsync(PostModernGroupItemType PostModernGroupItem1, object userState)
		{
			if (this.PostModernGroupItemOperationCompleted == null)
			{
				this.PostModernGroupItemOperationCompleted = new SendOrPostCallback(this.OnPostModernGroupItemOperationCompleted);
			}
			base.InvokeAsync("PostModernGroupItem", new object[]
			{
				PostModernGroupItem1
			}, this.PostModernGroupItemOperationCompleted, userState);
		}

		private void OnPostModernGroupItemOperationCompleted(object arg)
		{
			if (this.PostModernGroupItemCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.PostModernGroupItemCompleted(this, new PostModernGroupItemCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		internal override XmlNamespaceDefinition[] PredefinedNamespaces
		{
			get
			{
				return Constants.EwsNamespaces;
			}
		}

		public PrivateExchangeServiceBinding(string componentId, RemoteCertificateValidationCallback remoteCertificateValidationCallback) : base(componentId, remoteCertificateValidationCallback, true)
		{
		}

		public PrivateExchangeServiceBinding(string componentId, RemoteCertificateValidationCallback remoteCertificateValidationCallback, bool normalization) : base(componentId, remoteCertificateValidationCallback, normalization)
		{
		}

		protected void SetClientRequestIdHeaderFromActivityId()
		{
			Guid? activityId = ActivityContext.ActivityId;
			if (activityId != null)
			{
				base.HttpHeaders["client-request-id"] = activityId.ToString();
				return;
			}
			PrivateExchangeServiceBinding.Tracer.TraceWarning((long)this.GetHashCode(), "ActivityContext.ActivityId is null. Request will omit the client-request-id header.");
		}

		public RequestServerVersion RequestServerVersionValue;

		public ServerVersionInfo ServerVersionInfoValue;

		private SendOrPostCallback CreateUMPromptOperationCompleted;

		private SendOrPostCallback DeleteUMPromptsOperationCompleted;

		private SendOrPostCallback GetUMPromptOperationCompleted;

		private SendOrPostCallback GetUMPromptNamesOperationCompleted;

		private SendOrPostCallback GetClientExtensionOperationCompleted;

		private SendOrPostCallback SetClientExtensionOperationCompleted;

		private SendOrPostCallback StartFindInGALSpeechRecognitionOperationCompleted;

		private SendOrPostCallback CompleteFindInGALSpeechRecognitionOperationCompleted;

		private SendOrPostCallback CreateUMCallDataRecordOperationCompleted;

		private SendOrPostCallback GetUMCallDataRecordsOperationCompleted;

		private SendOrPostCallback GetUMCallSummaryOperationCompleted;

		private SendOrPostCallback InitUMMailboxOperationCompleted;

		private SendOrPostCallback ResetUMMailboxOperationCompleted;

		private SendOrPostCallback ValidateUMPinOperationCompleted;

		private SendOrPostCallback SaveUMPinOperationCompleted;

		private SendOrPostCallback GetUMPinOperationCompleted;

		private SendOrPostCallback GetClientIntentOperationCompleted;

		private SendOrPostCallback GetUMSubscriberCallAnsweringDataOperationCompleted;

		private SendOrPostCallback UpdateMailboxAssociationOperationCompleted;

		private SendOrPostCallback UpdateGroupMailboxOperationCompleted;

		private SendOrPostCallback PostModernGroupItemOperationCompleted;

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.EwsClientTracer;
	}
}
