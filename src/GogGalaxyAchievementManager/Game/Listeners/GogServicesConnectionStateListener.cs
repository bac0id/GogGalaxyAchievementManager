using Galaxy.Api;

namespace GGAM.Game.Listeners;

// What's the purpose of GogServicesConnectionStateListener?
public class GogServicesConnectionStateListener : GlobalGogServicesConnectionStateListener {

	public event EventHandler<GogServicesConnectionStateChangedEventArgs> ConnectionStateChanged;

	public override void OnConnectionStateChange(GogServicesConnectionState connected) {
		ConnectionStateChanged?.Invoke(this, new GogServicesConnectionStateChangedEventArgs(connected));
	}

	public class GogServicesConnectionStateChangedEventArgs : EventArgs {
		public GogServicesConnectionState ConnectionState { get; }

		public GogServicesConnectionStateChangedEventArgs(GogServicesConnectionState connectionState) {
			ConnectionState = connectionState;
		}
	}
}
