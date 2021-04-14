using System;
using System.Linq;
using System.Management.Automation;
using System.Runtime.Serialization;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.DatacenterStrings;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ErrorRecord : ErrorInformationBase
	{
		public ErrorRecord PSErrorRecord { get; set; }

		public ErrorRecord()
		{
		}

		public ErrorRecord(ErrorRecord errorRecord) : base(errorRecord.Exception)
		{
			this.PSErrorRecord = errorRecord;
			this.TargetObject = ((errorRecord.TargetObject != null) ? errorRecord.TargetObject.ToString() : null);
			ErrorDetails errorDetails = errorRecord.ErrorDetails;
			if (errorDetails != null)
			{
				if (!string.IsNullOrEmpty(errorDetails.Message))
				{
					this.Message = errorDetails.Message;
				}
				if (!string.IsNullOrEmpty(errorDetails.RecommendedAction))
				{
					this.RecommendedAction = errorDetails.RecommendedAction;
				}
			}
		}

		public ErrorRecord(Exception exception) : base(exception)
		{
		}

		[DataMember]
		public override string Message
		{
			protected set
			{
				base.Message = (ErrorHandlingUtil.AddSourceToErrorMessages ? ("[Error message from cmdlet] " + value) : value);
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string RecommendedAction { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string TargetObject { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ErrorRecordContext Context { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Property { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Type { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string HelpUrl { get; set; }

		public override Exception Exception
		{
			protected set
			{
				base.Exception = value;
				Exception ex = this.Exception;
				if (this.Exception is ParameterBindingException)
				{
					this.Property = (this.Exception as ParameterBindingException).ParameterName;
					if (this.Exception.InnerException != null && this.Exception.InnerException.InnerException != null)
					{
						Exception innerException = this.Exception.InnerException.InnerException;
						ex = ((innerException.InnerException != null && innerException.GetType().Equals(typeof(PSInvalidCastException))) ? innerException.InnerException : innerException);
					}
				}
				else if (this.Exception is DataValidationException)
				{
					this.Property = (this.Exception as DataValidationException).PropertyName;
				}
				if (ex != this.Exception)
				{
					base.Exception = ex;
				}
				this.Message = ex.Message;
				Type type = ex.GetType();
				if (ErrorRecord.ExceptionTypeExposedToClient.Contains(type) || typeof(TransientException).IsInstanceOfType(ex))
				{
					this.Type = type.ToString();
				}
				LocalizedException ex2 = this.Exception as LocalizedException;
				if (ex2 != null)
				{
					this.HelpUrl = HelpUtil.BuildErrorAssistanceUrl(ex2);
				}
			}
		}

		private static readonly Type[] ExceptionTypeExposedToClient = new Type[]
		{
			typeof(AutoProvisionFailedException),
			typeof(WLCDUnmanagedMemberExistsException),
			typeof(ShouldContinueException),
			typeof(InvalidOperationInDehydratedContextException),
			typeof(NameNotAvailableException),
			typeof(IDSInternalException),
			typeof(WLCDManagedMemberExistsException),
			typeof(PropertyValueExistsException)
		};
	}
}
