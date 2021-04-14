using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
	public class DDICollectionDecoratorAttribute : DDIDecoratorAttribute
	{
		public DDICollectionDecoratorAttribute() : base("DDICollectionDecoratorAttribute")
		{
		}

		[DefaultValue(false)]
		public bool ExpandInnerCollection { get; set; }

		[DefaultValue(null)]
		public Type ObjectConverter { get; set; }

		public override List<string> Validate(object target, Service profile)
		{
			DDIValidateAttribute ddiattribute = base.GetDDIAttribute();
			if (ddiattribute == null)
			{
				throw new ArgumentException(string.Format("{0} is not a valid DDIAttribute", base.AttributeType));
			}
			List<string> list = new List<string>();
			if (target != null)
			{
				IEnumerable enumerable = target as IEnumerable;
				if (enumerable == null)
				{
					throw new ArgumentException("DDICollectionDecoratorAttribute can only be applied to collection target");
				}
				IDDIValidationObjectConverter iddivalidationObjectConverter = null;
				if (this.ObjectConverter != null)
				{
					iddivalidationObjectConverter = (Activator.CreateInstance(this.ObjectConverter) as IDDIValidationObjectConverter);
				}
				foreach (object obj in enumerable)
				{
					if (this.ExpandInnerCollection && obj is IEnumerable)
					{
						using (IEnumerator enumerator2 = (obj as IEnumerable).GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								object target2 = (iddivalidationObjectConverter == null) ? obj2 : iddivalidationObjectConverter.ConvertTo(obj2);
								list.AddRange(ddiattribute.Validate(target2, profile));
							}
							continue;
						}
					}
					object target3 = (iddivalidationObjectConverter == null) ? obj : iddivalidationObjectConverter.ConvertTo(obj);
					list.AddRange(ddiattribute.Validate(target3, profile));
				}
			}
			return list;
		}
	}
}
