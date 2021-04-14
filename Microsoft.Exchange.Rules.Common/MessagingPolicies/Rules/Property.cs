using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public abstract class Property : Argument
	{
		public Property(string propertyName, Type type) : base(type)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new RulesValidationException(RulesStrings.EmptyPropertyName);
			}
			this.propertyName = propertyName;
		}

		public string Name
		{
			get
			{
				return this.propertyName;
			}
		}

		public override object GetValue(RulesEvaluationContext context)
		{
			object obj = this.OnGetValue(context);
			if (obj != null)
			{
				Type type = obj.GetType();
				if (type != base.Type && !base.Type.IsAssignableFrom(type))
				{
					if (context.Tracer != null)
					{
						context.Tracer.TraceError("Property value is of the wrong type: {0}", new object[]
						{
							type
						});
					}
					return null;
				}
			}
			return obj;
		}

		public override int GetEstimatedSize()
		{
			int num = 0;
			if (this.propertyName != null)
			{
				num += this.propertyName.Length * 2;
				num += 18;
			}
			return num + base.GetEstimatedSize();
		}

		protected abstract object OnGetValue(RulesEvaluationContext context);

		private string propertyName;
	}
}
