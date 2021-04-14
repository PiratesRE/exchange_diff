using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class AgentInfo
	{
		public AgentInfo(string agent, string type, string factory, string path, bool enabled, bool isInternal)
		{
			this.agentName = agent;
			this.baseTypeName = type;
			this.factoryTypeName = factory;
			this.factoryAssemblyPath = path;
			this.enabled = enabled;
			this.isInternal = isInternal;
			this.id = this.agentName.ToLower(CultureInfo.InvariantCulture);
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public string AgentName
		{
			get
			{
				return this.agentName;
			}
		}

		public string BaseTypeName
		{
			get
			{
				return this.baseTypeName;
			}
		}

		public string FactoryTypeName
		{
			get
			{
				return this.factoryTypeName;
			}
		}

		public string FactoryAssemblyPath
		{
			get
			{
				return this.factoryAssemblyPath;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		public bool IsInternal
		{
			get
			{
				return this.isInternal;
			}
		}

		private readonly string id;

		private readonly string agentName;

		private readonly string baseTypeName;

		private readonly string factoryTypeName;

		private readonly string factoryAssemblyPath;

		private bool isInternal;

		private bool enabled;
	}
}
