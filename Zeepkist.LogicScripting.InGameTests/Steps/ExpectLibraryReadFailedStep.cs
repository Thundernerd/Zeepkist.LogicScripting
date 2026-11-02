using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the Libraries tab draws its warning that libraries.json couldn't be read, or doesn't, as expected.
/// </summary>
internal sealed class ExpectLibraryReadFailedStep : ScenarioStep
{
    public ExpectLibraryReadFailedStep(bool shown)
    {
        Shown = shown;
    }

    public bool Shown { get; }

    public override IEnumerator Run(StepContext context)
    {
        return WarningCheck.Run(
            context,
            Shown,
            "the libraries.json warning",
            w => w.LibraryReadFailedLayout,
            "the Libraries tab wasn't drawn: add show-tab libraries first, with the Scripts side shown");
    }
}
