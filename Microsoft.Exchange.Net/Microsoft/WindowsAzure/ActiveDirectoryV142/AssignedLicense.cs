using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class AssignedLicense
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static AssignedLicense CreateAssignedLicense(Collection<Guid> disabledPlans)
		{
			AssignedLicense assignedLicense = new AssignedLicense();
			if (disabledPlans == null)
			{
				throw new ArgumentNullException("disabledPlans");
			}
			assignedLicense.disabledPlans = disabledPlans;
			return assignedLicense;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<Guid> disabledPlans
		{
			get
			{
				return this._disabledPlans;
			}
			set
			{
				this._disabledPlans = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Guid? skuId
		{
			get
			{
				return this._skuId;
			}
			set
			{
				this._skuId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<Guid> _disabledPlans = new Collection<Guid>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _skuId;
	}
}
