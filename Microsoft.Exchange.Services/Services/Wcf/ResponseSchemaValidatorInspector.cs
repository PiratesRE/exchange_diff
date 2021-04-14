using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml.Schema;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class ResponseSchemaValidatorInspector : IOutboundInspector
	{
		private string GetMessageResponseText()
		{
			return this.message.ToString();
		}

		public void ProcessOutbound(ExchangeVersion requestVersion, Message reply)
		{
			if (!this.ShouldValidateResponse(requestVersion))
			{
				return;
			}
			if (!reply.IsFault)
			{
				this.message = reply;
				bool treatWarningsAsErrors = MessageInspectorManager.IsAvailabilityRequest(reply.Headers.Action);
				SchemaValidator schemaValidator = new SchemaValidator(delegate(XmlSchemaException exception, SoapSavvyReader.SoapSection section)
				{
					throw FaultExceptionUtilities.CreateFault(new ResponseSchemaValidationException(exception, exception.LineNumber, exception.LinePosition, exception.Message, this.GetMessageResponseText()), FaultParty.Receiver);
				});
				string s = reply.ToString();
				using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(s)))
				{
					RequestDetailsLogger.Current.Set(ServiceCommonMetadata.ResponseSize, memoryStream.Length);
					schemaValidator.ValidateMessage(memoryStream, ExchangeVersion.Current, treatWarningsAsErrors, false);
				}
			}
		}

		private bool ShouldValidateResponse(ExchangeVersion requestVersion)
		{
			bool? enableSchemaValidationOverride = Global.EnableSchemaValidationOverride;
			if (enableSchemaValidationOverride != null)
			{
				return enableSchemaValidationOverride.Value;
			}
			bool flag = requestVersion > ExchangeVersion.Exchange2013;
			return !flag;
		}

		private Message message;
	}
}
