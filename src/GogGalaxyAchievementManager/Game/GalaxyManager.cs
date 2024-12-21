using Galaxy.Api;
using GGAM.Game.Listeners;
using Timer = System.Timers.Timer;

namespace GGAM.Game;

public class GalaxyManager {

	// Project specific data (ClientID and ClientSecret can be obtained from devportal.gog.com's game page)
	// ClientID and ClientSecret are merged into initParams for later use with Init(InitParams initParams) method
	private string _clientID;
	private string _clientSecret;
	private IList<string> _achievementKeys;

	public AuthenticationListener _authenticationListener;
	public GogServicesConnectionStateListener _gogServicesConnectionStateListener;
	public GalaxyID GalaxyUserId { get; set; }
	public bool GalaxyFullyInitialized { get; set; }
	public StatsAndAchievementsManager StatsAndAchievementsManager { get; private set; }

	public GalaxyManager(string clientID, string clientSecret, IList<string> achievementKeys) {
		_clientID = clientID;
		_clientSecret = clientSecret;
		_achievementKeys = achievementKeys;
		InitGalaxyInstance();
		InitListeners();
	}

	~GalaxyManager() {
		ShutdownAllFeatureClasses();
		DisposeListeners();
		/* Shuts down the working instance of GalaxyPeer. 
        NOTE: Shutdown should be the last method called, and all listeners should be closed before that. */
		GalaxyInstance.Shutdown(true);
	}

	public void StartProcessDataTimer() {
		Timer t = new Timer(1000);
		t.Elapsed += (sender, e) => GalaxyInstance.ProcessData();
		t.Start();
	}

	private void InitListeners() {
		_authenticationListener = new();
		_authenticationListener.AuthSuccess += this._authenticationListener_AuthSuccess;
		_authenticationListener.AuthFailure += this._authenticationListener_AuthFailure;
		_authenticationListener.AuthLost += this._authenticationListener_AuthLost;
		_gogServicesConnectionStateListener = new();
		_gogServicesConnectionStateListener.ConnectionStateChanged += this._gogServicesConnectionStateListener_ConnectionStateChanged;
	}

	private void _gogServicesConnectionStateListener_ConnectionStateChanged(object? sender, GogServicesConnectionStateListener.GogServicesConnectionStateChangedEventArgs e) {
		Console.WriteLine("Connection state to GOG services changed to " + e.ConnectionState);
		if (e.ConnectionState == GogServicesConnectionState.GOG_SERVICES_CONNECTION_STATE_CONNECTED) {
		}
	}

	private void _authenticationListener_AuthLost(object? sender, EventArgs e) {
		Console.Error.WriteLine("Authorization lost");
	}

	private void _authenticationListener_AuthFailure(object? sender, AuthenticationListener.AuthFailureEventArgs e) {
		Console.Error.WriteLine("Failed to sign in for reason " + e.FailureReason);
	}

	private void _authenticationListener_AuthSuccess(object? sender, EventArgs e) {
		GalaxyUserId = GalaxyInstance.User().GetGalaxyID();
		Console.WriteLine("AuthenticationListener: Successfully signed in as user: " + GalaxyUserId);
		StatsAndAchievementsManager = new StatsAndAchievementsManager(GalaxyUserId, _achievementKeys);
	}

	private void DisposeListeners() {
		_authenticationListener = null;
		_gogServicesConnectionStateListener = null;
	}

	/* Following methods are used to start and shutdown each and every GalaxyManager feature class separately.
    Note: Each class closes its own listeners whyn disabled */
	public void ShutdownStatsAndAchievements() {
		StatsAndAchievementsManager = null;
	}

	public void ShutdownAllFeatureClasses() {
		ShutdownStatsAndAchievements();
	}

