using Galaxy.Api;

namespace GGAM.Game.Listeners;

/* Informs about the event of unlocking an achievement
    Callback for methods:
    SetAchievement(string key) */
public class AchievementChangeListener : GlobalAchievementChangeListener {

	public event EventHandler<AchievementUnlockedEventArgs> AchievementUnlocked;

	public override void OnAchievementUnlocked(string key) {
		AchievementUnlocked?.Invoke(this, new AchievementUnlockedEventArgs(key));
	}

	public class AchievementUnlockedEventArgs : EventArgs {
		public string Key { get; }

		public AchievementUnlockedEventArgs(string key) {
			Key = key;
		}
	}
}
