using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MRSExpectedDiagnosticsElementMissingException : MRSDiagnosticQueryException
	{
		public MRSExpectedDiagnosticsElementMissingException(string xPath, string xml) : base(Strings.MRSExpectedDiagnosticsElementMissing(xPath, xml))
		{
			this.xPath = xPath;
			this.xml = xml;
		}

		public MRSExpectedDiagnosticsElementMissingException(string xPath, string xml, Exception innerException) : base(Strings.MRSExpectedDiagnosticsElementMissing(xPath, xml), innerException)
		{
			this.xPath = xPath;
			this.xml = xml;
		}

		protected MRSExpectedDiagnosticsElementMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.xPath = (string)info.GetValue("xPath", typeof(string));
			this.xml = (string)info.GetValue("xml", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("xPath", this.xPath);
			info.AddValue("xml", this.xml);
		}

		public string XPath
		{
			get
			{
				return this.xPath;
			}
		}

		public string Xml
		{
			get
			{
				return this.xml;
			}
		}

		private readonly string xPath;

		private readonly string xml;
	}
}
