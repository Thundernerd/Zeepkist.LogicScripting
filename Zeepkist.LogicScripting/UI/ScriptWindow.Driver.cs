using System.Collections.Generic;
using System.Linq;
using Imui.Core;
using TNRD.Zeepkist.LogicScripting.Compiler;
using TNRD.Zeepkist.LogicScripting.Compiler.Editing;
using TNRD.Zeepkist.LogicScripting.Compiler.Manifest;
using TNRD.Zeepkist.LogicScripting.Compiler.Storage;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.UI;

internal sealed partial class ScriptWindow
{
    /// <summary>
    /// Does what the window's buttons and typing do, through the same code paths, for the in-game tests. Nothing in the
    /// mod itself uses it.
    /// </summary>
    internal sealed class Driver
    {
        private readonly ScriptWindow _window;

        public Driver(ScriptWindow window)
        {
            _window = window;
        }

        public bool IsOpen => _window.IsOpen;
        public bool HasScript => _window.CurrentScript != null;

        /// <summary>
        /// Whether a Script or a Library is selected, so there's text in the editor.
        /// </summary>
        public bool HasText => _window._edited != null;

        public IReadOnlyList<string> ScriptNames => [.. _window._scripts.Entries.Select(e => e.Name)];
        public IReadOnlyList<string> LibraryNames => [.. _window.Libraries.Entries.Select(l => l.Name)];

        /// <summary>
        /// Empty until a Compile, and again after an edit or selecting another Script.
        /// </summary>
        public string Result => _window._result;

        /// <summary>
        /// The selected Script's or Library's text in the editor, or null with neither selected.
        /// </summary>
        public string Text => _window._edited?.Editor.Text;

        /// <summary>
        /// Whether the window has checked the current text. It only checks while it's open, a moment after the last edit.
        /// </summary>
        public bool IsChecked => _window._edited?.IsChecked ?? false;

        /// <summary>
        /// The errors the last check found, or null when it couldn't run.
        /// </summary>
        public IReadOnlyList<Diagnostic> Errors => _window._edited?.Errors;

        /// <inheritdoc cref="ScriptEditor.CompletionItems"/>
        public IReadOnlyList<CompletionItem> CompletionItems => _window._edited?.Editor.CompletionItems ?? [];

        /// <inheritdoc cref="ScriptEditor.CallHelp"/>
        public CallHelp CallHelp => _window._edited?.Editor.CallHelp;

        public bool IsFieldFocused => _window.AnyFieldFocused;

        /// <summary>
        /// The shown tab's list and footer.
        /// </summary>
        public IListPanel ShownList => _window._state.ShowLibraries ? _window._libraryListPanel : _window._scriptListPanel;

        /// <summary>
        /// The window's rect as Imui last drew it.
        /// </summary>
        public ImRect WindowRect => new(_window._state.X, _window._state.Y, _window._state.Width, _window._state.Height);

        /// <inheritdoc cref="UI.EditorLayout"/>
        public EditorLayout EditorLayout => _window._editorLayout;

        /// <inheritdoc cref="UI.WindowLayout"/>
        public WindowLayout WindowLayout => _window._windowLayout;

        /// <summary>
        /// Whether the center drew the startup check's warnings.
        /// </summary>
        public WarningLayout StartupWarningLayout => _window._startupWarningLayout;

        /// <summary>
        /// Whether the Libraries tab drew its libraries.json warning, the last frame it was shown.
        /// </summary>
        public WarningLayout LibraryReadFailedLayout => _window._libraryReadFailedLayout;

        /// <summary>
        /// Kept while the Scripts side is collapsed, for when it's shown again.
        /// </summary>
        public bool ShowsLibraries => _window._state.ShowLibraries;

        /// <summary>
        /// Whether a size or position asked for waits for the window to be drawn.
        /// </summary>
        public bool IsRectPending => _window._requestedRect.IsPending;

        public void SetOpen(bool open)
        {
            _window.SetOpen(open);
        }

