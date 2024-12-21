using GGAM.Game;
using Timer = System.Timers.Timer;

namespace GGAM;

class Program {

	static GalaxyManager galaxyManager;

	static void Main(string[] args) {
		Console.WriteLine("Press Enter to exit. This will stop unlocking achievements.");

		string clientID = "50225266424144145";
		string clientSecret = "45955f1104f99b625a5733fa1848479b43d63bdb98f0929e37c9affaf900e99f";
		IList<string> achievementKeys = new string[] {
											"launchTheGame",
											"putBlack",
											"putBlue",
											"putBrown",
											"putGreen",
											"putPink",
											"putRed",
											"putYellow",
											"winSPRound",
											"win2PRound",
											"draw2PRound",
											"winMPRound",
											"drawMPRound",
											"loseMPRound",
											"test",
										};

		galaxyManager = new GalaxyManager(clientID, clientSecret, achievementKeys);
		galaxyManager.SignInGalaxy();
		galaxyManager.StartProcessDataTimer();

		// CheckUserSignInStatus(galaxyManager);

		Console.ReadLine();
	}

	// Check user sign in status every second,
	// and print status when changed.
	static void CheckUserSignInStatus(GalaxyManager galaxyManager) {
		bool isSignedIn = false;
		Timer galaxyUserStateCheckTimer = new Timer(1000);
		galaxyUserStateCheckTimer.Elapsed += (sender, e) => {
			bool isSignedInCurrently = galaxyManager.IsSignedIn(silent: true);
			if (isSignedInCurrently != isSignedIn) {
				Console.WriteLine($"User signed in: {isSignedInCurrently}");
			}
			isSignedIn = isSignedInCurrently;
		};
		galaxyUserStateCheckTimer.Start();
	}
}
