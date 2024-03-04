using System;
using UnityEngine;
using VContainer;
using UnityEngine.UI;

public sealed class FirstSceneDI : MonoBehaviour
{
    public Text DisplayArea;
    private  IGacha _gacha;
    private void Start()
    {
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Register<IGacha, Gacha>(Lifetime.Singleton);
        IObjectResolver objectResolverGacha = containerBuilder.Build();
        _gacha = objectResolverGacha.Resolve<IGacha>();
    }

    //次のシーンに遷移する
    public void OnclickNextScene()
    {
        //シーン遷移する
        UnityEngine.SceneManagement.SceneManager.LoadScene("SecondScene");
    }

    public void OnDestroy()
    {
        _gacha.Dispose();
    }

    public void OnClickGacha()
    {
        DisplayArea.text = _gacha.DrawGacha();
    }
}