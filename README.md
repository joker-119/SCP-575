#SCP-575
======
made by Joker119
## Description
SCP-575 is a Keter-class sentient being, capable of unknown levels of destruction and chaos, but instances of SCP-575 can only exist in near-complete darkness.
With the current containment breach in the facility, SCP-079 has deactivated several main generators in the facility, causing temporary power outages in Heavy and Light containment zones.
Any player caught in the dark without a flashlight will become prey for SCP-575, slowly losing health until they either die, leave the affected area, pull out a flashlight, or the blackout ends.

### Features
 - Timed and toggleable SCP-575 blackout events.
 - Configurable keter-class interactions with non-SCP players in blacked out rooms.
 - Configurable tesla deactivation during events (both timed and toggled).
 - Configurable settings to allow timed and toggled eents to affect LCZ seperately.
 - Configurable timers for time until the first event, time inbetween events, and duration of blackout events.
 - Configurable CASSIE announcements for SCP-575 blackouts.

### Config Settings
Config option | Config Type | Default Value | Description
:---: | :---: | :---: | :------
575_ranks | String | owner, admin | The server-roles able to run SCP-575 commands
575_timed | Bool | true | If timed SCP-575 blackouts should occur or not.
575_delay | Float | 300f | The number of seconds before the first blackout occurs every round.
575_duration | Float | 90f | The duration of each blackout event.
575_wait | Float | 180f | The amount of time between each subsequent blackout.
575_announce | Bool | True | If CASSIE should make announcements related to blackouts.
575_toggle_lcz | Bool | True | If manually toggled blackouts should affect LCZ.
575_timed_lcz | Bool | True | If timed blackouts should affect LCZ.
575_toggle_tesla | Bool | True | If tesla gates should be deactivated during toggled events.
575_timed_tesla | Bool | True | If tesla gates should be deactivated during timed events.
575_keter | Bool | True | If the SCP-575 keter-class effects should be used.
575_keter_damage | Int | 10 | The amount of damage (every 5 seconds) players affected by SCP-575's keter effect will take.

### Commands
  Command |  |  | Description
:---: | :---: | :---: | :------
**Aliases** | **scp575** | **575**
575 toggle | ~~ | ~~ | Toggles manual events on and off.
575 enable | ~~ | ~~ | Enabled timed events.
575 disable | ~~ | ~~ | Disables timed events.
575 anon | ~~ | ~~ | Turns on CASSIE announcements for blackouts.
575 anoff | ~~ | ~~ | Turns off CASSIE announcements for blackouts.
