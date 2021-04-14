using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class Pop3BodyText
	{
		public Pop3BodyText(string contentType, string text)
		{
			this.ContentType = contentType;
			this.Text = text;
		}

		public string ContentType { get; private set; }

		public string Text { get; private set; }
	}
}
