using BitFramework.Core;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProviderNodeManager : MonoBehaviour, IServiceProvider
{
    [SerializeField] private Transform worldUITrans;
    [SerializeField] private Transform roleLayerTrans;
    [SerializeField] private Transform mapLayerTrans;
    [SerializeField] private Tile pathTile;
    [SerializeField] private Canvas canvasLayer;

    public void Init()
    {
        App.Make<NodeManager>().Inject(worldUITrans, roleLayerTrans, mapLayerTrans, pathTile, canvasLayer);
    }

    public void Register()
    {
        App.Singleton<NodeManager>();
    }
}