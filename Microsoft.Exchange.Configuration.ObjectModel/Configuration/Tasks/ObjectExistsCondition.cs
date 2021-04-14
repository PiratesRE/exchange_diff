using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ObjectExistsCondition : Condition
	{
		public ObjectExistsCondition(string identity, Type type)
		{
			this.ObjectIdentity = identity;
			this.ObjectType = type;
		}

		public override bool Verify()
		{
			TaskLogger.LogEnter();
			if ("" == this.ObjectIdentity)
			{
				throw new ConditionInitializationException("ObjectIdentity", this);
			}
			if (this.ObjectIdentity == null)
			{
				throw new ConditionInitializationException("ObjectIdentity", this);
			}
			if (null == this.ObjectType)
			{
				throw new ConditionInitializationException("ObjectType", this);
			}
			if (!this.ObjectType.IsSubclassOf(typeof(ConfigObject)) || this.ObjectType.IsAbstract)
			{
				throw new ConditionInitializationException("ObjectType", this, new LocalizedException(Strings.ExceptionInvalidConfigObjectType(this.ObjectType)));
			}
			ConfigObject configObject = ConfigObjectReader.FindById(this.ObjectType, this.ObjectIdentity);
			bool result = configObject != null;
			TaskLogger.LogExit();
			return result;
		}

		public string ObjectIdentity
		{
			get
			{
				return this.objectIdentity;
			}
			set
			{
				this.objectIdentity = value;
			}
		}

		public Type ObjectType
		{
			get
			{
				return this.objectType;
			}
			set
			{
				this.objectType = value;
			}
		}

		private string objectIdentity;

		private Type objectType;
	}
}
