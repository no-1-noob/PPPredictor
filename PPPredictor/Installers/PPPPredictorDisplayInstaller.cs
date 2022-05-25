using PPPredictor.Events;
using PPPredictor.UI.ViewController;
using Zenject;

namespace PPPredictor
{
    internal class PPPPredictorDisplayInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<PPPredictorViewController>().AsSingle();
            Container.BindInterfacesTo<PPPredictorEventsMgr>().AsSingle();
        }
    }
}
