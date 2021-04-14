using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	internal class MdbPeopleModelDataBinderFactory
	{
		protected MdbPeopleModelDataBinderFactory()
		{
		}

		public static MdbPeopleModelDataBinderFactory Current
		{
			get
			{
				return MdbPeopleModelDataBinderFactory.hookableInstance.Value;
			}
		}

		internal static Hookable<MdbPeopleModelDataBinderFactory> HookableInstance
		{
			get
			{
				return MdbPeopleModelDataBinderFactory.hookableInstance;
			}
		}

		public virtual MdbPeopleModelDataBinder CreateInstance(MailboxSession session)
		{
			return new MdbPeopleModelDataBinder(session);
		}

		private static Hookable<MdbPeopleModelDataBinderFactory> hookableInstance = Hookable<MdbPeopleModelDataBinderFactory>.Create(true, new MdbPeopleModelDataBinderFactory());
	}
}
