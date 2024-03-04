using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

using UnityEngine.UI;

public class SecondSceneDI : MonoBehaviour
{
    private IUnit _player;
    private IUnit _enemy;
    
    public Text DisplayArea;
    
    // Start is called before the first frame update
    void Start()
    {
        var containerBuilder = new ContainerBuilder();
        
        containerBuilder.Register<IUnit,UnitImpl>(Lifetime.Transient);
        containerBuilder.Register<ICalcDamage,CalcDamageStory>(Lifetime.Singleton);
        
        IObjectResolver objectResolver = containerBuilder.Build();
        _player = objectResolver.Resolve<IUnit>();
        _enemy = objectResolver.Resolve<IUnit>();

    }

    public void OnAttackDamage()
    {  
        _enemy = _player.AddDamageToUnit(_enemy);
        DisplayArea.text = "enemyHp:" + _enemy.Hp;
    }

 
}



