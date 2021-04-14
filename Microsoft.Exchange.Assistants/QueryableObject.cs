using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class QueryableObject : ConfigurableObject
	{
		public QueryableObject() : base(new SimpleProviderPropertyBag())
		{
		}

		public Dictionary<string, object> ToDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (PropertyDefinition propertyDefinition in this.ObjectSchema.AllProperties)
			{
				object obj;
				if (string.Compare(propertyDefinition.Name, "ObjectState", true) != 0 && string.Compare(propertyDefinition.Name, "ExchangeVersion") != 0 && base.TryGetValueWithoutDefault(propertyDefinition, out obj) && obj != null)
				{
					if (typeof(QueryableObject).IsAssignableFrom(propertyDefinition.Type))
					{
						object arg = obj;
						SimpleProviderPropertyDefinition simpleProviderPropertyDefinition = propertyDefinition as SimpleProviderPropertyDefinition;
						if (simpleProviderPropertyDefinition != null)
						{
							if (simpleProviderPropertyDefinition.IsMultivalued)
							{
								List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
								if (QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site1 == null)
								{
									QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site1 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(IEnumerable), typeof(QueryableObject)));
								}
								using (IEnumerator enumerator2 = QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site1.Target(QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site1, arg).GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										if (QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site2 == null)
										{
											QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site2 = CallSite<Func<CallSite, object, QueryableObject>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(QueryableObject), typeof(QueryableObject)));
										}
										QueryableObject queryableObject = QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site2.Target(QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site2, enumerator2.Current);
										list.Add(queryableObject.ToDictionary());
									}
								}
								dictionary.Add(propertyDefinition.Name, list);
							}
							else
							{
								if (QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site3 == null)
								{
									QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site3 = CallSite<Action<CallSite, Dictionary<string, object>, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", null, typeof(QueryableObject), new CSharpArgumentInfo[]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Action<CallSite, Dictionary<string, object>, string, object> target = QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site3.Target;
								CallSite <>p__Site = QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site3;
								Dictionary<string, object> arg2 = dictionary;
								string name = propertyDefinition.Name;
								if (QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site4 == null)
								{
									QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site4 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToDictionary", null, typeof(QueryableObject), new CSharpArgumentInfo[]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								target(<>p__Site, arg2, name, QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site4.Target(QueryableObject.<ToDictionary>o__SiteContainer0.<>p__Site4, arg));
							}
						}
					}
					else
					{
						dictionary.Add(propertyDefinition.Name, obj);
					}
				}
			}
			return dictionary;
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		[CompilerGenerated]
		private static class <ToDictionary>o__SiteContainer0
		{
			public static CallSite<Func<CallSite, object, IEnumerable>> <>p__Site1;

			public static CallSite<Func<CallSite, object, QueryableObject>> <>p__Site2;

			public static CallSite<Action<CallSite, Dictionary<string, object>, string, object>> <>p__Site3;

			public static CallSite<Func<CallSite, object, object>> <>p__Site4;
		}
	}
}
