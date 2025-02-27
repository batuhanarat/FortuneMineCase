using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServiceProvider
{
    private static readonly Dictionary<Type, IProvidable> _registerDictionary = new();

    public static AssetLibrary AssetLibrary => GetManager<AssetLibrary>();
    public static WalletManager WalletManager => GetManager<WalletManager>();
    public static UIManager UIManager => GetManager<UIManager>();
    public static SceneLoaderManager ScenesManager => GetManager<SceneLoaderManager>();


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeServiceProvider()
    {
        InitializeCoreServices();
    }

    private static void InitializeCoreServices()
    {

    }
    private static void RegisterSceneEvents()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    private static T GetManager<T>() where T : class, IProvidable
    {
        if (_registerDictionary.ContainsKey(typeof(T)))
        {
            return (T)_registerDictionary[typeof(T)];
        }

        return null;
    }

    public static T Register<T>(T target) where T: class, IProvidable
    {
        _registerDictionary[typeof(T)] = target;
        return target;
    }
}