using Galaxy.Api;

namespace GGAM.Game.Listeners;

/* Informs about the event of storing changes made to the achievement or statiscis of a user
    Callback for method: 
    GalaxyInstance.Stats().StoreStatsAndAchievements(); */
public class StatsAndAchievementsStoreListener : IStatsAndAchievementsStoreListener {

	public event EventHandler<UserStatsAndAchievementsStoreFailureEventArgs> UserStatsAndAchievementsStoreFailure;
	public event EventHandler UserStatsAndAchievementsStoreSuccess;

	public override void OnUserStatsAndAchievementsStoreFailure(FailureReason failureReason) {
		UserStatsAndAchievementsStoreFailure?.Invoke(this, new UserStatsAndAchievementsStoreFailureEventArgs(failureReason));
	}

	public override void OnUserStatsAndAchievementsStoreSuccess() {
		UserStatsAndAchievementsStoreSuccess?.Invoke(this, EventArgs.Empty);
	}

	public class UserStatsAndAchievementsStoreFailureEventArgs : EventArgs {
		public FailureReason FailureReason { get; }

		public UserStatsAndAchievementsStoreFailureEventArgs(FailureReason failureReason) {
			FailureReason = failureReason;
		}
	}
}
