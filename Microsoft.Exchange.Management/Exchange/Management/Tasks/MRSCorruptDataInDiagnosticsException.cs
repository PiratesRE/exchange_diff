using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MRSCorruptDataInDiagnosticsException : MRSDiagnosticQueryException
	{
		public MRSCorruptDataInDiagnosticsException(string element, string value) : base(Strings.MRSCorruptDataInDiagnostics(element, value))
		{
			this.element = element;
			this.value = value;
		}

		public MRSCorruptDataInDiagnosticsException(string element, string value, Exception innerException) : base(Strings.MRSCorruptDataInDiagnostics(element, value), innerException)
		{
			this.element = element;
			this.value = value;
		}

		protected MRSCorruptDataInDiagnosticsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.element = (string)info.GetValue("element", typeof(string));
			this.value = (string)info.GetValue("value", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("element", this.element);
			info.AddValue("value", this.value);
		}

		public string Element
		{
			get
			{
				return this.element;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		private readonly string element;

		private readonly string value;
	}
}
