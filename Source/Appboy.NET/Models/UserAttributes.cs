using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;

namespace Appboy.NET {
	public class UserAttributes : DynamicObject {

		#region instance fields

		private readonly string _externalID;
		private string _firstName;
		private string _lastName;
		private string _email;
		private DateTime? _dateOfBirth;
		private string _country;
		private string _homeCity;
		private string _bio;
		private Gender? _gender;
		private string _phone;
		private bool? _isSubscribedToEmails;
		// TODO: support social data (Facebook, Twitter, Foursquare)
		private readonly IDictionary<string, object> _customAttributes;
		private readonly IList<PushTokenRegistration> _pushTokens;

		#endregion


		#region constructors

		public UserAttributes( string externalID ){
			Contract.Requires( !string.IsNullOrWhiteSpace( externalID ));
			if( string.IsNullOrWhiteSpace( externalID ))
				throw new ArgumentNullException( "externalID", "Argument is null or contains only whitespace" );

			_externalID = externalID;

			_customAttributes = new Dictionary<string, object>();
			_pushTokens = new List<PushTokenRegistration>();
		}

		#endregion


		#region properties

		[JsonProperty( "external_id" )]
		public string ExternalID {
			get { return _externalID; }
		}


		[JsonProperty( "first_name" )]
		public string FirstName {
			get { return _firstName; }
			set { _firstName = value; }
		}


		[JsonProperty( "last_name" )]
		public string LastName {
			get { return _lastName; }
			set { _lastName = value; }
		}


		[JsonProperty( "email" )]
		public string Email {
			get { return _email; }
			set { _email = value; }
		}


		[JsonProperty( "dob" )]
		public DateTime? DateOfBirth {
			get { return _dateOfBirth; }
			set { _dateOfBirth = value; }
		}


		[JsonProperty( "country" )]
		public string Country {
			get { return _country; }
			set { _country = value; }
		}


		[JsonProperty( "home_city" )]
		public string HomeCity {
			get { return _homeCity; }
			set { _homeCity = value; }
		}


		[JsonProperty( "bio" )]
		public string Bio {
			get { return _bio; }
			set { _bio = value; }
		}


		[JsonProperty( "gender" )]
		public Gender? Gender {
			get { return _gender; }
			set { _gender = value; }
		}


		[JsonProperty( "phone" )]
		public string Phone {
			get { return _phone; }
			set { _phone = value; }
		}


		[JsonProperty( "email_subscribe" )]
		public bool? IsSubscribedToEmails {
			get { return _isSubscribedToEmails; }
			set { _isSubscribedToEmails = value; }
		}


		[JsonProperty( "push_tokens" )]
		public IList<PushTokenRegistration> PushTokens {
			get { return _pushTokens; }
		}


		[JsonIgnore]
		public IDictionary<string, object> CustomAttributes {
			get { return _customAttributes; }
		}

		#endregion


		#region overrides

		public override IEnumerable<string> GetDynamicMemberNames() {
			return _customAttributes.Keys;
		}
		public override bool TrySetMember( SetMemberBinder binder, object value ){
			SetCustomAttributes( new Dictionary<string, object> {{ binder.Name, value }} );

			return true;
		}


		public override bool TryGetMember( GetMemberBinder binder, out object result ){
			result = (_customAttributes.ContainsKey( binder.Name )) ?
				_customAttributes[binder.Name]
				: null;

			return (result != null);
		}

		#endregion


		#region operations

		public void SetCustomStringAttribute( string attributeName, string attributeValue ){
			Contract.Requires( !string.IsNullOrWhiteSpace( attributeName ));
			if( string.IsNullOrWhiteSpace( attributeName ))
				throw new ArgumentNullException( "attributeName", "Argument is null or contains only whitespace" );
			// ensure value is not too long
			if( attributeValue.Length >= AppboyClient.MaxCustomStringAttributesLength )
				throw new ArgumentException( string.Format( "Attribute value cannot exceed {0} characters. Actual: {1}", AppboyClient.MaxCustomStringAttributesLength, attributeValue.Length ));

			_customAttributes.Add( attributeName, attributeValue );
		}


		public void SetCustomIntegerAttribute( string attributeName, int attributeValue ){
			Contract.Requires( !string.IsNullOrWhiteSpace( attributeName ));
			if( string.IsNullOrWhiteSpace( attributeName ))
				throw new ArgumentNullException( "attributeName", "Argument is null or contains only whitespace" );

			_customAttributes.Add( attributeName, attributeValue );
		}


		public void SetCustomFloatAttribute( string attributeName, float attributeValue ){
			Contract.Requires( !string.IsNullOrWhiteSpace( attributeName ));
			if( string.IsNullOrWhiteSpace( attributeName ))
				throw new ArgumentNullException( "attributeName", "Argument is null or contains only whitespace" );

			_customAttributes.Add( attributeName, attributeValue );
		}


		public void SetCustomBooleanAttribute( string attributeName, bool attributeValue ){
			Contract.Requires( !string.IsNullOrWhiteSpace( attributeName ));
			if( string.IsNullOrWhiteSpace( attributeName ))
				throw new ArgumentNullException( "attributeName", "Argument is null or contains only whitespace" );

			_customAttributes.Add( attributeName, attributeValue );
		}


		public void SetCustomDateTimeAttribute( string attributeName, DateTimeOffset? attributeValue ){
			Contract.Requires( !string.IsNullOrWhiteSpace( attributeName ));
			if( string.IsNullOrWhiteSpace( attributeName ))
				throw new ArgumentNullException( "attributeName", "Argument is null or contains only whitespace" );

			_customAttributes.Add( attributeName, attributeValue );
		}


		/// <summary>
		/// Sets all the attributes in the given dictionary.<br/>
		/// NOTE: This will not clear out any previously set attributes added by any of the single-attribute methods (ie: <see cref="SetCustomStringAttribute"/> etc.).
		/// </summary>
		/// <param name="customAttributes"></param>
		public void SetCustomAttributes( IDictionary<string, object> customAttributes ){
			Contract.Requires( customAttributes != null );
			if( customAttributes == null )
				throw new ArgumentNullException( "customAttributes" );

			// add all given attributes
			foreach( KeyValuePair<string, object> customAttribute in customAttributes )
				_customAttributes.Add( customAttribute );
		}


		public void AddPushToken( string appID, string token ){
			Contract.Requires( appID != null );
			Contract.Requires( token != null );
			if( appID == null )
				throw new ArgumentNullException( "appID" );
			if( token == null )
				throw new ArgumentNullException( "token" );

			// ensure appID does not have token already registered
			if( _pushTokens.Any( pushTokenRegistration => pushTokenRegistration.AppID == appID ))
				throw new InvalidOperationException( string.Format( "App ID {0} already has a push notification token registered. Only one token may be registered per user per app ID.", appID ));
			
			_pushTokens.Add( new PushTokenRegistration( appID, token ));
		}

		#endregion
		
	}
}
