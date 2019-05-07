using System.Collections.Generic;
using Commands.Startup;
using Data;
using Data.DataBase;
using Zenject;

namespace DI
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AppStarter>().AsSingle();
            Container.BindInterfacesAndSelfTo<Repository>().AsSingle();
            Container.BindInterfacesAndSelfTo<TestClass>().AsSingle();
            Container.BindInterfacesAndSelfTo<InitDataCommand>().AsTransient();
            Container.Bind<IDataBaseProxy>().To<FireBaseDbProxy>().AsSingle();

            Container.DeclareSignal<TestSignal>();
        }
    }

    public class TestSignal 
    {
    }
}