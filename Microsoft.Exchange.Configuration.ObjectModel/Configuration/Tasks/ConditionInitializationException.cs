using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConditionInitializationException : LocalizedException
	{
		public ConditionInitializationException(string uninitializedProperty, Condition owningCondition) : base(Strings.ConditionNotInitialized(uninitializedProperty, owningCondition))
		{
			this.uninitializedProperty = uninitializedProperty;
			this.owningCondition = owningCondition;
		}

		public ConditionInitializationException(string uninitializedProperty, Condition owningCondition, Exception innerException) : base(Strings.ConditionNotInitialized(uninitializedProperty, owningCondition), innerException)
		{
			this.uninitializedProperty = uninitializedProperty;
			this.owningCondition = owningCondition;
		}

		protected ConditionInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.uninitializedProperty = (string)info.GetValue("uninitializedProperty", typeof(string));
			this.owningCondition = (Condition)info.GetValue("owningCondition", typeof(Condition));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("uninitializedProperty", this.uninitializedProperty);
			info.AddValue("owningCondition", this.owningCondition);
		}

		public string UninitializedProperty
		{
			get
			{
				return this.uninitializedProperty;
			}
		}

		public Condition OwningCondition
		{
			get
			{
				return this.owningCondition;
			}
		}

		private readonly string uninitializedProperty;

		private readonly Condition owningCondition;
	}
}
