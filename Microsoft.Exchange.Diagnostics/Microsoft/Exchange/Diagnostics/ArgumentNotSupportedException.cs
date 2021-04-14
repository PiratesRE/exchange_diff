using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Diagnostics
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ArgumentNotSupportedException : DiagnosticArgumentException
	{
		public ArgumentNotSupportedException(string argumentName, string supportedArguments) : base(DiagnosticsResources.ArgumentNotSupported(argumentName, supportedArguments))
		{
			this.argumentName = argumentName;
			this.supportedArguments = supportedArguments;
		}

		public ArgumentNotSupportedException(string argumentName, string supportedArguments, Exception innerException) : base(DiagnosticsResources.ArgumentNotSupported(argumentName, supportedArguments), innerException)
		{
			this.argumentName = argumentName;
			this.supportedArguments = supportedArguments;
		}

		protected ArgumentNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.argumentName = (string)info.GetValue("argumentName", typeof(string));
			this.supportedArguments = (string)info.GetValue("supportedArguments", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("argumentName", this.argumentName);
			info.AddValue("supportedArguments", this.supportedArguments);
		}

		public string ArgumentName
		{
			get
			{
				return this.argumentName;
			}
		}

		public string SupportedArguments
		{
			get
			{
				return this.supportedArguments;
			}
		}

		private readonly string argumentName;

		private readonly string supportedArguments;
	}
}
