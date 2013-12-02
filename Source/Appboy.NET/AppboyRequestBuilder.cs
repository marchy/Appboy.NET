using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Security.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using JsonSerializer = RestSharp.Serializers.JsonSerializer;

namespace Appboy.NET {
	public class AppboyRequestBuilder {

		#region constants

		private const string BaseEndpointURL = "https://api.appboy.com/";
		private const string UserTrackingEndpointPath = "users/track";

		#endregion


		#region instance fields

		private readonly string _companySecret;
		private readonly string _appGroupID;
		private readonly List<UserAttributes> _userAttributes;

		#endregion
		

		#region constructor

		public AppboyRequestBuilder( string companySecret, string appGroupID ){
			Contract.Requires( !string.IsNullOrWhiteSpace( companySecret ));
			Contract.Requires( !string.IsNullOrWhiteSpace( appGroupID ));
			if( companySecret == null )
				throw new ArgumentNullException( "companySecret" );
			if( appGroupID == null )
				throw new ArgumentNullException( "appGroupID" );

			_companySecret = companySecret;
			_appGroupID = appGroupID;

			_userAttributes = new List<UserAttributes>();
//			Contract.Ensures( _usersToAdd != null );
		}

		#endregion


		#region properties

		public int MaxUserAttributesPerRequest {
			get {
				return AppboyClient.MaxUserAttributesPerRequest;
			}
		}


		public ICollection<UserAttributes> UserAttributes {
			get {
				return _userAttributes.AsReadOnly();
			}
		}

		#endregion


		#region public methods

		/// <summary>
		/// Adds attributes for a single user to the pending request.
		/// </summary>
		/// <param name="userAttributes"></param>
		public AppboyRequestBuilder AddUserAttributes( UserAttributes userAttributes ){
			Contract.Requires( userAttributes != null );
			if( userAttributes == null )
				throw new ArgumentNullException( "userAttributes" );
			
			AddUserAttributes( new[] { userAttributes });

			return this;
		}


		/// <summary>
		/// Adds attributes for a multiple users to the pending request.
		/// </summary>
		/// <param name="users"></param>
		public AppboyRequestBuilder AddUserAttributes( IEnumerable<UserAttributes> users ){
			Contract.Requires( users != null );
			if( users == null )
				throw new ArgumentNullException( "users" );
			// ensure within max limit
			if( _userAttributes.Count >= 50 )
				throw new InvalidOperationException( string.Format( "Maximum of {0} user attributes allowed per request", MaxUserAttributesPerRequest ));

			_userAttributes.AddRange( users );

			return this;
		}


		public void SendDataAsync( Action<ImportResult> onSuccess, Action<Exception> onError ){
			Contract.Requires( onSuccess != null );
			Contract.Requires( onError != null );
			if( onSuccess == null )
				throw new ArgumentNullException( "onSuccess" );
			if( onError == null )
				throw new ArgumentNullException( "onError" );

			var restClient = new RestClient( BaseEndpointURL );
			var request = new RestRequest( UserTrackingEndpointPath, Method.POST ){
				RequestFormat = DataFormat.Json,
				JsonSerializer = new JsonSerializer()
			};
			var appboyRequest = new AppboyRequestDTO {
				CompanySecret = _companySecret,
				AppGroupID = _appGroupID,
				Attributes = _userAttributes,
			};
			request.AddBody( appboyRequest );
			restClient.ExecuteAsyncPost( request, ( response, handle ) => {
				// TODO: handler success and error scenarios. Call callbacks accordingly
				if( response.ErrorException != null ){
					// TODO: parse error message if needed (is it made up of JSON content or a simple string?)

					onError( new Exception( response.ErrorMessage ));
				}

				// process result
				dynamic responseJSON;
				string message;
				switch( response.StatusCode ){
					case HttpStatusCode.OK:
					case HttpStatusCode.Created:
						break;

					// NOTE: This may not be necessary if the client library is coded properly. But here just in case!
					case HttpStatusCode.BadRequest:
						responseJSON = JObject.Parse( response.Content );
						message = responseJSON.message;
						onError( new InvalidDataException( message ));
							
						return;

					case HttpStatusCode.Unauthorized:
						responseJSON = JObject.Parse( response.Content );
						message = responseJSON.message;
						onError( new AuthenticationException( message ));

						return;

					case HttpStatusCode.NotFound:
						responseJSON = JObject.Parse( response.Content );
						message = responseJSON.message;
						onError( new KeyNotFoundException( message ));

						return;

					case (HttpStatusCode)429:
						responseJSON = JObject.Parse( response.Content );
						message = responseJSON.message;
						onError( new IndexOutOfRangeException( message ));

						return;

					default:
						throw new Exception( response.ErrorMessage );
				}

				// TODO: parse response
//				var responseJSON = JsonConvert.DeserializeObject( response.Content );
				ImportResult importResult = JsonConvert.DeserializeObject<ImportResult>( response.Content );
//				var responseJSON3 = JObject.Parse( response.Content );

//				var responseJSON = JsonConvert.DeserializeObject<>(  )
				// TODO: model success object (needs to include non-fatal errors)
 				onSuccess( importResult );

			}, Method.POST.ToString() );
		}

		#endregion
		

		#region private methods

//		private void ObjectInvariant() {
//			Contract.Invariant( _usersAttributes != null );
//		}

		#endregion

	}
}
