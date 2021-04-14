using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetComplianceConfigurationResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetComplianceConfigurationResponseMessage : ResponseMessage
	{
		public GetComplianceConfigurationResponseMessage()
		{
		}

		internal GetComplianceConfigurationResponseMessage(IEnumerable<RmsTemplate> value, ServiceResultCode code = ServiceResultCode.Success, ServiceError error = null) : base(code, error)
		{
			this.InitializeRmsComplianceEntry(value);
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetComplianceConfigurationResponseMessage;
		}

		private void InitializeRmsComplianceEntry(IEnumerable<RmsTemplate> templates)
		{
			this.RmsTemplates = (from template in templates
			select new RmsComplianceEntry(template.Id.ToString(), template.Name, template.Description)).ToArray<RmsComplianceEntry>();
		}

		public RmsComplianceEntry[] RmsTemplates;
	}
}
