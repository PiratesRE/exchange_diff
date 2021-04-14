using System;
using System.Xml;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ExecuteDiagnosticMethod : SingleStepServiceCommand<ExecuteDiagnosticMethodRequest, XmlNode>
	{
		public ExecuteDiagnosticMethod(CallContext callContext, ExecuteDiagnosticMethodRequest request) : base(callContext, request)
		{
		}

		internal override ServiceResult<XmlNode> Execute()
		{
			DiagnosticMethodDelegate @delegate = DiagnosticMethodDelegateCollection.Singleton.GetDelegate(base.Request.Verb);
			return new ServiceResult<XmlNode>(@delegate(base.Request.Parameter));
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			BaseInfoResponse baseInfoResponse = new ExecuteDiagnosticMethodResponse();
			baseInfoResponse.ProcessServiceResult<XmlNode>(base.Result);
			return baseInfoResponse;
		}

		public const string MethodName = "ExecuteDiagnosticMethod";
	}
}
