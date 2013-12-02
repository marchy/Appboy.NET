using Newtonsoft.Json;

namespace Appboy.NET {
	public class ImportResult {

		#region properties

		[JsonProperty( "attributes_processed" )]
		public int NumAttributesProcessed { get; set; }

		#endregion

	}
}
