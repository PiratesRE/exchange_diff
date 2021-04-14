using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
	public class DDICollectionDecoratorAttribute : DDIDecoratorAttribute
	{
		public DDICollectionDecoratorAttribute() : base("DDICollectionDecoratorAttribute")
		{
		}

		[DefaultValue(false)]
		public bool ExpandInnerCollection { get; set; }

		public override List<string> Validate(object target, PageConfigurableProfile profile)
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
				foreach (object obj in enumerable)
				{
					if (this.ExpandInnerCollection && obj is IEnumerable)
					{
						using (IEnumerator enumerator2 = (obj as IEnumerable).GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								object target2 = enumerator2.Current;
								list.AddRange(ddiattribute.Validate(target2, profile));
							}
							continue;
						}
					}
					list.AddRange(ddiattribute.Validate(obj, profile));
				}
			}
			return list;
		}
	}
}
