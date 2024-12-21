using Galaxy.Api;
using GGAM.Game.Listeners;

namespace GGAM.Game;

public class StatsAndAchievementsManager {

	private UserStatsAndAchievementsRetrieveListener _userStatsAndAchievementsRetrieveListener;
	private AchievementChangeListener _achievementChangeListener;
	private StatsAndAchievementsStoreListener _statsAndAchievementsStoreListenerListener;
	private GalaxyID _galaxyUserId;
	private int _achievementUnlockedCount = 0;

	// List of all in-game achievements api keys
	public IList<string> AchievementKeys { get; }

	private IStats Stats => GalaxyInstance.Stats();

	public StatsAndAchievementsManager(GalaxyID galaxyUserId, IList<string> achievementKeys) {
		_galaxyUserId = galaxyUserId;
		AchievementKeys = achievementKeys;
		InitListeners();
		RequestUserStatsAndAchievements();
	}

	~StatsAndAchievementsManager() {
		DisposeListeners();
	}

	private void InitListeners() {
		_userStatsAndAchievementsRetrieveListener = new();
		_userStatsAndAchievementsRetrieveListener.UserStatsAndAchievementsRetrieveSuccess += this._userStatsAndAchievementsRetrieveListener_UserStatsAndAchievementsRetrieveSuccess;
		_userStatsAndAchievementsRetrieveListener.UserStatsAndAchievementsRetrieveFailure += this._userStatsAndAchievementsRetrieveListener_UserStatsAndAchievementsRetrieveFailure;
		_achievementChangeListener = new();
		_achievementChangeListener.AchievementUnlocked += this._achievementChangeListener_AchievementUnlocked;
		_statsAndAchievementsStoreListenerListener = new();
		_statsAndAchievementsStoreListenerListener.UserStatsAndAchievementsStoreSuccess += this._statsAndAchievementsStoreListenerListener_UserStatsAndAchievementsStoreSuccess;
		_statsAndAchievementsStoreListenerListener.UserStatsAndAchievementsStoreFailure += this._statsAndAchievementsStoreListenerListener_UserStatsAndAchievementsStoreFailure;
	}

	private void _statsAndAchievementsStoreListenerListener_UserStatsAndAchievementsStoreFailure(object? sender, StatsAndAchievementsStoreListener.UserStatsAndAchievementsStoreFailureEventArgs e) {
		Console.Error.WriteLine("User Stats And Achievements Store Failure: " + e.FailureReason);
	}

	private void _userStatsAndAchievementsRetrieveListener_UserStatsAndAchievementsRetrieveSuccess(object? sender, UserStatsAndAchievementsRetrieveListener.UserStatsAndAchievementsRetrieveSuccessEventArgs e) {
		Console.WriteLine($"User {e.UserID} stats and achievements retrieved");

		//ResetStatsAndAchievements();
		foreach (string ach in AchievementKeys) SetAchievement(ach);
	}

	private void _userStatsAndAchievementsRetrieveListener_UserStatsAndAchievementsRetrieveFailure(object? sender, UserStatsAndAchievementsRetrieveListener.UserStatsAndAchievementsRetrieveFailureEventArgs e) {
		Console.Error.WriteLine("User " + e.UserID + " stats and achievements could not be retrieved, for reason " + e.FailureReason);
	}

	private void _achievementChangeListener_AchievementUnlocked(object? sender, AchievementChangeListener.AchievementUnlockedEventArgs e) {
		_achievementUnlockedCount += 1;
		Console.WriteLine($"Achievement \"{e.Key}\" unlocked. Unlocked achievement count: {_achievementUnlockedCount}");
		if (_achievementUnlockedCount == AchievementKeys.Count) {
			Console.WriteLine("All achievements unlocked. Press Enter to exit.");
		}
	}

	private void _statsAndAchievementsStoreListenerListener_UserStatsAndAchievementsStoreSuccess(object? sender, EventArgs e) {
		Console.WriteLine("UserStatsAndAchievements Store Success");
	}

	private void DisposeListeners() {
		_userStatsAndAchievementsRetrieveListener = null;
		_achievementChangeListener = null;
		_statsAndAchievementsStoreListenerListener = null;
	}

	/* Coroutine for retriving stats and achievements 
    Note: This request has to finish successfully before using any achievements or statistics related methods */
	public void RequestUserStatsAndAchievements() {
		Console.WriteLine("Requesting Stats and Achievements");
		try {
			Stats.RequestUserStatsAndAchievements(_galaxyUserId, _userStatsAndAchievementsRetrieveListener);
			//Stats.RequestUserStatsAndAchievements();
		} catch (GalaxyInstance.Error e) {
			Console.Error.WriteLine("Achievements definitions could not be retrived for reason: " + e);
		}
	}

	// 	Unlocks achievement specified by API Key
	public void SetAchievement(string achievementKey) {
		Console.WriteLine("Trying to unlock achievement " + achievementKey);
		try {
			Stats.SetAchievement(achievementKey);
			Stats.StoreStatsAndAchievements(_statsAndAchievementsStoreListenerListener);
			//Stats.StoreStatsAndAchievements();
		} catch (GalaxyInstance.Error e) {
			Console.Error.WriteLine("Achievement " + achievementKey + " could not be unlocked for reason: " + e);
		}
	}

	// 	Gets status of achievement specified by API key
	public bool GetAchievement(string achievementKey) {
		Console.WriteLine("Trying to get achievement status for " + achievementKey);
		bool unlocked = false;
		try {
			uint unlockTime = 0;
			Stats.GetAchievement(achievementKey, ref unlocked, ref unlockTime);
			Console.WriteLine("Achievement: \"" + achievementKey + "\" unlocked: " + unlocked + " unlock time: " + unlockTime);
		} catch (GalaxyInstance.Error e) {
			Console.Error.WriteLine("Could not get status of achievement " + achievementKey + " for reason: " + e);
		}
		return unlocked;
	}

	// 	Gets name of achievement specified by API key
	public string GetAchievementName(string achievementKey) {
		Console.WriteLine("Trying to get achievement name " + achievementKey);
		string name = "";
		try {
			name = Stats.GetAchievementDisplayName(achievementKey);
			Console.WriteLine("Achievement display name: " + name);
		} catch (GalaxyInstance.Error e) {
			Console.Error.WriteLine("Could not get name of achievement " + achievementKey + " for reason: " + e);
		}
		return name;
	}

	// Resets stats and achievements
	public void ResetStatsAndAchievements() {
		Console.WriteLine("Trying to reset user stats and achievements");
		try {
			Stats.ResetStatsAndAchievements(_statsAndAchievementsStoreListenerListener);
			//Stats.ResetStatsAndAchievements();
			Console.WriteLine("User stats and achievements reset");
		} catch (GalaxyInstance.Error e) {
			Console.Error.WriteLine("Could not get reset user stats and achievements for reason: " + e);
		}
	}
}
