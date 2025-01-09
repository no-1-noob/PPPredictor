using BeatSaberMarkupLanguage;
using HMUI;
using PPPredictor.UI.ViewController;
using PPPredictor.Utilities;

namespace PPPredictor.UI
{
    class PPPredictorFlowCoordinator : FlowCoordinator
    {
        private static PPPredictorFlowCoordinator instance = null;
        private static SettingsMidViewController settingsMidView;
        public override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            instance = this;
            SetTitle("PPPredictor");
            showBackButton = true;
            settingsMidView = BeatSaberUI.CreateViewController<SettingsMidViewController>();
            ProvideInitialViewControllers(settingsMidView);
        }

        public override void BackButtonWasPressed(HMUI.ViewController topViewController) => Close();

        private void Close()
        {
            Plugin.pppViewController.ApplySettings();
            ProfileInfoMgr.ParentFlow.DismissFlowCoordinator(instance, () => {
                instance = null;
            }, immediately: true);
        }
    }
}
