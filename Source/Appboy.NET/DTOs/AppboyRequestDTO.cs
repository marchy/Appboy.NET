using System.Collections.Generic;
using Newtonsoft.Json;

namespace Appboy.NET {
	internal class AppboyRequestDTO {

		#region properties

		[JsonProperty( "company_secret" )]
		public string CompanySecret { get; set; }

		[JsonProperty( "app_group_id" )]
		public string AppGroupID { get; set; }

		[JsonProperty( "attributes" )]
		public IEnumerable<UserAttributes> Attributes { get; set; }

		// TODO
//		[JsonProperty( "events" )]
//		public IEnumerable<UserAttributes> Events { get; set; }
//
//		[JsonProperty( "purchases" )]
//		public IEnumerable<UserAttributes> Purchases { get; set; }

		#endregion

	}
}
