using System;

namespace Windows.Foundation.Diagnostics
{
	internal enum CausalityRelation
	{
		AssignDelegate,
		Join,
		Choice,
		Cancel,
		Error
	}
}
