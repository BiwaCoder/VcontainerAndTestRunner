using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using VContainer;

public class EditorModeTest
{
    [Test]
    public void GachaTest()
    {
        var builder = new ContainerBuilder();
        
        builder.Register<IGacha, GachaDummy>(Lifetime.Singleton);
        IObjectResolver _container = builder.Build();
        IGacha _gacha = _container.Resolve<IGacha>();
        var gachaResult = _gacha.DrawGacha();
        Assert.That(gachaResult, Is.EqualTo("ダミーアイテム1"));
    }

    [Test]
    public void DamageTest()
    {
        var builder = new ContainerBuilder();
        builder.Register<IUnit,UnitImpl>(Lifetime.Transient);
        builder.Register<ICalcDamage,CalcDamageStory>(Lifetime.Singleton);
        IObjectResolver _container = builder.Build();
        var _player = _container.Resolve<IUnit>();
        var _enemy = _container.Resolve<IUnit>();
        _enemy = _player.AddDamageToUnit(_enemy);
        
        Assert.That(_enemy.Hp, Is.EqualTo(90));
    }
    
    
}

