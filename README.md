# VcontainerとTestRunnerの簡単なサンプル

このプロジェクトはVContainerとTestRunnerを学ぶための簡単なサンプルです

https://github.com/hadashiA/VContainer



# Vcontainerとは

VContainerとはDIコンテナです。
DIコンテナのDIとは何かというとDependency Injection、依存性の注入と訳されています。

なんだかわかりにくいですね。やっていることは、インタフェースにインスタンスを代入しているので、

「仕様（インタフェース）に基づきゲームを作る」

といったイメージをもつと想像しやすいのではないでしょうか？

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

作るbuilderに登録Registerしています。<IGacha, Gacha>でインタフェースと実態を教えてあげます。

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
この
- テストを容易にできる
- 簡単実装を切り替えられる
  
という点がDIコンテナを使う大きなメリットです。
今回のサンプルは
FirstSceneと、SecondSceneという２つのシーンを作りました。
２つのシーンに各々、FirstSceneDI、SecondSceneDIというスクリプトを作り、こちらでDIを実施しています。

自分も最初にVContainerを学ぶときは、すべて最初に解決するかと思いましたが、画面ごとなど単位に分けてDIを実施し、また画面が破棄されるタイミングで
DIしたものをまとめて破棄できたりするもので、このように生存期間(スコープ）の管理を一元化できるのも、DIのポイントになります。

# Test Runner
次に、UnityのテストツールであるTest runnerについて説明します。

Test runnerは以下のようなコードをPlayせずとも動作させ確認できます。(Playmodeでもテストする方法もあります）


```
public class EditorModeTest
{
    [Test]
    public void GachaDITest()
    {
        var builder = new ContainerBuilder();
        
        builder.Register<IGacha, GachaDummy>(Lifetime.Singleton);
        IObjectResolver _container = builder.Build();
        IGacha _gacha = _container.Resolve<IGacha>();
        var gachaResult = _gacha.DrawGacha();
        Assert.That(gachaResult, Is.EqualTo("ダミーアイテム1"));
    }

    [Test]
    public void GachaTest()
    {
        IGacha _gacha = new GachaDummy();
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
```

このような感じで、意図した値になるかを確認するように実装します。

Assert.That(gachaResult, Is.EqualTo("ダミーアイテム1"));

Assert.That(_enemy.Hp, Is.EqualTo(90));

VContainerを使っていれば、インスタンスの構築もPlaymode同様にでき、テストを容易に行うことができるでしょう。
詰まるのは、Assert.Thatでのテストコードを書く所と、Assembly Definitionを使う所でしょうか。

Assert.Thatのコードは、今ならChatGPT(Github Copilot)に依頼するのがいいのではないでしょうか？
何をテストするべきかを人間が考えて、コード自体はAIが生成するのが快適なテストライフを実現できるかと思います。

Test RunnerはAssembly Definitionを仕様するため、参照が解決できなかったり、循環参照でエラーになってしまうことがあります。
以下のサイトが参考になるかと思います。
https://papacoder.net/unity-test-runner-assembly-definition/

また、Assembly Definitionを正しく理解することで、問題発生時に解決できるようになりますので、時間をとって、まずはAssembly Definitionを学ぶことをおすすめします。
https://qiita.com/toRisouP/items/d206af3029c7d80326ed


# その他参考資料

Vcontainerを学ぶにはこのサイトが丁寧でおすすめです

https://qiita.com/sakano/items/b91e01f7fc0a946090ac

