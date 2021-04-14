using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public abstract class ResourceBase
	{
		public ResourceBase(string resourceName, ResourceTarget.Filter targetFilter, string currentOwaVersion, bool hasUserSpecificData)
		{
			if (resourceName != null)
			{
				this.resourceName = resourceName.ToLowerInvariant();
			}
			this.hasUserSpecificData = hasUserSpecificData;
			this.targetFilter = targetFilter;
			this.resourcesRelativeFolderPath = ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(currentOwaVersion);
		}

		public bool HasUserSpecificData
		{
			get
			{
				return this.hasUserSpecificData;
			}
		}

		public ResourceTarget.Filter TargetFilter
		{
			get
			{
				return this.targetFilter;
			}
		}

		public string ResourcesRelativeFolderPath
		{
			get
			{
				return this.resourcesRelativeFolderPath;
			}
		}

		internal virtual string ResourceName
		{
			get
			{
				return this.resourceName;
			}
		}

		public abstract string GetResourcePath(IPageContext pageContext, bool isBootResourcePath);

		protected static string CombinePath(params string[] parts)
		{
			for (int i = 0; i < parts.Length - 1; i++)
			{
				parts[i] = parts[i].TrimEnd(new char[]
				{
					'/'
				});
				parts[i + 1] = parts[i + 1].TrimStart(new char[]
				{
					'/'
				});
			}
			return string.Join("/", parts);
		}

		private readonly string resourceName;

		private readonly bool hasUserSpecificData;

		private readonly ResourceTarget.Filter targetFilter;

		private readonly string resourcesRelativeFolderPath;
	}
}
