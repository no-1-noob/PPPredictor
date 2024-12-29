using PPPredictor.Utilities;
using Zenject;

namespace PPPredictor.Installers
{
    class CoreInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PPPredictorMgr>().AsSingle();
        }
    }
}
