using Galaxy.Api;

namespace GGAM.Game.Listeners;

/* Informs about the event of receiving user stats and achievements definitions
	Callbacks for method: 
	RequestUserStatsAndAchievements() */
public class UserStatsAndAchievementsRetrieveListener : IUserStatsAndAchievementsRetrieveListener {

	public event EventHandler<UserStatsAndAchievementsRetrieveSuccessEventArgs> UserStatsAndAchievementsRetrieveSuccess;
	public event EventHandler<UserStatsAndAchievementsRetrieveFailureEventArgs> UserStatsAndAchievementsRetrieveFailure;

	public override void OnUserStatsAndAchievementsRetrieveSuccess(GalaxyID userID) {
		UserStatsAndAchievementsRetrieveSuccess?.Invoke(this, new UserStatsAndAchievementsRetrieveSuccessEventArgs(userID));
	}

	public override void OnUserStatsAndAchievementsRetrieveFailure(GalaxyID userID, FailureReason failureReason) {
		UserStatsAndAchievementsRetrieveFailure?.Invoke(this, new UserStatsAndAchievementsRetrieveFailureEventArgs(userID, failureReason));
	}

	public class UserStatsAndAchievementsRetrieveSuccessEventArgs : EventArgs {
		public GalaxyID UserID { get; }

		public UserStatsAndAchievementsRetrieveSuccessEventArgs(GalaxyID userID) {
			UserID = userID;
		}
	}

	public class UserStatsAndAchievementsRetrieveFailureEventArgs : EventArgs {
		public GalaxyID UserID { get; }
		public FailureReason FailureReason { get; }

		public UserStatsAndAchievementsRetrieveFailureEventArgs(GalaxyID userID, FailureReason failureReason) {
			UserID = userID;
			FailureReason = failureReason;
		}
	}
}
