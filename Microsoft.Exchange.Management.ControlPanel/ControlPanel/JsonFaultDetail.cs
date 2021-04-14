using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class JsonFaultDetail : ErrorInformationBase
	{
		public JsonFaultDetail(Exception ex) : base(ex)
		{
			this.ExceptionDetail = new ExceptionDetail(ex);
			this.ExceptionType = ex.GetType().FullName;
			if (!string.IsNullOrEmpty(base.CallStack))
			{
				this.StackTrace = base.CallStack;
				base.CallStack = string.Empty;
			}
			if (!(ex is FaultException) && PowerShellMessageTranslator.ShouldTranslate)
			{
				this.Message = PowerShellMessageTranslator.Instance.Translate(null, ex, Strings.WebServiceErrorMessage);
			}
		}

		[DataMember]
		public ExceptionDetail ExceptionDetail { get; set; }

		[DataMember]
		public string ExceptionType { get; set; }

		[DataMember]
		public string StackTrace { get; set; }
	}
}
