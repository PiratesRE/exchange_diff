using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal sealed class RedactAttribute : Attribute
	{
		static RedactAttribute()
		{
			RedactAttribute.redactionFunctions.Add(typeof(string), delegate(PropertyInfo property, object propertyInstance)
			{
				RedactAttribute.Redact(property, propertyInstance, (string value) => SuppressingPiiData.Redact(value));
			});
			RedactAttribute.redactionFunctions.Add(typeof(SmtpAddress), delegate(PropertyInfo property, object propertyInstance)
			{
				RedactAttribute.Redact(property, propertyInstance, (string value) => SuppressingPiiData.RedactSmtpAddress(value));
			});
		}

		public Type RedactAs { get; set; }

		internal void Redact(PropertyInfo property, object propertyInstance)
		{
			Type type = (this.RedactAs != null) ? this.RedactAs : property.PropertyType;
			RedactAttribute.RedactionDelegate redactionDelegate;
			if (RedactAttribute.redactionFunctions.TryGetValue(type, out redactionDelegate))
			{
				redactionDelegate(property, propertyInstance);
				return;
			}
			throw new NotSupportedException(string.Format("Redaction is not supported for {0}.", type.Name));
		}

		private static void Redact(PropertyInfo property, object propertyInstance, Func<string, string> redactionFunction)
		{
			string text = property.GetValue(propertyInstance) as string;
			if (text != null)
			{
				string value = redactionFunction(text);
				property.SetValue(propertyInstance, value);
			}
		}

		private static readonly Dictionary<Type, RedactAttribute.RedactionDelegate> redactionFunctions = new Dictionary<Type, RedactAttribute.RedactionDelegate>();

		private delegate void RedactionDelegate(PropertyInfo property, object propertyInstance);
	}
}
