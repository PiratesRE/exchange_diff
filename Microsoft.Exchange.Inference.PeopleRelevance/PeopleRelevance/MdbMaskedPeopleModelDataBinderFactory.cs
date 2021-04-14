using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	internal class MdbMaskedPeopleModelDataBinderFactory
	{
		protected MdbMaskedPeopleModelDataBinderFactory()
		{
		}

		public static MdbMaskedPeopleModelDataBinderFactory Current
		{
			get
			{
				return MdbMaskedPeopleModelDataBinderFactory.hookableInstance.Value;
			}
		}

		internal static Hookable<MdbMaskedPeopleModelDataBinderFactory> HookableInstance
		{
			get
			{
				return MdbMaskedPeopleModelDataBinderFactory.hookableInstance;
			}
		}

		public virtual MdbMaskedPeopleModelDataBinder CreateInstance(MailboxSession session)
		{
			return new MdbMaskedPeopleModelDataBinder(session);
		}

		private static Hookable<MdbMaskedPeopleModelDataBinderFactory> hookableInstance = Hookable<MdbMaskedPeopleModelDataBinderFactory>.Create(true, new MdbMaskedPeopleModelDataBinderFactory());
	}
}
