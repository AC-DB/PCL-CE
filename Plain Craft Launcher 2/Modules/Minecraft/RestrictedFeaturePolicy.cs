using PCL.Core.App.Localization;
using PCL.Core.UI.Hints;
using PCL.Core.App.Region;

namespace PCL;

internal static class RestrictedFeaturePolicy
{
    // 关闭区域限制
    private static readonly bool EnforcementEnabled = false;
    
    internal static RegionAssessment Assess() => RegionPolicy.Evaluate(RegionPolicy.ReadWindowsSignals());

    internal static bool IsRestricted => EnforcementEnabled && Assess().IsRestricted;

    internal static bool Check(string feature, bool showHint = true)
    {
        if (!EnforcementEnabled) return true;
        
        var assessment = Assess();
        if (!assessment.IsRestricted) return true;

        Common.Log($"[RegionPolicy] 已阻止功能 {feature}，判定原因：{assessment.Reason}");
        if (showHint)
        {
            HintService.Show(Hints.Error(Lang.Text(assessment.Classification == RegionClassification.Unknown
                ? "Region.Feature.Unknown"
                : "Region.Feature.Restricted")));
        }
        return false;
    }

    internal static void ThrowIfRestricted(string feature)
    {
        if (Check(feature, false)) return;
        throw new Common.CancelledException();
    }
}
