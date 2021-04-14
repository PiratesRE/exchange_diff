using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SetTimeZone : ServiceCommand<bool>
	{
		public SetTimeZone(CallContext callContext, string newTimeZone) : base(callContext)
		{
			this.newTimeZone = newTimeZone;
		}

		protected override bool InternalExecute()
		{
			UserConfigurationPropertyDefinition propertyDefinition = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.TimeZone);
			new UserOptionsType
			{
				TimeZone = this.newTimeZone
			}.Commit(base.CallContext, new UserConfigurationPropertyDefinition[]
			{
				propertyDefinition
			});
			return true;
		}

		private readonly string newTimeZone;
	}
}