        /// <summary>
        /// Like New Script on the Scripts tab and naming it in the prompt, which selects it. Returns why the name isn't
        /// taken, or null.
        /// </summary>
        public string NewScript(string name)
        {
            ShowTab(libraries: false);
            var entry = _window._scriptListPanel.New(name, out var error);
            if (entry != null)
                _window.Select(entry);

            return error;
        }

        /// <summary>
        /// Like clicking the Script in the list on the Scripts tab, which selects it the next time the list is drawn:
        /// see <see cref="IListPanel.IsInputPending"/>. Returns whether there's one by that name.
        /// </summary>
        public bool SelectScript(string name)
        {
            if (!_window._scriptListPanel.Click(name))
                return false;

            ShowTab(libraries: false);
            return true;
        }

        /// <summary>
        /// Like Select group in the Script's context menu on the Scripts tab. Returns whether there's one by that name.
        /// </summary>
        public bool SelectGroup(string name)
        {
            var entry = FindScript(name);
            if (entry == null)
                return false;

            ShowTab(libraries: false);
            _window._scriptListPanel.SelectGroup(entry);
            return true;
        }

        /// <summary>
        /// Like dragging the window's edge until it's this tall, which it is from the next frame.
        /// </summary>
        public void SetWindowHeight(float height)
        {
            _window._requestedRect.SetHeight(height);
        }

        /// <summary>
        /// Like dragging the window's resize handle until it's this big, which it is from the next frame.
        /// </summary>
        public void SetWindowSize(float width, float height)
        {
            _window._requestedRect.SetWidth(width);
            _window._requestedRect.SetHeight(height);
        }

        /// <summary>
        /// Moves the window to the middle of the screen, at the size it has then, from the next frame.
        /// </summary>
        public void CenterWindow()
        {
            _window._requestedRect.MoveToCenter();
        }

        /// <summary>
        /// Makes the Libraries tab show, or stop showing, the warning it shows when libraries.json couldn't be read.
        /// Libraries still save.
        /// </summary>
        public void ShowLibraryReadFailed(bool show)
        {
            _window._showLibraryReadFailed = show;
        }

        /// <summary>
        /// Makes the center show, or stop showing, the startup check's warnings, as on a game version the mod doesn't
        /// fully support. Compile and the checks work as before.
        /// </summary>
        public void ShowStartupWarning(bool show)
        {
            _window._showStartupWarning = show;
        }

        /// <summary>
        /// Turns a style off as the startup check does when it doesn't match this game version, so Compile and the checks
        /// report a Script that places one or refers to one. Returns false when it's already off.
        /// </summary>
        public bool TurnOffStyle(BlockType type, BlockStyle style)
        {
            return _window._startupCheck.TurnOff(type, style);
        }

        /// <summary>
        /// The Script's ID, or null when there's none by that name.
        /// </summary>
        public ScriptId FindScriptId(string name)
        {
            return FindScript(name)?.Id;
        }

        /// <summary>
        /// Like New Library on the Libraries tab and naming it in the prompt, which selects it. Returns why the name
        /// isn't taken, or null.
        /// </summary>
        public string NewLibrary(string name)
        {
            ShowTab(libraries: true);
            var library = _window._libraryListPanel.New(name, out var error);
            if (library != null)
                _window.SelectLibrary(library);

            return error;
        }

        /// <summary>
        /// Like clicking the Library in the list on the Libraries tab, which selects it the next time the list is drawn:
        /// see <see cref="IListPanel.IsInputPending"/>. Returns whether there's one by that name.
        /// </summary>
        public bool SelectLibrary(string name)
        {
            if (!_window._libraryListPanel.Click(name))
                return false;

            ShowTab(libraries: true);
            return true;
        }

        /// <summary>
        /// Like Delete on every Library on the Libraries tab, without the prompts. A prompt left open is closed too, as
        /// it could be for a Library that's gone.
        /// </summary>
        public void DeleteLibraries()
        {
            _window._libraryListPanel.Clear();
            foreach (var library in _window.Libraries.Entries.ToList())
            {
                _window.Libraries.Remove(library.Id);
                _window.OnLibraryDeleted(library);
            }
        }

