using System;
using System.Collections.Generic;
using System.Threading;

namespace Appboy.NET.Example {
	public class Program {

		#region class methods

		public static void Main() {
			const string companySecret = "20ahF1IS3kAN_plOcqsHqw";
			const string appGroupID = "NOPE61550b17-53ae-42ac-941c-c19c1d764dbd";
			var appboyClient = new AppboyClient( companySecret, appGroupID );

			// build up the list of users you want to add (NOTE: you would normally query this from your database / data sources, and convert your own user entities to AppBoy users)
			IEnumerable<UserAttributes> usersToAdd = GetUsersToAdd();
			var semaphore = new Semaphore( 0, 1 );
			appboyClient.BuildUpRequest()
				.AddUserAttributes( usersToAdd )
				.SendDataAsync( result => {
					Console.WriteLine( "SUCCESS. Num attributes processed: {0}", result.NumAttributesProcessed );

					// signal completion
					semaphore.Release();
				},
				exception => {
					Console.WriteLine( "ERROR: {0}", exception.Message );

					// signal completion
					semaphore.Release();
				});

			// wait for data upload to complete
			semaphore.WaitOne();
			Console.WriteLine( "All finished!" );
			Console.ReadKey();
		}

		#endregion

		#region private methods

		private static IEnumerable<UserAttributes> GetUsersToAdd() {
			// create dummy users
			var user1 = new UserAttributes( "EXTERNAL USER ID 1" ){
				Email = "user1@exmaple.com",
				FirstName = "Test User",
				LastName = "1",
				Gender = Gender.Female,
				IsSubscribedToEmails = true,
			};
			user1.SetCustomStringAttribute( "CustomAttribute1", "CUSTOM ATTRIBUTE 1 VALUE" );
			var user2 = new UserAttributes( "EXTERNAL USER ID 2" );
			var user3 = new UserAttributes( "EXTERNAL USER ID 3" );
			
			var usersToAdd = new List<UserAttributes> { user1, user2, user3 };

			return usersToAdd;
		}

		#endregion

	}
}
