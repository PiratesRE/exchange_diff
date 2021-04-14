using System;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace System.Security
{
	[ComVisible(true)]
	public interface IEvidenceFactory
	{
		Evidence Evidence { get; }
	}
}
