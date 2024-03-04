# VcontainerとTestRunnerの簡単なサンプル

このプロジェクトはvContainerとTestRunnerを学ぶための簡単なサンプルです

https://github.com/hadashiA/VContainer



# Vcontainerとは

vContainerとはDIコンテナです。
DIコンテナのDIとは何かというとDependency Injection、依存性の注入と訳されていますが、なんだかわかりにくいですね。
やっていることは、インタフェースにインスタンスを代入しているので、

「仕様（インタフェース）に基づきゲームを作る」

といったイメージをもつとイメージしやすいのではないでしょうか？

実際の具体例を説明していきます。
ソーシャルゲームでは必須のガチャ、まずインタフェースを考えるとこのようになります。
```
public interface IGacha : IDisposable
{
    public string DrawGacha();
}
```

実装はこのように作りました。

```
public class Gacha : IGacha
{
    //ガチャのテーブル、1-10にあわせて　アイテムを設定するDictionary
    private Dictionary<int, string> gachaTable = new Dictionary<int, string>()
    {
        {1, "Rアイテム1"},
        {2, "Rアイテム2"},
        {3, "$アイテム3"},
        {4, "Rアイテム4"},
        {5, "Rアイテム5"},
        {6, "R アイテム6"},
        {7, "SRアイテム7"},
        {8, "SR アイテム8"},
        {9, "SR アイテム9"},
        {10, "SSR アイテム10"}
    };
    
    //ガチャを引いて文字列を返す
    public string DrawGacha()
    {
        int gachaNumber = Random.Range(1, 11);
        return gachaTable[gachaNumber];
    }
    
    public void Dispose()
    {
        Debug.Log("Dispose");
    }
}
```

通常の実装方法だとこのようになります。

```
 private void Start()
    {
        IGacha _gacha = new GachaDummy();
        var gachaResult = _gacha.DrawGacha();
    }
```

DIをするとこんな感じです。

```
 private void Start()
    {
        var builder = new ContainerBuilder();
        builder.Register<IGacha, Gacha>(Lifetime.Singleton);
        IObjectResolver _container = builder.Build();
        IGacha _gacha = _container.Resolve<IGacha>();
        var gachaResult = _gacha.DrawGacha();
    }
```

ContainerBuilderという構築する入れ物を作り、Registerにより何をどう作るか登録します。

こちらが登録作業、作るbuilderに登録Registerしています。<IGacha, Gacha>でインタフェースと実態を教えてあげます。

builder.Register<IGacha, Gacha>(Lifetime.Singleton);

Buildすると解決屋さんであるResolverが生まれます。今回は1インタフェース1クラスでシンプルですが、複数のインタフェースやクラスから

仕組みを作れるのがDIのメリットです

IObjectResolver _container = builder.Build();

最後は解決、これでインスタンスを取り出せます。

IGacha _gacha = _container.Resolve<IGacha>();

仕組みとしシンプルなものはこのようになりますが、実際は次のコードのように複数のインタフェースやクラスを登録し、容易に変更したり、生存期間の管理をまとめてできることがメリットになります。

```

public interface ICalcDamage 
{
    public int CalcDamage(int attack, int defence);
}


public class CalcDamageStory : ICalcDamage
{
    public int CalcDamage(int attack, int defence)
    {
        int ret = attack - defence;
        return ret;
    }
    
}

public class CalcDamagePvP
{
    public int CalcDamage(int attack, int defence)
    {
        int ret = attack*2 - defence;
        return ret;
    }
}


public interface IUnit
{
    public IUnit AddDamageToUnit(IUnit enemyUnit);
    
    //プロパティ
    public int Hp { get; set; }
    public int Atk { get; set; }
    public int Def { get; set; }
    
}


public class UnitImpl : IUnit
{
    //ステータス
    public int Hp { get; set; }
    public int Atk { get; set; }
    public int Def { get; set; }
    
    private ICalcDamage _calcDamage;
    
    [Inject]
    public UnitImpl(ICalcDamage calcDamage)
    {
        _calcDamage = calcDamage;
        Hp = 100;
        Atk = 30;
        Def = 20;
    } 
    
    //引数で渡されたクラスにダメージを与える
    public IUnit AddDamageToUnit(IUnit enemyUnit)
    {
        int damage = _calcDamage.CalcDamage(Atk, Def);
        enemyUnit.Hp -= damage;
        return enemyUnit;
    }
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

```

Unityのシーンから起動させる時

```
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
```

テストを実装する時、同じやり方でインタフェースからインスタンスを取得し、また容易に実装を切り替えてテストを行うことができます。

この
- テストを容易にできる
- 簡単実装を切り替えられる
  

```
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
```
という点がDIコンテナを使う大きなメリットです。




https://qiita.com/sakano/items/b91e01f7fc0a946090ac
