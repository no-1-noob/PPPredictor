using PPPredictor.Utilities;
using Zenject;

namespace PPPredictor.Installers
{
    internal class MainMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MainMenuMgr>().AsSingle();
        }
    }
}
