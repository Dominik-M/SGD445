using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Munition", menuName = "Scriptable Objects/Munition")]
public class Munition : ScriptableObject
{
    public enum MunitionsTyp { STANDARD, SCHROT, LASER, RAKETEN, COUNT }
    [Header("Munitions Typ")]
    [SerializeField] private MunitionsTyp type;
    [Header("Maximal getragene Munition")]
    [SerializeField] private int maxAmmo;

    public MunitionsTyp Type => type;
    public int MaxAmmo => maxAmmo;


#if UNITY_EDITOR


    [MenuItem("Tools/Munition Assets Erstellen")]
    public static void CreateAssets()
    {
        for (int i = 0; i < (int)MunitionsTyp.COUNT; i++)
        {
            Munition asset = ScriptableObject.CreateInstance<Munition>();
            asset.name = ((MunitionsTyp)i).ToString();
            asset.type = ((MunitionsTyp)i);
            Debug.Log("Erstelle Asset: " + asset.name);
            AssetDatabase.CreateAsset(
                asset,
                "Assets/Resources/Munition/" + asset.name + ".asset"
            );
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

#endif
}