        /// <summary>
        /// What a Scenario can change that the window keeps into the next one: its size and position, the shown tab and
        /// the warnings the tests turn on. A window that was never drawn has no rect, for the one it first opens at. The
        /// styles the tests turn off aren't in it, as none are at the start of a run.
        /// </summary>
        public KeptState RememberKeptState()
        {
            return new KeptState(
                _window._state.Width > 0 && _window._state.Height > 0 ? WindowRect : null,
                _window._state.ShowLibraries,
                _window._showLibraryReadFailed,
                _window._showStartupWarning);
        }

        /// <summary>
        /// Puts back what <see cref="RememberKeptState"/> remembered, and turns back on the styles
        /// <see cref="TurnOffStyle"/> turned off. A closed window takes the size and position when it next opens. With no
        /// rect remembered, it gets the size it first opens at, in the middle of the screen, as it first opens.
        /// </summary>
        public void PutBackKeptState(KeptState state)
        {
            _window._startupCheck.TurnTestStylesBackOn();
            if (state.WindowRect is { } rect)
            {
                SetWindowSize(rect.W, rect.H);
                _window._requestedRect.MoveTo(new Vector2(rect.X, rect.Top));
            }
            else
            {
                SetWindowSize(DefaultWidth, DefaultHeight);
                CenterWindow();
            }

            ShowTab(state.ShowLibraries);
            ShowLibraryReadFailed(state.ShowLibraryReadFailed);
            ShowStartupWarning(state.ShowStartupWarning);
        }

        /// <summary>
        /// Like clicking the Scripts or the Libraries tab, whose list the buttons for one are in.
        /// </summary>
        public void ShowTab(bool libraries)
        {
            _window._state.ShowLibraries = libraries;
        }

        /// <summary>
        /// Null when there's none by that name.
        /// </summary>
        private ScriptEntry FindScript(string name)
        {
            return _window._scripts.Entries.FirstOrDefault(e => e.Name == name);
        }

        /// <summary>
        /// Replaces the selected Script's or Library's text as one edit, as typing it would leave it.
        /// </summary>
        public void SetText(string text)
        {
            _window._edited.Editor.Replace(text, text.Length, Time.unscaledTimeAsDouble);
            _window.OnTyped();
        }

        /// <summary>
        /// Like clicking at a character index of the selected Script's or Library's text, which focuses the field.
        /// </summary>
        public void PlaceCaret(int index)
        {
            _window._edited.Editor.JumpTo(index);
        }

        /// <inheritdoc cref="ScriptEditor.RequestCompletion"/>
        public void RequestCompletion()
        {
            _window._edited.Editor.RequestCompletion();
        }

        /// <inheritdoc cref="ScriptEditor.IsCompletionRequestPending"/>
        public bool IsCompletionRequestPending => _window._edited?.Editor.IsCompletionRequestPending ?? false;

        /// <inheritdoc cref="ScriptEditor.HoldSentEscape"/>
        public void HoldSentEscape()
        {
            _window._edited?.Editor.HoldSentEscape();
        }

        /// <summary>
        /// Like Insert selected. Returns whether it changed the text.
        /// </summary>
        public bool InsertSelected(LEV_LevelEditorCentral central)
        {
            var edited = _window._placedBlocksPanel.InsertSelected(central, _window._edited.Editor, Time.unscaledTimeAsDouble);
            if (edited)
                _window.OnEdited();

            return edited;
        }

        /// <summary>
        /// Like Ctrl+Enter in the editor. Returns the result line, which says why when Compile can't run, or null when
        /// the check it runs first on unchecked text found errors, which the error list shows.
        /// </summary>
        public string Compile()
        {
            var result = _window._edited.CompileFromKeyboard();
            _window.ShowResult(result);
            return result;
        }
    }
}
