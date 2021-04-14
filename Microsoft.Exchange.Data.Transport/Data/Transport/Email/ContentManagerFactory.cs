using System;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class ContentManagerFactory
	{
		private ContentManagerFactory()
		{
			try
			{
				Assembly.Load("Microsoft.Exchange.UnifiedContent");
				this.installed = true;
			}
			catch (FileNotFoundException)
			{
				this.installed = false;
			}
		}

		internal static ContentManagerFactory Instance
		{
			get
			{
				return ContentManagerFactory.instance;
			}
		}

		internal static IDisposable ExtractContentManager(EmailMessage msg)
		{
			if (msg.ContentManager == null)
			{
				msg.ContentManager = ContentManagerFactory.Instance.Create();
				if (msg.ContentManager == null)
				{
					throw new InvalidOperationException("ContentManager is not available");
				}
			}
			return msg.ContentManager;
		}

		internal IDisposable Create()
		{
			if (!this.installed)
			{
				return null;
			}
			ObjectHandle objectHandle = Activator.CreateInstance("Microsoft.Exchange.UnifiedContent", "Microsoft.Exchange.UnifiedContent.ContentManager", false, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[]
			{
				this.GetTempFilePath()
			}, null, null);
			return (IDisposable)objectHandle.Unwrap();
		}

		private string GetTempFilePath()
		{
			return TemporaryDataStorage.GetTempPath();
		}

		private const string AssemblyNameLoad = "Microsoft.Exchange.UnifiedContent";

		private const string TheType = "Microsoft.Exchange.UnifiedContent.ContentManager";

		private static readonly ContentManagerFactory instance = new ContentManagerFactory();

		private readonly bool installed;
	}
}
