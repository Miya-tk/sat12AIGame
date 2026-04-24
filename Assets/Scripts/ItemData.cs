using UnityEngine;

// 右クリックメニューからこのデータファイルを作れるようにする
[CreateAssetMenu(fileName = "NewItem", menuName = "ClickerGame/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;   // アイテムの名前
    public int baseCost;      // 基本価格
    public int scorePerClick; // 1クリックあたりの増加量
}