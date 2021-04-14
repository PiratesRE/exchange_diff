using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class WacDiscoveryResultSuccess : WacDiscoveryResultBase
	{
		public WacDiscoveryResultSuccess()
		{
			this.wacViewUrlTemplateMapping = new Dictionary<string, string>(13);
			this.wacEditUrlTemplateMapping = new Dictionary<string, string>(13);
			this.viewOnlyFileTypes = new HashSet<string>
			{
				".odt",
				".ott",
				".fodt",
				".fott",
				".ods",
				".ots",
				".fods",
				".fots",
				".odp",
				".otp",
				".fodp",
				".fotp",
				".odc",
				".otc",
				".fodc",
				".fotc",
				".odi",
				".oti",
				".fodi",
				".foti",
				".odg",
				".otg",
				".fodg",
				".fotg",
				".odf",
				".otf",
				".fodf",
				".fotf",
				".odb",
				".fodb",
				".odm",
				".fodm",
				".oth",
				".foth"
			};
		}

		public override string[] WacViewableFileTypes
		{
			get
			{
				if (!this.isInitialized)
				{
					throw new InvalidOperationException("This operation should not be invoked when the object has not been completely initialized yet.");
				}
				return this.wacSupportedFileTypes;
			}
		}

		public override string[] WacEditableFileTypes
		{
			get
			{
				if (!this.isInitialized)
				{
					throw new InvalidOperationException("This operation should not be invoked when the object has not been completely initialized yet.");
				}
				return this.wacEditableFileTypes;
			}
		}

		public override string GetWacViewableFileTypesDisplayText()
		{
			StringBuilder stringBuilder = new StringBuilder(40);
			foreach (string text in this.wacViewUrlTemplateMapping.Keys)
			{
				stringBuilder.Append(text);
				stringBuilder.Append("-> ");
				stringBuilder.Append(this.wacViewUrlTemplateMapping[text]);
				stringBuilder.Append(";");
			}
			return stringBuilder.ToString();
		}

		public override void AddViewMapping(string fileExtension, string path)
		{
			if (this.isInitialized)
			{
				throw new InvalidOperationException("This operation should not be invoked once the object has been marked as completely initialized");
			}
			fileExtension = fileExtension.ToLowerInvariant();
			this.wacViewUrlTemplateMapping[fileExtension] = path;
		}

		public override void AddEditMapping(string fileExtension, string path)
		{
			if (this.isInitialized)
			{
				throw new InvalidOperationException("This operation should not be invoked once the object has been marked as completely initialized");
			}
			fileExtension = fileExtension.ToLowerInvariant();
			if (this.viewOnlyFileTypes.Contains(fileExtension))
			{
				return;
			}
			this.wacEditUrlTemplateMapping[fileExtension] = path;
		}

		public override bool TryGetViewUrlForFileExtension(string extension, string cultureName, out string url)
		{
			return WacDiscoveryResultSuccess.TryGetUrlForFileExtension(this.wacViewUrlTemplateMapping, "view", extension, cultureName, out url);
		}

		public override bool TryGetEditUrlForFileExtension(string extension, string cultureName, out string url)
		{
			return WacDiscoveryResultSuccess.TryGetUrlForFileExtension(this.wacEditUrlTemplateMapping, "edit", extension, cultureName, out url);
		}

		public override void MarkInitializationComplete()
		{
			this.CreateSupportedItemsArrayFromMapping();
			this.CreateEditableItemsArrayFromMapping();
			this.isInitialized = true;
		}

		private static bool TryGetUrlForFileExtension(Dictionary<string, string> mapping, string verb, string extension, string cultureName, out string url)
		{
			if (string.IsNullOrEmpty(extension))
			{
				throw new ArgumentException("extension");
			}
			if (mapping == null)
			{
				throw new ArgumentException("mapping");
			}
			extension = extension.ToLowerInvariant();
			if (!mapping.ContainsKey(extension))
			{
				url = null;
				return false;
			}
			string text = mapping[extension];
			url = text.Replace("<ui=UI_LLCC&>", string.Format("ui={0}&", cultureName));
			url = url.Replace("<rs=DC_LLCC&>", string.Format("rs={0}&", cultureName));
			url = url.Replace("<showpagestats=PERFSTATS&>", string.Empty);
			if (!WacConfiguration.Instance.UseHttpsForWacUrl)
			{
				url = url.Replace("https", "http");
			}
			return true;
		}

		private void CreateSupportedItemsArrayFromMapping()
		{
			int count = this.wacViewUrlTemplateMapping.Keys.Count;
			this.wacSupportedFileTypes = new string[count];
			int num = 0;
			foreach (string text in this.wacViewUrlTemplateMapping.Keys)
			{
				this.wacSupportedFileTypes[num++] = text;
			}
		}

		private void CreateEditableItemsArrayFromMapping()
		{
			int count = this.wacEditUrlTemplateMapping.Keys.Count;
			this.wacEditableFileTypes = new string[count];
			int num = 0;
			foreach (string text in this.wacEditUrlTemplateMapping.Keys)
			{
				this.wacEditableFileTypes[num++] = text;
			}
		}

		private Dictionary<string, string> wacViewUrlTemplateMapping;

		private Dictionary<string, string> wacEditUrlTemplateMapping;

		private string[] wacSupportedFileTypes;

		private string[] wacEditableFileTypes;

		private HashSet<string> viewOnlyFileTypes;

		private bool isInitialized;
	}
}
