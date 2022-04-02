using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;
using PPPredictor;

namespace PPPredictor.UI
{
    internal class PPPredictorFlowCoordinator : FlowCoordinator, IInitializable, IDisposable
    {
            private FlowCoordinator parentFlowCoordinator;
            private PPPredictorViewController ppPredictorController;
            private MainFlowCoordinator mainFlowCoordinator;

            [Inject]
            public void Construct(PPPredictorViewController ppPredictorController)
            {
                this.ppPredictorController = ppPredictorController;
                Plugin.Log?.Info("PPPredictorFlowCoordinator Construct");
        }

            public void Initialize()
            {

            }

            public void Dispose()
            {
            }

            protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
            {
                //ProvideInitialViewControllers(ppPredictorController);
            }
    }
}
