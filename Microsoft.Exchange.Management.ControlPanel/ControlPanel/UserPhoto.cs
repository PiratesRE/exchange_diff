using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UserPhoto : BaseRow
	{
		public UserPhoto(UserPhotoConfiguration photo) : base(photo.Identity.ToIdentity(string.Empty), photo)
		{
			this.Photo = photo;
		}

		internal UserPhotoConfiguration Photo { get; private set; }
	}
}
