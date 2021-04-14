using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class Pop3Attachment
	{
		public Pop3Attachment(string contentType, string name, byte[] content)
		{
			this.ContentType = contentType;
			this.Name = name;
			this.Content = content;
		}

		public string ContentType { get; private set; }

		public string Name { get; private set; }

		public byte[] Content { get; private set; }
	}
}
