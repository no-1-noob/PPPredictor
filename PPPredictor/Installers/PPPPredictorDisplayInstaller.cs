using Zenject;
using PPPredictor.UI.ViewController;

namespace PPPredictor
{
    internal class PPPPredictorDisplayInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<PPPredictorViewController>().AsSingle();
        }
    }
}