	/* Initializes GalaxyPeer instance
    NOTE: Even if Init will throw exepction Apps interface will still be available. 
    If you want to use Apps interface in your game make sure that shutdown is NOT called when Init throws an exception.*/
	private void InitGalaxyInstance() {
		InitParams initParams = new InitParams(_clientID, _clientSecret);
		Console.WriteLine("Initializing GalaxyPeer instance...");
		try {
			GalaxyInstance.Init(initParams);
			GalaxyFullyInitialized = true;
		} catch (GalaxyInstance.Error e) {
			Console.Error.WriteLine("Init failed for reason " + e);
			GalaxyFullyInitialized = false;
		}
	}

	/* Signs the current user in to Galaxy services
    NOTE: This call is asynchronus. Sign in result is received by AuthListener. */
	public void SignInGalaxy() {
		Console.WriteLine("Signing user in using Galaxy client...");
		try {
			//GalaxyInstance.User().SignInGalaxy();
			GalaxyInstance.User().SignInGalaxy(false, _authenticationListener);
		} catch (GalaxyInstance.Error e) {
			Console.Error.WriteLine("SignInGalaxy failed for reason " + e);
		}
	}

	public void SignInCredentials(string username, string password) {
		Console.WriteLine("Signing user in using credentials...");
		try {
			GalaxyInstance.User().SignInCredentials(username, password);
		} catch (GalaxyInstance.Error e) {
			Console.Error.WriteLine("SignInCredentials failed for reason " + e);
		}
	}

	/* Signs the current user out from Galaxy services */
	public void SignOut() {
		Console.WriteLine("Singing user out...");
		try {
			GalaxyInstance.User().SignOut();
		} catch (GalaxyInstance.Error e) {
			Console.Error.WriteLine("SignOut failed for reason " + e);
		}
	}

	/* Checks current user singed in status
    NOTE: Signed in means that the user is signed in to GOG Galaxy client (he does not have to have working internet connection). */
	public bool IsSignedIn(bool silent = false) {
		bool signedIn = false;
		if (!silent) Console.WriteLine("Checking SignedIn status...");
		try {
			signedIn = GalaxyInstance.User().SignedIn();
		} catch (GalaxyInstance.Error e) {
			if (!silent) Console.Error.WriteLine("Could not check user signed in status for reason " + e);
		}
		return signedIn;
	}

	/* Checks if user is logged on
    NOTE: Logged on means that the user is signed in to GOG Galaxy client and he does have working internet connection */
	public bool IsLoggedOn(bool silent = false) {
		bool isLoggedOn = false;
		if (!silent) Console.WriteLine("Checking LoggedOn status...");
		try {
			isLoggedOn = GalaxyInstance.User().IsLoggedOn();
		} catch (GalaxyInstance.Error e) {
			if (!silent) Console.Error.WriteLine("Could not check user logged on status for reason " + e);
		}
		return isLoggedOn;
	}

	// Checks if DLC specified by productID is installed on users machine
	public bool IsDlcInstalled(ulong productID) {
		bool isDlcInstalled = false;
		Console.WriteLine("Checking is DLC " + productID + " installed");
		try {
			isDlcInstalled = GalaxyInstance.Apps().IsDlcInstalled(productID);
			Console.WriteLine("DLC " + productID + " installed " + isDlcInstalled);
		} catch (GalaxyInstance.Error e) {
			Console.Error.WriteLine("Could not check is DLC " + productID + " installed for reason " + e);
		}
		return isDlcInstalled;
	}

	public string GetCurrentGameLanguage() {
		string gameLanguage = null;
		Console.WriteLine("Checking current game language");
		try {
			gameLanguage = GalaxyInstance.Apps().GetCurrentGameLanguage();
			Console.WriteLine("Current game language is " + gameLanguage);
		} catch (GalaxyInstance.Error e) {
			Console.WriteLine("Could not check current game language for reason " + e);
		}
		return gameLanguage;
	}

	public void ShowOverlayWithWebPage(string url) {
		Console.WriteLine("Opening overlay with web page " + url);
		try {
			GalaxyInstance.Utils().ShowOverlayWithWebPage(url);
			Console.WriteLine("Opened overlay with web page " + url);
		} catch (GalaxyInstance.Error e) {
			Console.WriteLine("Could not open overlay with web page " + url + " for reason " + e);
		}
	}
}
