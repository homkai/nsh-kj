using System;

namespace nshkj
{
	public class KeyboardHookTriggeredEventArgs : EventArgs {
		public int ID {
			get;
			private set;
		}

		public KeyboardHookTriggeredEventArgs(int id) {
			ID = id;
		}
	}
}
