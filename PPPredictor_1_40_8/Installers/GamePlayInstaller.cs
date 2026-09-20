using PPPredictor.Utilities;
using Zenject;

namespace PPPredictor.Installers
{
    internal class GamePlayInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GamePlayMgr>().AsSingle();
        }
    }
}
