using Galaxy.Api;

namespace GGAM.Game.Listeners;

public class AuthenticationListener : IAuthListener {

	public event EventHandler AuthSuccess;
	public event EventHandler<AuthFailureEventArgs> AuthFailure;
	public event EventHandler AuthLost;

	public override void OnAuthSuccess() {
		AuthSuccess?.Invoke(this, EventArgs.Empty);
	}

	public override void OnAuthFailure(FailureReason failureReason) {
		AuthFailure?.Invoke(this, new AuthFailureEventArgs(failureReason));
	}

	public override void OnAuthLost() {
		AuthLost?.Invoke(this, EventArgs.Empty);
	}

	public class AuthFailureEventArgs : EventArgs {
		public FailureReason FailureReason { get; }

		public AuthFailureEventArgs(FailureReason failureReason) {
			FailureReason = failureReason;
		}
	}
}
