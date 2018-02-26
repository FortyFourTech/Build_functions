using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class BuildFunctions {
    private readonly static string k_CORRECTION_DEFINE_SYMBOL = "CORRECTION";
    private readonly static string k_SERVICE_PRODUCT_NAME = "iHome-service";
    private readonly static string k_GAME_PRODUCT_NAME = "iHome";
    private readonly static string k_SERVICE_APP_BUNDLE = "com.dimar.ihomeservice";
    private readonly static string k_GAME_APP_BUNDLE = "com.dimar.ihome";

#region MENU ITEMS
    [MenuItem("Builds/Full build %b",false,0)]
    public static void BuildAll() {
        BuildGame();
        BuildService();
    }

    [MenuItem("Builds/Build Game app",false,11)]
    public static void BuildGame() {
        BuildGame(false);
    }
    [MenuItem("Builds/Run Game app &b",false,11)]
    public static void BuildNPlayGame() {
        BuildGame(true);
    }

    [MenuItem("Builds/Build Service app",false,22)]
    public static void BuildService() {
        BuildService(false);
    }
    [MenuItem("Builds/Run Service app &#b",false,22)]
    public static void BuildNPlayService() {
        BuildService(true);
    }
#endregion

#region BUILD FUNCTIONS
    static void BuildGame(bool launch) {
        // change player settings before build
        var symbolsChanged = setCorrectionSymbol(false);
        var nameChanged = setProductName(false);
        var bundleChanged = setBundleName(false);

        // build
        MakeBuild("Builds/playBuild.apk", launch);

        // return player settings after build
        if (symbolsChanged)
            setCorrectionSymbol(true);
        if (nameChanged)
            setProductName(true);
        if (bundleChanged)
            setBundleName(true);
    }

    static void BuildService(bool launch) {
        // change player settings before build
        var symbolsChanged = setCorrectionSymbol(true);
        var nameChanged = setProductName(true);
        var bundleChanged = setBundleName(true);

        // build
        MakeBuild("Builds/serviceBuild.apk", launch);

        // return player settings after build
        if (symbolsChanged)
            setCorrectionSymbol(false);
        if (nameChanged)
            setProductName(false);
        if (bundleChanged)
            setBundleName(false);
    }

    static void MakeBuild(string savePath, bool playThen) {
        var options = new BuildPlayerOptions {
            scenes = EditorBuildSettings.scenes
                .Where(x => x.enabled)
                .Select(x => x.path).ToArray(),
            locationPathName = savePath,
            target = BuildTarget.Android,
            options = playThen
                ? BuildOptions.AutoRunPlayer
                : BuildOptions.None
        };
        BuildPipeline.BuildPlayer(options);
    }
#endregion

#region CHANGE SETTINGS FUNCTIONS
    static bool setCorrectionSymbol(bool serviceApp) {
        var currentSymbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        var currentSymbols = new List<string>(currentSymbolsString.Split(';'));

        var symbolContained = currentSymbols.Contains(k_CORRECTION_DEFINE_SYMBOL);
        var stateChanged = false;
        if (!symbolContained && serviceApp) {
            currentSymbols.Add(k_CORRECTION_DEFINE_SYMBOL);
            stateChanged = true;
        } else if (symbolContained && !serviceApp) {
            currentSymbols.Remove(k_CORRECTION_DEFINE_SYMBOL);
            stateChanged = true;
        }

        if (stateChanged)
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", currentSymbols.ToArray())
            );

        return stateChanged;
    }

    static bool setProductName(bool serviceApp) {
        var currentAppName = PlayerSettings.productName;
        var nameToSet = PlayerSettings.productName;

        var nameIsService = currentAppName == k_SERVICE_PRODUCT_NAME;
        var nameChanged = false;
        if (!nameIsService && serviceApp) {
            nameToSet = k_SERVICE_PRODUCT_NAME;
            nameChanged = true;
        } else if (nameIsService && !serviceApp) {
            nameToSet = k_GAME_PRODUCT_NAME;
            nameChanged = true;
        }

        if (nameChanged)
            PlayerSettings.productName = nameToSet;

        return nameChanged;
    }

    static bool setBundleName(bool serviceApp) {
        var currentBundle = PlayerSettings.applicationIdentifier;
        var bundleToSet = PlayerSettings.applicationIdentifier;

        var nameIsService = currentBundle == k_SERVICE_APP_BUNDLE;
        var bundleChanged = false;
        if (!nameIsService && serviceApp) {
            bundleToSet = k_SERVICE_APP_BUNDLE;
            bundleChanged = true;
        } else if (nameIsService && !serviceApp) {
            bundleToSet = k_GAME_APP_BUNDLE;
            bundleChanged = true;
        }

        if (bundleChanged)
            PlayerSettings.applicationIdentifier = bundleToSet;

        return bundleChanged;
    }
#endregion
}
