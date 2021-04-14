using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class MessageOptionsDialog : OwaPage
	{
		protected static int ImportanceLow
		{
			get
			{
				return 0;
			}
		}

		protected static int ImportanceNormal
		{
			get
			{
				return 1;
			}
		}

		protected static int ImportanceHigh
		{
			get
			{
				return 2;
			}
		}

		protected static int SensitivityNormal
		{
			get
			{
				return 0;
			}
		}

		protected static int SensitivityPersonal
		{
			get
			{
				return 1;
			}
		}

		protected static int SensitivityPrivate
		{
			get
			{
				return 2;
			}
		}

		protected static int SensitivityCompanyConfidential
		{
			get
			{
				return 3;
			}
		}
	}
}
