using System;
using Newtonsoft.Json;

namespace Appboy.NET {
	public class PushTokenRegistration {

		#region instance fields

		private readonly string _appID;
		private readonly string _token;

		#endregion


		#region constructors

		public PushTokenRegistration( string appID, string token ){
			if( appID == null )
				throw new ArgumentNullException( "appID" );
			if( token == null )
				throw new ArgumentNullException( "token" );

			_appID = appID;
			_token = token;
		}

		#endregion


		#region properties

		[JsonProperty( "app_id" )]
		public string AppID {
			get { return _appID; }
		}


		[JsonProperty( "token" )]
		public string Token {
			get { return _token; }
		}

		#endregion

	}
}
