using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	[DataServiceKey("objectId")]
	public class Notification : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static Notification CreateNotification(string objectId, Collection<string> filters)
		{
			Notification notification = new Notification();
			notification.objectId = objectId;
			if (filters == null)
			{
				throw new ArgumentNullException("filters");
			}
			notification.filters = filters;
			return notification;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string callbackUri
		{
			get
			{
				return this._callbackUri;
			}
			set
			{
				this._callbackUri = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> filters
		{
			get
			{
				return this._filters;
			}
			set
			{
				this._filters = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _callbackUri;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _filters = new Collection<string>();
	}
}
