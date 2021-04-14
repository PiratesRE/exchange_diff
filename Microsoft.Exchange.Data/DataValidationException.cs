using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataValidationException : LocalizedException, IErrorContextException
	{
		public DataValidationException(ValidationError error) : base(error.Description)
		{
			this.error = error;
			this.propertyName = error.PropertyName;
		}

		public DataValidationException(ValidationError error, Exception innerException) : base(error.Description, innerException)
		{
			this.error = error;
			this.propertyName = error.PropertyName;
		}

		protected DataValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public ValidationError Error
		{
			get
			{
				return this.error;
			}
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		public override string Message
		{
			get
			{
				if (this.context == null || this.context.ExecutionHost.Equals("Exchange Control Panel"))
				{
					return base.Message;
				}
				if (this.errorMessage == null)
				{
					this.errorMessage = base.Message + " " + DataStrings.PropertyName(this.propertyName);
				}
				return this.errorMessage;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		public void SetContext(IErrorExecutionContext context)
		{
			this.context = context;
		}

		private readonly ValidationError error;

		private readonly string propertyName;

		private IErrorExecutionContext context;

		private string errorMessage;
	}
}
