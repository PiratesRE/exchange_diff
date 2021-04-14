using System;

namespace System.Text
{
	internal enum ExtendedNormalizationForms
	{
		FormC = 1,
		FormD,
		FormKC = 5,
		FormKD,
		FormIdna = 13,
		FormCDisallowUnassigned = 257,
		FormDDisallowUnassigned,
		FormKCDisallowUnassigned = 261,
		FormKDDisallowUnassigned,
		FormIdnaDisallowUnassigned = 269
	}
}
