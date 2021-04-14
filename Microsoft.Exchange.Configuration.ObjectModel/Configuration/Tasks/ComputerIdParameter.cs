using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ComputerIdParameter : UserContactComputerIdParameter
	{
		public ComputerIdParameter(string identity) : base(identity)
		{
		}

		public ComputerIdParameter()
		{
		}

		public ComputerIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ComputerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return ComputerIdParameter.AllowedRecipientTypes;
			}
		}

		public new static ComputerIdParameter Parse(string identity)
		{
			return new ComputerIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeComputer(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.Computer
		};
	}
}
