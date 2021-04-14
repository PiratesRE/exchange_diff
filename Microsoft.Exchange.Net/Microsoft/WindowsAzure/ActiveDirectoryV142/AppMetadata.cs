using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class AppMetadata
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static AppMetadata CreateAppMetadata(int version, Collection<AppMetadataEntry> data)
		{
			AppMetadata appMetadata = new AppMetadata();
			appMetadata.version = version;
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			appMetadata.data = data;
			return appMetadata;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public int version
		{
			get
			{
				return this._version;
			}
			set
			{
				this._version = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<AppMetadataEntry> data
		{
			get
			{
				return this._data;
			}
			set
			{
				this._data = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private int _version;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<AppMetadataEntry> _data = new Collection<AppMetadataEntry>();
	}
}
