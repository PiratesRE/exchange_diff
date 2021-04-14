using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class QualifiedName
	{
		internal QualifiedName(string rawName, ActivityManagerConfig defaultNamespace) : this(rawName, (defaultNamespace == null) ? "GlobalActivityManager" : defaultNamespace.ClassName)
		{
		}

		internal QualifiedName(string rawName, string defaultNamespace)
		{
			if (-1 != rawName.IndexOf(":", StringComparison.InvariantCulture))
			{
				string[] array = rawName.Split(new char[]
				{
					':'
				});
				if (array == null || 2 != array.Length)
				{
					throw new FsmConfigurationException(Strings.InvalidQualifiedName(rawName));
				}
				this.nameSpace = array[0];
				this.shortName = array[1];
			}
			else
			{
				this.shortName = rawName;
				this.nameSpace = defaultNamespace;
			}
			this.fullName = defaultNamespace + ":" + this.shortName;
		}

		internal string FullName
		{
			get
			{
				return this.fullName;
			}
		}

		internal string ShortName
		{
			get
			{
				return this.shortName;
			}
		}

		internal string Namespace
		{
			get
			{
				return this.nameSpace;
			}
		}

		public override string ToString()
		{
			return this.fullName;
		}

		private string nameSpace;

		private string shortName;

		private string fullName;
	}
}
