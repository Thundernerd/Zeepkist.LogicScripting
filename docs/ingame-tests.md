# In-game tests

`tools/ingame-test.sh` runs Scenarios against the real game: it builds this branch, installs exactly that build into Zeepkist, starts the game through Steam, and lets a test plugin (`Zeepkist.LogicScripting.InGameTests`) walk from the start screen into the level editor and run each Scenario's steps there. It records every Run Step's result and screenshots, quits the game, and puts the install back as it was.

It runs on Linux, with Zeepkist installed through Steam and running under Proton.

## Running it

```sh
tools/ingame-test.sh                     # the smoke Scenario
tools/ingame-test.sh my.zlstest other.zlstest
```

Steam has to be running and Zeepkist closed. A run takes over the game for a minute or two, plus about half a minute for each Scenario after the first: don't touch it meanwhile, and raise `--timeout` for a long list.

Scenarios run in the order given, each on a new, empty level in the editor: the first on the one the walk from the start screen opens, every later one on a new level started from the main menu, the way a player starts one. A failing step stops its own Scenario, with a screenshot, and the run goes on with the next. If the game can't get to a new level, the run stops there.

The script prints a summary and exits 0 only when every step of every Scenario passed. Everything the run found is in `.ingame-test/<run id>/`:

- `results.md`: each Scenario's steps, with what a failing check expected and found, the exceptions the game logged meanwhile, and the exceptions it logged as it quit
- `results.json`: the same, for tools, without the exceptions at quit
- the screenshots: `editor.png` once the walk reaches the editor, and `<scenario>-<name>.png` for a Scenario's own, including `<scenario>-failure.png` where one failed
- `LogOutput.log`: the game's BepInEx log, where the test plugin's lines start with `[ZLS-TEST]`. The script removes the game's log before it starts the game, so a run whose game wrote none has none, and its `results.md` says so.

Only one run can use the game at a time. A second run, say from another worktree, waits until the first is done.

### Options

| Option | What it does |
| --- | --- |
| `--no-build` | Uses the Release build that's already there instead of building. |
| `--keep` | Leaves this build installed afterwards, for playing it by hand. The other copies of the mod stay set aside until the next run puts them back. The test plugin is always removed. |
| `--timeout <seconds>` | Stops the game and fails the run after this long. The default is 300. |
| `-h`, `--help` | Prints the usage. |

`ZEEPKIST_DIR` sets the game's install folder, when it isn't found in Steam's libraries. `STEAM` sets the command that starts the game, `steam` by default: `STEAM=true` tries a run whose game never starts.

When the game hasn't started a minute after the launch, the run stops there and fails, with the reason Steam's log gives, such as `failed with AppError_19` when Steam has an update for the game to install first. Start the game from Steam once to let it update, and run again.

### Exceptions

The test plugin records every exception the game logs while the run is going, mod or not, with the first line of its stack trace. `results.md` lists each Scenario's under it, and those logged on the way to the editor at the top, counted where the same one comes from the same place. They don't fail a step: end a Scenario with `expect-no-exceptions` to check that it logged none.

Once the game has quit, the script reads what it logged after the run finished from `LogOutput.log` into an **Exceptions at quit** section, grouped by the exception and its first stack line, with a count. These are reported, but don't fail the run. When the run didn't finish, or the log stops before the run's end, the section says so instead, and when the log stops partway through a line it says some may be missing.

## Scenarios

A Scenario is a `.zlstest` file, named after the file. It has one step per line. Blank lines and lines starting with `#` are skipped. `set-text` takes the lines below it, up to a line that is just `end`, so indent a Function's own `end` inside it. The indentation they share is taken off.

```
# A Script drives a block in test mode, and is still there back in the editor
open-window
new-script Lights
set-text
power_display(1 & 1)
end
compile
expect-result Placed 3 blocks
expect-blocks 3
place-start
test-mode
wait 2
screenshot running
back-to-editor
expect-blocks 3
open-window
select-script Lights
expect-text power_display(1 & 1)
```

