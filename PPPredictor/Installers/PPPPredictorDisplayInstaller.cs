using Zenject;

namespace PPPredictor
{
    internal class PPPPredictorDisplayInstaller : Installer
    {
        public override void InstallBindings()
        {
            //Container.BindInterfacesAndSelfTo<PPPredictorFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            //Container.Bind<PPPredictorViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesTo<PPPredictorViewController>().AsSingle();
            //Container.BindInterfacesTo<PPPredictorViewController>().AsSingle();
            Plugin.Log?.Info("InstallBindings");
        }
    }
}
