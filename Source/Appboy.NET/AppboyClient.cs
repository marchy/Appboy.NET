using System;
using System.Diagnostics.Contracts;

namespace Appboy.NET {
	public class AppboyClient {

		#region constants
		
		public const int MaxUserAttributesPerRequest = 50;
		public const int MaxCustomStringAttributesLength = 255;

		#endregion


		#region instance fields

		private readonly string _companySecret;
		private readonly string _appGroupID;

		#endregion


		#region constructors

		public AppboyClient( string companySecret, string appGroupID ){
			Contract.Requires( !string.IsNullOrWhiteSpace( companySecret ));
			Contract.Requires( !string.IsNullOrWhiteSpace( appGroupID ));
			if( companySecret == null )
				throw new ArgumentNullException( "companySecret" );
			if( appGroupID == null )
				throw new ArgumentNullException( "appGroupID" );

			_companySecret = companySecret;
			_appGroupID = appGroupID;
		}

		#endregion

		
		#region operations

		public AppboyRequestBuilder BuildUpRequest() {
			return new AppboyRequestBuilder( _companySecret, _appGroupID );
		}

		#endregion

	}
}
