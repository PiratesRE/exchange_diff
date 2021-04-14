using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UrlHandlerNotFoundException : LocalizedException
	{
		public UrlHandlerNotFoundException(string url) : base(Strings.FailToOpenURL(url))
		{
			this.url = url;
		}

		public UrlHandlerNotFoundException(string url, Exception innerException) : base(Strings.FailToOpenURL(url), innerException)
		{
			this.url = url;
		}

		protected UrlHandlerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.url = (string)info.GetValue("url", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("url", this.url);
		}

		public string Url
		{
			get
			{
				return this.url;
			}
		}

		private readonly string url;
	}
}
