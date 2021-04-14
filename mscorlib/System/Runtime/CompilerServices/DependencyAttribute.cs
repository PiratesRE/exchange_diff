using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	[Serializable]
	public sealed class DependencyAttribute : Attribute
	{
		public DependencyAttribute(string dependentAssemblyArgument, LoadHint loadHintArgument)
		{
			this.dependentAssembly = dependentAssemblyArgument;
			this.loadHint = loadHintArgument;
		}

		public string DependentAssembly
		{
			get
			{
				return this.dependentAssembly;
			}
		}

		public LoadHint LoadHint
		{
			get
			{
				return this.loadHint;
			}
		}

		private string dependentAssembly;

		private LoadHint loadHint;
	}
}
