# Network Controller

`NetworkController` is a Singleton, use `NetworkController.GetInstance()` to obtain a Reference.

To make a Server Request:

1. Define a Callback Method with the Name of a Server Command and one Parameter of Type `string[]`
1. Register your Callback with `NetworkController.RegisterListener(NetworkMethod networkMethod)`
1. Call `NetworkController.SendRequest(NetworkMethod callback, params KeyValuePair<string, string>[] parameters)`
1. Your Callback Method will then be called as soon as the Server Reply arrives
1. Your Callback Method should parse the `string[]` Parameter provided by the Server

Some Server Commands require the User to be logged in. Login is performed automatically by `NetworkController`.
You can check if the User is already logged in by calling `NetworkController.IsLoggedIn()`.

# Server Commands

- Command: Register
- Description: Registers a new User in the Database.
- Requires Login: false
- Parameters:
	- Username: Name of the new User. Must be unique.
	- Password: Password for the new User.
	- RepeatPassword: Repeated Password from another TextField to make sure, that the User typed in his desired Password correctly.
- Returns:
	- 0: "Successful"
	
	OR
	
	- 0-3: Error Messages

---

- Command: Login
- Description: Checks Username and Password against the Database and logs the User in if successful.
- Requires Login: false
- Parameters:
	- Username: Name of the current User.
	- Password: Password of the current User.
- Returns:
	- 0: "Successful"
	 
	OR
	
	- 0-2: Error Messages

---

- Command: Logout
- Description: Terminates the current Session if there is any.
- Requires Login: false
- Parameters:
	- none
- Returns:
	- 0: "Successful"

---

- Command: Save
- Description: Uploads a Savegame to the Database.
- Requires Login: true
- Parameters:
	- Timestamp: Current Time in a Format which should be independent of Timezones and Restarts, and should return the newest Timestamp first when ordered descending.
	- Save: The Savegame to be uploaded as JSON String.
- Returns:
	- 0: "Successful"

---

- Command: Load
- Description: Download the newest Savegame from the Database.
- Requires Login: true
- Parameters:
	- none
- Returns:
	- 0: "Successful"
	
	OR
	
	- 0-1: Error Messages

# Example Calls
	SendRequest(Register, new KeyValuePair<string, string>("Username", username),
		new KeyValuePair<string, string>("Password", password),
		new KeyValuePair<string, string>("RepeatPassword", password));

---

	SendRequest(Login, new KeyValuePair<string, string>("Username", username),
		new KeyValuePair<string, string>("Password", password));

---

	SendRequest(Logout);

---

	SendRequest(Save, new KeyValuePair<string, string>("Timestamp", DateTime.UtcNow.ToString("yyyy/MM/dd/HH/mm/ss", CultureInfo.InvariantCulture)),
		new KeyValuePair<string, string>("Save", "Definitely a JSON"));

---

	SendRequest(Load);