A Scenario that can't be read fails before the game starts it, with the line that's wrong.

The test plugin's own Scenarios are in `Zeepkist.LogicScripting.InGameTests/Scenarios/`.

### Steps

| Step | What it does |
| --- | --- |
| `open-window` | Opens the Script window. |
| `close-window` | Closes it. |
| `new-script <name>` | Makes a new Script and selects it, as New Script does. Fails when the name is taken. |
| `select-script <name>` | Selects a Script, as clicking it in the list does: the window selects it as it draws the list, after it worked out that frame's layout. Fails when there's none by that name, or when the Scripts side is collapsed, so the list isn't drawn. |
| `new-library <name>` | Makes a new Library and selects it, as New Library on the Libraries tab does. Fails when the name is taken. |
| `select-library <name>` | Selects a Library, as clicking it in the Libraries tab's list does, the same way. Fails when there's none by that name. |
| `show-tab scripts\|libraries` | Shows the Scripts side's Scripts or Libraries tab, as clicking it does. The steps for a list and its footer work on the shown tab. |
| `expect-tab scripts\|libraries` | Which tab the Scripts side shows. |
| `open-new` | Opens New's name prompt on the shown tab, as clicking New Script or New Library does. |
| `open-rename <name>` | Opens the rename prompt for a Script or Library on the shown tab, as Rename in its context menu does. Fails when there's none by that name. |
| `open-delete <name>` | Opens the delete prompt for a Script or Library on the shown tab, as Delete in its context menu does, leaving it open. Fails when there's none by that name. |
| `type-name <text>` | Replaces the text in the open name prompt, as typing it does. Fails when no name prompt is open. |
| `press-prompt <button>` | Clicks a button of the open prompt: `ok`, `cancel`, `delete` (Delete blocks too, for a Script with blocks) or `keep-blocks`. Fails when the open prompt has no such button. |
| `select-group <name>` | Selects a Script's blocks, as Select group in its context menu on the Scripts tab does, showing that tab. Fails when there's no Script by that name. |
| `expect-list-result <text>` | The shown tab's result line, in its footer, contains the text, matching case. |
| `expect-prompt none\|new\|rename\|delete` | Which prompt the shown tab's footer has open. |
| `expect-footer-in-view` | The open prompt, the result line and New all lie inside the shown tab, and the tab inside the window, with the list at least two rows tall. The prompts may scroll when there isn't room for all of them, but get at least a row. |
| `set-window-height <px>` | Sets the window's height, as dragging its edge does, keeping its top edge. The window stays no shorter than its minimum, so a small enough height gives the smallest window, and no taller than the screen. It sets the height before the window draws, without Imui's resize handle: `drag-window-edge` drags that. |
| `expect-window-height <px>` | The window is drawn this tall, to within half a pixel. Its result says how tall it is. |
| `set-window-size <width> <height>` | Sets the window's width and height, as dragging its corner does, keeping its top left corner. The window stays no smaller than its minimum, so a small enough width or height gives the narrowest or shortest window, and no bigger than the screen. A bigger window can run off the screen's right or bottom edge: `center-window` brings it back. Like `set-window-height`, it sets the size before the window draws. |
| `center-window` | Moves the window to the middle of the screen, keeping its size. |
| `expect-window-size <width> <height>` | The window is drawn this big, to within half a pixel. Its result says where it is and how big. |
| `expect-window-width <px>\|screen` | The window is drawn this wide, or with `screen` as wide as the screen, to within half a pixel. Its result says how wide it and the screen are. |
| `expect-window-centered` | The window's middle is the screen's, to within half a pixel, as `center-window` and the window's first open leave it. Its result says where the window is, how big, and how big the screen is. |
| `drag-window-edge <px> [over <frames>] [then <px> [over <frames>]]...` | Drags the window's resize handle, in its bottom right corner, with the mouse the way a player does: the mouse moves onto the handle, presses, moves `<px>` down, or up for a number below 0, in even steps over the frames (1 when left out), and lets go. With `then`, the mouse makes each move after it in turn before letting go, such as `drag-window-edge -1000 over 10 then 200 over 2` to drag past the minimum and back in one press. It checks every frame from the press to the last move as `expect-editor-in-view` does, and that the window is no shorter or narrower than its minimum, failing with the first frame that's wrong: frame 0 is the press. Its result says how tall the window was before, after and at the shortest. The mouse is let go also when the step fails. See [Mouse](#mouse). |
| `drag-window-edge-sideways <px> [over <frames>] [then <px> [over <frames>]]...` | The same, moving the mouse `<px>` right, or left for a number below 0. Its result says how wide the window was before, after and at the narrowest. |
| `remember-window-rect` | Remembers where the window is drawn and how big it is, for `expect-window-rect`, for the rest of the Scenario. |
| `expect-window-rect` | The window is drawn at the size and position `remember-window-rect` remembered, to within half a pixel, both in its result. |
| `expect-window-start-rect` | The window is drawn at the size and position it was first drawn at in the run, which the run puts back before each Scenario after the first, to within half a pixel, both in its result. Fails when no Scenario has drawn the window yet. |
| `library-read-failed on\|off` | Makes the Libraries tab show (`on`) or stop showing (`off`) its warning that libraries.json couldn't be read, as when the file fails to load at startup. The step doesn't touch the file, and Libraries still save, so later steps and Scenarios find them working as usual. |
| `startup-warning on\|off` | Makes the center show (`on`) or stop showing (`off`) the startup check's warnings, as on a game version the mod doesn't fully support: that Compile places nothing, and a line of blocks turned off for it. Only the window changes: Compile and the checks work as before, so later steps and Scenarios find them working as usual. |
| `expect-library-read-failed on\|off` | Whether the Libraries tab draws its warning that libraries.json couldn't be read, as `library-read-failed` makes it or a file that failed to load at startup does. Fails when the tab isn't drawn, with the Scripts tab shown or the Scripts side collapsed: add `show-tab libraries` first. |
| `expect-startup-warning on\|off` | Whether the center draws the startup check's warnings, as `startup-warning` makes it or a game version the mod doesn't fully support does. |
| `set-text` | Replaces the selected Script's or Library's text with the lines up to `end`, as one edit. `{uid:<name>}` in them is written as the UID of the block `place-block ... as <name>` placed or `name-group-block` named, so a Script can write `placed("{uid:lamp}")`. Fails for a name no block was placed or named as. |
| `compile` | Compiles the selected Script, as Ctrl+Enter does. It passes whatever the Compile did: check that with the steps below. |
| `expect-editor-in-view` | The editor of the selected Script or Library is at least three rows tall, and it and the error panel's header lie inside the center, and the center inside the window. It waits a few frames first, for the window to settle. |
| `expect-editor-in-view-each-frame <frames>` | The same, without waiting: in the frame the step before ended on, frame 0, and in each of the next frames the window draws, failing with the first frame that's wrong. Its result says which frames drew the center's storage warning, so a Scenario can check that the frame it first showed was among them. |
| `expect-text <text>` | The selected Script's or Library's text contains the text, matching case. |
| `expect-result <text>` | The result line contains the text, matching case. |
| `expect-no-errors` | The window's check finds no errors in the text. |
| `expect-errors <count>` | The check finds this many errors. |
| `expect-error <line>:<column> <text>` | The check finds an error at that one-based line and column whose message contains the text, matching case, such as `expect-error 2:15 'nope' isn't defined`. |
| `expect-blocks <count>` | The level has this many Logic Blocks, placed by a Script or not. Other blocks, such as a start block, don't count. |
| `caret-at-start` | Puts the caret before the selected Script's or Library's first character and focuses the field, as clicking there does. |
| `caret-at-end` | Puts the caret after its last character, the same way. The call help shows for the call the caret is then in. |
| `complete` | Opens the completion list at the caret, as Ctrl+Space does, and waits for the editor to read its rows, so the steps after it don't check the rows a list already open had before. Fails when the editor isn't drawn in time. |
| `expect-completion <label> [<detail>]` | The open completion list has a row with the label, matching case, and with exactly the detail when one is given, such as `t logic` or `a input`. |
| `expect-completion-no-detail <label>` | The open completion list has a row with the label, matching case, and no detail, such as the `function` keyword. `expect-completion` can't say that, as a line's trailing spaces are taken off. |
| `expect-completion-missing <label>` | The open completion list has no row with the label, matching case, such as `a:` inside `logic(a, `, whose `a` is already given. Fails when the list is closed, as `expect-completion` does. Its result lists the rows the list has. |
| `expect-call-help <text>` | The call help shows, and its call, written as `logic(a, b, op_in, op:, hide:)`, contains the text, matching case. |
| `expect-call-help-active <label>` | The call help shows, and the input or Setting it highlights has the label, written as the call help writes it, such as `b`, `op_in` or `op:`. |
| `expect-call-help-no-active` | The call help shows, and highlights none of its inputs and Settings, such as for an argument past the last input, or after a named argument the call doesn't have. |
| `expect-no-completion` | The completion list is closed. |
| `expect-no-call-help` | The call help doesn't show. |
| `hover <text>` | Moves the mouse onto the first character of the first place the selected Script's or Library's text has the text, as a player's mouse rests there, and waits until it has rested long enough for the hover box, the box that tells about the name or error underline under the mouse. The mouse stays there for the steps after it, until the Scenario ends. Fails when the text isn't in the editor, isn't in view in it, or when the mouse doesn't rest on the editor, such as with the completion list over it. See [Mouse](#mouse). |
| `expect-hover <text>` | The hover box shows, and its text, as it draws it, contains the text, matching case: a Block Function's entry, the row of the input or Setting a named argument names, such as `b input`, a name's definition, such as `2: x = 1`, or an error underline's message. Its lines are separated by `\|`, and a row's name and value are one line, with a space between, unless the name is too long for the box's name column. A value that wraps is still one line. Its result lists the box's lines. |
| `expect-no-hover` | The hover box doesn't show, such as with the mouse on a number. It checks the frame the step before ended on, so put it after `hover`, which waits for the box. |
| `press <keys>` | Presses the keys, separated by spaces, in the same frame, such as `press ctrl+space`, `press enter` or `press escape escape`, up to 8. A key is a character, such as `a` or `/`, or one of `space`, `enter`, `tab`, `escape`, `backspace`, `delete`, `up`, `down`, `left`, `right`, `home`, `end`, `pageup` and `pagedown`, with any of `shift+`, `ctrl+`, `alt+` and `cmd+` before it. `shift+` makes a letter upper case and leaves other characters as they are. The keys go to the focused field the way a player's do; see [Keys](#keys). |
| `type <text>` | Types the text into the focused field, one key a frame. Spaces at either end are taken off with the line's: press them with `press space`. |
| `place-block <function> [as <name>]` | Places a block of the Block Function's type in its first style, beside where `place-start` puts the start block, and selects it. A placed-only type such as `lamp` works too. With `as <name>`, the Scenario's later `set-text` steps can write its UID as `{uid:<name>}`. The name takes letters, digits, `-` and `_`, and fails when a block was already placed or named as it. |
| `set-block-setting <name> <setting> <value>` | Changes a Setting of the block `place-block ... as <name>` placed or `name-group-block` named, as changing it in the block's settings panel does, as one undo step. The Setting and its value are written as a Script writes them, such as `set-block-setting t mode global_velocity`, `invert true`, `player 2` or `ticks 5`. Fails for a name no block was placed or named as, a Setting the block's type doesn't have, a value the Setting doesn't take, a Toggler's `targets`, or when the block doesn't keep the value. |
| `name-group-block <script> <function> as <name>` | Names a block of the Block Function's type in the Script's Block Group, the blocks its last Compile placed, the first by UID when there are several, so the same one each time. The Scenario's later steps then use it as one `place-block ... as <name>` placed: `set-text` can write its UID as `{uid:<name>}`, so another Script can write `placed("{uid:gate}")` to it, and `set-block-setting` takes the name. The name takes letters, digits, `-` and `_`. Fails for no Script by that name, when its Block Group has no block of that type, or when a block was already placed or named as it. |
| `turn-off-style <function> [<style>]` | Turns a style of the Block Function's type off, as the startup check does when it doesn't match this game version, the type's first style when the style is left out. A placed-only type such as `lamp` works too. Compile and the window's check then report a Script that places one and a `placed(...)` to one, and the center's startup warnings list it. The window checks a Script a moment after an edit, so turn the style off before the `set-text` whose check should see it. The run turns it back on before the next Scenario. |
| `insert-selected` | Writes a Placed Block Reference for each selected block at the caret, as Insert selected does. Fails when it writes none. |
| `undo` | Presses the editor's undo key. Fails when there was nothing to undo. |
| `redo` | Presses the editor's redo key. Fails when there was nothing to redo. |
| `place-start` | Places a start block on the ground below the camera, facing the way it looks, as one undo step. A level needs one for test mode. |
| `test-mode` | Starts test mode, as the editor's Test key does, and waits until the round starts and the soapbox can drive. Fails without a start block. |
| `back-to-editor` | Quits test mode from the pause menu, and waits until the editor has loaded the level again. |
| `wait <seconds>` | Waits, such as for the level to run a little before a screenshot. |
| `screenshot <name>` | Saves a screenshot as `<scenario>-<name>.png`. The name takes letters, digits, `-` and `_`. |
| `expect-no-exceptions` | The game logged no exception since the Scenario started, or since the last `expect-no-exceptions`. It fails with the ones it logged. |
| `log <text>` | Writes the text to the game's log. |

A run starts without the player's Libraries and Drafts, and deletes the Libraries before each Scenario after the first, so a Scenario sees only the Libraries it makes itself.

Before each Scenario after the first, the run also puts back the window's size, position and shown tab, the libraries.json warning `library-read-failed` turns on and the warnings `startup-warning` turns on, as they were when the run started, and turns back on the styles `turn-off-style` turned off, also after a Scenario that failed partway. So a Scenario that changes them doesn't need to put them back. Each Scenario also gives Imui the player's mouse again as it ends, from where `hover` left it. The size and position are the ones the window was first drawn at, by the first Scenario that opened it: before one has, the window still opens at them. The window takes them when it next opens. Everything else about the window, such as whether it's open and its panels, stays from one Scenario to the next, the way it does for a player. `expect-window-size`, `expect-window-centered`, `expect-window-start-rect`, `expect-tab`, `expect-library-read-failed` and `expect-startup-warning` check what they are.

The window steps need the window open, and the steps for the editor's text need a Script or a Library selected: they fail and say which step to add first. `compile` and `insert-selected` need a Script. `press` and `type` need a field focused, such as by `caret-at-end`.

### Keys

`press` and `type` hand each key to Imui the way Unity hands it a player's: a key event, then the character it types as an event of its own. Every field, the Script editor's completion list and call help among them, handles them the next frame as typed. A character is the one a US keyboard types, and `type` sends a character without a key of its own here, such as `(`, as its character alone.

Unity's own `Input` never sees these keys, so whatever reads it doesn't either. The Script editor reads Escape there before it handles Escape's key event, so the steps hold an Escape for it the same way. Unity reports one Escape a frame however often it was pressed, and the Script editor handles every Escape event of the frame after. So the Escapes in one `press` come in one frame, as a quick double press does. Unity's other Escape path isn't covered: a name prompt that reads Escape from `Input`, and the level editor's own pause key, never see one.

### Mouse

`drag-window-edge`, `drag-window-edge-sideways` and `hover` hand Imui a mouse the way Unity hands it a player's: Imui finds it at the step's position instead of the player's. The drag steps also give it the press, moves and release as events, one a frame. Imui finds the control under the mouse a frame after the mouse moves onto it, so the press comes the frame after the mouse reaches the handle. A drag step drags past what a player's screen would let the mouse reach, when asked to.

Unity's own `Input` never sees this mouse, so the level editor doesn't either: the window blocks the editor's mouse input while Imui's mouse is over it, the same as for a player's, but the editor never gets a click or drag to block. Once a drag step ends, also when it fails, Imui has the player's mouse again and its resize handle has let go. `hover` leaves the mouse where it put it, so the steps after it can check the hover box, until the next `hover` or drag step moves it or the Scenario ends: the run gives Imui the player's mouse again after each Scenario, also one that failed partway.

The hover box waits for the mouse to rest, and starts waiting again when the mouse moves or a key or button is pressed. It reads the keys from Unity's `Input`, which never sees the ones `press` and `type` send, so a key a step sends doesn't hide it. The completion list and the call help, which show while the field is focused, can cover the text the mouse is to rest on.

### Adding a step

1. Add a class to `Zeepkist.LogicScripting.InGameTests/Steps/` that derives from `ScenarioStep`. Its `Run` is a coroutine: it can wait for frames, and it fails the step with `context.Fail(...)`. A check also records what it compared, with `context.Pass(...)` or `context.Fail(detail, expected, actual)`.
2. Add it to `StepList`, by the name it's written with, reading what follows the name with `StepArgument`.
3. Add it to the table above and to `StepListTests`, which checks that the two lists match.

To do what a button in the Script window does, add a method to `ScriptWindow.Driver` in the mod. It calls the same code the button does, so the step tests the real thing. A button in a list tab's footer is on `ScriptWindow.Driver.ShownList`, the shown tab's list, whose prompts take a button pressed for the player the next time they're drawn, and whose list takes a click on an entry the same way.

## What a run changes, and how it puts it back

The script undoes all of this when it ends, also after a failure, a timeout or Ctrl+C:

- **Other copies of the mod** anywhere under `BepInEx/plugins` (mod.io, an old manual install) move to `BepInEx/zls-ingame-test-disabled/`, so the game loads only this build. They move back afterwards. The test plugin fails the run when the game loaded the mod from anywhere else.
- **This build** goes into `BepInEx/plugins/Sideloaded/Plugins/Zeepkist.LogicScripting/`, and the test plugin, the Scenarios and a `run.json` into `.../Zeepkist.LogicScripting.InGameTests/`. Both are removed afterwards, except the build with `--keep`. The run records the build's file hashes next to it, and removes only the files that still match them, so a different copy of the mod installed over a kept build is set aside and put back like any other. A file that's the same as the build's is removed with it.
- **The mod's data** (Drafts, Libraries, the window's layout) and its BepInEx config are copied into `BepInEx/zls-ingame-test-backup/` first, and put back once the game has quit. The backup then moves to the run folder, as `backup/`. The game starts without the mod's data, so a run sees none of the player's Drafts or Libraries and the window opens as it does the first time. Meanwhile the player's data waits in the game's `AppData/Roaming/Zeepkist/zls-ingame-test-mod-data/`.
- **The game's level saves** in its `Autosaves` and `Backups` folders, which test mode and going back to the main menu add to, are copied the same way and put back.

The test plugin only does anything when the game starts with the script's command-line flag and finds a `run.json` next to it, so a copy left behind by a crash can't take over normal play. If a run was killed outright, the next run first puts back what the last one left. It removes the build, which the killed run marked as its own before installing it. A file that no longer matches the build stays, as the player's. It then moves the other copies of the mod back. It puts back the mod's data and config and the level saves from the killed run's backup, which then moves to this run's folder as `earlier-backup/`.
