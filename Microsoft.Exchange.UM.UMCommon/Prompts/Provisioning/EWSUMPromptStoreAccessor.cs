using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	internal class EWSUMPromptStoreAccessor : DisposableBase, IUMPromptStorage, IDisposeTrackable, IDisposable
	{
		public EWSUMPromptStoreAccessor(ExchangePrincipal user, Guid configurationObject)
		{
			EWSUMPromptStoreAccessor <>4__this = this;
			ExAssert.RetailAssert(configurationObject != Guid.Empty, "ConfigurationObject Guid cannot be empty");
			this.tracer = new DiagnosticHelper(this, ExTraceGlobals.XsoTracer);
			PIIMessage pii = PIIMessage.Create(PIIType._SmtpAddress, user.MailboxInfo.PrimarySmtpAddress.ToString());
			this.tracer.Trace(pii, "EWSUMPromptStoreAccessor for configObject {0}, user: _PrimarySmtpAddress", new object[]
			{
				configurationObject
			});
			this.configurationObject = configurationObject;
			Exception e = UMMailboxAccessorEwsBinding.ExecuteEWSOperation(delegate
			{
				<>4__this.ewsBinding = new UMMailboxAccessorEwsBinding(user, <>4__this.tracer);
				<>4__this.tracer.Trace("EWSUMPromptStoreAccessor, EWS Url = {0}", new object[]
				{
					<>4__this.ewsBinding.Url
				});
			}, this.tracer);
			this.CheckForErrors(e);
		}

		public void DeleteAllPrompts()
		{
			this.tracer.Trace("EWSUMPromptStoreAccessor : DeleteAllPrompts", new object[0]);
			this.InternalDeletePrompts(null);
		}

		public void DeletePrompts(string[] prompts)
		{
			this.tracer.Trace("EWSUMPromptStoreAccessor : DeletePrompts", new object[0]);
			ValidateArgument.NotNull(prompts, "Prompts");
			this.InternalDeletePrompts(prompts);
		}

		public string[] GetPromptNames()
		{
			return this.GetPromptNames(TimeSpan.Zero);
		}

		public string[] GetPromptNames(TimeSpan timeSinceLastModified)
		{
			this.tracer.Trace("EWSUMPromptStoreAccessor : GetPromptNames, for Guid {0}", new object[]
			{
				this.configurationObject
			});
			GetUMPromptNamesType request = new GetUMPromptNamesType();
			request.ConfigurationObject = this.configurationObject.ToString();
			request.HoursElapsedSinceLastModified = (int)timeSinceLastModified.TotalHours;
			GetUMPromptNamesResponseMessageType response = null;
			Exception e = UMMailboxAccessorEwsBinding.ExecuteEWSOperation(delegate
			{
				response = this.ewsBinding.GetUMPromptNames(request);
			}, this.tracer);
			this.CheckResponse(e, response, null);
			this.tracer.Trace("EWSUMPromptStoreAccessor : GetPromptNames, Number of Prompts {0}", new object[]
			{
				response.PromptNames.Length
			});
			return response.PromptNames;
		}

		public void CreatePrompt(string promptName, string audioBytes)
		{
			ValidateArgument.NotNullOrEmpty(promptName, "promptName");
			ExAssert.RetailAssert(audioBytes != null && audioBytes.Length > 0, "AudioData passed cannot be null or empty");
			this.tracer.Trace("EWSUMPromptStoreAccessor : CreatePrompt, promptName {0}", new object[]
			{
				promptName
			});
			CreateUMPromptType request = new CreateUMPromptType();
			request.ConfigurationObject = this.configurationObject.ToString();
			request.PromptName = promptName;
			request.AudioData = Convert.FromBase64String(audioBytes);
			CreateUMPromptResponseMessageType response = null;
			Exception e = UMMailboxAccessorEwsBinding.ExecuteEWSOperation(delegate
			{
				response = this.ewsBinding.CreateUMPrompt(request);
			}, this.tracer);
			this.CheckResponse(e, response, promptName);
		}

		public string GetPrompt(string promptName)
		{
			ValidateArgument.NotNullOrEmpty(promptName, "promptName");
			this.tracer.Trace("EWSUMPromptStoreAccessor : GetPrompt, promptName {0}", new object[]
			{
				promptName
			});
			GetUMPromptType request = new GetUMPromptType();
			request.ConfigurationObject = this.configurationObject.ToString();
			request.PromptName = promptName;
			GetUMPromptResponseMessageType response = null;
			Exception e = UMMailboxAccessorEwsBinding.ExecuteEWSOperation(delegate
			{
				response = this.ewsBinding.GetUMPrompt(request);
			}, this.tracer);
			this.CheckResponse(e, response, promptName);
			this.tracer.Trace("EWSUMPromptStoreAccessor : GetPrompt, AduioData Length {0}", new object[]
			{
				response.AudioData.Length
			});
			return Convert.ToBase64String(response.AudioData);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.tracer.Trace("EWSUMPromptsStorage : InternalDispose", new object[0]);
				if (this.ewsBinding != null)
				{
					this.ewsBinding.Dispose();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<EWSUMPromptStoreAccessor>(this);
		}

		private void InternalDeletePrompts(string[] prompts)
		{
			DeleteUMPromptsType request = new DeleteUMPromptsType();
			request.ConfigurationObject = this.configurationObject.ToString();
			if (prompts != null)
			{
				request.PromptNames = prompts;
			}
			DeleteUMPromptsResponseMessageType response = null;
			Exception e = UMMailboxAccessorEwsBinding.ExecuteEWSOperation(delegate
			{
				response = this.ewsBinding.DeleteUMPrompts(request);
			}, this.tracer);
			this.CheckResponse(e, response, null);
		}

		private void CheckForErrors(Exception e)
		{
			if (e != null)
			{
				this.tracer.Trace("EWSUMPromptStoreAccessor : CheckForErrors, Exception: {0}", new object[]
				{
					e
				});
				throw new PublishingPointException(e.Message, e);
			}
		}

		private void CheckResponse(Exception e, ResponseMessageType response, string promptName)
		{
			this.CheckForErrors(e);
			if (response == null)
			{
				this.tracer.Trace("EWSUMPromptStoreAccessor : CheckResponse, response == null", new object[0]);
				throw new EWSUMMailboxAccessException(Strings.EWSNoResponseReceived);
			}
			this.tracer.Trace("EWSUMPromptStoreAccessor : CheckResponse, ResponseCode = {0}, ResponseClass = {1}, MessageText = {2}", new object[]
			{
				response.ResponseCode,
				response.ResponseClass,
				response.MessageText
			});
			if (response.ResponseClass == ResponseClassType.Success && response.ResponseCode == ResponseCodeType.NoError)
			{
				return;
			}
			ResponseCodeType responseCode = response.ResponseCode;
			if (responseCode == ResponseCodeType.ErrorDeleteUnifiedMessagingPromptFailed)
			{
				throw new DeleteContentException(string.Empty);
			}
			if (responseCode == ResponseCodeType.ErrorPromptPublishingOperationFailed)
			{
				throw new PublishingPointException(response.MessageText);
			}
			if (responseCode != ResponseCodeType.ErrorUnifiedMessagingPromptNotFound)
			{
				throw new EWSUMMailboxAccessException(Strings.EWSOperationFailed(response.ResponseCode.ToString(), response.MessageText));
			}
			throw new SourceFileNotFoundException(promptName);
		}

		private readonly Guid configurationObject;

		private UMMailboxAccessorEwsBinding ewsBinding;

		private DiagnosticHelper tracer;
	}
}
