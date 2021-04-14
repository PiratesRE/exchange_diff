using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MRSDiagnosticXmlParsingException : MRSDiagnosticQueryException
	{
		public MRSDiagnosticXmlParsingException(string error, string xml) : base(Strings.MRSDiagnosticXmlParsingError(error, xml))
		{
			this.error = error;
			this.xml = xml;
		}

		public MRSDiagnosticXmlParsingException(string error, string xml, Exception innerException) : base(Strings.MRSDiagnosticXmlParsingError(error, xml), innerException)
		{
			this.error = error;
			this.xml = xml;
		}

		protected MRSDiagnosticXmlParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
			this.xml = (string)info.GetValue("xml", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
			info.AddValue("xml", this.xml);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		public string Xml
		{
			get
			{
				return this.xml;
			}
		}

		private readonly string error;

		private readonly string xml;
	}
}
